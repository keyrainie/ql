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
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using System.Collections;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class SettledProductsRuleMaintain : UserControl
    {
        public IDialog Dialog { get; set; }

        public ConsignSettlementRulesFacade serviceFacade;
        public ConsignSettlementRulesInfoVM editVM;
        public PurchaseOrderFacade poFacade;

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }


        public SettledProductsRuleMaintain(ConsignSettlementRulesInfoVM editVM)
        {
            InitializeComponent();
            this.editVM = editVM;
            this.ucProduct.ProductSelected += new EventHandler<Basic.Components.UserControls.ProductPicker.ProductSelectedEventArgs>(ucProduct_ProductSelected);
            this.Loaded += new RoutedEventHandler(SettledProductsRuleMaintain_Loaded);
        }

        void ucProduct_ProductSelected(object sender, Basic.Components.UserControls.ProductPicker.ProductSelectedEventArgs e)
        {
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
                        decimal getLastPrice = (args.Result.Rows[0]["LastPrice"] == null ? 0m : Convert.ToDecimal(args.Result.Rows[0]["LastPrice"].ToString()));
                        editVM.OldSettlePrice = getLastPrice.ToString("f2");
                    }
                    else
                    {
                        CurrentWindow.Alert(string.Format(ResSettledProductsRuleQuery.InfoMsg_SettleRule_LastPurchasePriceNotFound, getProductSysNo));
                        editVM.OldSettlePrice = null;
                        return;
                    }
                });
            }
        }

        void SettledProductsRuleMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(SettledProductsRuleMaintain_Loaded);
            serviceFacade = new ConsignSettlementRulesFacade(CPApplication.Current.CurrentPage);
            poFacade = new PurchaseOrderFacade(CPApplication.Current.CurrentPage);
            this.DataContext = editVM;
            ShowActionButtons();
        }

        private void ShowActionButtons()
        {
            if (this.editVM.Status.HasValue)
            {
                //未审核，显示保存按钮:
                if (this.editVM.Status == ConsignSettleRuleStatus.Wait_Audit)
                {
                    this.btnAbandon.IsEnabled = true;
                    this.btnUpdate.IsEnabled = true;
                    this.btnAudit.IsEnabled = true;
                    this.ucVendor.IsAllowVendorSelect = true;
                    this.ucProduct.IsEnabled = true;
                }
                else if (this.editVM.Status == ConsignSettleRuleStatus.Available || this.editVM.Status == ConsignSettleRuleStatus.Enable)
                {
                    this.btnStop.IsEnabled = true;
                }
            }
        }

        #region [Events]
        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            //取消操作 ：
            Dialog.Close(true);
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            //审核操作 ：
            CurrentWindow.Confirm(ResSettledProductsRuleQuery.InfoMsg_SettleRule_ConfirmAudit, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(editVM);
                    serviceFacade.AuditConsignSettleRule(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        CurrentWindow.Alert(ResSettledProductsRuleQuery.InfoMsg_SettleRule_Title, ResSettledProductsRuleQuery.InfoMsg_SettleRule_OprSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                Dialog.ResultArgs.Data = editVM;
                                Dialog.Close(true);
                            }
                        });
                    });
                }
            });
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //终止操作 ：
            CurrentWindow.Confirm(ResSettledProductsRuleQuery.InfoMsg_SettleRule_ConfirmStop, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(editVM);
                    serviceFacade.StopConsignSettleRule(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        CurrentWindow.Alert(ResSettledProductsRuleQuery.InfoMsg_SettleRule_Title, ResSettledProductsRuleQuery.InfoMsg_SettleRule_OprSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                Dialog.ResultArgs.Data = editVM;
                                Dialog.Close(true);
                            }
                        });
                    });
                }
            });
        }
        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            //作废操作 ：
            //终止操作 ：
            CurrentWindow.Confirm(ResSettledProductsRuleQuery.InfoMsg_SettleRule_ConfirmAbandon, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(editVM);
                    serviceFacade.AbandonConsignSettleRule(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        CurrentWindow.Alert(ResSettledProductsRuleQuery.InfoMsg_SettleRule_Title, ResSettledProductsRuleQuery.InfoMsg_SettleRule_OprSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                Dialog.ResultArgs.Data = editVM;
                                Dialog.Close(true);
                            }
                        });
                    });
                }
            });
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //保存更新操作 ：
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            List<string> checkPricesList = new List<string>();
            string pricesCheckAlertString = string.Empty;
            string oldPriceNullString = string.Empty;
            if (string.IsNullOrEmpty(editVM.OldSettlePrice))
            {
                oldPriceNullString += ResSettledProductsRuleQuery.InfoMsg_SettleRule_OldSettlePriceNullAlert + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(editVM.OldSettlePrice) && Convert.ToDecimal(editVM.OldSettlePrice) <= 0)
            {
                checkPricesList.Add(ResSettledProductsRuleQuery.InfoMsg_SettleRule_OldSettlePrice);
            }
            if (!string.IsNullOrEmpty(editVM.NewSettlePrice) && Convert.ToDecimal(editVM.NewSettlePrice) <= 0)
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
                        ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(editVM);
                        //结束时间换算到当天的23:59:59.997
                        //数据库最大保存到 .997
                        info.EndDate = info.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(997);
                        serviceFacade.UpdateConsignSettleRule(info, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            CurrentWindow.Alert(ResSettledProductsRuleQuery.InfoMsg_SettleRule_Title, ResSettledProductsRuleQuery.InfoMsg_SettleRule_OprSuc, MessageType.Information, (obj3, args3) =>
                            {
                                if (args3.DialogResult == DialogResultType.Cancel)
                                {
                                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                                    Dialog.ResultArgs.Data = editVM;
                                    Dialog.Close(true);
                                }
                            });
                        });
                    }
                });
            }
            else
            {
                ConsignSettlementRulesInfo info = EntityConverter<ConsignSettlementRulesInfoVM, ConsignSettlementRulesInfo>.Convert(editVM);
                //结束时间换算到当天的23:59:59.997
                //数据库最大保存到 .997
                info.EndDate = info.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(997);
                serviceFacade.UpdateConsignSettleRule(info, (obj2, args2) =>
                {
                    if (args2.FaultsHandle())
                    {
                        return;
                    }
                    CurrentWindow.Alert(ResSettledProductsRuleQuery.InfoMsg_SettleRule_Title, ResSettledProductsRuleQuery.InfoMsg_SettleRule_OprSuc, MessageType.Information, (obj3, args3) =>
                    {
                        if (args3.DialogResult == DialogResultType.Cancel)
                        {
                            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            Dialog.ResultArgs.Data = editVM;
                            Dialog.Close(true);
                        }
                    });
                });
            }
        }
        #endregion

        public bool ValidationMananger { get; set; }
    }
}
