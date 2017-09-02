using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SaleAdvertisementVM : ModelBase
    {
        public SaleAdvertisementVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_Select });

            this.StatusList = EnumConverter.GetKeyValuePairs<SaleAdvStatus>();
            this.Status = SaleAdvStatus.Active;

            this.CustomerRankList = EnumConverter.GetKeyValuePairs<CustomerRank>();
            this.EnableReplyRank = CustomerRank.Ferrum;

            this.GroupTypeList = EnumConverter.GetKeyValuePairs<GroupType>();
            this.GroupType = BizEntity.MKT.GroupType.LevelOne;

            this.IsImageTextType = true;         
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

        private string header;
        [Validate(ValidateType.Required)]
        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                SetValue("Header", ref header, value);
            }
        }

        private string footer; 
        [Validate(ValidateType.MaxLength,2000)]
        public string Footer
        {
            get
            {
                return footer;
            }
            set
            {
                SetValue("Footer", ref footer, value);
            }
        }

        private string jumpAdvertising;
        [Validate(ValidateType.MaxLength, 2000)]
        public string JumpAdvertising
        {
            get
            {
                return jumpAdvertising;
            }
            set
            {
                SetValue("JumpAdvertising", ref jumpAdvertising, value);
            }
        }

        private string cssPath;
        public string CssPath
        {
            get
            {
                return cssPath;
            }
            set
            {
                SetValue("CssPath", ref cssPath, value);
            }
        }

        private DateTime? beginDate;
        [Validate(ValidateType.Required)]
        public DateTime? BeginDate
        {
            get
            {
                return beginDate;
            }
            set
            {
                SetValue("BeginDate", ref beginDate, value);
            }
        }

        private DateTime? endDate;
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                SetValue("EndDate", ref endDate, value);
            }
        }

        private SaleAdvStatus? status;
        public SaleAdvStatus? Status
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

        private bool enableComment;
        public bool EnableComment
        {
            get
            {
                return enableComment;
            }
            set
            {
                SetValue("EnableComment", ref enableComment, value);
            }
        }

        private GroupType? groupType;
        public GroupType? GroupType
        {
            get
            {
                return groupType;
            }
            set
            {
                SetValue("GroupType", ref groupType, value);
            }
        }

        private bool isGroupByCategory;
        public bool IsGroupByCategory
        {
            get
            {
                return isGroupByCategory;
            }
            set
            {
                SetValue("IsGroupByCategory", ref isGroupByCategory, value);
            }
        }      
		
        private CustomerRank? enableReplyRank;
        public CustomerRank? EnableReplyRank
        {
            get
            {
                return enableReplyRank;
            }
            set
            {
                SetValue("EnableReplyRank", ref enableReplyRank, value);
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

        private ShowType? type;
        public ShowType? Type
        {
            get
            {
                return type;
            }
            set
            {
                SetValue("Type", ref type, value);
            }
        }       
		
        private bool isTableType;
		public bool IsTableType 
		{ 
			get
			{
				return isTableType;
			}			
			set
			{
				SetValue("IsTableType", ref isTableType, value);
			} 
		}
		
        private bool isImageTextType;
		public bool IsImageTextType 
		{ 
			get
			{
				return isImageTextType;
			}			
			set
			{
				SetValue("IsImageTextType", ref isImageTextType, value);
			} 
		}

        private string isHold;
		public string IsHold 
		{ 
			get
			{
				return isHold;
			}			
			set
			{
				SetValue("IsHold", ref isHold, value);
			} 
		}		

        public ObservableCollection<SaleAdvItemVM> Items { get; set; }
        public ObservableCollection<SaleAdvGroupVM> Groups { get; set; }        


        #region UI扩展信息
        public List<KeyValuePair<SaleAdvStatus?, string>> StatusList { get; set; }
        public List<KeyValuePair<CustomerRank?, string>> CustomerRankList { get; set; }
        public List<KeyValuePair<GroupType?, string>> GroupTypeList { get; set; }                        
        public List<UIWebChannel> WebChannelList { get; set; }
        #endregion

        public void CalculateItemsCount()
        {
            this.Groups.ForEach(p =>
            {
                p.ItemsCount = this.Items.Where(i => i.GroupSysNo == p.SysNo).Count();
            });
        }

        public bool HasSaleAdvTemplateHoldPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_SaleAdvTemplate_Hold); }
        }

        public bool HasSaleAdvTemplateSavePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_SaleAdvTemplate_Save); }
        }
    }
}