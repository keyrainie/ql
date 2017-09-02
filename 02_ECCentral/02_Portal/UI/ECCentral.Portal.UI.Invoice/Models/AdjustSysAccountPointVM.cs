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
using System.Collections.Generic;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class AdjustSysAccountPointVM : ModelBase
    {
        public AdjustSysAccountPointVM() 
        {
            SysAccountList = new List<CustomerBasicInfo>();
        }
        private string customerSysNo;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set { base.SetValue("CustomerSysNo", ref customerSysNo, value); }
        }

        private string point;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string Point
        {
            get { return point; }
            set { base.SetValue("Point", ref point, value); }
        }

        private string memo;
        public string Memo
        {
            get { return memo; }
            set { base.SetValue("Memo", ref memo, value); }
        }

        public List<CustomerBasicInfo> sysAccountList;
        public List<CustomerBasicInfo> SysAccountList
        {
            get { return sysAccountList; }
            set { base.SetValue("SysAccountList", ref sysAccountList, value); }
        }
    }
}
