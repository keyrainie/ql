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
using ECCentral.Portal.UI.PO.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class SettledProductsRuleQuery : PageBase
    {

        public ConsignSettlementRulesQueryVM queryVM;
        public ConsignSettlementRulesFacade serviceFacade;
        public PurchaseOrderFacade poFacade;
        SettleRuleQueryFilter queryFilter;

        public SettledProductsRuleQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryVM = new ConsignSettlementRulesQueryVM();
            serviceFacade = new ConsignSettlementRulesFacade(this);
            poFacade = new PurchaseOrderFacade(this);
            queryFilter = new SettleRuleQueryFilter();
            
            InitializeComboBoxData();
            this.DataContext = queryVM;

            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                //如果有传入的getSettleName参数:
                string getSettleName = this.Request.Param;
                queryVM.SettleRuleName = getSettleName;
                btnSearch_Click(null, null);
            }
        }

        private void InitializeComboBoxData()
        {
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ConsignSettleRuleStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }

        #region [Events]
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            queryFilter = EntityConverter<ConsignSettlementRulesQueryVM, SettleRuleQueryFilter>.Convert(queryVM);
            this.QueryResultGrid.Bind();
        }
        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
            };

            serviceFacade.QueryConsignSettleRulesList(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var rulesList = args.Result.Rows;

                foreach (DynamicXml item in rulesList)
                {
                    char getStatusChar = Convert.ToChar(item["StatusString"].ToString());
                    switch (getStatusChar)
                    {
                        case (char)ConsignSettleRuleStatus.Wait_Audit:
                            item["Status"] = ConsignSettleRuleStatus.Wait_Audit;
                            break;
                        case (char)ConsignSettleRuleStatus.Stop:
                            item["Status"] = ConsignSettleRuleStatus.Stop;
                            break;
                        case (char)ConsignSettleRuleStatus.Forbid:
                            item["Status"] = ConsignSettleRuleStatus.Forbid;
                            break;
                        case (char)ConsignSettleRuleStatus.Enable:
                            item["Status"] = ConsignSettleRuleStatus.Enable;
                            break;
                        case (char)ConsignSettleRuleStatus.Disable:
                            item["Status"] = ConsignSettleRuleStatus.Disable;
                            break;
                        case (char)ConsignSettleRuleStatus.Available:
                            item["Status"] = ConsignSettleRuleStatus.Available;
                            break;
                        default:
                            break;
                    }
                    if ((ConsignSettleRuleStatus)item["Status"] == ConsignSettleRuleStatus.Enable || (ConsignSettleRuleStatus)item["Status"] == ConsignSettleRuleStatus.Disable)
                    {
                        item["RemainQty"] = (item["SettleRulesQuantity"] == null ? 0 : Convert.ToInt32(item["SettleRulesQuantity"].ToString())) - (item["SettledQuantity"].ToString() == null ? 0 : Convert.ToInt32(item["SettledQuantity"].ToString()));
                    }
                }

                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = rulesList;
            });
        }

        private void btnNewSettleRule_Click(object sender, RoutedEventArgs e)
        {
            //添加操作:
            SettledProductsRuleNew newCtrl = new SettledProductsRuleNew();
            newCtrl.Dialog = Window.ShowDialog(ResSettledProductsRuleQuery.Msg_NewRuleTitle, newCtrl, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    QueryResultGrid.PageIndex = 0;
                    QueryResultGrid.SelectedIndex = -1;
                    btnSearch_Click(null, null);
                }
            }, new Size(650, 220));
        }

        private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        {
            //查看:

            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                ConsignSettlementRulesInfoVM viewVM = DynamicConverter<ConsignSettlementRulesInfoVM>.ConvertToVM(getSelectedItem);
                SettledProductsRuleMaintain maintainCtrl = new SettledProductsRuleMaintain(viewVM);
                maintainCtrl.Dialog = Window.ShowDialog(ResSettledProductsRuleQuery.Msg_RuleEdit, maintainCtrl, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        this.QueryResultGrid.Bind();
                    }
                }, new Size(650, 220));
            }
        }

        #endregion

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            ////新增供应商:
            //if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_ExportExcel))
            //{
            //    Window.Alert("对不起，你没有权限进行此操作！");
            //    return;
            //}
            //导出全部:
            if (null != queryFilter)
            {
                SettleRuleQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ECCentral.QueryFilter.PO.SettleRuleQueryFilter>(queryFilter);
                exportQueryRequest.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };

                foreach (DataGridColumn col in QueryResultGrid.Columns)
                    if (col.Visibility == Visibility.Collapsed)
                        if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn)
                            (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn).NeedExport = false;
                        else if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn)
                            (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn).NeedExport = false;
                ColumnSet columnSet = new ColumnSet(QueryResultGrid);

                serviceFacade.ExportExcelForVendors(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

        private void Hyperlink_Audit_Click(object sender, RoutedEventArgs e)
        {
            //审核操作:
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                ConsignSettlementRulesInfoVM editInfo = DynamicConverter<ConsignSettlementRulesInfoVM>.ConvertToVM(getSelectedItem, "");
                if (null != editInfo)
                {
                    Window.Confirm(ResSettledProductsRuleQuery.AlertMsg_ConfirmAudit, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(editInfo);
                            serviceFacade.AuditConsignSettleRule(info, (obj2, args2) =>
                            {
                                if (args2.FaultsHandle())
                                {
                                    return;
                                }
                                Window.Alert(ResSettledProductsRuleQuery.AlertMsg_AlertTitle, ResSettledProductsRuleQuery.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                                {
                                    if (args3.DialogResult == DialogResultType.Cancel)
                                    {
                                        QueryResultGrid.Bind();
                                    }
                                });
                            });
                        }
                    });
                }
            }
        }

        private void Hyperlink_Stop_Click(object sender, RoutedEventArgs e)
        {
            //终止操作:
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                ConsignSettlementRulesInfoVM editInfo = DynamicConverter<ConsignSettlementRulesInfoVM>.ConvertToVM(getSelectedItem);
                if (null != editInfo)
                {
                    Window.Confirm(ResSettledProductsRuleQuery.AlertMsg_ConfirmStop, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(editInfo);
                            serviceFacade.StopConsignSettleRule(info, (obj2, args2) =>
                            {
                                if (args2.FaultsHandle())
                                {
                                    return;
                                }
                                Window.Alert(ResSettledProductsRuleQuery.AlertMsg_AlertTitle, ResSettledProductsRuleQuery.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                                {
                                    if (args3.DialogResult == DialogResultType.Cancel)
                                    {
                                        QueryResultGrid.Bind();
                                    }
                                });
                            });
                        }
                    });
                }
            }
        }

        private void Hyperlink_Abandon_Click(object sender, RoutedEventArgs e)
        {
            //作废操作:
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                ConsignSettlementRulesInfoVM editInfo = DynamicConverter<ConsignSettlementRulesInfoVM>.ConvertToVM(getSelectedItem);
                if (null != editInfo)
                {
                    Window.Confirm(ResSettledProductsRuleQuery.AlertMsg_ConfirmAbandon, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(editInfo);
                            serviceFacade.AbandonConsignSettleRule(info, (obj2, args2) =>
                            {
                                if (args2.FaultsHandle())
                                {
                                    return;
                                }
                                Window.Alert(ResSettledProductsRuleQuery.AlertMsg_AlertTitle, ResSettledProductsRuleQuery.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                                {
                                    if (args3.DialogResult == DialogResultType.Cancel)
                                    {
                                        QueryResultGrid.Bind();
                                    }
                                });
                            });
                        }
                    });
                }
            }
        }

    }

}
