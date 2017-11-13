using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using ECommerce.DataAccess.Category;
using ECommerce.DataAccess.Product;
using ECommerce.Entity;
using ECommerce.Entity.Category;
using ECommerce.Entity.Product;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Shipping;
using ECommerce.Enums;
using ECommerce.Facade.Product.Models;
using ECommerce.WebFramework;
using ECommerce.Utility;

namespace ECommerce.Facade.Product
{
    public static partial class ProductFacade
    {

        private static readonly string MY_BROWSEHISTORY_COOKIE_NAME = "NearestBrowseList";
        private static readonly int WarrantyForeverDay = 29970;
        private static readonly int WarrantyDay = 45;
        private static readonly int WarrantyMonth = 30;
        private static readonly int CMaxCompareNumber = 4;
        private static readonly string CProperty = "Property";
        private static readonly string CKey = "Key";
        private static readonly string CValue = "Value";
        /// <summary>
        /// 商品基本信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductBasicInfo GetProductBasicInfoBySysNo(int productSysNo)
        {
            string GetProductBasicInfoBySysNoCachKey = "GetProductBasicInfoBySysNoCachKey_" + productSysNo;
            if (HttpRuntime.Cache[GetProductBasicInfoBySysNoCachKey] != null)
            {
                return (ProductBasicInfo)HttpRuntime.Cache[GetProductBasicInfoBySysNoCachKey];
            }
            ProductBasicInfo info = ProductDA.GetProductBasicInfoBySysNo(productSysNo);
            if (info != null)
            {
                HttpRuntime.Cache.Insert(GetProductBasicInfoBySysNoCachKey, info, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            }
            return info;
        }

        /// <summary>
        /// 根据SysNo获取商家前台类别
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public static List<FrontProductCategoryInfo> GetFrontProductCategoryByVendorSysNo(int sellerSysNo)
        {
            var allCategory = ProductDA.GetFrontProductCategory(sellerSysNo);
            allCategory.RemoveAll(p => p.Status == 0);

            var result = new List<FrontProductCategoryInfo>();

            //1
            result.AddRange(allCategory.FindAll(p => string.IsNullOrEmpty(p.ParentCategoryCode)));
            allCategory.RemoveAll(p => string.IsNullOrEmpty(p.ParentCategoryCode));

            //2
            result.ForEach(p =>
            {
                p.Children = new List<FrontProductCategoryInfo>();
                p.Children.AddRange(allCategory.FindAll(q => q.ParentCategoryCode.Trim() == p.CategoryCode.Trim()));
                //3
                p.Children.ForEach(pp =>
                {
                    pp.Children = new List<FrontProductCategoryInfo>();
                    pp.Children.AddRange(allCategory.FindAll(qq => qq.ParentCategoryCode.Trim() == pp.CategoryCode.Trim()));
                });
            });

            return result;
        }

        /// <summary>
        /// 根据SysNo获取商家前台类别
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public static List<FrontProductCategoryInfo> GetFrontProductCategory(int sellerSysNo)
        {
            var allCategory = ProductDA.GetFrontProductCategory(sellerSysNo);
            allCategory.RemoveAll(p => p.Status == 0);
            return allCategory;
        }
        /// <summary>
        /// 获取商品简要信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductItemInfo GetProductShortInfoBySysNo(int productSysNo)
        {
            return ProductDA.GetProductShortInfoBySysNo(productSysNo);
        }

        /// <summary>
        /// 商品销售信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductSalesInfo GetProductSalesInfoBySysNo(int productSysNo)
        {
            return ProductDA.GetProductSalesInfoBySysNo(productSysNo);
        }

        /// <summary>
        /// 商品配送方式
        /// </summary>
        /// <param name="productSysno"></param>
        /// <returns></returns>
        public static ShipTypeInfo GetProductShippingInfo(int productSysno)
        {
            return ProductDA.GetProductShippingInfo(productSysno);
        }

        /// <summary>
        /// 商品组图片
        /// </summary>
        /// <param name="productCommonInfoSysNo"></param>
        /// <returns></returns>
        public static List<ProductImage> GetProductImages(int productCommonInfoSysNo)
        {
            string GetProductImagesListCachKey = "GetProductImagesListCachKey_" + productCommonInfoSysNo;
            if (HttpRuntime.Cache[GetProductImagesListCachKey] != null)
            {
                return (List<ProductImage>)HttpRuntime.Cache[GetProductImagesListCachKey];
            }
            ProductInfoFilter filter = new ProductInfoFilter()
            {
                ProductCommonInfoSysNo = productCommonInfoSysNo,
                CompanyCode = ConstValue.CompanyCode,
                LaguageCode = ConstValue.LanguageCode,
                StoreCompanyCode = ConstValue.StoreCompanyCode
            };
            List<ProductImage> list = ProductDA.GetProductGroupImageList(filter);
            if (list != null && list.Count > 0)
            {
                HttpRuntime.Cache.Insert(GetProductImagesListCachKey, list, null, DateTime.Now.AddSeconds(CacheTime.Middle), Cache.NoSlidingExpiration);
            }
            return list;
        }

        /// <summary>
        ///商品属性组相关信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductPropertyView> GetProductPropetyView(int productSysNo, int productCommonSysNo)
        {
            List<ProductPropertyListInfo> list = GetProductPropety(productSysNo, productCommonSysNo);

            if (list != null && list.Count > 0)
            {
                ProductPropertyInfo current = null;

                if (list[0].PropertyList != null)
                {
                    current = list[0].PropertyList.Find(delegate(ProductPropertyInfo find)
                     {
                         return find.ProductSysNo == productSysNo;
                     });
                }

                if (current == null)
                {
                    return null;
                }

                List<ProductPropertyView> viewList = new List<ProductPropertyView>();

                ProductPropertyView productPropertyView = new ProductPropertyView();
                productPropertyView.Current = current;
                productPropertyView.Type = 1;
                productPropertyView.ProductList = list[0].PropertyList;
                viewList.Add(productPropertyView);

                if (list.Count > 1)
                {
                    productPropertyView = new ProductPropertyView();
                    productPropertyView.Current = current;
                    productPropertyView.Type = 2;
                    productPropertyView.ProductList = list[1].PropertyList;
                    viewList.Add(productPropertyView);
                }

                return viewList;
            }
            return null;
        }

        /// <summary>
        /// 商品内容
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        public static List<ProductContent> GetProductContentList(ProductBasicInfo productInfo)
        {
            if (productInfo == null) return null;
            string productContentListCachKey = "productContentListCachKey_" + productInfo.ID;
            if (HttpRuntime.Cache[productContentListCachKey] != null)
            {
                return (List<ProductContent>)HttpRuntime.Cache[productContentListCachKey];
            }

            List<ProductContent> contentList = new List<ProductContent>();
            contentList.Add(new ProductContent() { ContentType = ProductContentType.Detail, Content = productInfo.ProductDescLong + productInfo.ProductPhotoDesc });
            contentList.Add(new ProductContent() { ContentType = ProductContentType.Performance, Content = BuildProductPerformanceToHtml(productInfo.Performance) });
            contentList.Add(new ProductContent() { ContentType = ProductContentType.Attention, Content = productInfo.Attention });
            //contentList.Add(new ProductContent() { ContentType = ProductContentType.Warranty, Content = productInfo.Warranty });

            HttpRuntime.Cache.Insert(productContentListCachKey, contentList, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            return contentList;
        }


        /// <summary>
        /// 商品附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductItemInfo> GetProductAttachmentList(int productSysNo)
        {
            string GetProductAttachmentListCachKey = "GetProductAttachmentListCachKey_" + productSysNo;
            if (HttpRuntime.Cache[GetProductAttachmentListCachKey] != null)
            {
                return (List<ProductItemInfo>)HttpRuntime.Cache[GetProductAttachmentListCachKey];
            }
            ProductInfoFilter filter = new ProductInfoFilter()
            {
                ProductSysNo = productSysNo,
                CompanyCode = ConstValue.CompanyCode,
                LaguageCode = ConstValue.LanguageCode,
                StoreCompanyCode = ConstValue.StoreCompanyCode
            };
            List<ProductItemInfo> list = ProductDA.GetProductAttachmentList(filter);
            if (list != null && list.Count > 0)
            {
                HttpRuntime.Cache.Insert(GetProductAttachmentListCachKey, list, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            }
            return list;
        }

        /// <summary>
        /// 写商品浏览记录，只保留最近的20个浏览记录
        /// </summary>
        public static void SetProductBrowseHistory(int productSysNo)
        {
            List<ProductBrowseHistory> browseHistoryList = CookieHelper.GetCookie<List<ProductBrowseHistory>>(MY_BROWSEHISTORY_COOKIE_NAME);
            if (browseHistoryList == null)
                browseHistoryList = new List<ProductBrowseHistory>();
            if (browseHistoryList.Exists(m => m.ProductSysNo.Equals(productSysNo)))
                browseHistoryList = browseHistoryList.FindAll(m => !m.ProductSysNo.Equals(productSysNo));
            browseHistoryList.Add(new ProductBrowseHistory()
            {
                ProductSysNo = productSysNo,
                ViewTime = DateTime.Now.ToString()
            });
            var orderByDescHistoryList = from r in browseHistoryList orderby r.ViewTime descending select r;
            browseHistoryList = orderByDescHistoryList.Take(20).ToList();
            CookieHelper.SaveCookie<List<ProductBrowseHistory>>(MY_BROWSEHISTORY_COOKIE_NAME, browseHistoryList);
        }

        /// <summary>
        /// 读取最近浏览商品记录列表
        /// </summary>
        /// <param name="top">获取最近top条</param>
        /// <returns></returns>
        public static List<ProductItemInfo> GetProductBrowseHistory(int top)
        {
            List<ProductBrowseHistory> browseHistoryList = CookieHelper.GetCookie<List<ProductBrowseHistory>>(MY_BROWSEHISTORY_COOKIE_NAME);
            if (browseHistoryList == null || browseHistoryList.Count == 0)
                return null;

            List<string> querySysNos = new List<string>();
            foreach (ProductBrowseHistory item in browseHistoryList)
            {
                querySysNos.Add(item.ProductSysNo.ToString());
            }
            List<ProductItemInfo> productList = ProductDA.GetProductBrowseHistoryInfoBySysNos(querySysNos);
            if (productList == null || productList.Count == 0)
                return null;

            //按时间顺序处理
            List<ProductItemInfo> result = new List<ProductItemInfo>();
            var orderByDescHistoryList = from r in browseHistoryList orderby r.ViewTime descending select r;
            browseHistoryList = orderByDescHistoryList.ToList();
            foreach (ProductBrowseHistory item in browseHistoryList)
            {
                ProductItemInfo currProduct = productList.Find(m => m.ID.Equals(item.ProductSysNo));
                if (currProduct != null)
                    result.Add(currProduct);
                if (result.Count.Equals(top))
                    break;
            }
            return result;
        }


        /// <summary>
        /// 获取商品相关类别
        /// </summary>
        /// <param name="c3SysNo">商品的C3SysNo</param>
        /// <returns></returns>
        public static List<CategoryInfo> GetProductRelatedCategoryList(int c3SysNo)
        {

            List<CategoryInfo> allCategories = null;
            //缓存所有类别
            string allCategoriesCachKey = "ProductDetail_ALLCategoriesCachKey";
            if (HttpRuntime.Cache[allCategoriesCachKey] != null)
            {
                allCategories = (List<CategoryInfo>)HttpRuntime.Cache[allCategoriesCachKey];
            }
            else
            {
                allCategories = CategoryDA.QueryCategories();

            }
            //与c3SysNo同级的三级类别
            List<CategoryInfo> subCategoryInfos = allCategories.FindAll(f =>
            {
                return f.CategoryType == CategoryType.SubCategory && f.C3Sysno == c3SysNo;
            });

            List<CategoryInfo> categoryInfos = allCategories.FindAll(f =>
           {
               return subCategoryInfos.Exists(t =>
              {
                  return t.ParentSysNo == f.CurrentSysNo;
              });
           });

            if (categoryInfos == null || categoryInfos.Count == 0)
            {
                return null;
            }

            List<CategoryInfo> subCategoryList = allCategories.FindAll(f =>
            {
                return categoryInfos.Exists(t =>
               {
                   return (f.ParentSysNo == t.CurrentSysNo) && (f.CategoryType == CategoryType.SubCategory);
               });
            });

            subCategoryList = subCategoryList.FindAll(f =>
           {
               return !subCategoryList.Exists(t =>
              {
                  return (t.CategoryID == f.CategoryID) && (f.CurrentSysNo > t.CurrentSysNo);
              });
           });
            if (allCategories != null && allCategories.Count > 0)
            {
                HttpRuntime.Cache.Insert(allCategoriesCachKey, allCategories, null, DateTime.Now.AddSeconds(CacheTime.Middle), Cache.NoSlidingExpiration);
            }
            return subCategoryList;
        }

        /// <summary>
        /// 商品相关品牌
        /// </summary>
        /// <returns></returns>
        public static List<BrandInfo> GetProductRelatedBrandInfo(int productSysNo)
        {
            string GetProductRelatedBrandInfoCachKey = "GetProductRelatedBrandInfo_" + productSysNo;
            if (HttpRuntime.Cache[GetProductRelatedBrandInfoCachKey] != null)
            {
                return (List<BrandInfo>)HttpRuntime.Cache[GetProductRelatedBrandInfoCachKey];
            }
            List<BrandInfo> list = ProductDA.GetProductRelatedBrandInfo(productSysNo);
            if (list != null && list.Count > 0)
            {
                HttpRuntime.Cache.Insert(GetProductRelatedBrandInfoCachKey, list, null, DateTime.Now.AddSeconds(CacheTime.Middle), Cache.NoSlidingExpiration);
            }
            return list;
        }

        /// <summary>
        /// 获取C1的推荐品牌
        /// </summary>
        /// <returns></returns>
        public static List<BrandInfo> GetRecommendBrandForECC1(int ECCategory1Id)
        {
            string cacheKey = "GetRecommendBrandForECC1_" + ECCategory1Id;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<BrandInfo>)HttpRuntime.Cache[cacheKey];
            }
            List<BrandInfo> list = ProductDA.GetRecommendBrandForECC1(ECCategory1Id);
            if (list != null && list.Count > 0)
            {
                HttpRuntime.Cache.Insert(cacheKey, list, null, DateTime.Now.AddSeconds(CacheTime.Middle), Cache.NoSlidingExpiration);
            }
            return list;
        }

        /// <summary>
        /// 获取所有品牌
        /// </summary>
        /// <returns></returns>
        public static List<BrandInfoExt> GetAllBrands()
        {
            string cacheKey = CommonFacade.GenerateKey("GetAllBrandInfo");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<BrandInfoExt>)HttpRuntime.Cache[cacheKey];
            }

