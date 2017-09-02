using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPageTypeQueryDA))]
    public class PageTypeQueryDA : IPageTypeQueryDA
    {
        public DataTable Query(PageTypeQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("PageType_QueryPageType");
            var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "A.SysNo DESC");

            sqlBuilder.ConditionConstructor.AddCondition(
        QueryConditionRelationType.AND,
        "A.PageTypeName",
        DbType.String,
        "@PageTypeName",
        QueryConditionOperatorType.Like,
        filter.PageTypeName);

            sqlBuilder.ConditionConstructor.AddCondition(
           QueryConditionRelationType.AND,
           "A.CompanyCode",
           DbType.AnsiStringFixedLength,
           "@CompanyCode",
           QueryConditionOperatorType.Equal,
         filter.CompanyCode);
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
