using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.PO.Models.PurchaseOrder
{
    public class VirtualPurchaseOrderQueryVM : ModelBase
    {
        public VirtualPurchaseOrderQueryVM()
        {
            m_CreateDateFrom = new DateTime(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)).Year, DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)).Month, DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)).Day);
            m_CreateDateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
        }

        private DateTime? m_CreateDateFrom;
        public DateTime? CreateDateFrom
        {
            get { return this.m_CreateDateFrom; }
            set { this.SetValue("CreateDateFrom", ref m_CreateDateFrom, value); }
        }

        private DateTime? m_CreateDateTo;
        public DateTime? CreateDateTo
        {
            get { return this.m_CreateDateTo; }
            set { this.SetValue("CreateDateTo", ref m_CreateDateTo, value); }
        }

        private VirtualPurchaseOrderStatus? m_Status;
        public VirtualPurchaseOrderStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private String m_VSPOSysNo;
        public String VSPOSysNo
        {
            get { return this.m_VSPOSysNo; }
            set { this.SetValue("VSPOSysNo", ref m_VSPOSysNo, value); }
        }

        private String m_SOSysNo;
        public String SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private Int32? m_ProductSysNo;
        public Int32? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private SOStatus? m_SOStatus;
        public SOStatus? SOStatus
        {
            get { return this.m_SOStatus; }
            set { this.SetValue("SOStatus", ref m_SOStatus, value); }
        }

        private Int32? m_PMLeaderSysNo;
        public Int32? PMLeaderSysNo
        {
            get { return this.m_PMLeaderSysNo; }
            set { this.SetValue("PMLeaderSysNo", ref m_PMLeaderSysNo, value); }
        }

        private Int32? m_PMExecSysNo;
        public Int32? PMExecSysNo
        {
            get { return this.m_PMExecSysNo; }
            set { this.SetValue("PMExecSysNo", ref m_PMExecSysNo, value); }
        }

        private PurchaseOrderStatus? m_POStatus;
        public PurchaseOrderStatus? POStatus
        {
            get { return this.m_POStatus; }
            set { this.SetValue("POStatus", ref m_POStatus, value); }
        }

        private Int32? m_PayTypeSysNo;
        public Int32? PayTypeSysNo
        {
            get { return this.m_PayTypeSysNo; }
            set { this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value); }
        }

        private ShiftRequestStatus? m_ShiftStatus;
        public ShiftRequestStatus? ShiftStatus
        {
            get { return this.m_ShiftStatus; }
            set { this.SetValue("ShiftStatus", ref m_ShiftStatus, value); }
        }

        private InStockStatus? m_InStockStatus;
        public InStockStatus? InStockStatus
        {
            get { return this.m_InStockStatus; }
            set { this.SetValue("InStockStatus", ref m_InStockStatus, value); }
        }

        private ConvertRequestStatus? m_TransferStatus;
        public ConvertRequestStatus? TransferStatus
        {
            get { return this.m_TransferStatus; }
            set { this.SetValue("TransferStatus", ref m_TransferStatus, value); }
        }

        private Int32? m_StockSysNo;
        public Int32? StockSysNo
        {
            get { return this.m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private VirtualPurchaseInStockOrderType? m_InStockOrderType;
        public VirtualPurchaseInStockOrderType? InStockOrderType
        {
            get { return this.m_InStockOrderType; }
            set { this.SetValue("InStockOrderType", ref m_InStockOrderType, value); }
        }

        private Int32? m_DelayDays;
        public Int32? DelayDays
        {
            get { return this.m_DelayDays; }
            set { this.SetValue("DelayDays", ref m_DelayDays, value); }
        }

        private Int32? m_EstimateDelayDays;
        public Int32? EstimateDelayDays
        {
            get { return this.m_EstimateDelayDays; }
            set { this.SetValue("EstimateDelayDays", ref m_EstimateDelayDays, value); }
        }

        private Int32? m_C1SysNo;
        public Int32? C1SysNo
        {
            get { return this.m_C1SysNo; }
            set { this.SetValue("C1SysNo", ref m_C1SysNo, value); }
        }

        private Int32? m_C2SysNo;
        public Int32? C2SysNo
        {
            get { return this.m_C2SysNo; }
            set { this.SetValue("C2SysNo", ref m_C2SysNo, value); }
        }

        private Int32? m_C3SysNo;
        public Int32? C3SysNo
        {
            get { return this.m_C3SysNo; }
            set { this.SetValue("C3SysNo", ref m_C3SysNo, value); }
        }

        private bool m_IsHasHistory;
        public bool IsHasHistory
        {
            get { return this.m_IsHasHistory; }
            set { this.SetValue("IsHasHistory", ref m_IsHasHistory, value); }
        }

    }
}
