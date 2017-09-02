using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(ProductProfileTemplateAppService))]
    public class ProductProfileTemplateAppService
    {
       

        /// <summary>
        /// 获取自定义查询模板列
        /// </summary>
        /// <returns></returns>
        public virtual List<ProductProfileTemplateInfo> QueryProductProfileTemplateList(string userId, string templateType)
        {
            var result=ObjectFactory<ProductProfileTemplateProcessor>.Instance.QueryProductProfileTemplateList(userId,templateType);
            return result;
        }

        /// <summary>
        /// 根据sysNo获取模板列
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductProfileTemplateInfo QueryProductProfileTemplate(int sysNo)
        {
            var result = ObjectFactory<ProductProfileTemplateProcessor>.Instance.QueryProductProfileTemplate(sysNo);
            return result;
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductProfileTemplateInfo CreateProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            var result = ObjectFactory<ProductProfileTemplateProcessor>.Instance.CreateProductProfileTemplate(entity);
            return result;
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductProfileTemplateInfo DeleteProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            var result = ObjectFactory<ProductProfileTemplateProcessor>.Instance.DeleteProductProfileTemplate(entity);
            return result;
        }

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductProfileTemplateInfo ModifyProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            var result = ObjectFactory<ProductProfileTemplateProcessor>.Instance.ModifyProductProfileTemplate(entity);
            return result;
        }


    }
}
