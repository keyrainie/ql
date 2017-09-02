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
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductAccessoriesEditMaintain : UserControl
    {

        public ProductAccessoriesVM Data { private get; set; }
        private ProductAccessoriesFacade facade;
        public IDialog Dialog { get; set; }
        public bool IsEdit { private get; set; } //是否编辑
        public ProductAccessoriesEditMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                if (IsEdit)
                {
                    this.DataContext = Data;
                }
                else
                {
                    this.DataContext = new ProductAccessoriesVM();
                }
                
                facade = new ProductAccessoriesFacade();
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            ProductAccessoriesVM vm = this.DataContext as ProductAccessoriesVM;
            if (IsEdit)
            {
                facade.UpdateProductAccessories(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {
                facade.CreateProductAccessories(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
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
