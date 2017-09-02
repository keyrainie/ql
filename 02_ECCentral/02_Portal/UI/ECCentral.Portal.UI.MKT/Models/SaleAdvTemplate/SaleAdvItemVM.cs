using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SaleAdvItemVM : ModelBase
    {
        public SaleAdvItemVM()
        {
            this.Groups = new ObservableCollection<SaleAdvGroupVM>();            

            this.StatusList = EnumConverter.GetKeyValuePairs<ADStatus>();
            this.Status = ADStatus.Active;

            this.RecommendTypeList = EnumConverter.GetKeyValuePairs<RecommendType>();
            this.RecommendType = BizEntity.MKT.RecommendType.Normal;
        }

        private bool isChecked;
		public bool IsChecked 
		{ 
			get
			{
				return isChecked;
			}			
			set
			{
				SetValue("IsChecked", ref isChecked, value);
			} 
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

        public int? SaleAdvSysNo { get; set; }

        private int? productSysNo;        
		public int? ProductSysNo 
		{ 
			get
			{
				return productSysNo;
			}			
			set
			{
				SetValue("ProductSysNo", ref productSysNo, value);
			} 
		}
       
        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public ProductStatus? ProductStatus { get; set; }

        public int? OnlineQty { get; set; }
		
        private string priority;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Regex, @"^[0-9]\d{0,2}$", ErrorMessage = "请输入1至999的整数！")]
        public string Priority 
		{ 
			get
			{
				return priority;
			}			
			set
			{
				SetValue("Priority", ref priority, value);
			} 
		}

        private RecommendType? recommendType;
        public RecommendType? RecommendType 
		{ 
			get
			{
                return recommendType;
			}			
			set
			{
                SetValue("RecommendType", ref recommendType, value);
			} 
		}

        private string iconAddr;
        [Validate(ValidateType.MaxLength, 200)]
        public string IconAddr
        {
            get
            {
                return iconAddr;
            }
            set
            {
                SetValue("IconAddr", ref iconAddr, value);
            }
        }
		
        private string groupName;
		public string GroupName 
		{ 
			get
			{
				return groupName;
			}			
			set
			{
				SetValue("GroupName", ref groupName, value);
			} 
		}
		
        private decimal? marketPrice;
		public decimal? MarketPrice 
		{ 
			get
			{
				return marketPrice;
			}			
			set
			{
                SetValue("MarketPrice", ref marketPrice, value);
			} 
		}
		
        private int? groupPriority;
        public int? GroupPriority 
		{ 
			get
			{
				return groupPriority;
			}			
			set
			{
				SetValue("GroupPriority", ref groupPriority, value);
			} 
		}		

        private int? groupSysNo;
		public int? GroupSysNo 
		{ 
			get
			{
				return groupSysNo;
			}			
			set
			{
				SetValue("GroupSysNo", ref groupSysNo, value);
                var group = this.Groups.FirstOrDefault(p => p.SysNo == value);
                if (group != null)
                {
                    this.GroupName = group.GroupName;
                }
			} 
		}
		
        private ADStatus? status;
		public ADStatus? Status 
		{ 
			get
			{
				return status;
			}			
			set
			{
				SetValue("Status", ref status, value);
			} 
		}
		
        private string introduction;
		public string Introduction 
		{ 
			get
			{
                return introduction;
			}			
			set
			{
                SetValue("Introduction", ref introduction, value);
			} 
		}
        /// <summary>
        /// 京东价
        /// </summary>
        public decimal? JDPrice { get; set; }
        public List<KeyValuePair<RecommendType?, string>> RecommendTypeList { get; set; }
        public List<KeyValuePair<ADStatus?, string>> StatusList { get; set; }
        public ObservableCollection<SaleAdvGroupVM> Groups { get; set; }
    }
}
