using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Models.Product.ProductGroup;
using ECCentral.Service.IM.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using DataGridTextColumn = Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Portal.UI.IM.UserControls.ProductGroup
{
    public partial class ProductGroupMaintainAddSimilarProduct : UserControl
    {
        private ProductGroupMaintainSimilarProductVM _vm;

        public ProductGroupMaintainSimilarProductVM VM
        {
            get { return DataContext as ProductGroupMaintainSimilarProductVM; }
        }

        public IDialog Dialog { get; set; }

        public List<ProductCountry> CountryList { get; set; }

        public ProductGroupMaintainAddSimilarProduct()
        {
            InitializeComponent();

            (new ProductCreateFacade()).GetProductCountryList((obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CountryList = arg.Result;
            });
            _vm = new ProductGroupMaintainSimilarProductVM();

            new ProductGroupFacade().GetProductList(_vm.MainPageVM.ProductListVM.ProductGroupProductVMList
                .Select(p => p.ProductSysNo).ToList(), (o, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                _vm.GroupProductList = a.Result;
                new ProductGroupFacade().GetPropertyValueInfoByPropertySysNoList(
                _vm.MainPageVM.PropertyVM.ProductGroupSettings
                    .Where(setting => setting.ProductGroupProperty.SysNo != 0).Select(p => p.ProductGroupProperty.SysNo)
                    .ToList(), (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        _vm.MainPageVM.PropertyVM.ProductGroupSettings.ForEach(setting =>
                        {
                            if (args.Result.ContainsKey(setting.ProductGroupProperty.SysNo)
                                && args.Result[setting.ProductGroupProperty.SysNo].Count > 0)
                            {
                                var selectGroupProperty = new SelectGroupProperty
                                {
                                    Property = new PropertyVM
                                                   {
                                                       SysNo = setting.ProductGroupProperty.SysNo,
                                                       PropertyName = setting.ProductGroupProperty.PropertyName
                                                   }
                                };
                                args.Result[setting.ProductGroupProperty.SysNo]
                                    .ForEach(property =>
                                    {
                                        if (property.SysNo.HasValue)
                                        {
                                            selectGroupProperty.SelectedPropertyValueList.Add(
                                                new SelectedPropertyValue
                                                    {
                                                        IsChecked = _vm.GroupProductList
                                                        .Any(p => p.ProductBasicInfo.ProductProperties
                                                            .Any(value => value.Property.SysNo == property.SysNo)),
                                                        PropertyValue = new PropertyValueVM
                                                        {
                                                            SysNo = property.SysNo.Value,
                                                            ValueDescription = property.ValueDescription.Content
                                                        }
                                                    });
                                        }
                                    });
                                _vm.SelectGroupPropertyValue.Add(selectGroupProperty);
                            }
                        });



                        if (_vm.SelectGroupPropertyValue.Count > 0)
                        {
                            _vm.SelectGroupPropertyValue[0].SelectedPropertyValueList.ForEach(i =>
                            {
                                var control = new CheckBox();
                                var binding = new Binding
                                                  {
                                                      Source = i,
                                                      Path = new PropertyPath("IsChecked"),
                                                      Mode = BindingMode.TwoWay
                                                  };
                                control.SetBinding(ToggleButton.IsCheckedProperty, binding);
                                control.IsEnabled = !i.IsChecked;
                                control.Content = i.PropertyValue.ValueDescription;
                                control.Click += (CheckBoxClick);
                                FirstPropertyList.Children.Add(control);
                            });


                            dgProductList.Columns.Insert(1, new DataGridTextColumn
                            {
                                Header = _vm.SelectGroupPropertyValue[0].Property.PropertyName,
                                Binding = new Binding("PropertyValueList[0].ValueDescription")
                            });
                        }

                        if (_vm.SelectGroupPropertyValue.Count > 1)
                        {
                            _vm.SelectGroupPropertyValue[1].SelectedPropertyValueList.ForEach(i =>
                            {
                                var control = new CheckBox();
                                var binding = new Binding
                                {
                                    Source = i,
                                    Path = new PropertyPath("IsChecked"),
                                    Mode = BindingMode.TwoWay
                                };
                                control.SetBinding(ToggleButton.IsCheckedProperty, binding);
                                control.IsEnabled = !i.IsChecked;
                                control.Content = i.PropertyValue.ValueDescription;
                                control.Click += (CheckBoxClick);
                                SecondPropertyList.Children.Add(control);
                            });

                            dgProductList.Columns.Insert(2, new DataGridTextColumn
                            {
                                Header = _vm.SelectGroupPropertyValue[1].Property.PropertyName,
                                Binding = new Binding("PropertyValueList[1].ValueDescription")
                            });
                        }
                        BuildProductList();
                        DataContext = _vm;
                    });
            });
        }

        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            BuildProductList();
        }

        private void BtnAddSimilarClick(object sender, RoutedEventArgs e)
        {
            var productGroupFacade = new ProductGroupFacade();

            if (_vm.SelectedProduct.Any(p => p.IsChecked))
            {
                if (ValidationManager.Validate(dgProductList))
                {
                    productGroupFacade.CreateSimilarProduct(_vm, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        args.Result.SuccessProductList.ForEach(
                            p =>
                            VM.MainPageVM.ProductListVM.ProductGroupProductVMList.Add(
                                productGroupFacade.ConvertProductInfoEntityToProductGroupProductVM(p)));
                        CloseDialog(DialogResultType.OK, args.Result.ErrorProductList);
                    });
                }
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请至少选择一个商品！", MessageType.Warning);
            }

        }

        private void CkbSelectAllRowClick(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null && checkbox.IsChecked.HasValue)
            {
                if (checkbox.IsChecked.Value)
                {
                    VM.SelectedProduct.ForEach(product => product.IsChecked = true);
                }
                else
                {
                    VM.SelectedProduct.ForEach(product => product.IsChecked = false);
                }
            }
        }

        private void BuildProductList()
        {
            var productList = new List<SelectProduct>();
            if (_vm.SelectGroupPropertyValue.Count == 1)
            {
                _vm.SelectGroupPropertyValue[0].SelectedPropertyValueList
                    .Where(p => p.IsChecked).ToList().ForEach(p =>
                    {
                        var selectProduct = new SelectProduct
                        {
                            PropertyValueList = new List<PropertyValueVM>
                            {
                                new PropertyValueVM
                                {
                                    PropertySysNo = _vm.SelectGroupPropertyValue[0].Property.SysNo,
                                    SysNo = p.PropertyValue.SysNo,
                                    ValueDescription = p.PropertyValue.ValueDescription
                                }
                            },
                            ProductTitle = _vm.MainPageVM.BasicInfoVM.ProductGroupName + " " + p.PropertyValue.ValueDescription,
                            ProductModel = _vm.MainPageVM.BasicInfoVM.ProductGroupModel + " " + p.PropertyValue.ValueDescription,
                        };
                        selectProduct.CountryList = this.CountryList;
                        productList.Add(selectProduct);
                    });
            }
            if (_vm.SelectGroupPropertyValue.Count == 2)
            {
                _vm.SelectGroupPropertyValue[0].SelectedPropertyValueList
                    .Where(p => p.IsChecked).ToList()
                    .SelectMany(p1 => _vm.SelectGroupPropertyValue[1].SelectedPropertyValueList
                        .Where(p => p.IsChecked).ToList(), (p1, p2) => new { p1, p2 }).ToList().ForEach(p =>
                        {
                            var selectProduct = new SelectProduct
                            {
                                PropertyValueList = new List<PropertyValueVM>
                                                        {
                                                            new PropertyValueVM
                                                            {
                                                                PropertySysNo = _vm.SelectGroupPropertyValue[0].Property.SysNo,
                                                                SysNo = p.p1.PropertyValue.SysNo,
                                                                ValueDescription = p.p1.PropertyValue.ValueDescription
                                                            },
                                                            new PropertyValueVM
                                                            {
                                                                PropertySysNo = _vm.SelectGroupPropertyValue[1].Property.SysNo,
                                                                SysNo = p.p2.PropertyValue.SysNo,
                                                                ValueDescription = p.p2.PropertyValue.ValueDescription
                                                            }
                                                        },
                                ProductTitle = _vm.MainPageVM.BasicInfoVM.ProductGroupName
                                + " " + p.p1.PropertyValue.ValueDescription
                                + " " + p.p2.PropertyValue.ValueDescription,
                                ProductModel = _vm.MainPageVM.BasicInfoVM.ProductGroupModel
                                + " " + p.p1.PropertyValue.ValueDescription
                                + " " + p.p2.PropertyValue.ValueDescription,
                            };
                            selectProduct.CountryList = this.CountryList;
                            productList.Add(selectProduct);
                        });
            }

            var existProductList = new List<SelectProduct>();

            _vm.GroupProductList.ForEach(p =>
            {
                var product = new SelectProduct
                                  {
                                      ProductTitle = p.ProductBasicInfo.ProductTitle.Content,
                                      ProductModel = p.ProductBasicInfo.ProductModel.Content
                                  };
                product.CountryList = this.CountryList;
                
                p.ProductBasicInfo.ProductProperties.ToList().ForEach(property =>
                {
                    if (_vm.MainPageVM.PropertyVM.ProductGroupSettings
                        .Any(setting => setting.ProductGroupProperty.SysNo == property.Property.PropertyInfo.SysNo))
                    {
                        if (property.Property.SysNo.HasValue && property.Property.PropertyInfo.SysNo.HasValue)
                        {
                            product.PropertyValueList.Add(new PropertyValueVM
                            {
                                PropertySysNo = property.Property.PropertyInfo.SysNo.Value,
                                PropertyName = property.Property.PropertyInfo.PropertyName.Content,
                                SysNo = property.Property.SysNo.Value,
                                ValueDescription = property.Property.ValueDescription.Content
                            });
                        }
                    }
                });
                existProductList.Add(product);
            });

            var result = new List<SelectProduct>();

            productList.ForEach(p =>
                                    {
                                        if (!existProductList.Contains(p))
                                        {
                                            result.Add(p);
                                        }
                                    });
            _vm.SelectedProduct = result;
        }

        private void CloseDialog(DialogResultType dialogResult, List<ErrorProduct> errorProductList)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.ResultArgs.Data = errorProductList;
                Dialog.Close();
            }
        }

    }
}
