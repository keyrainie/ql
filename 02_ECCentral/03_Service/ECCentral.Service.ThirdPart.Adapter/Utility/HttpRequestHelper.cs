using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace ECCentral.Service.ThirdPart.Adapter
{
    public class HttpRequestHelper
    {
        public static string GetResponse(RequestMethod method, string contentType, string url, WebProxy proxy, byte[] data)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = method.ToString();
            req.KeepAlive = true;
            req.Timeout = 300000;
            req.ContentType = contentType;
            req.Proxy = proxy;

            Stream reqStream = req.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
            Encoding encoding = Encoding.GetEncoding(rep.CharacterSet);

            Stream receiveStream = rep.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, encoding);

            string result = string.Empty;
            result = reader.ReadToEnd();
            if (reader != null) reader.Close();
            if (receiveStream != null) receiveStream.Close();
            if (rep != null) rep.Close();

            return result;
        }
    }

    public enum RequestMethod
    {
        Get, 
        POST
    }
}
