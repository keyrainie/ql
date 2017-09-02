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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.UserControls.ManufacturerPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.Controls;
using System.Windows.Data;

namespace ECCentral.Portal.Basic.Components.UserControls.ManufacturerPicker
{
    public partial class UCManufacturerAndBrandPicker : UserControl
    {

        public VendorManufacturerFacade serviceFacade;

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        #region [Properties]



        public bool IsAllowManufacturerAndBrandSelected
        {
            get { return (bool)GetValue(IsAllowManufacturerAndBrandSelectedProperty); }
            set { SetValue(IsAllowManufacturerAndBrandSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowManufacturerAndBrandSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowManufacturerAndBrandSelectedProperty =
            DependencyProperty.Register("IsAllowManufacturerAndBrandSelected", typeof(bool), typeof(UCManufacturerAndBrandPicker), new PropertyMetadata(true, (s, e) =>
            {
                var uc = s as UCManufacturerAndBrandPicker;
                if ((bool)e.NewValue == false)
                {
                    uc.txtManufacturerSysNo.IsReadOnly = true;
                    uc.txtBrandSysNo.IsReadOnly = true;
                    uc.btnChooseBrand.IsEnabled = false;
                    uc.btnChooseManufacturer.IsEnabled = false;
                }
            }));



        private static readonly DependencyProperty SelectedManufacturerSysNoProperty =
    DependencyProperty.Register("SelectedManufacturerSysNo", typeof(string), typeof(UCManufacturerAndBrandPicker), null);

        private static readonly DependencyProperty SelectedManufacturerNameProperty =
  DependencyProperty.Register("SelectedManufacturerName", typeof(string), typeof(UCManufacturerAndBrandPicker), null);

        private static readonly DependencyProperty SelectedBrandSysNoProperty =
 DependencyProperty.Register("SelectedBrandSysNo", typeof(string), typeof(UCManufacturerAndBrandPicker), null);

        private static readonly DependencyProperty SelectedBrandNameProperty =
  DependencyProperty.Register("SelectedBrandName", typeof(string), typeof(UCManufacturerAndBrandPicker), null);


        public string SelectedManufacturerSysNo
        {
            get
            {
                return ((string)base.GetValue(SelectedManufacturerSysNoProperty));
            }
            set
            {
                base.SetValue(SelectedManufacturerSysNoProperty, value);
            }
        }

        public string SelectedManufacturerName
        {
            get
            {
                return ((string)base.GetValue(SelectedManufacturerNameProperty));
            }
            set
            {
                base.SetValue(SelectedManufacturerNameProperty, value);
            }
        }

        public string SelectedBrandSysNo
        {
            get
            {
                return ((string)base.GetValue(SelectedBrandSysNoProperty));
            }
            set
            {
                base.SetValue(SelectedBrandSysNoProperty, value);
            }
        }

        public string SelectedBrandName
        {
            get
            {
                return ((string)base.GetValue(SelectedBrandNameProperty));
            }
            set
            {
                base.SetValue(SelectedBrandNameProperty, value);
            }
        }
        #endregion

        public UCManufacturerAndBrandPicker()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCManufacturerAndBrandPicker_Loaded);


        }

        void UCManufacturerAndBrandPicker_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCManufacturerAndBrandPicker_Loaded;

            #region [Binding]

            var expBrandSysNo = this.GetBindingExpression(UCManufacturerAndBrandPicker.SelectedBrandSysNoProperty);
            if (expBrandSysNo != null && expBrandSysNo.ParentBinding != null)
            {
                string path = expBrandSysNo.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                this.txtBrandSysNo.SetBinding(TextBox.TextProperty, binding);
            }


            var expBrandName = this.GetBindingExpression(UCManufacturerAndBrandPicker.SelectedBrandNameProperty);
            if (expBrandName != null && expBrandName.ParentBinding != null)
            {
                string path = expBrandName.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                this.txtBrandName.SetBinding(TextBox.TextProperty, binding);
            }



            var manufacturerSysNo = this.GetBindingExpression(UCManufacturerAndBrandPicker.SelectedManufacturerSysNoProperty);
            if (manufacturerSysNo != null && manufacturerSysNo.ParentBinding != null)
            {
                string path = manufacturerSysNo.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                this.txtManufacturerSysNo.SetBinding(TextBox.TextProperty, binding);
            }


            var manufacturerName = this.GetBindingExpression(UCManufacturerAndBrandPicker.SelectedManufacturerNameProperty);
            if (manufacturerName != null && manufacturerName.ParentBinding != null)
            {
                string path = manufacturerName.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                this.txtManufacturerName.SetBinding(TextBox.TextProperty, binding);
            }
            #endregion

            serviceFacade = new VendorManufacturerFacade(CurrentPage);
        }

