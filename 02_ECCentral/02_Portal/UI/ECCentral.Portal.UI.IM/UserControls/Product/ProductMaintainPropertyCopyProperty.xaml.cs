using System.Windows;
using System.Windows.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainPropertyCopyProperty : UserControl
    {
        public ProductCopyPropertyVM VM
        {
            get { return DataContext as ProductCopyPropertyVM; }
        }


        public IDialog Dialog { get; set; }

        public ProductMaintainPropertyCopyProperty()
        {
            InitializeComponent();
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            var productCopyPropertyFacade = new ProductCopyPropertyFacade();
            productCopyPropertyFacade.ProductCopyProperty(VM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResProductMaintain.Info_SaveSuccessfully);
            });
        }
    }
}
