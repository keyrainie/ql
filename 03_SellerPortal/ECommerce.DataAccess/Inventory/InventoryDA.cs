using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Inventory;
using ECommerce.Utility.DataAccess;
using System.Data;
using ECommerce.Entity.Common;
using ECommerce.Utility;

namespace ECommerce.DataAccess.Inventory
{
    public class InventoryDA
    {
        /// <summary>
        /// 查询总仓库存
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<InventoryQueryInfo> QueryProductInventoryTotal(InventoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductInventoryTotal");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, queryFilter, "a.ProductSysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);//8601
                if (!string.IsNullOrEmpty(queryFilter.MerchantSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[MerchantSysNo]", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, queryFilter.MerchantSysNo);
                }

                if (!string.IsNullOrEmpty(queryFilter.ProductSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductSysNo", DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);
                }

                if (!string.IsNullOrEmpty(queryFilter.AuthorizedPMsSysNumber))
                {
                    //queryFilter.AuthorizedPMsSysNumber = "-999";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "b.PMUserSysNo", QueryConditionOperatorType.In, queryFilter.AuthorizedPMsSysNumber);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                List<InventoryQueryInfo> resultList = dataCommand.ExecuteEntityList<InventoryQueryInfo>();
                int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                return new QueryResult<InventoryQueryInfo>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }

        /// <summary>
        /// 查询分仓库存
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static QueryResult<InventoryQueryInfo> QueryProductInventoryByStock(InventoryQueryFilter queryFilter)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductInventoryByStock");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, queryFilter, "a.ProductSysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);//8601
                if (!string.IsNullOrEmpty(queryFilter.MerchantSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.[MerchantSysNo]", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, queryFilter.MerchantSysNo);
                }

                if (!string.IsNullOrEmpty(queryFilter.ProductSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductSysNo", DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);
                }

                if (!string.IsNullOrEmpty(queryFilter.StockSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNo", DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                List<InventoryQueryInfo> resultList = dataCommand.ExecuteEntityList<InventoryQueryInfo>();
                int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                return new QueryResult<InventoryQueryInfo>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }

        /// <summary>
        /// 查询库存变化单据
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<InventoryItemCardInfo> QueryCardItemOrdersRelated(InventoryItemCardQueryFilter queryFilter)
        {
            QueryResult<InventoryItemCardInfo> result = new QueryResult<InventoryItemCardInfo>();

            CustomDataCommand dataCommand;
            if (!string.IsNullOrEmpty(queryFilter.StockSysNo))
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryItemsCardReLatedOrder");
                dataCommand.SetParameterValue("@WarehouseSysNo", queryFilter.StockSysNo);
            }
            else
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryItemsCardReLatedOrderWithOutWarehouseNumber");
            }

            dataCommand.SetParameterValue("@ItemSysNo", queryFilter.ProductSysNo);
            dataCommand.SetParameterValue("@MerchantSysNo", queryFilter.MerchantSysNo);

            if (queryFilter.RMAInventoryOnlineDate != null && queryFilter.RMAInventoryOnlineDate != new DateTime())
            {
                dataCommand.AddInputParameter("@RevertTime", DbType.DateTime, queryFilter.RMAInventoryOnlineDate.ToString("yyyy-MM-dd"));
            }

            dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);//8601
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, queryFilter, "OrderTime DESC"))
            {
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                List<InventoryItemCardInfo> resultList = dataCommand.ExecuteEntityList<InventoryItemCardInfo>();
                int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                result = new QueryResult<InventoryItemCardInfo>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }

            #region 计算结存数量
            if (result.ResultList != null && result.ResultList.Count > 0)
            {
                var orderThenQty = 0;
                for (var i = result.ResultList.Count - 1; i >= 0; i--)
                {
                    InventoryItemCardInfo cardInfo = result.ResultList[i];
                    //OrderCodeFromDB=3 means po-调价(instock)(PO调价单)
                    if (cardInfo.OrderCode != "3")
                    {
                        orderThenQty += cardInfo.OrderQty;
                    }
                    cardInfo.OrderThenQty = orderThenQty;
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 查询仓库
        /// </summary>
        /// <param name="merchantSysNo">商家编号</param>
        /// <returns></returns>
        public static List<StockInfo> GetStock(int merchantSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetStock");
            cmd.SetParameterValue("@MerchantSysNo", merchantSysNo);
            return cmd.ExecuteEntityList<StockInfo>();
        }

        #region [损益单  相关方法]
        public static AdjustRequestInfo GetAdjustRequestInfoBySysNo(int requestSysNo)
        {
            AdjustRequestInfo adjustRequest = GetAdjustRequestMainInfoBySysNo(requestSysNo);
            if (adjustRequest != null)
            {
                adjustRequest.AdjustItemInfoList = GetAdjustItemListByRequestSysNo(requestSysNo);
                adjustRequest.InvoiceInfo = GetInvoiceInfoByRequestSysNo(requestSysNo);

            }
            return adjustRequest;
        }

        /// <summary>
        /// 根据SysNO获取损益单信息
        /// </summary>
        /// <param name="brandSysNo"></param>
        /// <returns></returns>
        private static AdjustRequestInfo GetAdjustRequestMainInfoBySysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetAdjustRequestBySysNo");
            dc.SetParameterValue("@SysNo", requestSysNo);
            return dc.ExecuteEntity<AdjustRequestInfo>();
        }

        /// <summary>
        /// 根据SysNO获取损益单商品列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        private static List<AdjustRequestItemInfo> GetAdjustItemListByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetAdjustItemListByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<AdjustRequestItemInfo, List<AdjustRequestItemInfo>>(reader);
                return list;
            }
        }

