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
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class HelpCenterQuery : PageBase
    {
        private HelpCenterQueryVM _queryVM;
        public HelpCenterQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.GridFilter.DataContext = _queryVM = new HelpCenterQueryVM();
            LoadComboxData();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件，分页信息
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            p.SortBy = string.IsNullOrEmpty(p.SortBy) ? " B.SysNo desc " : p.SortBy;
            HelpCenterFacade facade = new HelpCenterFacade(this);
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                this.DataGrid.TotalCount = args.Result.TotalCount;
                var rows = args.Result.Rows;

                this.DataGrid.ItemsSource = rows;
            });
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_HelpCenterMaintainUrlFormat, ""), null, true);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            if (row != null)
            {
                Window.Navigate(string.Format(ConstValue.MKT_HelpCenterMaintainUrlFormat, row.SysNo), null, true);
            }
        }

        /// <summary>
        /// 加载查询区域Combox数据源
        /// </summary>
        private void LoadComboxData()
        {
            //绑定查询区域中的渠道列表
            var channelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            channelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.lstChannel.ItemsSource = channelList;

            this.lstStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
            this.lstFeatureType.ItemsSource = EnumConverter.GetKeyValuePairs<FeatureType>(EnumConverter.EnumAppendItemType.All);
        }

        private void lstChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstChannel.SelectedValue != null)
            {
                //根据渠道的变化，动态加载帮助类型
                HelpCenterFacade helpCenterFacade = new HelpCenterFacade(this);
                helpCenterFacade.QueryCategory(CPApplication.Current.CompanyCode, this.lstChannel.SelectedValue.ToString(), (s, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    List<HelpCenterCategoryVM> helpCategoryList = DynamicConverter<HelpCenterCategoryVM>.ConvertToVMList(args.Result.Rows);
                    BindHelpTypeList(helpCategoryList);
                });
            }
            else
            {
                BindHelpTypeList(null);
            }
        }

        private void BindHelpTypeList(List<HelpCenterCategoryVM> items)
        {
            if (items == null)
            {
                items = new List<HelpCenterCategoryVM>();
            }

            items.Insert(0, new HelpCenterCategoryVM { Name = ResCommonEnum.Enum_All });

            this.lstHelpCategory.ItemsSource = items;
        }
    }

}
