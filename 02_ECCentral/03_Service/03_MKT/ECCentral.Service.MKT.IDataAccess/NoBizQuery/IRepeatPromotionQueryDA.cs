using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IRepeatPromotionQueryDA
    {

        /// <summary>
        /// 查询销售规则
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetSaleRules(RepeatPromotionQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询赠品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetGifts(RepeatPromotionQueryFilter filter, out int totalCount);

        /// <summary>
        /// 查询优惠券
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetCoupons(RepeatPromotionQueryFilter filter, out int totalCount);

        /// <summary>
        ///  查询限时抢购
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetSaleCountDowns(RepeatPromotionQueryFilter filter, out int totalCount);

        /// <summary>
        ///  查询促销计划
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GeSaleCountDownPlan(RepeatPromotionQueryFilter filter, out int totalCount);

        /// <summary>
        ///  查询团购
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable GetProductGroupBuying(RepeatPromotionQueryFilter filter, out int totalCount);


    }
}
