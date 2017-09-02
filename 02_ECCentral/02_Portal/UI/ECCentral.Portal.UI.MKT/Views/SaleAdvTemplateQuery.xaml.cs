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
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SaleAdvTemplateQuery : PageBase
    {
        public SaleAdvQueryVM FilterVM { get; set; }

        public SaleAdvTemplateQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.FilterVM = new SaleAdvQueryVM();
            this.FilterVM.ChannelID = "1";

            this.gridCondition.DataContext = this.FilterVM;
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch_Click(this.btnSearch, new RoutedEventArgs());
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridCondition))
            {
                this.dataResult.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SaleAdvQueryVM>(this.FilterVM);
                this.dataResult.Bind();
            }
        }

        private void dataResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            new SaleAdvTemplateFacade(this).Query(this.dataResult.QueryCriteria as SaleAdvQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dataResult.ItemsSource = args.Result.Rows;
                this.dataResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_SaleAdvTemplateMaintainUrlFormat, ""), null, true);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            dynamic vm = btn.DataContext;
            this.Window.Navigate(string.Format(ConstValue.MKT_SaleAdvTemplateMaintainUrlFormat, vm.SysNo), null, true);
        }

        private void btnMaintainItem_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            dynamic vm = btn.DataContext;
            //this.Window.Navigate(string.Format(ConstValue.MKT_SaleAdvTemplateItemMaintainUrlFormat, vm.SysNo), null, true);
            this.Window.Navigate(string.Format(ConstValue.MKT_FloorPromotionUrlFormat, vm.SysNo), null, true);
        }

        private void HyperlinkButtonSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            dynamic vm = btn.DataContext;

            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebsitePromotionUrl);            
            CPApplication.Current.CurrentPage.Context.Window.Navigate(
                String.Format(urlFormat, vm.SysNo), null, true);
        }
    }
}
