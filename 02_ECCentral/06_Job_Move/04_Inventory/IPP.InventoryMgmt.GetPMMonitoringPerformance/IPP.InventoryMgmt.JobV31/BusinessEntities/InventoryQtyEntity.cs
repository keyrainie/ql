using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.InventoryMgmt.Taobao.JobV31.BusinessEntities
{
    /// <summary>
    /// 差异库存
    /// </summary>
    [Serializable]
    public class InventoryQtyEntity
    {
        public int ProductMappingSysNo { get; set; }

        /// <summary>
        /// 第三方商品标识ID
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品SysNo
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 实际的差异库存(即须向第三方同步的库存数量)
        /// </summary>
        public int SynInventoryQty { get; set; }

        /// <summary>
        /// 第三方标识
        /// </summary>
        public string PartnerType { get; set; }

        /// <summary>
        /// 库存预警值
        /// </summary>
        public int? InventoryAlarmQty { get; set; }

        /// <summary>
        /// 总库存的变化量
        /// </summary>
        public int InventoryQty { get; set; }

    }
}
