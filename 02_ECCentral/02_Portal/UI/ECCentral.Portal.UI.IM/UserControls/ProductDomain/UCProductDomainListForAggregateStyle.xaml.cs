using System;
using System.Windows.Controls;
using System.Collections.Generic;

using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductDomainListForAggregateStyle : UserControl, IListControl<ProductDepartmentCategoryVM>
    {
        public UCProductDomainListForAggregateStyle()
        {
            InitializeComponent();
        }

        private void dataProductDomainList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var filter = this.dataProductDomainList.QueryCriteria as ProductDomainQueryVM;
            new ProductDomainFacade(CPApplication.Current.CurrentPage).QueryProductDomain(filter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dataProductDomainList.ItemsSource = args.Result.Rows.ToList();
                this.dataProductDomainList.TotalCount = args.Result.TotalCount;                
            });
        }

        public void BindData(object filter)
        {
            this.dataProductDomainList.QueryCriteria = filter;
            this.dataProductDomainList.Bind();
        }

        public List<ProductDepartmentCategoryVM> GetSelectedSysNoList()
        {
            throw new NotSupportedException();
        }
    }
}
