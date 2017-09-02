using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IGiftQueryDA))]
    public class GiftQueryDA : IGiftQueryDA
    {
        public virtual DataTable Query(CustomerGiftQueryFilter queryEntity, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryEntity.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryEntity.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryEntity.PagingInfo.PageIndex * queryEntity.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Gift_Query");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "gift.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "customer.CustomerID", DbType.String, "@CustomerID",
                    QueryConditionOperatorType.Equal, queryEntity.CustomerID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "gift.Quantity", DbType.Int32, "@Quantity",
                    QueryConditionOperatorType.Equal, queryEntity.ProductQty);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "gift.Status", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal, queryEntity.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "gift.CreateDate", DbType.DateTime, "@CreateDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     queryEntity.CreateDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "gift.CreateDate", DbType.DateTime, "@CreateDateTo",
                    QueryConditionOperatorType.LessThan,
                    queryEntity.CreateDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "product.ProductID", DbType.String, "@ProductID",
                    QueryConditionOperatorType.Equal, queryEntity.ProductID);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
