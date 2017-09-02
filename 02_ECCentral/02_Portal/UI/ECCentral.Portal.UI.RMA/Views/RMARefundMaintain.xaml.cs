using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.UI.RMA.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class RMARefundMaintain : PageBase
    {
        private int loadCompletedCount = 0;
        private RefundFacade refundFacade;
        private CommonDataFacade commonFacade;
        private int sysNo = 0;

        private List<StockInfo> stocks;
        private List<CodeNamePair> refundReasons;
        private RefundVM vm;

        public RMARefundMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            refundFacade = new RefundFacade(this);
            commonFacade = new CommonDataFacade(this);

            string no = Request.Param;
            if (!string.IsNullOrEmpty(no))
            {
                int.TryParse(no, out sysNo);
            }
            else
            {
                this.DataContext = new RefundVM();
            }

            LoadStocks();

            LoadRefundReasons();

            LoadRefund();
        }

        private void LoadStocks()
        {
            commonFacade.GetStockList(false, (o, a) =>
            {
                this.stocks = a.Result;
                this.stocks.Insert(0, new StockInfo { StockName = ResCommonEnum.Enum_Select });

                SetDataContext();
            });
        }

        private void LoadRefundReasons()
        {
            refundFacade.GetRefundReaons((obj, args) =>
            {
                this.refundReasons = args.Result;
                this.refundReasons.Insert(0, new CodeNamePair { Name = ResCommonEnum.Enum_Select });

                SetDataContext();
            });
        }

        private void LoadRefund()
        {
            if (this.sysNo > 0)
            {
                refundFacade.Load(this.sysNo, (obj, args) =>
                {
                    this.vm = args.Result;

                    SetDataContext();
                });
            }
        }

        private void SetDataContext()
        {            
            Interlocked.Increment(ref loadCompletedCount);

            if (loadCompletedCount == 3)
            {
                this.vm.Stocks = this.stocks;
                this.vm.RefundReasons = this.refundReasons;
                this.DataContext = this.vm;
                this.vm.ValidationErrors.Clear();

                SetButtonStatus();
            }
        }

        /// <summary>
        /// 控制按钮的是否可用状态
        /// </summary>
        private void SetButtonStatus()
        {
            btnUpdate.IsEnabled = false;
            btnCalculate.IsEnabled = false;
            btnSubmitAudit.IsEnabled = false;
            btnCancelSubmitAudit.IsEnabled = false;
            btnVoid.IsEnabled = false;
            btnRefund.IsEnabled = false;
            btnRefundBalance.IsEnabled = false;
            btnPrint.IsEnabled = false;

            RefundVM vm = this.DataContext as RefundVM;
            if (vm.Status == RMARefundStatus.WaitingRefund)
            {
                btnUpdate.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_Update);
                btnCalculate.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_Calculate);
                btnSubmitAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_SubmitAudit);
                btnVoid.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_Abandon);
                vm.RefundItems.ForEach(p => p.CanChangeRefundPriceType = true);
            }
            else if (vm.Status == RMARefundStatus.WaitingAudit)
            {
                btnSubmitAudit.IsEnabled = false;
                btnCancelSubmitAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_CancelAudit);
                btnRefund.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_Refund);
                vm.RefundItems.ForEach(p => p.CanChangeRefundPriceType = false);
            }
            else if (vm.Status == RMARefundStatus.Refunded)
            {
                btnRefundBalance.IsEnabled = true;
                btnPrint.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Refund_Print);
                btnSetInvoiceNo.IsEnabled = true;
                vm.RefundItems.ForEach(p => p.CanChangeRefundPriceType = false);
            }
            else if (vm.Status == RMARefundStatus.Abandon)
            {
                btnUpdate.IsEnabled = false;
                btnCalculate.IsEnabled = false;
                btnSubmitAudit.IsEnabled = false;
                btnCancelSubmitAudit.IsEnabled = false;
                btnVoid.IsEnabled = false;
                btnRefund.IsEnabled = false;
                btnRefundBalance.IsEnabled = false;
                btnPrint.IsEnabled = false;
                btnSetInvoiceNo.IsEnabled = false;
                vm.RefundItems.ForEach(p => p.CanChangeRefundPriceType = false);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as RefundVM;
            var para = new Dictionary<string,string>();
            para.Add("SysNo",vm.SysNo.ToString());
            HtmlViewHelper.WebPrintPreview(ConstValue.DomainName_RMA, "PrintRefund", para);                   
        }

        private void btnSetInvoiceNo_Click(object sender, RoutedEventArgs e)
        {            
            UCSetInvoiceNo uc = new UCSetInvoiceNo();
            IDialog dialog = Window.ShowDialog("设置发票号", uc, (obj, args) =>
            {

            });
            uc.Dialog = dialog;
        }

        private void btnRefundBalance_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as RefundVM ;
            Window.Navigate(string.Format(ConstValue.RMA_RefundBalanceQueryUrl, vm.SysNo), null, true);
        }

        private void btnRefund_Click(object sender, RoutedEventArgs e)
        {
            RefundVM vm = this.DataContext as RefundVM;
            refundFacade.Refund(vm.SysNo.Value, (obj, args) =>
            {
                vm.Status = args.Result.Status;

                SetButtonStatus();

                Window.Alert(ResRefundMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            RefundVM vm = this.DataContext as RefundVM;
            refundFacade.Abandon(vm.SysNo.Value, (obj, args) =>
            {
                vm.Status = args.Result.Status;

                SetButtonStatus();

                Window.Alert(ResRefundMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnCancelSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
            RefundVM vm = this.DataContext as RefundVM;
            refundFacade.CancelSubmitAudit(vm, (obj, args) =>
            {
                vm.Status = args.Result.Status;

                SetButtonStatus();

                Window.Alert(ResRefundMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                RefundVM vm = this.DataContext as RefundVM;
                refundFacade.SubmitAudit(vm, (obj, args) =>
                {
                    vm.Status = args.Result.Status;

                    SetButtonStatus();

                    Window.Alert(ResRefundMaintain.Info_OperateSuccessfully);
                });
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                RefundVM vm = this.DataContext as RefundVM;
                refundFacade.Update(vm, (obj, args) =>
                {
                    Window.Alert(ResRefundMaintain.Info_OperateSuccessfully);
                });
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            var refundItem = btn.DataContext as RefundItemVM;
            Window.Navigate(string.Format(ConstValue.RMA_RegisterMaintainUrl, refundItem.RegisterSysNo), null, true);
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                RefundVM vm = this.DataContext as RefundVM;
                refundFacade.Calculate(vm, (obj, args) =>
                {
                    vm.SOCashPointRate = args.Result.SOCashPointRate;
                    vm.DeductPointFromAccount = args.Result.DeductPointFromAccount;
                    vm.DeductPointFromCurrentCash = args.Result.DeductPointFromCurrentCash;
                    vm.OrgPointAmt = args.Result.OrgPointAmt;
                    vm.PointAmt = args.Result.PointAmt;
                    vm.OrgCashAmt = args.Result.OrgCashAmt;
                    vm.CashAmt = args.Result.CashAmt;
                    vm.OrgGiftCardAmt = args.Result.OrgGiftCardAmt;
                    vm.GiftCardAmt = args.Result.GiftCardAmt;
                    vm.PriceprotectPoint = args.Result.PriceprotectPoint;

                    //刷新界面的初算退款金额
                    vm.RefundItems.ForEach(item =>
                    {
                        RefundItemInfo tmpInfo = args.Result.RefundItems.FirstOrDefault(x => x.RegisterSysNo == item.RegisterSysNo);
                        if (tmpInfo != null)
                            item.RefundCash = tmpInfo.RefundCash;
                    });

                    Window.Alert(ResRefundMaintain.Info_OperateSuccessfully);
                });
            }
        }       
    }
}