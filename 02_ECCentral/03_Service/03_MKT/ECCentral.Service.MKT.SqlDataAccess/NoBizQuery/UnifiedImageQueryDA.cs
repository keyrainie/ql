using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IUnifiedImageQueryDA))]
    public class UnifiedImageQueryDA : IUnifiedImageQueryDA
    {
        public DataTable Query(UnifiedImageQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("UnifiedImage_QueryImages");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "A.ImageName", DbType.String, "@ImageName",
                    QueryConditionOperatorType.Like, filter.ImageName);
                if (filter.DateTimeFrom.HasValue && filter.DateTimeTo.HasValue && filter.DateTimeFrom.Equals(filter.DateTimeTo)) filter.DateTimeTo = filter.DateTimeTo.Value.AddDays(1);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "A.CreateDate", DbType.DateTime, "@DateTimeFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.DateTimeFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "A.CreateDate", DbType.DateTime, "@DateTimeTo",
                     QueryConditionOperatorType.LessThan,
                     filter.DateTimeTo);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds;
            }
        }
    }
}
