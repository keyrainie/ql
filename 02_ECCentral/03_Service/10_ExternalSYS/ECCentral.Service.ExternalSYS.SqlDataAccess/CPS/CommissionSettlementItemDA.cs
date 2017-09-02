using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS.CPS;
using ECCentral.Service.ExternalSYS.IDataAccess.CPS;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.CPS
{
    [VersionExport(typeof(ICommissionSettlementItemDA))]
    public class CommissionSettlementItemDA : ICommissionSettlementItemDA
    {
        public List<int> GetHavingOrderUserList(DateTime beginDate, DateTime endDate, FinanceStatus status)
        {
            var list = new List<int>();
            DataCommand dc = DataCommandManager.GetDataCommand("GetHavingOrderUserList");
            dc.SetParameterValue("@BeginDate", beginDate);
            dc.SetParameterValue("@EndDate", endDate);
            dc.SetParameterValue("@Status", status);
            using (IDataReader reader = dc.ExecuteDataReader())
            {
                while (reader.Read())
                {
                    list.Add(reader.GetInt32(0));
                }
            }
            return list;
        }

        public List<CommissionSettlementItemInfo> GetCommissionSettlementItemInfoListByUserSysNo(int userSysNo, DateTime beginDate, DateTime endDate, FinanceStatus status)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetCommissionSettlementItemInfoListByUserSysNo");
            dc.SetParameterValue("@UserSysNo", userSysNo);
            dc.SetParameterValue("@BeginDate", beginDate);
            dc.SetParameterValue("@EndDate", endDate);
            dc.SetParameterValue("@Status", status);
            return dc.ExecuteEntityList<CommissionSettlementItemInfo>();
        }

        public void Update(CommissionSettlementItemInfo commissionSettlementItemInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateCommissionSettlementItemInfo");
            dc.SetParameterValue("@SysNo", commissionSettlementItemInfo.SysNo);
            dc.SetParameterValue("@Status", commissionSettlementItemInfo.Status);
            dc.SetParameterValue("@CommissionAmt", commissionSettlementItemInfo.CommissionAmt);
            dc.SetParameterValue("@EditUser", commissionSettlementItemInfo.OperateUser);
            dc.SetParameterValue("@CommissionSettlementSysNo", commissionSettlementItemInfo.CommissionSettlementSysNo);
            dc.ExecuteNonQuery();
        }
    }
}