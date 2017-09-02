using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT 
{
    /// <summary>
    /// 活动商品享受的规则，如赠品等
    /// </summary>
    public class PSGiftItemRule
    {
        /// <summary>
        /// 赠送的商品及数量列表
        /// </summary>
        public List<PSGiftItem> GiftItemSysNoList { get; set; }
        
        /// <summary>
        /// 当针对购买1个或多个主商品进行赠品时本属性有值
        /// 预留：用于做赠品分摊
        /// </summary>
        public List<int?> MasterProductSysNoList { get; set; }
        
    }
    /// <summary>
    /// 赠品信息
    /// </summary>
    public class PSGiftItem
    {
        /// <summary>
        /// 赠品商品SysNo
        /// </summary>
        public int? GiftItemSysNo { get; set; }

        /// <summary>
        /// 赠品商品数量
        /// </summary>
        public int? GiftItemCount { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }
    }
     
}
