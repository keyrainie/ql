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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.UserControls.Keywords;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class AdvancedKeywordsMaintain : PageBase
    {
        private AdvancedKeywordsQueryFacade facade;
        private AdvancedKeywordsQueryFilter filter;
        private AdvancedKeywordsQueryFilter filterVM;
        private List<AdvancedKeywordsVM> gridVM;
        private AdvancedKeywordsQueryVM model;


        public AdvancedKeywordsMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new AdvancedKeywordsQueryFacade(this);
            filter = new AdvancedKeywordsQueryFilter();
            model = new AdvancedKeywordsQueryVM();
            model.ChannelID = "1";
            QuerySection.DataContext = model;
            cbShowComment.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            facade.QueryAdvancedKeywords(QueryResultGrid.QueryCriteria as AdvancedKeywordsQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<AdvancedKeywordsVM>.ConvertToVMList<List<AdvancedKeywordsVM>>(args.Result.Rows);
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
            filter = model.ConvertVM<AdvancedKeywordsQueryVM, AdvancedKeywordsQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            //col.Insert(0, "ProductId", ResRMAReports.Excel_ProductID, 20) .SetWidth("ProductName", 30);
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this))
            {
                filter = model.ConvertVM<AdvancedKeywordsQueryVM, AdvancedKeywordsQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AdvancedKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        /// <summary>
        /// 添加跳转关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddAdvancedKeywords usercontrol = new UCAddAdvancedKeywords();
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_AddAdvancedKeywords, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<AdvancedKeywordsQueryVM, AdvancedKeywordsQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AdvancedKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 编辑跳转关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            AdvancedKeywordsVM item = this.QueryResultGrid.SelectedItem as AdvancedKeywordsVM;
            UCAddAdvancedKeywords usercontrol = new UCAddAdvancedKeywords();
            //usercontrol.SysNo = item.SysNo.Value;
            usercontrol.VM = gridVM.Single(a => a.SysNo.Value == item.SysNo.Value);//item;
            usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_EidtAdvancedKeywords, usercontrol, (obj, args) =>
            {
                QueryResultGrid.Bind();
            });
        }
    }

}
