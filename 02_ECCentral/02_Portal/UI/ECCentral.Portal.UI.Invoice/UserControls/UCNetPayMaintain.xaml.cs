using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Converters;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCNetPayMaintain : PopWindow
    {
        #region Private Fields

        private NetPayMaintainVM netpayVM;
        private NetPayFacade netpayFacede;
        private OtherDomainDataFacade otherDomainFacade;
        private int? netpaySysNO;

        #endregion Private Fields

        #region Constructor

        public UCNetPayMaintain()
        {
            InitializeComponent();
            RegisterEvents();
        }

        public UCNetPayMaintain(int netpaySysNo) :
            this()
        {
            netpaySysNO = netpaySysNo;
        }

        #endregion Constructor

        #region Event Handlers

        private void ucNetPayInfo_SearchOrderButtonClick(object sender, RoutedEventArgs e)
        {
            var payInfo = ucNetPayInfo.DataContext as PayInfoVM;
            if (payInfo != null && !string.IsNullOrEmpty(payInfo.SOSysNo))
            {
                btnSave.IsEnabled = false;
                int soSysNo;
                if (int.TryParse(payInfo.SOSysNo, out soSysNo))
                {
                    otherDomainFacade.GetSOBaseInfo(soSysNo, so =>
                    {
                        if (so == null)
                        {
                            CurrentWindow.Alert(ResUCNetPayMaintain.Message_InvalidSOSysNo);
                            return;
                        }

                        if (so.Status.HasValue)
                        {
                            if (so.Status == SOStatus.Abandon || so.Status == SOStatus.SystemCancel || so.Status == SOStatus.Reject)
                            {
                                CurrentWindow.Alert(ResUCNetPayMaintain.Message_SOStatusIsAbandon);
                                return;
                            }
                        }

                        btnSave.IsEnabled = true;

                        netpayVM.PayInfo = so.ConvertVM<SOBaseInfoVM, PayInfoVM>((s, t) =>
                        {
                            t.PayAmt = Convert.ToString(s.ReceivableAmount ?? 0);
                            t.PrepayAmt = s.PrepayAmount ?? 0;
                            t.ReceivableAmt = s.ReceivableAmount ?? 0;
                            t.SOTotalAmt = s.SOTotalAmount ?? 0;
                            t.GiftCardPayAmt = s.GiftCardPay ?? 0;
                            t.RelatedSOSysNo = s.SOSysNo.ToString();
                        });
                        netpayVM.PayInfo.ValidationErrors.Clear();
                    });
                }
            }
        }

        private void UCNetPayMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCNetPayMaintain_Loaded);
            netpayFacede = new NetPayFacade(CurrentPage);
            otherDomainFacade = new OtherDomainDataFacade(CurrentPage);

            this.ucNetPayInfo.SetColumnWidth(0, 77);
            this.ucRefundInfo.SetColumnWidth(2, 90);

            SetControls();
            NewNetPayDataContext();
            LoadComboBoxData();
            LoadForAudit();
        }

        private void LoadForAudit()
        {
            if (netpaySysNO.HasValue)
            {
                netpayFacede.LoadForAudit(netpaySysNO.Value, vm =>
                {
                    netpayVM = vm;
                    this.LayoutRoot.DataContext = netpayVM;
                    netpayVM.ValidationErrors.Clear();

                    bool canApprove = false;
                    //权限控制
                    if (netpayVM.Source == NetPaySource.Employee)
                    {
                        canApprove = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_NetPay_Approve_EmployeeAdd);
                    }
                    else
                    {
                        canApprove = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_NetPay_Approve_WebSiteAdd);
                    }
                    btnAudit.Visibility = canApprove ? Visibility.Visible : Visibility.Collapsed;
                });
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
            {
                return;
            }
            netpayVM.ValidationErrors.Clear();

            //标识来源为"手动添加"
            netpayVM.Source = NetPaySource.Employee;
            //状态为待审核
            netpayVM.Status = NetPayStatus.Origin;

            netpayFacede.Create(netpayVM, () =>
            {
                AlertInformationDialog(ResUCNetPayMaintain.Message_SaveSuccessful, () => CloseDialog(DialogResultType.OK));
            });
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
            {
                return;
            }
            AlertConfirmDialog(ResUCNetPayMaintain.Message_ConfirmDlgDefaultTitle, ResUCNetPayMaintain.Message_ConfrimAuditContent,
                (args) =>
                {
                    netpayVM.ValidationErrors.Clear();

                    List<int> sysNoList = new List<int>() { netpayVM.SysNo.Value };
                    netpayFacede.BatchAudit(sysNoList, msg =>
                    {
                        AlertInformationDialog(msg, () => CloseDialog(DialogResultType.OK));
                    });
                }, null);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        #endregion Event Handlers

        #region Private Methods

        private void SetControls()
        {
            if (!netpaySysNO.HasValue)
            {
                ucNetPayInfo.IsEnabled = true;
                btnSave.Visibility = Visibility.Visible;
                btnSave.IsEnabled = false;
                btnAudit.Visibility = Visibility.Collapsed;
            }
            else
            {
                ucNetPayInfo.IsEnabled = false;
                btnSave.Visibility = Visibility.Collapsed;
                btnAudit.Visibility = Visibility.Visible;
                chkRefundInfo.IsEnabled = false;
                ucRefundInfo.DisableRefundPayType();
            }
        }

        private void RegisterEvents()
        {
            ucNetPayInfo.SearchOrderButtonClick += new RoutedEventHandler(ucNetPayInfo_SearchOrderButtonClick);
            Loaded += new RoutedEventHandler(UCNetPayMaintain_Loaded);
        }

        private void NewNetPayDataContext()
        {
            netpayVM = new NetPayMaintainVM();
            this.LayoutRoot.DataContext = netpayVM;
        }

        private void LoadComboBoxData()
        {
            if (PayInfoVM.NetPayTypeList == null)
            {
                otherDomainFacade.LoadPayTypeList(payTypeList =>
                {
                    payTypeList.Insert(0, new PayType
                    {
                        PayTypeName = ResCommonEnum.Enum_Select
                    });
                    PayInfoVM.NetPayTypeList = payTypeList;
                    ucNetPayInfo.cbmPayType.ItemsSource = payTypeList;
                    ucNetPayInfo.cbmPayType.SelectedIndex = 0;
                });
            }
            else
            {
                ucNetPayInfo.cbmPayType.ItemsSource = PayInfoVM.NetPayTypeList;
                ucNetPayInfo.cbmPayType.SelectedIndex = 0;
            }
        }

        private bool ValidateData()
        {
            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;

            flag1 = ValidationManager.Validate(this.ucNetPayInfo);
            if (netpayVM.IsForceCheck == true)
            {
                flag2 = ValidationManager.Validate(this.ucRefundInfo);
                if (flag1 && flag2)
                {
                    flag3 = ValidateToleranceAmt();
                }
            }

            return flag1 && flag2 & flag3;
        }

        private bool ValidateToleranceAmt()
        {
            decimal payAmt = decimal.Round(Convert.ToDecimal(netpayVM.PayInfo.PayAmt), 2);
            decimal receivableAmt = decimal.Round(Convert.ToDecimal(netpayVM.PayInfo.ReceivableAmt), 2);
            decimal refundCashAmt = decimal.Round(Convert.ToDecimal(netpayVM.RefundInfo.RefundCashAmt), 2);

            decimal toleranceAmt = payAmt - receivableAmt - refundCashAmt;
            netpayVM.RefundInfo.ToleranceAmt = Math.Abs(toleranceAmt);

            if (Math.Abs(toleranceAmt) > 0.1M)
            {
                AlertErrorDialog(ResUCNetPayMaintain.Message_ToleranceAmtIncorrect);
                return false;
            }
            return true;
        }

        #endregion Private Methods

        private void ucRefundInfo_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (netpaySysNO.HasValue)
            { 
                
            }
        }
    }
}