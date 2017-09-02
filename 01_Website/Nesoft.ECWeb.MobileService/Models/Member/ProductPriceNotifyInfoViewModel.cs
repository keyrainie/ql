using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Member;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class ProductPriceNotifyInfoViewModel
    {

        public int SysNo { get; set; }

        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 即时价格
        /// </summary>
        public decimal InstantPrice { get; set; }

        /// <summary>
        /// 期望价
        /// </summary>
        public decimal ExpectedPrice { get; set; }

        /// <summary>
        /// 提醒状态 O,A,D,H,F
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int OnlineQty { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 真实价格
        /// </summary>
        public decimal RealPrice { get; set; }

    }

    public class DeleteProductPriceNotifyInfoViewModel
    {
        public int PriceNofitySysNo { get; set; }
    }
    public class AddProductPriceNotifyInfoViewModel
    {
        public int ProductSysNo { get; set; }
        public decimal ExpectedPrice { get; set; }
        public decimal InstantPrice { get; set; }
    }
}