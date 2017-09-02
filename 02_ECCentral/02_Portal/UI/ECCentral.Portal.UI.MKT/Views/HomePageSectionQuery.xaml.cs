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
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class HomePageSectionMaintain : PageBase
    {
        private HomePageSectionQueryVM _queryVM;

        public HomePageSectionMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _queryVM = new HomePageSectionQueryVM();
            this.DataContext = _queryVM;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            //调用DataGrid.Bind方法，触发LoadingDataSource事件加载数据
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            HomePageSectionFacade facade = new HomePageSectionFacade(this);
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                this.DataGrid.ItemsSource = args.Result.Rows;
                this.DataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            UCHomePageSectionMaintain uc = new UCHomePageSectionMaintain();
            uc.DialogHandle = this.Window.ShowDialog(ResHomePageSection.Info_AddTitle, uc, OnMaintainDialogClose);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic selected = (sender as FrameworkElement).DataContext as dynamic;
            if (selected != null)
            {
                UCHomePageSectionMaintain uc = new UCHomePageSectionMaintain();
                uc.BeginEditing(selected.SysNo);
                uc.DialogHandle = this.Window.ShowDialog(ResHomePageSection.Info_EditTitle, uc, OnMaintainDialogClose);
            }
        }

        private void OnMaintainDialogClose(object sender, ResultEventArgs e)
        {
            if (e.DialogResult == DialogResultType.OK)
            {
                this.DataGrid.Bind();
            }
        }
    }

}
