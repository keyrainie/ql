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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Inventory.Models.Inventory;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ExperienceHallAllocateOrderQuery : PageBase
    {

         public ExperienceHallAllocateOrderQueryVM queryVM;
         public ExperienceHallAllocateOrderQueryFilter queryFilter;
         public ExperienceMaintainFacade serviceFacade;


        public ExperienceHallAllocateOrderQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryFilter = new ExperienceHallAllocateOrderQueryFilter();
            queryVM = new ExperienceHallAllocateOrderQueryVM();
            serviceFacade = new ExperienceMaintainFacade(this);
            this.DataContext = queryVM;
           // expander1.DataContext = queryVM;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.expander1);
            if (queryVM.HasValidationErrors)
            {
                return;
            }

            this.dgQueryResult.Bind();
        }

        private void dgQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (string.IsNullOrEmpty(queryVM.ProductID))
            {
                queryVM.ProductSysNo = null;
            }

            queryFilter = EntityConverter<ExperienceHallAllocateOrderQueryVM, ExperienceHallAllocateOrderQueryFilter>.Convert(queryVM);
            
            
            
            this.queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo();

            this.queryFilter.PagingInfo.PageIndex = e.PageIndex;
            this.queryFilter.PagingInfo.PageSize = e.PageSize;
            this.queryFilter.PagingInfo.SortBy = e.SortField;
            serviceFacade.ExperienceHallAllocateOrderQuery(queryFilter, (innerObj, innerArgs) =>
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


        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            ExperienceHallAllocateOrderQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ExperienceHallAllocateOrderQueryFilter>(queryFilter);
            exportQueryRequest.PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
            ColumnSet columnSet = new ColumnSet()
            .Add("SysNo", "单据编号", 20)
            .Add("Status", "状态", 40)
            .Add("AllocateType", "调拨性质", 40)
            .Add("Indate", "创建时间", 20)
            .Add("InUserName", "创建人", 20)
            .Add("EditDate", "编辑时间", 20)
            .Add("EditUserName", "编辑人", 20)
            .Add("AuditDate", "审核时间", 20)
            .Add("AuditUserName", "审核人", 20);
            serviceFacade.ExportExcelExperienceHallAllocateOrderQuery(exportQueryRequest, new ColumnSet[] { columnSet });
        }

        private void hyperlink_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            Window.Navigate(String.Format(ConstValue.Inventory_ExperienceHallAllocateOrderUrlFormat, btn.CommandParameter), null, true);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.Inventory_ExperienceHallAllocateOrderNewUrlFormat, null, true);
        }
    }
}
