using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.IM.BizProcessor
{
    /// <summary>
    /// 调用外部接口的内部通用类。将IM中调用外部接口的地方都集中到这里。
    /// </summary>
    public static class ExternalDomainBroker
    {
        /// <summary>
        /// 创建po单，返回新创建的PO单的SysNo
        /// </summary>
        /// <param name="newPOInfo"></param>
        /// <returns></returns>
        public static string CreatePurchaseOrder(BizEntity.PO.PurchaseOrderInfo newPOInfo)
        {
            return ObjectFactory<ECCentral.Service.IBizInteract.IPOBizInteract>.Instance.CreatePurchaseOrder(newPOInfo);
        }

        public static ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo)
        {
            return ObjectFactory<IBizInteract.IInventoryBizInteract>.Instance.GetProductTotalInventoryInfo(productSysNo);
        }

        public static int GetInStockDaysByProductSysNo(int productSysNo)
        {
            return ObjectFactory<IBizInteract.IInventoryBizInteract>.Instance.GetInStockDaysByProductSysNo(productSysNo);
        }

        public static bool CheckMarketIsActivity(int productSysNo)
        {
            return ObjectFactory<IBizInteract.IMKTBizInteract>.Instance.CheckMarketIsActivity(productSysNo);
        }

        public static void CheckComboPriceAndSetStatus(int productSysNo)
        {
            ObjectFactory<IBizInteract.IMKTBizInteract>.Instance.CheckComboPriceAndSetStatus(productSysNo);
        }

        public static List<ProductPromotionDiscountInfo> GetProductPromotionDiscountInfoList(int productSysNo)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.GetProductPromotionDiscountInfoList(productSysNo);
        }

        /// <summary>
        /// 创建商品时初始化商品库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static bool InitProductInventoryInfo(int productSysNo)
        {
            return ObjectFactory<IBizInteract.IInventoryBizInteract>.Instance.InitProductInventoryInfo(productSysNo);
        }

        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓库</param>
        /// <returns>ProductInventoryEntity</returns>
        public static ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo)
        {
            return ObjectFactory<IBizInteract.IInventoryBizInteract>.Instance.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
        }

        /// <summary>
        /// 销售数量
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        public static ProductSalesTrendInfo GetProductSalesTrendInfoByStock(int productSysNo, int stockSysNo)
        {
            return ObjectFactory<IBizInteract.IInventoryBizInteract>.Instance.GetProductSalesTrendInfoByStock(productSysNo, stockSysNo);
        }

        /// <summary>
        /// 获取分仓列表 [Ray.L.Xing 泰隆优选不存在多渠道 故将StockInfo 改为WarehouseInfo 返回]
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return ObjectFactory<IBizInteract.IInventoryBizInteract>.Instance.GetWarehouseList(companyCode);
        }


        /// <summary>
        /// 创建操作日志，提供统一的日志记录服务 bober add
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public static int CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }
        //public static bool CheckMarketIsActivity(int productSysNo)
        //{
        //    return ObjectFactory<IBizInteract.IMKTBizInteract>.Instance.c
        //}

        /// <summary>
        /// 设置渠道商品指定库存
        /// </summary>
        /// <param name="channelSysNo">渠道编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="channelSellCount">指定库存数量</param>
        /// <returns>IsSucceed</returns>
        public static bool SetChannelProductInventory(int channelSysNo, int productSysNo, int channelSellCount)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.SetChannelProductInventory(channelSysNo, productSysNo, channelSellCount);
        }

        /// <summary>
        /// 取消渠道商品指定库存
        /// </summary>
        /// <param name="channelSysNo">渠道编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="channelSellCount">指定库存数量</param>
        /// <returns>IsSucceed</returns>
        public static bool AbandonChannelProductInventory(int channelSysNo, int productSysNo, int channelSellCount)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.SetChannelProductInventory(channelSysNo, productSysNo, channelSellCount);
        }


        /// <summary>
        /// 取得积分换算成钱的比率，如果是积分换算成钱，则除以此值；如果是钱换算成积分，则乘以此值
        /// </summary>
        /// <returns></returns>
        public static decimal GetPointToMoneyRatio()
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetPointToMoneyRatio();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(int sysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserInfoBySysNo(sysNo);
        }

        public static void GetGiftSNAndCouponSNByProductSysNo(int productsysno, out int giftsysno, out int couponsysno)
        {
            ObjectFactory<IMKTBizInteract>.Instance.GetGiftSNAndCouponSNByProductSysNo(productsysno, out giftsysno, out couponsysno);
        }

        /// <summary>
        /// 系统多语言
        /// </summary>
        /// <returns></returns>
        public static List<Language> GetAllLanguageList()
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetAllLanguageList();
        }

        /// <summary>
        /// 写入多语言
        /// </summary>
        /// <returns></returns>
        public static void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity)
        {
            ObjectFactory<ICommonBizInteract>.Instance.SetMultiLanguageBizEntity(entity);
        }
    }
}
