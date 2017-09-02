using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums.Promotion;
using ECommerce.Utility;

namespace ECommerce.Entity.Promotion
{
    public class ComboQueryResult : ComboInfo
    {
        /// <summary>
        /// 原价
        /// </summary>
        public decimal CurrentPriceAmt { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal DiscountAmt { get; set; }

        /// <summary>
        /// 活动价
        /// </summary>
        public decimal PromoAmt
        {
            get
            {
                decimal promoPrice = 0;
                if (DiscountAmt > 0)
                {
                    promoPrice = CurrentPriceAmt - DiscountAmt;
                }
                else
                {
                    promoPrice = CurrentPriceAmt + DiscountAmt;
                }
                if (promoPrice < 0)
                {
                    return 0;
                }
                return promoPrice;
            }
        }

        public string UIInDate
        {
            get
            {
                return this.InDate.ToString("yyyy-MM-dd");
            }
        }

        public string UIStatus
        {
            get
            {
                return EnumHelper.GetDescription(this.Status);
            }
        }

        public bool CanEdit
        {
            get
            {
                if (this.Status == ComboStatus.Deactive)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanSubmit
        {
            get
            {
                if (this.Status == ComboStatus.Deactive)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanVoid
        {
            get
            {
                if (this.Status == ComboStatus.Active)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
