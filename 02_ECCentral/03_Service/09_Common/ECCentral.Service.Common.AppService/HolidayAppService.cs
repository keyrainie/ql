using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(HolidayAppService))]
    public class HolidayAppService
    {
        public virtual void Create(Holiday entity)
        {
            ObjectFactory<HolidayProcessor>.Instance.Create(entity);
        }

        public virtual void DeleteBatch(List<int> sysNos)
        {
            ObjectFactory<HolidayProcessor>.Instance.DeleteBatch(sysNos);
        }
    }
}
