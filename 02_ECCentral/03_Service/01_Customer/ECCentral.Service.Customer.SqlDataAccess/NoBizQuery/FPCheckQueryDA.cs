using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Customer;
using System.Data;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IFPCheckQueryDA))]
    public class FPCheckQueryDA : IFPCheckQueryDA
    {
        public virtual DataTable Query(FPCheckQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryFPCheckMaster");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " a.sysno ASC "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal, "8601");
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                cmd.SetParameterValue("@ChannelID", queryCriteria.ChannelID);
                DataTable dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
        public virtual DataTable QueryCHSet(CHQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCHSet");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " a.sysno desc "))
            {
                if (!string.IsNullOrEmpty(queryCriteria.ProductID) || !queryCriteria.IsSearchCategory)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                                    QueryConditionRelationType.AND,
                                                    "a.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType",
                                                    QueryConditionOperatorType.Equal, ("PID"));
                }
                else if (queryCriteria.CategorySysNo != null || queryCriteria.IsSearchCategory)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                                    QueryConditionRelationType.AND,
                                                    "a.ReferenceType", DbType.AnsiStringFixedLength, "@ReferenceType",
                                                    QueryConditionOperatorType.Equal, ("PC3"));
                }
                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "a.ReferenceContent", DbType.AnsiStringFixedLength, "@ProductID",
                QueryConditionOperatorType.Equal, queryCriteria.ProductID);

                sqlBuilder.ConditionConstructor.AddCondition(
              QueryConditionRelationType.AND,
              "a.Description", DbType.AnsiStringFixedLength, "@CategorySysNo",
              QueryConditionOperatorType.Equal, queryCriteria.CategorySysNo);

                sqlBuilder.ConditionConstructor.AddCondition(
             QueryConditionRelationType.AND,
             "a.Status", DbType.AnsiStringFixedLength, "@Status",
             QueryConditionOperatorType.Equal, queryCriteria.Status);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList cl = new EnumColumnList();
                cl.Add(5, typeof(FPCheckItemStatus));
                cl.Add(6, typeof(FPCheckItemStatus));
                DataTable dt = cmd.ExecuteDataTable(cl);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public virtual DataTable GetETC(string webChannelID, out int totalCount)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryETC");
            DataTable dt = cmd.ExecuteDataTable();
            totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            return dt;
        }


    }
}
