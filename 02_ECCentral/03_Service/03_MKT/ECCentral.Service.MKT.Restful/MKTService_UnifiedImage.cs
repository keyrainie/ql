using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Web;

using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 统一图片查询
        /// </summary>
        [WebInvoke(UriTemplate = "/UnifiedImage/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryUnifiedImages(UnifiedImageQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<IUnifiedImageQueryDA>.Instance.Query(msg, out totalCount);
            return new QueryResult()
            {
                Data = ds,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 新建图片
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/UnifiedImage/Create", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual void CreateUnifiedImage(UnifiedImage entity)
        {
            ObjectFactory<UnifiedImageAppService>.Instance.CreateUnifiedImage(entity);
        }

    }
}
