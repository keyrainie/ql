using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Facade.GroupBuying;
using Nesoft.ECWeb.Entity.Promotion.GroupBuying;
using Nesoft.ECWeb.MobileService.Models.Product;

namespace Nesoft.ECWeb.MobileService.Models.Promotion
{
    public class GroupBuyManager
    {
        /// <summary>
        /// 团购查询
        /// </summary>
        /// <param name="criteria">查询条件</param>
        /// <returns></returns>
        public GroupBuyQueryResult GetGroupBuyList(GroupBuyQueryModel criteria)
        {
            GroupBuyingQueryInfo queryInfo = new GroupBuyingQueryInfo()
            {
                PageInfo = new Entity.PageInfo()
                {
                    PageIndex = criteria.PageIndex,
                    PageSize = criteria.PageSize
                },
                SortType = criteria.SortType
            };
            //仅查询普通团购
            queryInfo.GroupBuyingTypeSysNo = 0;
            if (criteria.CatSysNo > 0)
                queryInfo.CategorySysNo = criteria.CatSysNo;
            else
                queryInfo.CategorySysNo = null;

            GroupBuyQueryResult result = new GroupBuyQueryResult();
            if (criteria.GetFilters)
            {
                var categoryList = GroupBuyingFacade.GetGroupBuyingCategory();

                result.Filters = MapCatList(categoryList);
                if (result.Filters.Count > 0)
                {
                    //增加一个全部选项
                    GroupBuyCatModel allCat = new GroupBuyCatModel();
                    allCat.CateSysNo = 0;
                    allCat.Name = "全部";
                    result.Filters.Insert(0, allCat);
                }
            }

            var groupBuyResult = GroupBuyingFacade.QueryGroupBuyingInfo(queryInfo);

            result.Result = MapItemList(groupBuyResult);

            return result;
        }

        private QueryResult<GroupBuyBaseModel> MapItemList(QueryResult<GroupBuyingInfo> queryResult)
        {
            QueryResult<GroupBuyBaseModel> result = new QueryResult<GroupBuyBaseModel>();
            result.ResultList = new List<GroupBuyBaseModel>();
            var resultList = queryResult.ResultList ?? new List<GroupBuyingInfo>();
            foreach (var item in resultList)
            {
                GroupBuyBaseModel itemModel = new GroupBuyBaseModel();
                itemModel.GroupBuyingPicUrl = item.GroupBuyingSmallPicUrl;
                itemModel.GroupBuyingSysNo = item.SysNo;
                itemModel.GroupBuyingTitle = item.GroupBuyingTitle;
                itemModel.CurrentSellCount = item.CurrentSellCount;
               
                //计算剩余时间
                if (item.EndDate <= DateTime.Now)
                {
                    itemModel.LeftSeconds = 0;
                }
                else
                {
                    itemModel.LeftSeconds = (long)(item.EndDate - DateTime.Now).TotalSeconds;
                }
                //计算销售状态
                if (itemModel.LeftSeconds == 0)
                {
                    itemModel.SellStatusStr = "团购结束";
                }
                else if (item.OnlineQty <= 0)
                {
                    itemModel.SellStatusStr = "已售罄";
                }
                else
                {
                    itemModel.SellStatusStr = "团购进行中";
                }
                var priceModel = new SalesInfoModel();
                itemModel.Price = priceModel;


                priceModel.CurrentPrice = item.CurrentPrice;
                priceModel.CashRebate = 0;
                //海外商品，算税
                if (item.GroupBuyingTypeSysNo == 0)
                {
                    priceModel.TariffPrice = item.TaxRate * item.CurrentPrice;
                    decimal snapShotTariffPrice = item.SnapShotCurrentPrice * item.TaxRate;
                    if (snapShotTariffPrice <= ConstValue.TariffFreeLimit)
                    {
                        snapShotTariffPrice = 0;
                    }
                    priceModel.BasicPrice = item.SnapShotCurrentPrice + item.SnapShotCashRebate + snapShotTariffPrice;
                }
                else
                {
                    priceModel.TariffPrice = 0;
                    priceModel.BasicPrice = item.SnapShotCurrentPrice + item.SnapShotCashRebate;
                }
                decimal realTariffPrice = priceModel.TariffPrice;
                bool isTaxFree = priceModel.TariffPrice <= ConstValue.TariffFreeLimit;
                if (isTaxFree)
                {
                    realTariffPrice = 0;
                }
                priceModel.TotalPrice = item.CurrentPrice + realTariffPrice;
                priceModel.FreeEntryTax = isTaxFree;
                priceModel.OnlineQty = item.OnlineQty;

                //计算折扣
                decimal finalPrice = priceModel.TotalPrice;
                if (item.MarketPrice == 0)
                {
                    itemModel.DiscountStr = "";
                }
                else
                {
                    itemModel.DiscountStr = (finalPrice / item.MarketPrice * 10).ToString("F2") + "折";
                }
                //计算省好多
                itemModel.SaveMoney = item.MarketPrice - finalPrice;

                result.ResultList.Add(itemModel);
            }

            //分页信息
            if (queryResult.PageInfo != null)
            {
                var pageInfo = new PageInfo();
                pageInfo.PageIndex = queryResult.PageInfo.PageIndex;
                pageInfo.PageSize = queryResult.PageInfo.PageSize;
                pageInfo.TotalCount = queryResult.PageInfo.TotalCount;

                result.PageInfo = pageInfo;
            }

            return result;
        }

        private List<GroupBuyCatModel> MapCatList(List<GroupBuyingCategoryInfo> categoryList)
        {
            categoryList = categoryList ?? new List<GroupBuyingCategoryInfo>();
            List<GroupBuyCatModel> result = new List<GroupBuyCatModel>();
            foreach (var cat in categoryList)
            {
                GroupBuyCatModel model = new GroupBuyCatModel();
                model.CateSysNo = cat.SysNo;
                model.IsHotKey = cat.IsHotKey;
                model.Name = cat.Name;

                result.Add(model);
            }

            return result;
        }
    }
}