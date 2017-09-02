using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IUnicomContractPhoneDA
    {
        void UpdateUnicomContractPhoneNumberStatus(string phone, UnicomContractPhoneNumberStatus status, UserInfo operationUser);
    }
}
