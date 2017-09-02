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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Converters;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Media.Imaging;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.UI.MKT.UserControls.Floor;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class BannerQuery : PageBase
    {
        private BannerQueryVM _queryVM;

        private ImageSource SelectRowImageSource;

        public BannerQuery()
        {
            InitializeComponent();

        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _queryVM = new BannerQueryVM();
            this.Grid.DataContext = _queryVM;
            this.lstChannel.SelectedIndex = 0;
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_BannerMaintainUrlFormat, ""), null, true);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            this.Window.Navigate(string.Format(ConstValue.MKT_BannerMaintainUrlFormat, row.SysNo), null, true);
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
            _queryVM.PageType = ucPageType.PageType;
            _queryVM.PageID = ucPageType.PageID;
            BannerFacade facade = new BannerFacade(this);
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var rows = args.Result.Rows.ToList();
                foreach (var row in rows)
                {
                    if (row.Width != null && row.Height != null)
                    {
                        row.DimensionInfo = string.Format("{0} x {1}", row.Width, row.Height);
                    }
                    bool isImage = row.BannerType == BannerType.Image;
                    row.IsImage = isImage;
                    bool isFlash = row.BannerType == BannerType.Flash;
                    row.IsFlash = isFlash;
                    row.IsOther = !isImage && !isFlash;

                }
                this.DataGrid.ItemsSource = rows;
                this.DataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        private void ButtonVoid_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Confirm(ResBanner.Confirm_Void, (cs, cargs) =>
            {
                if (cargs.DialogResult == DialogResultType.OK)
                {
                    HyperlinkButton btnEdit = sender as HyperlinkButton;
                    var row = btnEdit.DataContext as dynamic;
                    BannerFacade facade = new BannerFacade(this);
                    facade.Deactive((int)row.SysNo, (s, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        this.Window.Alert(ResBanner.Info_VoidSuccessfully);
                    });
                }
            });

        }

        private void lstChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (this.lstChannel.SelectedValue != null && this.lstChannel.SelectedValue.ToString().Length > 0)
            //{
            //    this.txtAreaNoLimit.Visibility = Visibility.Collapsed;
            //    this.ucMainArea.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    this.txtAreaNoLimit.Visibility = Visibility.Visible;
            //    this.ucMainArea.Visibility = Visibility.Collapsed;
            //}
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectRowImageSource != null)
            {
                Image tipImage = sender as Image;
                if (tipImage != null)
                {
                    tipImage.Source = SelectRowImageSource;
                }
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            if (image != null)
            {
                SelectRowImageSource = image.Source;
            }
        }

    }

}
