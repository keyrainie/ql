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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls.Inventory;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Inventory.Views
{
     [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ExperienceHallInventoryQuery : PageBase
    {
         public ExperienceHallInventoryInfoQueryVM queryVM;
         public ExperienceHallInventoryInfoQueryFilter queryFilter;
         public ExperienceMaintainFacade serviceFacade;


        public ExperienceHallInventoryQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryFilter = new ExperienceHallInventoryInfoQueryFilter();
            queryVM = new ExperienceHallInventoryInfoQueryVM();
            serviceFacade = new ExperienceMaintainFacade(this);
            this.DataContext = queryVM;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.dgQueryResult.Bind();
        }

        //加载数据
        private void dgQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (string.IsNullOrEmpty(queryVM.ProductID))
            {
                queryVM.ProductSysNo = null;
            }

            queryFilter = EntityConverter<ExperienceHallInventoryInfoQueryVM, ExperienceHallInventoryInfoQueryFilter>.Convert(queryVM);
            this.queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo();

            this.queryFilter.PagingInfo.PageIndex = e.PageIndex;
            this.queryFilter.PagingInfo.PageSize = e.PageSize;
            this.queryFilter.PagingInfo.SortBy = e.SortField;
            serviceFacade.QueryExperienceHallInventory(queryFilter, (innerObj, innerArgs) =>
                           {
                               if (innerArgs.FaultsHandle())
                               {
                                   return;
                               }

                               int totalCount = innerArgs.Result.TotalCount;
                               this.dgQueryResult.ItemsSource = innerArgs.Result.Rows;
                               this.dgQueryResult.TotalCount = totalCount;
                           });
        }


        //导出
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            ExperienceHallInventoryInfoQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ExperienceHallInventoryInfoQueryFilter>(queryFilter);
            exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
            ColumnSet columnSet = new ColumnSet()
            .Add("ProductID", "商品编号", 20)
            .Add("ProductName", "商品名称", 40)
            .Add("TotalQty", "体验厅库存", 40)
            .Add("OutStockQty", "已出库数量", 20);
            serviceFacade.ExportExcelQueryExperienceHallInventory(exportQueryRequest, new ColumnSet[] { columnSet });
        }
    }
}
