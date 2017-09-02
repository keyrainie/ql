using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CostChangeMaintain : PageBase
    {
        public string CostChangeSysNo;
        public CostChangeInfoVM ChangeInfoVM;
        public CostChangeFacade serviceFacade;

        public CostChangeMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            serviceFacade = new CostChangeFacade(this);
            ChangeInfoVM = new CostChangeInfoVM();
            CostChangeSysNo = this.Request.Param;
            int ccSysNo = 0;
            if (!string.IsNullOrEmpty(CostChangeSysNo))
            {
                int.TryParse(CostChangeSysNo, out ccSysNo);
                if (ccSysNo == 0)
                {
                    Window.Alert(ResCostChangeMaintain.AlertMsg_ErrorInputPram);
                    return;
                }
                else
                {
                    //加载成本变价单信息:
                    LoadCostChangeInfo();
                }

            }

            this.DataContext = ChangeInfoVM;
        }
        private void SetAccessControl()
        {
            //权限
            CostChangeStatus viewStatus = this.ChangeInfoVM.CostChangeBasicInfo.Status.Value;
            if (viewStatus == CostChangeStatus.Created)
            {
                this.btnAddItem.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_AddCCItem);
                this.btnRemoveItem.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_RemoveCCItem);
                this.btnSaveCostChange.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_Update);
                this.btnSubmit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_Submit);
                this.btnAudit.IsEnabled = false;
                this.btnCancelSubmit.IsEnabled = false;
                this.btnVoid.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_Abandon);
                this.btnDeny.IsEnabled = false;
                this.ChangeInfoVM.IsEnabled = true;
            }
            else if(viewStatus == CostChangeStatus.WaitingForAudited)
            {
                this.btnAddItem.IsEnabled = false;
                this.btnRemoveItem.IsEnabled = false;
                this.btnSaveCostChange.IsEnabled = false;
                this.btnSubmit.IsEnabled = false;
                this.btnAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_Audit);
                this.btnCancelSubmit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_CancelSubmit);
                this.btnVoid.IsEnabled = false;
                this.btnDeny.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_Deny);
                this.ChangeInfoVM.IsEnabled = false;
            }
            else if (viewStatus == CostChangeStatus.Audited)//审核通过
            {
                this.btnAddItem.IsEnabled = false;
                this.btnRemoveItem.IsEnabled = false;
                this.btnSaveCostChange.IsEnabled = false;
                this.btnSubmit.IsEnabled = false;
                this.btnAudit.IsEnabled = false;
                this.btnCancelSubmit.IsEnabled = false;
                this.btnVoid.IsEnabled = false;
                this.btnDeny.IsEnabled = false;
                this.ChangeInfoVM.IsEnabled = false;
                this.btnPrint.IsEnabled = true;
            }
            else
            {
                this.btnAddItem.IsEnabled = false;
                this.btnRemoveItem.IsEnabled = false;
                this.btnSaveCostChange.IsEnabled = false;
                this.btnSubmit.IsEnabled = false;
                this.btnAudit.IsEnabled = false;
                this.btnCancelSubmit.IsEnabled = false;
                this.btnVoid.IsEnabled = false;
                this.btnDeny.IsEnabled = false;
                this.ChangeInfoVM.IsEnabled = false;
            }
        }

        /// <summary>
        /// 加载成本变价单信息
        /// </summary>
        private void LoadCostChangeInfo()
        {
            serviceFacade.LoadCostChangeInfo(CostChangeSysNo, (obj, args) =>
            {
                if (args.FaultsHandle() || args.Result==null)
                {
                    return;
                }

                ChangeInfoVM = EntityConverter<CostChangeInfo, CostChangeInfoVM>.Convert(args.Result, (s, t) =>
                {
                    t.CCSysNo = s.SysNo;
                    t.CompanyCode = s.CompanyCode;
                    t.CostChangeBasicInfo.VendorSysNo = s.CostChangeBasicInfo.VendorSysNo;
                    t.CostChangeBasicInfo.PMSysNo = s.CostChangeBasicInfo.PMSysNo;
                    t.CostChangeBasicInfo.Memo = s.CostChangeBasicInfo.Memo;
                    t.CostChangeBasicInfo.AuditMemo = s.CostChangeBasicInfo.AuditMemo;
                    t.CostChangeBasicInfo.Status = s.CostChangeBasicInfo.Status;
                    t.CostChangeBasicInfo.TotalDiffAmt = s.CostChangeBasicInfo.TotalDiffAmt;

                    t.CostChangeItems = new List<CostChangeItemInfoVM>();
                    foreach (var item in s.CostChangeItems)
                    {
                        CostChangeItemInfoVM itemVM = item.Convert<CostChangeItemsInfo, CostChangeItemInfoVM>();
                        t.CostChangeItems.Add(itemVM);
                    }
                });

                this.DataContext = ChangeInfoVM;
                SetAccessControl();
                this.gridItemListInfo.Bind();
                this.lblTotalDiffAmt.Text = string.Format("变价总金额：{0}元", ChangeInfoVM.CostChangeBasicInfo.TotalDiffAmt);
                //ShowActionButton(consignSettleVM.Status.Value);
            });
        }

        #region [Events]

        private void gridItemListInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var costChangeItemList = this.ChangeInfoVM.CostChangeItems.Where(i => i.ItemActionStatus != ItemActionStatus.Delete).ToList();
            this.gridItemListInfo.ItemsSource = costChangeItemList;
            this.gridItemListInfo.TotalCount = costChangeItemList.Count;
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {
                if (null != this.gridItemListInfo.ItemsSource)
                {
                    foreach (var item in this.gridItemListInfo.ItemsSource)
                    {
                        if (item is CostChangeItemInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((CostChangeItemInfoVM)item).IsCheckedItem)
                                {
                                    ((CostChangeItemInfoVM)item).IsCheckedItem = true;
                                }
                            }
                            else
                            {
                                if (((CostChangeItemInfoVM)item).IsCheckedItem)
                                {
                                    ((CostChangeItemInfoVM)item).IsCheckedItem = false;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void txtNewPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (null != txt && !txt.IsReadOnly)
            {
                decimal getInputNewPrice = 0m;
                if(string.IsNullOrEmpty(txt.Text.Trim()))
                {
                    Window.Alert(ResCostChangeNew.InfoMsg_NewPriceNull);
                    return;
                }

                CostChangeItemInfoVM getSelectedItemVM = txt.DataContext as CostChangeItemInfoVM;

                if (decimal.TryParse(txt.Text.Trim(), out getInputNewPrice))
                {
                    if (getInputNewPrice > 0)
                    {
                        getSelectedItemVM.NewPrice = getInputNewPrice.ToString();
                        CalcTotalDiffAmt();
                    }
                    else
                    {
                        Window.Alert(ResCostChangeNew.InfoMsg_NewPriceError);
                    }
                }
            }
        }

        private void txtChangeCount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (null != txt && !txt.IsReadOnly)
            {
                int getInputChangeCount = 0;
                if (string.IsNullOrEmpty(txt.Text.Trim()))
                {
                    Window.Alert(ResCostChangeNew.InfoMsg_ChangeCountNull);
                    return;
                }

                CostChangeItemInfoVM getSelectedItemVM = txt.DataContext as CostChangeItemInfoVM;

                if (int.TryParse(txt.Text.Trim(), out getInputChangeCount))
                {
                    if (getInputChangeCount <= getSelectedItemVM.AvaliableQty && getInputChangeCount>=0)
                    {
                        getSelectedItemVM.ChangeCount = getInputChangeCount.ToString();
                        CalcTotalDiffAmt();
                    }
                    else
                    {
                        Window.Alert(ResCostChangeNew.InfoMsg_ChangeCountError);
                    }
                }
            }
        }

        /// <summary>
        /// 根据items重新计算变价总金额
        /// </summary>
        private void CalcTotalDiffAmt()
        {
            decimal totalDiffAmt = 0m;
            int changeCount;
            decimal newPrice;
            this.ChangeInfoVM.CostChangeItems.ForEach(delegate(CostChangeItemInfoVM itemVM)
            {
                if (!int.TryParse(itemVM.ChangeCount, out changeCount))
                {
                    changeCount = 0;
                }

                if (!Decimal.TryParse(itemVM.NewPrice, out newPrice))
                {
                    newPrice = 0m;
                }

                if (itemVM.ItemActionStatus != ItemActionStatus.Delete)
                {
                    totalDiffAmt += changeCount * (newPrice - itemVM.OldPrice.Value);
                }
            });

            this.ChangeInfoVM.CostChangeBasicInfo.TotalDiffAmt = totalDiffAmt;
            this.lblTotalDiffAmt.Text = string.Format("变价总金额：{0}元",totalDiffAmt);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveCostChange_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this) || !CheckVM())
            {
                return;
            }

            CostChangeInfo info = BuildVMToEntity();
            int ItemsCount = info.CostChangeItems.Where(i=>i.ItemActionStatus!=ItemActionStatus.Delete).Count();
            if (ItemsCount==0)
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_NoAvaliableItem);
                return;
            }

            ////保存PM高级权限，用于业务端验证
            //info.PurchaseOrderBasicInfo.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
            serviceFacade.UpdateCostChangeInfo(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result.SysNo.HasValue)
                {
                    Window.Alert(ResCostChangeMaintain.Msg_Title, ResCostChangeMaintain.Msg_SaveCCSuc, MessageType.Information, (objj, argss) =>
                    {
                        LoadCostChangeInfo();
                    });
                }
                else
                {
                    Window.Alert(ResCostChangeMaintain.AlertMsg_SaveCCFailed);
                    return;
                }
            });
        }

        /// <summary>
        /// 添加明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            //添加采购商品:
            if (!ChangeInfoVM.CostChangeBasicInfo.VendorSysNo.HasValue)
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_VendorEmpty);
                return;
            }
            if (!ChangeInfoVM.CostChangeBasicInfo.PMSysNo.HasValue)
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_PMEmpty);
                return;
            }

            CostChangeItemsQuery newCtrl = new CostChangeItemsQuery(ChangeInfoVM.CostChangeBasicInfo.VendorSysNo.Value, ChangeInfoVM.CostChangeBasicInfo.PMSysNo.Value);
            newCtrl.DialogHandler = Window.ShowDialog(ResCostChangeMaintain.Button_AddItem, newCtrl, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    List<CostItemInfoVM> IstAddedVM = new List<CostItemInfoVM>();
                    IstAddedVM = args.Data as List<CostItemInfoVM>;
                    if (IstAddedVM.Count > 0)
                    {
                        CostChangeItemInfoVM existItemVM;
                        foreach (CostItemInfoVM itemVM in IstAddedVM)
                        {
                            existItemVM = ChangeInfoVM.CostChangeItems.SingleOrDefault(i => i.ProductSysNo == itemVM.ProductSysNo.Value && i.POSysNo == itemVM.POSysNo.Value);

                            if (existItemVM == null)//如不在列表中
                            {
                                CostChangeItemInfoVM changeItem = new CostChangeItemInfoVM()
                                {
                                    ItemSysNo = 0,
                                    ProductSysNo = itemVM.ProductSysNo.Value,
                                    ProductID = itemVM.ProductID,
                                    ProductName = itemVM.ProductName,
                                    POSysNo = itemVM.POSysNo.Value,
                                    OldPrice = itemVM.Cost.Value,
                                    NewPrice = itemVM.Cost.Value.ToString(),
                                    AvaliableQty = itemVM.AvaliableQty.Value,
                                    ChangeCount = "0",
                                    CompanyCode = itemVM.CompanyCode,
                                    IsCheckedItem = false,
                                    ItemActionStatus = ItemActionStatus.Add
                                };

                                ChangeInfoVM.CostChangeItems.Add(changeItem);
                            }
                            else if (existItemVM!=null && existItemVM.ItemActionStatus == ItemActionStatus.Delete)//如在列表中但是已删除
                            {
                                existItemVM.ChangeCount = "0";
                                existItemVM.IsCheckedItem = false;
                                existItemVM.ItemActionStatus = ItemActionStatus.Update;
                            }
                        }

                        this.gridItemListInfo.Bind();
                    }
                }
            });

        }

        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            List<CostChangeItemInfoVM> delItems = new List<CostChangeItemInfoVM>();

            int deleteCount = this.ChangeInfoVM.CostChangeItems.Where(i => i.IsCheckedItem == true).Count();

            if (deleteCount <= 0)
            {
                Window.Alert(ResCostChangeMaintain.InfoMsg_CheckDeleteItems);
                return;
            }
            Window.Confirm(ResCostChangeMaintain.ConfirmMsg_RemoveItems, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    foreach (CostChangeItemInfoVM item in this.ChangeInfoVM.CostChangeItems.Where(i => i.IsCheckedItem == true))
                    {
                        item.ItemActionStatus = ItemActionStatus.Delete;
                    }

                    this.ChangeInfoVM.CostChangeItems.RemoveAll(i => i.IsCheckedItem == true && i.ItemSysNo.Value==0);
                    this.gridItemListInfo.Bind();
                    CalcTotalDiffAmt();
                }
            });
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            CostChangeInfo costChangeInfo = BuildVMToEntity();
            costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.WaitingForAudited;
            serviceFacade.SubmitAuditCostChange(costChangeInfo, (obj2, args2) =>
            {
                if (args2.FaultsHandle())
                {
                    Window.Alert(ResCostChangeMaintain.AlertMsg_SubmitCCFailed);
                    return;
                }

                AlertAndRefreshPage(ResCostChangeMaintain.Msg_SubmitCCSuc);
            });
        }

        /// <summary>
        /// 取消提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelSubmit_Click(object sender, RoutedEventArgs e)
        {
            CostChangeInfo costChangeInfo = BuildVMToEntity();
            costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Created;
            serviceFacade.CancelSubmitAuditPOCostChange(costChangeInfo, (obj2, args2) =>
            {
                if (args2.FaultsHandle())
                {
                    Window.Alert(ResCostChangeMaintain.AlertMsg_CancelSubmitCCFailed);
                    return;
                }

                AlertAndRefreshPage(ResCostChangeMaintain.Msg_CancelSubmitCCSuc);
            });
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            CostChangeInfo costChangeInfo = BuildVMToEntity();
            if (string.IsNullOrEmpty(ChangeInfoVM.CostChangeBasicInfo.AuditMemo))
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_AuditMemoEmpty);
                return;
            }

            costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Audited;
            serviceFacade.AuditCostChange(costChangeInfo, (obj2, args2) =>
            {
                if (args2.FaultsHandle())
                {
                    //Window.Alert(ResCostChangeMaintain.AlertMsg_AuditCCFailed);
                    return;
                }

                AlertAndRefreshPage(ResCostChangeMaintain.Msg_AuditCCSuc);
            });
        }

        /// <summary>
        /// 审核拒绝（退回）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeny_Click(object sender, RoutedEventArgs e)
        {
            CostChangeInfo costChangeInfo = BuildVMToEntity();
            if (string.IsNullOrEmpty(ChangeInfoVM.CostChangeBasicInfo.AuditMemo))
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_AuditMemoEmpty);
                return;
            }

            costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Created;
            serviceFacade.RefuseCostChange(costChangeInfo, (obj2, args2) =>
            {
                if (args2.FaultsHandle())
                {
                    Window.Alert(ResCostChangeMaintain.AlertMsg_DenyCCFailed);
                    return;
                }

                AlertAndRefreshPage(ResCostChangeMaintain.Msg_DenyCCSuc);
            });
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            CostChangeInfo costChangeInfo = BuildVMToEntity();
            costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Abandoned;
            serviceFacade.AbandonCostChange(costChangeInfo, (obj2, args2) =>
            {
                if (args2.FaultsHandle())
                {
                    Window.Alert(ResCostChangeMaintain.AlertMsg_VoidCCFailed);
                    return;
                }

                AlertAndRefreshPage(ResCostChangeMaintain.Msg_VoidCCSuc);
            });
        }
        
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (null == CostChangeSysNo)
            {
                return;
            }
            //打印操作:
            HtmlViewHelper.WebPrintPreview("PO", "CostChangePrint", new Dictionary<string, string>() { { "CostChangeSysNo", CostChangeSysNo } });
        }

        private void AlertAndRefreshPage(string alertString)
        {
            Window.Alert(ResCostChangeMaintain.Msg_Title, alertString, MessageType.Information, (obj, args) =>
            {
                Window.Refresh();
            });
        }

        /// <summary>
        /// 数据检查
        /// </summary>
        /// <returns></returns>
        private bool CheckVM()
        {
            if (!ChangeInfoVM.CostChangeBasicInfo.VendorSysNo.HasValue)
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_VendorEmpty);
                return false;
            }
            if (!ChangeInfoVM.CostChangeBasicInfo.PMSysNo.HasValue)
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_PMEmpty);
                return false;
            }
            if (string.IsNullOrEmpty(ChangeInfoVM.CostChangeBasicInfo.Memo))
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_MemoEmpty);
                return false;
            }

            foreach (CostChangeItemInfoVM changeItem in ChangeInfoVM.CostChangeItems)
            {
                if (changeItem.OldPrice == Convert.ToDecimal(changeItem.NewPrice))
                {
                    Window.Alert(ResCostChangeMaintain.AlertMsg_ExistsInvalidNewPrice);
                    return false;
                }

                if (Convert.ToInt32(changeItem.ChangeCount) == 0)
                {
                    Window.Alert(ResCostChangeMaintain.AlertMsg_ExistsInvalidChangeCount);
                    return false;
                }
            }

            return true;
        }

        private CostChangeInfo BuildVMToEntity()
        {
            CostChangeInfo info = EntityConverter<CostChangeInfoVM, CostChangeInfo>.Convert(ChangeInfoVM, (s, t) =>
            {
                t.SysNo = s.CCSysNo;
                t.CompanyCode = s.CompanyCode;
                t.CostChangeBasicInfo.VendorSysNo = s.CostChangeBasicInfo.VendorSysNo.Value;
                t.CostChangeBasicInfo.PMSysNo = s.CostChangeBasicInfo.PMSysNo.Value;
                t.CostChangeBasicInfo.Memo = s.CostChangeBasicInfo.Memo;
                t.CostChangeBasicInfo.AuditMemo = s.CostChangeBasicInfo.AuditMemo;
                t.CostChangeItems = new List<CostChangeItemsInfo>();

                foreach (CostChangeItemInfoVM changeItem in s.CostChangeItems)
                {
                    t.CostChangeItems.Add(
                        new CostChangeItemsInfo()
                        {
                            ItemSysNo = changeItem.ItemSysNo,
                            ProductSysNo = changeItem.ProductSysNo.Value,
                            POSysNo = changeItem.POSysNo.Value,
                            OldPrice = changeItem.OldPrice.Value,
                            NewPrice = Convert.ToDecimal(changeItem.NewPrice),
                            ChangeCount = Convert.ToInt32(changeItem.ChangeCount),
                            CompanyCode = changeItem.CompanyCode,
                            ItemActionStatus = changeItem.ItemActionStatus
                        }
                    );
                }

            });

            return info;
        }

        #endregion
    }
}
