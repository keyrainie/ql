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
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class SOFreightStatDetailQueryVM : ModelBase
    {
        private string soSysNo;
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get { return soSysNo; }
            set { base.SetValue("SOSysNo", ref soSysNo, value); }
        }

        private RealFreightStatus? realFreightConfirm;
        public RealFreightStatus? RealFreightConfirm
        {
            get { return realFreightConfirm; }
            set { base.SetValue("RealFreightConfirm", ref realFreightConfirm, value); }
        }

        private CheckStatus? soFreightConfirm;
        public CheckStatus? SOFreightConfirm
        {
            get { return soFreightConfirm; }
            set { base.SetValue("SOFreightConfirm", ref soFreightConfirm, value); }
        }
    }
}
