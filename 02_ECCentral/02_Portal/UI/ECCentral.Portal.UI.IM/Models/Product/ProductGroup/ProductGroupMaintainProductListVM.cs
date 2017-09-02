using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductGroupMaintainProductListVM : ModelBase
    {
        public ProductGroupMaintainProductListVM()
        {
            ProductGroupProductVMList = new List<ProductGroupProductVM>();
        }

        private List<ProductGroupProductVM> _productGroupProductList;

        public List<ProductGroupProductVM> ProductGroupProductVMList
        {
            get { return _productGroupProductList; }
            set { SetValue("ProductGroupProductVMList", ref _productGroupProductList, value); }
        }

        public ProductGroupMaintainVM MainPageVM
        {
            get { return ((Views.ProductGroupMaintain)CPApplication.Current.CurrentPage).VM; }
        }
    }

    public class ProductGroupProductVM : ModelBase
    {
        public int ProductSysNo { get; set; }

        public String ProductID { get; set; }

        public String ProductTitle { get; set; }

        public String ProductModel { get; set; }

        public BrandVM ProductBrand { get; set; }

        public CategoryVM ProductCategory { get; set; }

        public ProductStatus ProductStatus { get; set; }

        public decimal? ProductCurrentPrice { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetValue("IsChecked", ref _isChecked, value); }
        }

        public override bool Equals(object obj)
        {
            if (obj is ProductGroupProductVM)
            {
                var p = obj as ProductGroupProductVM;
                if (p.ProductSysNo == ProductSysNo)
                {
                    return true;
                }
            }
            return false;
        }
    }











    public class ProductSimilarItemVM : ModelBase
    {
        public int ProductGroupSysNo { get; set; }

        public IList<GroupInfoVM> GroupInfos { get; set; }

        public ProductGroup ProductGroup { get; set; }

        public IList<PropertyValueInfo> ProductGroupPropertyValueInfo { get; set; }
    }

    public class GroupInfoVM : ModelBase
    {
        public bool IsChecked { get; set; }

        public string GroupInfoDesc { get; set; }

        public string GroupInfoValue { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GroupInfoVM)
            {
                var o = obj as GroupInfoVM;
                var temparrays = o.GroupInfoDesc.Split('+');
                var arrays = GroupInfoDesc.Split('+');
                var result = temparrays.Except(arrays);
                return result == null || result.Count() == 0;
            }
            return false;
        }


        public string ProductModel { get; set; }
    }
}
