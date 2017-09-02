using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
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

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class CouponUsedReportFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        public CouponUsedReportFacade()
            : this(null)
        {
        }

        public CouponUsedReportFacade(IPage viewPage)
        {
            this.viewPage = viewPage;
            restClient = new RestClient(ServiceBaseUrl, viewPage);
        }

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
        public void QueryCouponUsedReport(ResSalesCouponUsedReportQueryVM queryVM,
            int pageSize,
            int pageIndex,
            string sortField,
            Action<SalesCouponUsedReportQueryView> callback)
        {
            var filter = queryVM.ConvertVM<ResSalesCouponUsedReportQueryVM, CouponUsedReportFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            const string relativeUrl = "/InvoiceService/CouponUseedReport/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var result = new SalesCouponUsedReportQueryView();
                if (args.Result[0] != null && args.Result[0].Rows != null)
                {
                    result.Result = DynamicConverter<SalesCouponUsedReportDataVM>.ConvertToVMList(args.Result[0].Rows);
                    result.TotalCount = args.Result[0].TotalCount;


                    //result.TxtAllStatisticInfo = "总  计：--优惠券折扣金额：￥" + args.Result[1].Rows[0]["TotalAmount"].ToString("#0.00");
                    //result.TxtCurrentPageStatisticInfo = "本页小计：--优惠券折扣金额：￥" + args.Result[1].Rows[0]["CurrentPageAmount"].ToString("#0.00");
                }
                if (args.Result[1] != null && args.Result[1].Rows != null)
                {
                    result.Staticsticinfo = DynamicConverter<StaticsticInfo>.ConvertToVMList(args.Result[1].Rows);
                }
                if (result.Staticsticinfo==null)
                {
                    result.Staticsticinfo = DynamicConverter<StaticsticInfo>.ConvertToVMList(args.Result[0].Rows);
                }
                callback(result);
            });
        }


        public void ExportCouponUsedReport(ResSalesCouponUsedReportQueryVM queryVM, ColumnSet[] columnSet)
        {
            var filter = queryVM.ConvertVM<ResSalesCouponUsedReportQueryVM, CouponUsedReportFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = 0,
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                SortBy = null
            };

            const string relativeUrl = "/InvoiceService/CouponUseedReport/Export";
            restClient.ExportFile(relativeUrl, filter, columnSet);
        }


    }
}
