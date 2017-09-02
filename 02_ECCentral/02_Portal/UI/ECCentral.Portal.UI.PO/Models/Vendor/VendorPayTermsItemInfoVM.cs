using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorPayTermsItemInfoVM : ModelBase
    {
        private int? payTermsNo;

        public int? PayTermsNo
        {
            get { return payTermsNo; }
            set { base.SetValue("PayTermsNo", ref payTermsNo, value); }
        }

        /// <summary>
        /// 是否代销
        /// </summary>
        private int? isConsignment;

        public int? IsConsignment
        {
            get { return isConsignment; }
            set { base.SetValue("IsConsignment", ref isConsignment, value); }
        }

        /// <summary>
        /// 账期计算公式(前台显示 ）
        /// </summary>
        private string discribComputer;
        public string DiscribComputer
        {
            get { return discribComputer; }
            set { base.SetValue("DiscribComputer", ref discribComputer, value); }
        }

        private string payTermsName;
        public string PayTermsName
        {
            get { return payTermsName; }
            set { base.SetValue("PayTermsName", ref payTermsName, value); }
        }

    }
}
