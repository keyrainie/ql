using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.ExternalSYS.BizProcessor.VendorPortal
{
    [VersionExport(typeof(VendorPortalLogProcessor))]
    public class VendorPortalLogProcessor
    {
        IVendorSystemInfoDA m_da;

        public VendorPortalLogProcessor()
        {
            this.m_da = ObjectFactory<IVendorSystemInfoDA>.Instance;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        public void WriteLog(VendorPortalLog log)
        {
            m_da.WriteLog(log);
        }
    }
}
