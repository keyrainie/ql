using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(FinanceAppService))]
    public class FinanceAppService
    {
        private FinanceProcessor processor = ObjectFactory<FinanceProcessor>.Instance;

        /// <summary>
        /// 添加备注
        /// </summary>
        /// <param name="payableInfo"></param>
        public virtual PayableInfo AddMemo(PayableInfo info)
        {
            return processor.AddMemo(info);
        }

        /// <summary>
        /// 查询备注
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual List<PayableInfo> PayableQuery(PayableCriteriaInfo info)
        {
            return processor.PayableQuery(info);
        }
    }
}
