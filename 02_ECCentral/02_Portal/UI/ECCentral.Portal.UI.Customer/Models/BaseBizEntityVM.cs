using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Customer.Models
{
   public class BaseBizEntityViewModel : ModelBase
   {
       private String m_InUserAcct;
       public String InUserAcct
       {
           get { return this.m_InUserAcct; }
           set { this.SetValue("InUserAcct", ref m_InUserAcct, value);  }
       }

       private Int32? m_InUserSysNo;
       public Int32? InUserSysNo
       {
           get { return this.m_InUserSysNo; }
           set { this.SetValue("InUserSysNo", ref m_InUserSysNo, value);  }
       }

       private DateTime? m_InDate;
       public DateTime? InDate
       {
           get { return this.m_InDate; }
           set { this.SetValue("InDate", ref m_InDate, value);  }
       }

       private String m_EditUserAcct;
       public String EditUserAcct
       {
           get { return this.m_EditUserAcct; }
           set { this.SetValue("EditUserAcct", ref m_EditUserAcct, value);  }
       }

       private Int32? m_EditUserSysNo;
       public Int32? EditUserSysNo
       {
           get { return this.m_EditUserSysNo; }
           set { this.SetValue("EditUserSysNo", ref m_EditUserSysNo, value);  }
       }

       private DateTime? m_EditDate;
       public DateTime? EditDate
       {
           get { return this.m_EditDate; }
           set { this.SetValue("EditDate", ref m_EditDate, value);  }
       }

       private String m_CompanyCode;
       public String CompanyCode
       {
           get { return this.m_CompanyCode; }
           set { this.SetValue("CompanyCode", ref m_CompanyCode, value);  }
       }

       private String m_StoreCompanyCode;
       public String StoreCompanyCode
       {
           get { return this.m_StoreCompanyCode; }
           set { this.SetValue("StoreCompanyCode", ref m_StoreCompanyCode, value);  }
       }

       private String m_LanguageCode;
       public String LanguageCode
       {
           get { return this.m_LanguageCode; }
           set { this.SetValue("LanguageCode", ref m_LanguageCode, value);  }
       }

       private WebChannel m_WebChannel;
       public WebChannel WebChannel
       {
           get { return this.m_WebChannel; }
           set { this.SetValue("WebChannel", ref m_WebChannel, value);  }
       }

   }
}
