using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using ECommerce.DataAccess.GroupBuying;
using ECommerce.Entity;
using ECommerce.Entity.Order;
using ECommerce.Entity.Promotion.GroupBuying;
using ECommerce.Entity.Shopping;

namespace ECommerce.Facade.GroupBuying
{
    public static class GroupBuyingFacade
    {
        /// <summary>
        /// 根据团购编号获取团购信息，只获取运行中和已完成的
        /// </summary>
        /// <param name="groupBuyingSysNo">团购编号</param>
        /// <returns></returns>
        public static GroupBuyingInfo GetGroupBuyingInfoBySysNo(int groupBuyingSysNo)
        {
            return GroupBuyingDA.GetGroupBuyingInfoBySysNo(groupBuyingSysNo);
        }

        /// <summary>
        /// 获取团购分类
        /// </summary>
        /// <returns></returns>
        public static List<GroupBuyingCategoryInfo> GetGroupBuyingCategory()
        {
            string cacheKey = CommonFacade.GenerateKey("GroupBuying_GetGroupBuyingCategory");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<GroupBuyingCategoryInfo>)HttpRuntime.Cache[cacheKey];
            }

            List<GroupBuyingCategoryInfo> result = GroupBuyingDA.GetGroupBuyingCategory();

            HttpRuntime.Cache.Insert(cacheKey, result, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);

            return result;
        }

        /// <summary>
        /// 查询团购
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static QueryResult<GroupBuyingInfo> QueryGroupBuyingInfo(GroupBuyingQueryInfo query)
        {
            return GroupBuyingDA.QueryGroupBuyingInfo(query);
        }

        /// <summary>
        /// 查询我的团购券
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static QueryResult<GroupBuyingTicketInfo> QueryGroupBuyingTicketInfo(PageInfo pageInfo, int customerSysNo)
        {
            return GroupBuyingDA.QueryGroupBuyingTicketInfo(pageInfo, customerSysNo);
        }

        /// <summary>
        /// 作废团购券
        /// </summary>
        /// <param name="sysNo"></param>
        public static void VoidedTicketBySysNo(int sysNo)
        {
            GroupBuyingDA.VoidedTicketBySysNo(sysNo);
        }

        /// <summary>
        /// 支付时获取团购券
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <returns></returns>
        public static GroupBuyTicketPayInfo GetGroupBuyingPayGetTicketInfo(int orderSysNo)
        {
            return GroupBuyingDA.GetGroupBuyingPayGetTicketInfo(orderSysNo);
        }
    }
}
