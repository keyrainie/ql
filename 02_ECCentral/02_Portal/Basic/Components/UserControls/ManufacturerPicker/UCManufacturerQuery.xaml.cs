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
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Portal.Basic.Components.UserControls.ManufacturerPicker
{
    public partial class UCManufacturerQuery : UserControl
    {
        public IDialog Dialog { get; set; }
        public VendorManufacturerFacade serviceFacade;
        public VendorManufacturerQueryVM queryVM;
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

        public UCManufacturerQuery()
        {
            InitializeComponent();

            BindComboBoxData();

            serviceFacade = new VendorManufacturerFacade(CurrentPage);
            queryVM = new VendorManufacturerQueryVM();
            queryFilter = new ManufacturerQueryFilter();
            this.DataContext = queryVM;
        }

        private void BindComboBoxData()
        {
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ManufacturerStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.PagingInfo.SortBy = e.SortField;

            serviceFacade.QueryManufacturers(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var vendorList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = vendorList;
            });
        }

        private void btnAddManufacturer_Click(object sender, RoutedEventArgs e)
        {
            if (this.QueryResultGrid.SelectedIndex < 0 || this.QueryResultGrid.ItemsSource == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("错误", "请先选择一个代理厂商!", MessageType.Error, (obj, args) => { });
                return;
            }
            DynamicXml selectedManufacturer = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != selectedManufacturer)
            {
                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                this.Dialog.ResultArgs.Data = selectedManufacturer;
                this.Dialog.Close(true);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.QueryResultGrid.SelectedIndex = -1;

            queryFilter.ManufacturerNameLocal = this.txtManufacturerName.Text.Trim();
            queryFilter.ManufacturerNameGlobal = this.txtManufacturerName.Text.Trim();

            KeyValuePair<ManufacturerStatus?, string> getSelectedItem = (KeyValuePair<ManufacturerStatus?, string>)this.cmbStatus.SelectedItem;
            queryFilter.Status = getSelectedItem.Key;
            QueryResultGrid.Bind();
        }
    }
}
