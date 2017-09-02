using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(AutoInnerOnlineListProcessor))]
    public class AutoInnerOnlineListProcessor
    {
        public void ClearTableOnLinelist(string day)
        {
            ObjectFactory<IAutoInnerOnlineList>.Instance.ClearTableOnLinelist(day);
        }
    }
}
