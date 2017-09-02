using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
      [VersionExport(typeof(CategoryTemplateProcessor))]
   public class CategoryTemplateProcessor
    {
       /// <summary>
       /// 保存类别模板
       /// </summary>
       /// <param name="info"></param>
       public void SaveCategoryTemplate(List<CategoryTemplateInfo> list)
       {
           bool IsExists = ObjectFactory<ICategoryTemplateDA>.Instance.IsExistsCategoryTemplate(list.First());
           foreach (var item in list)
           {
               if (IsExists)
               {
                   ObjectFactory<ICategoryTemplateDA>.Instance.UpdateCategoryTemplate(item);
               }
               else
               {
                   ObjectFactory<ICategoryTemplateDA>.Instance.InsertCategoryTemplate(item);
               }
               
           }
                
       }

      
    
      
    }
}
