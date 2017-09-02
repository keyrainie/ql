using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    public class InventoryQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string UserName { get; set; }

        public int? UserSysNo { get; set; }

        public List<Int32> ProductSysNos { get; set; }

        /// <summary>
        ///  渠道编号
        /// </summary>
        public int? WebChannelSysNo { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? WarehouseSysNo { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string PositionInWarehouse { get; set; }

        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        public int? StockSysNo { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 厂商编号
        /// </summary>
        public int? ManufacturerSysNo { get; set; }
        /// <summary>
        /// 品牌编号
        /// </summary>
        public int? BrandSysNo { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandNameDisplay { get; set; }
        /// <summary>
        /// 厂商名称
        /// </summary>
        public string manufacturerNameDisplay { get; set; }
        /// <summary>
        /// 商品一级类别编号
        /// </summary>
        public int? C1SysNo { get; set; }
        /// <summary>
        /// 商品二级类别编号
        /// </summary>
        public int? C2SysNo { get; set; }
        /// <summary>
        /// 商品三级类别编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        ///  商品SysNo
        /// </summary>
        public int? ProductSysNo { get; set; }

        //商品名称
        public string ProductName { get; set; }

        /// <summary>
        /// 是否显示总库存
        /// </summary>
        public bool? IsShowTotalInventory { get; set; }

        /// <summary>
        /// 是否财务库存大于0
        /// </summary>
        public bool? IsAccountQtyLargerThanZero { get; set; }

        /// <summary>
        /// PM权限
        /// </summary>
        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }

        /// <summary>
        /// 自己权限内能访问到的PM
        /// </summary>
        public string AuthorizedPMsSysNumber { get; set; }

        /// <summary>
        /// 是否显示未支付订单数量
        /// </summary>
        public bool? IsUnPayShow { get; set; }

        public string RMAInventoryOnlineDate { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 库存预警
        /// </summary>
        public bool? IsInventoryWarning { get; set; }
    }
}
