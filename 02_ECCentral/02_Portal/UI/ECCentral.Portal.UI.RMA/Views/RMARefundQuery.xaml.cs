using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(NeedAccess = false)]
    public partial class RMARefundQuery : PageBase
    {
        private CommonDataFacade commonFacade = null;
        private RefundFacade facade = null;

        public RefundQueryReqVM FilterVM { get; set; }

        private RefundQueryReqVM exportVM;

        public RMARefundQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, System.EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.FilterVM = new RefundQueryReqVM();

            facade = new RefundFacade(this);
            commonFacade = new CommonDataFacade(this);
            commonFacade.GetStockList(true, (obj, args) =>
            {
                this.FilterVM.Stocks = args.Result;

                this.ucFilter.DataContext = this.FilterVM;
            });
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //权限控制:
            btnNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_New);
            dataRMARefundList.IsShowAllExcelExporter = false;
            //dataRMARefundList.IsShowAllExcelExporter=AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_Export);
         }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.ucFilter))
            {
                exportVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RefundQueryReqVM>(FilterVM);
                dataRMARefundList.QueryCriteria = exportVM;
                dataRMARefundList.Bind();
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.RMA_CreateRefundUrl, null, true);
        }

        private void dataRMARefundList_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.Query(this.dataRMARefundList.QueryCriteria as RefundQueryReqVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dataRMARefundList.ItemsSource = args.Result.Rows;
                this.dataRMARefundList.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            string url = string.Format(ConstValue.RMA_RefundMaintainUrl, vm.SysNo);
            Window.Navigate(url, null, true);
        }

        private void dataRMARefundList_ExportAllClick(object sender, System.EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_Export))
            {
                Window.Alert(ResRefundQuery.Msg_AuthError);
                return;
            }
            if (exportVM == null || this.dataRMARefundList.TotalCount < 1)
            {
                Window.Alert(ResRefundQuery.Msg_ExportError);
                return;
            }

            if (this.dataRMARefundList.TotalCount > 10000)
            {
                Window.Alert(ResRefundQuery.Msg_ExportExceedsLimitCount);
                return;
            }
            ColumnSet columnSet = new ColumnSet(dataRMARefundList, true);
            facade.ExportExcelFile(this.exportVM, new ColumnSet[] { columnSet });
        }
    }
}
