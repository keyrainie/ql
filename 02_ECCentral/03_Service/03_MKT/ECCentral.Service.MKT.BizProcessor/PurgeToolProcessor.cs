using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.BizProcessor
{
      [VersionExport(typeof(PurgeToolProcessor))]
   public class PurgeToolProcessor
    {
          private IPurgeToolDA _purgeToolDA = ObjectFactory<IPurgeToolDA>.Instance;

          /// <summary>
          /// 创建
          /// </summary>
          /// <param name="info"></param>
          public void CreatePurgeTool(List<PurgeToolInfo> list)
          {
              foreach (var item in list)
              {
                  _purgeToolDA.CreatePurgeTool(item);
              }
             
          }
    }
}
