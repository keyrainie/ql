using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductPayTypeQueryDA))]
    public class ProductPayTypeQueryDA : IProductPayTypeQueryDA
    {
        /// <summary>
        /// 查询商品支付方式
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryProductPayType(ProductPayTypeQueryFilter filter, out int totalCount)
        {
            var pagingEntity = new PagingInfoEntity
                                                {
                                                    SortField = filter.PageInfo.SortBy,
                                                    MaximumRows = filter.PageInfo.PageSize,
                                                    StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize
                                                };

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ProductPayType_GetProductPayTypeList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "pdp.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pdp.ProductSysNo",
                    DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal,
                    filter.ProductSysNo);
                var minDateTime = DateTime.MinValue;
                if (filter.BeginDateFrom.HasValue && filter.BeginDateFrom.Value.Date != minDateTime)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pdp.BeginDate",
                    DbType.DateTime, "@BeginDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                    filter.BeginDateFrom);
                }

                if (filter.BeginDateTo.HasValue && filter.BeginDateTo.Value.Date != minDateTime)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pdp.BeginDate",
                    DbType.DateTime, "@BeginDateTo", QueryConditionOperatorType.LessThanOrEqual,
                    filter.BeginDateTo);
                }

                if (filter.EndDateFrom.HasValue && filter.EndDateFrom.Value.Date != minDateTime)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pdp.EndDate",
                    DbType.DateTime, "@EndDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                    filter.EndDateFrom);
                }

                if (filter.EndDateTo.HasValue && filter.EndDateTo.Value.Date != minDateTime)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pdp.EndDate",
                    DbType.DateTime, "@EndDateTo", QueryConditionOperatorType.LessThanOrEqual,
                    filter.EndDateTo);
                }

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pdp.PayTypeSysNo",
                    DbType.String, "@PayTypeSysNo", QueryConditionOperatorType.Equal,
                    filter.PayTypeSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pdp.Status",
                    DbType.String, "@Status", QueryConditionOperatorType.Equal,
                    filter.Status);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds.Tables[0];
            }
        }


        public List<PayTypeInfo> GetProductPayTypeList()
        {
            var entityList = new List<PayTypeInfo>();
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductPayTypeList");
            entityList = cmd.ExecuteEntityList<PayTypeInfo>();
            return entityList;
        }
    }
}
