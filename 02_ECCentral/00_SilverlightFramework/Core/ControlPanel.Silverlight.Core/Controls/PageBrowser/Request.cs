using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Resources;
using System.Text.RegularExpressions;
using System.Windows.Browser;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Resources;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class Request
    {
        public const string CONST_CONFIGNAME = "Client.config";
        private const string CONST_FORMAT = "{0}/{{module}}/{{view}}/{{param}}";
        private Dictionary<string, string> m_query;
        private string m_url;
        private static string m_host;
        private static string m_clientBinPath;
        private static readonly Regex s_regex;
        private object m_userState;
        private string m_xapUrl;
        private IModuleInfo m_moduleInfo;

        static Request()
        {
            string format = null;
            Dictionary<string, string> setting = GetConfig();

            if (setting.ContainsKey("RouteFormat"))
            {
                format = setting["RouteFormat"];
            }

            if (string.IsNullOrEmpty(format) || format.ToLower() == "default")
            {
                int index;

                if (!Application.Current.IsRunningOutOfBrowser)
                {
                    if ((index = Application.Current.Host.Source.AbsoluteUri.IndexOf("/ClientBin/")) > -1)
                    {
                        m_clientBinPath = Application.Current.Host.Source.AbsoluteUri.Substring(0, index) + "/ClientBin/";
                    }
                    m_host = HtmlPage.Document.DocumentUri.AbsoluteUri;
                    if (!string.IsNullOrEmpty(HtmlPage.Document.DocumentUri.Fragment))
                    {
                        m_host = m_host.Replace(HtmlPage.Document.DocumentUri.Fragment, "");
                    }

                    if (!string.IsNullOrEmpty(HtmlPage.Document.DocumentUri.Query))
                    {
                        m_host = m_host.Replace(HtmlPage.Document.DocumentUri.Query, "");
                    }
                }
                else
                {
                    m_host = CPApplication.Current.PortalBaseAddress;
                    m_clientBinPath = string.Format("{0}{1}", m_host, "ClientBin/");
                }

                format = string.Format(CONST_FORMAT, m_host + "#");
            }

            Regex regex = new Regex(@"\{(?<name>[\w\.]+)\}", RegexOptions.IgnoreCase);
            Match formatMatch = regex.Match(format);
            string regexStr;

            if (!formatMatch.Success)
            {
                throw new Exception("Url format is error!");
            }

            regexStr = regex.Replace(format, @"(?<${name}>[^/?]+)").Replace(@"/(?<param>[^/?]+)", @"(/(?<param>[[\s\S*]+))?");
            s_regex = new Regex(regexStr, RegexOptions.IgnoreCase);

            //            if (!Application.Current.IsRunningOutOfBrowser)
            //            {
            //             //   Modify By Ryan:在FF中浏览器还没有把ActiveX控件加载完就在触发该hashchanged的事件导致了pageBrowser is undefined的异常；
            //                HtmlPage.Window.Eval(
            //                        @" 
            //                        var hashHandle = function(){
            //                            if(window.location.href.replace( /^[^#]*#?(.*)$/, '$1' ) != '' && silverlightHost){
            //                                var pageBrowser = silverlightHost.content.PageBrowser;
            //                                if(pageBrowser != null && pageBrowser != undefined)
            //                                {
            //                                    pageBrowser.NavigateFromScript(window.location.href); 
            //                                }
            //                            }
            //                        };
            //                        $(window).hashchange(hashHandle);");

            //            }
        }

        //public string DomainName
        //{
        //    get;
        //    internal set;
        //}

        public string ModuleName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Request view name
        /// </summary>
        /// <example>The request URL is: http://merchant.neweggmall.biz/Portal/InvoiceMgmt/MaintainInvoice/455000. View value is: MaintainInvoice</example>
        public string ViewName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Request param
        /// </summary>
        /// <example>The request URL is: http://merchant.neweggmall.biz/Portal/InvoiceMgmt/MaintainInvoice/455000. Param value is: 455000</example>
        public string Param
        {
            get;
            internal set;
        }

        /// <summary>
        /// Request URL
        /// </summary>
        /// <example>The request URL is: http://merchant.neweggmall.biz/Portal/InvoiceMgmt/MaintainInvoice/455000. URL value is: http://merchant.neweggmall.biz/Portal/InvoiceMgmt/MaintainInvoice/455000</example>
        public string URL
        {
            get
            {
                return m_url;
            }
            internal set
            {
                m_url = value;
            }
        }

        public string ClientBinPath
        {
            get
            {
                return m_clientBinPath;
            }
        }

        /// <summary>
        /// Request param
        /// </summary>
        /// <example>The request URL is: http://merchant.neweggmall.biz/Portal/InvoiceMgmt/MaintainInvoice/455000. Host value is: http://merchant.neweggmall.biz/Portal/</example>
        public static string Host
        {
            get
            {
                return m_host;
            }
        }

        /// <summary>
        /// Set any type or value to this property.
        /// </summary>
        public object UserState
        {
            get
            {
                return m_userState;
            }

            internal set
            {
                m_userState = value;
            }
        }

        public string XapUrl
        {
            get
            {
                return m_xapUrl;
            }
        }

        public IModuleInfo ModuleInfo
        {
            get
            {
                return m_moduleInfo;
            }
        }

        /// <summary>
        /// Request query string.
        /// </summary>
        /// <example>The request URL is: http://merchant.neweggmall.biz/Portal/InvoiceMgmt/MaintainInvoice?invoicenumber=455000. QueryString["invoiceNumber"] value is: 455000</example>
        public Dictionary<string, string> QueryString
        {
            get
            {
                return m_query;
            }
            internal set
            {
                m_query = value;
            }
        }

        /// <summary>
        /// save current page's history data 
        /// </summary>
        public object HistoryData { get; set; }

        /// <summary>
        /// get referer request
        /// </summary>
        public Request RefererRequest
        {
            get;
            internal set;
        }

        private Request()
            : base()
        { }

        public Request(string url)
            : this()
        {
            if (url.ToLower().IndexOf("/") == 0)
            {
                url = string.Format("{0}#/{1}", Request.Host, url.Trim('/'));
            }

            Match match = s_regex.Match(url);

            if (match.Success)
            {
                m_url = url;
                //m_url = HttpUtility.UrlDecode(url);

                if (string.IsNullOrEmpty(match.Groups["module"].Value))
                {
                    throw new ArgumentNullException("url");
                }

                //  this.DomainName = match.Groups["domain"].Value;

                ModuleName = match.Groups["module"].Value;

                if (string.IsNullOrEmpty(match.Groups["view"].Value))
                {
                    this.ViewName = "Index";
                }
                else
                {
                    this.ViewName = match.Groups["view"].Value;
                }

                if (!string.IsNullOrEmpty(match.Groups["param"].Value))
                {
                    this.Param = match.Groups["param"].Value;
                }

                m_xapUrl = Path.Combine(ClientBinPath, ModuleName + ".xap?version=" + ComponentFactory.GetComponent<IXapVersionController>().GetXapVersion(ModuleName + ".xap"));

            }
            else
            {
                m_url = url;
                throw new PageException(MessageResource.ErrorInfo_Invalid_Url,
                        string.Format(MessageResource.ErrorInfo_Invalid_Url_Message, m_url), this);
            }

            match = Regex.Match(url, @"\?(?<query>.*)");
            if (match.Success)
            {
                CreateQueryString(match.Groups["query"].Value);
            }

            IModuleManager manager = ComponentFactory.GetComponent<IModuleManager>();
            m_moduleInfo = manager.GetModuleInfoByName(ModuleName);
            if (m_moduleInfo == null)
            {
                m_moduleInfo = manager.CreateModuleInfo(this.ModuleName);
            }
        }

        private void CreateQueryString(string query)
        {
            m_query = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(query))
            {
                Regex regex = new Regex(@"(?<pair>(?<name>[^=&]+)=(?<value>[^=&]+))");

                query = query.TrimStart('?');
                foreach (Match match in regex.Matches(query))
                {
                    m_query.Add(match.Groups["name"].Value, match.Groups["value"].Value);
                }
            }
        }

        private static Dictionary<string, string> GetConfig()
        {
            StreamResourceInfo configStream = Application.GetResourceStream(new Uri(CONST_CONFIGNAME, UriKind.Relative));
            Dictionary<string, string> settings = new Dictionary<string, string>();

            if (configStream != null)
            {
                XDocument doc = XDocument.Load(configStream.Stream);
                var config = from node in doc.Descendants("add")
                             select new { Key = node.Attribute("key").Value, Value = node.Attribute("value").Value };

                foreach (var item in config)
                {
                    settings.Add(item.Key, item.Value);
                }
            }

            return settings;
        }

    }
}
