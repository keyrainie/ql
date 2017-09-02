using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 退款审核页面
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class AuditRefundQuery : PageBase
    {
        private AuditRefundQueryVM queryVM;
        private AuditRefundQueryVM lastQueryVM;
        private AuditRefundFacade auditRefundFacade;
        private OtherDomainDataFacade otherFacade;
        private CommonDataFacade commonFacade;

        public AuditRefundQuery()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(AuditRefundQuery_Loaded);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermissions();
            base.OnPageLoad(sender, e);
        }

        private void AuditRefundQuery_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(AuditRefundQuery_Loaded);

            InitData();
            auditRefundFacade = new AuditRefundFacade(this);
            otherFacade = new OtherDomainDataFacade(this);
            commonFacade = new CommonDataFacade(this);
            LoadComboBoxData();
        }

        private void InitData()
        {
            queryVM = new AuditRefundQueryVM();
            SeachBuilder.DataContext = lastQueryVM = queryVM;
        }

        private void LoadComboBoxData()
        {
            //默认选中第一个销售渠道
            this.cmbWebChannel.SelectedIndex = 0;

            //加载退款原因列表 & 分仓列表
            otherFacade.GetRefundReaons(true, (obj, args) =>
            {
                queryVM.RefundReasonList = args.Result;
            });

            commonFacade.GetStockList(true, (obj, args) =>
            {
                queryVM.StockList = args.Result;
            });
        }

        /// <summary>
        /// 验证权限
        /// </summary>
        private void VerifyPermissions()
        {
            this.btnCSAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_Audit);
            this.btnCSReject.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_AuditReject);
            this.btnFinAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_FinPass);
            this.btnFinReject.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_FinRefuse);
            this.btnCancelAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_CancelAudit);
            this.dgAuditRefundQueryResult.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_Export);
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.dgAuditRefundQueryResult.ItemsSource as List<AuditRefundVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.SeachBuilder);
            if (flag)
            {
                this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AuditRefundQueryVM>(queryVM);

                this.dgAuditRefundQueryResult.Bind();
            }
        }

        private void dgAuditRefundQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            auditRefundFacade.Query(lastQueryVM, e.PageSize, e.PageIndex, e.SortField,
                result =>
                {
                    this.dgAuditRefundQueryResult.ItemsSource = result.ResultList;
                    this.dgAuditRefundQueryResult.TotalCount = result.TotalCount;
                });
        }

        /// <summary>
        /// 退款信息编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_AuditRefundQuery_Edit))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var cur = this.dgAuditRefundQueryResult.SelectedItem as AuditRefundVM;
            if (cur != null)
            {
                var uc = new UCSOIncomeRefundMaintain(cur.DeepCopy(), auditRefundFacade);
                uc.ShowDialog(ResAuditRefundQuery.Message_EditDlgTitle, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        this.dgAuditRefundQueryResult.Bind();
                    }
                });
            }
        }

        /// <summary>
        /// 退积分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RefundPoint_Click(object sender, RoutedEventArgs e)
        {
            //TODO:平安万里通积分支付“退积分”按钮才可见，先暂时不做，等到中蛋实施时候再做。
            Window.Alert("平安万里通积分支付“退积分”按钮才可见，先暂时不做，等到中蛋实施时候再做。");
        }

        /// <summary>
        /// 批量CS审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCSAudit_Click(object sender, RoutedEventArgs e)
        {
            //var selectedSysNoList = GetSelectedSysNoList();
            //if (selectedSysNoList.Count <= 0)
            //{
            //    Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
            //    return;
            //}



            List<AuditRefundVM> vmList = GetSelectVMList();
            if (vmList == null || vmList.Count == 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }


            auditRefundFacade.BatchCSAudit(vmList, msg =>
            {
                Window.Alert(msg, () => this.dgAuditRefundQueryResult.Bind());
            });
        }

        /// <summary>
        /// 批量CS审核拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCSReject_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            auditRefundFacade.BatchCSReject(selectedSysNoList, msg =>
            {
                Window.Alert(msg, () => this.dgAuditRefundQueryResult.Bind());
            });
        }

        /// <summary>
        /// 批量财务审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFinAudit_Click(object sender, RoutedEventArgs e)
        {
            //var selectedSysNoList = GetSelectedSysNoList();
            //if (selectedSysNoList.Count <= 0)
            //{
            //    Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
            //    return;
            //}

            List<AuditRefundVM> vmList = GetSelectVMList();
            if (vmList == null || vmList.Count == 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            auditRefundFacade.BatchFinAudit(vmList, msg =>
            {
                Window.Alert(msg, () => this.dgAuditRefundQueryResult.Bind());
            });
        }

        /// <summary>
        /// 批量财务审核拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFinReject_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            //弹出对话框，填写财务附加备注
            new UCAppendComment().ShowDialog(ResAuditRefundQuery.Message_AppendCommentDlgTitle, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    auditRefundFacade.BatchFinReject(selectedSysNoList, args.Data.ToString(), msg =>
                    {
                        Window.Alert(msg, () => this.dgAuditRefundQueryResult.Bind());
                    });
                }
            });
        }

        /// <summary>
        /// 批量取消审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            auditRefundFacade.BatchCancelAudit(selectedSysNoList, msg =>
            {
                Window.Alert(msg, () => this.dgAuditRefundQueryResult.Bind());
            });
        }

        private List<int> GetSelectedSysNoList()
        {
            var selectedSysNoList = new List<int>();
            var dataSource = this.dgAuditRefundQueryResult.ItemsSource as List<AuditRefundVM>;
            if (dataSource != null)
            {
                selectedSysNoList = dataSource.Where(w => w.IsChecked)
                    .Select(s => s.SysNo.Value).ToList();
            }
            return selectedSysNoList;
        }
        /// <summary>
        /// 获取选中的销售单信息
        /// </summary>
        /// <returns></returns>
        private List<AuditRefundVM> GetSelectVMList()
        {
            List<AuditRefundVM> vmList = null;
            var data = this.dgAuditRefundQueryResult.ItemsSource as List<AuditRefundVM>;
            if (data != null)
            {
                vmList = data.Where(w => w.IsChecked).ToList();
                for (int i = 0; i < vmList.Count; i++)
                {
                    vmList[i].AuditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
                }
            }
            return vmList;
        }

        private void dgAuditRefundQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.dgAuditRefundQueryResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }

            ColumnSet col = new ColumnSet(this.dgAuditRefundQueryResult);
            col.Add("RefundStatusDesc", ResAuditRefundQuery.Grid_RMARefundStatus);
            col.Add("RefundCashAmt", ResAuditRefundQuery.Grid_RefundAmount);
            col.Add("CreateUser", ResAuditRefundQuery.Grid_CreateUser);
            col.Add("CreateTime", ResAuditRefundQuery.Grid_CreateTime);
            col.Add("AuditUser", ResAuditRefundQuery.Grid_AuditUser);
            col.Add("AuditTime", ResAuditRefundQuery.Grid_AuditTime);

            auditRefundFacade.ExportExcelFile(lastQueryVM, new ColumnSet[] { col });
        }

        private void Hyperlink_CustomerSysNo_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgAuditRefundQueryResult.SelectedItem as AuditRefundVM;
            if (cur != null)
            {
                Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, cur.CustomerSysNo), null, true);
            }
        }

        private void Hyperlink_SOSysNo_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgAuditRefundQueryResult.SelectedItem as AuditRefundVM;
            if (cur != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, cur.SOSysNo), null, true);
            }
        }

        private void Hyperlink_RMANumber_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgAuditRefundQueryResult.SelectedItem as AuditRefundVM;
            if (cur != null)
            {
                if (cur.OrderType == RefundOrderType.RO)
                {
                    Window.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, cur.RMANumber), null, true);
                }
                else if (cur.OrderType == RefundOrderType.AO || cur.OrderType == RefundOrderType.OverPayment)
                {
                    Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, cur.RMANumber), null, true);
                }
               
            }
        }

        /// <summary>
        /// 退预付卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RefundPrepayCard_Click(object sender, RoutedEventArgs e)
        {
            var data = dgAuditRefundQueryResult.SelectedItem as AuditRefundVM;
            UCRefundPrepayCard uc = new UCRefundPrepayCard(data);
            uc.ShowDialog(ResAuditRefundQuery.Title_RefundPrepayCard, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                        this.dgAuditRefundQueryResult.Bind();
                });
        }

        /// <summary>
        /// 判断退款结果查询条件是否可用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRefundPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {           
            {
                this.cmbRefundResult.SelectedIndex = 0;
                this.cmbRefundResult.IsEnabled = false;
            }
        }
    }
}