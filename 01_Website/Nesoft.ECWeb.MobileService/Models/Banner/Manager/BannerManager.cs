using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Facade.Recommend;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Entity;
using System.Text.RegularExpressions;
using Nesoft.ECWeb.MobileService.AppCode;

namespace Nesoft.ECWeb.MobileService.Models.Banner
{
    public class BannerManager
    {
        public List<BannerModel> GetHomeBanners()
        {
            var config = AppSettings.GetCachedConfig();
            var homeBanner = RecommendFacade.GetBannerInfoByPositionID(config.PageIDAppHome, PageType.PageTypeAppHome, BannerPosition.PositionAppHomeTopBanner).Take(5);
           return Transform(homeBanner);
        }

        private List<BannerModel> Transform(IEnumerable<BannerInfo> bannerList)
        {
            List<BannerModel> result = new List<BannerModel>();
            foreach (var bannerEntity in bannerList)
            {
                var bannerModel = new BannerModel();
                bannerModel.SysNo = bannerEntity.SysNo;
                bannerModel.BannerLink = (bannerEntity.BannerLink??"").Trim();
                bannerModel.BannerTitle = (bannerEntity.BannerTitle??"").Trim();
                bannerModel.BannerResourceUrl = (bannerEntity.BannerResourceUrl??"").Trim();
                //从BannerLink中提取相关信息(比如ProductSysNo等)
                BannerHelper.FillPromoInfo(bannerModel);

                result.Add(bannerModel);
            }

            return result;
        }

        
    }
}