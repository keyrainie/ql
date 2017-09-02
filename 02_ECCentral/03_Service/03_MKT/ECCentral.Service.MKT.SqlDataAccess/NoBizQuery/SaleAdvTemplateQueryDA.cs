using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using System.Data;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISaleAdvTemplateQueryDA))]
    public class SaleAdvTemplateQueryDA : ISaleAdvTemplateQueryDA
    {
        /// <summary>
        /// 查询有效的页面促销模版
        /// </summary>
        public List<WebPage> GetActiveCodeNames(string companyCode, string channelID)
        {
            var cmd = DataCommandManager.GetDataCommand("SaleAdvTemplate_GetActiveCodeNames");
            cmd.SetParameterValue("@CompanyCode",companyCode);
            cmd.SetParameterValue("@ChannelID", channelID);
            return cmd.ExecuteEntityList<WebPage>();
        }

        public DataTable Query(SaleAdvTemplateQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySaleAdvTemplates");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "result.CreateDate DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "sa.SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "sa.CreateTime", DbType.DateTime, "@CreateDateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "sa.CreateTime", DbType.DateTime, "@CreateDateTo", QueryConditionOperatorType.LessThan, filter.CreateDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "sa.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "v_user.DisplayName", DbType.String, "@CreateUser", QueryConditionOperatorType.Like, filter.CreateUser);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "sa.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                DataTable dt = cmd.ExecuteDataTable(new EnumColumnList {
                    { "Status", typeof(SaleAdvStatus)},
                    { "EnableReplyRank", typeof(CustomerRank)},
                    { "Type", typeof(ShowType)},
                    { "GroupType", typeof(GroupType)}
                });

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public List<WebPage> GetNowActiveCodeNames()
        {
            var cmd = DataCommandManager.GetDataCommand("SaleAdvTemplate_GetNowActiveCodeNames");
            return cmd.ExecuteEntityList<WebPage>();
        }
    }
}
