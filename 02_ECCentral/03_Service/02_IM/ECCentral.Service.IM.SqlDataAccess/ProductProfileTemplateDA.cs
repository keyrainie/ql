using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;

using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductProfileTemplateDA))]
    public class ProductProfileTemplateDA : IProductProfileTemplateDA
    {
        /// <summary>
        /// 获取自定义查询模板列
        /// </summary>
        /// <returns></returns>
        public List<ProductProfileTemplateInfo> GetProductProfileTemplateList(string userId, string templateType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductProfileTemplateList");
            cmd.SetParameterValue("@UserId", userId);
            cmd.SetParameterValue("@TemplateType", templateType);
            var result = cmd.ExecuteEntityList<ProductProfileTemplateInfo>();
            return result;
        }

        /// <summary>
        /// 根据sysNo获取模板列
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ProductProfileTemplateInfo GetProductProfileTemplate(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductProfileTemplate");
            cmd.SetParameterValue("@SysNo", sysNo);
            var result = cmd.ExecuteEntity<ProductProfileTemplateInfo>();
            return result;
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductProfileTemplateInfo InsertProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductProfileTemplate");
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.SetParameterValue("@TemplateName", entity.TemplateName);
            cmd.SetParameterValue("@TemplateType", entity.TemplateType);
            cmd.SetParameterValue("@TemplateValue", entity.TemplateValue);
            cmd.SetParameterValue("@UserId", entity.UserId);
            cmd.SetParameterValue("@Description", entity.Description);
            cmd.SetParameterValue("@InDate", DateTime.Now);
            cmd.SetParameterValue("@InUser", entity.UserName);
            cmd.SetParameterValue("@ReferenceSysNo", entity.ReferenceSysNo);
            cmd.ExecuteNonQuery();
            var result = (int)cmd.GetParameterValue("@SysNo");
            entity.SysNo = result;
            return entity;
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductProfileTemplateInfo DeleteProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductProfileTemplate");
            cmd.SetParameterValue("@TemplateName", entity.TemplateName);
            cmd.SetParameterValue("@TemplateType", entity.TemplateType);
            cmd.SetParameterValue("@UserId", entity.UserId);
            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductProfileTemplateInfo UpdateProductProfileTemplate(ProductProfileTemplateInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductProfileTemplate");
            cmd.SetParameterValue("@TemplateName", entity.TemplateName);
            cmd.SetParameterValue("@TemplateValue", entity.TemplateValue);
            cmd.SetParameterValue("@Description", entity.Description);
            cmd.SetParameterValue("@EditUser", entity.UserName);
            cmd.SetParameterValue("@ReferenceSysNo", entity.ReferenceSysNo);
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.ExecuteNonQuery();
            return entity;
        }
    }
}
