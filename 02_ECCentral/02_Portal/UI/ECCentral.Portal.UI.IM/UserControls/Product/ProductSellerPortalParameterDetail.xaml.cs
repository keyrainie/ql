using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades.Product;
using System.Windows.Media;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductSellerPortalParameterDetail : UserControl
    {
        public IDialog Dialog { get; set; }

        public int? SysNo { get; set; }

        public string ProductID { get; set; }

        #region 属性
        private SellerProductRequestFacade _facade;
        private int _sysNo;
        public string _productID { get; set; }
        #endregion

        public ProductSellerPortalParameterDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BindPage();
        }

        #region 查询绑定
        private void BindPage()
        {
           
            if (!string.IsNullOrEmpty(ProductID) && SysNo != null)
            {
                //绑定原值
                _facade = new SellerProductRequestFacade();

                _facade.GetSellerProductRequestByProductID(ProductID, (objOld, argsOld) =>
                {
                    if (argsOld.FaultsHandle())
                    {
                        return;
                    }
                    if (argsOld.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得商家产品信息.", MessageBoxType.Warning);
                        return;
                    }
                    var vm = argsOld.Result.Convert<SellerProductRequestInfo, SellerProductRequestVM>();

                    _productID = ProductID;

                    this.OldParameterDetail.DataContext = vm;

                    if (vm.SellerProductRequestPropertyList.Count > 0)
                    {
                        BindProperty(vm, this.OldParameterDetail);
                    }


                    //绑定修改的值
                    _facade.GetSellerProductRequestBySysNo(SysNo.Value, (objNew, argsNew) =>
                    {
                        if (argsNew.FaultsHandle())
                        {
                            return;
                        }
                        if (argsNew.Result == null)
                        {
                            CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有获得商家产品信息.", MessageBoxType.Warning);
                            return;
                        }

                        var vmNew = argsNew.Result.Convert<SellerProductRequestInfo, SellerProductRequestVM>();

                        _sysNo = SysNo.Value;

                        this.expander1.DataContext = vmNew;

                        this.NewParameterDetail.DataContext = vmNew;
                        this.NewParameterDetail.SetControlBackground(Colors.Yellow);

                        ProductFacade _productFacade = new ProductFacade();

                        if (vmNew.SellerProductRequestPropertyList.Count > 0)
                        {
                            BindProperty(vmNew, this.NewParameterDetail);
                        }
                        else
                        {
                            this.NewParameterDetail.PropertySection.IsEnabled = false;
                            this.OldParameterDetail.PropertySection.IsEnabled = false;
                        }

                        foreach (object item in this.NewParameterDetail.controlDetail.Children)
                        {
                            if (item.GetType() == typeof(TextBox))
                            {
                                if (((TextBox)item).Text.Equals(((TextBox)this.OldParameterDetail.controlDetail.FindName(((TextBox)item).Name)).Text))
                                {
                                    ((TextBox)item).Visibility = System.Windows.Visibility.Collapsed;
                                    ((TextBox)this.OldParameterDetail.controlDetail.FindName(((TextBox)item).Name)).Visibility = System.Windows.Visibility.Collapsed;
                                    ((TextBlock)this.NewParameterDetail.controlDetail.FindName(((TextBox)item).Name + "Block")).Visibility = System.Windows.Visibility.Collapsed;
                                    ((TextBlock)this.OldParameterDetail.controlDetail.FindName(((TextBox)item).Name + "Block")).Visibility = System.Windows.Visibility.Collapsed;
                                }
                            }
                            else if (item.GetType() == typeof(Combox))
                            {
                                if (((Combox)item).SelectedValue.Equals((((Combox)this.OldParameterDetail.controlDetail.FindName(((Combox)item).Name)).SelectedValue)))
                                {
                                    ((Combox)item).Visibility = System.Windows.Visibility.Collapsed;
                                    ((Combox)this.OldParameterDetail.controlDetail.FindName(((Combox)item).Name)).Visibility = System.Windows.Visibility.Collapsed;
                                    ((TextBlock)this.NewParameterDetail.controlDetail.FindName(((Combox)item).Name + "Block")).Visibility = System.Windows.Visibility.Collapsed;
                                    ((TextBlock)this.OldParameterDetail.controlDetail.FindName(((Combox)item).Name + "Block")).Visibility = System.Windows.Visibility.Collapsed;
                                }
                            }
                        }
                    });

                });
   

            }
            else
            {
                _sysNo = 0;
                var item = new SellerProductRequestVM();
                DataContext = item;
            }

        }

        private void BindProperty(SellerProductRequestVM vm,ProductSellerPortalParameterControl sellControl)
        {
            ProductFacade _productFacade = new ProductFacade();

            _productFacade.GetPropertyValueList(vm.SellerProductRequestPropertyList.Select(p => p.PropertySysno).ToList(), (ob, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                foreach (var propertyVM in vm.SellerProductRequestPropertyList)
                {
                    propertyVM.PropertyValueList = new List<PropertyValueVM>();
                    foreach (var i in arg.Result.Keys)
                    {
                        if (propertyVM.PropertySysno == i)
                        {
                            propertyVM.PropertyValueList = _productFacade.ConvertPropertyValueInfoToPropertyValueVM(arg.Result[i]);
                        }
                    }
                    propertyVM.PropertyValueList.Insert(0, new PropertyValueVM
                    {
                        SysNo = 0,
                        ValueDescription = "请选择..."
                    });
                }

                sellControl.dgPropertyQueryResult.ItemsSource = vm.SellerProductRequestPropertyList;
                sellControl.dgPropertyQueryResult.TotalCount = vm.SellerProductRequestPropertyList.Count;

            });
        }


        #endregion

        private void btnDeny_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.NewParameterDetail.DataContext as SellerProductRequestVM;
            if (vm == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(vm.Memo))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("必须输入退回理由.", MessageBoxType.Warning);
                return;
            }

            _facade = new SellerProductRequestFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                return;
            }
            else
            {
                _facade.DenyProductRequest(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                    CloseDialog(DialogResultType.OK);
                });
            }
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.NewParameterDetail.DataContext as SellerProductRequestVM;
            if (vm == null)
            {
                return;
            }

            _facade = new SellerProductRequestFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                return;
            }
            else
            {
                _facade.ApproveProductRequest(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                    BindPage();
                });
            }
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.NewParameterDetail.DataContext as SellerProductRequestVM;
            if (vm == null)
            {
                return;
            }

            if (!ValidationManager.Validate(this))
            {
                return;
            }

            _facade = new SellerProductRequestFacade();
            vm.SysNo = _sysNo;
            if (vm.SysNo == null || vm.SysNo.Value <= 0)
            {
                return;
            }
            else
            {
                _facade.UpdateProductRequest(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResBrandMaintain.Info_SaveSuccessfully);

                    CloseDialog(DialogResultType.OK);
                });
            }
        }
    }
}
