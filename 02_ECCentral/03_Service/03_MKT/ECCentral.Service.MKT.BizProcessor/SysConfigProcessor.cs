using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(SysConfigProcessor))]
    public class SysConfigProcessor
    {
        public void Update(string key, string value, string channelID)
        {
            ObjectFactory<ISysConfigDA>.Instance.Update(key, value,channelID);
        }
    }
}
