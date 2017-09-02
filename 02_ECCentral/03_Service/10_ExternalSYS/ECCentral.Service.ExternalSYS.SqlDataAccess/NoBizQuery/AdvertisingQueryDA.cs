using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IAdvertisingQueryDA))]
    public class AdvertisingQueryDA : IAdvertisingQueryDA
    {
        public DataTable Query(AdvertisingQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PageInfo.SortBy;
            pagingEntity.MaximumRows = filter.PageInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AdvertisingQuery");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "product.InDate DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "category.SysNo",
                        DbType.Int32, 
                        "@ProductLineCategorySysNo", 
                        QueryConditionOperatorType.Equal, 
                        filter.ProductLineCategorySysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Link.ProductLineSysNo",
                        DbType.Int32, 
                        "@ProductLineSysNo", 
                        QueryConditionOperatorType.Equal,
                        filter.ProductLineSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Link.InUser",
                        DbType.String, 
                        "@InUser", 
                        QueryConditionOperatorType.Like,
                        filter.InUser);

                string sql = string.Empty;
                if (filter.OperateDateFrom.HasValue)
                {
                    sql = string.Format(@" Link.InDate >= '{0}'", filter.OperateDateFrom);
                }
                if (filter.OperateDateTo.HasValue)
                {
                    sql = string.Format(@" Link.InDate <= '{0}' or Link.EditDate <='{0}'",
                        filter.OperateDateTo.Value.AddDays(1).AddSeconds(-1));
                }
                if (filter.OperateDateFrom.HasValue && filter.OperateDateTo.HasValue)
                {
                    sql = string.Format(@" ((Link.InDate >= '{0}' AND Link.InDate <='{1}') OR (Link.EditDate >= '{0}' AND Link.EditDate <='{1}'))",
                        filter.OperateDateFrom, filter.OperateDateTo.Value.AddDays(1).AddSeconds(-1));
                }

                if (!string.IsNullOrEmpty(sql))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, sql);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Link.ImageWidth",
                        DbType.Int32, 
                        "@ImageWidth", 
                        QueryConditionOperatorType.Equal,
                        filter.ImageWidth);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Link.ImageHeight",
                        DbType.Int32, 
                        "@ImageHeight", 
                        QueryConditionOperatorType.Equal,
                        filter.ImageHeight);


                object _type;
                if (filter.Type != null && EnumCodeMapper.TryGetCode(filter.Type, out _type))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Link.Type",
                        DbType.String,
                        "@Type",
                        QueryConditionOperatorType.Like,
                        _type);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Type", typeof(AdvertisingType));
                enumList.Add("Status", typeof(ValidStatus));

                var dt = cmd.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
