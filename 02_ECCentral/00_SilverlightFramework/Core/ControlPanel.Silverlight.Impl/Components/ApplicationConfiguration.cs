using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.ConfigurationService;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.ObjectModel;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class ApplicationConfiguration : IConfiguration
    {
        static ObservableCollection<ConfigKeyValueMsg> m_ConfigList;


        #region IConfiguration Members

        public void LoadConfig(Action callback)
        {
            ConfigurationServiceV40Client serviceClient = new ConfigurationServiceV40Client();
            serviceClient.GetApplicationConfigCompleted += new EventHandler<GetApplicationConfigCompletedEventArgs>(serviceClient_GetApplicationConfigCompleted);
            serviceClient.GetApplicationConfigAsync(callback);


        }

        void serviceClient_GetApplicationConfigCompleted(object sender, GetApplicationConfigCompletedEventArgs e)
        {
            m_ConfigList = e.Result;

            Action callback = e.UserState as Action;
            if (callback != null)
            {
                callback();
            }
        }

        public string GetConfigValue(string domainName, string key)
        {
            if (m_ConfigList != null)
            {
                string prefix = string.Empty;
                if (CPApplication.Current.CommonData.ContainsKey("ECCentralConfigPrefix"))
                {
                    object tmp = CPApplication.Current.CommonData["ECCentralConfigPrefix"];
                    if (tmp != null)
                    {
                        prefix = tmp.ToString().Trim();
                    }
                }
                foreach (ConfigKeyValueMsg kv in m_ConfigList)
                {
                    if (kv.Domain.ToLower().Trim() == domainName.Trim().ToLower()
                        && kv.Key.ToLower().Trim() == (prefix + key).Trim().ToLower())
                    {
                        return kv.Value;
                    }
                }

                return null;
            }

            throw new Exception("Not found application config.");
        }

        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "Configuration"; }
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

        public void Dispose()
        {

        }
        #endregion

        #region 扩展配置，ECCentral专用配置
        public void GetECCentralServiceURL(Action callback)
        {
            ConfigurationServiceV40Client serviceClient = new ConfigurationServiceV40Client();
            serviceClient.GetECCentralConfigCompleted += new EventHandler<GetECCentralConfigCompletedEventArgs>(serviceClient_GetECCentralConfigCompleted);
            serviceClient.GetECCentralConfigAsync(callback);

        }

        void serviceClient_GetECCentralConfigCompleted(object sender, GetECCentralConfigCompletedEventArgs e)
        {
            ECCentralMsg msg = e.Result;
            if (CPApplication.Current.CommonData == null)
            {
                CPApplication.Current.CommonData = new Dictionary<string, object>();
            }
            CPApplication.Current.CommonData.Add("ECCentralServiceURL", msg.ServiceURL);
            CPApplication.Current.CommonData.Add("ECCentralConfigPrefix", (msg.ConfigPrefix == null || msg.ConfigPrefix.Trim().Length <= 0 ? string.Empty : msg.ConfigPrefix.Trim()));
            Action callback = e.UserState as Action;
            if (callback != null)
            {
                callback();
            }
        }
        #endregion
    }
}
