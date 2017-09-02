using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;

namespace ECCentral.Portal.Basic.Components.UserControls.ProductPicker
{
    public delegate void ProductIsCheckedChange(bool ischecked, ProductVM vm);
    public class ProductVM : ModelBase
    {
        #region 界面展示专用属性

        public ProductIsCheckedChange onIsCheckedChange;
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                base.SetValue("IsChecked", ref _isChecked, value);
                if (onIsCheckedChange != null)
                    onIsCheckedChange(value, this);
            }
        }
        #endregion

        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

        public int? PromotionSysNo { get; set; }

        private string _productID;
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }

        private int _isHasBatch;
        /// <summary>
        /// 商品是否有批次信息
        /// 1:有批次信息
        /// 0：无批次信息
        /// </summary>
        public int IsHasBatch
        {
            get { return _isHasBatch; }
            set
            {
                base.SetValue("IsHasBatch", ref _isHasBatch, value);
            }
        }

        private ProductInventoryType inventoryType;
        /// <summary>
        /// 库存模式
        /// </summary>
        public ProductInventoryType InventoryType
        {
            get { return inventoryType; }
            set { base.SetValue("InventoryType", ref inventoryType, value); }
        }

        private string _IsConsign;
        public string IsConsign
        {
            get { return _IsConsign; }
            set { SetValue("IsConsign", ref _IsConsign, value); }
        }

        private string _productName;
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            set
            {
                base.SetValue("ProductName", ref _productName, value);
            }
        }

        private ProductType? _productType;
        /// <summary>
        /// 商品名称
        /// </summary>
        public ProductType? ProductType
        {
            get { return _productType; }
            set
            {
                base.SetValue("ProductType", ref _productType, value);
            }
        }

        private ProductStatus? _productStatus;
        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus? Status
        {
            get { return _productStatus; }
            set
            {
                base.SetValue("Status", ref _productStatus, value);
            }
        }


        private decimal _unitCost;
        /// <summary>
        /// 商品成本
        /// </summary>
        public decimal UnitCost
        {
            get { return _unitCost; }
            set
            {
                base.SetValue("UnitCost", ref _unitCost, value);
            }
        }

        private decimal _unitCostWithoutTax;
        /// <summary>
        /// 商品去税成本
        /// </summary>
        public decimal UnitCostWithoutTax
        {
            get { return _unitCostWithoutTax; }
            set
            {
                base.SetValue("UnitCostWithoutTax", ref _unitCostWithoutTax, value);
            }
        }


        private decimal _avgCost;
        /// <summary>
        /// 平均成本
        /// </summary>
        public decimal AvgCost
        {
            get { return _avgCost; }
            set
            {
                base.SetValue("AvgCost", ref _avgCost, value);
            }
        }

        private decimal _salesMargin;
        /// <summary>
        /// 毛利额
        /// </summary>
        public decimal SalesMargin
        {
            get { return _salesMargin; }
            set
            {
                base.SetValue("SalesMargin", ref _salesMargin, value);
            }
        }

        private decimal _salesMarginRate;
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal SalesMarginRate
        {
            get { return _salesMarginRate; }
            set
            {
                base.SetValue("SalesMarginRate", ref _salesMarginRate, value);
            }
        }

        private int _accountQty;
        /// <summary>
        /// 财务库存
        /// </summary>
        public int AccountQty
        {
            get { return _accountQty; }
            set
            {
                base.SetValue("AccountQty", ref _accountQty, value);
            }
        }

        private int _allocatedQty;
        /// <summary>
        /// 已分配库存
        /// </summary>
        public int AllocatedQty
        {
            get { return _allocatedQty; }
            set
            {
                base.SetValue("AllocatedQty", ref _allocatedQty, value);
            }
        }

        private int _availableQty;
        /// <summary>
        /// 可用库存
        /// </summary>
        public int AvailableQty
        {
            get { return _availableQty; }
            set
            {
                base.SetValue("AvailableQty", ref _availableQty, value);
            }
        }

        private int _consignQty;
        /// <summary>
        /// 代销库存
        /// </summary>
        public int ConsignQty
        {
            get { return _consignQty; }
            set
            {
                base.SetValue("ConsignQty", ref _consignQty, value);
            }
        }

        private int _orderQty;
        /// <summary>
        /// 已订购库存
        /// </summary>
        public int OrderQty
        {
            get { return _orderQty; }
            set
            {
                base.SetValue("OrderQty", ref _orderQty, value);
            }
        }

        int? _onlineQty;
        public int? OnlineQty
        {
            get { return _onlineQty; }
            set
            {
                base.SetValue("OnlineQty", ref _onlineQty, value);
            }
        }

        private int _point;
        /// <summary>
        /// 返点
        /// </summary>
        public int Point
        {
            get { return _point; }
            set
            {
                base.SetValue("Point", ref _point, value);
            }
        }

        private decimal _discountAmount;
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal DiscountAmount
        {
            get { return _discountAmount; }
            set
            {
                base.SetValue("DiscountAmount", ref _discountAmount, value);
            }
        }

        private decimal _weight;
        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight
        {
            get { return _weight; }
            set
            {
                base.SetValue("Weight", ref _weight, value);
            }
        }

        private string _pmUserName;
        /// <summary>
        /// PM名称
        /// </summary>
        public string PMUserName
        {
            get { return _pmUserName; }
            set
            {
                base.SetValue("PMUserName", ref _pmUserName, value);
            }
        }

        public int? MasterSysNo
        {
            get;
            set;
        }

        public List<int> MasterSysNos
        {
            get;
            set;
        }

        int? _quantity;
        public int? Quantity
        {
            get { return _quantity; }
            set
            {
                base.SetValue("Quantity", ref _quantity, value);
            }
        }

        /// <summary>
        /// 规则数量（用于保存赠品商品规则设定值）
        /// </summary>
        public int? RuleQty { get; set; }

        #region Order相关

        SOProductType? _soProductType;
        public SOProductType? SOProductType
        {
            get { return _soProductType; }
            set
            {
                base.SetValue("SOProductType", ref _soProductType, value);
            }
        }

        #endregion

        /// <summary>
        /// 保修描述
        /// </summary>
        public string Warranty
        {
            get;
            set;
        }

        List<ExtendedWarrantyVM> _extendedWarrantyList;
        public List<ExtendedWarrantyVM> ExtendedWarrantyList
        {
            get { return _extendedWarrantyList; }
            set
            {
                base.SetValue("ExtendedWarrantyList", ref _extendedWarrantyList, value);
            }
        }
        private decimal _CurrentPrice;

        public decimal CurrentPrice
        {
            get { return _CurrentPrice; }
            set { base.SetValue("CurrentPrice", ref _CurrentPrice, value); }
        }

        private decimal? _PurchasePrice;

        public decimal? PurchasePrice
        {
            get { return _PurchasePrice; }
            set { base.SetValue("PurchasePrice", ref _PurchasePrice, value); }
        }

        public decimal MinMargin { get; set; }

        private string _VFItem;

        public string VFItem
        {
            get { return _VFItem; }
            set { base.SetValue("VFItem", ref _VFItem, value); }
        }

        private decimal? _BasicPrice;

        public decimal? BasicPrice
        {
            get { return _BasicPrice; }
            set { base.SetValue("BasicPrice", ref _BasicPrice, value); }
        }

        private decimal? _VirtualPrice;

        public decimal? VirtualPrice
        {
            get { return _VirtualPrice; }
            set { base.SetValue("VirtualPrice", ref _VirtualPrice, value); }
        }

        private string merchantName;
        public string MerchantName
        {
            get { return merchantName; }
            set
            {
                base.SetValue("MerchantName", ref merchantName, value);
            }
        }

        #region BatchInfo
        public List<BatchInfoVM> ProductBatchLst { get; set; }
        #endregion

        public int GiftVoucherType { get; set; }


        private int c3SysNo;
        public int C3SysNo
        {
            get { return c3SysNo; }
            set
            {
                base.SetValue("C3SysNo", ref c3SysNo, value);
            }
        }

        /// <summary>
        /// 最优库存仓库
        /// </summary>
        public string OptimalizingStock { get; set; }

        /// <summary>
        /// 存储运输方式
        /// </summary>
        public StoreType? StoreType { get; set; }


        Decimal? m_TariffRate;
        /// <summary>
        /// 税率
        /// </summary>
        public Decimal? TariffRate
        {
            get { return this.m_TariffRate; }
            set { this.SetValue("TariffRate", ref m_TariffRate, value); }
        }
    }

    public class ExtendedWarrantyVM : ModelBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 延保服务编号
        /// </summary>
        public int ExtendedWarrantyID { get; set; }

        /// <summary>
        /// 延保服务限定的商品价格下限
        /// </summary>
        public decimal MinUnitPrice { get; set; }

        /// <summary>
        /// 延保服务限定的商品价格上限
        /// </summary>
        public decimal MaxUnitPrice { get; set; }

        /// <summary>
        /// 延保服务年限
        /// </summary>
        public decimal ServiceYears { get; set; }

        /// <summary>
        /// 延保服务价格
        /// </summary>
        public decimal ServiceUnitPrice { get; set; }

        /// <summary>
        /// 延保服务成本
        /// </summary>
        public decimal ServiceCost { get; set; }

        /// <summary>
        /// 类型3的描述
        /// </summary>
        public string C3Name { get; set; }

        /// <summary>
        /// 简短说明
        /// </summary>
        public string BriefName
        {
            get
            {
                return string.Format(ResProductPicker.Tip_BriefNameFormat, C3Name, MinUnitPrice, MaxUnitPrice, ServiceYears, ServiceUnitPrice);
            }
        }

        /// <summary>
        /// 是否被选中(UI显示用，需要双向绑定)
        /// </summary>
        bool _isCheck;
        public bool IsCheck
        {
            get { return _isCheck; }
            set
            {
                base.SetValue("IsCheck", ref _isCheck, value);
            }
        }
    }

    /// <summary>
    /// 商品批次信息
    /// </summary>
    public class BatchInfoVM : ModelBase
    {
        private string batchNumber;
        public string BatchNumber
        {
            get { return batchNumber; }
            set
            {
                this.SetValue("BatchNumber", ref batchNumber, value);
            }
        }

        private DateTime inDate;
        public DateTime InDate
        {
            get { return inDate; }
            set { this.SetValue("InDate", ref inDate, value); }
        }

        private DateTime expDate;
        public DateTime ExpDate
        {
            get { return expDate; }
            set { this.SetValue("ExpDate", ref expDate, value); }
        }

        private int _MaxDeliveryDays;
        public int MaxDeliveryDays
        {
            get { return _MaxDeliveryDays; }
            set { this.SetValue("MaxDeliveryDays", ref _MaxDeliveryDays, value); }
        }

        private string lotNo;
        public string LotNo
        {
            get { return lotNo; }
            set
            {
                this.SetValue("LotNo", ref lotNo, value);
            }
        }

        private int _ActualQty;
        public int ActualQty
        {
            get { return _ActualQty; }
            set { this.SetValue("ActualQty", ref _ActualQty, value); }
        }

        private int _AllocatedQty;
        public int AllocatedQty
        {
            get { return _AllocatedQty; }
            set { this.SetValue("AllocatedQty", ref _AllocatedQty, value); }
        }

        private int _OperateNum;
        public int Quantity
        {
            get { return _OperateNum; }
            set { this.SetValue("Quantity", ref _OperateNum, value); }
        }

        private int _returnNum;
        public int ReturnQuantity
        {
            get { return _returnNum; }
            set { this.SetValue("ReturnQuantity", ref _returnNum, value); }
        }

        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public int StockSysNo { get; set; }

        public ProductInventoryType InventoryType { get; set; }

    }
}
