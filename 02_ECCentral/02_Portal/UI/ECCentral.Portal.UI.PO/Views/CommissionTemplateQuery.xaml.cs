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
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.PO.Models.Vendor;
using ECCentral.Portal.UI.PO.UserControls;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class CommissionTemplateQuery : PageBase
    {
        public CommissionRuleTemplateQueryFilter queryRequest;
        public VendorFacade serviceFacade;
        public CommissionRuleTemplateQueryVM queryVM;


        public CommissionTemplateQuery()
        {
            InitializeComponent();
        }



        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryRequest = new CommissionRuleTemplateQueryFilter();
            serviceFacade = new VendorFacade(this);
            queryVM = new CommissionRuleTemplateQueryVM();
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<CommissionRuleStatus>(EnumConverter.EnumAppendItemType.All);
            this.DataContext = queryVM;
            base.OnPageLoad(sender, e);
        }


        #region [Events]

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryRequest = EntityConverter<CommissionRuleTemplateQueryVM, CommissionRuleTemplateQueryFilter>.Convert(queryVM);

            queryRequest.PageInfo.PageSize = QueryResultGrid.PageSize;
            queryRequest.PageInfo.PageIndex = QueryResultGrid.PageIndex;
            queryRequest.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryCommissionRuleTemplateInfo(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                foreach (var item in args.Result.Rows)
                {
                    item.IsChecked = false;
                }
                var commissionList = args.Result.Rows.ToList();
                foreach (var item in commissionList)
                {
                    if (!string.IsNullOrEmpty(item.SalesRule))
                    {
                        VendorStagedSaleRuleEntity StagedRuls = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.XmlDeserialize<VendorStagedSaleRuleEntity>(item.SalesRule);
                        item.SalesRule = string.Empty;
                        if (StagedRuls.MinCommissionAmt != null)
                        {
                            item.SalesRule = string.Format("保底金额：{0};", StagedRuls.MinCommissionAmt.Value.ToString("f2"));
                        }
                        for (int i = 0; i < StagedRuls.StagedSaleRuleItems.Count; i++)
                        {
                            string startAmt = StagedRuls.StagedSaleRuleItems[i].StartAmt.GetValueOrDefault().ToString("f2");
                            string endAmt = StagedRuls.StagedSaleRuleItems[i].EndAmt.GetValueOrDefault().ToString("f2");
                            string percentage = (StagedRuls.StagedSaleRuleItems[i].Percentage).GetValueOrDefault().ToString("f2") + "%";
                            if (StagedRuls.StagedSaleRuleItems[i].StartAmt == 0.0m && StagedRuls.StagedSaleRuleItems[i].EndAmt == 0.0m)
                            {
                                item.SalesRule += string.Format("按销售总额的 {0} 收取佣金;", percentage);
                                break;
                            }
                            else if (StagedRuls.StagedSaleRuleItems[i].StartAmt == 0.0m)
                                item.SalesRule += string.Format("不超过 {0}元的部分，按销售总额的 {1} 收取佣金;", endAmt, percentage);
                            else if (StagedRuls.StagedSaleRuleItems[i].EndAmt == 0.0m)
                                item.SalesRule += string.Format("超过 {0}元的部分，按销售总额的 {1} 收取佣金;", startAmt, percentage);
                            else
                                item.SalesRule += string.Format("超过 {0}元 至 {1}元的部分，按销售总额的 {2} 收取佣金;", startAmt, endAmt, percentage);
                        }
                    }
                }
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = commissionList;
            });
        }
        #endregion


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            UCCommissionTemplateMaintain agentInfo = new UCCommissionTemplateMaintain();
            agentInfo.Dialog = Window.ShowDialog("添加提成规则模板", agentInfo, (obj, args) =>
            {
                if (DialogResultType.OK == args.DialogResult)
                {
                    QueryResultGrid.Bind();
                }
            }, new Size(800, 700));
        }

        private void btnBatchActive_Click(object sender, RoutedEventArgs e)
        {
            if (this.QueryResultGrid.ItemsSource != null)
            {
                List<int> selected = new List<int>();
                foreach (dynamic item in this.QueryResultGrid.ItemsSource)
                {
                    if (item.IsChecked)
                    {
                        selected.Add(item.SysNo);
                    }
                }
                if (selected.Count == 0)
                {
                    MessageBox.Show("请选择一个规则");
                    return;
                }
                serviceFacade.BatchActiveCommissionRuleTemplate(selected, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    MessageBox.Show("更新成功！");
                    QueryResultGrid.Bind();
                });
            }
        }

        private void btnBatchDEActive_Click(object sender, RoutedEventArgs e)
        {
            if (this.QueryResultGrid.ItemsSource != null)
            {
                List<int> selected = new List<int>();
                foreach (dynamic item in this.QueryResultGrid.ItemsSource)
                {
                    if (item.IsChecked)
                    {
                        selected.Add(item.SysNo);
                    }
                }
                if (selected.Count == 0)
                {
                    MessageBox.Show("请选择一个规则");
                    return;
                }
                serviceFacade.BatchDEActiveCommissionRuleTemplate(selected, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    MessageBox.Show("更新成功！");
                    QueryResultGrid.Bind();
                });
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
                if (viewList != null)
                {
                    foreach (var view in viewList)
                    {
                        view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                    }
                }
            }

        }

        private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        {
            UCCommissionTemplateMaintain agentInfo = new UCCommissionTemplateMaintain();
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            agentInfo.SysNo = (int)getSelectedItem["SysNo"];
            agentInfo.Dialog = Window.ShowDialog("编辑提成规则模板", agentInfo, (obj, args) =>
            {
                if (DialogResultType.OK == args.DialogResult)
                {
                    QueryResultGrid.Bind();
                }
            }, new Size(800, 420));
        }


    }

}
