using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(YTExpressProcessor))]
    public class YTExpressProcessor
    {
        ISODA _SODA = ObjectFactory<ISODA>.Instance;
        ISOLogDA _SOLogDA = ObjectFactory<ISOLogDA>.Instance;

        /// <summary>
        /// 获取查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        public List<string> GetWaitingFinishShippingList()
        {
            return _SODA.GetWaitingFinishShippingList(ExpressType.YT);
        }

        /// <summary>
        /// 查询物流信息
        /// </summary>
        /// <param name="trackingNumberList"></param>
        public void QueryTracking(List<string> trackingNumberList)
        {
            YTExpressQuery(BuildQueryRequestXml(trackingNumberList));
        }

        #region 异步使用圆通接口查询
        /// <summary>
        /// 构造请求协议
        /// </summary>
        /// <param name="trackingNumberList"></param>
        /// <returns></returns>
        private string BuildQueryRequestXml(List<string> trackingNumberList)
        {
            string logisticProviderID = AppSettingManager.GetSetting("SO", "YTExpressLogisticProviderID");
            string clientID = AppSettingManager.GetSetting("SO", "YTExpressClientID");
            string trackingNumbers = "";
            foreach (string str in trackingNumberList)
            {
                trackingNumbers += string.Format("<order><mailNo>{0}</mailNo></order>", str);
            }

            string requestXml = @"<BatchQueryRequest>
    <logisticProviderID>#LogisticProviderID#</logisticProviderID>
    <clientID>#ClientID#</clientID>
    <orders>#TrackingNumbers#</orders>
</BatchQueryRequest>";
            requestXml = requestXml.Replace("#LogisticProviderID#", logisticProviderID);
            requestXml = requestXml.Replace("#ClientID#", clientID);
            requestXml = requestXml.Replace("#TrackingNumbers#", trackingNumbers);

            return requestXml;
        }
        /// <summary>
        /// 解析返回协议
        /// </summary>
        /// <param name="responseXml"></param>
        /// <returns></returns>
        private YTBatchQueryResponse AnalysisResponseXml(string responseXml)
        {
            YTBatchQueryResponse result = null;
            try
            {
                var t = SerializationUtility.XmlDeserialize<YTResponse>(responseXml);
                result = EntityConverter<YTResponse, YTBatchQueryResponse>.Convert(t);
                if (!t.success)
                {
                    Logger.WriteLog(string.Format("responseXml:{0},Code:{1},ErrMsg:{2}", responseXml, t.reason, t.reason), "圆通订单物流信息查询返回错误");
                    return null;
                }
            }
            catch (Exception ex)
            { }

            if (result == null)
            {
                result = SerializationUtility.XmlDeserialize<YTBatchQueryResponse>(responseXml);
            }

            if (result == null)
            {
                Logger.WriteLog(string.Format("responseXml:{0}", responseXml), "圆通订单物流信息查询返回NULL");
                return null;
            }
            if (result.orders == null
                || result.orders.Count == 0)
            {
                Logger.WriteLog(string.Format("responseXml:{0}", responseXml), "圆通订单物流信息查询返回NULL");
                return null;
            }
            return result;
        }
        private void YTExpressQuery(string requestXml)
        {
            string postBody = string.Format("logistics_interface={0}&data_digest={1}&clientId={2}"
                , HttpUtility.UrlEncode(requestXml, Encoding.UTF8)
                , HttpUtility.UrlEncode(Sign(requestXml + AppSettingManager.GetSetting("SO", "YTExpressParternID")), Encoding.UTF8)
                , HttpUtility.UrlEncode(AppSettingManager.GetSetting("SO", "YTExpressClientID"), Encoding.UTF8)
                );
            string host = AppSettingManager.GetSetting("SO", "YTExpressQueryHost");
            HttpWebRequest request = WebRequest.Create(host) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            byte[] postData = Encoding.UTF8.GetBytes(postBody);
            request.ContentLength = postData.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postData, 0, postData.Length);
            stream.Close();
            request.BeginGetResponse(new AsyncCallback(QueryCompleted), request);

        }
        /// <summary>
        /// 异步查询完成处理查询结果
        /// </summary>
        /// <param name="ar"></param>
        private void QueryCompleted(IAsyncResult ar)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream streamResponse = null;
            StreamReader streamRead = null;
            string responseXml = null;
            try
            {
                request = (HttpWebRequest)ar.AsyncState;
                response = (HttpWebResponse)request.EndGetResponse(ar);
                streamResponse = response.GetResponseStream();
                streamRead = new StreamReader(streamResponse, Encoding.UTF8);
                responseXml = streamRead.ReadToEnd();
            }
            catch (Exception ex)
            {
                //
            }
            finally
            {
                if (streamRead != null)
                {
                    streamRead.Close();
                }
                if (streamResponse != null)
                {
                    streamResponse.Close();
                }
            }
            //check sign

            YTBatchQueryResponse result = AnalysisResponseXml(responseXml);
            if (result != null && result.orders != null && result.orders.Count > 0)
            {
                foreach (YTOrder item in result.orders)
                {
                    //根据运单号获取订单号
                    int soSysNo = _SODA.GetSOSysNoByTrackingNumber(item.mailNo);
                    if (soSysNo <= 0)
                        continue;
                    if (item.orderStatus == YTOrderStatus.UNACCEPT
                        || item.orderStatus == YTOrderStatus.NOT_SEND
                        || item.orderStatus == YTOrderStatus.FAILED)
                    {
                        //物流派件不成功，订单更新为物流派件不成功
                        SOStatusChangeInfo soStatusChangeInfo = new SOStatusChangeInfo()
                        {
                            SOSysNo = soSysNo,
                            OperatorType = SOOperatorType.System,
                            OperatorSysNo = 0,
                            Status = SOStatus.ShippingReject,
                            ChangeTime = DateTime.Now,
                            IsSendMailToCustomer = false,
                            Note = "物流派件不成功"
                        };
                        _SODA.UpdateSOStatus(soStatusChangeInfo);
                    }
                    if (item.orderStatus == YTOrderStatus.SIGNED)
                    {
                        //已收货，订单更新为已完成
                        SOStatusChangeInfo soStatusChangeInfo = new SOStatusChangeInfo()
                        {
                            SOSysNo = soSysNo,
                            OperatorType = SOOperatorType.System,
                            OperatorSysNo = 0,
                            Status = SOStatus.Complete,
                            ChangeTime = DateTime.Now,
                            IsSendMailToCustomer = false,
                            Note = "物流已完成"
                        };
                        _SODA.UpdateSOStatus(soStatusChangeInfo);

                        //已收货，更新SO_CheckShipping表的LastChangeStatusDate字段，以便同步至WMS
                        _SODA.UpdateSOCheckShippingLastChangeStatusDate(soSysNo);

                        //已收货，检查并赠送优惠券
                        var soInfo = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNo(soSysNo);
                        ObjectFactory<IMKTBizInteract>.Instance.CheckAndGivingPromotionCodeForSO(soInfo);
                    }
                    if (item.steps != null && item.steps.Count > 0)
                    {
                        #region 更新SOLog
                        var routeList = EntityConverter<YTStep, SOLogisticsInfo>.Convert(item.steps, (s, t) =>
                        {
                            t.AcceptAddress = s.acceptAddress;
                            t.AcceptTime = s.acceptTime;
                            t.Name = s.name;
                            t.Status = s.status;
                            t.Type = ExpressType.YT;
                        });
                        string routeLogMsg = SerializationUtility.XmlSerialize(routeList);
                        var logList = _SOLogDA.GetSOLogBySOSysNoAndLogType(soSysNo, BizLogType.Sale_SO_ShippingInfo);
                        bool bIsCreate = logList == null || logList.Count == 0;
                        if (bIsCreate)
                        {
                            //创建
                            SOLogInfo soLog = new SOLogInfo()
                            {
                                SOSysNo = soSysNo,
                                IP = "::1",
                                OperationType = BizLogType.Sale_SO_ShippingInfo,
                                Note = routeLogMsg,
                                UserSysNo = 0,
                                CompanyCode = "8601"
                            };
                            _SOLogDA.InsertSOLog(soLog);
                        }
                        else
                        {
                            //更新
                            SOLogInfo soLog = logList[0];
                            soLog.Note = routeLogMsg;
                            _SOLogDA.UpdateSOLogNoteBySysNo(soLog);
                        }
                        #endregion
                    }
                }
            }
        }
        #endregion

        #region 签名
        /// <summary>
        /// 签名
        /// 1、MD5Bytes
        /// 2、Base64
        /// </summary>
        /// <param name="originData"></param>
        /// <returns></returns>
        private string Sign(string originData)
        {
            byte[] bytes = null;
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(originData));
            }
            return Convert.ToBase64String(bytes);
        }
        #endregion
    }
    #region Request/Response Protocol
    [XmlRoot("Response")]
    public class YTResponse
    {
        /// <summary>
        /// 物流公司编号 
        /// </summary>
        public string logisticProviderID { get; set; }
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string reason { get; set; }
    }

    [XmlRoot("BatchQueryResponse")]
    public class YTBatchQueryResponse
    {
        public YTBatchQueryResponse()
        {
            this.orders = new List<YTOrder>();
        }
        /// <summary>
        /// 物流公司编号 
        /// </summary>
        public string logisticProviderID { get; set; }
        /// <summary>
        /// 查询到的订单列表
        /// </summary>
        [XmlArray(ElementName="orders")]
        [XmlArrayItem("order", typeof(YTOrder))]
        public List<YTOrder> orders { get; set; } 
    }

    public class YTOrder
    {
        public YTOrder()
        {
            this.steps = new List<YTStep>();
        }
        /// <summary>
        /// 邮件号
        /// </summary>
        public string mailNo { get; set; }
        /// <summary>
        /// 邮件类型
        /// </summary>
        public string mailType { get; set; }
        /// <summary>
        /// 当前订单状态
        /// </summary>
        //public string orderStatus { get; set; }
        public YTOrderStatus orderStatus { get; set; }
        /// <summary>
        /// 物流流程列表
        /// </summary>
        [XmlArray(ElementName = "steps")]
        [XmlArrayItem("step", typeof(YTStep))]
        public List<YTStep> steps { get; set; }
    }

    public class YTStep
    {
        /// <summary>
        /// 接受时间
        /// </summary>
        public string acceptTime { get; set; }
        /// <summary>
        /// 接受地点
        /// </summary>
        public string acceptAddress { get; set; }
        /// <summary>
        /// 接受人
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }

    /// <summary>
    /// 物流状态
    /// </summary>
    public enum YTOrderStatus
    {
        /// <summary>
        /// 等待确认
        /// </summary>
        CONFIRM,
        /// <summary>
        /// 接单
        /// </summary>
        ACCEPT,
        /// <summary>
        /// 不接单
        /// </summary>
        UNACCEPT,
        /// <summary>
        /// 揽收成功
        /// </summary>
        GOT,
        /// <summary>
        /// 揽收失败
        /// </summary>
        NOT_SEND,
        /// <summary>
        /// 运输中
        /// </summary>
        TRANSIT,
        /// <summary>
        /// 派件扫描
        /// </summary>
        SENT_SCAN,
        /// <summary>
        /// 流转信息
        /// </summary>
        TRACKING,
        /// <summary>
        /// 签收成功
        /// </summary>
        SIGNED,
        /// <summary>
        /// 签收失败
        /// </summary>
        FAILED,
        /// <summary>
        /// 订单已取消
        /// </summary>
        WITHDRAW,
    }
    #endregion
}
