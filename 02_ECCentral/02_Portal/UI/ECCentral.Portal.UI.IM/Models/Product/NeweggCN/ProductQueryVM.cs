using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Linq;

namespace ECCentral.Portal.UI.IM.Models
{
    [DataContract]
    public class ProductQueryExVM : ModelBaseEx
    {
        private string _queryFilter = "";
        private string[] _queryFilterList;
        private const int ProductBasicIndex = 0;
        public string QueryFilter
        {
            get { return _queryFilter; }
            set
            {
                SetValue("QueryFilter", ref _queryFilter, value);
            }
        }
        public ProductQueryExVM()
        {
            _queryFilterList = new[] { "", "", "", "", "", "", "" };
            SyncThirdPartyInventoryTypeList = EnumConverter.GetKeyValuePairs<InventorySync>(EnumConverter.EnumAppendItemType.All);
            ProductStatusList = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
            ProductStatusCompareOpList = EnumConverter.GetKeyValuePairs<Comparison>(EnumConverter.EnumAppendItemType.None);
            VirtualRequestStatusList = EnumConverter.GetKeyValuePairs<VirtualRequest>(EnumConverter.EnumAppendItemType.All);
            VirtualTypeList = EnumConverter.GetKeyValuePairs<VirtualType>(EnumConverter.EnumAppendItemType.All);
            ProductManufactureQuery = new ProductManufactureQueryFilterVM();
            ProductPriceQuery = new ProductPriceQueryFilterVM();
            ProductInventoryQuery = new ProductInventoryQueryFilterVM();
            ProductStatusQuery = new ProductStatusQueryFilterVM();
            ProductStatus = new ComparisonEx<int?, Comparison>();
            OtherQuery = new OtherQueryFilterVM();
            StockQuery = new StockQueryFilterVM();
            ProductManufactureQuery.OnQueryFilterChange += SetQueryFilter;
            ProductPriceQuery.OnQueryFilterChange += SetQueryFilter;
            ProductInventoryQuery.OnQueryFilterChange += SetQueryFilter;
            ProductStatusQuery.OnQueryFilterChange += SetQueryFilter;
            OtherQuery.OnQueryFilterChange += SetQueryFilter;
            StockQuery.OnQueryFilterChange += SetQueryFilter;
            ProductStatus.OnChange += () =>
                                          {
                                              ProductStatusValue = ProductStatusValue;
                                          };
            OperatorTypeList = EnumConverter.GetKeyValuePairs<OperatorType>(EnumConverter.EnumAppendItemType.None);
            IsQueryDefaultList = EnumConverter.GetKeyValuePairs<IsQueryDefault>(EnumConverter.EnumAppendItemType.All);
            ConsignFlagList = EnumConverter.GetKeyValuePairs<VendorConsignFlag>(EnumConverter.EnumAppendItemType.All);
            IsTariffApplyList = EnumConverter.GetKeyValuePairs<IsTariffApply>(EnumConverter.EnumAppendItemType.All);
            this.EntryStatusList = EnumConverter.GetKeyValuePairs<ProductEntryStatus>(EnumConverter.EnumAppendItemType.All);
            this.EntryStatusExList = EnumConverter.GetKeyValuePairs<ProductEntryStatusEx>(EnumConverter.EnumAppendItemType.All);
            ProductPriceQuery.OnEnumChange += GetEnumDesc;
            ProductStatusQuery.OnEnumChange += GetEnumDesc;
            ProductInventoryQuery.OnEnumChange += GetEnumDesc;
            ProductManufactureQuery.OnEnumChange += GetEnumDesc;
            OtherQuery.OnEnumChange += GetEnumDesc;
            OnEnumChange += GetEnumDesc;
            GetProductTypeByAuthKey();
        }

        private void SetQueryFilter(string obj, int index)
        {
            if (obj == null) obj = "";
            _queryFilterList[index] = obj.Trim();
            var desc = _queryFilterList.Join(" ");
            QueryFilter = Regex.Replace(desc, "\\s+", " ");
        }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get;
            set;
        }

