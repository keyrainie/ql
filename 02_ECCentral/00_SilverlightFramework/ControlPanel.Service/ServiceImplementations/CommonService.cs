using System;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using System.Xml;

using Newegg.Oversea.Framework.ExceptionHandler;
using Newegg.Oversea.Framework.Log;
using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces;
using System.Collections.Specialized;


namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [ServiceErrorHandling]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    public class CommonService : ICommonServiceV40
    {
        private const string s_FrameworkXapName = "ControlPanel.SilverlightUI.xap";

        #region ICommonServiceV40 Members


        [ErrorHandling]
        public AppVersionV40 CheckAppVersion()
        {
            AppVersionV40 apps = new AppVersionV40();
            apps.ComputerName = Dns.GetHostName();

            var xapFiles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientBin"), "*.xap", SearchOption.TopDirectoryOnly);

            foreach (var xapFile in xapFiles)
            {
                XapVersion xapVersion = GetXapVersion(xapFile);
                apps.Body.Add(xapVersion);
            }

            //对Xap的信息按照XapName进行升序排序
            apps.Body = (from xap in apps.Body
                         orderby xap.XapName ascending
                         select xap).ToList();

            return apps;
        }

        [ErrorHandling]
        public XapVersionV40 GetFrameworkVersion()
        {
            XapVersionV40 xap = new XapVersionV40();

            string xapFile = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientBin"), s_FrameworkXapName);
            xap.Body = GetXapVersion(xapFile);
            return xap;
        }

        [ErrorHandling]
        public AppParamsV41 GetAppParams()
        {
            var appParams = new AppParamsV41();

            appParams.Body.GlobalRegion = CPConfig.Application.GloalRegion;
            appParams.Body.LocalRegion = CPConfig.Application.LocalRegion;
            appParams.Body.DefaultPage = CPConfig.Application.DefaultPage;

            appParams.Body.ServerComputerName = Dns.GetHostName();
            appParams.Body.ServerIPAddress = HttpContext.Current.Request.ServerVariables["Local_Addr"];
            string url = string.Empty;

            if (HttpContext.Current.Request.UrlReferrer != null
                && HttpContext.Current.Request.UrlReferrer.AbsoluteUri != null)
            {
                url = HttpContext.Current.Request.UrlReferrer.AbsoluteUri.ToString();
            }
            else
            {
                url = HttpContext.Current.Request.Url.AbsoluteUri.ToString();
            }

            appParams.Body.HostAddress = string.Format(url.Substring(0, url.IndexOf("/ClientBin") + 1));
            if (string.IsNullOrWhiteSpace(appParams.Body.HostAddress) && (!string.IsNullOrWhiteSpace(url)) && url.Length>9 )
            {
                appParams.Body.HostAddress = url.Substring(0, url.IndexOf("/", 8) + 1);
            }

            appParams.Body.FrameworkXapName = s_FrameworkXapName;
            appParams.Body.ClientIPAddress = CPContext.Current.GetClientIP();
            appParams.Body.ClientComputerName = CPContext.Current.GetClientComputerName();
            return appParams;
        }



        #endregion

        #region Private Methods

        private XapVersion GetXapVersion(string xapFile)
        {
            FileInfo xapFileInfo = new FileInfo(xapFile);
            XapVersion xapVersion = new XapVersion();

            string xapConfigPath = xapFile.Replace(".xap", ".xml");
            if (File.Exists(xapConfigPath))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xapConfigPath);
                
                xapVersion.XapName = xapFileInfo.Name;
                xapVersion.Title = xmlDoc.SelectNodes("DomainSettings/DomainVersion/Title")[0].InnerText;
                xapVersion.Version = xmlDoc.SelectNodes("DomainSettings/DomainVersion/Version")[0].InnerText;
                xapVersion.PublishDate = xapFileInfo.LastWriteTime.ToString();
                xapVersion.Description = xmlDoc.SelectNodes("DomainSettings/DomainVersion/Description")[0].InnerText;
                xapVersion.UpdateLevel = xmlDoc.SelectNodes("DomainSettings/DomainVersion/UpdateLevel")[0].InnerText;

                var hashCode = Math.Abs(xapFileInfo.Length.ToString().GetHashCode());
                xapVersion.Version = xapVersion.Version + "." + hashCode;
            }
            else
            {
                xapVersion.XapName = xapFileInfo.Name;
                xapVersion.Title = xapFileInfo.Name;
                xapVersion.Version = Math.Abs(xapFileInfo.Length.ToString().GetHashCode()).ToString();
                xapVersion.PublishDate = xapFileInfo.LastWriteTime.ToString();
                xapVersion.Description = "N/A";
                xapVersion.UpdateLevel = "N";
            }

            return xapVersion;
        }

        #endregion
    }

}
