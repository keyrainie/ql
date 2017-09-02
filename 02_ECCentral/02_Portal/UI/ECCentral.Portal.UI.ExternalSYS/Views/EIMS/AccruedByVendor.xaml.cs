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
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class AccruedByVendor : PageBase
    {
        AccruedQueryFilter m_queryRequest;

        public AccruedByVendor()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.SearchCondition.DataContext = m_queryRequest = new AccruedQueryFilter();
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
            AccruedReportFacade facade = new AccruedReportFacade(this);
            facade.AccruedByVendor(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.dgQueryResult.TotalCount = args.Result.TotalCount;
                this.dgQueryResult.ItemsSource = args.Result.Rows;
            });
        }

        /// <summary>
        /// 导出报表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_AccruedByVendor_Export))
            {
                Window.Alert(ResEIMSAccrued.Msg_HasNoRight);
                return;
            }
            if (dgQueryResult.ItemsSource != null)
            {
                m_queryRequest.PagingInfo = new PagingInfo()
                {
                    PageSize = ConstValue.MaxRowCountLimit,
                    PageIndex = 0,
                    SortBy = string.Empty
                };

                AccruedReportFacade facade = new AccruedReportFacade(this);

                ColumnSet col = new ColumnSet(dgQueryResult);

                col.Insert(3, "BeginBalanceAccrued", "期初余额_应计金额", 20);
                col.Insert(4, "BeginBalanceTax", "期初余额_税金", 20);
                col.Insert(5, "BeginBalance", "期初余额_总额", 20);

                col.Insert(6, "curAccruedAmount", "当期返利_应计金额", 20);
                col.Insert(7, "AccruedAmountTax", "当期返利_税金", 20);
                col.Insert(8, "AccruedAmount", "当期返利_总额", 20);

                col.Insert(9, "ReceivedPO", "已收返利金额(含税金额)_PO单扣减", 20);
                col.Insert(10, "ReceivedConsign", "已收返利金额(含税金额)_代销结算单扣减", 20);
                col.Insert(11, "ReceivedCash", "已收返利金额(含税金额)_现金", 20);
                col.Insert(12, "ReceivedAcctDeduct", "已收返利金额(含税金额)_帐扣", 20);
                col.Insert(13, "ReceivedVoteBuckle", "已收返利金额(含税金额)_票扣", 20);
                col.Insert(14, "ReceivedAmount", "已收返利金额(含税金额)_总计", 20);

                facade.ExportAccruedByVendor(m_queryRequest, new ColumnSet[] { col });
            }
            else
            {
                Window.Alert(ResEIMSAccrued.Msg_PleaseQueryData);
                return;
            }
        }
    }
}
