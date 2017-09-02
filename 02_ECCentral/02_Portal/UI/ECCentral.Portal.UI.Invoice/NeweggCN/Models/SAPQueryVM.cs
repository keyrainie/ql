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
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Models
{
    public class SAPVendorQueryVM : ModelBase
    {
        private int? vendorSysNo;
        public int? VendorSysNo
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
    }

    public class SAPCompanyQueryVM : ModelBase
    {
        private string stockID;
        [Validate(ValidateType.Interger)]
        public string StockID
        {
            get { return stockID; }
            set { base.SetValue("StockID", ref stockID, value); }
        }
        private string stockName;
        public string StockName
        {
            get { return stockName; }
            set { base.SetValue("StockName", ref stockName, value); }
        }

        public string sapCompanyCode;
        public string SapCompanyCode
        {
            get { return sapCompanyCode; }
            set { base.SetValue("SapCompanyCode", ref sapCompanyCode, value); }
        }
    }


    public class SAPIPPUserQueryVM : ModelBase
    {
        private int? payType;
        public int? PayType
        {
            get { return payType; }
            set { base.SetValue("PayType", ref payType, value); }
        }
        private string custID;
        [Validate(ValidateType.Regex, @"^CN[0-9]{3}$$", ErrorMessageResourceName = "Msg_EnterCNThreeDigit",ErrorMessageResourceType=typeof(ResSAP))]
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
