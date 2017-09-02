using System;
using System.Data;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IShiftRequestMemoQueryDA))]
    public class ShiftRequestMemoQueryDA : IShiftRequestMemoQueryDA
    {

        private string SortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "REQUESTSYSNO":
                        tsort[0] = " a.ShiftSysNo";
                        break;
                    case "MEMOSTATUS":
                        tsort[0] = "a.Status";
                        break;
                    case "CREATEDATE":
                        tsort[0] = "e.CreateTime";
                        break;
                    case "EDITDATE":
                        tsort[0] = "a.UpdateTime";
                        break;
                    case "REMINDTIME":
                        tsort[0] = "a.RemindTime";
                        break;

                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }


        public virtual DataTable QueryShiftRequestMemo(ShiftRequestMemoQueryFilter queryCriteria, out int totalCount)
        {

            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = SortFieldMapping(queryCriteria.PagingInfo.SortBy);
            pagingEntity.MaximumRows = queryCriteria.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryShiftRequestMemo");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {

                if (!string.IsNullOrEmpty(queryCriteria.CompanyCode))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                            "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                            QueryConditionOperatorType.Equal, queryCriteria.CompanyCode);
                }

                if (queryCriteria.MemoStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.Status", DbType.Int32, "@MemoStatus",
                        QueryConditionOperatorType.Equal, queryCriteria.MemoStatus);
                }

                if (queryCriteria.CreateDateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, 
                        "a.CreateTime", DbType.Date, "@CreateDateFrom", 
                        QueryConditionOperatorType.MoreThan, queryCriteria.CreateDateFrom);
                }

                if (queryCriteria.CreateDateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, 
                        "a.CreateTime", DbType.Date, "@CreateDateTo", 
                        QueryConditionOperatorType.LessThan, queryCriteria.CreateDateTo.Value.AddDays(1));
                }

                if (queryCriteria.RequestSysNo.HasValue && queryCriteria.RequestSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, 
                        "a.ShiftSysNo", DbType.Int32, "@RequestSysNo", 
                        QueryConditionOperatorType.Equal, queryCriteria.RequestSysNo.Value);    
                }

                if (queryCriteria.CreateUserSysNo.HasValue && queryCriteria.CreateUserSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, 
                        "a.CreateUserSysNo", DbType.Int32, "@CreateUserSysNo", 
                        QueryConditionOperatorType.Equal, queryCriteria.CreateUserSysNo.Value);
                }

                if (queryCriteria.EditUserSysNo.HasValue && queryCriteria.EditUserSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, 
                        "a.UpdateUserSysNo", DbType.Int32, "@EditUserSysNo", 
                        QueryConditionOperatorType.Equal, queryCriteria.EditUserSysNo);

                }   

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("MemoStatus", typeof(ShiftRequestMemoStatus));

                var resultData = cmd.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }
        }
    }
}
