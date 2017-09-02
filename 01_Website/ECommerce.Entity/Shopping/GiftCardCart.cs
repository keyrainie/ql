using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Shopping
{
    /// <summary>
    /// 礼品卡购物车Cookie对象
    /// </summary>
    [Serializable]
    [DataContract]
    public class GiftCardCart
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }
    }
}
