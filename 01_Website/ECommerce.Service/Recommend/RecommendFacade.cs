using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.Recommend;
using ECommerce.Entity;
using ECommerce.Entity.Category;
using ECommerce.Enums;
using ECommerce.Facade.Common;
using System.Web;
using System.Web.Caching;
using ECommerce.DataAccess.Category;
using ECommerce.Facade.Product;
using ECommerce.Entity.Product;
using System.IO;
using System.Xml.Serialization;
using ECommerce.Entity.Order;
using ECommerce.WebFramework;
using ECommerce.Entity.Recommend;
using ECommerce.Entity.Product.Review;
using ECommerce.DataAccess.Product;

namespace ECommerce.Facade.Recommend
{
    public class RecommendFacade
    {
        /// <summary>
        /// 获取推荐商品
        /// </summary>
        /// <param name="pageID">类别sysno/其他sysno</param>
        /// <param name="pageType">页面类型,如:一级类别页面,二级类别页等</param>
        /// <param name="posID">页面位置</param>
        /// <param name="count">数量</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryRecommendProduct(int pageID,
            int pageType,
            int posID,
            int count,
            string languageCode = "zh-CN",
            string companyCode = "8601")
        {

            string cacheKey = CommonFacade.GenerateKey("QueryRecommendProduct", pageType.ToString(), pageID.ToString()
                , posID.ToString(), count.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            List<RecommendProduct> result = RecommendDA.QueryRecommendProduct(pageID, pageType, posID, count, languageCode, companyCode);

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }

        /// <summary>
        /// 获得推荐位的新上架商品
        /// </summary>
        /// <param name="count"></param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryNewRecommendProduct(
            int count,
            string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryNewRecommendProduct", languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            List<RecommendProduct> result = RecommendDA.QueryNewRecommendProduct(count, languageCode, companyCode);

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }







