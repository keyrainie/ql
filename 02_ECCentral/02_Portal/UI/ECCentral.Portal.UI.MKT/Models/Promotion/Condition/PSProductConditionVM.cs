using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSProductConditionViewModel : ModelBase
   {      

       private RelCategory3ViewModel m_RelCategories;
       public RelCategory3ViewModel RelCategories
       {
           get { return this.m_RelCategories; }
           set { this.SetValue("RelCategories", ref m_RelCategories, value);  }
       }

       private RelBrandViewModel m_RelBrands;
       public RelBrandViewModel RelBrands
       {
           get { return this.m_RelBrands; }
           set { this.SetValue("RelBrands", ref m_RelBrands, value);  }
       }

       private RelProductViewModel m_RelProducts;
       public RelProductViewModel RelProducts
       {
           get { return this.m_RelProducts; }
           set { this.SetValue("RelProducts", ref m_RelProducts, value);  }
       }
       /// <summary>
       /// 限定商家集合
       /// </summary>
       private List<RelVendorViewModel> listRelVendorViewModel;
       public List<RelVendorViewModel> ListRelVendorViewModel
       {
           get { return this.listRelVendorViewModel; }
           set { SetValue("ListRelVendorViewModel", ref listRelVendorViewModel, value); }
       }


   }

   public class RelCategory3ViewModel : ModelBase
   {
       private bool? m_IsIncludeRelation;
       public bool? IsIncludeRelation
       {
           get { return this.m_IsIncludeRelation; }
           set { this.SetValue("IsIncludeRelation", ref m_IsIncludeRelation, value); }
       }

       private bool? m_IsExcludeRelation;
       public bool? IsExcludeRelation
       {
           get { return this.m_IsExcludeRelation; }
           set { this.SetValue("IsExcludeRelation", ref m_IsExcludeRelation, value); }
       }

       private List<SimpleObjectViewModel> m_CategoryList;
       public List<SimpleObjectViewModel> CategoryList
       {
           get { return this.m_CategoryList; }
           set { this.SetValue("CategoryList", ref m_CategoryList, value); }
       }

   }

   public class RelBrandViewModel : ModelBase
   {
       private bool? m_IsIncludeRelation;
       public bool? IsIncludeRelation
       {
           get { return this.m_IsIncludeRelation; }
           set { this.SetValue("IsIncludeRelation", ref m_IsIncludeRelation, value); }
       }

       private bool? m_IsExcludeRelation;
       public bool? IsExcludeRelation
       {
           get { return this.m_IsExcludeRelation; }
           set { this.SetValue("IsExcludeRelation", ref m_IsExcludeRelation, value); }
       }

       private List<SimpleObjectViewModel> m_BrandList;
       public List<SimpleObjectViewModel> BrandList
       {
           get { return this.m_BrandList; }
           set { this.SetValue("BrandList", ref m_BrandList, value); }
       }

   }

   public class RelProductViewModel : ModelBase
   {
       public RelProductViewModel()
       {
           IsIncludeRelation = true;
           IsExcludeRelation = false;
       }

       private bool? m_IsIncludeRelation;
       public bool? IsIncludeRelation
       {
           get { return this.m_IsIncludeRelation; }
           set { this.SetValue("IsIncludeRelation", ref m_IsIncludeRelation, value); }
       }

       private bool? m_IsExcludeRelation;
       public bool? IsExcludeRelation
       {
           get { return this.m_IsExcludeRelation; }
           set { this.SetValue("IsExcludeRelation", ref m_IsExcludeRelation, value); }
       }


       private ObservableCollection<RelProductAndQtyViewModel> m_ProductList;
       public ObservableCollection<RelProductAndQtyViewModel> ProductList
       {
           get { return this.m_ProductList; }
           set { this.SetValue("ProductList", ref m_ProductList, value); }
       }

   }

    /// <summary>
    /// 限定商家VM
    /// </summary>
   public class RelVendorViewModel : ModelBase
   {

       /// <summary>
       /// 是否选中
       /// </summary>
       private bool isChecked;
       public bool IsChecked
       {
           get { return isChecked; }
           set { base.SetValue("IsChecked", ref isChecked, value); }
       }

       /// <summary>
       /// 系统编号
       /// </summary>
       public int VendorSysNo { get; set; }
        /// <summary>
       /// 商家名称
       /// </summary>
       public string VendorName { get; set; }


       /// <summary>
       /// 活动状态
       /// </summary>
       public CouponsStatus? CouponsStatus { get; set; }
   }
}
