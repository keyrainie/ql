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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ThesaurusKeywordsMaintain : PageBase
    {
        private ThesaurusKeywordsQueryVM model;
        private ThesaurusKeywordsQueryFacade facade;
        private ThesaurusKeywordsQueryFilter filter;
        private ThesaurusKeywordsQueryFilter filterVM;
        private List<ThesaurusKeywordsVM> gridVM;

        public ThesaurusKeywordsMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new ThesaurusKeywordsQueryFacade(this);
            model = new ThesaurusKeywordsQueryVM();
            model.ChannelID = "1";
            filter = new ThesaurusKeywordsQueryFilter();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            cbTypeSource.ItemsSource = EnumConverter.GetKeyValuePairs<ThesaurusWordsType>(EnumConverter.EnumAppendItemType.All);
            cbStatusSource.ItemsSource = EnumConverter.GetKeyValuePairs<ADTStatus>(EnumConverter.EnumAppendItemType.All);
            QuerySection.DataContext = model;

            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryThesaurusKeywords(QueryResultGrid.QueryCriteria as ThesaurusKeywordsQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<ThesaurusKeywordsVM>.ConvertToVMList<List<ThesaurusKeywordsVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
                if (gridVM != null)
                    btnBatchUpdate.IsEnabled = true;
                else
                    btnBatchUpdate.IsEnabled = false;
            });	
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
            filter = model.ConvertVM<ThesaurusKeywordsQueryVM, ThesaurusKeywordsQueryFilter>(); 
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }


        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddThesaurusKeywords usercontrol = new UCAddThesaurusKeywords();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_CreateThesaurusKeywords, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<ThesaurusKeywordsQueryVM, ThesaurusKeywordsQueryFilter>();

                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ThesaurusKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }
       
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<ThesaurusKeywordsQueryVM, ThesaurusKeywordsQueryFilter>();
           
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ThesaurusKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchUpdate_Click(object sender, RoutedEventArgs e)
        {
            //StopWordsQueryVM vm = this.QueryResultGrid.SelectedItem as StopWordsQueryVM;
            //StopWordsInfo item = EntityConvertorExtensions.ConvertVM<StopWordsQueryVM, StopWordsInfo>(vm, (v, t) =>
            //{
            //    t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            //});

            if (!ValidationManager.Validate(this.LayoutRoot))
                return;


            List<ThesaurusInfo> list = new List<ThesaurusInfo>();
            gridVM.ForEach(vm =>
            {
                if (vm.IsChecked == true)
                {
                    ThesaurusInfo item = EntityConvertorExtensions.ConvertVM<ThesaurusKeywordsVM, ThesaurusInfo>(vm, (v, t) =>
                    {
                        t.ThesaurusWords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.ThesaurusWords);
                    });
                    list.Add(item);
                }
            });
            if (list.Count > 0)
            {
                facade.BatchUpdateThesaurusInfo(list, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    QueryResultGrid.Bind();
                });
            }
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);

            //dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            //UCAddStopWords usercontrol = new UCAddStopWords();
            //usercontrol.SysNo = item.SysNo;
            //IDialog dialog = Window.ShowDialog("编辑阻止词", usercontrol, (obj, args) =>
            //{
            //    QueryResultGrid.Bind();
            //});
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 单个编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;

            ThesaurusKeywordsVM vm = this.QueryResultGrid.SelectedItem as ThesaurusKeywordsVM;
            ThesaurusInfo item = EntityConvertorExtensions.ConvertVM<ThesaurusKeywordsVM, ThesaurusInfo>(vm, (v, t) =>
            {
                t.ThesaurusWords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.ThesaurusWords);
            });
            List<ThesaurusInfo> list = new List<ThesaurusInfo>();
            list.Add(item);
            facade.BatchUpdateThesaurusInfo(list, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                QueryResultGrid.Bind();
            });
        }

    }

}
