using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Utility.AsynQueue
{
    public class MemcachedQueue<T> : IQueue<T>
    {
        private MemcachedClient m_Client;
        private string m_QueueName;
        private const ulong MAX_LENGTH = 1024 * 1024;

        public MemcachedQueue(string queueName, string[] serverList, MemcachedConfig config = null)
        {
            m_QueueName = queueName;
            serverList = serverList.BubbleSort();
            string k = string.Join(",", serverList);
            if (MemcachedClient.Exists(k) == false)
            {
                MemcachedClient.Setup(k, serverList);
            }
            m_Client = MemcachedClient.GetInstance(k);
            if (config != null)
            {
                if (config.SendReceiveTimeout.HasValue)
                {
                    m_Client.SendReceiveTimeout = config.SendReceiveTimeout.Value;
                }
                if (config.ConnectTimeout.HasValue)
                {
                    m_Client.ConnectTimeout = config.ConnectTimeout.Value;
                }
                if (config.MinPoolSize.HasValue)
                {
                    m_Client.MinPoolSize = config.MinPoolSize.Value;
                }
                if (config.MaxPoolSize.HasValue)
                {
                    m_Client.MaxPoolSize = config.MaxPoolSize.Value;
                }
                if (config.SocketRecycleAge.HasValue)
                {
                    m_Client.SocketRecycleAge = config.SocketRecycleAge.Value;
                }
                if (config.CompressionThreshold.HasValue)
                {
                    m_Client.CompressionThreshold = config.CompressionThreshold.Value;
                }
            }
        }

        private ulong GetIncreasedIndex(string index_key)
        {
            ulong? index = null;
            while (index == null)
            {
                bool r = m_Client.Add(index_key, 0);
                if (r)
                {
                    index = 0;
                }
                else
                {
                    //index = m_Client.Increment(en_index_key, 1);
                    MemcachedClient.CasResult casRst;
                    ulong unique;
                    do
                    {
                        object t = m_Client.Gets(index_key, out unique);
                        if (t == null)
                        {
                            index = null;
                            break;
                        }
                        index = Convert.ToUInt64(t);
                        if (index >= MAX_LENGTH)
                        {
                            index = 0;
                        }
                        else
                        {
                            index = index + 1;
                        }
                        casRst = m_Client.CheckAndSet(index_key, index.Value, unique);
                    } while (casRst != MemcachedClient.CasResult.Stored);
                }
            }
            return index.Value;
        }

        private string BuildKeyByIndex(ulong index)
        {
            return "qu_(" + m_QueueName + ")_" + index;
        }

        public void Enqueue(T msg)
        {
            string en_index_key = "qu_en_Index_" + m_QueueName;
            ulong index = GetIncreasedIndex(en_index_key);
            string key = BuildKeyByIndex(index);
            m_Client.Set(key, msg);
        }

        public int Dequeue(out T msg)
        {
            string de_index_key = "qu_de_Index_" + m_QueueName;
            ulong index;
            MemcachedClient.CasResult casRst;
            ulong unique;
            do
            {
                object t = m_Client.Gets(de_index_key, out unique);
                if (t == null || ulong.TryParse(t.ToString().Trim(), out index) == false || index < 0 || index >= MAX_LENGTH)
                {
                    index = 0;
                }
                else
                {
                    index = index + 1;
                }
                object tmp = m_Client.Get(BuildKeyByIndex(index));
                if (tmp == null)
                {
                    msg = default(T);
                    return -1;
                }
                msg = (T)tmp;
                if (t == null)
                {
                    casRst =m_Client.Add(de_index_key, index) ?  MemcachedClient.CasResult.Stored : MemcachedClient.CasResult.NotStored;
                }
                else
                {
                    casRst = m_Client.CheckAndSet(de_index_key, index, unique);
                }
            } while (casRst != MemcachedClient.CasResult.Stored);
            return (int)index;
        }
    }

    public class MemcachedConfig
    {
        public int? SendReceiveTimeout { get; set; }
        public int? ConnectTimeout { get; set; }
        public uint? MinPoolSize { get; set; }
        public uint? MaxPoolSize { get; set; }
        public TimeSpan? SocketRecycleAge { get; set; }
        public uint? CompressionThreshold { get; set; }
    }
}
