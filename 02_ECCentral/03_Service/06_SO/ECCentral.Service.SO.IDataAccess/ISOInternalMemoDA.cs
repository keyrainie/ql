using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOInternalMemoDA
    {
        List<CSInfo> GetSOLogCreater(string companyCode);

        List<CSInfo> GetSOLogUpdate(string companyCode);

        void AddSOInternalMemoInfo(SOInternalMemoInfo info, string companyCode);

        void UpdateSOInternalMemoInfo(SOInternalMemoInfo info);

        void Update_AssignInfo(SOInternalMemoInfo info);

        void Update_StatusInfo(SOInternalMemoInfo info);

        SOInternalMemoInfo GetBySysNo(int sysNo);

        List<SOInternalMemoInfo> GetBySOSysNo(int soSysNo);
    }
}
