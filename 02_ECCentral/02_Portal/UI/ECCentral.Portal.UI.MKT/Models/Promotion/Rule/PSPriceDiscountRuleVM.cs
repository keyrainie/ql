using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSPriceDiscountRuleViewModel : ModelBase
   {
       private Int32? m_ProductSysNo;
       public Int32? ProductSysNo
       {
           get { return this.m_ProductSysNo; }
           set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value);  }
       }

       private PSDiscountTypeForProductPrice? m_DiscountType;
       public PSDiscountTypeForProductPrice? DiscountType
       {
           get { return this.m_DiscountType; }
           set { this.SetValue("DiscountType", ref m_DiscountType, value);  }
       }

       private Decimal? m_DiscountValue;
       public Decimal? DiscountValue
       {
           get { return this.m_DiscountValue; }
           set { this.SetValue("DiscountValue", ref m_DiscountValue, value);  }
       }

       private Int32? m_MinQty;
       public Int32? MinQty
       {
           get { return this.m_MinQty; }
           set { this.SetValue("MinQty", ref m_MinQty, value);  }
       }

       private Int32? m_MaxQty;
       public Int32? MaxQty
       {
           get { return this.m_MaxQty; }
           set { this.SetValue("MaxQty", ref m_MaxQty, value); }
       }
   }
}
