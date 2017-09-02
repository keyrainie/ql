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
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class VendorRMARefundQuery : PageBase
    {
        public VendorRMARefundQueryFilter queryFilter;
        public VendorRMARefundQueryVM queryVM;
        public VendorRefundFacade serviceFacade;

        public VendorRMARefundQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryFilter = new VendorRMARefundQueryFilter();
            queryVM = new VendorRMARefundQueryVM();
            serviceFacade = new VendorRefundFacade(this);
            LoadComboBoxData();
            this.DataContext = queryVM;
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //查询列表
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VendorRefund_Query))
            {
                this.btnSearch.IsEnabled = false;
            }
        }

        private void LoadComboBoxData()
        {
            //供应商退款状态
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VendorRefundStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
            //付款方式:
            this.cmbPayType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorRefundPayType>(EnumConverter.EnumAppendItemType.All);
            this.cmbPayType.SelectedIndex = 0;
        }
        #region [Events]

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            queryFilter = EntityConverter<VendorRMARefundQueryVM, VendorRMARefundQueryFilter>.Convert(queryVM);
            this.QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryVendorRMARefundList(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var refundList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = refundList;
            });
        }
        #endregion

        private void Hyperlink_VendorRefundEdit_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                string getSysNo = getSelectedItem["SysNo"].ToString();
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/VendorRMARefundMaintain/{0}", getSelectedItem["SysNo"]), null, true);
            }
        }

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            //权限控制:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VendorRefund_Export))
            {
                Window.Alert("对不起，你没有权限进行此操作!");
                return;
            }
            //导出全部:
            if (null != queryFilter)
            {
                VendorRMARefundQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<VendorRMARefundQueryFilter>(queryFilter);
                exportQueryRequest.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };
                
                foreach (DataGridColumn col in QueryResultGrid.Columns)
                    if (col.Visibility == Visibility.Collapsed)
                        if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn)
                            (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn).NeedExport = false;
                        else if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn)
                            (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn).NeedExport = false;
                ColumnSet columnSet = new ColumnSet(QueryResultGrid);
                columnSet.Insert(1, "Status", ResVendorRMARefundQuery.GridHeader_Status);

                serviceFacade.ExportExcelForVendorRMARefundList(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }
    }


}
