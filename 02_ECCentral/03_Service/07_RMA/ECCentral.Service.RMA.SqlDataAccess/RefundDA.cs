using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice;
using System.Data;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IRefundDA))]
    public class RefundDA : IRefundDA
    {
        #region IRefundDA Members

        public RefundInfo InsertMaster(RefundInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertRefundMaster");

            command.SetParameterValue<RefundInfo>(entity);

            command.ExecuteNonQuery();

            return entity;
        }

        public RefundItemInfo InsertItem(RefundItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertRefundItem");

            command.SetParameterValue<RefundItemInfo>(entity);

            object result = command.ExecuteScalar();

            entity.SysNo = Convert.ToInt32(result);

            return entity;
        }

        public List<int> GetWaitingSOForRefund()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetWaitingSOForRefund");
            command.SetParameterValue("@RegisterRefundStatus", RMARefundStatus.WaitingRefund);
            command.SetParameterValue("@RegisterStatus", RMARequestStatus.Abandon);
            command.SetParameterValue("@RefundStatus", RMARefundStatus.Abandon);

            var dataTable = command.ExecuteDataTable();
            List<int> result = new List<int>();
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(int.Parse(row[0].ToString()));
                }
            }
            return result;
        }

        public RefundInfo GetMasterBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundMaster");

            command.SetParameterValue("@SysNo", sysNo);

            RefundInfo entity = command.ExecuteEntity<RefundInfo>();

            return entity;
        }

        public void UpdateMaster(RefundInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRefundMaster");

            command.SetParameterValue<RefundInfo>(entity);

            command.ExecuteNonQuery();
        }

        public void UpdateRefundPayTypeAndReason(int sysNo, int refundPayType, int refundReason)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRefundPayTypeAndReason");

            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@RefundPayType", refundPayType);
            command.SetParameterValue("@RefundReason", refundReason);

            command.ExecuteNonQuery();
        }

        public List<RefundItemInfo> GetItemsByRefundSysNo(int refundSysNo,string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundItems");

            command.SetParameterValue("@RefundSysNo", refundSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<RefundItemInfo>();
        }

        public List<RefundItemInfo> GetItemsWithProductInfoByRefundSysNo(int refundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetItemsWithProductInfoByRefundSysNo");

            command.SetParameterValue("@RefundSysNo", refundSysNo);

            return command.ExecuteEntityList<RefundItemInfo>();
        }

        public void UpdateMasterForCalc(RefundInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRefundMasterForCalc");

            command.SetParameterValue<RefundInfo>(entity);

            command.ExecuteNonQuery();
        }

        public void UpdateItemForCalc(RefundItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRefundItemForCalc");

            command.SetParameterValue<RefundItemInfo>(entity);

            command.ExecuteNonQuery();
        }

        public void BatchUpdateRegisterRefundStatus(int refundSysNo, RMARefundStatus? refundStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("BatchUpdateRegisterRefundStatus");

            command.SetParameterValue("@RefundSysNo", refundSysNo);
            command.SetParameterValue("@RefundStatus", refundStatus);

            command.ExecuteNonQuery();
        }

        public List<RefundAmountForCheck> GetSOAmountForRefund(int refundSysNo,string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOAmountForRefund");

            command.SetParameterValue("@RefundSysNo", refundSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<RefundAmountForCheck>();
        }

        public List<RefundAmountForCheck> GetROAmountForRefund(int refundSysNo,string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetROAmountForRefund");

            command.SetParameterValue("@RefundSysNo", refundSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<RefundAmountForCheck>();
        }

        public List<RegisterForRefund> GetRegistersForRefund(int refundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRegistersForRefund");

            command.SetParameterValue("@RefundSysNo", refundSysNo);

            return command.ExecuteEntityList<RegisterForRefund>();
        }

        public RefundItemInfo GetRefundItemCost(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundItemCost");

            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<RefundItemInfo>();
        }

        public void UpdateItemForRefund(RefundItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRefundItemForRefund");

            command.SetParameterValue<RefundItemInfo>(entity);

            command.ExecuteNonQuery();
        }

        public List<RefundInfo> GetMasterByOrderSysNo(int orderSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundMasterByOrderSysNo");

            command.SetParameterValue("@OrderSysNo", orderSysNo);

            return command.ExecuteEntityList<RefundInfo>();
        }

        public List<CodeNamePair> GetRefundReasons()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundReasons");

            return command.ExecuteEntityList<CodeNamePair>();
        }

        #region 历史退款统计
        public HistoryRefundAmount GetRefundCashAmtBySOSysNo(int soSysNo, RMARefundStatus Status)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetRefundCashAmtBySOSysNo");
            dc.SetParameterValue("@SOSysNo", soSysNo);
            dc.SetParameterValue("@Status", Status);
            HistoryRefundAmount result = dc.ExecuteEntity<HistoryRefundAmount>();
            return result;
        }

        public HistoryRefundAmount GetRO_BalanceCashAmtByOrgSOSysNo(int soSysNo, RMARefundStatus Status)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetRO_BalanceCashAmtByOrgSOSysNo");
            dc.SetParameterValue("@OrgSOSysNo", soSysNo);
            dc.SetParameterValue("@Status", Status);

            HistoryRefundAmount result = dc.ExecuteEntity<HistoryRefundAmount>();
            return result;
        }

        public HistoryRefundAmount GetRO_BalanceShipPriceAmtByOrgSOSysNo(int soSysNo, RefundBalanceStatus Status, int productSysNo, string stockID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetRO_BalanceShipPriceAmtByOrgSOSysNoAndWarehouseNumber");
            dc.SetParameterValue("@OrgSOSysNo", soSysNo);
            dc.SetParameterValue("@Status", Status);
            dc.SetParameterValue("@ProductSysNo", productSysNo);//"运费补偿及其他"
            dc.SetParameterValue("@InvoiceLocation", stockID);

            HistoryRefundAmount result = dc.ExecuteEntity<HistoryRefundAmount>();
            return result;
        }

        public HistoryRefundAmount GetRO_BalanceShipPriceAmtByOrgSOSysNoAndOtherStockID(int soSysNo, RefundBalanceStatus Status, int productSysNo, string stockID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetRO_BalanceShipPriceAmtByOrgSOSysNoAndOtherWarehouseNumber");
            dc.SetParameterValue("@OrgSOSysNo", soSysNo);
            dc.SetParameterValue("@Status", Status);
            dc.SetParameterValue("@ProductSysNo", productSysNo);//"运费补偿及其他"
            dc.SetParameterValue("@InvoiceLocation", stockID);

            HistoryRefundAmount result = dc.ExecuteEntity<HistoryRefundAmount>();
            return result;
        }

        public HistoryRefundAmount GetRefundShipPriceBySOSysNoAndStockID(int soSysNo, RMARefundStatus Status, string stockID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetRefundShipPriceBySOSysNoAndWareHouseNumber");
            dc.SetParameterValue("@SOSysNo", soSysNo);
            dc.SetParameterValue("@Status", (int)Status);
            dc.SetParameterValue("@InvoiceLocation", stockID);

            object tmp = dc.ExecuteScalar();

            return new HistoryRefundAmount() { TotalShipPriceAmt = tmp == null ? 0m : tmp.ToDecimal() };
        }

        public HistoryRefundAmount GetRefundShipPriceBySOSysNoAndOtherStockID(int soSysNo, RMARefundStatus Status, string stockID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetRefundShipPriceBySOSysNoAndOtherWareHouseNumber");
            dc.SetParameterValue("@SOSysNo", soSysNo);
            dc.SetParameterValue("@Status", Status);
            dc.SetParameterValue("@InvoiceLocation", stockID);

            HistoryRefundAmount result = dc.ExecuteEntity<HistoryRefundAmount>();
            return result;
        }
        #endregion

        public List<RMARegisterInfo> GetRegistersForCreate(List<int> sysNoList)
        {
            string condition = "";
            sysNoList.ForEach(item => condition += ", " + item);
            if (condition.Length > 0)
            {
                condition = condition.TrimStart(',', ' ');
            }
            else
            {
                condition = "0";
            }

            DataCommand command = DataCommandManager.GetDataCommand("GetRegistersForCreate");

            command.ReplaceParameterValue("#SysNoList", condition);

            return command.ExecuteEntityList<RMARegisterInfo>();
        }

        public List<RefundItemInfo> GetItemsByRegisterSysNoList(List<int> sysNoList)
        {
            List<RefundItemInfo> result = null;

            string condition = "";

            sysNoList.ForEach(item => condition += ", " + item);
            if (condition.Length > 0)
            {
                condition = condition.TrimStart(',', ' ');
            }
            else
            {
                condition = "0";
            }

            DataCommand command = DataCommandManager.GetDataCommand("GetRefundItemsByRegisterSysNo");

            command.ReplaceParameterValue("#SysNoList", condition);

            result = command.ExecuteEntityList<RefundItemInfo>();

            return result;
        }

        public int GetRefundCountBySOSysNoAndProductSysNo(int soSysNo, int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundBySOSysNoAndProductSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);

            int result = (int)command.ExecuteScalar();

            return result;
        }

        public int CreateSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateRefundSysNumber");

            object result = command.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        public string GetWarehouseNo(int refundSysNo)
        {
            string result = null;

            DataCommand command = DataCommandManager.GetDataCommand("GetWarehouseNumberForRefund");

            command.SetParameterValue("@RefundSysNo", refundSysNo);

            result = (String)command.ExecuteScalar();

            return result ?? String.Empty;
        }

        public int GetAutoRMARefundCountBySOSysNo(int soSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetAutoRMARefundBySoSysNo");
            dc.SetParameterValue("@SOSysNo", soSysNo);
            object o = dc.ExecuteScalar();
            return Convert.ToInt32(o);
        }

        public int GetRefundSysNoByRegisterSysNo(int registerSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetRefundSysNoByRegisterSysNo");
            dc.SetParameterValue("@RegisterSysNo", registerSysNo);
            object o = dc.ExecuteScalar();
            return Convert.ToInt32(o);
        }
        #endregion

        public string GetMSMQAddressByStockID(string stockID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetMSMQAddressByStockID");
            dc.SetParameterValue("@WarehouseNumber", stockID);
            object o = dc.ExecuteScalar();
            return o == null ? string.Empty : o.ToString();
        }
    }
}
