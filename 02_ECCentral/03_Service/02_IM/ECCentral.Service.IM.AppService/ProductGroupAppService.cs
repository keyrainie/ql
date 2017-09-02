using System.Transactions;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using System.Collections.Generic;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductGroupAppService))]
    public class ProductGroupAppService
    {
        private readonly ProductGroupProcessor _productGroupProcessor = ObjectFactory<ProductGroupProcessor>.Instance;

        /// <summary>
        /// 根据商品SysNo获取该商品所在商品组信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public ProductGroup GetProductGroup(int productSysNo)
        {
            return _productGroupProcessor.GetProductGroup(productSysNo);
        }

        /// <summary>
        /// 根据SysNo获取商品组信息
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        /// <returns></returns>
        public ProductGroup GetProductGroupInfoBySysNo(int productGroupSysNo)
        {
            return _productGroupProcessor.GetProductGroupInfoBySysNo(productGroupSysNo);
        }

        /// <summary>
        /// 创建商品组
        /// </summary>
        /// <param name="productGroup"></param>
        /// <returns></returns>
        public ProductGroup CreateProductGroupInfo(ProductGroup productGroup)
        {
            using (var tran = new TransactionScope())
            {
                productGroup = _productGroupProcessor.CreateProductGroupInfo(productGroup);
                tran.Complete();
                return productGroup;
            }
        }

        /// <summary>
        /// 编辑商品组信息
        /// </summary>
        /// <param name="productGroup"></param>
        /// <returns></returns>
        public ProductGroup UpdateProductGroupInfo(ProductGroup productGroup)
        {
            using (var tran = new TransactionScope())
            {
                productGroup = _productGroupProcessor.UpdateProductGroupInfo(productGroup);
                tran.Complete();
                return productGroup;
            }
        }


        /// <summary>
        /// 获取商品所在商品组的其它商品ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<string> GetProductGroupIDSFromProductID(string productID)
        {
            return _productGroupProcessor.GetProductGroupIDSFromProductID(productID);
        }
    }
}
