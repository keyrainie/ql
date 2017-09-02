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
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class PurchaseOrderItemHistory : PageBase
    {
        public PurchaseOrderFacade serviceFacade;
        public PurchaseOrderQueryVM queryVM;
        PurchaseOrderQueryFilter filter;
        public string ProductID;
        public string ProductSysNo;

        public PurchaseOrderItemHistory()
        {
            InitializeComponent();
            queryVM = new PurchaseOrderQueryVM();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.serviceFacade = new PurchaseOrderFacade(this);
            filter = new PurchaseOrderQueryFilter();
            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                string[] paramsArray = this.Request.Param.Split('|');
                queryVM.ProductSysNo = paramsArray[0];
                if (paramsArray.Length > 1)
                {
                    queryVM.ProductID = paramsArray[1];
                }
                btnSearch_Click(null, null);
            }
            this.DataContext = queryVM;

        }

        #region [Events]
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(queryVM.ProductSysNo) && string.IsNullOrEmpty(queryVM.ProductID))
            {
                Window.Alert("商品ID和商品编号，至少要填写一项!");
                return;
            }
            //搜索操作:
            filter = EntityConverter<PurchaseOrderQueryVM, PurchaseOrderQueryFilter>.Convert(this.queryVM);
            this.QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            serviceFacade.QueryPurchaseOrderHistory(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var getList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = getList;
            });
        }

        private void Hyperlink_EditPO_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/PurchaseOrderMaintain/{0}", getSelectedItem["SysNo"]), null, true);
            }
        }

        private void Hyperlink_EditVendor_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/VendorMaintain/{0}", getSelectedItem["VendorSysNo"]), null, true);
            }
        }
        #endregion


    }

}
