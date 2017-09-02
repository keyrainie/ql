using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Topic;
using ECommerce.Entity;
using ECommerce.Enums;
using System.Data;

namespace ECommerce.DataAccess.Topic
{
    public class TopicDA
    {
        public static void SetSubscription(Subscription entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetSubscription");
            cmd.SetParameterValue<Subscription>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 分页查询新闻公告
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static QueryResult<NewsInfo> QueryNewsInfo(NewsQueryFilter filter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Content_QueryTopic");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, filter.ConvertToPaging(), "TopMost DESC,SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 1);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ReferenceSysNo", DbType.Int32, "@ReferenceSysNo", QueryConditionOperatorType.Equal, filter.ReferenceSysNo);
                if (filter.NewsType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "NewsType", DbType.Int32, "@NewsType", QueryConditionOperatorType.Equal, (int)filter.NewsType.Value);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PageShowInheritance", DbType.Int32, "@PageShowInheritance", QueryConditionOperatorType.Equal, filter.PageShowInheritance);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ExpireDate", DbType.Time, "@ExpireDateBegin", QueryConditionOperatorType.MoreThan, DateTime.Now);
                command.CommandText = sqlBuilder.BuildQuerySql();
                var newsList = command.ExecuteEntityList<NewsInfo>();
                var totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                QueryResult<NewsInfo> result = new QueryResult<NewsInfo>();
                result.ResultList = newsList;
                result.PageInfo = filter.ConvertToPageInfo(totalCount);
                return result;
            }
        }

        /// <summary>
        /// 获取新闻信息
        /// </summary>
        /// <param name="newsType">要获取的新闻类型</param>
        /// <param name="topNum">要获取的新闻数量</param>
        /// <returns>新闻列表</returns>
        public static List<NewsInfo> GetNewsInfoByNewsType(NewsType newsType , int topNum)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Content_GetNewsInfoByNewsType");
            dataCommand.SetParameterValue("@NewsType", (int)newsType);
            dataCommand.SetParameterValue("@TopNum", topNum);

            List<NewsInfo> entitys = dataCommand.ExecuteEntityList<NewsInfo>();
            return entitys;
        }

        public static List<NewsInfo> GetTopTopicList(NewsType newsType, int? refSysNo, int? pageShowInheritance, int topNum)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Content_GetTopTopicList");
            string strWhere = " NewsType=" + (int)newsType;
            if (refSysNo.HasValue)
            {
                strWhere += " AND ReferenceSysNo=" + refSysNo;
            }
            if (pageShowInheritance.HasValue)
            {
                strWhere += " AND PageShowInheritance=" + pageShowInheritance;
            }
            strWhere += " AND Status = 1";
            strWhere += string.Format(" AND ExpireDate >= '{0}'", DateTime.Now);

            cmd.CommandText = cmd.CommandText.Replace("#strWhere#", strWhere);
            cmd.SetParameterValue("@TopNum", topNum);
            List<NewsInfo> entitys = cmd.ExecuteEntityList<NewsInfo>();
            return entitys;
        }

        /// <summary>
        /// 根据编号获取新闻信息
        /// </summary>
        /// <param name="sysNo">新闻编号</param>
        /// <returns>新闻信息</returns>
        public static NewsInfo GetNewsInfoBySysNo(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Content_GetNewsInfoBySysNo");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            return dataCommand.ExecuteEntity<NewsInfo>();
        }

        /// <summary>
        /// 获取帮助中心新闻类型
        /// </summary>
        /// <param name="newsType">要获取的帮助中心新闻类型</param>
        /// <returns>帮助中心新闻类型列表</returns>
        public static List<NewsInfo> GetHelperCenterCategory()
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Content_GetHelperCenterCategory");
            List<NewsInfo> entitys = dataCommand.ExecuteEntityList<NewsInfo>();
            return entitys;
        }

        /// <summary>
        ///  获取指定数量帮助中心新闻
        /// </summary>
        /// <param name="categorySysNo">新闻类型</param>
        /// <param name="topNum">获取数量</param>
        /// <returns>帮助中心新闻</returns>
        public static List<NewsInfo> GetTopHelperCenterList(string categorySysNo, int topNum)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Content_GetTopHelperCenterList");
            cmd.SetParameterValue("@TopNum", topNum);
            cmd.SetParameterValue("@CategorySysNo", categorySysNo);
            List<NewsInfo> entitys = cmd.ExecuteEntityList<NewsInfo>();
            return entitys;
        }

        /// <summary>
        ///  根据编号获取帮助中心新闻
        /// </summary>
        /// <param name="sysNo">新闻编号</param>
        /// <returns>帮助中心新闻</returns>
        public static NewsInfo GetTopHelperCenterBySysNo(int sysNo)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Content_GetTopHelperCenterBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteEntity<NewsInfo>();
        }
    }
}
