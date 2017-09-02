using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NeweggCN.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NeweggCN.NoBizQuery
{
    [VersionExport(typeof(IQCSubjectQueryDA))]
    public class QCSubjectQueryDA : IQCSubjectQueryDA
    {
        #region IQCSubjectQueryDA Members

        public List<BizEntity.Customer.QCSubject> GetAllQCSubject(QueryFilter.Customer.QCSubjectFilter filter)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QCSubjectQuery");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "OrderNum ASC "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "[Status]",
                   DbType.Int32,
                   "@Status",
                   QueryConditionOperatorType.Equal,
                   filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "CompanyCode",
                    DbType.AnsiStringFixedLength,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                   filter.CompanyCode);
                cmd.CommandText = sqlBuilder.BuildQuerySql();
                return cmd.ExecuteEntityList<QCSubject>();
            }
        #endregion
        }
    }
}
