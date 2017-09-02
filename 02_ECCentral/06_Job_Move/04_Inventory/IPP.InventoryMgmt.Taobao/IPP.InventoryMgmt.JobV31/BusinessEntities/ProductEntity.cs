using System;
using System.Data;

using Newegg.Oversea.Framework.Entity;
namespace IPP.InventoryMgmt.Taobao.JobV31.BusinessEntities
{
    [Serializable]
    public class ProductEntity
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMapping("ProductID", DbType.Int32)]
        public string ProductID { get; set; }

        /// <summary>
        /// 第三方的商品ID标识
        /// </summary>
        [DataMapping("SynProductID", DbType.String)]
        public string SKU { get; set; }

        /// <summary>
        /// 第三方标识
        /// </summary>
        [DataMapping("PartnerType", DbType.String)]
        public string PartnerType { get; set; }

        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMapping("WarehouseNumber", DbType.Int32)]
        public int WarehouseNumber { get; set; }

        /// <summary>
        /// 当前的Online库存
        /// </summary>
        [DataMapping("InventoryQty", DbType.Int32)]
        public int InventoryQty { get; set; }

        [DataMapping("MappingInventoryQty", DbType.Int32)]
        public int MappingInventoryQty { get; set; }

        /// <summary>
        /// 映射主表SysNo
        /// </summary>
        [DataMapping("ProductMappingSysNo", DbType.Int32)]
        public int ProductMappingSysNo { get; set; }

        /// <summary>
        /// 旧的库存(尚未同步之前的库存)
        /// </summary>
        [DataMapping("OldInventoryQty", DbType.Int32)]
        public int OldInventoryQty { get; set; }


        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("InventoryAlarmQty", DbType.Int32)]
        public int? InventoryAlarmQty { get; set; }

        /// <summary>
        /// 实际要调整的库存数量
        /// </summary>
        public int ResultQty
        {
            get
            {
                return InventoryQty - OldInventoryQty;
            }
        }
    }
}
