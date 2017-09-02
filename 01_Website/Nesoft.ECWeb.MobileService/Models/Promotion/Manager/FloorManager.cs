using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Facade.Recommend;
using Nesoft.ECWeb.Facade.Common;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.MobileService.Models.Banner;
using Nesoft.ECWeb.MobileService.AppCode;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class FloorManager
    {
        public List<FloorModel> GetHomeFloors()
        {
            var config = AppSettings.GetCachedConfig();
            List<FloorEntity> floorList = RecommendFacade.GetFloorInfo((PageCodeType)config.PageCodeFloorAppHome, 1).ToList();

            List<FloorModel> result = new List<FloorModel>();

            foreach (var floor in floorList)
            {
                var model = MapFloor(floor);

                result.Add(model);
            }

            return result;
        }

        private FloorModel MapFloor(FloorEntity floorEntity)
        {
            List<FloorItemBanner> bannerList = FloorHelper.GetFloorItem<FloorItemBanner>(floorEntity).OrderBy(x => x.Priority).ToList();
            List<FloorItemProduct> productList = FloorHelper.GetFloorItem<FloorItemProduct>(floorEntity).OrderBy(x => x.Priority).ToList();

            FloorModel model = new FloorModel();
            //楼层基本信息
            model.Name = floorEntity.FloorName;
            //banner
            var banner = bannerList.FirstOrDefault();
            if (banner != null)
            {
                model.Banner.BannerResourceUrl = banner.ImageSrc;
                model.Banner.BannerTitle = banner.BannerText;
                model.Banner.BannerLink = banner.LinkUrl;
                //从BannerLink中提取相关信息(比如ProductSysNo等)
                BannerHelper.FillPromoInfo(model.Banner);
            }

            //推荐商品列表
            ImageSize imageSize = ImageUrlHelper.GetImageSize(ImageType.Small);
            for (int i = 0; i < 3 && i < productList.Count; i++)
            {
                var item = productList[i];
                var floorItemModel = MapFloorItem(item, imageSize);

                model.ItemList.Add(floorItemModel);
            }

            return model;
        }

        private RecommendItemModel MapFloorItem(FloorItemProduct itemEntity, ImageSize imageSize)
        {
            RecommendItemModel itemModel = new RecommendItemModel();
            itemModel.ID = itemEntity.ProductSysNo;
            itemModel.ProductTitle = itemEntity.ProductTitle;
            //itemModel.PromotionTitle = itemEntity.PromotionTitle;
            itemModel.Code = "";
            itemModel.ImageUrl = ProductFacade.BuildProductImage(imageSize, itemEntity.DefaultImage);
            var priceModel = new SalesInfoModel();
            itemModel.Price = priceModel;
            priceModel.BasicPrice = itemEntity.BasicPrice ?? 0m;
            priceModel.CurrentPrice = itemEntity.ProductPrice ?? 0m;
            priceModel.CashRebate = itemEntity.CashRebate ?? 0m;;
            priceModel.TariffPrice = 0;
            priceModel.TotalPrice = itemEntity.RealPrice ?? 0m ;

            return itemModel;
        }
    }
}