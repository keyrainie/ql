using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;


namespace ECCentral.Portal.UI.RMA.Models
{
    public class RequestQueryReqVM : ModelBase
    {
        public RequestQueryReqVM()
        {
            this.ReceiveUsers = new List<UserInfo>();
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //修改UIWebChannelType.publicChennel 后放开
            //this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All, ChannelType = UIWebChannelType.publicChennel });

            this.SellerTypeList = EnumConverter.GetKeyValuePairs<SellersType>(EnumConverter.EnumAppendItemType.All);       
            
            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            this.RequestStatusList = EnumConverter.GetKeyValuePairs<RMARequestStatus>(EnumConverter.EnumAppendItemType.All);
            this.ConfirmUsers = new List<UserInfo>();
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
		
        private DateTime? receivedDateFrom;
		public DateTime? ReceivedDateFrom 
		{ 
			get
			{
				return receivedDateFrom;
			}			
			set
			{
				SetValue("ReceivedDateFrom", ref receivedDateFrom, value);
			} 
		}
		
        private DateTime? receivedDateTo;
		public DateTime? ReceivedDateTo 
		{ 
			get
			{
				return receivedDateTo;
			}			
			set
			{
				SetValue("ReceivedDateTo", ref receivedDateTo, value);
			} 
		}
		
        private DateTime? etakeDateFrom;
		public DateTime? ETakeDateFrom 
		{ 
			get
			{
				return etakeDateFrom;
			}			
			set
			{
				SetValue("ETakeDateFrom", ref etakeDateFrom, value);
			} 
		}
		
        private DateTime? etakeDateTo;
		public DateTime? ETakeDateTo 
		{ 
			get
			{
				return etakeDateTo;
			}			
			set
			{
				SetValue("ETakeDateTo", ref etakeDateTo, value);
			} 
		}
		
        private string soID;
		public string SOID 
		{ 
			get
			{
				return soID;
			}			
			set
			{
				SetValue("SOID", ref soID, value);
			} 
		}
		
        private string requestID;
		public string RequestID 
		{ 
			get
			{
				return requestID;
			}			
			set
			{
				SetValue("RequestID", ref requestID, value);
			} 
		}
		
        private bool? isVIP;
		public bool? IsVIP 
		{ 
			get
			{
				return isVIP;
			}			
			set
			{
				SetValue("IsVIP", ref isVIP, value);
			} 
		}
		
        private RMARequestStatus? status;
        public RMARequestStatus? Status 
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
		
        private string customerSysNo;
        [Validate(ValidateType.Interger)]
		public string CustomerSysNo 
		{ 
			get
			{
				return customerSysNo;
			}			
			set
			{
				SetValue("CustomerSysNo", ref customerSysNo, value);
			} 
		}
		
        private string customerID;
		public string CustomerID 
		{ 
			get
			{
				return customerID;
			}			
			set
			{
				SetValue("CustomerID", ref customerID, value);
			} 
		}

        private bool? isSubmit;
		public bool? IsSubmit 
		{ 
			get
			{
				return isSubmit;
			}			
			set
			{
				SetValue("IsSubmit", ref isSubmit, value);
			} 
		}
		
        private int? recieveUserSysNo;
        [Validate(ValidateType.Interger)]
		public int? ReceiveUserSysNo 
		{ 
			get
			{
				return recieveUserSysNo;
			}			
			set
			{
				SetValue("ReceiveUserSysNo", ref recieveUserSysNo, value);
			} 
		}
		
        private SellersType? sellersType;
        public SellersType? SellersType 
		{ 
			get
			{
				return sellersType;
			}			
			set
			{
				SetValue("SellersType", ref sellersType, value);
			} 
		}
		
        private bool? isLabelPrinted;
        public bool? IsLabelPrinted 
		{ 
			get
			{
				return isLabelPrinted;
			}			
			set
			{
				SetValue("IsLabelPrinted", ref isLabelPrinted, value);
			} 
		}
		
        private string companyCode;
		public string CompanyCode 
		{ 
			get
			{
				return companyCode;
			}			
			set
			{
				SetValue("CompanyCode", ref companyCode, value);
			}
        }

        private string webChannelID;
		public string WebChannelID 
		{ 
			get
			{
				return webChannelID;
			}			
			set
			{
                SetValue("WebChannelID", ref webChannelID, value);
			} 
		}

        /// <summary>
        /// 服务编号
        /// </summary>
        public string ServiceCode { get; set; }       

        private DateTime? auditDateFrom;
        public DateTime? AuditDateFrom
        {
            get
            {
                return auditDateFrom;
            }
            set
            {
                SetValue("AuditDateFrom", ref auditDateFrom, value);
            }
        }

        private DateTime? auditDateTo;
        public DateTime? AuditDateTo
        {
            get
            {
                return auditDateTo;
            }
            set
            {
                SetValue("AuditDateTo", ref auditDateTo, value);
            }
        }


        private int? auditUserSysNo;
        [Validate(ValidateType.Interger)]
        public int? AuditUserSysNo
        {
            get
            {
                return auditUserSysNo;
            }
            set
            {
                SetValue("AuditUserSysNo", ref auditUserSysNo, value);
            }
        }

        #region 扩展信息 
        public List<UIWebChannel> WebChannelList { get; set; }
        public List<KeyValuePair<Boolean?, string>> YNList { get; set; }
        public List<KeyValuePair<Nullable<RMARequestStatus>, string>> RequestStatusList { get; set; }
        public List<UserInfo> ReceiveUsers { get; set; }
        public List<KeyValuePair<SellersType?,string>> SellerTypeList { get; set; }
        public List<UserInfo> ConfirmUsers { get; set; }
        #endregion
    }
}
