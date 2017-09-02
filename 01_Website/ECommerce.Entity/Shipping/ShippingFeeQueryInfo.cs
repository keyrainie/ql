using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Shipping
{
    public class ShippingFeeQueryInfo
    {
        public int AreaId { get; set; }

        public int CustomerSysNo { get; set; }
        
        public int IsUseDiscount { get; set; }
        
        public int MerchantSysNo { get; set; }
        
        public int SellType { get; set; }
        
        public int ShipTypeId { get; set; }
        
        public decimal SoAmount { get; set; }
        
        public int SOSingleMaxWeight { get; set; }
        
        public int SoTotalWeight { get; set; }

        public string SubShipTypeList { get; set; }

        public string TransID { get; set; }
    }
}
