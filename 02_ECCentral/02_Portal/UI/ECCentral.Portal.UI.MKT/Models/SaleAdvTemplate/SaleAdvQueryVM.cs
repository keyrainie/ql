using System;
using System.Collections.Generic;
using System.Linq;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SaleAdvQueryVM : ModelBase
    {
        public SaleAdvQueryVM()
        {
            //修改UIWebChannelType.publicChennel 后放开

            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All, ChannelType = UIWebChannelType.Publicity });

            this.StatusList = EnumConverter.GetKeyValuePairs<SaleAdvStatus>(EnumConverter.EnumAppendItemType.All);            
        }

        private string sysNo;
        [Validate(ValidateType.Interger)]
		public string SysNo 
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
		
        private string createUser;
		public string CreateUser 
		{ 
			get
			{
				return createUser;
			}			
			set
			{
				SetValue("CreateUser", ref createUser, value);
			} 
		}
		
        private DateTime? createDateFrom;
		public DateTime? CreateDateFrom 
		{ 
			get
			{
				return createDateFrom;
			}			
			set
			{
				SetValue("CreateDateFrom", ref createDateFrom, value);
			} 
		}
		
        private DateTime? createDateTo;
		public DateTime? CreateDateTo 
		{ 
			get
			{
				return createDateTo;
			}			
			set
			{
				SetValue("CreateDateTo", ref createDateTo, value);
			} 
		}
		
        private string channelID;
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

        public List<KeyValuePair<SaleAdvStatus?, string>> StatusList { get; set; }
        public List<UIWebChannel> WebChannelList { get; set; }
    }
}
