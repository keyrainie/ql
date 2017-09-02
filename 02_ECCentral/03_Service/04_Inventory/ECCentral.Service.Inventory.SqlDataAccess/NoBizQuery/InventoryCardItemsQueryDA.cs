using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IInventoryCardItemsQueryDA))]
    class InventoryCardItemsQueryDA : IInventoryCardItemsQueryDA
    {
        #region IInventoryCardItemsQueryDA Members

        public DataTable QueryCardItemOrdersRelated(InventoryItemCardQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryFilter == null || queryFilter.ProductSysNo == null || !queryFilter.ProductSysNo.HasValue)
            {
                totalCount = 0;
                return null;
            }
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };
            CustomDataCommand dataCommand;
            if (queryFilter.StockSysNo.HasValue)
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryItemsCardReLatedOrder");
                dataCommand.SetParameterValue("@WarehouseSysNo", queryFilter.StockSysNo.Value);
            }
            else
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryItemsCardReLatedOrderWithOutWarehouseNumber");
            }

            dataCommand.SetParameterValue("@ItemSysNo", queryFilter.ProductSysNo.Value);

            if (queryFilter.RMAInventoryOnlineDate.HasValue)
            {
                dataCommand.AddInputParameter("@RevertTime", DbType.DateTime, queryFilter.RMAInventoryOnlineDate.Value);
            }

            dataCommand.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "OrderTime DESC"))
            {
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }

            #region 计算结存数量
            if (null != dt)
            {
                dt.Columns.Add("OrderThenQty", typeof(int));
            }
            var orderThenQty = 0;
            for (var i = dt.Rows.Count - 1; i >= 0; i--)
            {
                //OrderCodeFromDB=3 means po-调价(instock)(PO调价单)
                if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "3")
                {
                    orderThenQty += (dt.Rows[i]["OrderQty"] == null ? 0 : int.Parse(dt.Rows[i]["OrderQty"].ToString()));
                }
                dt.Rows[i]["OrderThenQty"] = orderThenQty;
            }
            #endregion

            if (null != dt && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["OrderNameString"] = CodeNamePairManager.GetName("Inventory", "InventoryCardOrderType", dr["OrderName"].ToString());
                }
            }

            return dt;
        }

        #endregion
    }
}
