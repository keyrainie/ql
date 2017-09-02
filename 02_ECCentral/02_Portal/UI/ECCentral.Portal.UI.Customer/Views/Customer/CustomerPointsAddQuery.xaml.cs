using System;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Customer;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View(IsSingleton = true)]
    public partial class CustomerPointsAddQuery : PageBase
    {
        public CustomerPointsAddQueryFacade serviceFacade;
        public CustomerPointsAddQueryVM viewModel;
        CustomerPointsAddRequestFilter queryRequest;
        public CustomerPointsAddQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            serviceFacade = new CustomerPointsAddQueryFacade(this);
            viewModel = new CustomerPointsAddQueryVM();
            viewModel.HasExportRight = AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PointAddRequest_Export);
            queryRequest = new CustomerPointsAddRequestFilter();
            this.DataContext = viewModel;
            int customerSysNo = 0;
            if (!string.IsNullOrEmpty(Request.Param) && int.TryParse(Request.Param, out customerSysNo))
            {
                TextBox_CustomerID.SetCustomerSysNo(customerSysNo);
            }
            base.OnPageLoad(sender, e);
            CodeNamePairHelper.GetList("Customer", "SystemAccount", (s, arg) =>
            {
                if (arg.FaultsHandle())
                    return;
                foreach (var item in arg.Result)
                {
                    viewModel.sysAccountList.Add(new CodeNamePair() { Code = item.Code, Name = item.Name });
                }
                viewModel.sysAccountList.Insert(0, new CodeNamePair() { Name = ResCommonEnum.Enum_All });
                Combo_Account.SelectedIndex = 0;
            });
            CheckRights();
        }

        private void DataGrid_Result_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            viewModel.NeweggAccountDesc = this.Combo_Account.SelectedValue != null ? (this.Combo_Account.SelectedItem as dynamic).Name.ToString() : null;
            viewModel.OwnByDepartmentDesc = this.Combo_OwnByDepartment.SelectedValue != null ? (this.Combo_OwnByDepartment.SelectedItem as dynamic).Name.ToString() : null;
            viewModel.OwnByReasonDesc = (this.Combo_Memo.SelectedIndex != 0 && this.Combo_Memo.SelectedIndex != -1) ? (this.Combo_Memo.SelectedItem as dynamic).Name.ToString() : null;
            queryRequest = viewModel.ConvertVM<CustomerPointsAddQueryVM, CustomerPointsAddRequestFilter>();
            queryRequest.PageInfo = new PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            serviceFacade.QueryCustomerPointsAddList(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var customerPointsAddRequestList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = customerPointsAddRequestList;
            });
        }


        /// <summary>
        /// 查看顾客加积分申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperLink_ViewPointsAddDetail_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml selectedModel = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != selectedModel)
            {
                CustomerPointsAddRequestDetailInfo detailInfoCtrl = new CustomerPointsAddRequestDetailInfo(selectedModel, "View");
                detailInfoCtrl.Dialog = Window.ShowDialog(
                           ResCustomerPointsAddRequest.ViewPointsAddRequest_Header
                           , detailInfoCtrl
                           , (s, args) =>
                           {
                               if (args.DialogResult == DialogResultType.OK && args.Data != null)
                               {
                                   QueryResultGrid.PageIndex = 0;
                                   QueryResultGrid.SelectedIndex = -1;
                                   QueryResultGrid.Bind();
                               }
                           }
                           , new Size(600, 530)
                    );
            }
        }

        /// <summary>
        /// 搜索顾客加积分查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!viewModel.HasValidationErrors)
            {
                QueryResultGrid.Bind();
            }
        }

        /// <summary>
        /// 新增顾客加积分申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            //if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_PointAddRequest_Submit))
            //{
            //    Window.Alert(ResCustomerPointsAddRequest.rightMsg_NoRight_SubmitRequest, MessageType.Error);
            //    return;
            //}
            CustomerPointsAddRequestNew newRequestCtrl = new CustomerPointsAddRequestNew(TextBox_CustomerID.CustomerSysNo);
            newRequestCtrl.Dialog = Window.ShowDialog(
                       ResCustomerPointsAddRequest.AddPointsAddRequest_Header
                       , newRequestCtrl
                       , (s, args) =>
                       {
                           if (args.DialogResult == DialogResultType.OK && args.Data != null)
                           {
                               QueryResultGrid.PageIndex = 0;
                               QueryResultGrid.SelectedIndex = -1;
                               QueryResultGrid.Bind();
                           }
                       }
                       , new Size(650, 400)
                );
        }

        //private void QueryResultGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (this.QueryResultGrid.SelectedItem != null)
        //    {
        //        this.btnAudit.IsEnabled = ((CustomerPointsAddRequestStatus)(this.QueryResultGrid.SelectedItem as dynamic).status == CustomerPointsAddRequestStatus.AuditWaiting);
        //    }
        //}

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (this.QueryResultGrid.ItemsSource == null)
            {
                Window.Alert(ResCustomerPointsAddRequest.Msg_NoData);
                return;
            }
            queryRequest = viewModel.ConvertVM<CustomerPointsAddQueryVM, CustomerPointsAddRequestFilter>();
            queryRequest.PageInfo = new PagingInfo()
            {
                PageIndex = 0,
                PageSize = ConstValue.MaxRowCountLimit,
                SortBy = string.Empty
            };
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            col.SetWidth("neweggaccount", 15);
            col.SetWidth("ownbydepartment", 15);
            col.SetWidth("point", 15);
            col.SetWidth("memo", 20);
            col.SetWidth("CreateDate", 20);
            col.SetWidth("confirmtime", 20);
            serviceFacade.ExportCustomerPointsAddList(queryRequest, new ColumnSet[] { col });
        }

        private void Hyperlink_SOSysNo_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml customerPointsAddInfo = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (customerPointsAddInfo != null && customerPointsAddInfo["sosysno"] != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, customerPointsAddInfo["sosysno"].ToString()), null, true);
            }
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Points_Add))
                this.btnNew.IsEnabled = false;
        }
        #endregion

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Audit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Points_Audit))
            {
                Window.Alert(ResCustomerPointsAddRequest.rightMsg_NoRight_Audit);
                return;
            }
            //审核顾客加积分申请操作:
            DynamicXml selectedModel = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != selectedModel)
            {
                CustomerPointsAddRequestDetailInfo detailInfoCtrl = new CustomerPointsAddRequestDetailInfo(selectedModel, "Audit");
                detailInfoCtrl.Dialog = Window.ShowDialog(
                           ResCustomerPointsAddRequest.AuditPointsAddRequest_Header
                           , detailInfoCtrl
                           , (s, args) =>
                           {
                               if (args.DialogResult == DialogResultType.OK && args.Data != null)
                               {
                                   QueryResultGrid.PageIndex = 0;
                                   QueryResultGrid.SelectedIndex = -1;
                                   QueryResultGrid.Bind();
                               }
                           }
                           , new Size(600, 530)
                    );
            }
        }

        //系统账户
        private void Combo_Account_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BindReasonsBySysAccount();
        }

        //责任部门
        private void Combo_OwnByDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != this.Combo_OwnByDepartment.SelectedValue && !string.IsNullOrEmpty(this.Combo_OwnByDepartment.SelectedValue.ToString()))
            {
                viewModel.OwnByReasonVisibility = System.Windows.Visibility.Visible;
                CodeNamePairHelper.GetList("Customer", string.Format("ReasonForSystemAccountCode_OwnByDepartmentCode_{0}", this.Combo_OwnByDepartment.SelectedValue.ToString()), CodeNamePairAppendItemType.All, (obj, args) =>
                {
                    this.Combo_Memo.ItemsSource = args.Result;
                    this.Combo_Memo.SelectedIndex = 0;
                });
            }
            else
            {
                if (this.Combo_Account.SelectedValue != null && !string.IsNullOrEmpty(this.Combo_Account.SelectedValue.ToString()))
                {
                    string getSysAccountNo = this.Combo_Account.SelectedValue.ToString();
                    BindReason(getSysAccountNo);
                }
            }
        }

        private void BindReasonsBySysAccount()
        {
            this.Combo_Memo.Margin = new Thickness(7, 0, 0, 0);
            if (null != Combo_Account.SelectedValue && !string.IsNullOrEmpty(Combo_Account.SelectedValue.ToString()))
            {
                string getSysAccountNo = this.Combo_Account.SelectedValue.ToString();
                // 如果是CS - 补偿性积分，则绑定"责任部门"，清空
                if (Convert.ToInt32(getSysAccountNo) == 705571)
                {
                    this.Combo_Memo.ItemsSource = null;
                    this.Combo_OwnByDepartment.IsEnabled = true;
                    viewModel.OwnByDepartmentVisibility = System.Windows.Visibility.Visible;
                    CodeNamePairHelper.GetList("Customer", "OwnByDepartment", CodeNamePairAppendItemType.All, (obj, args) =>
                    {
                        this.Combo_OwnByDepartment.ItemsSource = args.Result;
                        this.Combo_OwnByDepartment.SelectedIndex = 0;
                    });
                    this.Combo_Memo.Margin = new Thickness(20, 0, 0, 0);
                }
                else
                {
                    viewModel.OwnByDepartmentVisibility = System.Windows.Visibility.Collapsed;
                    viewModel.OwnByReasonVisibility = System.Windows.Visibility.Visible;
                    this.Combo_OwnByDepartment.IsEnabled = false;
                    this.Combo_OwnByDepartment.ItemsSource = null;
                    BindReason(getSysAccountNo);

                }
            }
            else
            {
                viewModel.OwnByDepartmentVisibility = System.Windows.Visibility.Collapsed;
                viewModel.OwnByReasonVisibility = System.Windows.Visibility.Collapsed;
                this.Combo_Memo.ItemsSource = null;
                this.Combo_OwnByDepartment.ItemsSource = null;
            }
        }

        private void BindReason(string getSysAccountNo)
        {
            CodeNamePairHelper.GetList("Customer", string.Format("ReasonForSystemAccountCode_{0}", getSysAccountNo), CodeNamePairAppendItemType.All, (obj, args) =>
            {
                this.Combo_Memo.ItemsSource = args.Result;
                this.Combo_Memo.SelectedIndex = 0;
            });
        }
    }
}
