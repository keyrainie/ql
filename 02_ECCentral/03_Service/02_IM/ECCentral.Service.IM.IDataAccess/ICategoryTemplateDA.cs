using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface ICategoryTemplateDA
    {
      
     

       bool IsExistsCategoryTemplate(CategoryTemplateInfo info);

       void UpdateCategoryTemplate(CategoryTemplateInfo info);

       void InsertCategoryTemplate(CategoryTemplateInfo info);
    }
}
