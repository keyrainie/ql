using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IShiftRequestItemBasketQueryDA))]
    public class ShiftRequestItemBasketQueryDA : IShiftRequestItemBasketQueryDA
    {
        #region IShiftRequestItemBasketQuery Members

        private string SortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "INUSER":
                        tsort[0] = "a.InUser";
                        break;
                    case "INDATE":
                        tsort[0] = "a.InDate";
                        break;
                    case "PRODUCTSYSNO":
                        tsort[0] = "a.ProductSysNo";
                        break;
                    case "PRODUCTID":
                        tsort[0] = "b.ProductID";
                        break;
                    case "PRODUCTNAME":
                        tsort[0] = "b.ProductName";
                        break;
                    case "OUTSTOCKNAME":
                        tsort[0] = "c.StockName";
                        break;
                    case "INSTOCKNAME":
                        tsort[0] = "d.StockName";
                        break;  
                    case "SHIFTQTY":
                        tsort[0] = "a.ShiftQty";
                    break;  
                        
                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        public System.Data.DataTable QueryShiftRequestItemBasketList(QueryFilter.Inventory.ShiftRequestItemBasketQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            DataTable dt = new DataTable();
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = SortFieldMapping(queryFilter.PagingInfo.SortBy),
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            CustomDataCommand dataCommand =
                DataCommandManager.CreateCustomDataCommandFromConfig("GetShiftBasketByQuery");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "a.SysNo"))
            {
                #region 构建查询条件:
                if (!string.IsNullOrEmpty(queryFilter.ShiftOutStockSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNoA",
                                                                 DbType.Int32, "@StockSysNoA",
                                                                 QueryConditionOperatorType.Equal,
                                                                Convert.ToInt32(queryFilter.ShiftOutStockSysNo));
                }
                if (!string.IsNullOrEmpty(queryFilter.ShiftInStockSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNoB",
                                                                 DbType.Int32, "@StockSysNoB",
                                                                 QueryConditionOperatorType.Equal,
                                                                 Convert.ToInt32(queryFilter.ShiftInStockSysNo));
                }
                if (!string.IsNullOrEmpty(queryFilter.CreateUserName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.InUser",
                                                                 DbType.String, "@InUser",
                                                                 QueryConditionOperatorType.Equal,
                                                                 queryFilter.CreateUserName);
                }
                #endregion

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return dt;
        }

        #endregion
    }
}
