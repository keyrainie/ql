using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.VendorPicker
{
    public partial class UCVendorPicker : UserControl
    {
        #region [Properties]

        public static readonly DependencyProperty SelectedVendorSysNoProperty =
    DependencyProperty.Register("SelectedVendorSysNo", typeof(string), typeof(UCVendorPicker), new PropertyMetadata(null, (s, e) =>
    {
        var uc = s as UCVendorPicker;
        if (!string.IsNullOrEmpty(uc.SelectedVendorSysNo) && uc.serviceFacade != null)
        {
            uc.LoadVendorInfo(int.Parse(uc.SelectedVendorSysNo));
        }
    }));

        public static readonly DependencyProperty SelectedVendorNameProperty =
  DependencyProperty.Register("SelectedVendorName", typeof(string), typeof(UCVendorPicker), new PropertyMetadata(string.Empty, (s, e) =>
   {
       if (e.NewValue != null)
       {
           var uc = s as UCVendorPicker;
           uc.txtVendorName.Text = e.NewValue.ToString();
       }
   }));

        public static readonly DependencyProperty IsAllowVendorSelectPropertey =
              DependencyProperty.Register("IsAllowVendorSelect", typeof(bool), typeof(UCVendorPicker), new PropertyMetadata(true, (s, e) =>
   {
       var uc = s as UCVendorPicker;
       uc.txtVendorSysNo.IsReadOnly = !(bool)e.NewValue;
       uc.btnChooseVendor.IsEnabled = (bool)e.NewValue;
   }));
       
        public string SelectedVendorSysNo
        {
            get
            {
                return ((string)base.GetValue(SelectedVendorSysNoProperty));
            }
            set
            {
                base.SetValue(SelectedVendorSysNoProperty, value);
            }
        }

        private VendorConsignFlag? isConsign;
        public VendorConsignFlag? IsConsign
        {
            get
            {
                return isConsign;
            }
            set
            {
                isConsign = value;
            }
        }

        /// <summary>
        /// 控件是否为可编辑状态:
        /// </summary>
        public bool IsAllowVendorSelect
        {
            get
            {
                return ((bool)base.GetValue(IsAllowVendorSelectPropertey));
            }
            set
            {
                base.SetValue(IsAllowVendorSelectPropertey, value);
            }
        }

        public string SelectedVendorName
        {
            get
            {
                return ((string)base.GetValue(SelectedVendorNameProperty));
            }
            set
            {
                base.SetValue(SelectedVendorNameProperty, value);
            }
        }

        #endregion [Properties]

        public event EventHandler<VendorSelectedEventArgs> VendorSelected;

        public Action InitVendorAction;

        private VendorFacade serviceFacade;
        public VendorInfo VendorInfo;

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

        public UCVendorPicker()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCVendorPicker_Loaded);
        }

        private void UCVendorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCVendorPicker_Loaded;
            var exp = this.GetBindingExpression(UCVendorPicker.SelectedVendorNameProperty);
            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                this.txtVendorName.SetBinding(WaterMarkTextBox.TextProperty, binding);
            }

            var exp2 = this.GetBindingExpression(UCVendorPicker.SelectedVendorSysNoProperty);
            if (exp2 != null && exp2.ParentBinding != null)
            {
                string path = exp2.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                binding.Converter = new ECCentral.Portal.Basic.Converters.NullableValueTypeConverter();
                this.txtVendorSysNo.SetBinding(WaterMarkTextBox.TextProperty, binding);
            }

            serviceFacade = new VendorFacade(CurrentPage);
            VendorInfo = new VendorInfo();

            if (!string.IsNullOrEmpty(SelectedVendorSysNo))
            {
                int vendorSysNo = 0;
                int.TryParse(SelectedVendorSysNo, out vendorSysNo);
                if (vendorSysNo > 0)
                {
                    LoadVendorInfo(vendorSysNo);
                }
            }
        }

        private void btnChooseVendor_Click(object sender, RoutedEventArgs e)
        {
            UCVendorQuery selectDialog = new UCVendorQuery();
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("供应商查询", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    DynamicXml getSelectedVendor = args.Data as DynamicXml;
                    if (null != getSelectedVendor)
                    {
                        this.SelectedVendorSysNo = getSelectedVendor["SysNo"].ToString();
                        this.txtVendorSysNo.Text = this.SelectedVendorSysNo;
                        this.SelectedVendorName = getSelectedVendor["VendorName"].ToString();
                        serviceFacade.GetVendorBySysNo(SelectedVendorSysNo, (obj2, args2) =>
                          {
                              if (args2.FaultsHandle())
                              {
                                  InitializeTextBox();
                                  return;
                              }
                              OnVendorSelected(args2.Result);
                          });
                    }
                }
            }, new Size(700, 580));
        }

        public void txtVendorSysNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int getInputVendorSysNo = 0;
                if (string.IsNullOrWhiteSpace(this.txtVendorSysNo.Text.Trim()))
                {
                    InitializeTextBox();
                    return;
                }
                if (!int.TryParse(this.txtVendorSysNo.Text.Trim(), out getInputVendorSysNo))
                {
                    InitializeTextBox();
                    CurrentWindow.Alert("无效的供应商编号!");
                    return;
                }
                LoadVendorInfo(getInputVendorSysNo);
            }
        }

        private void OnVendorSelected(VendorInfo vendorInfo)
        {
            if (VendorSelected != null)
            {
                VendorSelectedEventArgs argsVendor = new VendorSelectedEventArgs(vendorInfo);
                VendorSelected(this, argsVendor);
            }
        }

        public void InitializeTextBox()
        {
            this.txtVendorSysNo.Text = string.Empty;
            this.txtVendorName.Text = string.Empty;
            this.SelectedVendorName = string.Empty;
            this.SelectedVendorSysNo = string.Empty;
            if (InitVendorAction != null)
            {
                InitVendorAction();
            }
        }

        private void txtVendorSysNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtVendorSysNo.Text))
            {
                InitializeTextBox();
            }
        }

        public void LoadVendorInfo(int VendorSysNo)
        {
            if (VendorSysNo > 0)
            {
                if (CurrentPage != null)
                {
                    serviceFacade = new VendorFacade(CurrentPage);
                }
                serviceFacade.GetVendorBySysNo(VendorSysNo.ToString(), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        InitializeTextBox();
                        return;
                    }
                    VendorInfo = args.Result;
                    if (null != VendorInfo)
                    {
                        this.txtVendorSysNo.Text = VendorInfo.SysNo.Value.ToString();
                        this.txtVendorName.Text = VendorInfo.VendorBasicInfo.VendorNameLocal.ToString();
                        this.SelectedVendorName = this.txtVendorName.Text;
                        this.SelectedVendorSysNo = this.txtVendorSysNo.Text;
                        this.IsConsign = VendorInfo.VendorBasicInfo.ConsignFlag;
                        OnVendorSelected(VendorInfo);
                    }
                    else
                    {
                        InitializeTextBox();
                        CurrentWindow.Alert("无效的供应商编号!");
                    }
                });
            }
            else
            {
                InitializeTextBox();
            }
        }
    }

    public class VendorSelectedEventArgs : EventArgs
    {
        public VendorSelectedEventArgs(VendorInfo selectedCustomer)
        {
            this.SelectedVendorInfo = selectedCustomer;
        }

        public VendorInfo SelectedVendorInfo
        {
            get
            ;
            private set;
        }
    }
}
