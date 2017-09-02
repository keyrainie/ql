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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderStatisticsQuery : UserControl
    {
        public IDialog Dialog { get; set; }
        public PurchaseOrderQueryFilter filter { get; set; }
        public PurchaseOrderFacade facade { get; set; }
        public PurchaseOrderStatisticsQuery(PurchaseOrderQueryFilter queryFilter)
        {
            InitializeComponent();
            filter = queryFilter;
            this.Loaded += new RoutedEventHandler(PurchaseOrderStatisticsQuery_Loaded);
        }

        void PurchaseOrderStatisticsQuery_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(PurchaseOrderStatisticsQuery_Loaded);
            this.facade = new PurchaseOrderFacade(CPApplication.Current.CurrentPage);
            if (null != filter)
            {
                QueryResultGrid.Bind();
            }
        }

        #region [Events]
        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.CountPurchaseOrders(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.QueryResultGrid.ItemsSource = args.Result.Rows;
                foreach (DynamicXml item in args.Result.Rows)
                {
                    item["status"] = EnumConverter.GetDescription(item["status"], typeof(PurchaseOrderStatus));
                }
                this.QueryResultGrid.ItemsSource = args.Result.Rows;
            });
        }

        #endregion
    }
}
