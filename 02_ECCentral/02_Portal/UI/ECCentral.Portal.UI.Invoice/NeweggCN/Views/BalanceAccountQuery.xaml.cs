using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Views
{
    /// <summary>
    /// 余额账户预收查询
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class BalanceAccountQuery : PageBase
    {
        private BalanceAccountFacade _facade;
        private BalanceAccountQueryVM _queryVM;
        private BalanceAccountQueryVM _lastQueryVM;

        public BalanceAccountQuery()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            _queryVM = new BalanceAccountQueryVM();
            this.QueryBuilder.DataContext = _lastQueryVM = _queryVM;
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermissions();
            base.OnPageLoad(sender, e);
            _facade = new BalanceAccountFacade(this);
            this.cmbWebChannel.SelectedIndex = 0;
        }

        private void VerifyPermissions()
        {
            this.DataGrid.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceAccountQuery_Export);

        }

        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_BalanceAccountQuery_Query))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var flag = ValidationManager.Validate(this.QueryBuilder);
            if (flag)
            {
                this._lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<BalanceAccountQueryVM>(_queryVM);

                this.DataGrid.Bind();
            }
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _facade.Query(_lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                this.DataGrid.ItemsSource = result[0].Rows.ToList("IsChecked", false);
                this.DataGrid.TotalCount = result[0].TotalCount;

                this.tbStatisticInfo.Visibility = Visibility.Collapsed;
                if (result[1] != null && !(result[1].Rows is DynamicXml.EmptyList))
                {
                    string totalInfo = string.Format(ResBalanceAccountQuery.Message_TotalInfo,
                        ConstValue.Invoice_ToCurrencyString(result[1].Rows[0].TotalStartBalance)
                        , ConstValue.Invoice_ToCurrencyString(result[1].Rows[0].PayedIn)
                        , ConstValue.Invoice_ToCurrencyString(result[1].Rows[0].PayedOut)
                        , ConstValue.Invoice_ToCurrencyString(result[1].Rows[0].TotalEndBalance));
                    this.tbStatisticInfo.Text = totalInfo;
                    this.tbStatisticInfo.Visibility = Visibility.Visible;
                }
            });
        }

        private void DataGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (_lastQueryVM == null || this.DataGrid.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }

            ColumnSet col = new ColumnSet(this.DataGrid);
            _facade.ExportExcelFile(_lastQueryVM, new ColumnSet[] { col });
        }
    }
}