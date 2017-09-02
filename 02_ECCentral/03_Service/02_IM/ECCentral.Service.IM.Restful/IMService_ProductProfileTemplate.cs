using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        /// <summary>
        /// 获取自定义查询模板列
        /// </summary>
        /// <returns></returns>
         [WebGet(UriTemplate = "/ProductProfileTemplate/QueryProductProfileTemplateList/{userId}/{templateType}")]
        public virtual List<ProductProfileTemplateInfo> QueryProductProfileTemplateList(string userId, string templateType)
        {
            var result = ObjectFactory<ProductProfileTemplateAppService>.Instance.QueryProductProfileTemplateList(userId, templateType);
            return result;
        }

        /// <summary>
        /// 根据sysNo获取模板列
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
         [WebInvoke(UriTemplate = "/ProductProfileTemplate/QueryProductProfileTemplate", Method = "POST")]
         [DataTableSerializeOperationBehavior]
        public virtual ProductProfileTemplateInfo QueryProductProfileTemplate(int sysNo)
        {
            var result = ObjectFactory<ProductProfileTemplateAppService>.Instance.QueryProductProfileTemplate(sysNo);
            return result;
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         [WebInvoke(UriTemplate = "/ProductProfileTemplate/CreateProductProfileTemplate", Method = "POST")]
         [DataTableSerializeOperationBehavior]
        public virtual ProductProfileTemplateInfo CreateProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            var result = ObjectFactory<ProductProfileTemplateAppService>.Instance.CreateProductProfileTemplate(entity);
            return result;
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         [WebInvoke(UriTemplate = "/ProductProfileTemplate/DeleteProductProfileTemplate", Method = "DELETE")]
         [DataTableSerializeOperationBehavior]
        public ProductProfileTemplateInfo DeleteProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            var result = ObjectFactory<ProductProfileTemplateAppService>.Instance.DeleteProductProfileTemplate(entity);
            return result;
        }

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
         [WebInvoke(UriTemplate = "/ProductProfileTemplate/ModifyProductProfileTemplate", Method = "PUT")]
         [DataTableSerializeOperationBehavior]
        public ProductProfileTemplateInfo ModifyProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            var result = ObjectFactory<ProductProfileTemplateAppService>.Instance.ModifyProductProfileTemplate(entity);
            return result;
        }


    }
}
