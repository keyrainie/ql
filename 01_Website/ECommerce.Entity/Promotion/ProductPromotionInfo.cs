using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Promotion
{
    /// <summary>
    /// 商品的所有促销信息
    /// </summary>
    [Serializable]
    public class ProductPromotionInfo
    {
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 是否是限时抢购，正在限时抢购，返回限时抢购编码，否则返回0
        /// </summary>
        public int CountdownSysNo { get; set; }

        /// <summary>
        /// 是否是秒杀，秒杀是限时抢购中的一种
        /// </summary>
        public bool IsSecKill { get; set; }

        /// <summary>
        /// 是否是团购,在团购，返回团购编码，否则返回0
        /// </summary>
        public int GroupBuySysNo { get; set; }

        /// <summary>
        /// 下单立减金额，如果大于0，则表示下单立减
        /// </summary>
        public decimal BuyReducePrice { get; set; }

        /// <summary>
        /// 组合套餐的列表
        /// </summary>
        public List<ComboInfo> ComboList { get; set; }


        /// <summary>
        /// 赠品活动列表，为空或数量为0则无赠品活动。赠品活动实体内包含指定模式的赠品
        /// </summary>
        public List<SaleGiftInfo> SaleGiftList { get; set; }

        /// <summary>
        /// 显示抢购或者秒杀
        /// </summary>
        public CountdownInfo Countdown { get; set; }

         
    }
}
