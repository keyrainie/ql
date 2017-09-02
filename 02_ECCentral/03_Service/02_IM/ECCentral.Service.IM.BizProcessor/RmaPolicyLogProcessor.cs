using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
     [VersionExport(typeof(RmaPolicyLogProcessor))]
  public  class RmaPolicyLogProcessor
    {
     /// <summary>
     /// 创建退换货日志
     /// </summary>
     /// <param name="info"></param>
     /// <param name="actionType"></param>
      public void CreateRMAPolicyLog(RmaPolicyInfo info, RmaLogActionType actionType)
      {
          ObjectFactory<IRmaPolicyLogDA>.Instance.CreateRMAPolicyLog(info, actionType);
      }
    }
}
