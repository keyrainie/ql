using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface ICurrencyDA
    {
        List<CurrencyInfo> QueryCurrencyList();

        CurrencyInfo Create(CurrencyInfo currencyInfo);

        CurrencyInfo Update(CurrencyInfo currencyInfo);

        CurrencyInfo Load(int sysNo);
    }
}
