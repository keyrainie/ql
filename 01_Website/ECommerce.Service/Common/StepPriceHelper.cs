using ECommerce.DataAccess.Common;
using ECommerce.Entity;
using ECommerce.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace ECommerce.Facade.Common
{
    public static class StepPriceHelper
    {
        //public static decimal GetProductStepPrice(this int productSysNo, decimal oldPrice, int productCount = 1)
        //{
        //    List<ProductStepPrice> productStepPrices = null;
        //    string cacheKey = "GetProductStepPrice_" + productSysNo;
        //    if (HttpRuntime.Cache[cacheKey] != null)
        //    {
        //        productStepPrices = (List<ProductStepPrice>)HttpRuntime.Cache[cacheKey];
        //    }
        //    else
        //    {
        //        productStepPrices = CommonDA.GetProductStepPrice(productSysNo);
        //        HttpRuntime.Cache.Insert(cacheKey, productStepPrices, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);
        //    }

        //    if (productStepPrices == null)
        //    {
        //        return oldPrice;
        //    }
        //    ProductStepPrice productStepPrice = productStepPrices.FirstOrDefault(c => (c.BaseCount >= productCount && productCount <= c.TopCount));
        //    if (productStepPrice == null)
        //    {
        //        return oldPrice;
        //    }
        //    return productStepPrice.StepPrice;
        //}

        public static decimal GetProductStepPrice(this decimal oldPrice, int productSysNo, int productCount = 1)
        {
            List<ProductStepPrice> productStepPrices = null;
            string cacheKey = "GetProductStepPrice_" + productSysNo;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                productStepPrices = (List<ProductStepPrice>)HttpRuntime.Cache[cacheKey];
            }
            else
            {
                productStepPrices = CommonDA.GetProductStepPrice(productSysNo);
                HttpRuntime.Cache.Insert(cacheKey, productStepPrices, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);
            }

            if (productStepPrices == null)
            {
                return oldPrice;
            }
            ProductStepPrice productStepPrice = productStepPrices.FirstOrDefault(c => (c.BaseCount <= productCount && productCount <= c.TopCount));
            if (productStepPrice == null)
            {
                return productStepPrices.FirstOrDefault(c => (c.BaseCount == 0 && 0 == c.TopCount)).StepPrice;
            }
            return productStepPrice.StepPrice;
        }
    }
}
