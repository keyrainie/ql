using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Model
{
    public class CommissionToCashRecordEntity
    {
        #region Table Field 对应表字段
        /// <summary>
        /// 获取或设置单据编号
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        /// <summary>
        /// 获取或设置佣金兑现申请状态
        /// </summary>
        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }

        /// <summary>
        /// 获取或设置结算状态
        /// </summary>
        [DataMapping("UserSysNo", DbType.Int32)]
        public int UserSysNo { get; set; }

        /// <summary>
        /// 获取或设置申请兑现金额
        /// </summary>
        [DataMapping("ToCashAmt", DbType.Decimal)]
        public decimal ToCashAmt { get; set; }

        /// <summary>
        /// 获取或设置兑现税后金额
        /// </summary>
        [DataMapping("AfterTaxAmt", DbType.Decimal)]
        public decimal? AfterTaxAmt { get; set; }

        public string AfterTaxAmtStr
        {
            get
            {
                if (this.AfterTaxAmt.HasValue)
                {
                    return this.AfterTaxAmt.Value.ToString("#0.00") + "元";
                }
                return "--";
            }
        }


        /// <summary>
        /// 获取或设置确认兑现金额
        /// </summary>
        [DataMapping("ConfirmToCashAmt", DbType.Decimal)]
        public decimal ConfirmToCashAmt { get; set; }

        /// <summary>
        /// 获取或设置确认兑现时间
        /// </summary>
        [DataMapping("ConfirmToCashTime", DbType.DateTime)]
        public DateTime? ConfirmToCashTime { get; set; }

        public string DisplayConfirmToCashTime
        {
            get
            {
                string result = "--";
                if (ConfirmToCashTime.HasValue)
                {
                    result = ConfirmToCashTime.Value.ToString("yyyy-MM-dd");
                }
                return result;
            }
        }


        /// <summary>
        /// 获取或设置实际支付金额
        /// </summary>
        [DataMapping("PayAmt", DbType.Decimal)]
        public decimal PayAmt { get; set; }

        /// <summary>
        /// 获取或设置实际支付时间
        /// </summary>
        [DataMapping("PayTime", DbType.DateTime)]
        public DateTime? PayTime { get; set; }

        public string DisplayPayTime
        {
            get
            {
                string result = "--";
                if (PayTime.HasValue)
                {
                    result = PayTime.Value.ToString("yyyy-MM-dd");
                }
                return result;
            }
        }
        /// <summary>
        /// 是否能够提供发票：Y/N
        /// </summary>
        [DataMapping("IsHasInvoice", DbType.String)]
        public string IsHasInvoice { get; set; }

        /// <summary>
        /// 获取或设置 用户银行代码
        /// </summary>
        [DataMapping("BankCode", DbType.String)]
        public string BankCode { get; set; }


        /// <summary>
        /// 获取或设置 用户银行名称
        /// </summary>
        [DataMapping("BankName", DbType.String)]
        public string BankName { get; set; }

        /// <summary>
        /// 获取或设置 开户支行信息
        /// </summary>
        [DataMapping("BranchBank", DbType.String)]
        public string BranchBank { get; set; }

        /// <summary>
        /// 获取或设置 银行卡卡号码
        /// </summary>
        [DataMapping("BankCardNumber", DbType.String)]
        public string BankCardNumber { get; set; }


        /// <summary>
        /// 获取或设置 银行卡卡号码
        /// </summary>
        public string DisplayBankCardNumber
        {
            get
            {
                string bankCardNumber = this.BankCardNumber;
                if (this.BankCardNumber.Length > 4)
                {
                    bankCardNumber = BankCardNumber.Substring(0, 4);
                }
                for (int i = 0; i < this.BankCardNumber.Length - 4; i++)
                {
                    bankCardNumber += "*";
                }
                return bankCardNumber;
            }
        }

        public string DisplayStatus
        {
            get
            {
                string result = "--";
                switch (this.Status)
                {
                    case "R":
                        result = "已申请";
                        break;
                    case "C":
                        result = "已确认";
                        break;
                    case "P":
                        result = "已付款";
                        break;
                }
                return result;
            }
        }

        /// <summary>
        /// 获取或设置 收款人名称
        /// </summary>
        [DataMapping("ReceivableName", DbType.String)]
        public string ReceivableName { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [DataMapping("Memo", DbType.String)]
        public string Memo { get; set; }

        #endregion Table Field 对应表字段

        /// <summary>
        /// 获取或设置确认兑现时间
        /// </summary>
        [DataMapping("RequestDate", DbType.DateTime)]
        public DateTime RequestDate { get; set; }

        [DataMapping("SettledMonth", DbType.DateTime)]
        public string SettledMonth { get; set; }
        public string DisplaySettledMonth
        {
            get
            {
                return SettledMonth.Replace(",", "<br/>");
            }
        }
        #region Common Field 公用字段
        /// <summary>
        /// 获取或设置创建人
        /// </summary>
        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        /// <summary>
        /// 获取或设置创建时间
        /// </summary>
        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        /// <summary>
        /// 获取或设置编辑人
        /// </summary>
        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        /// <summary>
        /// 获取或设置编辑时间
        /// </summary>
        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 获取或设置公司代码
        /// </summary>
        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 获取或设置公司代码
        /// </summary>
        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 获取或设置语言编码
        /// </summary>
        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }
        #endregion Common Field
    }
}
