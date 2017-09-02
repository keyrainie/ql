using System;
using System.Linq;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class POSPayFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public POSPayFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryPOSPayConfirmList(POSPayQueryVM query, int pageSize, int pageIndex, string sortField, Action<POSPayQueryResultVM> callback)
        {
            POSPayQueryFilter filter = query.ConvertVM<POSPayQueryVM, POSPayQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/POSPay/QueryConfirmList";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                POSPayQueryResultVM result = new POSPayQueryResultVM();
                if (args.Result[0] != null && args.Result[0].Rows != null)
                {
                    result.ResultList = DynamicConverter<POSPayVM>.ConvertToVMList(args.Result[0].Rows);
                    result.TotalCount = args.Result[0].TotalCount;
                }
                
                if (args.Result[1] != null && args.Result[1].Rows != null)
                {
                    result.Statistic = DynamicConverter<POSPayQueryStatisticVM>.ConvertToVMList(args.Result[1].Rows);
                }
                callback(result);
            });
        }
    }
}