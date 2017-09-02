using System;
using System.Linq;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductGroupMaintainBasicInfo : UserControl
    {
        public ProductGroupMaintainBasicInfoVM VM
        {
            get { return DataContext as ProductGroupMaintainBasicInfoVM; }
        }

        public ProductGroupMaintainBasicInfo()
        {
            InitializeComponent();
        }

        public void C3SelectChangedClick(object sender, EventArgs e)
        {
            var cmbSelect = (Combox)sender;

            if (cmbSelect.SelectedValue != null)
            {
                var productGroupFacade = new ProductGroupFacade();
                productGroupFacade.GetCategorySetting((int)cmbSelect.SelectedValue, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    var categoryPropertyList = productGroupFacade.ConvertCategoryPropertyListToPropertyVMList(
                            args.Result.CategoryProperties.Where(
                                property => property.PropertyType == PropertyType.Grouping));

                    categoryPropertyList.Insert(0, new PropertyVM
                    {
                        SysNo = 0,
                        PropertyName = "请选择..."
                    });

                    var currentPage = (Views.ProductGroupMaintain)CPApplication.Current.CurrentPage;
                    currentPage.VM.PropertyVM.CategoryPropertyList = categoryPropertyList;
                    RefreshProductGroupSettingVM(currentPage.VM.PropertyVM.ProductGroupSettings[0]);
                    RefreshProductGroupSettingVM(currentPage.VM.PropertyVM.ProductGroupSettings[1]);
                });
            }
        }

        private void RefreshProductGroupSettingVM(ProductGroupSettingVM vm)
        {
            vm.ProductGroupProperty.SysNo = 0;
            vm.PropertyBriefName = String.Empty;
            vm.ImageShow = ProductGroupImageShow.No;
            vm.Polymeric = ProductGroupPolymeric.No;
        }
    }
}
