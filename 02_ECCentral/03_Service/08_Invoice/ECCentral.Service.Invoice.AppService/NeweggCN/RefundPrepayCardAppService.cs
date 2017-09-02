using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(RefundPrepayCardAppService))]
    public class RefundPrepayCardAppService
    {
        private RefundPrepayCardProcessor processor = ObjectFactory<RefundPrepayCardProcessor>.Instance;

        /// <summary>
        /// 神州运通——退预付卡
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int RefundPrepayCard(RefundPrepayCardInfo info)
        {
            return processor.RefundPrepayCard(info);
        }
    }
}
