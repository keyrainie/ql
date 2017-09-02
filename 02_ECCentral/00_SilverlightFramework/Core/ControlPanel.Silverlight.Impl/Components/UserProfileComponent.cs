using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading;

using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.ProfileService;
using Newegg.Oversea.Silverlight.Utilities;
using System.Text;
using System.IO;
using System.Windows;


namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class UserProfileComponent : IUserProfile
    {
        private const string SYNC_KEYS = "FM.SyncKeys";

        private ProfileV40Client m_serviceClient;
        private Timer m_timer;
        private TimeSpan m_syncIntervalTime;
        private Dictionary<string, bool> m_changedFlags = new Dictionary<string, bool>();

        public TimeSpan SyncIntervalTime
        {
            get { return m_syncIntervalTime; }
            set
            {
                m_syncIntervalTime = value;
                if (m_timer == null)
                {
                    m_timer = new Timer(obj => SyncData());
                }

                m_timer.Change(TimeSpan.FromMinutes(3), m_syncIntervalTime);
            }
        }

        public UserProfileComponent()
        {
            m_serviceClient = new ProfileV40Client();
            m_serviceClient.QueryCompleted += new EventHandler<QueryCompletedEventArgs>(m_serviceClient_QueryCompleted);
            m_serviceClient.SaveCompleted += new EventHandler<SaveCompletedEventArgs>(m_serviceClient_SaveCompleted);
            m_serviceClient.GetDataGridProfileItemsCompleted += new EventHandler<GetDataGridProfileItemsCompletedEventArgs>(m_serviceClient_GetDataGridProfileItemsCompleted);
        }

        void m_serviceClient_GetDataGridProfileItemsCompleted(object sender, GetDataGridProfileItemsCompletedEventArgs e)
        {
            var callBack = e.UserState as Action<List<DataGridProfileItem>>;
            if (e.Error == null && e.Result != null)
            {
                if (callBack != null)
                {
                    callBack(ConvertFromMsg(e.Result.Body));
                }
            }
            else
            {
                if (callBack != null)
                {
                    callBack(null);
                }
            }
        }

        public List<DataGridProfileItem> ConvertFromMsg(ObservableCollection<DataGridProfileItemMsg> msg)
        {
            List<DataGridProfileItem> result = new List<DataGridProfileItem>();
            if (msg != null)
            {
                foreach (var msgItem in msg)
                {
                    result.Add(new DataGridProfileItem
                    {
                        Owner = new AuthUser
                        {
                            DepartmentName = msgItem.Owner.DepartmentName,
                            DepartmentNumber = msgItem.Owner.DepartmentNumber,
                            DisplayName = msgItem.Owner.DisplayName,
                            Domain = msgItem.Owner.Domain,
                            ID = msgItem.Owner.UserName,
                            LoginName = msgItem.Owner.UserName,
                            UserEmailAddress = msgItem.Owner.EmailAddress
                        },
                        DataGridProfileXml = msgItem.DataGridProfileXml
                    });
                }
            }
            if (result.Count == 0)
            {
                result.Add(new DataGridProfileItem
                    {
                        Owner = new AuthUser
                        {
                            DisplayName = "No data to copy.",
                        },
                        DataGridProfileXml = null
                    });
            }
            return result;
        }

        #region Service Callback

        void m_serviceClient_SaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    CPApplication.Current.Browser.Logger.LogError(e.Error);
                });
                return;
            }
            if (e.Result.Faults != null && e.Result.Faults.Count > 0)
            {
                var errorMsg = e.Result.Faults[0].ErrorDetail;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    CPApplication.Current.Browser.Logger.WriteLog(errorMsg, "ExceptionLog");
                });
                return;
            }
        }

        void m_serviceClient_QueryCompleted(object sender, QueryCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    CPApplication.Current.Browser.Logger.LogError(e.Error);
                });
                return;
            }
            if (e.Result.Faults != null && e.Result.Faults.Count > 0)
            {
                var errorMsg = e.Result.Faults[0].ErrorDetail;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    CPApplication.Current.Browser.Logger.WriteLog(errorMsg, "ExceptionLog");
                });
                return;
            }

            //True:加载ProfileData的数据； 
            //False:加载同步Key的数据
            var loadProfileType = e.UserState as LoadProfileType;

            if (loadProfileType != null)
            {
                if (loadProfileType.IsLoadProfile)
                {
                    var result = e.Result.Value;
                    var key = result.ToList().SingleOrDefault(p => p.Key == SYNC_KEYS);
                    if (key != null)
                        result.Remove(key);

                    foreach (var userProfile in result)
                    {
                        IsolatedStoreageHelper.Write(BuildKey(userProfile.Key), userProfile.Data);
                    }

                    if (loadProfileType.CallBack != null)
                    {
                        loadProfileType.CallBack(null);
                    }
                }
                else
                {
                    if (e.Result.Value != null && e.Result.Value.Count == 1)
                    {
                        var keys = UtilityHelper.XmlDeserialize<List<string>>(e.Result.Value[0].Data);
                        IsolatedStoreageHelper.Write(BuildKey(SYNC_KEYS), keys);

                        if (keys != null)
                            LoadDataByKeys(keys, loadProfileType.CallBack);
                    }
                    else
                    {

                        if (loadProfileType.CallBack != null)
                        {
                            loadProfileType.CallBack(null);
                        }
                    }
                }
            }
        }

        #endregion

        #region IUserProfile Members

        /// <summary>
        /// 加载UserProfile的数据
        /// 如果独立存储中不存在数据，将从数据库同步数据到独立存储;
        /// </summary>
        public void LoadProfileData(Action<object> callback)
        {
            var sync_keys = IsolatedStoreageHelper.Read(BuildKey(SYNC_KEYS)) as List<string>;

            if (sync_keys != null && sync_keys.Count > 0)
            {
                m_changedFlags[BuildKey(SYNC_KEYS)] = true;

                LoadDataByKeys(sync_keys, callback);
            }
            else
            {
                m_changedFlags[BuildKey(SYNC_KEYS)] = false;

                m_serviceClient.QueryAsync(new ProfileQueryV40
                {
                    Header = new MessageHeader(),
                    Body = new ProfileQueryMsg
                    {
                        ApplicationId = CPApplication.Current.Application.Id,
                        InUser = CPApplication.Current.LoginUser.ID,
                        ProfileType = SYNC_KEYS
                    }
                }, new LoadProfileType { CallBack = callback, IsLoadProfile = false });
            }
        }

        /// <summary>
        /// 保存数据到独立存储
        /// </summary>
        /// <param name="key">在独立存储中保存或获取的唯一Key</param>
        /// <param name="data">数据对象</param>
        public void Set(string key, object data)
        {
            Set(key, data, false);
        }

        /// <summary>
        /// 保存数据到独立存储
        /// </summary>
        /// <param name="key">在独立存储中保存或获取的唯一Key</param>
        /// <param name="data">数据对象</param>
        /// <param name="needSync">是否需要同步数据到数据库</param>
        public void Set(string key, object data, bool needSync)
        {
            var keys = IsolatedStoreageHelper.Read(BuildKey(SYNC_KEYS)) as List<string>;
            var b = false;

            if (keys == null)
            {
                keys = new List<string>();
            }
            if (needSync)
            {
                if (!keys.Contains(key))
                {
                    keys.Add(key);
                    m_changedFlags[BuildKey(SYNC_KEYS)] = true;
                    b = true;
                }

                m_changedFlags[BuildKey(key)] = true;
            }
            else
            {
                if (keys.Contains(key))
                {
                    keys.Remove(key);
                    m_changedFlags[BuildKey(SYNC_KEYS)] = true;
                    b = true;
                }
            }

            if (b)
            {
                IsolatedStoreageHelper.Write(BuildKey(SYNC_KEYS), keys);
            }

            IsolatedStoreageHelper.Write(BuildKey(key), UtilityHelper.XmlSerialize(data));
        }

        /// <summary>
        /// 通过Key获取独立存储中的数据
        /// </summary>
        /// <typeparam name="T">数据对象的类型</typeparam>
        /// <param name="key">唯一Key</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            var value = IsolatedStoreageHelper.Read(BuildKey(key));

            try
            {
                return value != null ? UtilityHelper.XmlDeserialize<T>(value.ToString()) : default(T);
            }
            catch
            {
                //当反序列化失败的时候，需要将相关key值，设置成null.
                IsolatedStoreageHelper.Write(BuildKey(key), null);
                return default(T);
            }

        }

        /// <summary>
        /// 同步独立存储的数据到数据库
        /// 由固定的Timer调用
        /// </summary>
        public void SyncData()
        {
            var userProfiles = new ObservableCollection<UserProfile>();
            var keys = IsolatedStoreageHelper.Read(BuildKey(SYNC_KEYS)) as List<string>;

            if (keys != null)
            {
                #region 同步Keys的数据

                if (m_changedFlags.ContainsKey(BuildKey(SYNC_KEYS))
                    && m_changedFlags[BuildKey(SYNC_KEYS)])
                {
                    userProfiles.Add(new UserProfile
                                         {
                                             ApplicationId = CPApplication.Current.Application.Id,
                                             InUser = CPApplication.Current.LoginUser.LoginName,
                                             Key = SYNC_KEYS,
                                             Data = UtilityHelper.XmlSerialize(keys)
                                         });

                    m_changedFlags[BuildKey(SYNC_KEYS)] = false;
                }

                #endregion

                #region 同步UserProfile的数据

                foreach (var key in keys)
                {
                    var data = IsolatedStoreageHelper.Read(BuildKey(key));

                    if (data != null && m_changedFlags.ContainsKey(BuildKey(key))
                        && m_changedFlags[BuildKey(key)])
                    {
                        userProfiles.Add(new UserProfile
                        {
                            ApplicationId = CPApplication.Current.Application.Id,
                            InUser = CPApplication.Current.LoginUser.LoginName,
                            Key = key,
                            Data = data.ToString()
                        });
                        m_changedFlags[BuildKey(key)] = false;
                    }
                }

                #endregion

                if (userProfiles.Count > 0)
                {
                    m_serviceClient.SaveAsync(new SimpleTypeDataContractOfArrayOfUserProfilep9OHFywk
                    {
                        Header = new MessageHeader(),
                        Value = userProfiles
                    });
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 生成最终的Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string BuildKey(string key)
        {
            return string.Format("UP_{0}_{1}", CPApplication.Current.LoginUser.LoginName, key).ToLower();
        }

        /// <summary>
        /// 通过同步的Key获取对应的Profile Data数据到独立存储
        /// </summary>
        private void LoadDataByKeys(List<string> keys, Action<object> callback)
        {
            var count = keys.Count(key => IsolatedStoreageHelper.Read(BuildKey(key)) == null);

            if (count == keys.Count)
            {
                m_serviceClient.QueryAsync(new ProfileQueryV40
                {
                    Header = new MessageHeader(),
                    Body = new ProfileQueryMsg
                    {
                        ApplicationId = CPApplication.Current.Application.Id,
                        InUser = CPApplication.Current.LoginUser.ID,
                    }
                }, new LoadProfileType { CallBack = callback, IsLoadProfile = true });

                foreach (var key in keys)
                {
                    m_changedFlags[BuildKey(key)] = false;
                }
            }
            else
            {
                foreach (var key in keys)
                {
                    m_changedFlags[BuildKey(key)] = true;
                }
                callback(null);
            }
        }

        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "User Profile Component"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {

        }

        public object GetInstance(TabItem tab)
        {
            return this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion


        public void  GetDataGridProfileItems(string gridGuid,Action<List<DataGridProfileItem>> callBack)
        {
            m_serviceClient.GetDataGridProfileItemsAsync(new SimpleTypeDataContractOfstring { Value = gridGuid }, callBack);
        }
    }

    public class LoadProfileType
    {
        public Action<object> CallBack { get; set; }

        public bool IsLoadProfile { get; set; }
    }

}
