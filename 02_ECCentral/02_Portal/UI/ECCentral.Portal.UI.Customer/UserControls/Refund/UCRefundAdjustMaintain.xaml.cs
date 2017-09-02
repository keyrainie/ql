using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.UI.Customer.UserControls.Refund;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class UCRefundAdjustMaintain : UserControl
    {
        #region 页面初始化
        public IDialog Dialog { get; set; }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private int RefundSysNo { get; set; }
        private int RefundAdjustSysNo { get; set; }

        RefundAdjustFacade facade;
        private RefundAdjustMaintainVM maintainVM;
        private RefundAdjustVM refundVM;
        List<ValidationEntity> SoSysNoList;

        List<ValidationEntity> validationList;

        List<ValidationEntity> cashAmtList;

        public RefundAdjustVM RefundVM
        {
            get { return refundVM; }
            set { refundVM = value; }
        }

        public UCRefundAdjustMaintain()
        {
            InitializeComponent();
        }

        public UCRefundAdjustMaintain(RefundAdjustVM vm, string action)
        {
            InitializeComponent();
            maintainVM = new RefundAdjustMaintainVM();
            if (vm != null)
            {
                this.RefundVM = vm;
                maintainVM.AdjustSysNo = RefundVM.AdjustSysNo;
                maintainVM.AdjustOrderType = RefundVM.AdjustOrderType;
            }
            maintainVM.Action = action;
            this.Loaded += new RoutedEventHandler(UCRefundAdjustMaintain_Loaded);
        }
        #endregion

        #region 页面加载事件
        void UCRefundAdjustMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new RefundAdjustFacade(CPApplication.Current.CurrentPage);
            BindComboxData();
            switch (maintainVM.Action)
            {
                case "Create":
                    {
                        this.spVisbale.Visibility = System.Windows.Visibility.Collapsed;
                        maintainVM.SOSysNo = maintainVM.Note = maintainVM.CashAmt = string.Empty;
                        maintainVM.AdjustOrderType = RefundAdjustType.ShippingAdjust;
                        maintainVM.RefundPayType = RefundPayType.PrepayRefund;
                    }
                    break;
                default:
                    {
                        this.spVisbale.Visibility = System.Windows.Visibility.Visible;
                        maintainVM.CashAmt = this.Text_CashAmount.Text;
                        maintainVM.RequestID = this.RefundVM.RequestID;
                        maintainVM.RefundPayType = this.RefundVM.RefundPayType;
                        maintainVM.SOSysNo = this.RefundVM.SOSysNo.ToString();
                        maintainVM.CustomerID = this.RefundVM.CustomerID;
                        maintainVM.CashAmt = this.RefundVM.CashAmt.ToString();
                        maintainVM.Note = this.RefundVM.Note;
                        maintainVM.Status = this.RefundVM.Status;

                        //银行信息
                        maintainVM.BankName = this.RefundVM.BankName;
                        maintainVM.BranchBankName = this.RefundVM.BranchBankName;
                        maintainVM.CardNo = this.RefundVM.CardNumber;
                        maintainVM.CardOwnerName = this.RefundVM.CardOwnerName;
                        break;
                    }
            }
            this.DataContext = maintainVM;
            SwitchControlEnable(maintainVM.Action);

        }

        private void SwitchControlEnable(string Action)
        {
            switch (Action)
            {
                case "Create":
                    break;
                default:
                    {
                        this.txtSoSysNo.IsEnabled = false;
                        this.Combox_RefundPayType.IsEnabled = false;
                        this.Combox_AdjustType.IsEnabled = false;
                        this.Text_CashAmount.IsEnabled = false;
                        this.TextBox_Note.IsEnabled = false;
                        this.gridBankInfo.IsHitTestVisible = false;
                    }
                    break;
            }
        }

        private void BindComboxData()
        {
            //补偿类型
            this.Combox_AdjustType.ItemsSource = EnumConverter.GetKeyValuePairs<RefundAdjustType>();
            this.Combox_AdjustType.SelectedIndex = 0;

            //退款方式（银行转账，网关直接退款，退入余额账户）
            List<KeyValuePair<RefundPayType?, string>> list = EnumConverter.GetKeyValuePairs<RefundPayType>();
            List<KeyValuePair<RefundPayType?, string>> newList = new List<KeyValuePair<RefundPayType?, string>>();
            foreach (var item in list)
            {
                if (item.Key == RefundPayType.BankRefund
                    || item.Key == RefundPayType.NetWorkRefund
                    || item.Key == RefundPayType.PrepayRefund
                    || item.Key == null)
                    newList.Add(item);
            }
            this.Combox_RefundPayType.ItemsSource = newList;
            //默认退入余额账户
            this.Combox_RefundPayType.SelectedIndex = 2;

            //处理状态
            this.cbxStatus.ItemsSource = EnumConverter.GetKeyValuePairs<RefundAdjustStatus>();
            this.cbxStatus.SelectedIndex = 0;
        }

        #endregion

        #region 按钮事件
        private void Button_Action_Click(object sender, RoutedEventArgs e)
        {
            this.Button_Aduit.IsEnabled = this.Button_Void.IsEnabled = false;
            Button btn = sender as Button;
            if (maintainVM.HasValidationErrors)
                return;
            var entity = maintainVM.ConvertVM<RefundAdjustMaintainVM, RefundAdjustInfo>();
            entity.CardNumber = maintainVM.CardNo;
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            entity.RequestSysNo = maintainVM.RequestSysNo;
            entity.CustomerSysNo = RefundVM.CustomerSysNo;
            entity.CashAmt = Convert.ToDecimal(maintainVM.CashAmountShow);
            entity.SysNo = maintainVM.AdjustSysNo.Value;
            entity.Action = btn.CommandParameter.ToString();
            entity.Status = maintainVM.Status;
            entity.RefundUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            facade.UpdateRefundAdjustStatus(entity, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (args.Result)
                    {
                        CurrentWindow.Alert(ResRefundAdjust.Info_UpdateSuccess);
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    }
                    this.Button_Aduit.IsEnabled = this.Button_Void.IsEnabled = true;
                });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!CreatePreCheck())
                return;
            var entity = maintainVM.ConvertVM<RefundAdjustMaintainVM, RefundAdjustInfo>();
            entity.CardNumber = maintainVM.CardNo;
            entity.CustomerSysNo = maintainVM.CustomerSysNo;
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.CreateUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            entity.RequestSysNo = maintainVM.RequestSysNo;
            entity.Status = RefundAdjustStatus.Initial;
            entity.CashAmt = Convert.ToDecimal(maintainVM.CashAmountShow);
            //if (maintainVM.Action == "Create")
            //{
            //创建补偿退款单
            facade.CreateRefundAdjust(entity, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                CurrentWindow.Alert(ResRefundAdjust.Info_CreateSuccess);
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close(true);
            });
            //}
            //else if (maintainVM.Action == "Edit")
            //{
            //    //更新补偿退款单
            //    facade.UpdateRefundAdjust(entity, (obj, args) =>
            //    {
            //        if (args.FaultsHandle()) return;
            //        CurrentWindow.Alert(ResRefundAdjust.Info_UpdateSuccess);
            //        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            //        Dialog.Close(true);
            //    });
            //}
        }

        private bool CreatePreCheck()
        {
            //退款类型为【银行转账】时，银行信息不能为空
            if (maintainVM.RefundPayType == RefundPayType.BankRefund)
            {
                validationList = new List<ValidationEntity>();
                validationList.Add((new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRefundAdjust.Msg_EmptyFiled)));
                if (!ValidationHelper.Validation(this.TextBox_BankName, validationList)
                    || !ValidationHelper.Validation(this.Text_CardNo, validationList)
                    || !ValidationHelper.Validation(this.Text_BranchBankName, validationList)
                    || !ValidationHelper.Validation(this.Text_CardOwnerName, validationList))
                    return false;
            }
            if (maintainVM.RefundPayType == RefundPayType.NetWorkRefund)
            {
                validationList = new List<ValidationEntity>();
                validationList.Add((new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRefundAdjust.Msg_EmptyFiled)));
                if (!ValidationHelper.Validation(this.TextBox_BankName, validationList))
                {
                    return false;
                }
            }
            //\\^(0\.{1}[1-9]{1,2})|(([1-9]\d{0,11})(\.{1}\d{1,2})?)$
            cashAmtList = new List<ValidationEntity>();
            cashAmtList.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRefundAdjust.Msg_EmptyFiled));
            //cashAmtList.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^(0{1}|([1-9]\d{0,11}))(\.(\d){1,2})?$", ResRefundAdjust.Msg_IsNotMoney));
            if (!ValidationHelper.Validation(this.Text_CashAmount, cashAmtList))
                return false;
            if (maintainVM.HasValidationErrors)
                return false;
            return true;
        }

        /// <summary>
        /// 关闭按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            this.Dialog.Close();
        }
        #endregion

        #region 付款类型为银行转账时，需要填写相关的银行信息,为网关直接退款时，开户银行不能为空
        private void SwitchBankInfoIsEnable(RefundPayType type)
        {
            switch (type)
            {
                case RefundPayType.BankRefund:
                case RefundPayType.NetWorkRefund:
                    {
                        this.TextBox_BankName.IsEnabled = this.Text_BranchBankName.IsEnabled = this.Text_CardNo.IsEnabled =
                            this.Text_CardOwnerName.IsEnabled = true;
                        this.toolBankInfo.IsExpanded = true;
                        break;
                    }
                default:
                    {
                        this.TextBox_BankName.IsEnabled = this.Text_BranchBankName.IsEnabled = this.Text_CardNo.IsEnabled =
                            this.Text_CardOwnerName.IsEnabled = false;
                        this.toolBankInfo.IsExpanded = false;
                        break;
                    }
            }
        }

        private void Combox_RefundPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (maintainVM.RefundPayType != null)
                SwitchBankInfoIsEnable(maintainVM.RefundPayType.Value);
        }
        #endregion

        /// <summary>
        /// 根据退款单号加载相关信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtSoSysNo.Text))
            {
                SoSysNoList = new List<ValidationEntity>();
                SoSysNoList.Add(new ValidationEntity(ValidationEnum.IsInteger, null, ResRefundAdjust.Info_SoSysNoError));
                if (!ValidationHelper.Validation(this.txtSoSysNo, SoSysNoList)) return;
                maintainVM.SOSysNo = this.txtSoSysNo.Text;
                this.hlbtnViewDetials.Visibility = System.Windows.Visibility.Collapsed;
                this.Text_CashAmount.IsEnabled = true;
                facade.GetRefundAdjustBySoSysNo(maintainVM, (obj, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        if (args.Result != null)
                        {
                            if (maintainVM.AdjustOrderType == RefundAdjustType.EnergySubsidy)
                            {
                                this.hlbtnViewDetials.Visibility = System.Windows.Visibility.Visible;
                                this.Text_CashAmount.IsEnabled = false;
                            }
                            maintainVM.RequestID = args.Result.RequestID;
                            maintainVM.CustomerID = args.Result.CustomerID;
                            maintainVM.RequestSysNo = args.Result.RequestSysNo;
                            maintainVM.CustomerSysNo = args.Result.CustomerSysNo;
                            if (maintainVM.AdjustOrderType == RefundAdjustType.EnergySubsidy)
                            {
                                maintainVM.CashAmountShow = args.Result.CashAmt.ToString();
                            }
                        }
                        else
                        {
                            CurrentWindow.Alert(ResRefundAdjust.Info_RMARequestIDError);
                            maintainVM.SOSysNo = string.Empty;
                            maintainVM.RequestID = string.Empty;
                            maintainVM.CustomerID = string.Empty;
                            maintainVM.CashAmountShow = string.Empty;
                        }
                    });
            }
            else
            {
                this.hlbtnViewDetials.Visibility = System.Windows.Visibility.Collapsed;
                this.Text_CashAmount.IsEnabled = true;
                maintainVM.CashAmountShow = string.Empty;
            }
        }

        /// <summary>
        /// 查看节能补贴的详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtnViewDetials_Click(object sender, RoutedEventArgs e)
        {
            UCEnergySubsidyDetails uc = new UCEnergySubsidyDetails(int.Parse(maintainVM.SOSysNo), maintainVM.Action);
            uc.Dialog = this.CurrentWindow.ShowDialog(ResRefundAdjust.Dialog_EnergySubsidyDetails, uc, null, new Size(780, 380));
        }

        /// <summary>
        /// 当补偿类型为 【节能补贴】时，显【详细信息】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Combox_AdjustType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (maintainVM.AdjustOrderType == RefundAdjustType.EnergySubsidy)
            {
                this.Text_CashAmount.IsEnabled = false;
                maintainVM.RefundPayType = RefundPayType.BankRefund;
                this.Combox_RefundPayType.IsEnabled = false;
                if (!string.IsNullOrEmpty(this.txtSoSysNo.Text))
                {
                    this.hlbtnViewDetials.Visibility = System.Windows.Visibility.Visible;
                    TextBox_LostFocus(null, null);
                }
            }
            else
            {
                this.hlbtnViewDetials.Visibility = System.Windows.Visibility.Collapsed;
                if (maintainVM.Action == "Create")
                {
                    this.Text_CashAmount.IsEnabled = true;
                    maintainVM.RefundPayType = RefundPayType.PrepayRefund;
                    this.Combox_RefundPayType.IsEnabled = true;
                    maintainVM.CashAmountShow = string.Empty;
                    if (!string.IsNullOrEmpty(this.txtSoSysNo.Text))
                    {
                        TextBox_LostFocus(null, null);
                    }
                }
            }
        }
    }
}
