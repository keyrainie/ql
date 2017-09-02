using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;
using System.Windows;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignToAcctLogInfoVM : ModelBase
    {
        private Int32? m_LogSysNo;
        public Int32? LogSysNo
        {
            get { return this.m_LogSysNo; }
            set { this.SetValue("LogSysNo", ref m_LogSysNo, value); }
        }

        private Int32? m_VendorSysNo;
        public Int32? VendorSysNo
        {
            get { return this.m_VendorSysNo; }
            set { this.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }

        private Int32? m_StockSysNo;
        public Int32? StockSysNo
        {
            get { return this.m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        /// <summary>
        /// 仓库名称
        /// </summary>
        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { this.SetValue("StockName", ref stockName, value); }
        }

        private Int32? m_OrderSysNo;
        public Int32? OrderSysNo
        {
            get { return this.m_OrderSysNo; }
            set { this.SetValue("OrderSysNo", ref m_OrderSysNo, value); }
        }

        private ConsignToAccountType? m_ConsignToAccType;
        public ConsignToAccountType? ConsignToAccType
        {
            get { return this.m_ConsignToAccType; }
            set { this.SetValue("ConsignToAccType", ref m_ConsignToAccType, value); }
        }

        private ConsignToAccountLogStatus? m_ConsignToAccStatus;
        public ConsignToAccountLogStatus? ConsignToAccStatus
        {
            get { return this.m_ConsignToAccStatus; }
            set { this.SetValue("ConsignToAccStatus", ref m_ConsignToAccStatus, value); }
        }

        private SettleType? m_SettleType;
        public SettleType? SettleType
        {
            get { return this.m_SettleType; }
            set { this.SetValue("SettleType", ref m_SettleType, value); }
        }

        private Int32? m_OrderCount;
        public Int32? OrderCount
        {
            get { return this.m_OrderCount; }
            set { this.SetValue("OrderCount", ref m_OrderCount, value); }
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

        private Decimal? m_CreateCost;
        public Decimal? CreateCost
        {
            get { return this.m_CreateCost; }
            set { this.SetValue("CreateCost", ref m_CreateCost, value); }
        }

        private string m_Cost;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string Cost
        {
            get { return this.m_Cost; }
            set { this.SetValue("Cost", ref m_Cost, value); }
        }

        private Decimal? m_FoldCost;
        public Decimal? FoldCost
        {
            get { return this.m_FoldCost; }
            set { this.SetValue("FoldCost", ref m_FoldCost, value); }
        }

        private Decimal? m_SettleCost;
        public Decimal? SettleCost
        {
            get { return this.m_SettleCost; }
            set { this.SetValue("SettleCost", ref m_SettleCost, value); }
        }

        private Int32? m_Point;
        public Int32? Point
        {
            get { return this.m_Point; }
            set { this.SetValue("Point", ref m_Point, value); }
        }

        private Decimal? m_RateMargin;
        public Decimal? RateMargin
        {
            get { return this.m_RateMargin; }
            set { this.SetValue("RateMargin", ref m_RateMargin, value); }
        }

        private Decimal? m_CountMany;
        public Decimal? CountMany
        {
            get { return this.m_CountMany; }
            set { this.SetValue("CountMany", ref m_CountMany, value); }
        }

        private Decimal? m_TotalAmt;
        public Decimal? TotalAmt
        {
            get { return this.m_TotalAmt; }
            set { this.SetValue("TotalAmt", ref m_TotalAmt, value); }
        }

        private Decimal? m_RateMarginTotal;
        public Decimal? RateMarginTotal
        {
            get { return this.m_RateMarginTotal; }
            set { this.SetValue("RateMarginTotal", ref m_RateMarginTotal, value); }
        }

        private Decimal? m_MinCommission;
        public Decimal? MinCommission
        {
            get { return this.m_MinCommission; }
            set { this.SetValue("MinCommission", ref m_MinCommission, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public String m_StoreCompanyCode;
        public String StoreCompanyCode
        {
            get { return this.m_StoreCompanyCode; }
            set { this.SetValue("StoreCompanyCode", ref m_StoreCompanyCode, value); }
        }
        /// <summary>
        /// 单据出库时间
        /// </summary>
        public DateTime? m_OutStockTime;
        public DateTime? OutStockTime
        {
            get { return this.m_OutStockTime; }
            set { this.SetValue("OutStockTime", ref m_OutStockTime, value); }
        }
    }
}
