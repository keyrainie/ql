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
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Models.PurchaseOrder;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true)]
    public partial class ConsignAdjustQuery : PageBase
    {
        private ConsignAdjustQueryVM queryVM;
        private ConsignAdjustFacade serviceFacade;
        private ConsignAdjustQueryFilter queryFilter;
        public ConsignAdjustQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryVM = new ConsignAdjustQueryVM();
            queryFilter = new ConsignAdjustQueryFilter();
            serviceFacade = new ConsignAdjustFacade(this);
            InitControlData();

            this.DataContext = queryVM;
            base.OnPageLoad(sender, e);
        }

        //初始化状态
        public void InitControlData()
        {
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ConsignAdjustStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }

        //查询
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //构造查询条件
            this.queryFilter = EntityConverter<ConsignAdjustQueryVM, ConsignAdjustQueryFilter>.Convert(queryVM);
            QueryResultGrid.Bind();
        }


        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.Query(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var list = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;

                foreach (var item in list)
                {
                    //只有待审核的可以编辑,删除,，其他状态仅可以查看
                    bool edtiVisibility = false;
                    bool viewVisibility = false;
                    bool delVisibility = false;
                    bool auditVisibility = false;
                    if (item.Status == ConsignAdjustStatus.WaitAudit)
                    {
                        edtiVisibility = true;
                        delVisibility = true;
                        auditVisibility = true;
                    }
                    else
                    {
                        viewVisibility = true;
                    }
                    item.AuditButtonVisibility = auditVisibility;
                    item.DelButtonVisibility = delVisibility;
                    item.ViewButtonVisibility = viewVisibility;
                    item.EditButtonVisibility = edtiVisibility;
                }
                QueryResultGrid.ItemsSource = list;
            });
        }

        //编辑
        private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;

            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/ConsignAdjustMaintain/{0}", getSelectedItem["SysNo"]), null, true);
            }
        }

        //查看
        private void Hyperlink_View_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;

            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/ConsignAdjustMaintain/{0}", getSelectedItem["SysNo"]), null, true);
            }
        }

        //删除
        private void Hyperlink_Del_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            serviceFacade.Delete(Convert.ToInt32(getSelectedItem["SysNo"]), (_obj, _args) =>
            {
                if (_args.FaultsHandle())
                {
                    return;
                }

                QueryResultGrid.Bind();
            });
        }

        //审核
        private void Hyperlink_Audit_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;

            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/ConsignAdjustMaintain/{0}", getSelectedItem["SysNo"]), null, true);
            }
        }
    }
}
