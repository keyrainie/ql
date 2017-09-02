
using System;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;
using System.Collections.Generic;

namespace ECCentral.QueryFilter.IM
{
    public class ProductQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        public string ChannelID
        {
            get;
            set;
        }

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
        /// 渠道ID
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        public int? StockSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 是/否代销
        /// </summary>
        public string IsConsign
        {
            get;
            set;
        }

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
        /// 合作商ID
        /// </summary>
        public string ThirdPartyProductID { get; set; }

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
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 创建结束日期
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        /// <summary>
        /// 库存同步合作
        /// </summary>
        public InventorySync? SyncThirdPartyInventoryType { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public ProductType? ProductType { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus? ProductStatus { get; set; }

        /// <summary>
        /// 状态不为作废状态
        /// </summary>
        public bool? IsNotAbandon { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 品牌SysNo
        /// </summary>
        public int? BrandSysNo { get; set; }

        /// <summary>
        /// 在线库存
        /// </summary>
        public int? OnlineQty { get; set; }

        /// <summary>
        /// 库存条件操作符
        /// </summary>
        public string OnlineCondition { get; set; }

        /// <summary>
        /// 供应商SysNo
        /// </summary>
        public int? VendorSysNo { get; set; }

        public int? AZCustomer { get; set; }

        public int? MerchantSysNo { get; set; }

        public List<string> ProductIds { get; set; }
    }
}
