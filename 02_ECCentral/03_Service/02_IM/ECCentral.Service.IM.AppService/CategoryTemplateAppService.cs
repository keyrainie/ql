using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
     [VersionExport(typeof(CategoryTemplateAppService))]
   public class CategoryTemplateAppService
    {
        /// <summary>
        /// 保存类别模板
        /// </summary>
        /// <param name="info"></param>
        public void SaveCategoryTemplate(List<CategoryTemplateInfo> list)
        {

            ObjectFactory<CategoryTemplateProcessor>.Instance.SaveCategoryTemplate(list);
            
           
        }
    }
}
