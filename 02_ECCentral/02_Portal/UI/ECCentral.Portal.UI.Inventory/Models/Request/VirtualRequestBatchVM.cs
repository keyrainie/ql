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
using ECCentral.Portal.UI.Inventory.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class VirtualRequestBatchVM
    {

    }
    public class VirtualRequestProductVM : ModelBase
    {
        public bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetValue("IsChecked", ref isChecked, value); }
        }
        public string requestQuantity;
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Regex, @"^\d(\d{0,5})$", ErrorMessageResourceName = "Msg_Quantity_Format", ErrorMessageResourceType = typeof(ResVirtualRequestMaintainBatch))]
        public string RequestQuantity
        {
            get { return requestQuantity; }
            set { SetValue("RequestQuantity", ref requestQuantity, value); }
        }
        public string reason;
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public string Reason
        {
            get { return reason; }
            set { SetValue("Reason", ref reason, value); }
        }
        public int ItemNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? StockSysNo { get; set; }
        public int? AccountQuantity { get; set; }
        public int? AvailableQuantity { get; set; }
        public int? AllocatedQuantity { get; set; }
        public int? OrderQuantity { get; set; }
        public int? VirtualQuantity { get; set; }
        public int? ConsignQuantity { get; set; }
        public int? OnlineQuantity { get; set; }
        public int? PurchaseQuantity { get; set; }
        public int? WarehouseAccountQuantity { get; set; }
        public int? WarehouseSafeQuantity { get; set; }
        public string PMName { get; set; }
        public decimal? UnitCost { get; set; }
        public int? ItemPoint { get; set; }
        public ECCentral.BizEntity.IM.ProductStatus? Status { get; set; }
        public int? Day1SalesCount { get; set; }
        public int? Day2SalesCount { get; set; }
        public int? Day3SalesCount { get; set; }
        public int? Day4SalesCount { get; set; }
        public int? Day5SalesCount { get; set; }
        public int? Day6SalesCount { get; set; }
        public int? Day7SalesCount { get; set; }
        public int? Week1SalesCount { get; set; }
        public int? Week2SalesCount { get; set; }
        public int? Week3SalesCount { get; set; }
        public int? Week4SalesCount { get; set; }
        public int? Month1SalesCount { get; set; }
        public int? Month2SalesCount { get; set; }
        public int? Month3SalesCount { get; set; }
        public decimal? CurrentUnitPrice { get; set; }
        public decimal? LastPrice { get; set; }
        public string LastVendor { get; set; }
        public string CompanyCode { get; set; }
        public string LanguageCode { get; set; }
        public string ISSynProduct { get; set; }
        public string VFType { get; set; }
        public string ISLimitTimeAndQtyProduct { get; set; }
    }

    public class VirtualRequestQueryProductsVM : ModelBase
    {
        public ECCentral.QueryFilter.Common.PagingInfo PagingInfo { get; set; }

        private string productID;
        public string ProductID
        {
            get { return productID; }
            set { SetValue("ProductID", ref productID, value); }
        }

        private string productName;
        public string ProductName
        {
            get { return productName; }
            set { SetValue("ProductName", ref productName, value); }
        }

        private int? pmSysNo;
        public int? PMSysNo
        {
            get { return pmSysNo; }
            set { SetValue("PMSysNo", ref pmSysNo, value); }
        }

        private ECCentral.QueryFilter.Inventory.VirtualRequestQueryProductsFilter.OperationType _operator;
        public ECCentral.QueryFilter.Inventory.VirtualRequestQueryProductsFilter.OperationType Operator
        {
            get { return _operator; }
            set { SetValue("Operator", ref _operator, value); }
        }

        private ECCentral.BizEntity.IM.ProductStatus? status;
        public ECCentral.BizEntity.IM.ProductStatus? Status
        {
            get { return status; }
            set { SetValue("Status", ref status, value); }
        }

        private int? stockSysNo;
        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { SetValue("StockSysNo", ref stockSysNo, value); }
        }

        private ECCentral.BizEntity.IM.ProductType? productType;
        public ECCentral.BizEntity.IM.ProductType? ProductType
        {
            get { return productType; }
            set { SetValue("ProductType", ref productType, value); }
        }

        private int? category1SysNo;
        public int? Category1SysNo
        {
            get { return category1SysNo; }
            set { SetValue("Category1SysNo", ref category1SysNo, value); }
        }

        private int? category2SysNo;
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { SetValue("Category2SysNo", ref category2SysNo, value); }
        }

        private int? category3SysNo;
        public int? Category3SysNo
        {
            get { return category3SysNo; }
            set { SetValue("Category3SysNo", ref category3SysNo, value); }
        }

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { SetValue("CompanyCode", ref companyCode, value); }
        }

        public List<KeyValuePair<ECCentral.BizEntity.IM.ProductStatus?, string>> ProductStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        public List<KeyValuePair<ECCentral.BizEntity.IM.ProductType?, string>> ProductTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductType>(EnumConverter.EnumAppendItemType.All);
            }
        }
        public List<KeyValuePair<ECCentral.QueryFilter.Inventory.VirtualRequestQueryProductsFilter.OperationType?, string>> OperationTypeList
        {
            get
            {
                return new List<KeyValuePair<QueryFilter.Inventory.VirtualRequestQueryProductsFilter.OperationType?, string>> 
                {
                    new  KeyValuePair<QueryFilter.Inventory.VirtualRequestQueryProductsFilter.OperationType?,string>(QueryFilter.Inventory.VirtualRequestQueryProductsFilter.OperationType.Equal,ResVirtualRequestMaintainBatch.OperationType_Equal),
                    new  KeyValuePair<QueryFilter.Inventory.VirtualRequestQueryProductsFilter.OperationType?,string>(QueryFilter.Inventory.VirtualRequestQueryProductsFilter.OperationType.NotEqual,ResVirtualRequestMaintainBatch.OperationType_NotEqual)
                };
            }
        }
    }

    public class VirtualRequestBatchView : ModelBase
    {
        public VirtualRequestQueryProductsVM QueryInfo
        {
            get;
            set;
        }

        private List<VirtualRequestProductVM> result;
        public List<VirtualRequestProductVM> Result
        {
            get { return result; }
            set
            {
                SetValue("Result", ref result, value);
            }
        }

        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                SetValue<int>("TotalCount", ref totalCount, value);
            }
        }

        public VirtualRequestBatchView()
        {
            QueryInfo = new VirtualRequestQueryProductsVM();
        }
    }
}
