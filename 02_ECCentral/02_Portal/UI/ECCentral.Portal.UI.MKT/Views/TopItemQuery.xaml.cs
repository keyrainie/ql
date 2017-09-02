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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class TopItemQuery : PageBase
    {
        private TopItemQueryVM viewModel;
        private TopItemFilter filter;
        private TopItemFacade facade;
        private int? CategoryType; //每次点击查询时 保存CategoryType 和  CategorySysNo
        private int? CategorySysNo;
        public TopItemQuery()
        {
            viewModel = new TopItemQueryVM() { ChannelID="1"};

            this.DataContext = viewModel;
            InitializeComponent();
            ucPosition.PageSelectionChanged += new EventHandler<UserControls.PageSelectionChangedEventArgs>(ucPosition_PageSelectionChanged);
            this.ucPosition.PageTypeSelectionChanged += new EventHandler<UserControls.PageTypeSelectionChangedEventArgs>(ucPosition_PageTypeSelectionChanged);

        }

        void ucPosition_PageTypeSelectionChanged(object sender, UserControls.PageTypeSelectionChangedEventArgs e)
        {
            if (ucPosition.PageType != null)
            {
                PageTypePresentationType type = PageTypeUtil.ResolvePresentationType(ModuleType.NewsAndBulletin, ucPosition.PageType.Value.ToString());
                this.chkIsExtend.IsEnabled = type == PageTypePresentationType.Category3;
                this.chkIsExtend.IsChecked = false;
            }
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new TopItemFacade(this);
            AppSettingHelper.GetSetting("MKT", "TopItemPreviewUrlBase4Category", (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                viewModel.FrontPageUrlBase4Category = args.Result;
            });

            AppSettingHelper.GetSetting("MKT", "TopItemPreviewUrlBase4SubCategory", (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                viewModel.FrontPageUrlBase4SubCategory = args.Result;
            });
        }
        void ucPosition_PageSelectionChanged(object sender, UserControls.PageSelectionChangedEventArgs e)
        {
            SetQueryFilter();
            if (filter.PageType != null && filter.RefSysNo != null)
            {
                facade.QueryTopItemConfig(filter, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    viewModel.ISSendMailStore = args.Result != null && args.Result.ISSendMailStore.Value;
                    viewModel.ISShowTopStore = args.Result != null && args.Result.ISShowTopStore.Value;

                });
            }
        }


        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (ucPosition.PageID == null || ucPosition.PageID <= 0)
            {
                Window.Alert("请选择页面类型!");
                return;
            }
            TopItemConfigInfo entity = new TopItemConfigInfo();
            entity.PageType = ucPosition.PageType;
            entity.RefSysNo = ucPosition.PageID;
            entity.ISSendMailStore = viewModel.ISSendMailStore;
            entity.ISShowTopStore = viewModel.ISShowTopStore;
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            facade.UpdateTopItemConfig(entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                Window.Alert("操作成功!");
            });
        }



        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            SetQueryFilter();

            if (filter.PageType == null)
            {
                Window.Alert("请选择页面类型。");
                return;
            }

            if (filter.RefSysNo == null)
            {
                Window.Alert("请选择类别。");
                return;
            }
            CategorySysNo = ucPosition.PageID;
            CategoryType = ucPosition.PageType;
            DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {

            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            if (filter.PageType == null || filter.PageType == 0)
            {
                filter.PageType = CategoryType;
            }
            if (filter.RefSysNo == null || filter.RefSysNo == 0)
            {
                filter.RefSysNo = CategorySysNo;
            }
            facade.QueryTopItemList(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                var list = DynamicConverter<TopItemVM>.ConvertToVMList(args.Result.Rows) as List<TopItemVM>;
                list.ForEach(item =>
                {
                    if (filter.PageType == 2)
                        item.PageUrl = string.Format(viewModel.FrontPageUrlBase4Category, filter.RefSysNo, item.PageNumber, filter.FrontPageSize);
                    else
                        item.PageUrl = string.Format(viewModel.FrontPageUrlBase4SubCategory, filter.RefSysNo, item.PageNumber, filter.FrontPageSize);
                });
                DataGrid.ItemsSource = list;
                DataGrid.TotalCount = args.Result.TotalCount;
            });
        }
        private void SetQueryFilter()
        {
            filter = viewModel.ConvertVM<TopItemQueryVM, TopItemFilter>();
            filter.PageType = ucPosition.PageType;
            filter.RefSysNo = ucPosition.PageID;
            filter.C1SysNo = ucPosition.ucCategoryPicker.SelectedCategory1SysNo;
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                List<TopItemVM> viewList = this.DataGrid.ItemsSource as List<TopItemVM>;
                if (viewList != null)
                {
                    foreach (var view in viewList)
                    {
                        if (view.IsSetTop)
                            view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                    }
                }
            }
        }

        private List<TopItemVM> GetSelectedSysNoList()
        {
            List<TopItemVM> sysNoList = new List<TopItemVM>();
            if (this.DataGrid.ItemsSource != null)
            {
                List<TopItemVM> viewList = this.DataGrid.ItemsSource as List<TopItemVM>;
                foreach (var view in viewList)
                {
                    if (view.IsChecked)
                    {
                        sysNoList.Add(view);
                    }
                }
            }
            return sysNoList;
        }

        private void btnBatchRemove_Click(object sender, RoutedEventArgs e)
        {
            List<TopItemVM> list = GetSelectedSysNoList();
            if (list.Count > 0)
            {
                TopItemInfo entity = new TopItemInfo();
                entity.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = viewModel.ChannelID };
                entity.CategoryType = ucPosition.PageType;
                List<TopItemInfo> entityList = new List<TopItemInfo>();
                list.ForEach(item =>
                {
                    var newEntity = entity.DeepCopy<TopItemInfo>();
                    newEntity.ProductSysNo = item.ProductSysNo;
                    newEntity.CategorySysNo = CategorySysNo;
                    newEntity.CategoryType = CategoryType;
                    entity.IsExtend = chkIsExtend.IsChecked;
                    entityList.Add(newEntity);
                });
                facade.CancleTopItem(entityList, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    DataGrid.Bind();
                });
            }
        }

        private void hlbUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem != null)
            {

                TopItemVM vm = (DataGrid.SelectedItem as TopItemVM);

                if (vm.HasValidationErrors)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(vm.Priority))
                {
                    TopItemInfo entity = new TopItemInfo();
                    entity.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = viewModel.ChannelID };
                    entity.CategorySysNo = CategorySysNo;
                    entity.CategoryType = CategoryType;
                    entity.ProductSysNo = vm.ProductSysNo;
                    entity.Priority = int.Parse(vm.Priority);
                    entity.CompanyCode = viewModel.CompanyCode;
                    entity.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = viewModel.ChannelID };
                    entity.IsExtend = chkIsExtend.IsChecked;
                    facade.SetTopItem(entity, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        DataGrid.Bind();
                    });
                }
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = sender as HyperlinkButton;
            if (link != null)
            {
                var prarm = Convert.ToString(link.Tag);
                CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.IM_ProductlinkPriceUrlFormat, prarm), null, true);
            }
        }


    }
}
