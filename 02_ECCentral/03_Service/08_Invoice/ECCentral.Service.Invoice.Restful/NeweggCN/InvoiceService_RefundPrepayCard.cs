using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 神州运通 退预付卡
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Invoice/RefundPrepayCard", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public int RefundPrepayCard(RefundPrepayCardInfo info)
        {
            return ObjectFactory<RefundPrepayCardAppService>.Instance.RefundPrepayCard(info);
        }
    }
}
