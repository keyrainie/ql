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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.ManufacturerPicker
{
    public partial class UCBrandQuery : UserControl
    {
        public IDialog Dialog { get; set; }
        public VendorManufacturerFacade serviceFacade;
        public BrandQueryFilter queryFilter;

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

        public UCBrandQuery(string manufacturerSysNo, string manufacuturerName)
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCBrandQuery_Loaded);

            serviceFacade = new VendorManufacturerFacade(CurrentPage);
            queryFilter = new BrandQueryFilter();

            if (null == manufacturerSysNo || string.IsNullOrEmpty(manufacturerSysNo))
            {
                queryFilter.ManufacturerSysNo = null;
            }
            else
            {
                queryFilter.ManufacturerSysNo = int.Parse(manufacturerSysNo);
            }
            queryFilter.ManufacturerName = (string.IsNullOrEmpty(manufacuturerName) ? string.Empty : manufacuturerName);
            this.txtManufacturerName.Text = queryFilter.ManufacturerName;
        }

        void UCBrandQuery_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.All);
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
            serviceFacade.QueryBrands(queryFilter, (obj, args) =>
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.QueryResultGrid.SelectedIndex = -1;
            this.queryFilter.BrandNameGlobal = this.txtBrandName.Text.Trim();
            this.queryFilter.BrandNameLocal = this.txtBrandName.Text.Trim();

            KeyValuePair<ValidStatus?, string> getSelectedItem = (KeyValuePair<ValidStatus?, string>)this.cmbStatus.SelectedItem;

            this.queryFilter.Status = getSelectedItem.Key;
            this.queryFilter.ManufacturerName = this.txtManufacturerName.Text.Trim();
            this.queryFilter.ManufacturerSysNo = null;

            QueryResultGrid.Bind();
        }

        private void btnChooseBrand_Click(object sender, RoutedEventArgs e)
        {
            if (this.QueryResultGrid.SelectedIndex < 0 || this.QueryResultGrid.ItemsSource == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("错误", "请先选择一个代理品牌!", MessageType.Error, (obj, args) => { });
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

    }
}
