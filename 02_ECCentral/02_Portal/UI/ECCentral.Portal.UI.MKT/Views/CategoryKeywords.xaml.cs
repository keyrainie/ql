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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.UserControls.Keywords;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryKeywords : PageBase
    {

        private CategoryKeywordsQueryFacade facade;
        private CategoryKeywordsQueryFilter filter;
        private CategoryKeywordsQueryFilter filterVM;
        private List<CategoryKeywordsVM> gridVM;
        private CategoryKeywordsQueryVM model;

        public CategoryKeywords()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new CategoryKeywordsQueryFacade(this);
            filter = new CategoryKeywordsQueryFilter();
            model = new CategoryKeywordsQueryVM();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            model.ChannelID = "1";
            QuerySection.DataContext = model;
            btnStackPanel.DataContext = model;
            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryCategoryKeywords(QueryResultGrid.QueryCriteria as CategoryKeywordsQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<CategoryKeywordsVM>.ConvertToVMList<List<CategoryKeywordsVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
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
            filter = model.ConvertVM<CategoryKeywordsQueryVM, CategoryKeywordsQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()  
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            //col.Insert(0, "ProductId", ResRMAReports.Excel_ProductID, 20) .SetWidth("ProductName", 30);
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }


        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddUniversalCategoryKeywords usercontrol = new UCAddUniversalCategoryKeywords();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_CreateCommonKeywords, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<CategoryKeywordsQueryVM, CategoryKeywordsQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<CategoryKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }

        private void btnNewProItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddPropertyCategoryKeywords usercontrol = new UCAddPropertyCategoryKeywords();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_CreatePropertyKeywords, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<CategoryKeywordsQueryVM, CategoryKeywordsQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<CategoryKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 编辑通用关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEditCommonKeywords_Click(object sender, RoutedEventArgs e)
        {
            CategoryKeywordsVM item = this.QueryResultGrid.SelectedItem as CategoryKeywordsVM;
            UCAddUniversalCategoryKeywords usercontrol = new UCAddUniversalCategoryKeywords();
            usercontrol.VM = gridVM.SingleOrDefault(a => a.SysNo.Value == item.SysNo.Value);
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_EditCommonKeywords, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 编辑类别属性关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEditPropertyKeywords_Click(object sender, RoutedEventArgs e)
        {
            CategoryKeywordsVM item = this.QueryResultGrid.SelectedItem as CategoryKeywordsVM;
            UCAddPropertyCategoryKeywords usercontrol = new UCAddPropertyCategoryKeywords();
            usercontrol.VM = gridVM.SingleOrDefault(a => a.SysNo.Value == item.SysNo.Value);
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_EditPropertyKeywords, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<CategoryKeywordsQueryVM, CategoryKeywordsQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<CategoryKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }
    }

}
