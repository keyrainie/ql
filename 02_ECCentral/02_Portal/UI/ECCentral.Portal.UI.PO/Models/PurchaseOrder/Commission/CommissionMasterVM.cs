using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CommissionMasterVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { this.SetValue("VendorName", ref vendorName, value); }
        }

        private Int32? m_MerchantSysNo;
        public Int32? MerchantSysNo
        {
            get { return this.m_MerchantSysNo; }
            set { this.SetValue("MerchantSysNo", ref m_MerchantSysNo, value); }
        }

        private VendorCommissionMasterStatus? m_Status;
        public VendorCommissionMasterStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private decimal? totalAmt;

        public decimal? TotalAmt
        {
            get { return totalAmt; }
            set { this.SetValue("TotalAmt", ref totalAmt, value); }
        }

        private DateTime? beginDate;

        public DateTime? BeginDate
        {
            get { return beginDate; }
            set { beginDate = value; }
        }
        private DateTime? endDate;

        public DateTime? EndDate
        {
            get { return endDate; }
            set { this.SetValue("EndDate", ref endDate, value); }
        }
        private DateTime? inDate;

        public DateTime? InDate
        {
            get { return inDate; }
            set { this.SetValue("InDate", ref inDate, value); }
        }
        private DateTime? closeDate;

        public DateTime? CloaseDate
        {
            get { return closeDate; }
            set { this.SetValue("CloaseDate", ref closeDate, value); }
        }

        private VendorCommissionSettleStatus? m_SettleStatus;
        public VendorCommissionSettleStatus? SettleStatus
        {
            get { return this.m_SettleStatus; }
            set { this.SetValue("SettleStatus", ref m_SettleStatus, value); }
        }

        private string m_SettleStatusDisplay;
        public string SettleStatusDisplay
        {
            get { return this.m_SettleStatusDisplay; }
            set { this.SetValue("SettleStatusDisplay", ref m_SettleStatusDisplay, value); }
        }

        private Decimal? m_RentFee;
        public Decimal? RentFee
        {
            get { return this.m_RentFee; }
            set { this.SetValue("RentFee", ref m_RentFee, value); }
        }

        private Decimal? m_DeliveryFee;
        public Decimal? DeliveryFee
        {
            get { return this.m_DeliveryFee; }
            set { this.SetValue("DeliveryFee", ref m_DeliveryFee, value); }
        }

        private Decimal? m_SalesCommissionFee;
        public Decimal? SalesCommissionFee
        {
            get { return this.m_SalesCommissionFee; }
            set { this.SetValue("SalesCommissionFee", ref m_SalesCommissionFee, value); }
        }

        private Decimal? m_OrderCommissionFee;
        public Decimal? OrderCommissionFee
        {
            get { return this.m_OrderCommissionFee; }
            set { this.SetValue("OrderCommissionFee", ref m_OrderCommissionFee, value); }
        }

        private List<CommissionItemVM> m_ItemList;
        public List<CommissionItemVM> ItemList
        {
            get { return this.m_ItemList; }
            set { this.SetValue("ItemList", ref m_ItemList, value); }
        }

        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { this.SetValue("IsChecked", ref isChecked, value); }
        }

        private string memo;
        public string Memo
        {
            get { return memo; }
            set { this.SetValue("Memo", ref memo, value); }
        }

    }
}
