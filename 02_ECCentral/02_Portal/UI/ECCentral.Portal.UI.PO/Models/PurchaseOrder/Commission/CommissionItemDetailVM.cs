using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CommissionItemDetailVM : ModelBase
    {

        private int? detailSysNo;

        public int? DetailSysNo
        {
            get { return detailSysNo; }
            set { this.SetValue("DetailSysNo", ref detailSysNo, value); }
        }

        private Int32? m_CommissionItemSysNo;
        public Int32? CommissionItemSysNo
        {
            get { return this.m_CommissionItemSysNo; }
            set { this.SetValue("CommissionItemSysNo", ref m_CommissionItemSysNo, value); }
        }

        private String m_ReferenceSysNo;
        public String ReferenceSysNo
        {
            get { return this.m_ReferenceSysNo; }
            set { this.SetValue("ReferenceSysNo", ref m_ReferenceSysNo, value); }
        }

        private VendorCommissionReferenceType? m_ReferenceType;
        public VendorCommissionReferenceType? ReferenceType
        {
            get { return this.m_ReferenceType; }
            set { this.SetValue("ReferenceType", ref m_ReferenceType, value); }
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

        private Decimal? m_SalePrice;
        public Decimal? SalePrice
        {
            get { return this.m_SalePrice; }
            set { this.SetValue("SalePrice", ref m_SalePrice, value); }
        }

        private Int32? m_Quantity;
        public Int32? Quantity
        {
            get { return this.m_Quantity; }
            set { this.SetValue("Quantity", ref m_Quantity, value); }
        }

        private bool? m_HaveAutoRMA;
        public bool? HaveAutoRMA
        {
            get { return this.m_HaveAutoRMA; }
            set { this.SetValue("HaveAutoRMA", ref m_HaveAutoRMA, value); }
        }

        public Decimal? TotalAmt
        {
            get { return (this.m_Quantity ?? 0) * (this.m_SalePrice ?? 0) - Math.Abs(this.PromotionDiscount); }
        }

        private DateTime? m_CreateDate;
        public DateTime? CreateDate
        {
            get { return this.m_CreateDate; }
            set { this.SetValue("CreateDate", ref m_CreateDate, value); }
        }

        public decimal PromotionDiscount { get; set; }

    }
}
