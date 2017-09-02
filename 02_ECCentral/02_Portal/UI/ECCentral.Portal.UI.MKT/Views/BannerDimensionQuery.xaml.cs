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
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class BannerDimensionQuery : PageBase
    {
        private BannerDimensionQueryVM _queryVM;
        /// <summary>
        /// 是否有维护权限
        /// </summary>
        //private bool _hasMaintainRight = AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Banner_BannerDimensionMaintain);
        public BannerDimensionQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            //权限控制
            //this.ButtonCreate.IsEnabled = _hasMaintainRight;
            _queryVM = new BannerDimensionQueryVM();
            _queryVM.ChannelID = "1";
            this.GridFilter.DataContext = _queryVM;

            var channelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            channelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.lstChannelList.ItemsSource = channelList;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (_queryVM.HasValidationErrors) return;

            //1.初始化查询条件
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            BannerDimensionFacade facade = new BannerDimensionFacade(this);
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                //权限控制
               // var rows = args.Result.Rows.ToList("HasMaintainRight", _hasMaintainRight);
                this.DataGrid.ItemsSource = args.Result.Rows;
                this.DataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            UCBannerDimensionMaintain uc = new UCBannerDimensionMaintain(-1);
            uc.DialogHandle = this.Window.ShowDialog(ResBannerDimension.Info_AddTitle, uc, OnDialogClosed);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            UCBannerDimensionMaintain uc = new UCBannerDimensionMaintain(row.SysNo);

            uc.DialogHandle = this.Window.ShowDialog(ResBannerDimension.Info_EditTitle, uc, OnDialogClosed);
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

        private void lstChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstChannelList.SelectedValue != null)
            {
                //根据渠道的变化，动态加载页面类型
                var pageTypeFacade = new PageTypeFacade(this);
                pageTypeFacade.GetPageTypes(CPApplication.Current.CompanyCode, this.lstChannelList.SelectedValue.ToString(), (int)ModuleType.Banner, (s, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        this.lstPageTypeList.ItemsSource = AppendAllForPageTypeList(args.Result);
                        this.lstPageTypeList.SelectedIndex = 0;
                    });
            }
            else
            {
                this.lstPageTypeList.ItemsSource = AppendAllForPageTypeList(null);
                this.lstPageTypeList.SelectedIndex = 0;
            }
        }

        private List<CodeNamePair> AppendAllForPageTypeList(List<CodeNamePair> items)
        {
            if (items == null)
            {
                items = new List<CodeNamePair>();
            }

            items.Insert(0, new CodeNamePair { Name = ResCommonEnum.Enum_All });
            return items;
        }
    }

}
