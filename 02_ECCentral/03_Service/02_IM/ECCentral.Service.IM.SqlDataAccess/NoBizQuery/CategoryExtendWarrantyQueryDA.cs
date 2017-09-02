//************************************************************************
// 用户名				泰隆优选
// 系统名				类别延保管理
// 子系统名		        类别延保管理NoBizQuery查询接口实现
// 作成者				Kevin
// 改版日				2012.6.5
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
    [VersionExport(typeof(ICategoryExtendWarrantyQueryDA))]
    class CategoryExtendWarrantyQueryDA : ICategoryExtendWarrantyQueryDA
    {
        public DataTable QueryCategoryExtendWarranty(CategoryExtendWarrantyQueryFilter queryCriteria, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCategoryExtendWarranty");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "A.SysNo DESC"))
            {
                if (queryCriteria.C1SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C2.C1SysNo",
                        DbType.Int32, "@C1SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C1SysNo);
                }

                if (queryCriteria.C2SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C2.SysNo",
                        DbType.Int32, "@C2SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C2SysNo);
                }

                if (queryCriteria.C3SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.SysNo",
                        DbType.Int32, "@C3SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C3SysNo);
                }

                if (!string.IsNullOrEmpty(queryCriteria.BrandName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "B.ManufacturerName + B.BriefName",
                        DbType.String, "@BrandName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.BrandName);
                }

                if (!string.IsNullOrEmpty(queryCriteria.ProductCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.ProductCode",
                        DbType.String, "@ProductCode",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ProductCode);
                }

                if (queryCriteria.Years>0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.Years",
                        DbType.Int32, "@Years",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.Years);
                }

                if (queryCriteria.MinUnitPrice > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.MinUnitPrice",
                        DbType.Decimal, "@MinUnitPrice",
                        QueryConditionOperatorType.LessThanOrEqual,
                        queryCriteria.MinUnitPrice);
                }

                if (queryCriteria.MaxUnitPrice > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.MaxUnitPrice",
                        DbType.Decimal, "@MaxUnitPrice",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        queryCriteria.MaxUnitPrice);
                }

                if (queryCriteria.UnitPrice > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.UnitPrice",
                        DbType.Decimal, "@UnitPrice",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.UnitPrice);
                }

                if (queryCriteria.Cost >= 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.Cost",
                        DbType.Decimal, "@Cost",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.Cost);
                }
                
                if (!queryCriteria.Status.ToString().Equals("0"))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "A.Status",
                      DbType.String, "@Status",
                      QueryConditionOperatorType.Equal,
                      queryCriteria.Status);
                }

                if (!queryCriteria.IsECSelected.ToString().Equals("0"))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "A.IsECSelected",
                      DbType.String, "@IsECSelected",
                      QueryConditionOperatorType.Equal,
                      queryCriteria.IsECSelected);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();


                var enumList = new EnumColumnList { { "Status", typeof(CategoryExtendWarrantyStatus) }
                                                  , { "Years", typeof(CategoryExtendWarrantyYears) }
                                                  , { "IsECSelected", typeof(BooleanEnum)}};

                DataTable dt = dataCommand.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }

        }

        public DataTable QueryCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrandQueryFilter queryCriteria, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCategoryExtendWarrantyDisuseBrand");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "A.SysNo DESC"))
            {
                if (queryCriteria.C1SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C2.C1SysNo",
                        DbType.Int32, "@C1SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C1SysNo);
                }

                if (queryCriteria.C2SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C2.SysNo",
                        DbType.Int32, "@C2SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C2SysNo);
                }

                if (queryCriteria.C3SysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "C.SysNo",
                        DbType.Int32, "@C3SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C3SysNo);
                }

                if (!string.IsNullOrEmpty(queryCriteria.BrandName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "B.ManufacturerName + B.BriefName",
                        DbType.String, "@BrandName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.BrandName);
                }
              

                if (!queryCriteria.Status.ToString().Equals("0"))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "A.Status",
                      DbType.String, "@Status",
                      QueryConditionOperatorType.Equal,
                      queryCriteria.Status);
                }


                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = dataCommand.ExecuteDataTable("Status", typeof(CategoryExtendWarrantyStatus));

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }

        }
    }
}
