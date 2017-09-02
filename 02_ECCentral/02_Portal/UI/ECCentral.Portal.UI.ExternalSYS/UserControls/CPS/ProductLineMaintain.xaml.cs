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
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class ProductLineMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public ProductLineVM Data {private get; set; } //更新时接受数据源
        public bool IsEdit {private get; set; }
        private ProductLineVM model;
        private ProductLineFacade facade;
        public ProductLineMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                facade = new ProductLineFacade();
                if (IsEdit)
                {
                    model = Data;
                }
                else
                {
                    model = new ProductLineVM();
                }
                List<ProductLineCategoryVM> templist = new List<ProductLineCategoryVM>();
                templist.Add(new ProductLineCategoryVM() { CategoryName = "--请选择--", SysNo = 0 });
                facade.GetAllProductLineCategory((obj, arg) => 
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
 
                    foreach (var item in arg.Result.Rows)
                    {
                        templist.Add(new ProductLineCategoryVM() { SysNo = item.SysNo, CategoryName = item.Name });
                    }
                    model.CategoryList = templist;
                    if (!IsEdit) //新建时才需要默认请选择
                    {
                        model.Category = (from p in templist where p.SysNo == 0 select p).ToList().FirstOrDefault();
                    }
                    else
                    {
                        model.Category = (from p in templist where p.SysNo == Data.Category.SysNo select p).ToList().FirstOrDefault();
                    }
                    this.DataContext = model;
                });
                 this.DataContext = model;
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            ProductLineVM vm = this.DataContext as ProductLineVM;
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (vm.Category.SysNo == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择产品线分类",MessageType.Error);
                return;
            }

            if (vm.SysNo > 0)
            {
                facade.UpdateProductLine(vm, (obj, arg) => 
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功!");
                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {
                facade.CreateProductLine(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("添加成功!");
                    CloseDialog(DialogResultType.OK);
                });
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
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
