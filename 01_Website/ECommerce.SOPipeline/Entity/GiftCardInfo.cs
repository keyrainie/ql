using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 礼品卡
    /// </summary>
    public class GiftCardInfo : ExtensibleObject
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>   
        /// 卡号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        public decimal AvailableAmount { get; set; }

        /// <summary>
        /// 使用的金额
        /// </summary>
        public decimal UseAmount { get; set; }

        /// <summary>
        /// 有效开始时间
        /// </summary>
        public DateTime ValidBeginDate { get; set; }

        /// <summary>
        /// 有效截止时间
        /// </summary>
        public DateTime ValidEndDate { get; set; }

        /// <summary>
        /// 绑定用户编号
        /// </summary>
        public int BindingCustomerSysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 是否已被锁定
        /// </summary>
        public bool IsLock { get; set; }

        public override ExtensibleObject CloneObject()
        {
            return new GiftCardInfo()
            {
                Code = this.Code,
                AvailableAmount = this.AvailableAmount,
                BindingCustomerSysNo = this.BindingCustomerSysNo,
                Password = this.Password,
                Status = this.Status,
                TotalAmount = this.TotalAmount,
                UseAmount = this.UseAmount,
                ValidBeginDate = this.ValidBeginDate,
                ValidEndDate = this.ValidEndDate,
            };
        }
    }

    /// <summary>
    /// 礼品卡比较器
    /// </summary>
    public class GiftCardInfoEqualityComparer : IEqualityComparer<GiftCardInfo>
    {
        public bool Equals(GiftCardInfo x, GiftCardInfo y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }
            return x.Code.Equals(y.Code, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(GiftCardInfo obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
