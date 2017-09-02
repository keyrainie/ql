using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.BizEntity.Enum.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.RMA;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View]
    public partial class RMARequestQuery : PageBase
    {               
        private CommonDataFacade facade = null;
        private RequestFacade requestFacade = null;

        public RequestQueryReqVM FilterVM { get; set; }

        public RMARequestQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {           
            base.OnPageLoad(sender, e);
            this.FilterVM = new RequestQueryReqVM();
            facade = new CommonDataFacade(this);
            requestFacade = new RequestFacade(this);
           
            requestFacade.GetAllReceiveUsers((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                var list = args.Result;
                list.Insert(0, new BizEntity.Common.UserInfo { SysNo = null, UserName = ResCommonEnum.Enum_All });
                this.FilterVM.ReceiveUsers = args.Result;

                requestFacade.GetAllConfirmUsers((obj1, args1) =>
                {
                    if (args1.FaultsHandle())
                    {
                        return;
                    }
                    var confirmlist = args1.Result;
                    confirmlist.Insert(0, new BizEntity.Common.UserInfo { SysNo = null, UserName = ResCommonEnum.Enum_All });
                    this.FilterVM.ConfirmUsers = args1.Result;

                    this.ucFilter.DataContext = this.FilterVM;
                    FilterVM.Status = RMARequestStatus.WaitingAudit;
                });
                
            });

           
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //权限控制:
            btnNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanAdd);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.ucFilter))
            {
                dataRMARequestList.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RequestQueryReqVM>(this.FilterVM);
                dataRMARequestList.Bind();
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.RMA_CreateRequestUrl, null, true);
        }

        private void dataRMARequestList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            requestFacade.Query(this.dataRMARequestList.QueryCriteria as RequestQueryReqVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dataRMARequestList.ItemsSource = args.Result.Rows;
                this.dataRMARequestList.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            string url = string.Format(ConstValue.RMA_RequestMaintainUrl,vm.SysNo);
            Window.Navigate(url, null, true);
        }

        private void btnSO_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = (sender as HyperlinkButton).DataContext;
            Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, d.SOID), null, true);
        }
    }
}
