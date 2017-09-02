using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 应付款信息
    /// </summary>
    public class PayableInfo : IIdentity, ICompany
    {

        /// <summary>
        /// 根据不同的Type
        /// 单据编号
        /// </summary>
        public int? OrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 应付款对应的付款单列表
        /// </summary>
        public List<PayItemInfo> PayItemList
        {
            get;
            set;
        }

        /// <summary>
        /// 货币系统编号
        /// </summary>
        public int? CurrencySysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? OrderAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 已付款金额
        /// </summary>
        public decimal? AlreadyPayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 应付款状态
        /// </summary>
        public PayableStatus? PayStatus
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
        /// 备注
        /// </summary>
        public string Note
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
        /// 单据状态
        /// </summary>
        public int? OrderStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 发票更改时间
        /// </summary>
        public DateTime? InvoiceUpdateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 发票实际状态
        /// </summary>
        public PayableInvoiceFactStatus? InvoiceFactStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 批次号
        /// </summary>
        public int? BatchNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 已入库金额
        /// </summary>
        public decimal? InStockAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 预计付款时间
        /// </summary>
        public DateTime? EstimatedTimeOfPay
        {
            get;
            set;
        }

        /// <summary>
        /// EIMS返点金额
        /// </summary>
        public decimal? EIMSAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 原始订单金额
        /// </summary>
        public decimal? RawOrderAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 返点编号
        /// </summary>
        public int? EIMSNo
        {
            get;
            set;
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public PayableAuditStatus? AuditStatus
        {
            get;
            set;
        }

        /// <summary>
        /// PM系统编号
        /// </summary>
        public int? PMSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 审核人系统编号
        /// </summary>
        public int? AuditUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDatetime
        {
            get;
            set;
        }

        /// <summary>
        /// 发票信息更新人用户系统编号
        /// </summary>
        public int? UpdateInvoiceUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 当前操作员名称
        /// </summary>
        public string OperationUserFullName
        {
            get;
            set;
        }

        public DateTime? ETP { get; set; }

        public DateTime? EGP { get; set; }

        public string extendSysNo { get; set; }

        public string Memo { get; set; }

        public string NewMemo { get; set; }
        public string Tag { get; set; }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion IIdentity Members

        #region ICompany Members

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion ICompany Members
    }
}