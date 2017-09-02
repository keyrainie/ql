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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models.Vendor
{
    public class CommissionRuleTemplateQueryVM : ModelBase
    {

        private int? _C1SysNo;

        public int? C1SysNo
        {
            get { return _C1SysNo; }
            set { base.SetValue("C1SysNo", ref _C1SysNo, value); }
        }


        private int? _C2SysNo;

        public int? C2SysNo
        {
            get { return _C2SysNo; }
            set { base.SetValue("C2SysNo", ref _C2SysNo, value); }
        }


        private int? _C3SysNo;

        public int? C3SysNo
        {
            get { return _C3SysNo; }
            set { base.SetValue("C3SysNo", ref _C3SysNo, value); }
        }



        private CommissionRuleStatus? _Status;

        public CommissionRuleStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }



        private int? _BrandSysNo;

        public int? BrandSysNo
        {
            get { return _BrandSysNo; }
            set { base.SetValue("BrandSysNo", ref _BrandSysNo, value); }
        }



    }
}
