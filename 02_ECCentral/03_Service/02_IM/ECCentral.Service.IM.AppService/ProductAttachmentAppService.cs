using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductAttachmentAppService))]
    public class ProductAttachmentAppService
    {
        /// <summary>
        /// 创建商品附件
        /// </summary>
        /// <param name="mainProductEntity"></param>
        /// <returns></returns>
        public virtual void CreateProductAttachment(ProductInfo mainProductEntity)
        {
            ObjectFactory<ProductAttachmentProcessor>.Instance.CreateProductAttachment(mainProductEntity);
        }


        /// <summary>
        /// 创建商品附件
        /// </summary>
        /// <param name="mainProductEntity"></param>
        /// <returns></returns>
        public virtual void ModifyProductAttachment(ProductInfo mainProductEntity)
        {
            ObjectFactory<ProductAttachmentProcessor>.Instance.ModifyProductAttachment(mainProductEntity);
        }

        /// <summary>
        /// 根据商品SysNo获取商品附件信息组
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<ProductAttachmentInfo> GetProductAttachmentList(int productSysNo)
        {
            List<ProductAttachmentInfo> result =
                ObjectFactory<ProductAttachmentProcessor>.Instance.GetProductAttachmentList(productSysNo);
            return result;
        }

        /// <summary>
        /// 删除附件信息
        /// </summary>
        /// <param name="productSysNo"></param>
        public void DeleteAttachmentByProductSysNo(int productSysNo)
        {
            ObjectFactory<ProductAttachmentProcessor>.Instance.DeleteAttachmentByProductSysNo(productSysNo);            
        }
    }
}
