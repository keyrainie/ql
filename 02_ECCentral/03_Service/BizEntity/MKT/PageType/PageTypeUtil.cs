using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    public class PageTypeUtil
    {
        public static PageTypePresentationType ResolvePresentationType(ModuleType moduleType, string pageTypeID)
        {
            PageTypePresentationType result = PageTypePresentationType.NoneSubPages;
            if (moduleType == ModuleType.NewsAndBulletin)
            {
                if (pageTypeID == "9")
                    result = PageTypePresentationType.Category1;
                if (pageTypeID == "0")
                    result = PageTypePresentationType.Category2;
                if (pageTypeID == "3")
                    result = PageTypePresentationType.Category3;
            }
            else
            {
                if (pageTypeID == "1")
                    result = PageTypePresentationType.Category1;
                if (pageTypeID == "2")
                    result = PageTypePresentationType.Category2;
                if (pageTypeID == "3")
                    result = PageTypePresentationType.Category3;
            }


            if (moduleType == ModuleType.Banner || moduleType == ModuleType.SEO)
            {
                switch (pageTypeID)
                {
                    case "4"://品牌专区
                        result = PageTypePresentationType.Stores;
                        break;
                    case "5"://其他促销页面
                        result = PageTypePresentationType.OtherSales;
                        break;
                    case "9"://品牌旗舰店
                        result = PageTypePresentationType.Flagship;
                        break;
                    case "29"://商家页面
                        result = PageTypePresentationType.Merchant;
                        break;
                    case "6":
                        result = PageTypePresentationType.BrandExclusive;
                        break;
                }
            }
            else if (moduleType == ModuleType.ProductRecommend)
            {
                switch (pageTypeID)
                {
                    case _productRecommendBrandPageTypeID://品牌专区
                        result = PageTypePresentationType.Brand;
                        break;
                    case "6"://名品汇品牌专属
                        result = PageTypePresentationType.BrandExclusive;
                        break;
                    case "12"://商家页面
                        result = PageTypePresentationType.Merchant;
                        break;
                }
            }
            else if (moduleType == ModuleType.HotKeywords)
            {

            }
            else if (moduleType == ModuleType.DefaultKeywords)
            {
                switch (pageTypeID)
                {
                    case "4"://品牌专区
                        result = PageTypePresentationType.Stores;
                        break;
                    case "9"://品牌旗舰店
                        result = PageTypePresentationType.Flagship;
                        break;
                }
            }
            else if (moduleType == ModuleType.NewsAndBulletin)
            {
                switch (pageTypeID)
                {
                    case "5"://专卖店
                        result = PageTypePresentationType.Stores;
                        break;
                    case "20":
                    case "21":
                    case "22":
                    case "23":
                        result = PageTypePresentationType.Brand;
                        break;
                    case "24":
                    case "25":
                    case "26":
                    case "27":
                        result = PageTypePresentationType.Category3;
                        break;
                    case "32":
                        result = PageTypePresentationType.Merchant;
                        break;
                    case "35"://品牌旗舰店
                        result = PageTypePresentationType.AppleZone;
                        break;
                }
            }


            return result;
        }

        /// <summary>
        /// 商品推荐模块中品牌页面的类型编号
        /// </summary>
        private const string _productRecommendBrandPageTypeID = "4";
        public static int ProductRecommendBrandPageTypeID
        {
            get
            {
                return Convert.ToInt32(_productRecommendBrandPageTypeID);
            }
        }

        public static bool IsProductRecommendHomePageNewPosition(int? pageType, int? pageID, int? positionID)
        {
            return pageType == 0 && pageID == 0 && positionID == 0;
        }
    }
}
