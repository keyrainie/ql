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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.UserControls.Keywords;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class HotKeywords : PageBase
    {

        private HotKeywordsQueryFacade facade;
        private HotKeywordsQueryFilter filter;
        private List<HotKeywordsVM> gridVM;
        private HotKeywordsQueryVM queryVM;
        private HotKeywordsQueryFilter filterVM;

        public HotKeywords()
        {
            InitializeComponent();
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
            filter = queryVM.ConvertVM<HotKeywordsQueryVM, HotKeywordsQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            
            facade = new HotKeywordsQueryFacade(this);
            filter = new HotKeywordsQueryFilter();
            queryVM = new HotKeywordsQueryVM();
            queryVM.CompanyCode = CPApplication.Current.CompanyCode;
            queryVM.ChannelID = "1";
            QuerySection.DataContext = queryVM;

            facade.GetHotKeywordsEditUserList(CPApplication.Current.CompanyCode, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                BindEditUserList(args.Result);
            });
            base.OnPageLoad(sender, e);
        }

        private void BindEditUserList(List<UserInfo> userList)
        {
            if (userList == null)
            {
                userList = new List<UserInfo>();
            }
            userList.Insert(0, new UserInfo { SysNo = null, UserName = ResCommonEnum.Enum_All });
            comEditUser.ItemsSource = userList;
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryHotKeywords(QueryResultGrid.QueryCriteria as HotKeywordsQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<HotKeywordsVM>.ConvertToVMList<List<HotKeywordsVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;

                if (gridVM != null)
                {
                    btnVoidItem.IsEnabled = true;
                    btnAvailableItem.IsEnabled = true;
                }
                else
                {
                    btnVoidItem.IsEnabled = false;
                    btnAvailableItem.IsEnabled = false;
                }
            });
        }

        /// <summary>
        /// 预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlViewItem_Click(object sender, RoutedEventArgs e)
        {
            HotKeywordsVM vm = QueryResultGrid.SelectedItem as HotKeywordsVM;
            HotKeywordsVM vmItem = gridVM.SingleOrDefault(a => a.SysNo.Value == vm.SysNo.Value);
            HotSearchKeyWords item = EntityConvertorExtensions.ConvertVM<HotKeywordsVM, HotSearchKeyWords>(vmItem, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            });

            UCViewHotSearchKeywords usercontrol = new UCViewHotSearchKeywords();
            usercontrol.Model = item;
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_ReviewHotKeywords, usercontrol, OnMaintainDialogResult);
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddHotKeywords usercontrol = new UCAddHotKeywords();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_NewHotKeywords, usercontrol, OnMaintainDialogResult);
        }

        private void OnMaintainDialogResult(object sender, ResultEventArgs args)
        {
            if (args.DialogResult == DialogResultType.OK)
            {
                filter = queryVM.ConvertVM<HotKeywordsQueryVM, HotKeywordsQueryFilter>();
                filter.PageType = ucPageType.PageType;
                filter.PageID = ucPageType.PageID;
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<HotKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        /// <summary>
        /// 屏蔽
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
                facade.BatchSetHotKeywordsInvalid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                    Window.Alert(ResKeywords.Information_OperateSuccessful, MessageType.Information);
                });
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            HotKeywordsVM item = this.QueryResultGrid.SelectedItem as HotKeywordsVM;
            UCAddHotKeywords usercontrol = new UCAddHotKeywords();
            //usercontrol.SysNo = item.SysNo.Value;
            usercontrol.VM = gridVM.Single(a => a.SysNo.Value == item.SysNo.Value);//item;
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_EditHotKeywords, usercontrol, OnMaintainDialogResult);
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = queryVM.ConvertVM<HotKeywordsQueryVM, HotKeywordsQueryFilter>();
                filter.PageType = ucPageType.PageType;
                filter.PageID = ucPageType.PageID;
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<HotKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
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

        /// <summary>
        /// 有效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAvailableItem_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.BatchSetHotKeywordsAvailable(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                    Window.Alert(ResKeywords.Information_OperateSuccessful, MessageType.Information);
                });
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Error);
        }
    }

}
