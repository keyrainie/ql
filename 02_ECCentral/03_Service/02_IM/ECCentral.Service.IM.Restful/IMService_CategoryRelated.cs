using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.QueryFilter.IM;
using System.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.Restful
{
   public partial class IMService
    {
       /// <summary>
       /// 根据query得到相关类别信息
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
        [WebInvoke(UriTemplate = "/CategoryRelated/GetCategoryRelatedByQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
       public virtual QueryResult GetCategoryRelatedByQuery(CategoryRelatedQueryFilter query)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICategoryRelatedDA>.Instance.GetCategoryRelatedByQuery(query, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建新的相关类别
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/CategoryRelated/CreateCategoryRelated", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual void CreateCategoryRelated(CategoryRelatedInfo info)
        {
            ObjectFactory<CategoryRelatedAppService>.Instance.CreateCategoryRelated(info);
        }

        /// <summary>
        /// 批量删除相关类别
        /// </summary>
        /// <param name="sysnos"></param>
        [WebInvoke(UriTemplate = "/CategoryRelated/DeleteCategoryRelated", Method = "DELETE")]
        [DataTableSerializeOperationBehavior]
        public virtual void DeleteCategoryRelated(List<string> sysnos)
        {
            ObjectFactory<CategoryRelatedAppService>.Instance.DeleteCategoryRelated(sysnos);
        }
    }
}
