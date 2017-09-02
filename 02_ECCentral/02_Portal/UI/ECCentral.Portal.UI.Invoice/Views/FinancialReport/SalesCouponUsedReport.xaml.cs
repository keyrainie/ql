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
using System.Windows.Shapes;
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Invoice.Views.FinancialReport
{
    [View(IsSingleton = true)]
    public partial class SalesCouponUsedReport : PageBase
    {
        #region 初始化
        private ResSalesCouponUsedReportQueryVM QueryVM = null;
        private CouponUsedReportFacade facade = null;
        private SalesCouponUsedReportQueryView ResultModel = null;
        public SalesCouponUsedReport()
        {
            InitializeComponent();
        }
        private void IniPageData()
        {
            QueryVM = new ResSalesCouponUsedReportQueryVM();
            facade = new CouponUsedReportFacade(this);
            ResultModel = new SalesCouponUsedReportQueryView();
            gridQueryBuilder.DataContext = QueryVM;
            dgdResult.DataContext = ResultModel;

        }
        #endregion

        private void SalesCouponUsedReport_OnOnLoad(object sender, EventArgs e)
        {
            IniPageData();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            bool flag = ValidationManager.Validate(this.gridQueryBuilder);
            if (flag)
            {
                this.dgdResult.Bind();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //重置查询条件:
            IniPageData();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryCouponUsedReport(QueryVM, e.PageSize, e.PageIndex, e.SortField, resultVM =>
            {
                dgdResult.ItemsSource = resultVM.Result;
                dgdResult.TotalCount = resultVM.TotalCount;

                if (resultVM.Result != null && resultVM.Result.Count > 0)
                {
                    svStatisticInfo.Visibility = Visibility.Visible;
                    txtCurrentPageStatisticInfo.Text = "本页小计：--优惠券折扣金额：￥" + resultVM.Staticsticinfo[0].CurrentPageAmount;
                    txtAllStatisticInfo.Text = "总    计：--优惠券折扣金额：￥" + resultVM.Staticsticinfo[0].TotalAmount;
                }
                else
                {
                    svStatisticInfo.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void DataGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (QueryVM == null || this.dgdResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }
            facade.ExportCouponUsedReport(QueryVM, new[] { new ColumnSet(dgdResult) });
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.SOMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void hybtnCouponName_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = dgdResult.SelectedItem;
            if (row == null) return;
            var sysno = row.CouponSysNo;
            this.Window.Navigate(string.Format(ConstValue.MKT_CouponMaintainUrlFormat + "?sysno={0}", sysno), null, true);
        }
    }
}
