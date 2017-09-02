using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Invoice
{
    public class CommissionItemLogDetailInfo
    {
        public int SN { get; set; }

        public int SysNo { get; set; }


        public int MerchantSysNo { get; set; }

        public int CommissionItemSysNo { get; set; }


        public int ReferenceSysNo { get; set; }


        public string ReferenceType { get; set; }


        public int ProductSysNo { get; set; }


        public string ProductID { get; set; }


        public string ProductName { get; set; }


        public decimal Price { get; set; }

        public int Qty { get; set; }

        public string InUser { get; set; }


        public DateTime? InDate { get; set; }


        public string EditUser { get; set; }


        public DateTime? EditDate { get; set; }

        public string CurrencyCode { get; set; }


        public string Type { get; set; }

        public string InDateStr
        {
            get
            {
                return (this.InDate.HasValue) ? this.InDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            }
            set { }
        }


        public string ReferenceTypeDesc
        {
            get
            {
                switch (ReferenceType.Trim().ToUpper())
                {
                    case "SO":
                        return "销售订单";
                    case "RMA":
                        return "退货单";
                    default:
                        return string.Empty;
                }
            }
        }

        public decimal TotalAmt
        {
            get
            {
                decimal promotionDiscount = 0;
                if (PromotionDiscount.HasValue)
                {
                    promotionDiscount = Math.Round(PromotionDiscount.Value, 2);
                }
                return Qty * Price + promotionDiscount;

            }
        }


        public decimal? PromotionDiscount
        {
            get;
            set;
        }

        public string PromotionDiscountDesc
        {
            get
            {
                string text = string.Empty;
                if (PromotionDiscount.HasValue)
                {
                    text = PromotionDiscount.Value.ToString("0.00");
                }
                return text;
            }

        }
    }
}
