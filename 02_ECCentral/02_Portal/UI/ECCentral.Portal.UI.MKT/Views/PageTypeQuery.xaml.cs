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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models.PageType;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class PageTypeQuery : PageBase
    {
        private PageTypeQueryVM _queryVM;

        public PageTypeQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            _queryVM = new PageTypeQueryVM();
            _queryVM.ChannelID = "1";
            this.GridFilter.DataContext = _queryVM;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
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
            PageTypeFacade facade = new PageTypeFacade(this);
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }

                DataGrid.ItemsSource = list;
                DataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void OnDialogClosed(object s, ResultEventArgs r)
        {
            if (r.Data != null)
            {
                var refreshDataGrid = (bool)r.Data;
                if (refreshDataGrid)
                {
                    this.DataGrid.Bind();
                }
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            UCPageTypeMaintain ucMaintain = new UCPageTypeMaintain(-1);
            ucMaintain.DialogHandle = this.Window.ShowDialog(ResPageType.Info_AddTitle, ucMaintain, OnDialogClosed);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            UCPageTypeMaintain ucMaintain = new UCPageTypeMaintain(row.SysNo);
            ucMaintain.DialogHandle = this.Window.ShowDialog(ResPageType.Info_EditTitle, ucMaintain, OnDialogClosed);
        }
    }

}
