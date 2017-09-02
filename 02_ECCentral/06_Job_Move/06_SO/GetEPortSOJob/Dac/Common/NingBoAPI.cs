using GetEPortSOJob.BusinessEntities;
using GetEPortSOJob.Utilities;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GetEPortSOJob.Dac.Common
{
    public class NingBoAPI
    {
        /// <summary>
        /// 宁波跨境贸易电子商务平台
        /// 申报单状态查询（根据更新时间）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Message GetSOAndUpdateStatus(JobContext context, int index)
        {
            Message messageinfo = new Message();
            Header Head = new Header();
            Body bodys = new Body();
            //电商企业代码
            string CustomsCode = System.Configuration.ConfigurationManager.AppSettings["CustomsCode"];
            //电商企业名称
            string OrgName = System.Configuration.ConfigurationManager.AppSettings["OrgName"];
            //APIUrl
            string APIQA = System.Configuration.ConfigurationManager.AppSettings["APIQA"];
            string APIPRD = System.Configuration.ConfigurationManager.AppSettings["APIPRD"];
            //账号
            string userid = System.Configuration.ConfigurationManager.AppSettings["userid"];
            //密钥
            string PassWord = System.Configuration.ConfigurationManager.AppSettings["PassWord"];
            //API类型
            string msgtype = System.Configuration.ConfigurationManager.AppSettings["msgtype"];
            //关区代码
            string customs = System.Configuration.ConfigurationManager.AppSettings["customs"];
            DateTime StartTime = DateTime.Now;
            DateTime EndTime = DateTime.Now.AddDays(-7);
            context.Message += string.Format("{0}宁波跨境开始查询订单的申报状态订单,时间区间{1}至{2}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "), StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff "), EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff "));
            #region API
            StringBuilder XMLBuilder = new StringBuilder();
            XMLBuilder.Append("<?xml version='1.0' encoding='UTF-8' ?>");
            XMLBuilder.Append("<Message><Header>");
            XMLBuilder.Append("<CustomsCode>" + CustomsCode + "</CustomsCode>");
            XMLBuilder.Append("<OrgName>" + OrgName + "</OrgName>");
            //开始时间和截至时间，请根据实际业务量来设定，时间跨度不能大于7天。
            XMLBuilder.Append("<StartTime>" + StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "</StartTime>");
            XMLBuilder.Append("<EndTime>" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "</EndTime>");
            XMLBuilder.Append("<Page>" + index + "</Page>");//指定查询页码(1/2/3...)，从1开始计算，每页1000条纪录
            XMLBuilder.Append("</Header>");
            XMLBuilder.Append("</Message>");

            
            Uri address = new Uri(APIQA);

            // Create the web request
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            // Add authentication to request
            //request.Credentials = new NetworkCredential(userid, PassWord);
            // Set type to POST
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            // Create the data we want to send
            string Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string timestamp = System.Web.HttpUtility.UrlEncode(Date, Encoding.UTF8);

            string sign = ECCentral.Service.Utility.Hash_MD5.GetMD5(userid + PassWord + Date);
            string xmlstr = System.Web.HttpUtility.UrlEncode(XMLBuilder.ToString());
            StringBuilder data = new StringBuilder();
            data.Append("userid=" + userid);
            data.Append("&timestamp=" + timestamp);
            data.Append("&sign=" + sign);
            data.Append("&xmlstr=" + xmlstr);
            data.Append("&msgtype=" + msgtype);
            data.Append("&customs=" + customs);
            // Create a byte array of the data we want to send
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());
            // Set the content length in the request headers
            request.ContentLength = byteData.Length;
            // Write data
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }
            // Get response
            string strBuff = "";
            char[] cbuffer = new char[256];
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                try
                {
                    // Get the response stream
                    #region 获取网页数据
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    // Console application output
                    //Console.WriteLine(reader.ReadToEnd());
                    int byteRead = reader.Read(cbuffer, 0, 256);
                    while (byteRead != 0)
                    {
                        string strResp = new string(cbuffer, 0, byteRead);
                        strBuff = strBuff + strResp;
                        byteRead = reader.Read(cbuffer, 0, 256);
                    }
                    Console.WriteLine(strBuff);
                    #endregion
                    XDocument doc = XDocument.Parse(strBuff);
                    XElement root = doc.Root;
                    XElement Header = root.Element("Header");
                    //T：操作成功；F：操作失败
                    string Result = GetElementValue(Header, "Result");
                    //结果描述（操作失败时必需）
                    string ResultMsg = GetElementValue(Header, "ResultMsg");
                    //是否存在下一页（T:是；F：否）
                    string NextPage = GetElementValue(Header, "NextPage");
                    Head = XmlUtil.Deserialize(typeof(Header), Header.ToString()) as Header;
                    if (Head.Result == "T")
                    {
                        XElement Mfts = root.Element("Mft");
                        List<Mft> MftList = XmlUtil.Deserialize(typeof(List<Mft>), Mfts.ToString()) as List<Mft>;
                        bodys.MftList = MftList;
                        List<Mft> MftListAgree = MftList.FindAll(t => t.Status == "24" || t.Status == "99");
                        context.Message += string.Format("\r\n总共查到{0}订单\r\n", MftList.Count());
                        context.Message += string.Format("\r\n其中{0}订单海关货物放行\r\n", MftListAgree.Count());
                        if (MftListAgree.Count > 0)
                        {
                            foreach (var item in MftListAgree)
                            {
                                CommonDA.UpdateOrderStatus(int.Parse(item.OrderNo));
                            }
                        }
                    }
                    context.Message += strBuff;
                }
                catch (Exception ex)
                {
                    context.Message += ex.Message;
                }
            }
            #endregion
            context.Message += string.Format("\r\n{0}宁波跨境查询订单的申报状态订单结束\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));
            Log.WriteLog(context.Message, "log.txt");
            messageinfo.header = Head;
            messageinfo.body = bodys;
            return messageinfo;
        }
        /// <summary>
        /// 获取子节点值
        /// </summary>
        /// <param name="parentElement">根节点</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        private static string GetElementValue(XElement parentElement, string key)
        {
            if (parentElement != null && !string.IsNullOrEmpty(key))
            {
                XElement element = parentElement.Element(key.Trim());
                if (element != null)
                    return element.Value;
            }

            return string.Empty;
        }

    }
}
