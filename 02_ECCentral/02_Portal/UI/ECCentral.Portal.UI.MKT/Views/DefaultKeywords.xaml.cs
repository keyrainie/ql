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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class DefaultKeywords : PageBase
    {
        private DefaultKeywordsQueryFacade facade;
        private DefaultKeywordsQueryFilter filter;
        private DefaultKeywordsQueryVM model;
        private DefaultKeywordsQueryFilter filterVM;

        public DefaultKeywords()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            comStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);

            facade = new DefaultKeywordsQueryFacade(this);
            filter = new DefaultKeywordsQueryFilter();
            model = new DefaultKeywordsQueryVM();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            QuerySection.DataContext = model;
            //默认选第一个渠道
            this.lstChannel.SelectedIndex = 0;
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
            filter = model.ConvertVM<DefaultKeywordsQueryVM, DefaultKeywordsQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            facade.QueryDefaultKeywords(QueryResultGrid.QueryCriteria as DefaultKeywordsQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                QueryResultGrid.ItemsSource = args.Result.Rows;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddDefaultKeywords uc = new UCAddDefaultKeywords();
            uc.Dialog = Window.ShowDialog(ResKeywords.Title_CreateDefaultKeywords, uc, OnMaintainDialogResult);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            var row = btn.DataContext as dynamic;
            UCAddDefaultKeywords uc = new UCAddDefaultKeywords();
            uc.SysNo = row.SysNo;
            uc.PageID = row.PageID;
            uc.PageType = row.PageType;
            uc.Dialog = Window.ShowDialog(ResKeywords.Title_EditDefaultKeywords, uc, OnMaintainDialogResult);
        }

        private void OnMaintainDialogResult(object sender,ResultEventArgs args)
        {
            if (args.DialogResult == DialogResultType.OK)
            {
                filter = model.ConvertVM<DefaultKeywordsQueryVM, DefaultKeywordsQueryFilter>();
                filter.PageType = ucPageType.PageType;
                filter.PageID = ucPageType.PageID;
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<DefaultKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<DefaultKeywordsQueryVM, DefaultKeywordsQueryFilter>();
                filter.PageType = ucPageType.PageType;
                filter.PageID = ucPageType.PageID;
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<DefaultKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
			
        }

    }

}