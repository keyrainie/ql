using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.Restful.ResponseMsg;
using ECCentral.Service.Inventory.AppService;
using System.Data;
using System.Reflection;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Inventory.Request;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.Inventory.Restful
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public partial class InventoryService
    {
        [WebInvoke(UriTemplate = "/Inventory/QueryInventoryList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryInventoryList(InventoryQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;

            if (null != queryFilter.PMQueryRightType)
            {
                List<int> pms = new List<int>();
                pms = new InentoryAppService().QueryPMListByRight(queryFilter.PMQueryRightType.Value, queryFilter.UserName, queryFilter.CompanyCode);
                if (pms != null && pms.Count > 0)
                {
                    foreach (var item in pms)
                    {
                        queryFilter.AuthorizedPMsSysNumber += "," + item;
                    }
                }
                if (queryFilter.AuthorizedPMsSysNumber.Contains(","))
                {
                    queryFilter.AuthorizedPMsSysNumber = queryFilter.AuthorizedPMsSysNumber.Remove(0, 1);
                }
            }

            //如果是查询总库存，则调用QueryInventory的service,否则调用QueryInventoryStock:
            if (queryFilter.IsShowTotalInventory.HasValue && queryFilter.IsShowTotalInventory.Value == true)
            {
                result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryInventoryList(queryFilter, out getTotalCount);

            }
            else
            {
                result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryInventoryStockList(queryFilter, out getTotalCount);
            }
            result.TotalCount = getTotalCount;
            return result;
        }

        //查询商品库存信息
        [WebInvoke(UriTemplate = "/Inventory/QueryProductInventory", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductInventory(InventoryQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;

            if (null != queryFilter.PMQueryRightType)
            {
                List<int> pms = new List<int>();
                pms = new InentoryAppService().QueryPMListByRight(queryFilter.PMQueryRightType.Value, queryFilter.UserName, queryFilter.CompanyCode);
                if (pms != null && pms.Count > 0)
                {
                    foreach (var item in pms)
                    {
                        queryFilter.AuthorizedPMsSysNumber += "," + item;
                    }
                }
                if (queryFilter.AuthorizedPMsSysNumber.Contains(","))
                {
                    queryFilter.AuthorizedPMsSysNumber = queryFilter.AuthorizedPMsSysNumber.Remove(0, 1);
                }
            }

            //如果是查询总库存，则调用QueryInventory的service,否则调用QueryInventoryStock:
            if (queryFilter.IsShowTotalInventory.HasValue && queryFilter.IsShowTotalInventory.Value == true)
            {
                result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryProductInventoryTotal(queryFilter, out getTotalCount);

            }
            else
            {
                result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryProductInventoryByStock(queryFilter, out getTotalCount);
            }
            result.TotalCount = getTotalCount;
            return result;
        }

        /// <summary>
        /// 查询 - 备货中心List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryInventoryTransferStockingList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public InventoryTransferStockingQueryRsp QueryInventoryTransferStockingList(InventoryTransferStockingQueryFilter queryFilter)
        {
            InventoryTransferStockingQueryRsp result = new InventoryTransferStockingQueryRsp();
            int getTotalCount = 0;
            result.ResultList = ObjectFactory<IInventoryTransferStockingDA>.Instance.QueryInventoryTransferStockingList(queryFilter, out getTotalCount);
            result.TotalCount = getTotalCount;
            return result;
        }

        [WebInvoke(UriTemplate = "/Inventory/QueryInventoryTransferStockingListForExportExcel", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryInventoryTransferStockingListForExport(InventoryTransferStockingQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            List<ProductCenterItemInfo> itemInfoList = ObjectFactory<IInventoryTransferStockingDA>.Instance.QueryInventoryTransferStockingList(queryFilter, out getTotalCount);
            #region [构建DataTable]
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemCode");
            dt.Columns.Add("ItemName");
            dt.Columns.Add("ItemSysNumber");
            dt.Columns.Add("ManufacturerName");
            dt.Columns.Add("Brand");
            dt.Columns.Add("AllAvailableQty");
            dt.Columns.Add("ConsignQty");
            dt.Columns.Add("AllStockAVGDailySales");
            dt.Columns.Add("AllStockAvailableSalesDays");
            dt.Columns.Add("AllOutStockQuantity");
            dt.Columns.Add("VirtualQty");
            dt.Columns.Add("InventoryQty");
            dt.Columns.Add("PurchasePrice");
            dt.Columns.Add("OrderQty");
            dt.Columns.Add("TransferStockQty");
            dt.Columns.Add("PurchaseQty");
            dt.Columns.Add("SuggestQtyAll");
            dt.Columns.Add("VirtualPrice");
            dt.Columns.Add("UnitCost");
            dt.Columns.Add("GrossProfitRate");
            dt.Columns.Add("UnmarketableQty");
            dt.Columns.Add("D1");
            dt.Columns.Add("D2");
            dt.Columns.Add("D3");
            dt.Columns.Add("D4");
            dt.Columns.Add("D5");
            dt.Columns.Add("D6");
            dt.Columns.Add("D7");
            dt.Columns.Add("W1");
            dt.Columns.Add("W2");
            dt.Columns.Add("W3");
            dt.Columns.Add("W4");
            dt.Columns.Add("M1");
            dt.Columns.Add("M2");
            dt.Columns.Add("M3");
            dt.Columns.Add("PO_Memo");
            dt.Columns.Add("CurrentPrice");
            dt.Columns.Add("JDPrice");

            if (null != itemInfoList && itemInfoList.Count > 0)
            {
                for (int i = 0; i < itemInfoList[0].SuggestTransferStocks.Count; i++)
                {
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_AvailableQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_VirtualQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_OrderQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_ConsignQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_AvailableSalesDays");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_PurchaseInQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_ShiftInQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_D1");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_D2");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_D3");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_D4");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_D5");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_D6");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_D7");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_W1");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_W2");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_W3");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_W4");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_M1");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_M2");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_M3");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_W1RegionSalesQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_M1RegionSalesQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_AVGDailySales");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_SuggestQty");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_Lastintime");
                    dt.Columns.Add("SuggestTransferStocks_" + i + "_LastPrice");
                }
            }

            itemInfoList.ForEach(x =>
            {
                DataRow dr = dt.NewRow();
                dr["ItemCode"] = x.ItemCode;
                dr["ItemName"] = x.ItemName;
                dr["ItemSysNumber"] = x.ItemSysNumber.ToInteger();
                dr["ManufacturerName"] = x.ManufacturerName;
                dr["Brand"] = x.Brand;
                dr["AllAvailableQty"] = x.AllAvailableQty.ToInteger();
                dr["ConsignQty"] = x.ConsignQty.ToInteger();
                dr["AllStockAVGDailySales"] = x.AllStockAVGDailySales.ToDecimal().ToString("f2");
                dr["AllStockAvailableSalesDays"] = x.AllStockAvailableSalesDays.ToInteger();
                dr["AllOutStockQuantity"] = x.AllOutStockQuantity.ToInteger();
                dr["VirtualQty"] = x.VirtualQty.ToInteger();
                dr["InventoryQty"] = x.InventoryQty.ToInteger();
                dr["PurchasePrice"] = x.PurchasePrice.ToDecimal().ToString("f2");
                dr["OrderQty"] = x.OrderQty.ToInteger();
                dr["TransferStockQty"] = x.TransferStockQty.ToInteger();
                dr["PurchaseQty"] = x.PurchaseQty.ToInteger();
                dr["SuggestQtyAll"] = x.SuggestQtyAll.ToInteger();
                dr["VirtualPrice"] = x.VirtualPrice.ToDecimal().ToString("f2");
                dr["UnitCost"] = x.UnitCost.ToDecimal().ToString("f2");
                dr["GrossProfitRate"] = x.GrossProfitRate;
                dr["D1"] = x.D1.ToInteger();
                dr["D2"] = x.D2.ToInteger();
                dr["D3"] = x.D3.ToInteger();
                dr["D4"] = x.D4.ToInteger();
                dr["D5"] = x.D5.ToInteger();
                dr["D6"] = x.D6.ToInteger();
                dr["D7"] = x.D7.ToInteger();
                dr["W1"] = x.W1.ToInteger();
                dr["W2"] = x.W2.ToInteger();
                dr["W3"] = x.W3.ToInteger();
                dr["W4"] = x.W4.ToInteger();
                dr["M1"] = x.M1.ToInteger();
                dr["M2"] = x.M2.ToInteger();
                dr["M3"] = x.M3.ToInteger();
                dr["PO_Memo"] = x.PO_Memo;
                dr["CurrentPrice"] = x.CurrentPrice.ToDecimal().ToString("f2");
                dr["JDPrice"] = x.JDPrice.ToDecimal().ToString("f2");

                int stockIndex = 0;
                x.SuggestTransferStocks.ForEach(y =>
                {
                    dr["SuggestTransferStocks_" + stockIndex + "_AvailableQty"] = y.AvailableQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_VirtualQty"] = y.VirtualQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_OrderQty"] = y.OrderQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_ConsignQty"] = y.ConsignQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_AvailableSalesDays"] = y.AvailableSalesDays.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_PurchaseInQty"] = y.PurchaseInQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_ShiftInQty"] = y.ShiftInQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_D1"] = y.D1.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_D2"] = y.D2.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_D3"] = y.D3.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_D4"] = y.D4.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_D5"] = y.D5.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_D6"] = y.D6.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_D7"] = y.D7.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_W1"] = y.W1.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_W2"] = y.W2.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_W3"] = y.W3.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_W4"] = y.W4.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_M1"] = y.M1.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_M2"] = y.M2.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_M3"] = y.M3.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_W1RegionSalesQty"] = y.W1RegionSalesQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_M1RegionSalesQty"] = y.M1RegionSalesQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_AVGDailySales"] = y.AVGDailySales.ToDecimal().ToString("f2");
                    dr["SuggestTransferStocks_" + stockIndex + "_SuggestQty"] = y.SuggestQty.ToInteger();
                    dr["SuggestTransferStocks_" + stockIndex + "_Lastintime"] = y.Lastintime;
                    dr["SuggestTransferStocks_" + stockIndex + "_LastPrice"] = y.LastPrice.ToDecimal().ToString("f2");
                    stockIndex++;
                });

                dt.Rows.Add(dr);

            });
            #endregion

            result.Data = dt;
            result.TotalCount = getTotalCount;
            return result;
        }

        /// <summary>
        /// 备货中心 - 当日需备货供应商查询
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryVendorInfoListForBackOrderToday", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryVendorInfoListForBackOrderToday(BackOrderForTodayQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryVendorInfoForBackOrderToday(queryFilter, out getTotalCount);
            result.TotalCount = getTotalCount;
            return result;
        }

        #region 指定商品库存查询

        /// <summary>
        /// 根据商品系统编号查询的商品总库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/GetProductTotalInventoryInfo", Method = "POST")]
        public ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductTotalInventoryInfo(productSysNo);
        }

        /// <summary>
        /// 根据商品系统编号查询的商品总库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/GetProductInventoryInfo", Method = "POST")]
        public List<ProductInventoryInfo> GetProductInventoryInfo(int productSysNo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductInventoryInfo(productSysNo);
        }

        #endregion 指定商品库存查询

        #region Temp Test

        /// <summary>
        /// 根据商品系统编号查询的商品总库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/TestInventoryService", Method = "GET")]
        public String TestInventoryService()
        {


            StringBuilder result = new StringBuilder();
            result.Append("Inventory Service Result: ");
            try
            {
                TestBasicInfo();
            }
            catch (Exception ex)
            {
                result.Append(ex.Message);
            }


            return result.ToString();
        }

        private void TestInventoryAdjustTemp()
        {
            InventoryAdjustContractInfo adjustContractInfo = new InventoryAdjustContractInfo();
            adjustContractInfo.ReferenceSysNo = "111";
            adjustContractInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();

            InventoryAdjustItemInfo adjustItem1 = new InventoryAdjustItemInfo()
            {
                ProductSysNo = 36492,
                StockSysNo = 51,
                AdjustQuantity = 2
            };
            InventoryAdjustItemInfo adjustItem2 = new InventoryAdjustItemInfo()
            {
                ProductSysNo = 999998,
                StockSysNo = 51,
                AdjustQuantity = -9
            };

            adjustContractInfo.AdjustItemList.Add(adjustItem1);
            adjustContractInfo.AdjustItemList.Add(adjustItem2);

            adjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.SO_Order;
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Create;
            Object r61 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);

        }

        private void TestInventoryAdjust()
        {

            InventoryAdjustContractInfo adjustContractInfo = new InventoryAdjustContractInfo();
            adjustContractInfo.ReferenceSysNo = "111";
            adjustContractInfo.AdjustItemList = new List<InventoryAdjustItemInfo>();

            InventoryAdjustItemInfo adjustItem1 = new InventoryAdjustItemInfo()
            {
                ProductSysNo = 999999,
                StockSysNo = 51,
                AdjustQuantity = 9
            };
            InventoryAdjustItemInfo adjustItem2 = new InventoryAdjustItemInfo()
            {
                ProductSysNo = 999998,
                StockSysNo = 51,
                AdjustQuantity = -9
            };

            adjustContractInfo.AdjustItemList.Add(adjustItem1);
            adjustContractInfo.AdjustItemList.Add(adjustItem2);


            adjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_LendRequest;
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Create;
            Object r11 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Abandon;
            Object r12 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.CancelAbandon;
            Object r13 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Update;
            Object r14 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.OutStock;
            Object r15 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Return;
            Object r16 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);

            adjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_AdjustRequest;
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Create;
            Object r21 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Abandon;
            Object r22 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.CancelAbandon;
            Object r23 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Update;
            Object r24 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.OutStock;
            Object r25 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);

            adjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_ConvertRequest;
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Create;
            Object r31 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Abandon;
            Object r32 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.CancelAbandon;
            Object r33 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Update;
            Object r34 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.OutStock;
            Object r35 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);

            adjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_ShiftRequest;
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Create;
            Object r41 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Abandon;
            Object r42 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.CancelAbandon;
            Object r43 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Update;
            Object r44 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.OutStock;
            Object r45 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.InStock;
            Object r46 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.AbandonForPO;
            Object r47 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);

            adjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_VirtualRequest;
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Create;
            Object r51 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.CreateForJob;
            Object r52 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Run;
            Object r53 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Close;
            Object r54 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);

            adjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.SO_Order;
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Create;
            Object r61 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Abandon;
            Object r62 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Abandon_RecoverStock;
            Object r63 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.OutStock;
            Object r64 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.WHUpdate;
            Object r65 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Pending;
            Object r66 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);

            adjustContractInfo.SourceBizFunctionName = InventoryAdjustSourceBizFunction.PO_Order;
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.Audit;
            Object r71 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.CancelAudit;
            Object r72 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.StopInStock;
            Object r73 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);
            adjustContractInfo.SourceActionName = InventoryAdjustSourceAction.InStock;
            Object r74 = ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(adjustContractInfo);

        }

        private void TestBasicInfo()
        {
            int productSysNo = 999999;
            int stockSysNo = 51;
            string companyCode = "8601";
            string webChannelID = "0";
            List<int> productSysNoList = new List<int>();
            productSysNoList.Add(999999);
            productSysNoList.Add(999998);
            productSysNoList.Add(999997);

            ObjectFactory<ProductInventoryAppService>.Instance.InitProductInventoryInfo(productSysNo);


            Object r8 = ObjectFactory<StockAppService>.Instance.GetWarehouseList(companyCode);
            Object r9 = ObjectFactory<StockAppService>.Instance.GetStockList(webChannelID);
            Object r10 = ObjectFactory<StockAppService>.Instance.GetStockInfo(stockSysNo);

            ObjectFactory<ProductInventoryAppService>.Instance.SetProductReservedQty(productSysNo, stockSysNo, 9);

            Object r1 = ObjectFactory<ProductInventoryAppService>.Instance.GetProductInventoryInfo(productSysNo);
            Object r2 = ObjectFactory<ProductInventoryAppService>.Instance.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
            Object r3 = ObjectFactory<ProductInventoryAppService>.Instance.GetProductTotalInventoryInfo(productSysNo);
            Object r4 = ObjectFactory<ProductInventoryAppService>.Instance.GetProductTotalInventoryInfoByProductList(productSysNoList);
            Object r11 = ObjectFactory<ProductInventoryAppService>.Instance.GetProductSalesTrendInfoByStock(productSysNo, stockSysNo);
            Object r12 = ObjectFactory<ProductInventoryAppService>.Instance.GetProductSalesTrendInfo(productSysNo);
            Object r13 = ObjectFactory<ProductInventoryAppService>.Instance.GetProductSalesTrendInfoTotal(productSysNo);
        }
        #endregion Temp Test

        [WebInvoke(UriTemplate = "/Inventory/QueryPMMonitoringPerformanceIndicators", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryPMMonitoringPerformanceIndicators(PMMonitoringPerformanceIndicatorsQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            result.Data = ObjectFactory<IInventoryQueryDA>.Instance.QueryPMMonitoringPerformanceIndicators(queryFilter, out getTotalCount);
            result.TotalCount = getTotalCount;
            return result;
        }

        /// <summary>
        /// 当前用户是否有访问此商品的权限
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/CheckOperateRightForCurrentUser", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public bool CheckOperateRightForCurrentUser(InventoryQueryFilter queryFilter)
        {
            return ObjectFactory<InentoryAppService>.Instance.CheckOperateRightForCurrentUser(queryFilter.UserSysNo.Value, queryFilter.ProductSysNo.Value);
        }

        /// <summary>
        /// 根据商品sysNo获取每个商品的产品线和所属PM
        /// </summary>
        /// <param name="productLineSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/GetProductLineSysNoByProductList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<ProductPMLine> GetProductLineSysNoByProductList(InventoryQueryFilter queryFilter)
        {
            return ObjectFactory<InentoryAppService>.Instance.GetProductLineSysNoByProductList(queryFilter.ProductSysNos.ToArray());
        }


        /// <summary>
        /// 获取滞销库存详细信息
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryUnmarketableInventoryInfo/{productSysNo}/{companyCode}", Method = "GET")]
        public List<UnmarketabelInventoryInfo> QueryUnmarketableInventoryInfo(string productSysNo, string companyCode)
        {
            return ObjectFactory<IInventoryQueryDA>.Instance.GetUnmarketableInventoryInfo(int.Parse(productSysNo), companyCode);
        }

        /// <summary>
        /// 查询商品批次信息
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryProductBacthInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public QueryResult QueryProductBatchInfoByFilter(ProductBatchQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int totalCount = 0;
            result.Data = ObjectFactory<IProductBatchQueryDA>.Instance.QueryProductBatch(queryFilter, out totalCount);
            result.TotalCount = totalCount;

            return result;
        }

        /// <summary>
        /// JOB 库存报警
        /// </summary>
        [WebInvoke(UriTemplate = "/Inventory/Job/ProductRing", Method = "POST")]
        public void GetProductRingAndSendEmail()
        {
            ObjectFactory<ProductInventoryAppService>.Instance.GetProductRingAndSendEmail();
        }

        /// <summary>
        /// 查询商品的库存成本序列
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryProductCostInList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductCostInList(ProductCostQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            result.Data = ObjectFactory<IProductCostQueryDA>.Instance.QueryProductCostInList(queryFilter, out getTotalCount);
            result.TotalCount = getTotalCount;
            return result;
        }

        /// <summary>
        /// 查询商品入库成本明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryAvaliableCostInList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryAvaliableCostInList(CostChangeItemsQueryFilter queryFilter)
        {
            QueryResult result = new QueryResult();
            int getTotalCount = 0;
            result.Data = ObjectFactory<IProductCostQueryDA>.Instance.QueryAvaliableCostInList(queryFilter, out getTotalCount);
            result.TotalCount = getTotalCount;
            return result;
        }


        /// <summary>
        /// 批量设置库存成本序列优先级
        /// </summary>
        /// <param name="list"></param>
        [WebInvoke(UriTemplate = "/Inventory/BatchUpdateProductCostPriority", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void BatchUpdateProductCostPriority(List<ProductCostInfo> list)
        {
            ObjectFactory<ProductInventoryAppService>.Instance.UpdateProductCostPriority(list);
        }

        /// <summary>
        /// 修改批次信息状态
        /// </summary>
        /// <param name="list"></param>
        [WebInvoke(UriTemplate = "/Inventory/UpdateProductBatchStatus", Method = "PUT")]
        [DataTableSerializeOperationBehavior]
        public void UpdateProductBatchStatus(InventoryBatchDetailsInfo productBatchInfo)
        {
            ObjectFactory<ProductInventoryAppService>.Instance.UpdateProductBatchStatus(productBatchInfo);
        }


        /// <summary>
        /// 商品入库出库报表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryCostInAndCostOutReport", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductCostInAndCostOutReport(CostInAndCostOutReportQueryFilter queryFilter)
        {
            int totalCount;
            DataTable table = ObjectFactory<IInventoryQueryDA>.Instance.QueryProductCostInAndCostOutReport(queryFilter, out totalCount);

            QueryResult result = new QueryResult()
            {
                Data = table,
                TotalCount = totalCount
            };

            return result;
        }

        /// <summary>
        /// 商品库龄报表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Inventory/QueryStockAgeReport", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryProductStockAgeReport(StockAgeReportQueryFilter queryFilter)
        {
            int totalCount;
            DataTable table = ObjectFactory<IInventoryQueryDA>.Instance.QueryProductStockAgeReport(queryFilter, out totalCount);

            QueryResult result = new QueryResult()
            {
                Data = table,
                TotalCount = totalCount
            };

            return result;
        }

        /// <summary>
        /// 月底自动发送库存邮件
        /// </summary>
        /// <param name="sosysno">订单编号</param>
        [WebInvoke(UriTemplate = "/Inventory/Job/SendInventoryEmailEndOfMonth", Method = "PUT")]
        public void AutoAuditSendInventoryMessage(string address)
        {
            string downloadPath = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/EndOfMonthInventoryEmail";
            string savePath = System.Web.HttpContext.Current.Server.MapPath("~/EndOfMonthInventoryEmail");
            ObjectFactory<InentoryAppService>.Instance.SendInventoryEmailEndOfMonth(address, "zh-CN", downloadPath, savePath);
        }
    }
}
