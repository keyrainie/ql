using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductRelateAppService))]
    public class ProductRelateAppService
    {
        /// <summary>
        /// Create ProductRelated
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ProductRelatedInfo CreateProductRelated(ProductRelatedInfo info)
        {
            return ObjectFactory<ProductRelateProcessor>.Instance.CreateItemRelated(info);
        }
        public virtual void DeleteItemRelated(List<string> list)
        {
            ObjectFactory<ProductRelateProcessor>.Instance.DeleteItemRelated(list);
        }
        public void UpdateProductRelatePriority(List<ProductRelatedInfo> list)
        {
            ObjectFactory<ProductRelateProcessor>.Instance.UpdateProductRelatePriority(list);


        }
        /// <summary>
        /// 批量设置相关商品
        /// </summary>
        /// <param name="listInfo"></param>
        public void CreateItemRelatedByList(List<ProductRelatedInfo> listInfo)
        {
            ObjectFactory<ProductRelateProcessor>.Instance.CreateItemRelatedByList(listInfo);
        }
    }
}