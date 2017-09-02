using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISMSQueryDA))]
    public class SMSQueryDA : ISMSQueryDA
    {
        #region ISMSQueryDA Members

        public virtual DataTable Query(QueryFilter.Customer.SMSQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetSMSListByQuery");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "CellNumber",
                DbType.String, "@CellNumber",
                QueryConditionOperatorType.Like,
                queryCriteria.Tel);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "CreateTime",
                DbType.String, "@EndDate",
                QueryConditionOperatorType.LessThanOrEqual,
                queryCriteria.ToDate);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
               "CreateTime",
               DbType.String, "@BeginDate",
               QueryConditionOperatorType.MoreThanOrEqual,
               queryCriteria.FromDate);

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
                queryCriteria.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable(7, typeof(SMSSendStatus));
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }

        #endregion
        }
    }
}
