using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.UI.RMA.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Service.RMA.Restful.RequestMsg;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View]
    public partial class RMATrackingQuery : PageBase
    {
        private RMATrackingFacade facade;
        private RMATrackingQueryVM queryVM;
        private RMATrackingQueryVM lastQueryVM;
        private List<RMATrackingVM> list;

        public RMATrackingQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new RMATrackingFacade(this);
            this.QueryFilter.DataContext = queryVM = new RMATrackingQueryVM();
            LoadComboBoxData();
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //权限控制:
            Button_Dispatch.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RMATracking_Dispatch);
            Button_CancelDispatch.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RMATracking_CancelDispatch);
            this.DataGrid_Query_ResultList.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RMATracking_Export);
        }

        private void LoadComboBoxData()
        {
            facade.GetRMATrackingCreateUsers(true, (obj, args) =>
            {
                this.queryVM.CreateUserList = args.Result;
            });
            facade.GetRMATrackingUpdateUsers(true, (obj1, args1) =>
            {
                this.queryVM.UpdateUserList = args1.Result;
            });

        }

        private void DataGrid_Query_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            ValidationManager.Validate(this.QueryFilter);
            if (queryVM.HasValidationErrors)
            {
                return;
            }
            facade.Query(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                list = DynamicConverter<RMATrackingVM>.ConvertToVMList(args.Result.Rows);
                this.DataGrid_Query_ResultList.ItemsSource = list;
                this.DataGrid_Query_ResultList.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_Dispatch_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectList = GetSelectedList();
            if (selectList.Count == 0)
            {
                Window.Alert(ResRMATracking.Msg_SelectItem);
            }
            else
            {
                UCChooseTrackingHandler uctl = new UCChooseTrackingHandler();
                uctl.vm.SysNoList = selectList;
                uctl.Dialog = Window.ShowDialog(ResRMATracking.Dialog_SelectHandler, uctl, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        this.DataGrid_Query_ResultList.Bind();
                        Window.Alert(ResRMATracking.Msg_SaveSuccess);

                    }
                });
            }
        }
        private void Button_CancelDispatch_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectList = GetSelectedList();
            if (selectList.Count == 0)
            {
                Window.Alert(ResRMATracking.Msg_SelectItem);
            }
            else
            {
                RMATrackingBatchActionReq request = new RMATrackingBatchActionReq();
                request.SysNoList = selectList;
                facade.CancelDispatch(request, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    else
                    {
                        this.DataGrid_Query_ResultList.Bind();
                        Window.Alert(ResRMATracking.Msg_SaveSuccess);

                    }
                });
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            RMATrackingVM vm = (sender as HyperlinkButton).DataContext as RMATrackingVM;
            string url = string.Format(ConstValue.RMA_TrackingMaintainUrl, vm.RegisterSysNo);
            Window.Navigate(url, null, true);
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RMATrackingQueryVM>(queryVM);
            this.DataGrid_Query_ResultList.Bind();
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.DataGrid_Query_ResultList.ItemsSource as dynamic;
                foreach (var view in viewList)
                {
                    view.IsChecked = (ckb.IsChecked != null && view.IsEnable == true) ? ckb.IsChecked.Value : false;
                }
            }
        }

        private List<int> GetSelectedList()
        {
            List<int> trackingList = new List<int>();
            if (this.DataGrid_Query_ResultList.ItemsSource != null)
            {
                dynamic viewList = this.DataGrid_Query_ResultList.ItemsSource as dynamic;

                foreach (var view in viewList)
                {
                    if (view.IsChecked)
                    {
                        trackingList.Add(view.SysNo);
                    }
                }
            }
            return trackingList;
        }

        private void btnEditRegister_Click(object sender, RoutedEventArgs e)
        {
            RMATrackingVM vm = (sender as HyperlinkButton).DataContext as RMATrackingVM;
            string url = string.Format(ConstValue.RMA_EditRegisterUrl, vm.RegisterSysNo);
            Window.Navigate(url, null, true);
        }

        private void btnEditSO_Click(object sender, RoutedEventArgs e)
        {
            RMATrackingVM vm = (sender as HyperlinkButton).DataContext as RMATrackingVM;
            string url = string.Format(ConstValue.SOMaintainUrlFormat, vm.SOSysNo);
            Window.Navigate(url, null, true);
        }

        private void DataGrid_Query_ResultList_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_Export))
            {
                Window.Alert(ResRMATracking.Msg_AuthError);
                return;
            }
            if (lastQueryVM == null || this.DataGrid_Query_ResultList.TotalCount < 1)
            {
                Window.Alert(ResRMATracking.Msg_ExportError);
                return;
            }

            if (this.DataGrid_Query_ResultList.TotalCount > 10000)
            {
                Window.Alert(ResRMATracking.Msg_ExportExceedsLimitCount);
                return;
            }

            ColumnSet columnSet = new ColumnSet(DataGrid_Query_ResultList, true);
            facade.ExportExcelFile(lastQueryVM, new ColumnSet[] { columnSet });
        }
    }
}
