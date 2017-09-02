using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace ECCentral.Service.EventMessage.SZPointAlliance
{
    public class SZPointAllianceMessage : IEventMessage
    {

        public string Subject
        {
            get { return "SZPointAllianceMessage"; }
        }
    }

    public class SZPointAllianceRequestMessage : IEventMessage
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public int SOSysNo { get; set; }

        /// <summary>
        /// 退款类型
        /// </summary>
        public PointAllianceRefundType RefundType { get; set; }

        /// <summary>
        /// 支付交易编号
        /// </summary>
        public string TNumber { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 退款描述
        /// </summary>
        public string RefundDescription { get; set; }

        /// <summary>
        /// 部分退款的一个唯一标识，用来防止同一次的部分退款重复提交
        /// </summary>
        public string RefundKey { get; set; }

        /// <summary>
        /// 登陆用户名
        /// </summary>
        public string LogUserName { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        public string LanguageCode { get; set; }

        public string StoreCompanyCode { get; set; }

        public string Subject
        {
            get { return "SZPointAllianceRequestMessage"; }
        }
    }

    [Serializable]
    [XmlRoot("request")]
    public class SZPointAllianceResponseMessage
    {
        [XmlElement("result")]
        public int Result { get; set; }

        [XmlElement("message")]
        public string Message { get; set; }
    }

    public enum PointAllianceRefundType
    {
        /// <summary>
        /// 预付卡
        /// </summary>
        PrepaidCard = 0,
        /// <summary>
        /// 积分
        /// </summary>
        Point = 1,
    }
}
