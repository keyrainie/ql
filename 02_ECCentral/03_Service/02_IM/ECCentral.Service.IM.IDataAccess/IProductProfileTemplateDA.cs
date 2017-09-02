using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public  interface IProductProfileTemplateDA
    {
        /// <summary>
        /// 获取自定义查询模板列
        /// </summary>
        /// <returns></returns>
        List<ProductProfileTemplateInfo> GetProductProfileTemplateList(string userId, string templateType);

        /// <summary>
        /// 根据sysNo获取模板列
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ProductProfileTemplateInfo GetProductProfileTemplate(int sysNo);

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductProfileTemplateInfo InsertProductProfileTemplate(ProductProfileTemplateInfo entity);

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductProfileTemplateInfo DeleteProductProfileTemplate(ProductProfileTemplateInfo entity);


        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductProfileTemplateInfo UpdateProductProfileTemplate(ProductProfileTemplateInfo entity);


    }
}
