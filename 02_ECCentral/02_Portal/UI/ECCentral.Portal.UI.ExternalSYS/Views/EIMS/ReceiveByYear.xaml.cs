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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class ReceiveByYear : PageBase
    {
        ReceivedReportQueryFilter m_queryRequest;
        public ReceiveByYear()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.SearchCondition.DataContext = m_queryRequest = new ReceivedReportQueryFilter();
            BindComboBoxData();
            this.dgQueryResult.Bind();
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
            ReceivedReportFacade facade = new ReceivedReportFacade(this);
            facade.ReceiveByYearQuery(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.dgQueryResult.TotalCount = args.Result.TotalCount;
                this.dgQueryResult.ItemsSource = args.Result.Rows;
            });
        }

        /// <summary>
        /// 绑定下拉列表数据
        /// </summary>
        private void BindComboBoxData()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_EIMSType,
               CodeNamePairAppendItemType.All, (o, p) =>
               {
                   this.cmbFeeType.ItemsSource = p.Result;
                   this.cmbFeeType.SelectedIndex = 0;
               });

            for (int i = 2011; i <= 2050; i++)
            {
                this.cmbYear.Items.Add(i);
            }
            this.cmbYear.SelectedIndex = 0;
        }

        /// <summary>
        /// 导出报表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_ReceiveByYear_Export))
            {
                Window.Alert(ResEIMSReceiveReport.Msg_HasNoRight);
                return;
            }
            if (dgQueryResult.ItemsSource == null)
            {
                Window.Alert(ResEIMSReceiveReport.Msg_PleaseQueryData);
                return;
            }
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            ReceivedReportFacade facade = new ReceivedReportFacade(this);

            ColumnSet col = new ColumnSet();

            col.Insert(0, "VendorNumber", "供应商编号", 10);
            col.Insert(1, "VendorName", "供应商名称", 25);
            col.Insert(2, "TotalReceiveAmount", "总收款金额", 25);

            col.Insert(3, "RAJanuary", "1月", 10);
            col.Insert(4, "RAFebruary", "2月", 10);
            col.Insert(5, "RAMarch", "3月", 10);
            col.Insert(6, "RAApril", "4月", 10);
            col.Insert(7, "RAMay", "5月", 10);
            col.Insert(8, "RAJune", "6月", 10);
            col.Insert(9, "RAJuly", "7月", 10);
            col.Insert(10, "RAAugust", "8月", 10);
            col.Insert(11, "RASeptember", "9月", 10);
            col.Insert(12, "RAOctober", "10月", 10);
            col.Insert(13, "RANovember", "11月", 10);
            col.Insert(14, "RADecember", "12月", 10);

            facade.ExportReceiveByYear(m_queryRequest, new ColumnSet[] { col });
        }
    }
}
