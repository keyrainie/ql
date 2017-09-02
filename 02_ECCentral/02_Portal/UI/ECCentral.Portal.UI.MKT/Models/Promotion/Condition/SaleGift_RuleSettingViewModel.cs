using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class SaleGift_RuleSettingViewModel : ModelBase
   {
       public SaleGift_RuleSettingViewModel()
       {
           m_RelC3 = new SimpleObjectViewModel();
           m_RelBrand = new SimpleObjectViewModel();
           m_RelProduct = new RelProductAndQtyViewModel();
       }

       private SimpleObjectViewModel m_RelC3;
       public SimpleObjectViewModel RelC3
       {
           get { return this.m_RelC3; }
           set { this.SetValue("RelC3", ref m_RelC3, value);  }
       }

       private SimpleObjectViewModel m_RelBrand;
       public SimpleObjectViewModel RelBrand
       {
           get { return this.m_RelBrand; }
           set { this.SetValue("RelBrand", ref m_RelBrand, value);  }
       }

       private RelProductAndQtyViewModel m_RelProduct;
       public RelProductAndQtyViewModel RelProduct
       {
           get { return this.m_RelProduct; }
           set { this.SetValue("RelProduct", ref m_RelProduct, value);  }
       }

       private SaleGiftSaleRuleType? m_Type;
       public SaleGiftSaleRuleType? Type
       {
           get { return this.m_Type; }
           set { this.SetValue("Type", ref m_Type, value);  }
       }
       
       private AndOrType? m_ComboType;
       public AndOrType? ComboType
       {
           get { return this.m_ComboType; }
           set { this.SetValue("ComboType", ref m_ComboType, value);  }
       }

       private bool isChecked=false;
       public bool IsChecked
       {
           get { return isChecked; }
           set { base.SetValue("IsChecked", ref isChecked, value); }
       }
       
   }
}
