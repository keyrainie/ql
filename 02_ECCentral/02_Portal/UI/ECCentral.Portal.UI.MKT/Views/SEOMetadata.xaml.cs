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
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SEOMetadata : PageBase
    {
        private SEOMetadataQueryVM _queryVM;
        private SEOFacade facade;
        private SEOQueryFilter filter;
        private List<SEOMetadataVM> gridVM;
        private SEOQueryFilter filterVM;
		

        public SEOMetadata()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            filter = new SEOQueryFilter();
            _queryVM = new SEOMetadataQueryVM();
            _queryVM.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            _queryVM.ChannelID = "1";
            QuerySection.DataContext = _queryVM;
            btnStackPanel.DataContext = _queryVM;
            facade = new SEOFacade(this);
            cbNewsCategory.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
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
                Window.Alert(ResNewsInfo.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = _queryVM.ConvertVM<SEOMetadataQueryVM, SEOQueryFilter>();
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

            facade.SEOQuery(QueryResultGrid.QueryCriteria as SEOQueryFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<SEOMetadataVM>.ConvertToVMList<List<SEOMetadataVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void OnMaintainDialogResult(object sender, ResultEventArgs args)
        {
            if (args.DialogResult == DialogResultType.OK)
            {
                filter = _queryVM.ConvertVM<SEOMetadataQueryVM, SEOQueryFilter>();

                filter.PageType = ucPageType.PageType;
                filter.PageID = ucPageType.PageID;
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SEOQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstChannel.SelectedValue == null)
                Window.Alert(ResNewsInfo.Information_SelectTheChannel, MessageType.Error);
            else
            {
                UCAddSEOItem usercontrol = new UCAddSEOItem();
                usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_CreateSEO, usercontrol, OnMaintainDialogResult);
            }
        }

        /// <summary>
        /// 编辑该行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            SEOMetadataVM item = this.QueryResultGrid.SelectedItem as SEOMetadataVM;
            if (item != null)
            {
                UCAddSEOItem usercontrol = new UCAddSEOItem();
                usercontrol.SysNo = item.SysNo.Value;
                usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_EditSEO, usercontrol, OnMaintainDialogResult);
            }
            else
                Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (lstChannel.SelectedValue == null)
                Window.Alert(ResNewsInfo.Information_SelectTheChannel, MessageType.Error);
            else
            {
                if (ValidationManager.Validate(this.QuerySection))
                {
                    filter = _queryVM.ConvertVM<SEOMetadataQueryVM, SEOQueryFilter>();

                    filter.PageType = ucPageType.PageType;
                    filter.PageID = ucPageType.PageID;
                    filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SEOQueryFilter>(filter);
                    QueryResultGrid.QueryCriteria = this.filter;
                    QueryResultGrid.Bind();
                }
            }
        }

    }

}
