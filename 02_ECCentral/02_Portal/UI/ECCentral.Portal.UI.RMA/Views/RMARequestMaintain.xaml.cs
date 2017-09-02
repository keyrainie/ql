using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.RMA.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class RMARequestMaintain : PageBase
    {
        private RequestFacade facade;
        private CommonDataFacade commonFacade;
        private int loadCompletedCount = 0;
        private int sysNo;
        private RequestVM vm;
        private List<StockInfo> stocks;

        public RMARequestMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            facade = new RequestFacade(this);
            commonFacade = new CommonDataFacade(this);

            string no = Request.Param;
            if (!string.IsNullOrEmpty(no))
            {
                int.TryParse(no, out sysNo);
            }
            else
            {
                this.DataContext = new RequestVM();
            }

            LoadStocks();

            LoadRequest();
        }

        private void LoadStocks()
        {
            commonFacade.GetStockList(false, (o, a) =>
            {
                this.stocks = a.Result;
                this.stocks.Insert(0, new StockInfo { StockName = ResCommonEnum.Enum_Select });

                SetDataContext();
            });
        }

        private void LoadRequest()
        {
            facade.LoadBySysNo(sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                this.vm = args.Result;

                foreach (RegisterVM rvm in vm.Registers)
                {
                    string tmpGiftName = string.Empty;
                    if (rvm.BasicInfo.SOItemType.HasValue)
                    {
                        switch (rvm.BasicInfo.SOItemType)
                        {
                            case SOProductType.Gift:
                                tmpGiftName = ResCreateRequest.ESOItemType_Gift_Factory;
                                break;
                            case SOProductType.SelfGift:
                                tmpGiftName = ResCreateRequest.ESOItemType_Gift_Newegg;
                                break;
                            case SOProductType.Accessory:
                                tmpGiftName = ResCreateRequest.ESOItemType_Accessory;
                                break;
                            case SOProductType.ExtendWarranty:
                                tmpGiftName = ResCreateRequest.ESOItemType_ExtendWarranty;
                                break;
                        }
                        if (!tmpGiftName.Equals(string.Empty))
                            rvm.BasicInfo.ProductName = string.Format("{0}[{1}]", rvm.BasicInfo.ProductName, tmpGiftName);
                    }
                }

                SetDataContext();
            });
        }

        private void SetDataContext()
        {
            Interlocked.Increment(ref loadCompletedCount);
            if (loadCompletedCount == 2)
            {
                this.vm.Stocks = this.stocks;
                this.vm.ValidationErrors.Clear();

                this.DataContext = this.vm;

                SetBtnStatus();
            }
        }

        private void SetBtnStatus()
        {
            RequestVM vm = this.DataContext as RequestVM;
            RMARequestStatus? status = vm.Status;
            btnUpdate.IsEnabled = false;
            btnAbandon.IsEnabled = false;
            btnReceive.IsEnabled = false;
            btnCancelReceive.IsEnabled = false;
            btnPrint.IsEnabled = false;
            btnPrintLabel.IsEnabled = false;
            btnClose.IsEnabled = false;
            btnAdjust.IsEnabled = false;
            btnRefused.IsEnabled = false;

            if (vm.InvoiceType == InvoiceType.SELF
                && vm.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.SELF
                && vm.StockType == StockType.SELF)
            {
                btnPrintLabel.IsEnabled = false;
            }
            else
            {
                btnPrintLabel.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_PrintLabel);
            }

            if (status == RMARequestStatus.Origin)
            {
                btnPrint.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_Print);
                btnUpdate.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanUpdate);
                btnReceive.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanReceive);
                btnCancelReceive.IsEnabled = false;
                btnAbandon.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanAbandon);
                btnClose.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanClose);
                btnAdjust.IsEnabled = false;
                btnRefused.IsEnabled = false;
            }
            else if (status == RMARequestStatus.Abandon || status == RMARequestStatus.Complete || status == RMARequestStatus.AuditRefuesed)
            {
                btnPrint.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_Print);
            }
            else if (status == RMARequestStatus.Handling)
            {
                btnPrint.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_Print);
                btnReceive.IsEnabled = false;
                btnCancelReceive.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanCancelReceive);
                btnClose.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanClose);
            }
            else if (status == RMARequestStatus.WaitingAudit)
            {
                btnPrint.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_Print);
                btnUpdate.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanUpdate);
                btnAdjust.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanAdjust);
                btnRefused.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanRefused);
                btnAbandon.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Request_CanAbandon);
                btnReceive.IsEnabled = false;
                btnCancelReceive.IsEnabled = false;

            }
            //MET模式不支持Receive和CancelReceive
            if ((vm.StockType == StockType.MET && vm.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.SELF && vm.InvoiceType == InvoiceType.SELF)
                || (vm.StockType == StockType.MET && vm.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.SELF && vm.InvoiceType == InvoiceType.MET)
                || (vm.StockType == StockType.MET && vm.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.MET && vm.InvoiceType == InvoiceType.SELF)
                || (vm.StockType == StockType.MET && vm.ShippingType == ECCentral.BizEntity.Invoice.DeliveryType.MET && vm.InvoiceType == InvoiceType.MET))
            {
                btnReceive.IsEnabled = false;
                btnCancelReceive.IsEnabled = false;
            }
        }

        private void btnReceive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ValidationManager.Validate(ucRequestBasicInfo))
            {
                var vm = this.DataContext as RequestVM;
                facade.Receive(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm.Status = RMARequestStatus.Handling;
                    vm.ReceiveUserName = args.Result.ReceiveUserName;
                    vm.ReceiveTime = args.Result.ReceiveTime;                 
                    foreach (var item in vm.Registers)
                    {
                        item.BasicInfo.Status = vm.Status;
                    }

                    SetBtnStatus();

                    Window.Alert(ResRequestMaintain.Info_OperateSuccessfully);
                });
            }
        }

        private void btnCancelReceive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RequestVM;
            facade.CancelReceive(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                vm.Status = RMARequestStatus.Origin;
                vm.ReceiveUserName = string.Empty;
                vm.ReceiveTime = null;               
                foreach (var item in vm.Registers)
                {
                    item.BasicInfo.Status = vm.Status;
                }

                SetBtnStatus();

                Window.Alert(ResRequestMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //todo:目前的关闭按钮作用主要是用于批量关闭单件，目前对于Seller申请单处理有问题暂时隐藏
            var vm = this.DataContext as RequestVM;
            facade.Close(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                vm.Status = RMARequestStatus.Complete;

                SetBtnStatus();

                Window.Alert(ResRequestMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnPrint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RequestVM;
            var para = new Dictionary<string, string>();
            para.Add("SysNo", vm.SysNo.ToString());
            HtmlViewHelper.WebPrintPreview(ConstValue.DomainName_RMA, "PrintRequest", para);
        }

        private void btnPrintLabel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RequestVM;
            var para = new Dictionary<string, string>();
            para.Add("SysNo", vm.SysNo.ToString());
            HtmlViewHelper.WebPrintPreview(ConstValue.DomainName_RMA, "PrintRequestLabel", para);
        }

        private void btnUpdate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ValidationManager.Validate(ucRequestBasicInfo))
            {
                var vm = this.DataContext as RequestVM;

                if (!vm.CustomerSendTime.HasValue)
                {
                    Window.Alert(ResRequestMaintain.Info_NeedCustomerSendTime, MessageType.Information);
                    return;
                }

                facade.Update(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }                 
                    Window.Alert("提示", ResRequestMaintain.Info_OperateSuccessfully, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj1, args1) =>
                    {
                        if (args1.DialogResult == DialogResultType.Cancel)
                        {
                            Window.Refresh();
                        }
                    });
                });
            }
        }

        private void btnAbandon_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RequestVM;
            facade.Abandon(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                vm.Status = RMARequestStatus.Abandon;

                SetBtnStatus();

                Window.Alert(ResRequestMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnEditRegister_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as RegisterVM;
            string url = string.Format(ConstValue.RMA_RegisterMaintainUrl, vm.BasicInfo.SysNo);
            Window.Navigate(url, null, true);
        }

        private void btnProduct_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var d = (sender as HyperlinkButton).DataContext as RegisterVM;
            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
            Window.Navigate(string.Format(urlFormat, d.BasicInfo.ProductSysNo), null, true);
        }

        private void btnAdjust_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RequestVM;
            facade.AuditPassed(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                vm.Status = RMARequestStatus.Origin;
                vm.ServiceCode = args.Result.ServiceCode;

                SetBtnStatus();

                Window.Alert(ResRequestMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnRefused_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RequestVM;
            facade.AuditRefused(vm, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                vm.Status = RMARequestStatus.AuditRefuesed;

                SetBtnStatus();

                Window.Alert(ResRequestMaintain.Info_OperateSuccessfully);
            });
        }
    }
}
