using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Components;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class AssemblyLoader : IAssemblyLoader
    {
        private int count = 0;
        private int loaded = 0;
        private Action m_callback;

        public void LoadShareAssembly(Action callback)
        {
            m_callback = callback;

            LoadXmlConfig();
        }

        private void LoadXmlConfig()
        {
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadXmlCompleted);
            
            var configUrl = "ShareDll.xml";
            string requestUrl = string.Empty;
            if (Application.Current.IsRunningOutOfBrowser)
            {
                requestUrl = string.Format("{0}{1}{2}", CPApplication.Current.PortalBaseAddress, "ClientBin/ShareDll/", configUrl);
            }
            else
            {
                requestUrl = string.Format("{0}/{1}", "ShareDll", configUrl);
            }
            webClient.OpenReadAsync(new Uri(requestUrl, UriKind.RelativeOrAbsolute));
        }

        void webClient_DownloadXmlCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }
            string xmlStr = "";
            using (StreamReader reader = new StreamReader(e.Result))
            {
                xmlStr = reader.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(xmlStr))
            {
                var list = UtilityHelper.XmlDeserialize<ObservableCollection<AssemblyInfo>>(xmlStr).ToList();
                count = list.Count;

                if (list != null)
                {                    
                    list.ForEach(p =>
                    {
                        WebClient client = new WebClient();
                        client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_DownloadAssemblyCompleted);
                        
                        string requestUrl = string.Empty;
                        if (Application.Current.IsRunningOutOfBrowser)
                        {
                            requestUrl = string.Format("{0}{1}{2}", CPApplication.Current.PortalBaseAddress, "ClientBin/ShareDll/", p.Name);
                        }
                        else
                        {
                            requestUrl = string.Format("{0}/{1}", "ShareDll", p.Name);
                        }
                        client.OpenReadAsync(new Uri(requestUrl, UriKind.RelativeOrAbsolute));
                    });
                }
            }
        }

        void client_DownloadAssemblyCompleted(object sender, OpenReadCompletedEventArgs e)
        {           
            //加载到内存
            AssemblyPart assemblyPart = new AssemblyPart();
            Assembly assembly = assemblyPart.Load(e.Result);

            Interlocked.Increment(ref loaded);

            if (loaded == count && this.m_callback != null)
            {
                m_callback();
            }
        }

        #region IComponent Members

        public string Name
        {
            get { return "Share Assembly Load Component"; }
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
    }
}