            List<BrandInfoExt> allBrands = ProductDA.GetAllBrands();

            HttpRuntime.Cache.Insert(cacheKey, allBrands, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);

            return allBrands;
        }

        /// <summary>
        /// 根据品牌编号获取品牌信息
        /// </summary>
        /// <param name="brandSysno">品牌编号</param>
        /// <returns></returns>
        public static BrandInfo GetBrandBySysNo(int brandSysno)
        {
            return ProductDA.GetBrandBySysNo(brandSysno);
        }
        /// <summary>
        /// buy also buy
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static List<ProductItemInfo> GetProductBuyAlsoBuy(int productID, int top)
        {
            string BuyAlsoBuyCachKey = "GetProductBuyAlsoBuy_" + productID;
            if (HttpRuntime.Cache[BuyAlsoBuyCachKey] != null)
            {
                return (List<ProductItemInfo>)HttpRuntime.Cache[BuyAlsoBuyCachKey];
            }
            ProductInfoFilter filter = new ProductInfoFilter()
            {
                ProductSysNo = productID,
                CompanyCode = ConstValue.CompanyCode,
                LaguageCode = ConstValue.LanguageCode,
                StoreCompanyCode = ConstValue.StoreCompanyCode
            };
            string alsoBuyItem = ProductDA.GetProductBuyAlsoBuy(filter);
            List<ProductItemInfo> products = GetAlsoBuyList(alsoBuyItem, top);
            if (products != null && products.Count > 0)
            {
                HttpRuntime.Cache.Insert(BuyAlsoBuyCachKey, products, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            }
            return products;
        }


        /// <summary>
        /// browse also buy
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="commonSysNo"></param>
        /// <returns></returns>
        public static List<ProductItemInfo> GetProductBrowseAlsoBuy(string productID, int top)
        {
            string BrowseAlsoBuyCachKey = "GetProductBrowseAlsoBuy_" + productID;
            if (HttpRuntime.Cache[BrowseAlsoBuyCachKey] != null)
            {
                return (List<ProductItemInfo>)HttpRuntime.Cache[BrowseAlsoBuyCachKey];
            }
            ProductInfoFilter filter = new ProductInfoFilter()
            {
                ProductID = productID,
                CompanyCode = ConstValue.CompanyCode,
                LaguageCode = ConstValue.LanguageCode,
                StoreCompanyCode = ConstValue.StoreCompanyCode
            };
            string alsoBuyItem = ProductDA.GetProductBuyAlsoBuy(filter);
            List<ProductItemInfo> products = GetAlsoBuyList(alsoBuyItem, top);
            if (products != null && products.Count > 0)
            {
                HttpRuntime.Cache.Insert(BrowseAlsoBuyCachKey, products, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            }
            return products;
        }

        public static string BuildProductImage(ImageSize size, string productImageUrl)
        {
            return BuildProductImage(size, productImageUrl, false);
        }
        public static string BuildProductImage(ImageSize size, string productImageUrl, bool needSSL)
        {
            //Host+ImageSize+ResourceUrl
            string url = string.Empty;
            if (productImageUrl != null)
            {
                if (productImageUrl.ToUpper().StartsWith("HTTP")
                    || productImageUrl.ToUpper().StartsWith("HTTPS"))
                {
                    return productImageUrl;
                }

                string baseUrl = ConstValue.ImageServerHost;
                if (ConstValue.HaveSSLWebsite && needSSL)
                {
                    baseUrl = ConstValue.ImageServerHostSSL;
                }
                url = baseUrl + "/" + size.ToString() + (productImageUrl.StartsWith("/") ? productImageUrl : "/" + productImageUrl);
            }
            return url;
        }

        /// <summary>
        /// 用户是否已经收藏该商品
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static bool IsProductWished(int productSysNo, int customerSysNo)
        {
            return ProductDA.IsProductWished(productSysNo, customerSysNo);
        }


        /// <summary>
        /// 构建商品详情页面包屑
        /// 格式如：首页   > 数码产品 > 数码相机/摄像机 > 数码相机 > Canon 佳能 > 产品编号：90-c05-012 
        /// </summary>
        /// <param name="cagtegoryID">三级类别</param>
        /// <param name="productCode">商品Code</param>
        /// <param name="brandID">品牌ID</param>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        public static string BuildProductBreadCrumb(int subCategoryID, string productCode, int brandID, string brandName, bool isProductDetailPage)
        {
            //所有类别
            List<CategoryInfo> allCategories = null;
            string allCategoryCachKey = "ProductDetail_ALLCategoriesCachKey";
            if (HttpRuntime.Cache[allCategoryCachKey] != null)
            {
                allCategories = (List<CategoryInfo>)HttpRuntime.Cache[allCategoryCachKey];
            }
            else
            {
                allCategories = CategoryDA.QueryCategories();
            }
            //三级类别
            CategoryInfo subCategoryInfo = null;
            if (isProductDetailPage)
            {
                subCategoryInfo = allCategories.Find(f => f.C3Sysno == subCategoryID && f.CategoryType == CategoryType.SubCategory);
            }
            else
            {
                subCategoryInfo = allCategories.Find(f => f.CategoryID == subCategoryID && f.CategoryType == CategoryType.SubCategory);
            }
            //2级类别
            CategoryInfo categoryInfo = null;
            if (subCategoryInfo != null)
            {
                categoryInfo = allCategories.Find(f => f.CurrentSysNo == subCategoryInfo.ParentSysNo && f.CategoryType == CategoryType.Category);
            }
            else
            {
                categoryInfo = allCategories.Find(f => f.CategoryID == subCategoryID && f.CategoryType == CategoryType.Category);
            }
            //大类
            CategoryInfo tabStoreInfo = null;
            if (categoryInfo != null)
            {
                tabStoreInfo = allCategories.Find(f => f.CurrentSysNo == categoryInfo.ParentSysNo && f.CategoryType == CategoryType.TabStore);
            }
            StringBuilder breadCrumbBuilder = new StringBuilder(string.Format("<div class=\"crumb\"><div class=\"inner\"><span class=\"grayB\"></span><a href=\"{0}\">首页</a>", ConstValue.CDNWebDomain));
            if (tabStoreInfo != null)
            {
                breadCrumbBuilder.AppendFormat("<b>></b><a href=\"{0}\">{1}</a>",
                    PageHelper.BuildUrl("C1Route", tabStoreInfo.CategoryID),
                    tabStoreInfo.CategoryName);
            }
            if (categoryInfo != null)
            {
                breadCrumbBuilder.AppendFormat("<b>></b><a href=\"{0}\">{1}</a>",
                    PageHelper.BuildUrl("C2Route", categoryInfo.CategoryID),
                    categoryInfo.CategoryName);
            }
            if (subCategoryInfo != null)
            {
                breadCrumbBuilder.AppendFormat("<b>></b><a href=\"{0}\">{1}</a>",
                    PageHelper.BuildUrl("C3Route", subCategoryInfo.CategoryID),
                    subCategoryInfo.CategoryName);
            }
            if (brandID > 0 && !string.IsNullOrEmpty(brandName))
            {
                breadCrumbBuilder.AppendFormat("<b>></b><a title=\"{0}\" href=\"{1}\">{2}</a>",
                    brandName,
                    //ConstValue.WebDomain + "/product/searchresult?N=" + (ConstValue.SINGLE_BRAND_STORE_DMSID_SEED + brandID),
                    PageHelper.BuildUrl("BrandProducts", brandID),
                    StringUtility.TruncateString(brandName, 50, string.Empty));
            }
            if (!string.IsNullOrEmpty(productCode))
            {
                breadCrumbBuilder.AppendFormat("<b class=\"gray\">></b>产品编号：{0} ", productCode);
            }
            breadCrumbBuilder.Append("</div></div>");
            HttpRuntime.Cache.Insert(allCategoryCachKey, allCategories, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            return breadCrumbBuilder.ToString();
        }


        public static string BuildSubStoreBreadCrumb(int subCategoryID)
        {
            //所有类别
            List<CategoryInfo> allCategories = null;
            string allCategoryCachKey = "ProductDetail_ALLCategoriesCachKey";
            if (HttpRuntime.Cache[allCategoryCachKey] != null)
            {
                allCategories = (List<CategoryInfo>)HttpRuntime.Cache[allCategoryCachKey];
            }
            else
            {
                allCategories = CategoryDA.QueryCategories();
            }
            //三级类别
            CategoryInfo subCategoryInfo = null;
            subCategoryInfo = allCategories.Find(f => f.CategoryID == subCategoryID && f.CategoryType == CategoryType.SubCategory);

            //2级类别
            CategoryInfo categoryInfo = null;
            if (subCategoryInfo != null)
            {
                categoryInfo = allCategories.Find(f => f.CurrentSysNo == subCategoryInfo.ParentSysNo && f.CategoryType == CategoryType.Category);
            }
            else
            {
                categoryInfo = allCategories.Find(f => f.CategoryID == subCategoryID && f.CategoryType == CategoryType.Category);
            }
            //大类
            CategoryInfo tabStoreInfo = null;
            if (categoryInfo != null)
            {
                tabStoreInfo = allCategories.Find(f => f.CurrentSysNo == categoryInfo.ParentSysNo && f.CategoryType == CategoryType.TabStore);
            }
            StringBuilder breadCrumbBuilder = new StringBuilder(string.Format("<div class=\"crumb\"><div class=\"inner\"><span class=\"grayB\"></span><a href=\"{0}\">首页</a>", ConstValue.CDNWebDomain));
            if (tabStoreInfo != null)
            {
                breadCrumbBuilder.AppendFormat("<b>&gt;</b><a href=\"{0}\">{1}</a>",
                    PageHelper.BuildUrl("C1Route", tabStoreInfo.CategoryID),
                    tabStoreInfo.CategoryName);
            }
            if (categoryInfo != null)
            {
                if (subCategoryInfo == null)
                {
                    breadCrumbBuilder.AppendFormat("<b class=\"gray\">&gt;</b><strong data-href='{0}'>{1}</strong>",
          PageHelper.BuildUrl("C2Route", categoryInfo.CategoryID),
          categoryInfo.CategoryName);
                }
                else
                {
                    breadCrumbBuilder.AppendFormat("<b>&gt;</b><a href=\"{0}\">{1}</a>",
                        PageHelper.BuildUrl("C2Route", categoryInfo.CategoryID),
                        categoryInfo.CategoryName);
                }
            }
            if (subCategoryInfo != null)
            {
                breadCrumbBuilder.AppendFormat("<b class=\"gray\">&gt;</b><strong data-href='{0}'>{1}</strong>",
                    PageHelper.BuildUrl("C3Route", subCategoryInfo.CategoryID),
                    subCategoryInfo.CategoryName);
            }

            breadCrumbBuilder.Append("</div></div>");
            HttpRuntime.Cache.Insert(allCategoryCachKey, allCategories, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            return breadCrumbBuilder.ToString();
        }


        /// <summary>
        /// 产品比较视图
        /// </summary>
        /// <param name="itemList">产品IDList</param>
        /// <returns></returns>
        public static ProductCompareView GetProductCompareList(List<string> itemList)
        {
            //string GetProductCompareListCachKey = "GetProductCompareList_itemList";
            //if (HttpRuntime.Cache[GetProductCompareListCachKey] != null)
            //{
            //   return  (ProductCompareView)HttpRuntime.Cache[GetProductCompareListCachKey];
            //}
            List<ProductCompareInfo> productList = ProductDA.GetProductCompareList(itemList, ConstValue.CompanyCode, ConstValue.LanguageCode, ConstValue.StoreCompanyCode);
            if (productList == null || productList.Count <= 0)
            {
                return null;
            }
            foreach (ProductCompareInfo product in productList)
            {
                product.WarrantyDetail = GetWarrantyDetail(product.Warranty, product.HostWarrantyDay, product.PartWarrantyDay);
            }

            ProductCompareView view = GennerateCompareView(itemList, productList);

            // HttpRuntime.Cache.Insert(GetProductCompareListCachKey, view, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);

            return view;
        }

        /// <summary>
        /// code 用于判断显示咨询或者评论 1:评论列表 2：评论详情 3：咨询列表 4：咨询详情
        /// </summary>
        /// <param name="subCategoryID"></param>
        /// <param name="productCode"></param>
        /// <param name="brandID"></param>
        /// <param name="brandName"></param>
        /// <param name="isProductDetailPage"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string BuildProductCommentBreadCrumb(int subCategoryID, string productCode, int brandID, string brandName, bool isProductDetailPage, string code)
        {
            //所有类别
            List<CategoryInfo> allCategories = null;
            string allCategoryCachKey = "ProductDetail_ALLCategoriesCachKey";
            if (HttpRuntime.Cache[allCategoryCachKey] != null)
            {
                allCategories = (List<CategoryInfo>)HttpRuntime.Cache[allCategoryCachKey];
            }
            else
            {
                allCategories = CategoryDA.QueryCategories();
            }
            //三级类别
            CategoryInfo subCategoryInfo = null;
            if (isProductDetailPage)
            {
                subCategoryInfo = allCategories.Find(f => f.C3Sysno == subCategoryID && f.CategoryType == CategoryType.SubCategory);
            }
            else
            {
                subCategoryInfo = allCategories.Find(f => f.CategoryID == subCategoryID && f.CategoryType == CategoryType.SubCategory);
            }
            //2级类别
            CategoryInfo categoryInfo = null;
            if (subCategoryInfo != null)
            {
                categoryInfo = allCategories.Find(f => f.CurrentSysNo == subCategoryInfo.ParentSysNo && f.CategoryType == CategoryType.Category);
            }
            //大类
            CategoryInfo tabStoreInfo = null;
            if (categoryInfo != null)
            {
                tabStoreInfo = allCategories.Find(f => f.CurrentSysNo == categoryInfo.ParentSysNo && f.CategoryType == CategoryType.TabStore);
            }
            StringBuilder breadCrumbBuilder = new StringBuilder(string.Format("<div class=\"crumb\"><span class=\"grayB\"></span><a href=\"{0}\">首页</a>", ConstValue.WebDomain));
            if (tabStoreInfo != null)
            {
                breadCrumbBuilder.AppendFormat("<b>></b><a href=\"{0}\">{1}</a>",
                    ConstValue.WebDomain + "/tabStore/" + tabStoreInfo.CategoryID,
                    tabStoreInfo.CategoryName);
            }
            if (categoryInfo != null)
            {
                breadCrumbBuilder.AppendFormat("<b>></b><a href=\"{0}\">{1}</a>",
                    PageHelper.BuildUrl("C2Route", categoryInfo.CategoryID),
                    categoryInfo.CategoryName);
            }
            if (subCategoryInfo != null)
            {
                breadCrumbBuilder.AppendFormat("<b>></b><a href=\"{0}\">{1}</a>",
                    PageHelper.BuildUrl("C3Route", subCategoryInfo.CategoryID),
                    subCategoryInfo.CategoryName);
            }
            if (brandID > 0 && !string.IsNullOrEmpty(brandName))
            {
                breadCrumbBuilder.AppendFormat("<b>></b><a title=\"{0}\" href=\"{1}\">{2}</a>",
                    brandName,
                    //ConstValue.WebDomain + "/product/searchresult?N=" + (ConstValue.SINGLE_BRAND_STORE_DMSID_SEED + brandID),
                    PageHelper.BuildUrl("BrandProducts", brandID),
                    StringUtility.TruncateString(brandName, 50, string.Empty));
            }
            if (!string.IsNullOrEmpty(productCode))
            {
                breadCrumbBuilder.AppendFormat("<b class=\"gray\">></b>产品编号：{0} ", productCode);
            }
            if (!string.IsNullOrEmpty(code))
            {
                if (code == "1")
                {
                    breadCrumbBuilder.AppendFormat("<b class=\"gray\">></b>商品评论");
                }
                else if (code == "2")
                {
                    breadCrumbBuilder.AppendFormat("<b class=\"gray\">></b>评论详情");
                }
                else if (code == "3")
                {
                    breadCrumbBuilder.AppendFormat("<b class=\"gray\">></b>商品咨询");
                }
                else if (code == "4")
                {
                    breadCrumbBuilder.AppendFormat("<b class=\"gray\">></b>咨询详情");
                }
                else if (code == "5")
                {
                    breadCrumbBuilder.AppendFormat("<b class=\"gray\">></b>撮合交易");
                }
            }
            breadCrumbBuilder.Append("</div>");
            HttpRuntime.Cache.Insert(allCategoryCachKey, allCategories, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            return breadCrumbBuilder.ToString();
        }

        /// <summary>
        /// 将商品规格(Performance)转换成html显示
        /// </summary>
        /// <param name="xmlPerformance"></param>
        /// <returns></returns>
        public static string BuildProductPerformanceToHtml(string xmlPerformance)
        {
            if (string.IsNullOrEmpty(xmlPerformance)) return string.Empty;
            if (xmlPerformance.StartsWith("<LongDescription"))
            {
                ECommerce.Facade.Product.Models.LongDescription performance;

                xmlPerformance = xmlPerformance.Replace("&", "&amp;");
                XmlSerializer mySerializer = new XmlSerializer(typeof(LongDescription));
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlPerformance)))
                {
                    using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        performance = (ECommerce.Facade.Product.Models.LongDescription)mySerializer.Deserialize(sr);
                    }
                }

                if (performance != null && performance.Groups != null && performance.Groups.Count > 0)
                {
                    StringBuilder builder = new StringBuilder("<table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody>");
                    foreach (ECommerce.Facade.Product.Models.Group entity in performance.Groups)
                    {
                        builder.AppendFormat("<tr><th colspan=\"2\" class=\"title\">{0}</th></tr>", entity.GroupName);
                        if (entity.Propertys != null && entity.Propertys.Count > 0)
                        {
                            foreach (ECommerce.Facade.Product.Models.Property item in entity.Propertys)
                            {
                                builder.AppendFormat("<tr><th>{0}</th>", item.Key);
                                builder.AppendFormat("<td>{0}</td></tr>", item.Value);
                            }
                        }
                    }
                    builder.Append("</tbody></table>");
                    return builder.ToString();
                }

                return string.Empty;
            }
            return xmlPerformance;
        }


        /// <summary>
        /// 商品附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductAccessories> GetProductAccessoriesList(int productSysNo)
        {
            string GetProductAccessoriesListCachKey = "GetProductAccessoriesList_" + productSysNo;
            if (HttpRuntime.Cache[GetProductAccessoriesListCachKey] != null)
            {
                return (List<ProductAccessories>)HttpRuntime.Cache[GetProductAccessoriesListCachKey];
            }
            List<ProductAccessories> list = ProductDA.GetProductAccessoriesList(productSysNo);
            if (list != null && list.Count > 0)
            {
                HttpRuntime.Cache.Insert(GetProductAccessoriesListCachKey, list, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            }
            return list;
        }


        /// <summary>
        /// 促销模板
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static SaleAdvertisement GetSaleAdvertisementInfo(int sysNo)
        {
            string GetSaleAdvertisementInfoCachKey = "GetSaleAdvertisementInfo_" + sysNo;
            if (HttpRuntime.Cache[GetSaleAdvertisementInfoCachKey] != null)
            {
                return (SaleAdvertisement)HttpRuntime.Cache[GetSaleAdvertisementInfoCachKey];
            }
            SaleAdvertisement promotion = ProductDA.GetSaleAdvertisementInfo(sysNo);
            if (promotion != null)
            {
                HttpRuntime.Cache.Insert(GetSaleAdvertisementInfoCachKey, promotion, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            }
            return promotion;
        }

        public static List<ProductPropertyInfo> GetProductCategoryTemplatePropertys(int SysNo)
        {
            return ProductDA.GetProductCategoryTemplatePropertys(SysNo);
        }

        /// <summary>
        /// 商品会员价格
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <param name="customerRank"></param>
        /// <returns></returns>
        public static ProductCustomerRankPrice GetProductCustomerRankPrice(int customerSysNo, int productSysNo)
        {
            return ProductDA.GetProductCustomerRankPrice(customerSysNo, productSysNo);
        }

        /// <summary>
        /// 获取商品销售区域省份信息集合
        /// Author:     Ausra
        /// CreateDate: 2015-07-16
        /// </summary>
        /// <param name="productSysID">商品编号</param>
        /// <returns></returns>
        public static List<Area> GetProductAreas(int productSysNo)
        {
            List<Area> coll = ProductDA.GetProductAreas(productSysNo);
            return coll;
        }
        /// <summary>
        /// 获取商品销售区域指定省份下的市级信息集合
        /// Author:     Ausra
        /// CreateDate: 2015-07-16
        /// </summary>
        /// <param name="productSysID">商品编号</param>
        /// <param name="proviceSysNo">省份编号</param>
        /// <returns></returns>
        public static List<Area> GetProductCitys(int productSysNo, int proviceSysNo)
        {
            List<Area> coll = ProductDA.GetProductCitys(productSysNo, proviceSysNo);
            return coll;
        }

        /// <summary>
        /// 获取商品运费
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="areaSysNo">地区编号</param>
        /// <returns></returns>
        public static List<ProductShippingPrice> GetProductShippingPrice(int productSysNo, int areaSysNo)
        {
            return ProductDA.GetProductShippingPrice(productSysNo, areaSysNo);
        }

        #region  私有辅助方法


        private static List<ProductItemInfo> GetAlsoBuyList(string alsoBuyItem, int top)
        {
            if (string.IsNullOrEmpty(alsoBuyItem)) return null;
            string[] alsoBuyArry = alsoBuyItem.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (alsoBuyArry == null || alsoBuyArry.Length < 1)
            {
                return null;
            }
            List<string> productIDs = new List<string>();
            //按顺序取出,排重
            for (int i = 0; i < alsoBuyArry.Length; i++)
            {
                string[] alsoBuy = alsoBuyArry[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (alsoBuy == null || alsoBuyArry.Length < 1 || string.IsNullOrEmpty(alsoBuyArry[0]) || productIDs.Contains(alsoBuy[0]))
                {
                    continue;
                }
                productIDs.Add(alsoBuy[0]);
                if (productIDs.Count >= top)
                {
                    break;
                }
            }
            List<ProductItemInfo> products = ProductDA.GetProductCellInfoListByIDs(productIDs);

            return products;
        }


        #region   获取商品组属性辅助方法

        private static List<ProductPropertyListInfo> GetProductPropety(int productSysNo, int commonSysNo)
        {
            List<ProductPropertyListInfo> productPropertyList = new List<ProductPropertyListInfo>();
            ProductInfoFilter filter = new ProductInfoFilter()
            {
                ProductCommonInfoSysNo = commonSysNo,
                CompanyCode = ConstValue.CompanyCode,
                LaguageCode = ConstValue.LanguageCode,
                StoreCompanyCode = ConstValue.StoreCompanyCode
            };
            List<ProductPropertyInfo> list = ProductDA.GetProductPropety(filter);
            // 删除不合法项
            list = FilterInValidProperty(list);

            if (list != null && list.Count > 0)
            {
                ProductPropertyInfo current = FindCurrentProduct(productSysNo, list);
                if (current != null)
                {
                    // 仅有一个属性组
                    if (current.Type == 1)
                    {
                        List<ProductPropertyInfo> propertyList = new List<ProductPropertyInfo>();
                        propertyList.Add(current);

                        // 属性和价格排序
                        SortParentProductProperty(list);

                        // 排除当前属性值
                        List<ProductPropertyInfo> tmpPropertyList = FilterParentProperty(list);
                        foreach (ProductPropertyInfo info in tmpPropertyList)
                        {
                            if (info.ParentValue != current.ParentValue)
                            {
                                propertyList.Add(info);
                            }
                        }

                        SortParentProductPropertyValue(propertyList);

                        ProductPropertyListInfo parent = new ProductPropertyListInfo();
                        parent.PropertyList = propertyList;
                        productPropertyList.Add(parent);
                    }
                    else
                    {
                        ProductPropertyListInfo parentPropertyList = GetParentProductProperty(current, list);
                        productPropertyList.Add(parentPropertyList);

                        ProductPropertyListInfo propertyList = GetProductProperty(current, list);
                        productPropertyList.Add(propertyList);
                    }
                }
            }

            return productPropertyList;
        }

        /// <summary>
        /// 筛选，删除 Value 为空的项
        /// </summary>
        /// <param name="list"></param>
        private static List<ProductPropertyInfo> FilterInValidProperty(List<ProductPropertyInfo> list)
        {
            if (list != null && list.Count > 0)
            {
                int type = list[0].Type;
                if (type == 1)
                {
                    list = list.FindAll(delegate(ProductPropertyInfo find)
                    {
                        return !string.IsNullOrEmpty(find.ParentValue);
                    });
                }
                else
                {
                    list = list.FindAll(delegate(ProductPropertyInfo find)
                    {
                        return !string.IsNullOrEmpty(find.ParentValue) && !string.IsNullOrEmpty(find.Value);
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 查找当前商品属性
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static ProductPropertyInfo FindCurrentProduct(int productSysNo, List<ProductPropertyInfo> list)
        {
            ProductPropertyInfo info = list.Find(delegate(ProductPropertyInfo find)
            {
                return find.ProductSysNo == productSysNo;
            });

            return info;
        }

        /// <summary>
        /// 排序第一属性，相同属性价格最小的优先
        /// 价格相同取 ProductSysNo 小的
        /// </summary>
        /// <param name="list"></param>
        private static void SortParentProductProperty(List<ProductPropertyInfo> list)
        {
            list.Sort(delegate(ProductPropertyInfo x, ProductPropertyInfo y)
            {
                int compareValue = x.ParentValue.CompareTo(y.ParentValue);
                if (compareValue == 0)
                {
                    compareValue = y.ProStatus.CompareTo(x.ProStatus);
                    if (compareValue == 0)
                    {
                        compareValue = x.CurrentPrice.CompareTo(y.CurrentPrice);
                        if (compareValue == 0)
                        {
                            compareValue = x.ProductSysNo.CompareTo(y.ProductSysNo);
                        }
                    }
                }

                return compareValue;
            });
        }

        /// <summary>
        /// 获取商品第一属性,
        /// 相同属性值编号的商品取第一个
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<ProductPropertyInfo> FilterParentProperty(List<ProductPropertyInfo> list)
        {
            string lastValue = string.Empty;

            List<ProductPropertyInfo> parentList = new List<ProductPropertyInfo>();
            foreach (ProductPropertyInfo info in list)
            {
                if (info.ParentValue != lastValue)
                {
                    parentList.Add(info);
                }

                lastValue = info.ParentValue;
            }

            return parentList;
        }

        /// <summary>
        /// 按值的优先级排序
        /// </summary>
        /// <param name="list"></param>
        private static void SortParentProductPropertyValue(List<ProductPropertyInfo> list)
        {
            list.Sort(delegate(ProductPropertyInfo x, ProductPropertyInfo y)
            {
                int i = x.ParentPriority.CompareTo(y.ParentPriority);
                if (i == 0)
                {
                    i = x.ParentValue.CompareTo(y.ParentValue);
                    if (i == 0)
                    {
                        i = x.ProductSysNo.CompareTo(y.ProductSysNo);
                    }
                }

                return i;
            });
        }

        /// <summary>
        /// 获取第一属性列表
        /// </summary>
        /// <param name="current"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static ProductPropertyListInfo GetParentProductProperty(ProductPropertyInfo current, List<ProductPropertyInfo> list)
        {
            // 设置第一属性
            List<ProductPropertyInfo> propertyList1 = new List<ProductPropertyInfo>();
            propertyList1.Add(current);

            // 属性和价格排序
            SortParentProductProperty(list);

            bool isFind = false;
            string lastValue = string.Empty;
            ProductPropertyInfo lowestPrice = null;

            foreach (ProductPropertyInfo info in list)
            {
                if (info.ParentValue == current.ParentValue)
                {
                    continue;
                }

                if (lastValue != info.ParentValue)
                {
                    if (!isFind && lowestPrice != null)
                    {
                        propertyList1.Add(lowestPrice);
                    }

                    // init
                    lowestPrice = null;
                    isFind = false;
                }

                if (lowestPrice == null)
                {
                    lowestPrice = info;
                }

                if (!isFind && info.Value == current.Value)
                {
                    propertyList1.Add(info);
                    isFind = true;
                }

                lastValue = info.ParentValue;
            }

            // 最后一个未匹配时添加价格最小的
            if (!isFind && lowestPrice != null)
            {
                propertyList1.Add(lowestPrice);
            }

            // 保存第一属性
            SortParentProductPropertyValue(propertyList1);
            ProductPropertyListInfo parent = new ProductPropertyListInfo();
            parent.PropertyList = propertyList1;

            return parent;
        }

        /// <summary>
        /// 获取第二属性列表
        /// </summary>
        /// <param name="current"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static ProductPropertyListInfo GetProductProperty(ProductPropertyInfo current, List<ProductPropertyInfo> list)
        {
            List<ProductPropertyInfo> propertyList2 = new List<ProductPropertyInfo>();
            propertyList2.Add(current);

            // 属性和价格排序
            SortProductProperty(list);

            bool isFind = false;
            string lastValue = string.Empty;
            ProductPropertyInfo lowestPrice = null;

            foreach (ProductPropertyInfo info in list)
            {
                if (info.Value == current.Value)
                {
                    continue;
                }

                if (lastValue != info.Value)
                {
                    if (!isFind && lowestPrice != null)
                    {
                        propertyList2.Add(lowestPrice);
                    }

                    // init
                    lowestPrice = null;
                    isFind = false;
                }

                if (lowestPrice == null)
                {
                    lowestPrice = info;
                }

                if (!isFind && info.ParentValue == current.ParentValue)
                {
                    propertyList2.Add(info);
                    isFind = true;
                }

                lastValue = info.Value;
            }

            // 最后一个未匹配添加价格最小的
            if (!isFind && lowestPrice != null)
            {
                propertyList2.Add(lowestPrice);
            }

            // 保存第二属性
            SortProductPropertyValue(propertyList2);
            ProductPropertyListInfo parent2 = new ProductPropertyListInfo();
            parent2.PropertyList = propertyList2;

            return parent2;
        }

        /// <summary>
        /// 排序第二属性，相同属性价格最小的优先
        /// 价格相同取 ProductSysNo 小的
        /// </summary>
        /// <param name="list"></param>
        private static void SortProductProperty(List<ProductPropertyInfo> list)
        {
            list.Sort(delegate(ProductPropertyInfo x, ProductPropertyInfo y)
            {
                int compareValue = x.Value.CompareTo(y.Value);
                if (compareValue == 0)
                {
                    compareValue = y.ProStatus.CompareTo(x.ProStatus);
                    if (compareValue == 0)
                    {
                        compareValue = x.CurrentPrice.CompareTo(y.CurrentPrice);
                        if (compareValue == 0)
                        {
                            compareValue = x.ProductSysNo.CompareTo(y.ProductSysNo);
                        }
                    }
                }
                return compareValue;
            });
        }

        /// <summary>
        /// 按值的优先级排序
        /// </summary>
        /// <param name="list"></param>
        private static void SortProductPropertyValue(List<ProductPropertyInfo> list)
        {
            list.Sort(delegate(ProductPropertyInfo x, ProductPropertyInfo y)
            {
                int i = x.Priority.CompareTo(y.Priority);
                if (i == 0)
                {
                    i = x.Value.CompareTo(y.Value);
                    if (i == 0)
                    {
                        i = x.ProductSysNo.CompareTo(y.ProductSysNo);
                    }
                }

                return i;
            });
        }
        #endregion

        #region  商品保修细则

        private static string GetWarrantyDetail(string warrantyDetail, int hostWarrantyDay, int partWarrantyDay)
        {
            if (hostWarrantyDay == WarrantyForeverDay)
            {
                if (string.IsNullOrEmpty(warrantyDetail))
                {
                    return "终身保修";
                }
            }
            else
            {
                string hostDay = CatWarrantyDays(hostWarrantyDay);
                string partDay = CatWarrantyDays(partWarrantyDay);

                if (hostWarrantyDay == 0 && partWarrantyDay == 0)
                {
                    return "该商品无保修期。";
                }
                else if (hostWarrantyDay > 0 && partWarrantyDay == 0)
                {
                    return string.Format("该商品保修期为{0}。", hostDay);
                }
                else if (hostWarrantyDay == 0 && partWarrantyDay > 0)
                {
                    return string.Format("该商品无保修期，零部件保修期为{0}", partDay);
                }
                else
                {
                    return string.Format("该商品保修期为{0}，零部件保修期为{1}。", hostDay, partDay);
                }
            }

            return string.Empty;
        }

        private static string CatWarrantyDays(int realDay)
        {
            if (realDay <= WarrantyDay)
            {
                return string.Format("{0}天", realDay);
            }
            else
            {
                realDay = Convert.ToInt32(realDay / WarrantyMonth);
                return string.Format("{0}个月", realDay);
            }
        }
        #endregion

        #region  构造产品比较视图

        /// <summary>
        /// 获取产品比较视图
        /// </summary>
        /// <param name="itemList">query比较item列表</param>
        /// <param name="productList">数据库item列表</param>
        /// <param name="forDiy">是否用于DIY比较页</param>
        /// <returns>产品比较视图</returns>
        private static ProductCompareView GennerateCompareView(List<string> itemList, List<ProductCompareInfo> productList)
        {
            List<ProductCompareInfo> matchedProductCompareInfo = new List<ProductCompareInfo>();
            string removeParms = string.Empty;

            foreach (string itemNo in itemList)
            {
                ProductCompareInfo compareInfo = productList.Find(delegate(ProductCompareInfo compare)
                {
                    if (compare.ProductSysNo.ToString().Trim() == itemNo.Trim())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

                if (compareInfo != null)
                {
                    removeParms += itemNo + ",";
                    matchedProductCompareInfo.Add(compareInfo);
                }
            }

            if (matchedProductCompareInfo.Count == 0)
            {
                return null;
            }

            ProductCompareView compareView = new ProductCompareView();

            compareView.RemoveList = new List<string>(CMaxCompareNumber);
            compareView.ProductCellList = matchedProductCompareInfo;

            ////构造删除链接列表
            if (matchedProductCompareInfo.Count == 1)
            {
                ProductCompareInfo compareInfo = matchedProductCompareInfo[0];
                string url = PageHelper.BuildUrl("C3Route", compareInfo.CategoryID.ToString());

                compareView.RemoveList.Add(url);
            }
            else
            {
                foreach (ProductCompareInfo compareInfo in matchedProductCompareInfo)
                {
                    string currentRemove = removeParms.Replace(compareInfo.ProductCode + ",", "").TrimEnd(',');

                    string url = ConstValue.WebDomain + "/product/productcompare?Item=" + currentRemove;// Url.BuildUrl(PageAlias.Compare, "Item", currentRemove);

                    compareView.RemoveList.Add(url);
                }
            }

            ////构造产品规格列表

            compareView.ComparePropertyList = GetComparePropertyList(matchedProductCompareInfo);

            return compareView;
        }

        /// <summary>
        /// 获取产品属性列表
        /// </summary>
        /// <param name="matchedProductCompareInfo">匹配上的比较产品列表</param>
        /// <returns>产品属性列表</returns>
        private static List<CompareProperty> GetComparePropertyList(List<ProductCompareInfo> matchedProductCompareInfo)
        {
            string perFormance = string.Empty;
            string key = string.Empty;
            string value = string.Empty;
            int totalCount = matchedProductCompareInfo.Count;

            List<CompareProperty> comparePropertyList = new List<CompareProperty>();

            ////创建所有属性列表
            StringReader sr = null;
            XmlTextReader tr = null;

            foreach (ProductCompareInfo compareInfo in matchedProductCompareInfo)
            {
                try
                {
                    perFormance = compareInfo.Performance;

                    if (perFormance == null || perFormance.Trim() == string.Empty)
                    {
                        continue;
                    }

                    sr = new StringReader(perFormance);
                    tr = new XmlTextReader(sr);
                    while (tr.Read())
                    {
                        if (tr.MoveToContent() != XmlNodeType.Element)
                            continue;

                        if (tr.Name == CProperty)
                        {
                            key = tr.GetAttribute(CKey);

                            CompareProperty property = comparePropertyList.Find(delegate(CompareProperty prop)
                            {
                                if (prop.PropertyName == key)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            });

                            if (property == null)
                            {
                                CompareProperty newProperty = new CompareProperty();
                                newProperty.PropertyName = key;
                                newProperty.CompareValueList = new List<string>(totalCount);
                                comparePropertyList.Add(newProperty);
                            }
                        }
                    }
                }
                catch
                {
                    return comparePropertyList;
                }
                finally
                {
                    if (tr != null) { tr.Close(); }
                    if (sr != null) { sr.Close(); }
                }
            }


            ////填充所有属性的value
            for (int i = 0; i < totalCount; i++)
            {
                foreach (CompareProperty prop in comparePropertyList)
                {
                    prop.CompareValueList.Add(string.Empty);
                }

                perFormance = matchedProductCompareInfo[i].Performance;

                if (perFormance == null || perFormance.Trim() == string.Empty)
                {
                    continue;
                }

                try
                {
                    sr = new StringReader(perFormance);
                    tr = new XmlTextReader(sr);
                    while (tr.Read())
                    {
                        if (tr.MoveToContent() != XmlNodeType.Element)
                            continue;

                        if (tr.Name == CProperty)
                        {
                            key = tr.GetAttribute(CKey);
                            value = tr.GetAttribute(CValue);

                            CompareProperty property = comparePropertyList.Find(delegate(CompareProperty prop)
                            {
                                if (prop.PropertyName == key)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            });

                            if (property != null)
                            {
                                property.CompareValueList[i] = value;
                            }
                        }
                    }
                }
                finally
                {
                    if (tr != null) { tr.Close(); }
                    if (sr != null) { sr.Close(); }
                }
            }

            ////移除属性名前缀
            foreach (CompareProperty prop in comparePropertyList)
            {
                prop.PropertyName = Regex.Replace(prop.PropertyName, @"[^']+_", "");
            }

            return comparePropertyList;
        }

        #endregion

        #endregion
    }
}
