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
using ECCentral.BizEntity.Enum;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class AmbassadorNewsVM : ModelBase
    {

        public string CompanyCode { get; set; }

      
        public int? SysNo { get; set; }


        private int? _ReferenceSysNo;

        public int? ReferenceSysNo
        {
            get { return _ReferenceSysNo; }
            set { base.SetValue("ReferenceSysNo", ref _ReferenceSysNo, value); }
        }



        private string _Title;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength,100)]
        public string Title
        {
            get { return _Title; }
            set { base.SetValue("Title", ref _Title, value); }
        }

        private string _Content;
        [Validate(ValidateType.Required)]
        public string Content
        {
            get { return _Content; }
            set { base.SetValue("Content", ref _Content, value); }
        }



        private AmbassadorNewsStatus? status;
        [Validate(ValidateType.Required)]
        public AmbassadorNewsStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

  
    }
}
