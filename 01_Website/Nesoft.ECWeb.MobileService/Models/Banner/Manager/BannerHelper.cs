using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Nesoft.ECWeb.MobileService.Models.Banner
{
    public class BannerHelper
    {
        public static void FillPromoInfo(BannerModel uiBannerInfo)
        {
            if (string.IsNullOrWhiteSpace(uiBannerInfo.BannerLink))
            {
                return;
            }

            if (uiBannerInfo.BannerLink.IndexOf("http://127.0.0.1/") >= 0)
            {
                //提取促销模板系统编号
                uiBannerInfo.PromotionSysNo = ExtractPromotionSysNo(uiBannerInfo.BannerLink);
                uiBannerInfo.BannerLink = "";
            }
            else if (uiBannerInfo.BannerLink.IndexOf("/product/detail") >= 0)
            {
                //提取商品系统编号
                uiBannerInfo.ProductSysNo = ExtractProductSysNo(uiBannerInfo.BannerLink);
                uiBannerInfo.BannerLink = "";
            }
            else if (uiBannerInfo.BannerLink.IndexOf("/groupbuying") >= 0)
            {
                //提取团购系统编号
                uiBannerInfo.GroupBuySysNo = ExtractGroupBuySysNo(uiBannerInfo.BannerLink);
                uiBannerInfo.BannerLink = "";
            }

            else if (uiBannerInfo.BannerLink.IndexOf("/product/searchresult") >= 0)
            {
                //提取搜索关键字
                uiBannerInfo.Keywords = ExtractKeywords(uiBannerInfo.BannerLink);
                uiBannerInfo.BannerLink = "";
            }
            else if (uiBannerInfo.BannerLink.IndexOf("/substore") >= 0)
            {
                //提取三级分类系统编号
                uiBannerInfo.CatSysNo = ExtractC3SysNo(uiBannerInfo.BannerLink);
                uiBannerInfo.BannerLink = "";
            }
            else if (uiBannerInfo.BannerLink.IndexOf("/BrandZone") >= 0)
            {
                //提取品牌系统编号
                uiBannerInfo.BrandID = ExtractBrandSysNo(uiBannerInfo.BannerLink);
                uiBannerInfo.BannerLink = "";
            }

        }

        /// <summary>
        /// 提取促销模板系统编号
        /// </summary>
        /// <param name="bannerLink"></param>
        /// <returns></returns>
        public static int ExtractPromotionSysNo(string bannerLink)
        {
            bannerLink = (bannerLink ?? "").Trim();
            string promotionSysNo = bannerLink.Replace("http://127.0.0.1/", "");
            int sysNo;
            if (int.TryParse(promotionSysNo, out sysNo))
            {
                return sysNo;
            }

            return 0;
        }

        /// <summary>
        /// 提取商品系统编号
        /// </summary>
        /// <param name="bannerLink"></param>
        /// <returns></returns>
        public static int ExtractProductSysNo(string bannerLink)
        {
            bannerLink = (bannerLink ?? "").Trim();
            var m = Regex.Match(bannerLink, @"http:\/\/www\.dchnu\.cn\/product\/detail\/(\d*).*", RegexOptions.IgnoreCase);
            int productSysNo;
            if (m.Success && int.TryParse(m.Groups[1].Value, out productSysNo))
            {
                return productSysNo;
            }

            return 0;
        }

        /// <summary>
        /// 提取团购系统编号
        /// </summary>
        /// <param name="bannerLink"></param>
        /// <returns></returns>
        public static int ExtractGroupBuySysNo(string bannerLink)
        {
            bannerLink = (bannerLink ?? "").Trim();
            int groupBuySysNo;
            var m = Regex.Match(bannerLink, @"http:\/\/www\.dchnu\.cn\/groupbuying\/(\d+).*", RegexOptions.IgnoreCase);
            if (m.Success && int.TryParse(m.Groups[1].Value, out groupBuySysNo))
            {
                return groupBuySysNo;
            }

            return 0;
        }

        /// <summary>
        /// 提取搜索关键字
        /// </summary>
        /// <param name="bannerLink"></param>
        /// <returns></returns>
        public static string ExtractKeywords(string bannerLink)
        {
            bannerLink = (bannerLink ?? "").Trim();
            var m = Regex.Match(bannerLink, @"http:\/\/www\.dchnu\.cn\/product\/searchresult\?keyword=([^&]+)&?.*", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return "";
        }

        /// <summary>
        /// 提取三级分类系统编号
        /// </summary>
        /// <param name="bannerLink"></param>
        /// <returns></returns>
        public static int ExtractC3SysNo(string bannerLink)
        {
            bannerLink = (bannerLink ?? "").Trim();
            int catSysNo;
            var m = Regex.Match(bannerLink, @"http:\/\/www\.dchnu\.cn\/substore\/(\d+)&?.*", RegexOptions.IgnoreCase);
            if (m.Success && int.TryParse(m.Groups[1].Value, out catSysNo))
            {
                return catSysNo;
            }

            return 0;
        }

        /// <summary>
        /// 从BannerLink中抽取品牌系统编号
        /// </summary>
        /// <param name="bannerLink"></param>
        /// <returns></returns>
        public static int ExtractBrandSysNo(string bannerLink)
        {
            bannerLink = (bannerLink ?? "").Trim();
            var m = Regex.Match(bannerLink, @"http:\/\/www\.dchnu\.cn\/BrandZone\/(\d+).*", RegexOptions.IgnoreCase);
            int brandSysNo;
            if (m.Success && int.TryParse(m.Groups[1].Value, out brandSysNo))
            {
                return brandSysNo;
            }

            return 0;
        }
    }
}