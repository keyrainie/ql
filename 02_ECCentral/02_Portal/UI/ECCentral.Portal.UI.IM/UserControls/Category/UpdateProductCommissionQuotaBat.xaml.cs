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
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UpdateProductCommissionQuotaBat : UserControl
    {
        public IDialog Dialog { get; set; }
        public ProductCommissionQuotaVM Source { private get; set; }
        public CategoryType Type {private get; set; }
        private CategoryKPIFacade facade;
    

        public UpdateProductCommissionQuotaBat()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                if (Source.Category2SysNo == Source.Category3SysNo) //如果是二级类别的话页面传过来的值会相等。
                {
                    Source.Category3SysNo = null; 
                }
                this.DataContext = Source;
                if (Type == CategoryType.CategoryType3)
                {
                    myUCCategoryPicker.Category3Visibility = Visibility.Visible;
                }
                cboCategoryType.SelectedValue = Type;
                facade = new CategoryKPIFacade();
            };

            this.BtnClose.Click += (sender, e) => 
            {
                CloseDialog(DialogResultType.Cancel);
            };

            this.BtnSave.Click += (sender, e) => 
            {
                ProductCommissionQuotaVM vm = this.DataContext as ProductCommissionQuotaVM;
                if (!ValidationManager.Validate(this))
                {
                    return;
                }
                if (vm.Category2SysNo == 0 || vm.Category2SysNo == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("C2级别不能为所有", MessageType.Error);
                    return;
                }
                if (vm.Category3SysNo != null && vm.Category3SysNo >0) //三级类不为null 则保存三级类的限额
                {
                    facade.UpdateCategoryProductMinCommission(vm, (obj, arg) =>
                    {
                        if (arg.FaultsHandle())
                        {

                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功");
                    });
                }
                else //保存该二级类下所有的三级类限额
                {
                    var categorylist = (from p in myUCCategoryPicker.Category3List where p.ParentSysNumber == vm.Category2SysNo select p).ToList();
                    List<ProductCommissionQuotaVM> list = new List<ProductCommissionQuotaVM>();
                    if (categorylist.Count > 0)
                    {
                        foreach (var item in categorylist)
                        {

                            list.Add(new ProductCommissionQuotaVM() {Category1SysNo=vm.Category1SysNo,Category2SysNo=vm.Category2SysNo,Category3SysNo=(int)item.SysNo,CategoryType=vm.CategoryType,CommissionMin=vm.CommissionMin,ManufacturerSysNo=vm.ManufacturerSysNo,PMSysNo=vm.PMSysNo,ProductStatus=vm.ProductStatus,Comparison=vm.Comparison});
                        }
                        facade.UpdateCategory2ProductMinCommission(list, (obj, arg) => 
                        {
                            if (arg.FaultsHandle())
                            {

                                return;
                            }
                            CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功");
                        });
                    }
                }
            };

            cboCategoryType.SelectionChanged += (sender, e) =>
            {
                CategoryType categoryType = (CategoryType)cboCategoryType.SelectedValue;
                if (categoryType == CategoryType.CategoryType3)
                {
                    myUCCategoryPicker.Category3Visibility = Visibility.Visible;
                  
                }
                else
                {
                    myUCCategoryPicker.Category3Visibility = Visibility.Collapsed;
                }
              
            };
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

    }
}
