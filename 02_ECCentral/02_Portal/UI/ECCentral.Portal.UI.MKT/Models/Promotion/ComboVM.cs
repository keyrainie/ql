using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ComboVM : ModelBase
    {
        public ComboVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_Select });
            this.ChannelID = this.WebChannelList.FirstOrDefault().ChannelID;
            this.Priority = "9999";
            this.IsDeactive = true;
            this.ComboTypeList = EnumConverter.GetKeyValuePairs<ComboType>();
            this.SaleRuleType = ComboType.Common;
            this.Items = new ObservableCollection<ComboItemVM>();
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

        private string name;
        [Validate(ValidateType.Required)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetValue("Name", ref name, value);
            }
        }

        private bool isShowName;
        public bool IsShowName
        {
            get
            {
                return isShowName;
            }
            set
            {
                SetValue("IsShowName", ref isShowName, value);
            }
        }

        private string priority;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
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

        private ComboStatus? status;
        public ComboStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                SetValue("Status", ref status, value);
                if (value == ComboStatus.Active)
                {
                    IsActive = true;
                }
                else if (value == ComboStatus.WaitingAudit)
                {
                    IsWaitingAudit = true;
                }
                else
                {
                    IsDeactive = true;
                }
            }
        }

        private ComboStatus? targetStatus;
		public ComboStatus? TargetStatus 
		{ 
			get
			{
				return targetStatus;
			}			
			set
			{
				SetValue("TargetStatus", ref targetStatus, value);
			} 
		}		

        private ComboType? saleRuleType;
        public ComboType? SaleRuleType
        {
            get
            {
                return saleRuleType;
            }
            set
            {
                SetValue("SaleRuleType", ref saleRuleType, value);
            }
        }

        private string channelID;
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get
            {
                return channelID;
            }
            set
            {
                SetValue("ChannelID", ref channelID, value);
            }
        }

        public List<KeyValuePair<ComboType?, string>> ComboTypeList { get; set; }
        public ObservableCollection<ComboItemVM> Items { get; set; }

        private bool isDeactive;
		public bool IsDeactive 
		{ 
			get
			{
				return isDeactive;
			}			
			set
			{
				SetValue("IsDeactive", ref isDeactive, value);
			} 
		}		

        private bool isActive;
		public bool IsActive 
		{ 
			get
			{
				return isActive;
			}			
			set
			{
				SetValue("IsActive", ref isActive, value);
			} 
		}
		
        private bool isWaitingAudit;
		public bool IsWaitingAudit 
		{ 
			get
			{
				return isWaitingAudit;
			}			
			set
			{
				SetValue("IsWaitingAudit", ref isWaitingAudit, value);
			} 
		}
        public int RequestSysNo { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        private string reason;
        public string Reason 
        {
            get { return reason; }
            set { SetValue("Reason", ref reason, value); }
        }

        public List<UIWebChannel> WebChannelList { get; set; }
    }

    public class ComboItemVM : ModelBase
    {
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

        private int? productSysNo;
        [Validate(ValidateType.Required)]
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

        private string productID;
        public string ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                SetValue("ProductID", ref productID, value);
            }
        }

        private string productName;
        public string ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                SetValue("ProductName", ref productName, value);
            }
        }

        public int? MerchantSysNo { get; set; }

        private string merchantName;
        public string MerchantName
        {
            get
            {
                return merchantName;
            }
            set
            {
                SetValue("MerchantName", ref merchantName, value);
            }
        }


        private string quantity;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                SetValue("Quantity", ref quantity, value);
            }
        }

        private string discount;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^-[0-9]\d*|[0]$", ErrorMessage = "折扣必须是整数，且小于等于0")]
        public string Discount
        {
            get
            {
                return discount;
            }
            set
            {
                SetValue("Discount", ref discount, value);
            }
        }

        private decimal? productUnitCost;
        public decimal? ProductUnitCost
        {
            get
            {
                return productUnitCost;
            }
            set
            {
                SetValue("ProductUnitCost", ref productUnitCost, value);
            }
        }

        private decimal prodcuctCurrentPrice;
        public decimal ProductCurrentPrice
        {
            get
            {
                return prodcuctCurrentPrice;
            }
            set
            {
                SetValue("ProductCurrentPrice", ref prodcuctCurrentPrice, value);
            }
        }

        private bool isMasterItemB;
        public bool IsMasterItemB
        {
            get
            {
                return isMasterItemB;
            }
            set
            {
                SetValue("IsMasterItemB", ref isMasterItemB, value);
            }
        }
    }
}
