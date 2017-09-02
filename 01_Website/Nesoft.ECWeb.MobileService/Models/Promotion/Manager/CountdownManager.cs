using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Entity.Seckill;
using Nesoft.ECWeb.Facade.Seckill;
using Nesoft.ECWeb.Facade.Product;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class CountdownManager
    {
        /// <summary>
        /// 限时抢购列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public QueryResult<CountDownItemModel> GetCountDownList(int pageIndex, int pageSize)
        {
            var countDownList = CountDownFacade.GetCountDownList(pageIndex, pageSize);
            //列表信息
            QueryResult<CountDownItemModel> result = new QueryResult<CountDownItemModel>();
            result.ResultList = new List<CountDownItemModel>();
            ImageSize imageSize = ImageUrlHelper.GetImageSize(ImageType.Middle);
            var countDownItemList = countDownList.ResultList ?? new List<CountDownInfo>();
            foreach (var item in countDownItemList)
            {

                CountDownItemModel itemModel = new CountDownItemModel();
                itemModel.Code = item.ProductID;
                itemModel.ID = item.ProductSysNo;
                itemModel.ImageUrl = ProductFacade.BuildProductImage(imageSize, item.DefaultImage);
                itemModel.ProductTitle = item.ProductTitle;
                itemModel.PromotionTitle = item.PromotionTitle;
                //Asura 添加开始时间   开始

                itemModel.StartTime = string.Format("{0:G}", item.StartTime);

                //Asura 添加开始时间   结束
                if (item.StartTime > DateTime.Now)
                {
                    itemModel.LeftSeconds = (int)(DateTime.Now - item.StartTime).TotalSeconds;
                }
                else
                {
                    if (item.EndTime < DateTime.Now)
                    {
                        itemModel.LeftSeconds = 0;
                    }
                    else
                    {
                        itemModel.LeftSeconds = (int)(item.EndTime - DateTime.Now).TotalSeconds;
                    }
                }
                //销售相关信息
                var priceModel = new SalesInfoModel();
                itemModel.Price = priceModel;
                decimal realSnapShotTariffPrice = item.SnapShotTariffPrice;
                if (realSnapShotTariffPrice <= ConstValue.TariffFreeLimit)
                {
                    realSnapShotTariffPrice = 0;
                }
                priceModel.BasicPrice = item.SnapShotCurrentPrice + item.SnapShotCashRebate + realSnapShotTariffPrice;
                priceModel.CurrentPrice = item.CountDownPrice;
                priceModel.CashRebate = item.CountDownCashRebate;
                priceModel.TariffPrice = item.TariffPrice;
                decimal realTariffPrice = item.TariffPrice;
                if (item.TariffPrice <= ConstValue.TariffFreeLimit)
                {
                    realTariffPrice = 0;
                }
                priceModel.TotalPrice = item.CountDownPrice + item.CountDownCashRebate + realTariffPrice;
                priceModel.OnlineQty = item.OnlineQty;
                result.ResultList.Add(itemModel);
            }
            result.ResultList.Sort((x, y) => x.LeftSeconds.CompareTo(y.LeftSeconds));
            //result.ResultList.Reverse();
            //分页信息
            if (countDownList.PageInfo != null)
            {
                var pageInfo = new PageInfo();
                pageInfo.PageIndex = pageIndex;
                pageInfo.PageSize = pageSize;
                pageInfo.TotalCount = countDownList.PageInfo.TotalCount;

                result.PageInfo = pageInfo;
            }

            return result;
        }
    }
}