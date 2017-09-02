using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
   public interface IRmaPolicyLogDA
    {
       void CreateRMAPolicyLog(RmaPolicyInfo info, RmaLogActionType actionType);
    }
}
