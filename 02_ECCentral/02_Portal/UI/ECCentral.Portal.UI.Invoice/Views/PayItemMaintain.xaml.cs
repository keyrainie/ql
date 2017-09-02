using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 维护付款单
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url, NeedAccess = true)]
    public partial class PayItemMaintain : PageBase
    {
        private PayItemFacade _payItemFacade;
        private PayableFacade _payFacade;
        private PaymentOrderMaintainVM _pageVM;

        public PayItemMaintain()
        {
            InitializeComponent();
            _pageVM = new PaymentOrderMaintainVM();
            this.LayoutRoot.DataContext = _pageVM;
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            _payItemFacade = new PayItemFacade(this);
            _payFacade = new PayableFacade(this);
            VerifyPermission();
            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                LoadPayItemDetailInfo();
            }
            base.OnPageLoad(sender, e);
        }

        private void VerifyPermission()
        {
            this.btnNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItem_InvoiceInputMaintain_Insert);

        }

        private bool ValidQueryParam(string queryParam, ref PayItemDetailInfoReq request)
        {
            bool valid = false;

            var match = Regex.Match(this.Request.Param, @"^\?PaySysNo=(\d+)$|^\?OrderSysNo=(\d+)&OrderType=(\d+)$");
            if (match != null && !string.IsNullOrEmpty(match.Value))
            {
                if (match.Value.IndexOf("PaySysNo=") >= 0)
                {
                    int paySysNo;
                    if (int.TryParse(match.Groups[1].Value, out paySysNo))
                    {
                        request.PaySysNo = paySysNo;
                        valid = true;
                    }
                }
                else if (match.Value.IndexOf("OrderSysNo=") >= 0)
                {
                    int orderSysNo;
                    int orderType;

                    bool flag1 = false;
                    bool flag2 = false;
                    if (int.TryParse(match.Groups[2].Value, out orderSysNo))
                    {
                        request.OrderSysNo = orderSysNo;
                        flag1 = true;
                    }
                    if (int.TryParse(match.Groups[3].Value, out orderType) &&
                        Enum.IsDefined(typeof(PayableOrderType), orderType))
                    {
                        request.OrderType = (PayableOrderType)orderType;
                        flag2 = true;
                    }
                    valid = flag1 && flag2;
                }
            }
            return valid;
        }

        private void LoadPayItemDetailInfo()
        {
            PayItemDetailInfoReq request = new PayItemDetailInfoReq();
            if (ValidQueryParam(this.Request.Param, ref request))
            {
                this._payFacade.LoadPayDetailInfoForEdit(request, result =>
                {
                    this.btnNew.IsEnabled = true && AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItem_InvoiceInputMaintain_Insert);

                    _pageVM = result;
                    this.LayoutRoot.DataContext = _pageVM;
                    this.tbTotalInfo.Text = string.Format(ResPayItemMaintain.Message_TotalInfo,
                        _pageVM.OrderSysNo, ConstValue.Invoice_ToCurrencyString(_pageVM.TotalAmt), ConstValue.Invoice_ToCurrencyString(_pageVM.PaidAmt));
                    this.tbTotalInfo.Visibility = Visibility.Visible;

                    if (_pageVM.OrderType == PayableOrderType.POAdjust ||_pageVM.OrderType == PayableOrderType.RMAPOR)
                    {
                        this.btnNew.IsEnabled = false;
                    }
                });
            }
            else 
            {
                this.Window.Confirm(ResPayItemMaintain.Message_RecordDataError, (x => {
                    this.Window.Close();
                }));
                
 
            }
        }

        /// <summary>
        /// 创建付款单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var entity = _pageVM.ConvertVM<PaymentOrderMaintainVM, PayItemVM>();
            var uc = new UCPayItemMaintan(entity, UCPayItemMaintan.ActionType.New);
            uc.ShowDialog(ResPayItemMaintain.Message_CreateDlgTitle, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    LoadPayItemDetailInfo();
                }
            });
        }

        /// <summary>
        /// 修改付款单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Modify_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItem_InvoiceInputMaintain_Update))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var cur = this.dgPayItemList.SelectedItem as PayItemVM;
            if (cur != null)
            {
                var uc = new UCPayItemMaintan(cur.DeepCopy(), UCPayItemMaintan.ActionType.Update);
                uc.ShowDialog(ResPayItemMaintain.Message_ModifyDlgTitle, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        LoadPayItemDetailInfo();
                    }
                });
            }
        }

        /// <summary>
        /// 作废付款单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Abandon_Click(object sender, RoutedEventArgs e)
        {
            var payItem = this.dgPayItemList.SelectedItem as PayItemVM;
            if (payItem != null)
            {
                Window.Confirm(ResPayItemMaintain.Message_ConfirmAbandonDlgTitle, () =>
                {
                    _payItemFacade.Abandon(payItem.DeepCopy(), () => 
                        {
                            Window.Alert(ResPayItemMaintain.Message_SuccessAbandon);
                            LoadPayItemDetailInfo();
                        });
                });
            }
        }

        /// <summary>
        /// 取消作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_CancelAbandon_Click(object sender, RoutedEventArgs e)
        {
            var payItem = this.dgPayItemList.SelectedItem as PayItemVM;
            if (payItem != null)
            {
                Window.Confirm(ResPayItemMaintain.Message_ConfirmUnAbandonDlgTitle, () =>
                {
                    _payItemFacade.CancelAbandon(payItem.DeepCopy(), () =>
                    {
                        Window.Alert(ResPayItemMaintain.Message_SuccessCancelAbandon);
                        LoadPayItemDetailInfo();
                    });
                });
            }
        }

        /// <summary>
        /// 支付付款单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Pay_Click(object sender, RoutedEventArgs e)
        {
            var payItem = this.dgPayItemList.SelectedItem as PayItemVM;
            if (payItem != null)
            {
                Window.Confirm(ResPayItemMaintain.Message_ConfirmPayDlgTitle, () =>
                {
                    if ((_pageVM.OrderAmt >= 0 && (payItem.PayAmt ?? 0M) > ((_pageVM.OrderAmt ?? 0M) - _pageVM.PaidAmt))
                               || (_pageVM.OrderAmt < 0 && Math.Abs(payItem.PayAmt ?? 0M) > Math.Abs(_pageVM.OrderAmt.Value - _pageVM.PaidAmt.Value)))
                    {
                        Window.Alert(ResPayItemMaintain.Message_PayAmtNotEqualUnPayAmt);
                        return;
                    }
                    else
                    {
                        //if (payItem.OrderType != PayableOrderType.ReturnPointCashAdjust)
                        //{
                        //    _payItemFacade.Pay(payItem.DeepCopy(), () =>
                        //    {
                        //        Window.Alert(ResPayItemMaintain.Message_SuccessPay);
                        //        LoadPayItemDetailInfo();
                        //    });
                        //}
                        //else
                        {
                            //现金支付需要输入银行科目
                            var uc = new UCBankGLAccountPanel();
                            uc.ShowDialog(ResPayItemMaintain.Message_BankGLAccountDlgTitle, (obj, args) =>
                            {
                                if (args.DialogResult == DialogResultType.OK)
                                {
                                    payItem.BankGLAccount = args.Data.ToString();
                                    _payItemFacade.Pay(payItem.DeepCopy(), () =>
                                    {
                                        Window.Alert(ResPayItemMaintain.Message_SuccessPay);
                                        LoadPayItemDetailInfo();
                                    });
                                }
                            });
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 取消支付
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_CancelPay_Click(object sender, RoutedEventArgs e)
        {
            var payItem = this.dgPayItemList.SelectedItem as PayItemVM;
            if (payItem != null)
            {
                Window.Confirm(ResPayItemMaintain.Message_ConfirmUnPayDlgTitle, () =>
                {
                    _payItemFacade.CancelPay(payItem.DeepCopy(), () =>
                    {
                        Window.Alert(ResPayItemMaintain.Message_SuccessCancelPay);
                        LoadPayItemDetailInfo();
                    });
                });
            }
        }

        /// <summary>
        /// 锁定付款单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Lock_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItem_InvoiceInputMaintain_PayItemLock))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var payItem = this.dgPayItemList.SelectedItem as PayItemVM;
            if (payItem != null)
            {
                Window.Confirm(ResPayItemMaintain.Message_ConfirmLockDlgTitle, () =>
                {
                    _payItemFacade.Lock(payItem.DeepCopy(), () => LoadPayItemDetailInfo());
                });
            }
        }

        /// <summary>
        /// 取消锁定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_CancelLock_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItem_InvoiceInputMaintain_CancelPayItemLock))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var payItem = this.dgPayItemList.SelectedItem as PayItemVM;
            if (payItem != null)
            {
                Window.Confirm(ResPayItemMaintain.Message_ConfirmUnLockDlgTitle, () =>
                {
                    _payItemFacade.CancelLock(payItem.DeepCopy(), () => LoadPayItemDetailInfo());
                });
            }
        }

        /// <summary>
        /// 查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_View_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgPayItemList.SelectedItem as PayItemVM;
            if (cur != null)
            {
                var uc = new UCPayItemMaintan(cur.DeepCopy(), UCPayItemMaintan.ActionType.View);
                uc.ShowDialog(ResPayItemMaintain.Message_ViewDlgTitle, null);
            }
        }

        /// <summary>
        /// 设置发票号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_ReferenceID_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgPayItemList.SelectedItem as PayItemVM;
            if (cur != null)
            {
                var uc = new UCPayItemMaintan(cur.DeepCopy(), UCPayItemMaintan.ActionType.SetReferenceID);
                uc.ShowDialog(ResPayItemMaintain.Message_SetReferenceIDDlgTitle, (_, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        LoadPayItemDetailInfo();
                    }
                });
            }
        }
    }
}