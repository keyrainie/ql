using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    /// <summary>
    /// 备货中心 - QueryFilter
    /// </summary>
    public class InventoryTransferStockingQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 自己权限内能访问到的PM
        /// </summary>
        public string AuthorizedPMsSysNumber { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public ProductType? ProductType { get; set; }

        /// <summary>
        /// 分仓编号
        /// </summary>
        public string StockSysNo { get; set; }

        /// <summary>
        /// 1级分类编号
        /// </summary>
        public string Category1SysNo { get; set; }

        /// <summary>
        /// 2级分类编号
        /// </summary>
        public string Category2SysNo { get; set; }

        /// <summary>
        /// 3级分类编号
        /// </summary>
        public string Category3SysNo { get; set; }

        /// <summary>
        /// PM编号
        /// </summary>
        public string PMSysNo { get; set; }

        /// <summary>
        /// 备货天数
        /// </summary>
        public string BackDay { get; set; }

        /// <summary>
        /// 代销属性
        /// </summary>
        public string ProductConsignFlag { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public string SysNO { get; set; }

        /// <summary>
        /// 状态:
        /// </summary>
        public string ProductStatusCompareCode { get; set; }

        public ProductStatus? ProductStatus { get; set; }

        /// <summary>
        /// 日均销量
        /// </summary>
        public string DaySalesCountCompareCode { get; set; }
        public string DaySalesCount { get; set; }

        /// <summary>
        /// 可销售天数
        /// </summary>
        public string AvailableSaleDaysCompareCode { get; set; }
        public string AvailableSaleDays { get; set; }

        /// <summary>
        /// 建议备货数量
        /// </summary>
        public string RecommendBackQtyCompareCode { get; set; }
        public string RecommendBackQty { get; set; }

        /// <summary>
        /// 生产商名称
        /// </summary>
        public string ManufacturerName { get; set; }

        public List<int> VendorSysNoList { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 平均成本
        /// </summary>
        public string AverageUnitCostCompareCode { get; set; }
        public string AverageUnitCost { get; set; }


        /// <summary>
        /// 价格
        /// </summary>
        public string SalePriceCompareCode { get; set; }
        public string SalePrice { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public string PointCompareCode { get; set; }
        public string Point { get; set; }


        /// <summary>
        /// 财务库存
        /// </summary>
        public string FinanceQtyCompareCode { get; set; }
        public string FinanceQty { get; set; }


        /// <summary>
        /// 可用库存
        /// </summary>
        public string AvailableQtyCompareCode { get; set; }
        public string AvailableQty { get; set; }

        /// <summary>
        ///  被订购数
        /// </summary>
        public string OrderedQtyCompareCode { get; set; }
        public string OrderedQty { get; set; }

        /// <summary>
        /// 分仓库存
        /// </summary>
        public string SubStockQtyCompareCode { get; set; }
        public string SubStockQty { get; set; }

        /// <summary>
        /// 代销库存
        /// </summary>
        public string ConsignQtyCompareCode { get; set; }
        public string ConsignQty { get; set; }

        /// <summary>
        /// 被占用库存
        /// </summary>
        public string OccupiedQtyCompareCode { get; set; }

        public string OccupiedQty { get; set; }

        /// <summary>
        /// Online库存
        /// </summary>
        public string OnlineQtyCompareCode { get; set; }
        public string OnlineQty { get; set; }

        /// <summary>
        /// 库存同步
        /// </summary>
        public YNStatus? IsAsyncStock { get; set; }

        /// <summary>
        /// 虚库数量
        /// </summary>
        public string VirtualQtyCompareCode { get; set; }
        public string VirtualQty { get; set; }

        /// <summary>
        /// 采购在途
        /// </summary>
        public string PurchaseQtyCompareCode { get; set; }
        public string PurchaseQty { get; set; }

        /// <summary>
        /// 是否为大货
        /// </summary>
        public YNStatus? IsLarge { get; set; }


        public string SortByField
        {
            get;
            set;
        }

        public bool IsSortByAsc
        {
            get;
            set;
        }
        public bool IsSortByDesc
        {
            get;
            set;
        }

        public string CompanyCode { get; set; }
    }
}
