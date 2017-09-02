using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region  查询
        /// <summary>
        /// 查询商品附件信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAttachment/QueryProductAttachmentList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductAttachmentList(ProductAttachmentQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductAttachment", "RequestIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IProductAttachmentQueryDA>.Instance.QueryProductAttachment(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据商品SysNo获取商品附件信息组
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAttachment/GetProductAttachmentList", Method = "POST")]
        public List<ProductAttachmentInfo> GetProductAttachmentList(int productSysNo)
        {
            List<ProductAttachmentInfo> result =
                ObjectFactory<ProductAttachmentAppService>.Instance.GetProductAttachmentList(productSysNo);
            return result;
        }
        #endregion

        #region 修改、添加以及删除操作
        /// <summary>
        /// 创建商品附件
        /// </summary>
        /// <param name="mainProductEntity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAttachment/CreateProductAttachment", Method = "POST")]
        public void CreateProductAttachment(ProductInfo mainProductEntity)
        {
            ObjectFactory<ProductAttachmentAppService>.Instance.CreateProductAttachment(mainProductEntity);
        }

        /// <summary>
        /// 修改商品附件
        /// </summary>
        /// <param name="mainProductEntity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ProductAttachment/ModifyProductAttachment", Method = "PUT")]
        public void ModifyProductAttachment(ProductInfo mainProductEntity)
        {
            ObjectFactory<ProductAttachmentAppService>.Instance.ModifyProductAttachment(mainProductEntity);
        }

        /// <summary>
        /// 删除附件信息
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/ProductAttachment/DeleteProductAttachmentByProductSysNo", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public void DeleteAttachmentByProductSysNo(int productSysNo)
        {
            ObjectFactory<ProductAttachmentAppService>.Instance.DeleteAttachmentByProductSysNo(productSysNo);
        }
        #endregion

    }
}
