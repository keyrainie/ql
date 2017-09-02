using System.Collections.Generic;
using System.Transactions;
using ECCentral.Service.ExternalSYS.BizProcessor.CPS;
using ECCentral.Service.Utility;

namespace ECCentral.Service.ExternalSYS.AppService.CPS
{
    [VersionExport(typeof(CPSJOBAppService))]
    public class CPSJOBAppService
    {
        public List<int> GetSettledUserSysNoList()
        {
            return ObjectFactory<CommissionSettlementProcessor>.Instance.GetSettledUserSysNoList();
        }

        public void ProcessUserSettledCommissionInfo(int userSysNo)
        {
            using (var tran = new TransactionScope())
            {
                ObjectFactory<CommissionSettlementProcessor>.Instance.ProcessUserSettledCommissionInfo(userSysNo);
                tran.Complete();
            }
        }

        public List<int> GetPendingCommissionSettlement()
        {
            return ObjectFactory<CommissionSettlementProcessor>.Instance.GetPendingCommissionSettlement();
        }

        public void ProcessUserAutoSettledCommissionInfo(int userSysNo)
        {
            using (var tran = new TransactionScope())
            {
                ObjectFactory<CommissionSettlementProcessor>.Instance.ProcessUserAutoSettledCommissionInfo(userSysNo);
                tran.Complete();
            }
        }
    }
}
