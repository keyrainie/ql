using System;
using System.Windows;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.Portal.UI.ExternalSYS.UserControls.VendorPortal;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.ExternalSYS;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class VendorPortalLogSearch : PageBase
    {
        VendorSystemQueryFilter m_query;
        public VendorPortalLogSearch()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            spConditions.DataContext = m_query = new VendorSystemQueryFilter();
            //CodePair
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS
                , new string[] { ConstValue.Key_ExternalSYSLogType }
                , CodeNamePairAppendItemType.All, (o, p) =>
                {
                    this.cmbLogType.ItemsSource = p.Result[ConstValue.Key_ExternalSYSLogType];
                    this.cmbLogType.SelectedIndex = 0;
                });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dataGridLog.Bind();
        }

        private void hlbtnDetail_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Vendor_PortalLog_Details))
            {
                Window.Alert(ResVendorInfo.Msg_HasNoRight);
                return;
            }
            DynamicXml selectedModel = this.dataGridLog.SelectedItem as DynamicXml;
            if (null != selectedModel)
            {
                VendorPortalDetail ctrl = new VendorPortalDetail();
                ctrl.Data = selectedModel;
                ctrl.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(
                           ResVendorInfo.Header_LogDetail
                           , ctrl
                           , null
                           , new Size(610, 550)
                    );
            }
        }

        private void dataGridLog_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            VendorFacade facade = new VendorFacade(this);
            facade.QueryLog(m_query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridLog.TotalCount = args.Result.TotalCount;
                dataGridLog.ItemsSource = args.Result.Rows;
            });
        }
    }
}
