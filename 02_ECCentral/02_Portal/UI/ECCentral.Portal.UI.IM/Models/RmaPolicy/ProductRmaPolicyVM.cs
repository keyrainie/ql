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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductRmaPolicyVM:ModelBase
    {
        public int? RmaPolicySysNo { get; set; }
        private string returnDate;
           [Validate(ValidateType.Interger)]
        public string ReturnDate 
        {
            get { return returnDate; }
            set { SetValue("ReturnDate", ref returnDate, value); }
        }
        private string changeDate;
           [Validate(ValidateType.Interger)]
        public string ChangeDate
        {
            get { return changeDate; }
            set { SetValue("ChangeDate", ref changeDate, value); }
        }
        private bool isRequest;
        public bool IsRequest 
        {
            get { return isRequest; }
            set { SetValue("IsRequest", ref isRequest, value); }
        }
        private bool isBrandWarranty;
        public bool IsBrandWarranty 
        {
            get { return isBrandWarranty; }
            set { SetValue("IsBrandWarranty", ref isBrandWarranty, value);  }
        }

        private string warrantyDay;
         [Validate(ValidateType.Regex, @"^1$|^[1-9]\d{0,7}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_MustInput1ToMaxInt")]
         //[Validate(ValidateType.Required)]
        public string  WarrantyDay
        {
            get { return warrantyDay; }
            set { SetValue("WarrantyDay", ref warrantyDay, value); }
        }
        private string warrantyDesc;
        //[Validate(ValidateType.Required)]
        public string WarrantyDesc
        {
            get { return warrantyDesc; }
            set { SetValue("WarrantyDesc", ref warrantyDesc, value); }
        }
    }
}
