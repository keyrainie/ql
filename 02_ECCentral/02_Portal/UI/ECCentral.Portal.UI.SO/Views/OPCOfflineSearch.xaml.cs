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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.QueryFilter.SO;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.UI.SO.UserControls;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class OPCOfflineSearch : PageBase
    {
        CommonDataFacade CommonDataFacade;
        OPCQueryFilter m_query;
        public OPCOfflineSearch()
        {
            InitializeComponent();
            CommonDataFacade = new Basic.Components.Facades.CommonDataFacade(this);
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            spConditions.DataContext = m_query = new OPCQueryFilter();
            IntiPageData();
        }
        private void IntiPageData()
        {
            this.cmbActionType.ItemsSource = EnumConverter.GetKeyValuePairs<WMSAction>(EnumConverter.EnumAppendItemType.All);
            this.cmbActionType.SelectedIndex = 0;

            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<OPCStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            m_query.CompanyCode = CPApplication.Current.CompanyCode;
            facade.QueryOPCMaster(m_query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGrid.TotalCount = args.Result.TotalCount;
                dataGrid.ItemsSource = args.Result.Rows;
            });
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml info = this.dataGrid.SelectedItem as DynamicXml;
            if (info != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, info["SONumber"]), null, true);
            }
        }

        private void hlbtnSysNo_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml selectedModel = this.dataGrid.SelectedItem as DynamicXml;
            if (selectedModel != null)
            {
                OPCOfflineTransactionShow ctrl = new OPCOfflineTransactionShow(this);
                ctrl.MasterSysNo = (int)selectedModel["TransactionNumber"];
                ctrl.Dialog = Window.ShowDialog(ResWMS.Header_OPCTransactionDetail, ctrl);
            }
        }
    }
}
