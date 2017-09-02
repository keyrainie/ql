using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.UserControls.DynamicQueryFilter.SaleIncome;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Browser;
using System.Text;
using System.IO;
using System.Threading;


namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 销售-收款单查询界面
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page)]
    public partial class SaleIncomeQuery : PageBase
    {
        #region Private Fields

        private SaleIncomeQueryVM queryVM;
        private SaleIncomeQueryVM lastQueryVM;
        private SaleIncomeFacade soIncomeFacade;
        private OtherDomainDataFacade otherFacade;
        private Dictionary<string, UserControl> DynamicQueryFilters = new Dictionary<string, UserControl>();

        #endregion Private Fields

        #region Constructor

        public SaleIncomeQuery()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SaleIncomeQuery_Loaded);
            IncomeTypeCombox.SelectionChanged += IncomeTypeCombox_SelectionChanged;
        }

        void IncomeTypeCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckCashOnDeliveryTypeVisable();
        }

        private void CheckCashOnDeliveryTypeVisable()
        {
            CashOnDeliveryTypeTitle.Visibility = System.Windows.Visibility.Collapsed;
            CashOnDeliveryTypeCombox.Visibility = System.Windows.Visibility.Collapsed;
            SOIncomeOrderStyle? sOIncomeOrderStyle = (SOIncomeOrderStyle?)IncomeTypeCombox.SelectedValue;
            SOIncomeOrderType? orderType = (SOIncomeOrderType?)cbxOrderType.SelectedValue;
            if ((sOIncomeOrderStyle.HasValue && sOIncomeOrderStyle.Value == SOIncomeOrderStyle.Normal) &&           //收款类型：货到付款
                (orderType.HasValue == false || orderType == SOIncomeOrderType.SO))    //单据类型：所有单据或者SO单
            {
                CashOnDeliveryTypeTitle.Visibility = System.Windows.Visibility.Visible;
                CashOnDeliveryTypeCombox.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermission();
            base.OnPageLoad(sender, e);
        }

        #endregion Constructor

        #region Event Handler

        private void SaleIncomeQuery_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(SaleIncomeQuery_Loaded);

            RegisterDynamicQueryFilters();
            InitData();
            soIncomeFacade = new SaleIncomeFacade(this);
            otherFacade = new OtherDomainDataFacade(this);
            LoadComboBoxData();
            cbxOrderType.SelectedIndex = 0;
        }

        private void VerifyPermission()
        {
            this.btnBatchSetReferenceID.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_SetReferenceID);
            this.btnConfirm.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_Confirm);
            // this.btnCancelConfirm.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_CancelConfirm_InConfirmDay) || AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_CancelConfirm_AfterConfirmDay);
            this.btnAbandon.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_Abandon);
            //this.btnAutoConfirm.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_AutoConfirm);
            this.btnExportRO.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_GetROExport);
            this.dgSaleIncomeQueryResult.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_Export);
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.dgSaleIncomeQueryResult.ItemsSource as List<SaleIncomeVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }

        /// <summary>
        /// 点击单据系统编号的跳转，只有当单据类型是RO_Balance的时候才可用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnOrderSysNo_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgSaleIncomeQueryResult.SelectedItem as SaleIncomeVM;
            if (cur != null)
            {
                if (cur.OrderType == SOIncomeOrderType.RO_Balance)
                {
                    int balanceSysNo = int.Parse(cur.OrderID.Substring(2, 8));
                    Window.Navigate(string.Format(ConstValue.RMA_RefundBalanceQueryUrl, balanceSysNo), null, true);
                }
            }
        }

        /// <summary>
        /// 点击用户编号链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnCustomerSysNo_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgSaleIncomeQueryResult.SelectedItem as SaleIncomeVM;
            if (cur != null)
            {
                Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, cur.CustomerSysNo), null, true);
            }
        }

        /// <summary>
        /// 点击订单编号链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnOrderID_Click(object sender, RoutedEventArgs e)
        {
            //需要根据不同的单据类型跳转到不同的单据维护界面
            var cur = this.dgSaleIncomeQueryResult.SelectedItem as SaleIncomeVM;
            if (cur != null)
            {
                if (cur.OrderType == SOIncomeOrderType.SO ||
                    cur.OrderType == SOIncomeOrderType.AO ||
                    cur.OrderType == SOIncomeOrderType.OverPayment)
                {
                    //链接到订单维护页面
                    Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, cur.OrderSysNo, true), null, true);
                }
                else if (cur.OrderType == SOIncomeOrderType.RO)
                {
                    //链接到退款单维护页面
                    Window.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, cur.OrderSysNo, true), null, true);
                }
            }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            bool flag = ValidationManager.Validate(this.gridQueryBuilder);
            if (flag)
            {
                this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SaleIncomeQueryVM>(queryVM);

                this.dgSaleIncomeQueryResult.Bind();
            }
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnReferenceID_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgSaleIncomeQueryResult.SelectedItem as SaleIncomeVM;
            if (cur != null)
            {
                new UCSaleIncomeEdit(cur.DeepCopy(), UCSaleIncomeEdit.ActionType.SetReferenceID)
                    .ShowDialog(ResCommon.Message_ShowDlgDefaultTitle, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            this.dgSaleIncomeQueryResult.Bind();
                        }
                    });
            }
        }

        /// <summary>
        /// 设置收款单金额
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnIncomeAmt_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgSaleIncomeQueryResult.SelectedItem as SaleIncomeVM;
            if (cur != null)
            {
                new UCSaleIncomeEdit(cur.DeepCopy(), UCSaleIncomeEdit.ActionType.SetInvoiceAmt)
                    .ShowDialog(ResCommon.Message_ShowDlgDefaultTitle, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            this.dgSaleIncomeQueryResult.Bind();
                        }
                    });
            }
        }

        private void dgSaleIncomeQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            lastQueryVM.ByCustomer = (rbByCustomer.IsChecked ?? false) ? "1"
                : (rbByOrder.IsChecked ?? false) ? "2" : "0";

            soIncomeFacade.Query(lastQueryVM, e.PageSize, e.PageIndex, e.SortField,
                result =>
                {
                    //绑定结果列表
                    this.dgSaleIncomeQueryResult.ItemsSource = result.ResultList;
                    this.dgSaleIncomeQueryResult.TotalCount = result.TotalCount;

                    svStatisticInfo.Visibility = Visibility.Collapsed;
                    if (result.TotalCount > 0 && result.Statistic != null && result.Statistic.Count != 0)
                    {
                        //显示统计信息
                        tbStatisticInfo.Text = result.Statistic.ToStatisticText();
                        svStatisticInfo.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        tbStatisticInfo.Text = string.Empty;
                        this.svStatisticInfo.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
        }

        private void dgSaleIncomeQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.dgSaleIncomeQueryResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }

            ColumnSet col = new ColumnSet(this.dgSaleIncomeQueryResult);
            col.Insert(3, "OrderType", ResSaleIncomeQuery.Grid_OrderType);
            //col.Insert(7, "PayTypeName", ResSaleIncomeQuery.Grid_PayType);
            // col.Insert(8, "ShipTypeName", ResSaleIncomeQuery.Grid_ShipType);
            col.Insert(17, "ReturnCashAmt", ResSaleIncomeQuery.Grid_ReturnCash);
            col.Insert(19, "ReturnPointAmt", ResSaleIncomeQuery.Grid_ReturnPoint);
            col.Insert(20, "ToleranceAmt", ResSaleIncomeQuery.Grid_ToleranceAmt);
            col.Insert(28, "BankInfo", ResSaleIncomeQuery.Grid_BankInfo);

            soIncomeFacade.ExportExcelFile(lastQueryVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 导出RO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportRO_Click(object sender, RoutedEventArgs e)
        {
            if (lastQueryVM == null || this.dgSaleIncomeQueryResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }

            var flag = ValidationManager.Validate(this.gridQueryBuilder);
            if (flag)
            {
                if (lastQueryVM.OrderType != SOIncomeOrderType.RO)
                {
                    lastQueryVM.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(ResSaleIncomeQuery.Message_PlsChooseROTypeDlgTitle
                        , new string[] { "OrderType" }));
                    Window.Alert(ResSaleIncomeQuery.Message_PlsChooseROTypeDlgTitle);
                    return;
                }
                else if (!lastQueryVM.RORefundDateFrom.HasValue && !lastQueryVM.RORefundDateTo.HasValue)
                {
                    lastQueryVM.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(ResSaleIncomeQuery.Message_PlsChooseROReturnTimeDlgTitle
                        , new string[] { "RORefundDateFrom", "RORefundDateTo" }));
                    Window.Alert(ResSaleIncomeQuery.Message_PlsChooseROReturnTimeDlgTitle);
                    return;
                }

                ColumnSet col = new ColumnSet();
                col.Add("RefundID", ResSaleIncomeQuery.Grid_RefundID, 20);
                col.Add("CashAmt", ResSaleIncomeQuery.Grid_RefundCashAmt);
                col.Add("RefoundTime", ResSaleIncomeQuery.Grid_RefundTime);
                col.Add("SOSysNo", ResSaleIncomeQuery.Grid_SOSysNo);
                col.Add("OutTime", ResSaleIncomeQuery.Grid_OutTime);

                soIncomeFacade.ExportROExcelFile(lastQueryVM, new ColumnSet[] { col });
            }
        }

        private void cbxOrderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamicFilter.Children.Clear();
            if (e.AddedItems.Count > 0)
            {
                UserControl dynamicFilterUC = null;
                KeyValuePair<SOIncomeOrderType?, string> selectedVal = (KeyValuePair<SOIncomeOrderType?, string>)e.AddedItems[0];
                if (selectedVal.Key.HasValue)
                {
                    string key = string.Format("DynamicQueryFilter{0}", selectedVal.Key);
                    if (DynamicQueryFilters.ContainsKey(key))
                    {
                        dynamicFilterUC = DynamicQueryFilters[key];
                    }
                }

                if (dynamicFilterUC != null)
                    dynamicFilter.Children.Add(dynamicFilterUC);
            }

            CheckCashOnDeliveryTypeVisable();
        }

        /// <summary>
        /// 收款单确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSOIncomeSysNoList();
            if (selectedSysNoList == null)
            {
                return;
            }
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }


            //放到了服务端验证
            #region 货到付款订单确认时判断订单实收金额和支付金额是否一致，如果不一致则确认不通过，提示信息：订单11111实收金额（PayAmount），现金支付：120，银行卡支付：129，预付款：100

            //string msgInfo = "";
            //List<SaleIncomeVM> datasource = (List<SaleIncomeVM>)dgSaleIncomeQueryResult.ItemsSource;
            //foreach (var sub in selectedSysNoList)
            //{
            //    var target = datasource.First(m => (m.SysNo == sub));
            //    var DecimalIsNull = new Func<decimal?, decimal>(decimalObject => { return decimalObject == null ? 0 : decimalObject.Value; });
            //    if (target.IncomeStyle == SOIncomeOrderStyle.Normal)//货到付款
            //    {
            //        //check
            //        var posAmount = DecimalIsNull(target.PosPrePay) + DecimalIsNull(target.PosCash) + DecimalIsNull(target.PosBankCard);
            //        var orderAmount = DecimalIsNull(target.OrderAmt.Value);
            //        var realGetAmount = DecimalIsNull(target.IncomeAmt.Value);
            //        if (posAmount != realGetAmount)
            //        {
            //            var msg = "";
            //            msg += string.Format("订单号{0} 实收金额{1}，现金支付{2}，银行支付{3}，预付款{4}。",
            //                target.OrderSysNo.Value.ToString(),
            //                realGetAmount.ToString("#0.00"),
            //                DecimalIsNull(target.PosCash),
            //                DecimalIsNull(target.PosBankCard),
            //                DecimalIsNull(target.PosPrePay));
            //            msg += Environment.NewLine;
            //            msgInfo += msg;
            //        }
            //    }
            //}
            //if (string.IsNullOrEmpty(msgInfo) == false)
            //{
            //    msgInfo = "订单实收金额和支付金额不一致，无法确认。" + Environment.NewLine + msgInfo;
            //    Window.Alert(msgInfo);
            //    return;
            //}
            #endregion

            bool hasRight = false;
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_Confirm_AnyRefundType))
            {
                hasRight = true;
            }

            soIncomeFacade.BatchConfirm(selectedSysNoList, hasRight, msg =>
            {
                Window.Alert(msg, () => this.dgSaleIncomeQueryResult.Bind());
            });
        }

        /// <summary>
        /// 收款单取消确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntites = GetSelectedSOIncomeList();
            if (selectedEntites.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            var sysNoList = new List<int>();
            var cancelSysNoList = new List<int>();

            selectedEntites.ForEach(w =>
            {
                bool isCanceled = true; //是否取消选择
                bool isConfirmed = false;
                DateTime confirmDate = DateTime.MinValue;

                if (w.IncomeStatus == SOIncomeStatus.Confirmed)
                {
                    isConfirmed = w.ConfirmTime.HasValue;
                    confirmDate = w.ConfirmTime ?? DateTime.MinValue;
                }

                #region 判断权限，如果不满足收款单确认的条件，则不向服务端提交

                if (isConfirmed && AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_CancelConfirm_InConfirmDay))
                {
                    if (DateTime.Now.Subtract(confirmDate).Days == 0)
                    {
                        isCanceled = false;
                    }
                }
                if (isConfirmed && AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeQuery_CancelConfirm_AfterConfirmDay))
                {
                    if (DateTime.Now.Subtract(confirmDate).Days > 0)
                    {
                        isCanceled = false;
                    }
                }

                #endregion 判断权限，如果不满足收款单确认的条件，则不向服务端提交

                if (!isCanceled)
                {
                    sysNoList.Add(w.SysNo.Value);
                }
                else
                {
                    cancelSysNoList.Add(w.SysNo.Value);
                }
            });

            soIncomeFacade.BatchCancelConfirm(sysNoList, msg =>
            {
                if (cancelSysNoList.Count > 0)
                {
                    //fixbug:修正提示信息
                    int matchIndex = 0;
                    msg = System.Text.RegularExpressions.Regex.Replace(msg, "(\\d+)", match =>
                    {
                        matchIndex++;
                        if (matchIndex == 1 || matchIndex == 3)
                        {
                            return (int.Parse(match.Value) + cancelSysNoList.Count).ToString();
                        }
                        return match.Value;
                    });

                    System.Text.StringBuilder cancelMsg = new System.Text.StringBuilder();
                    cancelMsg.AppendLine(ResSaleIncomeQuery.Message_RecordsNotSubmit);
                    cancelSysNoList.ForEach(w =>
                    {
                        cancelMsg.AppendLine(string.Format(ResSaleIncomeQuery.Message_NoOperatePermissionFormat, w));
                    });
                    msg += cancelMsg.ToString();
                }
                Window.Alert(msg, () => this.dgSaleIncomeQueryResult.Bind());
            });
        }

        /// <summary>
        /// 收款单手动退款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManualRefund_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSOIncomeSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            soIncomeFacade.BatchManualRefund(selectedSysNoList, msg =>
            {
                Window.Alert(msg, () => this.dgSaleIncomeQueryResult.Bind());
            });
        }

        /// <summary>
        /// 收款单作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSOIncomeSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            soIncomeFacade.BatchAbandon(selectedSysNoList, msg =>
            {
                Window.Alert(msg, () => this.dgSaleIncomeQueryResult.Bind());
            });
        }

        /// <summary>
        /// 收款单自动确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoConfirm_Click(object sender, RoutedEventArgs e)
        {
            //跳转到收款单自动确认页面
            Window.Navigate(ConstValue.SaleIncomeAutoConfirmUrl, null, true);
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchSetReferenceID_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSOIncomeSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            new UCReferenceIDSetter().ShowDialog(ResSaleIncomeQuery.Message_BatchSetReferenceIDDlgTitle, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    soIncomeFacade.BatchSetReferenceID(selectedSysNoList, args.Data.ToString(), msg =>
                    {
                        Window.Alert(msg, () => this.dgSaleIncomeQueryResult.Bind());
                    });
                }
            });
        }

        private void btnSOSyncSAP_Click(object sender, RoutedEventArgs e)
        {
            lastQueryVM.ByCustomer = (rbByCustomer.IsChecked ?? false) ? "1"
                : (rbByOrder.IsChecked ?? false) ? "2" : "0";

            if (this.dgSaleIncomeQueryResult.ItemsSource == null)
            {
                Window.Alert(ResSaleIncomeQuery.Msg_NoSearchResult);
                return;
            }
            if (lastQueryVM.OrderType == null)
            {
                Window.Alert(ResSaleIncomeQuery.Msg_SelectOrderType);
                return;
            }

            soIncomeFacade.SyncSAPSales(lastQueryVM, 500000, 0, "SysNo desc",
                () =>
                {
                    Window.Alert(ResSaleIncomeQuery.Msg_SyncSAPSalesSuccess);
                });
        }

        private void Hyperlink_ShowFailedDetail_Click(object sender, RoutedEventArgs e)
        {
            SaleIncomeVM vm = (sender as HyperlinkButton).DataContext as SaleIncomeVM;
            if (vm.SapInFailedReason != null)
            {
                Window.Alert(vm.SapInFailedReason);
            }
        }
        private void rbByOrder_Checked(object sender, RoutedEventArgs e)
        {
            if (rbByOrder.IsChecked.HasValue && rbByOrder.IsChecked.Value)
            {
                cbxOrderType.SelectedIndex = 1;
            }
        }
        #endregion Event Handler

        #region Private Methods

        private void InitData()
        {
            queryVM = new SaleIncomeQueryVM();
            gridQueryBuilder.DataContext = lastQueryVM = queryVM;
        }

        private void RegisterDynamicQueryFilters()
        {
            DynamicQueryFilters.Add(string.Format("DynamicQueryFilter{0}", SOIncomeOrderType.SO), new DynamicFilterSO());
            DynamicQueryFilters.Add(string.Format("DynamicQueryFilter{0}", SOIncomeOrderType.AO), new DynamicFilterAO());
            DynamicQueryFilters.Add(string.Format("DynamicQueryFilter{0}", SOIncomeOrderType.RO), new DynamicFilterRO());
            DynamicQueryFilters.Add(string.Format("DynamicQueryFilter{0}", SOIncomeOrderType.RO_Balance), new DynamicFilterRO_Balance());
            DynamicQueryFilters.Add(string.Format("DynamicQueryFilter{0}", SOIncomeOrderType.OverPayment), new DynamicFilterOverpayment());
        }

        private void LoadComboBoxData()
        {
            //默认选中第一个销售渠道
            this.cmbWebChannel.SelectedIndex = 0;

            //延迟加载收款单审核人列表 & 投递员列表列表
            otherFacade.GetBizOperationUser(new BizOperationUserQueryFilter()
            {
                BizTableName = "IPP3.dbo.Finance_SOIncome",
                UserType = BizOperationUserType.ConfirmUser,
                UserStatus = BizOperationUserStatus.Active,
                CompanyCode = CPApplication.Current.CompanyCode
            }, true, confirmerList =>
           {
               queryVM.IncomeConfirmerList = confirmerList;
           });

            otherFacade.GetFreightManList(true, freightManList =>
            {
                queryVM.FreightMenList = freightManList;
            });
        }

        private List<int> GetSelectedSOIncomeSysNoList()
        {
            var selectedEntities = GetSelectedSOIncomeList();
            //foreach (var item in selectedEntities)
            //{
            //    if (item.RMARefundPayType.HasValue && item.RMARefundPayType == RefundPayType.NetWorkRefund &&
            //        item.OrderType.HasValue && item.OrderType == SOIncomeOrderType.AO && item.PayTypeSysNo.HasValue && item.PayTypeSysNo == 112)
            //    {
            //        Window.Alert("支付宝方式的网管直退财务负收款单请使用支付宝批量退款！");
            //        return null;
            //    }
            //}
            return selectedEntities
                .Select(s => s.SysNo.Value)
                .ToList();
        }

        private List<SaleIncomeVM> GetSelectedSOIncomeList()
        {
            var selectedEntities = new List<SaleIncomeVM>();
            var itemSource = this.dgSaleIncomeQueryResult.ItemsSource as List<SaleIncomeVM>;
            if (itemSource != null)
            {
                selectedEntities = itemSource.Where(w => w.IsChecked == true)
                    .Select(s => s)
                    .ToList();
            }
            return selectedEntities;
        }

        #endregion Private Methods

        private void btnForcesConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSOIncomeSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            soIncomeFacade.BatchForcesConfirm(selectedSysNoList, msg =>
            {
                Window.Alert(msg, () => this.dgSaleIncomeQueryResult.Bind());
            });
        }
    }
}