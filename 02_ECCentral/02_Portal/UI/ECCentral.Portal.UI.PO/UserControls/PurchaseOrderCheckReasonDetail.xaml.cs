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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderCheckReasonDetail : UserControl
    {
        private PurchaseOrderFacade serviceFacade;
        private PurchaseOrderInfoVM infoVM;
        private bool isEdit;

        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public PurchaseOrderCheckReasonDetail(PurchaseOrderInfoVM vm, bool isEdit)
        {
            InitializeComponent();
            infoVM = new PurchaseOrderInfoVM();
            infoVM = vm;
            this.isEdit = isEdit;
            this.Loaded += new RoutedEventHandler(PurchaseOrderCheckReasonDetail_Loaded);
        }

        void PurchaseOrderCheckReasonDetail_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= PurchaseOrderCheckReasonDetail_Loaded;
            serviceFacade = new PurchaseOrderFacade(CurrentPage);
            LoadCheckReasonMemo();
        }

        /// <summary>
        /// 加载检查信息
        /// </summary>
        private void LoadCheckReasonMemo()
        {
            if (isEdit && infoVM.POItems.Count == 1 && infoVM.POItems[0].ItemSysNo.HasValue)
            {
                serviceFacade.LoadPurchaseOrderItemInfo(infoVM.POItems[0].ItemSysNo.Value.ToString(), (obj, args) =>
                 {

                     if (args.FaultsHandle())
                     {
                         return;
                     }
                     PurchaseOrderItemInfo item = args.Result;
                     Dictionary<string, string> dic = new Dictionary<string, string>();
                     CodeNamePairHelper.GetList("PO", "PartlyReceiveReason", (obj2, args2) =>
                     {
                         if (args2.FaultsHandle())
                         {
                             return;
                         }
                         args2.Result.ForEach(x =>
                         {
                             dic.Add(x.Code, x.Name);
                         });

                         int Quantity = 0;
                         string ReasonType = dic.First(a => a.Key == "-1").Value;
                         string CheckReasonMemo = "";
                         bool Message = false;
                         if (item != null)
                         {
                             if (item.CheckStatus != PurchaseOrdeItemCheckStatus.UnCheck)
                             {
                                 Quantity = item.Quantity.Value;
                             }
                             if (item.CheckReasonMemo != null && item.CheckReasonMemo.Trim() != "")
                             {
                                 if (dic.Any(a => a.Value != item.CheckReasonMemo.Trim()))
                                 {
                                     CheckReasonMemo = item.CheckReasonMemo.Trim();
                                 }
                                 else
                                 {
                                     ReasonType = dic.First(a => a.Value != item.CheckReasonMemo.Trim()).Value;
                                 }
                             }
                             //TODO:调用Invoice接口:采购单是否有预付款:
                             //if (QueryProviderFactory.GetQueryProvider<IQueryPO>().GetPayItemByPaySysno(condition.SysNo.Value))
                             //{
                             //    Message = true;
                             //    Quantity = item.Quantity.Value;
                             //}

                             //return Json(new
                             //{
                             //    Quantity = Quantity,
                             //    ReasonType = ReasonType,
                             //    CheckReasonMemo = CheckReasonMemo,
                             //    Message = Message
                             //});

                             this.txtQuantity.Text = Quantity.ToString();
                             this.txtReasonType.Text = ReasonType.ToString();
                             this.txtReasonMemo.Text = CheckReasonMemo;
                             this.lblReasonMessage.Text = (Message == true ? ResPurchaseOrderMaintain.InfoMsg_CheckReason_PrePay : string.Empty);
                         }
                     });
                 });

            }
        }

        #region [Events]
        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            //关闭本窗口:
            this.Dialog.Close(true);
        }
        #endregion
    }
}
