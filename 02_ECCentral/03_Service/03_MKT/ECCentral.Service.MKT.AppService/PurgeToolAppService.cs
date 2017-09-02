using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(PurgeToolAppService))]
 public class PurgeToolAppService
    {
        private PurgeToolProcessor _processor = ObjectFactory<PurgeToolProcessor>.Instance;

        public void CreatePurgeTool(List<PurgeToolInfo> list)
        {
            _processor.CreatePurgeTool(list);
        }
    }
}
