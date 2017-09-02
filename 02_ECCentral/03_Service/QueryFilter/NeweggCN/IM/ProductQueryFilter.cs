using System;
using System.Runtime.Serialization;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class NeweggProductQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get;
            set;
        }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// 合作商ID
        /// </summary>
        public string IngramID { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// PMSsyNO
        /// </summary>
        public int? PMUserSysNo { get; set; }

        /// <summary>
        /// 根据条件是否可以查看备份PM的商品信息
        /// </summary>
        public int PMUserCondition { get; set; }

        /// <summary>
        /// 所有PM值
        /// </summary>
        public string AllPMValue { get; set; }


        /// <summary>
        /// 三级类编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 二级类编号
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 一级类编号
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 创建开始日期
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 创建结束日期
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否交易
        /// </summary>
        public int? IsPurchase { get; set; }

        public int? CurrentUserSysNo { get; set; }

        /// <summary>
        /// 是否有合作方
        /// </summary>
        public InventorySync? IngramTypeCondition { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public int? ProductType { get; set; }

        public string ERPProductID { get; set; }
        /// <summary>
        /// 商品状态
        /// </summary>
        public Comparison<int?, Comparison> ProductStatus { get; set; }

        /// <summary>
        /// 生产商以及供应商
        /// </summary>
        public ProductManufactureQueryFilter ProductManufactureQuery { get; set; }

        /// <summary>
        /// 商品价格查询
        /// </summary>
        public ProductPriceQueryFilter ProductPriceQuery { get; set; }

        /// <summary>
        /// 商品库存
        /// </summary>
        public ProductInventoryQueryFilter ProductInventoryQuery { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatusQueryFilter ProductStatusQuery { get; set; }

        /// <summary>
        /// 其他条件
        /// </summary>
        public OtherQueryFilter OtherQuery { get; set; }

        /// <summary>
        /// 分仓
        /// </summary>
        public StockQueryFilter StockQuery { get; set; }

        public IsTariffApply? TariffApplyStatus { get; set; }

        public ProductEntryStatus? EntryStatus { get; set; }

        public ProductEntryStatusEx? EntryStatusEx { get; set; }
    }

    /// <summary>
    /// 生产商以及供应商查询
    /// </summary>
    public class ProductManufactureQueryFilter
    {
        /// <summary>
        /// 生产商
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 商家SysNO
        /// </summary>
        public int? MerchantSysNo { get; set; }

        /// <summary>
        /// 代收
        /// </summary>
        public VendorConsignFlag? IsConsign { get; set; }

        public int? IsInstalmentProduct { get; set; }
    }

    /// <summary>
    /// 商品价格查询
    /// </summary>
    public class ProductPriceQueryFilter
    {
        /// <summary>
        /// 平均成本
        /// </summary>
        public Comparison<decimal?, OperatorType> UnitCost { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public Comparison<decimal?, OperatorType> CurrentPrice { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public Comparison<int?, OperatorType> Point { get; set; }


        /// <summary>
        /// 是否供应商调价商品
        /// </summary>
        public IsQueryDefault? SupplyPriceCondition { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        public int? PriceApplyStatus { get; set; }


    }

    /// <summary>
    /// 商品库存
    /// </summary>
    public class ProductInventoryQueryFilter
    {
        /// <summary>
        /// 财务库存
        /// </summary>
        public Comparison<int?, OperatorType> AccountQty
        {
            get;
            set;
        }

        /// <summary>
        /// 可用库存
        /// </summary>
        public Comparison<int?, OperatorType> AvailableQty
        {
            get;
            set;
        }

        /// <summary>
        /// 被订购数
        /// </summary>
        public Comparison<int?, OperatorType> OrderQty
        {
            get;
            set;
        }

        /// <summary>
        /// 可买天数
        /// </summary>
        public Comparison<int?, OperatorType> DaysToSell
        {
            get;
            set;
        }

        /// <summary>
        /// 代销库存
        /// </summary>
        public Comparison<int?, OperatorType> ConsignQty
        {
            get;
            set;
        }

        /// <summary>
        ///被占用库存
        /// </summary>
        public Comparison<int?, OperatorType> AllocatedQty
        {
            get;
            set;
        }

        /// <summary>
        ///Online库存DaysToSell
        /// </summary>
        public Comparison<int?, OperatorType> OnlineQty
        {
            get;
            set;
        }

        public Comparison<int?, OperatorType> SafeQty
        {
            get;
            set;
        }
        /// <summary>
        ///是否批号管理
        /// </summary>
        public int? IsBatch
        {
            get;
            set;
        }

        /// <summary>
        ///虚库库存
        /// </summary>
        public Comparison<int?, OperatorType> VirtualQty
        {
            get;
            set;
        }

        /// <summary>
        ///移仓在途数量
        /// </summary>
        public Comparison<int?, OperatorType> ShiftQty
        {
            get;
            set;
        }

        /// <summary>
        ///采购在途数量
        /// </summary>
        public Comparison<int?, OperatorType> PurchaseQty
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 商品状态
    /// </summary>
    public class ProductStatusQueryFilter
    {
        /// <summary>
        /// 消息
        /// </summary>
        public IsQueryDefault? InfoStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 价格
        /// </summary>
        public IsQueryDefault? PriceStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 重量
        /// </summary>
        public IsQueryDefault? WeightStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 商品资料是否完善
        /// </summary>
        public IsQueryDefault? ProductInfoFinishStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 图片
        /// </summary>
        public IsQueryDefault? PicStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许显示在WEB上
        /// </summary>
        public IsQueryDefault? AllowStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 保证条款
        /// </summary>
        public IsQueryDefault? WarrantyStatus
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 其他条件
    /// </summary>
    public class OtherQueryFilter
    {
        /// <summary>
        /// 虚库图片
        /// </summary>
        public IsQueryDefault? VirtualPicStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 商品描述
        /// </summary>
        public IsQueryDefault? ProductDescLong
        {
            get;
            set;
        }

        /// <summary>
        /// 大货
        /// </summary>
        public IsQueryDefault? IsLarge
        {
            get;
            set;
        }

        /// <summary>
        /// 详细参数
        /// </summary>
        public IsQueryDefault? Performance
        {
            get;
            set;
        }

        /// <summary>
        /// 详细参数
        /// </summary>
        public IsQueryDefault? Is360Show
        {
            get;
            set;
        }

        /// <summary>
        /// 是否需要拍照
        /// </summary>
        public IsQueryDefault? IsTakePictures
        {
            get;
            set;
        }

        /// <summary>
        /// 重量
        /// </summary>
        public Comparison<int?, OperatorType> Weight { get; set; }

        /// <summary>
        /// 视频
        /// </summary>
        public IsQueryDefault? IsVideo
        {
            get;
            set;
        }

        /// <summary>
        /// 促销信息
        /// </summary>
        public string PromotionTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 虚库类型
        /// </summary>
        public VirtualType? VirtualType { get; set; }

        /// <summary>
        /// 虚库审核状态
        /// </summary>
        public VirtualRequest? VirtualRequest { get; set; }
    }

    /// <summary>
    /// 分仓
    /// </summary>
    public class StockQueryFilter
    {
        public int? WarehouseNumber
        {
            get;
            set;
        }

        public Comparison<int?, OperatorType> StockAccountQty
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
    public class Comparison<T, TV>
    {
        //public ItemListQueryEntity QueryCondtion { get; set; }
        private TV _queryConditionOperator;
        /// <summary>
        /// 比较（如大于）
        /// </summary>
        [DataMember]
        public TV QueryConditionOperator
        {
            get { return _queryConditionOperator; }
            set
            {
                _queryConditionOperator = value;
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        [DataMember]
        public T ComparisonValue { get; set; }
    }
}
