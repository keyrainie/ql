using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;
using System.Data;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface ICategoryRelatedDA
    {
       /// <summary>
       /// 根据query得到相关类别信息
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       DataTable GetCategoryRelatedByQuery(CategoryRelatedQueryFilter query, out int totalCount);
       /// <summary>
       /// 添加相关类别
       /// </summary>
       /// <param name="info"></param>
       int CreateCategoryRelated(CategoryRelatedInfo info);
       /// <summary>
       /// 删除相关类别
       /// </summary>
       /// <param name="sysno"></param>
       void DeleteCategoryRelated(string sysno);
    }
}
