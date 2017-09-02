using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.QueryFilter.MKT.Promotion;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ICouponsQueryDA
    {
        DataSet QueryCouponCode(CouponCodeQueryFilter filter, out int totalCount);

        DataTable QueryCoupons(CouponsQueryFilter filter, out int totalCount);

        DataTable QueryCouponCodeRedeemLog(CouponCodeRedeemLogFilter filter, out int totalCount);

        DataTable QueryCouponCodeCustomerLog(CouponCodeCustomerLogFilter filter, out int totalCount);
    }
     
}
