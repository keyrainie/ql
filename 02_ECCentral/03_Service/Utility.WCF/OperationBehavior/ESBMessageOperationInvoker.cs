using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.Configuration;

namespace ECCentral.Service.Utility.WCF
{
    public class ESBMessageOperationInvoker : IOperationInvoker
    {
        private IOperationInvoker m_Core;
        private string m_CacheName = ConfigurationManager.AppSettings["ESBMessageIDCache"];
        private string GetCacheName()
        {
            if (m_CacheName != null && m_CacheName.Trim().Length > 0)
            {
                return m_CacheName;
            }
            return null;
        }

        private object GetFromCache(string key)
        {
            try
            {
                return CacheFactory.GetInstance(GetCacheName()).Get(key);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
                return null;
            }
        }

        public ESBMessageOperationInvoker(IOperationInvoker invoker)
        {
            m_Core = invoker;
        }

        public object[] AllocateInputs()
        {
            return m_Core.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            if (inputs != null && inputs.Length == 1 && inputs[0] != null && inputs[0] is ESBMessage)
            {
                ESBMessage msg = (ESBMessage)inputs[0];
                string key = msg.MessageID + "[|]" + msg.SubscriberID;
                var obj = GetFromCache(key);
                if (obj != null && obj.ToString() == "esb")
                {
                    Logger.WriteLog("Find the duplicated msg task and ignore it: [MessageID]:[" + msg.MessageID + "], [SubscriberID]:[" + msg.SubscriberID + "].", "Duplicated_ESB_MessageTask");
                    outputs = new object[0];
                    return null;
                }
                else
                {
                    var obj1 = m_Core.Invoke(instance, inputs, out outputs);
                    try
                    {
                        CacheFactory.GetInstance(GetCacheName()).Set(key, "esb", DateTime.Now.AddMinutes(10));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.HandleException(ex);
                    }
                    return obj1;
                }
            }
            else
            {
                return m_Core.Invoke(instance, inputs, out outputs);
            }
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return m_Core.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return m_Core.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return m_Core.IsSynchronous; }
        }
    }
}
