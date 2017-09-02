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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.Basic.Components.UserControls.VendorPicker
{
    public partial class UCVendorQuery : UserControl
    {
        #region Properties
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(UCVendorQuery),
            new PropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(UCVendorQuery.OnSelectionModePropertyChanged)));

        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UCVendorQuery;
            if (control != null)
            {
                control.OnSelectionModePropertyChanged(e);
            }
        }

        void OnSelectionModePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (this.SelectionMode)
            {
                case SelectionMode.Multiple:
                    this.QueryResultGrid.Columns[0].Visibility = Visibility.Visible;
                    break;
                default:
                    this.QueryResultGrid.Columns[0].Visibility = Visibility.Collapsed;
                    break;
            }
        }
        #endregion

        public IDialog Dialog { get; set; }

        private VendorFacade serviceFacade;
        private VendorQueryVM queryVM;
        private VendorQueryFilter queryFilter;
        List<ValidationEntity> validateList;
        private List<DynamicXml> _vmList;

        public UCVendorQuery()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCVendorQuery_Loaded);
        }

        void UCVendorQuery_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UCVendorQuery_Loaded;

            validateList = new List<ValidationEntity>();
            validateList.Add(new ValidationEntity(ValidationEnum.IsInteger, this.txtVendorSysNo.Text, "供应商编号必须为整数"));

            Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn colStatus = this.QueryResultGrid.Columns[2] as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
            colStatus.Binding.ConverterParameter = typeof(ValidStatus);
            BindComboBoxData();
            serviceFacade = new VendorFacade(CPApplication.Current.CurrentPage);
            queryVM = new VendorQueryVM();
            queryFilter = new VendorQueryFilter();
            this.DataContext = queryVM;
        }

        private void BindComboBoxData()
        {
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VendorStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.txtVendorSysNo, validateList))
            {
                return;
            }
            this.QueryResultGrid.SelectedIndex = -1;
            this.queryFilter = EntityConverter<VendorQueryVM, VendorQueryFilter>.Convert(queryVM);
            QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            //查询Vendor，不包含Seller:

            queryFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryVendors(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vendorList = args.Result.Rows.ToList("IsChecked", false);
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = vendorList;
            });
        }

        private void btnNewVendor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectionMode == SelectionMode.Single)
            {
                if (this.QueryResultGrid.SelectedIndex < 0 || this.QueryResultGrid.ItemsSource == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("错误", "请先选择一个供应商!", MessageType.Error, (obj, args) => { });
                    return;
                }
                DynamicXml selectedVendor = this.QueryResultGrid.SelectedItem as DynamicXml;
                if (null != selectedVendor)
                {
                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    this.Dialog.ResultArgs.Data = selectedVendor;
                    this.Dialog.Close(true);
                }
            }
            else
            {
                List<DynamicXml> list = new List<DynamicXml>();
                dynamic rows = this.QueryResultGrid.ItemsSource;
                if (rows != null)
                {
                    foreach (dynamic row in rows)
                    {
                        if (row.IsChecked)
                        {
                            list.Add(row);
                        }
                    }
                }

                if (list.Count <= 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("错误", "请先选择供应商!", MessageType.Error, (obj, args) => { });
                    return;
                }

                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                this.Dialog.ResultArgs.Data = list;
                this.Dialog.Close(true);

            }
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = this.QueryResultGrid.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    row.IsChecked = chk.IsChecked.Value;
                }
            }
        }
    }
}
