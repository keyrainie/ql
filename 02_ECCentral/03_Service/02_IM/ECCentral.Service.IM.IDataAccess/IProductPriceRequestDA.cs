using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductPriceRequestDA
    {
        /// <summary>
        /// 更新商品价格审核申请状态
        /// </summary>
        /// <param name="productPriceRequest"></param>
        void UpdateProductPriceRequestStatus(ProductPriceRequestInfo productPriceRequest);

        /// <summary>
        /// 插入商品价格审核请求
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="productPriceRequestInfo"></param>
        void InsertProductPriceRequest(int productSysNo, ProductPriceRequestInfo productPriceRequestInfo);

        /// <summary>
        /// 根据tPriceRequestSysNo获取商品价格变动信息
        /// </summary>
        /// <param name="productPriceRequestSysNo"></param>
        /// <returns></returns>
        ProductPriceRequestInfo GetProductPriceRequestInfoBySysNo(int productPriceRequestSysNo);

        /// <summary>
        /// 获取当前商品会员价格更改请求
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<ProductRankPriceInfo> GetProductRequestRankPriceBySysNo(int productSysNo);

        /// <summary>
        /// 获取当前商品批发价格更改请求
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<ProductWholeSalePriceInfo> GetProductRequestWholeSalePriceInfoBySysNo(int productSysNo);

        /// <summary>
        /// 获取商品最后一次价格申请信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        ProductPriceRequestInfo GetProductLastProductPriceRequestInfo(int productSysNo);

        /// <summary>
        /// 更新商品价格审核申请状态
        /// </summary>
        /// <param name="productPriceRequestSysNo"></param>
        /// <param name="status"></param>
        void UpdateProductPriceRequestStatus(int productPriceRequestSysNo, ProductPriceRequestStatus status);

        /// <summary>
        /// 获取商品编号
        /// </summary>
        /// <param name="productPriceRequestSysNo"></param>
        /// <returns></returns>
        int GetProductSysNoBySysNo(int productPriceRequestSysNo);
    }
}
