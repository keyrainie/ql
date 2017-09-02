using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductGroupMaintainPropertySettingVM : ModelBase
    {
        public ProductGroupMaintainPropertySettingVM()
        {
            ProductGroupSettings = new List<ProductGroupSettingVM>();
            ProductGroupSettings.Add(new ProductGroupSettingVM());
            ProductGroupSettings.Add(new ProductGroupSettingVM());
            CategoryPropertyList = new List<PropertyVM>();
        }

        public List<ProductGroupSettingVM> ProductGroupSettings { get; set; }

        private List<PropertyVM> _categoryPropertyList;

        public List<PropertyVM> CategoryPropertyList
        {
            get { return _categoryPropertyList; }
            set { SetValue("CategoryPropertyList", ref _categoryPropertyList, value); }
        }
    }
}
