using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorDeductInfoVM : ModelBase
    {

        public VendorDeductInfoVM()
        {
          
        }

        private int deductSysNo;
      
        public int DeductSysNo
        {
            get { return deductSysNo; }
            set { base.SetValue("DeductSysNo", ref deductSysNo, value); }
        }

        private VendorCalcType? calcType;
        public VendorCalcType? CalcType
        {
            get { return calcType; }
            set { base.SetValue("CalcType", ref calcType, value); }
        }
        private string deductPercent;
        [Validate(ValidateType.Regex, @"^((100(\.00|\.0|\.)?)|(\d?\d(\.\d\d|\.\d|\.)?))$", ErrorMessage="请输入100以内的正数")]
        public string DeductPercent
        {
            get { return deductPercent; }
            set { base.SetValue("DeductPercent", ref deductPercent, value); }
        }

        private string fixAmt;
         [Validate(ValidateType.Regex, @"^(\d{0,9}\.[0-9]{0,2}|\d{0,9})$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string FixAmt
        {
            get { return fixAmt; }
            set { base.SetValue("FixAmt", ref fixAmt, value); }
        }

         private string maxAmt;
         [Validate(ValidateType.Regex, @"^(\d{0,9}\.[0-9]{0,2}|\d{0,9})$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
         public string MaxAmt
        {
            get { return maxAmt; }
            set { base.SetValue("MaxAmt", ref maxAmt, value); }
        }      
    }
}
