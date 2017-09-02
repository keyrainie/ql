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
using ECCentral.Portal.UI.MKT.UserControls.Comment;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ReviewScoreItemPage : PageBase
    {
        private ReviewScoreItemQueryVM model;
        private ReviewScoreItemFacade facade;
        private ReviewScoreItemQueryFilter filter;
        private ReviewScoreItemQueryFilter filterVM;
        private List<ReviewScoreItemVM> gridVM;

        public ReviewScoreItemPage()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            filter = new ReviewScoreItemQueryFilter();
            facade = new ReviewScoreItemFacade(this);
            model = new ReviewScoreItemQueryVM();
            filter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            model.ChannelID = "1";
            QuerySection.DataContext = model;
            btnStackPanel.DataContext = model;

            Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn status = QueryResultGrid.Columns[4] as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
            status.Binding.ConverterParameter = typeof(ECCentral.BizEntity.MKT.ADStatus);

            cbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.MKT.ADStatus>(EnumConverter.EnumAppendItemType.All);
            //new CommonDataFacade(CPApplication.Current.CurrentPage).GetWebChannelList(false, (s, args) =>
            //{
            //    this.lstChannel.ItemsSource = args.Result;
            //    //this.lstChannel.SelectedIndex = 0;
            //});	

            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            cbDemo.IsChecked = false;
            facade.QueryProductReviewScore(QueryResultGrid.QueryCriteria as ReviewScoreItemQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<ReviewScoreItemVM>.ConvertToVMList<List<ReviewScoreItemVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;

                if (gridVM != null)
                {
                    btnInvalidItem.IsEnabled = true;
                    btnValidItem.IsEnabled = true;
                }
                else
                {
                    btnInvalidItem.IsEnabled = false;
                    btnValidItem.IsEnabled = false;
                }
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddReviewScoreItem usercontrol = new UCAddReviewScoreItem();
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_EditReviewScoreItem, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<ReviewScoreItemQueryVM, ReviewScoreItemQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ReviewScoreItemQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 无效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInvalidItem_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.SetReviewScoreInvalid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    Window.Alert(ResComment.Information_BatchSettingSuccessful, MessageType.Information);
                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 有效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValidItem_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.SetReviewScoreValid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    Window.Alert(ResComment.Information_BatchSettingSuccessful, MessageType.Information);
                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 详细就是编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlNewsLink_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            UCAddReviewScoreItem usercontrol = new UCAddReviewScoreItem();
            usercontrol.SysNo = item.SysNo;
            usercontrol.Dialog = Window.ShowDialog(ResComment.Title_ReviewScoreItem, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<ReviewScoreItemQueryVM, ReviewScoreItemQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ReviewScoreItemQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
			
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
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResComment.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<ReviewScoreItemQueryVM, ReviewScoreItemQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

    }

}
