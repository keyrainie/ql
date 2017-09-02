using IPP.ContentMgmt.SyncShopPrice.DataAccess;
using IPP.ContentMgmt.SyncShopPrice.Entities;
using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.ContentMgmt.SyncShopPrice.BizProcess
{
    public static class SyncBizProcessor
    {
        public static void Start(JobContext jobContext)
        {
            List<ProductPriceEntity> lstProductPrice = SyncShopPriceDA.GetSyncShopPriceProducts();
            List<ProductShopPriceEntity> shopProductLst = new List<ProductShopPriceEntity>();

            ProductShopPriceEntity syncEntity = null;

            #region 调用接口获得需要同步价格的商品的门店价格 有接口后此处替换掉
            foreach (ProductPriceEntity entity in lstProductPrice)
            {
                syncEntity = new ProductShopPriceEntity();
                syncEntity.ProductSysNo = entity.ProductSysNo;
                syncEntity.ProductID = entity.ProductID;
                syncEntity.ShopPrice = entity.Price-1;
                shopProductLst.Add(syncEntity);
            }
            #endregion

            foreach (ProductShopPriceEntity shopEntity in shopProductLst)
            {
                var orignalPrice = (from p in lstProductPrice
                                    where p.ProductSysNo == shopEntity.ProductSysNo
                                    select p.Price).First();
                if (orignalPrice != null && orignalPrice != shopEntity.ShopPrice)
                {
                    SyncShopPriceDA.UpdateProductPrice(shopEntity, orignalPrice);
                }
            }
        }

    }
}
