using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Components.UserControls.EPortPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
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

namespace ECCentral.Portal.Basic.Components.UserControls.EPortPicker
{
    public enum AppendItemType
    {
        Select,
        All
    }
    public partial class UCEPortPicker : UserControl
    {
        public AppendItemType AppendItemType
        {
            get;
            set;
        }

        public int? SelectedEPort
        {
            get
            {
                return (int?)GetValue(SelectedEPortProperty);
            }
            set
            {
                SetValue(SelectedEPortProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedEPortProperty =
     DependencyProperty.Register("SelectedEPort", typeof(int?), typeof(UCEPortPicker), new PropertyMetadata(null));

        public UCEPortPicker()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCEPortPicker_Loaded);
        }

        private void UCEPortPicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCEPortPicker_Loaded);

            var exp = this.GetBindingExpression(UCEPortPicker.SelectedEPortProperty);

            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbEPort.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            LoadShipTypeList();
        }

        private void LoadShipTypeList()
        {
            new EPortFacade(CPApplication.Current.CurrentPage).GetEPortList((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (this.AppendItemType == AppendItemType.Select)
                {
                    args.Result.Insert(0, new EPortEntity
                    {
                        ePortName = ResCommonEnum.Enum_Select
                    });
                }
                else
                {
                    args.Result.Insert(0, new EPortEntity
                    {
                        ePortName = ResCommonEnum.Enum_All
                    });
                }
                cmbEPort.ItemsSource = args.Result;
            });
        }


        public event SelectionChangedEventHandler SelectionChanged;
        private void cmbEPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, e);
            }
        }
    }
}
