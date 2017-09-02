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
    [VersionExport(typeof(ICategoryPropertyQueryDA))]
    class CategoryPropertyQueryDA : ICategoryPropertyQueryDA
    {
        public DataTable QueryCategoryProperty(CategoryPropertyQueryFilter queryCriteria, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCategoryProperty");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "P.SysNo DESC"))
            {
                if (queryCriteria.CategorySysNo > 0)
                {
                    //dataCommand.AddInputParameter("@CategorySysNo", DbType.Int32, queryCriteria.CategorySysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "CategorySysNo",
                        DbType.Int32, "@CategorySysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.CategorySysNo);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var enumList = new EnumColumnList();
                enumList.Add("IsItemSearch", typeof(CategoryPropertyStatus));
                enumList.Add("IsInAdvSearch", typeof(CategoryPropertyStatus));
                enumList.Add("IsMustInput", typeof(CategoryPropertyStatus));
                enumList.Add("WebDisplayStyle", typeof(WebDisplayStyle));
                enumList.Add("PropertyType", typeof(PropertyType));
                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }

        }
    }
}
