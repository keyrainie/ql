using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.PO.PurchaseOrder;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IDeductQueryDA))]
    public class DeductQueryDA : IDeductQueryDA
    {
        /// <summary>
        /// 扣款项维护列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable LoadDeductInfo(DeductQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = -1;
                return null;
            }

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryDeductList");
            
            if (null!=queryFilter.PageInfo.SortBy)
            {
                if (queryFilter.PageInfo.SortBy.IndexOf("[Status]") > -1)
                {
                    queryFilter.PageInfo.SortBy = "D." + queryFilter.PageInfo.SortBy;
                }
            }

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "Indate DESC"))
            {
                if (!string.IsNullOrEmpty(queryFilter.Name))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Name",
                         DbType.String, "@Name", QueryConditionOperatorType.Like, queryFilter.Name);    
                }

                if (queryFilter.DeductType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "DeductType",
                      DbType.Int32, "@DeductType", QueryConditionOperatorType.Equal, queryFilter.DeductType.Value);
                }

                if (queryFilter.AccountType.HasValue)
                {
                      sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "AccountType",
                    DbType.Int32, "@AccountType", QueryConditionOperatorType.Equal, queryFilter.AccountType.Value);
                }

                if (queryFilter.DeductMethod.HasValue)
                {
                     sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "DeductMethod",
                    DbType.Int32, "@DeductMethod", QueryConditionOperatorType.Equal, queryFilter.DeductMethod.Value);
                }

                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.Status",
                   DbType.Int32, "@Status", QueryConditionOperatorType.Equal, queryFilter.Status.Value);
                }

                command.CommandText = sqlBuilder.BuildQuerySql();
                command.SetParameterValue("@StartNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex);
                command.SetParameterValue("@EndNumber", queryFilter.PageInfo.PageSize * queryFilter.PageInfo.PageIndex + queryFilter.PageInfo.PageSize);
                EnumColumnList columnEnums = new EnumColumnList();
                columnEnums.Add("DeductType", typeof(DeductType));
                columnEnums.Add("AccountType", typeof(AccountType));
                columnEnums.Add("DeductMethod", typeof(DeductMethod));
                columnEnums.Add("Status", typeof(Status));
                DataTable dt = command.ExecuteDataTable(columnEnums);
                totalCount =Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
