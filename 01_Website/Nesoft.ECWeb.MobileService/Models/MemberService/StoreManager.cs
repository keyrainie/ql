using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Entity.Category;
using Nesoft.ECWeb.Entity.Product;
using Nesoft.ECWeb.Entity.SearchEngine;
using Nesoft.ECWeb.Entity.SolrSearch;
using Nesoft.ECWeb.Entity.Store;
using Nesoft.ECWeb.Entity.Store.Filter;
using Nesoft.ECWeb.Enums;
using Nesoft.ECWeb.Facade.Member;
using Nesoft.ECWeb.Facade.Product;
using Nesoft.ECWeb.Facade.Product.Models;
using Nesoft.ECWeb.Facade.Store;
using Nesoft.ECWeb.MobileService.Models.Product;
using Nesoft.ECWeb.MobileService.Models.Search;
using Nesoft.ECWeb.UI;
using Nesoft.Utility;
using Nesoft.Utility.DataAccess.SearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.MemberService
{
    public class StoreManager
    {
        /// <summary>
        /// 获得商铺详细信息
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public StoreDetailModel GetStoreDetailInfo(int sellerSysNo)
        {

            //商家信息
            StoreBasicInfo storeinfo = StoreFacade.QueryStoreBasicInfo(sellerSysNo);
            if (storeinfo == null)
            {
                //提示商品找不到
                throw new BusinessException("商家找不到啦，请进入其他店铺，谢谢。");
            }



            //头部信息
            string header = StoreFacade.QueryStorePageHeader(storeinfo.SellerSysNo.Value);

            //导航
            QueryResult<StoreNavigation> StoreNavigation = StoreFacade.QueryStoreNavigationList(new StorePageListQueryFilter
            {
                PageIndex = 0,
                PageSize = 100
            }, storeinfo.SellerSysNo.Value);

            //店铺网页的主信息
            StorePage homeinfo = StoreFacade.QueryHomePage(storeinfo.SellerSysNo.Value);

            //获得店铺的页面,如果是预览则查询ECStore..StorePageInfo,否则查询ECStore..[PublishedStorePageInfo]
            StorePage pageinfo = StoreFacade.QueryStorePage(new StorePageFilter
            {
                SellerSysNo = storeinfo.SellerSysNo.Value,
                PublishPageSysNo = homeinfo.SysNo,
                IsPreview = false
            });

            bool IfDecorate = false;//如果按照装修元素确定店铺新品和一周排行设置为true,否则设置为false
            //店铺新品商品10个
            List<RecommendProduct> newStoreRecommendList = new List<RecommendProduct>();
            //一周排行10个
            List<RecommendProduct> WeekRankingProduct = new List<RecommendProduct>();
            if (IfDecorate)
            {
                //店铺新品
                bool NewProducts = false;
                //一周排行
                bool WeekRanking = false;
                if (pageinfo != null)
                {
                    if (pageinfo.StorePageTemplate.StorePageLayouts.Count > 0)
                    {
                        foreach (var item in pageinfo.StorePageTemplate.StorePageLayouts)
                        {
                            //店铺新品
                            if (item.StorePageElements.Exists(x => x.Key == "NewProducts"))
                            {
                                NewProducts = true;
                            }
                            //一周排行
                            if (item.StorePageElements.Exists(x => x.Key == "WeekRanking"))
                            {
                                WeekRanking = true;
                            }
                        }
                    }
                }
                if (NewProducts)
                {
                    newStoreRecommendList = StoreFacade.QueryStoreNewRecommendProduct(storeinfo.SellerSysNo.Value, null, 10, ConstValue.LanguageCode, ConstValue.CompanyCode);
                }
                
                if (WeekRanking)
                {
                    WeekRankingProduct = StoreFacade.QueryWeekRankingForCategoryCode(storeinfo.SellerSysNo.Value, null, 10, ConstValue.LanguageCode, ConstValue.CompanyCode);
                }
            }
            else
            {
                //店铺新品商品10个
                newStoreRecommendList = StoreFacade.QueryStoreNewRecommendProduct(storeinfo.SellerSysNo.Value, null, 10, ConstValue.LanguageCode, ConstValue.CompanyCode);

                //一周排行10个
                WeekRankingProduct = StoreFacade.QueryWeekRankingForCategoryCode(storeinfo.SellerSysNo.Value, null, 10, ConstValue.LanguageCode, ConstValue.CompanyCode);
            }

            StoreDetailModel result = new StoreDetailModel();
            //商铺基本信息
            result.StoreBasicInfo = Transformstoreinfo(storeinfo);

            //头部信息
            result.HeaderInfo = header;
            //导航
            if (StoreNavigation.ResultList.Count > 0)
            {
                result.StoreNavigationInfo = Transformsstorenavigation(StoreNavigation);
            }
            //店铺新品商品10个
            if (newStoreRecommendList.Count > 0)
            {
                result.StoreNewProductRecommendInfo = Transformsnewstorerecommend(newStoreRecommendList);
            }
            //一周排行10个
            if (WeekRankingProduct.Count > 0)
            {
                result.StoreWeekRankingProductInfo = TransformsWeekrankingproduct(WeekRankingProduct);
            }
            return result;
        }

        /// <summary>
        /// 获得商铺前台类别
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public List<FrontProductCategoryInfoModel> GetFrontProductCategoryByVendorSysNo(int sellerSysNo)
        {
            List<FrontProductCategoryInfo> category = ProductFacade.GetFrontProductCategoryByVendorSysNo(sellerSysNo);
            List<FrontProductCategoryInfoModel> List = EntityConverter<List<FrontProductCategoryInfo>, List<FrontProductCategoryInfoModel>>.Convert(category);
            List.ForEach(p1 =>
            {
                p1.N = (ConstValue.Product_SINGLE_STORECATE_DMSID_SEED + p1.SysNo).ToString();
                p1.Children.ForEach(p2 =>
                {
                    p2.N = (ConstValue.Product_SINGLE_STORECATE_DMSID_SEED + p2.SysNo).ToString();
                    p2.Children.ForEach(p3 =>
                    {
                        p3.N = (ConstValue.Product_SINGLE_STORECATE_DMSID_SEED + p3.SysNo).ToString();

                    });
                });
            });
            return List;
        }

        /// <summary>
        /// 店铺新品商品10个
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public List<StoreNewProductRecommendModel> QueryStoreNewRecommendProduct(int sellerSysNo)
        {
            List<RecommendProduct> newStoreRecommendList = StoreFacade.QueryStoreNewRecommendProduct(sellerSysNo, null, 10, ConstValue.LanguageCode, ConstValue.CompanyCode);
            return EntityConverter<List<RecommendProduct>, List<StoreNewProductRecommendModel>>.Convert(newStoreRecommendList);
        }

        /// <summary>
        /// 一周排行10个
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public List<StoreWeekRankingProductModel> QueryWeekRankingForCategoryCode(int sellerSysNo)
        {
            List<RecommendProduct> WeekRankingProduct = StoreFacade.QueryWeekRankingForCategoryCode(sellerSysNo, null, 10, ConstValue.LanguageCode, ConstValue.CompanyCode);
            return EntityConverter<List<RecommendProduct>, List<StoreWeekRankingProductModel>>.Convert(WeekRankingProduct);
        }

        /// <summary>
        /// 根据类别查询店铺商品
        /// </summary>
        /// <param name="criteria">搜索条件</param>
        /// <returns></returns>
        public SearchResultModel GetVendorProductByCategoryCode(int sellerSysNo, int categoryCode)
        {
            List<FrontProductCategoryInfo> category = ProductFacade.GetFrontProductCategory(sellerSysNo);
            int strCurrentCategorySysNo;
            if (category.Exists(x => x.SysNo == categoryCode))
            {
                strCurrentCategorySysNo = categoryCode;
            }
            else
            {
                var defaultCate = category.First(p => p.IsLeaf == CommonYesOrNo.Yes);
                if (defaultCate != null)
                {
                    strCurrentCategorySysNo = defaultCate.SysNo;
                }
                else
                {
                    strCurrentCategorySysNo = -1;
                }
            }
            ProductSearchCondition condition = new ProductSearchCondition();
            condition.NValueList = new List<string>();
            //前台分类
            condition.NValueList.Add((ConstValue.Product_SINGLE_STORECATE_DMSID_SEED + strCurrentCategorySysNo).ToString());
            condition.Filters = new List<FilterBase>();
            //商家编号
            condition.Filters.Add(new FieldFilter("p_sellersysno", sellerSysNo.ToString()));

            var searchResult = ProductSearchFacade.GetProductSearchResultBySolr(condition);

            SearchResultModel model = new SearchResultModel();
            model.ProductListItems = TransformResultItemList(searchResult.ProductDataList);
            model.PageInfo = MapPageInfo(searchResult.ProductDataList);
            model.Filters = MapSearchFilter(searchResult.Navigation);

            return model;
        }

        /// <summary>
        /// 店铺列表页面信息
        /// </summary>
        /// <param name="sellerSysNo">商铺编号</param>
        /// <param name="categoryCode">分类系统编号</param>
        /// <param name="sort">价格升序40、价格降序50，销量降序20</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public StoreProductListModel SearchVendorProduct(int sellerSysNo)
        {
            #region 旧代码
            //List<FrontProductCategoryInfo> category = ProductFacade.GetFrontProductCategoryByVendorSysNo(sellerSysNo);
            //List<FrontProductCategoryInfo> categoryall = ProductFacade.GetFrontProductCategory(sellerSysNo);
            //List<string> NValues = new List<string>();
            ////前台分类
            //if (category.Exists(x => x.SysNo == categoryCode))
            //{
            //    //p1一级分类
            //    FrontProductCategoryInfo p1 = category.Find(x => x.SysNo == categoryCode);
            //    if (p1.Children.Count > 0)
            //    {
            //        //p2二级分类
            //        foreach (FrontProductCategoryInfo p2 in p1.Children)
            //        {
            //            if (p2.Children.Count > 0)
            //            {
            //                foreach (FrontProductCategoryInfo p3 in p2.Children)
            //                {
            //                    NValues.Add((ConstValue.Product_SINGLE_STORECATE_DMSID_SEED + p3.SysNo).ToString());
            //                }
            //            }
            //            else
            //            {
            //                NValues.Add((ConstValue.Product_SINGLE_STORECATE_DMSID_SEED + p2.SysNo).ToString());
            //            }
            //        }
            //    }
            //    else
            //    {
            //        NValues.Add((ConstValue.Product_SINGLE_STORECATE_DMSID_SEED + p1.SysNo).ToString());
            //    }
            //}
            //else
            //{
            //    List<FrontProductCategoryInfo> p = categoryall.FindAll(c => c.IsLeaf == CommonYesOrNo.Yes);
            //    foreach (FrontProductCategoryInfo item in p)
            //    {
            //        NValues.Add((ConstValue.Product_SINGLE_STORECATE_DMSID_SEED + item.SysNo).ToString());
            //    }
            //}
            //PagedResult<ProductSearchResultItem> ProductSearch = new PagedResult<ProductSearchResultItem>();
            //foreach (var nitem in NValues)
            //{
            //    var filter = new SearchFilter();
            //    filter.SellerSysNo = sellerSysNo;
            //    SolrProductQueryVM vm = new SolrProductQueryVM();
            //    vm.IsSearchResultPage = 1;

            //    var condition = ProductSearchFacade.GetSearchCondition(vm);
            //    condition.NValueList = new List<string>();
            //    condition.NFilter = nitem;
            //    condition.NValueList.Add(nitem);

            //    condition.Filters = new List<FilterBase>();
            //    condition.Filters.Add(new FieldFilter("p_sellersysno", filter.SellerSysNo.ToString()));
            //    var searchresult = ProductSearchFacade.GetProductSearchResultBySolr(condition);
            //    if (searchresult.ProductDataList.Count > 0)
            //    {
            //        foreach (var productitem in searchresult.ProductDataList)
            //        {
            //            ProductSearch.Add(productitem);
            //        }
            //    }
            //}
            //var sfilter = new SearchFilter();
            //sfilter.SellerSysNo = sellerSysNo;
            //SolrProductQueryVM qvm = new SolrProductQueryVM();
            //qvm.IsSearchResultPage = 1;

            //var scondition = ProductSearchFacade.GetSearchCondition(qvm);
            //scondition.NValueList = new List<string>();
            //scondition.NFilter = "0";
            //scondition.NValueList.Add("0");

            //scondition.Filters = new List<FilterBase>();
            //scondition.Filters.Add(new FieldFilter("p_sellersysno", sfilter.SellerSysNo.ToString()));
            //var result = ProductSearchFacade.GetProductSearchResultBySolr(scondition);
            //result.ProductDataList = ProductSearch;
            //result.ProductDataList.PageNumber = 1;
            //result.ProductDataList.PageSize = 24;
            //result.ProductDataList.TotalRecords = ProductSearch.Count;
            //StoreProductListModel model = new StoreProductListModel();
            //model.ProductListItems = TransformResultItemList(result.ProductDataList);
            //model.PageInfo = MapPageInfo(result.ProductDataList);
            //model.Filters = MapSearchFilter(result.Navigation);

            #endregion
            var filter = new SearchFilter();
            filter.SellerSysNo = sellerSysNo;
            SolrProductQueryVM vm = new SolrProductQueryVM();
            vm.IsSearchResultPage = 1;

            var condition = ProductSearchFacade.GetSearchCondition(vm);
            condition.Filters = new List<Nesoft.Utility.DataAccess.SearchEngine.FilterBase>();
            condition.Filters.Add(new FieldFilter("p_sellersysno", filter.SellerSysNo.ToString()));
            var result = ProductSearchFacade.GetProductSearchResultBySolr(condition);
            result.FilterNavigation.NavigationItems.RemoveAll(p => p.ItemType == NavigationItemType.SubCategory || p.ItemType == NavigationItemType.Category);

            StoreProductListModel model = new StoreProductListModel();
            model.ProductListItems = TransformResultItemList(result.ProductDataList);
            model.PageInfo = MapPageInfo(result.ProductDataList);
            model.Filters = MapSearchFilter(result.Navigation);
            return model;
        }


        #region 获得商铺详细信息私有方法
        //商铺基本信息
        private StoreBasicInfoModel Transformstoreinfo(StoreBasicInfo storeinfo)
        {
            StoreBasicInfoModel result = new StoreBasicInfoModel();
            result.Address = storeinfo.Address;
            result.BrandAuthorize = storeinfo.BrandAuthorize;
            result.ContactName = storeinfo.ContactName;
            result.CooperationMode = storeinfo.CooperationMode;
            result.CurrentECChannel = storeinfo.CurrentECChannel;
            result.ECExpValue = storeinfo.ECExpValue;
            result.EditDate = storeinfo.EditDate;
            result.EditUserName = storeinfo.EditUserName;
            result.EditUserSysNo = storeinfo.EditUserSysNo;
            result.Email = storeinfo.Email;
            result.ExportExpValue = storeinfo.ExportExpValue;
            result.HaveECExp = storeinfo.HaveECExp;
            result.HaveExportExp = storeinfo.HaveExportExp;
            result.InDate = storeinfo.InDate;
            result.InUserName = storeinfo.InUserName;
            result.InUserSysNo = storeinfo.InUserSysNo;
            result.MainBrand = storeinfo.MainBrand;
            result.MainProductCategory = storeinfo.MainProductCategory;
            result.Mobile = storeinfo.Mobile;
            result.Name = storeinfo.Name;
            result.Phone = storeinfo.Phone;
            result.QQ = storeinfo.QQ;
            result.Remark = storeinfo.Remark;
            result.SellerSysNo = storeinfo.SellerSysNo;
            result.Site = storeinfo.Site;
            result.Status = storeinfo.Status;
            result.StoreName = storeinfo.StoreName;
            result.SysNo = storeinfo.SysNo;
            result.ValidDate = storeinfo.ValidDate;

            //构造商品图片
            ImageSize imageSizeMiddle = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(ImageType.Middle);
            result.LogoURL = ProductFacade.BuildProductImage(imageSizeMiddle, storeinfo.LogoURL);

            //是否被收藏
            #region 是否被收藏
            LoginUser CurrUser = UserMgr.ReadUserInfo();
            bool StoreIsWished = false;
            if (CurrUser == null || CurrUser.UserSysNo < 0)
            {
                StoreIsWished = false;
            }
            else
            {
                StoreIsWished = CustomerFacade.IsMyFavoriteSeller(CurrUser.UserSysNo, storeinfo.SellerSysNo.Value);
            }
            #endregion
            result.StoreIsWished = StoreIsWished;
            return result;
        }
        //导航
        private List<StoreNavigationModel> Transformsstorenavigation(QueryResult<StoreNavigation> storeNavigation)
        {
            List<StoreNavigationModel> result = new List<StoreNavigationModel>();
            foreach (var p in storeNavigation.ResultList)
            {
                StoreNavigationModel item = new StoreNavigationModel();
                item.CompanyCode = p.CompanyCode;
                item.Content = p.Content;
                item.EditDate = p.EditDate;
                item.EditDateStr = p.EditDateStr;
                item.EditUserName = p.EditUserName;
                item.EditUserSysNo = p.EditUserSysNo;
                item.InDate = p.InDate;
                item.InDateStr = p.InDateStr;
                item.InUserName = p.InUserName;
                item.InUserSysNo = p.InUserSysNo;
                item.LanguageCode = p.LanguageCode;
                item.LinkUrl = p.LinkUrl;
                item.Priority = p.Priority;
                item.Status = p.Status;
                item.StoreCompanyCode = p.StoreCompanyCode;
                item.SysNo = p.SysNo;
                result.Add(item);
            }
            return result;
        }
        //店铺新品商品10个
        private List<StoreNewProductRecommendModel> Transformsnewstorerecommend(List<RecommendProduct> newStoreRecommendList)
        {
            List<StoreNewProductRecommendModel> result = new List<StoreNewProductRecommendModel>();
            foreach (var p in newStoreRecommendList)
            {
                StoreNewProductRecommendModel item = new StoreNewProductRecommendModel();
                item.CompanyCode = p.CompanyCode;
                item.EditDate = p.EditDate;
                item.EditDateStr = p.EditDateStr;
                item.EditUserName = p.EditUserName;
                item.EditUserSysNo = p.EditUserSysNo;
                item.InDate = p.InDate;
                item.InDateStr = p.InDateStr;
                item.InUserName = p.InUserName;
                item.InUserSysNo = p.InUserSysNo;
                item.LanguageCode = p.LanguageCode;
                item.StoreCompanyCode = p.StoreCompanyCode;

                item.SysNo = p.SysNo;
                item.BasicPrice = p.BasicPrice;
                item.BrandSysNo = p.BrandSysNo;
                item.BriefName = p.BriefName;
                item.CashRebate = p.CashRebate;
                item.ProductID = p.ProductID;
                item.Priority = p.Priority;
                item.ProductName = p.ProductName;
                item.ProductTitle = p.ProductTitle;
                item.CurrentPrice = p.CurrentPrice;
                item.Discount = p.Discount;
                item.TariffRate = p.TariffRate;
                item.Description = p.Description;
                item.PromotionTitle = p.PromotionTitle;
                item.RealPrice = p.RealPrice;
                item.TariffPrice = p.TariffPrice;
                //构造商品图片
                ImageSize imageSizeMiddle = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(ImageType.Middle);
                item.DefaultImage = ProductFacade.BuildProductImage(imageSizeMiddle, p.DefaultImage);

                result.Add(item);
            }
            return result; 
        }
        //一周排行10个
        private List<StoreWeekRankingProductModel> TransformsWeekrankingproduct(List<RecommendProduct> weekRankingProduct)
        {
            List<StoreWeekRankingProductModel> result = new List<StoreWeekRankingProductModel>();
            foreach (var p in weekRankingProduct)
            {
                StoreWeekRankingProductModel item = new StoreWeekRankingProductModel();
                item.CompanyCode = p.CompanyCode;
                item.EditDate = p.EditDate;
                item.EditDateStr = p.EditDateStr;
                item.EditUserName = p.EditUserName;
                item.EditUserSysNo = p.EditUserSysNo;
                item.InDate = p.InDate;
                item.InDateStr = p.InDateStr;
                item.InUserName = p.InUserName;
                item.InUserSysNo = p.InUserSysNo;
                item.LanguageCode = p.LanguageCode;
                item.StoreCompanyCode = p.StoreCompanyCode;

                item.SysNo = p.SysNo;
                item.BasicPrice = p.BasicPrice;
                item.BrandSysNo = p.BrandSysNo;
                item.BriefName = p.BriefName;
                item.CashRebate = p.CashRebate;
                item.ProductID = p.ProductID;
                item.Priority = p.Priority;
                item.ProductName = p.ProductName;
                item.ProductTitle = p.ProductTitle;
                item.CurrentPrice = p.CurrentPrice;
                item.Discount = p.Discount;
                item.TariffRate = p.TariffRate;
                item.Description = p.Description;
                item.PromotionTitle = p.PromotionTitle;
                item.RealPrice = p.RealPrice;
                item.TariffPrice = p.TariffPrice;
                //构造商品图片
                ImageSize imageSizeMiddle = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(ImageType.Middle);
                item.DefaultImage = ProductFacade.BuildProductImage(imageSizeMiddle, p.DefaultImage);
                result.Add(item);
            }
            return result;
        }
        #endregion

        #region 根据类别查询店铺商品信息私有方法
        private List<ProductItemModel> TransformResultItemList(PagedResult<ProductSearchResultItem> solrItemList)
        {
            List<ProductItemModel> result = new List<ProductItemModel>();
            ImageSize imageSizeItemList = Nesoft.ECWeb.MobileService.Models.Product.ImageUrlHelper.GetImageSize(ImageType.Middle);
            foreach (var item in solrItemList)
            {
                ProductItemModel model = new ProductItemModel();
                //基本信息
                model.ID = item.ProductSysNo;
                model.Code = item.ProductID;
                model.ProductTitle = item.ProductDisplayName;
                model.PromotionTitle = item.PromotionTitle;
                model.ImageUrl = ProductFacade.BuildProductImage(imageSizeItemList, item.ProductDefaultImage);

                //价格相关信息
                SalesInfoModel salesInfo = new SalesInfoModel();
                salesInfo.BasicPrice = item.MarketPrice;
                salesInfo.CurrentPrice = item.SalesPrice;
                salesInfo.CashRebate = item.CashRebate;
                salesInfo.TariffPrice = item.ProductTariffAmt;
                salesInfo.FreeEntryTax = item.ProductTariffAmt <= ConstValue.TariffFreeLimit;
                salesInfo.TotalPrice = item.TotalPrice;
                //赠送积分数量
                salesInfo.PresentPoint = item.Point;
                salesInfo.OnlineQty = item.OnlineQty;
                salesInfo.IsHaveValidGift = item.IsHaveValidGift;
                salesInfo.IsCountDown = item.IsCountDown;
                salesInfo.IsNewProduct = item.IsNewproduct;
                salesInfo.IsGroupBuyProduct = item.IsGroupBuyProduct;
                model.SalesInfo = salesInfo;

                //其它信息
                model.ReviewCount = item.ReviewCount;
                model.ReviewScore = item.ReviewScore;

                result.Add(model);
            }

            return result;
        }

        private PageInfo MapPageInfo(PagedResult<ProductSearchResultItem> solrItemList)
        {
            var pageInfo = new PageInfo();
            pageInfo.PageIndex = solrItemList.PageNumber > solrItemList.TotalPages ? solrItemList.TotalPages : solrItemList.PageNumber;
            pageInfo.PageSize = solrItemList.PageSize;
            pageInfo.TotalCount = solrItemList.TotalRecords;

            return pageInfo;
        }

        private List<SearchFilterModel> MapSearchFilter(NavigationContainer navigation)
        {
            List<SearchFilterModel> result = new List<SearchFilterModel>();
            if (navigation != null && navigation.NavigationItems != null)
            {
                foreach (var navItem in navigation.NavigationItems)
                {
                    //如果没有筛选项或数量过少就丢弃
                    if (navItem.SubNavigationItems == null
                            || navItem.SubNavigationItems.Count == 0)
                    {
                        continue;
                    }

                    SearchFilterModel filterModel = new SearchFilterModel();
                    filterModel.ID = (int)navItem.ItemType;
                    filterModel.Name = navItem.Name;

                    foreach (var subItem in navItem.SubNavigationItems)
                    {
                        SearchFilterItemModel itemModel = new SearchFilterItemModel();
                        itemModel.EnId = subItem.Value;
                        itemModel.Name = subItem.Name;
                        itemModel.ProductCount = subItem.NumberOfItem;
                        filterModel.Items.Add(itemModel);
                    }

                    result.Add(filterModel);
                }
            }

            return result;
        }
        #endregion
    }
}