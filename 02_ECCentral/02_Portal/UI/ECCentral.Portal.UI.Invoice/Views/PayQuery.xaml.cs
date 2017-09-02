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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 采购-应付款查询页面
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class PayQuery : PageBase
    {
        private PayableFacade facade;
        private PayableQueryVM queryVM;
        private PayableQueryVM lastQueryVM;
        private CommonDataFacade commonFacade;
        private List<PayableVM> list;

        public PayQuery()
        {
            InitializeComponent();
            
        }

        private void VerifyPermission()
        {
            DataGrid_QueryResult.IsShowExcelExporter = false;
            DataGrid_QueryResult.IsShowAllExcelExporter =  AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayQuery_Export);
           
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new PayableFacade(this);
            VerifyPermission();
            commonFacade = new CommonDataFacade(this);
            LoadComboBoxData();

            queryVM = new PayableQueryVM();
            this.QueryBuilder.DataContext = lastQueryVM = queryVM;
            base.OnPageLoad(sender, e);
            SetControlStatus();
            cmbOrderType.SelectedIndex = 0;
        }

        private void SetControlStatus()
        {
            CheckBox_IsOnlyNegativeOrder.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayQuery_IsOnlyNegativeOrder) ? true : false;
            this.DataGrid_QueryResult.IsShowAllExcelExporter = this.DataGrid_QueryResult.IsShowExcelExporter
                = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayQuery_Export) ? true : false;
        }

        private void LoadComboBoxData()
        {
            commonFacade.GetStockList(true, (obj, args) =>
            {
                this.Combox_StockList.ItemsSource = args.Result;
            });
            Combox_StockList.SelectedIndex = 0;

            facade.GetAllVendorPayTerms(vendorPayTerms =>
            {
                vendorPayTerms.Insert(0, new CodeNamePair()
                {
                    Name = ResCommonEnum.Enum_Select
                });
                Combox_PayPeriodType.ItemsSource = vendorPayTerms;
            });
            Combox_PayPeriodType.SelectedIndex = 0;
        }

        private void DataGrid_QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            ValidationManager.Validate(this.QueryBuilder);
            if (queryVM.HasValidationErrors)
            {
                return;
            }
            facade.Query(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                list = DynamicConverter<PayableVM>.ConvertToVMList(args.Result[0].Rows);
                this.DataGrid_QueryResult.ItemsSource = list;
                this.DataGrid_QueryResult.TotalCount = args.Result[0].TotalCount;

                this.svStatisticInfo.Visibility = Visibility.Visible;
                if (list != null && list.Count > 0)
                {
                    decimal totalPay = 0;
                    decimal totalAlreadyPay = 0;
                    decimal totalUnPay = 0;
                    decimal sapAmount = 0;
                    list.Where(x => x.PayStatus != PayableStatus.Abandon).ToList().ForEach(p =>
                    {
                        totalPay += p.PayableAmt ?? 0;
                        totalAlreadyPay += p.AlreadyPayAmt ?? 0;
                        if (p.OrderType == 0)
                        {
                            sapAmount += (p.InstockAmt ?? 0) - (p.ReturnPoint ?? 0);
                        }
                        else if (Convert.ToInt32(p.OrderType) == 11 || Convert.ToInt32(p.OrderType) == 12)
                        {
                            sapAmount += p.PayableAmt ?? 0;
                        }
                        else
                        {
                            sapAmount += p.InstockAmt ?? 0;
                        }
                    });
                    totalUnPay = totalPay - totalAlreadyPay;
                    Text_CurrentPageStatistic.Text = string.Format(ResPayQuery.Label_CurrentPageStatistic,
                        ConstValue.Invoice_ToCurrencyString(totalPay), ConstValue.Invoice_ToCurrencyString(totalAlreadyPay)
                        , ConstValue.Invoice_ToCurrencyString(totalUnPay), ConstValue.Invoice_ToCurrencyString(sapAmount));

                    decimal puamt = Convert.ToDecimal(args.Result[1].Rows[0]["PUSum"]);
                    decimal apamt = Convert.ToDecimal(args.Result[1].Rows[0]["APSum"]);
                    decimal sapamt = Convert.ToDecimal(args.Result[1].Rows[0]["SapSum"]);
                    Text_TotalStatistic.Text = string.Format(ResPayQuery.Label_TotalStatistic,
                        ConstValue.Invoice_ToCurrencyString(puamt), ConstValue.Invoice_ToCurrencyString(apamt)
                        , ConstValue.Invoice_ToCurrencyString(puamt - apamt), ConstValue.Invoice_ToCurrencyString(sapamt));
                }
                else
                {
                    Text_CurrentPageStatistic.Text = string.Format(ResPayQuery.Label_CurrentPageStatistic,
       ConstValue.Invoice_ToCurrencyString(0), ConstValue.Invoice_ToCurrencyString(0)
       , ConstValue.Invoice_ToCurrencyString(0), ConstValue.Invoice_ToCurrencyString(0));

                    Text_TotalStatistic.Text = string.Format(ResPayQuery.Label_TotalStatistic,
     ConstValue.Invoice_ToCurrencyString(0), ConstValue.Invoice_ToCurrencyString(0)
     , ConstValue.Invoice_ToCurrencyString(0), ConstValue.Invoice_ToCurrencyString(0));
                }
            });
        }

        private void DataGrid_QueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || DataGrid_QueryResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }
            ColumnSet col = new ColumnSet(this.DataGrid_QueryResult);
            facade.ExportExcelFile(lastQueryVM, new ColumnSet[] { col });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PayableQueryVM>(queryVM);

            this.DataGrid_QueryResult.Bind();
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.DataGrid_QueryResult.ItemsSource as dynamic;
                foreach (var view in viewList)
                {
                    view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                }
            }
        }

        private void Combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Text_POStatus.Visibility = Text_POType.Visibility = Text_PayPeriodType.Visibility
                = ComBox_POStatus.Visibility = Combox_POType.Visibility = Combox_PayPeriodType.Visibility 
                = Combox_InStockDate.Visibility=Text_InStockDate.Visibility
                = (queryVM.OrderType == PayableOrderType.PO) ? Visibility.Visible : Visibility.Collapsed;

            Text_VendorSettleStatus.Visibility = Combox_VendorSettleStatus.Visibility
                = queryVM.OrderType == PayableOrderType.VendorSettleOrder ? Visibility.Visible : Visibility.Collapsed;

             CheckBox_IsOnlyNegativeOrder.Visibility = CheckBox_IsNotInStock.Visibility
                = queryVM.OrderType == PayableOrderType.CostChange ? Visibility.Collapsed : Visibility.Visible;

             //Text_FinanceOrderStatus.Visibility = Combox_FinanceOrderStatus.Visibility
             //    = queryVM.OrderType == PayableOrderType.FinanceSettleOrder ? Visibility.Visible : Visibility.Collapsed;

             //Text_BalanceOrderStatus.Visibility = Combox_BalanceOrderStatus.Visibility
             //    = queryVM.OrderType == PayableOrderType.BalanceOrder ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        {
            PayableVM vm = (sender as HyperlinkButton).DataContext as PayableVM;
            string url = string.Format(ConstValue.Invoice_PayItemMaintainUrl, "?PaySysNo=" + vm.PaySysNo);
            Window.Navigate(url, null, true);
        }

        private void Hyperlink_OrderIDLinkUrl(object sender, RoutedEventArgs e)
        {
            PayableVM vm = (sender as HyperlinkButton).DataContext as PayableVM;
            string url = string.Empty;
            switch (vm.OrderType)
            {
                case PayableOrderType.PO:
                case PayableOrderType.POAdjust:
                    url = string.Format(ConstValue.PO_PurchaseOrderMaintain, vm.OrderSysNo);
                    break;
                case PayableOrderType.VendorSettleOrder:
                    //url = vm.IsOldConsignSettle.Value ? string.Format(ConstValue.PO_ConsignMaintain, vm.OrderSysNo) : string.Format(ConstValue.PO_ConsignNewEdit, vm.OrderSysNo);
                    //借的代销结算单页面已经没有在使用了。
                    url = string.Format(ConstValue.PO_ConsignMaintain, vm.OrderSysNo);
                    break;
                case PayableOrderType.RMAPOR:
                    url = string.Format(ConstValue.PO_VendorRMARefundMaintain, vm.OrderSysNo);
                    break;
                case PayableOrderType.CollectionSettlement:
                    url = string.Format(ConstValue.PO_GatherMaintain, vm.OrderSysNo);
                    break;
                case PayableOrderType.Commission:
                    url = string.Format(ConstValue.PO_CommissionItemView, vm.OrderSysNo);
                    break;
                case PayableOrderType.CollectionPayment:
                    url = string.Format(ConstValue.PO_CollectionPaymentItemView, vm.OrderSysNo);
                    break;
                case PayableOrderType.CostChange:
                    url = string.Format(ConstValue.PO_CostChangeMaintain, vm.OrderSysNo);
                    break;
                case PayableOrderType.ConsignAdjust:
                    url = string.Format(ConstValue.PO_ConsignAdjustMaintain, vm.OrderSysNo);
                    break;
                case PayableOrderType.GroupSettle:
                    url = string.Format(ConstValue.PO_GroupSettleMaintain, vm.OrderSysNo);
                    break;
                default:
                    break;
            }
            Window.Navigate(url, null, true);
        }

        private void Hyperlink_UpdateInvoiceStatus(object sender, RoutedEventArgs e)
        {
            PayableVM vm = (sender as HyperlinkButton).DataContext as PayableVM;

            var model = new PayInvoiceMaintainVM();
            model.OrderSysNos = vm.OrderSysNo.ToString();
            model.InvoiceStatus = vm.InvoiceStatus;
            model.InvoiceFactStatus = vm.InvoiceFactStatus;
            model.Note = vm.Note;
            model.SysNos = vm.PaySysNo.ToString();
            UCPayInvoiceMaintain uctl = new UCPayInvoiceMaintain(model);
            uctl.ShowDialog(ResPayQuery.Dialog_InvoiceMaintain, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    dynamic result = args.Data as dynamic;
                    Window.Alert(result.Result.ToString());
                    this.DataGrid_QueryResult.Bind();
                }
            });
        }

        private void Button_BatchUpdateInvoiceStatus_Click(object sender, RoutedEventArgs e)
        {
            List<PayableInfo> selectList = GetSelectedList();
            if (selectList.Count == 0)
            {
                Window.Alert(ResPayQuery.Msg_SelectData);
            }
            else
            {
                var model = new PayInvoiceMaintainVM();
                model.OrderSysNos = string.Join(",", selectList.Select(s => s.OrderSysNo.ToString()).ToList());
                model.InvoiceStatus = selectList.FirstOrDefault().InvoiceStatus;
                model.InvoiceFactStatus = selectList.FirstOrDefault().InvoiceFactStatus;
                model.Note = selectList.FirstOrDefault().Note;
                model.SysNos = string.Join(",", selectList.Select(s => s.SysNo.ToString()).ToList());
                UCPayInvoiceMaintain uctl = new UCPayInvoiceMaintain(model);
                uctl.ShowDialog(ResPayQuery.Dialog_InvoiceMaintain, (s, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        this.DataGrid_QueryResult.Bind();
                    }
                });
            }
        }

        private List<PayableInfo> GetSelectedList()
        {
            List<PayableInfo> payableList = new List<PayableInfo>();
            if (this.DataGrid_QueryResult.ItemsSource != null)
            {
                List<PayableVM> viewList = this.DataGrid_QueryResult.ItemsSource as List<PayableVM>;

                foreach (PayableVM view in viewList)
                {
                    if (view.IsChecked)
                    {
                        payableList.Add(new PayableInfo()
                        {
                            SysNo = view.PaySysNo,
                            OrderSysNo = view.OrderSysNo,
                            InvoiceStatus = view.InvoiceStatus,
                            InvoiceFactStatus = view.InvoiceFactStatus,
                            Note = view.Note
                        });
                    }
                }
            }
            return payableList;
        }

        private void Hyperlink_ShowFailedDetail_Click(object sender, RoutedEventArgs e)
        {
            PayableVM vm = (sender as HyperlinkButton).DataContext as PayableVM;
            if (vm.SapInFailedReason != null)
            {
                Window.Alert(vm.SapInFailedReason);
            }
            //var viewer = new UCMessageViewer();
            //viewer.Message = vm.SapInFailedReason;
            //viewer.ShowDialog(ResPayQuery.Dialog_FailedDetail, null);
        }
    }
}