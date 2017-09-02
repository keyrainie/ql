using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IBannerDimensionQueryDA))]
    public class BannerDimensionQueryDA : IBannerDimensionQueryDA
    {

        #region IBannerDimensionQueryDA Members

        public DataTable Query(BannerDimensionQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("BannerDimension_QueryBannerDimension");
            var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "B.SysNo DESC");

            sqlBuilder.ConditionConstructor.AddCondition(
        QueryConditionRelationType.AND,
        "B.PageType",
        DbType.Int32,
        "@PageType",
        QueryConditionOperatorType.Equal,
        filter.PageTypeID);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "B.PositionID",
           DbType.Int32,
           "@PositionID",
           QueryConditionOperatorType.Equal,
           filter.PositionID);

            sqlBuilder.ConditionConstructor.AddCondition(
             QueryConditionRelationType.AND,
             "B.PositionName",
             DbType.AnsiStringFixedLength,
             "@PositionName",
             QueryConditionOperatorType.Like,
             filter.PositionName);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "B.CompanyCode",
           DbType.AnsiStringFixedLength,
           "@CompanyCode",
           QueryConditionOperatorType.Equal,
         filter.CompanyCode);

            cmd.CommandText = sqlBuilder.BuildQuerySql();
            var dt = cmd.ExecuteDataTable();
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }

        #endregion
    }
}
