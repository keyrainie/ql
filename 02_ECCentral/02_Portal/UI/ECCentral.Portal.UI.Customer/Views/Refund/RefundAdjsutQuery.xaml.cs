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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.UI.Customer.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Refund
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class RefundAdjustQuery : PageBase
    {
        #region 页面初始化
        RefundAdjustFacade facade;
        List<RefundAdjustVM> listVM;
        RefundAdjustQueryFilter filter;
        List<ValidationEntity> SoSysNoList;
        List<ValidationEntity> SysNoList;

        private int sysNo = 0;
        public int? SysNo
        {
            get
            {
                if (this.Request != null)
                {
                    return (int.TryParse(this.Request.Param, out sysNo) ? sysNo : 0);
                }
                return null;
            }
            set { }
        }

        public bool IsUrl { get; set; }

        public RefundAdjustQuery()
        {
            InitializeComponent();
        }
        #endregion

        #region 页面加载事件
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            CheckRights();
            facade = new RefundAdjustFacade(this);
            this.DataContext = filter = new RefundAdjustQueryFilter();
            IsUrl = false;
            BindComboxData();
            if (SysNo != null && SysNo != 0)
            {
                filter.SysNo = SysNo.ToString();
                this.txtSysNo.Text = filter.SysNo;
                IsUrl = true;
                DataGrid_ResultList.Bind();
            }
        }

        /// <summary>
        /// 绑定下拉框数据
        /// </summary>
        private void BindComboxData()
        {
            //处理状态
            this.cbxStatus.ItemsSource = EnumConverter.GetKeyValuePairs<RefundAdjustStatus>(EnumConverter.EnumAppendItemType.All);
            this.cbxStatus.SelectedIndex = 0;

            //退款方式（银行转账，网关直接退款，退入余额账户）
            List<KeyValuePair<RefundPayType?, string>> list = EnumConverter.GetKeyValuePairs<RefundPayType>(EnumConverter.EnumAppendItemType.All);
            List<KeyValuePair<RefundPayType?, string>> newList = new List<KeyValuePair<RefundPayType?, string>>();
            foreach (var item in list)
            {
                if (item.Key == RefundPayType.BankRefund
                    || item.Key == RefundPayType.NetWorkRefund
                    || item.Key == RefundPayType.PrepayRefund
                    || item.Key == null)
                    newList.Add(item);
            }
            this.cbxRefundType.ItemsSource = newList;
            this.cbxRefundType.SelectedIndex = 0;

            //补偿类型
            this.cbxAdjustType.ItemsSource = EnumConverter.GetKeyValuePairs<RefundAdjustType>(EnumConverter.EnumAppendItemType.All);
            this.cbxAdjustType.SelectedIndex = 0;
        }
        #endregion

        #region 查询相关
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SoSysNoList = new List<ValidationEntity>();
            SysNoList = new List<ValidationEntity>();
            SysNoList.Add(new ValidationEntity(ValidationEnum.IsInteger, null, ResRefundAdjust.Msg_IsInteger));
            SoSysNoList.Add(new ValidationEntity(ValidationEnum.IsInteger, null, ResRefundAdjust.Msg_IsInteger));
            if (!ValidationHelper.Validation(this.txtSoNum, SoSysNoList)) return;
            if (!ValidationHelper.Validation(this.txtSysNo, SysNoList)) return;
            this.DataGrid_ResultList.Bind();
        }

        private void DataGrid_ResultList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                SortBy = e.SortField,
                PageSize = e.PageSize
            };
            if (filter.AdjustType != RefundAdjustType.EnergySubsidy)
            {
                facade.QueryRefundAdjust(filter, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    listVM = DynamicConverter<RefundAdjustVM>.ConvertToVMList(args.Result.Rows);
                    this.DataGrid_ResultList.TotalCount = args.Result.TotalCount;
                    this.DataGrid_ResultList.ItemsSource = listVM;
                    if (SysNo != 0 && IsUrl == true && this.DataGrid_ResultList.ItemsSource != null)
                    {
                        IsUrl = false;
                        DataGrid_ResultList.SelectedIndex = 0;
                        hlbtnView_Click(null, null);
                    }
                });
            }
            else
            {
                facade.QueryEnergySubsidy(filter, (obj, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        listVM = DynamicConverter<RefundAdjustVM>.ConvertToVMList(args.Result.Rows);
                        this.DataGrid_ResultList.TotalCount = args.Result.TotalCount;
                        this.DataGrid_ResultList.ItemsSource = listVM;
                        if (SysNo != 0 && IsUrl == true && this.DataGrid_ResultList.ItemsSource != null)
                        {
                            IsUrl = false;
                            DataGrid_ResultList.SelectedIndex = 0;
                            hlbtnView_Click(null, null);
                        }
                    });
            }
        }
        #endregion

        #region 按钮事件
        //新建
        private void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            RefundAdjustVM vm = this.DataGrid_ResultList.SelectedItem as RefundAdjustVM;
            UCRefundAdjustMaintain uctl = new UCRefundAdjustMaintain(vm, "Create");
            uctl.Dialog = Window.ShowDialog(ResRefundAdjust.Dialog_RefundAdjustCreate, uctl, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid_ResultList.Bind();
                }
            });
        }

        //private bool ValidateSelect()
        //{
        //    bool result = true;
        //    if (listVM == null)
        //    {
        //        result = false;
        //    }
        //    else
        //    {
        //        var selectedList = listVM.Where(item => item.IsChecked).ToList();
        //        if (selectedList.Count == 0)
        //        {
        //            result = false;
        //        }
        //    }
        //    return result;
        //}

        //提交审核
        private void Button_Aduit_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_ResultList.SelectedItem == null)
            {
                Window.Alert(ResRefundAdjust.Msg_SelectData);
                return;
            }
            this.Button_Aduit.IsEnabled = false;
            RefundAdjustVM vm = this.DataGrid_ResultList.SelectedItem as RefundAdjustVM;
            var entity = vm.ConvertVM<RefundAdjustVM, RefundAdjustInfo>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.SysNo = vm.AdjustSysNo;
            entity.Action = "Audit";
            facade.UpdateRefundAdjustStatus(entity, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Window.Alert(ResRefundAdjust.Info_UpdateSuccess);
                    this.DataGrid_ResultList.Bind();
                }
                this.Button_Aduit.IsEnabled = true;
            });
        }

        //作废
        private void Button_Void_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_ResultList.SelectedItem == null)
            {
                Window.Alert(ResRefundAdjust.Msg_SelectData);
                return;
            }
            this.Button_Void.IsEnabled = false;
            RefundAdjustVM vm = this.DataGrid_ResultList.SelectedItem as RefundAdjustVM;
            RefundAdjustInfo entity = new RefundAdjustInfo()
            {
                SysNo = vm.AdjustSysNo,
                Action = "Void",
                Status = vm.Status

            };
            facade.UpdateRefundAdjustStatus(entity, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Window.Alert(ResRefundAdjust.Info_UpdateSuccess);
                    this.DataGrid_ResultList.Bind();
                }
                this.Button_Void.IsEnabled = true;
            });
        }

        //退款
        private void Button_Refund_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid_ResultList.SelectedItem == null)
            {
                Window.Alert(ResRefundAdjust.Msg_SelectData);
                return;
            }
            this.Button_Refund.IsEnabled = false;
            RefundAdjustVM vm = this.DataGrid_ResultList.SelectedItem as RefundAdjustVM;
            RefundAdjustInfo entity = new RefundAdjustInfo()
            {
                SysNo = vm.AdjustSysNo,
                Action = "Refund",
                Status = vm.Status,
                RefundUserSysNo = CPApplication.Current.LoginUser.UserSysNo,
                CashAmt = vm.CashAmt,
                CompanyCode = CPApplication.Current.CompanyCode,
                CustomerSysNo = vm.CustomerSysNo,
                SOSysNo = int.Parse(vm.SOSysNo),
                CreateUserSysNo = vm.CreateUserSysNo,
                RefundPayType = vm.RefundPayType
            };
            facade.UpdateRefundAdjustStatus(entity, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Window.Alert(ResRefundAdjust.Info_UpdateSuccess);
                    this.DataGrid_ResultList.Bind();
                }
                this.Button_Refund.IsEnabled = true;
            });
        }

        /// <summary>
        /// 响应回车查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                var txtBinding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (txtBinding != null)
                {
                    txtBinding.UpdateSource();
                }
            }
        }

        /// <summary>
        /// 选择全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox ckb = sender as CheckBox;
        //    if (ckb != null)
        //    {
        //        List<RefundAdjustVM> viewList = this.DataGrid_ResultList.ItemsSource as List<RefundAdjustVM>;
        //        if (viewList != null && viewList.Count != 0)
        //        {
        //            foreach (var view in viewList)
        //            {
        //                view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
        //            }
        //        }
        //    }
        //}

        #endregion

        #region 查询结果操作
        //查看
        private void hlbtnView_Click(object sender, RoutedEventArgs e)
        {
            RefundAdjustVM vm = this.DataGrid_ResultList.SelectedItem as RefundAdjustVM;
            UCRefundAdjustMaintain uctl = new UCRefundAdjustMaintain(vm, "View");
            uctl.Dialog = Window.ShowDialog(ResRefundAdjust.Dialog_RefundAdjustView, uctl, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid_ResultList.Bind();
                }
            });
        }

        //编辑
        private void hlbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_RefundAdjust_Edit))
            {
                Window.Alert(ResRefundAdjust.Msg_HasNoEditRignts);
                return;
            }
            RefundAdjustVM vm = this.DataGrid_ResultList.SelectedItem as RefundAdjustVM;
            UCRefundAdjustMaintain uctl = new UCRefundAdjustMaintain(vm, "Edit");
            uctl.Dialog = Window.ShowDialog(ResRefundAdjust.Dialog_RefundAdjustEdit, uctl, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid_ResultList.Bind();
                }
            });
        }

        //订单链接
        private void hlbtnSoSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.SOMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }
        #endregion

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_RefundAdjust_Add))
                this.Button_Create.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_RefundAdjust_Audit))
                this.Button_Aduit.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_RefundAdjust_Void))
                this.Button_Void.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_RefundAdjust_Refund))
                this.Button_Refund.IsEnabled = false;
        }
        #endregion

        private void cbxAdjustType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filter.AdjustType == RefundAdjustType.EnergySubsidy)
            {
                this.txtRequestID.IsEnabled = false;
                this.Button_Export.Visibility = AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_EnergySubsidy_Export) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                this.spEnergySubsidyCondition.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.txtRequestID.IsEnabled = true;
                this.Button_Export.Visibility = System.Windows.Visibility.Collapsed;
                this.DataGrid_ResultList.IsShowAllExcelExporter = false;
                this.spEnergySubsidyCondition.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 节能补贴导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_Export.Bind();
        }

        /// <summary>
        /// 导出控件绑定  隐藏 只用作导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_Export_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.EnergySubsidyExport(filter, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (args.Result != null)
                    {
                        this.DataGrid_Export.ItemsSource = DynamicConverter<EnergySubsidyVM>.ConvertToVMList(args.Result.Rows);
                        //仅用于导出，并不代表实际的Count值
                        this.DataGrid_Export.TotalCount = args.Result.TotalCount;

                        ColumnSet col = new ColumnSet(DataGrid_Export);

                        facade.ExportEnergySubsidy(filter, new ColumnSet[] { col });
                    }
                });
        }
    }
}
