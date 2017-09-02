using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models
{
    public class GatherSettlementItemInfoVM : ModelBase
    {

        private bool isDeleteChecked;

        public bool IsDeleteChecked
        {
            get { return isDeleteChecked; }
            set { this.SetValue("IsDeleteChecked", ref isDeleteChecked, value); }
        }

        private Int32? m_ItemSysNo;
        public Int32? ItemSysNo
        {
            get { return this.m_ItemSysNo; }
            set { this.SetValue("ItemSysNo", ref m_ItemSysNo, value); }
        }

        private Int32? m_VendorSysNo;
        public Int32? VendorSysNo
        {
            get { return this.m_VendorSysNo; }
            set { this.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }


        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set { this.SetValue("VendorName", ref vendorName, value); }
        }

        /// <summary>
        /// 单据编号 
        /// </summary>
        private int? invoiceNumber;

        public int? InvoiceNumber
        {
            get { return invoiceNumber; }
            set { this.SetValue("InvoiceNumber", ref invoiceNumber, value); }
        }

        private Int32? m_OrderSysNo;
        public Int32? OrderSysNo
        {
            get { return this.m_OrderSysNo; }
            set { this.SetValue("OrderSysNo", ref m_OrderSysNo, value); }
        }

        private GatherSettleType? m_SettleType;
        public GatherSettleType? SettleType
        {
            get { return this.m_SettleType; }
            set { this.SetValue("SettleType", ref m_SettleType, value); }
        }

        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { this.SetValue("ProductSysNo", ref productSysNo, value); }
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

        private Int32? m_ProductQuantity;
        public Int32? ProductQuantity
        {
            get { return this.m_ProductQuantity; }
            set { this.SetValue("ProductQuantity", ref m_ProductQuantity, value); }
        }

        private Decimal? m_SalePrice;
        public Decimal? SalePrice
        {
            get { return this.m_SalePrice; }
            set { this.SetValue("SalePrice", ref m_SalePrice, value); }
        }

        private DateTime? m_CreateDate;
        public DateTime? CreateDate
        {
            get { return this.m_CreateDate; }
            set { this.SetValue("CreateDate", ref m_CreateDate, value); }
        }

        private DateTime? m_OutOrRefundDate;
        public DateTime? OutOrRefundDate
        {
            get { return this.m_OutOrRefundDate; }
            set { this.SetValue("OutOrRefundDate", ref m_OutOrRefundDate, value); }
        }
        
        private Decimal? m_TotalAmt;
        public Decimal? TotalAmt
        {
            get { return this.m_TotalAmt-Math.Abs((this.PromotionDiscount?? 0)); }
            set { this.SetValue("TotalAmt", ref m_TotalAmt, value); }
        }

        private Int32? m_StockSysNo;
        public Int32? StockSysNo
        {
            get { return this.m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }


        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { this.SetValue("StockName", ref stockName, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }


        public decimal? TotalAmount 
        {
            get 
            {
                return ((OriginalPrice ?? 0) * (Quantity ?? 0) + (PromotionDiscount ?? 0));
 
            }
        }

        public int? Quantity { get; set; }

        public decimal? OriginalPrice { get; set; }

        public DateTime? OrderDate { get; set; }

        public int? SONumber { get; set; }

        public int? Point { get; set; }

        public int? WarehouseNumber { get; set; }

        public decimal? PromotionDiscount { get; set; }
    }
}
