using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.MKT;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPollItemQueryDA))]
    public class PollItemQueryDA : IPollItemQueryDA
    {
        /// <summary>
        /// 查询投票列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryPollList(PollQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Poll_QueryPollList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "P.SysNo DESC"))
            {
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "ChannelID", DbType.String, "@ChannelID", QueryConditionOperatorType.Equal, filter.ChannelID);
                
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.PageType", DbType.Int32, "@PageType", QueryConditionOperatorType.Equal, filter.PageType);
                if (filter.PageType.HasValue && filter.PageID.HasValue)
                {
                    if (filter.PageType == 2 && filter.PageID.Value > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "P.PageID IN ( SELECT DISTINCT C2SysNo  FROM OverseaECommerceManagement.dbo.V_EM_EC_Category WHERE (C2SysNo IS NOT NULL OR C3SysNo IS NOT NULL) AND (C1SysNo =" + filter.PageID + " OR C2SysNo = " + filter.PageID + ") AND CompanyCode=" + filter.CompanyCode + "  )");

                    else if (filter.PageType == 3 && filter.PageID.Value > 0)
                        sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "P.PageID IN ( SELECT DISTINCT C3SysNo  FROM OverseaECommerceManagement.dbo.V_EM_EC_Category WHERE (C2SysNo IS NOT NULL OR C3SysNo IS NOT NULL) AND (C1SysNo =" + filter.PageID + " OR C2SysNo = " + filter.PageID + " OR C3SysNo = " + filter.PageID + ") AND CompanyCode=" + filter.CompanyCode + "  )");
                    else if (filter.PageID > 0)
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.PageID", DbType.String, "@PageID", QueryConditionOperatorType.Equal, filter.PageID);
                }
                if (filter.UserDefined == YNStatus.Yes)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "P.SysNo in (SELECT distinct PollSysno FROM OverseaECommerceManagement.dbo.Poll_ItemGroup with (nolock) WHERE CompanyCode=" + filter.CompanyCode + " AND Type = 'A')");

                else if (filter.UserDefined == YNStatus.No)
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "P.SysNo not in (SELECT distinct PollSysno FROM OverseaECommerceManagement.dbo.Poll_ItemGroup with (nolock) WHERE CompanyCode=" + filter.CompanyCode + " AND Type = 'A')");
                
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.PollName", DbType.String, "@PollName", QueryConditionOperatorType.Like, filter.PollName);


                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ADStatus));
                enumList.Add("UserDefined", typeof(YNStatus));

                var dt = cmd.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

    }
}
