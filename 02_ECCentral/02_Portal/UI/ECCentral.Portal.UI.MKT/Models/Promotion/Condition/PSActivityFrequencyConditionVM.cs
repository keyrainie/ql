using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSActivityFrequencyConditionViewModel : ModelBase
   {
       private string m_CustomerMaxFrequency;
       [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "必须是整数，且大于0")]
       public string CustomerMaxFrequency
       {
           get { return this.m_CustomerMaxFrequency; }
           set { this.SetValue("CustomerMaxFrequency", ref m_CustomerMaxFrequency, value);  }
       }

       private string m_CustomerUsedFrequency;
       public string CustomerUsedFrequency
       {
           get { return this.m_CustomerUsedFrequency; }
           set { this.SetValue("CustomerUsedFrequency", ref m_CustomerUsedFrequency, value);  }
       }

       private string m_MaxFrequency;
       [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "必须是整数，且大于0")]
       public string MaxFrequency
       {
           get { return this.m_MaxFrequency; }
           set { this.SetValue("MaxFrequency", ref m_MaxFrequency, value);  }
       }

       private string m_UsedFrequency;
       public string UsedFrequency
       {
           get { return this.m_UsedFrequency; }
           set { this.SetValue("UsedFrequency", ref m_UsedFrequency, value);  }
       }

   }
}
