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
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryKPIMinMargin : UserControl, ICategroyKPI
    {
        public CategoryType Type {private get; set; }
      
        public CategoryKPIMinMargin()
        {
            InitializeComponent();
        }

        private void btnMinMargin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public int SysNo { get; set; }

        public void Save()
        {
            var vm = DataContext as CategoryKPIMinMarginVM;
            if (vm == null)
            {
                return;
            }
            if (!ValidationManager.Validate(this))
            {
                return;
            }  
            vm.GetMargin();
            var _facade = new CategoryKPIFacade();
            _facade.UpdateCategoryMinMargin(vm,Type,(obj, args) =>
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

        /// <summary>
        /// 根据不同类型保存
        /// </summary>
        /// <param name="Category3List"></param>
        /// <param name="type"></param>
        public void SaveCategoryByType(List<CategoryInfo> CategoryList,CategoryType type)
        {
            var vm = DataContext as CategoryKPIMinMarginVM;
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            vm.GetMargin();
            var _facade = new CategoryKPIFacade();
            List<CategoryKPIMinMarginVM> list = new List<CategoryKPIMinMarginVM>();
            
            foreach (var item in CategoryList)
            {
                list.Add(new CategoryKPIMinMarginVM() { CategorySysNo = (int)item.SysNo, M1 = vm.M1, M2 = vm.M2, M3 = vm.M3, M4 = vm.M4, M5 = vm.M5, M6 = vm.M6, Margin = vm.Margin });
            }
             if (list.Count > 0)
             {
                 _facade.UpdateCategoryMinMarginByType(list,type,(obj, arg) => 
                 {
                     if (arg.FaultsHandle())
                     {
                         return;
                     }
                     CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);
                 });
             }
        }
        
        public void Bind(ModelBase data)
        {
            if (data != null)
            {
                var source = (CategoryKPIMinMarginVM)data;
                var value = source.Margin;
                this.DataContext = source;
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得三级分类基本信息.", MessageBoxType.Warning);
            }
        }
    }
}
