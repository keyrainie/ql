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


using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.UserControls.Customer;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CustomerQuery : PageBase
    {
        #region 属性

        CustomerQueryReqVM viewModel;
        CustomerQueryReqVM newViewModel;
        CustomerQueryFilter filter;

        CommonDataFacade commonFacade;
        #endregion

        #region 初始化加载

        public CustomerQuery()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化控件状态
        /// </summary>
        private void InitContral()
        {
            Init();
            InitSelectCombox();
            viewModel.IsBuyCountEndpointValue = true;
            viewModel.OperationSign = 0;
            tbCustomerID.Focus();
            CheckRights();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {

            base.OnPageLoad(sender, e);
            viewModel = new CustomerQueryReqVM();
            commonFacade = new CommonDataFacade(this);
            this.DataContext = viewModel;
            InitContral();
        }

        public void Init()
        {
            CommonDataFacade facade = new CommonDataFacade(this);
            AppSettingHelper.GetSetting("Customer", "AvtarImageBasePath", (obj, args) =>
              {
                  viewModel.AvtarImgBasePath = args.Result;
              });
        }
        public void InitSelectCombox()
        {
            cbCustomerType.SelectedIndex = 0;
            cbCustomerStatus.SelectedIndex = 0;
            cbIsVIP.SelectedIndex = 0;
            cbIsEmailConfirmed.SelectedIndex = 0;
            cbIsPhoneConfirmed.SelectedIndex = 0;
            cbAvtarImageStatus.SelectedIndex = 0;
            cbCustomerType.SelectedIndex = 0;
            //commonFacade.GetWebChannelList(true, (sender, e) =>
            //{
            //    this.cmbChannel.ItemsSource = e.Result;
            //});
            commonFacade.GetSociety(obj =>
            {
                this.cmbSociety.ItemsSource = obj;
            });
        }

        #endregion

        #region 查询绑定

        private void DataGrid_Result_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            CustomerQueryFacade facade = new CustomerQueryFacade(this);
            filter.PagingInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            facade.QueryCustomer(filter, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (!(args == null || args.Result == null || args.Result.Rows == null))
                    {
                        #region 拼接头像地址
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                            CustomerRank rank = item.Rank ?? CustomerRank.Ferrum;
                            string AvtarImage = item.AvtarImage;
                            if (!string.IsNullOrEmpty(AvtarImage))
                            {
                                item.AvtarImage = string.Format("{0}{1}", viewModel.AvtarImgBasePath, AvtarImage);
                            }
                            else
                            {
                                item.AvtarImage = string.Format("/Images/Customer/CustomerRankImage/P48Rank{0}.jpg", (int)rank);
                            }
                        }
                        #endregion
                        this.dataGrid1.ItemsSource = args.Result.Rows.ToList();
                        this.dataGrid1.TotalCount = args.Result.TotalCount;
                    }
                });
        }



        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            newViewModel = viewModel.DeepCopy();
            ValidationManager.Validate(this.BaseSeachBuilder);
            if (!string.IsNullOrWhiteSpace(viewModel.CustomerSysNo))
            {
                int tempSysNo = 0;
                if (!int.TryParse(viewModel.CustomerSysNo, out tempSysNo))
                {
                    Window.Alert("无效的“系统编号”!");
                    return;
                }
            }
            if (ckb_MoreQueryBuilder.IsChecked == true)
            {
                ValidationManager.Validate(this.DetailSeachBuilder);
                if (!viewModel.HasValidationErrors)
                {
                    filter = viewModel.ConvertVM<CustomerQueryReqVM, CustomerQueryFilter>();
                    dataGrid1.Bind();
                }
            }
            else
            {
                IsNormalQuery();
                filter = newViewModel.ConvertVM<CustomerQueryReqVM, CustomerQueryFilter>();
                dataGrid1.Bind();
            }
        }

        //普通查询时，不使用更条条件
        private void IsNormalQuery()
        {
            newViewModel.CustomerSysNo = string.Empty;
            newViewModel.IsEmailConfirmed = null;
            newViewModel.IsPhoneConfirmed = null;
            newViewModel.FromLinkSource = string.Empty;
            newViewModel.RecommendedByCustomerID = string.Empty;
            newViewModel.AvtarImageStatus = null;
            newViewModel.CustomersType = null;
            newViewModel.IsVip = null;
            newViewModel.RegisterTimeFrom = null;
            newViewModel.RegisterTimeTo = null;
        }

        #endregion

        #region 页面内按钮处理事件

        //导入
        private void btnBatchImportCustomer_Click(object sender, RoutedEventArgs e)
        {
            BatchImportCustomer DialogPage = new BatchImportCustomer();
            DialogPage.Dialog = Window.ShowDialog(ECCentral.Portal.UI.Customer.Resources.ResCustomerQuery.PopTitle_BatchImportCustomer, DialogPage, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        if (filter == null)
                            filter = new CustomerQueryFilter();
                        this.dataGrid1.Bind();
                    }
                });
        }



        #region 头像处理
        private void btnActiveCustomerImage_Click(object sender, RoutedEventArgs e)
        {
            List<int> sysNoList = GetSelectCustomerSysNoList();
            if (sysNoList.Count > 0)
            {
                CustomerFacade facade = new CustomerFacade(this);
                facade.BatchShowAvatar(sysNoList, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert(ResCustomerQuery.Msg_OperationOk, MessageType.Information);
                    RefreshDataGrid();
                });
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }


        private void btnInactiveCustomerImage_Click(object sender, RoutedEventArgs e)
        {
            List<int> sysNoList = GetSelectCustomerSysNoList();
            if (sysNoList.Count > 0)
            {
                CustomerFacade facade = new CustomerFacade(this);
                facade.BatchNotShowAvatar(sysNoList, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert(ResCustomerQuery.Msg_OperationOk, MessageType.Information);
                    RefreshDataGrid();
                });
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }


        private List<int> GetSelectCustomerSysNoList()
        {
            List<int> sysNoList = new List<int>();
            if (this.dataGrid1.ItemsSource != null)
            {
                dynamic viewList = this.dataGrid1.ItemsSource as dynamic;
                foreach (var view in viewList)
                {
                    if (view.IsChecked)
                    {
                        sysNoList.Add(view.SysNo);
                    }
                }
            }
            return sysNoList;
        }

        private void RefreshDataGrid()
        {
            this.dataGrid1.Bind();
        }

        #endregion

        #region 界面事件

        //更多条件
        private void ckb_MoreQueryBuilder_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_MoreQueryBuilder.IsChecked.HasValue && ckb_MoreQueryBuilder.IsChecked.Value)
            {
                spMoreQueryBuilder.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                spMoreQueryBuilder.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        //购买数量
        private void ckb_BuyCount_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_BuyCount.IsChecked.HasValue && ckb_BuyCount.IsChecked.Value)
            {
                rbtnBuyCountAreaValue.IsEnabled = true;
                rbtnBuyCountEndpointValue.IsEnabled = true;
                cbOperationSign.IsEnabled = true;
                tbBuyCountValue.IsEnabled = true;
                tbMinBuyCount.IsEnabled = true;
                tbMaxBuyCount.IsEnabled = true;
            }
            else
            {
                rbtnBuyCountAreaValue.IsEnabled = false;
                rbtnBuyCountEndpointValue.IsEnabled = false;
                cbOperationSign.IsEnabled = false;
                tbBuyCountValue.IsEnabled = false;
                tbMinBuyCount.IsEnabled = false;
                tbMaxBuyCount.IsEnabled = false;
            }
        }

        //选择全部
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.dataGrid1.ItemsSource as dynamic;
                if (viewList != null)
                {
                    foreach (var view in viewList)
                    {
                        view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                    }
                }
            }
        }

        #endregion
        #endregion

        #region 跳转

        //经验值历史
        private void btnExperienceLog_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.dataGrid1.SelectedItem as dynamic;
            if (customer != null)
            {
                Window.Navigate(string.Format(ConstValue.CustomerExperienceLogQueryUrlFormat, customer.SysNo), null, true);
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }

        //优惠券发放历史
        private void btnPromotion_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.dataGrid1.SelectedItem as dynamic;
            if (customer != null)
            {
                Window.Navigate(string.Format("{0}?CustomerSysNo={1}", ConstValue.MKT_CouponCodeCustomerLogMaintainUrlFormat, customer.SysNo), null, true);
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }

        //积分历史
        private void btnPointLog_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.dataGrid1.SelectedItem as dynamic;
            if (customer != null)
            {
                Window.Navigate(string.Format(ConstValue.CustomerPointLogQueryUrlFormat, customer.SysNo), null, true);
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }

        //编辑
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.dataGrid1.SelectedItem as dynamic;
            if (customer != null)
            {
                Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, customer.SysNo), null, true);
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }

        //HyperLink编辑跳转
        private void HyperLink_CustomerNumber_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.dataGrid1.SelectedItem as dynamic;
            if (customer != null)
            {
                this.Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, customer.SysNo), null, true);
            }
        }

        //创建
        private void btnCustomerNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.CustomerMaintainCreateUrl, null, true);
        }

        //创建订单
        private void btnNewSo_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.dataGrid1.SelectedItem as dynamic;
            if (customer != null)
            {
                Window.Navigate(string.Format(ConstValue.CreateSoByCustomerUrlFormat, customer.SysNo), null, true);
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }

        //权限
        private void btnCustomerRights_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.dataGrid1.SelectedItem as dynamic;
            if (customer != null)
            {
                CustomerRightMaintain dilog = new CustomerRightMaintain();
                dilog.viewModel.CustomerSysNo = customer.SysNo;
                dilog.Dialog = Window.ShowDialog(ResCustomerQuery.Dialog_RightManage, dilog, null, new Size(400, 380));
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }

        //查看密保问题
        private void btnViewSecurityQues_Click(object sender, RoutedEventArgs e)
        {
            dynamic customer = this.dataGrid1.SelectedItem as dynamic;
            if (customer != null)
            {
                UCViewSecurityQuestion dialog = new UCViewSecurityQuestion(customer.SysNo);
                dialog.Dialog = Window.ShowDialog(ResCustomerQuery.Dialog_ViewSecurityQues, dialog, null, new Size(540, 400));
            }
            else
            {
                Window.Alert(ResCustomerQuery.Msg_OnSelectCostomer, MessageType.Error);
            }
        }
        #endregion

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_Add))
                this.btnCustomerNew.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_BatchActiveImg))
                this.btnActiveCustomerImage.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_BatchInActiveImg))
                this.btnInactiveCustomerImage.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerQuery_BatchImportCustomer))
                this.btnBatchImportCustomer.IsEnabled = false;

        }
        #endregion

        private void cmbSociety_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbSociety.SelectedItem != null && this.cmbSociety.SelectedValue != null)
            {
                viewModel.SocietyID = Convert.ToInt32(this.cmbSociety.SelectedValue);
            }
        }
    }

}
