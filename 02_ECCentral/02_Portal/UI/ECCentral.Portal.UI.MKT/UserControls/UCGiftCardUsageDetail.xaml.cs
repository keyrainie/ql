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
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCGiftCardUsageDetail : UserControl
    {
        public IDialog Dialog { get; set; }
        private GiftCardFacade facade;
        public GiftCardVM VM { get; set; }
        private DateTime? oldEndDate;
        private ECCentral.BizEntity.Customer.CustomerInfo customerInfo;

        public UCGiftCardUsageDetail()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(UCGiftCardUsageDetail_Loaded);
        }

        void UCGiftCardUsageDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCGiftCardUsageDetail_Loaded);
            facade = new GiftCardFacade(CPApplication.Current.CurrentPage);

            comGiftCardStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.GiftCardStatus>();
            comGiftCardCategory.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.GiftCardType>();
            customerInfo = new BizEntity.Customer.CustomerInfo();

            this.btnSave.Visibility = System.Windows.Visibility.Collapsed;
            this.btnVoid.Visibility = System.Windows.Visibility.Collapsed;

            if (VM != null)
            {
                VM.ChannelID = "1";
                oldEndDate = VM.EndDate;
                BaseInfo.DataContext = VM;

                customerInfo.BasicInfo.CustomerID = VM.CustomerID;
                customerInfo.BasicInfo.CustomerSysNo = int.Parse(string.IsNullOrEmpty(VM.CustomerSysNo)?"0":VM.CustomerSysNo);
                facade.GetGiftCardOperateLogByCode(VM.CardCode, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    List<ECCentral.BizEntity.IM.GiftCardOperateLog> logs = args.Result;
                    if (logs != null && logs.Count > 0)
                    {
                        StringBuilder str = new StringBuilder();
                        logs = logs.OrderByDescending(p => p.InDate).ToList();
                        foreach (ECCentral.BizEntity.IM.GiftCardOperateLog log in logs)
                        {
                            str.AppendFormat("{0} {1} by {2} {3}", log.InDate.Value.ToString("yyyy-MM-dd HH:mm"), log.ActionType.ToDescription(), log.InUser, log.Memo);
                            str.AppendLine();
                        }
                        tbLog.Text = str.ToString().TrimEnd();
                    }
                });
                facade.GetGiftCardRedeemLogJoinSOMaster(VM.CardCode, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    DataGrid.ItemsSource = args.Result;
                });

                if ((VM.Status == GiftCardStatus.Valid||VM.Status == GiftCardStatus.ManualActive) && VM.AvailAmount.HasValue && VM.AvailAmount > 0)
                {
                    this.btnSave.Visibility = System.Windows.Visibility.Visible;
                    //this.btnVoid.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.dpEndDate.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (oldEndDate == VM.EndDate)
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_NoValueForUpdate, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Confirm(ResGiftCardInfo.Information_NeedForUpdateAgain, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        GiftCardInfo item = VM.ConvertVM<GiftCardVM, GiftCardInfo>();
                        item.Customer = customerInfo;
                        facade.UpdateGiftCardInfo(item, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                                return;

                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);

                            CloseDialog(DialogResultType.OK);
                        });
                    }
                });
            }
        }


        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }


        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {


            GiftCardInfo item = VM.ConvertVM<GiftCardVM, GiftCardInfo>();
            facade.SetGiftCardInvalid(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                CloseDialog(DialogResultType.OK);
            });
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}
