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
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.NeweggCN.Models
{
    public class QCSubjectVM : ModelBase
    {
        public QCSubjectVM()
        {
            CompanyCode = CPApplication.Current.CompanyCode;
        }
        private int? _SysNo;

        public int? SysNo
        {
            get { return _SysNo; }
            set { base.SetValue("SysNo", ref _SysNo, value); }
        }

        private string _Name;
        [Validate(ValidateType.Required)]
        public string Name
        {
            get { return _Name; }
            set { base.SetValue("Name", ref _Name, value); }
        }
        private string _OrderNum;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string OrderNum
        {
            get { return _OrderNum; }
            set { base.SetValue("OrderNum", ref _OrderNum, value); }
        }
        private QCSubjectStatus? _Status;
        [Validate(ValidateType.Required)]
        public QCSubjectStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }
        private string _ParentSysNo;

        [Validate(ValidateType.Required)]
        public string ParentSysNo
        {
            get { return _ParentSysNo; }
            set { base.SetValue("ParentSysNo", ref _ParentSysNo, value); }
        }

        private string _CompanyCode;

        public string CompanyCode
        {
            get { return _CompanyCode; }
            set { base.SetValue("CompanyCode", ref _CompanyCode, value); }
        }

        private WebChannel _WebChannel;

        public WebChannel WebChannel
        {
            get { return _WebChannel; }
            set { base.SetValue("WebChannel", ref _WebChannel, value); }
        }
    }
}
