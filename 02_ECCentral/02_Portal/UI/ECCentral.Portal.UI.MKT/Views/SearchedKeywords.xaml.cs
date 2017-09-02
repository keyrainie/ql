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
using ECCentral.Portal.UI.MKT.UserControls.Keywords;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SearchedKeywords : PageBase
    {
        private SearchedKeywordsQueryFacade facade;
        private SearchedKeywordsQueryVM model;
        private SearchedKeywordsFilter filter;
        private List<SearchedKeywordsVM> gridVM;
        private SearchedKeywordsFilter filterVM;

        public SearchedKeywords()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new SearchedKeywordsQueryFacade(this);
            model = new SearchedKeywordsQueryVM();
            filter = new SearchedKeywordsFilter();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            model.ChannelID = "1";
            QuerySection.DataContext = model;

            facade.LoadEditUsers(Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode, (s, args) =>
            {
                if (args.FaultsHandle()) return;
                List<UserInfo> list=args.Result;
                UserInfo user=new UserInfo();
                user.UserName="--所有--";
                list.Add(user);
                this.comEditUserList.ItemsSource = list;
            });

            comCreateUserType.ItemsSource = EnumConverter.GetKeyValuePairs<KeywordsOperateUserType>(EnumConverter.EnumAppendItemType.All);
            cbShowStatus.ItemsSource = new List<KeyValuePair<ADStatus?, string>>() 
            {
                new KeyValuePair<ADStatus?, string>(null,"--所有--"),
                new KeyValuePair<ADStatus?, string>(ADStatus.Active,"展示"),
                new KeyValuePair<ADStatus?, string>(ADStatus.Deactive,"屏蔽"),
            };

            
            //CodeNamePairHelper.GetList("MKT", "CreateUserType", (s, args) =>
            //{
            //    if (args.FaultsHandle()) return;
            //    this.CreateUserType.ItemsSource = args.Result;
            //    this.CreateUserType.SelectedIndex = 0;
            //});
            //Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn status = QueryResultGrid.Columns[3] as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
            //status.Binding.ConverterParameter = typeof(ADStatus);
            btnVoidItem.IsEnabled = false;
            btnDeleteItem.IsEnabled = false;
            base.OnPageLoad(sender, e);
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResKeywords.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<SearchedKeywordsQueryVM, SearchedKeywordsFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            facade.QuerySearchedKeywords(QueryResultGrid.QueryCriteria as SearchedKeywordsFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<SearchedKeywordsVM>.ConvertToVMList<List<SearchedKeywordsVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;// DynamicConverter<SearchedKeywordVM>.ConvertToVMList<List<SearchedKeywordVM>>(args.Result.Rows);
                QueryResultGrid.TotalCount = args.Result.TotalCount;

                if (gridVM != null)
                {
                    btnVoidItem.IsEnabled = true;
                    btnDeleteItem.IsEnabled = true;
                }
                else
                {
                    btnVoidItem.IsEnabled = false;
                    btnDeleteItem.IsEnabled = false;
                }
            });
        }

        /// <summary>
        /// 查询自动匹配关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<SearchedKeywordsQueryVM, SearchedKeywordsFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SearchedKeywordsFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        /// <summary>
        /// 添加自动匹配关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddSearchedKeywords usercontrol = new UCAddSearchedKeywords();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_CreateSearchedKeywords, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<SearchedKeywordsQueryVM, SearchedKeywordsFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SearchedKeywordsFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }
        
        /// <summary>
        /// 编辑自动匹配关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            SearchedKeywordsVM item = this.QueryResultGrid.SelectedItem as SearchedKeywordsVM;
            UCAddSearchedKeywords usercontrol = new UCAddSearchedKeywords();
            usercontrol.SysNo = item.SysNo.Value;
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_EditSearchedKeywords, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 屏蔽自动匹配关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVoidItem_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.ChangeSearchedKeywordsStatus(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);
        }

        /// <summary>
        /// 删除自动匹配关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.DeleteSearchedKeywords(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (gridVM == null || checkBoxAll == null)
                return;
            gridVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

    }

}
