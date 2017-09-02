using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Report;
using ECCentral.BizEntity.Invoice.InvoiceReport;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IInvoiceReportDA
    {
        TrackingNumberInfo CreateTrackingNumber(TrackingNumberInfo entity);

        SOInvoiceMaster GetSOInvoiceMaster(int soNo, int stockSysNo, string strNEG, string strMET);

        InvoiceInfo GetInvoicePrintHead(int soNo, int stockSysNo);

        List<InvoiceItem> GetSOInvoiceProductItem(int soNo, int stockSysNo);

        List<InvoiceItem> GetSOExtendWarrantyItem(int soNo, int stockSysNo);

        List<InvoiceSub> GetInvoiceSub(int soNo, int stockSysNo);

        string GetSysConfiguration(string Key);
    }
}