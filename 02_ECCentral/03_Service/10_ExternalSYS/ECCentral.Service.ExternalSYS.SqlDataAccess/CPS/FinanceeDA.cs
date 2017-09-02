using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    [VersionExport(typeof(IFinanceDA))]
    public class FinanceeDA : IFinanceDA
    {
        /// <summary>
        /// 得到所有结算单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetAllFinancee(FinanceQueryFilter query, out int totalCount)
        {

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommissionSettlement");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = query.PageInfo.SortBy,
                StartRowIndex = query.PageInfo.PageIndex * query.PageInfo.PageSize,
                MaximumRows = query.PageInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "A.SysNo DESC"))
            {
                if (query.Status != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "A.Status",
                    DbType.String, "@Status",
                    QueryConditionOperatorType.Equal,
                    query.Status.Value);
                }
                if (!string.IsNullOrEmpty(query.SysNoList))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("A.SysNo IN ({0})", query.SysNoList));
                }
                if (!string.IsNullOrEmpty(query.CustomerID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "C.CustomerID",
                    DbType.String, "@CustomerID",
                    QueryConditionOperatorType.Like,
                    query.CustomerID);
                }
                if (query.SettleDateTo != null && query.SettleDateFrom != null)
                {
                    if (query.SettleDateFrom.Value.CompareTo(query.SettleDateTo) == 0)
                    {
                        query.SettleDateTo = query.SettleDateTo.Value.AddDays(1);
                    }
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "A.SettledTime", DbType.DateTime, "@SettleDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     query.SettleDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "A.SettledTime", DbType.DateTime, "@SettleDateTo",
                     QueryConditionOperatorType.LessThan,
                     query.SettleDateTo);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
            }

            DataTable dt = new DataTable();

            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("Status", typeof(FinanceStatus));
            enumList.Add("ToCashStatus", typeof(ToCashStatus));

            dt = cmd.ExecuteDataTable(enumList);
            totalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;

        }




        /// <summary>
        /// 更新确认结算金额
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCommisonConfirmAmt(FinanceInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCommisonConfirmAmt");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@CommisonConfirmAmt", info.ConfirmCommissionAmt);
            cmd.SetParameterValue("@UserName", info.User.UserName);
            cmd.ExecuteNonQuery();
        }
    }
}
