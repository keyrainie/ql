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
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class AccruedByRule : PageBase
    {
        AccruedQueryFilter m_queryRequest;

        public AccruedByRule()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.SearchCondition.DataContext = m_queryRequest = new AccruedQueryFilter();
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
            AccruedReportFacade facade = new AccruedReportFacade(this);
            facade.AccruedByRule(m_queryRequest, (obj, args) =>
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
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS,ConstValue.Key_EIMSType,
               CodeNamePairAppendItemType.All, (o, p) =>
               {
                   this.cmbEIMSType.ItemsSource = p.Result;
                   this.cmbEIMSType.SelectedIndex = 0;
               });
        }

        /// <summary>
        /// 导出报表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_AccruedByRule_Export))
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

                col.Insert(5, "BeginBalanceAccrued", "期初余额_应计金额", 20);
                col.Insert(6, "BeginBalanceTax", "期初余额_税金", 20);
                col.Insert(7, "BeginBalance", "期初余额_总额", 20);

                col.Insert(8, "curAccruedAmount", "当期返利_应计金额", 20);
                col.Insert(9, "AccruedAmountTax", "当期返利_税金", 20);
                col.Insert(10, "AccruedAmount", "当期返利_总额", 20);

                col.Insert(11, "ReceivedPO", "已收返利金额(含税金额)_PO单扣减", 20);
                col.Insert(12, "ReceivedConsign", "已收返利金额(含税金额)_代销结算单扣减", 20);
                col.Insert(13, "ReceivedCash", "已收返利金额(含税金额)_现金", 20);
                col.Insert(14, "ReceivedAcctDeduct", "已收返利金额(含税金额)_帐扣", 20);
                col.Insert(15, "ReceivedVoteBuckle", "已收返利金额(含税金额)_票扣", 20);
                col.Insert(16, "ReceivedAmount", "已收返利金额(含税金额)_总计", 20);

                col.Insert(17, "EndBalanceAccrued", "期末余额_应计金额", 20);
                col.Insert(18, "EndBalanceTax", "期末余额_税金", 20);
                col.Insert(19, "EndBalance", "期末余额_总额", 20);

                facade.ExportAccruedByRule(m_queryRequest, new ColumnSet[] { col });
            }
            else
            {
                Window.Alert(ResEIMSAccrued.Msg_PleaseQueryData);
                return;
            }
        }
    }
}
