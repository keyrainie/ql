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
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Service.Utility;

namespace ECCentral.Portal.UI.PO.Models
{
    public class PurchaseOrderItemInfoVM : ModelBase
    {

        private int? itemSysNo;

        public int? ItemSysNo
        {
            get { return itemSysNo; }
            set { this.SetValue("ItemSysNo", ref itemSysNo, value); }
        }

        private string batchInfo;

        public string BatchInfo
        {
            get { return batchInfo; }
            set { this.SetValue("BatchInfo", ref batchInfo, value); }
        }

        private int? pOSysNo;

        public int? POSysNo
        {
            get { return pOSysNo; }
            set { this.SetValue("POSysNo", ref pOSysNo, value); }
        }

        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { this.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private string briefName;

        public string BriefName
        {
            get { return briefName; }
            set { this.SetValue("BriefName", ref briefName, value); }
        }

        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { this.SetValue("StockName", ref stockName, value); }
        }

        private int? quantity;

        public int? Quantity
        {
            get { return quantity; }
            set { this.SetValue("Quantity", ref quantity, value); }
        }

        private int? weight;

        public int? Weight
        {
            get { return weight; }
            set { this.SetValue("Weight", ref weight, value); }
        }

        private int? currencyCode;

        public int? CurrencyCode
        {
            get { return currencyCode; }
            set { this.SetValue("CurrencyCode", ref currencyCode, value); }
        }

        private string currencySymbol;

        public string CurrencySymbol
        {
            get { return currencySymbol; }
            set { this.SetValue("CurrencySymbol", ref currencySymbol, value); }
        }

        private string orderPrice;

        /// <summary>
        /// 采购价格
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string OrderPrice
        {
            get { return orderPrice; }
            set { this.SetValue("OrderPrice", ref orderPrice, value); }
        }

        public string OrderPriceDisplay
        {
            get
            {
                decimal output = 0m;
                if (!decimal.TryParse(orderPrice, out output))
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + decimal.Parse(orderPrice).ToString("f2");
                }
            }
        }

        /// <summary>
        /// 价格变动百分比
        /// </summary>
        public decimal? PriceChangePercent { get; set; }

        private decimal? apportionAddOn; ////摊销被取消

        public decimal? ApportionAddOn
        {
            get { return apportionAddOn; }
            set { this.SetValue("ApportionAddOn", ref apportionAddOn, value); }
        }

        private decimal? unitCost;   ////采购成本

        public decimal? UnitCost
        {
            get { return unitCost; }
            set { this.SetValue("UnitCost", ref unitCost, value); }
        }

        private decimal? currentUnitCost;////当前成本

        public decimal? CurrentUnitCost
        {
            get { return currentUnitCost; }
            set { this.SetValue("CurrentUnitCost", ref currentUnitCost, value); }
        }

        public string CurrentUnitCostDisplay
        {
            get
            {
                if (!currentUnitCost.HasValue || currentUnitCost <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + currentUnitCost.Value.ToString("f2");
                }
            }
        }

        private decimal? returnCost;

        public decimal? ReturnCost
        {
            get { return returnCost; }
            set { this.SetValue("ReturnCost", ref returnCost, value); }
        }

        public string ReturnCostDisplay
        {
            get
            {
                if (!returnCost.HasValue || returnCost <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + returnCost.Value.ToString("f2");
                }
            }
        }

        private decimal? lineReturnedPointCost;

        public decimal? LineReturnedPointCost
        {
            get { return lineReturnedPointCost; }
            set { this.SetValue("LineReturnedPointCost", ref lineReturnedPointCost, value); }
        }

        public string LineReturnedPointCostDisplay
        {
            get
            {
                if (!lineReturnedPointCost.HasValue || lineReturnedPointCost <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + lineReturnedPointCost.Value.ToString("f2");
                }
            }
        }

        private decimal? purchasePrice;////采购价格

        public decimal? PurchasePrice
        {
            get { return purchasePrice; }
            set { this.SetValue("PurchasePrice", ref purchasePrice, value); }
        }

        public string PurchasePriceDisplay
        {
            get
            {
                if (!purchasePrice.HasValue || purchasePrice <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + purchasePrice.Value.ToString("f2");
                }
            }
        }

