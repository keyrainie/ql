using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.Configuration;
using System.IO;
using System.Web.Configuration;

namespace ECCentral.Service.EventConsumer
{
    // IPP3中 SO 中的 WCFHelper类
    public class WCFAdapter<T> where T : class
    {
        public static T GetProxy()
        {
            return CreateChannel();
        }
        private static T CreateChannel()
        {
            ChannelFactory<T> factory
                = new ChannelFactory<T>(EndPointConfigName);
            return factory.CreateChannel();
        }

        private static Dictionary<string, string> EndpointConfigNameDic = new Dictionary<string, string>();
        private static object getEndpointConfigNameLocker = new object();
        private static string EndPointConfigName
        {
            get
            {
                string tFullName = typeof(T).FullName;
                string configName = null;
                if (EndpointConfigNameDic.ContainsKey(tFullName))
                {
                    configName = EndpointConfigNameDic[tFullName];
                }
                if (configName == null)
                {
                    lock (getEndpointConfigNameLocker)
                    {
                        if (configName == null)
                        {
                            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");// ConfigurationManager.OpenExeConfiguration("");
                            ServiceModelSectionGroup sectionGroup = (ServiceModelSectionGroup)configuration.GetSectionGroup("system.serviceModel");
                            foreach (ChannelEndpointElement ce in sectionGroup.Client.Endpoints)
                            {
                                if (ce.Contract == tFullName)
                                {
                                    if (String.IsNullOrEmpty(ce.Name))
                                    {
                                        throw new Exception("请给WCF配置端点添加唯一名称name");
                                    }
                                    configName = ce.Name;
                                    EndpointConfigNameDic.Add(tFullName, configName);
                                    break;
                                }
                            }
                            if (configName == null)
                            {
                                throw new Exception("没有找到对应的WCF服务配置。");
                            }
                        }
                    }
                }
                return configName;
            }
        }
    }
}
