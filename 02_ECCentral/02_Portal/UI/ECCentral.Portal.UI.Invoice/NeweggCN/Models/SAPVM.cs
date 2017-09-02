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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Models
{
    public class SAPVendorVM : ModelBase
    {
        private string vendorSysNo;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }

        private string vendorName;
        public string VendorName
        {
            get { return vendorName; }
            set { base.SetValue("VendorName", ref vendorName, value); }
        }

        private string sapVendorID;
        public string SAPVendorID
        {
            get { return sapVendorID; }
            set { base.SetValue("SAPVendorID", ref sapVendorID, value); }
        }
        private string sapVendorName;
        [Validate(ValidateType.Required)]
        public string SAPVendorName
        {
            get { return sapVendorName; }
            set { base.SetValue("SAPVendorName", ref sapVendorName, value); }
        }

        private string paymentTerm;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string PaymentTerm
        {
            get { return paymentTerm; }
            set { base.SetValue("PaymentTerm", ref paymentTerm, value); }
        }
    }
    public class SAPCompanyVM : ModelBase
    {
        private string stockID;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string StockID
        {
            get { return stockID; }
            set { base.SetValue("StockID", ref stockID, value); }
        }
        private string stockName;
        [Validate(ValidateType.Required)]
        public string StockName
        {
            get { return stockName; }
            set { base.SetValue("StockName", ref stockName, value); }
        }

        private string sapCompanyCode;
        [Validate(ValidateType.Required)]
        public string SAPCompanyCode
        {
            get { return sapCompanyCode; }
            set { base.SetValue("SAPCompanyCode", ref sapCompanyCode, value); }
        }
        private string sapBusinessArea;
        [Validate(ValidateType.Required)]
        public string SAPBusinessArea
        {
            get { return sapBusinessArea; }
            set { base.SetValue("SAPBusinessArea", ref sapBusinessArea, value); }
        }
        private decimal? salesTaxRate;
        public decimal? SalesTaxRate
        {
            get { return ConstValue.Invoice_TaxRateBase; }
            set { base.SetValue("SalesTaxRate", ref salesTaxRate, value); }
        }
        public decimal? purchaseTaxRate;
        public decimal? PurchaseTaxRate
        {
            get
            {
                return ConstValue.Invoice_TaxRateBase;
            }
            set { base.SetValue("PurchaseTaxRate", ref purchaseTaxRate, value); }
        }
    }

    public class SAPIPPUserVM : ModelBase
    {
        public int? SysNo { get; set; }

        private int? payTypeSysNo;
        [Validate(ValidateType.Required)]
        public int? PayTypeSysNo
        {
            get { return payTypeSysNo; }
            set { base.SetValue("PayTypeSysNo", ref payTypeSysNo, value); }
        }

        private string custDescription;
        public string CustDescription
        {
            get { return custDescription; }
            set { base.SetValue("CustDescription", ref custDescription, value); }
        }

        private string custID;
        [Validate(ValidateType.Regex, @"^CN[0-9]{3}$$", ErrorMessageResourceName = "Msg_EnterCNThreeDigit", ErrorMessageResourceType = typeof(ResSAP))]
        public string CustID
        {
            get { return custID; }
            set { base.SetValue("CustID", ref custID, value); }
        }

        public string systemConfirmID;
        [Validate(ValidateType.Regex, @"^CN[0-9]{3}$$", ErrorMessageResourceName = "Msg_EnterCNThreeDigit", ErrorMessageResourceType = typeof(ResSAP))]
        public string SystemConfirmID
        {
            get { return systemConfirmID; }
            set { base.SetValue("SystemConfirmID", ref systemConfirmID, value); }
        }
    }
}
