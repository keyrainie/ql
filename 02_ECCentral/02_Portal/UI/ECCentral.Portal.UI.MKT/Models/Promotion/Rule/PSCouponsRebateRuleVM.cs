using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSCouponsRebateRuleViewModel : ModelBase
   {
       private Decimal? m_CouponRebate;
       public Decimal? CouponRebate
       {
           get { return this.m_CouponRebate; }
           set { this.SetValue("CouponRebate", ref m_CouponRebate, value);  }
       }

       private String m_CouponCode;
       public String CouponCode
       {
           get { return this.m_CouponCode; }
           set { this.SetValue("CouponCode", ref m_CouponCode, value);  }
       }
        
   }
}
