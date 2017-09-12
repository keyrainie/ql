using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.UserControls.Customer;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.BizEntity.Enum.Resources;


namespace ECCentral.Portal.UI.Customer.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CustomerMaintain : PageBase
    {
        private CustomerFacade facade;
        private List<CodeNamePair> customerCompanyList;

        CustomerVM vm;
        public CustomerMaintain()
        {
            vm = new CustomerVM();
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            CodeNamePairHelper.GetList(ConstValue.DomainName_Customer, "CompanyCustomer",
             (obj, args) =>
             {
                 if (!args.FaultsHandle() && args.Result != null)
                 {
                     customerCompanyList = args.Result;
                 }
             });

            facade = new CustomerFacade(this);
            vm = new CustomerVM();

            string customerSysNo = this.Request.Param;

            if (!string.IsNullOrEmpty(customerSysNo))
            {
                AppSettingHelper.GetSetting("Customer", "AvtarImageBasePath", (obj1, args1) =>
                {
                    int temp = 0;
                    if (int.TryParse(customerSysNo, out temp))
                    {
                        facade.GetCustomerBySysNo(temp, (obj, args) =>
                        {

                            if (args.FaultsHandle())
                                return;
                            if (args.Result == null)
                            {
                                Window.Alert(ResCustomerMaintain.Info_CustomerNoFound);
                                return;
                            }
                            if (args.Result != null)
                            {
                                vm = args.Result.Convert<CustomerInfo, CustomerVM>();

                                vm.BasicInfo = args.Result.BasicInfo.Convert<ECCentral.BizEntity.Customer.CustomerBasicInfo, CustomerBasicVM>();
                                vm.BasicInfo.CustomerSysNo = args.Result.SysNo;
                                vm.BasicInfo.Rank = args.Result.Rank;
                                vm.BasicInfo.CustomersType = args.Result.CustomersType;
                                vm.BasicInfo.IsBadCustomer = args.Result.IsBadCustomer;
                                vm.BasicInfo.LastLoginDate = args.Result.LastLoginDate;
                                vm.BasicInfo.DwellAreaSysNo = args.Result.BasicInfo.DwellAreaSysNo == 0 ? null : args.Result.BasicInfo.DwellAreaSysNo.ToString();
                                if (!vm.BasicInfo.IsEmailConfirmed.Value)
                                { this.btnCancelConfirmEmail.IsEnabled = false; }
                                else
                                { this.ucDetailInfo.txt_email.IsEnabled = false; }
                                if (!vm.BasicInfo.CellPhoneConfirmed.Value)
                                { this.btnCancelConfirmPhone.IsEnabled = false; }
                                else
                                { this.ucDetailInfo.txt_cellphone.IsEnabled = false; }

                                vm.ExperienceInfo.CustomerSysNo = vm.SysNo;
                                vm.ExperienceInfo.TotalSOMoney = Convert.ToInt32(Math.Round(args.Result.TotalSOMoney.Value).ToString());

                                vm.ScoreInfo = args.Result.Convert<CustomerInfo, ScoreVM>();
                                vm.ScoreInfo.CustomerSysNo = args.Result.SysNo;
                                vm.ScoreInfo.CustomerName = args.Result.BasicInfo.CustomerName;
                                vm.ScoreInfo.CardNo = args.Result.VipCardNo;
                                vm.BasicInfo.CompanyCustomer = args.Result.BasicInfo.CompanyCustomer.Value;
                                vm.BasicInfo.CompanyCustomers = this.customerCompanyList;

                                if (vm.AgentInfo == null || !vm.AgentInfo.CustomerSysNo.HasValue)
                                {
                                    vm.AgentInfo = new AgentInfoVM
                                    {
                                        CustomerSysNo = vm.SysNo
                                    };
                                }
                                vm.IsEdit = true;

                                vm.BasicInfo.OriginalIsBadCustomer = vm.BasicInfo.IsBadCustomer;
                                //清空备注的Required验证信息
                                vm.ExperienceInfo.ValidationErrors.Clear();
                                if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_LookCustomerPwd))
                                {
                                    vm.BasicInfo.Pwd = "******";
                                }
                                vm.BasicInfo.AvtarImageBasePath = args1.Result;
                                CommonDataFacade commonFacade = new CommonDataFacade(this);
                                commonFacade.GetWebChannelList(true, (sender2, e2) =>
                                {
                                    var data = new List<WebChannelVM>();
                                    data.AddRange(e2.Result);
                                    vm.BasicInfo.WebChannelList = data;
                                    this.DataContext = vm;
                                });
                            }
                        });
                    }
                });
            }
            else
            {
                vm.BasicInfo.Status = CustomerStatus.InValid;
                vm.BasicInfo.CompanyCustomers = this.customerCompanyList;    
            }
            CheckRights();

            facade.GetSociety(obj =>
            {
                vm.BasicInfo.Societies = obj;
                this.DataContext = vm;
            });

            this.UCShippingAddressInfo.OnShipingAddressUpdated += new EventHandler(UCShippingAddressInfo_OnShipingAddressUpdated);
            this.UCValueAddedTaxInvoice.OnVATUpdated += new EventHandler(UCValueAddedTaxInvoice_OnVATUpdated);
        }

        private void btnAddVAT_Click(object sender, RoutedEventArgs e)
        {
            CustomerVM vm = this.DataContext as CustomerVM;
            ValueAddedTaxInvoiceDetail uc = new ValueAddedTaxInvoiceDetail();
            IDialog dialog = Window.ShowDialog(ResCustomerMaintain.Title_ValueAddedTaxInfo, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var vat = args.Data as ValueAddedTaxInfoVM;
                    vat.CustomerSysNo = vm.SysNo;
                    var original = vm.ValueAddedTaxInfoList.FirstOrDefault(p => p.SysNo == vat.SysNo);
                    if (original == null)
                    {
                        facade.CreateValueAddedTaxInfo(vat, (result) =>
                        {
                            //默认的话把其它默认清空
                            if (result.IsDefault.Value)
                            {
                                vm.ValueAddedTaxInfoList.ForEach(item => { item.IsDefault = false; });
                            }
                            vm.ValueAddedTaxInfoList.Add(result);
                        });
                    }
                    else
                    {
                        int index = vm.ValueAddedTaxInfoList.IndexOf(original);
                        vm.ValueAddedTaxInfoList.RemoveAt(index);
                        vm.ValueAddedTaxInfoList.Insert(index, vat);
                    }
                }
            });
            uc.Dialog = dialog;
        }

        private void btnAddShippingAddress_Click(object sender, RoutedEventArgs e)
        {
            CustomerVM vm = this.DataContext as CustomerVM;
            ShippingAddressInfoDetail shipping = new ShippingAddressInfoDetail();
            IDialog dialog = Window.ShowDialog(ResCustomerMaintain.Title_ShippingAddress, shipping, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var shippingAddress = args.Data as ShippingAddressVM;
                    shippingAddress.CustomerSysNo = vm.SysNo;
                    var original = vm.ShippingAddressList.FirstOrDefault(p => p.SysNo == shippingAddress.SysNo);
                    if (original == null)
                    {
                        facade.CreateShippingAddress(shippingAddress, (result) =>
                        {
                            //默认的话把其它默认清空
                            if (result.IsDefault.Value)
                            {
                                vm.ShippingAddressList.ForEach(item => { item.IsDefault = false; });
                            }
                            vm.ShippingAddressList.Add(result);
                        });
                    }
                    else
                    {
                        int index = vm.ShippingAddressList.IndexOf(original);
                        vm.ShippingAddressList.RemoveAt(index);
                        vm.ShippingAddressList.Insert(index, shippingAddress);
                    }
                }
            });
            shipping.Dialog = dialog;
        }
        /// <summary>
        /// 编辑收货地址时触发，重新绑定地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UCShippingAddressInfo_OnShipingAddressUpdated(object sender, EventArgs e)
        {
            facade.GetCustomerBySysNo(vm.SysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                vm.ShippingAddressList.Clear();
                args.Result.ShippingAddressList.ForEach(item =>
                {
                    vm.ShippingAddressList.Add(item.Convert<ECCentral.BizEntity.Customer.ShippingAddressInfo, ShippingAddressVM>());
                });
            });

        }
        /// <summary>
        /// 编辑增值税的时候触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UCValueAddedTaxInvoice_OnVATUpdated(object sender, EventArgs e)
        {
            facade.GetCustomerBySysNo(vm.SysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                vm.ValueAddedTaxInfoList.Clear();
                args.Result.ValueAddedTaxInfoList.ForEach(item =>
                {
                    vm.ValueAddedTaxInfoList.Add(item.Convert<ECCentral.BizEntity.Customer.ValueAddedTaxInfo, ValueAddedTaxInfoVM>());
                });
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            CustomerVM vm = this.DataContext as CustomerVM;
            ValidationManager.Validate(this.gridCustomerInfo);
            bool flag1 = true;

            if (chkAgent.IsChecked.Value)
            {
                flag1 = ValidationManager.Validate(this.expAgentInfo);
            }

            if (vm.BasicInfo.ValidationErrors.Count == 0
                && vm.ScoreInfo.ValidationErrors.Count == 0
                && flag1)
            {
                if (vm.SysNo > 0)
                {
                    facade.UpdateCustomer(vm, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        Window.Alert(ResCustomerMaintain.Info_SaveSuccessfully);
                    });
                }
                else
                {
                    facade.CreateCustomer(vm, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResCustomerMaintain.Info_SaveSuccessfully);
                        Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, args.Result.SysNo), null, false);
                    });
                }
            }
        }

        private void btnPointHistory_Click(object sender, RoutedEventArgs e)
        {
            CustomerVM vm = this.DataContext as CustomerVM;
            Window.Navigate(string.Format(ConstValue.CustomerPointLogQueryUrlFormat, vm.SysNo), null, true);
        }



        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            CustomerVM vm = new CustomerVM();
            vm.IsEdit = false;
            vm.BasicInfo.Status = CustomerStatus.InValid;
            this.expAgentInfo.IsExpanded = false;
            this.DataContext = vm;
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Abandon))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.rightMsg_NoRight_Abandon);
                return;
            }
            CustomerBasicVM basicVM = this.ucBasicInfo.DataContext as CustomerBasicVM;

            facade.VoidCustomer(basicVM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                basicVM.Status = CustomerStatus.InValid;

                Window.Alert(ResCustomerMaintain.Info_VoidSuccessfully);
            });
        }

        private void btnRights_Click(object sender, RoutedEventArgs e)
        {
            CustomerRightMaintain dilog = new CustomerRightMaintain();
            dilog.viewModel.CustomerSysNo = vm.SysNo.Value;
            dilog.Dialog = Window.ShowDialog(ResCustomerMaintain.Title_CustomerRight, dilog, null, new Size(400, 380));
        }

        private void btnViewCouponHistory_Click(object sender, RoutedEventArgs e)
        {
            CustomerVM vm = this.DataContext as CustomerVM;
            Window.Navigate(string.Format("{0}?CustomerSysNo={1}", ConstValue.MKT_CouponCodeCustomerLogMaintainUrlFormat, vm.SysNo), null, true);
        }

        private void btnShowBadUserHistory_Click(object sender, RoutedEventArgs e)
        {
            CustomerVM vm = this.DataContext as CustomerVM;
            Window.ShowDialog(ResCustomerMaintain.Title_MaliceUserQuery, string.Format(ConstValue.MaliceUserQueryUrlFormat, vm.SysNo), null, new Size(760, 400));
        }

        private void btnAddPoint_Click(object sender, RoutedEventArgs e)
        {
            CustomerVM vm = this.DataContext as CustomerVM;
            Window.Navigate(ConstValue.CustomerPointsAddQuery + "/" + vm.BasicInfo.CustomerSysNo, true);
        }

        private void btnCancelConfirmEmail_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CancelConfirmEmail))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.rightMsg_NoRight_CancelConfirmEmail);
                return;
            }
            CustomerBasicVM basicVM = this.ucBasicInfo.DataContext as CustomerBasicVM;
            facade.CancelConfrimEmail(basicVM, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    basicVM.IsEmailConfirmed = false;
                    this.btnCancelConfirmEmail.IsEnabled = false;
                    this.ucDetailInfo.txt_email.IsEnabled = true;
                    Window.Alert(ResCustomerMaintain.Info_CancelConfirm);
                });
        }

        private void btnCancelConfirmPhone_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CancelConfirmPhone))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResCustomerMaintain.rightMsg_NoRight_CancelConfirmPhone);
                return;
            }
            CustomerBasicVM basicVM = this.ucBasicInfo.DataContext as CustomerBasicVM;
            facade.CancelConfrimPhone(basicVM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                basicVM.CellPhoneConfirmed = false;
                this.btnCancelConfirmPhone.IsEnabled = false;
                this.ucDetailInfo.txt_cellphone.IsEnabled = true;
                Window.Alert(ResCustomerMaintain.Info_CancelConfirm);
            });
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_Add))
                this.btnNew.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_Save))
                this.btnSave.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_AbandonCustomer))
                this.btnVoid.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_AddPoint))
                this.btnAddPoint.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_SetCustomerRights))
                this.btnRights.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_ViewPointHistory))
                this.btnPointHistory.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_ViewBadUserHistory))
                this.btnShowBadUserHistory.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_ViewCouponHistory))
                this.btnViewCouponHistory.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_CancelConfirmEmail))
                this.btnCancelConfirmEmail.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerMaintain_CancelConfirmPhone))
                this.btnCancelConfirmPhone.IsEnabled = false;
        }
        #endregion
    }
}
