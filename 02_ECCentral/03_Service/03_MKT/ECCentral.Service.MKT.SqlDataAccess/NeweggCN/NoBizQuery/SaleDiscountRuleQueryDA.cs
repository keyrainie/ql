using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISaleDiscountRuleQueryDA))]
    public class SaleDiscountRuleQueryDA : ISaleDiscountRuleQueryDA
    {
        #region ISaleDiscountRuleQueryDA Members

        public DataTable Query(SaleDiscountRuleQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("NewPromotion_SaleDiscountRule_Query");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "a.ActivityName", DbType.String, "@ActivityName",
                   QueryConditionOperatorType.LeftLike, filter.ActivityName);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "a.Status", DbType.Int32, "@Status",
                   QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.BeginDate", DbType.DateTime, "@BeginDate",
                    QueryConditionOperatorType.LessThanOrEqual, filter.BeginDate);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.EndDate", DbType.DateTime, "@EndDate",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.EndDate);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "a.BrandSysNo", DbType.Int32, "@BrandSysNo",
                  QueryConditionOperatorType.Equal, filter.BrandSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "a.ProductSysNo", DbType.Int32, "@ProductSysNo",
                  QueryConditionOperatorType.Equal, filter.ProductSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "p.ProductID", DbType.String, "@ProductID",
                  QueryConditionOperatorType.Equal, filter.ProductID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "c3.SysNo", DbType.Int32, "@C3SysNo",
                  QueryConditionOperatorType.Equal, filter.C3SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "c2.SysNo", DbType.Int32, "@C2SysNo",
                  QueryConditionOperatorType.Equal, filter.C2SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "c1.SysNo", DbType.Int32, "@C1SysNo",
                  QueryConditionOperatorType.Equal, filter.C1SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "b.SysNo", DbType.Int32, "@BrandSysNo",
                  QueryConditionOperatorType.Equal, filter.BrandSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "v.SysNo", DbType.Int32, "@VendorSysNo",
                  QueryConditionOperatorType.Equal, filter.VendorSysNo);


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CompanyCode", DbType.String, "@CompanyCode",
                    QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumConfig = new EnumColumnList();
                enumConfig.Add("Status", typeof(SaleDiscountRuleStatus));
                enumConfig.Add("RuleType", typeof(SaleDiscountRuleType));

                var dt = cmd.ExecuteDataTable(enumConfig);
                int.TryParse(cmd.GetParameterValue("@TotalCount").ToString(), out totalCount);

                return dt;
            }
        }

        #endregion
    }
}
