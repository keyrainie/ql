using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Invoice
{
    /// <summary>
    /// 销售-电汇邮局收款单
    /// </summary>
    public class PostIncomeInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 订单编号。
        /// 财务人员在建立收款单时填写（非必填项），作为客服人员建立和订单关联时的依据。
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal? IncomeAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 付款人
        /// </summary>
        public string PayUser
        {
            get;
            set;
        }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime? IncomeDate
        {
            get;
            set;
        }

        /// <summary>
        /// 付款行
        /// </summary>
        public string PayBank
        {
            get;
            set;
        }

        /// <summary>
        /// 收款行
        /// </summary>
        public string IncomeBank
        {
            get;
            set;
        }

        /// <summary>
        /// 财务备注
        /// </summary>
        public string Notes
        {
            get;
            set;
        }

        /// <summary>
        /// 确认收款单操作用户编号
        /// </summary>
        public int? ConfirmUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单确认状态
        /// </summary>
        public PostIncomeStatus? ConfirmStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime? ConfirmDate
        {
            get;
            set;
        }

        /// <summary>
        /// 处理收款单操作用户编号
        /// </summary>
        public int? HandleUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单处理状态
        /// </summary>
        public PostIncomeHandleStatus? HandleStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 收款单处理时间
        /// </summary>
        public DateTime? HandleDate
        {
            get;
            set;
        }

        /// <summary>
        /// 客服备注
        /// </summary>
        public string CSNotes
        {
            get;
            set;
        }

        /// <summary>
        /// 银行流水号
        /// </summary>
        public string BankNo
        {
            get;
            set;
        }

        /// <summary>
        /// 电汇邮局收款单关联订单
        /// </summary>
        public List<PostIncomeConfirmInfo> ConfirmInfoList
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

    /// <summary>
    /// 电汇邮局收款单关联信息
    /// </summary>
    public class PostIncomeConfirmInfo : IIdentity
    {
        /// <summary>
        /// 电汇邮局收款单编号
        /// </summary>
        public int? PostIncomeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// CS确认的订单编号
        /// </summary>
        public int? ConfirmedSoSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public PostIncomeConfirmStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 销售渠道编号（冗余）
        /// </summary>
        public string ChannelID
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

    public class ImportPostIncome : IComparer<ImportPostIncome>
    {
        public int? SysNo
        {
            get;
            set;
        }
        public string SOSysNo
        {
            get;
            set;
        }
        public string IncomeAmtString
        {
            get;
            set;
        }
        public string PayUser
        {
            get;
            set;
        }
        public string IncomeDateString
        {
            get;
            set;
        }
        public string PayBank
        {
            get;
            set;
        }
        public string IncomeBank
        {
            get;
            set;
        }
        public string Notes
        {
            get;
            set;
        }
        public string BankNo
        {
            get;
            set;
        }

        #region IComparer<ImportPostIncome> Members

        public int Compare(ImportPostIncome x, ImportPostIncome y)
        {
            if (x.SysNo > y.SysNo)
                return 1;
            else if (x.SysNo < y.SysNo)
                return -1;
            return 0;
        }

        #endregion IComparer<ImportPostIncome> Members
    }
}