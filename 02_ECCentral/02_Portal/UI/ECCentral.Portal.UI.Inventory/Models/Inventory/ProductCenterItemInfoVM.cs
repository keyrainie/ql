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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ProductCenterItemInfoVM : ModelBase
    {
        public ProductCenterItemInfoVM()
        {
            isChecked = false;
            m_SuggestTransferStocks = new List<ProductCenterStockInfoVM>();
        }

        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { this.SetValue("IsChecked", ref isChecked, value); }
        }

        private Int32? m_ItemSysNumber;
        public Int32? ItemSysNumber
        {
            get { return this.m_ItemSysNumber; }
            set { this.SetValue("ItemSysNumber", ref m_ItemSysNumber, value); }
        }

        private String m_ItemCode;
        public String ItemCode
        {
            get { return this.m_ItemCode; }
            set { this.SetValue("ItemCode", ref m_ItemCode, value); }
        }

        private String m_ItemName;
        public String ItemName
        {
            get { return this.m_ItemName; }
            set { this.SetValue("ItemName", ref m_ItemName, value); }
        }

        private Int32? m_ProductStatus;
        public Int32? ProductStatus
        {
            get { return this.m_ProductStatus; }
            set { this.SetValue("ProductStatus", ref m_ProductStatus, value); }
        }

        private Int32? m_AllAvailableQty;
        public Int32? AllAvailableQty
        {
            get { return this.m_AllAvailableQty; }
            set { this.SetValue("AllAvailableQty", ref m_AllAvailableQty, value); }
        }

        private Int32? m_ConsignQty;
        public Int32? ConsignQty
        {
            get { return this.m_ConsignQty; }
            set { this.SetValue("ConsignQty", ref m_ConsignQty, value); }
        }

        private Int32? m_IsSynProduct;
        public Int32? IsSynProduct
        {
            get { return this.m_IsSynProduct; }
            set { this.SetValue("IsSynProduct", ref m_IsSynProduct, value); }
        }

        private String m_SynProductID;
        public String SynProductID
        {
            get { return this.m_SynProductID; }
            set { this.SetValue("SynProductID", ref m_SynProductID, value); }
        }

        private String m_PartnerType;
        public String PartnerType
        {
            get { return this.m_PartnerType; }
            set { this.SetValue("PartnerType", ref m_PartnerType, value); }
        }

        private Int32? m_InventoryQty;
        public Int32? InventoryQty
        {
            get { return this.m_InventoryQty ?? 0; }
            set { this.SetValue("InventoryQty", ref m_InventoryQty, value); }
        }

        private Decimal? m_PurchasePrice;
        public Decimal? PurchasePrice
        {
            get { return this.m_PurchasePrice ?? 0.00M; }
            set { this.SetValue("PurchasePrice", ref m_PurchasePrice, value); }
        }

        private String m_ProductDescription;
        public String ProductDescription
        {
            get { return this.m_ProductDescription; }
            set { this.SetValue("ProductDescription", ref m_ProductDescription, value); }
        }

        private Int32? m_PurchaseQty;
        public Int32? PurchaseQty
        {
            get { return this.m_PurchaseQty ?? 0; }
            set { this.SetValue("PurchaseQty", ref m_PurchaseQty, value); }
        }

        private String m_UnmarketableQty;
        public String UnmarketableQty
        {
            get { return this.m_UnmarketableQty; }
            set { this.SetValue("UnmarketableQty", ref m_UnmarketableQty, value); }
        }

        private Int32? m_InstockDays;
        public Int32? InstockDays
        {
            get { return this.m_InstockDays.HasValue?this.m_InstockDays.Value:0; }
            set { this.SetValue("InstockDays", ref m_InstockDays, value); }
        }

        private Decimal? m_VirtualPrice;
        public Decimal? VirtualPrice
        {
            get { return this.m_VirtualPrice.HasValue ? m_VirtualPrice.Value : 0.00M; }
            set { this.SetValue("VirtualPrice", ref m_VirtualPrice, value); }
        }

        private Int32? m_OrderQty;
        public Int32? OrderQty
        {
            get { return this.m_OrderQty; }
            set { this.SetValue("OrderQty", ref m_OrderQty, value); }
        }

        private Int32? m_VirtualQty;
        public Int32? VirtualQty
        {
            get { return this.m_VirtualQty; }
            set { this.SetValue("VirtualQty", ref m_VirtualQty, value); }
        }

        private Int32? m_TransferStockQty;
        public Int32? TransferStockQty
        {
            get { return this.m_TransferStockQty; }
            set { this.SetValue("TransferStockQty", ref m_TransferStockQty, value); }
        }

        private Int32? m_SuggestQtyAll;
        public Int32? SuggestQtyAll
        {
            get { return this.m_SuggestQtyAll; }
            set { this.SetValue("SuggestQtyAll", ref m_SuggestQtyAll, value); }
        }

        public Int32? m_SuggestQtyAllDisplay; //页面显示（根据是否中专 选择性显示 SuggestQtyAll 或者 SuggestQtyAllZhongZhuan）
        public Int32? SuggestQtyAllDisplay
        {
            get { return this.m_SuggestQtyAllDisplay; }
            set { this.SetValue("SuggestQtyAllDisplay", ref m_SuggestQtyAllDisplay, value); }
        }

        private Int32? m_SuggestQtyAllZhongZhuan;
        public Int32? SuggestQtyAllZhongZhuan
        {
            get { return this.m_SuggestQtyAllZhongZhuan; }
            set { this.SetValue("SuggestQtyAllZhongZhuan", ref m_SuggestQtyAllZhongZhuan, value); }
        }

        private Decimal? m__unitCost;
        public Decimal? _unitCost
        {
            get { return this.m__unitCost; }
            set { this.SetValue("_unitCost", ref m__unitCost, value); }
        }

        private Decimal? m_UnitCost;
        public Decimal? UnitCost
        {
            get { return this.m_UnitCost; }
            set { this.SetValue("UnitCost", ref m_UnitCost, value); }
        }

        private Int32? m_D1;
        public Int32? D1
        {
            get { return this.m_D1.HasValue ? m_D1.Value : 0; }
            set { this.SetValue("D1", ref m_D1, value); }
        }

        private Int32? m_D2;
        public Int32? D2
        {
            get { return this.m_D2.HasValue ? m_D2.Value : 0; }
            set { this.SetValue("D2", ref m_D2, value); }
        }

        private Int32? m_D3;
        public Int32? D3
        {
            get { return this.m_D3.HasValue ? m_D3.Value : 0; }
            set { this.SetValue("D3", ref m_D3, value); }
        }

        private Int32? m_D4;
        public Int32? D4
        {
            get { return this.m_D4.HasValue ? m_D4.Value : 0; }
            set { this.SetValue("D4", ref m_D4, value); }
        }

        private Int32? m_D5;
        public Int32? D5
        {
            get { return this.m_D5.HasValue ? m_D5.Value : 0; }
            set { this.SetValue("D5", ref m_D5, value); }
        }

        private Int32? m_D6;
        public Int32? D6
        {
            get { return this.m_D6.HasValue ? m_D6.Value : 0; }
            set { this.SetValue("D6", ref m_D6, value); }
        }

        private Int32? m_D7;
        public Int32? D7
        {
            get { return this.m_D7.HasValue ? m_D7.Value : 0; }
            set { this.SetValue("D7", ref m_D7, value); }
        }

        private Int32? m_D123;
        public Int32? D123
        {
            get { return this.m_D123.HasValue ? m_D123.Value : 0; }
            set { this.SetValue("D123", ref m_D123, value); }
        }

        private Int32? m_W1;
        public Int32? W1
        {
            get { return this.m_W1.HasValue ? m_W1.Value : 0; }
            set { this.SetValue("W1", ref m_W1, value); }
        }

        private Int32? m_W2;
        public Int32? W2
        {
            get { return this.m_W2.HasValue ? m_W2.Value : 0; }
            set { this.SetValue("W2", ref m_W2, value); }
        }

        private Int32? m_W3;
        public Int32? W3
        {
            get { return this.m_W3.HasValue ? m_W3.Value : 0; }
            set { this.SetValue("W3", ref m_W3, value); }
        }

        private Int32? m_W4;
        public Int32? W4
        {
            get { return this.m_W4.HasValue ? m_W4.Value : 0; }
            set { this.SetValue("W4", ref m_W4, value); }
        }

        private Int32? m_M1;
        public Int32? M1
        {
            get { return this.m_M1.HasValue ? m_M1.Value : 0; }
            set { this.SetValue("M1", ref m_M1, value); }
        }

        private Int32? m_M2;
        public Int32? M2
        {
            get { return this.m_M2.HasValue ? m_M2.Value : 0; }
            set { this.SetValue("M2", ref m_M2, value); }
        }

        private Int32? m_M3;
        public Int32? M3
        {
            get { return this.m_M3.HasValue ? m_M3.Value : 0; }
            set { this.SetValue("M3", ref m_M3, value); }
        }

        private String m_PO_Memo;
        public String PO_Memo
        {
            get { return this.m_PO_Memo; }
            set { this.SetValue("PO_Memo", ref m_PO_Memo, value); }
        }

        private Decimal? m_CurrentPrice;
        public Decimal? CurrentPrice
        {
            get { return this.m_CurrentPrice; }
            set { this.SetValue("CurrentPrice", ref m_CurrentPrice, value); }
        }

        private Decimal? m_JDPrice;
        public Decimal? JDPrice
        {
            get { return this.m_JDPrice; }
            set { this.SetValue("JDPrice", ref m_JDPrice, value); }
        }

        private Int32? m_IsConsign;
        public Int32? IsConsign
        {
            get { return this.m_IsConsign; }
            set { this.SetValue("IsConsign", ref m_IsConsign, value); }
        }

        private String m_VFType;
        public String VFType
        {
            get { return this.m_VFType; }
            set { this.SetValue("VFType", ref m_VFType, value); }
        }

        private Decimal? m_AllStockAVGDailySales;
        public Decimal? AllStockAVGDailySales
        {
            get { return this.m_AllStockAVGDailySales; }
            set { this.SetValue("AllStockAVGDailySales", ref m_AllStockAVGDailySales, value); }
        }

        private Int32? m_AllStockAvailableSalesDays;
        public Int32? AllStockAvailableSalesDays
        {
            get
            {
                if (VirtualQty > 0)
                {
                    return 0;
                }
                else if (AllStockAVGDailySales == 0 && (AllAvailableQty + ConsignQty) > 0)
                {
                    return 999;
                }
                else
                {
                    return m_AllStockAvailableSalesDays;
                }
            }

        }

        private String m_BrandCh;
        public String BrandCh
        {
            get { return this.m_BrandCh; }
            set { this.SetValue("BrandCh", ref m_BrandCh, value); }
        }

        private String m_BrandEn;
        public String BrandEn
        {
            get { return this.m_BrandEn; }
            set { this.SetValue("BrandEn", ref m_BrandEn, value); }
        }

        private String m_Brand;
        public String Brand
        {
            get { return this.m_Brand; }
            set { this.SetValue("Brand", ref m_Brand, value); }
        }

        private String m_ManufacturerName;
        public String ManufacturerName
        {
            get { return this.m_ManufacturerName; }
            set { this.SetValue("ManufacturerName", ref m_ManufacturerName, value); }
        }

        private List<ProductCenterStockInfoVM> m_SuggestTransferStocks;
        public List<ProductCenterStockInfoVM> SuggestTransferStocks
        {
            get { return this.m_SuggestTransferStocks; }
            set { this.SetValue("SuggestTransferStocks", ref m_SuggestTransferStocks, value); }
        }

        private String m_IsBatch;
        public String IsBatch
        {
            get { return this.m_IsBatch; }
            set { this.SetValue("IsBatch", ref m_IsBatch, value); }
        }

        private Int32? m_AllOutStockQuantity;
        public Int32? AllOutStockQuantity
        {
            get { return this.m_AllOutStockQuantity.HasValue ? m_AllOutStockQuantity.Value : 0; }
            set { this.SetValue("AllOutStockQuantity", ref m_AllOutStockQuantity, value); }
        }

        private String m_JDItemNumber;
        public String JDItemNumber
        {
            get { return this.m_JDItemNumber; }
            set { this.SetValue("JDItemNumber", ref m_JDItemNumber, value); }
        }

        private Int32? m_Point;
        public Int32? Point
        {
            get { return this.m_Point; }
            set { this.SetValue("Point", ref m_Point, value); }
        }

        private String m_GrossProfitRate;
        public String GrossProfitRate
        {
            get { return this.m_GrossProfitRate; }
            set { this.SetValue("GrossProfitRate", ref m_GrossProfitRate, value); }
        }

        #region ComboBox数据源
        public List<CodeNamePair> OutStockList_SH { get; set; }
        public List<CodeNamePair> OutStockList_BJ { get; set; }
        public List<CodeNamePair> OutStockList_WH { get; set; }
        public List<CodeNamePair> OutStockList_GZ { get; set; }
        public List<CodeNamePair> OutStockList_CD { get; set; }
        public List<CodeNamePair> OutStockList_KM { get; set; }
        public List<CodeNamePair> OutStockList_SZ { get; set; }

        public List<KeyValuePair<YNStatus?, string>> YNStatusList { get; set; }

        #endregion

    }
}
