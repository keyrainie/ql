using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Promotion;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Facade.Recommend;
using Nesoft.ECWeb.Facade.Common;
using Nesoft.ECWeb.MobileService.Models.Banner;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class SaleAdvManager
    {
        /// <summary>
        /// 获取促销模板信息
        /// </summary>
        /// <param name="promotionSysNo">促销模板编号</param>
        /// <returns></returns>
        public List<SaleAdvModel> GetSaleAdvInfo(int promotionSysNo)
        {
            SaleAdvertisement promotion = ProductFacade.GetSaleAdvertisementInfo(promotionSysNo);
            List<SaleAdvModel> result = new List<SaleAdvModel>();
            if (promotion != null && promotion.SaleAdvertisementGroupList != null)
            {
                SaleAdvModel model = null;
                foreach (var group in promotion.SaleAdvertisementGroupList)
                {
                    if (group.SaleAdvertisementItemList != null && group.SaleAdvertisementItemList.Count > 0)
                    {
                        model = new SaleAdvModel();
                        model.GroupName = group.GroupName;
                        model.ItemList = MapAdvItem(group.SaleAdvertisementItemList);
                        result.Add(model);
                    }
                }
            }
            return result;
        }

        public List<RecommendItemModel> MapAdvItem( List<SaleAdvertisementItem> advItemList)
        {
            List<RecommendItemModel> result = new List<RecommendItemModel>();
            if (advItemList == null)
            {
                return result;
            }
            ImageSize imageSize = ImageUrlHelper.GetImageSize(ImageType.Big);
            foreach (var itemEntity in advItemList)
            {
                RecommendItemModel itemModel = new RecommendItemModel();
                itemModel.ID = itemEntity.ProductSysNo;
                itemModel.ProductTitle = itemEntity.ProductTitle;
                itemModel.Code = itemEntity.ProductCode;
                itemModel.ImageUrl = ProductFacade.BuildProductImage(imageSize, itemEntity.DefaultImage);
                var priceModel = new SalesInfoModel();
                itemModel.Price = priceModel;
                priceModel.BasicPrice = itemEntity.MarketPrice;
                priceModel.CurrentPrice = itemEntity.CurrentPrice;
                priceModel.CashRebate = itemEntity.CashRebate;
                priceModel.FreeEntryTax = itemEntity.ProductTariffAmt <= ConstValue.TariffFreeLimit;
                priceModel.TariffPrice = itemEntity.ProductTariffAmt;
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