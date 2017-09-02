using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;
using System.Windows;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignSettlementItemInfoVM : ModelBase
    {

        public ConsignSettlementItemInfoVM()
        {
            consignToAccLogInfo = new ConsignToAcctLogInfoVM();
            vendorInfo = new VendorInfoVM();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        private int? itemSysNo;

        public int? ItemSysNo
        {
            get { return itemSysNo; }
            set { base.SetValue("ItemSysNo", ref itemSysNo, value); }
        }

        private int? pOConsignToAccLogSysNo;
        public int? POConsignToAccLogSysNo
        {
            get { return pOConsignToAccLogSysNo; }
            set { base.SetValue("POConsignToAccLogSysNo", ref pOConsignToAccLogSysNo, value); }
        }

        private int? settleRuleSysNo;

        public int? SettleRuleSysNo
        {
            get { return settleRuleSysNo; }
            set { base.SetValue("SettleRuleSysNo", ref settleRuleSysNo, value); }
        }

        private string settleRuleName;

        public string SettleRuleName
        {
            get { return settleRuleName; }
            set { base.SetValue("SettleRuleName", ref settleRuleName, value); }
        }

        private decimal? settlePrice;

        public decimal? SettlePrice
        {
            get { return settlePrice; }
            set { base.SetValue("SettlePrice", ref settlePrice, value); }
        }

        /// <summary>
        /// 结算单编号
        /// </summary>
        private int? settleSysNo;

        public int? SettleSysNo
        {
            get { return settleSysNo; }
            set { base.SetValue("SettleSysNo", ref settleSysNo, value); }
        }

        /// <summary>
        /// 供应商信息
        /// </summary>
        private VendorInfoVM vendorInfo;

        public VendorInfoVM VendorInfo
        {
            get { return vendorInfo; }
            set { base.SetValue("VendorInfo", ref vendorInfo, value); }
        }

        /// <summary>
        /// 结算类型
        /// </summary>
        private SettleType? settleType;

        public SettleType? SettleType
        {
            get { return settleType; }
            set { base.SetValue("SettleType", ref settleType, value); }
        }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }
        /// <summary>
        /// 商品编号
        /// </summary>
        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }


        /// <summary>
        /// 商品名称
        /// </summary>
        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { base.SetValue("ProductName", ref productName, value); }
        }


        private ValidStatus? status;

        public ValidStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        ///// <summary>
        ///// 创建时成本
        ///// </summary>
        //private decimal? createCost;

        //public decimal? CreateCost
        //{
        //    get { return createCost; }
        //    set { base.SetValue("CreateCost", ref createCost, value); }
        //}

        /// <summary>
        /// 发放积分
        /// </summary>
        private int? point;

        public int? Point
        {
            get { return point; }
            set { base.SetValue("Point", ref point, value); }
        }

        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }

        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { base.SetValue("StockName", ref stockName, value); }
        }

        /// <summary>
        /// 结算价格
        /// </summary>
        private decimal? cost;

        public decimal? Cost
        {
            get { return cost; }
            set { base.SetValue("Cost", ref cost, value); }
        }

        private DateTime? inDate;

        public DateTime? InDate
        {
            get { return inDate; }
            set { base.SetValue("InDate", ref inDate, value); }
        }
        /// <summary>
        /// 数量
        /// </summary>
        private int? quantity;

        public int? Quantity
        {
            get { return quantity; }
            set { base.SetValue("Quantity", ref quantity, value); }
        }

        /// <summary>
        /// 货币编码
        /// </summary>
        private int? currenyCode;

        public int? CurrenyCode
        {
            get { return currenyCode; }
            set { base.SetValue("CurrenyCode", ref currenyCode, value); }
        }
        /// <summary>
        /// 佣金百分比
        /// </summary>
        private string settlePercentage;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,6}$", ErrorMessageResourceName = "Decimal_Required", ErrorMessageResourceType = typeof(ResValidationErrorMessage))]
        public string SettlePercentage
        {
            get { return settlePercentage; }
            set { base.SetValue("SettlePercentage", ref settlePercentage, value); }
        }

        public string SettlePercentageDisplay
        {
            get
            {
                return string.IsNullOrEmpty(settlePercentage) ? string.Empty : settlePercentage + "%";
            }
        }

        private int? acquireReturnPointType;

        public int? AcquireReturnPointType
        {
            get { return acquireReturnPointType; }
            set { base.SetValue("AcquireReturnPointType", ref acquireReturnPointType, value); }
        }

        private decimal? acquireReturnPoint;

        public decimal? AcquireReturnPoint
        {
            get { return acquireReturnPoint; }
            set { base.SetValue("AcquireReturnPoint", ref acquireReturnPoint, value); }
        }

        //结算商品 产出返利金额
        private decimal? expectGetPoint;
        public decimal? ExpectGetPoint
        {
            get { return expectGetPoint; }
            set { base.SetValue("ExpectGetPoint", ref expectGetPoint, value); }
        }

        /// <summary>
        /// 设置合同返利
        /// </summary>
        private string contractReturnPointSet;

        public string ContractReturnPointSet
        {
            get { return contractReturnPointSet; }
            set { base.SetValue("ContractReturnPointSet", ref contractReturnPointSet, value); }
        }


        /// <summary>
        /// 代销转财务记录(结算商品Items)
        /// </summary>
        private ConsignToAcctLogInfoVM consignToAccLogInfo;

        public ConsignToAcctLogInfoVM ConsignToAccLogInfo
        {
            get { return consignToAccLogInfo; }
            set { base.SetValue("ConsignToAccLogInfo", ref consignToAccLogInfo, value); }
        }

        private bool isCheckedItem;

        public bool IsCheckedItem
        {
            get { return isCheckedItem; }
            set { base.SetValue("IsCheckedItem", ref isCheckedItem, value); }
        }

        /// <summary>
        /// 订单数量
        /// </summary>
        private int? orderCount;
        public int? OrderCount
        {
            get { return orderCount; }
            set { base.SetValue("OrderCount", ref orderCount, value); }
        }

        private string allOrderSysNoFormatString;

        public string AllOrderSysNoFormatString
        {
            get;
            set;
        }

        public bool IsSettleCostTextBoxReadOnly
        {
            get;
            set;
        }

        public bool IsSettlePercentageTextBoxReadOnly
        {
            get;
            set;
        }

        public Visibility SettlePercentageTextBoxVisibility
        {
            get;
            set;
        }
    }
}
