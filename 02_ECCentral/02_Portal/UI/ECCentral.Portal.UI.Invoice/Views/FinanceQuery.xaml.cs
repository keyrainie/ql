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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.Invoice;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Invoice.Views
{
    [View(IsSingleton = true)]
    public partial class FinanceQuery : PageBase
    {
        FinanceQueryFilter m_queryRequest;
        FinancialFacade facade;
        List<FinanceVM> viewList;
        List<FinanceVM> viewByVendorGroupList;
        PayableFacade payableFacade;
        OtherDomainDataFacade otherDomainFacade;

        public FinanceQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermissions();
            base.OnPageLoad(sender, e);
            this.SearchCondition.DataContext = m_queryRequest = new FinanceQueryFilter();
            facade = new FinancialFacade(this);

            payableFacade = new PayableFacade(this);
            otherDomainFacade = new OtherDomainDataFacade(this);
            BindComboBoxData();

            m_queryRequest.IsMangerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.Invoice_FinanceQuery_PM);
            m_queryRequest.OperationUserSysNo = CPApplication.Current.LoginUser.UserSysNo.Value;

        }

        private void VerifyPermissions()
        {
            this.btnCaclMerged.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_FinanceQuery_CaclMerged);
            this.btnAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_FinanceQuery_PMAudit)
                || AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_FinanceQuery_FNAudit);
            this.btnRefuseAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_FinanceQuery_RefuseAudit);
            this.btnBatchPay.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_FinanceQuery_BatchPay);

            this.dgQueryResult.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_FinanceQuery_Export);

        }

        /// <summary>
        /// 查询按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (m_queryRequest.IsGroupByVendor == null
                || m_queryRequest.IsGroupByVendor == false)
            {

                this.dgQueryResult.Bind();
            }
            else
            {
                this.dgVendorByGroupQueryResult.Bind();
            }
        }

        /// <summary>
        /// 非按供应商汇总查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            svStatisticInfo.Visibility = System.Windows.Visibility.Collapsed;
            facade.QueryFinance(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.dgQueryResult.TotalCount = args.Result[0].TotalCount;
                viewList = DynamicConverter<FinanceVM>.ConvertToVMList(args.Result[0].Rows);
                this.dgQueryResult.ItemsSource = viewList;
                if (this.dgQueryResult.TotalCount > 0)
                {
                    svStatisticInfo.Visibility = System.Windows.Visibility.Visible;
                    tbStatisticInfo.Text = string.Format(ResFinanceQuery.Message_TotalInfo, double.Parse((string)args.Result[1].Rows[0]["Amt"]).ToString("###,###,###0.00"));
                }
            });
        }
        /// <summary>
        /// 按供应商汇总查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgVendorByGroupQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };

            svStatisticInfo.Visibility = System.Windows.Visibility.Collapsed;
            facade.QueryFinance(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.dgVendorByGroupQueryResult.TotalCount = args.Result[0].TotalCount;
                viewByVendorGroupList = DynamicConverter<FinanceVM>.ConvertToVMList(args.Result[0].Rows);
                this.dgVendorByGroupQueryResult.ItemsSource = viewByVendorGroupList;

                if (this.dgVendorByGroupQueryResult.TotalCount > 0)
                {
                    svStatisticInfo.Visibility = System.Windows.Visibility.Visible;
                    tbStatisticInfo.Text = string.Format(ResFinanceQuery.Message_TotalInfo, double.Parse((string)args.Result[1].Rows[0]["Amt"]).ToString("###,###,###0.00"));
                }
            });
        }

        /// <summary>
        /// 绑定下拉框数据
        /// </summary>
        private void BindComboBoxData()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_Invoice, new string[] { ConstValue.Key_AuditStatus, ConstValue.Key_VendorPayPeriod }, CodeNamePairAppendItemType.All, (o, p) =>
                {
                    this.cmbAuditStaus.ItemsSource = p.Result[ConstValue.Key_AuditStatus];
                    this.cmbAuditStaus.SelectedIndex = 0;
                    this.cmbVendorPayPeriod.ItemsSource = p.Result[ConstValue.Key_VendorPayPeriod];
                    this.cmbVendorPayPeriod.SelectedIndex = 0;
                });

            //facade.GetPMGroup((obj, args) =>
            //    {
            //        if (args.FaultsHandle()) return;
            //        this.cmbPMGroup.ItemsSource = args.Result.Rows;
            //        this.cmbPMGroup.SelectedIndex = 0;
            //    });
            this.cmbPaySettleCompany.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.PaySettleCompany>(EnumConverter.EnumAppendItemType.All);
            this.cmbPaySettleCompany.SelectedIndex = 0;
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic vList = this.dgQueryResult.ItemsSource as dynamic;
            if (vList != null)
            {
                foreach (var view in vList)
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkEdit_Click(object sender, RoutedEventArgs e)
        {
            FinanceVM selectedView = this.dgQueryResult.SelectedItem as FinanceVM;
            if (selectedView != null)
            {             
                var uc = new UCRemarkEdit(selectedView);
                uc.ShowDialog(ResFinanceQuery.Message_EditRemark, (obj, args) =>
                    {                       
                        if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                        {
                            this.dgQueryResult.Bind();
                        }
                    }, new Size(650, 450));
            }
        }

        /// <summary>
        /// 合计应收款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCaclMerged_Click(object sender, RoutedEventArgs e)
        {
            if (viewList == null || viewList.Count == 0)
            {
                this.Window.Alert(ResFinanceQuery.Message_PleaseSelect);
            }
            else
            {
                int selectCount = 0;
                decimal MergerCount = 0;
                foreach (var view in viewList)
                {
                    if (view.IsCheck)
                    {
                        selectCount++;
                        MergerCount += view.PayableAmt;
                    }
                }
                if (selectCount != 0)
                {
                    Window.Alert(string.Format(ResFinanceQuery.Message_MergerCount, selectCount, Math.Round(MergerCount, 2)));
                }
                else
                {
                    this.Window.Alert(ResFinanceQuery.Message_PleaseSelect);
                }
            }
        }

        #region 工具栏按钮事件
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            bool isPM = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_FinanceQuery_PMAudit);
            bool isFN = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_FinanceQuery_FNAudit);
            //if (!isPM
            //    && !isFN)
            //{
            //    this.Window.Alert(ResFinanceQuery.Message_InValidRight);
            //    return;
            //}
            string msg = string.Empty;
            if (viewList == null || viewList.Count == 0)
            {
                this.Window.Alert(ResFinanceQuery.Message_PleaseSelect);
                return;
            }
            else
            {
                List<PayableInfo> selectList = new List<PayableInfo>();
                foreach (var view in viewList)
                {
                    if (view.IsCheck)
                    {
                        var newList = new PayableInfo()
                        {
                            SysNo = view.SysNo,
                            AuditStatus = view.AuditStatus,
                            OrderSysNo = view.OrderID
                        };
                        if (view.AuditStatus == PayableAuditStatus.NotAudit)
                        {
                            if (isPM)
                            {
                                newList.AuditStatus = PayableAuditStatus.WaitFNAudit;
                            }
                            else
                            {
                                msg = ResFinanceQuery.Message_InValidRight_PMAuditRight;
                            }
                        }
                        else if (view.AuditStatus == PayableAuditStatus.WaitFNAudit)
                        {
                            if (isFN)
                            {
                                newList.AuditStatus = PayableAuditStatus.Audited;
                            }
                            else
                            {
                                msg = ResFinanceQuery.Message_InValidRight_FNAuditRight;
                            }
                        }
                        else
                        {
                            msg = ResFinanceQuery.Message_InValidRight_Audited;
                        }
                        newList.Tag = msg;
                        selectList.Add(newList);
                    }
                }
                if (selectList == null || selectList.Count == 0)
                {
                    this.Window.Alert(ResFinanceQuery.Message_PleaseSelect);
                }
                else
                {
                    payableFacade.BatchAudit(selectList, (obj, args) =>
                    {
                        if (this.m_queryRequest.IsGroupByVendor == false
                            || !this.m_queryRequest.IsGroupByVendor.HasValue)
                        {
                            this.dgQueryResult.Bind();
                        }
                        Window.Alert(args.Result);
                    });
                }
            }
        }

        /// <summary>
        /// 审核拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefuseAudit_Click(object sender, RoutedEventArgs e)
        {
            if (viewList == null || viewList.Count == 0)
            {
                this.Window.Alert(ResFinanceQuery.Message_PleaseSelect);
            }
            else
            {
                List<PayableInfo> selectList = new List<PayableInfo>();
                foreach (var view in viewList)
                {
                    if (view.IsCheck)
                    {
                        var newList = new PayableInfo()
                        {
                            SysNo = view.SysNo,
                            AuditStatus = PayableAuditStatus.NotAudit,
                            OrderSysNo = view.OrderID,
                            PMSysNo = view.PMUserNo
                        };
                        selectList.Add(newList);
                    }
                }
                if (selectList == null || selectList.Count == 0)
                {
                    this.Window.Alert(ResFinanceQuery.Message_PleaseSelect);
                }
                else
                {
                    payableFacade.BatchRefuseAudit(selectList, (obj, args) =>
                    {
                        Window.Alert(args.Result);
                    });
                }
            }
        }

        /// <summary>
        /// 批量付款
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchPay_Click(object sender, RoutedEventArgs e)
        {
            if (viewList == null || viewList.Count == 0)
            {
                this.Window.Alert(ResFinanceQuery.Message_PleaseSelect);
            }
            else
            {
                List<PayableInfo> selectList = new List<PayableInfo>();
                foreach (var view in viewList)
                {
                    if (view.IsCheck)
                    {
                        var newList = new PayableInfo()
                        {
                            SysNo = view.SysNo,
                            AuditStatus = view.AuditStatus,
                            OrderSysNo = view.OrderID,
                            AlreadyPayAmt = view.AlreadyP
                        };
                        selectList.Add(newList);
                    }
                }
                if (selectList == null || selectList.Count == 0)
                {
                    this.Window.Alert(ResFinanceQuery.Message_PleaseSelect);
                }
                else
                {
                    payableFacade.BatchUpdateStatusAndAlreadyPayAmt(selectList, (obj, args) =>
                    {
                        Window.Alert(args.Result);
                        this.dgQueryResult.Bind();
                    });
                }
            }
        }
        #endregion

        /// <summary>
        /// 是否按供应商汇总
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxVendorSummary_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            if (cbx.IsChecked == true)
            {
                this.txtInvoiceID.IsEnabled = false;
                this.SearchResult.Visibility = System.Windows.Visibility.Collapsed;
                this.functionPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.SearchResultByVendorGroup.Visibility = System.Windows.Visibility.Visible;

            }
            else
            {
                this.txtInvoiceID.IsEnabled = true;
                this.SearchResult.Visibility = System.Windows.Visibility.Visible;
                this.functionPanel.Visibility = System.Windows.Visibility.Visible;
                this.SearchResultByVendorGroup.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #region 报表数据导出
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (this.dgQueryResult == null || this.dgQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }

            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            FinancialFacade facade = new FinancialFacade(this);

            ColumnSet col = new ColumnSet(dgQueryResult, true);
            col.Add("RMACountDescription", ResFinanceQuery.DataGrid_Column_Head_IsVendorRepairParts);

            facade.ExportFinance(m_queryRequest, new ColumnSet[] { col });
        }

        private void dgVendorByGroupQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            FinancialFacade facade = new FinancialFacade(this);

            ColumnSet col = new ColumnSet(dgVendorByGroupQueryResult);

            col.Insert(10, "R4", "未扣减的EIMS-票扣", 20);
            col.Insert(11, "R2", "未扣减的EIMS-帐扣", 20);
            col.Insert(12, "R0", "未扣减的EIMS-PO单扣减", 20);
            col.Insert(13, "R3", "未扣减的EIMS-代销结算单扣减", 20);

            facade.ExportFinanceVendorByGroup(m_queryRequest, new ColumnSet[] { col });
        }
        #endregion
    }
}
