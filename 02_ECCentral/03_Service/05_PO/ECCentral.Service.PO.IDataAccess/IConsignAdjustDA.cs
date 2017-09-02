using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess
{
    public interface IConsignAdjustDA
    {

        DataTable Query(ConsignAdjustQueryFilter queryFilter, out int totalCount);

        ConsignAdjustInfo LoadInfo(int SysNo);

        ConsignAdjustInfo Create(ConsignAdjustInfo request);

        ConsignAdjustInfo Update(ConsignAdjustInfo request);      

        ConsignAdjustInfo UpdateStatus(ConsignAdjustInfo info);
        
        bool CreateConsignAdjustItem(ConsignAdjustItemInfo item);

        ConsignAdjustInfo Delete(int sysNo);
    }
}