        private string purchaseQty;

        /// <summary>
        /// 采购数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string PurchaseQty
        {
            get { return purchaseQty; }
            set { this.SetValue("PurchaseQty", ref purchaseQty, value); }
        }

        /// <summary>
        /// 移仓在途库存
        /// </summary>
        private int? shiftQty;

        public int? ShiftQty
        {
            get { return shiftQty; }
            set { this.SetValue("ShiftQty", ref shiftQty, value); }
        }

        private PurchaseOrdeItemCheckStatus? checkStatus;

        public PurchaseOrdeItemCheckStatus? CheckStatus
        {
            get { return checkStatus; }
            set { this.SetValue("CheckStatus", ref checkStatus, value); }
        }

        private string checkReasonMemo;

        public string CheckReasonMemo
        {
            get { return checkReasonMemo; }
            set { this.SetValue("CheckReasonMemo", ref checkReasonMemo, value); }
        }

        private decimal? lastOrderPrice;

        public decimal? LastOrderPrice
        {
            get { return lastOrderPrice; }
            set { this.SetValue("LastOrderPrice", ref lastOrderPrice, value); }
        }

        public string LastOrderPriceDisplay
        {
            get
            {
                if (!lastOrderPrice.HasValue || lastOrderPrice <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + lastOrderPrice.Value.ToString("f2");
                }
            }
        }

        /// <summary>
        /// 总价:Order Price * PurchaseQty:
        /// </summary>

        public decimal? TotalPrice
        {
            get { return (string.IsNullOrEmpty(orderPrice) ? 0 : Convert.ToDecimal(orderPrice)) * (string.IsNullOrEmpty(purchaseQty) ? 0 : Convert.ToDecimal(purchaseQty)); }
        }

        public string TotalPriceDisplay
        {
            get
            {
                if (!TotalPrice.HasValue || TotalPrice <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + TotalPrice.Value.ToString("f2");
                }
            }
        }

        private decimal? tax;

        public decimal? Tax
        {
            get { return tax; }
            set { this.SetValue("Tax", ref tax, value); }
        }

        public string TaxString
        {
            get
            {
                return string.Format("{0}%", tax.HasValue ? (tax.Value * 100).TruncateDecimal(2) : "0.00");
            }
        }

        /// <summary>
        /// 实际总价 OrderPrice* Quantity
        /// </summary>
        public decimal? ActualPrice
        {
            get { return (string.IsNullOrEmpty(orderPrice) ? 0 : Convert.ToDecimal(orderPrice)) * quantity; }
        }

        public string ActualPriceDisplay
        {
            get
            {
                if (!ActualPrice.HasValue || ActualPrice <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + ActualPrice.Value.ToString("f2");
                }
            }
        }

        private int? execptStatus;

        public int? ExecptStatus
        {
            get { return execptStatus; }
            set { this.SetValue("ExecptStatus", ref execptStatus, value); }
        }

        private string productMode;////商品型号

        public string ProductMode
        {
            get { return productMode; }
            set { this.SetValue("ProductMode", ref productMode, value); }
        }


        private string _BMCode;

