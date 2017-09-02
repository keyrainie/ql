using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.ComponentModel;


namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class Coupon : DetailBaseEntity
    {
        [DataMapping("CouponName", DbType.String)]
        public string CouponName { get; set; }

        [DataMapping("BeginDate", DbType.DateTime)]
        public DateTime BeginDate { get; set; }

        [DataMapping("EndDate", DbType.DateTime)]
        public DateTime EndDate { get; set; }

        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }

        [DataMapping("BindStatus", DbType.String)]
        public string BindStatus { get; set; }

        [DataMapping("MerchantSysNo", DbType.Int32)]
        public int MerchantSysNo { get; set; }

        [DataMapping("LimitType", DbType.String)]
        public string LimitType { get; set; }

        /// <summary>
        /// 是否定时发放到账户中心
        /// </summary>
        [DataMapping("IsAutoBinding", DbType.String)]
        public string IsAutoBinding { get; set; }

        /// <summary>
        /// 发放日期
        /// </summary>
        [DataMapping("BindingDate", DbType.DateTime)]
        public DateTime BindingDate { get; set; }

        [DataMapping("IsSendMail", DbType.String)]
        public string IsSendMail { get; set; }

        /// <summary>
        /// 触发条件
        /// </summary>
        [DataMapping("BindCondition", DbType.String)]
        public string BindCondition { get; set; }

        [DataMapping("ValidPeriod", DbType.Int32)]
        public int ValidPeriod { get; set; }

        /// <summary>
        /// 自定义code开始时间
        /// </summary>
        [DataMapping("BindBeginDate", DbType.DateTime)]
        public DateTime BindBeginDate { get; set; }

        /// <summary>
        /// 自定义code结束时间
        /// </summary>
        [DataMapping("BindEndDate", DbType.DateTime)]
        public DateTime BindEndDate { get; set; }

        /// <summary>
        /// 是否订阅邮件
        /// </summary>
        [DataMapping("IsSubscribe", DbType.Int32 )]
        public int IsSubscribe { get; set; }

        /// <summary>
        ///门槛金额
        /// </summary>
        [DataMapping("AmountLimit", DbType.Decimal)]
        public Decimal AmountLimit { get; set; }
    }

    [Serializable]
    public enum ValidPeriodType : int
    {
        [Description("不限")]
        ALL = 0,
        [Description("自发放日起一周")]
        OneWeek = 1,
        [Description("自发放日起一个月")]
        OneMonth = 2,
        [Description("自发放日起两个月")]
        TwoMonth = 3,
        [Description("自发放日起三个月")]
        Threemonth = 4,
        [Description("自发放日起六个月")]
        SixMonth = 5,
        [Description("自定义时间")]
        Customerize = 6
    }
}
