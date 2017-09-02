using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.QueryFilter.Common;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class LotteryQuery : PageBase
    {
        private LotteryFacade _Facade;
        private LotteryQueryVM _VM;

        public LotteryQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _Facade = new LotteryFacade(this);
            _VM = new LotteryQueryVM();

            this.DataContext = _VM;

        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DataGrid.Bind();
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            var clonedVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<LotteryQueryVM>(_VM);
            this.DataGrid.QueryCriteria = clonedVM;
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            _Facade.Query(this.DataGrid.QueryCriteria as LotteryQueryVM, p, (s, args) =>
            {
                if (!args.FaultsHandle())
                {
                    this.DataGrid.TotalCount = args.Result.TotalCount;
                    this.DataGrid.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                }
            });
        }

        private void GoToCustomerMaintain_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var data = element.DataContext as dynamic;
            this.Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, data.CustomerSysNo), true);
        }

    }
}
