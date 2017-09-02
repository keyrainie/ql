using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOInterceptDA
    {
        List<SOInterceptInfo> GetSOInterceptInfoListBySOSysNo(int soSysNo);

        void AddSOInterceptInfo(SOInterceptInfo info, string companyCode);

        void BatchUpdateSOInterceptInfo(SOInterceptInfo info);

        void DeleteSOInterceptInfo(SOInterceptInfo info);
    }
}
