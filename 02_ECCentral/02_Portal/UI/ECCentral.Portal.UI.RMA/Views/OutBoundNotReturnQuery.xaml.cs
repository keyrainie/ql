using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Service.RMA.Restful.RequestMsg;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class OutBoundNotReturnQuery : PageBase
    {
        private ReportFacade facade;
        private OutBoundNotReturnQueryVM queryVM;

        private OutBoundNotReturnQueryVM lastQueryVM;

        private CommonDataFacade commonFacade;
        private List<OutBoundNotReturnListVM> list;
        public OutBoundNotReturnQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new ReportFacade(this);
            commonFacade = new CommonDataFacade(this);
            queryVM = new OutBoundNotReturnQueryVM();
            string getQueryParams = Request.Param;

            //Victor Added ,接收PO查询页面传过来的查询条件参数:
            if (!string.IsNullOrEmpty(getQueryParams))
            {
                string[] getParamList = getQueryParams.Split('|');
                queryVM.SendDays = getQueryParams.Split('|')[0].Trim();
                queryVM.HasResponse = getQueryParams.Split('|')[1].Trim() == "1" ? true : false;
                if (getParamList.Length > 2)
                {
                    queryVM.VendorSysNo = getQueryParams.Split('|')[2].Trim();

                }
                this.QueryFilter.DataContext = queryVM;
                Button_Search_Click(null, null);
            }
            else
            {
                this.QueryFilter.DataContext = queryVM;
            }

        }
        private void DataGrid_Query_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryOutBoundNotReturn(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                list = DynamicConverter<OutBoundNotReturnListVM>.ConvertToVMList(args.Result.Rows);
                this.DataGrid_Query_ResultList.ItemsSource = list;
                this.DataGrid_Query_ResultList.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.QueryFilter);
            if (queryVM.HasValidationErrors)
            {
                return;
            }
            lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<OutBoundNotReturnQueryVM>(queryVM);

            this.DataGrid_Query_ResultList.Bind();
        }

        private void btnProductID_Click(object sender, RoutedEventArgs e)
        {
            OutBoundNotReturnListVM vm = (sender as HyperlinkButton).DataContext as OutBoundNotReturnListVM;
            // 链接到website 产品页面
            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);            
            UtilityHelper.OpenWebPage(string.Format(urlFormat, vm.ProductSysNo));
        }

        private void btnProductName_Click(object sender, RoutedEventArgs e)
        {
            //Victor Added :链接到PO - 查看商品采购历史 页面 ：
            OutBoundNotReturnListVM vm = (sender as HyperlinkButton).DataContext as OutBoundNotReturnListVM;
            string url = string.Format(ConstValue.IM_ProductPurchaseHistoryUrlFormat, vm.ProductSysNo + "|" + vm.ProductID);
            Window.Navigate(url, null, true);

        }

        private void btnRegisterSysNo_Click(object sender, RoutedEventArgs e)
        {
            OutBoundNotReturnListVM vm = (sender as HyperlinkButton).DataContext as OutBoundNotReturnListVM;
            string url = string.Format(ConstValue.RMA_EditRegisterUrl, vm.RegisterSysNo);
            Window.Navigate(url, null, true);
        }

        private void btnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            OutBoundNotReturnListVM vm = (sender as HyperlinkButton).DataContext as OutBoundNotReturnListVM;
            string url = string.Format(ConstValue.SOMaintainUrlFormat, vm.SOSysNo);
            Window.Navigate(url, null, true);

        }

        private void btnSendMail_Click(object sender, RoutedEventArgs e)
        {
            OutBoundNotReturnListVM vm = (sender as HyperlinkButton).DataContext as OutBoundNotReturnListVM;
            SendDunEmailReq request = new SendDunEmailReq();
            request.SendMailCount = vm.IsSendMail.HasValue ? (vm.IsSendMail.Value + 1) : 1;
            request.OutboundSysNo = vm.OutboundSysNo.Value;
            request.RegisterSysNo = vm.RegisterSysNo.Value;
            request.SOSysNo = vm.SOSysNo.Value;
            facade.SendDunEmail(request, (obj, args) =>
            {
                this.DataGrid_Query_ResultList.Bind();
            });
        }
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                var txtBinding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (txtBinding != null)
                {
                    txtBinding.UpdateSource();
                    this.DataGrid_Query_ResultList.Bind();
                }
            }
        }

        private void DataGrid_Query_ResultList_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.DataGrid_Query_ResultList.TotalCount < 1)
            {
                Window.Alert(ResRMAReports.Msg_ExportError);
                return;
            }
            if (this.DataGrid_Query_ResultList.TotalCount > 10000)
            {
                Window.Alert(ResRMAReports.Msg_ExportExceedsLimitCount);
                return;
            }

            ColumnSet col = new ColumnSet(this.DataGrid_Query_ResultList, true);
            facade.ExportOutBoundExcelFile(lastQueryVM, new ColumnSet[] { col });

        }
    }
}
