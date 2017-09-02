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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.QueryFilter.Customer;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;

namespace ECCentral.Portal.UI.Customer.UserControls.Calling
{
    public partial class CustomerCallingMaintain : UserControl
    {
        public IDialog Dialog;
        public CustomerCalingMaintainVM viewModel;
        public string Operate;
        private CallsEventsStatus? status;
        private bool needRefulshList = false;
        public CustomerCallingMaintain()
        {
            viewModel = new CustomerCalingMaintainVM();
            this.Loaded += new RoutedEventHandler(CustomerCallingMaintain_Loaded);
            InitializeComponent();
        }

        void CustomerCallingMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = viewModel;
            InitVM();
        }

        private void InitVM()
        {
            CodeNamePairHelper.GetList("Customer", "RecordOrigion", CodeNamePairAppendItemType.Select, (s, arg) =>
            {
                if (arg.FaultsHandle())
                    return;
                foreach (var item in arg.Result)
                {
                    viewModel.RecordOrigionList.Add(item);
                }
            });
            if (viewModel.SysNo != null)
            {
                txpBasicInfo.IsEnabled = false;
                new CustomerCallingFacade(CPApplication.Current.CurrentPage).LoadCallsEvent(viewModel.SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    viewModel.SysNo = args.Result.SysNo;
                    viewModel.CustomerID = args.Result.CustomerID;
                    viewModel.Phone = args.Result.Phone;
                    viewModel.CustomerSysNo = args.Result.CustomerSysNo;
                    viewModel.CustomerName = args.Result.CustomerName;
                    viewModel.Email = args.Result.Email;
                    viewModel.FromLinkSource = args.Result.FromLinkSource;
                    viewModel.Address = args.Result.Address;
                    viewModel.OrderSysNo = args.Result.OrderSysNo.HasValue ? args.Result.OrderSysNo.ToString() : string.Empty;
                    status = args.Result.Status;

                    if (!string.IsNullOrEmpty(Operate))
                    {
                        switch (Operate.ToLower())
                        {
                            case "open":
                                ButtonSave.IsEnabled = status != CallsEventsStatus.Abandoned;
                                break;
                            case "close":
                                cbStatus.IsEnabled = false;
                                viewModel.NewCallingLog.Status = CallsEventsStatus.Handled;
                                ButtonSave.IsEnabled = !(status != null && status == CallsEventsStatus.Handled);
                                break;
                            case "reopen":
                                cbStatus.IsEnabled = false;
                                viewModel.NewCallingLog.Status = CallsEventsStatus.Replied;
                                break;

                        }
                    }
                });
                dataGridLogList.Bind();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ButtonSave.IsEnabled = false;
            ValidationManager.Validate(this.newGrid);
            if (viewModel.NewCallingLog.HasValidationErrors)
            {
                ButtonSave.IsEnabled = true;
                return;
            }
            if (string.IsNullOrEmpty(viewModel.CustomerID) && string.IsNullOrEmpty(viewModel.Phone))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerCallingMaintain.msg_VaildateCustomer);
                ButtonSave.IsEnabled = true;
                return;
            }
            if (viewModel.SysNo == null || viewModel.SysNo <= 0)
            {
                CallsEvents callingEntity = viewModel.ConvertVM<CustomerCalingMaintainVM, CallsEvents>();
                callingEntity.LogList = new List<CallsEventsFollowUpLog>();
                callingEntity.LogList.Add(viewModel.NewCallingLog.ConvertVM<CallingLogVM, CallsEventsFollowUpLog>());
                callingEntity.Status = callingEntity.LogList[0].Status;
                new CustomerCallingFacade(CPApplication.Current.CurrentPage).CreateCallsEvents(callingEntity, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        ButtonSave.IsEnabled = true;
                        return;
                    }
                    ButtonSave.IsEnabled = callingEntity.Status != CallsEventsStatus.Handled;
                    viewModel.NewCallingLog = new CallingLogVM();
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerCallingMaintain.msg_OperateSuccess);
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                });
            }
            else
            {
                CallsEventsFollowUpLog entity = new CallsEventsFollowUpLog();
                entity = viewModel.NewCallingLog.ConvertVM<CallingLogVM, CallsEventsFollowUpLog>();
                entity.CallsEventsSysNo = viewModel.SysNo;
                new CustomerCallingFacade(CPApplication.Current.CurrentPage).CreateCallsEventsFollowUpLog(entity, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        ButtonSave.IsEnabled = true;
                        return;
                    }
                    ButtonSave.IsEnabled = entity.Status != CallsEventsStatus.Handled;
                    viewModel.NewCallingLog = new CallingLogVM();
                    dataGridLogList.Bind();
                    needRefulshList = true;
                });
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            UCReasonCodePicker uc = new UCReasonCodePicker();
            uc.ReasonCodeType = BizEntity.Common.ReasonCodeType.PreSell;

            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResCustomerCallingMaintain.hlb_SlelectReasonCode, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    if (args.Data != null)
                    {
                        var data = (KeyValuePair<string, string>)args.Data;
                        viewModel.NewCallingLog.ReasonCodeSysNo = int.Parse(data.Key);
                        viewModel.NewCallingLog.ReasonCodePath = data.Value;
                    }
                    else
                    {
                        viewModel.NewCallingLog.ReasonCodeSysNo = null;
                        viewModel.NewCallingLog.ReasonCodePath = string.Empty;
                    }
                }
            });
            uc.Dialog = dialog;
        }

        private void BindControl(dynamic customer, string customerID, string phone)
        {
            if (!string.IsNullOrEmpty(customerID))
                viewModel.CustomerID = customerID;
            else
                viewModel.CustomerID = customer.CustomerID;
            if (!string.IsNullOrEmpty(phone))
            {
                viewModel.Phone = phone;
            }
            else
            {
                if (!string.IsNullOrEmpty(customer.CellPhone))
                    viewModel.Phone = customer.CellPhone;
                else
                    viewModel.Phone = customer.Phone;
            }
            viewModel.CustomerSysNo = customer.SysNo;
            viewModel.CustomerName = customer.CustomerName;
            viewModel.Email = customer.Email;
            viewModel.FromLinkSource = customer.FromLinkSource;
            viewModel.Address = customer.DwellAddress;
        }
        private void ClearControl()
        {
            viewModel.CustomerID = string.Empty;
            viewModel.Phone = string.Empty;
            viewModel.CustomerSysNo = null;
            viewModel.CustomerName = string.Empty;
            viewModel.Email = string.Empty;
            viewModel.FromLinkSource = string.Empty;
            viewModel.Address = string.Empty;
        }

        private void dataGridLogList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            CustomerCallsEventLogFilter filter = new CustomerCallsEventLogFilter();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.CallsEventsSysNo = viewModel.SysNo;
            filter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            new CustomerCallingFacade(CPApplication.Current.CurrentPage).QueryCallsEventLog(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                dataGridLogList.ItemsSource = args.Result.Rows;
                dataGridLogList.TotalCount = args.Result.TotalCount;
            });
        }

        private void hlbSoSysNo_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, viewModel.OrderSysNo), null, true);
        }

        private string soSysNo = string.Empty;

        private void tbOrderSysNo_GotFocus(object sender, RoutedEventArgs e)
        {
            soSysNo = tbOrderSysNo.Text.Trim();
        }

        private void tbOrderSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            List<ValidationEntity> list = new List<ValidationEntity>();
            list.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResCustomerCallingMaintain.msg_InputOrderSysNo));
            list.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"\d{3}", ResCustomerCallingMaintain.msg_InputOrderSysNo));
            if (!ValidationHelper.Validation(this.tbOrderSysNo, list))
                return;
            if (!string.IsNullOrEmpty(tbOrderSysNo.Text.Trim()) && soSysNo != tbOrderSysNo.Text.Trim())
            {
                GetCustomerBySoSysNo();
            }
        }


        private void hlbQueryBySoSysNo_Click(object sender, RoutedEventArgs e)
        {
            List<ValidationEntity> list = new List<ValidationEntity>();
            list.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResCustomerCallingMaintain.msg_InputOrderSysNo));
            list.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"\d{3}", ResCustomerCallingMaintain.msg_InputOrderSysNo));
            if (!ValidationHelper.Validation(this.tbOrderSysNo, list))
                return;
            if (!string.IsNullOrEmpty(tbOrderSysNo.Text.Trim()) && soSysNo != tbOrderSysNo.Text.Trim())
            {
                GetCustomerBySoSysNo();
            }
        }
        private void GetCustomerBySoSysNo()
        {

            new CustomerPointsAddQueryFacade(CPApplication.Current.CurrentPage).QuerySO(int.Parse(tbOrderSysNo.Text.Trim()),string.Empty, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                if (args.Result != null && args.Result.BaseInfo != null && args.Result.BaseInfo.CustomerSysNo != null)
                {
                    new ECCentral.Portal.UI.Customer.Facades.CustomerFacade(CPApplication.Current.CurrentPage).GetCustomerBySysNo(args.Result.BaseInfo.CustomerSysNo.Value, (obj2, argss) =>
                    {
                        viewModel.CustomerID = argss.Result.BasicInfo.CustomerID;
                        if (string.IsNullOrEmpty(argss.Result.BasicInfo.CellPhone))
                            viewModel.Phone = argss.Result.BasicInfo.Phone;
                        else
                            viewModel.Phone = argss.Result.BasicInfo.CellPhone;
                        viewModel.CustomerSysNo = argss.Result.SysNo;
                        viewModel.CustomerName = argss.Result.BasicInfo.CustomerName;
                        viewModel.Email = argss.Result.BasicInfo.Email;
                        viewModel.FromLinkSource = argss.Result.BasicInfo.FromLinkSource;
                        viewModel.Address = argss.Result.BasicInfo.DwellAddress;
                    });
                }
                else
                {
                    ClearControl();
                    viewModel.OrderSysNo = string.Empty;
                }
            });
        }

        private void tbCustomerID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            LoadCustomerByCustomerID();
        }
        private void hlbQueryByCustomerID_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomerByCustomerID();
        }
        private void LoadCustomerByCustomerID()
        {

            List<ValidationEntity> list = new List<ValidationEntity>();
            list.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, "", ResCustomerCallingMaintain.msg_InputCustomerID));
            if (!ValidationHelper.Validation(this.tbCustomerID, list))
                return;
            viewModel.CustomerID = this.tbCustomerID.Text.Trim();
            CustomerQueryFilter filter = new CustomerQueryFilter();
            filter.CustomerID = viewModel.CustomerID;
            filter.PagingInfo = new PagingInfo() { PageIndex = 0, PageSize = int.MaxValue };
            new CustomerQueryFacade(CPApplication.Current.CurrentPage).QueryCustomer(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                if (args.Result != null && args.Result.TotalCount != 0)
                {
                    if (args.Result.TotalCount > 1)//>1的话就让用户选择具体的用户
                    {
                        UCCustomerSearch ucCustomerSearch = new UCCustomerSearch();
                        ucCustomerSearch.SelectionMode = SelectionMode.Single;
                        ucCustomerSearch.DialogHandle = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResCustomerPicker.Dialog_Title, ucCustomerSearch, (param, dialogRes) =>
                        {
                            if (dialogRes.DialogResult == DialogResultType.OK)
                            {
                                BindControl(dialogRes.Data, viewModel.CustomerID, string.Empty);
                            }
                        });
                        ucCustomerSearch._viewModel.CustomerID = viewModel.CustomerID;
                        ucCustomerSearch.BindDataGrid(1, null);
                    }
                    else
                    {
                        BindControl(args.Result.Rows[0], viewModel.CustomerID, string.Empty);
                    }
                }
                else
                    ClearControl();
            });
        }
        private void tbCustomerPhone_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            LoadCustomerByPhone();
        }

        private void hlbQueryByPhone_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomerByPhone();
        }

        private void LoadCustomerByPhone()
        {
            viewModel.Phone = tbCustomerPhone.Text.Trim();
            List<ValidationEntity> list = new List<ValidationEntity>();
            list.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"\d{7}", string.Format(ResCustomerCallingMaintain.msg_VaildatePhone, viewModel.Phone.Length)));
            if (!ValidationHelper.Validation(this.tbCustomerPhone, list))
                return;

            CustomerQueryFilter filter = new CustomerQueryFilter();
            filter.Phone = viewModel.Phone;
            filter.PagingInfo = new PagingInfo() { PageIndex = 0, PageSize = int.MaxValue };
            new CustomerQueryFacade(CPApplication.Current.CurrentPage).QueryCustomer(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                if (args.Result != null && args.Result.TotalCount != 0)
                {
                    if (args.Result.TotalCount > 1)//>1的话就让用户选择具体的用户
                    {
                        UCCustomerSearch ucCustomerSearch = new UCCustomerSearch();
                        ucCustomerSearch.SelectionMode = SelectionMode.Single;
                        ucCustomerSearch.DialogHandle = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(ResCustomerPicker.Dialog_Title, ucCustomerSearch, (param, dialogRes) =>
                        {
                            if (dialogRes.DialogResult == DialogResultType.OK)
                            {
                                BindControl(dialogRes.Data, string.Empty, viewModel.Phone);
                            }
                        });
                        ucCustomerSearch._viewModel.CustomerPhone = viewModel.Phone;
                        ucCustomerSearch.BindDataGrid(1, null);
                    }
                    else
                    {
                        BindControl(args.Result.Rows[0], string.Empty, viewModel.Phone);
                    }
                }
                else
                    ClearControl();
            });
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (needRefulshList)
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            else
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close();
        }

    }

}
