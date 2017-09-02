using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;

using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.Models
{    
    public class RequestVM : ModelBase
    {
        public RequestVM()
        {
            this.Registers = new ObservableCollection<RegisterVM>();
            this.ShipTypes = new List<CodeNamePair>();       
        }

        private int? m_SysNo;
        public int? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private string m_SOSysNo;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }       

        private int? m_CustomerSysNo;
        public int? CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        private string m_Address;
        [Validate(ValidateType.Required)]
        public string Address
        {
            get { return this.m_Address; }
            set { this.SetValue("Address", ref m_Address, value); }
        }

        private int? m_AreaSysNo;
        [Validate(ValidateType.Required)]
        public int? AreaSysNo
        {
            get { return this.m_AreaSysNo; }
            set { this.SetValue("AreaSysNo", ref m_AreaSysNo, value); }
        }

        private string m_Contact;
        [Validate(ValidateType.Required)]
        public string Contact
        {
            get { return this.m_Contact; }
            set { this.SetValue("Contact", ref m_Contact, value); }
        }

        private string m_Phone;
        [Validate(ValidateType.Required)]
        public string Phone
        {
            get { return this.m_Phone; }
            set { this.SetValue("Phone", ref m_Phone, value); }
        }

        private DateTime? m_ReceiveTime;
        public DateTime? ReceiveTime
        {
            get { return this.m_ReceiveTime; }
            set { this.SetValue("ReceiveTime", ref m_ReceiveTime, value); }
        }

        private int? m_ReceiveUserSysNo;
        public int? ReceiveUserSysNo
        {
            get { return this.m_ReceiveUserSysNo; }
            set { this.SetValue("ReceiveUserSysNo", ref m_ReceiveUserSysNo, value); }
        }

        private string receiveUserName;
        public string ReceiveUserName
        {
            get
            {
                return receiveUserName;
            }
            set
            {
                SetValue("ReceiveUserName", ref receiveUserName, value);
            }
        }

        private RMARequestStatus? m_Status;
        public RMARequestStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private DateTime? m_CustomerSendTime;
        //[Validate(ValidateType.Required)]
        public DateTime? CustomerSendTime
        {
            get { return this.m_CustomerSendTime; }
            set { this.SetValue("CustomerSendTime", ref m_CustomerSendTime, value); }
        }

        private bool m_IsRejectRMA;
        public bool IsRejectRMA
        {
            get { return this.m_IsRejectRMA; }
            set { this.SetValue("IsRejectRMA", ref m_IsRejectRMA, value); }
        }

        private bool isCancelVerifyDuplicate;
        public bool IsCancelVerifyDuplicate 
		{ 
			get
			{
                return isCancelVerifyDuplicate;
			}			
			set
			{
                SetValue("IsCancelVerifyDuplicate", ref isCancelVerifyDuplicate, value);
			} 
		}
		
        private string m_Express;
        public string Express
        {
            get { return this.m_Express; }
            set { this.SetValue("Express", ref m_Express, value); }
        }

        private string m_PackageNumber;
        public string PackageNumber
        {
            get { return this.m_PackageNumber; }
            set { this.SetValue("PackageNumber", ref m_PackageNumber, value); }
        }

        private Boolean? m_IsSubmit;
        public Boolean? IsSubmit
        {
            get { return this.m_IsSubmit; }
            set { this.SetValue("IsSubmit", ref m_IsSubmit, value); }
        }

        private string m_ReceiveWarehouse;
        //[Validate(ValidateType.Required)]
        public string ReceiveWarehouse
        {
            get { return this.m_ReceiveWarehouse; }
            set { this.SetValue("ReceiveWarehouse", ref m_ReceiveWarehouse, value); }
        }

        private string m_Note;
        public string Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        private bool m_IsViaPostOffice;
        public bool IsViaPostOffice
        {
            get { return this.m_IsViaPostOffice; }
            set { this.SetValue("IsViaPostOffice", ref m_IsViaPostOffice, value); }
        }

        private string m_PostageToPoint;
        //[Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string PostageToPoint
        {
            get { return this.m_PostageToPoint; }
            set { this.SetValue("PostageToPoint", ref m_PostageToPoint, value); }
        }

        private int? m_ReturnPoint;
        public int? ReturnPoint
        {
            get { return this.m_ReturnPoint; }
            set { this.SetValue("ReturnPoint", ref m_ReturnPoint, value); }
        }

        private DateTime? m_ETakeDate;
        public DateTime? ETakeDate
        {
            get { return this.m_ETakeDate; }
            set { this.SetValue("ETakeDate", ref m_ETakeDate, value); }
        }

        private string m_RequestID;
        public string RequestID
        {
            get { return this.m_RequestID; }
            set { this.SetValue("RequestID", ref m_RequestID, value); }
        }

        private string m_IsLabelPrinted;
        public string IsLabelPrinted
        {
            get { return this.m_IsLabelPrinted; }
            set { this.SetValue("IsLabelPrinted", ref m_IsLabelPrinted, value); }
        }

        private int? m_MerchantSysNo;
        public int? MerchantSysNo
        {
            get { return this.m_MerchantSysNo; }
            set { this.SetValue("MerchantSysNo", ref m_MerchantSysNo, value); }
        }

        private StockType? m_StockType;
        public StockType? StockType
        {
            get { return this.m_StockType; }
            set { this.SetValue("StockType", ref m_StockType, value); }
        }

        private DeliveryType? shippingType;
		public DeliveryType? ShippingType 
		{ 
			get
			{
				return shippingType;
			}			
			set
			{
				SetValue("ShippingType", ref shippingType, value);
			} 
		}

        private string m_ShipType;
        //[Validate(ValidateType.Required)]
        public string ShipType
        {
            get { return this.m_ShipType; }
            set
            {
                this.SetValue("ShipType", ref m_ShipType, value);                
            }
        }

        private InvoiceType? m_InvoiceType;
        public InvoiceType? InvoiceType
        {
            get { return this.m_InvoiceType; }
            set { this.SetValue("InvoiceType", ref m_InvoiceType, value); }
        }

        private DateTime? m_GetbackDate;
        public DateTime? GetbackDate
        {
            get { return this.m_GetbackDate; }
            set { this.SetValue("GetbackDate", ref m_GetbackDate, value); }
        }

        private int? m_GetbackAreaSysNo;
        public int? GetbackAreaSysNo
        {
            get { return this.m_GetbackAreaSysNo; }
            set { this.SetValue("GetbackAreaSysNo", ref m_GetbackAreaSysNo, value); }
        }

        private string m_GetbackAddress;
        public string GetbackAddress
        {
            get { return this.m_GetbackAddress; }
            set { this.SetValue("GetbackAddress", ref m_GetbackAddress, value); }
        }

        private string shipViaCode;
        //[Validate(ValidateType.Required)]
		public string ShipViaCode 
		{ 
			get
			{
				return shipViaCode;
			}			
			set
			{
				SetValue("ShipViaCode", ref shipViaCode, value);
			} 
		}
		
        private string trackingNumber;        
		public string TrackingNumber 
		{ 
			get
			{
				return trackingNumber;
			}			
			set
			{
				SetValue("TrackingNumber", ref trackingNumber, value);
			} 
		}

        private string memo;
		public string Memo 
		{ 
			get
			{
				return memo;
			}			
			set
			{
				SetValue("Memo", ref memo, value);
			} 
		}		

        #region 扩展信息
        public string BusinessModel { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUserName { get; set; }

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
		
        private string customerName;
		public string CustomerName 
		{ 
			get
			{
				return customerName;
			}			
			set
			{
				SetValue("CustomerName", ref customerName, value);
			} 
		}
		       
        public string SOID { get; set; }        
		        
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// 配送员名称
        /// </summary>
        public string DeliveryUserName { get; set; }        

        /// <summary>
        /// 指派状态
        /// </summary>
        public string DeliveryStatus { get; set; }
        public List<CodeNamePair> ShipTypes { get; set; }
        public List<StockInfo> Stocks { get; set; }
        #endregion

        private ObservableCollection<RegisterVM> registers;
		public ObservableCollection<RegisterVM> Registers 
		{ 
			get
			{
				return registers;
			}			
			set
			{
				SetValue("Registers", ref registers, value);
			} 
		}

        private string serviceCode;
        [Validate(ValidateType.Required)]
        public string ServiceCode
        {
            get
            {
                return serviceCode;
            }
            set
            {
                SetValue("ServiceCode", ref serviceCode, value);
            }
        }


       
        private DateTime? auditTime;
        public DateTime? AuditTime
        {
            get
            {
                return auditTime;
            }
            set
            {
                SetValue("AuditTime", ref auditTime, value);
            }
        }      
        
        private int? auditUserSysNo;
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

        private string auditUserName;
        public string AuditUserName
        {
            get
            {
                return auditUserName;
            }
            set
            {
                SetValue("AuditUserName", ref auditUserName, value);
            }
        }

        private bool m_IsReceiveMsg;
        public bool IsReceiveMsg
        {
            get { return this.m_IsReceiveMsg; }
            set { this.SetValue("IsReceiveMsg", ref m_IsReceiveMsg, value); }
        }
    }
}
