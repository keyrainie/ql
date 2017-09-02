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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true)]
    public partial class VendorQuery : PageBase
    {
        public VendorQueryVM vendorQueryVM;
        public VendorFacade vendorFacade;
        public ECCentral.QueryFilter.PO.VendorQueryFilter queryRequest;

        public VendorQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            vendorQueryVM = new VendorQueryVM();

            vendorFacade = new VendorFacade(this);
            queryRequest = new ECCentral.QueryFilter.PO.VendorQueryFilter();
            BindComboBoxData();
            this.DataContext = vendorQueryVM;
            SetAccessControl();
            //this.cmbEPort.SelectedEPort = 0;
        }

        private void SetAccessControl()
        {
            //权限控制:
            //新增供应商:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_EditAndCreateVendor))
            {
                this.btnNewVendor.IsEnabled = false;
            }
            //搜索可用状态的供应商
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_SearchValidVendor))
            {
                (this.cmbStatus.ItemsSource as List<KeyValuePair<VendorStatus?, string>>).RemoveAt(0);
                (this.cmbStatus.ItemsSource as List<KeyValuePair<VendorStatus?, string>>).RemoveAt(1);
                vendorQueryVM.Status = VendorStatus.UnAvailable;
            }
        }

        #region [Methods]

        private void BindComboBoxData()
        {

            //财务审核状态:
            this.cmbRequestStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VendorFinanceRequestStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbRequestStatus.SelectedIndex = 0;

            //供应商属性:
            var vendorConsignFlagList = EnumConverter.GetKeyValuePairs<VendorConsignFlag>(EnumConverter.EnumAppendItemType.All);
            //vendorConsignFlagList.RemoveAll(item => item.Key == VendorConsignFlag.Consign);//隐藏代销
            this.cmbConsignFlag.ItemsSource = vendorConsignFlagList;
            this.cmbConsignFlag.SelectedIndex = 0;
        
            //状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VendorStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;

            //供应商等级
            this.cmbRank.ItemsSource = EnumConverter.GetKeyValuePairs<VendorRank>(EnumConverter.EnumAppendItemType.All);
            this.cmbRank.SelectedIndex = 0;

            //等级状态 ：
            this.cmbRankStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VendorRankStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbRankStatus.SelectedIndex = 0;

            //代理级别:
            CodeNamePairHelper.GetList("PO", "VendorAgentLevel", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                this.cmbAgentLevel.ItemsSource = args.Result;
                this.cmbAgentLevel.SelectedIndex = 0;
            });
            //开票方式:
            this.cmbInvoiceType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorInvoiceType>(EnumConverter.EnumAppendItemType.All);
            this.cmbInvoiceType.SelectedIndex = 0;

            //仓储方式:
            this.cmbStockType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorStockType>(EnumConverter.EnumAppendItemType.All);
            this.cmbStockType.SelectedIndex = 0;

            //配送方式:
            this.cmbShippingType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorShippingType>(EnumConverter.EnumAppendItemType.All);
            this.cmbShippingType.SelectedIndex = 0;



        }
        #endregion

        #region [Events]

        private void btnNewVendor_Click(object sender, RoutedEventArgs e)
        {
            VendorNew VendorNewPage = new VendorNew();
            Window.Navigate("/ECCentral.Portal.UI.PO/VendorNew", null, true);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.StackPanelCondition))
            {
                this.queryRequest = EntityConverter<VendorQueryVM, ECCentral.QueryFilter.PO.VendorQueryFilter>.Convert(vendorQueryVM);
                QueryResultGrid.Bind();
            }
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryRequest.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            if (!string.IsNullOrWhiteSpace(queryRequest.C1SysNo))
            {
                if (string.IsNullOrWhiteSpace(queryRequest.C2SysNo))
                {
                    Window.Alert("代理类别至少选到二级类别！");
                    return;
                }
            }
            queryRequest.PageInfo.SortBy = e.SortField;
            vendorFacade.QueryVendors(queryRequest, (obj, args) =>
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

        /// <summary>
        /// 编辑，加载供应商信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_EditVendor_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/VendorMaintain/{0}", getSelectedItem["SysNo"]), null, true);
            }
        }
        #endregion

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            //新增商家:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_ExportExcel))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }
            //导出全部:
            if (null != queryRequest)
            {
                VendorQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ECCentral.QueryFilter.PO.VendorQueryFilter>(queryRequest);
                exportQueryRequest.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };

                foreach (DataGridColumn col in QueryResultGrid.Columns)
                    if (col.Visibility == Visibility.Collapsed)
                        if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn)
                            (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn).NeedExport = false;
                        else if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn)
                            (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn).NeedExport = false;
                ColumnSet columnSet = new ColumnSet(QueryResultGrid);

                //ColumnSet columnSet = new ColumnSet()
                //.Add("SysNo", ResVendorQuery.GridHeader_VendorSysNo, 20)
                //.Add("VendorName", ResVendorQuery.GridHeader_VendorName, 40)
                //.Add("IsConsign", ResVendorQuery.GridHeader_Consign, 20)
                //.Add("RANK", ResVendorQuery.GridHeader_VendorLevel, 20)
                //.Add("Status", ResVendorQuery.GridHeader_Status, 20)
                //.Add("PayTermsName", ResVendorQuery.GridHeader_PayPeriodType, 40)
                //.Add("Bank", ResVendorQuery.GridHeader_Bank, 20)
                //.Add("Account", ResVendorQuery.GridHeader_Account, 20)
                //.Add("VendorContractInfo", ResVendorQuery.GridHeader_ContractInfo, 20)
                //.Add("CreateVendorUserName", ResVendorQuery.GridHeader_CreateUser, 20)
                //.Add("CreateVendorTime", ResVendorQuery.GridHeader_CreateTime, 20)
                //.Add("AuditStatus", ResVendorQuery.GridHeader_AuditStatus, 20);
                vendorFacade.ExportExcelForVendors(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }


    }

}
