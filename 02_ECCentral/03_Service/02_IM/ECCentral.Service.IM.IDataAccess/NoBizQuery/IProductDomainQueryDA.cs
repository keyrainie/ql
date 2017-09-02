using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IProductDomainQueryDA
    {
        string GetUserNameList(string strList, string companyCode);

        DataTable QueryProductDomainList(ProductDomainFilter filter, out int totalCount);
    }
}
