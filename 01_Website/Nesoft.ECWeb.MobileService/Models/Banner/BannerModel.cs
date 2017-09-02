using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Banner
{
    public class BannerModel
    {
        public int SysNo { get; set; }

        /// <summary>
        /// Banner图片地址
        /// </summary>
        public string BannerResourceUrl { get; set; }

        /// <summary>
        /// 指向链接
        /// </summary>
        public string BannerLink { get; set; }

        /// <summary>
        /// Banner标题
        /// </summary>
        public string BannerTitle { get; set; }

        /// <summary>
        /// 从BannerLink中提取到的促销活动系统编号
        /// </summary>
        public int PromotionSysNo { get; set; }

        /// <summary>
        /// 从BannerLink中提取到的商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 从BannerLink中提取到的团购活动系统编号
        /// </summary>
        public int GroupBuySysNo { get; set; }

        /// <summary>
        /// 从BannerLink中提取到的三级分类系统编号
        /// </summary>
        public int CatSysNo { get; set; }

        /// <summary>
        /// 从BannerLink中提取到的搜索关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 从BannerLink中提取到的品牌ID
        /// </summary>
        public int BrandID { get; set; }
    }
}