using System;
using System.Collections.Generic;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Inventory;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RegisterRevertVM : ModelBase
    {
        public RegisterRevertVM()
        {
            this.IsDecline = true;
            this.NewProductStatusList = EnumConverter.GetKeyValuePairs<RMANewProductStatus>(EnumConverter.EnumAppendItemType.Select);
            this.Stocks = new List<StockInfo>();
            this.SecondhandList = new List<RegisterSecondHandRspVM>();
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
		
        private RMARevertStatus? revertStatus;
		public RMARevertStatus? RevertStatus 
		{ 
			get
			{
                return revertStatus;
			}			
			set
			{
				SetValue("RevertStatus", ref revertStatus, value);
			} 
		}
		
        private RMANewProductStatus? newProductStatus;
		public RMANewProductStatus? NewProductStatus 
		{ 
			get
			{
				return newProductStatus;
			}			
			set
			{
				SetValue("NewProductStatus", ref newProductStatus, value);
			} 
		}
		
        private int? revertStockSysNo;
        public int? RevertStockSysNo 
		{ 
			get
			{
				return revertStockSysNo;
			}			
			set
			{
                SetValue("RevertStockSysNo", ref revertStockSysNo, value);
			} 
		}
		
        private int? revertProductSysNo;
		public int? RevertProductSysNo 
		{ 
			get
			{
				return revertProductSysNo;
			}			
			set
			{
				SetValue("RevertProductSysNo", ref revertProductSysNo, value);
			} 
		}

        private string revertProductID;
		public string RevertProductID 
		{ 
			get
			{
				return revertProductID;
			}			
			set
			{
				SetValue("RevertProductID", ref revertProductID, value);
			} 
		}		
		
        private string revertAuditMemo;
		public string RevertAuditMemo 
		{ 
			get
			{
				return revertAuditMemo;
			}			
			set
			{
				SetValue("RevertAuditMemo", ref revertAuditMemo, value);
			} 
		}
		
        private int? revertAuditUserSysNo;
		public int? RevertAuditUserSysNo 
		{ 
			get
			{
				return revertAuditUserSysNo;
			}			
			set
			{
				SetValue("RevertAuditUserSysNo", ref revertAuditUserSysNo, value);
			} 
		}
		
        private string revertAuditUserName;
		public string RevertAuditUserName 
		{ 
			get
			{
				return revertAuditUserName;
			}			
			set
			{
				SetValue("RevertAuditUserName", ref revertAuditUserName, value);
			} 
		}
		
        private DateTime? revertAuditTime;
		public DateTime? RevertAuditTime 
		{ 
			get
			{
				return revertAuditTime;
			}			
			set
			{
				SetValue("RevertAuditTime", ref revertAuditTime, value);
			} 
		}
		
        private DateTime? setWaitingRevertTime;
		public DateTime? SetWaitingRevertTime 
		{ 
			get
			{
				return setWaitingRevertTime;
			}			
			set
			{
				SetValue("SetWaitingRevertTime", ref setWaitingRevertTime, value);
			} 
		}

        public List<KeyValuePair<Nullable<RMANewProductStatus>, string>> NewProductStatusList { get; set; }
        public List<StockInfo> Stocks { get; set; }
        public List<RegisterSecondHandRspVM> SecondhandList { get; set; }

        private bool isApproved;
		public bool IsApproved 
		{ 
			get
			{
				return isApproved;
			}			
			set
			{
				SetValue("IsApproved", ref isApproved, value);
			} 
		}
		
        private bool isDecline;
	    public bool IsDecline 
		{ 
			get
			{
				return isDecline;
			}			
			set
			{
				SetValue("IsDecline", ref isDecline, value);
			} 
		}		       					
    }
}
