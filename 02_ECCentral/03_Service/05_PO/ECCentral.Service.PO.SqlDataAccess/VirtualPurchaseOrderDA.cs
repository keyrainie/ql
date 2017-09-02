using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;
using System.Data;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IVirtualPurchaseOrderDA))]
    public class VirtualPurchaseOrderDA : IVirtualPurchaseOrderDA
    {
        #region IVirtualPurchaseOrderDA Members

        public BizEntity.PO.VirtualStockPurchaseOrderInfo LoadVSPO(int vspoSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOVirtualItemRequest");
            command.SetParameterValue("@SysNo", vspoSysNo);
            return command.ExecuteEntity<VirtualStockPurchaseOrderInfo>();
        }

        public bool ValidateFromPO(int vspoSysNo, int poSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ValidatePO");
            command.SetParameterValue("@SysNo", vspoSysNo);
            command.SetParameterValue("@POSysNo", poSysNo);
            return command.ExecuteScalar() != null;
        }

        public bool ValidateFromShift(int vspoSysNo, int shiftsysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ValidateShift");
            command.SetParameterValue("@SysNo", vspoSysNo);
            command.SetParameterValue("@shiftsysno", shiftsysno);
            return command.ExecuteScalar() != null;
        }

        public bool ValidateFromTransfer(int vspoSysNo, int transferSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ValidateTransfer");
            command.SetParameterValue("@SysNo", vspoSysNo);
            command.SetParameterValue("@TransferSysNo", transferSysNo);
            return command.ExecuteScalar() != null;
        }

        public VirtualStockPurchaseOrderInfo UpdateVSPO(VirtualStockPurchaseOrderInfo vspoInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOVirtualItemRequest");
            command.SetParameterValue("@SysNo", vspoInfo.SysNo);
            command.SetParameterValue("@InStockOrderType", vspoInfo.InStockOrderType);
            command.SetParameterValue("@InStockOrderSysNo", vspoInfo.InStockOrderSysNo);
            command.SetParameterValue("@EstimateArriveTime", vspoInfo.EstimateArriveTime);
            command.SetParameterValue("@PMMemo", vspoInfo.PMMemo);
            command.SetParameterValue("@CSMemo", vspoInfo.CSMemo);
            command.SetParameterValueAsCurrentUserSysNo("@PMHandleUserSysNo");
            command.SetParameterValue("@PMHandleTime", DateTime.Now);
            command.SetParameterValue("@Status", (int)vspoInfo.Status);
            if (command.ExecuteNonQuery() <= 0)
            {
                return null;
            }
            return vspoInfo;
        }

        public int CalcVSPOQuantity(int soItemSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("CalculatePurchaseQty");
            dataCommand.SetParameterValue("@SOItemSysNo", soItemSysNo);
            DataTable dt = dataCommand.ExecuteDataSet().Tables[0];
            int purchaseQty = 0, qty = 0;
            if (dt.Rows.Count > 0)
            {
                if ((dt.Rows[0]["InventoryQty"] == null) || (dt.Rows[0]["InventoryQty"].ToString() == "-999999"))
                    qty = 0;
                else
                    qty = Convert.ToInt32(dt.Rows[0]["InventoryQty"]);
                int unPayOrderQty = CalUnPayOrderQty(Convert.ToInt32(dt.Rows[0]["UnProductSysNo"]), dt.Rows[0]["UnStockSysNo"].ToString());
                qty = qty + unPayOrderQty;
                int siqty = Convert.ToInt32(dt.Rows[0]["Quantity"]);
                if (qty < 0)//虚库
                {
                    if ((System.Math.Abs(qty) - siqty) >= 0)
                        purchaseQty = siqty;
                    else
                        purchaseQty = System.Math.Abs(qty);

                }
            }
            return purchaseQty;
        }

        public VirtualStockPurchaseOrderInfo CreateVSPO(VirtualStockPurchaseOrderInfo vspoInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertSOVirtualItemRequest");
            command.SetParameterValue("@SOSysNo", vspoInfo.SOSysNo);
            command.SetParameterValue("@ProductSysNo", vspoInfo.ProductSysNo);
            command.SetParameterValue("@ProductID", vspoInfo.ProductID);
            command.SetParameterValue("@BriefName", vspoInfo.ProductName);
            command.SetParameterValue("@PMHandleTime", DBNull.Value);
            command.SetParameterValue("@PMHandleUserSysNo", DBNull.Value);
            command.SetParameterValue("@PurchaseQty", vspoInfo.PurchaseQty);
            command.SetParameterValue("@PMUserSysNo", vspoInfo.PMUserSysNo);
            command.SetParameterValue("@EstimateArriveTime", vspoInfo.EstimateArriveTime);
            command.SetParameterValue("@InStockOrderType", (int)vspoInfo.InStockOrderType);
            command.SetParameterValue("@InStockOrderSysNo", vspoInfo.InStockOrderSysNo);
            command.SetParameterValue("@Status", (int)vspoInfo.Status);
            command.SetParameterValue("@CreateTime", vspoInfo.CreateTime);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@PMMemo", vspoInfo.PMMemo);
            command.SetParameterValue("@CSMemo", vspoInfo.CSMemo);
            command.SetParameterValue("@CompanyCode", vspoInfo.CompanyCode);

            object o = command.ExecuteScalar();
            if (o != null)
            {
                vspoInfo.SysNo = Convert.ToInt32(o);
            }
            else
            {
                return null;
            }
            return vspoInfo;
        }

        public VirtualStockPurchaseOrderInfo AbandonVSPO(VirtualStockPurchaseOrderInfo vspoInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AbandonSOVirtualItemRequest");
            command.SetParameterValue("@SysNo", vspoInfo.SysNo);
            command.SetParameterValue("@PMMemo", vspoInfo.PMMemo);
            command.SetParameterValueAsCurrentUserSysNo("@PMHandleUserSysNo");
            command.SetParameterValue("@PMHandleTime", DateTime.Now);
            command.SetParameterValue("@Status", (int)vspoInfo.Status);
            command.ExecuteNonQuery();
            return vspoInfo;
        }

        public int CheckExistsSOVirtualItemRequest(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ExistSOVirtualItemRequest");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@Status", (int)VirtualPurchaseOrderStatus.Abandon);
            object number = command.ExecuteScalar();
            if (number != null)
            {
                return 1;
            }
            return 0;
        }
        #endregion

        /// <summary>
        /// 虚库采购数量逻辑改进,排除未支付的OrderQty
        /// </summary>
        /// <param name="ProductSysNo"></param>
        /// <param name="whNo"></param>
        /// <returns></returns>
        private int CalUnPayOrderQty(int productSysNo, string warehouseNumber)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CalUnPayOrderQty");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@WarehouseNumber", warehouseNumber);
            object number = command.ExecuteScalar();
            if (number != null)
            {
                return (int)number;
            }
            return 0;
        }

        public DataTable GetEmailContentForUpdateVSPO(int sysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetMailContent");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, null, " sv.SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sv.SysNo",
                     DbType.Int32, "@svSysNo", QueryConditionOperatorType.Equal, sysNo);
                command.CommandText = sqlBuilder.BuildQuerySql();
                return command.ExecuteDataSet().Tables[0];
            }
        }

        public DataTable GetEmailContentForCreateVSPO(int soSysNo, int productSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetMailContent");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, null, " sv.SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sv.SOSysNo",
                     DbType.Int32, "@SOSysNo", QueryConditionOperatorType.Equal, soSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sv.ProductSysNo",
                     DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, productSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sv.status",
                     DbType.Int32, "@status", QueryConditionOperatorType.NotEqual, (int)VirtualPurchaseOrderStatus.Abandon); //-1表示Abandon
                command.CommandText = sqlBuilder.BuildQuerySql();
                return command.ExecuteDataSet().Tables[0];
            }
        }

        public string GetBackUpPMEmailAddress(int userSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetBackUpPMEmail");
            command.SetParameterValue("@UserSysNo", userSysNo);
            return command.ExecuteScalar() as string;
        }


        #region IVirtualPurchaseOrderDA Members


        public VirtualStockPurchaseOrderInfo GetMemoInfoFromVirtualRequest(int itemSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetMemo");
            dataCommand.SetParameterValue("@SysNo", itemSysNo);
            VirtualStockPurchaseOrderInfo info = dataCommand.ExecuteEntity<VirtualStockPurchaseOrderInfo>();
            return info;
        }

        #endregion
    }
}
