using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class CouponCodeQueryFilterViewModel : ModelBase
   {
       private PagingInfo m_PageInfo;
       public PagingInfo PageInfo
       {
           get { return this.m_PageInfo; }
           set { this.SetValue("PageInfo", ref m_PageInfo, value);  }
       }

       private Int32? m_CouponSysNo;
       public Int32? CouponSysNo
       {
           get { return this.m_CouponSysNo; }
           set { this.SetValue("CouponSysNo", ref m_CouponSysNo, value);  }
       }

       private string m_CodeSysNoFrom;
       [Validate(ValidateType.Interger)]
       public string CodeSysNoFrom
       {
           get { return this.m_CodeSysNoFrom; }
           set { this.SetValue("CodeSysNoFrom", ref m_CodeSysNoFrom, value);  }
       }

       private string m_CodeSysNoTo;
       [Validate(ValidateType.Interger)]
       public string CodeSysNoTo
       {
           get { return this.m_CodeSysNoTo; }
           set { this.SetValue("CodeSysNoTo", ref m_CodeSysNoTo, value);  }
       }

       private String m_CouponCode;
       public String CouponCode
       {
           get { return this.m_CouponCode; }
           set { this.SetValue("CouponCode", ref m_CouponCode, value);  }
       }

       private DateTime? m_InDateFrom;
       public DateTime? InDateFrom
       {
           get { return this.m_InDateFrom; }
           set { this.SetValue("InDateFrom", ref m_InDateFrom, value);  }
       }

       private DateTime? m_InDateTo;
       public DateTime? InDateTo
       {
           get { return this.m_InDateTo; }
           set { this.SetValue("InDateTo", ref m_InDateTo, value);  }
       }

       private DateTime? m_BeginDateFrom;
       public DateTime? BeginDateFrom
       {
           get { return this.m_BeginDateFrom; }
           set { this.SetValue("BeginDateFrom", ref m_BeginDateFrom, value);  }
       }

       private DateTime? m_EndDateTo;
       public DateTime? EndDateTo
       {
           get { return this.m_EndDateTo; }
           set { this.SetValue("EndDateTo", ref m_EndDateTo, value);  }
       }

   }
}
