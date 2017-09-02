using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM.Product.Request;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductPriceRequestQueryDA))]
    public class ProductPriceRequestQueryDA : IProductPriceRequestQueryDA
    {
        #region 字段以及构造函数
        private readonly static Dictionary<Comparison, QueryConditionOperatorType> ComparisonMapping = new Dictionary<Comparison, QueryConditionOperatorType>();

        static ProductPriceRequestQueryDA()
        {
            SetComparisonMapping();
        }

        private static void SetComparisonMapping()
        {
            ComparisonMapping.Add(Comparison.Equal, QueryConditionOperatorType.Equal);
            ComparisonMapping.Add(Comparison.Unequal, QueryConditionOperatorType.NotEqual);
        }
        #endregion


        /// <summary>
        /// 查询商品价格变动申请单据
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductPriceRequesList(ProductPriceRequestQueryFilter queryFilter, out int totalCount)
        {

            CustomDataCommand dataCommand =
                DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductPriceRequesList");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };
            if (ComparisonMapping.Count == 0)
            {
                SetComparisonMapping();
            }

            using (
                var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo,
                                                            "P.ProductSysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.Status",
                    DbType.String, "@Status",
                    QueryConditionOperatorType.In,
                    new List<object> { ProductPriceRequestStatus.Origin, ProductPriceRequestStatus.NeedSeniorApprove });
                if (!String.IsNullOrEmpty(queryFilter.ProductID))
                {
                    dataCommand.AddInputParameter("@ProductID", DbType.String, queryFilter.ProductID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                                 "PP.ProductID",
                                                                 DbType.String, "@ProductID",
                                                                 QueryConditionOperatorType.Equal,
                                                                 queryFilter.ProductID);
                }
                if (queryFilter.ProductStatus != null)
                {
                    dataCommand.AddInputParameter("@ProductStatus", DbType.Int32, queryFilter.ProductStatus);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                                 "PP.Status",
                                                                 DbType.Int32, "@ProductStatus",
                                                                 QueryConditionOperatorType.Equal,
                                                                 queryFilter.ProductStatus);
                }
                if (queryFilter.ManufacturerSysNo != null && queryFilter.ManufacturerSysNo > 0)
                {
                    dataCommand.AddInputParameter("@ManufacturerSysNo", DbType.Int32, queryFilter.ManufacturerSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                                 "PP.ManufacturerSysNo",
                                                                 DbType.Int32, "@ManufacturerSysNo",
                                                                 QueryConditionOperatorType.Equal,
                                                                 queryFilter.ManufacturerSysNo);
                }
                if (queryFilter.Category3 != null && queryFilter.Category3.Value > 0)
                {
                    dataCommand.AddInputParameter("@C3SysNo", DbType.Int32, queryFilter.Category3.Value);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                                 "PP.C3SysNo",
                                                                 DbType.Int32, "@C3SysNo",
                                                                 QueryConditionOperatorType.Equal,
                                                                 queryFilter.Category3.Value);
                }
                if (queryFilter.Category2 != null && queryFilter.Category2.Value > 0)
                {
                    dataCommand.AddInputParameter("@C2SysNo", DbType.Int32, queryFilter.Category2.Value);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                                 "A.C2SysNo",
                                                                 DbType.Int32, "@C2SysNo",
                                                                 QueryConditionOperatorType.Equal,
                                                                 queryFilter.Category2.Value);
                }
                if (queryFilter.Category1 != null && queryFilter.Category1.Value > 0)
                {
                    dataCommand.AddInputParameter("@C1SysNo", DbType.Int32, queryFilter.Category1.Value);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                                 "B.C1SysNo",
                                                                 DbType.Int32, "@C1SysNo",
                                                                 QueryConditionOperatorType.Equal,
                                                                 queryFilter.Category1.Value);
                }

                dataCommand.AddInputParameter("@PMRole", DbType.Int32, queryFilter.PMRole);
                if(queryFilter.PMRole==1)//初级权限
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " PP.PMUserSysNo IN (SELECT PMSysNo FROM #temp1 t1) ");
                }
                else if (queryFilter.PMRole == 3)//高级权限
                {

                }
                else//无权限
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " 1<>1 ");
                }

                if (queryFilter.AuditType != null)
                {
                    switch (queryFilter.AuditType)
                    {
                        case QueryProductPriceRequestAuditType.Audit:
                            sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                              "P.Type",
                                                              DbType.String, "@AuditType",
                                                             ComparisonMapping[queryFilter.ComparisonOperators],
                                                              ProductPriceRequestAuditType.Audit);
                            break;
                        case QueryProductPriceRequestAuditType.SeniorAudit:

                            if (queryFilter.ComparisonOperators == Comparison.Equal)
                            {
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                           "P.Type",
                                                           DbType.String, "@AuditType",
                                                          ComparisonMapping[queryFilter.ComparisonOperators],
                                                           ProductPriceRequestAuditType.SeniorAudit);
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                "P.Status",
                                DbType.String, "@OtherStatus",
                                QueryConditionOperatorType.Equal,
                                ProductPriceRequestStatus.NeedSeniorApprove
                                );

                            }
                            else
                            {
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                             "P.Status",
                             DbType.String, "@OtherStatus",
                             QueryConditionOperatorType.Equal,
                             ProductPriceRequestStatus.Origin
                             );
                            }
                            break;
                        case QueryProductPriceRequestAuditType.Submit:

                            if (queryFilter.ComparisonOperators == Comparison.Equal)
                            {
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                                           "P.Type",
                                                           DbType.String, "@AuditType",
                                                          ComparisonMapping[queryFilter.ComparisonOperators],
                                                           ProductPriceRequestAuditType.SeniorAudit);
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                "P.Status",
                                DbType.String, "@OtherStatus",
                                QueryConditionOperatorType.Equal,
                                ProductPriceRequestStatus.Origin
                                );

                            }
                            else
                            {
                                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                                         "P.Type",
                                                         DbType.String, "@AuditType",
                                                         ComparisonMapping[queryFilter.ComparisonOperators],
                                                         ProductPriceRequestAuditType.SeniorAudit);
                                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR,
                                 "P.Status",
                                 DbType.String, "@OtherStatus",
                                 ComparisonMapping[queryFilter.ComparisonOperators],
                                 ProductPriceRequestStatus.Origin
                                 );

                                sqlBuilder.ConditionConstructor.EndGroupCondition();

                            }
                            break;
                    }
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList
                                              {
                                                  {"AuditType", typeof (ProductPriceRequestAuditType)},
                                                  {"Status", typeof (ProductPriceRequestStatus)}
                                              };
                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}