        public string BMCode
        {
            get { return _BMCode; }
            set { this.SetValue("BMCode", ref _BMCode, value); }
        }

        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { this.SetValue("ProductID", ref productID, value); }
        }



        public string ProductIDDisplayString
        {
            get { return IsVirtualStockProduct == true ? productID + ("[虚库商品]") : productID; }
        }

        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { this.SetValue("ProductName", ref productName, value); }
        }

        private decimal? unitCostWithoutTax;

        public decimal? UnitCostWithoutTax
        {
            get { return unitCostWithoutTax; }
            set { this.SetValue("UnitCostWithoutTax", ref unitCostWithoutTax, value); }
        }

        private decimal? jingDongPrice;

        public decimal? JingDongPrice
        {
            get { return jingDongPrice; }
            set { this.SetValue("JingDongPrice", ref jingDongPrice, value); }
        }

        /// <summary>
        /// 京东毛利率
        /// </summary>
        private decimal? jingDongTax;

        public decimal? JingDongTax
        {
            get { return jingDongTax; }
            set { this.SetValue("JingDongTax", ref jingDongTax, value); }
        }

        private int? availableQty; ////有效库存

        public int? AvailableQty
        {
            get { return availableQty; }
            set { this.SetValue("AvailableQty", ref availableQty, value); }
        }

        private int? m1; ////上月销售总量

        public int? M1
        {
            get { return m1; }
            set { this.SetValue("M1", ref m1, value); }
        }

        /// <summary>
        /// 上周销售总量
        /// </summary>
        private int? week1SalesCount;

        public int? Week1SalesCount
        {
            get { return week1SalesCount; }
            set { this.SetValue("Week1SalesCount", ref week1SalesCount, value); }
        }


        private decimal? currentPrice;////当前价格

        public decimal? CurrentPrice
        {
            get { return currentPrice; }
            set { this.SetValue("CurrentPrice", ref currentPrice, value); }
        }

        public string CurrentPriceDisplay
        {
            get
            {
                if (!currentPrice.HasValue || currentPrice <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + currentPrice.Value.ToString("f2");
                }
            }
        }

        private DateTime? lastAdjustPriceDate; ////上一次调价价格

        public DateTime? LastAdjustPriceDate
        {
            get { return lastAdjustPriceDate; }
            set { this.SetValue("LastAdjustPriceDate", ref lastAdjustPriceDate, value); }
        }

        private DateTime? lastInTime; ////上一次采购时间

        public DateTime? LastInTime
        {
            get { return lastInTime; }
            set { this.SetValue("LastInTime", ref lastInTime, value); }
        }

        private int? acquireReturnPointType;

        public int? AcquireReturnPointType
        {
            get { return acquireReturnPointType; }
            set { this.SetValue("AcquireReturnPointType", ref acquireReturnPointType, value); }
        }

        private decimal? acquireReturnPoint;

        public decimal? AcquireReturnPoint
        {
            get { return acquireReturnPoint; }
            set { this.SetValue("AcquireReturnPoint", ref acquireReturnPoint, value); }
        }

        /// <summary>
        /// 未激活或者已失效的库存
        /// </summary>
        private int? unActivatyCount;

        public int? UnActivatyCount
        {
            get { return unActivatyCount; }
            set { this.SetValue("UnActivatyCount", ref unActivatyCount, value); }
        }

        private decimal? virtualPrice;

        public decimal? VirtualPrice
        {
            get { return virtualPrice; }
            set { this.SetValue("VirtualPrice", ref virtualPrice, value); }
        }

        public string VirtualPriceDisplay
        {
            get
            {
                if (!virtualPrice.HasValue || virtualPrice <= 0m)
                {
                    return currencySymbol + "0.00";
                }
                else
                {
                    return currencySymbol + virtualPrice.Value.ToString("f2");
                }
            }
        }

        private int? readyQuantity;

        public int? ReadyQuantity
        {
            get { return readyQuantity; }
            set { this.SetValue("ReadyQuantity", ref readyQuantity, value); }
        }


        /// <summary>
        /// 是否为同步采购商品
        /// </summary>
        private YNStatus? isVFItem;

        public YNStatus? IsVFItem
        {
            get { return isVFItem; }
            set { this.SetValue("IsVFItem", ref isVFItem, value); }
        }

        private bool isCheckedItem;

        public bool IsCheckedItem
        {
            get { return isCheckedItem; }
            set { this.SetValue("IsCheckedItem", ref isCheckedItem, value); }
        }

        /// <summary>
        /// 是否是虚库商品
        /// </summary>
        private bool? isVirtualStockProduct;

        public bool? IsVirtualStockProduct
        {
            get { return isVirtualStockProduct; }
            set { this.SetValue("IsVirtualStockProduct", ref isVirtualStockProduct, value); }
        }

        #region [各分仓库存信息]
        private int? m_SHInventoryStock;

        public int? SHInventoryStock
        {
            get { return m_SHInventoryStock; }
            set { this.SetValue("SHInventoryStock", ref m_SHInventoryStock, value); }
        }
        private int? m_BJInventoryStock;

        public int? BJInventoryStock
        {
            get { return m_BJInventoryStock; }
            set { this.SetValue("BJInventoryStock", ref m_BJInventoryStock, value); }
        }
        private int? m_GZInventoryStock;

        public int? GZInventoryStock
        {
            get { return m_GZInventoryStock; }
            set { this.SetValue("GZInventoryStock", ref m_GZInventoryStock, value); }
        }
        private int? m_CDInventoryStock;

        public int? CDInventoryStock
        {
            get { return m_CDInventoryStock; }
            set { this.SetValue("CDInventoryStock", ref m_CDInventoryStock, value); }
        }
        private int? m_WHInventoryStock;

        public int? WHInventoryStock
        {
            get { return m_WHInventoryStock; }
            set { this.SetValue("WHInventoryStock", ref m_WHInventoryStock, value); }
        }

        #region 广州仓
        private int? m_GZHaveStockNumber;

        public int? GZHaveStockNumber
        {
            get { return m_GZHaveStockNumber; }
            set { this.SetValue("GZHaveStockNumber", ref m_GZHaveStockNumber, value); }
        }
        private int? m_GZSheftOnRoadNumber;

        public int? GZSheftOnRoadNumber
        {
            get { return m_GZSheftOnRoadNumber; }
            set { this.SetValue("GZSheftOnRoadNumber", ref m_GZSheftOnRoadNumber, value); }
        }
        private int? m_GZWaitInStockNumber;

        public int? GZWaitInStockNumber
        {
            get { return m_GZWaitInStockNumber; }
            set { this.SetValue("GZWaitInStockNumber", ref m_GZWaitInStockNumber, value); }
        }
        private int? m_GZWaitCheckNumber;

        public int? GZWaitCheckNumber
        {
            get { return m_GZWaitCheckNumber; }
            set { this.SetValue("GZWaitCheckNumber", ref m_GZWaitCheckNumber, value); }
        }
        private int? m_GZOnRoadNumber;

        public int? GZOnRoadNumber
        {
            get { return m_GZOnRoadNumber; }
            set { this.SetValue("GZOnRoadNumber", ref m_GZOnRoadNumber, value); }
        }
        private int? m_GZW1;

        public int? GZW1
        {
            get { return m_GZW1; }
            set { this.SetValue("GZW1", ref m_GZW1, value); }
        }
        #endregion

        #region 北京仓
        private int? m_BJHaveStockNumber;

        public int? BJHaveStockNumber
        {
            get { return m_BJHaveStockNumber; }
            set { this.SetValue("BJHaveStockNumber", ref m_BJHaveStockNumber, value); }
        }
        private int? m_BJSheftOnRoadNumber;

        public int? BJSheftOnRoadNumber
        {
            get { return m_BJSheftOnRoadNumber; }
            set { this.SetValue("BJSheftOnRoadNumber", ref m_BJSheftOnRoadNumber, value); }
        }
        private int? m_BJWaitInStockNumber;

        public int? BJWaitInStockNumber
        {
            get { return m_BJWaitInStockNumber; }
            set { this.SetValue("BJWaitInStockNumber", ref m_BJWaitInStockNumber, value); }
        }
        private int? m_BJWaitCheckNumber;

        public int? BJWaitCheckNumber
        {
            get { return m_BJWaitCheckNumber; }
            set { this.SetValue("BJWaitCheckNumber", ref m_BJWaitCheckNumber, value); }
        }
        private int? m_BJOnRoadNumber;

        public int? BJOnRoadNumber
        {
            get { return m_BJOnRoadNumber; }
            set { this.SetValue("BJOnRoadNumber", ref m_BJOnRoadNumber, value); }
        }
        private int? m_BJW1;

        public int? BJW1
        {
            get { return m_BJW1; }
            set { this.SetValue("BJW1", ref m_BJW1, value); }
        }
        #endregion

        #region 上海仓
        private int? m_SHHaveStockNumber;

        public int? SHHaveStockNumber
        {
            get { return m_SHHaveStockNumber; }
            set { this.SetValue("SHHaveStockNumber", ref m_SHHaveStockNumber, value); }
        }
        private int? m_SHSheftOnRoadNumber;

        public int? SHSheftOnRoadNumber
        {
            get { return m_SHSheftOnRoadNumber; }
            set { this.SetValue("SHSheftOnRoadNumber", ref m_SHSheftOnRoadNumber, value); }
        }
        private int? m_SHWaitInStockNumber;

        public int? SHWaitInStockNumber
        {
            get { return m_SHWaitInStockNumber; }
            set { this.SetValue("SHWaitInStockNumber", ref m_SHWaitInStockNumber, value); }
        }
        private int? m_SHWaitCheckNumber;

        public int? SHWaitCheckNumber
        {
            get { return m_SHWaitCheckNumber; }
            set { this.SetValue("SHWaitCheckNumber", ref m_SHWaitCheckNumber, value); }
        }
        private int? m_SHOnRoadNumber;

        public int? SHOnRoadNumber
        {
            get { return m_SHOnRoadNumber; }
            set { this.SetValue("SHOnRoadNumber", ref m_SHOnRoadNumber, value); }
        }
        private int? m_SHW1;

        public int? SHW1
        {
            get { return m_SHW1; }
            set { this.SetValue("SHW1", ref m_SHW1, value); }
        }
        #endregion

        #region 武汉仓
        private int? m_WHHaveStockNumber;

        public int? WHHaveStockNumber
        {
            get { return m_WHHaveStockNumber; }
            set { this.SetValue("WHHaveStockNumber", ref m_WHHaveStockNumber, value); }
        }
        private int? m_WHSheftOnRoadNumber;

        public int? WHSheftOnRoadNumber
        {
            get { return m_WHSheftOnRoadNumber; }
            set { this.SetValue("WHSheftOnRoadNumber", ref m_WHSheftOnRoadNumber, value); }
        }
        private int? m_WHWaitInStockNumber;

        public int? WHWaitInStockNumber
        {
            get { return m_WHWaitInStockNumber; }
            set { this.SetValue("WHWaitInStockNumber", ref m_WHWaitInStockNumber, value); }
        }
        private int? m_WHWaitCheckNumber;

        public int? WHWaitCheckNumber
        {
            get { return m_WHWaitCheckNumber; }
            set { this.SetValue("WHWaitCheckNumber", ref m_WHWaitCheckNumber, value); }
        }
        private int? m_WHOnRoadNumber;

        public int? WHOnRoadNumber
        {
            get { return m_WHOnRoadNumber; }
            set { this.SetValue("WHOnRoadNumber", ref m_WHOnRoadNumber, value); }
        }
        private int? m_WHW1;

        public int? WHW1
        {
            get { return m_WHW1; }
            set { this.SetValue("WHW1", ref m_WHW1, value); }
        }
        #endregion

        #region 成都仓
        private int? m_CDHaveStockNumber;

        public int? CDHaveStockNumber
        {
            get { return m_CDHaveStockNumber; }
            set { this.SetValue("CDHaveStockNumber", ref m_CDHaveStockNumber, value); }
        }
        private int? m_CDSheftOnRoadNumber;

        public int? CDSheftOnRoadNumber
        {
            get { return m_CDSheftOnRoadNumber; }
            set { this.SetValue("CDSheftOnRoadNumber", ref m_CDSheftOnRoadNumber, value); }
        }
        private int? m_CDWaitInStockNumber;

        public int? CDWaitInStockNumber
        {
            get { return m_CDWaitInStockNumber; }
            set { this.SetValue("CDWaitInStockNumber", ref m_CDWaitInStockNumber, value); }
        }
        private int? m_CDWaitCheckNumber;

        public int? CDWaitCheckNumber
        {
            get { return m_CDWaitCheckNumber; }
            set { this.SetValue("CDWaitCheckNumber", ref m_CDWaitCheckNumber, value); }
        }
        private int? m_CDOnRoadNumber;

        public int? CDOnRoadNumber
        {
            get { return m_CDOnRoadNumber; }
            set { this.SetValue("CDOnRoadNumber", ref m_CDOnRoadNumber, value); }
        }
        private int? m_CDW1;

        public int? CDW1
        {
            get { return m_CDW1; }
            set { this.SetValue("CDW1", ref m_CDW1, value); }
        }
        #endregion


        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { this.SetValue("CompanyCode", ref companyCode, value); }
        }

        #endregion
    }
}