        private void btnChooseManufacturer_Click(object sender, RoutedEventArgs e)
        {
            UCManufacturerQuery selectDialog = new UCManufacturerQuery();
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("查询代理厂商", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    DynamicXml getSelectedManufacturer = args.Data as DynamicXml;
                    if (null != getSelectedManufacturer)
                    {
                        this.SelectedManufacturerSysNo = getSelectedManufacturer["SysNo"].ToString();
                        this.SelectedManufacturerName = getSelectedManufacturer["ManufacturerNameLocal"].ToString();

                        BrandQueryFilter requestBrand = new BrandQueryFilter()
                        {
                            ManufacturerSysNo = int.Parse(SelectedManufacturerSysNo),
                            ManufacturerName = SelectedManufacturerName,
                            PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = 1 }
                        };
                        serviceFacade.QueryBrands(requestBrand, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            var brandList = args2.Result.Rows;
                            if (null != brandList)
                            {
                                try
                                {
                                    SelectedBrandSysNo = null != brandList[0]["SysNo"] ? brandList[0]["SysNo"].ToString() : string.Empty;
                                    SelectedBrandName = brandList[0]["BrandName_Ch"];
                                }
                                catch
                                {
                                    SelectedBrandSysNo = null;
                                    SelectedBrandName = null;
                                }
                            }
                        });
                    }
                }

            }, new Size(750, 500));
        }

        private void btnChooseBrand_Click(object sender, RoutedEventArgs e)
        {
            UCBrandQuery selectDialog = new UCBrandQuery(SelectedManufacturerSysNo, SelectedManufacturerName);
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("查询代理品牌", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    DynamicXml getSelectedBrand = args.Data as DynamicXml;
                    if (null != getSelectedBrand)
                    {
                        this.SelectedManufacturerSysNo = getSelectedBrand["ManufacturerSysNo"].ToString();
                        this.SelectedManufacturerName = getSelectedBrand["ManufacturerName"].ToString();
                        this.SelectedBrandSysNo = getSelectedBrand["SysNo"].ToString();
                        this.SelectedBrandName = getSelectedBrand["BrandName_Ch"].ToString();
                    }
                }

            }, new Size(750, 500));
        }

        private void txtBrandSysNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int brandSysNo = 0;
                if (!int.TryParse(this.txtBrandSysNo.Text.Trim(), out brandSysNo))
                {
                    CurrentWindow.Alert("无效的品牌编号!");
                    this.SelectedBrandSysNo = null;
                    this.SelectedBrandName = null;
                    this.txtBrandSysNo.Text = string.Empty;
                    return;
                }

                serviceFacade.QueryBrandBySysNo(brandSysNo.ToString(), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    if (args.Result == null || args.Result.Manufacturer == null)
                    {
                        CurrentWindow.Alert("无效的品牌编号!");
                        this.SelectedManufacturerSysNo = string.Empty;
                        this.SelectedManufacturerName = string.Empty;
                        this.SelectedBrandSysNo = string.Empty;
                        this.SelectedBrandName = string.Empty;
                        return;
                    }
                    this.SelectedManufacturerSysNo = args.Result.Manufacturer.SysNo.Value.ToString();
                    this.SelectedManufacturerName = args.Result.Manufacturer.ManufacturerNameLocal.Content;
                    this.SelectedBrandSysNo = args.Result.SysNo.Value.ToString();
                    this.SelectedBrandName = args.Result.BrandNameLocal.Content;
                });
            }
        }

        private void txtManufacturerSysNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int manufacturerSysNo = 0;
                if (!int.TryParse(this.txtManufacturerSysNo.Text.Trim(), out manufacturerSysNo))
                {
                    CurrentWindow.Alert("无效的厂商编号!");
                    this.SelectedManufacturerName = null;
                    this.SelectedManufacturerSysNo = null;
                    this.txtManufacturerSysNo.Text = string.Empty;
                    return;
                }

                serviceFacade.QueryManufacturerBySysNo(manufacturerSysNo.ToString(), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result.ManufacturerNameLocal == null)
                    {
                        CurrentWindow.Alert("无效的厂商编号!");
                        this.SelectedManufacturerName = null;
                        this.SelectedManufacturerSysNo = null;
                        this.txtManufacturerSysNo.Text = string.Empty;
                        return;
                    }
                    this.SelectedManufacturerSysNo = args.Result.SysNo.Value.ToString();
                    this.SelectedManufacturerName = args.Result.ManufacturerNameLocal.Content;

                    BrandQueryFilter requestBrand = new BrandQueryFilter()
                    {
                        ManufacturerSysNo = int.Parse(SelectedManufacturerSysNo),
                        ManufacturerName = SelectedManufacturerName,
                        PagingInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = 1 }
                    };
                    serviceFacade.QueryBrands(requestBrand, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        var brandList = args2.Result.Rows;
                        if (null != brandList)
                        {
                            try
                            {
                                SelectedBrandSysNo = null != brandList[0]["SysNo"] ? brandList[0]["SysNo"].ToString() : string.Empty;
                                SelectedBrandName = brandList[0]["BrandName_Ch"];
                            }
                            catch
                            {
                                SelectedBrandName = null;
                                SelectedBrandSysNo = null;
                            }
                        }
                    });
                });
            }
        }

        private void txtManufacturerSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtManufacturerSysNo.Text))
            {
                this.SelectedManufacturerSysNo = string.Empty;
                this.SelectedManufacturerName = string.Empty;
                this.txtManufacturerName.Text = string.Empty;
            }
        }

        private void txtBrandSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtBrandSysNo.Text))
            {
                this.SelectedBrandSysNo = string.Empty;
                this.SelectedBrandName = string.Empty;
                this.txtBrandName.Text = string.Empty;
            }
        }
    }
}
