using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class OPCOfflineTransactionShow : UserControl
    {
        public IDialog Dialog { get; set; }

        public int MasterSysNo { get; set; }

        public IPage Page { get; set; }

        public OPCOfflineTransactionShow(IPage page)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(OPCOfflineTransactionShow_Loaded);
            Page = page;
        }

        void OPCOfflineTransactionShow_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SOQueryFacade facade = new SOQueryFacade(Page);
            facade.QueryOPCTransaction(MasterSysNo, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    dataGrid.ItemsSource = args.Result;
                }
            });
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }
    }
}
