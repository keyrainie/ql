using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    public class RMAInventory
    {       
        public int WarehouseSysNo{ get; set; }
       
        public int ProductSysNo { get; set; }
       
        public string ProductBriefName { get; set; }
       
        public int? RMAStockQty
        { get; set; }
       
        public int? RMAOnVendorQty
        { get; set; }
        
        public int? ShiftQty
        { get; set; }
       
        public string CompanyCode
        { get; set; }
      
        public string LanguageCode
        { get; set; }
       
        public string StoreCompanyCode
        { get; set; }
       
        public int? OwnbyNeweggQty
        { get; set; }
       
        public int? OwnbyCustomerQty
        { get; set; }
       
        public decimal? AverageCost
        { get; set; }
       
        public decimal? AverageCostWithoutTax
        { get; set; }
    }
}
