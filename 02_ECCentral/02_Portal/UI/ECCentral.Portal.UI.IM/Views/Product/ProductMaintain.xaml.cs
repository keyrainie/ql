using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;

using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Browser;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.Portal.UI.IM.UserControls.Product;


namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductMaintain : PageBase
    {
        private ProductFacade _productFacade;
        private ProductLineFacade _productLineFacade;
        private ProductVM _vm;

        public ProductPriceRequestFacade _facade;

        public ProductVM VM
        {
            get { return DataContext as ProductVM; }
        }

        public ProductMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            #region 2012-12-18 Jack.G.tang Update
            /*
             *修改目的:点击商品列表中的价格链接到此页面，并选中价格tab 其他tab禁用
             * 新加一个方法（SetTabFocus）和以下代码
             */

            string opertionType = string.Empty;
            string tempSysNo = string.Empty;
            string cookieTabIndex = string.Empty;
            if (this.Request.QueryString != null)
            {
                opertionType = this.Request.QueryString["operation"];
                tempSysNo = this.Request.QueryString["ProductSysNo"];
            }
            if (!string.IsNullOrEmpty(Request.Param))
            {
                tempSysNo = Request.Param;
            }
            #endregion

            this.ucProductMaintainProperty.MyWindow = Window;
            this.ucProductMaintainAccessory.MyWindow = Window;
            this.productEntryInfo.MyWindow = Window;

            int productSysNo;
            if (Int32.TryParse(tempSysNo, out productSysNo))
            {
                _productFacade = new ProductFacade(this);
                _vm = new ProductVM();
                //cookieTabIndex = GetCookie("CookieTabIndex");

                _productLineFacade = new ProductLineFacade(this);

                ucProductMaintainBasicInfo.ucSpecificationInfo.ucCategoryPicker.LoadCategoryCompleted += (s, args2) => _productFacade.GetProductInfo(productSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null || args.Result.ProductBasicInfo == null)
                    {
                        Window.MessageBox.Show("无此商品或商品数据完整性有误，请联系管理员检查", MessageBoxType.Error);
                        return;
                    }

                    if (!_vm.HasItemMaintainAllTypePermission && args.Result.ProductBasicInfo.ProductType != ProductType.OpenBox)
                    {
                        Window.MessageBox.Show("该商品不是二手品，无此商品访问权限！", MessageBoxType.Error);
                        return;
                    }

                    var pmQueryFacade = new PMQueryFacade(CPApplication.Current.CurrentPage);
                    var pmQueryFilter = new ProductManagerQueryFilter
                    {
                        UserName = CPApplication.Current.LoginUser.LoginName,
                        CompanyCode = CPApplication.Current.CompanyCode
                    };
                    if (AuthMgr.HasFunctionPoint(AuthKeyConst.IM_SeniorPM_Query))//高级权限
                    {
                        pmQueryFilter.PMQueryType = PMQueryType.All.ToString();
                    }
                    else if (AuthMgr.HasFunctionPoint(AuthKeyConst.IM_JuniorPM_Query))
                    {
                        pmQueryFilter.PMQueryType = PMQueryType.SelfAndInvalid.ToString();
                    }
                    else
                    {
                        pmQueryFilter.PMQueryType = PMQueryType.None.ToString();
                    }
                    int c3sysno = args.Result.ProductBasicInfo.ProductCategoryInfo.SysNo.HasValue ? args.Result.ProductBasicInfo.ProductCategoryInfo.SysNo.Value : 0;
                    int brandsysno = args.Result.ProductBasicInfo.ProductBrandInfo.SysNo.HasValue ? args.Result.ProductBasicInfo.ProductBrandInfo.SysNo.Value : 0;
                    int usersysno = CPApplication.Current.LoginUser.UserSysNo.Value;

                    _productLineFacade.HasRightByPMUser(usersysno
                        , c3sysno
                        , brandsysno, (pmobj, pmargs) =>
                    {
                        if (pmargs.FaultsHandle())
                        {
                            return;
                        }
                        bool userright = AuthMgr.HasFunctionPoint(AuthKeyConst.IM_SeniorPM_Query);
                        if (!userright)
                        {
                            if (!pmargs.Result)
                            {
                                Window.MessageBox.Show("无此商品访问权限！", MessageBoxType.Error);
                                return;
                            }
                        }

                        _vm.ProductSysNo = args.Result.SysNo;
                        #region 商品退换货信息需要productSysNo
                        productRmaPolicy.ProductSysNo = _vm.ProductSysNo;
                        #endregion
                        _vm.Note = args.Result.ProductBasicInfo.Note;
                        _vm.ProductMaintainBasicInfo = new ProductMaintainBasicInfoVM
                        {
                            ProductMaintainBasicInfoSpecificationInfo = _productFacade.ConvertProductEntityToProductMaintainBasicInfoSpecificationInfoVM(args.Result),
                            ProductMaintainBasicInfoDisplayInfo = _productFacade.ConvertProductEntityToProductMaintainBasicInfoDisplayInfoVM(args.Result),
                            ProductMaintainBasicInfoStatusInfo = _productFacade.ConvertProductEntityToProductMaintainBasicInfoStatusInfoVM(args.Result),
                            ProductMaintainBasicInfoChannelInfo = _productFacade.ConvertProductEntityToProductMaintainBasicInfoChannelInfoVM(args.Result),
                            ProductMaintainBasicInfoDescriptionInfo = _productFacade.ConvertProductEntityToProductMaintainBasicInfoDescriptionInfoVM(args.Result),
                            ProductMaintainBasicInfoOther = _productFacade.ConvertProductEntityToProductMaintainBasicInfoOtherVM(args.Result)
                        };
                        _vm.ProductMaintainDescription = _productFacade.ConvertProductEntityToProductMaintainDescriptionVM(args.Result);
                        _vm.ProductMaintainBatchManagementInfo = _productFacade.ConvertProductEntityToProductMaintainBatchManagementInfoVM(args.Result);

                        _vm.ProductMaintainAccessory = new ProductMaintainAccessoryVM
                        {
                            ProductAccessoryList = _productFacade.ConvertProductEntityToProductMaintainAccessoryProductAccessoryVMList(args.Result),
                            IsAccessoryShow = args.Result.ProductBasicInfo.IsAccessoryShow
                        };

                        _vm.ProductMaintainImage = _productFacade.ConvertProductEntityToProductMaintainImageVM(args.Result);

                        var priceInfo = _productFacade.ConvertProductEntityToProductMaintainPriceInfoVM(args.Result);
                        if (args.Result.ProductConsignFlag == VendorConsignFlag.Consign
                        && args.Result.Merchant.SysNo != -1)
                        {
                            priceInfo.ProductMaintainPriceInfoBasicPrice.HasMinCommissionVisible = "Visible";
                        }
                        _vm.ProductMaintainPriceInfo = priceInfo;
                        //_vm.ProductMaintainThirdPartyInventory = _productFacade.ConvertProductEntityToProductMaintainThirdPartyInventoryVM(args.Result);

                        _productFacade.GetProductGroup(productSysNo, (o, group) =>
                        {
                            if (@group.FaultsHandle())
                            {
                                return;
                            }
                            _vm.ProductMaintainCommonInfo = @group.Result.SysNo.HasValue ? _productFacade.ConvertProductGroupEntityToProductMaintainCommonInfoVM(@group.Result, args.Result) : _productFacade.ConvertProductEntityToProductMaintainCommonInfoVM(args.Result);

                            _vm.ProductMaintainProperty = _productFacade.ConvertProductEntityToProductMaintainPropertyVM(args.Result, group.Result);

                            _productFacade.GetPropertyValueList(args.Result.ProductBasicInfo.ProductProperties.Select(p => p.Property.PropertyInfo.SysNo.HasValue ? p.Property.PropertyInfo.SysNo.Value : 0).ToList(), (ob, arg) =>
                            {
                                if (args.FaultsHandle())
                                {
                                    return;
                                }
                                foreach (var productMaintainPropertyPropertyValueVM in _vm.ProductMaintainProperty.ProductPropertyValueList)
                                {
                                    productMaintainPropertyPropertyValueVM.PropertyValueList = new List<PropertyValueVM>();
                                    foreach (var i in arg.Result.Keys)
                                    {
                                        if (productMaintainPropertyPropertyValueVM.Property.SysNo == i)
                                        {
                                            productMaintainPropertyPropertyValueVM.PropertyValueList = _productFacade.ConvertPropertyValueInfoToPropertyValueVM(arg.Result[i]);
                                        }
                                    }
                                    productMaintainPropertyPropertyValueVM.PropertyValueList.Insert(0, new PropertyValueVM
                                    {
                                        SysNo = 0,
                                        ValueDescription = "请选择..."
                                    });
                                }

                                DataContext = _vm;
                            });

                            _vm.ProductMaintainWarranty = _productFacade.ConvertProductEntityToProductMaintainWarrantyVM(args.Result);

                            _vm.ProductMaintainDimension = _productFacade.ConvertProductEntityToProductMaintainDimensionVM(args.Result);

                            _vm.ProductMaintainPurchaseInfo = _productFacade.ConvertProductEntityToProductMaintainPurchaseInfoVM(args.Result);

                            _vm.ProductMaintainSalesArea = new ProductMaintainSalesAreaVM
                            {
                                ProductMaintainSalesAreaSaveList = _productFacade.ConvertProductInfoEntityToProductMaintainSalesAreaSelectVM(args.Result)
                            };

                            DataContext = _vm;

                            ucProductMaintainCommonInfo.dgRelatedProduct.ItemsSource = _vm.ProductMaintainCommonInfo.GroupProductList;

                            foreach (var control in VM.ProductMaintainImage.ProductImageList.Select(i => new ProductMaintainProductImageSingle { VM = i }))
                            {
                                ucProductMaintainImage.ImageListPanel.Children.Add(control);
                            }

                            //Ocean.20130523. 记录上次打开的Tab，会引起商品属性的加载，暂时屏蔽此功能
                            //int cookieTabIndexVal = 0;
                            //if (!string.IsNullOrEmpty(cookieTabIndex) && int.TryParse(cookieTabIndex, out cookieTabIndexVal))
                            //{
                            //    ucProductTabList.SelectedIndex = cookieTabIndexVal;
                            //}

                            var areaFacade = new AreaQueryFacade();
                            areaFacade.QueryProvinceAreaList((areaObj, areaArgs) =>
                            {
                                if (areaArgs.FaultsHandle())
                                {
                                    return;
                                }
                                List<ProvinceVM> provinceList = _productFacade.ConvertAreaInfoEntityToProvinceVM(areaArgs.Result);
                                provinceList.Insert(0, new ProvinceVM { ProvinceSysNo = 0, ProvinceName = "请选择..." });
                                ucProductMaintainSalesArea.cmbProvinceList.ItemsSource = provinceList;
                                ucProductMaintainSalesArea.cmbProvinceList.SelectedValue = 0;
                                List<CityVM> cityList = new List<CityVM>();
                                cityList.Insert(0, new CityVM { CitySysNo = 0, CityName = "请选择..." });
                                ucProductMaintainSalesArea.cmbCityList.ItemsSource = cityList;
                                ucProductMaintainSalesArea.cmbCityList.SelectedValue = 0;
                                SetTabFocus(opertionType);
                            });

                            _facade = new ProductPriceRequestFacade();
                            _facade.GetProductStepPricebyProductSysNo(productSysNo, (priceobj, priceargs) =>
                            {
                                if (priceargs.FaultsHandle())
                                {
                                    return;
                                }
                                VM.ProductMaintainStepPrice.QueryResultList = priceargs.Result;
                                VM.ProductMaintainStepPrice.ProductSysNo = VM.ProductSysNo;
                            });

                        });
                    });
                    if (args.Result.ProductConsignFlag == VendorConsignFlag.GroupBuying || (args.Result.ProductBasicInfo != null && args.Result.ProductBasicInfo.TradeType == TradeType.Internal))
                    {
                        productEntryInfo_tab.Visibility = System.Windows.Visibility.Collapsed;
 
                    }
                });
                //TLYH去除商品备案功能 by Key on 2015/7/23
                //if (AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemProductMaintainEntryInfo))
                //{
                //    productEntryInfo_tab.Visibility = System.Windows.Visibility.Visible;
                //}
            }
            else
            {
                Window.MessageBox.Show("无效商品编号", MessageBoxType.Error);
            }
        }


        private void SetTabFocus(string opertionType)
        {

            if (opertionType == "pricelink")
            {
                ucProductTabList.SelectedItem = ucProductMaintainPriceInfo_Tab;
                foreach (TabItem item in ucProductTabList.Items)
                {
                    if (item != ucProductMaintainPriceInfo_Tab)
                    {
                        item.IsEnabled = false;
                    }
                }
            }

        }
        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            foreach (var tab in ucProductTabList.Items)
            {
                var tabItem = tab as TabItem;
                if (tabItem != null && tabItem.Focus())
                {
                    var controlName = tabItem.Name.Split('_')[0];
                    if (!String.IsNullOrEmpty(controlName))
                    {
                        var control = tabItem.FindName(controlName);
                        if (control != null && control is ISave)
                        {
                            if (ValidationManager.Validate(control as Control))
                            {
                                (control as ISave).Save();
                            }
                        }
                    }
                }
            }
        }

        private void BtnBatchSaveClick(object sender, RoutedEventArgs e)
        {
            foreach (var tab in ucProductTabList.Items)
            {
                var tabItem = tab as TabItem;
                if (tabItem != null && tabItem.Focus())
                {
                    var controlName = tabItem.Name.Split('_')[0];
                    if (!String.IsNullOrEmpty(controlName))
                    {
                        var control = tabItem.FindName(controlName);

                        if (control != null && !(control is IBatchSave))
                        {
                            Window.Alert("该Tab不支持批量保存！", MessageType.Warning);
                            return;
                        }

                        if (control != null)
                        {
                            if (!VM.ProductMaintainCommonInfo.GroupProductList.Any(p => p.IsChecked))
                            {
                                Window.Alert("请选择需要批量更新的商品！", MessageType.Warning);
                                return;
                            }

                            if (ValidationManager.Validate(control as Control))
                            {
                                (control as IBatchSave).BatchSave();
                            }
                        }
                    }
                }
            }
        }

        private void BtnRequestAuditClick(object sender, RoutedEventArgs e)
        {
            if (!ucProductMaintainPriceInfo_Tab.Focus())
            {
                Window.Alert("请切换到价格Tab执行", MessageType.Warning);
                return;
            }

            if (!VM.ProductMaintainPriceInfo.HasItemPriceMaintainPermission)
            {
                Window.Alert("没有编辑该商品价格的权限，无法提交审核", MessageType.Error);
                return;
            }

            if (ValidationManager.Validate(ucProductMaintainPriceInfo))
            {
                _productFacade.AuditProductPriceInfo(VM, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.MessageBox.Show("提交审核成功", MessageBoxType.Success);
                    VM.ProductMaintainPriceInfo.ProductPriceRequestStatus = ProductPriceRequestStatus.Origin;
                });
            }
        }

        private void BtnCancelAuditClick(object sender, RoutedEventArgs e)
        {
            if (!ucProductMaintainPriceInfo_Tab.Focus())
            {
                Window.Alert("请切换到价格Tab执行", MessageType.Warning);
                return;
            }
            _productFacade.CancelAuditProductPriceInfo(VM, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Window.MessageBox.Show("撤销审核价格成功", MessageBoxType.Success);
                ProductPriceRequestCancel();
            });
        }

        private void BtnImageUploadClick(object sender, RoutedEventArgs e)
        {
            Window.Navigate(String.Format(ConstValue.IM_ProductResourcesUrlFormat, VM.ProductSysNo), null, true);
        }

        private void BtnOnSaleClick(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemProductOnSale))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            _productFacade.ProductOnSale(VM.ProductSysNo, ((obj, args) => OnSaleCallBack(args, ProductStatus.Active)));
        }

        private void BtnOnShowClick(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemProductOnShow))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            _productFacade.ProductOnShow(VM.ProductSysNo, ((obj, args) => OnSaleCallBack(args, ProductStatus.InActive_Show)));
        }

        private void BtnNotShowClick(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemProductOnNotShow))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            _productFacade.ProductUnShow(VM.ProductSysNo, ((obj, args) => OnSaleCallBack(args, ProductStatus.InActive_UnShow)));
        }

        private void BtnAbandonClick(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemProductAbandon))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("你无此操作权限");
                return;
            }

            Window.Confirm(ResProductMaintain.ProductMaintain_ProductAbandon, (obj, args) =>
               {
                   if (args.DialogResult == DialogResultType.OK)
                   {
                       _productFacade.ProductInvalid(VM.ProductSysNo, ((tempObj, tempArgs) => OnSaleCallBack(tempArgs, ProductStatus.Abandon)));
                   }
               });
        }

        private void OnSaleCallBack(RestClientEventArgs<int> args, ProductStatus status)
        {
            if (args.FaultsHandle())
            {
                return;
            }
            Window.MessageBox.Show("更新成功，影响记录数" + args.Result + "条", MessageBoxType.Success);
            StatusSync(status);
        }

        private void StatusSync(ProductStatus status)
        {
            VM.ProductMaintainBasicInfo.ProductMaintainBasicInfoStatusInfo.ProductStatus = status;
            VM.ProductMaintainCommonInfo.GroupProductList.First(p => p.ProductSysNo == VM.ProductSysNo).ProductStatus
                = status;
        }

        private void ProductPriceRequestCancel()
        {
            VM.ProductMaintainPriceInfo.ProductPriceRequestStatus = null;
            VM.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.RequestCurrentPrice = null;
            VM.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.RequestCashRebate = null;
            VM.ProductMaintainPriceInfo.ProductMaintainPriceInfoBasicPrice.RequestPoint = null;
            VM.ProductMaintainPriceInfo.ProductMaintainPriceInfoRankPrice.ProductRankPriceList.ForEach(rankPrice => rankPrice.RequestRankPrice = null);
            VM.ProductMaintainPriceInfo.ProductMaintainPriceInfoVolumePrice.ProductVolumePriceList
                .ForEach(delegate(ProductVolumePriceVM volumePriceVM)
                             {
                                 volumePriceVM.VolumePriceRequestQty = null;
                                 volumePriceVM.VolumePriceRequestPrice = null;
                             });
        }

        private void ucProductTabList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                //do work when tab is changed
                TabControl tabMain = sender as TabControl;

                DateTime expireDate = DateTime.Now + TimeSpan.FromDays(30);
                string newCookie = "CookieTabIndex=" + tabMain.SelectedIndex + ";expires=" + expireDate.ToString("R");
                HtmlPage.Document.SetProperty("cookie", newCookie);
            }

            if (this.productEntryInfo_tab != null && productEntryInfo_tab.Focus())
            {
                productEntryInfo_tab.Loaded += new RoutedEventHandler(productEntryInfo_tab_Loaded);
            }

            if (this.productEntryInfo_tab != null && !productEntryInfo_tab.Focus())
            {
                this.btnAudit.Visibility = System.Windows.Visibility.Collapsed;
                this.btnToInspection.Visibility = System.Windows.Visibility.Collapsed;
                this.btnInspection.Visibility = System.Windows.Visibility.Collapsed;
                this.btnToCustoms.Visibility = System.Windows.Visibility.Collapsed;
                this.btnCustoms.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void productEntryInfo_tab_Loaded(object sender, RoutedEventArgs e)
        {
            productEntryInfo_tab.Loaded -= new RoutedEventHandler(productEntryInfo_tab_Loaded);
            productEntryInfo.ProductMaintainBasicEntryInfo_Loaded(showProductEntryButton);
        }

        private void showProductEntryButton()
        {
            //待审核 显示审核按钮
            if (productEntryInfo.vm.EntryStatus == ProductEntryStatus.WaitingAudit)
            {
                this.btnAudit.Visibility = System.Windows.Visibility.Visible;
            }
            //审核成功 显示提交商检按钮
            if (productEntryInfo.vm.EntryStatus == ProductEntryStatus.AuditSucess)
            {
                this.btnToInspection.Visibility = System.Windows.Visibility.Visible;
            }
            //提交商检之后，备案审核状态变为备案中，扩展状态变为待商检
            if (productEntryInfo.vm.EntryStatus == ProductEntryStatus.Entry || productEntryInfo.vm.EntryStatus == ProductEntryStatus.EntryFail)
            {
                productEntryInfo.EntryStatusExLabel.Visibility = System.Windows.Visibility.Visible;
                productEntryInfo.EntryStatusExTxt.Visibility = System.Windows.Visibility.Visible;
                //扩展状态为带商检 显示商检按钮
                if (productEntryInfo.vm.EntryStatusEx == ProductEntryStatusEx.Inspection)
                {
                    this.btnInspection.Visibility = System.Windows.Visibility.Visible;
                }
                //扩展状态为商检成功 显示提交报关按钮
                if (productEntryInfo.vm.EntryStatusEx == ProductEntryStatusEx.InspectionSucess)
                {
                    this.btnToCustoms.Visibility = System.Windows.Visibility.Visible;
                }
                //扩展状态为待报关，显示报关按钮
                if (productEntryInfo.vm.EntryStatusEx == ProductEntryStatusEx.Customs)
                {
                    this.btnCustoms.Visibility = System.Windows.Visibility.Visible;
                }

            }
        }

        /// <summary>
        /// 提交商检
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToInspection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            productEntryInfo.facades.ToInspection(productEntryInfo.vm.ProductSysNo.Value, (args) =>
            {
                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交商检成功！");
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交商检失败！");
                }
            });
        }

        /// <summary>
        /// 提交报关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToCustoms_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            productEntryInfo.facades.ToCustoms(productEntryInfo.vm.ProductSysNo.Value, (args) =>
            {
                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交报关成功！");
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提交报关失败！");
                }
            });
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Audit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UCEntryStatusOperation addNews = new UCEntryStatusOperation(EntryStatusOperation.Audit, productEntryInfo.vm.ProductSysNo.Value);
            addNews.dialog = Window.ShowDialog("备案审核", addNews, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                }
            });
        }

        /// <summary>
        /// 商检
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Inspection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UCEntryStatusOperation addNews = new UCEntryStatusOperation(EntryStatusOperation.Inspection, productEntryInfo.vm.ProductSysNo.Value);
            addNews.dialog = Window.ShowDialog("备案商检", addNews, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                }
            });
        }

        /// <summary>
        /// 报关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Customs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UCEntryStatusOperation addNews = new UCEntryStatusOperation(EntryStatusOperation.Customs, productEntryInfo.vm.ProductSysNo.Value);
            addNews.dialog = Window.ShowDialog("备案报关", addNews, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                }
            });
        }

        /// <summary>
        /// Get Cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetCookie(string key)
        {
            string[] cookies = HtmlPage.Document.Cookies.Split(';');
            foreach (string cookie in cookies)
            {
                string[] keyValue = cookie.Split('=');
                if (keyValue.Length == 2)
                {
                    if (keyValue[0].Trim().ToString() == key)
                    {
                        return keyValue[1];
                    }
                }
            } return null;
        }
    }
}
