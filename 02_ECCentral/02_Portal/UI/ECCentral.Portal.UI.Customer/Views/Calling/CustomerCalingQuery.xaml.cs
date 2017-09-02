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
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.UserControls.Calling;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Customer.Views.Calling
{
    [View]
    public partial class CustomerCalingQuery : PageBase
    {
        CustomerCalingQueryVM viewModel;
        CustomerCallingFacade facade;
        CustomerCallingQueryFilter filter;
        public CustomerCalingQuery()
        {
            filter = new CustomerCallingQueryFilter();
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            viewModel = new CustomerCalingQueryVM();
            viewModel.HasExportRight = AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CallsEvents_Export);
            this.DataContext = viewModel;
            CheckRights();
            facade = new CustomerCallingFacade(this);
            facade.GetUpdateUser((obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                if (args.Result != null && args.Result.Rows != null)
                {
                    List<UserInfo> list = new List<UserInfo>();
                    foreach (var item in args.Result.Rows)
                    {
                        list.Add(new UserInfo() { UserDisplayName = item.DisplayName, SysNo = item.LastEditUserSysNo });
                    }
                    list.Insert(0, new UserInfo() { UserDisplayName = ResCommonEnum.Enum_All });
                    cbLastUpdateUser.ItemsSource = list;
                }
            });
            base.OnPageLoad(sender, e);
        }



        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            CustomerCallingMaintain uc = new CustomerCallingMaintain();
            uc.Dialog = Window.ShowDialog(ResCustomerCalingQuery.Dialog_Title_Edit, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    Search();
                }
            }, new Size(1050, 600));
        }

        private void dataGridSo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.PagingInfo = new PagingInfo()
              {
                  PageSize = e.PageSize,
                  PageIndex = e.PageIndex,
                  SortBy = e.SortField
              };
            facade.QuerySOList(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                viewModel.SoList = DynamicConverter<SOVM>.ConvertToVMList(args.Result.Rows);
                dataGridSo.ItemsSource = viewModel.SoList;
                dataGridSo.TotalCount = args.Result.TotalCount;
            });
        }

        private void dataGridCalling_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.PagingInfo = new PagingInfo()
              {
                  PageSize = e.PageSize,
                  PageIndex = e.PageIndex,
                  SortBy = e.SortField
              };
            facade.QueryCalling(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                viewModel.CallingList = DynamicConverter<CallingVM>.ConvertToVMList(args.Result.Rows);
                dataGridCalling.ItemsSource = viewModel.CallingList;
                dataGridCalling.TotalCount = args.Result.TotalCount;
            });
        }

        private void dataGridComplain_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {

            filter.PagingInfo = new PagingInfo()
              {
                  PageSize = e.PageSize,
                  PageIndex = e.PageIndex,
                  SortBy = e.SortField
              };
            facade.QueryComplainList(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                viewModel.ComplainList = DynamicConverter<ComplainVM>.ConvertToVMList(args.Result.Rows);
                dataGridComplain.ItemsSource = viewModel.ComplainList;
                dataGridComplain.TotalCount = args.Result.TotalCount;
            });
        }

        private void dataGridRMA_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.PagingInfo = new PagingInfo()
           {
               PageSize = e.PageSize,
               PageIndex = e.PageIndex,
               SortBy = e.SortField
           };
            facade.QueryRMAList(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                viewModel.RMAList = DynamicConverter<RMAVM>.ConvertToVMList(args.Result.Rows);
                dataGridRMA.ItemsSource = viewModel.RMAList;
                dataGridRMA.TotalCount = args.Result.TotalCount;
            });
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            //对重启次数输入框做验证
            if (viewModel.IsReopen)
            {
                List<ValidationEntity> list = new List<ValidationEntity>();
                list.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, "重启次数不能为空!"));
                list.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^\d{1,5}$", "请输入5位以内正整数!"));
                if (!ValidationHelper.Validation(this.txtReopenCount, list))
                    return;
            }
            ValidationHelper.ClearValidation(this.txtReopenCount);
            //对来电完成时数
            if (viewModel.OperaterCallingHours != null)
            {
                List<ValidationEntity> list = new List<ValidationEntity>();
                list.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, "来电完成时数不能为空!"));
                list.Add(new ValidationEntity(ValidationEnum.IsInteger, null, "请输入有效的正整数!"));
                if (!ValidationHelper.Validation(this.txtCallingHours, list))
                    return;
            }
            ValidationHelper.ClearValidation(this.txtCallingHours);
            //对来电完成次数
            if (viewModel.OperaterCallingTimes != null)
            {
                List<ValidationEntity> list = new List<ValidationEntity>();
                list.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, "来电完成次数不能为空!"));
                list.Add(new ValidationEntity(ValidationEnum.IsInteger, null, "请输入有效的正整数!"));
                if (!ValidationHelper.Validation(this.txtCallingTimes, list))
                    return;
            }
            ValidationHelper.ClearValidation(this.txtCallingTimes);

            filter = viewModel.ConvertVM<CustomerCalingQueryVM, CustomerCallingQueryFilter>();
            dataGridSo.Bind();
            dataGridCalling.Bind();
            dataGridComplain.Bind();
            dataGridRMA.Bind();

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.IsReopen)
            {
                txtReopenCount.IsReadOnly = false;
            }
            else
            {
                txtReopenCount.IsReadOnly = true;
                viewModel.ReopenCount = null;
                ValidationHelper.ClearValidation(this.txtReopenCount);
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(viewModel.PhoneORCellphone))
            {
                //
                CustomerQueryFilter filter = new CustomerQueryFilter();
                filter.Phone = viewModel.PhoneORCellphone;
                filter.CompanyCode = CPApplication.Current.CompanyCode;
                new CustomerQueryFacade(this).QueryCustomer(filter, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    if (args.Result != null && args.Result.TotalCount > 0)
                    {
                        viewModel.CustomerID = args.Result.Rows[0]["CustomerID"];
                        viewModel.CustomerName = args.Result.Rows[0]["CustomerName"];
                        viewModel.Address = args.Result.Rows[0]["DwellAddress"];
                    }
                    else
                    {
                        viewModel.PhoneORCellphone = string.Empty;
                        viewModel.CustomerID = string.Empty;
                        viewModel.CustomerName = string.Empty;
                        viewModel.Address = string.Empty;
                    }
                });
            }
        }

        private void hlbCustomer_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(string.Format("{0}/{1}", ConstValue.CustomerMaintainCreateUrl, (dataGridSo.SelectedItem as SOVM).CustomerSysNo), null, true);
        }

        private void hlbSO_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, (dataGridSo.SelectedItem as SOVM).SOID), null, true);
        }

        private void hlbToComplan_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerCallingList_ToComplain))
            {
                Window.Alert(ResCustomerCalingQuery.Msg_HasNoRights);
                return;
            }
            ComplainAdd newRequestCtrl = new ComplainAdd();
            CallingVM vm = dataGridCalling.SelectedItem as CallingVM;
            newRequestCtrl.viewModel.SOSysNo = vm.OrderSysNo;
            newRequestCtrl.viewModel.CustomerEmail = vm.Email;
            newRequestCtrl.viewModel.CustomerPhone = vm.Phone;
            newRequestCtrl.viewModel.CallsEventSysNo = vm.SysNo.Value;
            newRequestCtrl.Dialog = Window.ShowDialog(ResCustomerCalingQuery.Title_CreateComplaint, newRequestCtrl, (s, args) =>
                       {
                           if (args.DialogResult == DialogResultType.OK)
                           {
                               Search();
                           }
                       }, new Size(600, 300)
                );
        }

        private void hlbToRMA_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerCallingList_ToRMA))
            {
                Window.Alert(ResCustomerCalingQuery.Msg_HasNoRights);
                return;
            }
            Window.Navigate(string.Format(ConstValue.CustomerCalingRMAAdd, (dataGridCalling.SelectedItem as CallingVM).SysNo), null, true);
        }

        private void hlbReopen_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerCallingList_ReOpen))
            {
                Window.Alert(ResCustomerCalingQuery.Msg_HasNoRights);
                return;
            }
            CustomerCallingMaintain uc = new CustomerCallingMaintain();
            uc.viewModel.SysNo = (dataGridCalling.SelectedItem as CallingVM).SysNo;
            uc.Operate = "reopen";
            uc.Dialog = Window.ShowDialog(ResCustomerCalingQuery.Dialog_Title_Edit, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    Search();
                }
            }, new Size(1050, 600));
        }

        private void hlbOpenCallsEvents_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerCallingList_Open))
            {
                Window.Alert(ResCustomerCalingQuery.Msg_HasNoRights);
                return;
            }
            CustomerCallingMaintain uc = new CustomerCallingMaintain();
            uc.viewModel.SysNo = (dataGridCalling.SelectedItem as CallingVM).SysNo;
            uc.Operate = "open";
            uc.Dialog = Window.ShowDialog(ResCustomerCalingQuery.Dialog_Title_Edit, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    Search();
                }
            }, new Size(1050, 600));
        }

        private void hlbCloseCallsEvents_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerCallingList_Close))
            {
                Window.Alert(ResCustomerCalingQuery.Msg_HasNoRights);
                return;
            }
            CustomerCallingMaintain uc = new CustomerCallingMaintain();
            uc.viewModel.SysNo = (dataGridCalling.SelectedItem as CallingVM).SysNo;
            uc.Operate = "close";
            uc.Dialog = Window.ShowDialog(ResCustomerCalingQuery.Dialog_Title_Edit, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    Search();
                }
            }, new Size(1050, 600));
        }

        private void hlbOpenRMA_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerRMA_Open))
            {
                Window.Alert(ResCustomerCalingQuery.Msg_HasNoRights);
                return;
            }
            Window.Navigate(string.Format(ConstValue.RMA_EditRegisterUrl, (dataGridRMA.SelectedItem as RMAVM).SysNo), null, true);
        }

        private void hlbOpenCompaint_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerComplain_Open))
            {
                Window.Alert(ResCustomerCalingQuery.Msg_HasNoRights);
                return;
            }
            Window.Navigate(string.Format(ConstValue.SO_ComplainReplyUrl, (dataGridComplain.SelectedItem as ComplainVM).SysNo), null, true);
        }

        private void InitExportFilter()
        {
            filter.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
        }
        private void dataGridCalling_ExportAllClick(object sender, EventArgs e)
        {
            if (this.dataGridCalling.ItemsSource == null || this.dataGridCalling.TotalCount == 0)
            {
                Window.Alert(ResCustomerCalingQuery.Msg_NoData);
                return;
            }
            InitExportFilter();
            ColumnSet col = new ColumnSet(this.dataGridCalling);
            col.SetWidth("OrderSysNo", 15);
            col.SetWidth("LastEditDate", 20);
            col.SetWidth("Status", 15);
            col.SetWidth("LastEditUserName", 20);
            facade.ExportCalling(filter, new ColumnSet[] { col });
        }

        private void dataGridComplain_ExportAllClick(object sender, EventArgs e)
        {
            if (this.dataGridComplain.ItemsSource == null || this.dataGridComplain.TotalCount == 0)
            {
                Window.Alert(ResCustomerCalingQuery.Msg_NoData);
                return;
            }
            InitExportFilter();
            ColumnSet col = new ColumnSet(this.dataGridComplain);
            col.SetWidth("ComplainSysNo", 15);
            col.SetWidth("SOSysNo", 15);
            col.SetWidth("Subject", 100);
            facade.ExportComplainList(filter, new ColumnSet[] { col });
        }

        private void dataGridRMA_ExportAllClick(object sender, EventArgs e)
        {
            if (this.dataGridRMA.ItemsSource == null || this.dataGridRMA.TotalCount == 0)
            {
                Window.Alert(ResCustomerCalingQuery.Msg_NoData);
                return;
            }
            InitExportFilter();
            ColumnSet col = new ColumnSet(this.dataGridRMA);
            col.SetWidth("ProductID", 15);
            col.SetWidth("ProductName", 100);
            facade.ExportRMAList(filter, new ColumnSet[] { col });
        }

        private void dataGridSo_ExportAllClick(object sender, EventArgs e)
        {
            if (this.dataGridSo.ItemsSource == null || this.dataGridSo.TotalCount == 0)
            {
                Window.Alert(ResCustomerCalingQuery.Msg_NoData);
                return;
            }
            InitExportFilter();
            ColumnSet col = new ColumnSet(this.dataGridSo);
            col.Insert(0, "SOID", ResCustomerCalingQuery.Grid_So_SoSysNo, 15);
            col.SetWidth("CustomerID", 20);
            col.SetWidth("IsFPSO", 15);
            col.SetWidth("TotalAmount", 15);
            col.SetWidth("UpdatedMan", 20);
            facade.ExportSOList(filter, new ColumnSet[] { col });
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerCalling_Add))
                this.btnNew.IsEnabled = false;
        }
        #endregion

    }

}
