using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryRequestApprovalQueryVM:ModelBase
    {
        public CategoryRequestApprovalQueryVM()
        {
            CategoryTypeList = EnumConverter.GetKeyValuePairs<CategoryType>(EnumConverter.EnumAppendItemType.None);
        }

        private CategoryType? _category = CategoryType.CategoryType1;
        public CategoryType? Category {
            get
            {
                return _category;
            }
            set
            {
                SetValue("Category", ref _category, value);
            }
        }

        public List<KeyValuePair<CategoryType?, string>> CategoryTypeList { get; set; }

    }
}
