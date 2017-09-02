using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICategoryTemplateQueryDA))]
    public class CategoryTemplateQueryDA : ICategoryTemplateQueryDA
    {
        #region ICategoryTemplateQueryDA Members
        /// <summary>
        /// 根据Category3的SysNo获取类别模板
        /// </summary>
        /// <param name="C3SysNo"></param>
        /// <returns></returns>
        public List<CategoryTemplateInfo> GetCategoryTemplateListByC3SysNo(int? C3SysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetCategoryTemplateListByC3SysNo");
            dc.SetParameterValue("@C3SysNo", C3SysNo);
            List<CategoryTemplateInfo> data = dc.ExecuteEntityList<CategoryTemplateInfo>();
            return data;
        }

         /// <summary>
        /// 根据Category3的SysNo获取类别属性
        /// </summary>
        /// <param name="C3SysNo"></param>
        /// <returns></returns>
        public List<CategoryTemplatePropertyInfo> GetCategoryPropertyListByC3SysNo(int? C3SysNo)
        {
             DataCommand dc = DataCommandManager.GetDataCommand("GetCategoryPropertyListByC3SysNo");
            dc.SetParameterValue("@C3SysNo", C3SysNo);
            List<CategoryTemplatePropertyInfo> data = dc.ExecuteEntityList<CategoryTemplatePropertyInfo>();
            return data;
            
        } 
        }

        #endregion
    }

