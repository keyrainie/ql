using System;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Facades
{
    public class BalanceAccountFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string InvoiceServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public BalanceAccountFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        public void Query(BalanceAccountQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            BalanceAccountQueryFilter filter = queryVM.ConvertVM<BalanceAccountQueryVM, BalanceAccountQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/BalanceAccount/Query";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void ExportExcelFile(BalanceAccountQueryVM queryVM, ColumnSet[] columnSet)
        {
            BalanceAccountQueryFilter queryFilter = queryVM.ConvertVM<BalanceAccountQueryVM, BalanceAccountQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = ""
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/BalanceAccount/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }
    }
}