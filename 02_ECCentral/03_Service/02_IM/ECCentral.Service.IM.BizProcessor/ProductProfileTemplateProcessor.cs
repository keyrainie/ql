using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductProfileTemplateProcessor))]
    public class ProductProfileTemplateProcessor
    {
        private readonly IProductProfileTemplateDA _templateDA = ObjectFactory<IProductProfileTemplateDA>.Instance;

        /// <summary>
        /// 获取自定义查询模板列
        /// </summary>
        /// <returns></returns>
        public virtual List<ProductProfileTemplateInfo> QueryProductProfileTemplateList(string userId, string templateType)
        {
            CheckProductProfileTemplateProcessor.CheckUserIDAndTemplateType(userId, templateType);
            var result=_templateDA.GetProductProfileTemplateList(userId, templateType);
            return result;
        }

        /// <summary>
        /// 根据sysNo获取模板列
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ProductProfileTemplateInfo QueryProductProfileTemplate(int sysNo)
        {
            CheckProductProfileTemplateProcessor.CheckSysNo(sysNo);
            var result = _templateDA.GetProductProfileTemplate(sysNo);
            return result;
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ProductProfileTemplateInfo CreateProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            CheckProductProfileTemplateProcessor.CheckProductProfileTemplateInfo(entity);
            var result = _templateDA.InsertProductProfileTemplate(entity);
            return result;
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductProfileTemplateInfo DeleteProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            CheckProductProfileTemplateProcessor.CheckProductProfileTemplateInfo(entity);
            CheckProductProfileTemplateProcessor.CheckSysNo(entity.SysNo);
            var result = _templateDA.DeleteProductProfileTemplate(entity);
            return result;
        }

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductProfileTemplateInfo ModifyProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            CheckProductProfileTemplateProcessor.CheckProductProfileTemplateInfo(entity);
            CheckProductProfileTemplateProcessor.CheckSysNo(entity.SysNo);
            var result = _templateDA.UpdateProductProfileTemplate(entity);
            return result;
        }

        #region 检查自定义查询逻辑
        private static class CheckProductProfileTemplateProcessor
        {
            /// <summary>
            /// 检查自定义查询实体
            /// </summary>
            /// <param name="entity"></param>
            public static void CheckProductProfileTemplateInfo(ProductProfileTemplateInfo entity)
            {
                if (entity == null)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductProfileTemplate", "TemplateIsNull"));
                }
                if (String.IsNullOrWhiteSpace(entity.TemplateType))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductProfileTemplate", "TemplateTypeIsNull"));
                }
                if (String.IsNullOrWhiteSpace(entity.TemplateName))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductProfileTemplate", "TemplateNameIsNull"));
                }
                if (String.IsNullOrWhiteSpace(entity.TemplateValue))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductProfileTemplate", "TemplateValueIsNull"));
                }
                if (String.IsNullOrWhiteSpace(entity.UserId))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductProfileTemplate", "UserIdIsNull"));
                }
            }

            /// <summary>
            /// 检查自定义查询类别
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="templateType"></param>
            public static void CheckUserIDAndTemplateType(string userId, string templateType)
            {

                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductProfileTemplate", "UserIdIsNull"));
                }
                if (String.IsNullOrWhiteSpace(templateType))
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductProfileTemplate", "TemplateTypeIsNull"));
                }
            }

            /// <summary>
            /// 检查自定义查询编号
            /// </summary>
            /// <param name="sysNo"></param>
            public static void CheckSysNo(int? sysNo)
            {
                if (!sysNo.HasValue||sysNo <= 0)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductProfileTemplate", "TemplateSysNoIsNull"));
                }
            }
        }
        #endregion

    }
}
