using System;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.Models
{
    public class publicEmailVM : ModelBase
    {
        private String m_emailTo;
        [Validate(ValidateType.Required)]
        public String EmailTo
        {
            get { return this.m_emailTo; }
            set { this.SetValue("EmailTo", ref m_emailTo, value); }
        }

        private String m_content;
        [Validate(ValidateType.Required)]
        public String SendContent
        {
            get { return this.m_content; }
            set { this.SetValue("SendContent", ref m_content, value); }
        }
    }
}