        private string _productName;
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string ProductName
        {
            get { return _productName; }
            set
            {
                SetValue("ProductName", ref _productName, value,
                    typeof(ResProductQuery), "Label_BasicInfo_ProductName",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        private string _productID;
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ProductID
        {
            get { return _productID; }
            set
            {
                SetValue("ProductID", ref _productID, value,
                    typeof(ResProductQuery), "Label_BasicInfo_ProductID",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }
        private string _ERPProductID;
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string ERPProductID
        {
            get { return _ERPProductID; }
            set
            {
                SetValue("ERPProductID", ref _ERPProductID, value,
                    typeof(ResProductQuery), "Label_BasicInfo_ERPProductID",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }


        private string _ingramID;
        /// <summary>
        /// 合作商ID
        /// </summary>
        [DataMember]
        public string IngramID
        {
            get { return _ingramID; }
            set
            {
                SetValue("IngramID", ref _ingramID, value,
                   typeof(ResProductQuery), "Label_BasicInfo_ThirdPartyProductID",
                             ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        private int? _pmUserSysNo;

        /// <summary>
        /// PMSsyNO
        /// </summary>
        [DataMember]
        public int? PMUserSysNo
        {
            get { return _pmUserSysNo; }
            set
            {
                SetValue("PMUserSysNo", ref _pmUserSysNo, value);
            }
        }

        private string _pmName;

        /// <summary>
        /// PMSsyNO
        /// </summary>
        [DataMember]
        public string PMName
        {
            get { return _pmName; }
            set
            {
                SetValue("PMName", ref _pmName, value,
                   typeof(ResProductQuery), "Label_BasicInfo_ProductManager",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }
        /// <summary>
        /// 根据条件是否可以查看备份PM的商品信息
        /// </summary>
        public int PMUserCondition { get; set; }

        /// <summary>
        /// 所有PM值
        /// </summary>
        public string AllPMValue { get; set; }

        private int? _c3SysNo;
        /// <summary>
        /// 三级类编号
        /// </summary>
        [DataMember]
        public int? C3SysNo
        {
            get { return _c3SysNo; }
            set
            {
                SetValue("C3SysNo", ref _c3SysNo, value);
                if (Category3List != null && value != null)
                {
                    var category = Category3List.Where(p => p.SysNo == value.Value).SingleOrDefault();
                    if (category != null)
                    {
                        C3Name = category.CategoryName != null ? category.CategoryName.Content : "";
                    }
                }
                else
                {
                    C3Name = null;
                }
            }
        }

        private int? _c2SysNo;
        /// <summary>
        /// 二级类编号
        /// </summary>
        [DataMember]
        public int? C2SysNo
        {
            get { return _c2SysNo; }
            set
            {
                SetValue("C2SysNo", ref _c2SysNo, value);
                if (Category2List != null && value != null)
                {
                    var category = Category2List.Where(p => p.SysNo == value.Value).SingleOrDefault();
                    if (category != null)
                    {
                        C2Name = category.CategoryName != null ? category.CategoryName.Content : "";
                    }
                }
                else
                {
                    C2Name = null;
                }
            }
        }

        private int? _c1SysNo;
        /// <summary>
        /// 一级类编号
        /// </summary>
        [DataMember]
        public int? C1SysNo
        {
            get { return _c1SysNo; }
            set
            {
                SetValue("C1SysNo", ref _c1SysNo, value);
                if (Category1List != null && value != null)
                {
                    var category = Category1List.Where(p => p.SysNo == value.Value).SingleOrDefault();
                    if (category != null)
                    {
                        C1Name = category.CategoryName != null ? category.CategoryName.Content : "";
                    }
                }
                else
                {
                    C1Name = null;
                }
            }
        }

        private string _c3Name;
        /// <summary>
        /// 三级类编号
        /// </summary>
        [DataMember]
        public string C3Name
        {
            get { return _c3Name; }
            set
            {
                SetValue("C3Name", ref _c3Name, value,
                   typeof(ResProductQuery), "Label_BasicInfo_Category3Name",
                            ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        private string _c2Name;
        /// <summary>
        /// 二级类编号
        /// </summary>
        [DataMember]
        public string C2Name
        {
            get { return _c2Name; }
            set
            {
                C3Name = "";
                SetValue("C2Name", ref _c2Name, value,
                   typeof(ResProductQuery), "Label_BasicInfo_Category2Name",
                             ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");

            }
        }

        private string _c1Name;
        /// <summary>
        /// 一级类编号
        /// </summary>
        [DataMember]
        public string C1Name
        {
            get { return _c1Name; }
            set
            {
                C2Name = "";
                C3Name = "";
                SetValue("C1Name", ref _c1Name, value,
                   typeof(ResProductQuery), "Label_BasicInfo_Category1Name",
                               ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");

            }
        }

        private DateTime? _beginTime;
        /// <summary>
        /// 创建开始日期
        /// </summary>
        [DataMember]
        public DateTime? BeginTime
        {
            get { return _beginTime; }
            set
            {
                SetValue("BeginTime", ref _beginTime, value,
                   typeof(ResProductQuery), "DatePicker_ProductCreateDateRange_Start",
                             ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        private DateTime? _endTime;
        /// <summary>
        /// 创建结束日期
        /// </summary>
        [DataMember]
        public DateTime? EndTime
        {
            get { return _endTime; }
            set
            {
                SetValue("EndTime", ref _endTime, value,
                   typeof(ResProductQuery), "DatePicker_ProductCreateDateRange_End",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        private InventorySync? _ingramTypeCondition;
        /// <summary>
        /// 是否有合作方
        /// </summary>
        [DataMember]
        public InventorySync? IngramTypeCondition
        {
            get { return _ingramTypeCondition; }
            set
            {
                SetValue("IngramTypeCondition", ref _ingramTypeCondition, value,
                   typeof(ResProductQuery), "Label_BasicInfo_SyncThirdPartyInventoryType",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        private ProductStatus? _productStatusValue;
        [DataMember]
        public ProductStatus? ProductStatusValue
        {
            get { return _productStatusValue; }
            set
            {
                SetValue("ProductStatusValue", ref _productStatusValue, value);
                var tempName = ProductStatusList.Where(p => p.Key == value).SingleOrDefault().Value;
                if (value == null)
                {
                    ProductStatus.ComparisonValue = null;
                }
                else
                {
                    ProductStatus.ComparisonValue = Convert.ToInt32(value);
                }
                SetValueEx(ProductStatus,
                 typeof(ResProductQuery), "Label_BasicInfo_ProductStatus",
                             ref _queryFilterList[ProductBasicIndex], tempName);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        /// <summary>
        /// 商品状态
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, Comparison> ProductStatus { get; set; }

        /// <summary>
        /// 生产商以及供应商
        /// </summary>
        [DataMember]
        public ProductManufactureQueryFilterVM ProductManufactureQuery { get; set; }

        /// <summary>
        /// 商品价格查询
        /// </summary>
        [DataMember]
        public ProductPriceQueryFilterVM ProductPriceQuery { get; set; }

        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMember]
        public ProductInventoryQueryFilterVM ProductInventoryQuery { get; set; }


        /// <summary>
        /// 商品状态
        /// </summary>
        [DataMember]
        public ProductStatusQueryFilterVM ProductStatusQuery { get; set; }

        /// <summary>
        /// 其他条件
        /// </summary>
        [DataMember]
        public OtherQueryFilterVM OtherQuery { get; set; }

        /// <summary>
        /// 分仓
        /// </summary>
        [DataMember]
        public StockQueryFilterVM StockQuery { get; set; }

        private ProductType? _productTypeValue;
        /// <summary>
        /// 商品类型
        /// </summary>
        [DataMember]
        public ProductType? ProductTypeValue
        {
            get { return _productTypeValue; }
            set
            {
                SetValue("ProductTypeValue", ref _productTypeValue, value,
                   typeof(ResProductQuery), "Label_BasicInfo_ProductType",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
                ProductType = GetPorductTypeEnumInt(value);
            }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        [DataMember]
        public int? ProductType { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        [IgnoreDataMember]
        public List<KeyValuePair<ProductType?, string>> ProductTypeList { get; set; }

        /// <summary>
        /// 库存同步合作
        /// </summary>
        [IgnoreDataMember]
        public List<KeyValuePair<InventorySync?, string>> SyncThirdPartyInventoryTypeList { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        [IgnoreDataMember]
        public List<KeyValuePair<ProductStatus?, string>> ProductStatusList { get; set; }

        /// <summary>
        /// 商品状态比较
        /// </summary>
        [IgnoreDataMember]
        public List<KeyValuePair<Comparison?, string>> ProductStatusCompareOpList { get; set; }

        [IgnoreDataMember]
        public List<KeyValuePair<OperatorType?, string>> OperatorTypeList { get; set; }

        [IgnoreDataMember]
        public List<KeyValuePair<IsQueryDefault?, string>> IsQueryDefaultList { get; set; }

        [IgnoreDataMember]
        public List<KeyValuePair<VendorConsignFlag?, string>> ConsignFlagList { get; set; }

        [IgnoreDataMember]
        public List<KeyValuePair<VirtualRequest?, string>> VirtualRequestStatusList { get; set; }

        [IgnoreDataMember]
        public List<KeyValuePair<VirtualType?, string>> VirtualTypeList { get; set; }

        [IgnoreDataMember]
        public static List<CategoryInfo> Category1List;

        [IgnoreDataMember]
        public static List<CategoryInfo> Category2List;

        [IgnoreDataMember]
        public static List<CategoryInfo> Category3List;


        private string GetEnumDesc(object value)
        {
            var desc = "";
            var currentType = UtilityHelper.GetGenericType(value, value.GetType());
            if (currentType.IsEnum)
            {
                var name = currentType.Name.ToLower();
                if (name.IndexOf('.') != -1)
                    name = name.Substring(name.LastIndexOf('.') + 1);
                switch (name)
                {
                    case "producttype":
                        if (ProductTypeList != null)
                        {
                            var tempValue = (ProductType?)value;
                            desc = ProductTypeList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                    case "inventorysync":
                        if (SyncThirdPartyInventoryTypeList != null)
                        {
                            var tempValue = (InventorySync?)value;
                            desc = SyncThirdPartyInventoryTypeList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                    case "isquerydefault":
                        if (IsQueryDefaultList != null)
                        {
                            var tempValue = (IsQueryDefault?)value;
                            desc = IsQueryDefaultList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                    case "vendorconsignflag":
                        if (ConsignFlagList != null)
                        {
                            var tempValue = (VendorConsignFlag)value;
                            desc = ConsignFlagList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                    case "virtualrequest":
                        if (VirtualRequestStatusList != null)
                        {
                            var tempValue = (VirtualRequest)value;
                            desc = VirtualRequestStatusList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                    case "virtualtype":
                        if (VirtualTypeList != null)
                        {
                            var tempValue = (VirtualType)value;
                            desc = VirtualTypeList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                    case "istariffapply":
                        if (IsTariffApplyList != null)
                        {
                            var tempValue = (IsTariffApply)value;
                            desc = IsTariffApplyList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                    case "productentrystatus":
                        if (EntryStatus != null)
                        {
                            var tempValue = (ProductEntryStatus)value;
                            desc = EntryStatusList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                    case "productentrystatusex":
                        if (IsTariffApplyList != null)
                        {
                            var tempValue = (ProductEntryStatusEx)value;
                            desc = EntryStatusExList.Where(p => p.Key == tempValue).SingleOrDefault().Value;
                        }
                        break;
                }
            }
            return desc;
        }

        private int? GetPorductTypeEnumInt(ProductType? type)
        {
            if (type.HasValue)
            {
                switch (type.Value)
                {
                    case BizEntity.IM.ProductType.Normal:
                        return 0;

                    case BizEntity.IM.ProductType.OpenBox:
                        return 1;

                    case BizEntity.IM.ProductType.Bad:
                        return 2;
                    case BizEntity.IM.ProductType.Virtual:
                        return 3;

                    default:
                        return null;

                }
            }
            return null;
        }

        private void GetProductTypeByAuthKey()
        {
            var list = EnumConverter.GetKeyValuePairs<ProductType>(EnumConverter.EnumAppendItemType.All);
            var searchAuth = AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductQuery_ItemMaintainAllType);
            //var searchAuth = true;
            ProductTypeList = list;
            if (!searchAuth)
            {
                if (list != null && list.Count > 0)
                {
                    IList<ProductType?> keys = new List<ProductType?>();
                    list.ForEach(v => keys.Add(v.Key));
                    foreach (var key in keys)
                    {
                        if (key != BizEntity.IM.ProductType.OpenBox)
                        {
                            var result = list.Any(p => p.Key == key);
                            if (result)
                            {
                                var item = list.Where(p => p.Key == key).First();
                                list.Remove(item);
                            }
                        }
                    }
                    ProductTypeList = list;
                    ProductTypeValue = BizEntity.IM.ProductType.OpenBox;
                }
            }
            if (!ProductTypeValue.HasValue)
            {
                ProductTypeValue = BizEntity.IM.ProductType.Normal;
            }
        }

        public static bool HasDisplaycolumnPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductQuery_ItemDisplaycolumn); }
            //get { return true; }

        }

        public static bool HasSeniorPMPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_SeniorPM_Query); }
        }

        public static bool HasJuniorPMPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_JuniorPM_Query); }
        }

        public Type SearchConditionType
        {
            get { return typeof(ProductQueryExVM); }
        }

        [IgnoreDataMember]
        public List<KeyValuePair<IsTariffApply?, string>> IsTariffApplyList { get; set; }

        private IsTariffApply? _tariffApplyStatus;

        /// <summary>
        /// 报关状态
        /// </summary>
        [DataMember]
        public IsTariffApply? TariffApplyStatus
        {
            get { return _tariffApplyStatus; }
            set
            {
                var tempName = IsTariffApplyList.Where(p => p.Key == value).SingleOrDefault().Value;

                SetValue("TariffApplyStatus", ref _tariffApplyStatus, value,
                   typeof(ResProductQuery), "Label_TariffApplyStatus",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        private ProductEntryStatus? _EntryStatus;

        /// <summary>
        /// 报关状态
        /// </summary>
        [DataMember]
        public ProductEntryStatus? EntryStatus
        {
            get { return _EntryStatus; }
            set
            {

                SetValue("EntryStatus", ref _EntryStatus, value,
                   typeof(ResProductQuery), "Label_TariffApplyStatus",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        [IgnoreDataMember]
        public List<KeyValuePair<ProductEntryStatus?, string>> EntryStatusList { get; set; }

        private ProductEntryStatusEx? _EntryStatusEx;

        /// <summary>
        /// 报关扩展状态
        /// </summary>
        [DataMember]
        public ProductEntryStatusEx? EntryStatusEx
        {
            get { return _EntryStatusEx; }
            set
            {
                SetValue("EntryStatusEx", ref _EntryStatusEx, value,
                   typeof(ResProductQuery), "Label_TariffApplyStatus",
                              ref _queryFilterList[ProductBasicIndex]);
                QueryFilter = _queryFilterList.Join(" ");
            }
        }

        [IgnoreDataMember]
        public List<KeyValuePair<ProductEntryStatusEx?, string>> EntryStatusExList { get; set; }
    }

    /// <summary>
    /// 生产商以及供应商查询
    /// </summary>
    [DataContract]
    public class ProductManufactureQueryFilterVM : ModelBaseEx
    {
        private const int ProductManufactureIndex = 1;
        public delegate void QueryFilterChange(string args, int index);
        public QueryFilterChange OnQueryFilterChange { get; set; }
        private string _queryFilter;

        private string _manufacturerName;
        /// <summary>
        /// 生产商
        /// </summary>
        [DataMember]
        public string ManufacturerName
        {
            get { return _manufacturerName; }
            set
            {
                SetValue("ManufacturerName", ref _manufacturerName, value,
                   typeof(ResProductQuery), "Label_BasicInfo_Manufacturer",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductManufactureIndex);
                }
            }
        }

        private string _brandName;
        /// <summary>
        /// 品牌
        /// </summary>
        [DataMember]
        public string BrandName
        {
            get { return _brandName; }
            set
            {
                SetValue("BrandName", ref _brandName, value,
                   typeof(ResProductQuery), "Label_BasicInfo_Brand",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductManufactureIndex);
                }
            }
        }

        private string _vendorName;
        /// <summary>
        /// 供应商
        /// </summary>
        [DataMember]
        public string VendorName
        {
            get { return _vendorName; }
            set
            {
                SetValue("VendorName", ref _vendorName, value,
                   typeof(ResProductQuery), "Label_BasicInfo_Vendor",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductManufactureIndex);
                }
            }
        }

        private string _merchantSysNo;
        /// <summary>
        /// 商家SysNO
        /// </summary>
        [Validate(ValidateType.Regex, new object[] { "^\\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "ProductManufactureQueryFilterVM_Error_MerchantSysNoInfo")]
        [DataMember]
        public string MerchantSysNo
        {
            get { return _merchantSysNo; }
            set
            {
                SetValue("MerchantSysNo", ref _merchantSysNo, value,
               typeof(ResProductQuery), "Label_BasicInfo_Seller",
                         ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductManufactureIndex);
                }
            }
        }

        private string _merchantName;
        /// <summary>
        /// 商家SysNO
        /// </summary>
        [DataMember]
        public string MerchantName
        {
            get { return _merchantName; }
            set
            {
                SetValue("MerchantName", ref _merchantName, value,
                   typeof(ResProductQuery), "Label_BasicInfo_Seller",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductManufactureIndex);
                }
            }
        }

        private VendorConsignFlag? _isConsign;
        /// <summary>
        /// 代收
        /// </summary>
        [DataMember]
        public VendorConsignFlag? IsConsign
        {
            get { return _isConsign; }
            set
            {
                SetValue("IsConsign", ref _isConsign, value,
                   typeof(ResProductQuery), "Label_BasicInfo_ConsignFlag",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductManufactureIndex);
                }
            }
        }

        [IgnoreDataMember]
        public int? IsInstalmentProduct { get; set; }
    }

    /// <summary>
    /// 商品价格查询
    /// </summary>
    [DataContract]
    public class ProductPriceQueryFilterVM : ModelBaseEx
    {
        private const int ProductPriceIndex = 2;
        public delegate void QueryFilterChange(string args, int index);
        [IgnoreDataMember]
        public QueryFilterChange OnQueryFilterChange { get; set; }
        private string _queryFilter;


        public ProductPriceQueryFilterVM()
        {
            UnitCost = new ComparisonEx<decimal?, OperatorType>();
            UnitCost.OnChange += () =>
            {
                UnitCostValue = UnitCostValue;
            };
            CurrentPrice = new ComparisonEx<decimal?, OperatorType>();
            CurrentPrice.OnChange += () =>
            {
                CurrentPriceValue = CurrentPriceValue;
            };
            Point = new ComparisonEx<int?, OperatorType>();
            Point.OnChange += () =>
            {
                PointValue = PointValue;
            };
        }

        private string _unitCostValue;

        [Validate(ValidateType.Regex, new object[] { @"(-)?\d+(\.\d\d)?" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string UnitCostValue
        {
            get { return _unitCostValue; }
            set
            {
                SetValue("UnitCostValue", ref _unitCostValue, value);
                if (String.IsNullOrEmpty(_unitCostValue))
                {
                    UnitCost.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"(-)?\d+(\.\d\d)?");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        UnitCost.ComparisonValue = Convert.ToDecimal(value);
                    }
                }
                SetValueEx(UnitCost,
             typeof(ResProductQuery), "Label_PriceInfo_AvgCost",
                       ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductPriceIndex);
                }
            }
        }

        /// <summary>
        /// 平均成本
        /// </summary>
        [DataMember]
        public ComparisonEx<decimal?, OperatorType> UnitCost { get; set; }

        private string _currentPriceValue;

        [Validate(ValidateType.Regex, new object[] { @"(-)?\d+(\.\d\d)?" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string CurrentPriceValue
        {
            get { return _currentPriceValue; }
            set
            {
                SetValue("CurrentPriceValue", ref _currentPriceValue, value);
                if (String.IsNullOrEmpty(_currentPriceValue))
                {
                    CurrentPrice.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"(-)?\d+(\.\d\d)?");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        CurrentPrice.ComparisonValue = Convert.ToDecimal(value);
                    }
                }
                SetValueEx(CurrentPrice,
            typeof(ResProductQuery), "Label_PriceInfo_UnitPrice",
                      ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductPriceIndex);
                }
            }
        }

        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public ComparisonEx<decimal?, OperatorType> CurrentPrice { get; set; }

        private string _pointValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string PointValue
        {
            get { return _pointValue; }
            set
            {
                SetValue("PointValue", ref _pointValue, value);
                if (String.IsNullOrEmpty(_pointValue))
                {
                    Point.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        Point.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(Point,
          typeof(ResProductQuery), "Label_PriceInfo_Point",
                    ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductPriceIndex);
                }
            }
        }

        /// <summary>
        /// 积分
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> Point { get; set; }

        private IsQueryDefault? _supplyPriceCondition;
        /// <summary>
        /// 是否供应商调价商品
        /// </summary>
        [DataMember]
        public IsQueryDefault? SupplyPriceCondition
        {
            get { return _supplyPriceCondition; }
            set
            {
                SetValue("SupplyPriceCondition", ref _supplyPriceCondition, value,
                  typeof(ResProductQuery), "Label_PriceInfo_SupplyPriceCondition",
                    ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductPriceIndex);
                }

            }
        }

        /// <summary>
        /// 审核类型
        /// </summary>
        [DataMember]
        public int? PriceApplyStatus { get; set; }



    }

    /// <summary>
    /// 商品库存
    /// </summary>
    [DataContract]
    public class ProductInventoryQueryFilterVM : ModelBaseEx
    {
        private const int ProductInventoryIndex = 3;
        private string _queryFilter;
        public delegate void QueryFilterChange(string args, int index);
        public QueryFilterChange OnQueryFilterChange { get; set; }

        public ProductInventoryQueryFilterVM()
        {
            AccountQty = new ComparisonEx<int?, OperatorType>();
            AccountQty.OnChange += () =>
            {
                AccountQtyValue = AccountQtyValue;
            };
            AvailableQty = new ComparisonEx<int?, OperatorType>();
            AvailableQty.OnChange += () =>
            {
                AvailableQtyValue = AvailableQtyValue;
            };
            OrderQty = new ComparisonEx<int?, OperatorType>();
            OrderQty.OnChange += () =>
            {
                OrderQtyValue = OrderQtyValue;
            };
            DaysToSell = new ComparisonEx<int?, OperatorType>();
            DaysToSell.OnChange += () =>
            {
                DaysToSellValue = DaysToSellValue;
            };
            ConsignQty = new ComparisonEx<int?, OperatorType>();
            ConsignQty.OnChange += () =>
            {
                ConsignQtyValue = ConsignQtyValue;
            };
            AllocatedQty = new ComparisonEx<int?, OperatorType>();
            AllocatedQty.OnChange += () =>
            {
                AllocatedQtyValue = AllocatedQtyValue;
            };
            OnlineQty = new ComparisonEx<int?, OperatorType>();
            OnlineQty.OnChange += () =>
            {
                OnlineQtyValue = OnlineQtyValue;
            };

            SafeQty = new ComparisonEx<int?, OperatorType>();
            SafeQty.OnChange += () =>
            {
                SafeQtyValue = SafeQtyValue;
            };
            VirtualQty = new ComparisonEx<int?, OperatorType>();
            VirtualQty.OnChange += () =>
            {
                VirtualQtyValue = VirtualQtyValue;
            };
            ShiftQty = new ComparisonEx<int?, OperatorType>();
            ShiftQty.OnChange += () =>
            {
                ShiftQtyValue = ShiftQtyValue;
            };
            PurchaseQty = new ComparisonEx<int?, OperatorType>();
            PurchaseQty.OnChange += () =>
            {
                PurchaseQtyValue = PurchaseQtyValue;
            };
        }

        private string _accountQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string AccountQtyValue
        {
            get { return _accountQtyValue; }
            set
            {
                SetValue("AccountQtyValue", ref _accountQtyValue, value);
                if (String.IsNullOrEmpty(_accountQtyValue))
                {
                    AccountQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        AccountQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(AccountQty,
                typeof(ResProductQuery), "Label_Inventory_AccountQty",
                          ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }
        /// <summary>
        /// 财务库存
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> AccountQty
        {
            get;
            set;
        }

        private string _availableQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string AvailableQtyValue
        {
            get { return _availableQtyValue; }
            set
            {
                SetValue("AvailableQtyValue", ref _availableQtyValue, value);
                if (String.IsNullOrEmpty(_availableQtyValue))
                {
                    AvailableQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        AvailableQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(AvailableQty,
                typeof(ResProductQuery), "Label_Inventory_AvailableQty",
                          ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }

        /// <summary>
        /// 可用库存
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> AvailableQty
        {
            get;
            set;
        }

        private string _orderQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string OrderQtyValue
        {
            get { return _orderQtyValue; }
            set
            {
                SetValue("OrderQtyValue", ref _orderQtyValue, value);
                if (String.IsNullOrEmpty(_orderQtyValue))
                {
                    OrderQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        OrderQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(OrderQty,
                typeof(ResProductQuery), "Label_Inventory_OrderQty",
                          ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }

        /// <summary>
        /// 被订购数
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> OrderQty
        {
            get;
            set;
        }

        private string _daysToSellValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string DaysToSellValue
        {
            get { return _daysToSellValue; }
            set
            {
                SetValue("DaysToSellValue", ref _daysToSellValue, value);
                if (String.IsNullOrEmpty(_daysToSellValue))
                {
                    DaysToSell.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        DaysToSell.ComparisonValue = Convert.ToInt32(value);
                    }
                }

                SetValueEx(DaysToSell,
              typeof(ResProductQuery), "Label_Inventory_DaysToSell",
                        ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }

        /// <summary>
        /// 可买天数
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> DaysToSell
        {
            get;
            set;
        }

        private string _consignQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string ConsignQtyValue
        {
            get { return _consignQtyValue; }
            set
            {
                SetValue("ConsignQtyValue", ref _consignQtyValue, value);
                if (String.IsNullOrEmpty(_consignQtyValue))
                {
                    ConsignQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        ConsignQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(ConsignQty,
            typeof(ResProductQuery), "Label_Inventory_ConsignQty",
                      ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }

        /// <summary>
        /// 代销库存
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> ConsignQty
        {
            get;
            set;
        }

        private string _allocatedQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string AllocatedQtyValue
        {
            get { return _allocatedQtyValue; }
            set
            {
                SetValue("AllocatedQtyValue", ref _allocatedQtyValue, value);
                if (String.IsNullOrEmpty(_allocatedQtyValue))
                {
                    AllocatedQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        AllocatedQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(AllocatedQty,
          typeof(ResProductQuery), "Label_Inventory_AllocatedQty",
                    ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }

        /// <summary>
        ///被占用库存
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> AllocatedQty
        {
            get;
            set;
        }

        private string _onlineQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string OnlineQtyValue
        {
            get { return _onlineQtyValue; }
            set
            {
                SetValue("OnlineQtyValue", ref _onlineQtyValue, value);
                if (String.IsNullOrEmpty(_onlineQtyValue))
                {
                    OnlineQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        OnlineQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(OnlineQty,
         typeof(ResProductQuery), "Label_Inventory_OnlineQty",
                   ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }



        private string _safeQtyValue;
        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "SafeQtyValue_ErrorInfo")]
        [DataMember]
        public string SafeQtyValue
        {
            get { return _safeQtyValue; }
            set
            {
                SetValue("SafeQtyValue", ref _safeQtyValue, value);
                if (String.IsNullOrEmpty(_safeQtyValue))
                {
                    SafeQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        SafeQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(SafeQty,
         typeof(ResProductQuery), "Label_Inventory_SafeQty",
                   ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }

        /// <summary>
        ///Online库存DaysToSell
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> OnlineQty
        {
            get;
            set;
        }

        /// <summary>
        ///Online库存DaysToSell
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> SafeQty
        {
            get;
            set;
        }

        private IsQueryDefault? _isBatch;
        /// <summary>
        ///是否批号管理
        /// </summary>
        [DataMember]
        public IsQueryDefault? IsBatch
        {
            get { return _isBatch; }
            set
            {
                SetValue("IsBatch", ref _isBatch, value, typeof(ResProductQuery), "Label_Inventory_BatchProductFlag",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }

            }
        }

        private string _virtualQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string VirtualQtyValue
        {
            get { return _virtualQtyValue; }
            set
            {
                SetValue("VirtualQtyValue", ref _virtualQtyValue, value);
                if (String.IsNullOrEmpty(_virtualQtyValue))
                {
                    VirtualQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        VirtualQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(VirtualQty,
                typeof(ResProductQuery), "Label_Inventory_VirtualQty",
                 ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }


        /// <summary>
        ///虚库库存
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> VirtualQty
        {
            get;
            set;
        }

        private string _shiftQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string ShiftQtyValue
        {
            get { return _shiftQtyValue; }
            set
            {
                SetValue("ShiftQtyValue", ref _shiftQtyValue, value);
                if (String.IsNullOrEmpty(_shiftQtyValue))
                {
                    ShiftQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        ShiftQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(ShiftQty,
              typeof(ResProductQuery), "Label_Inventory_ShiftQty",
               ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }

        /// <summary>
        ///移仓在途数量
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> ShiftQty
        {
            get;
            set;
        }

        private string _purchaseQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string PurchaseQtyValue
        {
            get { return _purchaseQtyValue; }
            set
            {
                SetValue("PurchaseQtyValue", ref _purchaseQtyValue, value);
                if (String.IsNullOrEmpty(_purchaseQtyValue))
                {
                    PurchaseQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        PurchaseQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(PurchaseQty,
            typeof(ResProductQuery), "Label_Inventory_PurchaseQty",
             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductInventoryIndex);
                }
            }
        }

        /// <summary>
        ///采购在途数量
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> PurchaseQty
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 商品状态
    /// </summary>
    [DataContract]
    public class ProductStatusQueryFilterVM : ModelBaseEx
    {
        private const int ProductStatusIndex = 4;
        private string _queryFilter;
        public delegate void QueryFilterChange(string args, int index);
        public QueryFilterChange OnQueryFilterChange { get; set; }


        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        public IsQueryDefault? InfoStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public IsQueryDefault? PriceStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 重量
        /// </summary>
        [DataMember]
        public IsQueryDefault? WeightStatus
        {
            get;
            set;
        }

        private IsQueryDefault? _productInfoFinishStatus;
        /// <summary>
        /// 商品资料是否完善
        /// </summary>
        [DataMember]
        public IsQueryDefault? ProductInfoFinishStatus
        {
            get { return _productInfoFinishStatus; }
            set
            {
                SetValue("ProductInfoFinishStatus", ref _productInfoFinishStatus, value, typeof(ResProductQuery), "Label_ProductStatus_ProductInfoFinishStatus",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, ProductStatusIndex);
                }

            }
        }

        /// <summary>
        /// 图片
        /// </summary>
        [DataMember]
        public IsQueryDefault? PicStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许显示在WEB上
        /// </summary>
        [DataMember]
        public IsQueryDefault? AllowStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 保证条款
        /// </summary>
        [DataMember]
        public IsQueryDefault? WarrantyStatus
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 其他条件
    /// </summary>
    [DataContract]
    public class OtherQueryFilterVM : ModelBaseEx
    {

        private const int OtherQueryIndex = 5;
        private string _queryFilter;
        public delegate void QueryFilterChange(string args, int index);
        [IgnoreDataMember]
        public QueryFilterChange OnQueryFilterChange { get; set; }



        public OtherQueryFilterVM()
        {
            Weight = new ComparisonEx<int?, OperatorType>();
            Weight.OnChange += () =>
                                   {
                                       WeightValue = WeightValue;
                                   };
        }

        private IsQueryDefault? _virtualPicStatus;
        /// <summary>
        /// 虚库图片
        /// </summary>
        [DataMember]
        public IsQueryDefault? VirtualPicStatus
        {
            get { return _virtualPicStatus; }
            set
            {
                SetValue("VirtualPicStatus", ref _virtualPicStatus, value, typeof(ResProductQuery), "Label_Other_VirtualPicStatus",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }

            }
        }

        private IsQueryDefault? _productDescLong;
        /// <summary>
        /// 商品描述
        /// </summary>
        [DataMember]
        public IsQueryDefault? ProductDescLong
        {
            get { return _productDescLong; }
            set
            {
                SetValue("ProductDescLong", ref _productDescLong, value, typeof(ResProductQuery), "Label_Other_ProductDescLong",
                             ref _queryFilter);

                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }
            }
        }

        private IsQueryDefault? _isLarge;
        /// <summary>
        /// 大货
        /// </summary>
        [DataMember]
        public IsQueryDefault? IsLarge
        {
            get { return _isLarge; }
            set
            {
                SetValue("IsLarge", ref _isLarge, value, typeof(ResProductQuery), "Label_Other_IsLarge",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }

            }
        }

        private IsQueryDefault? _performance;
        /// <summary>
        /// 详细参数
        /// </summary>
        [DataMember]
        public IsQueryDefault? Performance
        {
            get { return _performance; }
            set
            {
                SetValue("Performance", ref _performance, value, typeof(ResProductQuery), "Label_Other_Performance",
                             ref _queryFilter);

                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }
            }
        }

        /// <summary>
        /// 详细参数
        /// </summary>
        [DataMember]
        public IsQueryDefault? Is360Show
        {
            get;
            set;
        }

        private IsQueryDefault? _isTakePictures;
        /// <summary>
        /// 是否需要拍照
        /// </summary>
        [DataMember]
        public IsQueryDefault? IsTakePictures
        {
            get { return _isTakePictures; }
            set
            {
                SetValue("IsTakePictures", ref _isTakePictures, value, typeof(ResProductQuery), "Label_Other_IsTakePictures",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }
            }
        }

        private string _weightValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string WeightValue
        {
            get { return _weightValue; }
            set
            {
                SetValue("WeightValue", ref _weightValue, value);
                if (String.IsNullOrEmpty(_weightValue))
                {
                    Weight.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        Weight.ComparisonValue = Convert.ToInt32(value);
                    }
                }

                SetValueEx(Weight,
                typeof(ResProductQuery), "Label_Other_Weight",
                          ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }
            }
        }

        /// <summary>
        /// 重量
        /// </summary>
        [DataMember]
        public ComparisonEx<int?, OperatorType> Weight { get; set; }

        /// <summary>
        /// 视频
        /// </summary>
        [DataMember]
        public IsQueryDefault? IsVideo
        {
            get;
            set;
        }

        private VirtualRequest? _virtualRequest;
        /// <summary>
        /// 虚库审核状态
        /// </summary>
        [DataMember]
        public VirtualRequest? VirtualRequest
        {
            get { return _virtualRequest; }
            set
            {
                SetValue("VirtualRequest", ref _virtualRequest, value, typeof(ResProductQuery), "Label_Other_VirtualRequest",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }
            }
        }

        private VirtualType? _virtualType;
        /// <summary>
        /// 虚库类型
        /// </summary>
        [DataMember]
        public VirtualType? VirtualType
        {
            get { return _virtualType; }
            set
            {
                SetValue("VirtualType", ref _virtualType, value, typeof(ResProductQuery), "Label_Other_VirtualType",
                             ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }
            }
        }

        private string _promotionTitle;
        /// <summary>
        /// 促销信息
        /// </summary>
        [DataMember]
        public string PromotionTitle
        {
            get { return _promotionTitle; }
            set
            {
                SetValue("PromotionTitle", ref _promotionTitle, value, typeof(ResProductQuery), "Label_Other_PromotionTitle",
                           ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, OtherQueryIndex);
                }

            }
        }
    }

    /// <summary>
    /// 分仓
    /// </summary>
    [DataContract]
    public class StockQueryFilterVM : ModelBaseEx
    {
        public StockQueryFilterVM()
        {
            StockAccountQty = new ComparisonEx<int?, OperatorType>();
            StockAccountQty.OnChange += () =>
            {
                StockAccountQtyValue = StockAccountQtyValue;
            };
        }

        private const int StockQueryIndex = 6;
        private string _queryFilter;
        public delegate void QueryFilterChange(string args, int index);
        public QueryFilterChange OnQueryFilterChange { get; set; }

        private int? _warehouseNumber;
        [DataMember]
        public int? WarehouseNumber
        {
            get { return _warehouseNumber; }
            set
            {
                SetValue("WarehouseNumber", ref _warehouseNumber, value);
            }
        }

        /// <summary>
        /// 分仓名称
        /// </summary>
        private string _warehouseName;
        [DataMember]
        public string WarehouseName
        {
            get { return _warehouseName; }
            set
            {
                SetValue("WarehouseName", ref _warehouseName, value, typeof(ResProductQuery), "Label_Stock_WarehouseNumber",
                           ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, StockQueryIndex);
                }
            }
        }

        private string _stockAccountQtyValue;

        [Validate(ValidateType.Regex, new object[] { @"^-?\d+$" }, ErrorMessageResourceType = typeof(ResProductQuery), ErrorMessageResourceName = "AccountQtyValue_ErrorInfo")]
        [DataMember]
        public string StockAccountQtyValue
        {
            get { return _stockAccountQtyValue; }
            set
            {
                SetValue("StockAccountQtyValue", ref _stockAccountQtyValue, value);
                if (String.IsNullOrEmpty(_stockAccountQtyValue))
                {
                    StockAccountQty.ComparisonValue = null;
                }
                else
                {
                    var regex = new Regex(@"^-?\d+$");
                    if (regex.IsMatch(Convert.ToString(value)))
                    {
                        StockAccountQty.ComparisonValue = Convert.ToInt32(value);
                    }
                }
                SetValueEx(StockAccountQty,
         typeof(ResProductQuery), "Label_Stock_StockAccountQty",
          ref _queryFilter);
                if (OnQueryFilterChange != null)
                {
                    OnQueryFilterChange(_queryFilter, StockQueryIndex);
                }
            }
        }

        [DataMember]
        public ComparisonEx<int?, OperatorType> StockAccountQty
        {
            get;
            set;
        }

    }


    /// <summary>
    /// 比较
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TV"></typeparam>
    [DataContract]
    public class ComparisonEx<T, TV> : Comparison<T, TV>
    {
        public delegate void ChangeHadle();

        public ChangeHadle OnChange { get; set; }

        private TV _queryOperator;
        /// <summary>
        /// 比较（如大于）
        /// </summary>
        [DataMember]
        public TV QueryOperator
        {
            get { return _queryOperator; }
            set
            {
                _queryOperator = value;
                if (OnChange != null)
                {
                    OnChange();
                }
            }
        }

        public ComparisonEx<T, TV> DeepCopy(ComparisonEx<T, TV> source)
        {
            var target = new ComparisonEx<T, TV> { QueryOperator = QueryConditionOperator };
            return target;
        }
    }



}
