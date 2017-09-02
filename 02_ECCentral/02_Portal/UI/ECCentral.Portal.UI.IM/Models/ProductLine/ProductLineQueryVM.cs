using System.Collections.ObjectModel;

using ECCentral.BizEntity.IM;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductLineQueryVM : ModelBase
    {
        public ProductLineQueryVM()
        {
            PMRangeTypeList = EnumConverter.GetKeyValuePairs<PMRangeType>(EnumConverter.EnumAppendItemType.All);
        }

        private int? productLineSysNo;
        public int? ProductLineSysNo
        {
            get
            {
                return productLineSysNo;
            }
            set
            {
                SetValue("ProductLineSysNo", ref productLineSysNo, value);
            }
        }

        private int? pmUserSysNo;
        public int? PMUserSysNo 
		{ 
			get
			{
                return pmUserSysNo;
			}			
			set
			{
                SetValue("PMUserSysNo", ref pmUserSysNo, value);
			} 
		}

        private string pmUserName;
        public string PMUserName
        {
            get
            {
                return pmUserName;
            }
            set
            {
                SetValue("PMUserName", ref pmUserName, value);
            }
        }

        private PMRangeType? pmRangeType = null;
        public PMRangeType? PMRangeType
        {
            get
            {
                return pmRangeType;
            }
            set
            {
                SetValue("PMRangeType", ref pmRangeType, value);
            }
        }

        private List<KeyValuePair<PMRangeType?,string>> _pmRangeTypeList;

        public List<KeyValuePair<PMRangeType?, string>> PMRangeTypeList
        {
            get { return _pmRangeTypeList; }
            set { _pmRangeTypeList = value; }
        }

        private int? c1SysNo;
        public int? C1SysNo
        {
            get
            {
                return c1SysNo;
            }
            set
            {
                SetValue("C1SysNo", ref c1SysNo, value);
            }
        }

        private int? c2SysNo;
        public int? C2SysNo
        {
            get
            {
                return c2SysNo;
            }
            set
            {
                SetValue("C2SysNo", ref c2SysNo, value);
            }
        }

        private int? brandSysNo;
        public int? BrandSysNo
        {
            get
            {
                return brandSysNo;
            }
            set
            {
                SetValue("BrandSysNo", ref brandSysNo, value);
            }
        }

        private bool isSearchEmptyCategory;
        public bool IsSearchEmptyCategory
        {
            get
            {
                return isSearchEmptyCategory;
            }
            set
            {
                SetValue("IsSearchEmptyCategory", ref isSearchEmptyCategory, value);
            }
        }
    }
}
