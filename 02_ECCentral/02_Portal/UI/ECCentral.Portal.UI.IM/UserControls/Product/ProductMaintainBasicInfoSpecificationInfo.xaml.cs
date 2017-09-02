using System.Windows.Controls;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;


namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainBasicInfoSpecificationInfo : UserControl
    {
        public ProductMaintainBasicInfoSpecificationInfoVM VM
        {
            get
            {
                return DataContext as ProductMaintainBasicInfoSpecificationInfoVM;
            }
        }

        public ProductMaintainBasicInfoSpecificationInfo()
        {
            InitializeComponent();
            ucBrandPicker.selectedBrandCompletedHandler += (ucBrandPicker_selectedBrandCompletedHandler);
        }

        void ucBrandPicker_selectedBrandCompletedHandler(object sender, System.EventArgs e)
        {
            VM.ManufacturerInfo.SysNo = ucBrandPicker.ManufacturerSysNo;
            VM.ManufacturerInfo.ManufacturerNameLocal = ucBrandPicker.ManufacturerName;
        }
    }
}
