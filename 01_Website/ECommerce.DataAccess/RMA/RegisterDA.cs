using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess;
using System.Data;
using ECommerce.Entity.RMA;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.DataAccess.RMA
{
    public class RegisterDA
    {
        public static int CreateSysNo()
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("CreateRegisterSysNo");
            insertCommand.ExecuteNonQuery();
            return (int)insertCommand.GetParameterValue("@SysNo");
        }

        public static bool Create(RMARegisterInfo register)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertRegister");

            cmd.SetParameterValue<RMARegisterInfo>(register);
            cmd.SetParameterValue("@SysNo", register.SysNo);
            cmd.SetParameterValue("@ProductSysNo", register.ProductSysNo);
            cmd.SetParameterValue("@RequestType", register.RequestType);
            cmd.SetParameterValue("@CustomerDesc", register.CustomerDesc);
            cmd.SetParameterValue("@RMAReason", register.RMAReason);
            cmd.SetParameterValue("@Status", register.Status);
            cmd.SetParameterValue("@OwnBy", register.OwnBy);
            cmd.SetParameterValue("@Location", register.Location);
            cmd.SetParameterValue("@IsWithin7Days", register.IsWithin7Days);
            cmd.SetParameterValue("@IsRecommendRefund", register.IsRecommendRefund);
            cmd.SetParameterValue("@NewProductStatus", register.NewProductStatus);
            cmd.SetParameterValue("@NextHandler", register.NextHandler);
            cmd.SetParameterValue("@SOItemType", register.SOItemType);
            cmd.SetParameterValue("@RevertStatus", register.RevertStatus);
            cmd.SetParameterValue("@ShippedWarehouse", register.ShippedWarehouse);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool InsertRequestItem(int requestSysNo, int registerSysNo)
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("InsertRequestItem");
            insertCommand.SetParameterValue("@RequestSysNo", requestSysNo);
            insertCommand.SetParameterValue("@RegisterSysNo", registerSysNo);
            return insertCommand.ExecuteNonQuery() > 0;
        }

        public static List<RMARegisterInfo> LoadRegisterByRequestSysNo(int requestSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("LoadRegisterByRequestSysNo");
            dataCommand.SetParameterValue("@RequestSysNo", requestSysNo);
            DataTable dt = dataCommand.ExecuteDataTable();
            List<RMARegisterInfo> rmaRegisterInfos = null;
            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                rmaRegisterInfos = DataMapper.GetEntityList<RMARegisterInfo, List<RMARegisterInfo>>(dt.Rows);
                return rmaRegisterInfos;
            }

            return new List<RMARegisterInfo>();
        }

        public static RMARegisterInfo LoadRegisterByRegisterSysNo(int RegisterSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("LoadRegisterByRegisterSysNo");
            dataCommand.SetParameterValue("@RegisterSysNo", RegisterSysNo);
            DataTable dt = dataCommand.ExecuteDataTable();
            List<RMARegisterInfo> rmaRegisterInfos = null;
            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                rmaRegisterInfos = DataMapper.GetEntityList<RMARegisterInfo, List<RMARegisterInfo>>(dt.Rows);
                if (rmaRegisterInfos.Count() > 0)
                    return rmaRegisterInfos[0];
                else
                {
                    return new RMARegisterInfo();
                }
            }

            return new RMARegisterInfo();
        }

        public static int GetRegisterQty(int productSysNo, int soItemType, int soSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRegisterQty");
            cmd.SetParameterValue("@SOItemType", soItemType);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            cmd.SetParameterValue("@WaitingAudit", (int)RMARequestStatus.WaitingAudit);
            cmd.SetParameterValue("@Origin", (int)RMARequestStatus.Origin);
            cmd.SetParameterValue("@Handling", (int)RMARequestStatus.Handling);
            return cmd.ExecuteScalar<int>();
        }
    }
}
