//************************************************************************
// 用户名				泰隆优选
// 系统名				商品批量移类
// 子系统名		        商品批量移类业务逻辑实现
// 作成者				Kevin
// 改版日				2012.6.7
// 改版内容				新建
//************************************************************************
using System.Linq;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;



namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductBatchChangeCategoryProcessor))]
    public class ProductBatchChangeCategoryProcessor
    {
        private readonly ICategoryPropertyDA _categoryPropertyDA = ObjectFactory<ICategoryPropertyDA>.Instance;
        private readonly IProductGroupDA _productGroupDA = ObjectFactory<IProductGroupDA>.Instance;

        #region 商品移类业务方法      

        /// <summary>
        ///  更新商品请求
        /// </summary>
        /// <param name="productID"> </param>
        /// <param name="categoryInfo"> </param>
        /// <returns></returns>
        public void ProductChangeCategory(string productID, CategoryInfo categoryInfo,UserInfo operateUser)
        {
            ProductProcessor productBp = new ProductProcessor();

            ProductInfo product = productBp.GetProductInfoByID(productID);

            if (product == null)
            {
                //商品不存在
                throw new BizException(ResouceManager.GetMessageString("IM.Product", "ProductIDNotExist"));
            }

            

            #region 类别处理

            var group = _productGroupDA.GetProductGroup(product.SysNo);
            group.OperateUser = operateUser;
            product.OperateUser = operateUser;

            if (product.ProductBasicInfo.ProductCategoryInfo.SysNo != categoryInfo.SysNo)
            {              

                if (product.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue)
                {
                    if (_categoryPropertyDA.GetCategoryPropertyByCategorySysNo(
                        product.ProductBasicInfo.ProductCategoryInfo.SysNo.Value).Any(sourceCategoryProperty => !_categoryPropertyDA.GetCategoryPropertyByCategorySysNo(
                        product.ProductBasicInfo.ProductCategoryInfo.SysNo.Value).Any(p => p.Property.SysNo == sourceCategoryProperty.Property.SysNo)))
                    {
                        throw new BizException(ResouceManager.GetMessageString("IM.Category", "ProductChangeCategoryResult"));
                    }

                    product.ProductBasicInfo.ProductCategoryInfo = categoryInfo;

                    productBp.UpdateGroupProductCategoryInfo(group, product);
                }
            }

            #endregion


        }     

        #endregion


    }

}
