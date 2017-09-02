using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.BizProcessor;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.AppService.EIMS
{
    [VersionExport(typeof(EIMSEventMemoAppService))]
    public class EIMSEventMemoAppService
    {
        private EIMSEventMemoProcessor processor = ObjectFactory<EIMSEventMemoProcessor>.Instance;

        #region 录入发票信息
        /// <summary>
        /// 录入发票信息
        /// </summary>
        /// <param name="entities"></param>
        public virtual EIMSInvoiceResult CreateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities)
        {
            return processor.CreateEIMSInvoiceInput(entities);
        }
        #endregion

        #region 修改发票信息
        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="entities"></param>
        public virtual EIMSInvoiceResult UpdateEIMSInvoiceInput(List<EIMSInvoiceInfoEntity> entities)
        {
            return processor.UpdateEIMSInvoiceInput(entities);
        }
        #endregion
    }
}
