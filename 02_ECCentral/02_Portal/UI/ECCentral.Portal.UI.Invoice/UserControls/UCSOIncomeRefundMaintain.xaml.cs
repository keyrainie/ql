using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCSOIncomeRefundMaintain : PopWindow
    {
        private SOIncomeRefundMaintainVM currentRefund;
        private AuditRefundFacade auditRefundFacade;

        private UCSOIncomeRefundMaintain()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCSOIncomeRefundEdit_Loaded);
            ucRefund.HideRefundCashAmtAndToleranceAmt();
            ucRefund.SetColumnWidth(0, 77);
            ucRefund.SetColumnWidth(2, 53);
        }

        public UCSOIncomeRefundMaintain(AuditRefundVM refund, AuditRefundFacade facade)
            : this()
        {
            auditRefundFacade = facade;
            currentRefund = EntityConverter<AuditRefundVM, SOIncomeRefundMaintainVM>.Convert(refund);
            currentRefund.RefundInfo = EntityConverter<AuditRefundVM, RefundInfoVM>.Convert(refund, (s, t) =>
            {
                t.RefundMemo = s.Note;
            });
        }

        private void UCSOIncomeRefundEdit_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitData();
            LoadComboBoxData();

            SetControlStatus();
        }

        //private void VerifyPermissions()
        //{
        //    this.btnAuditAutoRMA.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_AuditAutoRMA);
           
        //}

        private void LoadComboBoxData()
        {
            //绑定退款原因
            new OtherDomainDataFacade(CurrentPage).GetRefundReaons(true, (obj, args) =>
                {
                    currentRefund.RefundReasonList = args.Result;
                });
        }

        private void InitData()
        {
            this.LayoutRoot.DataContext = currentRefund;
        }

        //验证权限，设置控件状态
        private void SetControlStatus()
        {
            //如是是RMA物流拒收且为待RMA退款状态，则显示“审核RMA物流拒收”按钮
            if (currentRefund.ShipRejected == true 
                && currentRefund.AuditStatus == RefundStatus.WaitingRefund
                && AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_AuditAutoRMA))
            {
                btnAuditAutoRMA.Visibility = Visibility.Visible;
            }
            else
            {
                btnAuditAutoRMA.Visibility = Visibility.Collapsed;
            }

            var hasUpdateRight = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_UpdateRefundPayTypeAndReason);
            var rule = hasUpdateRight && (currentRefund.OrderType == RefundOrderType.AO || currentRefund.OrderType == RefundOrderType.OverPayment);
            var auditAndBank = (currentRefund.AuditStatus == RefundStatus.Audit && currentRefund.RefundInfo.RefundPayType == RefundPayType.BankRefund);

            //规则：如果退款类型为“银行转帐”并且状态为“审核通过”，那么不能编辑“退款类型”和“退款原因”

            //如果单据类型是AO或多付款退款，或者是"物流拒收"，则可以编辑“退款类型”
            if (!(rule || currentRefund.ShipRejected == true) || auditAndBank)
            {
                ucRefund.DisableRefundPayType();
            }
            //如果单据类型是AO或多付款退款，则可以编辑“退款原因”
            cmbRefundReason.IsEnabled = !(!rule || auditAndBank);

            //如果是退款调整单，则退款单编号的超链接不可点击
            if (currentRefund.OrderType == RefundOrderType.RO_Balance)
            {
                tbRMANumber.Visibility = Visibility.Visible;
                Hyperlink_RMANumber.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbRMANumber.Visibility = Visibility.Collapsed;
                Hyperlink_RMANumber.Visibility = Visibility.Visible;
            }
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.LayoutRoot);
            if (flag)
            {
                //原来需要把追加财务备注传到服务端进行组装，现在直接在客户端组装好后再传到服务端
                if (currentRefund.FinAppendNote.Length + currentRefund.FinNote.Length > 500)
                {
                    currentRefund.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(
                        ResUCSOIncomeRefundMaintain.Message_FinNoteMsgLengthMoreThan500, new string[] { "FinAppendNote" }));
                    tbFinAppendNote.Focus();
                    return;
                }
                //新的财务备注和原来的财务备注之间用分号；隔开
                string finNote = string.Concat(currentRefund.FinNote, ";", currentRefund.FinAppendNote).TrimStart(';').TrimEnd(';');
                currentRefund.FinNote = finNote;

                auditRefundFacade.Update(currentRefund, () => CloseDialog(DialogResultType.OK));
            }
        }

        private void btnAuditAutoRMA_Click(object sender, RoutedEventArgs e)
        {
           

            AlertConfirmDialog(ResUCSOIncomeRefundMaintain.Message_ConfirmAuditAutoRMADlgTitle, data =>
            {
                auditRefundFacade.AuditAutoRMA(currentRefund.SysNo.Value, () => CloseDialog(DialogResultType.OK));
            });
        }

        private void Hyperlink_SOSysNo_Click(object sender, RoutedEventArgs e)
        {
            //跳转到订单维护页面
            CurrentWindow.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, currentRefund.SOSysNo), null, true);
            //CurrentWindow.Navigate(ConstValue.SOMaintainUrlFormat, currentRefund.SOSysNo, true);
        }

        private void Hyperlink_RMANumber_Click(object sender, RoutedEventArgs e)
        {
            if (currentRefund.OrderType == RefundOrderType.RO)
            {
                //跳转到RMA退款单维护页面
                //CurrentWindow.Navigate(ConstValue.RMA_RefundMaintainUrl, currentRefund.RMANumber, true);
                CurrentWindow.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, currentRefund.RMANumber), null, true);
            }
            else if (currentRefund.OrderType != RefundOrderType.RO_Balance)
            {
                //跳转到订单维护页面
                CurrentWindow.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, currentRefund.RMANumber), null, true);
                //CurrentWindow.Navigate(ConstValue.SOMaintainUrlFormat, currentRefund.RMANumber, true);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}