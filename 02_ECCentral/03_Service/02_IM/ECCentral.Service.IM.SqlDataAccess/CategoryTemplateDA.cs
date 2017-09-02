using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
     [VersionExport(typeof(ICategoryTemplateDA))]
   public class CategoryTemplateDA : ICategoryTemplateDA
    {
       

        #region ICategoryTemplateDA Members


        public bool IsExistsCategoryTemplate(CategoryTemplateInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("IsExistsCategoryTemplate");
            dc.SetParameterValue("@C3SysNo", info.TargetSysNo);
            dc.ExecuteNonQuery();
            return (int)dc.GetParameterValue("@flag")<0;
        }

        public void UpdateCategoryTemplate(CategoryTemplateInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateCategoryTemplate");
            dc.SetParameterValue("@C3SysNo", info.TargetSysNo);
            dc.SetParameterValue("@LastEditUserSysNo", info.User.SysNo);
            dc.SetParameterValue("@TemplateType", info.TemplateType);
            dc.SetParameterValue("@Description", info.Templates);
            dc.ExecuteNonQuery();
        }

        public void InsertCategoryTemplate(CategoryTemplateInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertCategoryTemplate");
            dc.SetParameterValue("@C3SysNo", info.TargetSysNo);
            dc.SetParameterValue("@LastEditUserSysNo", info.User.SysNo);
            dc.SetParameterValue("@TemplateType", info.TemplateType);
            dc.SetParameterValue("@Description", info.Templates);
            dc.ExecuteNonQuery();
        }

        #endregion

    }
}
