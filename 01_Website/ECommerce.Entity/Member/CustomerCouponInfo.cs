using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Member
{
    /// <summary>
    /// 用户优惠券信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomerCouponInfo
    {
        /// <summary>
        /// 活动系统编号
        /// </summary>
        public int CouponSysNo { get; set; }

        /// <summary>
        /// 优惠券号码系统编号
        /// </summary>
        public int CouponCodeSysNo { get; set; }

        /// <summary>
        /// 优惠券号码
        /// </summary>
        [DataMember]
        public string CouponCode { get; set; }
        /// <summary>
        /// 优惠券活动名称
        /// </summary>
        [DataMember]
        public string CouponName { get; set; }
        /// <summary>
        /// 优惠券活动描述
        /// </summary>
        [DataMember]
        public string CouponDesc { get; set; }
        /// <summary>
        /// 生效日期
        /// </summary>
        [DataMember]
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 是否使用，0-未使用；1-已使用
        /// </summary>
        [DataMember]
        public int IsUsed
        {
            get
            {
                return this.UsedCount > 0 ? 1 : 0;
            }
        }
        /// <summary>
        /// 使用次数
        /// </summary>
        [DataMember]
        public int UsedCount { get; set; }

        /// <summary>
        /// 是否过期，true-过期；false-未过期
        /// </summary>
        [DataMember]
        public bool IsExpired
        {
            get
            {
                return DateTime.Now <= EndDate ? false : true;
            }
        }
        /// <summary>
        /// 客户最大使用次数
        /// </summary>
        [DataMember]
        public int? CustomerMaxFrequency { get; set; }

        /// <summary>
        /// 全网最大使用次数
        /// </summary>
        [DataMember]
        public int? WebsiteMaxFrequency { get; set; }

        /// <summary>
        /// 优惠券代码类型：C=通用型，T=投放型
        /// </summary>
        [DataMember]
        public string CodeType { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        [DataMember]
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        [DataMember]
        public string VendorName { get; set; }
    }
}
