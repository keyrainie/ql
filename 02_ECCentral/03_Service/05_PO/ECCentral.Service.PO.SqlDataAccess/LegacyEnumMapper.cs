using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess
{
    public static class LegacyEnumMapper
    {
        public static int? ConvertPaySettleCompany(PaySettleCompany? p)
        {
            int? result = new Nullable<int>();
            if (p.HasValue)
            {
                result = (int)p;
            }
            return result;
        }
    }
}
