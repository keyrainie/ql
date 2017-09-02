using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
   public interface ICategoryTemplateQueryDA
    {
        /// <summary>
        /// 根据Category3的SysNo获取类别模板
        /// </summary>
        /// <param name="C3SysNo"></param>
        /// <returns></returns>
       List<CategoryTemplateInfo> GetCategoryTemplateListByC3SysNo(int? C3SysNo);

        /// <summary>
        /// 根据Category3的SysNo获取类别属性
        /// </summary>
        /// <param name="C3SysNo"></param>
        /// <returns></returns>
       List<CategoryTemplatePropertyInfo> GetCategoryPropertyListByC3SysNo(int? C3SysNo);
    }
}
