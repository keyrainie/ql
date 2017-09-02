using System;
using System.Data;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Text.RegularExpressions;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IStockQueryDA))]
    public class StockQueryDA : IStockQueryDA
    {
        private string SortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "SYSNO":
                        tsort[0] = "a.SysNo";
                        break;
                    case "STOCKID":
                        tsort[0] = "a.StockID";
                        break;
                    case "STOCKNAME":
                        tsort[0] = "a.StockName";
                        break;
                    case "WAREHOUSENAME":
                        tsort[0] = "b.WarehouseName";
                        break;
                    case "STOCKSTATUS":
                        tsort[0] = "a.[Status]";
                        break;
                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }
        /// <summary>
        /// 查询渠道仓库
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryStock(StockQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = SortFieldMapping(filter.PagingInfo.SortBy);
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryStock");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "a.SysNo DESC"))
            {
                bool isSysNo = false;
                //编号格式验证
                if (filter.StockSysNo != null && Regex.IsMatch(filter.StockSysNo, @"^[,\. ]*\d+[\d,\. ]*$"))
                {
                    filter.StockSysNo = String.Join(",", filter.StockSysNo.Split(new char[] { '.', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    isSysNo = true;
                }
                else
                {
                    filter.StockSysNo = null;
                }

                if (filter.StockID != null && Regex.IsMatch(filter.StockID, @"^[, ]*\w+[\w-#, ]*$"))
                {
                    filter.StockID = "'" + String.Join("','", filter.StockID.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)) + "'";
                    isSysNo = true;
                }
                else
                {
                    filter.StockID = null;
                }
                if (isSysNo)
                {
                    sb.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "a.SysNo", QueryConditionOperatorType.In, filter.StockSysNo);
                    sb.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND,
                            "a.StockID", QueryConditionOperatorType.In, filter.StockID);
                    sb.ConditionConstructor.EndGroupCondition();
                }
              
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.StockName", DbType.String, "@StockName",
                        QueryConditionOperatorType.LeftLike, filter.StockName);

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.Status", DbType.AnsiStringFixedLength, "@StockStatus",
                        QueryConditionOperatorType.Equal, filter.StockStatus);

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.WarehouseSysNo", DbType.Int32, "@WarehouseSysNo",
                        QueryConditionOperatorType.Equal, filter.WarehouseSysNo);

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.WebChannelSysNo", DbType.Int32, "@WebChannelSysNo",
                        QueryConditionOperatorType.Equal, filter.WebChannelSysNo);
             
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                        QueryConditionOperatorType.Equal, filter.CompanyCode);
                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("StockStatus", typeof(ValidStatus));
                cmd.CommandText = sb.BuildQuerySql();
                var resultData = cmd.ExecuteDataTable(enumColumnList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return resultData;
            }
        }
    }
}
