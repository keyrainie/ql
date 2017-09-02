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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class UnbilledRuleListReport : PageBase
    {
        UnbilledRuleListQueryFilter m_queryRequest;

        public UnbilledRuleListReport()
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
            this.SearchCondition.DataContext = m_queryRequest = new UnbilledRuleListQueryFilter();
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
            facade.QueryUnbilledRuleList(m_queryRequest, (obj, args) => {
                if (args.FaultsHandle())
                    return;
                this.dgQueryResult.TotalCount = args.Result.TotalCount;
                this.dgQueryResult.ItemsSource = args.Result.Rows;
            });
        }

        /// <summary>
        /// 绑定下拉框数据
        /// </summary>
        private void BindComboBoxData()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_EIMSType ,CodeNamePairAppendItemType.All, (o, p) =>
            {
                this.cmbEIMSType.ItemsSource = p.Result;
                this.cmbEIMSType.SelectedIndex = 0;
            });
        }

        /// <summary>
        /// 导出全部数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_UnbilledRuleList_Export))
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
            facade.ExportUnbilledRuleList(m_queryRequest, new ColumnSet[] { col });
        }
    }
}
