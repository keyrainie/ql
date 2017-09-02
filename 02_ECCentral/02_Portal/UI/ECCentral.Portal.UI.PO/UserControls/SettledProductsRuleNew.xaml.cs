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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class SettledProductsRuleNew : UserControl
    {
        public IDialog Dialog { get; set; }

        public ConsignSettlementRulesFacade serviceFacade;
        public PurchaseOrderFacade poFacade;


        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public ConsignSettlementRulesInfoVM viewVM;

        public SettledProductsRuleNew()
        {
            InitializeComponent();
            viewVM = new ConsignSettlementRulesInfoVM();
            serviceFacade = new ConsignSettlementRulesFacade(CPApplication.Current.CurrentPage);
            poFacade = new PurchaseOrderFacade(CPApplication.Current.CurrentPage);
            this.ucProduct.ProductSelected += new EventHandler<Basic.Components.UserControls.ProductPicker.ProductSelectedEventArgs>(ucProduct_ProductSelected);
            this.Loaded += new RoutedEventHandler(SettledProductsRuleNew_Loaded);
        }

        void ucProduct_ProductSelected(object sender, Basic.Components.UserControls.ProductPicker.ProductSelectedEventArgs e)
        {
            bool IsLastPriceNotFound = false;

            //查询商品的最后一次PO入库的价格:
            string getProductSysNo = e.SelectedProduct.SysNo.HasValue ? e.SelectedProduct.SysNo.Value.ToString() : "";
            if (!string.IsNullOrEmpty(getProductSysNo))
            {
                poFacade.QueryPurchaseOrderLastPrice(getProductSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (null != args.Result && null != args.Result.Rows)
                    {
                        try
                        {
                            decimal getLastPrice = (args.Result.Rows[0]["LastPrice"] == null ? 0m : Convert.ToDecimal(args.Result.Rows[0]["LastPrice"].ToString()));
                            viewVM.OldSettlePrice = getLastPrice.ToString("f2");
                        }
                        catch
                        {
                            IsLastPriceNotFound = true;
                        }
                    }
                    if (IsLastPriceNotFound)
                    {
                        CurrentWindow.Alert(string.Format(ResSettledProductsRuleQuery.InfoMsg_SettleRule_LastPurchasePriceNotFound, getProductSysNo));
                        viewVM.OldSettlePrice = null;
                        return;
                    }
                });
            }
        }

        void SettledProductsRuleNew_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(SettledProductsRuleNew_Loaded);
            this.DataContext = viewVM;
        }

        #region [Events]
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //保存操作 ：
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            List<string> checkPricesList = new List<string>();
            string pricesCheckAlertString = string.Empty;
            string oldPriceNullString = string.Empty;
            if (string.IsNullOrEmpty(viewVM.OldSettlePrice))
            {
                oldPriceNullString += ResSettledProductsRuleQuery.InfoMsg_SettleRule_OldSettlePriceNullAlert + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(viewVM.OldSettlePrice) && Convert.ToDecimal(viewVM.OldSettlePrice) <= 0)
            {
                checkPricesList.Add(ResSettledProductsRuleQuery.InfoMsg_SettleRule_OldSettlePrice);
            }
            if (!string.IsNullOrEmpty(viewVM.NewSettlePrice) && Convert.ToDecimal(viewVM.NewSettlePrice) <= 0)
            {
                checkPricesList.Add(ResSettledProductsRuleQuery.InfoMsg_SettleRule_NewSettlePrice);
            }
            if (checkPricesList.Count > 0)
            {
                pricesCheckAlertString += string.Join(ResSettledProductsRuleQuery.Label_And, checkPricesList);
            }

            if (!string.IsNullOrEmpty(pricesCheckAlertString) || !string.IsNullOrEmpty(oldPriceNullString))
            {
                string confirmStr = string.Empty;
                if (!string.IsNullOrEmpty(pricesCheckAlertString))
                {
                    confirmStr += string.Format(ResSettledProductsRuleQuery.InfoMsg_SettleRule_ContinueAlert, pricesCheckAlertString) + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(oldPriceNullString))
                {
                    confirmStr += oldPriceNullString + Environment.NewLine;
                }
                CurrentWindow.Confirm(confirmStr, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(viewVM);
                        info.OldSettlePrice = (info.OldSettlePrice.HasValue ? info.OldSettlePrice.Value : 0);
                        info.EndDate = info.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(997);
                        serviceFacade.CreateConsignSettleRule(info, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            CurrentWindow.Alert(ResSettledProductsRuleQuery.InfoMsg_SettleRule_Title, ResSettledProductsRuleQuery.InfoMsg_SettleRule_OprSuc, MessageType.Information, (obj3, args3) =>
                            {
                                if (args3.DialogResult == DialogResultType.Cancel)
                                {
                                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                    this.Dialog.Close(true);
                                }
                            });
                        });
                    }
                });
            }
            else
            {
                ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(viewVM);
                info.OldSettlePrice = (info.OldSettlePrice.HasValue ? info.OldSettlePrice.Value : 0);
                serviceFacade.CreateConsignSettleRule(info, (obj2, args2) =>
                {
                    if (args2.FaultsHandle())
                    {
                        return;
                    }
                    this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    CurrentWindow.Alert(ResSettledProductsRuleQuery.InfoMsg_SettleRule_Title, ResSettledProductsRuleQuery.InfoMsg_SettleRule_OprSuc, MessageType.Information, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.Cancel)
                        {
                            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            this.Dialog.Close(true);
                        }
                    });
                });
            }
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            //取消操作 ：
            Dialog.Close(true);
        }
        #endregion

    }
}
