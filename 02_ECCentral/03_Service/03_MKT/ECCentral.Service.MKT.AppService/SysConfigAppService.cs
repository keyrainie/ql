using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(SysConfigAppService))]
    public class SysConfigAppService
    {
        public void Update(string key, string value, string channelID)
        {
            ObjectFactory<SysConfigProcessor>.Instance.Update(key, value, channelID);
        }
    }
}
