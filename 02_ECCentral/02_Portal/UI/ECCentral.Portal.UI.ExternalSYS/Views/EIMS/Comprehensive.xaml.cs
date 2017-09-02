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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class Comprehensive : PageBase
    {
        EIMSComprehensiveQueryFilter m_queryRequest;

        public Comprehensive()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void IniPageData()
        {
            this.SearchCondition.DataContext = m_queryRequest = new EIMSComprehensiveQueryFilter();
            BindComboBoxData();
        }

        /// <summary>
        /// 查询按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.dgQueryResult.Bind();
        }

        private void dgQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            ComprehensiveReportFacade facade = new ComprehensiveReportFacade(this);
            facade.QueryComprehensive(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.dgQueryResult.TotalCount = args.Result.TotalCount;
                this.dgQueryResult.ItemsSource = args.Result.Rows;
            });
        }

        /// <summary>
        /// 重置按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtInvoiceNo.Text = txtRuleNo.Text = string.Empty;
            cmbRuleStatus.SelectedIndex = cmbInvoiceStatus.SelectedIndex = cmbEIMSType.SelectedIndex = 0;
            dateApproved.SelectedRangeType = RangeType.None;
            dateCycle.SelectedRangeType = RangeType.None;
            ucVendorPicker.SelectedVendorSysNo = null;
        }

        /// <summary>
        /// 绑定下拉列表数据
        /// </summary>
        private void BindComboBoxData()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS,
               new string[] { ConstValue.Key_EIMSType, ConstValue.Key_RuleStatus, ConstValue.Key_InvoiceStatus },
               CodeNamePairAppendItemType.All, (o, p) =>
               {
                   this.cmbEIMSType.ItemsSource = p.Result[ConstValue.Key_EIMSType];
                   this.cmbEIMSType.SelectedIndex = 0;

                   this.cmbRuleStatus.ItemsSource = p.Result[ConstValue.Key_RuleStatus];
                   this.cmbRuleStatus.SelectedIndex = 0;

                   this.cmbInvoiceStatus.ItemsSource = p.Result[ConstValue.Key_InvoiceStatus];
                   this.cmbInvoiceStatus.SelectedIndex = 0;
               });
        }

        /// <summary>
        /// 导出所有数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_Comprehensive_Export))
            {
                Window.Alert(ResEIMSComprehensive.Msg_HasNoRight);
                return;
            }
            if (dgQueryResult.ItemsSource == null)
            {
                Window.Alert(ResEIMSComprehensive.Msg_PleaseQueryData);
                return;
            }
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            ComprehensiveReportFacade facade = new ComprehensiveReportFacade(this);

            ColumnSet col = new ColumnSet(dgQueryResult);

            col.Insert(9, "BeginBalanceAccrued", "期初余额_应计金额", 20);
            col.Insert(10, "BeginBalanceTax", "期初余额_税金", 20);
            col.Insert(11, "BeginBalance", "期初余额_总额", 20);

            col.Insert(12, "EndBalanceAccrued", "期末余额_应计金额", 20);
            col.Insert(13, "EndBalanceTax", "期末余额_税金", 20);
            col.Insert(14, "EndBalance", "期末余额_总额", 20);

            col.Insert(20, "InvoiceAmountAccrued", "单据金额_应计金额", 20);
            col.Insert(21, "InvoiceAmountTax", "单据金额_税金", 20);
            col.Insert(22, "InvoiceAmount", "单据金额_总额", 20);

            facade.ExportComprehensive(m_queryRequest, new ColumnSet[] { col });
        }
    }
}
