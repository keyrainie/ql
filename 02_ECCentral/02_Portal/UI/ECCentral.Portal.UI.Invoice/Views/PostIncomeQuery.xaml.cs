using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic;
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

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 销售-银行电汇邮局收款单查询页面
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class PostIncomeQuery : PageBase
    {
        #region Private Fields

        private PostIncomeQueryVM queryVM;
        private PostIncomeQueryVM lastQueryVM;
        private PostIncomeFacade facade;

        #endregion Private Fields

        #region Constructor

        public PostIncomeQuery()
        {
            InitializeComponent();
            
            InitData();
            Loaded += new RoutedEventHandler(PostIncomeQuery_Loaded);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermission();
            base.OnPageLoad(sender, e);
        }


        #endregion Constructor

        #region Event Handlers

        private void PostIncomeQuery_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(PostIncomeQuery_Loaded);
            
            this.facade = new PostIncomeFacade(this);
            LoadComboBoxData();
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.dgPostIncomeQueryResult.ItemsSource as List<PostIncomeVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            bool flag = ValidationManager.Validate(this.SeachBuilder);
            if (flag)
            {
                this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PostIncomeQueryVM>(queryVM);

                this.dgPostIncomeQueryResult.Bind();
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.PostIncomeImportUrl, null, true);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.Invoice_PostIncomeQuery_New))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            new UCPostIncomeMaintain().ShowDialog(ResPostIncomeQuery.Message_CreatePostIncomeDlgTitle, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.dgPostIncomeQueryResult.Bind();
                }
            });
        }

        private void dgPostIncomeQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.facade.Query(lastQueryVM, e.PageSize, e.PageIndex, e.SortField,
                result =>
                {
                    this.dgPostIncomeQueryResult.ItemsSource = result.ResultList;
                    this.dgPostIncomeQueryResult.TotalCount = result.TotalCount;
                });
        }

        private void dgPostIncomeQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.dgPostIncomeQueryResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }

            ColumnSet col = new ColumnSet(this.dgPostIncomeQueryResult);
            col.Add("IncomeAmt", ResPostIncomeQuery.Grid_IncomeAmt);
            col.Add("HandleStatusDesc", ResPostIncomeQuery.Grid_HandleStatus);
            col.Add("OrderTime", ResPostIncomeQuery.Grid_OrderTime);
            col.Add("OutTime", ResPostIncomeQuery.Grid_OutTime);
            col.Add("CreateDate", ResPostIncomeQuery.Grid_CreateDate,"yyyy-MM-dd", 12);
            col.Add("ModifyDate", ResPostIncomeQuery.Grid_ModifyDate, "yyyy-MM-dd", 12);
            col.Add("IncomeDate", ResPostIncomeQuery.Grid_IncomeDate, "yyyy-MM-dd", 12);
            col.Add("ConfirmDate", ResPostIncomeQuery.Grid_ConfirmDate, "yyyy-MM-dd", 12);

            facade.ExportExcelFile(lastQueryVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_View_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.Invoice_PostIncomeQuery_View))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var cur = this.dgPostIncomeQueryResult.SelectedItem as PostIncomeVM;
            if (cur != null)
            {
                new UCPostIncomeMaintain(cur.DeepCopy(), UCPostIncomeMaintain.MaintanMode.View).ShowDialog(ResPostIncomeQuery.Message_ViewPostIncomeDlgTitle, null);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Modify_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.Invoice_PostIncomeQuery_Edit))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var cur = this.dgPostIncomeQueryResult.SelectedItem as PostIncomeVM;
            if (cur != null)
            {
                new UCPostIncomeMaintain(cur.DeepCopy(), UCPostIncomeMaintain.MaintanMode.Modify).ShowDialog(ResPostIncomeQuery.Message_ModifyPostIncomeDlgTitle, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        this.dgPostIncomeQueryResult.Bind();
                    }
                });
            }
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Abandon_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PostIncomeQuery_Abandon))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var cur = this.dgPostIncomeQueryResult.SelectedItem as PostIncomeVM;
            if (cur != null)
            {
                Window.Confirm(ResPostIncomeQuery.Message_AbandonPostIncomeDlgTitle, (data) =>
                {
                    facade.Abandon(cur.SysNo.Value, () => this.dgPostIncomeQueryResult.Bind());
                });
            }
        }

        /// <summary>
        /// 取消作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_CancelAbandon_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PostIncomeQuery_CancelAbandon))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var cur = this.dgPostIncomeQueryResult.SelectedItem as PostIncomeVM;
            if (cur != null)
            {
                Window.Confirm(ResPostIncomeQuery.Message_CancelAbandonPostIncomeDlgTitle, (data) =>
                {
                    facade.CancelAbandon(cur.SysNo.Value, () => this.dgPostIncomeQueryResult.Bind());
                });
            }
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PostIncomeQuery_Confirm))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }


            var cur = this.dgPostIncomeQueryResult.SelectedItem as PostIncomeVM;
            if (cur != null)
            {
                Window.Confirm(ResPostIncomeQuery.Message_ConfirmPostIncomeDlgTitle, (data) =>
                {
                    facade.Confirm(cur.SysNo.Value, () => this.dgPostIncomeQueryResult.Bind());
                });
            }
        }

        /// <summary>
        /// 取消确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_CancelConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PostIncomeQuery_CancelConfrim))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var cur = this.dgPostIncomeQueryResult.SelectedItem as PostIncomeVM;
            if (cur != null)
            {
                Window.Confirm(ResPostIncomeQuery.Message_CancelConfirmPostIncomeDlgTitle, (data) =>
                {
                    facade.CancelConfrim(cur.SysNo.Value, () => this.dgPostIncomeQueryResult.Bind());
                });
            }
        }

        /// <summary>
        /// 链接到订单维护页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_SOSysNo_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgPostIncomeQueryResult.SelectedItem as PostIncomeVM;
            if (cur != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, cur.SOSysNo), null, true);
            }
        }

        /// <summary>
        /// 批量确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count == 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            //执行批量操作
            facade.BatchConfirm(selectedSysNoList,
                msg => Window.Alert(msg, () => this.dgPostIncomeQueryResult.Bind())
            );
        }

        #endregion Event Handlers

        #region Private Methods

        private void InitData()
        {
            queryVM = new PostIncomeQueryVM();
            this.SeachBuilder.DataContext = lastQueryVM = queryVM;
        }

        private void LoadComboBoxData()
        {
            //默认选中第一个销售渠道
            this.cmbWebChannel.SelectedIndex = 0;
        }

        /// <summary>
        /// 验证权限，控制按钮的可用性
        /// </summary>
        private void VerifyPermission()
        {
            this.dgPostIncomeQueryResult.IsShowExcelExporter = false;

            this.btnBatchConfirm.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PostIncomeQuery_Confirm);
            this.dgPostIncomeQueryResult.IsShowAllExcelExporter =  AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PostIncomeQuery_Export);
        }

        private List<int> GetSelectedSysNoList()
        {
            var selectedSysNoList = new List<int>();
            var itemSource = this.dgPostIncomeQueryResult.ItemsSource as List<PostIncomeVM>;
            if (itemSource != null)
            {
                selectedSysNoList = itemSource.Where(w => w.IsChecked)
                    .Select(s => s.SysNo.Value)
                    .ToList();
            }
            return selectedSysNoList;
        }

        #endregion Private Methods
    }
}