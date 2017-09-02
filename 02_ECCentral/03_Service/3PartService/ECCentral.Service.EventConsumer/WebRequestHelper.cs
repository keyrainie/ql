using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using System.Web;

namespace ECCentral.Service.EventConsumer
{
    public class WebRequestHelper
    {
        public static string GetResponse(string request, Encoding encode)
        {
            string rspXML = null;

            WebClient client = new WebClient();
            bool isUseProxy = AppSettingHelper.IsUseProxy;

            if (isUseProxy)
            {
                WebProxy proxy = new WebProxy();
                proxy.Address = new Uri(AppSettingHelper.ProxyUrl);

                string username = AppSettingHelper.ProxyUserID;
                string password = AppSettingHelper.ProxyPassword;

                proxy.Credentials = new System.Net.NetworkCredential(username, password);
                client.Proxy = proxy;
            }

            using (Stream stream = client.OpenRead(request))
            {
                StreamReader reader = new StreamReader(stream, encode);
                rspXML = reader.ReadToEnd();
            }

            return rspXML;
        }
    }
}
