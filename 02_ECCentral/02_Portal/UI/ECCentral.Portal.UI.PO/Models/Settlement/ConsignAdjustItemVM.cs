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
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignAdjustItemVM : ModelBase
    {
        /// <summary>
        /// 扣款项编号
        /// </summary>
        private int deductSysNo;
        public int DeductSysNo
        {
            get { return deductSysNo; }
            set { base.SetValue("DeductSysNo", ref deductSysNo, value); }
        }

        private decimal deductAmt;
        [RequiredField]
        [Validate(ValidateType.Regex, @"^(\d{0,9}\.[0-9]{0,2}|\d{0,9})$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public decimal DeductAmt
        {
            get { return deductAmt; }
            set { base.SetValue("DeductAmt", ref deductAmt, value); }
        }

        private string deductName;
        public string DeductName
        {
            get { return deductName; }
            set { base.SetValue("DeductName", ref deductName, value); }
        }

        private string consignAdjustSysNo;
        public string ConsignAdjustSysNo
        {
            get { return consignAdjustSysNo; }
            set { base.SetValue("ConsignAdjustSysNo", ref consignAdjustSysNo, value); }
        }

        private AccountType accountType;
        public AccountType AccountType
        {
            get { return accountType; }
            set { base.SetValue("AccountType", ref accountType, value); }
        }
        private DeductMethod deductMethod;
        public DeductMethod DeductMethod
        {
            get { return deductMethod; }
            set { base.SetValue("DeductMethod", ref deductMethod, value); }
        }
        private bool isCheckedItem;
        public bool IsCheckedItem {

            get { return isCheckedItem; }
            set { base.SetValue("IsCheckedItem", ref isCheckedItem, value); }
        }

    }
}