        /// <summary>
        /// 根据SysNO获取损益单发票信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        private static AdjustRequestInvoiceInfo GetInvoiceInfoByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetAdjustInvoiceByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);
            return dc.ExecuteEntity<AdjustRequestInvoiceInfo>();
        }

        /// <summary>
        /// 获取商品库存成本
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static decimal GetItemCost(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetItemCost");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteScalar<decimal>();
        }

        /// <summary>   
        /// 更新损益成本
        /// </summary>
        /// <param name="adjustItem"></param>      
        /// <returns></returns>
        public static void UpdateAdjustItemCost(AdjustRequestItemInfo adjustItem)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_UpdateAdjustItemCost");
            command.SetParameterValue("@AdjustItemSysNo", adjustItem.SysNo);
            command.SetParameterValue("@AdjustCost", adjustItem.AdjustCost);
            command.ExecuteNonQuery();
        }


        /// <summary>
        /// 更新损益单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static AdjustRequestInfo UpdateAdjustRequestStatus(AdjustRequestInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_UpdateAdjustRequestStatus");
            dc.SetParameterValue("@RequestSysNo", entity.SysNo);
            dc.SetParameterValue("@RequestStatus", (int)entity.RequestStatus);
            dc.SetParameterValue("@AuditDate", entity.AuditDate);
            dc.SetParameterValue("@AuditUserSysNo", 1/*entity.AuditUser.SysNo*/);
            dc.SetParameterValue("@OutStockDate", entity.OutStockDate);
            dc.SetParameterValue("@OutStockUserSysNo", 1/* entity.OutStockUser.SysNo*/);

            return dc.ExecuteEntity<AdjustRequestInfo>();
        }

        /// <summary>
        /// 获取商品归属Vendor
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static int GetProductBelongVendorSysNo(int productSysNo)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductBelongVendorSysNo");
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            Object result = dataCommand.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        /// <summary>
        /// 创建代销转财务记录(Inventory)
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public static ConsignToAcctLogInfo CreatePOConsignToAccLogForInventory(ConsignToAcctLogInfo logInfo)
        {
            var command = DataCommandManager.GetDataCommand("CreatePOConsignToAccLogForInventory");
            command.SetParameterValue("@ProductSysNo", logInfo.ProductSysNo);
            command.SetParameterValue("@StockSysNo", logInfo.StockSysNo);
            command.SetParameterValue("@Quantity", logInfo.ProductQuantity);
            command.SetParameterValue("@CreateTime", logInfo.OutStockTime);
            command.SetParameterValue("@CreateCost", logInfo.CreateCost);
            command.SetParameterValue("@CompanyCode", logInfo.CompanyCode);
            command.SetParameterValue("@StoreCompanyCode", logInfo.StoreCompanyCode);
            command.SetParameterValue("@OrderSysNo", logInfo.OrderSysNo);
            command.SetParameterValue("@VendorSysNoOut", logInfo.VendorSysNo.HasValue ? logInfo.VendorSysNo : 0);
            command.SetParameterValue("@IsConsign", logInfo.IsConsign.HasValue ? logInfo.IsConsign : 1);
            logInfo.LogSysNo = System.Convert.ToInt32(command.ExecuteScalar());
            return logInfo;
        }


        /// <summary>
        /// 根据单据编号 获取批号信息表信息
        /// </summary>
        /// <param name="DocumentNumber"></param>
        /// <returns></returns>
        public static List<InventoryBatchDetailsInfo> GetBatchDetailsInfoEntityListByNumber(int DocumentNumber)
        {
            var command = DataCommandManager.GetDataCommand("GetBatchDetailsInfoEntityListByNumber");
            command.SetParameterValue("@Number", DocumentNumber);
            return command.ExecuteEntityList<InventoryBatchDetailsInfo>();
        }

        /// <summary>
        /// 单据出库后给仓库发SSB
        /// </summary>
        /// <param name="paramXml">SSB</param>
        /// <returns></returns>
        public static void SendSSBToWMS(string paramXml)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_SendSSBToWMS");
            command.SetParameterValue("@Msg", paramXml);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 检测 是否是批号管理商品
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static bool CheckISBatchNumberProduct(int productSysNo)
        {
            var command = DataCommandManager.GetDataCommand("Inventory_CheckISBatchNumberProduct");
            command.SetParameterValue("@SysNo", productSysNo);
            if (command.ExecuteScalar() != null && command.ExecuteScalar().ToString() == "Y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region [更新商品库存相关方法:]
        /// <summary>
        /// 更新商品渠道仓库各项库存数量(增量更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        public static void AdjustProductStockInventoryInfo(ProductInventoryInfo inventoryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_AdjustProductInventoryByStock");
            cmd.SetParameterValue<ProductInventoryInfo>(inventoryInfo);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 更新商品渠道仓库各项库存数量(增量更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        public static void AdjustProductTotalInventoryInfo(ProductInventoryInfo inventoryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_AdjustProductTotalInventory");
            cmd.SetParameterValue<ProductInventoryInfo>(inventoryInfo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 初始化商品库存记录
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        public static void InitProductInventoryInfo(int productSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_InitProductInventoryInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@StockSysNo", stockSysNo);
            cmd.ExecuteNonQuery();
        }


        public static int GetProductInventroyType(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductInventroyType");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteScalar<int>();
        }


        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productID">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓库</param>
        /// <returns>ProductInventoryEntity</returns>
        public static ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductInventoryInfoByStock");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@StockSysNo", stockSysNo);
            return cmd.ExecuteEntity<ProductInventoryInfo>();
        }

        /// <summary>
        /// 获取指定商品的总库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductTotalInventoryInfoByProduct");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductInventoryInfo>();
        }

        public static List<ProductCostIn> GetProductCostIn(int productSysNo, int poSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductCostIn");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@PoSysNo", poSysNo);
            cmd.SetParameterValue("@WarehouseNumber", stockSysNo);
            return cmd.ExecuteEntityList<ProductCostIn>();
        }

        public static void LockProductCostInList(List<ProductCostIn> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_LockProductCostInList");
                cmd.SetParameterValue("@SysNo", item.SysNo);
                cmd.SetParameterValue("@LockQuantity", item.LockQuantity);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 单据作废 取消作废 出库时执行此方法
        /// </summary>
        /// <param name="paramXml"></param>
        /// <returns></returns>  
        public static int AdjustBatchNumberInventory(string paramXml)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AdjustBatchNumberInventory");
            command.SetParameterValue("@Msg", paramXml);
            return command.ExecuteNonQuery();
        }


        public static void WriteCostLog(int billType, int billSysNo, string msg)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_WriteCostLog");
            cmd.SetParameterValue("@BillType", billType);
            cmd.SetParameterValue("@BillSysNo", billSysNo);
            cmd.SetParameterValue("@Message", msg);
            cmd.ExecuteNonQuery();
        }

        public static void WriteProductCost(List<ProductCostIn> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_WriteProductCost");
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@BillType", item.BillType);
                cmd.SetParameterValue("@BillSysNo", item.BillSysNo);
                cmd.SetParameterValue("@Quantity", item.Quantity);
                cmd.SetParameterValue("@Cost", item.Cost);
                cmd.SetParameterValue("@WarehouseNumber", item.WarehouseNumber);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<ProductCostIn> GetCostList(int productSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetCostListByProductStock");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@WarehouseNumber", stockSysNo);
            return cmd.ExecuteEntityList<ProductCostIn>();
        }

        public static void UpdateProductCost(List<ProductCostOut> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_UpdateProductCost");
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@BillType", item.BillType);
                cmd.SetParameterValue("@BillSysNo", item.BillSysNo);
                cmd.SetParameterValue("@Quantity", item.Quantity);
                cmd.SetParameterValue("@Cost", item.Cost);
                cmd.SetParameterValue("@CostInSysNo", item.CostInSysNo);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateCostToBiz(List<ProductCostOut> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_UpdateCostToBiz");
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@BillType", item.BillType);
                cmd.SetParameterValue("@BillSysNo", item.BillSysNo);
                cmd.SetParameterValue("@UnitCost", item.Cost);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
