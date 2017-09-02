using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS.CPS;

namespace ECCentral.Service.ExternalSYS.IDataAccess.CPS
{
    public interface ICommissionPercentageDA
    {
        List<CommissionPercentage> GetByC1SysNo(int c1SysNo);

        List<CommissionPercentage> GetByDefault();
    }
}
