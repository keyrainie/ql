using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.UI.Invoice.NeweggCN.UserControls;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Views
{
    /// <summary>
    /// 客户余额退款查询页面
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class BalanceRefundQuery : PageBase
    {
        #region Private Fields

        private BalanceRefundFacade _facade;
        private BalanceRefundQueryVM _queryVM;
        private BalanceRefundQueryVM _lastQueryVM;

        #endregion Private Fields

        #region Constructor

        public BalanceRefundQuery()
        {
            InitializeComponent();

            InitData();
        }

        #endregion Constructor

        #region Event Handlers

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermission();
            base.OnPageLoad(sender, e);
            _facade = new BalanceRefundFacade(this);
            this.cmbWebChannel.SelectedIndex = 0;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.QueryBuilder);
            if (flag)
            {
                this._lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<BalanceRefundQueryVM>(_queryVM);

                this.DataGrid_BalanceRefund.Bind();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_BalanceRefund_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _facade.Query(_lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                this.DataGrid_BalanceRefund.ItemsSource = result[0].Rows.ToList("IsChecked", false);
                this.DataGrid_BalanceRefund.TotalCount = result[0].TotalCount;

                this.tbStatisticInfo.Visibility = Visibility.Collapsed;
                if (result[1] != null && !(result[1].Rows is DynamicXml.EmptyList))
                {
                    string totalInfo = string.Format("所有统计 --- 退款金额总计(财务审核通过): {0}", ConstValue.Invoice_ToCurrencyString(result[1].Rows[0].TotalAmount));
                    this.tbStatisticInfo.Text = totalInfo;
                    this.tbStatisticInfo.Visibility = Visibility.Visible;
                }
            });
        }

        /// <summary>
        /// 导出全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_BalanceRefund_ExportAllClick(object sender, EventArgs e)
        {
            if (_lastQueryVM == null || this.DataGrid_BalanceRefund.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }

            ColumnSet col = new ColumnSet(this.DataGrid_BalanceRefund);
            _facade.ExportExcelFile(_lastQueryVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.DataGrid_BalanceRefund.ItemsSource;
            if (dataSource != null)
            {
                var ckbAll = sender as CheckBox;
                foreach (dynamic item in dataSource)
                {
                    item.IsChecked = ckbAll.IsChecked ?? false;
                }
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        {
            var data = DataGrid_BalanceRefund.SelectedItem as dynamic;
            if (data != null)
            {
                BalanceRefundVM refundVM = DynamicConverter<BalanceRefundVM>.ConvertToVM(data);
                UCBalanceRefundMaintain uc = new UCBalanceRefundMaintain(refundVM, _facade);
                uc.ShowDialog("修改客户余额退款", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        this.DataGrid_BalanceRefund.Bind();
                    }
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
            var data = DataGrid_BalanceRefund.SelectedItem as dynamic;
            if (data != null)
            {
                BalanceRefundVM refundVM = DynamicConverter<BalanceRefundVM>.ConvertToVM(data);
                UCBalanceRefundMaintain uc = new UCBalanceRefundMaintain(refundVM);
                uc.ShowDialog("查看客户余额退款", null);
            }
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_ReferenceID_Click(object sender, RoutedEventArgs e)
        {
            var data = DataGrid_BalanceRefund.SelectedItem as dynamic;
            if (data != null)
            {
                BalanceRefundVM refundVM = DynamicConverter<BalanceRefundVM>.ConvertToVM(data);
                UCBalanceRefundReferenceIDSetter uc = new UCBalanceRefundReferenceIDSetter(refundVM, _facade);
                uc.ShowDialog("设置", (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        this.DataGrid_BalanceRefund.Bind();
                    }
                });
            }
        }

        /// <summary>
        /// 合计选择项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTotal_Click(object sender, RoutedEventArgs e)
        {
            var selectedItemList = GetSelectedItemList();
            if (selectedItemList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            decimal totalRefundAmt = 0;
            foreach (dynamic item in selectedItemList)
            {
                totalRefundAmt += item.ReturnPrepayAmt;
            }
            string totalInfo = string.Format("共选择了{0}条记录; 合计退款金额:{1}", selectedItemList.Count, ConstValue.Invoice_ToCurrencyString(totalRefundAmt));
            this.tbStatisticInfo.Text = totalInfo;
            this.tbStatisticInfo.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 客服审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCSAudit_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            _facade.BatchCSConfirm(selectedSysNoList, msg =>
                Window.Alert(msg, () => this.DataGrid_BalanceRefund.Bind()));
        }

        /// <summary>
        /// 财务审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFinAudit_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            _facade.BatchFinConfirm(selectedSysNoList, msg =>
                Window.Alert(msg, () => this.DataGrid_BalanceRefund.Bind()));
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            _facade.BatchCancelConfirm(selectedSysNoList, msg =>
                Window.Alert(msg, () => this.DataGrid_BalanceRefund.Bind()));
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            _facade.BatchAbandon(selectedSysNoList, msg =>
                Window.Alert(msg, () => this.DataGrid_BalanceRefund.Bind()));
        }

        /// <summary>
        /// 导入SAP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportSAP_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetReferenceID_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            new UCReferenceIDSetter().ShowDialog("批量设置凭证号", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    _facade.BatchSetReferenceID(selectedSysNoList, args.Data.ToString(), msg =>
                    {
                        Window.Alert(msg, () => this.DataGrid_BalanceRefund.Bind());
                    });
                }
            });
        }

        #endregion Event Handlers

        #region Private Methods

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            _queryVM = new BalanceRefundQueryVM();
            this.QueryBuilder.DataContext = _lastQueryVM = _queryVM;
        }

        /// <summary>
        /// 验证权限
        /// </summary>
        private void VerifyPermission()
        {
            this.btnCSAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceRefundQuery_CSAudit);
            this.btnFinAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceRefundQuery_FinAudit);
            this.btnCancelAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceRefundQuery_CancelAudit);
            this.btnAbandon.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceRefundQuery_Abandon);           
            this.btnSetReferenceID.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceRefundQuery_SetReferenceID);
            this.DataGrid_BalanceRefund.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceRefundQuery_Export);
        }

        /// <summary>
        /// 取得选中记录的系统编号列表
        /// </summary>
        /// <returns></returns>
        private List<int> GetSelectedSysNoList()
        {
            List<int> getSelected = new List<int>();
            var dataSource = this.DataGrid_BalanceRefund.ItemsSource;
            if (dataSource != null)
            {
                foreach (dynamic item in dataSource)
                {
                    if (item.IsChecked)
                    {
                        getSelected.Add(item.SysNo);
                    }
                }
            }
            return getSelected;
        }

        /// <summary>
        /// 取得选中记录
        /// </summary>
        /// <returns></returns>
        private List<dynamic> GetSelectedItemList()
        {
            List<dynamic> getSelected = new List<dynamic>();
            var dataSource = this.DataGrid_BalanceRefund.ItemsSource;
            if (dataSource != null)
            {
                foreach (dynamic item in dataSource)
                {
                    if (item.IsChecked)
                    {
                        getSelected.Add(item);
                    }
                }
            }
            return getSelected;
        }

        /// <summary>
        /// 客户编号跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_CustomerSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.DataGrid_BalanceRefund.SelectedItem as dynamic;
            if (customer != null)
            {
                Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, customer.CustomerSysNo), null, true);
            }
        }

        #endregion Private Methods

    }
}