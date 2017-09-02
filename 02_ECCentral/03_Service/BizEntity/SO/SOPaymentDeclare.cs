using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    public class SOPaymentDeclare
    {
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public int PayTypeSysNo { get; set; }
        /// <summary>
        /// 支付方式编号
        /// </summary>
        public string PayTypeID { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MerchantOrderId { get; set; }
        /// <summary>
        /// 支付总金额
        /// </summary>
        public string PayAmount { get; set; }
        /// <summary>
        /// 付款币种
        /// </summary>
        public string PayCUR { get; set; }
        /// <summary>
        /// 支付交易号
        /// </summary>
        public string PayTransNumber { get; set; }
        /// <summary>
        /// 支付交易时间
        /// </summary>
        public string PayTime { get; set; }
        /// <summary>
        /// 付款银行名称
        /// </summary>
        public string BankPayName { get; set; }
        /// <summary>
        /// 付款银行编号
        /// </summary>
        public string BankPayCode { get; set; }
        /// <summary>
        /// 付款银行交易号
        /// </summary>
        public string BankSerialNo { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public int IDCardType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string IdentifyCode { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 所属国家代码
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 申报序列号
        /// </summary>
        public string DeclareTrackingNumber { get; set; }
        /// <summary>
        /// 申报状态(0：失败；1：成功)
        /// </summary>
        public int? DeclareStatus { get; set; }
    }
}
