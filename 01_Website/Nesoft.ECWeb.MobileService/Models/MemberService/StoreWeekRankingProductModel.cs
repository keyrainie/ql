using Nesoft.ECWeb.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.MemberService
{
    public class StoreWeekRankingProductModel : EntityBase
    {
        public int SysNo { get; set; }
        public int BrandSysNo { get; set; }
        public string ProductID { get; set; }
        public int Priority { get; set; }
        public string BriefName { get; set; }
        public string DefaultImage { get; set; }

        public string ProductName { get; set; }
        public string ProductTitle { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal BasicPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TariffRate { get; set; }
        public decimal CashRebate { get; set; }
        public string PromotionTitle { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal TariffPrice
        { get; set; }

        /// <summary>
        /// 实际的销售价格
        /// </summary>
        public decimal RealPrice
        { get; set; }
    }
}