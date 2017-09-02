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
using System.Text;
using System.Xml.Serialization;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(KD100Processor))]
    public class KD100Processor
    {
        ISODA _SODA = ObjectFactory<ISODA>.Instance;
        ISOLogDA _SOLogDA = ObjectFactory<ISOLogDA>.Instance;

        /// <summary>
        /// 获取查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        public List<string> GetWaitingFinishShippingList()
        {
            return _SODA.GetWaitingFinishShippingListForKD100(ExpressType.KD100);
        }

        /// <summary>
        /// 查询物流信息
        /// </summary>
        /// <param name="trackingNumberList"></param>
        public void QueryTracking(List<string> trackingNumberList)
        {
            for (int i = 0; i < trackingNumberList.Count; i++)
            {
                var temp = trackingNumberList[i].Split(',');
                ExecuteQuery(temp[0], temp[1]);
            }
        }

        private string ExecuteQuery(string number, string typeCom)
        {
            if (typeCom.Equals("1"))//顺丰速递
            {
                typeCom = "shunfeng";
            }
            if (typeCom.Equals("2"))//圆通速递
            {
                typeCom = "yuantong";
            }
            if (typeCom.Equals("3"))//申通快递
            {
                typeCom = "shentong";
            }
            if (typeCom.Equals("4"))//中通速递
            {
                typeCom = "zhongtong";
            }
            if (typeCom.Equals("5"))//百世汇通
            {
                typeCom = "huitongkuaidi";
            }
            if (typeCom.Equals("6"))//韵达快递
            {
                typeCom = "yunda";
            }

            string apiurl = AppSettingManager.GetSetting("SO", "KuaiDi100QueryApiurl");
            string ApiKey = AppSettingManager.GetSetting("SO", "KuaiDi100QueryApiKey");

            apiurl = string.Format(apiurl, ApiKey, typeCom, number);

            WebRequest request = WebRequest.Create(apiurl);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = Encoding.UTF8;
            StreamReader reader = new StreamReader(stream, encode);
            string detail = reader.ReadToEnd();

            QueryCompleted(detail, number);

            return detail;
        }

        /// <summary>
        /// 异步查询完成处理查询结果
        /// </summary>
        /// <param name="ar"></param>
        private void QueryCompleted(string responseStr, string number)
        {
            #region old function
            //KD100QueryResponse result = AnalysisResponseXml(responseStr);
            //if (result != null)
            //{
            //    //根据运单号获取订单号
            //    int soSysNo = _SODA.GetSOSysNoByTrackingNumber(result.nu);
            //    if (soSysNo <= 0)
            //        return;
            //    if (result.state == 2 || result.state == 4 || result.state == 6)
            //    {
            //        //物流派件不成功，订单更新为物流派件不成功
            //        SOStatusChangeInfo soStatusChangeInfo = new SOStatusChangeInfo()
            //        {
            //            SOSysNo = soSysNo,
            //            OperatorType = SOOperatorType.System,
            //            OperatorSysNo = 0,
            //            Status = SOStatus.ShippingReject,
            //            ChangeTime = DateTime.Now,
            //            IsSendMailToCustomer = false,
            //            Note = "物流派件不成功"
            //        };
            //        _SODA.UpdateSOStatus(soStatusChangeInfo);
            //    }
            //    if (result.state == 3)
            //    {
            //        //已收货，订单更新为已完成
            //        SOStatusChangeInfo soStatusChangeInfo = new SOStatusChangeInfo()
            //        {
            //            SOSysNo = soSysNo,
            //            OperatorType = SOOperatorType.System,
            //            OperatorSysNo = 0,
            //            Status = SOStatus.Complete,
            //            ChangeTime = DateTime.Now,
            //            IsSendMailToCustomer = false,
            //            Note = "物流已完成"
            //        };
            //        _SODA.UpdateSOStatus(soStatusChangeInfo);

            //        //已收货，更新SO_CheckShipping表的LastChangeStatusDate字段，以便同步至WMS
            //        _SODA.UpdateSOCheckShippingLastChangeStatusDate(soSysNo);

            //        //已收货，检查并赠送优惠券
            //        var soInfo = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNo(soSysNo);
            //        ObjectFactory<IMKTBizInteract>.Instance.CheckAndGivingPromotionCodeForSO(soInfo);
            //    }
            //    #region 更新SOLog
            //    var routeList = ConverterSOLog(result.dataList);
            //    string routeLogMsg = SerializationUtility.XmlSerialize(routeList);
            //    var logList = _SOLogDA.GetSOLogBySOSysNoAndLogType(soSysNo, BizLogType.Sale_SO_ShippingInfo);
            //    bool bIsCreate = logList == null || logList.Count == 0;
            //    if (bIsCreate)
            //    {
            //        //创建
            //        SOLogInfo soLog = new SOLogInfo()
            //        {
            //            SOSysNo = soSysNo,
            //            IP = "::1",
            //            OperationType = BizLogType.Sale_SO_ShippingInfo,
            //            Note = routeLogMsg,
            //            UserSysNo = 0,
            //            CompanyCode = "8601"
            //        };
            //        _SOLogDA.InsertSOLog(soLog);
            //    }
            //    else
            //    {
            //        //更新
            //        SOLogInfo soLog = logList[0];
            //        soLog.Note = routeLogMsg;
            //        _SOLogDA.UpdateSOLogNoteBySysNo(soLog);
            //    }
            //    #endregion
            //}
            #endregion
            if (!string.IsNullOrEmpty(responseStr))
            {
                //根据运单号获取订单号
                int soSysNo = _SODA.GetSOSysNoByTrackingNumber(number);
                if (soSysNo <= 0) return;
                 
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
                        Note = responseStr,
                        UserSysNo = 0,
                        CompanyCode = "8601"
                    };
                    _SOLogDA.InsertSOLog(soLog);
                }
                else
                {
                    //更新
                    SOLogInfo soLog = logList[0];
                    soLog.Note = responseStr;
                    _SOLogDA.UpdateSOLogNoteBySysNo(soLog);
                }
            }
        }

        private List<SOLogisticsInfo> ConverterSOLog(List<KD100QueryResponsedata> dataList)
        {
            List<SOLogisticsInfo> routeList = new List<SOLogisticsInfo>();
            if (dataList != null && dataList.Count > 0)
            {
                foreach (var data in dataList)
                {
                    routeList.Add(new SOLogisticsInfo()
                    {
                        Type = ExpressType.KD100,
                        AcceptTime = data.time,
                        Remark = data.context,
                        AcceptAddress = "", //无用字段
                        Code = "", //无用字段
                        Name = "", //无用字段
                        Status = false //无用字段
                    });
                }
            }
            return routeList;
        }

        /// <summary>
        /// 解析返回协议
        /// </summary>
        /// <param name="responseXml"></param>
        /// <returns></returns>
        private KD100QueryResponse AnalysisResponseXml(string responseXml)
        {
            KD100QueryResponse res = SerializationUtility.XmlDeserialize<KD100QueryResponse>(responseXml);
            if (res == null)
            {
                Logger.WriteLog(string.Format("responseXml:{0}", responseXml), "快递100订单物流信息查询返回NULL");
                return null;
            }
            if (res.status == 0 || res.dataList == null
                || res.dataList.Count == 0)
            {
                Logger.WriteLog(string.Format("responseXml:{0}", responseXml), "快递100订单物流信息查询，物流单暂无结果");
                return null;
            }
            if (res.status == 2)
            {
                Logger.WriteLog(string.Format("responseXml:{0}", responseXml), "快递100订单物流信息查询，接口出现异常");
                return null;
            }

            return res;
        }

    }

    [XmlRoot("xml")]
    public class KD100QueryResponse
    {
        /// <summary>
        /// 物流单号
        /// </summary>
        [XmlElement("nu")]
        public string nu { get; set; }

        /// <summary>
        /// 查询结果状态：0：物流单暂无结果，1：查询成功， 2：接口出现异常，
        /// </summary>
        [XmlElement("status")]
        public int status { get; set; }

        /// <summary>
        /// 物流公司编号
        /// </summary>
        [XmlElement("com")]
        public string com { get; set; }


        //[XmlArrayItem("data")]
        [XmlElement("data")]
        public List<KD100QueryResponsedata> dataList { get; set; }

        /// <summary>
        /// 快递单当前的状态 ：
        /// 0：在途，即货物处于运输过程中；
        /// 1：揽件，货物已由快递公司揽收并且产生了第一条跟踪信息；
        /// 2：疑难，货物寄送过程出了问题；
        /// 3：签收，收件人已签收；
        /// 4：退签，即货物由于用户拒签、超区等原因退回，而且发件人已经签收；
        /// 5：派件，即快递正在进行同城派件；
        /// 6：退回，货物正处于退回发件人的途中；
        /// </summary>
        [XmlElement("state")]
        public int state { get; set; }
    }


    public class KD100QueryResponsedata
    {
        [XmlElement("time")]
        public string time { get; set; }

        [XmlElement("context")]
        public string context { get; set; }
    }

}
