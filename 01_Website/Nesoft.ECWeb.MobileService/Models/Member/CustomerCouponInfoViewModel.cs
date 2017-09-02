using Nesoft.ECWeb.Entity.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class CustomerCouponInfoViewModel
    {
        /// <summary>
        /// 优惠券号码
        /// </summary>
        public string CouponCode { get; set; }
        /// <summary>
        /// 优惠券活动名称
        /// </summary>
        public string CouponName { get; set; }
        /// <summary>
        /// 优惠券活动描述
        /// </summary>
        public string CouponDesc { get; set; }
        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 使用次数
        /// </summary>
        public int UsedCount { get; set; }
        /// <summary>
        /// 是否使用，0-未使用；1-已使用
        /// </summary>
        public int IsUsed
        {
            get
            {
                return this.UsedCount > 0 ? 1 : 0;
            }
        }
        /// <summary>
        /// 是否过期，true-过期；false-未过期
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return DateTime.Now <= EndDate ? false : true;
            }
        }
        public string BeginDateStr
        {
            get
            {
                return this.BeginDate.ToString("yyyy-MM-dd");
            }
        }
        public string EndDateStr
        {
            get
            {
                return this.EndDate.ToString("yyyy-MM-dd");
            }
        }
        public string StatusStr
        {
            get
            {
                if (this.IsUsed == 1)
                {
                    return "已使用";
                }
                else
                {
                    if (this.IsExpired)
                    {
                        return "已过期";
                    }
                    else
                    {
                        return "未使用";
                    }
                }
            }
        }
        /// <summary>
        /// 商家编号
        /// </summary>
        public string VendorName { get; set; }
    }
}