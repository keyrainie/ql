using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductDomainVM : ModelBase
    {
        public ProductDomainVM()
        {
            this.DepartmentCategoryList = new ObservableCollection<ProductDepartmentCategoryVM>();
            this.DepartmentMerchandiserListForUI = new ObservableCollection<DepartmentMerchandiserVM>();
            this.DepartmentMerchandiserSysNoList = new List<int?>();
        }

        private int? sysNo;
		public int? SysNo 
		{ 
			get
			{
				return sysNo;
			}			
			set
			{
				SetValue("SysNo", ref sysNo, value);
			} 
		}
		
        private string productDomainName;
        [Validate(ValidateType.Required)]
		public string ProductDomainName 
		{ 
			get
			{
				return productDomainName;
			}			
			set
			{
				SetValue("ProductDomainName", ref productDomainName, value);
			} 
		}
		
        private int? productDomainLeaderUserSysNo;
        [Validate(ValidateType.Required)]
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

        public List<int?> DepartmentMerchandiserSysNoList { get; set; }

        public ObservableCollection<ProductDepartmentCategoryVM> DepartmentCategoryList { get; set; }
        
        public ObservableCollection<DepartmentMerchandiserVM> DepartmentMerchandiserListForUI { get; set; }
    }
}
