using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.IO;
using System.Xml.Serialization;

namespace SendMessage.Class
{
    public class SmsServiceHttp
    {
        public Result Submit(string CellNumber, string SMSContent, string guid)
        {
            Encoding encoding = Encoding.UTF8;
            string responseData = String.Empty;
            String url = ConfigurationManager.AppSettings["HttpSMSServiceUrl"];

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:32.0) Gecko/20100101 Firefox/32.0";

            string param = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            @"<service>
                <SYS_HEAD>
                    <SERVICE_CODE>11002000003</SERVICE_CODE>
                    <SERVICE_SCENE>01</SERVICE_SCENE>
                    <CONSUMER_SEQ_NO>{0}</CONSUMER_SEQ_NO>
                    <TRAN_DATE>{1}</TRAN_DATE>
                    <TRAN_TIMESTAMP>{2}</TRAN_TIMESTAMP>
                    <CONSUMER_ID>{3}</CONSUMER_ID>
                    <PROVIDER_ID>{4}</PROVIDER_ID>
                </SYS_HEAD>
                <APP_HEAD>
                    <BUSS_SEQ_NO>{5}</BUSS_SEQ_NO>
                </APP_HEAD>
                <BODY>
                    <ACCT_NO>{6}</ACCT_NO>
                    <SERVICE_ID>{7}</SERVICE_ID>
                    <PHONE_NO>{8}</PHONE_NO>
                    <MSG_CONTEXT>{9}</MSG_CONTEXT>
                    <REMARK>{10}</REMARK>
                    <PROVIDER_ID>{11}</PROVIDER_ID>
                </BODY>
            </service>";

            param = string.Format(param,
                guid,                                               //CONSUMER_SEQ_NO
                DateTime.Now.ToString("yyyyMMdd"),                  //TRAN_DATE
                DateTime.Now.ToString("hhmmss"),                    //TRAN_TIMESTAMP
                ConfigurationManager.AppSettings["CONSUMER_ID"],    //CONSUMER_ID
                ConfigurationManager.AppSettings["PROVIDER_ID"],    //PROVIDER_ID
                guid,                                               //BUSS_SEQ_NO
                ConfigurationManager.AppSettings["ACCT_NO"],        //ACCT_NO
                ConfigurationManager.AppSettings["SERVICE_ID"],     //SERVICE_ID
                CellNumber,                                         //PHONE_NO
                SMSContent,                                         //MSG_CONTEXT
                ConfigurationManager.AppSettings["REMARK"],         //REMARK
                ConfigurationManager.AppSettings["PROVIDER_ID"]     //PROVIDER_ID
                );

            //int len = param.Length + 200;
            //param = len.ToString().PadLeft(8, '0') + param;
            byte[] bs = Encoding.UTF8.GetBytes(param);

            int len = bs.Length + 8;
            param = len.ToString().PadLeft(8, '0') + param;
            bs = Encoding.UTF8.GetBytes(param);

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
            string str = reader.ReadToEnd().ToString();
            if (str.Length > 8)
            {
                str = str.Substring(8);
                bs = Encoding.UTF8.GetBytes(str);
                XmlSerializer serializer = new XmlSerializer(typeof(Result));
                try
                {
                    Result r = (Result)serializer.Deserialize(new MemoryStream(bs));
                    return r;
                }
                catch (Exception e)
                {
                    return null;
                }
                
            }
            else
            {
                Result r = new Result();
                r.SYS_HEAD = new Result_SYS_HEAD();
                r.SYS_HEAD.RET = new Result_RET();
                r.SYS_HEAD.RET.CODE = "error";
                return r;
            }
        }
    }

    [XmlRoot("SERVICE")]
    public class Result
    {
        [XmlElement("SYS_HEAD")]
        public Result_SYS_HEAD SYS_HEAD
        {
            get;
            set;
        }

        [XmlElement("APP_HEAD")]
        public Result_APP_HEAD APP_HEAD
        {
            get;
            set;
        }
    }

    [XmlRoot("APP_HEAD")]
    public class Result_APP_HEAD
    {
        [XmlElement("BUSS_SEQ_NO")]
        public string BUSS_SEQ_NO
        {
            get;
            set;
        }
    }

    [XmlRoot("SYS_HEAD")]
    public class Result_SYS_HEAD
    {
        [XmlElement("RET")]
        public Result_RET RET
        {
            get;
            set;
        }
        [XmlElement("ESB_SEQ_NO")]
        public string ESB_SEQ_NO
        {
            get;
            set;
        }
        [XmlElement("PROVIDER_ID")]
        public string PROVIDER_ID
        {
            get;
            set;
        }
        [XmlElement("CONSUMER_SEQ_NO")]
        public string CONSUMER_SEQ_NO
        {
            get;
            set;
        }
        [XmlElement("RET_STATUS")]
        public string RET_STATUS
        {
            get;
            set;
        }
        [XmlElement("SERVICE_SCENE")]
        public string SERVICE_SCENE
        {
            get;
            set;
        }
        [XmlElement("SERVICE_CODE")]
        public string SERVICE_CODE
        {
            get;
            set;
        }
        [XmlElement("CONSUMER_ID")]
        public string CONSUMER_ID
        {
            get;
            set;
        }
    }

    [XmlRoot("RET")]
    public class Result_RET
    {
        [XmlElement("RET_MSG")]
        public string MSG
        {
            get;
            set;
        }
        [XmlElement("RET_CODE")]
        public string CODE
        {
            get;
            set;
        }
    }
}
