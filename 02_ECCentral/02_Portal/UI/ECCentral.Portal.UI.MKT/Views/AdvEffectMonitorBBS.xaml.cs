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
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models.Adv;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class AdvEffectMonitorBBS : PageBase
    {
        private AdvEffectBBSQueryFilter filter;
        private AdvEffectBBSQueryFilter filterVM;
        private AdvEffectBBSVM model;
        private AdvEffectMonitorFacade facade;

        public AdvEffectMonitorBBS()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            model = new AdvEffectBBSVM();
            model.ChannelID = "1";
            QuerySection.DataContext = model;
            facade = new AdvEffectMonitorFacade(this);
            filter = new AdvEffectBBSQueryFilter();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            facade.QueryAdvEffectBBS(QueryResultGrid.QueryCriteria as AdvEffectBBSQueryFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                QueryResultGrid.ItemsSource = args.Result.Rows;
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
                Window.Alert(ResNewsInfo.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<AdvEffectBBSVM, AdvEffectBBSQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo() 
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            //col.Insert(0, "ProductId", ResRMAReports.Excel_ProductID, 20) .SetWidth("ProductName", 30);
            facade.ExportEffectBBSExcelFile(filterVM, new ColumnSet[] { col });
        }
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<AdvEffectBBSVM, AdvEffectBBSQueryFilter>();

                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AdvEffectBBSQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        private void btnBBSClickReport_Click(object sender, RoutedEventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResNewsInfo.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<AdvEffectBBSVM, AdvEffectBBSQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportEffectBBSExcelFile(filter, new ColumnSet[] { col });
        }
    }

}
