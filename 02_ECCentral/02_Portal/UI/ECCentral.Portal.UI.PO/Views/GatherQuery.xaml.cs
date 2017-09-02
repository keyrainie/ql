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
using ECCentral.Portal.UI.PO.Models;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class GatherQuery : PageBase
    {
        public GatherSettleQueryFilter queryFilter;
        public GatherSettlementFacade serviceFacade;

        public GatherQuery()
        {
            InitializeComponent();
        }

        private void LoadComboBoxData()
        {
            //代销结算单状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<GatherSettleStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
            //是否自动结算:
            this.cmbIsAutoSettle.ItemsSource = EnumConverter.GetKeyValuePairs<AutoSettleStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsAutoSettle.SelectedIndex = 0;
            //付款结算公司:
            this.cmbPaySettleCompany.ItemsSource = EnumConverter.GetKeyValuePairs<PaySettleCompany>(EnumConverter.EnumAppendItemType.All);
            this.cmbPaySettleCompany.SelectedIndex = 0;
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            serviceFacade = new GatherSettlementFacade(this);
            queryFilter = new GatherSettleQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
            };
            this.DataContext = queryFilter;

            DataGridTextColumn colStatus = this.QueryResultGrid.Columns[5] as DataGridTextColumn;
            colStatus.Binding.ConverterParameter = typeof(SettleStatus);

            LoadComboBoxData();
            SetAccessControl();
            base.OnPageLoad(sender, e);
        }

        private void SetAccessControl()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Query_New))
            {
                btnNewGatherSettled.IsEnabled = false;
            }
        }

        #region [Events]

        private void Hyperlink_EditConsign_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            //编辑代收结算单:
            Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/GatherMaintain/{0}", getSelectedItem["SysNo"]), null, true);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            queryFilter.CreateUser = ucPM.SelectedPMName;
            this.QueryResultGrid.Bind();
        }
        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryGatherSettlements(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var consignList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = consignList;
            });
        }

        private void btnNewGatherSettled_Click(object sender, RoutedEventArgs e)
        {
            //创建新的代收结算单:
            Window.Navigate("/ECCentral.Portal.UI.PO/GatherNew", true);
        }

        #endregion
    }

}
