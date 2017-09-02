using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.ExternalSYS.CPS;
using ECCentral.Service.ExternalSYS.IDataAccess.CPS;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.CPS
{
    [VersionExport(typeof(ICommissionSettlementDA))]
    public class CommissionSettlementDA : ICommissionSettlementDA
    {
        public void Insert(CommissionSettlementInfo commissionSettlementInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertCommissionSettlementInfo");
            dc.SetParameterValue("@UserSysNo", commissionSettlementInfo.UserSysNo);
            dc.SetParameterValue("@Status", commissionSettlementInfo.Status);
            dc.SetParameterValue("@SettledBeginDate", commissionSettlementInfo.SettledBeginDate);
            dc.SetParameterValue("@SettledEndDate", commissionSettlementInfo.SettledEndDate);
            dc.SetParameterValue("@InUser", commissionSettlementInfo.OperateUser);
            dc.ExecuteNonQuery();
        }

        public List<int> GetPendingCommissionSettlementUserList()
        {
            var list = new List<int>();
            DataCommand dc = DataCommandManager.GetDataCommand("GetPendingCommissionSettlementUserList");
            using (IDataReader reader = dc.ExecuteDataReader())
            {
                while (reader.Read())
                {
                    list.Add(reader.GetInt32(0));
                }
            }
            return list;
        }

        public List<CommissionSettlementInfo> GetPendingCommissionSettlementByUserSysNo(int userSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetPendingCommissionSettlementByUserSysNo");
            dc.SetParameterValue("@UserSysNo", userSysNo);
            return dc.ExecuteEntityList<CommissionSettlementInfo>();
        }

        public List<CommissionSettlementInfo> GetUnRequestCommissionSettlementList(int userSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetUnRequestCommissionSettlementList");
            dc.SetParameterValue("@UserSysNo", userSysNo);
            return dc.ExecuteEntityList<CommissionSettlementInfo>();
        }

        public void Update(CommissionSettlementInfo commissionSettlementInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateCommissionSettlementInfo");
            dc.SetParameterValue("@SysNo", commissionSettlementInfo.SysNo);
            dc.SetParameterValue("@CommissionAmt", commissionSettlementInfo.CommissionAmt);
            dc.SetParameterValue("@ConfirmCommissionAmt", commissionSettlementInfo.CommissionAmt);
            dc.ExecuteNonQuery();
        }

        public void UpdateCashRecord(int commissionSettlementSysNo, int cashRecordSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateCommissionSettlementInfoCashRecord");
            dc.SetParameterValue("@CommissionToCashRecordSysNo", cashRecordSysNo);
            dc.ReplaceParameterValue("@SysNo", commissionSettlementSysNo);
            dc.ExecuteNonQuery();
        }
    }
}
