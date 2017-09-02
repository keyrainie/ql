using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.IM;

namespace ECCentral.QueryFilter.Inventory
{
    public class VirtualRequestQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }

        public int? ProductSysNo { get; set; }

        public int? CreateUserSysNo { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? StockSysNo { get; set; }

        public string CompanyCode { get; set; }

        public int? VirtualRequestType { get; set; }

        public VirtualRequestStatus? RequestStatus { get; set; }

        public string Note { get; set; }
    }

    public class VirtualRequestQueryProductsFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public int? PMSysNo { get; set; }

        public OperationType Operator { get; set; }

        public ProductStatus? Status { get; set; }

        public int? StockSysNo { get; set; }

        public ProductType? ProductType { get; set; }

        public int? Category1SysNo { get; set; }

        public int? Category2SysNo { get; set; }

        public int? Category3SysNo { get; set; }

        public string CompanyCode { get; set; }

        public enum OperationType
        {
            /// <summary>
            /// 等于
            /// </summary>
            Equal,
            /// <summary>
            /// 不等于
            /// </summary>
            NotEqual
        }
    }
}