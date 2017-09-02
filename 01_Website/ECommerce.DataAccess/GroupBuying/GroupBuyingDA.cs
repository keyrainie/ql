using System;
using System.Collections.Generic;

using ECommerce.Utility.DataAccess;
using ECommerce.Entity;
using ECommerce.Entity.Promotion.GroupBuying;
using System.Data;
using ECommerce.Entity.Order;
using ECommerce.Entity.Shopping;

namespace ECommerce.DataAccess.GroupBuying
{
    public class GroupBuyingDA
    {
        /// <summary>
        /// 根据团购编号获取团购信息，只获取运行中和已完成的
        /// </summary>
        /// <param name="groupBuyingSysNo">团购编号</param>
        /// <returns></returns>
        public static GroupBuyingInfo GetGroupBuyingInfoBySysNo(int groupBuyingSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GroupBuying_GetGroupBuyingInfoBySysNo");
            cmd.SetParameterValue("@SysNo", groupBuyingSysNo);
            return cmd.ExecuteEntity<GroupBuyingInfo>();
        }

        /// <summary>
        /// 获取团购分类
        /// </summary>
        /// <returns></returns>
        public static List<GroupBuyingCategoryInfo> GetGroupBuyingCategory()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GroupBuying_GetGroupBuyingCategory");
            return cmd.ExecuteEntityList<GroupBuyingCategoryInfo>();
        }

        /// <summary>
        /// 查询团购
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static QueryResult<GroupBuyingInfo> QueryGroupBuyingInfo(GroupBuyingQueryInfo query)
        {
            QueryResult<GroupBuyingInfo> result = new QueryResult<GroupBuyingInfo>();
            result.PageInfo = query.PageInfo;

            PagingInfoEntity page = new PagingInfoEntity();
            page.MaximumRows = query.PageInfo.PageSize;
            page.StartRowIndex = query.PageInfo.PageIndex * query.PageInfo.PageSize;

            #region 排序
            string sortColumn = "";
            switch (query.SortType)
            {
                case 10:
                    sortColumn = "GroupBuying.CurrentSellCount ASC";
                    break;
                case 11:
                    sortColumn = "GroupBuying.CurrentSellCount DESC";
                    break;
                case 20:
                    sortColumn = "Price.CurrentPrice ASC";
                    break;
                case 21:
                    sortColumn = "Price.CurrentPrice DESC";
                    break;
                case 30:
                    sortColumn = "ISNULL(ProductReview.ReviewCount, 0) ASC";
                    break;
                case 31:
                    sortColumn = "ISNULL(ProductReview.ReviewCount, 0) DESC";
                    break;
                case 40:
                    sortColumn = "ProductStatus.LastOnlineTime ASC";
                    break;
                case 41:
                    sortColumn = "ProductStatus.LastOnlineTime DESC";
                    break;
                case 50:
                    sortColumn = "GroupBuying.EndDate ASC";
                    break;
                case 51:
                    sortColumn = "GroupBuying.EndDate DESC";
                    break;
                default:
                    sortColumn = "GroupBuying.CurrentSellCount DESC";
                    break;
            }
            #endregion

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GroupBuying_QueryGroupBuyingInfo");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, page, sortColumn))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GroupBuying.[Status]", DbType.String, "@Status", QueryConditionOperatorType.Equal, "A");
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "GroupBuying.GroupBuyingCategorySysNo", DbType.Int32, "@GroupBuyingCategorySysNo", QueryConditionOperatorType.Equal, query.CategorySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "GroupBuying.GroupBuyingTypeSysNo", 
                    DbType.Int32,
                    "@GroupBuyingTypeSysNo", 
                    QueryConditionOperatorType.Equal, 
                    query.GroupBuyingTypeSysNo);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                result.ResultList = cmd.ExecuteEntityList<GroupBuyingInfo>();

                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                result.PageInfo.TotalCount = totalCount;

                return result;
            }
        }

        /// <summary>
        /// 查询我的团购券
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static QueryResult<GroupBuyingTicketInfo> QueryGroupBuyingTicketInfo(PageInfo pageInfo, int customerSysNo)
        {
            QueryResult<GroupBuyingTicketInfo> result = new QueryResult<GroupBuyingTicketInfo>();
            result.PageInfo = pageInfo;

            PagingInfoEntity page = new PagingInfoEntity();
            page.MaximumRows = pageInfo.PageSize;
            page.StartRowIndex = pageInfo.PageIndex * pageInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GroupBuying_QueryGroupBuyingTicketInfo");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, page, "Ticket.OrderSysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Ticket.[CustomerSysNo]", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, customerSysNo);
 
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                result.ResultList = cmd.ExecuteEntityList<GroupBuyingTicketInfo>();

                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                result.PageInfo.TotalCount = totalCount;

                return result;
            }
        }

        /// <summary>
        /// 作废团购券
        /// </summary>
        /// <param name="sysNo"></param>
        public static void VoidedTicketBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GroupBuying_VoidedTicketBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 支付时获取团购券
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <returns></returns>
        public static GroupBuyTicketPayInfo GetGroupBuyingPayGetTicketInfo(int orderSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GroupBuying_PayGetTicket");
            cmd.SetParameterValue("@OrderSysNo", orderSysNo);
            return cmd.ExecuteEntity<GroupBuyTicketPayInfo>();
        }
    }
}
