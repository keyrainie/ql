using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IAdvertiserQueryDA
    {
        /// <summary>
        /// 广告商查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryAdvertiser(AdvertiserQueryFilter filter, out int totalCount);

        /// <summary>
        /// 广告效果查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryAdvEffect(AdvEffectQueryFilter filter, out int totalCount);

        /// <summary>
        /// 广告效果BBS查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryAdvEffectBBS(AdvEffectBBSQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询订阅维护
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QuerySubscription(SubscriptionQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询订阅分类
        /// </summary>
        /// <returns></returns>
        List<SubscriptionCategory> QuerySubscriptionCategory();
    }
}
