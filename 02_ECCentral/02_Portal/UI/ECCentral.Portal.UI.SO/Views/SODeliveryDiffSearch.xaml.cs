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
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.SO.Views
{
    [View]
    public partial class SODeliveryDiffSearch : PageBase
    {
        SOLogisticsFacade Facade;
        SODeliveryDiffSearchVM QueryVM;
        SODeliveryDiffQueryView PageView;

        public SODeliveryDiffSearch()
        {
            InitializeComponent();
        }


        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            Facade = new SOLogisticsFacade(this);
            PageView = new SODeliveryDiffQueryView();
            QueryVM = new SODeliveryDiffSearchVM();

            this.SearchCondition.DataContext = QueryVM;
            this.QueryResultGrid.DataContext = PageView;
            this.QueryResultGrid.ItemsSource = PageView.Result;
            BindComboBoxData();

        }

        private void BindComboBoxData()
        {
            List<KeyValuePair<SOStatus?, string>> soTypeSource = EnumConverter.GetKeyValuePairs<SOStatus>(EnumConverter.EnumAppendItemType.None);
            this.cmbSOStatus.ItemsSource = soTypeSource;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            QueryVM.PageInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOLogisticsFacade facade = new SOLogisticsFacade(this);
            facade.QueryDiffSODelivery(QueryVM, (result, count) =>
            {
                PageView.Result = result;
                PageView.TotalCount = count;
            });

        }

        
    }
}
