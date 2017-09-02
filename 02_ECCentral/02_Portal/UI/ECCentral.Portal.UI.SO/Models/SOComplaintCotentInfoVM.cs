using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.SO.Models
{
   public class SOComplaintCotentInfoVM : ModelBase
   {
       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private String m_ComplainID;
       public String ComplainID
       {
           get { return this.m_ComplainID; }
           set { this.SetValue("ComplainID", ref m_ComplainID, value);  }
       }

       private Int32? m_SOSysNo;
       [Validate(ValidateType.Interger)]
       public Int32? SOSysNo
       {
           get { return this.m_SOSysNo; }
           set { this.SetValue("SOSysNo", ref m_SOSysNo, value);  }
       }

       private String m_ComplainType;

       public String ComplainType
       {
           get { return this.m_ComplainType; }
           set { this.SetValue("ComplainType", ref m_ComplainType, value);  }
       }

       private String m_ComplainSourceType;
       public String ComplainSourceType
       {
           get { return this.m_ComplainSourceType; }
           set { this.SetValue("ComplainSourceType", ref m_ComplainSourceType, value);  }
       }

       private String m_Subject;
       [Validate(ValidateType.Required)]
       public String Subject
       {
           get { return this.m_Subject; }
           set { this.SetValue("Subject", ref m_Subject, value);  }
       }

       private Int32? m_CustomerSysNo;
       public Int32? CustomerSysNo
       {
           get { return this.m_CustomerSysNo; }
           set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value);  }
       }

       private String m_CustomerName;
       public String CustomerName
       {
           get { return this.m_CustomerName; }
           set { this.SetValue("CustomerName", ref m_CustomerName, value); }
       }

       private String m_CustomerEmail;
       [Validate(ValidateType.Email)]
       [Validate(ValidateType.Required)]
       public String CustomerEmail
       {
           get { return this.m_CustomerEmail; }
           set { this.SetValue("CustomerEmail", ref m_CustomerEmail, value);  }
       }

       private String m_CustomerPhone;
       [Validate(ValidateType.Regex, RegexHelper.Phone)]
       public String CustomerPhone
       {
           get { return this.m_CustomerPhone; }
           set { this.SetValue("CustomerPhone", ref m_CustomerPhone, value);  }
       }

       private String m_ComplainContent;
       [Validate(ValidateType.Required)]
       public String ComplainContent
       {
           get { return this.m_ComplainContent; }
           set { this.SetValue("ComplainContent", ref m_ComplainContent, value);  }
       }

       private DateTime? m_ComplainTime;
       public DateTime? ComplainTime
       {
           get { return this.m_ComplainTime; }
           set { this.SetValue("ComplainTime", ref m_ComplainTime, value);  }
       }

       private string m_CompanyCode;
       public string CompanyCode
       {
           get { return this.m_CompanyCode; }
           set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
       }
   }
}
