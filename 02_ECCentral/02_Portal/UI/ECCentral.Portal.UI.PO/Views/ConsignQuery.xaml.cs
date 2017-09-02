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
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
     [View]
    public partial class ConsignQuery : PageBase
    {

        public ConsignQueryVM queryVM;
        public ConsignSettleQueryFilter queryFilter;
        public ConsignSettlementFacade serviceFacade;
        public ConsignQuery()
        {
            InitializeComponent();

        }

        private void LoadComboBoxData()
        {
            //代销结算单状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SettleStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
            //是否自动结算:
            this.cmbIsAutoSettle.ItemsSource = EnumConverter.GetKeyValuePairs<AutoSettleStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsAutoSettle.SelectedIndex = 0;

            this.cmbLeaseList.ItemsSource = EnumConverter.GetKeyValuePairs<VendorIsToLease>(EnumConverter.EnumAppendItemType.All);
            this.cmbLeaseList.SelectedIndex = 0;
            
          

        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryVM = new ConsignQueryVM();
            serviceFacade = new ConsignSettlementFacade(this);
            queryFilter = new ConsignSettleQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
            };

            LoadComboBoxData();
            this.DataContext = queryVM;
            base.OnPageLoad(sender, e);
            SetAccessControl();

            //if (this.Request.Param != null && this.Request.Param != string.Empty)
            //{
            //    queryFilter.VendorSysNo = int.Parse(this.Request.Param);
            //    btnSearch_Click(null, null);
            //}
        }

        private void SetAccessControl()
        {
            //创建
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_New))
            {
               // btnNewConsign.IsEnabled = false;
            }
        }

        #region  [Events]

        public void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            queryFilter = EntityConverter<ConsignQueryVM, ConsignSettleQueryFilter>.Convert(queryVM);

            this.QueryResultGrid.Bind();
        }
        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //ConsignSettlementBizInfo tmpBizInfo = new ConsignSettlementBizInfo();
            //tmpBizInfo.CompanyCode = CPApplication.Current.CompanyCode;
            //tmpBizInfo.CurrentUserName = CPApplication.Current.LoginUser.LoginName;
            ////2012-9-14 Jack 有效的PMSysNo,Portal填充，用于Service拼装查询条件
            //if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query))
            //{
            //    tmpBizInfo.QueryType = ECCentral.BizEntity.Common.PMQueryType.All;
            //}
            //else if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_IntermediatePM_Query))
            //{
            //    tmpBizInfo.QueryType = ECCentral.BizEntity.Common.PMQueryType.TeamAndInvalid;
            //}
            //else if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_JuniorPM_Query))
            //{
            //    tmpBizInfo.QueryType = ECCentral.BizEntity.Common.PMQueryType.SelfAndInvalid;
            //}
            //else
            //{
            //    queryFilter.AccessiblePMSysNo = new List<int> { -999 };
            //    LoadGridData(e);
            //    return;
            //}

            //serviceFacade.GetPMSysNoListByType(tmpBizInfo, (obj, args) =>
            //{
            //    if (args.FaultsHandle())
            //        return;
            //    if (args.Result.Count == 0)
            //        queryFilter.AccessiblePMSysNo = new List<int> { -999 };
            //    else
            //        queryFilter.AccessiblePMSysNo = args.Result;

                LoadGridData(e);
            //});

        }

        private void LoadGridData(Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {

            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
            queryFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryConsignSettlements(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var consignList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = consignList;

                decimal totalDiffAmt = 0m;

                foreach (var x in consignList)
                {
                    totalDiffAmt += x["Balance"] == null ? 0m : Convert.ToDecimal(x["Balance"].ToString());
                }

                this.lblSettleDiffTotalAmt.Text = string.Format("结算差额总计:{0}", totalDiffAmt.ToString("f2"));
            });

        }

        private void Hyperlink_EditConsign_Click(object sender, RoutedEventArgs e)
        {
            //编辑代销结算单:
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/ConsignMaintain/{0}", getSelectedItem["SysNo"]), null, true);
            }
        }
        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (this.QueryResultGrid.SelectedItem == null || this.QueryResultGrid.SelectedIndex < 0)
            {
                Window.Alert(ResConsignQuery.AlertMsg_Audit);
                return;
            }
            //审核代销结算单:
            Window.Confirm(ResConsignQuery.AlertMsg_Alert, ResConsignQuery.AlertMsg_Confirm, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    DynamicXml getObject = this.QueryResultGrid.SelectedItem as DynamicXml;
                    if (null != getObject)
                    {
                        int getSysNo = Convert.ToInt32(getObject["SysNo"].ToString());
                        ConsignSettlementInfo newAuditConsign = new ConsignSettlementInfo() { SysNo = getSysNo };
                        serviceFacade.AuditConsignSettlement(newAuditConsign, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResConsignQuery.AlertMsg_Alert, ResConsignQuery.AlertMsg_Success, MessageType.Information, (obj3, args3) =>
                            {
                                if (args3.DialogResult == DialogResultType.Cancel)
                                {
                                    this.QueryResultGrid.PageIndex = 0;
                                    this.QueryResultGrid.SelectedIndex = -1;
                                    this.QueryResultGrid.Bind();
                                }
                            });
                        });
                    }
                }
            });
        }
        private void btnNewConsign_Click(object sender, RoutedEventArgs e)
        {
            // 创建代销结算单:
            Window.Navigate("/ECCentral.Portal.UI.PO/ConsignNew", null, true);
        }

        #endregion

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            //权限控制:
            //导出全部数据:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_ExportConsignList))
            {
                Window.Alert("对不起，你没有权限进行此操作!");
                return;
            }

            //导出全部:
            if (null == queryFilter || this.QueryResultGrid.ItemsSource == null || this.QueryResultGrid.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据！");
                return;
            }
            ConsignSettleQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ConsignSettleQueryFilter>(queryFilter);
            exportQueryRequest.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };

            foreach (DataGridColumn col in QueryResultGrid.Columns)
                if (col.Visibility == Visibility.Collapsed)
                    if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn)
                        (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn).NeedExport = false;
                    else if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn)
                        (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn).NeedExport = false;
            ColumnSet columnSet = new ColumnSet(QueryResultGrid);
            columnSet.Add("Status", ResConsignQuery.GridHeader_Status);

            serviceFacade.ExportExcelFroConsignSettlementList(exportQueryRequest, new ColumnSet[] { columnSet });
        }

    }

}
