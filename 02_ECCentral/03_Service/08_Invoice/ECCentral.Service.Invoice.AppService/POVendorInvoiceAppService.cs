using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.BizProcessor;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(POVendorInvoiceAppService))]
    public class POVendorInvoiceAppService
    {
        private POVendorInvoiceProcessor processor = ObjectFactory<POVendorInvoiceProcessor>.Instance;
        public POVendorInvoiceInfo Create(BizEntity.Invoice.POVendorInvoiceInfo input)
        {
            return processor.Create(input);
        }

        public void Update(BizEntity.Invoice.POVendorInvoiceInfo entity)
        {
            processor.Update(entity);
        }


        public string BatchAuditPOVendorInvoice(List<int> sysNoList)
        {
            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.Audit(sysNo);
            });
            return result.PromptMessage;
        }

        public string BatchUnAuditPOVendorInvoice(List<int> sysNoList)
        {

            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.UnAudit(sysNo);

            });
            return result.PromptMessage;
        }

        public string BatchAbandonPOVendorInvoice(List<int> sysNoList)
        {
            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.Abandon(sysNo);

            });
            return result.PromptMessage;
        }

        public string BatchUnAbandonPOVendorInvoice(List<int> sysNoList)
        {
            var result = BatchActionManager.DoBatchAction(GetRequestItem(sysNoList), (sysNo) =>
            {
                processor.UnAbandon(sysNo);

            });
            return result.PromptMessage;
        }

        private List<BatchActionItem<int>> GetRequestItem(List<int> sysNoList)
        {
            return sysNoList.Select(x => new BatchActionItem<int>()
             {
                 ID = x.ToString(),
                 Data = x
             }).ToList();
        }
    }
}
