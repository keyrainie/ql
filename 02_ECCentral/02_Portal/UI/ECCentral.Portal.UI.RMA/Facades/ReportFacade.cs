using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.RMA.Restful.RequestMsg;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.RMA.Facades
{
    public class ReportFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_RMA, "ServiceBaseUrl");
            }
        }
        public ReportFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public ReportFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }


        public void QueryProductCardInventory(int? productSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/RMAService/Report/ProductCards/QueryProductCardsInventory/{0}", productSysNo);
            restClient.QueryDynamicData(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void QueryProductCards(int? productSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/RMAService/Report/ProductCards/QueryProductCards/{0}", productSysNo);
            restClient.QueryDynamicData(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void QueryOutBoundNotReturn(OutBoundNotReturnQueryVM queryVM, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            OutBoundNotReturnQueryFilter queryFilter = new OutBoundNotReturnQueryFilter();
            queryFilter = queryVM.ConvertVM<OutBoundNotReturnQueryVM, OutBoundNotReturnQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/RMAService/Report/OutBoundNotReturn/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void SendDunEmail(SendDunEmailReq request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/RMAService/Report/OutBoundNotReturn/SendDunEmail";
            restClient.Update(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }


        public void QueryRMAProductInventory(RMAInventoryQueryVM queryVM, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            RMAInventoryQueryFilter queryFilter = new RMAInventoryQueryFilter();
            queryFilter = queryVM.ConvertVM<RMAInventoryQueryVM, RMAInventoryQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/RMAService/Report/RMAInventory/QueryProduct";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        public void QueryRMAItemInventory(RMAInventoryQueryVM queryVM, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            RMAInventoryQueryFilter queryFilter = new RMAInventoryQueryFilter();
            queryFilter = queryVM.ConvertVM<RMAInventoryQueryVM, RMAInventoryQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/RMAService/Report/RMAInventory/QueryRMAItem";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void ExportOutBoundExcelFile(OutBoundNotReturnQueryVM queryVM, ColumnSet[] columns)
        {
            OutBoundNotReturnQueryFilter queryFilter = new OutBoundNotReturnQueryFilter();
            queryFilter = queryVM.ConvertVM<OutBoundNotReturnQueryVM, OutBoundNotReturnQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/RMAService/Report/OutBoundNotReturn/Query";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        public void ExportProductExcelFile(RMAInventoryQueryVM queryVM, ColumnSet[] columns)
        {
            RMAInventoryQueryFilter queryFilter = new RMAInventoryQueryFilter();
            queryFilter = queryVM.ConvertVM<RMAInventoryQueryVM, RMAInventoryQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/RMAService/Report/RMAInventory/QueryProductForExport";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        public void ExportRMAItemExcelFile(RMAInventoryQueryVM queryVM, ColumnSet[] columns)
        {
            RMAInventoryQueryFilter queryFilter = new RMAInventoryQueryFilter();
            queryFilter = queryVM.ConvertVM<RMAInventoryQueryVM, RMAInventoryQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/RMAService/Report/RMAInventory/QueryRMAItem";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

       
    }
}
