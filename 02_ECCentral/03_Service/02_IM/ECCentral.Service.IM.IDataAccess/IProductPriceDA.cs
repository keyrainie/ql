//************************************************************************
// 用户名				泰隆优选
// 系统名				商品价格变动申请单据
// 子系统名		        商品价格变动申请单据业务数据底层接口
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductPriceDA
    {
        /// <summary>
        /// 商品价格更新（市场价、付款方式,每天最大订购数,折扣,每单最小订购数）
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="productPriceInfo"> </param>
        /// <returns></returns>
        void UpdateProductBasicPrice(int productSysNo, ProductPriceInfo productPriceInfo);

        /// <summary>
        /// 商品价格更新（售价、返现、积分、批发价、会员价、VIP专享价）
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="productPriceInfo"></param>
        void UpdateProductPrice(int productSysNo, ProductPriceInfo productPriceInfo);

        /// <summary>
        /// 更新商品正常采购价格
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="originalVirtualPrice"> </param>
        /// <param name="virtualPrice"></param>
        void UpdateProductVirtualPrice(int productSysNo, decimal originalVirtualPrice, decimal virtualPrice);

        /// <summary>
        /// 更新商品是否同步门店价格属性
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="originalVirtualPrice"> </param>
        /// <param name="virtualPrice"></param>
        void UpdateProductSyncShopPrice(int productSysNo, IsSyncShopPrice isSyncShopPrice);

        /// <summary>
        /// 获取商品会员价格
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<ProductRankPriceInfo> GetProductRankPriceBySysNo(int productSysNo);

        /// <summary>
        /// 获取商品批发价格
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<ProductWholeSalePriceInfo> GetWholeSalePriceInfoBySysNo(int productSysNo);

        /// <summary>
        /// 某段时间内调价的日志
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        PriceChangeLogInfo GetPriceChangeLogInfoByProductSysNo(int productSysNo, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 更新商品市场售价
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="basicPrice"></param>
        void UpdateProductBasicPriceOnly(int productSysNo, decimal basicPrice);

        /// <summary>
        /// 更新商品销售价格
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="currentPrice"></param>
        void UpdateProductCurrentPriceOnly(int productSysNo, decimal currentPrice);
    }
}
