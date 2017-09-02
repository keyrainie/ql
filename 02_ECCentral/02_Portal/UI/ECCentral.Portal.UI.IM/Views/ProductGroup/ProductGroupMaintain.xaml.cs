using System;
using System.Linq;
using System.Windows;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductGroupMaintain : PageBase
    {

        private ProductGroupFacade _productGroupFacade;

        private ProductGroupMaintainVM _vm;

        public ProductGroupMaintainVM VM
        {
            get { return DataContext as ProductGroupMaintainVM; }
        }

        public ProductGroupMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            int productGroupSysNo;

            _vm = new ProductGroupMaintainVM();

            if (Int32.TryParse(Request.Param, out productGroupSysNo))
            {
                _productGroupFacade = new ProductGroupFacade(this);

                _vm.CreateFlag = false;

                ucProductGroupMaintainBasicInfo.ucCategoryPicker
                    .LoadCategoryCompleted += (s, arg)
                        => _productGroupFacade.GetProductGroupInfoBySysNo(productGroupSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null || !args.Result.SysNo.HasValue || args.Result.ProductList.Count == 0)
                    {
                        Window.MessageBox.Show("无此商品组或商品组数据完整性有误，请联系管理员检查", MessageBoxType.Error);
                        return;
                    }
                    ucProductGroupMaintainBasicInfo.ucCategoryPicker.cmbCategory3SelectionChanged -= ucProductGroupMaintainBasicInfo.C3SelectChangedClick;
                    _vm.ProductGroupSysNo = args.Result.SysNo.Value;
                    _vm.BasicInfoVM =
                        _productGroupFacade.ConvertProductGroupEntityToProductGroupMaintainBasicInfoVM(args.Result);
                    _vm.ProductListVM =
                        _productGroupFacade.ConvertProductGroupEntityToProductGroupMaintainProductListVM(args.Result);
                    var sysNo = args.Result.ProductList.First().ProductBasicInfo.ProductCategoryInfo.SysNo;
                    if (sysNo.HasValue)
                    {
                        _productGroupFacade.GetCategorySetting(sysNo.Value, (o, a) =>
                        {
                            if (a.FaultsHandle())
                            {
                                return;
                            }

                            var categoryPropertyList = _productGroupFacade.
                                    ConvertCategoryPropertyListToPropertyVMList
                                    (a.Result.CategoryProperties.Where(property => property.PropertyType == PropertyType.Grouping));

                            categoryPropertyList.Insert(0, new PropertyVM
                            {
                                SysNo = 0,
                                PropertyName = "请选择..."
                            });

                            _vm.PropertyVM =
                                _productGroupFacade.ConvertProductGroupEntityToProductGroupMaintainPropertySettingVM(
                                    args.Result);

                            _vm.PropertyVM.CategoryPropertyList
                                = categoryPropertyList;

                            DataContext = _vm;
                            ucProductGroupMaintainProductList.dgProductGroupProductList.ItemsSource =
                                _vm.ProductListVM.ProductGroupProductVMList;
                        });
                    }
                });
            }
            else
            {
                _vm.CreateFlag = true;
                DataContext = _vm;
            }
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            _productGroupFacade = new ProductGroupFacade(this);

            if (ValidationManager.Validate(this))
            {
                if (!CheckSaveCondition())
                {
                    return;
                }

                if (VM.CreateFlag)
                {
                    _productGroupFacade.CreateProductGroupInfo(VM, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        if (!args.Result.SysNo.HasValue)
                        {
                            Window.MessageBox.Show("商品组创建异常，请查系统日志", MessageBoxType.Error);
                            return;
                        }
                        Window.MessageBox.Show("商品组创建成功", MessageBoxType.Success);
                        VM.CreateFlag = false;
                        VM.ProductGroupSysNo = args.Result.SysNo.Value;
                    });
                }
                else
                {
                    _productGroupFacade.UpdateProductGroupInfo(VM, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Window.MessageBox.Show("商品组更新成功", MessageBoxType.Success);
                    });
                }
            }
        }

        private bool CheckSaveCondition()
        {
            if (string.IsNullOrEmpty(VM.BasicInfoVM.ProductGroupName))
            {
                Window.Alert("商品组名称不能为空！", MessageType.Error);
                return false;
            }

            if (string.IsNullOrEmpty(VM.BasicInfoVM.ProductGroupModel))
            {
                Window.Alert("商品组型号不能为空！", MessageType.Error);
                return false;
            }

            if (!VM.BasicInfoVM.ProductGroupCategory.SysNo.HasValue)
            {
                Window.Alert("必须选择三级类别！", MessageType.Error);
                return false;
            }

            if (!VM.BasicInfoVM.ProductGroupBrand.SysNo.HasValue)
            {
                Window.Alert("必须选择品牌！", MessageType.Error);
                return false;
            }

            if (VM.ProductListVM.ProductGroupProductVMList.Any())
            {
                if (VM.ProductListVM.ProductGroupProductVMList.First().ProductBrand.SysNo != VM.BasicInfoVM.ProductGroupBrand.SysNo)
                {
                    Window.Alert("商品品牌必须为" + VM.BasicInfoVM.ProductGroupBrand.BrandNameLocal, MessageType.Error);
                    return false;
                }

                if (VM.ProductListVM.ProductGroupProductVMList.First().ProductCategory.SysNo != VM.BasicInfoVM.ProductGroupCategory.SysNo)
                {
                    Window.Alert("商品类别必须为" + VM.BasicInfoVM.ProductGroupCategory.CategoryName, MessageType.Error);
                    return false;
                }
            }
            else
            {
                Window.Alert("必须选择至少一个商品！", MessageType.Error);
                return false;
            }
            return true;
        }
    }
}
