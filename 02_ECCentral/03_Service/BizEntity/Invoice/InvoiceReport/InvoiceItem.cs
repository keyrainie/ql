using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.InvoiceReport
{
    public class InvoiceItem
    {
        public string ItemNumber { get; set; }
        public string ItemNumberEx
        {
            get { return ItemNumber; }
        }

        public string Description { get; set; }

        
        public decimal UnitPrice { get; set; }

        
        public int Quantity { get; set; }

        
        public decimal SumExtendPrice { get; set; }
        public string SumExtendPriceEx
        {
            get
            {
                if (ProductType == 4)
                {
                    return "P." + SumExtendPrice.ToString("#########0");
                }
                else
                {
                    return SumExtendPrice.ToString("#########0.00");
                }
            }
        }

        
        public string RepairWarrantyDays { get; set; }

        /// <summary>
        /// 0：正常Item；1：二手品；3：优惠券；4：可得积分
        /// </summary>
        
        public int ProductType { get; set; }

        
        public bool IsExtendWarrantyItem { get; set; }

        
        public int ProductSysNo { get; set; }

        
        public string MasterProductSysNo { get; set; }

    }
}
