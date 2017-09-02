using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class CouponCodeSettingViewModel : ModelBase
   { 
       private Int32? m_CouponsSysNo;
       public Int32? CouponsSysNo
       {
           get { return this.m_CouponsSysNo; }
           set { this.SetValue("CouponsSysNo", ref m_CouponsSysNo, value); }
       }


       private DateTime? m_StartTime;
       public DateTime? StartTime
       {
           get { return this.m_StartTime; }
           set { this.SetValue("StartTime", ref m_StartTime, value);  }
       }

       private DateTime? m_EndTime;
       public DateTime? EndTime
       {
           get { return this.m_EndTime; }
           set { this.SetValue("EndTime", ref m_EndTime, value);  }
       }

       private CouponCodeType? m_CouponCodeType;
       public CouponCodeType? CouponCodeType
       {
           get { return this.m_CouponCodeType; }
           set { this.SetValue("CouponCodeType", ref m_CouponCodeType, value);  }
       }

       private string m_DueInvertRate;
       [Validate(ValidateType.Regex, "^[0-9]+(.[0-9]{1,2})?$",ErrorMessage="只能输入1-2位的正实数")]    
       public string DueInvertRate
       {
           get { return this.m_DueInvertRate; }
           set { this.SetValue("DueInvertRate", ref m_DueInvertRate, value);  }
       }

       private string m_CCCustomerMaxFrequency;
       [Validate(ValidateType.Interger)]
       public string CCCustomerMaxFrequency
       {
           get { return this.m_CCCustomerMaxFrequency; }
           set { this.SetValue("CCCustomerMaxFrequency", ref m_CCCustomerMaxFrequency, value);  }
       }

       private string m_CCMaxFrequency;
       [Validate(ValidateType.Interger)]
       public string CCMaxFrequency
       {
           get { return this.m_CCMaxFrequency; }
           set { this.SetValue("CCMaxFrequency", ref m_CCMaxFrequency, value);  }
       }

       private Int32? m_UsedCount;
       public Int32? UsedCount
       {
           get { return this.m_UsedCount; }
           set { this.SetValue("UsedCount", ref m_UsedCount, value);  }
       }

       private Int32? m_UsedAmount;
       public Int32? UsedAmount
       {
           get { return this.m_UsedAmount; }
           set { this.SetValue("UsedAmount", ref m_UsedAmount, value);  }
       }

       private Decimal? m_TotalDiscount;
       public Decimal? TotalDiscount
       {
           get { return this.m_TotalDiscount; }
           set { this.SetValue("TotalDiscount", ref m_TotalDiscount, value);  }
       }

       private  String  m_CouponCode;
       public String  CouponCode
       {
           get { return this.m_CouponCode; }
           set { this.SetValue("CouponCode", ref m_CouponCode, value); }
       }


       private String m_InUser;
       public String InUser
       {
           get { return this.m_InUser; }
           set { this.SetValue("InUser", ref m_InUser, value);  }
       }

       private DateTime? m_InDate;
       public DateTime? InDate
       {
           get { return this.m_InDate; }
           set { this.SetValue("InDate", ref m_InDate, value);  }
       }

       private string m_ThrowInCodeCount;
       [Validate(ValidateType.Interger)]
       [Validate(ValidateType.Regex, @"^(?:[1-9]\d{0,2}|[12]\d{3}|3000)$", ErrorMessage = "输入1-3000的正整数!")]
      public string ThrowInCodeCount
       {
           get { return this.m_ThrowInCodeCount; }
           set { this.SetValue("ThrowInCodeCount", ref m_ThrowInCodeCount, value); }
       }

   }
}
