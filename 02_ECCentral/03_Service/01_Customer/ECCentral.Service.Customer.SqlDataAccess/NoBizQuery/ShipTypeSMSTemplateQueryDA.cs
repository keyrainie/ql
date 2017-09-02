using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IShipTypeSMSTemplateQueryDA))]
    public class ShipTypeSMSTemplateQueryDA : IShipTypeSMSTemplateQueryDA
    {

        #region IShipTypeSMSTemplateQueryDA Members

        public virtual DataTable Query(QueryFilter.Customer.ShipTypeSMSTemplateQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetShipTypeSMSTemplateListByQuery");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "Template",
                DbType.String, "@Template",
                QueryConditionOperatorType.Like,
                queryCriteria.Keywords);
                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
                "8601");
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                DataTable dt = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion
    }
}
