using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOProcesser : UserControl
    {
        #region  初始化
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow Window
        {
            get
            {
                return Page != null ? Page.Context.Window : CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private IPage Page
        {
            get;
            set;
        }

        private int SOSysNo;
        CommonDataFacade CommonDataFacade;
        SOInternalMemoFacade SOInternalMemoFacade;
        SOQueryFacade SOQueryFacade;
        SOFacade SOFacade;
        dynamic giftList;

        SOVM _soVM;
        SOVM CurrentSOVM
        {
            get { return _soVM; }
            set
            {
                _soVM = value;
                if (_soVM != null)
                {
                    btnHoldSO.IsEnabled = btnSplitInvoice.IsEnabled = true;
                    gridSOInfo.DataContext = _soVM;
                    btnHoldSO.Content = _soVM.BaseInfoVM.HoldStatus.Value == BizEntity.SO.SOHoldStatus.BackHold || CurrentSOVM.BaseInfoVM.HoldStatus.Value == BizEntity.SO.SOHoldStatus.WebHold ? ResSO.Button_UnholdSO : ResSO.Button_HoldSO;
                    SetOperationButton();
                    List<SOItemInfoVM> mainProductList = new List<SOItemInfoVM>();
                    _soVM.ItemsVM.ForEach(item =>
                    {
                        if (item.ProductType == SOProductType.Product)
                        {
                            mainProductList.Add(item);
                        }
                    });
                    dataGridAdjustProduct.ItemsSource = mainProductList;
                    dataGridMainProduct.ItemsSource = _soVM.ItemsVM;
                }
            }
        }
        public SOProcesser(IPage page, int soSysNo)
        {
            SOSysNo = soSysNo;
            this.Page = page;
            InitializeComponent();
            Loaded += new RoutedEventHandler(SOProcesser_Loaded);
        }

        void SOProcesser_Loaded(object sender, RoutedEventArgs e)
        {
            CommonDataFacade = new CommonDataFacade(Page);
            SOInternalMemoFacade = new Facades.SOInternalMemoFacade(Page);
            SOQueryFacade = new SOQueryFacade(Page);
            SOFacade = new SOFacade(Page);
            LoadPage();
        }

        private void LoadPage()
        {
            SOQueryFacade.QuerySOInfo(SOSysNo, vm =>
            {
                if (vm == null)
                {
                    Window.Alert(ResSO.Info_SOIsNotExist, ResSO.Info_SOIsNotExist, MessageType.Warning, (obj, args) =>
                    {
                        Window.Close();
                    });
                    return;
                }
                CurrentSOVM = vm;
                List<int> mainProductSysNoList = new List<int>();
                CurrentSOVM.ItemsVM.ForEach(item =>
                {
                    if (item.ProductType == SOProductType.Product)
                    {
                        mainProductSysNoList.Add(item.ProductSysNo.Value);
                    }
                });
                new OtherDomainQueryFacade(Page).GetGiftByMasterProducts(_soVM.BaseInfoVM.OrderTime.Value, mainProductSysNoList, (vmList) =>
                {
                    if (vmList != null && vmList.Rows != null)
                    {
                        dataGridGift.ItemsSource = giftList = vmList.Rows.ToList("IsChecked", false);
                        btnAddGiftSO.IsEnabled = CurrentSOVM.BaseInfoVM.Status.Value == SOStatus.OutStock;
                    }
                    else
                    {
                        btnAddGiftSO.IsEnabled = false;
                    }
                });
                SOInternalMemoQueryFilter queryFilter = new SOInternalMemoQueryFilter();
                queryFilter.SOSysNo = CurrentSOVM.BaseInfoVM.SysNo;
                SOQueryFacade.QuerySOInternalMemo(queryFilter, (objSOInternalMemo, argsSOInternalMemo) =>
                {
                    if (!argsSOInternalMemo.FaultsHandle())
                    {
                        dataGridAddPointLog.TotalCount = argsSOInternalMemo.Result.TotalCount;
                        dataGridAddPointLog.ItemsSource = argsSOInternalMemo.Result.Rows;
                    }
                });

                SetReportedButtonIsEnabled();
            });

            dataGridRMA.Bind();

            #region 加载控件特殊值

            addpublic.SOSysNo = publicList.SOSysNo = complainList.SOSysNo = soLogList.SOSysNo = SOSysNo;

            addpublic.RefreshLog = publicList.Bind;

            addpublic.RefreshComplian = complainList.Bind;

            #endregion

            RightControl();
        }

        void RightControl()
        {
            btnAuditNetPayAndSO.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAuditNetPay);
            btnAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAudit);
            btnForceAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAudit);
            btnCancelAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOCancelAudit);
            btnManagerAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOManagerAudit);
            btnForceManagerAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOManagerAudit);
            btnHoldSO.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_HoldSO);
            btnSplit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SplitSO);
            btnCancelSplit.Visibility = System.Windows.Visibility.Collapsed;
            btnSplitInvoice.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SplitInvoice);
            btnAbandon.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAbandon);
            btnAbandonAndReturnInventory.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOEmployeeAbandon);
            btnCreateAOAndAbandon.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAbandon, AuthKeyConst.SO_SOMaintain_CreateNegativeFinanceRecord);
            btnSplit.Visibility = System.Windows.Visibility.Collapsed;
            btnSplitInvoice.Visibility = System.Windows.Visibility.Collapsed;
        }

        ECCentral.BizEntity.Invoice.SOIncomeInfo CurrentSOIncomeInfo;

        private void SetOperationButton()
        {
            OperationControlStatusHelper.SetAllButtonNotEnabled(ButtonPannel);

            btnRefresh.IsEnabled = true;

            if (CurrentSOVM.BaseInfoVM.Status == SOStatus.Split || CurrentSOVM.BaseInfoVM.Status == SOStatus.Abandon || CurrentSOVM.BaseInfoVM.Status == SOStatus.SystemCancel || CurrentSOVM.BaseInfoVM.Status == SOStatus.Reject)
            {
                btnPrintSO.IsEnabled = true;
            }
            if (CurrentSOVM.BaseInfoVM.Status == SOStatus.Origin)
            {
                btnForceAudit.IsEnabled = btnAudit.IsEnabled = AbandonButtonSet =
                    btnHoldSO.IsEnabled = btnSplitInvoice.IsEnabled = btnPrintSO.IsEnabled = true;
                btnAuditNetPayAndSO.IsEnabled = CurrentSOVM.BaseInfoVM.IsNet ?? false;

                if (CurrentSOVM.BaseInfoVM.SplitType == SOSplitType.Customer
                    || CurrentSOVM.BaseInfoVM.SplitType == SOSplitType.Force)
                {
                    btnSplit.IsEnabled = true;
                    btnAudit.IsEnabled = btnForceAudit.IsEnabled = btnAuditNetPayAndSO.IsEnabled = false;
                }
            }
            else if (CurrentSOVM.BaseInfoVM.Status == SOStatus.WaitingOutStock)
            {
                btnCancelAudit.IsEnabled = btnHoldSO.IsEnabled = AbandonButtonSet = btnPrintSO.IsEnabled = true;
            }
            else if (CurrentSOVM.BaseInfoVM.Status == SOStatus.WaitingManagerAudit)
            {
                btnForceManagerAudit.IsEnabled = btnCancelAudit.IsEnabled = btnManagerAudit.IsEnabled =
                    btnHoldSO.IsEnabled = btnPrintSO.IsEnabled = true;
            }
            else if (CurrentSOVM.BaseInfoVM.Status == SOStatus.OutStock)
            {
                btnSplitInvoice.IsEnabled = btnPrintSO.IsEnabled = btnHoldSO.IsEnabled = true;
            }

            if (CurrentSOVM.InvoiceInfoVM.IsVAT == true)
            {
                btnHoldSO.IsEnabled = true;
            }

            if (CurrentSOVM.ShippingInfoVM.StockType == BizEntity.Invoice.StockType.MET)
            {
                btnSplit.IsEnabled = btnCancelAudit.IsEnabled = false;
                switch (CurrentSOVM.BaseInfoVM.Status)
                {
                    case SOStatus.WaitingOutStock:
                        btnSplitInvoice.IsEnabled = AbandonButtonSet = btnHoldSO.IsEnabled = false;
                        break;
                    case SOStatus.OutStock:
                        btnSplitInvoice.IsEnabled = false;
                        break;
                    default:
                        break;
                }
            }

            if (CurrentSOVM != null && CurrentSOVM.BaseInfoVM != null && CurrentSOVM.BaseInfoVM.Status == SOStatus.OutStock)
            {
                new OtherDomainQueryFacade(Page).GetSOIncomeBySOSysNo(SOSysNo, (soIncomeInfo) =>
                {
                    CurrentSOIncomeInfo = soIncomeInfo;
                    if (CurrentSOVM.BaseInfoVM.SOType != SOType.PhysicalCard)//礼品卡订单不允许报关申报作废
                    {
                        btnReportedFaulure.IsEnabled = true;
                    }
                });
            }
            else
            {
                btnReportedFaulure.IsEnabled = false;
            }
            if (btnAbandon.IsEnabled)
            {
                new OtherDomainQueryFacade(Page).GetSOIncomeBySOSysNo(SOSysNo, (soIncomeInfo) =>
                {
                    CurrentSOIncomeInfo = soIncomeInfo;
                    SOFacade.ConfirmOperationSubSO(CurrentSOVM, SetAbandon, SetAOAbondon);
                });
            }
            if (CurrentSOVM != null && CurrentSOVM.BaseInfoVM != null && CurrentSOVM.BaseInfoVM.Status == SOStatus.Reject)
            {
                btnHoldSO.IsEnabled = false;
            }

            btnSplit.Visibility = System.Windows.Visibility.Collapsed;
            btnSplitInvoice.Visibility = System.Windows.Visibility.Collapsed;
        }

        void SetAbandon()
        {
            btnCreateAOAndAbandon.IsEnabled = false;
            btnAbandonAndReturnInventory.IsEnabled = btnAbandon.IsEnabled = !btnCreateAOAndAbandon.IsEnabled;
        }

        void SetAOAbondon()
        {
            btnCreateAOAndAbandon.IsEnabled = CurrentSOIncomeInfo != null;
            btnAbandonAndReturnInventory.IsEnabled = btnAbandon.IsEnabled = !btnCreateAOAndAbandon.IsEnabled;
        }

        bool AbandonButtonSet
        {
            set
            {
                btnAbandon.IsEnabled = btnAbandonAndReturnInventory.IsEnabled
                = btnCreateAOAndAbandon.IsEnabled = value;
            }
        }

        #endregion

        #region 订单操作

        #region 订单审核

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            if ((CurrentSOVM.BaseInfoVM.SOType == SOType.ElectronicCard || CurrentSOVM.BaseInfoVM.SOType == SOType.PhysicalCard)
                && !AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_AuditGiftCardOrder))
            {
                Window.Alert(ResSO.Msg_Error_RightGiftCardOrderAudit, MessageType.Error);
                return;
            }
            SOFacade.AuditSO(SOSysNo, false, (vm) =>
            {
                CurrentSOVM = vm;
                SetOperationButton();
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }

        private void btnForceAudit_Click(object sender, RoutedEventArgs e)
        {
            if ((CurrentSOVM.BaseInfoVM.SOType == SOType.ElectronicCard || CurrentSOVM.BaseInfoVM.SOType == SOType.PhysicalCard)
                && !AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_AuditGiftCardOrder))
            {
                Window.Alert(ResSO.Msg_Error_RightGiftCardOrderAudit, MessageType.Error);
                return;
            }
            SOFacade.AuditSO(SOSysNo, true, (vm) =>
            {
                CurrentSOVM = vm;
                SetOperationButton();
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }

        private void btnAuditNetPayAndSO_Click(object sender, RoutedEventArgs e)
        {
            if ((CurrentSOVM.BaseInfoVM.SOType == SOType.ElectronicCard || CurrentSOVM.BaseInfoVM.SOType == SOType.PhysicalCard)
                && !AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_AuditGiftCardOrder))
            {
                Window.Alert(ResSO.Msg_Error_RightGiftCardOrderAudit, MessageType.Error);
                return;
            }
            SOFacade.AuditNetPayAndSO(SOSysNo, (vm) =>
            {
                CurrentSOVM = vm;
                SetOperationButton();
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }

        private void btnManagerAudit_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.ManagerAuditSO(SOSysNo, false, (vm) =>
            {
                CurrentSOVM = vm;
                SetOperationButton();
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }

        private void btnForceManagerAudit_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.ManagerAuditSO(SOSysNo, true, (vm) =>
            {
                CurrentSOVM = vm;
                SetOperationButton();
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.ConfirmOperationSubSO(CurrentSOVM, ConfirmAbandonAllSubSO, CancelAudit);
        }

        private void CancelAudit()
        {
            SOFacade.CancelAuditSO(SOSysNo, (vm) =>
            {
                CurrentSOVM = vm;
                SetOperationButton();
                Window.Alert(ResSO.Info_SO_Processer_SO_CancelAudit_Pass, MessageType.Information);
            });
        }
        #endregion

        private void btnHoldSO_Click(object sender, RoutedEventArgs e)
        {

            HoldSO content = new HoldSO();
            content.Page = Page;
            content.CurrentSOVM = CurrentSOVM;
            content.Dialog = Window.ShowDialog(ResSO.UC_Title_SOHold, content, (obj, args) =>
            {
                CurrentSOVM = content.CurrentSOVM;
                SetOperationButton();
            });
        }

        private void btnSplitInvoice_Click(object sender, RoutedEventArgs e)
        {
            SplitInvoice invoiceDialog = new SplitInvoice(Page, CurrentSOVM);
            invoiceDialog.Width = 700;
            invoiceDialog.Height = 500;
            invoiceDialog.Dialog = Window.ShowDialog(String.Format("{0}{1}", ResSO.UC_Title_SplitInvoice, SOSysNo), invoiceDialog);
        }

        #region 订单作废
        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            Abandon(false);
        }
        private void btnAbandonAndReturnInventory_Click(object sender, RoutedEventArgs e)
        {
            Abandon(true);
        }

        private void Abandon(bool immediatelyReturnInventory)
        {
            Page.Context.Window.Confirm("确定要作废订单吗？", (s, e) =>
            {
                if (e.DialogResult == DialogResultType.OK)
                {
                    UCReasonCodePicker content = new UCReasonCodePicker();
                    content.ReasonCodeType = BizEntity.Common.ReasonCodeType.Order;
                    content.Dialog = Window.ShowDialog(ResSO.Info_SO_Processer_SO_Void, content, (obj, args) =>
                    {
                        if (args.Data != null)
                        {
                            KeyValuePair<string, string> data = (KeyValuePair<string, string>)args.Data;
                            if (CurrentSOVM.BaseInfoVM.SplitType == SOSplitType.SubSO)
                            {
                                SOFacade.GetIsAllSubSONotOutStock(SOSysNo, (subSOSender, subSOargs) =>
                                {
                                    if (!subSOargs.FaultsHandle())
                                    {
                                        if (subSOargs.Result)
                                        {
                                            ConfirmAbandonAllSubSO();
                                        }
                                        else
                                        {
                                            AbandonSO(immediatelyReturnInventory, data);
                                        }
                                    }
                                });
                            }
                            else
                            {
                                AbandonSO(immediatelyReturnInventory, data);
                            }
                        }
                    });
                }
            });
        }

        private void ConfirmAbandonAllSubSO()
        {
            Window.Confirm(ResSOMaintain.Info_AbandonAllSubSO, (cancelSplitSender, cancelSplitArgs) =>
            {
                if (cancelSplitArgs.DialogResult == DialogResultType.OK)
                {
                    SOFacade.CancelSplitSO(CurrentSOVM.BaseInfoVM.SOSplitMaster.Value, (vm) =>
                    {
                        SOSysNo = vm.SysNo.Value;
                        CurrentSOVM = vm;
                        SetOperationButton();
                    });
                }
            });
        }

        private void AbandonSO(bool immediatelyReturnInventory, KeyValuePair<string, string> data)
        {
            SOFacade.AbandonSO(SOSysNo, immediatelyReturnInventory, (vm) =>
            {
                if (vm != null)
                {
                    CurrentSOVM = vm;
                    SetOperationButton();

                    new SOInternalMemoFacade().Create(new SOInternalMemoInfo
                    {
                        SOSysNo = SOSysNo,
                        Content = data.Value,
                        LogTime = DateTime.Now,
                        ReasonCodeSysNo = int.Parse(data.Key),
                        CompanyCode = CPApplication.Current.CompanyCode,
                        Status = SOInternalMemoStatus.FollowUp
                    }, null);
                    Window.Alert(ResSO.Info_SO_Processer_SO_Void_Pass, MessageType.Information);
                }
            });
        }

        private void btnCreateAOAndAbandon_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.ConfirmOperationSubSO(CurrentSOVM, ConfirmAbandonAllSubSO, AbandonAndCreateAO);
        }

        private void AbandonAndCreateAO()
        {
            ECCentral.Portal.UI.SO.UserControls.CreateAOAndAbandonSO createAOControl = new CreateAOAndAbandonSO
            {
                Page = Page,
                SOSysNo = SOSysNo,
                CurrentSOIncomeInfo = CurrentSOIncomeInfo,
                PayTypeSysNo = CurrentSOVM.BaseInfoVM.PayTypeSysNo
            };
            createAOControl.Saved += (vm) =>
            {
                if (vm != null)
                {
                    CurrentSOVM = vm;
                    if (CurrentSOVM.BaseInfoVM == null || CurrentSOVM.BaseInfoVM.Status != SOStatus.OutStock)
                    {
                        btnReportedFaulure.IsEnabled = false;
                    }

                    if (CurrentSOVM.BaseInfoVM.Status == SOStatus.Abandon || CurrentSOVM.BaseInfoVM.Status == SOStatus.SystemCancel || CurrentSOVM.BaseInfoVM.Status == SOStatus.Reject)
                    {
                        btnHoldSO.IsEnabled = false;
                    }
                }
            };
            createAOControl.Dialog = Window.ShowDialog(String.Format(ResSO.UC_Title_CreateAO, SOSysNo), createAOControl, (obj, args) =>
            {

            });
        }

        #endregion

        #region 订单拆分/取消拆分
        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.SplitSO(SOSysNo, subSOSysList =>
            {
                if (subSOSysList != null && subSOSysList.Count > 0)
                {
                    Window.Alert(String.Format(ResSO.Info_SO_Processer_SO_Split_SubSO, String.Join(",", (from so in subSOSysList select so.SysNo))), MessageType.Information);
                }
                CurrentSOVM.BaseInfoVM.Status = SOStatus.Split;
                SetOperationButton();
            });
        }

        private void btnCancelSplit_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.CancelSplitSO(SOSysNo, (vm) =>
            {
                CurrentSOVM = vm;
                SetOperationButton();
                Window.Alert(ResSO.Msg_SO_CancelSplit, MessageType.Information);
            });
        }
        #endregion

        private void btnPrintSO_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> t = new Dictionary<string, string>();
            t.Add("SOSysNoList", SOSysNo.ToString());
            HtmlViewHelper.WebPrintPreview("SO", "SOInfo", t);
        }
        #endregion

        /// <summary>
        /// 创建客户加积分申请单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddPoint_Click(object sender, RoutedEventArgs e)
        {
            DateTime startTime = CurrentSOVM.BaseInfoVM.OrderTime.Value;
            if (string.IsNullOrEmpty(dpReceiveDate.Text))
            {
                Window.Alert(ResSO.Info_SOProcesser_ChangePrice_ReceiveDate_Error, MessageType.Error);
                return;
            }
            CPApplication.Current.CurrentPage.Context.Window.Confirm(ResSO.Info_Confirm_CreateAddPointRequest, (objConfirm, argsConfirm) =>
            {
                if (argsConfirm.DialogResult == DialogResultType.OK)
                {

                    DateTime endTime = DateTime.Parse(dpReceiveDate.Text);
                    List<int> sysnoList = new List<int>();
                    foreach (var item in CurrentSOVM.ItemsVM)
                    {
                        sysnoList.Add(item.ProductSysNo.Value);
                    }
                    SOQueryFacade.GetPriceChangeLogs(sysnoList, startTime, endTime, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        if (args.Result == null || args.Result.Count == 0)
                        {
                            Window.Alert(ResSO.ErrorInfo_CreateChangePriceRequest, MessageType.Error);
                        }
                        else
                        {
                            List<PriceChangeLogInfo> logList = args.Result;

                            int totalPoint = 0;
                            foreach (var item in CurrentSOVM.ItemsVM)
                            {
                                foreach (var loiitem in logList)
                                {
                                    if (item.ProductSysNo == loiitem.ProductSysNo)
                                    {
                                        item.CompensationPoint = Convert.ToInt32(((item.OriginalPrice - loiitem.NewPrice) * item.Quantity) * 10);
                                        totalPoint += item.CompensationPoint.Value;
                                    }
                                }
                            }
                            if (totalPoint < 0)
                            {
                                Window.Alert(ResSO.Info_SOProcesser_ChangePrice_Error, MessageType.Error);
                            }
                            else
                            {
                                //生成积分申请备注
                                string note = this.BuildAddPointRequestNote(logList, CurrentSOVM.ItemsVM, endTime.ToString());
                                SOFacade.CreateCustomerPointsAddRequest(CurrentSOVM, totalPoint, note, logList, (innerObj, innerArgs) =>
                                {
                                    if (innerArgs.FaultsHandle())
                                    {
                                        return;
                                    }
                                    if (innerArgs.Result != null && innerArgs.Result.SysNo.HasValue && innerArgs.Result.SysNo != 0)
                                    {
                                        AddPointInfo.Visibility = Visibility.Visible;
                                        txtCustomerAddPointRequestInfo.Text = ResSO.Info_SOProcesser_AddPoint_Request_Sucess;
                                        hlbtnCustomerAddPointRequestSysNo.Content = innerArgs.Result.SysNo;

                                        #region 创建固定信息的更新日志

                                        SOInternalMemoInfo publicMemo = new SOInternalMemoInfo();
                                        publicMemo.CallType = 14;//价格保护
                                        publicMemo.Content = note;
                                        publicMemo.SOSysNo = CurrentSOVM.BaseInfoVM.SysNo.Value;
                                        publicMemo.Note = txtAddPointNote.Text;
                                        publicMemo.ReasonCodeSysNo = 291;
                                        publicMemo.SourceSysNo = 5;
                                        SOInternalMemoFacade.Create(publicMemo, (publicMemoObj, publicMemoArgs) =>
                                        {
                                            if (publicMemoArgs.FaultsHandle())
                                            {
                                                return;
                                            }
                                        });
                                        #endregion
                                    }
                                });
                            }
                        }
                    });
                }
            });
        }

        /// <summary>
        /// 设置 顾客加积分申请说明:  
        /// </summary>
        /// <param name="logList">价格又降价的商品价格信息</param>
        /// <param name="soItems">订单商品</param>
        /// <param name="receiveTime">客户收货时间</param>
        /// <returns></returns>
        private string BuildAddPointRequestNote(List<PriceChangeLogInfo> logList, List<SOItemInfoVM> soItems, string receiveTime)
        {
            List<string> result = new List<string>();
            foreach (var logItem in logList)
            {
                foreach (var soItem in soItems)
                {
                    if (soItem.ProductSysNo == logItem.ProductSysNo)
                    {
                        result.Add(string.Format(ResSO.Info_SO_Processer_SO_Create_ChangePriceRequest
                                     , logItem.ProductSysNo.ToString()
                                     , soItem.OriginalPrice.ToString()
                                     , logItem.NewPrice.ToString()
                                     , logItem.CreateDate.ToString()
                                     , receiveTime
                                 ));
                    }
                }
            }
            return string.Join(";", result.ToArray());
        }

        private void hlbtnCustomerAddPointRequestSysNo_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.OK
            });
            string url = String.Format(ConstValue.CustomerPointsAddQuery + "/SysNo={0}", hlbtnCustomerAddPointRequestSysNo.Content);
            this.Window.Navigate(url, null, true);
        }
        /// <summary>
        /// 添加赠品订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddGiftSO_Click(object sender, RoutedEventArgs e)
        {
            if (giftList != null && giftList.Count > 0)
            {
                SOVM giftSOVM = new SOVM();
                giftSOVM.BaseInfoVM.SOType = SOType.Gift;
                giftSOVM.BaseInfoVM.Memo = txtAddGiftNote.Text;
                giftSOVM.BaseInfoVM.CustomerChannel = CurrentSOVM.BaseInfoVM.CustomerChannel;
                giftSOVM.BaseInfoVM.CustomerSysNo = CurrentSOVM.BaseInfoVM.CustomerSysNo;
                SOItemInfoVM soItem = null;

                foreach (dynamic g in dataGridGift.ItemsSource)
                {
                    if (g.IsChecked)
                    {
                        soItem = giftSOVM.ItemsVM.FirstOrDefault(item => item.ProductSysNo == g.ProductSysNo);
                        if (soItem == null)
                        {
                            soItem = new SOItemInfoVM
                            {
                                Quantity = 1,
                                Price = 0,
                                ProductSysNo = g.ProductSysNo,
                                ProductType = 0,
                                OriginalPrice = 0,
                                MasterProductSysNo = g.MasterProductSysNo.ToString()
                            };
                            giftSOVM.ItemsVM.Add(soItem);
                        }
                        else
                        {
                            soItem.Quantity += 1;
                        }
                    }
                }
                if (giftSOVM.ItemsVM.Count > 0)
                {
                    string errMsg = string.Empty;
                    #region 原订单中是否已经存在赠品

                    List<string> existsGifts = new List<string>();
                    foreach (var gift in giftSOVM.ItemsVM)
                    {
                        if (CurrentSOVM.ItemsVM.FirstOrDefault(x => { return x.ProductSysNo == gift.ProductSysNo; }) != null)
                        {
                            existsGifts.Add(gift.ProductID);
                        }
                    }

                    if (existsGifts.Count > 0)
                    {
                        errMsg = string.Format(ResSO.Msg_ConfirmCreateRepeatGiftSOOrder, string.Join(",", existsGifts.ToArray()));
                    }

                    #endregion

                    #region 订单是否创建过赠品单
                    SOLogQueryFilter query = new SOLogQueryFilter();
                    query.SOSysNo = SOSysNo;
                    query.PagingInfo = new PagingInfo()
                    {
                        PageSize = int.MaxValue,
                        PageIndex = 0,
                        SortBy = "SO_Log.OptTime DESC"
                    };
                    SOQueryFacade facade = new SOQueryFacade();
                    facade.QuerySOSystemLog(query, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        foreach (var item in args.Result.Rows)
                        {
                            //note需要特殊处理
                            if ((BizLogType)item["OptType"] == BizLogType.Sale_SO_CreateGiftSO)
                            {
                                errMsg += string.Format(ResSO.Msg_ConfirmCreateRepeatSOGiftOrder
                             , item["OptTime"]
                             , item["Note"]);
                                break;
                            }
                        }
                        if (errMsg.Length > 0)
                        {
                            Window.Confirm(errMsg, (giftCreaterSender, giftCreaterArgs) =>
                            {
                                if (giftCreaterArgs.DialogResult == DialogResultType.OK)
                                {
                                    ConfirmCreateGiftSO(giftSOVM);
                                }
                            });
                        }
                        else
                        {
                            ConfirmCreateGiftSO(giftSOVM);
                        }
                    });
                    #endregion
                }
                else
                {
                    Page.Context.Window.Alert(ResSO.Info_GiftSO_ItemIsNull);
                }
            }
            else
            {
                Page.Context.Window.Alert(ResSO.Info_GiftSO_ItemIsNull);
            }
        }

        private void ConfirmCreateGiftSO(SOVM giftSOVM)
        {
            SOFacade.CreateGiftSO(giftSOVM, CurrentSOVM.SysNo.Value, vm =>
            {
                if (vm != null)
                {
                    Page.Context.Window.Alert(String.Format(ResSO.Info_GiftSO_Created, vm.SysNo));
                }
            });
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(null);
            this.Window.Navigate(String.Format(ConstValue.SOMaintainUrlFormat, CurrentSOVM.SysNo), null, true);
        }

        private void hlbtnCustomerSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.CustomerMaintainUrlFormat, CurrentSOVM.BaseInfoVM.CustomerSysNo);
            this.Window.Navigate(url, null, true);
            this.Window.Navigate(Page.Context.Request.URL);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SOProcesser_Loaded(sender, e);
        }

        private void dataGridRMA_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            var query = new ECCentral.QueryFilter.RMA.RMARequestQueryFilter();
            query.SOSysNo = SOSysNo;
            query.IsReadRMAItemSysNos = true;
            query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            (new OtherDomainQueryFacade(CPApplication.Current.CurrentPage)).QueryRMARequest(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridRMA.TotalCount = args.Result.TotalCount;
                dataGridRMA.ItemsSource = args.Result.Rows;
            });
        }

        private void hlbtnRequestID_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml info = this.dataGridRMA.SelectedItem as DynamicXml;
            if (info != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.RMA_RequestMaintainUrl, info["SysNo"]), null, true);
            }
        }

        private void hlbtnRegisterID_Click(object sender, RoutedEventArgs e)
        {
            string registerSysNo = ((System.Windows.Controls.ContentControl)(sender)).Content.ToString();
            CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.RMA_RegisterMaintainUrl, registerSysNo), null, true);
        }

        private void hlbtnProduct_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            Page.Context.Window.Navigate(String.Format(ConstValue.IM_ProductMaintainUrlFormat, btn.CommandParameter), null, true);
        }

        #region Helper Methods

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        #endregion Helper Methods

        private void btnReportedFaulure_Click(object sender, RoutedEventArgs e)
        {
            AbandonAndCreateAO();
        }

        void SetReportedButtonIsEnabled()
        {
            btnReported.IsEnabled = false;
            btnReject.IsEnabled = false;
            btnCustomsPass.IsEnabled = false;
            btnCustomsReject.IsEnabled = false;
            if (CurrentSOVM.BaseInfoVM.Status == SOStatus.OutStock)
            {
                btnReported.IsEnabled = true;
                btnReject.IsEnabled = true;
            }
            if (CurrentSOVM.BaseInfoVM.Status == SOStatus.Reported)
            {
                btnCustomsPass.IsEnabled = true;
                btnCustomsReject.IsEnabled = true;
            }
        }

        private void btnReported_Click(object sender, RoutedEventArgs e)
        {
            //申报成功;
            SOFacade.UpdateSOStatusToReported(SOSysNo, (result) =>
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    LoadPage();
                    Window.Alert("操作成功", MessageType.Information);
                }
                else
                {
                    Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning);
                }
            });
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            //申报失败;
            SOFacade.UpdateSOStatusToReject(SOSysNo, (result) =>
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    LoadPage();
                    Window.Alert("操作成功", MessageType.Information);
                }
                else
                {
                    Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning);
                }
            });
        }

        private void btnCustomsPass_Click(object sender, RoutedEventArgs e)
        {
            //通关成功;
            SOFacade.UpdateSOStatusToCustomsPass(SOSysNo, (result) =>
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    LoadPage();
                    Window.Alert("操作成功", MessageType.Information);
                }
                else
                {
                    Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning);
                }
            });
        }

        private void btnCustomsReject_Click(object sender, RoutedEventArgs e)
        {
            //通关失败;
            SOFacade.UpdateSOStatusToCustomsReject(SOSysNo, (result) =>
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    LoadPage();
                    Window.Alert("操作成功", MessageType.Information);
                }
                else
                {
                    Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning);
                }
            });
        }
    }
}
