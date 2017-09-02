using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using System.Data;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISysConfigQueryDA))]
    public class SysConfigQueryDA : ISysConfigQueryDA
    {

        public System.Data.DataTable Query(QueryFilter.MKT.SysConfigQueryFilter filter, out int totalCount)
        {

            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SysConfigQuery");
            cmd.SetParameterValue("@SysConfigType", filter.ConfigType);
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
               
                if (!string.IsNullOrEmpty(filter.ConfigType))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "SysConfigType", DbType.String, "@SysConfigType",
                       QueryConditionOperatorType.Equal, filter.ConfigType);
                   
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysConfigType",
                              DbType.String, "@SysConfigType", QueryConditionOperatorType.IsNotNull, DBNull.Value);
                }
                cmd.CommandText = sqlBuilder.BuildQuerySql(); 
                CodeNamePairColumnList codeNamePairConfig = new CodeNamePairColumnList();
                codeNamePairConfig.Add(0, "MKT", "SysConfigType");

                DataTable dt = cmd.ExecuteDataTable(codeNamePairConfig);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

    }
}
