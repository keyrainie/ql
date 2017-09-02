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
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryKPIBasicInfo : UserControl, ICategroyKPI
    {
        public CategoryType Type { get; set; }
        public CategoryKPIBasicInfo()
        {
            InitializeComponent();
           
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public int SysNo { get; set; }

        public void Save()
        {
            var vm = DataContext as CategoryKPIBasicInfoVM;
            if (vm == null)
            {
                return;
            }
            if (!ValidationManager.Validate(this))
            {
                return;
            }             

            var _facade = new CategoryKPIFacade();
            _facade.UpdateCategoryBasic(vm,Type,(obj, args) =>
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
                
                var source = (CategoryKPIBasicInfoVM)data;
                this.DataContext = source;
                InitUserControl();
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得三级分类基本信息.", MessageBoxType.Warning);
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryKPIBasicInfoVM vm=this.DataContext as CategoryKPIBasicInfoVM;
            UpdateProductCommissionQuotaBat detail = new UpdateProductCommissionQuotaBat();
            detail.Type = Type;
            detail.Source = new ProductCommissionQuotaVM() {Category1SysNo=vm.Category1SysNo,Category2SysNo=vm.Category2SysNo, Category3SysNo = vm.CategorySysNo,CommissionMin=vm.MinCommission};
            detail.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("批量修改商品最低佣金限价", detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                   
                }
            }, new Size(600, 400));
        }
        /// <summary>
        /// 初始化当前页
        /// </summary>
        private void InitUserControl()
        {
            if (Type == CategoryType.CategoryType3)
            {
                this.spPayPeriodTypeAndLargeFlag.Visibility = Visibility.Visible;
                this.spRequired.Visibility = Visibility.Visible;
            }
            else
            {
                
                this.spAvgDailySalesCycle.Visibility = Visibility.Visible;
                epStockDays.Visibility = Visibility.Visible;
             
            }
        }
    }
}
