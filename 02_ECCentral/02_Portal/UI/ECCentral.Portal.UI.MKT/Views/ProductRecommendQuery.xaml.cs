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
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductRecommendQuery : PageBase
    {
        private ProductRecommendQueryVM _queryVM;
        public ProductRecommendQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            //1.初始化查询区域DataContext
            _queryVM = new ProductRecommendQueryVM();
            this.GridFilter.DataContext = _queryVM;
            //2.初始化页面数据
            //绑定查询区域中的渠道列表
            var channelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //channelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.lstChannel.ItemsSource = channelList;
            this.lstStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
            SetProductStatusSource();
            if (channelList != null && channelList.Count > 0)
                this.lstChannel.SelectedValue = 1;

            this.ucProductPicker.ProductSelected += new EventHandler<Basic.Components.UserControls.ProductPicker.ProductSelectedEventArgs>(ucProductPicker_ProductSelected);
        }

        void ucProductPicker_ProductSelected(object sender, Basic.Components.UserControls.ProductPicker.ProductSelectedEventArgs e)
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
            //PageType,PageID直接从控件取值
            _queryVM.PageType = this.ucPageType.PageType;
            _queryVM.PageID = this.ucPageType.PageID;
            ProductRecommendFacade facade = new ProductRecommendFacade(this);
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var rows = args.Result.Rows.ToList();
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
            this.Window.Confirm(ResProductRecommend.Confirm_Void, (cs, cargs) =>
                {
                    if (cargs.DialogResult == DialogResultType.OK)
                    {
                        HyperlinkButton btnEdit = sender as HyperlinkButton;
                        var row = btnEdit.DataContext as dynamic;
                        ProductRecommendFacade facade = new ProductRecommendFacade(this);
                        facade.Deactive((int)row.SysNo, (s, args) =>
                        {
                            if (args.FaultsHandle()) return;
                            row.Status = ADStatus.Deactive;
                            this.Window.Alert(ResProductRecommend.Info_VoidSuccessfully);
                        });
                    }
                });

        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_ProductRecommendMaintainUrlFormat, ""), null, true);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            this.Window.Navigate(string.Format(ConstValue.MKT_ProductRecommendMaintainUrlFormat, row.SysNo), null, true);
        }

        private void SetProductStatusSource()
        {
            var source = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
            source = (from e in source where e.Key == ProductStatus.Active || e.Key == null select e).ToList();
            lstProductStatus.ItemsSource = source;
        }
    }

}
