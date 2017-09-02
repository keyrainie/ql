using System;
using System.Collections.Generic;
using System.Threading;
using System.IO.IsolatedStorage;
using System.IO;

using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Core.Components;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class DefaultCacher : ICache
    {
        #region ICache Members

        private TimeSpan m_defaultCacheTime = TimeSpan.FromMinutes(30);

        public TimeSpan DefaultCacheTime
        {
            get { return this.m_defaultCacheTime; }
            set { this.m_defaultCacheTime = value; }
        }

        public object this[string key]
        {
            get
            {
                return PersistableCacheHelper.Get(key);
            }
            set
            {
                PersistableCacheHelper.Add(key, value, DefaultCacheTime);
            }
        }

        public void Add(string key, object value)
        {
            PersistableCacheHelper.Add(key, value, DefaultCacheTime);
        }

        public void Add(string key, object value, TimeSpan cacheTime)
        {
            PersistableCacheHelper.Add(key, value, cacheTime);
        }

        public void AddAsync(string key, Action<string, Action<object>> getDataAction)
        {
            PersistableCacheHelper.AddAsyncAction(key, DefaultCacheTime, getDataAction);
        }

        public void AddAsync(string key, TimeSpan cacheTime, Action<string, Action<object>> getDataAction)
        {
            PersistableCacheHelper.AddAsyncAction(key, cacheTime, getDataAction);
        }

        public object Get(string key)
        {
            return PersistableCacheHelper.Get(key);
        }

        public T Get<T>(string key)
        {
            return PersistableCacheHelper.Get<T>(key);
        }

        public void GetAsync(string key, Action<object> callback)
        {
            PersistableCacheHelper.GetAsync(key, callback);
        }

        public void GetAsync<T>(string key, Action<T> callback)
        {
            PersistableCacheHelper.GetAsync(key, callback);
        }

        public void Remove(string key)
        {
            PersistableCacheHelper.Remove(key);
        }

        public void RemoveAll()
        {
            PersistableCacheHelper.Clear();
        }

        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "DefaultCacher"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(Controls.IPageBrowser browser)
        {
            
        }

        public object GetInstance(System.Windows.Controls.TabItem tab)
        {
            return this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }

    /// <summary>
    /// The cache for silverlight client
    /// </summary>
    internal static class PersistableCacheHelper
    {
        private static readonly string persistFile = "newegg.persistablecacheV1.data";
        private static object syncObject = new object();

        private static Dictionary<string, CacheItem> Container { get; set; }
        private static Dictionary<string, CacheAsyncTask> AsyncTasks { get; set; }
        private static Queue<CacheAsyncTask> PendingAsyncQueue { get; set; }
        private static Thread AsyncThread { get; set; }
        private static Thread PersistThread { get; set; }
        private static Thread OverdueThread { get; set; }

        public static TimeSpan PersistInterval { get; set; }


        static PersistableCacheHelper()
        {
            Container = new Dictionary<string, CacheItem>();

            Init();

            AsyncTasks = new Dictionary<string, CacheAsyncTask>();
            PendingAsyncQueue = new Queue<CacheAsyncTask>();
            AsyncThread = new Thread(new ThreadStart(Async));
            AsyncThread.Name = "Async";
            AsyncThread.Start();

            PersistInterval = new TimeSpan(0, 5, 0);
            PersistThread = new Thread(new ThreadStart(Persist));
            PersistThread.Name = "Persist";
            PersistThread.Start();

            OverdueThread = new Thread(new ThreadStart(Overdue));
            OverdueThread.Name = "Overdue";
            OverdueThread.Start();
        }
        /// <summary>
        /// Add the data with specified key.
        /// (New data will add to cache and old data will be updated)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        internal static void Add(string key, object data, TimeSpan expires)
        {
            CacheItem cacheItem;

            lock (syncObject)
            {
                if (!Container.TryGetValue(key, out cacheItem))
                {
                    cacheItem = new CacheItem();
                }

                //cacheItem.Data = data;
                cacheItem.DataArray = UtilityHelper.BinarySerialize(data);
                cacheItem.Timeout = DateTime.Now.Add(expires);
                
                Container[key] = cacheItem;
            }
        }

        /// <summary>
        /// Adds the sync action.
        /// </summary>
        /// <param name="key">The cache item key for sync.</param>
        /// <param name="action">The sync action.</param>
        internal static void AddAsyncAction(string key, TimeSpan expires, Action<string, Action<object>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            lock (syncObject)
            {
                AsyncTasks[key] = new CacheAsyncTask { Key = key, SyncAction = action, DataExpires = expires };
            }
        }

        /// <summary>
        /// Gets the data with specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        internal static object Get(string key)
        {
            object result = null;

            CacheItem cacheItem;

            if (Container.TryGetValue(key, out cacheItem))
            {
                //result = cacheItem.Data;
                result = UtilityHelper.BinaryDeserialize(cacheItem.DataArray);
            }

            return result;
        }
        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T">type of cached data</typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        internal static T Get<T>(string key)
        {
            return (T)Get(key);
        }
        /// <summary>
        /// Gets data async.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="callback">The callback.</param>
        internal static void GetAsync(string key, Action<object> callback)
        {
            var data = Get(key);

            if (data != null)
            {
                callback(data);
            }
            else
            {
                CacheItem cacheItem;

                if (!Container.TryGetValue(key, out cacheItem))
                {
                    var syncTask = new CacheAsyncTask
                    {
                        Key = key,
                        Callback = callback
                    };

                    lock (syncObject)
                    {
                        PendingAsyncQueue.Enqueue(syncTask);
                    }
                }
            }
        }
        /// <summary>
        /// Gets data async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="callback">The callback.</param>
        internal static void GetAsync<T>(string key, Action<T> callback)
        {
            GetAsync(key, data => { callback((T)data); });
        }
        /// <summary>
        /// Removes data with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        internal static void Remove(string key)
        {
            lock (syncObject)
            {
                Container.Remove(key);
            }
        }
        /// <summary>
        /// Clears this cache.
        /// </summary>
        internal static void Clear()
        {
            lock (syncObject)
            {
                Container.Clear();
            }
        }

        /// <summary>
        /// Syncs cache data using sync action.
        /// </summary>
        private static void Async()
        {
            while (true)
            {
                while (PendingAsyncQueue.Count > 0)
                {
                    var task = PendingAsyncQueue.Dequeue();

                    CacheAsyncTask actionTask;

                    if (AsyncTasks.TryGetValue(task.Key, out actionTask))
                    {
                        actionTask.SyncAction(task.Key, data =>
                        {
                            Add(task.Key, data, actionTask.DataExpires);

                            task.Callback(data);
                        });
                    }
                }

                Thread.Sleep(TimeSpan.FromSeconds(60));
            }
        }


        /// <summary>
        /// Persists this instance to isolate storage.
        /// </summary>
        private static void Persist()
        {
            while (true)
            {
                Thread.Sleep(PersistInterval);

                lock (syncObject)
                {
                    IsolatedStoreageHelper.Write(persistFile, Container);
                }
            }
        }

        /// <summary>
        /// remove timeout items
        /// </summary>
        private static void Overdue()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(60));

                List<KeyValuePair<string, CacheItem>> itemsToRemove = new List<KeyValuePair<string, CacheItem>>();

                foreach (var item in Container)
                {
                    if (item.Value.Timeout < DateTime.Now)
                    {
                        itemsToRemove.Add(item);
                    }
                }

                lock (syncObject)
                {
                    foreach (var item in itemsToRemove)
                    {
                        if (Container.ContainsKey(item.Key))
                        {
                            Container.Remove(item.Key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inits this instance from isolate storage.
        /// </summary>
        private static void Init()
        {
            try
            {
                Container = IsolatedStoreageHelper.Read(persistFile) as Dictionary<string, CacheItem>;
            }
            finally
            {
                if (Container == null)
                {
                    Container = new Dictionary<string, CacheItem>();
                }
            }
        }
    }

    /// <summary>
    /// Cache item
    /// </summary>
    public class CacheItem
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        /// 
        public byte[] DataArray { get; set; }

        /// <summary>
        /// Gets or sets the time out.
        /// </summary>
        /// <value>The time out.</value>
        public DateTime Timeout { get; set; }
    }
    /// <summary>
    /// Sync task for cache
    /// </summary>
    internal class CacheAsyncTask
    {
        public string Key { get; set; }
        public Action<object> Callback { get; set; }
        public Action<string, Action<object>> SyncAction { get; set; }
        public TimeSpan DataExpires { get; set; }
    }



}
