using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS.CPS;
using ECCentral.Service.ExternalSYS.IDataAccess.CPS;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.CPS
{
    [VersionExport(typeof(ICommissionPercentageDA))]
    public class CommissionPercentageDA : ICommissionPercentageDA
    {
        public List<CommissionPercentage> GetByC1SysNo(int c1SysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetCommissionPercentageListByC1SysNo");
            dc.SetParameterValue("@Category1SysNo", c1SysNo);
            return dc.ExecuteEntityList<CommissionPercentage>();
        }

        public List<CommissionPercentage> GetByDefault()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetCommissionPercentageListByDefault");
            return dc.ExecuteEntityList<CommissionPercentage>();
        }
    }
}
