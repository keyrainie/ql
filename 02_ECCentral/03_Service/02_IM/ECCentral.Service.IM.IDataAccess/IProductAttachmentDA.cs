using System.Collections.Generic;

using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductAttachmentDA
    {


        /// <summary>
        /// 创建商品附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="attachmentEntity"></param>
        /// <returns></returns>
        ProductAttachmentInfo InsertAttachment(int productSysNo, ProductAttachmentInfo attachmentEntity);

        /// <summary>
        /// 是否某个商品存在附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        bool IsExistProductAttachment(int productSysNo);

        /// <summary>
        ///  删除商品附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        void DeleteAttachmentByProductSysNo(int productSysNo);

        /// <summary>
        /// 根据商品SysNo获取商品附件信息组
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<ProductAttachmentInfo> GetProductAttachmentList(int productSysNo);

        /// <summary>
        /// 是否为配件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        bool IsProductAttachment(int productSysNo);
    }
}
