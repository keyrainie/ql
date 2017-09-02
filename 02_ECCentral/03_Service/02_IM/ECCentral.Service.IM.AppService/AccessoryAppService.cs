using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(AccessoryAppService))]
    public class AccessoryAppService
    {
        private readonly AccessoryProcessor _accessoryProcessor = ObjectFactory<AccessoryProcessor>.Instance;

        public AccessoryInfo CreateAccessory(AccessoryInfo accessoryInfo)
        {
            return _accessoryProcessor.CreateAccessory(accessoryInfo);
        }

        public AccessoryInfo UpdateAccessory(AccessoryInfo accessoryInfo)
        {
            return _accessoryProcessor.UpdateAccessory(accessoryInfo);
        }

        public AccessoryInfo GetAccessory(int sysNo)
        {
            return _accessoryProcessor.GetAccessoryInfo(sysNo);
        }

        public IList<AccessoryInfo> GetAllAccessory()
        {
            return _accessoryProcessor.GetAllAccessory();
        }
    }
}
