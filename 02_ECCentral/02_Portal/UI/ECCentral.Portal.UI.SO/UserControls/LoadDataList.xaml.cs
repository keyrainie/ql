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
using System.Windows.Data;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public enum AppendItemType
    {
        Select,
        All
    }

    public partial class LoadDataList : UserControl
    {
        public LoadDataList()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCLoadDataList_Loaded);
        }

        public static readonly DependencyProperty SelectedValueProperty =
           DependencyProperty.Register("SelectedValue", typeof(int?), typeof(LoadDataList), new PropertyMetadata(null));

        private void UCLoadDataList_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCLoadDataList_Loaded);

            var exp = this.GetBindingExpression(LoadDataList.SelectedValueProperty);

            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbDatalist.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            LoadList();
        }



        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public AppendItemType AppendItemType
        {
            get;
            set;
        }

        public int? SelectedValue
        {
            get
            {
                return (int?)GetValue(SelectedValueProperty);
            }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }

        public string DataType { get; set; }

        private void LoadList()
        {
            new OtherDomainQueryFacade(CPApplication.Current.CurrentPage).GetFreightMen(true, (obj) =>
            {
                cmbDatalist.ItemsSource = obj;
                cmbDatalist.SelectedIndex = 0;
            });
        }

        private void cmbDatalist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(sender, e);
            }
        }
    }
}
