using System;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductGroupSettingVM : ModelBase
    {
        public ProductGroupSettingVM()
        {
            ProductGroupProperty = new PropertyVM();
        }


        public PropertyVM ProductGroupProperty { get; set; }

        private String _propertyBriefName;

        public String PropertyBriefName
        {
            get { return _propertyBriefName; }
            set { SetValue("PropertyBriefName", ref _propertyBriefName, value); }
        }

        private ProductGroupImageShow _imageShow;

        public ProductGroupImageShow ImageShow
        {
            get { return _imageShow; }
            set { SetValue("ImageShow", ref _imageShow, value); }
        }

        private ProductGroupPolymeric _polymeric;

        public ProductGroupPolymeric Polymeric
        {
            get { return _polymeric; }
            set { SetValue("Polymeric", ref _polymeric, value); }
        }
    }
}
