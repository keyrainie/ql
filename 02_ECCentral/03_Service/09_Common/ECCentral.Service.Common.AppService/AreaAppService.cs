using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(AreaAppService))]
    public class AreaAppService
    {
        public virtual AreaInfo Create(AreaInfo entity)
        {
            return ObjectFactory<AreaProcessor>.Instance.Create(entity);
        }

        public virtual AreaInfo Update(AreaInfo entity)
        {
            return ObjectFactory<AreaProcessor>.Instance.Update(entity);
        }

        public virtual AreaInfo Load(int sysNo)
        {
            return ObjectFactory<AreaProcessor>.Instance.Load(sysNo);
        }
    }
}