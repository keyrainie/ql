using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignSettlementRulesInfoVM : ModelBase
    {
        private int? ruleSysNo;

        public int? RuleSysNo
        {
            get { return ruleSysNo; }
            set { this.SetValue("RuleSysNo", ref ruleSysNo, value); }
        }

        private String m_SettleRulesCode;
        /// <summary>
        /// 规则代码
        /// </summary>
        [Validate(ValidateType.Required)]
        public String SettleRulesCode
        {
            get { return this.m_SettleRulesCode; }
            set { this.SetValue("SettleRulesCode", ref m_SettleRulesCode, value); }
        }

        private String m_SettleRulesName;
        /// <summary>
        /// 规则名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public String SettleRulesName
        {
            get { return this.m_SettleRulesName; }
            set { this.SetValue("SettleRulesName", ref m_SettleRulesName, value); }
        }

        private string productID;
        /// <summary>
        /// 商品ID    
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ProductID
        {
            get { return productID; }
            set { this.SetValue("ProductID", ref productID, value); }
        }

        private Int32? m_ProductSysNo;
        /// <summary>
        /// 商品编号
        /// </summary>
        public Int32? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private Int32? m_VendorSysNo;
        /// <summary>
        /// 供应商编号
        /// </summary>
        public Int32? VendorSysNo
        {
            get { return this.m_VendorSysNo; }
            set { this.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }

        /// <summary>
        /// 供应商名称
        /// </summary>
        private string vendorName;
        [Validate(ValidateType.Required)]
        public string VendorName
        {
            get { return vendorName; }
            set { this.SetValue("VendorName", ref vendorName, value); }
        }


        /// <summary>
        /// 剩余结算数量
        /// </summary>
        private int? remainQty;

        public int? RemainQty
        {
            get { return remainQty; }
            set { this.SetValue("RemainQty", ref remainQty, value); }
        }

        /// <summary>
        /// 原始结算价格
        /// </summary>
        private string oldSettlePrice;

        public string OldSettlePrice
        {
            get { return oldSettlePrice; }
            set { this.SetValue("OldSettlePrice", ref oldSettlePrice, value); }
        }

        /// <summary>
        /// 现在结算价格
        /// </summary>
        private string newSettlePrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string NewSettlePrice
        {
            get { return newSettlePrice; }
            set { this.SetValue("NewSettlePrice", ref newSettlePrice, value); }
        }

        private DateTime? m_BeginDate;
        /// <summary>
        /// 开始时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? BeginDate
        {
            get { return this.m_BeginDate; }
            set { this.SetValue("BeginDate", ref m_BeginDate, value); }
        }

        private DateTime? m_EndDate;
        /// <summary>
        /// 结束时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get { return this.m_EndDate; }
            set { this.SetValue("EndDate", ref m_EndDate, value); }
        }

        private string m_SettleRulesQuantity;
        /// <summary>
        /// 结算数量
        /// </summary>
        [Validate(ValidateType.Regex, @"^\d{1,8}$", ErrorMessageResourceName = "Integer_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string SettleRulesQuantity
        {
            get { return this.m_SettleRulesQuantity; }
            set { this.SetValue("SettleRulesQuantity", ref m_SettleRulesQuantity, value); }
        }


        private Int32? m_SettledQuantity;
        public Int32? SettledQuantity
        {
            get { return this.m_SettledQuantity; }
            set { this.SetValue("SettledQuantity", ref m_SettledQuantity, value); }
        }

        /// <summary>
        /// 货币编号
        /// </summary>
        private string currencyCode;

        public string CurrencyCode
        {
            get { return currencyCode; }
            set { this.SetValue("CurrencyCode", ref currencyCode, value); }
        }

        private ConsignSettleRuleStatus? status;

        public ConsignSettleRuleStatus? Status
        {
            get { return status; }
            set { this.SetValue("Status", ref status, value); }
        }

        private char? statusString;

        public char? StatusString
        {
            get { return statusString; }
            set { this.SetValue("StatusString", ref statusString, value); }
        }


    }
}
