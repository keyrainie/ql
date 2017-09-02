using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Cart
{
    public class ProductGiftReqModel
    {
        /// <summary>
        /// 活动编号
        /// </summary>
        public int ActivityNo { get; set; }
        /// <summary>
        /// 套餐编号
        /// </summary>
        public int PackageSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 赠品编号
        /// </summary>
        public int GiftSysNo { get; set; }
    }
}