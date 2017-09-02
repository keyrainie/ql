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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Views;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Text;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCSaleAdvItemBatchAdd : UserControl
    {
        public IDialog Dialog { get; set; }
        public SaleAdvTemplateItemMaintain Page { get; set; }

        public UCSaleAdvItemBatchAdd()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as SaleAdvItemVM;

            if (ValidationManager.Validate(this.LayoutRoot))
            {
                var group = vm.Groups.FirstOrDefault(p => p.SysNo == vm.GroupSysNo);
                if (group == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("请先添加分组信息!", MessageType.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(vm.Introduction))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("请选择商品!", MessageType.Warning);
                    return;
                }
                vm.RecommendType = group.RecommendType;

                if (group != null && group.SysNo > 0)
                {
                    vm.GroupPriority = int.Parse(group.Priority);
                    vm.GroupName = group.GroupName;
                }
                new SaleAdvTemplateFacade(this.Page).BatchCreateSaleAdvItem(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }

                    var result = new StringBuilder();
                    result.AppendLine("Result:");
                    arg.Result.SuccessRecords.ForEach(sp => result.AppendLine("商品添加成功，" + sp));
                    arg.Result.FailureRecords.ForEach(ep => result.AppendLine("添加失败，" + ep));

                    this.Dialog.Close();
                    this.ProductResult.Text = string.Empty;
                    if (arg.Result.SuccessRecords.Count() > 0)
                    {
                        this.Page.Window.Refresh();
                    }
                    this.Page.Window.MessageBox.Show(result.ToString()
                       , Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Warning);
                });
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }

        private void HyProductSelect_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch item = new UCProductSearch();
            item.SelectionMode = SelectionMode.Multiple;
            item.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商品", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    ProductBind(item._viewModel.SelectedProducts);
                }

            });
        }

        private void ProductBind(ObservableCollection<ProductVM> listProduct)
        {
            string SelectProduct = this.ProductResult.Text;
            foreach (var item in listProduct)
            {
                if (string.IsNullOrEmpty(SelectProduct))
                {
                    SelectProduct = item.ProductID;
                }
                else
                {
                    SelectProduct = SelectProduct + "\r" + item.ProductID;
                }
            }

            this.ProductResult.Text = SelectProduct ?? "";
        }
    }
}
