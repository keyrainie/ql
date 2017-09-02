using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RegisterBasicVM : ModelBase
    {
        public RegisterBasicVM()
        {
            this.RequestTypes = EnumConverter.GetKeyValuePairs<RMARequestType>();         

            this.NextHandlers = EnumConverter.GetKeyValuePairs<RMANextHandler>(EnumConverter.EnumAppendItemType.Select);            

            this.ReasonTypes = new ObservableCollection<CodeNamePair>();            
            this.RMAReason = 0;

            this.Stocks = new List<StockInfo>();
        }

        /// <summary>
        /// 仅用于在单件编辑页面显示用
        /// </summary>
        private RMARequestType? m_RequestType;
        public RMARequestType? RequestType
        {
            get { return this.m_RequestType; }
            set
            {
                this.SetValue("RequestType", ref m_RequestType, value);                
            }
        }

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }        

        private Int32? m_ProductSysNo;
        public Int32? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }       
       
        private String m_ProductNo;
        public String ProductNo
        {
            get { return this.m_ProductNo; }
            set { this.SetValue("ProductNo", ref m_ProductNo, value); }
        }

        private String m_CustomerDesc;
        [Validate(ValidateType.Required)]
        public String CustomerDesc
        {
            get { return this.m_CustomerDesc; }
            set { this.SetValue("CustomerDesc", ref m_CustomerDesc, value); }
        }

        private bool? m_IsWithin7Days;
        public bool? IsWithin7Days
        {
            get { return this.m_IsWithin7Days; }
            set { this.SetValue("IsWithin7Days", ref m_IsWithin7Days, value); }
        }

        private RMAOwnBy? m_OwnBy;
        public RMAOwnBy? OwnBy
        {
            get { return this.m_OwnBy; }
            set { this.SetValue("OwnBy", ref m_OwnBy, value); }
        }

        private RMALocation? m_Location;
        public RMALocation? Location
        {
            get { return this.m_Location; }
            set { this.SetValue("Location", ref m_Location, value); }
        }

        private RMANextHandler? m_NextHandler;
        public RMANextHandler? NextHandler
        {
            get { return this.m_NextHandler; }
            set { this.SetValue("NextHandler", ref m_NextHandler, value); }
        }

        private bool m_IsHaveInvoice;
        public bool IsHaveInvoice
        {
            get { return this.m_IsHaveInvoice; }
            set { this.SetValue("IsHaveInvoice", ref m_IsHaveInvoice, value); }
        }

        private bool m_IsFullAccessory;
        public bool IsFullAccessory
        {
            get { return this.m_IsFullAccessory; }
            set { this.SetValue("IsFullAccessory", ref m_IsFullAccessory, value); }
        }

        private bool m_IsFullPackage;
        public bool IsFullPackage
        {
            get { return this.m_IsFullPackage; }
            set { this.SetValue("IsFullPackage", ref m_IsFullPackage, value); }
        }

        private String m_LocationWarehouse;
        public String LocationWarehouse
        {
            get { return this.m_LocationWarehouse; }
            set { this.SetValue("LocationWarehouse", ref m_LocationWarehouse, value); }
        }

        private String m_OwnByWarehouse;
        public String OwnByWarehouse
        {
            get { return this.m_OwnByWarehouse; }
            set { this.SetValue("OwnByWarehouse", ref m_OwnByWarehouse, value); }
        }

        private String m_Memo;
        public String Memo
        {
            get { return this.m_Memo; }
            set { this.SetValue("Memo", ref m_Memo, value); }
        }

        private Decimal? m_Cost;
        public Decimal? Cost
        {
            get { return this.m_Cost; }
            set { this.SetValue("Cost", ref m_Cost, value); }
        }

        private int? m_RMAReason;
        public int? RMAReason
        {
            get { return this.m_RMAReason; }
            set { this.SetValue("RMAReason", ref m_RMAReason, value); }
        }

        public string RMAReasonDesc { get; set; }

        private Int32? m_OutBoundWithInvoice;
        public Int32? OutBoundWithInvoice
        {
            get { return this.m_OutBoundWithInvoice; }
            set { this.SetValue("OutBoundWithInvoice", ref m_OutBoundWithInvoice, value); }
        }

        private String m_ShippedWarehouse;
        public String ShippedWarehouse
        {
            get { return this.m_ShippedWarehouse; }
            set { this.SetValue("ShippedWarehouse", ref m_ShippedWarehouse, value); }
        }

        private int? warehouseSysNo;
		public int? WarehouseSysNo 
		{ 
			get
			{
				return warehouseSysNo;
			}			
			set
			{
				SetValue("WarehouseSysNo", ref warehouseSysNo, value);
			} 
		}		

        private DateTime? m_UpdateNoResponseTime;
        public DateTime? UpdateNoResponseTime
        {
            get { return this.m_UpdateNoResponseTime; }
            set { this.SetValue("UpdateNoResponseTime", ref m_UpdateNoResponseTime, value); }
        }

        private DateTime? m_SetWaitingRevertTime;
        public DateTime? SetWaitingRevertTime
        {
            get { return this.m_SetWaitingRevertTime; }
            set { this.SetValue("SetWaitingRevertTime", ref m_SetWaitingRevertTime, value); }
        }

        private Int32? m_FailureType;
        public Int32? FailureType
        {
            get { return this.m_FailureType; }
            set { this.SetValue("FailureType", ref m_FailureType, value); }
        }

        private SOProductType? m_SOItemType;
        public SOProductType? SOItemType
        {
            get { return this.m_SOItemType; }
            set { this.SetValue("SOItemType", ref m_SOItemType, value); }
        }

        private RMAOutBoundStatus? m_OutBoundStatus;
        public RMAOutBoundStatus? OutBoundStatus
        {
            get { return this.m_OutBoundStatus; }
            set { this.SetValue("OutBoundStatus", ref m_OutBoundStatus, value); }
        }

        private RMAReturnStatus? m_ReturnStatus;
        public RMAReturnStatus? ReturnStatus
        {
            get { return this.m_ReturnStatus; }
            set { this.SetValue("ReturnStatus", ref m_ReturnStatus, value); }
        }

        private RMARefundStatus? m_RefundStatus;
        public RMARefundStatus? RefundStatus
        {
            get { return this.m_RefundStatus; }
            set { this.SetValue("RefundStatus", ref m_RefundStatus, value); }
        }

        private RMARequestStatus? m_Status;
        public RMARequestStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private Int32? m_CloseUserSysNo;
        public Int32? CloseUserSysNo
        {
            get { return this.m_CloseUserSysNo; }
            set { this.SetValue("CloseUserSysNo", ref m_CloseUserSysNo, value); }
        }

        private string closeUserName;
		public string CloseUserName 
		{ 
			get
			{
				return closeUserName;
			}			
			set
			{
				SetValue("CloseUserName", ref closeUserName, value);
			} 
		}		

        private DateTime? m_CloseTime;
        public DateTime? CloseTime
        {
            get { return this.m_CloseTime; }
            set { this.SetValue("CloseTime", ref m_CloseTime, value); }
        }

        public string ProductID { get; set; }
        public string ProductName { get; set; }        
        public int? SOSysNo { get; set; }
        public int? RequestSysNo { get; set; }
        public string CustomerName { get; set; }
        public ProcessType ProcessType { get; set; }
        public InvoiceType? InvoiceType { get; set; }
        public string BusinessModel { get; set; }        
        public CustomerRank? CustomerRank { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public List<KeyValuePair<RMARequestType?, string>> RequestTypes { get; set; }
        public List<KeyValuePair<RMANextHandler?, string>> NextHandlers { get; set; }
        public List<StockInfo> Stocks { get; set; }
        public ObservableCollection<CodeNamePair> ReasonTypes { get; set; }
        public int? RefundSysNo { get; set; }
        public ERPReturnType ERPStatus { get; set; }
        public ProductInventoryType InventoryType { get; set; }
    }
}
