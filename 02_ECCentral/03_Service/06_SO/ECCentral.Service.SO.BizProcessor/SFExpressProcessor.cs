using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SFExpressProcessor))]
    public class SFExpressProcessor
    {
        ISODA _SODA = ObjectFactory<ISODA>.Instance;
        ISOLogDA _SOLogDA = ObjectFactory<ISOLogDA>.Instance;

        /// <summary>
        /// 获取查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        public List<string> GetWaitingFinishShippingList()
        {
            return _SODA.GetWaitingFinishShippingList(ExpressType.SF);
        }

        /// <summary>
        /// 查询物流信息
        /// </summary>
        /// <param name="trackingNumberList"></param>
        public void QueryTracking(List<string> trackingNumberList)
        {
            SFExpressQueryDelegate _SFQueryTracking = new SFExpressQueryDelegate(SFExpressQuery);
            IAsyncResult ar = _SFQueryTracking.BeginInvoke(BuildQueryRequestXml(trackingNumberList), new AsyncCallback(QueryCompleted), _SFQueryTracking);
        }

        #region 异步使用顺丰接口查询
        /// <summary>
        /// 构造请求协议
        /// </summary>
        /// <param name="trackingNumberList"></param>
        /// <returns></returns>
        private string BuildQueryRequestXml(List<string> trackingNumberList)
        {
            string requestService = AppSettingManager.GetSetting("SO", "SFExpressRequestService");
            string requestLang = AppSettingManager.GetSetting("SO", "SFExpressRequestLang");
            string customerID = AppSettingManager.GetSetting("SO", "SFExpressCustomerID");
            string checkWord = AppSettingManager.GetSetting("SO", "SFExpressCheckWord");
            string trackingNumbers = "";
            foreach (string str in trackingNumberList)
            {
                trackingNumbers += string.Format("{0},", str);
            }
            trackingNumbers = trackingNumbers.TrimEnd(',');

            string requestXml = @"<Request service='#RequestService#' lang='#RequestLang#'>
<Head>#CustomerID#,#Checkword#</Head><Body>
<RouteRequest tracking_type='1' method_type='1' tracking_number='#TrackingNumbers#'/>
</Body></Request>";
            requestXml = requestXml.Replace("#RequestService#", requestService);
            requestXml = requestXml.Replace("#RequestLang#", requestLang);
            requestXml = requestXml.Replace("#CustomerID#", customerID);
            requestXml = requestXml.Replace("#Checkword#", checkWord);
            requestXml = requestXml.Replace("#TrackingNumbers#", trackingNumbers);

            return requestXml;
        }
        /// <summary>
        /// 解析返回协议
        /// </summary>
        /// <param name="responseXml"></param>
        /// <returns></returns>
        private List<SFQueryResponseRouteResponse> AnalysisResponseXml(string responseXml)
        {
            SFQueryResponse res = SerializationUtility.XmlDeserialize<SFQueryResponse>(responseXml);
            if (res == null)
            {
                Logger.WriteLog(string.Format("responseXml:{0}", responseXml), "顺丰订单物流信息查询返回NULL");
                return null;
            }
            if (res.Head.Equals("ERR"))
            {
                Logger.WriteLog(string.Format("responseXml:{0},Code:{1},ErrMsg:{2}", responseXml, res.Error.ErrorCode, res.Error.ErrorMsg), "顺丰订单物流信息查询返回错误");
                return null;
            }
            if (!res.Head.Equals("OK"))
            {
                Logger.WriteLog(string.Format("responseXml:{0}", responseXml), "顺丰订单物流信息查询返回不为OK");
                return null;
            }
            if (res.Body == null
                || res.Body.RouteResponse == null
                || res.Body.RouteResponse.Count == 0)
            {
                Logger.WriteLog(string.Format("responseXml:{0}", responseXml), "顺丰订单物流信息查询返回NULL");
                return null;
            }
            return res.Body.RouteResponse;
        }
        private delegate string SFExpressQueryDelegate(string requestXml);
        private string SFExpressQuery(string requestXml)
        {
            string host = AppSettingManager.GetSetting("SO", "SFExpressQueryHost");
            string action = AppSettingManager.GetSetting("SO", "SFExpressAction");
            WebServiceProxy wsd = new WebServiceProxy(host, action);

            object[] paramArray = { requestXml };

            return wsd.ExecuteQuery(action, paramArray).ToString();
        }
        /// <summary>
        /// 异步查询完成处理查询结果
        /// </summary>
        /// <param name="ar"></param>
        private void QueryCompleted(IAsyncResult ar)
        {
            SFExpressQueryDelegate _SFExpressQuery = ar.AsyncState as SFExpressQueryDelegate;
            string responseXml = _SFExpressQuery.EndInvoke(ar);
            
            List<SFQueryResponseRouteResponse> result = AnalysisResponseXml(responseXml);
            if (result != null)
            {
                foreach (SFQueryResponseRouteResponse item in result)
                {
                    if (item.RouteList != null)
                    {
                        //根据运单号获取订单号
                        int soSysNo = _SODA.GetSOSysNoByTrackingNumber(item.TrackingNumber);
                        if (soSysNo <= 0)
                            continue;
                        if (item.RouteList.Exists(m => m.Code.Equals("70")))
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
                        if (item.RouteList.Exists(m => m.Code.Equals("80")))
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
                        #region 更新SOLog
                        var routeList = EntityConverter<SFQueryResponseRoute, SOLogisticsInfo>.Convert(item.RouteList, (s, t) =>
                        {
                            t.Type = ExpressType.SF;
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
    }

    #region Request/Response Protocol
    [XmlRoot("Response")]
    public class SFQueryResponse
    {
        [XmlAttribute("service")]
        public string Service { get; set; }
        public string Head { get; set; }
        [XmlElement("ERROR")]
        public SFQueryResponseError Error { get; set; }
        [XmlElement("Body")]
        public SFQueryResponseBody Body { get; set; }
    }
    public class SFQueryResponseError
    {
        [XmlText]
        public string ErrorMsg { get; set; }
        [XmlAttribute("code")]
        public string ErrorCode { get; set; }
    }
    public class SFQueryResponseBody
    {
        [XmlElement("RouteResponse")]
        public List<SFQueryResponseRouteResponse> RouteResponse { get; set; }
    }
    public class SFQueryResponseRouteResponse
    {
        [XmlAttribute("mailno")]
        public string TrackingNumber { get; set; }
        [XmlElement("Route")]
        public List<SFQueryResponseRoute> RouteList { get; set; }
    }
    public class SFQueryResponseRoute
    {
        [XmlAttribute("opcode")]
        public string Code { get; set; }
        [XmlAttribute("remark")]
        public string Remark { get; set; }
        [XmlAttribute("accept_time")]
        public string AcceptTime { get; set; }
        [XmlAttribute("accept_address")]
        public string AcceptAddress { get; set; }
    }
    #endregion
}
