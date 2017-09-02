using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.Inventory
{
    /// <summary>
    /// 转换单-转换商品信息
    /// </summary>
    public class ConvertRequestItemInfo : IIdentity
    {
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 借出商品信息
        /// </summary>
        public ProductInfo ConvertProduct { get; set; }

        /// <summary>
        /// 转换类型
        /// </summary>
        public ConvertProductType ConvertType { get; set; }

        /// <summary>
        /// 转换数量
        /// </summary>
        public int ConvertQuantity { get; set; }

        /// <summary>
        /// 转换成本
        /// </summary>
        public decimal ConvertUnitCost { get; set; }

        /// <summary>
        /// 转换去税成本
        /// </summary>
        public decimal ConvertUnitCostWithoutTax { get; set; }

        public ConvertRequestItemInfo()
        {
            ConvertProduct = new ProductInfo(); 
        }

        public List<InventoryBatchDetailsInfo> BatchDetailsInfoList { get; set; }

    }
}
