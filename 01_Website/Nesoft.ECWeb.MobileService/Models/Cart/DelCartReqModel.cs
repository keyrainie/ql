using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Cart
{
    public class DelCartReqModel
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 套餐编号
        /// </summary>
        public int PackageSysNo { get; set; }
    }
}