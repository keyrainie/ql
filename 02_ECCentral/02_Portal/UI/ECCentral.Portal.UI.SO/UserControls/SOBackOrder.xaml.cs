using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOBackOrder : UserControl
    {
        int m_soSysNo;

        public IDialog Dialog { get; set; }

        public SOBackOrder(int soSysNo)
        {
            m_soSysNo = soSysNo;
            InitializeComponent();
            Loaded += new RoutedEventHandler(SOBackOrder_Loaded);
        }

        void SOBackOrder_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SOFacade facade = new SOFacade(CPApplication.Current.CurrentPage);
            facade.QueryBackOrderItem(m_soSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGrid.TotalCount = args.Result.TotalCount;
                dataGrid.ItemsSource = args.Result.Rows;
            });
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.Close();
            }
        }
    }
}
