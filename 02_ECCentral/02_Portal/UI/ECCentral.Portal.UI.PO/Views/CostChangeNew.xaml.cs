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
    public partial class CostChangeNew : PageBase
    {
        public CostChangeInfoVM newChangeInfoVM;
        public CostChangeFacade serviceFacade;
        //public List<CostChangeItemInfoVM> mergedItemList;

        public CostChangeNew()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            serviceFacade = new CostChangeFacade(this);
            SetAccessControl();
            newChangeInfoVM = new CostChangeInfoVM();
            //mergedItemList = new List<CostChangeItemInfoVM>();

            this.DataContext = newChangeInfoVM;
        }
        private void SetAccessControl()
        {
            //创建成本变价单:
            this.btnCreateCostChange.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_Create);
            //添加明细:
            this.btnAddItem.IsEnabled = true;  //AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_AddNewCCItem);
            //删除明细:
            this.btnRemoveItem.IsEnabled = true; //AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_RemoveNewCCItem);
        }

        #region [Events]

        private void gridProductsListInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var costChangeItemList = this.newChangeInfoVM.CostChangeItems.ToList();
            this.gridProductsListInfo.ItemsSource = costChangeItemList;
            this.gridProductsListInfo.TotalCount = costChangeItemList.Count;
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {
                if (null != this.gridProductsListInfo.ItemsSource)
                {
                    foreach (var item in this.gridProductsListInfo.ItemsSource)
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

        private void btnCreateCostChange_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this) || !CheckVM())
            {
                return;
            }

            CostChangeInfo info = BuildVMToEntity();

            int ItemsCount = info.CostChangeItems.Where(i => i.ItemActionStatus != ItemActionStatus.Delete).Count();
            if (ItemsCount == 0)
            {
                Window.Alert(ResCostChangeMaintain.AlertMsg_NoAvaliableItem);
                return;
            }

            info.CostChangeBasicInfo.Status = CostChangeStatus.Created;
            ////保存PM高级权限，用于业务端验证
            //info.PurchaseOrderBasicInfo.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
            serviceFacade.CreateCostChange(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result.SysNo.HasValue)
                {
                    Window.Alert(ResCostChangeNew.Msg_Title, ResCostChangeNew.Msg_CreateCCSuc, MessageType.Information, (objj, argss) =>
                    {
                        if (argss.DialogResult == DialogResultType.Cancel)
                        {
                            Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/CostChangeMaintain/{0}", args.Result.SysNo.Value), true);
                        }
                    });
                }
                else
                {
                    Window.Alert(ResCostChangeNew.AlertMsg_CreateCCFailed);
                    return;
                }
            });
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (!newChangeInfoVM.CostChangeBasicInfo.VendorSysNo.HasValue)
            {
                Window.Alert(ResCostChangeNew.AlertMsg_VendorEmpty);
                return;
            }
            if (!newChangeInfoVM.CostChangeBasicInfo.PMSysNo.HasValue)
            {
                Window.Alert(ResCostChangeNew.AlertMsg_PMEmpty);
                return;
            }

            CostChangeItemsQuery newCtrl = new CostChangeItemsQuery(newChangeInfoVM.CostChangeBasicInfo.VendorSysNo.Value,newChangeInfoVM.CostChangeBasicInfo.PMSysNo.Value);
            newCtrl.DialogHandler = Window.ShowDialog(ResCostChangeNew.Button_AddItem, newCtrl, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    List<CostItemInfoVM> IstAddedVM = new List<CostItemInfoVM>();
                    IstAddedVM = args.Data as List<CostItemInfoVM>;
                    if (IstAddedVM.Count>0)
                    {
                        CostChangeItemInfoVM existItemVM;
                        foreach (CostItemInfoVM itemVM in IstAddedVM)
                        {
                            existItemVM = newChangeInfoVM.CostChangeItems.SingleOrDefault(i => i.ProductSysNo == itemVM.ProductSysNo.Value && i.POSysNo == itemVM.POSysNo.Value);
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

                                newChangeInfoVM.CostChangeItems.Add(changeItem);
                            }
                        }

                        this.gridProductsListInfo.Bind();
                    }
                }
            });

        }

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            List<CostChangeItemInfoVM> delItems = new List<CostChangeItemInfoVM>();

            int deleteCount = this.newChangeInfoVM.CostChangeItems.Where(i => i.IsCheckedItem == true).Count();

            if (deleteCount<=0)
            {
                Window.Alert(ResCostChangeNew.InfoMsg_CheckDeleteItems);
                return;
            }
            Window.Confirm(ResCostChangeNew.ConfirmMsg_RemoveItems, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.newChangeInfoVM.CostChangeItems.RemoveAll(i => i.IsCheckedItem == true);
                    this.gridProductsListInfo.Bind();
                    CalcTotalDiffAmt();
                }
            });
        }

        private void txtNewPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (null != txt && !txt.IsReadOnly)
            {
                decimal getInputNewPrice = 0m;
                if (string.IsNullOrEmpty(txt.Text.Trim()))
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
                        return;
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
                    if (getInputChangeCount <= getSelectedItemVM.AvaliableQty && getInputChangeCount >= 0)
                    {
                        getSelectedItemVM.ChangeCount = getInputChangeCount.ToString();
                        CalcTotalDiffAmt();
                    }
                    else
                    {
                        Window.Alert(ResCostChangeNew.InfoMsg_ChangeCountError);
                        return;
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
            this.newChangeInfoVM.CostChangeItems.ForEach(delegate(CostChangeItemInfoVM itemVM)
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

            this.newChangeInfoVM.CostChangeBasicInfo.TotalDiffAmt = totalDiffAmt;
            this.lblTotalDiffAmt.Text = string.Format("变价总金额：{0}元", totalDiffAmt);
        }

        /// <summary>
        /// 数据检查
        /// </summary>
        /// <returns></returns>
        private bool CheckVM()
        {
            if (!newChangeInfoVM.CostChangeBasicInfo.VendorSysNo.HasValue)
            {
                Window.Alert(ResCostChangeNew.AlertMsg_VendorEmpty);
                return false;
            }
            if (!newChangeInfoVM.CostChangeBasicInfo.PMSysNo.HasValue)
            {
                Window.Alert(ResCostChangeNew.AlertMsg_PMEmpty);
                return false;
            }
            if (string.IsNullOrEmpty(newChangeInfoVM.CostChangeBasicInfo.Memo))
            {
                Window.Alert(ResCostChangeNew.AlertMsg_MemoEmpty);
                return false;
            }

            foreach (CostChangeItemInfoVM changeItem in newChangeInfoVM.CostChangeItems)
            {
                if (changeItem.OldPrice == Convert.ToDecimal(changeItem.NewPrice))
                {
                    Window.Alert(ResCostChangeNew.AlertMsg_ExistsInvalidNewPrice);
                    return false;
                }

                if (Convert.ToInt32(changeItem.ChangeCount) == 0)
                {
                    Window.Alert(ResCostChangeNew.AlertMsg_ExistsInvalidChangeCount);
                    return false;
                }
            }

            return true;
        }

        private CostChangeInfo BuildVMToEntity()
        {
            CostChangeInfo info = EntityConverter<CostChangeInfoVM, CostChangeInfo>.Convert(newChangeInfoVM, (s, t) =>
            {
                t.CostChangeBasicInfo.VendorSysNo = s.CostChangeBasicInfo.VendorSysNo.Value;
                t.CostChangeBasicInfo.PMSysNo = s.CostChangeBasicInfo.PMSysNo.Value;
                t.CostChangeBasicInfo.Memo = s.CostChangeBasicInfo.Memo;
                t.CostChangeItems = new List<CostChangeItemsInfo>();

                foreach (CostChangeItemInfoVM changeItem in s.CostChangeItems)
                {
                    t.CostChangeItems.Add(
                        new CostChangeItemsInfo()
                        {
                            ProductSysNo = changeItem.ProductSysNo.Value,
                            POSysNo = changeItem.POSysNo.Value,
                            OldPrice = changeItem.OldPrice.Value,
                            NewPrice = Convert.ToDecimal(changeItem.NewPrice),
                            ChangeCount = Convert.ToInt32(changeItem.ChangeCount),
                            CompanyCode = changeItem.CompanyCode,
                            ItemActionStatus = ItemActionStatus.Add
                        }
                    );
                }

            });
            return info;
        }

        #endregion
    }
}
