using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Promotion; 
using System.Web;
using ECommerce.Entity;
using System.Web.Caching;
using ECommerce.DataAccess.Product;
using ECommerce.SOPipeline.Impl;

namespace ECommerce.Facade.Product
{
    public static partial class ProductFacade
    {
        /// <summary>
        /// 获取指定商品的所有促销信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductPromotionInfo GetProductPromotionInfo(int productSysNo)
        {
            if (productSysNo <= 0)
            {
                return null;
            }
            string cacheKey = "GetProductPromotionInfo_" + productSysNo;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (ProductPromotionInfo)HttpRuntime.Cache[cacheKey];
            }  

            ProductPromotionInfo promotion = new ProductPromotionInfo();
            promotion.ProductSysNo = productSysNo;
            //套餐
            promotion.ComboList = PromotionDA.GetComboListByMasterProductSysNo(productSysNo);
            //团购
            promotion.GroupBuySysNo = PromotionDA.ProductIsGroupBuying(productSysNo);
            //限时和秒杀
            CountdownInfo countdown = PromotionDA.GetProductCountdownByProductSysNo(productSysNo);
            if (countdown != null)
            {
                promotion.Countdown = countdown;
                promotion.CountdownSysNo = countdown.SysNo.Value;
                promotion.IsSecKill = countdown.IsSecondKill.HasValue ? countdown.IsSecondKill.Value : false;
            }
            else
            {
                promotion.CountdownSysNo = 0;
                promotion.IsSecKill = false;
            }
            //赠品
            promotion.SaleGiftList = PromotionDA.GetSaleGiftListByProductSysNo(productSysNo);


            
            HttpRuntime.Cache.Insert(cacheKey, promotion, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            return promotion;
        }
    }
}
