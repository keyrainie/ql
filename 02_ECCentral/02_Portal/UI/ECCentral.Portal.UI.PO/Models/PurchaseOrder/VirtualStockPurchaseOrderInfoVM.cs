using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Inventory;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VirtualStockPurchaseOrderInfoVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private Int32? m_SOSysNo;
        public Int32? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private Int32? m_SOItemSysNo;
        public Int32? SOItemSysNo
        {
            get { return this.m_SOItemSysNo; }
            set { this.SetValue("SOItemSysNo", ref m_SOItemSysNo, value); }
        }

        private Int32? m_ProductSysNo;
        public Int32? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private String m_ProductID;
        public String ProductID
        {
            get { return this.m_ProductID; }
            set { this.SetValue("ProductID", ref m_ProductID, value); }
        }

        private String m_ProductName;
        public String ProductName
        {
            get { return this.m_ProductName; }
            set { this.SetValue("ProductName", ref m_ProductName, value); }
        }

        private VirtualPurchaseOrderStatus? m_Status;
        public VirtualPurchaseOrderStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private Int32? m_PurchaseQty;
        public Int32? PurchaseQty
        {
            get { return this.m_PurchaseQty; }
            set { this.SetValue("PurchaseQty", ref m_PurchaseQty, value); }
        }

        private DateTime? m_EstimateArriveTime;
        /// <summary>
        /// 估计到达时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? EstimateArriveTime
        {
            get { return this.m_EstimateArriveTime; }
            set { this.SetValue("EstimateArriveTime", ref m_EstimateArriveTime, value); }
        }

        /// <summary>
        /// 估计到达时间(段)
        /// </summary>
        private DateTime? m_EstimateArriveTimePeriod;
        [Validate(ValidateType.Required)]
        public DateTime? EstimateArriveTimePeriod
        {
            get { return m_EstimateArriveTimePeriod; }
            set { this.SetValue("EstimateArriveTimePeriod", ref m_EstimateArriveTimePeriod, value); }
        }


        private DateTime m_CreateTime;
        public DateTime CreateTime
        {
            get { return this.m_CreateTime; }
            set { this.SetValue("CreateTime", ref m_CreateTime, value); }
        }

        private String m_InStockTime;
        public String InStockTime
        {
            get { return this.m_InStockTime; }
            set { this.SetValue("InStockTime", ref m_InStockTime, value); }
        }

        private String m_CreateUserName;
        public String CreateUserName
        {
            get { return this.m_CreateUserName; }
            set { this.SetValue("CreateUserName", ref m_CreateUserName, value); }
        }

        private String m_CSMemo;
        public String CSMemo
        {
            get { return this.m_CSMemo; }
            set { this.SetValue("CSMemo", ref m_CSMemo, value); }
        }

        private Int32? m_PMUserSysNo;
        public Int32? PMUserSysNo
        {
            get { return this.m_PMUserSysNo; }
            set { this.SetValue("PMUserSysNo", ref m_PMUserSysNo, value); }
        }

        private String m_PMMemo;
        public String PMMemo
        {
            get { return this.m_PMMemo; }
            set { this.SetValue("PMMemo", ref m_PMMemo, value); }
        }

        private VirtualPurchaseInStockOrderType? m_InStockOrderType;
        public VirtualPurchaseInStockOrderType? InStockOrderType
        {
            get { return this.m_InStockOrderType; }
            set { this.SetValue("InStockOrderType", ref m_InStockOrderType, value); }
        }

        private string m_InStockOrderSysNo;
        /// <summary>
        /// 单据号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^\d{1,8}$", ErrorMessageResourceName = "Integer_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string InStockOrderSysNo
        {
            get { return this.m_InStockOrderSysNo; }
            set { this.SetValue("InStockOrderSysNo", ref m_InStockOrderSysNo, value); }
        }

        private InStockStatus? m_InStockStatus;
        public InStockStatus? InStockStatus
        {
            get { return this.m_InStockStatus; }
            set { this.SetValue("InStockStatus", ref m_InStockStatus, value); }
        }

        private int? m_SOVirtualCount;

        public int? SOVirtualCount
        {
            get { return m_SOVirtualCount; }
            set { this.SetValue("SOVirtualCount", ref m_SOVirtualCount, value); }
        }

    }
}
