using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IECDynamicCategoryQueryDA))]
    public class ECDynamicCategoryQueryDA : IECDynamicCategoryQueryDA
    {
        public DataTable QueryECDynamicCategoryMapping(ECDynamicCategoryMappingQueryFilter filter, out int totalCount)
        {
            var pagingInfo = new PagingInfoEntity
            {
                SortField = filter.PagingInfo.SortBy,
                StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize,
                MaximumRows = filter.PagingInfo.PageSize
            };
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("MKT_GetECDynamicCategoryMapping");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "m.SysNo DESC"))
            {
                //状态
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "m.DynamicCategorySysNo",
                  DbType.Int32,
                  "@DynamicCategorySysNo",
                  QueryConditionOperatorType.Equal,
                filter.DynamicCategorySysNo);

                //只查询有效的
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "m.Status",
                  DbType.Int32,
                  "@Status",
                  QueryConditionOperatorType.Equal,
                0);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "m.CompanyCode",
                   DbType.AnsiStringFixedLength,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                 filter.CompanyCode);
                //TODO:添加渠道过滤条件

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList list = new EnumColumnList();
                list.Add("ProductStatus", typeof(ProductStatus));
              
                var dt = cmd.ExecuteDataTable(list);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }

        }
    }
}
