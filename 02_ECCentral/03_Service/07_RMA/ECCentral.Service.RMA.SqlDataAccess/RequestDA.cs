using System;
using System.Data;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Collections.Generic;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IRequestDA))]
    public class RequestDA : IRequestDA
    {
        #region IRMARequestDA Members

        public int CreateSysNo()
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("CreateRequestMasterSysNo");

            insertCommand.ExecuteNonQuery();

            return (int)insertCommand.GetParameterValue("@SysNo");
        }

        public RMARequestInfo Create(RMARequestInfo entity)
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("InsertRequestMaster");

            insertCommand.SetParameterValue<RMARequestInfo>(entity);

            insertCommand.ExecuteNonQuery();

            return entity;
        }

        public void Update(RMARequestInfo entity)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("PurelyUpdateRequestMaster");

            updateCommand.SetParameterValue<RMARequestInfo>(entity);

            updateCommand.ExecuteNonQuery();
        }

        public void UpdateStatus(int soSysNo, RMARequestStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRMARequestState");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@Status", status);       
            command.ExecuteNonQuery();           
        }

        public DataSet LoadForCheckCancelReceive(int requestSysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("LoadForCheckCancelReceive");
            selectCommand.SetParameterValue("@RequestSysNo", requestSysNo);
            return selectCommand.ExecuteDataSet();
        }

        public RMARequestInfo LoadBySysNo(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("LoadRequestBySysNo");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            return selectCommand.ExecuteEntity<RMARequestInfo>();
        }

        public RMARequestInfo LoadWithRegistersBySysNo(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("LoadRequestBySysNo");
            selectCommand.SetParameterValue("@SysNo", sysNo);

            RMARequestInfo requestView = selectCommand.ExecuteEntity<RMARequestInfo>();
            if (requestView == null)
            {
                return requestView;
            }
            requestView.Registers = ObjectFactory<IRegisterDA>.Instance.QueryByRequestSysNo(requestView.SysNo.Value);

            return requestView;
        }

        public RMARequestInfo LoadByRegisterSysNo(int registerSysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("LoadByRegisterSysNo");
            selectCommand.SetParameterValue("@RegisterSysNo", registerSysNo);

            return selectCommand.ExecuteEntity<RMARequestInfo>();
        }

        public List<RMARequestInfo> LoadRequestBySOSysNo(int soSysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("LoadRequestBySOSysNo");
            selectCommand.SetParameterValue("@SOSysNo", soSysNo);

            return selectCommand.ExecuteEntityList<RMARequestInfo>();
        }

        public string GetInventoryMemo(int? whNo, int? productSysNo, string companyCode)
        {
            var command = DataCommandManager.GetDataCommand("GetInventoryMemo");
            string InventoryMemo = string.Empty;
            string RMAStockQty = "RMAStockQty数量[";
            string RMAOnVendorQty = "RMAOnVendorQty数量[";
            string ShiftQty = "ShiftQty数量[";
            string OwnbyNeweggQty = "OwnbyNeweggQty数量[";
            string OwnbyCustomerQty = "OwnbyCustomerQty数量[";

            command.SetParameterValue("@WarehouseSysNo", whNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            DataSet ds = command.ExecuteDataSet();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                if (ds.Tables[0].Rows[0]["RMAStockQty"].ToString() != "")
                {
                    RMAStockQty += ds.Tables[0].Rows[0]["RMAStockQty"].ToString() + "] ";
                }
                else
                {
                    RMAStockQty += "0] ";
                }
                if (ds.Tables[0].Rows[0]["RMAOnVendorQty"].ToString() != "")
                {
                    RMAOnVendorQty += ds.Tables[0].Rows[0]["RMAOnVendorQty"].ToString() + "] ";
                }
                else
                {
                    RMAOnVendorQty += "0] ";
                }
                if (ds.Tables[0].Rows[0]["ShiftQty"].ToString() != "")
                {
                    ShiftQty += ds.Tables[0].Rows[0]["ShiftQty"].ToString() + "] ";
                }
                else
                {
                    ShiftQty += "0] ";
                }
                if (ds.Tables[0].Rows[0]["OwnbyNeweggQty"].ToString() != "")
                {
                    OwnbyNeweggQty += ds.Tables[0].Rows[0]["OwnbyNeweggQty"].ToString() + "] ";
                }
                else
                {
                    OwnbyNeweggQty += "0] ";
                }
                if (ds.Tables[0].Rows[0]["OwnbyCustomerQty"].ToString() != "")
                {
                    OwnbyCustomerQty += ds.Tables[0].Rows[0]["OwnbyCustomerQty"].ToString() + "] ";
                }
                else
                {
                    OwnbyCustomerQty += "0] ";
                }
            }
            else
            {
                RMAStockQty += "0] ";
                RMAOnVendorQty += "0] ";
                ShiftQty += "0] ";
                OwnbyNeweggQty += "0] ";
                OwnbyCustomerQty += "0] ";
            }
            InventoryMemo = RMAStockQty + RMAOnVendorQty + ShiftQty + OwnbyNeweggQty + OwnbyCustomerQty;
            return InventoryMemo;
        }

        public void InsertRMAInventoryLog(RMAInventoryLog entity)
        {
            var insertCommand = DataCommandManager.GetDataCommand("InsertRMAInventoryLog");
            insertCommand.SetParameterValue<RMAInventoryLog>(entity);

            insertCommand.ExecuteNonQuery();
        }

        public bool PrintLabels(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRequestLabelPrinted");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteNonQuery() > 0;
        }

        public bool IsRMARequestExists(int soSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("QueryRMARequestExistsBySOSysNo");
            dc.SetParameterValue("@SOSysNo", soSysNo);
            object result = dc.ExecuteScalar();
            return result != null && (int)result > 0 ? true : false;
        }

        public string CreateServiceCode()
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateServiceCode");
            return command.ExecuteScalar<string>();
        }

        #endregion       
    }
}
