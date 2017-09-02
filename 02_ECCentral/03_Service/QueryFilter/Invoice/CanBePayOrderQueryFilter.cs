using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// 采购相关单据查询条件
    /// </summary>
    public class CanBePayOrderQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 应付款单据类型
        /// </summary>
        public PayableOrderType? OrderType
        {
            get;
            set;
        }

        public string OrderID
        {
            get;
            set;
        }

        public ECCentral.BizEntity.PO.PurchaseOrderStatus? POStatus
        {
            get;
            set;
        }

        public ECCentral.BizEntity.PO.SettleStatus? VendorSettleStatus
        {
            get;
            set;
        }

        public DateTime? POETPFrom
        {
            get;
            set;
        }

        public DateTime? POETPTo
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public string WebChannelID
        {
            get;
            set;
        }
    }
}