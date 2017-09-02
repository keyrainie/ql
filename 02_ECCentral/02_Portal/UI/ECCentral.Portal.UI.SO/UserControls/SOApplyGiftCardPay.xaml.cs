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
using ECCentral.Portal.UI.SO.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Facades;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOApplyGiftCardPay : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private List<GiftCardInfoVM> m_GiftCardInfoVMList;
        public List<GiftCardInfoVM> GiftCardInfoVMList
        {
            get
            {
                return m_GiftCardInfoVMList;
            }
            private set
            {
                m_GiftCardInfoVMList = value;
            }
        }

        private List<GiftCardInfoVM> m_SelectedGiftCardInfoVMList;
        public List<GiftCardInfoVM> SelectedGiftCardInfoVMList
        {
            get
            {
                return m_SelectedGiftCardInfoVMList;
            }
            private set
            {
                m_SelectedGiftCardInfoVMList = value;
            }
        }

        public SOApplyGiftCardPay()
        {
            InitializeComponent();
        }

        public SOApplyGiftCardPay(List<GiftCardInfoVM> inputList, int customerSysNo)
            : this()
        {
            #region 加载客户绑定的礼品卡列表
            new SOQueryFacade().QuerySOGiftCardsInfo(customerSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<GiftCardInfoVM> tempGiftCards = args.Result.Convert<GiftCardInfo, GiftCardInfoVM>();
                if (tempGiftCards != null && tempGiftCards.Count > 0)
                {
                    if (inputList != null && inputList.Count > 0)
                    {
                        foreach (var item in inputList)
                        {
                            item.IsSelected = true;
                        }
                        foreach (var item in tempGiftCards)
                        {
                            bool exists = false;
                            foreach (var innerItem in inputList)
                            {
                                if (item.Amount == innerItem.Amount
                                    && item.AvailAmount == innerItem.AvailAmount
                                    && item.TotalAmount == innerItem.TotalAmount)
                                {
                                    exists = true;
                                }
                            }
                            if (!exists)
                            {
                                inputList.Add(item);
                            }
                        }
                        GiftCardInfoVMList = inputList;
                    }
                    else
                    {
                        GiftCardInfoVMList = tempGiftCards;
                    }
                }
                else
                {
                    if (inputList != null && inputList.Count > 0)
                    {
                        foreach (var item in inputList)
                        {
                            item.IsSelected = true;
                        }
                        GiftCardInfoVMList = inputList;
                    }
                }
                dgridSOGiftCardInfo.ItemsSource = GiftCardInfoVMList;
            });
            #endregion

            Loaded += new RoutedEventHandler(SOApplyGiftCardPay_Loaded);
        }

        void SOApplyGiftCardPay_Loaded(object sender, RoutedEventArgs e)
        {
            RightControl();
        }

        void RightControl()
        {
            hlkb_SOGiftCard_AddGiftCard.Visibility = AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_UseUnBindingGiftCard) ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Event Handler

        /// <summary>
        /// 添加礼品卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_SOGiftCard_AddGiftCard_Click(object sender, RoutedEventArgs e)
        {
            string strCode = txtGiftCardNumber.Text;
            string strPassword = txtGiftCardPassword.Password;
            if (!string.IsNullOrEmpty(strCode) && !string.IsNullOrEmpty(strPassword))
            {
                new SOQueryFacade().QueryGiftCardByCodeAndPassword(strCode, strPassword, (obj, args) =>
               {
                   if (args.FaultsHandle())
                   {
                       return;
                   }
                   GiftCardInfo giftCardInfo = args.Result;
                   string errorInfo = string.Empty;
                   if (giftCardInfo != null && GiftCardInfoVMList != null && GiftCardInfoVMList.Count > 0)
                   {
                       foreach (var item in GiftCardInfoVMList)
                       {
                           if (item.CardCode == giftCardInfo.CardCode)
                           {
                               errorInfo = ResSOMaintain.Info_AddGiftCard_Code_Error;
                               CPApplication.Current.CurrentPage.Context.Window.Alert(errorInfo, MessageType.Error);
                               return;
                           }
                       }
                   }
                   if (giftCardInfo == null)
                   {
                       errorInfo = ResSOMaintain.Info_AddGiftCard_Error;
                   }
                   else if (giftCardInfo.Status != ECCentral.BizEntity.IM.GiftCardStatus.Valid)
                   {
                       errorInfo = ResSOMaintain.Info_AddGiftCard_Status_Error;
                   }
                   else if (giftCardInfo.BindingCustomer != null)
                   {
                       errorInfo = ResSOMaintain.Info_AddGiftCard_BindCustomerSysNo_Error;
                   }
                   else if (giftCardInfo.AvailAmount == 0)
                   {
                       errorInfo = ResSOMaintain.Info_AddGiftCard_AvailableAmount_Error;
                   }
                   else if (giftCardInfo.EndDate < DateTime.Now)
                   {
                       errorInfo = ResSOMaintain.Info_AddGiftCard_Expired_Error;
                   }
                   else if (giftCardInfo.BeginDate > DateTime.Now)
                   {
                       errorInfo = ResSOMaintain.Info_AddGiftCard_LessBeginDate_Error;
                   }
                   if (string.IsNullOrEmpty(errorInfo))
                   {
                       if (GiftCardInfoVMList == null)
                       {
                           GiftCardInfoVMList = new List<GiftCardInfoVM>();
                       }
                       else
                       {
                           GiftCardInfoVMList.Add(giftCardInfo.Convert<GiftCardInfo, GiftCardInfoVM>());
                           dgridSOGiftCardInfo.ItemsSource = GiftCardInfoVMList;
                       }
                   }
                   else
                   {
                       CPApplication.Current.CurrentPage.Context.Window.Alert(errorInfo, MessageType.Error);
                   }
               });
            }
        }

        /// <summary>
        /// 使用礼品卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SOGiftCardInfo_Apply_Click(object sender, RoutedEventArgs e)
        {
            GiftCardInfoVMList = dgridSOGiftCardInfo.ItemsSource as List<GiftCardInfoVM>;
            if (GiftCardInfoVMList != null && GiftCardInfoVMList.Count > 0)
            {
                foreach (var item in GiftCardInfoVMList)
                {
                    if (item.IsSelected.HasValue && item.IsSelected.Value)
                    {
                        if (SelectedGiftCardInfoVMList == null)
                        {
                            SelectedGiftCardInfoVMList = new List<GiftCardInfoVM>();
                        }
                        SelectedGiftCardInfoVMList.Add(item);
                    }
                }
            }
            //由父窗口执行真正的保存操作     
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.OK,
                Data = SelectedGiftCardInfoVMList
            });
        }

        #endregion Event Handler

        #region Helper Methods

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        #endregion Helper Methods

    }
}
