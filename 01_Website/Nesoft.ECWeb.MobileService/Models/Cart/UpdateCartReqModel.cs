using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Cart
{
    public class UpdateCartReqModel
    {
        /// <summary>
        /// 系统编号
        /// 单个商品:ProductSysNo
        /// 套餐:PackageSysNo
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 是否套餐
        /// </summary>
        public bool IsPackage { get; set; }
    }
}