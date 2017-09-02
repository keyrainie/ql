using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductQueryVM : ModelBase
    {
        public ProductQueryVM()
        {
            ProductTypeList = EnumConverter.GetKeyValuePairs<ProductType>(EnumConverter.EnumAppendItemType.All);
            SyncThirdPartyInventoryTypeList = EnumConverter.GetKeyValuePairs<InventorySync>(EnumConverter.EnumAppendItemType.All);
            ProductStatusList = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
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
        /// 商品名称
        /// </summary>
        public string ProductName
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

        public List<KeyValuePair<ProductType?, string>> ProductTypeList { get; set; }

        public List<KeyValuePair<InventorySync?, string>> SyncThirdPartyInventoryTypeList { get; set; }

        public List<KeyValuePair<ProductStatus?, string>> ProductStatusList { get; set; }

    }

    public class ProductManufactureQueryVM : ModelBase
    {
        /// <summary>
        /// 生产商
        /// </summary>
        public string ManufactureName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string VendorName { get; set; }
    }
}
