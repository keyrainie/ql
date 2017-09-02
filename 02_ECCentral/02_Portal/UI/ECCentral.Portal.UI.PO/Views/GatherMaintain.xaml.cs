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
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.PO.Models.Settlement;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class GatherMaintain : PageBase
    {
        public string GatherSysNo;
        public GatherSettlementInfoVM GetherInfoVM;
        public GatherSettlementFacade serviceFacade;

        public GatherMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            serviceFacade = new GatherSettlementFacade(this);
            GetherInfoVM = new GatherSettlementInfoVM();
            GatherSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(GatherSysNo))
            {
                //加载代收结算单信息:
                LoadGatherInfo();

            }
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //查询代收结算单相关:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Edit_Abandon))
            {
                btnAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Edit_Audit))
            {
                btnAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Edit_CancelAudit))
            {
                btnCancelAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Edit_Settle))
            {
                btnSettle.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Edit_CancelSettle))
            {
                btnCancelSettle.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Edit_DeleteProduct))
            {
                btnDeleteSettledProducts.IsEnabled = false;
            }
        }

        private void LoadGatherInfo()
        {
            serviceFacade.LoadGatherSettlementInfo(GatherSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                GetherInfoVM = EntityConverter<GatherSettlementInfo, GatherSettlementInfoVM>.Convert(args.Result, (s, t) =>
                {
                    t.StockID = s.SourceStockInfo.SysNo.ToString();
                    t.StockName = s.SourceStockInfo.StockName;
                    t.VendorInfo.VendorBasicInfo.PaySettleCompany = s.VendorInfo.VendorBasicInfo.PaySettleCompany;
                    txtPaySettleCompany.Text = EnumConverter.GetDescription(t.VendorInfo.VendorBasicInfo.PaySettleCompany);
                });
                this.DataContext = GetherInfoVM;
                SettledProductsGrid.Bind();
                ShowActionButtons(GetherInfoVM.SettleStatus.Value);
            });
        }

        #region [Events]

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {

                if (null != this.SettledProductsGrid.ItemsSource)
                {
                    foreach (var item in this.SettledProductsGrid.ItemsSource)
                    {
                        if (item is GatherSettlementItemInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((GatherSettlementItemInfoVM)item).IsDeleteChecked)
                                {
                                    ((GatherSettlementItemInfoVM)item).IsDeleteChecked = true;
                                }
                            }
                            else
                            {
                                if (((GatherSettlementItemInfoVM)item).IsDeleteChecked)
                                {
                                    ((GatherSettlementItemInfoVM)item).IsDeleteChecked = false;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void SettledProductsGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (null != this.GetherInfoVM && null != this.GetherInfoVM.GatherSettlementItemInfoList)
            {
                //清空 CheckBox:
                this.GetherInfoVM.GatherSettlementItemInfoList.ForEach(delegate(GatherSettlementItemInfoVM vm)
                {
                    vm.IsDeleteChecked = false;
                });
                this.SettledProductsGrid.TotalCount = this.GetherInfoVM.GatherSettlementItemInfoList.Count;

                this.SettledProductsGrid.ItemsSource = this.GetherInfoVM.GatherSettlementItemInfoList.Skip(SettledProductsGrid.PageIndex * SettledProductsGrid.PageSize).Take(SettledProductsGrid.PageSize).ToList();
            }
        }

        private void btnDeleteSettledProducts_Click(object sender, RoutedEventArgs e)
        {
            //删除结算商品:
            List<GatherSettlementItemInfoVM> getItemVM = this.SettledProductsGrid.ItemsSource as List<GatherSettlementItemInfoVM>;
            if (null != getItemVM)
            {
                GatherSettlementInfoVM updateVM = new GatherSettlementInfoVM();
                updateVM = EntityConverter<GatherSettlementInfoVM, GatherSettlementInfoVM>.Convert(GetherInfoVM);
                updateVM.GatherSettlementItemInfoList.Clear();
                getItemVM.ForEach(delegate(GatherSettlementItemInfoVM vm)
                {
                    if (vm.IsDeleteChecked)
                    {
                        updateVM.GatherSettlementItemInfoList.Add(vm);
                    }
                });
                if (0 >= updateVM.GatherSettlementItemInfoList.Count)
                {
                    Window.Alert(ResGatherMaintain.AlertMsg_Delete);
                    return;
                }

                Window.Confirm(ResGatherMaintain.AlertMsg_AlertTitle, ResGatherMaintain.AlertMsg_Save, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {

                        serviceFacade.UpdateGatherSettlementInfo(BuildEntity(updateVM), (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            AlertAndRefreshPage(ResGatherMaintain.AlertMsg_SaveSuc);
                        });
                    }
                });
            }
            else
            {
                Window.Alert(ResGatherMaintain.AlertMsg_Delete);
                return;
            }
        }

        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            //作废:
            Window.Confirm(ResGatherMaintain.AlertMsg_AlertTitle, ResGatherMaintain.AlertMsg_Abandon, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        serviceFacade.AbandonGatherSettlementInfo(BuildEntity(GetherInfoVM), (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            AlertAndRefreshPage(ResGatherMaintain.AlertMsg_OperateSuc);
                        });
                    }
                });
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            // 审核:
            Window.Confirm(ResGatherMaintain.AlertMsg_AlertTitle, ResGatherMaintain.AlertMsg_Audit, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    serviceFacade.AuditGatherSettlementInfo(BuildEntity(GetherInfoVM), (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResGatherMaintain.AlertMsg_OperateSuc);
                    });
                }
            });
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResGatherMaintain.AlertMsg_AlertTitle, ResGatherMaintain.AlertMsg_CancelAudit, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    serviceFacade.CancelAuditGatherSettlementInfo(BuildEntity(GetherInfoVM), (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResGatherMaintain.AlertMsg_OperateSuc);
                    });
                }
            });
        }

        private void btnSettle_Click(object sender, RoutedEventArgs e)
        {
            //结算:
            Window.Confirm(ResGatherMaintain.AlertMsg_AlertTitle, ResGatherMaintain.AlertMsg_Settle, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    serviceFacade.SettleGatherSettlementInfo(BuildEntity(GetherInfoVM), (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResGatherMaintain.AlertMsg_OperateSuc);
                    });
                }
            });
        }

        private void btnCancelSettle_Click(object sender, RoutedEventArgs e)
        {
            //取消结算:
            Window.Confirm(ResGatherMaintain.AlertMsg_AlertTitle, ResGatherMaintain.AlertMsg_CancelSettle, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    serviceFacade.CancelSettleGatherSettlementInfo(BuildEntity(GetherInfoVM), (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResGatherMaintain.AlertMsg_OperateSuc);
                    });
                }
            });
        }

        private void itemChecked_Click(object sender, RoutedEventArgs e)
        {
            object obj = e.OriginalSource;

            GatherSettlementItemInfoVM getCurrentVM = this.SettledProductsGrid.SelectedItem as GatherSettlementItemInfoVM;
            if (null != getCurrentVM)
            {
                List<GatherSettlementItemInfoVM> getGridVMList = this.SettledProductsGrid.ItemsSource as List<GatherSettlementItemInfoVM>;
                if (null != getGridVMList)
                {
                    getGridVMList.ForEach(delegate(GatherSettlementItemInfoVM vm)
                    {
                        if (vm.ItemSysNo != getCurrentVM.ItemSysNo)
                        {
                            if (vm.SettleType == getCurrentVM.SettleType && vm.OrderSysNo == getCurrentVM.OrderSysNo)
                            {
                                vm.IsDeleteChecked = getCurrentVM.IsDeleteChecked;
                            }
                        }
                    });
                }
            }
        }
        #endregion

        private void ShowActionButtons(GatherSettleStatus status)
        {
            switch (status)
            {
                //已作废状态，不显示任何按钮 :
                case GatherSettleStatus.ABD:
                    break;
                //待审核状态，显示 删除 作废 审核 按钮:
                case GatherSettleStatus.ORG:
                    this.btnDeleteSettledProducts.Visibility = Visibility.Visible;
                    this.btnAbandon.Visibility = Visibility.Visible;
                    this.btnAudit.Visibility = Visibility.Visible;
                    break;
                //已审核状态，显示 结算 取消审核 按钮:
                case GatherSettleStatus.AUD:
                    this.btnSettle.Visibility = Visibility.Visible;
                    this.btnCancelAudit.Visibility = Visibility.Visible;
                    break;
                //已结算状态，显示 取消结算  按钮:
                case GatherSettleStatus.SET:
                    this.SettledProductsGrid.Columns[0].Visibility = Visibility.Collapsed;
                    this.btnCancelSettle.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private GatherSettlementInfo BuildEntity(GatherSettlementInfoVM vm)
        {
            return EntityConverter<GatherSettlementInfoVM, GatherSettlementInfo>.Convert(vm, (s, t) =>
            {
                t.SourceStockInfo = new BizEntity.Inventory.StockInfo()
                {
                    SysNo = Convert.ToInt32(s.StockID),
                    StockName = s.StockName
                };
            });
        }

        private void AlertAndRefreshPage(string alertString)
        {
            Window.Alert(ResGatherMaintain.AlertMsg_AlertTitle, alertString, MessageType.Information, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.Cancel)
                {
                    //Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/GatherMaintain/{0}", GatherSysNo), false);
                    Window.Refresh();
                }
            });
        }

        private void hlbtnSysNo_Click(object sender, RoutedEventArgs e)
        {
            var item = ((HyperlinkButton)sender).Tag as GatherSettlementItemInfoVM;
            if (item != null)
            {
                switch (item.SettleType)
                {
                    case GatherSettleType.SO:
                        Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, item.InvoiceNumber), null, true);
                        break;
                    case GatherSettleType.RMA:
                        Window.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, item.InvoiceNumber), null, true);
                        break;
                    //case GatherSettleType.RO_Adjust:
                    //    Window.Navigate(string.Format(ConstValue.Customer_RefundAdjustUrl, item.InvoiceNumber), null, true);
                    //    break;
                    default:
                        break;
                }
            }
        }
    }

}
