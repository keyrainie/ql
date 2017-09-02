using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface IVendorStoreQueryDA
    {
        DataTable QueryVendorStoreList(int vendorSysNo);

        DataTable QueryCommissionRuleTemplateInfo(CommissionRuleTemplateQueryFilter request, out int totalCount);

        DataTable QuerySecondDomain(SecondDomainQueryFilter request, out int totalCount);

        bool CheckSecondDomainStatus(int SysNo);

        void ChangeSecondDomainStatus(int SysNo, int SecondDomainStatus);
    }
}
