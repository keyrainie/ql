using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ICountdownQueryDA
    {
        DataTable Query(CountdownQueryFilter filter, out int totalCount);

        string GetPMByProductSysNo(string sysNo);

        void GetGiftAndCouponSysNo(int productSysNo, out int giftSysNo, out int couponSysNo);
    }
}
