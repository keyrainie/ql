using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Seckill;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Facade.Seckill;
using Nesoft.ECWeb.MobileService.Models.Promotion;
using Nesoft.ECWeb.Facade.Recommend;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Models.Banner;
using Nesoft.ECWeb.MobileService.AppCode;
using Nesoft.ECWeb.Entity.Category;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class RecommendManager
    {
        /// <summary>
        /// 获取精选品牌列表
        /// </summary>
        /// <returns></returns>
        public RecommendBrandModel GetRecommendBrands()
        {
            var config = AppSettings.GetCachedConfig();
            List<BannerInfo> bannerBrandBigList = RecommendFacade.GetBannerInfoByPositionID(config.PageIDAppHome, PageType.PageTypeAppHome, BannerPosition.PositionAppHomeBrandBig).Take(4).ToList();
            List<BannerInfo> bannerBrandSmallList = RecommendFacade.GetBannerInfoByPositionID(config.PageIDAppHome, PageType.PageTypeAppHome, BannerPosition.PositionAppHomeBrandSmall).Take(18).ToList();

            RecommendBrandModel result = new RecommendBrandModel();
            result.BigBrands = MapBrandItemList(bannerBrandBigList);
            result.SmallBrands = MapBrandItemList(bannerBrandSmallList);

            return result;
        }

        private List<RecommendBrandItemModel> MapBrandItemList(List<BannerInfo> bannerList)
        {
            List<RecommendBrandItemModel> brandList = new List<RecommendBrandItemModel>();
            if (bannerList == null)
            {
                return brandList;
            }

            RecommendBrandItemModel itemModel = null;
            foreach (var banner in bannerList)
            {
                itemModel = new RecommendBrandItemModel();
                itemModel.ImageUrl = (banner.BannerResourceUrl??"").Trim();
                itemModel.BrandName = (banner.BannerTitle??"").Trim();
                itemModel.BrandID = BannerHelper.ExtractBrandSysNo(banner.BannerLink);

                brandList.Add(itemModel);
            }

            return brandList;
        }

        /// <summary>
        /// 获取今日特卖商品列表
        /// </summary>
        /// <returns></returns>
        public List<RecommendItemModel> GetTodayHotSaleItemlist()
        {
            var config = AppSettings.GetCachedConfig();
            return GetHomeRecommendItemList(config.PositionIDTodaySaleItemList, config.CountTodaySaleItemList);
        }

        /// <summary>
        /// 获取首页新品上市商品列表
        /// </summary>
        /// <returns></returns>
        public List<RecommendItemModel> GetHomeRecommendProductList()
        {
            var config = AppSettings.GetCachedConfig();
            List<RecommendItemModel> res = new List<RecommendItemModel>();
            List<RecommendProduct> resultList = RecommendFacade.QueryNewRecommendProduct(config.CountRecommendProductItemList, ConstValue.LanguageCode, ConstValue.CompanyCode);
            if (null != resultList && resultList.Count > 0)
            {

                foreach (var item in resultList)
                {
                    RecommendItemModel model = new RecommendItemModel()
                    {
                        Code = item.ProductID,
                        ID = item.SysNo,
                        ImageUrl = ProductFacade.BuildProductImage(ImageUrlHelper.GetImageSize(ImageType.Middle), item.DefaultImage),
                        Price = new SalesInfoModel() { CurrentPrice = item.RealPrice, BasicPrice = item.BasicPrice, TariffPrice = item.TariffPrice, CashRebate=item.CashRebate },
                        ProductTitle = item.ProductTitle,
                        PromotionTitle = item.PromotionTitle
                    };
                    res.Add(model);
                }
            }
            return res;
        }

        /// <summary>
        /// 获取首页热卖推荐商品列表
        /// </summary>
        /// <returns></returns>
        public List<RecommendItemModel> GetHomeHotSaleItemList()
        {
            var config = AppSettings.GetCachedConfig();
            return GetHomeRecommendItemList(config.PositionIDHomeHotSaleItemList, config.CountHomeHotSaleItemList);
        }

        /// <summary>
        /// 获取首页品牌推荐商品列表
        /// </summary>
        /// <returns></returns>
        public List<RecommendItemModel> GetHomeBrandItemList()
        {
            var config = AppSettings.GetCachedConfig();
            return GetHomeRecommendItemList(config.PositionIDHomeBrandItemList, config.CountHomeBrandItemList);
        }

        /// <summary>
        /// 获取首页超低折扣商品列表
        /// </summary>
        /// <returns></returns>
        public List<RecommendItemModel> GetHomeDiscountItemList()
        {
            var config = AppSettings.GetCachedConfig();
            return GetHomeRecommendItemList(config.PositionIDHomeDiscountItemList, config.CountHomeDiscountItemList);
        }

        public List<RecommendItemModel> GetHomeRecommendItemList(int postionID, int count)
        {
            var recommendItemEntityList = RecommendFacade.QueryRecommendProduct(0, 0, postionID, count, ConstValue.LanguageCode, ConstValue.CompanyCode);
            List<RecommendItemModel> result = new List<RecommendItemModel>();
            ImageSize imageSize = ImageUrlHelper.GetImageSize(ImageType.Middle);
            foreach (var itemEntity in recommendItemEntityList)
            {
                RecommendItemModel itemModel = new RecommendItemModel();
                itemModel.ID = itemEntity.SysNo;
                itemModel.ProductTitle = itemEntity.BriefName;
                itemModel.PromotionTitle = itemEntity.PromotionTitle;
                itemModel.Code = itemEntity.ProductID;
                itemModel.ImageUrl = ProductFacade.BuildProductImage(imageSize, itemEntity.DefaultImage);
                var priceModel = new SalesInfoModel();
                itemModel.Price = priceModel;
                priceModel.BasicPrice = itemEntity.BasicPrice;
                priceModel.CurrentPrice = itemEntity.RealPrice;
                priceModel.CashRebate = itemEntity.CashRebate;
                priceModel.TariffPrice = itemEntity.TariffPrice;
                priceModel.FreeEntryTax = itemEntity.TariffPrice <= ConstValue.TariffFreeLimit;
                decimal realTariffPrice = priceModel.TariffPrice;
                if (priceModel.FreeEntryTax)
                {
                    realTariffPrice = 0;
                }
                priceModel.TotalPrice = itemEntity.CurrentPrice + itemEntity.CashRebate + realTariffPrice;

                result.Add(itemModel);
            }

            return result;
        }
    }
}