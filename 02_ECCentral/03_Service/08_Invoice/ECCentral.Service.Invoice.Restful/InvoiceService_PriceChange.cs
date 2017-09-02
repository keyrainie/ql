using ECCentral.BizEntity.Invoice.PriceChange;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        [WebInvoke(UriTemplate = "/Invoice/QueryPriceChange", Method = "POST")]
        public virtual QueryResult QueryPriceChange(ChangePriceFilter filter)
        {
            int totalCount = 0;

            var dataTable = ObjectFactory<IPriceChangeQueryDA>.Instance.QueryPriceChange(filter, out totalCount);

            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Invoice/SavePriceChange", Method = "POST")]
        public int SavePriceChange(PriceChangeMaster items)
        {
            return ObjectFactory<PriceChangeAppService>.Instance.SavePriceChange(items);
        }

        [WebInvoke(UriTemplate = "/Invoice/ClonePriceChange", Method = "POST")]
        public string ClonePriceChange(List<int> sysNos)
        {
            return ObjectFactory<PriceChangeAppService>.Instance.ClonePriceChange(sysNos);
        }

        [WebInvoke(UriTemplate = "/Invoice/UpdatePriceChange", Method = "POST")]
        public PriceChangeMaster UpdatePriceChange(PriceChangeMaster item)
        {
            return ObjectFactory<PriceChangeAppService>.Instance.UpdatePriceChange(item);
        }

        [WebInvoke(UriTemplate = "/Invoice/BatchAuditPriceChange", Method = "POST")]
        public string BatchAuditPriceChange(Dictionary<int,string> sysNos)
        {
            return ObjectFactory<PriceChangeAppService>.Instance.BatchAuditPriceChange(sysNos);
        }

        [WebInvoke(UriTemplate = "/Invoice/BatchVoidPriceChange", Method = "POST")]
        public string BatchVoidPriceChange(List<int> sysNos)
        {
            return ObjectFactory<PriceChangeAppService>.Instance.BatchVoidPriceChange(sysNos);
        }

        [WebInvoke(UriTemplate = "/Invoice/GetPriceChangeBySysNo/{sysno}", Method = "GET")]
        public PriceChangeMaster GetPriceChangeBySysNo(string sysno)
        {
            int num;
            int.TryParse(sysno, out num);
            return ObjectFactory<PriceChangeAppService>.Instance.GetPriceChangeBySysNo(num);
        }

        [WebInvoke(UriTemplate = "/Invoice/BatchRunPriceChangeByManual",Method = "POST")]
        public string BatchRunPriceChangeByManual(List<int> sysNo)
        {
            return ObjectFactory<PriceChangeAppService>.Instance.BatchRunPriceChangeByManual(sysNo);
        }

        [WebInvoke(UriTemplate = "/Invoice/BatchAbortedPriceChangeByManual", Method = "POST")]
        public string BatchAbortedPriceChangeByManual(List<int> sysNo)
        {
            return ObjectFactory<PriceChangeAppService>.Instance.BatchAbortedPriceChangeByManual(sysNo);
        }

        [WebInvoke(UriTemplate = "/Invoice/BatchRunPriceChangeByJob", Method = "POST")]
        public string BatchRunPriceChangeByJob()
        {
            return ObjectFactory<PriceChangeAppService>.Instance.BatchRunPriceChangeByJob();
        }

        [WebGet(UriTemplate = "/Invoice/BatchRunPriceChangeByJobByGet/{sysno}")]
        public string BatchRunPriceChangeByJobByGet(string sysno)
        {
            string temp = sysno;

            return ObjectFactory<PriceChangeAppService>.Instance.BatchRunPriceChangeByJob();
        }

        [WebInvoke(UriTemplate = "/Invoice/BatchAbortedPriceChangeByJob", Method = "POST")]
        public string BatchAbortedPriceChangeByJob()
        {
            return ObjectFactory<PriceChangeAppService>.Instance.BatchAbortedPriceChangeByJob();
        }

        [WebInvoke(UriTemplate = "/Invoice/QueryLastVendorSysNo", Method = "POST")]
        public DataTable GetVendorBasicInfo()
        {
            var dataTable = ObjectFactory<IPriceChangeQueryDA>.Instance.QUeryLastVendorSysNo();
            return dataTable;
        }
    }
}
