using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.ExternalSYS
{
    /// <summary>
    /// 佣金兑现申请单
    /// </summary>
    public class CommissionToCashInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 佣金申请兑现单状态
        /// </summary>
        public ToCashStatus Status { get; set; }

        /// <summary>
        /// CPS用户
        /// </summary>
        public CpsUserInfo CPSUserInfo { get; set; }

        /// <summary>
        /// 申请兑现金额
        /// </summary>
        public decimal ToCashAmt { get; set; }

        /// <summary>
        /// 用户银行代码
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// 用户银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 开户支行信息
        /// </summary>
        public string BranchBank { get; set; }

        /// <summary>
        /// 银行开卡号码
        /// </summary>
        public string BankCardNumber { get; set; }

        /// <summary>
        /// 收款人名称
        /// </summary>
        public string ReceivableName { get; set; }

        /// <summary>
        /// 是否提供发票
        /// </summary>
        public CanProvideInvoice CanProvideInvoice { get; set; }

        /// <summary>
        /// 税后申请兑现单金额
        /// </summary>
        public decimal AfterTaxAmt { get; set; }

        /// <summary>
        /// 应支付金额
        /// </summary>
        public decimal OldPayAmt { get; set; }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        public decimal NewPayAmt { get; set; }

        /// <summary>
        /// 奖金
        /// </summary>
        public decimal Bonus { get; set; }
        
        public decimal ConfirmToCashAmt { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public UserInfo User { get; set; }

    }
}
