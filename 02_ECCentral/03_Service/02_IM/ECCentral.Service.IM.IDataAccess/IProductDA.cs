using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductDA
    {
        /// <summary>
        /// 根据商品SysNo获取完整商品信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        ProductInfo GetProductInfoBySysNo(int productSysNo);

        /// <summary>
        /// 获得简单商品实体
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        ProductInfo GetSimpleProductInfoBySysNo(int productSysNo);

        /// <summary>
        /// 根据商品ID获取完整商品信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        ProductInfo GetProductInfoByID(string productID);

        /// <summary>
        /// 获取除自己以外的相同品牌、类别、型号、商家的商品数量
        /// </summary>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        int GetProductCountExcept(ProductInfo productInfo);

        /// <summary>
        /// 创建Product
        /// </summary>
        /// <param name="productInfo"></param>
        void InsertProductInfo(ProductInfo productInfo);

        /// <summary>
        /// 更新商品基本信息
        /// </summary>
        /// <param name="productInfo"></param>
        void UpdateProductBasicInfo(ProductInfo productInfo);


        /// <summary>
        /// 批量修改商品信息是调用（修改的内容：商品标题（和商品名称）、商品简名、简名附加、商品的描述）
        /// </summary>
        /// <param name="productInfo"></param>
        void UpdateProductBasicInfoWhenBatchUpdate(ProductInfo productInfo);

        /// <summary>
        /// 更新全组商品品牌信息
        /// </summary>
        /// <param name="productGroup"></param>
        /// <param name="brandInfo"></param>
        /// <param name="operationUser"> </param>
        void UpdateGroupProductBrandInfo(ProductGroup productGroup, BrandInfo brandInfo, UserInfo operationUser);

        /// <summary>
        /// 更新全组商品类别信息
        /// </summary>
        /// <param name="productGroup"></param>
        /// <param name="categoryInfo"></param>
        /// <param name="operationUser"> </param>
        void UpdateGroupProductCategoryInfo(ProductGroup productGroup, CategoryInfo categoryInfo, UserInfo operationUser);

        /// <summary>
        /// 更新商品状态
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int UpdateProductStatus(int productSysNo, ProductStatus status);

        /// <summary>
        /// 更新商品采购相关信息
        /// </summary>
        /// <param name="productInfo"></param>
        void UpdateProductPurchaseInfo(ProductInfo productInfo);

        ///// <summary>
        ///// 更新商品库存同步合作信息
        ///// </summary>
        ///// <param name="productInfo"> </param>
        //void UpdateProductThirdPartyInventory(ProductInfo productInfo);

        /// <summary>
        /// 更新商品促销类型
        /// </summary>
        /// <param name="productInfo"></param>
        void UpdateProductPromotionType(ProductInfo productInfo);

        /// <summary>
        /// 更新商品上架时间（第1次上架时间、最后1次上架时间）
        /// </summary>
        /// <param name="productSysNo"></param>
        void UpdateProductOnlineTime(int productSysNo);

        /// <summary>
        /// 更新商品信息已完善状态
        /// </summary>
        /// <param name="productInfo"></param>
        void UpdateProductInfoFinishStatus(ProductInfo productInfo);

        /// <summary>
        /// 更新商品是否参加延保
        /// </summary>
        /// <param name="productInfo"></param>
        void UpdateProductIsNoExtendWarranty(ProductInfo productInfo);

        /// <summary>
        /// 更新商品自动调价信息
        /// </summary>
        /// <param name="productInfo"></param>
        void UpdateProductAutoPriceInfo(ProductInfo productInfo);

        /// <summary>
        /// 更新商品源商品信息
        /// </summary>
        /// <param name="sourceProductID"></param>
        /// <param name="productSysNo"></param>
        void UpdateSourceProductID(string sourceProductID, int productSysNo);

        #region ProductRelationObjectDataAccess

        /// <summary>
        /// 添加商品配件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="productAccessory"></param>
        void InsertProductAccessory(int productSysNo, ProductAccessory productAccessory);

        /// <summary>
        /// 删除商品配件
        /// </summary>
        /// <param name="productSysNo"></param>
        void DeleteProductAccessory(int productSysNo);

        /// <summary>
        /// 添加商品销售区域
        /// </summary>
        /// <param name="productInfo"></param>
        /// <param name="productSalesAreaInfo"></param>
        void InsertProductSalesArea(ProductInfo productInfo, ProductSalesAreaInfo productSalesAreaInfo);

        /// <summary>
        /// 删除商品销售区域
        /// </summary>
        /// <param name="productSysNo"></param>
        void DeleteProductSalesArea(int productSysNo);

        /// <summary>
        /// 删除商品时效性促销语
        /// </summary>
        /// <param name="productSysNo"></param>
        void DeleteProductTimelyPromotionTitle(int productSysNo);

        /// <summary>
        /// 添加商品时效性促销语
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="productInfo"></param>
        void InsertProductTimelyPromotionTitle(int productSysNo,
                                               ProductInfo productInfo);

        #endregion

        List<ProductInfo> GetProductListByProductGroupSysNo(int productGroupSysNo);

        IEnumerable<int> GetProductSysNoListByProductGroupSysNo(int productGroupSysNo);

        /// <summary>
        /// 获取克隆次数
        /// </summary>
        /// <param name="productID">商品ID</param>
        /// <param name="sixthLetter">克隆类型</param>
        /// <returns>克隆次数</returns>
        int GetCloneCount(string productID, string sixthLetter);

        /// <summary>
        /// 根据商品ID查询商品列表
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        List<ProductInfo> GetSimpleProductListByID(string productID);

        /// <summary>
        /// 获取商品的组系统编号
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns></returns>
        int GetProductGroupSysNo(int productSysNo);

        /// <summary>
        /// 根据商品编号列表 获取商品关税信息列表
        /// </summary>
        /// <param name="sysNos"></param>
        /// <returns></returns>
        System.Data.DataTable GetProductTariffInfoProductSysNos(List<int> sysNos);

        /// <summary>
        /// 获取同系商品ID前缀最大的商品ID
        /// </summary>
        /// <param name="code">商品ID前缀，如：xx_xxx_xxx</param>
        /// <returns></returns>
        string GetProductSameIDMaxProductID(string code);

        /// <summary>
        /// 插入商品产地信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="origin">产地</param>
        void InsertProductEntryInfo(int productSysNo, string origin);

        /// <summary>
        /// 获取商品产地列表
        /// </summary>
        /// <returns></returns>
        List<ProductCountry> GetProductCountryList();

        /// <summary>
        /// 更新商品采购价格
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="virtualPrice">采购价格</param>
        void UpdateProductVirtualPrice(string productSysNo, string virtualPrice);

        /// <summary>
        /// 商品审核
        /// </summary>
        /// <param name="productSysNos"></param>
        /// <param name="status"></param>
        void BatchAuditProduct(List<int> productSysNos, ProductStatus status);
    }
}
