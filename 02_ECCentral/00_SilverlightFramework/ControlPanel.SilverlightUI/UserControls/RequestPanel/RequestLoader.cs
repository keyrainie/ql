using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;
using System.Xml;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities;
using System.Net;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.CommonService;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Threading;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.RequestPanel
{
    internal class RequestLoader
    {
        private RequestInfo m_request;
        private Action<object> m_callback;

        private readonly WebClient m_webClient;
        private static readonly Dictionary<string, Stream> m_xapStreams;

        public event DownloadProgressChangedEventHandler DownloadProgress;

        static RequestLoader()
        {
            m_xapStreams = new Dictionary<string, Stream>();
        }

        public RequestLoader()
        {
            m_webClient = new WebClient();
            m_webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(s_webClient_OpenReadCompleted);
            m_webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(m_webClient_DownloadProgressChanged);
        }

        void s_webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (m_callback != null)
                {
                    m_callback(new Exception("Xap package load failed."));
                    return;
                }
            }

            var xapStream = e.Result;
            m_xapStreams[m_request.XapUrl] = xapStream;

            this.OnDownLoadCompleted(xapStream);
        }

        void m_webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (this.DownloadProgress != null)
            {
                this.DownloadProgress(this, e);
            }
        }

        #region Public Methods.

        public void Load(RequestInfo request, Action<object> callback)
        {
            m_request = request;
            m_callback = callback;

            this.LoadInternal();
        }

        public ObservableCollection<RequestInfo> LoadDataSource()
        {
            try
            {
                var value = CPApplication.Current.Browser.Configuration.GetConfigValue("Framework", "RequestPanelConfig");

                if (!value.IsNullOrEmpty())
                {
                    var result = UtilityHelper.XmlDeserialize<ObservableCollection<RequestInfo>>(value);


                    //过滤TabName/XapUrl/AssemblyName/ClassName为空的数据源);
                    result.ToList().ForEach(item =>
                                                {
                                                    if (item.TabName.IsNullOrEmpty() || item.XapUrl.IsNullOrEmpty()
                                                        || item.AssemblyName.IsNullOrEmpty() ||
                                                        item.ClassName.IsNullOrEmpty())
                                                    {
                                                        result.Remove(item);
                                                    }
                                                });

                    //过滤没有权限的数据源
                    result.ToList().ForEach(item =>
                                                {
                                                    if (!item.AuthKey.IsNullOrEmpty()
                                                        && !CPApplication.Current.Browser.AuthManager.HasFunction(
                                                            item.AuthKey))
                                                    {
                                                        result.Remove(item);
                                                    }
                                                });


                    return result;
                }
                return null;
            }
            catch(Exception ex)
            {
                ComponentFactory.Logger.LogError(ex);
                return null;
            }
        }

        #endregion

        #region Private Methods.

        private void LoadInternal()
        {
            var sp = m_request.XapUrl.Split('/');
            var xapName = sp[sp.Length - 1];

            var version = ComponentFactory.GetComponent<IXapVersionController>().GetXapVersion(xapName);

            m_request.XapUrl = m_request.XapUrl.Replace(xapName, xapName + "?Version=" + version);

            //检查内存是否存在Xap的流，如果存在，将不从服务器上下载Xap包
            if (!m_xapStreams.ContainsKey(m_request.XapUrl))
            {
                var requestUrl = m_request.XapUrl;

                if (Application.Current.IsRunningOutOfBrowser)
                {
                    requestUrl = string.Format("{0}{1}{2}", CPApplication.Current.PortalBaseAddress, "ClientBin/", m_request.XapUrl);
                }

                m_webClient.OpenReadAsync(new Uri(requestUrl, UriKind.RelativeOrAbsolute));
            }
            else
            {
                this.OnDownLoadCompleted(m_xapStreams[m_request.XapUrl]);
            }

        }

        private void LoadAssembly(Stream stream, AssemblyPart assemblyPart, Action<Assembly> callback)
        {
            Assembly a = null;
            try
            {
                Stream assemblyStream = Application.GetResourceStream(
                    new StreamResourceInfo(stream, null),
                    new Uri(assemblyPart.Source, UriKind.Relative)).Stream;
                a = assemblyPart.Load(assemblyStream);

                if (callback != null)
                {
                    callback(a);
                }
            }
            catch (NullReferenceException)
            {
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        a = assemblyPart.Load(e.Result);
                    }

                    if (callback != null)
                    {
                        callback(a);
                    }
                };
                wc.OpenReadAsync(new Uri(assemblyPart.Source, UriKind.RelativeOrAbsolute));
            }
        }

        private IEnumerable<AssemblyPart> GetParts(Stream stream)
        {
            var streamResourceInfo = new StreamResourceInfo(stream, null);
            var manifestUri = new Uri("AppManifest.xaml", UriKind.Relative);
            var manifestStreamInfo = Application.GetResourceStream(streamResourceInfo, manifestUri);
            var assemblyParts = new List<AssemblyPart>();
            if (manifestStreamInfo != null)
            {
                var manifestStream = manifestStreamInfo.Stream;//或者到XML流 
                using (var reader = XmlReader.Create(manifestStream))
                {
                    if (reader.ReadToFollowing("AssemblyPart"))
                    {
                        do
                        {
                            string source = reader.GetAttribute("Source");
                            if (source != null)
                            {
                                //找到各个dll的文件名 
                                assemblyParts.Add(new AssemblyPart() { Source = source });
                            }
                        }
                        while (reader.ReadToNextSibling("AssemblyPart"));
                    }
                }
            }
            return assemblyParts;
        }

        private void OnDownLoadCompleted(Stream stream)
        {
            var parts = this.GetParts(stream);
            Assembly assembly = null;

            foreach (var part in parts)
            {
                LoadAssembly(stream, part, (a) =>
                {
                    if (part.Source.Equals(m_request.AssemblyName, StringComparison.OrdinalIgnoreCase))
                    {
                        assembly = a;
                    }
                });
            }

            if (assembly != null)
            {
                var element = assembly.CreateInstance(m_request.ClassName) as UIElement;

                if (element == null)
                {
                    if (m_callback != null)
                    {
                        m_callback(new Exception(PageResource.RequestPanel_Msg_LoadFailed));
                        return;
                    }
                }

                if (m_callback != null)
                {
                    m_callback(element);
                }
            }
            else
            {
                if (m_callback != null)
                    m_callback(new Exception(PageResource.RequestPanel_Msg_LoadFailed));
            }
        }

        #endregion
    }
}