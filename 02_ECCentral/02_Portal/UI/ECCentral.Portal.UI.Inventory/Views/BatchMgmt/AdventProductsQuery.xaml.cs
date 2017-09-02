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
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.UserControls.BatchMgmt;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View]
    public partial class AdventProductsQuery : PageBase
    {

        private AdventProductsFacade _facade;
        private AdventProductsQueryFilter _queryFilter;

        public AdventProductsQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _facade = new AdventProductsFacade(this);
            _queryFilter = new AdventProductsQueryFilter();

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _queryFilter.CategoryC3SysNo = this.ucCategory.ChooseCategory3SysNo;
            _queryFilter.BrandSysNo = string.IsNullOrEmpty(this.ucBrand.SelectedBrandSysNo) ? (int?)null : Convert.ToInt32(this.ucBrand.SelectedBrandSysNo);
            this.dgAdventProductsQueryList.Bind();
        }
        private void dgAdventProductsQueryList_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = dgAdventProductsQueryList.PageSize,
                PageIndex = dgAdventProductsQueryList.PageIndex,
                SortBy = e.SortField
            };
            _facade.QueryAdventProductsList(_queryFilter, (obj, args) =>
              {
                  if (args.FaultsHandle())
                  {
                      return;
                  }
                  var resultList = args.Result.Rows;
                  int totalCount = args.Result.TotalCount;
                  this.dgAdventProductsQueryList.TotalCount = totalCount;
                  this.dgAdventProductsQueryList.ItemsSource = resultList;
              });
        }

        private void btnNewAdventProduct_Click(object sender, RoutedEventArgs e)
        {
            //新增操作:
            UCAdventProductsEdit uc = new UCAdventProductsEdit();
            uc.Dialog = this.Window.ShowDialog("新建临时天数", uc, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    btnSearch_Click(null, null);
                }
            }, new Size(500, 300));
        }
        private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        {
            dynamic data = this.dgAdventProductsQueryList.SelectedItem as dynamic;
            if (data != null)
            {
                UCAdventProductsEdit uc = new UCAdventProductsEdit(data);
                uc.Dialog = this.Window.ShowDialog("编辑临时天数", uc, (obj, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        btnSearch_Click(null, null);
                    }
                }, new Size(500, 300));
            }
        }
    }

}
