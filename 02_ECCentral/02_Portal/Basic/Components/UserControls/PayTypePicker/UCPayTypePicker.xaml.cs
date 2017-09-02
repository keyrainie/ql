using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ECCentral.BizEntity.Enum.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.Basic.Components.UserControls.PayTypePicker
{
    public enum AppendItemType
    {
        Select,
        All
    }

    public partial class UCPayTypePicker : UserControl
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public AppendItemType AppendItemType
        {
            get;
            set;
        }

        public int? SelectedPayType
        {
            get
            {
                return (int?)GetValue(SelectedPayTypeProperty);
            }
            set
            {
                SetValue(SelectedPayTypeProperty, value);
            }
        }

        public ECCentral.BizEntity.Common.PayType SelectedPayTypeItem
        {
            get { return (BizEntity.Common.PayType)cmbPayType.SelectedItem; }
        }

        public static readonly DependencyProperty SelectedPayTypeProperty =
            DependencyProperty.Register("SelectedPayType", typeof(int?), typeof(UCPayTypePicker), new PropertyMetadata(null));

        public UCPayTypePicker()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCPayTypePicker_Loaded);
        }

        private void UCPayTypePicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCPayTypePicker_Loaded);

            var exp = this.GetBindingExpression(UCPayTypePicker.SelectedPayTypeProperty);

            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbPayType.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            LoadPayTypeList();
        }

        private void LoadPayTypeList()
        {
            new PayTypeFacade(CPApplication.Current.CurrentPage).GetPayTypeList((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (this.AppendItemType == AppendItemType.Select)
                {
                    args.Result.Insert(0, new BizEntity.Common.PayType
                    {
                        PayTypeName = ResCommonEnum.Enum_Select
                    });
                }
                else
                {
                    args.Result.Insert(0, new BizEntity.Common.PayType
                    {
                        PayTypeName = ResCommonEnum.Enum_All
                    });
                }
                cmbPayType.ItemsSource = args.Result;
            });
        }

        private void cmbPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, e);
            }
        }
    }
}