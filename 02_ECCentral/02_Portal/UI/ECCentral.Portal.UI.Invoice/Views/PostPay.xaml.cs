using System;
using System.Linq;
using System.Threading;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 核对银行电汇-邮局汇款
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class PostPay : PageBase
    {
        #region Properties

        private PostPayVM postpayVM;
        private PostPayFacade facade;

        #endregion Properties

        public PostPay()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PostPay_Loaded);
            ucPostPayInfo.SearchOrderButtonClick += new RoutedEventHandler(ucPostPayInfo_SearchOrderButtonClick);
            ucPostPayInfo.HideRelatedSOSysNo();
        }

        private void ucPostPayInfo_SearchOrderButtonClick(object sender, RoutedEventArgs e)
        {
            var payInfo = ucPostPayInfo.DataContext as PayInfoVM;
            if (payInfo != null && !string.IsNullOrEmpty(payInfo.SOSysNo))
            {
                btnSave.IsEnabled = false;
                int soSysNo;
                if (int.TryParse(payInfo.SOSysNo, out soSysNo))
                {
                    facade.LoadForEdit(soSysNo, result =>
                    {
                        postpayVM = result;
                        this.LayoutRoot.DataContext = postpayVM;

                        var confirmedOrder = result.ConfirmedOrderList.FirstOrDefault(s => s.SOSysNo == soSysNo);
                        btnSave.IsEnabled = (confirmedOrder.ConfirmStatus == PostIncomeConfirmStatus.Related);

                        //计算收款单剩余金额并设置提示信息
                        //tblReminAmtPrompMessage.Text = string.Format("提示：收款单剩余金额为{0}元，请注意！",
                        //    ConstValue.Invoice_ToCurrencyString(result.RemainAmt));
                        tblReminAmtPrompMessage.Text = string.Format(ResPostPay.Msg_BillAmountIsZero,
                            ConstValue.Invoice_ToCurrencyString(result.RemainAmt));

                        postpayVM.PayInfo.ValidationErrors.Clear();
                    });
                }
            }
        }

        private void PostPay_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(PostPay_Loaded);

            facade = new PostPayFacade(this);
            btnSave.IsEnabled = false;

            NewPostPayDataContext();
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            if (PayInfoVM.PostPayTypeList == null)
            {
                facade.LoadPayTypeList(payTypeList =>
                {
                    payTypeList.Insert(0, new BizEntity.Common.PayType
                    {
                        PayTypeName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_Select
                    });
                    PayInfoVM.PostPayTypeList = payTypeList;
                    ucPostPayInfo.cbmPayType.ItemsSource = payTypeList;
                    ucPostPayInfo.cbmPayType.SelectedIndex = 0;
                });
            }
            else
            {
                ucPostPayInfo.cbmPayType.ItemsSource = PayInfoVM.PostPayTypeList;
                ucPostPayInfo.cbmPayType.SelectedIndex = 0;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
            {
                return;
            }

            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PostPay_Create))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            //进行退款备注截断
            postpayVM.RefundInfo.RefundMemo = postpayVM.RefundInfo.RefundMemo.Cut(0, 200);
            facade.Create(postpayVM, () =>
            {
                Window.Alert(ResPostPay.Msg_SaveSuccess);
            });
        }

        private void NewPostPayDataContext()
        {
            postpayVM = new PostPayVM();
            this.LayoutRoot.DataContext = postpayVM;
        }

        private bool ValidateData()
        {
            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;

            flag1 = ValidationManager.Validate(this.ucPostPayInfo);
            bool expanded = false;
            if (expRefundInfo.IsExpanded)
            {
                expanded = true;
                flag2 = ValidationManager.Validate(this.ucRefundInfo);
                if (flag1 && flag2)
                {
                    flag3 = ValidateToleranceAmt();
                }
            }
            expRefundInfo.IsExpanded = expanded;
            return flag1 && flag2 && flag3;
        }

        private bool ValidateToleranceAmt()
        {
            decimal payAmt = decimal.Round(Convert.ToDecimal(postpayVM.PayInfo.PayAmt), 2);
            decimal receivableAmt = decimal.Round(Convert.ToDecimal(postpayVM.PayInfo.ReceivableAmt), 2);
            decimal refundCashAmt = decimal.Round(Convert.ToDecimal(postpayVM.RefundInfo.RefundCashAmt), 2);

            decimal toleranceAmt = payAmt - receivableAmt - refundCashAmt;
            postpayVM.RefundInfo.ToleranceAmt = Math.Abs(toleranceAmt);

            if (Math.Abs(toleranceAmt) > 0.1M)
            {
                //Window.Alert("精度冗余绝对值大于0.1，请检查返还金额和点数数值是否正确。");
                Window.Alert(ResPostPay.Msg_DataFormatError);
                return false;
            }
            return true;
        }
    }
}