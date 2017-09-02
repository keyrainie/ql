using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.Net;
using ECCentral.Service.ThirdPart.Interface;
using ECCentral.Service.Utility;

namespace ECCentral.Service.ThirdPart.Adapter
{
    [VersionExport(typeof(ITaoBaoAPI))]
    public class TaoBaoProcessor : ITaoBaoAPI
    {


        private static string tbURL = ConfigurationManager.AppSettings["TaoBaoURL"];

        /// <summary>
        /// 获取订单编号增量
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="useHasNext"></param>
        /// <returns></returns>
        public string GetIncrementOrderIDs(string startTime, string endTime, string status, string pageNo, string pageSize, string useHasNext)
        {
            string returnvalue = string.Empty;
            IDictionary<string, string> param = new Dictionary<string, string>();

            SetCommonParam(param);
            param.Add("method", "taobao.trades.sold.increment.get");
            param.Add("fields", "tid");
            param.Add("start_modified", startTime);//查询修改开始时间(修改时间跨度不能大于一天)。格式:yyyy-MM-dd HH:mm:ss 
            param.Add("end_modified", endTime);

            if (!string.IsNullOrEmpty(status))
                param.Add("status", status); // "WAIT_SELLER_SEND_GOODS" 
            if (!string.IsNullOrEmpty(pageNo))
                param.Add("page_no", pageNo);//页码。取值范围:大于零的整数;默认值:1 
            if (!string.IsNullOrEmpty(pageSize))
                param.Add("page_size", pageSize);//每页条数。   
            if (!string.IsNullOrEmpty(useHasNext))
                param.Add("use_has_next", useHasNext); //是否启用has_next的分页方式。 

            byte[] reqStream = Encoding.UTF8.GetBytes(PostData(param));

            returnvalue = HttpRequestHelper.GetResponse(RequestMethod.POST
                , "application/x-www-form-urlencoded;charset=utf-8"
                , tbURL
                , GetWebProxy()
                , reqStream);

            return returnvalue;
        }

        /// <summary>
        /// 获取订单详细信息
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public string GetOrderDetails(string orderID)
        {
            string returnValue = string.Empty;

            IDictionary<string, string> param = new Dictionary<string, string>();

            StringBuilder sb = new StringBuilder();
            sb.Append("tid");
            sb.Append(",buyer_nick");
            sb.Append(",buyer_message");
            sb.Append(",buyer_email");
            sb.Append(",receiver_state");
            sb.Append(",receiver_city");
            sb.Append(",receiver_district");
            sb.Append(",receiver_address");
            sb.Append(",receiver_mobile");
            sb.Append(",receiver_phone");
            sb.Append(",receiver_name");
            sb.Append(",receiver_zip");
            sb.Append(",received_payment");
            sb.Append(",post_fee");
            sb.Append(",discount_fee");
            sb.Append(",payment");
            sb.Append(",orders.num_iid");
            sb.Append(",orders.total_fee");
            sb.Append(",orders.discount_fee");
            sb.Append(",orders.adjust_fee");
            sb.Append(",orders.refund_status");
            sb.Append(",orders.num");
            sb.Append(",promotion_details");
            string fields = sb.ToString();

            SetCommonParam(param);
            param.Add("fields", fields);
            param.Add("tid", orderID);

            byte[] reqStream = Encoding.UTF8.GetBytes(PostData(param));

            //调用淘宝服务
            returnValue = HttpRequestHelper.GetResponse(RequestMethod.POST
                , "application/x-www-form-urlencoded;charset=utf-8"
                , tbURL
                , GetWebProxy()
                , reqStream);

            //WriteLog(string.Format("调用方:{0}| 参数（fields:{1}）|参数（tid:{2}）|返回信息:{3}", "Sycn3PartSO_Job", fields, orderID, returnValue), "淘宝--获取单笔交易数据--返回日志");
            return returnValue;
        }


        public string OfflineShip(string tID, string OutSID, string companyCode, string senderID, string cancelID)
        {
            throw new NotImplementedException();
        }


        private static void SetCommonParam(IDictionary<string, string> param)
        {

            string session = ConfigurationManager.AppSettings["TaoBaoSession"];
            param.Add("app_key", ConfigurationManager.AppSettings["TaoBaoAppKey"]);

            param.Add("session", session);//他用类型 且是获取私有数据的需要传递 sessionkey
            param.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            param.Add("format", "xml");
            param.Add("v", "2.0");
            param.Add("sign_method", "md5");
            param.Add("sign", CreateSign(param, ConfigurationManager.AppSettings["TaoBaoAppSecret"]));

        }

        private static string CreateSign(IDictionary<string, string> parameters, string secret)
        {
            parameters.Remove("sign");

            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            StringBuilder query = new StringBuilder(secret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }

            query.Append(secret);

            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                string hex = bytes[i].ToString("X");
                if (hex.Length == 1)
                {
                    result.Append("0");
                }
                result.Append(hex);
            }

            return result.ToString();
        }

        private static WebProxy GetWebProxy()
        {
            bool isUseProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["TaoBaoIsUseProxy"]);

            if (isUseProxy)
            {
                WebProxy proxy = new WebProxy();
                proxy.Address = new Uri(ConfigurationManager.AppSettings["ProxyUrl"].ToString());
                string username = ConfigurationManager.AppSettings["ProxyUserID"].ToString();
                string password = ConfigurationManager.AppSettings["ProxyPassword"].ToString();

                proxy.Credentials = new System.Net.NetworkCredential(username, password);
                return proxy;
            }
            else
                return null;
        }

        protected static string PostData(IDictionary<string, string> parameters)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(Uri.EscapeDataString(value));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

      
        
    }



}
