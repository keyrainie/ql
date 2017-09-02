using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Models.Category;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryKPIRMAInfo : UserControl, ICategroyKPI
    {
        public CategoryKPIRMAInfo()
        {
            InitializeComponent();
        }

        private void btnSaveRMAInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public int SysNo { get; set; }

        public void Save()
        {
            var vm = DataContext as CategoryKPIRMAInfoVM;
            if (vm == null)
            {
                return;
            }
            if (!ValidationManager.Validate(this))
            {
                return;
            }  

            var _facade = new CategoryKPIFacade();
            _facade.UpdateCategoryRMA(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    //var errorMsg = args.Error.Faults[0].ErrorDescription;
                    //CPApplication.Current.CurrentPage.Context.Window.Alert(errorMsg);
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
            });
        }

        public void Bind(ModelBase data)
        {
            if (data != null)
            {
                var source = (CategoryKPIRMAInfoVM)data;
                this.DataContext = source;
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得三级分类RMA信息.", MessageBoxType.Warning);
            }
        }
    }
}
