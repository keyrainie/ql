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
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.PO;
using System;
using System.Windows.Data;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.Basic.Components.UserControls.SellerPicker
{
    public partial class UCSellerPicker : UserControl
    {
        #region [Properties]


        public string SelectedSellerName
        {
            get { return (string)GetValue(SelectedSellerNameProperty); }
            set { SetValue(SelectedSellerNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedSellerNameProperty =
            DependencyProperty.Register("SelectedSellerName", typeof(string), typeof(UCSellerPicker), null);


        private static readonly DependencyProperty SelectedSellerSysNoProperty =
    DependencyProperty.Register("SelectedSellerSysNo", typeof(int?), typeof(UCSellerPicker), new PropertyMetadata(null, (s, e) =>
    {
        var uc = s as UCSellerPicker;
        uc.cmbSeller.SelectedValue = e.NewValue;
    }));


        public int? SelectedSellerSysNo
        {
            get
            {
                return ((int?)base.GetValue(SelectedSellerSysNoProperty));
            }
            set
            {
                base.SetValue(SelectedSellerSysNoProperty, value);
                GetSelectedSellerName(value);
            }
        }

        private void GetSelectedSellerName(int? value)
        {
            if (value != null)
            {
                var source = (VendorInfo)cmbSeller.SelectedItem;
                if (source != null)
                {
                    SelectedSellerName = source.VendorBasicInfo.VendorNameLocal;
                }
            }
            else
            {
                SelectedSellerName = "";
            }
        }
        #endregion

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

        public UCSellerPicker()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCSellerPicker_Loaded);
        }

        void UCSellerPicker_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCSellerPicker_Loaded;
            serviceFacade = new VendorFacade(CurrentPage);
            VendorInfo = new VendorInfo();
            var exp = this.GetBindingExpression(UCSellerPicker.SelectedSellerSysNoProperty);
            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;
                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbSeller.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            VendorQueryFilter filter = new VendorQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
                    {
                        PageIndex = 0,
                        PageSize = int.MaxValue,
                        SortBy = "V.SysNo asc"
                    },
                VendorType = 1,
                Status = VendorStatus.Available
            };
            serviceFacade.QueryVendors(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<VendorInfo> vendorInfo = new List<VendorInfo>();
                if (null != args.Result.Rows)
                {
                    foreach (var item in args.Result.Rows)
                    {
                        VendorInfo info = new VendorInfo()
                        {
                            SysNo = Convert.ToInt32(item["SysNo"].ToString()),

                        };
                        info.VendorBasicInfo = new VendorBasicInfo()
                        {
                            VendorNameLocal = item["VendorName"].ToString()
                        };
                        vendorInfo.Add(info);
                    }
                }
                vendorInfo.Insert(0, new VendorInfo() { SysNo = 1, VendorBasicInfo = new VendorBasicInfo() { VendorNameLocal = "泰隆优选" } });
                vendorInfo.Insert(0, new VendorInfo() { VendorBasicInfo = new VendorBasicInfo() { VendorNameLocal = ResCommonEnum.Enum_All } });
                this.cmbSeller.ItemsSource = vendorInfo;
            });
        }

        private void cmbSeller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var contrl = (Combox)sender;
            if (sender != null)
            {
                if (contrl.SelectedValue != null)
                {
                    var value = Convert.ToInt32(contrl.SelectedValue.ToString());
                    GetSelectedSellerName(value);
                }
                else
                {
                    SelectedSellerName = "";
                }
            }
            else
            {
                SelectedSellerName = "";
            }
        }

    }

}
