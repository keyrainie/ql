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
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class HotSaleCategoryQuery : PageBase
    {
        private HotSaleCategoryQueryVM _queryVM;

        public HotSaleCategoryQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            //绑定查询区域中的渠道列表
            var channelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //channelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.lstChannelList.ItemsSource = channelList;

            _queryVM = new HotSaleCategoryQueryVM();
            this.GridFilter.DataContext = _queryVM;
            this.lstChannelList.SelectedIndex = 0;

            this.lstStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
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
            _queryVM.PageType = this.ucPageType.PageType;
            HotSaleCategoryFacade facade = new HotSaleCategoryFacade(this);
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
            var uc = new UCHotSaleCategoryMaintain(0);
            uc.DialogHandle = Window.ShowDialog(ResHotSaleCategory.Info_AddTitle, uc, OnMaintainDialogClosed);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;

            var uc = new UCHotSaleCategoryMaintain(row.SysNo);
            uc.DialogHandle = Window.ShowDialog(ResHotSaleCategory.Info_AddTitle, uc, OnMaintainDialogClosed);
        }

        private void OnMaintainDialogClosed(object sender, ResultEventArgs e)
        {
            if (e.DialogResult == DialogResultType.OK)
            {
                this.DataGrid.Bind();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            Window.Confirm(ResHotSaleCategory.Confirm_Delete, (cs, cr) =>
            {
                if (cr.DialogResult == DialogResultType.OK)
                {

                    new HotSaleCategoryFacade(this).Delete(row.SysNo, new Action(()=>
                    {
                        Window.Alert(ResHotSaleCategory.Info_DeleteSuccess);
                        this.DataGrid.Bind();
                    }));
                }
            });
        }
    }
}
