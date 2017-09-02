using System.Collections.ObjectModel;

using ECCentral.BizEntity.IM;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductDomainQueryVM : ModelBase
    {
        public ProductDomainQueryVM()
        {
            
        }

        private int? productDomainSysNo;
        public int? ProductDomainSysNo
        {
            get
            {
                return productDomainSysNo;
            }
            set
            {
                SetValue("ProductDomainSysNo", ref productDomainSysNo, value);
            }
        }

        private int? pmSysNo;
		public int? PMSysNo 
		{ 
			get
			{
				return pmSysNo;
			}			
			set
			{
				SetValue("PMSysNo", ref pmSysNo, value);
			} 
		}		

        private int? category1SysNo;
        public int? Category1SysNo
        {
            get
            {
                return category1SysNo;
            }
            set
            {
                SetValue("Category1SysNo", ref category1SysNo, value);
            }
        }

        private int? category2SysNo;
        public int? Category2SysNo
        {
            get
            {
                return category2SysNo;
            }
            set
            {
                SetValue("Category2SysNo", ref category2SysNo, value);
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

        private int? productDomainLeaderUserSysNo;
        public int? ProductDomainLeaderUserSysNo
        {
            get
			{
                return productDomainLeaderUserSysNo;
			}
            set
            {
                SetValue("ProductDomainLeaderUserSysNo", ref productDomainLeaderUserSysNo, value);
            }
        }

        private bool asAggregateStyle;
        public bool AsAggregateStyle
        {
            get
            {
                return asAggregateStyle;
            }
            set
            {
                SetValue("AsAggregateStyle", ref asAggregateStyle, value);
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
