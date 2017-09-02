using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Inventory.Resources;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class InventoryTransferStockingQueryVM : ModelBase
    {

        public InventoryTransferStockingQueryVM()
        {
            m_ProductStatusCompareCode = "=";
            m_DaySalesCountCompareCode = "<=";
            m_AvailableSaleDaysCompareCode = "<=";
            m_RecommendBackQtyCompareCode = "<=";
            m_AverageUnitCostCompareCode = "<=";
            m_SalePriceCompareCode = "<=";
            m_PointCompareCode = "<=";
            m_FinanceQtyCompareCode = "<=";
            m_AvailableQtyCompareCode = "<=";
            m_OrderedQtyCompareCode = "<=";
            m_SubStockQtyCompareCode = "<=";
            m_ConsignQtyCompareCode = "<=";
            m_OccupiedQtyCompareCode = "<=";
            m_OnlineQtyCompareCode = "<=";
            m_VirtualQtyCompareCode = "<=";
            m_PurchaseQtyCompareCode = "<=";
            m_StockSysNo = "-999";
            m_BackDay = "10";
            m_UserDefinedBackDay = "10";
            m_Category1SysNo = null;
            m_Category2SysNo = null;
            m_Category3SysNo = null;
            m_PMSysNo = null;
            m_ProductConsignFlag = "-999";
            m_ProductStatus = ECCentral.BizEntity.IM.ProductStatus.Active;
            m_IsAsyncStock = null;
            m_IsLarge = null;
            sortByField = "ItemSysNumber";
            isSortByAsc = false;
            isSortByDesc = true;
            m_VendorSysNoList = new List<int>();
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

        private ProductType? m_ProductType;
        public ProductType? ProductType
        {
            get { return this.m_ProductType; }
            set { this.SetValue("ProductType", ref m_ProductType, value); }
        }

        private String m_StockSysNo;
        public String StockSysNo
        {
            get { return this.m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private String m_Category1SysNo;
        public String Category1SysNo
        {
            get { return this.m_Category1SysNo; }
            set { this.SetValue("Category1SysNo", ref m_Category1SysNo, value); }
        }

        private String m_Category2SysNo;
        public String Category2SysNo
        {
            get { return this.m_Category2SysNo; }
            set { this.SetValue("Category2SysNo", ref m_Category2SysNo, value); }
        }

        private String m_Category3SysNo;
        public String Category3SysNo
        {
            get { return this.m_Category3SysNo; }
            set { this.SetValue("Category3SysNo", ref m_Category3SysNo, value); }
        }

        private String m_PMSysNo;
        public String PMSysNo
        {
            get { return this.m_PMSysNo; }
            set { this.SetValue("PMSysNo", ref m_PMSysNo, value); }
        }

        private String m_BackDay;
        [Validate(ValidateType.Interger)]
        public String BackDay
        {
            get { return this.m_BackDay; }
            set { this.SetValue("BackDay", ref m_BackDay, value); }
        }

        private string m_UserDefinedBackDay;
        [Validate(ValidateType.Interger)]
        public string UserDefinedBackDay
        {
            get { return m_UserDefinedBackDay; }
            set { this.SetValue("UserDefinedBackDay", ref m_UserDefinedBackDay, value); }
        }

        private String m_ProductConsignFlag;
        public String ProductConsignFlag
        {
            get { return this.m_ProductConsignFlag; }
            set { this.SetValue("ProductConsignFlag", ref m_ProductConsignFlag, value); }
        }

        private String m_SysNO;
        [Validate(ValidateType.Interger)]
        public String SysNO
        {
            get { return this.m_SysNO; }
            set { this.SetValue("SysNO", ref m_SysNO, value); }
        }

        private String m_ProductStatusCompareCode;
        public String ProductStatusCompareCode
        {
            get { return this.m_ProductStatusCompareCode; }
            set { this.SetValue("ProductStatusCompareCode", ref m_ProductStatusCompareCode, value); }
        }

        private ProductStatus? m_ProductStatus;
        public ProductStatus? ProductStatus
        {
            get { return this.m_ProductStatus; }
            set { this.SetValue("ProductStatus", ref m_ProductStatus, value); }
        }

        private String m_DaySalesCountCompareCode;
        public String DaySalesCountCompareCode
        {
            get { return this.m_DaySalesCountCompareCode; }
            set { this.SetValue("DaySalesCountCompareCode", ref m_DaySalesCountCompareCode, value); }
        }

        private String m_DaySalesCount;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String DaySalesCount
        {
            get { return this.m_DaySalesCount; }
            set { this.SetValue("DaySalesCount", ref m_DaySalesCount, value); }
        }

        private String m_AvailableSaleDaysCompareCode;
        public String AvailableSaleDaysCompareCode
        {
            get { return this.m_AvailableSaleDaysCompareCode; }
            set { this.SetValue("AvailableSaleDaysCompareCode", ref m_AvailableSaleDaysCompareCode, value); }
        }

        private String m_AvailableSaleDays;
        [Validate(ValidateType.Interger)]
        public String AvailableSaleDays
        {
            get { return this.m_AvailableSaleDays; }
            set { this.SetValue("AvailableSaleDays", ref m_AvailableSaleDays, value); }
        }

        private String m_RecommendBackQtyCompareCode;

        public String RecommendBackQtyCompareCode
        {
            get { return this.m_RecommendBackQtyCompareCode; }
            set { this.SetValue("RecommendBackQtyCompareCode", ref m_RecommendBackQtyCompareCode, value); }
        }

        private String m_RecommendBackQty;
        [Validate(ValidateType.Interger)]
        public String RecommendBackQty
        {
            get { return this.m_RecommendBackQty; }
            set { this.SetValue("RecommendBackQty", ref m_RecommendBackQty, value); }
        }

        private String m_ManufacturerName;
        public String ManufacturerName
        {
            get { return this.m_ManufacturerName; }
            set { this.SetValue("ManufacturerName", ref m_ManufacturerName, value); }
        }

        private List<int> m_VendorSysNoList;
        public List<int> VendorSysNoList
        {
            get { return m_VendorSysNoList; }
            set { this.SetValue("VendorSysNoList", ref m_VendorSysNoList, value); }
        }


        private String m_VendorName;
        public String VendorName
        {
            get { return this.m_VendorName; }
            set { this.SetValue("VendorName", ref m_VendorName, value); }
        }

        private String m_BrandName;
        public String BrandName
        {
            get { return this.m_BrandName; }
            set { this.SetValue("BrandName", ref m_BrandName, value); }
        }

        private String m_AverageUnitCostCompareCode;
        public String AverageUnitCostCompareCode
        {
            get { return this.m_AverageUnitCostCompareCode; }
            set { this.SetValue("AverageUnitCostCompareCode", ref m_AverageUnitCostCompareCode, value); }
        }

        private String m_AverageUnitCost;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String AverageUnitCost
        {
            get { return this.m_AverageUnitCost; }
            set { this.SetValue("AverageUnitCost", ref m_AverageUnitCost, value); }
        }

        private String m_SalePriceCompareCode;
        public String SalePriceCompareCode
        {
            get { return this.m_SalePriceCompareCode; }
            set { this.SetValue("SalePriceCompareCode", ref m_SalePriceCompareCode, value); }
        }

        private String m_SalePrice;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String SalePrice
        {
            get { return this.m_SalePrice; }
            set { this.SetValue("SalePrice", ref m_SalePrice, value); }
        }

        private String m_PointCompareCode;
        public String PointCompareCode
        {
            get { return this.m_PointCompareCode; }
            set { this.SetValue("PointCompareCode", ref m_PointCompareCode, value); }
        }

        private String m_Point;
        [Validate(ValidateType.Interger)]
        public String Point
        {
            get { return this.m_Point; }
            set { this.SetValue("Point", ref m_Point, value); }
        }

        private String m_FinanceQtyCompareCode;
        public String FinanceQtyCompareCode
        {
            get { return this.m_FinanceQtyCompareCode; }
            set { this.SetValue("FinanceQtyCompareCode", ref m_FinanceQtyCompareCode, value); }
        }

        private String m_FinanceQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String FinanceQty
        {
            get { return this.m_FinanceQty; }
            set { this.SetValue("FinanceQty", ref m_FinanceQty, value); }
        }

        private String m_AvailableQtyCompareCode;
        public String AvailableQtyCompareCode
        {
            get { return this.m_AvailableQtyCompareCode; }
            set { this.SetValue("AvailableQtyCompareCode", ref m_AvailableQtyCompareCode, value); }
        }

        private String m_AvailableQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String AvailableQty
        {
            get { return this.m_AvailableQty; }
            set { this.SetValue("AvailableQty", ref m_AvailableQty, value); }
        }

        private String m_OrderedQtyCompareCode;
        public String OrderedQtyCompareCode
        {
            get { return this.m_OrderedQtyCompareCode; }
            set { this.SetValue("OrderedQtyCompareCode", ref m_OrderedQtyCompareCode, value); }
        }

        private String m_OrderedQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String OrderedQty
        {
            get { return this.m_OrderedQty; }
            set { this.SetValue("OrderedQty", ref m_OrderedQty, value); }
        }

        private String m_SubStockQtyCompareCode;
        public String SubStockQtyCompareCode
        {
            get { return this.m_SubStockQtyCompareCode; }
            set { this.SetValue("SubStockQtyCompareCode", ref m_SubStockQtyCompareCode, value); }
        }

        private String m_SubStockQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String SubStockQty
        {
            get { return this.m_SubStockQty; }
            set { this.SetValue("SubStockQty", ref m_SubStockQty, value); }
        }

        private String m_ConsignQtyCompareCode;
        public String ConsignQtyCompareCode
        {
            get { return this.m_ConsignQtyCompareCode; }
            set { this.SetValue("ConsignQtyCompareCode", ref m_ConsignQtyCompareCode, value); }
        }

        private String m_ConsignQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String ConsignQty
        {
            get { return this.m_ConsignQty; }
            set { this.SetValue("ConsignQty", ref m_ConsignQty, value); }
        }

        private String m_OccupiedQtyCompareCode;
        public String OccupiedQtyCompareCode
        {
            get { return this.m_OccupiedQtyCompareCode; }
            set { this.SetValue("OccupiedQtyCompareCode", ref m_OccupiedQtyCompareCode, value); }
        }

        private String m_OccupiedQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String OccupiedQty
        {
            get { return this.m_OccupiedQty; }
            set { this.SetValue("OccupiedQty", ref m_OccupiedQty, value); }
        }

        private String m_OnlineQtyCompareCode;
        public String OnlineQtyCompareCode
        {
            get { return this.m_OnlineQtyCompareCode; }
            set { this.SetValue("OnlineQtyCompareCode", ref m_OnlineQtyCompareCode, value); }
        }

        private String m_OnlineQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String OnlineQty
        {
            get { return this.m_OnlineQty; }
            set { this.SetValue("OnlineQty", ref m_OnlineQty, value); }
        }

        private YNStatus? m_IsAsyncStock;
        public YNStatus? IsAsyncStock
        {
            get { return this.m_IsAsyncStock; }
            set { this.SetValue("IsAsyncStock", ref m_IsAsyncStock, value); }
        }

        private String m_VirtualQtyCompareCode;
        public String VirtualQtyCompareCode
        {
            get { return this.m_VirtualQtyCompareCode; }
            set { this.SetValue("VirtualQtyCompareCode", ref m_VirtualQtyCompareCode, value); }
        }

        private String m_VirtualQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String VirtualQty
        {
            get { return this.m_VirtualQty; }
            set { this.SetValue("VirtualQty", ref m_VirtualQty, value); }
        }

        private String m_PurchaseQtyCompareCode;
        public String PurchaseQtyCompareCode
        {
            get { return this.m_PurchaseQtyCompareCode; }
            set { this.SetValue("PurchaseQtyCompareCode", ref m_PurchaseQtyCompareCode, value); }
        }

        private String m_PurchaseQty;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessageResourceName = "ErrorMsg_ValidateDecimal", ErrorMessageResourceType = typeof(ResTransferStockingCenter))]
        public String PurchaseQty
        {
            get { return this.m_PurchaseQty; }
            set { this.SetValue("PurchaseQty", ref m_PurchaseQty, value); }
        }

        private YNStatus? m_IsLarge;
        public YNStatus? IsLarge
        {
            get { return this.m_IsLarge; }
            set { this.SetValue("IsLarge", ref m_IsLarge, value); }
        }

        private string sortByField;

        public string SortByField
        {
            get { return sortByField; }
            set { this.SetValue("SortByField", ref sortByField, value); }
        }

        private bool isSortByAsc;

        public bool IsSortByAsc
        {
            get { return isSortByAsc; }
            set { this.SetValue("IsSortByAsc", ref isSortByAsc, value); }
        }
        private bool isSortByDesc;

        public bool IsSortByDesc
        {
            get { return isSortByDesc; }
            set { this.SetValue("IsSortByDesc", ref isSortByDesc, value); }
        }
        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { this.SetValue("CompanyCode", ref companyCode, value); }
        }
    }
}
