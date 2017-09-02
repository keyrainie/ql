using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 付款单信息
    /// </summary>
    public class PayItemInfo : IIdentity
    {
        /// <summary>
        /// 应付款编号
        /// </summary>
        public int? PaySysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public PayItemStyle? PayStyle
        {
            get;
            set;
        }

        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? PayAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 预估付款时间
        /// </summary>
        public DateTime? EstimatePayTime
        {
            get;
            set;
        }

        /// <summary>
        /// 相关单据ID
        /// </summary>
        public string ReferenceID
        {
            get;
            set;
        }

        /// <summary>
        /// 付款时间
        /// </summary>
        public DateTime? PayTime
        {
            get;
            set;
        }

        /// <summary>
        /// 付款人系统编号
        /// </summary>
        public int? PayUserSysNo
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
        /// 付款单状态
        /// </summary>
        public PayItemStatus? Status
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
        /// 单据编号
        /// </summary>
        public int? OrderSysNo
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
        /// 可用金额
        /// </summary>
        public decimal? AvailableAmt
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
        /// 原始单据金额
        /// </summary>
        public decimal? RawOrderAmt
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
        /// 银行科目
        /// </summary>
        public string BankGLAccount
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 编辑人用户
        /// </summary>
        public int? EditUserSysNo
        {
            get;
            set;
        }

        private int? _batchNumber;
        /// <summary>
        /// 批次号
        /// </summary>
        public int? BatchNumber
        {
            get
            {
                return _batchNumber;
            }
            set
            {
                //CRL18977:批次号为空时默认为1
                _batchNumber = value.HasValue ? value : 1;
            }
        }

        /// <summary>
        /// 创建人用户系统编号
        /// </summary>
        public int? CreateUserSysNo
        {
            get;
            set;
        }

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
    }
}