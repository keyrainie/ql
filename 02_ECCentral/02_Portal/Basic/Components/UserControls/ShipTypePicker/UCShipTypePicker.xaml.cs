using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.Basic.Components.UserControls.ShipTypePicker
{
    public enum AppendItemType
    {
        Select,
        All
    }

    public partial class UCShipTypePicker : UserControl
    {
        public AppendItemType AppendItemType
        {
            get;
            set;
        }

        public int? SelectedShipType
        {
            get
            {
                return (int?)GetValue(SelectedShipTypeProperty);
            }
            set
            {
                SetValue(SelectedShipTypeProperty, value);
            }
        }

       public static readonly DependencyProperty SelectedShipTypeProperty =
            DependencyProperty.Register("SelectedShipType", typeof(int?), typeof(UCShipTypePicker), new PropertyMetadata(null));

        public UCShipTypePicker()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCShipTypePicker_Loaded);
        }

        private void UCShipTypePicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCShipTypePicker_Loaded);

            var exp = this.GetBindingExpression(UCShipTypePicker.SelectedShipTypeProperty);

            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbShipType.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            LoadShipTypeList();
        }

        private void LoadShipTypeList()
        {
            new ShipTypeFacade(CPApplication.Current.CurrentPage).GetShipTypeList((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (this.AppendItemType == AppendItemType.Select)
                {
                    args.Result.Insert(0, new BizEntity.Common.ShippingType
                    {
                        ShippingTypeName = ResCommonEnum.Enum_Select
                    });
                }
                else
                {
                    args.Result.Insert(0, new BizEntity.Common.ShippingType
                    {
                        ShippingTypeName = ResCommonEnum.Enum_All
                    });
                }
                cmbShipType.ItemsSource = args.Result;
            });
        }

        public event SelectionChangedEventHandler SelectionChanged;

        private void cmbShipType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(sender, e);
            }
        }
    }
}