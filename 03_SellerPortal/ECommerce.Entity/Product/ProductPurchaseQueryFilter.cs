using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess.SearchEngine;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProductPurchaseQueryFilter : QueryFilter
    {
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
        public string VerifyStatus { get; set; }
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

        public int? BrandSysNo { get; set; }

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

        public bool? LeaseFlag { get; set; }
    }
}
