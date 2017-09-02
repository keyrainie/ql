using System.Collections.Generic;

using ECCentral.BizEntity.RMA;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Inventory;


namespace ECCentral.Portal.UI.RMA.Models
{
    public class RegisterVM : ModelBase
    {       
        private int? sysNo;

        public RegisterVM()
        {
            this.BasicInfo = new RegisterBasicVM();
            this.CheckInfo = new RegisterCheckVM();
            this.ResponseInfo = new RegisterResponseVM();
            this.RevertInfo = new RegisterRevertVM();
            this.ContactInfo = new CustomerContactVM();
            this.SellerInfo = new SellerRelatedVM();
            this.DunLogs = new List<RegisterDunLog>();
            this.RequestType = RMARequestType.Return;
        }

        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                sysNo = value;
                this.BasicInfo.SysNo = value;
                this.CheckInfo.SysNo = value;
                this.ResponseInfo.SysNo = value;
                this.RevertInfo.SysNo = value;
                this.SellerInfo.SysNo = value;
            }
        }

        private bool isChecked;
        /// <summary>
        /// Grid中某行是否选中
        /// </summary>
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

        private RMARequestType? m_RequestType;
        public RMARequestType? RequestType
        {
            get { return this.m_RequestType; }
            set
            {
                this.SetValue("RequestType", ref m_RequestType, value);                
            }
        }

        public RegisterBasicVM BasicInfo { get; set; }
        
        public RegisterCheckVM CheckInfo { get; set; }
        
        public RegisterResponseVM ResponseInfo { get; set; }

        public RegisterRevertVM RevertInfo { get; set; }

        private CustomerContactVM contactInfo;
        public CustomerContactVM ContactInfo
        {
            get
            {
                return contactInfo;
            }
            set
            {
                SetValue("ContactInfo", ref contactInfo, value);
            }
        }        

        public CustomerContactVM OriginContactInfo { get; set; }

        public SellerRelatedVM SellerInfo { get; set; }

        public List<RegisterDunLog> DunLogs { get; set; }

        public List<ProductInventoryInfo> ProductInventoryInfo { get; set; }
    }
}
