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
using ECCentral.BizEntity.Enum.Resources;
using System.Windows.Data;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.CurrencyTypePicker
{
    public partial class UCCurrencyTypePicker : UserControl
    {

        public int? SelectedCurrencyType
        {
            get
            {
                return (int?)GetValue(SelectedCurrencyTypeProperty);
            }
            set
            {
                SetValue(SelectedCurrencyTypeProperty, value);
            }
        }

        public event EventHandler<CurrencySelectedEventArgs> CurrencySelectChanged;

        public static readonly DependencyProperty SelectedCurrencyTypeProperty =
            DependencyProperty.Register("SelectedCurrencyType", typeof(int?), typeof(UCCurrencyTypePicker), new PropertyMetadata(null));

        public UCCurrencyTypePicker()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCCurrencyTypePicker_Loaded);
        }

        private void UCCurrencyTypePicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCCurrencyTypePicker_Loaded);

            var exp = this.GetBindingExpression(UCCurrencyTypePicker.SelectedCurrencyTypeProperty);

            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbCurrencyType.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            LoadCurrencyTypeList();
        }

        private void LoadCurrencyTypeList()
        {
            new CurrencyTypeFacade(CPApplication.Current.CurrentPage).GetCurrencyList((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                args.Result.ForEach(x =>
                {
                    x.CurrencySymbolAndName = x.CurrencySymbol + x.CurrencyName;

                });
                args.Result.Insert(0, new BizEntity.Common.CurrencyInfo
                {
                    CurrencySymbolAndName = ResCommonEnum.Enum_All
                });
                this.cmbCurrencyType.ItemsSource = args.Result;
            });
        }

        private void cmbCurrencyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Combox cmb = sender as Combox;
            if (null != cmb)
            {
                OnCurrencySelected((CurrencyInfo)cmb.SelectedItem);
            }
        }

        private void OnCurrencySelected(CurrencyInfo currency)
        {
            var handler = CurrencySelectChanged;
            if (handler != null)
            {
                CurrencySelectedEventArgs argsCurrency = new CurrencySelectedEventArgs(currency);
                handler(this, argsCurrency);
            }
        }

    }

    public class CurrencySelectedEventArgs : EventArgs
    {
        public CurrencySelectedEventArgs(CurrencyInfo selectedCurrency)
        {
            this.SelectedCurrencyInfo = selectedCurrency;
        }

        public CurrencyInfo SelectedCurrencyInfo
        {
            get;
            private set;
        }
    }
}
