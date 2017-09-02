//************************************************************************
// 用户名				泰隆优选
// 系统名				类别属性管理
// 子系统名		        类别属性管理NoBizQuery查询接口实现
// 作成者				Tom
// 改版日				2012.5.21
// 改版内容				新建
//************************************************************************

using System;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ICategoryAccessoriesQueryDA))]
    class CategoryAccessoriesQueryDA : ICategoryAccessoriesQueryDA
    {
        public DataTable QueryCategoryAccessories(CategoryAccessoriesQueryFilter queryCriteria, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCategoryAccessories");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "C.SysNo DESC"))
            {
                if (queryCriteria.Category3SysNo > 0)
                {
                    //dataCommand.AddInputParameter("@CategorySysNo", DbType.Int32, queryCriteria.CategorySysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C3Sysno",
                        DbType.Int32, "@Category3SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.Category3SysNo);
                }
                if (queryCriteria.Category2SysNo > 0)
                {
                    //dataCommand.AddInputParameter("@CategorySysNo", DbType.Int32, queryCriteria.CategorySysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C2Sysno",
                        DbType.Int32, "@Category2SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.Category2SysNo);
                }
                if (queryCriteria.Category1SysNo > 0)
                {
                    //dataCommand.AddInputParameter("@CategorySysNo", DbType.Int32, queryCriteria.CategorySysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "Z.SysNo",
                        DbType.Int32, "@Category1SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.Category1SysNo);
                }
                if (queryCriteria.Status != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "C.Status",
                DbType.Int32, "@Status",
                QueryConditionOperatorType.Equal,
                queryCriteria.Status.Value);
                }


                if (!string.IsNullOrEmpty(queryCriteria.AccessoriesName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "AccessoriesName",
                    DbType.String, "@AccessoriesName",
                    QueryConditionOperatorType.Like,
                    queryCriteria.AccessoriesName);
                }


                if (queryCriteria.AccessoryOrder != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "AccessoryOrder",
                       DbType.Int32, "@AccessoryOrder",
                       QueryConditionOperatorType.Equal,
                       queryCriteria.AccessoryOrder.Value);
                }

                if (queryCriteria.IsDefault!=null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
             "IsDefault",
             DbType.Int32, "@IsDefault",
             QueryConditionOperatorType.Equal,
             queryCriteria.IsDefault.Value);
                }
              

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList { { "Status", typeof(CategoryAccessoriesStatus) }, { "IsDefault", typeof(IsDefault) } };

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }

        }
    }
}
