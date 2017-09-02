using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.SO.BizProcessor;

namespace ECCentral.Service.SO.AppService
{
    [VersionExport(typeof(SOPendingAppService))]
    public class SOPendingAppService
    {
        SOPendingProcessor m_pro;
        public SOPendingAppService()
        {
            m_pro = ObjectFactory<SOPendingProcessor>.Instance;
        }

        public SOPending CreateSOPending(SOPending pending)
        {
            return null;
        }

        public void OpenSOPending(int soSysNo)
        {
            m_pro.Open(soSysNo);
        }

        public void CloseSOPending(int soSysNo)
        {
            m_pro.Close(soSysNo);
        }

        public void UpdateSOPending(int soSysNo)
        {
            m_pro.Update(soSysNo);
        }
    }
}
