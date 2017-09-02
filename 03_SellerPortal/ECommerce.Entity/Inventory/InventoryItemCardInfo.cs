using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.Inventory
{
    public class InventoryItemCardInfo
    {
        /// <summary>
        /// 单据名字
        /// </summary>
        public string OrderName { get; set; }
        /// <summary>
        /// 商品SysNo
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 单据时间
        /// </summary>
        public DateTime OrderTime { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 单据SysNo
        /// </summary>
        public int OrderSysNo { get; set; }
        /// <summary>
        /// 单据数量
        /// </summary>
        public int OrderQty { get; set; }
        /// <summary>
        /// 结存数量
        /// </summary>
        public int OrderThenQty { get; set; }
        /// <summary>
        /// 仓库SysNo
        /// </summary>
        public int StockSysNo { get; set; }
        public int RelatedSysNo { get; set; }
        public string CompanyCode { get; set; }

        public string OrderNameString
        {
            get
            {
                if (!string.IsNullOrEmpty(OrderName))
                {
                    return CodeNamePairManager.GetName("Inventory", "InventoryCardOrderType", OrderName);
                }
                return string.Empty;
            }
        }

        public string OrderTimeString
        {
            get
            {
                return (OrderTime != null && OrderTime != new DateTime()) ? OrderTime.ToString("yyyy年MM月dd日 HH:mm:ss") : string.Empty;
            }
        }
    }
}
