using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.TariffPicker;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.UserControls.LanguagesDescription;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainBasicInfoDisplayInfo : UserControl
    {

        public ProductMaintainBasicInfoDisplayInfoVM VM
        {
            get
            {
                return DataContext as ProductMaintainBasicInfoDisplayInfoVM;
            }
        }

        public IPage CurrentPage
        {
            get { return CPApplication.Current.CurrentPage; }
        }

        public ProductMaintainBasicInfoDisplayInfo()
        {
            InitializeComponent();
        }

        //protected void Button_Search_Click(object sender, RoutedEventArgs e)
        //{
        //    TariffInfoVM entity = new TariffInfoVM();
        //    UCTariffSelectPicker uct2 = new UCTariffSelectPicker();
        //    uct2.Dialog = CurrentPage.Context.Window.ShowDialog("税率规则信息查询", uct2, (s, args) =>
        //    {
        //        if (args.DialogResult == DialogResultType.OK && args.Data != null)
        //        {
        //            entity = args.Data as TariffInfoVM;
        //            this.taxNo.Text = entity.TariffCode;
        //        }
        //    });
        //}

        private void Button_LanguageSetting_Click(object sender, RoutedEventArgs e)
        {

            UCLanguagesDescription ut3 = new UCLanguagesDescription("Product", VM.ProductID);
            ut3.Dialog = CurrentPage.Context.Window.ShowDialog("多语言设置", ut3, (s, args) =>
                {

                });
        }
    }
}
