using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Enum;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NeweggCN.NoBizQuery
{
    [VersionExport(typeof(IHotSaleCategoryQueryDA))]
    public class HotSaleCategoryQueryDA : IHotSaleCategoryQueryDA
    {
        public DataTable Query(HotSaleCategoryQueryFilter query, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = query.PageInfo.SortBy;
            pagingEntity.MaximumRows = query.PageInfo.PageSize;
            pagingEntity.StartRowIndex = query.PageInfo.PageIndex * query.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("WebSite_QueryHotSaleCategory");
            var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "h.TransactionNumber desc");

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "h.Status",
                    DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal,
                    query.Status);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "h.Position",
                DbType.Int32, "@Position", QueryConditionOperatorType.Equal,
                query.Position);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "h.GroupName",
                DbType.String, "@GroupName", QueryConditionOperatorType.Like,
                query.GroupName);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "h.PageType",
            DbType.Int32, "@PageType", QueryConditionOperatorType.Equal,
            query.PageType);

            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "h.CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal,
                query.CompanyCode);
            //TODO:添加ChannelID参数

            cmd.CommandText = sqlBuilder.BuildQuerySql();

            EnumColumnList enumConfig = new EnumColumnList();
            enumConfig.Add("Status", typeof(ADStatus));
            var dt = cmd.ExecuteDataTable(enumConfig);
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;

        }
    }
}
