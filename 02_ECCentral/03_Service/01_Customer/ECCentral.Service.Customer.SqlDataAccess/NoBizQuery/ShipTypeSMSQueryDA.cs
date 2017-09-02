using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IShipTypeSMSQueryDA))]
    public class ShipTypeSMSQueryDA : IShipTypeSMSQueryDA
    {

        #region IShipTypeSMSQueryDA Members

        public virtual DataTable Query(QueryFilter.Customer.ShipTypeSMSQueryFilter queryCriteria, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = queryCriteria.PagingInfo.SortBy;
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetShipTypeSMSListByQuery");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " a.SysNo DESC "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                "a.SMSType",
                DbType.String, "@SMSType",
                QueryConditionOperatorType.Equal,
                queryCriteria.SMSType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "a.ShipTypeSysNo",
                   DbType.String, "@ShipTypeSysNo",
                   QueryConditionOperatorType.Equal,
                   queryCriteria.ShipType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Status",
                    DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal,
                    queryCriteria.ShipTypeSMSStatus);
                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                   queryCriteria.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(
                QueryConditionRelationType.AND,
                "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                QueryConditionOperatorType.Equal,
                queryCriteria.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                CodeNamePairColumnList cnpList = new CodeNamePairColumnList();
                cnpList.Add(2, "Customer", "SMSType", "SMSTypeSysNo");
                EnumColumnList ecList = new EnumColumnList();
                ecList.Add(8, typeof(ShipTypeSMSStatus));
                DataTable dt = cmd.ExecuteDataTable(ecList, cnpList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }

        #endregion
        }
    }
}