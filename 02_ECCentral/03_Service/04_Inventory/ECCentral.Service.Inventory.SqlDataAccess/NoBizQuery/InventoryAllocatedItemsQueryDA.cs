using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IInventoryAllocatedItemsQueryDA))]
    public class InventoryAllocatedItemsQueryDA : IInventoryAllocatedItemsQueryDA
    {
        #region IInventoryAllocatedItemsQueryDA Members

        public DataTable QueryAllocatedItemOrdersRelated(QueryFilter.Inventory.InventoryAllocatedCardQueryFilter queryFilter, out int totalCount)
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
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryItemAllocatedCardReLatedOrder");
                dataCommand.SetParameterValue("@WarehouseSysNumber", queryFilter.StockSysNo.Value);
            }
            else
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryItemAllocatedCardReLatedOrderWithOutWarehouseNumber");
            }

            dataCommand.SetParameterValue("@ItemSysNumber", queryFilter.ProductSysNo.Value);
            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "OrderTime DESC"))
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
                //OrderCode means po-调价(instock)(PO调价单)
                if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "3")
                    orderThenQty += (dt.Rows[i]["OrderQty"] != null ? int.Parse(dt.Rows[i]["OrderQty"].ToString()) : 0);
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
