using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.Customer.UserControls.Gift;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class CustomerGiftSearch : PageBase
    {
        private CustomerGiftFacade _facade;
        private CustomerGiftQueryVM viewModel;
        private List<CustomerGiftListVM> _rows;
        private CheckBox _checkBoxAll;
        private CommonDataFacade _facadeChannel;
        private CustomerGiftQueryFilter filter;

        public CustomerGiftSearch()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.Grid.DataContext = viewModel = new CustomerGiftQueryVM();
            _facade = new CustomerGiftFacade(this);
            _facadeChannel = new CommonDataFacade(this);

            //绑定状态ComboBox
            this.cmbCustomerGiftStatus.ItemsSource = EnumConverter.GetKeyValuePairs<CustomerGiftStatus>(EnumConverter.EnumAppendItemType.All);
            CheckRights();
        }

        /// <summary>
        /// 请求服务，并将查询结果绑定到DataGrid
        /// </summary>
        void DataGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            filter = viewModel.ConvertVM<CustomerGiftQueryVM, CustomerGiftQueryFilter>();
            filter.PagingInfo = new PagingInfo
           {
               PageIndex = e.PageIndex,
               PageSize = e.PageSize,
               SortBy = e.SortField
           };
            _facade.Query(filter, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    this.DataGrid.TotalCount = args.Result.TotalCount;

                    _rows = DynamicConverter<CustomerGiftListVM>.ConvertToVMList(args.Result.Rows);
                    this.DataGrid.ItemsSource = _rows;
                    if (_checkBoxAll != null)
                    {
                        //重新查询后，将全选CheckBox置为false
                        _checkBoxAll.IsChecked = false;
                    }
                });
        }

        private void BatchAction(string confirmMsg, Action<List<CustomerGiftListVM>> realAction)
        {
            if (_rows == null)
            {
                this.Window.Alert(ResCustomerGiftSearch.Msg_PleaseSelect);
                return;
            }

            var selectedList = _rows.Where(item => item.IsChecked).ToList();
            if (selectedList.Count == 0)
            {
                this.Window.Alert(ResCustomerGiftSearch.Msg_PleaseSelect);
                return;
            }
            if (realAction != null && !string.IsNullOrWhiteSpace(confirmMsg))
            {
                Window.Confirm(confirmMsg, (s, args) =>
                    {
                        if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                            realAction(selectedList);
                    });
            }
        }

        private void UpdateSelectedItems(List<CustomerGiftListVM> selectedList, List<int> successSysNoList, CustomerGiftStatus targetStatus)
        {
            int targetStatusCode = (int)targetStatus;
            foreach (var sysNo in successSysNoList)
            {
                var found = selectedList.FirstOrDefault(item => item.SysNo == sysNo);
                if (found != null)
                    found.Status = targetStatusCode;
            }
            UnselectItems(selectedList);
        }


        void ButtonCancelAbandon_Click(object sender, RoutedEventArgs e)
        {
            BatchAction(ResCustomerGiftSearch.Msg_ConfirmCancelAbandon, (selectedList) =>
                {
                    _facade.CancelAbandon(selectedList, (s, args) =>
                        {
                            this.DataGrid.Bind();
                            if (args.FaultsHandle())
                                return;
                            this.Window.Alert(ResCustomerGiftSearch.Msg_CancelAbandonSuccess);
                            UpdateSelectedItems(selectedList, args.Result, CustomerGiftStatus.Origin);
    
                        });
                });
        }

        void ButtonAbandon_Click(object sender, RoutedEventArgs e)
        {
            BatchAction(ResCustomerGiftSearch.Msg_ConfirmAbandon, (selectedList) =>
            {
                _facade.Abandon(selectedList, (s, args) =>
                {
                    this.DataGrid.Bind();
                    if (args.FaultsHandle())
                        return;
                    this.Window.Alert(ResCustomerGiftSearch.Msg_AbandonSuccess);
                    UpdateSelectedItems(selectedList, args.Result, CustomerGiftStatus.Voided);
                });
            });
        }

        private void UnselectItems(List<CustomerGiftListVM> selectedList)
        {
            foreach (var item in selectedList)
            {
                item.IsChecked = false;
            }
        }

        void ButtonVoid_Click(object sender, RoutedEventArgs e)
        {
            BatchAction(ResCustomerGiftSearch.Msg_ConfirmSendVoid, (selectedList) =>
            {
                _facade.Void(selectedList, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    this.Window.Alert(ResCustomerGiftSearch.Msg_SendVoidMailSuccess);
                    UnselectItems(selectedList);
                });
            });
        }

        void ButtonSendAlert_Click(object sender, RoutedEventArgs e)
        {
            BatchAction(ResCustomerGiftSearch.Msg_ConfirmSendAlert, (selectedList) =>
            {
                _facade.Remind(selectedList, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    this.Window.Alert(ResCustomerGiftSearch.Msg_SendExpiringMailSuccess);
                    UnselectItems(selectedList);
                });
            });
        }

        void ButtonSendWinGift_Click(object sender, RoutedEventArgs e)
        {
            BatchAction(ResCustomerGiftSearch.Msg_ConfirmSendWin, (selectedList) =>
            {
                _facade.Notify(selectedList, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    this.Window.Alert(ResCustomerGiftSearch.Msg_SendWinMailSuccess);
                    UnselectItems(selectedList);
                });
            });
        }

        void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CustomerGiftMaintain window = new CustomerGiftMaintain();
            window.Dialog = Window.ShowDialog(ResCustomerGiftSearch.Dialog_AddGift, window, (obj, args) =>
            {
            }, new Size(580, 400));
        }

        /// <summary>
        /// 查询按钮事件，直接调用DataGrid的Bind方法，DataGrid内部会触发LoadingDataSource事件
        /// </summary>
        void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            if (_checkBoxAll != null)
            {
                if (_rows != null)
                {
                    foreach (var item in _rows)
                    {
                        item.IsChecked = _checkBoxAll.IsChecked ?? false;
                    }
                }
            }
        }

        private void DataGridCheckBoxAll_Loaded(object sender, RoutedEventArgs e)
        {
            _checkBoxAll = sender as CheckBox;
        }

        /// <summary>
        /// 点击CustomerID跳转到顾客维护页面
        /// </summary>
        private void Hyperlink_CustomerID_Click(object sender, RoutedEventArgs e)
        {
            var lnk = sender as FrameworkElement;
            if (null != lnk)
            {
                this.Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, lnk.Tag.ToString()), null, true);
            }
        }

        private void Hyperlink_SoSysNo_Click(object sender, RoutedEventArgs e)
        {
            var lnk = sender as FrameworkElement;
            if (null != lnk)
            {
                this.Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, lnk.Tag.ToString()), null, true);
            }
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Gift_Add))
                this.ButtonCreate.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Gift_Abandon))
                this.ButtonAbandon.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Gift_CancelAbandon))
                this.ButtonCancelAbandon.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Gift_SendGiftMsg))
                this.ButtonSendWinGift.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Gift_SendGiftAwoke))
                this.ButtonSendAlert.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Gift_SendAbandonMsg))
                this.ButtonVoid.IsEnabled = false;
        }
        #endregion

       
    }

}
