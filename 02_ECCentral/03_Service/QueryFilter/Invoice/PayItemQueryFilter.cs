using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// 采购-付款单查询条件
    /// </summary>
    public class PayItemQueryFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public String CompanyCode
        {
            get;
            set;
        }

        public String WebChannelID
        {
            get;
            set;
        }

        /// <summary>
        /// 单据编号
        /// </summary>
        public String OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        public PayableOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public String Note
        {
            get;
            set;
        }

        /// <summary>
        /// 发票状态修改时间: 从
        /// </summary>
        public DateTime? InvoiceEditDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 发票状态修改时间: 到
        /// </summary>
        public DateTime? InvoiceEditDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 是否过滤Abandon状态的付款单
        /// </summary>
        public Boolean IsFilterAbandonItem
        {
            get;
            set;
        }

        /// <summary>
        /// 发票状态
        /// </summary>
        public PayableInvoiceStatus? InvoiceStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public Int32? VendorSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// PMID
        /// </summary>
        public Int32? UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 付款状态
        /// </summary>
        public PayItemStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 付款类型
        /// </summary>
        public PayItemStyle? PayStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间: 从
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间: 到
        /// </summary>
        public DateTime? CreateDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 是否到期的未付款PO明细
        /// </summary>
        public Boolean IsFilterPOETP
        {
            get;
            set;
        }

        /// <summary>
        /// 到期付款日： 从
        /// </summary>
        public DateTime? ETPFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 到期付款日： 到
        /// </summary>
        public DateTime? ETPTo
        {
            get;
            set;
        }

        /// <summary>
        /// 估计付款时间: 从
        /// </summary>
        public DateTime? EstimatePayDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 估计付款时间: 到
        /// </summary>
        public DateTime? EstimatePayDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 付款时间: 从
        /// </summary>
        public DateTime? PayDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 付款时间: 到
        /// </summary>
        public DateTime? PayDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库系统编号
        /// </summary>
        public Int32? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 凭证号
        /// </summary>
        public String ReferenceID
        {
            get;
            set;
        }

        /// <summary>
        /// 未入库（PO）
        /// </summary>
        public Boolean NotInStock
        {
            get;
            set;
        }

        /// <summary>
        /// 付款结算公司
        /// </summary>
        public int? PaySettleCompany
        {
            get;
            set;
        }
    }
}
