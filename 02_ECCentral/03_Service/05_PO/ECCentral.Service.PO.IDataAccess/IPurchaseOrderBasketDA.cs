using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IPurchaseOrderBasketDA
    {
        /// <summary>
        /// 创建采购蓝商品
        /// </summary>
        /// <param name="basketInfo"></param>
        /// <returns></returns>
        BasketItemsInfo CreateBasketItem(BasketItemsInfo basketItemInfo);

        /// <summary>
        /// 根据编号加载采购篮商品信息
        /// </summary>
        /// <param name="basketSysNo"></param>
        /// <returns></returns>
        BasketItemsInfo LoadBasketItemBySysNo(int? basketSysNo);

        /// <summary>
        /// 通过采购篮商品加载赠品
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        List<BasketItemsInfo> LoadGiftItemByBasketItem(List<int> productSysNoList);

        /// <summary>
        /// 创建采购单(备货中心)
        /// </summary>
        /// <param name="prepareInfo"></param>
        /// <returns></returns>
        BasketItemsInfo CreateBasketItemForPrepare(BasketItemsInfo prepareInfo);

        /// <summary>
        /// 更新采购篮商品
        /// </summary>
        /// <param name="basketInfo"></param>
        /// <returns></returns>
        BasketItemsInfo UpdateBasketItem(BasketItemsInfo basketItemInfo);

        /// <summary>
        /// 删除采购篮
        /// </summary>
        /// <param name="basketSysNo"></param>
        /// <returns></returns>
        int DeleteBasket(int basketSysNo);

        /// <summary>
        /// 删除采购蓝商品
        /// </summary>
        /// <param name="basketItemInfo"></param>
        /// <returns></returns>
        BasketItemsInfo DeleteBasketItem(BasketItemsInfo basketItemInfo);

        /// <summary>
        /// 采购篮添加赠品
        /// </summary>
        /// <param name="basketInfo"></param>
        /// <returns></returns>
        BasketItemsInfo UpdateBasketItemForGift(BasketItemsInfo basketItemInfo);


        /// <summary>
        /// 检查商品是否已经存在于采购篮中
        /// </summary>
        /// <param name="basketItemInfo"></param>
        /// <returns></returns>
        bool CheckProductHasExistInBasket(BasketItemsInfo basketItemInfo);

        /// <summary>
        ///  验证要添加的赠品是否已在采购篮中
        /// </summary>
        /// <param name="basketInfo"></param>
        /// <param name="giftSysNo"></param>
        /// <returns></returns>
        int CheckGiftInBasket(BasketItemsInfo basketInfo, int giftSysNo);

        /// <summary>
        /// 加载采购篮赠品信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        BasketItemsInfo LoadBasketGiftInfo(int productSysNo);

        int GetItemSysNoByItemID(string itemID, string companyCode);

        int GetStockSysNoByName(string stockName, string companyCode);

        int? AvailableQtyByProductSysNO(int productSysNo);

        int? M1ByProductSysNO(int productSysNo);

        decimal? JDPriceByProductSysNO(int productSysNo);

        int GetVendorSysNoByProductNoAndStockSysNo(int productSysNo, int stockSysNo);

    }
}
