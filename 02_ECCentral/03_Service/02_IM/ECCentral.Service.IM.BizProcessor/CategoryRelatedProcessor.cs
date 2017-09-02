using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(CategoryRelatedProcessor))]
  public  class CategoryRelatedProcessor
    {
      private readonly ICategoryRelatedDA categoryRelatedDA = ObjectFactory<ICategoryRelatedDA>.Instance;

      /// <summary>
      /// 创建新的相关类别
      /// </summary>
      /// <param name="info"></param>
      public virtual void CreateCategoryRelated(CategoryRelatedInfo info)
      {
         int result= categoryRelatedDA.CreateCategoryRelated(info);
         if (result == -1)
         {
             throw new BizException(ResouceManager.GetMessageString("IM.Category", "ExistsRelatedCategory"));
         }
      }

      /// <summary>
      /// 批量删除相关类别
      /// </summary>
      /// <param name="sysnos"></param>
      public virtual void DeleteCategoryRelated(List<string> sysnos)
      {
          foreach (var item in sysnos)
          {
              categoryRelatedDA.DeleteCategoryRelated(item);
          }
          
      }
    }
}
