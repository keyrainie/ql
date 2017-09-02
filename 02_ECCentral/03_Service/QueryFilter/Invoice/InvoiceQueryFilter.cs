using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// 分公司收款单查询条件
    /// </summary>
    public class InvoiceQueryFilter
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
        /// 单据ID
        /// </summary>
        public string OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// 客户系统编号
        /// </summary>
        public string CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单单据类型
        /// </summary>
        public SOIncomeOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 收款类型
        /// </summary>
        public SOIncomeOrderStyle? IncomeType
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单状态
        /// </summary>
        public SOIncomeStatus? IncomeStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public string PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        public string ShipTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单审核人
        /// </summary>
        public string IncomeConfirmer
        {
            get;
            set;
        }

        /// <summary>
        /// 单据系统编号
        /// </summary>
        public string OrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 涉及现金的RO
        /// </summary>
        public bool IsCash
        {
            get;
            set;
        }

        /// <summary>
        /// 统计异常数据
        /// </summary>
        public bool IsException
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库编号，多个仓库编号之间用逗号隔开
        /// </summary>
        public string StockID
        {
            get;
            set;
        }

        /// <summary>
        /// 根据单据的CustomerSysNo查询所有
        /// </summary>
        public bool? IsByCustomer
        {
            get;
            set;
        }

        /// <summary>
        /// 是否关联子母单
        /// </summary>
        public bool IsRelated
        {
            get;
            set;
        }

        /// <summary>
        /// 是否排除测试和作废的
        /// </summary>
        public bool IsSalesOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 是否排除礼品卡
        /// </summary>
        public bool IsGiftCard
        {
            get;
            set;
        }

        /// <summary>
        /// CompanyCode
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 网关收款时间起
        /// </summary>
        public DateTime? PayedDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 网关收款时间止
        /// </summary>
        public DateTime? PayedDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间起
        /// </summary>
        public DateTime? CreateDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间止
        /// </summary>
        public DateTime? CreateDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 确认时间起
        /// </summary>
        public DateTime? ConfirmDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间止
        /// </summary>
        public DateTime? ConfirmDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 出库时间起
        /// </summary>
        public DateTime? SOOutDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 出库时间止
        /// </summary>
        public DateTime? SOOutDateTo
        {
            get;
            set;
        }

        /// <summary>
        /// RO退款时间起
        /// </summary>
        public DateTime? RORefundDateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// RO退款时间止
        /// </summary>
        public DateTime? RORefundDateTo
        {
            get;
            set;
        }
    }
}