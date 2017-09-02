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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.Resources;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CSMaintain : UserControl
    {
        public IDialog Dialog { get; set; }

        public CSMaintainVM viewModel;
        public bool IsAdd = true;
        List<ValidationEntity> validationListForCSList;
        List<ValidationEntity> validationListForLeader;
        private int csdeparmentId = 0;
        public CSMaintain()
        {
            InitializeComponent();
            viewModel = new CSMaintainVM();

            if (IsAdd)
                viewModel.csvm.Role = CSRole.CS;
            BuildValidateCondition();
        }

        private void BuildValidateCondition()
        {
            validationListForCSList = new List<ValidationEntity>();
            validationListForLeader = new List<ValidationEntity>();
            validationListForCSList.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.cmbUser.SelectedValue, ResCSSet.ValidationMsg_CSUser));
            validationListForLeader.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.cmbLeader.SelectedValue, ResCSSet.ValidationMsg_Leader));
        }
        private bool isLoaded = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }
            AppSettingHelper.GetSetting("Customer", "CSDeparentID", (obj, arg) =>
            {
                csdeparmentId = int.Parse(arg.Result);
                InitVM();
                isLoaded = true;
                this.DataContext = viewModel;
            });
        }
        private void InitVM()
        {

            //init CSList;
            if (IsAdd)
            {
                new CSFacade().GetCSWithDepartmentId(csdeparmentId, (s, args) =>
                {
                    if (args.Error == null && null != args.Result)
                    {
                        args.Result.ForEach(item => viewModel.CSList.Add(item.Convert<CSInfo, CSVM>()));
                        cmbUser.ItemsSource = viewModel.CSList;
                    }
                });
            }
            else
            {
                viewModel.CSList.Add(new CSVM() { SysNo = viewModel.csvm.IPPUserSysNo, UserName = viewModel.csvm.UserName });
                cmbUser.ItemsSource = viewModel.CSList;
                cmbUser.IsEnabled = false;
                ckbAllDepartment.Visibility = System.Windows.Visibility.Collapsed;
            }
            //init AllCSList
            new CSFacade().GetAllCS((s, args) =>
            {
                if (args.Error == null && null != args.Result)
                {
                    args.Result.ForEach(item => viewModel.AllCSList.Add(item.Convert<CSInfo, CSVM>()));
                }
            });
            //init RoleList
            viewModel.RoleList = EnumConverter.GetKeyValuePairs<CSRole>();
            //init leaderList
            CSQueryFilter queryRequest = new CSQueryFilter();
            queryRequest.Role = CSRole.Leader;
            queryRequest.PagingInfo = new PagingInfo
            {
                PageSize = int.MaxValue,
                PageIndex = 0
            };
            new CSFacade().Query(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                viewModel.LeaderList = DynamicConverter<CSVM>.ConvertToVMList(args.Result.Rows); ;
            });
            //init managerList
            queryRequest = new CSQueryFilter();
            queryRequest.Role = CSRole.Manager;
            queryRequest.PagingInfo = new PagingInfo
            {
                PageSize = int.MaxValue,
                PageIndex = 0
            };
            new CSFacade().Query(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                viewModel.ManagerList = DynamicConverter<CSVM>.ConvertToVMList(args.Result.Rows); ;
            });

        }
        private void Role_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (null != Role.SelectedValue && !string.IsNullOrEmpty(Role.SelectedValue.ToString()))
            {
                CSRole role = (CSRole)Enum.Parse(typeof(CSRole), this.Role.SelectedValue.ToString(), true);
                switch (role)
                {
                    case CSRole.CS:
                        textBlock3.Visibility = System.Windows.Visibility.Collapsed;
                        textBlock4.Visibility = System.Windows.Visibility.Collapsed;
                        cmbManager.Visibility = System.Windows.Visibility.Collapsed;
                        textBlock5.Visibility = System.Windows.Visibility.Visible;
                        cmbLeader.Visibility = System.Windows.Visibility.Visible;
                        Underlings.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case CSRole.Leader:
                        textBlock3.Visibility = System.Windows.Visibility.Visible;
                        textBlock4.Visibility = System.Windows.Visibility.Visible;
                        cmbManager.Visibility = System.Windows.Visibility.Visible;
                        textBlock5.Visibility = System.Windows.Visibility.Collapsed;
                        cmbLeader.Visibility = System.Windows.Visibility.Collapsed;
                        Underlings.Visibility = System.Windows.Visibility.Visible;
                        SetUnderlings();
                        break;
                    case CSRole.Manager:
                        textBlock3.Visibility = System.Windows.Visibility.Collapsed;
                        textBlock4.Visibility = System.Windows.Visibility.Collapsed;
                        cmbManager.Visibility = System.Windows.Visibility.Collapsed;
                        textBlock5.Visibility = System.Windows.Visibility.Collapsed;
                        cmbLeader.Visibility = System.Windows.Visibility.Collapsed;
                        Underlings.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }

            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                if (IsAdd)//添加
                {
                    viewModel.csvm.UserName = (cmbUser.SelectedItem as CSVM).UserName;
                    if (viewModel.csvm.Role.Value == CSRole.Leader)
                    {
                        viewModel.csvm.CSIPPUserSysNos = new List<int>();
                        viewModel.csvm.CSUserNames = new List<string>();
                        foreach (var item in viewModel.CSCheckBoxList)
                        {
                            if (item.IsChecked)
                            {
                                viewModel.csvm.CSIPPUserSysNos.Add(item.SysNo);
                                viewModel.csvm.CSUserNames.Add(item.Name);
                            }
                        }
                    }
                    new CSFacade().Create(viewModel.csvm, (s, args) =>
                        {
                            if (args.Error != null)
                            {
                                CPApplication.Current.CurrentPage.Context.Window.Alert(args.Error.Faults[0].ErrorDescription);
                                return;
                            }
                            Dialog.ResultArgs.Data = args.Result;
                            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            Dialog.Close();
                        });
                }
                else
                {
                    if (viewModel.csvm.Role.Value == CSRole.Leader)
                    {
                        viewModel.csvm.CSIPPUserSysNos = new List<int>();
                        viewModel.csvm.CSUserNames = new List<string>();
                        foreach (var item in viewModel.CSCheckBoxList)
                        {
                            if (item.IsChecked)
                            {
                                viewModel.csvm.CSIPPUserSysNos.Add(item.SysNo);
                                viewModel.csvm.CSUserNames.Add(item.Name);
                            }
                        }
                    }
                    new CSFacade().Update(viewModel.csvm, (s, args) =>
                    {
                        if (args.Error != null)
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert(args.Error.Faults[0].ErrorDescription);
                            return;
                        }
                        Dialog.ResultArgs.Data = args.Result;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    });
                }
            }
        }
        public bool ValidateInput()
        {
            if (viewModel.csvm.Role.Value == CSRole.CS)
            {
                return ValidationHelper.Validation(this.cmbUser, validationListForCSList) && ValidationHelper.Validation(this.cmbLeader, validationListForLeader);
            }
            else
            {
                return ValidationHelper.Validation(this.cmbUser, validationListForCSList);
            }

        }

        private void ckbAllDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (ckbAllDepartment.IsChecked.Value)
                cmbUser.ItemsSource = viewModel.AllCSList;
            else
                cmbUser.ItemsSource = viewModel.CSList;
        }
        private void SetUnderlings()
        {
            if (IsAdd)
            {
                foreach (var item in cmbUser.ItemsSource as List<CSVM>)
                {
                    if (item != cmbUser.SelectedItem)
                        viewModel.CSCheckBoxList.Add(new CSCheckBoxVM() { IsChecked = false, Name = item.UserName, SysNo = item.SysNo.Value });
                }

            }
            else
            {
                if (viewModel.csvm.Role.Value == CSRole.Leader)
                {
                    new CSFacade().GetCSByLeaderSysNo(viewModel.csvm.SysNo.Value, (s, args) =>
                    {
                        if (args.Error == null && null != args.Result)
                        {
                            foreach (var item in args.Result)
                            {
                                viewModel.CSCheckBoxList.Add(new CSCheckBoxVM() { IsChecked = true, Name = item.UserName, SysNo = item.SysNo.Value });

                            }
                        }
                    });
                }
                new CSFacade().GetCSWithDepartmentId(csdeparmentId, (s, args) =>
                {
                    if (args.Error == null && null != args.Result)
                    {
                        foreach (var item in args.Result)
                        {
                            viewModel.CSCheckBoxList.Add(new CSCheckBoxVM() { IsChecked = false, Name = item.UserName, SysNo = item.SysNo.Value });
                        }
                    }
                });

            }
        }
    }
}