        /// <summary>
        /// 一级类别新品上架
        /// </summary>
        /// <param name="c1SysNo">前台一级类别</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryNewProductForC1(int c1SysNo, string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryNewProductForC1", c1SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 4;
            var p1 = QueryRecommendProduct(c1SysNo, (int)PageType.TabStore,
                (int)RecommendPosition.TabStore_NewProduct, count, languageCode, companyCode);
            if (p1.Count < count)
            {
                var p2 = RecommendDA.QueryNewProductForC1(c1SysNo, languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }

        /// <summary>
        /// 一级类别超级特惠(推荐位补位)
        /// </summary>
        /// <param name="c1SysNo">前台一级类别</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QuerySuperSpecialProductForC1(int c1SysNo, string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QuerySuperSpecialProductForC1", c1SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 4;
            var p1 = QueryRecommendProduct(c1SysNo, (int)PageType.TabStore,
                (int)RecommendPosition.TabStore_SuperSpecial, count, languageCode, companyCode);
            if (p1.Count < count)
            {
                var p2 = RecommendDA.QuerySuperSpecialProductForC1(c1SysNo, languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);


            return result;
        }

        /// <summary>
        /// 一级类别热销商品(推荐位补位);
        /// </summary>
        /// <param name="c1SysNo">前台一级类别</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryHotProductForC1(int c1SysNo, string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryHotProductForC1", c1SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 3;
            var p1 = QueryRecommendProduct(c1SysNo, (int)PageType.TabStore,
                (int)RecommendPosition.TabStore_HotProduct, count, languageCode, companyCode);
            if (p1.Count < count)
            {
                var p2 = RecommendDA.QueryHotProductForC1(c1SysNo, languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }

        /// <summary>
        /// 二级类别热销商品(推荐位补位);
        /// </summary>
        /// <param name="c1SysNo">前台二级类别</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryHotProductForC2(int c2SysNo, string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryHotProductForC2", c2SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 3;
            var p1 = QueryRecommendProduct(c2SysNo, (int)PageType.MidCategory,
                (int)RecommendPosition.MidCategory_ThisWeekProduct, count, languageCode, companyCode);
            if (p1.Count < count)
            {
                var p2 = RecommendDA.QueryHotProductForC2(c2SysNo, languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }

        /// <summary>
        /// 三级类别热卖推荐(推荐位补位)
        /// </summary>
        /// <param name="c3CategoryID">前台三级类别</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryHotProductForC3(int c3CategoryID, string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryHotProductForC3", c3CategoryID.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 5;
            var p1 = QueryRecommendProduct(c3CategoryID, (int)PageType.SubStore,
                (int)RecommendPosition.SubStore_HotSale, count, languageCode, companyCode);
            if (p1.Count < count)
            {
                var p2 = RecommendDA.QueryHotProductForC3(c3CategoryID, languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }

        /// <summary>
        /// 购物车热卖推荐（推荐位补位）
        /// </summary>
        /// <returns></returns>
        public static List<RecommendProduct> QueryShoppingCartList()
        {

            string cacheKey = CommonFacade.GenerateKey("QueryShoppingCartList");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 4;
            var p1 = RecommendFacade.QueryRecommendProduct(0, (int)PageType.ShoppingCart, 5001, count);
            if (p1.Count < count)
            {
                var p2 = RecommendDA.QueryNewProduct(ConstValue.LanguageCode, ConstValue.CompanyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }

        #region 一级类别--一周排行榜

        /// <summary>
        /// (已缓存)查询最近7天下单最多的前5个商品
        /// </summary>
        /// <param name="c1SysNo">前台一级类别sysno</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryWeekRankingForC1(string c1SysNo, string languageCode = "zh-CN", string companyCode = "8601")
        {

            string cacheKey = CommonFacade.GenerateKey("QueryWeekRankingForC1", c1SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            const int count = 8;
            var p1 = CategoryDA.QueryWeekRankingForC1(Convert.ToInt32(c1SysNo));
            if (p1.Count < 8)
            {
                var p2 = RecommendDA.QuerySuperSpecialProductForC1(Convert.ToInt32(c1SysNo), languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }
        #endregion

        #region 二级类别--一周排行榜

        /// <summary>
        /// (已缓存)查询最近7天下单最多的前5个商品
        /// </summary>
        /// <param name="c2SysNo">前台二级类别sysno</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryWeekRankingForC2(int c2SysNo, string languageCode = "zh-CN", string companyCode = "8601")
        {

            string cacheKey = CommonFacade.GenerateKey("QueryWeekRankingForC2", c2SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            const int count = 8;
            var p1 = CategoryDA.QueryWeekRankingForC2(Convert.ToInt32(c2SysNo));
            if (p1.Count < 8)
            {
                var p2 = RecommendDA.QuerySuperSpecialProductForC2(Convert.ToInt32(c2SysNo), languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }
        #endregion

        #region 三级类别--一周排行榜

        /// <summary>
        /// (已缓存)查询最近7天下单最多的前5个商品
        /// </summary>
        /// <param name="c3SysNo">前台三级类别sysno</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryWeekRankingForC3(int c3SysNo, string languageCode = "zh-CN", string companyCode = "8601")
        {

            string cacheKey = CommonFacade.GenerateKey("QueryWeekRankingForC3", c3SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            const int count = 8;
            var p1 = CategoryDA.QueryWeekRankingForC3(Convert.ToInt32(c3SysNo));
            if (p1.Count < 8)
            {
                var p2 = RecommendDA.QuerySuperSpecialProductForC3(Convert.ToInt32(c3SysNo), languageCode, companyCode);
                p2.ForEach(p =>
                {
                    if (p1.All(f => f.SysNo != p.SysNo))
                    {
                        p1.Add(p);
                    }
                });
            }
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }
        #endregion

        public static List<RecommendProduct> QueryWeeklyHotProduct(string languageCode = "zh-CN", string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryWeeklyHotProduct");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var result = RecommendDA.QueryWeeklyHotProduct(languageCode, companyCode);
            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }


        #region 首页楼层
        /// <summary>
        /// 获取楼层信息
        /// </summary>
        /// <param name="pageCode"></param>
        /// <returns></returns>
        public static List<FloorEntity> GetFloorInfo(PageCodeType pageType, int pageCode)
        {
            string cacheKey = CommonFacade.GenerateKey("GetFloorInfo", pageType.ToString(), pageCode.ToString());
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<FloorEntity>)HttpRuntime.Cache[cacheKey];
            }
            #region 获取信息

            //获取基本信息
            List<FloorEntity> floorEntitys = RecommendDA.GetFloorInfo(pageType, pageCode);

            //获取 Tab 信息
            foreach (FloorEntity entity in floorEntitys)
            {
                //获取 Tab 信息
                entity.FloorSections = RecommendDA.GetFloorSections(entity.FloorSysNo);
                List<FloorItemBase> originalItem = RecommendDA.GetFloorSectionItems(entity.FloorSysNo);

                entity.FloorSectionItems = new List<FloorItemBase>();
                //转换XML信息 to 实体
                foreach (FloorItemBase item in originalItem)
                {
                    FloorItemBase objItem = null;
                    switch (item.ItemType)
                    {
                        case FloorItemType.Product:

                            FloorItemProduct entityProduct = CreateObject<FloorItemProduct>(item.ItemValue);
                            ProductItemInfo productInfo = ProductFacade.GetProductShortInfoBySysNo(entityProduct.ProductSysNo);
                            if (productInfo != null && productInfo.Status == 1)
                            {
                                entityProduct.DefaultImage = productInfo.DefaultImage;
                                entityProduct.RealPrice = productInfo.RealPrice;
                                entityProduct.BasicPrice = productInfo.MarketPrice;
                                entityProduct.ProductPrice = productInfo.CurrentPrice;
                                entityProduct.PromotionTitle = productInfo.PromotionTitle;
                                entityProduct.ProductSubTitle = productInfo.PromotionTitle;
                                entityProduct.CashRebate = productInfo.CashRebate;
                                entityProduct.ProductTitle = productInfo.ProductTitle;
                                objItem = entityProduct;
                            }
                            break;

                        case FloorItemType.Banner:
                            objItem = CreateObject<FloorItemBanner>(item.ItemValue);
                            break;

                        case FloorItemType.Brand:
                            objItem = CreateObject<FloorItemBrand>(item.ItemValue);
                            break;

                        case FloorItemType.TextLink:
                            objItem = CreateObject<FloorItemTextLink>(item.ItemValue);
                            break;
                    }

                    if (objItem != null)
                    {
                        objItem.CloneFloorItemBase(item);
                        entity.FloorSectionItems.Add(objItem);
                    }
                }
            }
            #endregion


            HttpRuntime.Cache.Insert(cacheKey, floorEntitys, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return floorEntitys;
        }

        /// <summary>
        /// 转换XML数据
        /// </summary>
        /// <typeparam name="T">需要转换的类</typeparam>
        /// <param name="xmlText">XML字符串信息</param>
        /// <returns>返回的对象</returns>
        public static T CreateObject<T>(string xmlText) where T : class, new()
        {
            T objectEntity = new T();

            using (StringReader strReader = new StringReader(xmlText))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(T));
                objectEntity = xmls.Deserialize(strReader) as T;
            }

            return objectEntity;
        }
        #endregion

        #region Banner

        public static List<BannerInfo> GetBannerInfoByPositionID(int pageID, PageType pageType, BannerPosition positionID)
        {
            string cacheKey = CommonFacade.GenerateKey("GetBannerInfoByPositionID", pageType.ToString(), pageID.ToString(), positionID.ToString());
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<BannerInfo>)HttpRuntime.Cache[cacheKey];
            }

            List<BannerInfo> bannerInfoList = RecommendDA.GetBannerInfoByPositionID(pageID, pageType, positionID);

            HttpRuntime.Cache.Insert(cacheKey, bannerInfoList, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);
            return bannerInfoList;
        }

        /// <summary>
        /// 获取首页所有相关Banner
        /// </summary>
        /// <returns></returns>
        public static HomBannersVM GetHomeBanners(int sliderCount = 5, int middleItemCount = 5, int bottomItemCount = 5)
        {
            PageType pageType = PageType.Home;
            int pageID = 0;
            string cacheKey = CommonFacade.GenerateKey("GetBannerInfoOfHome", pageType.ToString(), pageID.ToString());
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (HomBannersVM)HttpRuntime.Cache[cacheKey];
            }

            List<BannerInfo> bannerInfoList = RecommendDA.GetBannerInfoByPositionID(pageID, pageType, null);
            HomBannersVM result = new HomBannersVM();
            //首页轮播广告
            result.Sliders = bannerInfoList.Where(item => item.PositionID == BannerPosition.HomePage_Top_Slider).Take(sliderCount).ToList();

            //新闻公告下部
            result.UnderNews = bannerInfoList.FirstOrDefault(item => item.PositionID == BannerPosition.HomePage_UnderNews);

            //首页底部精选品牌
            result.Bottom = bannerInfoList.Where(item => item.PositionID == BannerPosition.HomePage_Bottom).Take(bottomItemCount).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);
            return result;
        }
        #endregion

        public static List<RecommendProduct> QueryGroupBuyingRecommendProductList()
        {
            string cacheKey = CommonFacade.GenerateKey("QueryGroupBuyingRecommendProductList");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 8;
            var p1 = RecommendFacade.QueryRecommendProduct(0, (int)PageType.GroupBuying, (int)RecommendPosition.GroupBuy_HotSalesProduct, count);
            //泰隆银行商品很少，可能有些分类下面就没有商品，这样补位 就会出现 有时能补上，有时补不上；这样还照常客户困惑和质疑；所有暂时去掉补位逻辑
            //if (p1.Count < count)
            //{
            //    Random rand = new Random();
            //    var allCategory = CategoryFacade.QueryCategoryInfosForHomePage();
            //    var c1SysNoList = allCategory.FindAll(x => x.CategoryType == CategoryType.TabStore).Select(x => x.CategoryID).ToList();
            //    int c1SysNo = c1SysNoList[rand.Next(0, c1SysNoList.Count)];
            //    var p2 = RecommendDA.QueryHotProductForC1(c1SysNo, ConstValue.LanguageCode, ConstValue.CompanyCode);
            //    p2.ForEach(p =>
            //    {
            //        if (p1.All(f => f.SysNo != p.SysNo))
            //        {
            //            p1.Add(p);
            //        }
            //    });
            //}
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }

        public static List<UICustomerReviewInfo> GetLatestReviewInfos(int pageSize)
        {
            var result = ReviewDA.GetHomePageHotReview(pageSize);
            if (result.Count < pageSize)
            {
                //补位
                var otherGoodReviewList = ReviewDA.GetGoodReview(pageSize);
                foreach (var item in otherGoodReviewList)
                {
                    if (result.Count < pageSize && !result.Exists(review => review.SysNo == item.SysNo))
                    {
                        result.Add(item);
                    }
                }
            }

            return result.OrderByDescending(item => item.InDate).ToList();
        }

        #region 推荐品牌

        /// <summary>
        /// 获取推荐品牌
        /// </summary>
        /// <returns></returns>
        public static List<RecommendBrandInfo> GetRecommendBrands(PageType pageType, int pageID)
        {
            string cacheKey = CommonFacade.GenerateKey("GetRecommendBrands", pageType.ToString(), pageID.ToString());
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendBrandInfo>)HttpRuntime.Cache[cacheKey];
            }
            List<RecommendBrandInfo> result = RecommendDA.GetRecommendBrands(pageType, pageID);
            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);
            return result;
        }
        #endregion

        //热卖商品
        public static List<RecommendProduct> QueryHotSalesProductForC1(int c1SysNo, string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryHotSalesProductForC1", c1SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 6;
            var p1 = QueryRecommendProduct(c1SysNo, (int)PageType.TabStore,
                (int)RecommendPosition.TabStore_HotSalesProduct, count, languageCode, companyCode);

            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }


        //热卖商品
        public static List<RecommendProduct> QueryHotSalesProductForC2(int c2SysNo, string languageCode = "zh-CN",
            string companyCode = "8601")
        {
            string cacheKey = CommonFacade.GenerateKey("QueryHotSalesProductForC2", c2SysNo.ToString(), languageCode, companyCode);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<RecommendProduct>)HttpRuntime.Cache[cacheKey];
            }

            var count = 6;
            var p1 = QueryRecommendProduct(c2SysNo, (int)PageType.MidCategory,
                (int)RecommendPosition.MidCategory_HotSalesProduct, count, languageCode, companyCode);
            List<RecommendProduct> result = p1.Take(count).ToList();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Long), Cache.NoSlidingExpiration);

            return result;
        }
    }
}
