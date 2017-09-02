using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.InventoryMgmt.JobV31.Common
{
    public class CommonConst
    {
        /// <summary>
        /// 批量获取淘宝商品仓库中的商品信息
        /// </summary>
        public const string taobao_items_inventory_get = "taobao.items.inventory.get";

        /// <summary>
        /// 批量获取淘宝商品库存信息要查询的淘宝的字段名称集合
        /// </summary>
        public const string taobao_items_inventory_get_fields = "num_iid,num,outer_id";

        /// <summary>
        /// 批量获取淘宝正在出售的商品信息
        /// </summary>
        public const string taobao_items_onsale_get = "taobao.items.onsale.get";

        /// <summary>
        /// 批量获取淘宝商品库存信息要查询的批量数据量
        /// </summary>
        public const int taobao_items_inventory_get_pageSize = 40;

        /// <summary>
        /// 淘宝请求的字符编码
        /// </summary>
        public static Encoding taobao_response_encoding
        {
            get
            {
                return Encoding.GetEncoding("UTF-8");
            }
        }
    }
}
