using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单拦截信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOInterceptInfo : IIdentity
    {

        /// <summary>
        /// 公司编号
        /// </summary> 
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 渠道编号
        /// </summary> 
        [DataMember]
        public string WebChannelID { get; set; }

        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 配送方式类别
        /// </summary>
        [DataMember]
        public string ShipTypeCategory 
        {
            get 
            {
                if (ShipTypeEnum == "0")
                    return "奥硕";
                else if (ShipTypeEnum == "1,2")
                    return "自提";
                else
                    return "3PL";
            }
            set { }
        }
        
        /// <summary>
        /// 订单编号List
        /// </summary>
        [DataMember]
        public string Sysnolist { get; set; }

        /// <summary>
        /// 配送方式枚举
        /// </summary>
        [DataMember]
        public string ShipTypeEnum { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        public string StockSysNo { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        public string StockName { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        [DataMember]
        public int? ShipTypeSysNo { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        [DataMember]
        public string ShippingTypeID { get; set; }

        /// <summary>
        /// 配送方式名称
        /// </summary>
        [DataMember]
        public string ShippingTypeName { get; set; }

        /// <summary>
        /// 有无运单号
        /// </summary>
        [DataMember]
        public string HasTrackingNumber { get; set; }

        /// <summary>
        /// 配送时间类别
        /// </summary>
        [DataMember]
        public string ShipTimeType { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [DataMember]
        public string ContactName { get; set; }

        /// <summary>
        /// 订单拦截收件人 邮件地址
        /// </summary>
        [DataMember]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 订单拦截抄送人 邮件地址
        /// </summary>
        [DataMember]
        public string CCEmailAddress { get; set; }

        /// <summary>
        /// 发票拦截收件人 邮件地址
        /// </summary>
        [DataMember]
        public string FinanceEmailAddress { get; set; }

        /// <summary>
        /// 发票拦截收件人 邮件地址
        /// </summary>
        [DataMember]
        public string FinanceCCEmailAddress { get; set; }
    }
}
