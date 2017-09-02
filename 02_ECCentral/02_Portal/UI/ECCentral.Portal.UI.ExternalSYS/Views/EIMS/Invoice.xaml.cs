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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.ExternalSYS.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class Invoice : PageBase
    {
        EIMSInvoiceQueryFilter m_queryRequest;
        CommonDataFacade m_commonDataFacade;

        public Invoice()
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
            m_commonDataFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
            this.SearchCondition.DataContext = m_queryRequest = new EIMSInvoiceQueryFilter();
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
            facade.QueryEIMSInvoice(m_queryRequest, (obj, args) =>
                {
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
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS,
                new string[] { ConstValue.Key_EIMSType, ConstValue.Key_ReceivedType, ConstValue.Key_InvoiceStatus },
                CodeNamePairAppendItemType.All, (o, p) =>
                {
                    this.cmbEIMSType.ItemsSource = p.Result[ConstValue.Key_EIMSType];
                    this.cmbEIMSType.SelectedIndex = 0;

                    this.cmbReceiveType.ItemsSource = p.Result[ConstValue.Key_ReceivedType];
                    this.cmbReceiveType.SelectedIndex = 0;

                    this.cmbInvoiceStatus.ItemsSource = p.Result[ConstValue.Key_InvoiceStatus];
                    this.cmbInvoiceStatus.SelectedIndex = 0;
                });
        }

        /// <summary>
        /// 导出全部数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_Invoice_Export))
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
            facade.ExportEIMSInvoice(m_queryRequest, new ColumnSet[] { col });
        }
    }
}
