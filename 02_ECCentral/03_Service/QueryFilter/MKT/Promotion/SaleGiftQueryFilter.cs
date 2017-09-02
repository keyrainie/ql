using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT.Promotion
{
    public class SaleGiftQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? MasterProductSysNo { get; set; }

        public int? GiftProductSysNo { get; set; }

        public int? Category3SysNo { get; set; }

        public int? Category2SysNo { get; set; }

        public int? Category1SysNo { get; set; }

        public int? BrandSysNo { get; set; }

        public string PMUser { get; set; }

        public int? SysNo { get; set; }

        public SaleGiftStatus? Status { get; set; }

        public string PromotionName { get; set; }

        public SaleGiftType? Type { get; set; }

        public string ChannelID { get; set; }

        public DateTime? ActivityDateFrom { get; set; }

        public DateTime? ActivityDateTo { get; set; }
         
        public string CompanyCode{get;set;}

        public int VendorSysNo { get; set; }

    }

    public class SaleGiftLogQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? ProductSysNo { get; set; }    

    }
}
