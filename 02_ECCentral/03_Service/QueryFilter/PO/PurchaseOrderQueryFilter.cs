using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.PO
{
    public class PurchaseOrderQueryFilter
    {
        public PurchaseOrderQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }
        public string POSysNo { get; set; }
        public string ProductID { get; set; }
        public string ProductSysNo { get; set; }
        public string StockSysNo { get; set; }
        public string CompanySysNo { get; set; }
        public DateTime? CreateTimeBegin { get; set; }
        public DateTime? CreateTimeTo { get; set; }
        public DateTime? InStockFrom { get; set; }
        public DateTime? InStockTo { get; set; }
        public string CurrencySysNo { get; set; }
        public string IsApportion { get; set; }
        public PurchaseOrderConsignFlag? IsConsign { get; set; }
        public PurchaseOrderStatus? Status { get; set; }
        public string StatusList { get; set; }
        public string IsStockStatus { get; set; }
        public DateTime? PrintTime { get; set; }
        public PurchaseOrderVerifyStatus? VerifyStatus { get; set; }
        public string VendorName { get; set; }
        public string VendorSysNo { get; set; }
        public string PMSysNo { get; set; }
        public string CreatePOSysNo { get; set; }
        public PurchaseOrderType? POType { get; set; }
        public bool? isTransfer { get; set; }
        public int? TranferStock { get; set; }
        public string POSysNoExtention { get; set; }

        public string AuditUser { get; set; }
        public string PMName { get; set; }
        //public List<int> PMSysNoList { get; set; }
        public bool? IsManagerPM { get; set; }

        /// <summary>
        /// 是否为在途查询
        /// </summary>
        public bool? IsPurchaseQtySearch { get; set; }

        /// <summary>
        /// 获取或设置在途PO数据状态[,分割]
        /// </summary>
        public string QueryStatus { get; set; }
        /// <summary>
        /// 获取或设置在途查询仓库[,分割]
        /// </summary>
        public string QueryStock { get; set; }
        public string CompanyCode { get; set; }


        public List<int> PMList { get; set; }
        public PMQueryType? PMQueryType { get; set; }

        public List<int> PMAuthorizedList { get; set; }
        public string CurrentUserName { get; set; }

        public PaySettleCompany? PaySettleCompany { get; set; }

        public DateTime? ETATimeFrom { get; set; }
        public DateTime? ETATimeTo { get; set; }
        public decimal? FromTotalAmount { get; set; }
        public decimal? ToTotalAmount { get; set; }

        public string LogisticsNumber { get; set; }
        public string ExpressName { get; set; }

    }
}
