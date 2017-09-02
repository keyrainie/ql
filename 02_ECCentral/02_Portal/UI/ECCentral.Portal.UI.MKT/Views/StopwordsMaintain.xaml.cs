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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class StopwordsMaintain : PageBase
    {
        private StopWordsQueryFacade facade;
        private StopWordsQueryFilter filter;
        private StopWordsQueryFilter filterVM;
        private List<StopWordsQueryVM> gridVM;
        private StopWordsQueryVM model;

        public StopwordsMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new StopWordsQueryFacade(this);
            filter = new StopWordsQueryFilter();
            model = new StopWordsQueryVM();
            model.ChannelID = "1";
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            QuerySection.DataContext = model;

            cbShowComment.ItemsSource = EnumConverter.GetKeyValuePairs<ADTStatus>(EnumConverter.EnumAppendItemType.All);
            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            facade.QueryStopWords(QueryResultGrid.QueryCriteria as StopWordsQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<StopWordsQueryVM>.ConvertToVMList<List<StopWordsQueryVM>>(args.Result.Rows);
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
            filter = model.ConvertVM<StopWordsQueryVM, StopWordsQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<StopWordsQueryVM, StopWordsQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<StopWordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddStopWords usercontrol = new UCAddStopWords();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_CreateStopWords, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<StopWordsQueryVM, StopWordsQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<StopWordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
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

            List<StopWordsInfo> list = new List<StopWordsInfo>();
            List<StopWordsInfo> templist = new List<StopWordsInfo>();

            gridVM.ForEach(vm =>
            {
                if (vm.IsChecked == true)
                {
                    StopWordsInfo item = EntityConvertorExtensions.ConvertVM<StopWordsQueryVM, StopWordsInfo>(vm, (v, t) =>
                    {
                        
                            t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
                     });
                    if (!string.IsNullOrEmpty(item.Keywords.Content))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        templist.Add(item);
                    }
                   
                }
            });
             if (list.Count > 0)
            {
                if (templist.Count > 0)
                {
                    Window.MessageBox.Show("批量更新的数据中,有阻止词类容为空的数据，将不会更新成功!",MessageBoxType.Warning);
                }
                facade.BatchUpdateStopWords(list, (obj, args) =>
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
            StopWordsQueryVM vm = this.QueryResultGrid.SelectedItem as StopWordsQueryVM;


            StopWordsInfo item = EntityConvertorExtensions.ConvertVM<StopWordsQueryVM, StopWordsInfo>(vm, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            });
            List<StopWordsInfo> list = new List<StopWordsInfo>();
            list.Add(item);
            facade.BatchUpdateStopWords(list, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                QueryResultGrid.Bind();
            });
        }
    }

}
