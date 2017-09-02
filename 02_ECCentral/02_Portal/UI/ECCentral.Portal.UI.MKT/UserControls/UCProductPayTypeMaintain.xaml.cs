using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.UI.MKT.Models.Promotion;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.MKT.Promotion;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCProductPayTypeMaintain : UserControl
    {
        readonly IWindow _window = CPApplication.Current.CurrentPage.Context.Window;
        public IDialog Dialog { get; set; }
        private ProductPayTypeFacade _facade;
        private ProductPayTypeVM _productPayTypeVm;
        public UCProductPayTypeMaintain()
        {
            InitializeComponent();
            Loaded += (sender, e) =>
            {
                _facade = new ProductPayTypeFacade();
                _productPayTypeVm = new ProductPayTypeVM();
                //绑定支付方式列表
                //查询PM列表
                _facade.GetProductPayTypeList((obj, args) =>
                {
                    this.DataContext = _productPayTypeVm;
                    checkBoxListPayType.ItemsSource = args.Result;
                });
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            var payTypeList = (IEnumerable<PayTypeInfo>)checkBoxListPayType.ItemsSource;
            if (string.IsNullOrEmpty(txtProductList.Text))
            {

                _window.MessageBox.Show(ResProductPayTypeMaintain.Error_ProductIsEmpty, MessageBoxType.Warning);
                return;
            }

            var count = payTypeList.Count(p => p.IsChecked);
            if (count == 0)
            {
                _window.MessageBox.Show(ResProductPayTypeMaintain.Error_PayTypeIsEmpty, MessageBoxType.Warning);
                return;
            }
            if (!dpBeginDateFrom.SelectedDateTime.HasValue)
            {
                _window.MessageBox.Show(ResProductPayTypeMaintain.Error_BeginDateIsEmpty, MessageBoxType.Warning);
                return;
            }
            if (!dpEndDateTo.SelectedDateTime.HasValue)
            {
                _window.MessageBox.Show(ResProductPayTypeMaintain.Error_EndDateIsEmpty, MessageBoxType.Warning);
                return;
            }
            if (dpBeginDateFrom.SelectedDateTime.Value.CompareTo(dpEndDateTo.SelectedDateTime.Value) > 0)
            {
                _window.MessageBox.Show(ResProductPayTypeMaintain.Error_BeginNotGreaterEnd, MessageBoxType.Warning);
                return;
            }
            if (dpBeginDateFrom.SelectedDateTime.Value.CompareTo(dpEndDateTo.SelectedDateTime.Value) == 0)
            {
                _window.MessageBox.Show(ResProductPayTypeMaintain.Error_BeginTimeEqualToEndTime, MessageBoxType.Warning);
                return;
            }
            _productPayTypeVm.PayTypeList = payTypeList.Where(p => p.IsChecked).ToList();
            _productPayTypeVm.EditUser = CPApplication.Current.LoginUser.LoginName;
            _facade.BatchCreateProductPayType(_productPayTypeVm, (obj, args) =>
                                                                     {
                                                                         args.FaultsHandle();
                CloseDialog(DialogResultType.OK);
            });

        }

        private void BtnCanel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void linkSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            var item = new UCProductSearch { SelectionMode = SelectionMode.Multiple };
            item.DialogHandler = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("选择商品", item, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var selectedProducts = item._viewModel.SelectedProducts;
                    foreach (var product in selectedProducts.Where(product => !txtProductList.Text.Contains(product.ProductID)))
                    {
                        if (string.IsNullOrEmpty(txtProductList.Text))
                        {
                            txtProductList.Text = product.ProductID;
                        }
                        else
                        {
                            txtProductList.Text = txtProductList.Text + "\r" + product.ProductID;
                        }
                    }
                }
            });
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
