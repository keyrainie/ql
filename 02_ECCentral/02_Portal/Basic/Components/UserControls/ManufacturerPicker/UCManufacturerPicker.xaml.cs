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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.IM;
using System.Windows.Data;

namespace ECCentral.Portal.Basic.Components.UserControls.ManufacturerPicker
{
    public partial class UCManufacturerPicker : UserControl
    {

        #region [Properties]

        private static readonly DependencyProperty SelectedManufacturerSysNoProperty =
    DependencyProperty.Register("SelectedManufacturerSysNo", typeof(string), typeof(UCManufacturerPicker), new PropertyMetadata(null, (s, e) =>
    {
        var uc = s as UCManufacturerPicker;
        if (uc.serviceFacade != null && e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
        {
            BindManufacturerInfo(uc);
        }
        else
        {
            uc.SelectedManufacturerName = null;
        }
    }));


        private static readonly DependencyProperty SelectedManufacturerNameProperty =
  DependencyProperty.Register("SelectedManufacturerName", typeof(string), typeof(UCManufacturerPicker), null);



        public bool IsAllowManufacturerSelect
        {
            get { return (bool)GetValue(IsAllowManufacturerSelectedProperty); }
            set { SetValue(IsAllowManufacturerSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllowManufacturerSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowManufacturerSelectedProperty =
            DependencyProperty.Register("IsAllowManufacturerSelected", typeof(bool), typeof(UCManufacturerPicker), new PropertyMetadata(true, (s, e) =>
            {
                var uc = s as UCManufacturerPicker;
                uc.txtManufacturerSysNo.IsReadOnly = !(bool)e.NewValue;
                uc.btnChooseManufacturer.IsEnabled = (bool)e.NewValue;
            }));



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
        #endregion

        public VendorManufacturerFacade serviceFacade;
        public ManufacturerQueryFilter queryFilter;

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

        public UCManufacturerPicker()
        {
            InitializeComponent();
            queryFilter = new ManufacturerQueryFilter();
            this.Loaded += new RoutedEventHandler(UCManufacturerPicker_Loaded);
        }

        /// <summary>
        /// 根据SysNo绑定ManufacturerName
        /// </summary>
        private static void BindManufacturerInfo(UCManufacturerPicker uc)
        {
            int manufacturerSysNo = 0;
            if (!int.TryParse(uc.SelectedManufacturerSysNo, out manufacturerSysNo))
            {
                uc.CurrentWindow.Alert("无效的代理厂商编号!");
                uc.SelectedManufacturerSysNo = null;
                uc.txtManufacturerSysNo.Text = string.Empty;
                return;
            }

            uc.serviceFacade.QueryManufacturerBySysNo(manufacturerSysNo.ToString(), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (null != args.Result && args.Result.SysNo.HasValue && args.Result.ManufacturerNameLocal != null)
                {
                    uc.SelectedManufacturerSysNo = manufacturerSysNo.ToString();
                    uc.SelectedManufacturerName = args.Result.ManufacturerNameLocal.Content;
                }
                else
                {
                    uc.CurrentWindow.Alert("未找到相关的代理厂商!");
                    uc.SelectedManufacturerSysNo = null;
                    uc.txtManufacturerSysNo.Text = string.Empty;
                    return;
                }
            });
        }

        void UCManufacturerPicker_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCManufacturerPicker_Loaded;
            #region Binding

            var expManufacturerSysNo = this.GetBindingExpression(UCManufacturerPicker.SelectedManufacturerSysNoProperty);
            if (expManufacturerSysNo != null && expManufacturerSysNo.ParentBinding != null)
            {
                string path = expManufacturerSysNo.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                this.txtManufacturerSysNo.SetBinding(TextBox.TextProperty, binding);
            }


            var expManufacturerName = this.GetBindingExpression(UCManufacturerPicker.SelectedManufacturerNameProperty);
            if (expManufacturerName != null && expManufacturerName.ParentBinding != null)
            {
                string path = expManufacturerName.ParentBinding.Path.Path;
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
                        this.txtManufacturerSysNo.Text = this.SelectedManufacturerSysNo = getSelectedManufacturer["SysNo"] == null ? null : getSelectedManufacturer["SysNo"].ToString();
                        this.txtManufacturerName.Text = this.SelectedManufacturerName = getSelectedManufacturer["ManufacturerNameLocal"] == null ? null : getSelectedManufacturer["ManufacturerNameLocal"].ToString();
                    }
                }

            }, new Size(700, 500));
        }

        private void txtManufacturerSysNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(this.txtManufacturerSysNo.Text))
                {
                    SelectedManufacturerSysNo = null;
                    this.txtManufacturerSysNo.Text = string.Empty;
                    SelectedManufacturerName = null;
                    this.txtManufacturerName.Text = string.Empty;
                    return;
                }
                int manufacturerSysNo = 0;
                if (!int.TryParse(this.txtManufacturerSysNo.Text.Trim(), out manufacturerSysNo))
                {
                    CurrentWindow.Alert("无效的代理厂商编号!");
                    SelectedManufacturerSysNo = null;
                    this.txtManufacturerSysNo.Text = string.Empty;
                    return;
                }

                serviceFacade.QueryManufacturerBySysNo(manufacturerSysNo.ToString(), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (null != args.Result && args.Result.SysNo.HasValue 
                        && args.Result.ManufacturerNameLocal != null)
                    {
                        SelectedManufacturerSysNo = manufacturerSysNo.ToString();
                        this.txtManufacturerName.Text = SelectedManufacturerName = args.Result.ManufacturerNameLocal.Content;
                    }
                    else
                    {
                        //CurrentWindow.Alert("未找到相关的代理厂商!");
                        SelectedManufacturerSysNo = null;
                        this.txtManufacturerSysNo.Text = string.Empty;
                        return;
                    }
                });
            }
        }

        //文本框失去焦点
        private void txtManufacturerSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtManufacturerSysNo.Text))
            {
                SelectedManufacturerSysNo = null;
                this.txtManufacturerSysNo.Text = string.Empty;
                SelectedManufacturerName = null;
                this.txtManufacturerName.Text = string.Empty;
            }
        }

    }
}
