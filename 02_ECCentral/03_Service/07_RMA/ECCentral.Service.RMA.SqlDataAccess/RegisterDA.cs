using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IRegisterDA))]
    public class RegisterDA : IRegisterDA
    {
        #region IRMARegisterDA Members

        public RMARegisterInfo LoadBySysNo(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("LoadRegisterBySysNo");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return selectCommand.ExecuteEntity<RMARegisterInfo>();
        }

        public RMARegisterInfo LoadForEditBySysNo(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("LoadRegisterForEditBySysNo");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            RMARegisterInfo register = selectCommand.ExecuteEntity<RMARegisterInfo>();

            return register;
        }

        public List<RMARegisterInfo> LoadBySOSysNo(int soSysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("LoadRegisterBySoNumber");
            selectCommand.SetParameterValue("@SOSysNo", soSysNo);

            return selectCommand.ExecuteEntityList<RMARegisterInfo>();
        }

        public List<RMARegisterInfo> QueryByRequestSysNo(int requestSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadRegisterByRequestSysNo");
            cmd.SetParameterValue("@RequestSysNo", requestSysNo);

            List<RMARegisterInfo> list = cmd.ExecuteEntityList<RMARegisterInfo>();
            var reasons = CodeNamePairManager.GetList("RMA", "RMAReason");
            list.ForEach(p =>
            {
                if (p.BasicInfo.SOItemType == SOProductType.ExtendWarranty)
                {
                    //entity.BasicInfo.ProductInfo.ProductCommonInfo.ProductBasicInfo.ProductName = GetExWarrantyProductName(requestSysNo, entity.BasicInfo.ProductSysNo.Value.ToString());
                    p.BasicInfo.ProductName = GetExWarrantyProductName(requestSysNo, p.BasicInfo.ProductSysNo.Value.ToString());
                }
                var pair = reasons.FirstOrDefault(k => k.Code == p.BasicInfo.RMAReason.ToString());
                if (pair != null && !string.IsNullOrEmpty(pair.Code))
                {
                    p.BasicInfo.RMAReasonDesc = pair.Name;
                }
            });

            return list;
        }

        public List<RMARegisterInfo> GetRegistersBySysNoList(List<int> sysNoList)
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

            DataCommand command = DataCommandManager.GetDataCommand("GetRegistersBySysNoList");

            command.ReplaceParameterValue("#SysNoList", condition);

            return command.ExecuteEntityList<RMARegisterInfo>();
        }

        public int CreateSysNo()
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("CreateRegisterSysNo");
            insertCommand.ExecuteNonQuery();
            return (int)insertCommand.GetParameterValue("@SysNo");
        }

        public bool InsertRequestItem(int requestSysNo, int registerSysNo)
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("InsertRequestItem");
            insertCommand.SetParameterValue("@RequestSysNo", requestSysNo);
            insertCommand.SetParameterValue("@RegisterSysNo", registerSysNo);
            return insertCommand.ExecuteNonQuery() > 0;
        }

        public bool Create(RMARegisterInfo register)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertRegister");

            cmd.SetParameterValue<RMARegisterInfo>(register);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateForRMAAuto(RMARegisterInfo register)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateForRMAAuto");

            updateCommand.SetParameterValue<RegisterBasicInfo>(register.BasicInfo);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool UpdateBasicInfo(RMARegisterInfo register)
        {
            //ProductNo,NextHandlerCode,IsWithin7Days,IsHaveInvoice,IsFullPackage,IsFullAccessory
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateRegisterForRequest");

            updateCommand.SetParameterValue<RegisterBasicInfo>(register.BasicInfo);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool UpdateCheckInfo(RMARegisterInfo register)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateRegisterCheckInfo");

            updateCommand.SetParameterValue<RegisterCheckInfo>(register.CheckInfo);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool UpdateResponseInfo(RMARegisterInfo register)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateRegisterResponseInfo");

            updateCommand.SetParameterValue<RegisterResponseInfo>(register.ResponseInfo);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool UpdateRefundStatus(int sysNo, RMARefundStatus? refundStatus)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateRefundStatus");
            updateCommand.SetParameterValue("@SysNo", sysNo);
            updateCommand.SetParameterValue("@RefundStatus", refundStatus);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool UpdateReturnStatus(int sysNo, RMAReturnStatus? returnStatus)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateReturnStatus");
            updateCommand.SetParameterValue("@SysNo", sysNo);
            updateCommand.SetParameterValue("@ReturnStatus", returnStatus);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool UpdateOutboundStatus(int sysNo, RMAOutBoundStatus? outboundStatus)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateOutboundStatus");
            updateCommand.SetParameterValue("@SysNo", sysNo);
            updateCommand.SetParameterValue("@OutboundStatus", outboundStatus);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool UpdateRevertStatus(RMARegisterInfo register)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateRevertStatus");

            updateCommand.SetParameterValue<RegisterRevertInfo>(register.RevertInfo);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public void PurelyUpdate(RMARegisterInfo register)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("PurelyUpdateRegister");

            updateCommand.SetParameterValue<RMARegisterInfo>(register);

            updateCommand.ExecuteNonQuery();
        }

        public bool Close(RMARegisterInfo register)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("CloseRegister");

            updateCommand.SetParameterValue<RegisterBasicInfo>(register.BasicInfo);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool CloseForVendorRefund(RMARegisterInfo register)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("CloseRegisterForVendorRefund");

            updateCommand.SetParameterValue<RegisterBasicInfo>(register.BasicInfo);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool ReOpen(RMARegisterInfo register)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("ReOpenRegister");

            updateCommand.SetParameterValue<RegisterBasicInfo>(register.BasicInfo);

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool SetAbandon(int registerSysNo)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("RegisterSetAbandon");
            updateCommand.SetParameterValue("@RegisterSysNo", registerSysNo);
            updateCommand.ExecuteNonQuery();

            string strResult = updateCommand.GetParameterValue("@UpdateRegisterCount").ToString();
            if (int.Parse(strResult) == 1)
            {
                return true;
            }
            return false;
        }

        private string GetExWarrantyProductName(int requestSysNo, string masterProductSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetExWarrantyProductName");
            cmd.SetParameterValue("@RequestSysNo", requestSysNo);
            cmd.SetParameterValue("@MasterProductSysNo", masterProductSysNo);

            var result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : string.Empty;
        }

        public void UpdateInventory(int wareshouseSysNo, int productSysNo, bool isRecv, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateInventory");
            cmd.SetParameterValue("@WHSysNo", wareshouseSysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@IsRecv", isRecv ? 1 : 0);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            cmd.ExecuteNonQuery();
        }

        public int? GetRegisterQty(int productSysNo, int soItemType, int soSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRegisterQty");
            cmd.SetParameterValue("@SOItemType", soItemType);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            cmd.SetParameterValue("@Origin", (int)RMARequestStatus.Origin);
            cmd.SetParameterValue("@Handling", (int)RMARequestStatus.Handling);
            return cmd.ExecuteScalar<int>();
        }

        public bool CanWaitingRevert(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanWaitingRevert");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return selectCommand.ExecuteScalar<int>() > 0;
        }

        public bool CanWaitingRefund(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanWaitingRefund");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return selectCommand.ExecuteScalar<int>() > 0;
        }

        public bool CanCancelWaitingRefund(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanCancelWaitingRefund");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return selectCommand.ExecuteScalar<int>() > 0;
        }

        public bool CanWaitingReturn(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanWaitingReturn");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return selectCommand.ExecuteScalar<int>() > 0;
        }

        public bool CanCancelWaitingReturn(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanCancelWaitingReturn");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return selectCommand.ExecuteScalar<int>() > 0;
        }

        public bool CanReOpen(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanReOpenRegister");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return selectCommand.ExecuteScalar<int>() > 0;
        }

        public bool CanCancelWaitingRevert(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanCancelWaitingRevert");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return (int)selectCommand.ExecuteScalar() > 0;
        }

        public bool CanWaitingOutbound(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanWaitingOutbound");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return (int)selectCommand.ExecuteScalar() > 0;
        }

        public bool CanCancelWaitingOutbound(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("CanCancelWaitingOutbound");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return (int)selectCommand.ExecuteScalar() > 0;
        }

        public void UpdateMemo(RegisterBasicInfo registerEntity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRegisterMemo");
            command.SetParameterValue("@SysNo", registerEntity.SysNo);
            command.SetParameterValue("@Memo", registerEntity.Memo);
            command.ExecuteNonQuery();

        }

        public void UpdateRegisterAfterRefund(RMARegisterInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRegisterAfterRefunded");

            command.SetParameterValue<RMARegisterInfo>(entity);

            command.ExecuteNonQuery();
        }

        public RMAInventory GetRMAInventoryBy(int wareshouseSysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRMAInventory");
            cmd.SetParameterValue("@WHSysNo", wareshouseSysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            return cmd.ExecuteEntity<RMAInventory>();
        }

        public void UpdateInventory(RMAInventory inventory)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateInventoryCost");

            cmd.SetParameterValue<RMAInventory>(inventory);

            cmd.ExecuteNonQuery();
        }

        #endregion

        #region For PO Domain


        public string[] GetReceiveWarehouseByRegisterSysNo(int registerSysNo)
        {
            string[] str = new string[2];
            DataCommand command = DataCommandManager.GetDataCommand("GetRegisterRow");
            command.SetParameterValue("@RegisterSysNo", registerSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            if (null != dt && 0 < dt.Rows.Count)
            {
                //仓库编号:
                str[0] = dt.Rows[0]["ReceiveWarehouse"].ToString();
                //产品编号:
                str[1] = dt.Rows[0]["ProductSysNo"].ToString();
            }
            return str;
        }

        #endregion


        public virtual DataRow LoadRegisterMemoBySysNo(int registerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadRegisterMemoBySysNo");
            command.SetParameterValue("@SysNo", registerSysNo);
            return command.ExecuteDataRow();
        }

        public bool SyncERP(int sysNo)
        {
            DataCommand Command = DataCommandManager.GetDataCommand("RegisterSyncERP");
            Command.SetParameterValue("@SysNo", sysNo);
           return Command.ExecuteNonQuery()>0;           
        }
    }
}
