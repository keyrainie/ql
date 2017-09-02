using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCRefundBalanceMaintain : UserControl
    {
        public IDialog Dialog { get; set; }

        private string Action { get; set; }
        private int RefundSysNo { get; set; }
        private int RefundBalanceSysNo { get; set; }

        RefundBalanceFacade facade;
        private RefundBalanceMaintainVM MaintainVM;

        private List<ValidationEntity> ValidationAmount;
        private List<ValidationEntity> ValidationEmpt;

        public UCRefundBalanceMaintain(int refundSysNo, int refundBalanceSysNo, string action)
        {
            this.RefundSysNo = refundSysNo;
            this.RefundBalanceSysNo = refundBalanceSysNo;
            this.Action = action;
            MaintainVM = new RefundBalanceMaintainVM();
            facade = new RefundBalanceFacade(CPApplication.Current.CurrentPage);
            InitializeComponent();
            InitControls();
            BuildValidate();
        }

        private void BuildValidate()
        {
            ValidationAmount = new List<ValidationEntity>();
            ValidationAmount.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRefundBalance.Msg_EmptyBalanceAmt));
            ValidationAmount.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^(((([1-9]+\d*)|([0-9]{1}))$)|(([\-]?([1-9]\d*)|(0))(\.\d{1,2}){0,1}$)|(\-0\.\d{1,2}))", ResRefundBalance.Msg_IsNotMoney));
            ValidationEmpt = new List<ValidationEntity>();
            ValidationEmpt.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRefundBalance.Msg_EmptyFiled));
        }
        private void InitControls()
        {
            facade.LoadRefundItemListRefundSysNo(RefundSysNo, (obj, args) =>
            {
                List<RefundItemInfo> result = args.Result;
                result.Add(new RefundItemInfo()
                    {
                        ProductSysNo = 0,
                        ProductID = ResRefundBalance.RefundBalanceType_Other
                    });
                Combox_BalanceType.ItemsSource = result.Select(p => new KeyValuePair<int, string>(p.ProductSysNo.Value, p.ProductID)).Distinct().ToList();
            });
            #region 根据Action 设置ViewModel
            if (Action == "Create")
            {
                facade.LoadNewRefundBalanceByRefundSysNo(RefundSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        Dialog.Close();
                        return;
                    }
                    MaintainVM = args.Result.Convert<RefundBalanceInfo, RefundBalanceMaintainVM>();
                    MaintainVM.CustomerID = args.Result.CustomerID;
                    if (args.Result.IncomeBankInfo != null)
                    {
                        MaintainVM.BankName = args.Result.IncomeBankInfo.BankName;
                        MaintainVM.BranchBankName = args.Result.IncomeBankInfo.BranchBankName;
                        MaintainVM.CardNo = args.Result.IncomeBankInfo.CardNumber;
                        MaintainVM.CardOwnerName = args.Result.IncomeBankInfo.CardOwnerName;
                        MaintainVM.PostAddress = args.Result.IncomeBankInfo.PostAddress;
                        MaintainVM.PostCode = args.Result.IncomeBankInfo.PostCode;
                        MaintainVM.ReceiverName = args.Result.IncomeBankInfo.ReceiverName;
                        MaintainVM.IncomeNote = args.Result.IncomeBankInfo.Note;
                    }
                    this.DataContext = MaintainVM;
                    Combox_BalanceType.SelectedIndex = 0;
                    Combox_RefundPayType.IsEnabled = true;
                    Combox_BalanceType.IsEnabled = true;
                    Text_CashAmount.IsEnabled = true;
                    TextBox_Note.IsEnabled = true;
                    MaintainVM.ValidationErrors.Clear();

                });
            }
            else
            {
                facade.LoadRefundBalanceBySysNo(RefundBalanceSysNo, (obj, args) =>
                {
                    MaintainVM = args.Result.Convert<RefundBalanceInfo, RefundBalanceMaintainVM>();
                    MaintainVM.CustomerID = args.Result.CustomerID;
                    if (args.Result.IncomeBankInfo != null)
                    {
                        MaintainVM.BankName = args.Result.IncomeBankInfo.BankName;
                        MaintainVM.BranchBankName = args.Result.IncomeBankInfo.BranchBankName;
                        MaintainVM.CardNo = args.Result.IncomeBankInfo.CardNumber;
                        MaintainVM.CardOwnerName = args.Result.IncomeBankInfo.CardOwnerName;
                        MaintainVM.PostAddress = args.Result.IncomeBankInfo.PostAddress;
                        MaintainVM.PostCode = args.Result.IncomeBankInfo.PostCode;
                        MaintainVM.ReceiverName = args.Result.IncomeBankInfo.ReceiverName;
                        MaintainVM.IncomeNote = args.Result.IncomeBankInfo.Note;
                    }
                    this.DataContext = MaintainVM;

                    if (Action == "Aduit")
                    {
                        if (MaintainVM.RefundPayType == RefundPayType.NetWorkRefund)
                        {
                            TextBox_BankName.IsEnabled = true;
                        }
                        else if (MaintainVM.RefundPayType == RefundPayType.PostRefund)
                        {
                            Text_PostAddress.IsEnabled = Text_PostCode.IsEnabled
                                = Text_ReceiverName.IsEnabled = true;
                        }
                        else if (MaintainVM.RefundPayType == RefundPayType.BankRefund)
                        {
                            TextBox_BankName.IsEnabled = Text_BranchBankName.IsEnabled
                                = Text_CardNo.IsEnabled = Text_CardOwnerName.IsEnabled = true;
                        }
                        TextBox_IncomeNote.IsEnabled = true;

                    }
                    MaintainVM.ValidationErrors.Clear();


                });
            }
            #endregion
            SetControlStatus();

        }

        private void SetControlStatus()
        {
            #region 根据Action 设置按钮样式
            switch (Action)
            {
                case "Create":
                    Button_Action.Content = ResRefundBalance.Button_Save;
                    Button_Action.CommandParameter = "Create";
                    Button_Action.Tag = @"/Themes/Default/Icons/Button/save.png";
                    break;
                case "Aduit":
                    Button_Action.Content = ResRefundBalance.Button_Aduit;
                    Button_Action.CommandParameter = "Aduit";
                    Button_Action.Tag = @"/Themes/Default/Icons/Button/post.png";

                    break;
                case "Refund":
                    Button_Action.Content = ResRefundBalance.Button_Refund;
                    Button_Action.CommandParameter = "Refund";
                    Button_Action.Tag = @"/Themes/Default/Icons/Button/cancel-rush-order.png";

                    break;
                case "Void":
                    Button_Action.Content = ResRefundBalance.Button_Void;
                    Button_Action.CommandParameter = "Void";
                    Button_Action.Tag = @"/Themes/Default/Icons/Button/exit.png";

                    break;
                case "View":
                    Button_Action.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
            #endregion

        }

        private void Button_Action_Click(object sender, RoutedEventArgs e)
        {
            MaintainVM.ValidationErrors.Clear();
            string action = this.Button_Action.CommandParameter.ToString();
            RefundBalanceInfo entity = MaintainVM.ConvertVM<RefundBalanceMaintainVM, RefundBalanceInfo>();
            entity.IncomeBankInfo.BankName = MaintainVM.BankName;
            entity.IncomeBankInfo.BranchBankName = MaintainVM.BranchBankName;
            entity.IncomeBankInfo.CardNumber = MaintainVM.CardNo;
            entity.IncomeBankInfo.CardOwnerName = MaintainVM.CardOwnerName;
            entity.IncomeBankInfo.PostAddress = MaintainVM.PostAddress;
            entity.IncomeBankInfo.PostCode = MaintainVM.PostCode;
            entity.IncomeBankInfo.ReceiverName = MaintainVM.ReceiverName;
            entity.IncomeBankInfo.Note = MaintainVM.IncomeNote;
            entity.ProductID = ((KeyValuePair<int, string>)(((System.Windows.Controls.ComboBox)(Combox_BalanceType)).SelectionBoxItem)).Value;
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            switch (action)
            {
                #region Create
                case "Create":
                    if (!ValidationHelper.Validation(this.Text_CashAmount, ValidationAmount) ||
                        !ValidationHelper.Validation(this.TextBox_Note, ValidationEmpt))
                    {
                        return;
                    }
                    entity.CashAmt = Convert.ToDecimal(Text_CashAmount.Text.Trim());
                    facade.Create(entity, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        else
                        {
                            if (Dialog != null)
                            {
                                Dialog.ResultArgs.Data = args;
                                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                Dialog.Close();
                            }
                        }
                    });
                    break;
                #endregion
                #region Aduit
                case "Aduit":
                    if (ValidationManager.Validate(this.LayoutRoot))
                    {
                        facade.SubmitAudit(entity, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            else
                            {
                                if (Dialog != null)
                                {
                                    Dialog.ResultArgs.Data = args;
                                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                    Dialog.Close();
                                }
                            }
                        });
                    }
                    break;
                #endregion
                #region Refund

                case "Refund":
                    facade.Refund(entity, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        else
                        {
                            if (Dialog != null)
                            {
                                Dialog.ResultArgs.Data = args;
                                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                Dialog.Close();
                            }
                        }
                    });
                    break;
                #endregion
                #region Void
                case "Void":
                    facade.Abandon(entity.SysNo.Value, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        else
                        {
                            if (Dialog != null)
                            {
                                Dialog.ResultArgs.Data = args;
                                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                Dialog.Close();
                            }
                        }
                    });
                    break;
                #endregion
            }
        }


        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.Close(true);

        }
    }
}
