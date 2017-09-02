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
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.ExternalSYS;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class EIMSEventMemo : PageBase
    {
       
        EIMSEventMemoQueryFilter m_QueryFilter;
        EIMSOrderMgmtFacade m_QueryFacde;
        public EIMSEventMemo()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            m_QueryFacde = new EIMSOrderMgmtFacade(this);
            spCondition.DataContext = m_QueryFilter = new EIMSEventMemoQueryFilter();         
        }

        private void dgQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_QueryFilter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            m_QueryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            m_QueryFacde.QueryEIMSEventMemo(m_QueryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dgQueryResult.TotalCount = args.Result.TotalCount;
                dgQueryResult.ItemsSource = args.Result.Rows.ToList();
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgQueryResult.Bind();
        }

        /// <summary>
        /// 报表导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_EventMemo_Export))
            {
                Window.Alert(ResEIMSEventMemo.Msg_HasNoRight);
                return;
            }
            if (dgQueryResult.ItemsSource == null)
            {
                Window.Alert(ResEIMSEventMemo.Msg_PleaseQueryData);
                return;
            }
            m_QueryFilter.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            ComprehensiveReportFacade facade = new ComprehensiveReportFacade(this);

            ColumnSet col = new ColumnSet(dgQueryResult);
            m_QueryFacde.ExportEventMemo(m_QueryFilter, new ColumnSet[] { col });
        }
    }
}
