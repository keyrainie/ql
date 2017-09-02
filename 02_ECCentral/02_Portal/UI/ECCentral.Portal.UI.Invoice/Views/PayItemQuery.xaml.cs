using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 采购-付款单查询
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class PayItemQuery : PageBase
    {
        #region Private Fields

        private PayItemFacade facade;
        private PayItemQueryVM queryVM;
        private PayItemQueryVM lastQueryVM;
        private PayItemQueryStatisticVM statisticInfo;

        #endregion Private Fields

        #region Constructor

        public PayItemQuery()
        {
            InitializeComponent();
            //VerifyPermission();
            InitData();
            Loaded += new RoutedEventHandler(PayItemQuery_Loaded);
        }



        #endregion Constructor

        #region Event Handlers

        /// <summary>
        /// 处理“到期的未付款采购单明细”查询条件勾选事件，
        /// 解决深拷贝查询条件时，付款状态被清空的问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxFilterPOETP_Click(object sender, RoutedEventArgs e)
        {
            queryVM.Status = queryVM.IsFilterPOETP ? PayItemStatus.Origin : (PayItemStatus?)null;
        }

        /// <summary>
        /// 页面加载完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PayItemQuery_Loaded(object sender, RoutedEventArgs e)
        {
            
            Loaded -= new RoutedEventHandler(PayItemQuery_Loaded);
            facade = new PayItemFacade(this);
            LoadComboBoxData();
            cmbOrderType.SelectedIndex = 0;
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermission();
            base.OnPageLoad(sender, e);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.SeachBuilder);
            if (flag)
            {
                this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PayItemQueryVM>(queryVM);

                this.dgPayItemQueryResult.Bind();
            }
        }

        /// <summary>
        /// Grid执行绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPayItemQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryPayItem(lastQueryVM, e.PageSize, e.PageIndex, e.SortField,
                result =>
                {
                    this.dgPayItemQueryResult.ItemsSource = result.ResultList;
                    this.dgPayItemQueryResult.TotalCount = result.TotalCount;

                    if (result.Statistic == null)
                    {
                        svStatisticInfo.Visibility = Visibility.Collapsed;
                        return;
                    }
                    statisticInfo = result.Statistic;
                    tbStatisticInfo.Text = result.Statistic.ToStatisticText();
                    svStatisticInfo.Visibility = Visibility.Visible;
                });
        }

        /// <summary>
        /// 导出付款单列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPayItemQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.dgPayItemQueryResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }
            ColumnSet col = new ColumnSet(this.dgPayItemQueryResult);
            col.Insert(15, "InvoiceUpdate", ResPayItemQuery.Grid_InvoiceUpdate);
            col.SetFormat("EstimatePayTime", ConstValue.Invoice_ShortTimeFormat);
            facade.ExportExcelFile(lastQueryVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.dgPayItemQueryResult.ItemsSource as List<PayItemVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }

        /// <summary>
        /// 添加付款单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(string.Format(ConstValue.PayItemListOrderQueryUrl, ""), null, true);
        }

        /// <summary>
        /// 批量付款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchPay_Click(object sender, RoutedEventArgs e)
        {
            var selectedPayItemList = GetSelectedPayItemList();
            if (selectedPayItemList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }            

            Window.Confirm(ResPayItemQuery.Message_ConfirmBatchPayDlgTitle, () =>
            {
                facade.BatchPay(selectedPayItemList, msg =>
                {
                    Window.Alert(msg, () => this.dgPayItemQueryResult.Bind());
                });
            });
        }

        /// <summary>
        /// 合计选择项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTotalCalc_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.dgPayItemQueryResult.ItemsSource as List<PayItemVM>;
            if (dataSource != null)
            {
                statisticInfo.SelectedPayAmtList = dataSource.Where(w => w.IsChecked).Select(s => s.PayAmt ?? 0M).ToList();
                tbStatisticInfo.Text = statisticInfo.ToStatisticText();
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgPayItemQueryResult.SelectedItem as PayItemVM;
            if (cur != null)
            {
                string url = string.Format(ConstValue.Invoice_PayItemMaintainUrl, "?PaySysNo=" + cur.PaySysNo);
                Window.Navigate(url, null, true);
            }
        }

        /// <summary>
        /// 点击单据编号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_OrderID_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgPayItemQueryResult.SelectedItem as PayItemVM;
            if (cur != null)
            {
                //根据不同的单据类型链接到不同的页面
                string url = string.Empty;
                switch (cur.OrderType)
                {
                    case PayableOrderType.PO:
                    case PayableOrderType.POAdjust:
                        url = string.Format(ConstValue.PO_PurchaseOrderMaintain, cur.OrderSysNo, null, true);
                        break;
                    case PayableOrderType.VendorSettleOrder:
                        url = string.Format(ConstValue.PO_ConsignMaintain, cur.OrderSysNo);
                        break;
                    case PayableOrderType.RMAPOR:
                        url = string.Format(ConstValue.PO_VendorRMARefundMaintain, cur.OrderSysNo);
                        break;
                    case PayableOrderType.CollectionSettlement:
                        url = string.Format(ConstValue.PO_GatherMaintain, cur.OrderSysNo);
                        break;
                    case PayableOrderType.Commission:
                        url = string.Format(ConstValue.PO_CommissionItemView, cur.OrderSysNo);
                        break;
                    case PayableOrderType.CollectionPayment:
                        url = string.Format(ConstValue.PO_CollectionPaymentItemView, cur.OrderSysNo);
                        break;
                    case PayableOrderType.CostChange:
                        url = string.Format(ConstValue.PO_CostChangeMaintain, cur.OrderSysNo);
                        break;
                    case PayableOrderType.ConsignAdjust:
                        url = string.Format(ConstValue.PO_ConsignAdjustMaintain, cur.OrderSysNo);
                        break;
                    case PayableOrderType.GroupSettle:
                        url = string.Format(ConstValue.PO_GroupSettleMaintain, cur.OrderSysNo);
                        break;
                }
                Window.Navigate(url, null, true);
            }
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_ReferenceID_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgPayItemQueryResult.SelectedItem as PayItemVM;
            if (cur != null)
            {
                var uc = new UCPayItemMaintan(cur.DeepCopy(), UCPayItemMaintan.ActionType.SetReferenceID);
                uc.ShowDialog(ResPayItemQuery.Message_SetReferenceIDDlgTitle, (_, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        this.dgPayItemQueryResult.Bind();
                    }
                });
            }
        }

        /// <summary>
        /// 发票状态修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_InvoiceStatus_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgPayItemQueryResult.SelectedItem as PayItemVM;
            if (cur != null)
            {
                var vm = EntityConverter<PayItemVM, PayInvoiceMaintainVM>.Convert(cur, (s, t) =>
                {
                    t.SysNos = s.PaySysNo.ToString();
                    t.OrderSysNos = s.OrderID;
                    t.Note = s.InvoiceNumber;
                });
                var uc = new UCPayInvoiceMaintain(vm);
                uc.ShowDialog(ResPayItemQuery.Message_InvoiceStatusModifyDlgTitle, (_, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        dynamic result = args.Data as dynamic;
                        Window.Alert(result.Result.ToString());
                        this.dgPayItemQueryResult.Bind();
                    }
                });
            }
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchSetReferenceID_Click(object sender, RoutedEventArgs e)
        {
            var selectedPayItemList = GetSelectedPayItemList();
            if (selectedPayItemList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            new UCReferenceIDSetter().ShowDialog(ResPayItemQuery.Message_BatchSetReferenceIDDlgTitle, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    selectedPayItemList.ForEach(w => w.ReferenceID = args.Data.ToString());
                    facade.BatchSetReferenceID(selectedPayItemList, msg =>
                    {
                        Window.Alert(msg, () => this.dgPayItemQueryResult.Bind());
                    });
                }
            });
        }

        #endregion Event Handlers

        #region Private Methods

        private void InitData()
        {
            queryVM = new PayItemQueryVM();
            this.SeachBuilder.DataContext = lastQueryVM = queryVM;
        }

        private void LoadComboBoxData()
        {
            cmbWebChannel.SelectedIndex = 0;

            //加载分仓数据
            new CommonDataFacade(this).GetStockList(true, (obj, args) =>
            {
                this.queryVM.StockList = args.Result;
            });
        }

        /// <summary>
        /// 验证权限，设置控件状态
        /// </summary>
        private void VerifyPermission()
        {
            this.btnBatchPay.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemQuery_Pay);
            
            this.btnBatchSetReferenceID.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemQuery_SetReferenceID);
            this.dgPayItemQueryResult.IsShowAllExcelExporter =  AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PayItemQuery_Export);
        }

        /// <summary>
        /// 取得所有处于选中状态的付款单编号列表
        /// </summary>
        /// <returns></returns>
        private List<PayItemVM> GetSelectedPayItemList()
        {
            var selectedPayItemList = new List<PayItemVM>();
            var dataSource = this.dgPayItemQueryResult.ItemsSource as List<PayItemVM>;
            if (dataSource != null)
            {
                selectedPayItemList = dataSource.Where(w => w.IsChecked)
                    .Select(s => s)
                    .ToList();
            }
            return selectedPayItemList;
        }

        #endregion Private Methods
    }
}