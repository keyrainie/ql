using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainPropertyPropertyValueVM : ModelBase
    {
        public int? SysNo { get; set; }

        public PropertyGroupInfoVM PropertyGroupInfo { get; set; }

        public PropertyVM Property { get; set; }

        public PropertyValueVM ProductPropertyValue { get; set; }

        private String _personalizedValue;

        public String PersonalizedValue
        {
            get { return _personalizedValue; }
            set { SetValue("PersonalizedValue", ref _personalizedValue, value); }
        }

        public List<PropertyValueVM> PropertyValueList { get; set; }

        public PropertyType PropertyType { get; set; }

        public String RequiredColor { get; set; }

        public bool CanPersonalizedValueInput { get; set; }
    }
}
