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
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class VendorFinanceInfoMaintain : UserControl
    {
        public VendorInfoVM editVM;
        public bool isNewFinanceInfo;
        VendorFacade serviceFacade;
        public VendorPayTermsFacade payTermsServiceFacade;

        public IDialog Dialog { get; set; }
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }


        public VendorFinanceInfoMaintain(VendorInfoVM vendorInfoVM, bool isEdit)
        {
            isNewFinanceInfo = isEdit;
            this.editVM = vendorInfoVM.DeepCopy();// Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<VendorInfoVM>(vendorInfoVM);
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(VendorFinanceInfoMaintain_Loaded);
        }

        void VendorFinanceInfoMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(VendorFinanceInfoMaintain_Loaded);
            payTermsServiceFacade = new VendorPayTermsFacade(CPApplication.Current.CurrentPage);
            serviceFacade = new VendorFacade(CPApplication.Current.CurrentPage);

            if (editVM.VendorBasicInfo.ConsignFlag != VendorConsignFlag.Consign)
            {
                this.lblettlePeriodType.Visibility = Visibility.Collapsed;
                this.cmbSettlePeriodTypeForModify.Visibility = Visibility.Collapsed;
                this.lblAutoAudit.Visibility = Visibility.Visible;
                this.chkAutoAudit.Visibility = Visibility.Visible;
            }

            //财务 - 账期类型(调用Invoice接口获取LIST):
            payTermsServiceFacade.QueryVendorPayTermsList(CPApplication.Current.CompanyCode, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                //this.cmbPayPeriodType.ItemsSource = args.Result.Where(i => i.IsConsignment == (int)editVM.VendorBasicInfo.ConsignFlag).OrderBy(j => j.PayTermsNo).ToList();
                List<VendorPayTermsItemInfo> listItems = args.Result.Where(i => i.IsConsignment == (int)editVM.VendorBasicInfo.ConsignFlag).OrderBy(j => j.PayTermsNo).ToList();
                listItems.RemoveAll(item => item.PayTermsNo != 19);//只留账期类型为月结的
                this.cmbPayPeriodType.ItemsSource = listItems;


                //如果是新申请财务信息:
                if (this.isNewFinanceInfo)
                {
                    // 新申请财务默认合作金额10万，有效期1年
                    editVM.VendorFinanceInfo.CooperateAmt = "100000";
                    editVM.VendorFinanceInfo.CooperateValidDate = DateTime.Now.Date;
                    editVM.VendorFinanceInfo.CooperateExpiredDate = DateTime.Now.Date.AddYears(1);

                    this.btnRequestForApproval.Visibility = Visibility.Visible;
                }
                //如果是审核已有的财务信息 ：
                else
                {
                    this.editVM.VendorFinanceInfo.PayPeriodType = this.editVM.VendorFinanceInfo.FinanceRequestInfo.PayPeriodType;
                    this.editVM.VendorFinanceInfo.SettlePeriodType = this.editVM.VendorFinanceInfo.FinanceRequestInfo.SettlePeriodType;
                    this.editVM.VendorFinanceInfo.CooperateAmt = this.editVM.VendorFinanceInfo.FinanceRequestInfo.ContractAmt.HasValue ? this.editVM.VendorFinanceInfo.FinanceRequestInfo.ContractAmt.Value.ToString() : string.Empty;
                    this.editVM.VendorFinanceInfo.CooperateValidDate = this.editVM.VendorFinanceInfo.FinanceRequestInfo.ValidDate;
                    this.editVM.VendorFinanceInfo.CooperateExpiredDate = this.editVM.VendorFinanceInfo.FinanceRequestInfo.ExpiredDate;
                    this.editVM.VendorBasicInfo.VendorNameLocal = this.editVM.VendorFinanceInfo.FinanceRequestInfo.VendorName;
                    this.editVM.VendorFinanceInfo.IsAutoAudit = this.editVM.VendorFinanceInfo.FinanceRequestInfo.AutoAudit;
                    this.btnCancelAudit.Visibility = Visibility.Visible;
                    this.btnDeclineAudit.Visibility = Visibility.Visible;
                    this.btnPassAudit.Visibility = Visibility.Visible;
                    this.lblMemo.Visibility = Visibility.Visible;
                    this.txtMemo.Visibility = Visibility.Visible;

                    this.cmbPayPeriodType.IsEnabled = false;
                    this.cmbSettlePeriodTypeForModify.IsEnabled = false;
                    this.txtContractAmt.IsReadOnly = true;
                    this.txtVendorName.IsReadOnly = true;
                    this.dpContractDateFrom.IsEnabled = false;
                    this.dpContractDateTo.IsEnabled = false;
                }
                this.DataContext = editVM;
                LoadComboBoxData();
                SetAccessControl();
                editVM.ValidationErrors.Clear();
            });
        }

        private void SetAccessControl()
        {
            //提交审核,取消审核财务信息：
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_RequestVendor))
            {
                this.btnRequestForApproval.IsEnabled = false;
                this.btnCancelAudit.IsEnabled = false;
            }
            //审核通过，审核未通过财务信息：
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_VerifyVendor))
            {
                this.btnPassAudit.IsEnabled = false;
                this.btnDeclineAudit.IsEnabled = false;
            }
        }

        private void LoadComboBoxData()
        {
            this.cmbSettlePeriodTypeForModify.ItemsSource = EnumConverter.GetKeyValuePairs<VendorSettlePeriodType>(EnumConverter.EnumAppendItemType.None);
        }

        #region [Events]
        private void btnRequestForApproval_Click(object sender, RoutedEventArgs e)
        {
            if (!this.editVM.VendorFinanceInfo.CooperateValidDate.HasValue || !this.editVM.VendorFinanceInfo.CooperateExpiredDate.HasValue)
            {
                CurrentWindow.Alert(ResVendorMaintain.Msg_CooperateDateNull);
                return;
            }
            if (!ValidationManager.Validate(LayoutRoot))
            {
                return;
            }
            //提交审核:
            CurrentWindow.Confirm(ResVendorMaintain.InfoMsg_ConfirmSumitAudit, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorModifyRequestInfo info = new VendorModifyRequestInfo()
                    {
                        VendorSysNo = editVM.SysNo.Value,
                        ValidDate = editVM.VendorFinanceInfo.CooperateValidDate,
                        ExpiredDate = editVM.VendorFinanceInfo.CooperateExpiredDate,
                        ContractAmt = Convert.ToDecimal(editVM.VendorFinanceInfo.CooperateAmt),
                        VendorName = editVM.VendorBasicInfo.VendorNameLocal,
                        AutoAudit = editVM.VendorFinanceInfo.IsAutoAudit,
                        ActionType = VendorModifyActionType.Edit,
                        RequestType = VendorModifyRequestType.Finance,
                        Memo = this.txtMemo.Text,
                        PayPeriodType = new VendorPayTermsItemInfo()
                        {
                            PayTermsNo = editVM.VendorFinanceInfo.PayPeriodType.PayTermsNo
                        },
                        SettlePeriodType = editVM.VendorFinanceInfo.SettlePeriodType
                    };
                    serviceFacade.RequestForApprovalVendorFinanceInfo(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        CurrentWindow.Alert(ResVendorMaintain.AlertMsg_AlertTitle, ResVendorMaintain.InfoMsg_SumitAuditSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Dialog.Close();
                               // CurrentWindow.Refresh();
                            }
                        });
                    });
                }
            });
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            //取消审核:
            CurrentWindow.Confirm(ResVendorMaintain.InfoMsg_ConfirmCancelAudit, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorModifyRequestInfo info = EntityConverter<VendorModifyRequestInfoVM, VendorModifyRequestInfo>.Convert(editVM.VendorFinanceInfo.FinanceRequestInfo);
                    serviceFacade.WithDrawVendorFinanceInfo(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        CurrentWindow.Alert(ResVendorMaintain.AlertMsg_AlertTitle, ResVendorMaintain.InfoMsg_CancelAuditSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Dialog.Close();
                                //CurrentWindow.Refresh();
                            }
                        });
                    });
                }
            });
        }

        private void btnPassAudit_Click(object sender, RoutedEventArgs e)
        {
            //审核通过:
            CurrentWindow.Confirm(ResVendorMaintain.InfoMsg_ConfirmAuditPass, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorModifyRequestInfo info = EntityConverter<VendorModifyRequestInfoVM, VendorModifyRequestInfo>.Convert(editVM.VendorFinanceInfo.FinanceRequestInfo);
                    serviceFacade.ApproveVendorFinanceInfo(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        CurrentWindow.Alert(ResVendorMaintain.AlertMsg_AlertTitle, ResVendorMaintain.InfoMsg_AuditPassSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Dialog.Close();
                                //CurrentWindow.Refresh();
                            }
                        });
                    });
                }
            });
        }

        private void btnDeclineAudit_Click(object sender, RoutedEventArgs e)
        {
            //审核未通过:
            CurrentWindow.Confirm(ResVendorMaintain.InfoMsg_ConfirmAuditFail, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorModifyRequestInfo info = EntityConverter<VendorModifyRequestInfoVM, VendorModifyRequestInfo>.Convert(editVM.VendorFinanceInfo.FinanceRequestInfo, (s, t) =>
                    {
                        t.Memo = this.txtMemo.Text;

                    });
                    serviceFacade.DeclineVendorFinanceInfo(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        CurrentWindow.Alert(ResVendorMaintain.AlertMsg_AlertTitle, ResVendorMaintain.InfoMsg_AuditFailSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Dialog.Close();
                               //CurrentWindow.Refresh();
                            }
                        });

                    });
                }
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //关闭:
            this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.Dialog.Close(true);
        }

        #endregion

        private void cmbPayPeriodType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VendorPayTermsItemInfo getSelectedPayTerms = this.cmbPayPeriodType.SelectedItem as VendorPayTermsItemInfo;
            if (null != getSelectedPayTerms)
            {
                this.lblSettlePeriodTypeDesc.Text = getSelectedPayTerms.DiscribComputer.Replace("</br>", Environment.NewLine);
                if (getSelectedPayTerms.PayTermsNo == 29 || getSelectedPayTerms.PayTermsNo == 30 || getSelectedPayTerms.PayTermsNo == 31)
                {
                    if (isNewFinanceInfo)
                    {
                        this.chkAutoAudit.IsEnabled = true;
                    }
                }
                else
                {
                    if (isNewFinanceInfo)
                    {
                        this.chkAutoAudit.IsEnabled = false;
                        this.editVM.VendorFinanceInfo.IsAutoAudit = false;
                    }
                }
            }
        }

    }
}
