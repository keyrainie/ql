using System;
using System.Collections.Generic;
using System.Threading;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.UserControls;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.Basic;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class RegisterMaintain : PageBase
    {
        private int loadCompletedCount = 0;        
        private RegisterFacade facade;
        private CommonDataFacade commonFacade;
        private CustomerContactFacade contactFacade;
        private List<CodeNamePair> inspectionResultType;
        private List<CodeNamePair> vendorRepairResultType;
        private List<StockInfo> stocks;
        private List<RegisterSecondHandRspVM> secondHandList;
        private List<CodeNamePair> refundPayType;
        private int sysNo;
        private RegisterVM vm;

        public RegisterMaintain()
        {
            InitializeComponent();            
        }       

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            facade = new RegisterFacade(this);
            commonFacade = new CommonDataFacade(this);
            contactFacade = new CustomerContactFacade(this);

            string no = Request.Param;            
            if (!string.IsNullOrEmpty(no))
            {
                int.TryParse(no, out sysNo);               
            }
            else
            {
                this.DataContext = new RegisterVM();
            }
            GetRefundPayTypes();
            GetInspectionResultTypes();
            GetVendorRepairResultTypes();
            GetStocks();
            LoadRegister();

            this.ucRMATrackingInfo.BindData(sysNo);

            this.ucRevertInfo.Page = this;
        }

        #region Private methods

        private void GetInspectionResultTypes()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "InspectionResultType", CodeNamePairAppendItemType.Select, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                inspectionResultType = args.Result;

                SetDataContext();
            });
        }

        private void GetVendorRepairResultTypes()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "VendorRepairResultType", CodeNamePairAppendItemType.Select, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                vendorRepairResultType = args.Result;

                SetDataContext();
            });
        }      

        private void GetStocks()
        {
            commonFacade.GetStockList(false, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                stocks = args.Result;
                stocks.Insert(0, new StockInfo { StockName = ResCommonEnum.Enum_Select });

                SetDataContext();
            });
        }

        private void GetRefundPayTypes()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "RefundPayTypes", CodeNamePairAppendItemType.Select, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                this.refundPayType = args.Result;
                SetDataContext();
            });
          
        }

        private void SetDataContext()
        {
            Interlocked.Increment(ref loadCompletedCount);
            //所有6个列表服务查询完成后
            if (loadCompletedCount ==6)
            {
                this.vm.BasicInfo.Stocks = this.stocks;
                this.vm.CheckInfo.InspectionResultTypes = this.inspectionResultType;
                this.vm.ResponseInfo.VendorRepairResultTypes = this.vendorRepairResultType;
                this.vm.RevertInfo.Stocks = this.stocks;                
                this.vm.RevertInfo.SecondhandList = this.secondHandList;       
                this.DataContext = this.vm;             
                ucRevertInfo.IsLoadCompleted = true;
                //此句上下文绑定不出来
                this.vm.ContactInfo.RefundPayTypes = this.refundPayType;
                //奇怪的双向绑定，属性有值内容绑不出来,无奈手动赋值 by freegod。
                ucContactInfo.RefundPayTypes.ItemsSource= this.refundPayType;
                SetButtonStatus();
            }
        }

        private void LoadRegister()
        {
            if (this.sysNo > 0)
            {
                facade.LoadBySysNo(this.sysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    this.vm = args.Result;

                    //给产品名字后边写延保、赠品、附件等属性
                    if (vm.BasicInfo.SOItemType.HasValue)
                    {
                        string tmpGiftName = string.Empty;
                        switch (vm.BasicInfo.SOItemType)
                        {
                            case ECCentral.BizEntity.SO.SOProductType.Gift:
                                tmpGiftName = ResCreateRequest.ESOItemType_Gift_Factory;
                                break;
                            case ECCentral.BizEntity.SO.SOProductType.SelfGift:
                                tmpGiftName = ResCreateRequest.ESOItemType_Gift_Newegg;
                                break;
                            case ECCentral.BizEntity.SO.SOProductType.Accessory:
                                tmpGiftName = ResCreateRequest.ESOItemType_Accessory;
                                break;
                            case ECCentral.BizEntity.SO.SOProductType.ExtendWarranty:
                                tmpGiftName = ResCreateRequest.ESOItemType_ExtendWarranty;
                                break;
                        }
                        if (!tmpGiftName.Equals(string.Empty))
                            vm.BasicInfo.ProductName = string.Format("{0}[{1}]", vm.BasicInfo.ProductName, tmpGiftName);
                    }
                    GetSecondHandProducts();
                    SetDataContext();

                    //Get Customer Contact Info
                    contactFacade.LoadByRequestSysNo(this.vm.BasicInfo.RequestSysNo.Value, (s, e) =>
                    {
                        this.vm.ContactInfo = e.Result.Convert<CustomerContactInfo, CustomerContactVM>();

                        this.vm.CheckInfo.ValidationErrors.Clear();
                        this.vm.ResponseInfo.ValidationErrors.Clear();
                        this.vm.RevertInfo.ValidationErrors.Clear();
                        this.vm.ContactInfo.ValidationErrors.Clear();
                    });

                    contactFacade.LoadOriginByRequestSysNo(this.vm.BasicInfo.RequestSysNo.Value, (s, e) =>
                    {
                        this.vm.OriginContactInfo = e.Result.Convert<CustomerContactInfo, CustomerContactVM>();
                    });
                });
            }
        }

        private void GetSecondHandProducts()
        {
            facade.LoadSecondHandProducts(vm.BasicInfo.ProductID, (obj, args) =>
            {              
                if (args.FaultsHandle())
                {                   
                    return;
                }
                secondHandList = args.Result;
                RegisterSecondHandRspVM tmp = new RegisterSecondHandRspVM() { ProductID = ResCommonEnum.Enum_Select };
                secondHandList.Insert(0, tmp);
                SetDataContext();
            });
        }

        /// <summary>
        /// 通过状态和权限控制按钮是否可用
        /// </summary>
        public void SetButtonStatus()
        {
            btnSetWaitingOutbound.IsEnabled = false;
            btnCancelWaitingOutbound.IsEnabled = false;
            btnSetWaitingRevert.IsEnabled = false;
            btnCancelWaitingRevert.IsEnabled = false;
            btnSetWaitingRefund.IsEnabled = false;
            btnCancelWaitingRefund.IsEnabled = false;
            btnCloseRMA.IsEnabled = false;
            btnCloseCase.IsEnabled = false;
            btnReOpenRMA.IsEnabled = false;
            btnSetAbandon.IsEnabled = false;
            btnSyncERP.IsEnabled = false;

            RegisterVM vm = this.DataContext as RegisterVM;

            btnSetWaitingOutbound.IsEnabled = vm.BasicInfo.OutBoundStatus == null
                && vm.BasicInfo.Status== RMARequestStatus.Handling
                && vm.BasicInfo.ProcessType != ProcessType.MET
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanSetWaitingOutBound);
            if (vm.BasicInfo.InventoryType != ProductInventoryType.Normal)
            {
                //非0模式，一直禁用
                btnSetWaitingOutbound.IsEnabled = false;
            }

            btnCancelWaitingOutbound.IsEnabled = vm.BasicInfo.OutBoundStatus == RMAOutBoundStatus.Origin
                && vm.BasicInfo.Status == RMARequestStatus.Handling
                && vm.BasicInfo.ProcessType != ProcessType.MET
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanSetWaitingOutBound);

            btnSetWaitingRevert.IsEnabled = vm.RevertInfo.RevertStatus == null
                && vm.BasicInfo.Status == RMARequestStatus.Handling
                && vm.BasicInfo.ProcessType != ProcessType.MET
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanWaitingRevert);
            btnSetWaitingRefund.IsEnabled = vm.BasicInfo.RefundStatus == null
                && vm.BasicInfo.Status == RMARequestStatus.Handling
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanWaitingRefund);

            if (vm.BasicInfo.InventoryType != ProductInventoryType.Normal)
            {
                if (vm.RequestType == RMARequestType.Exchange)
                {
                    //btnSetWaitingRevert.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanWaitingRevert);
                    btnSetWaitingRefund.IsEnabled = false;
                }
            }

            btnCancelWaitingRevert.IsEnabled = (vm.RevertInfo.RevertStatus == RMARevertStatus.WaitingRevert || vm.RevertInfo.RevertStatus == RMARevertStatus.WaitingAudit)
                && vm.BasicInfo.Status == RMARequestStatus.Handling
                && vm.BasicInfo.ProcessType != ProcessType.MET
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanWaitingRevert);

            if (vm.BasicInfo.InventoryType != ProductInventoryType.Normal)
            {
                if (vm.RequestType == RMARequestType.Return)
                {
                    btnSetWaitingRevert.IsEnabled = false;
                    //btnSetWaitingRefund.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanWaitingRefund);
                }
            }

            //根据库存模式和ERP控制待退款
            //ERP退货，同步后可用，ERP换货，一直不可用
            if ( (vm.BasicInfo.InventoryType == ProductInventoryType.Company
                 || vm.BasicInfo.InventoryType == ProductInventoryType.TwoDoor)
                 &&
                 (vm.BasicInfo.ERPStatus == ERPReturnType.UnReturn && vm.RequestType== RMARequestType.Return
                || vm.RequestType == RMARequestType.Exchange))
            {
                btnSetWaitingRefund.IsEnabled = false;
            }          

            btnCancelWaitingRefund.IsEnabled = vm.BasicInfo.RefundStatus == RMARefundStatus.WaitingRefund
                && vm.BasicInfo.Status == RMARequestStatus.Handling
                //&& vm.BasicInfo.ProcessType == ProcessType.NEG
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanWaitingRefund);

            btnCloseRMA.IsEnabled = vm.BasicInfo.Status == RMARequestStatus.Handling
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanClose);

            btnCloseCase.IsEnabled = vm.BasicInfo.ProcessType == ProcessType.MET
                && vm.BasicInfo.Status == RMARequestStatus.Handling
                && vm.BasicInfo.RefundStatus != RMARefundStatus.WaitingRefund
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_RegisterCloseCase);

            //ERP模式的关闭单件处理
            btnCloseCase.IsEnabled=((vm.BasicInfo.InventoryType == ProductInventoryType.Company
                || vm.BasicInfo.InventoryType == ProductInventoryType.TwoDoor)
                && vm.BasicInfo.ERPStatus == ERPReturnType.Return
                && vm.BasicInfo.Status != RMARequestStatus.Complete);
           
            btnReOpenRMA.IsEnabled = vm.BasicInfo.Status == RMARequestStatus.Complete
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanReOpen);

            btnSetAbandon.IsEnabled = vm.BasicInfo.Status == RMARequestStatus.Origin
                && vm.BasicInfo.ProcessType == ProcessType.MET                
                && vm.SellerInfo.SellerReceived.ToUpper() == "N"
                && AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_CanSetAbandon);

            //btnSyncERP.IsEnabled = ((vm.BasicInfo.InventoryType == ProductInventoryType.Company
            //     || vm.BasicInfo.InventoryType == ProductInventoryType.TwoDoor)
            //     && vm.BasicInfo.ERPStatus == ERPReturnType.UnReturn);
	       
            this.ucBasicInfo.SetButtonStatus();
            this.ucCheckInfo.SetButtonStatus();
            this.ucResponseInfo.SetButtonStatus();
            this.ucRevertInfo.SetButtonStatus();
        }       

        #endregion

        #region EventHandler

        private void btnSetWaitingReturn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.SetWaitingReturn(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.ReturnStatus = args.Result.BasicInfo.ReturnStatus;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnCancelWaitingReturn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.CancelWaitingReturn(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.ReturnStatus = null;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnSetWaitingOutbound_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.SetWaitingOutbound(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.OutBoundStatus = args.Result.BasicInfo.OutBoundStatus;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnCancelWaitingOutbound_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.CancelWaitingOutbound(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.OutBoundStatus = null;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnSetWaitingRevert_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.SetWaitingRevert(vm, (obj, args) =>
            {
                vm.RevertInfo.RevertStatus = args.Result.RevertInfo.RevertStatus;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnCancelWaitingRevert_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.CancelWaitingRevert(vm, (obj, args) =>
            {
                vm.RevertInfo.RevertStatus = null;
                vm.RevertInfo.RevertAuditMemo = string.Empty;
                vm.RevertInfo.RevertAuditUserName = string.Empty;
                vm.RevertInfo.RevertAuditTime = null;
                vm.RevertInfo.RevertAuditUserSysNo = null;
                
                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnSetWaitingRefund_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.SetWaitingRefund(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.RefundStatus = args.Result.BasicInfo.RefundStatus;

                SetButtonStatus();

                string url = string.Format(ConstValue.RMA_CreateRefundUrl + "/{0}", vm.SysNo);

                //Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);

                Window.Navigate(url, null, true);                
            });
        }

        private void btnCancelWaitingRefund_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.CancelWaitingRefund(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.RefundStatus = null;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnReOpenRMA_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.ReOpen(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.Status = args.Result.BasicInfo.Status;
                vm.BasicInfo.CloseTime = null;
                vm.BasicInfo.CloseUserName = string.Empty;
                vm.BasicInfo.CloseUserSysNo = null;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnCloseRMA_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.Close(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.Status = args.Result.BasicInfo.Status;
                vm.BasicInfo.CloseTime = args.Result.BasicInfo.CloseTime;
                vm.BasicInfo.CloseUserName = args.Result.BasicInfo.CloseUserName;
                vm.BasicInfo.CloseUserSysNo = args.Result.BasicInfo.CloseUserSysNo;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnCloseCase_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RegisterVM vm = this.DataContext as RegisterVM;
            UCCloseRegisterCase uc = new UCCloseRegisterCase();
            uc.DataContext = vm;
            IDialog dialog = Window.ShowDialog(ResRegisterMaintain.PopTitle_CloseCase, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
                }
                
                SetButtonStatus();
            });
            uc.Dialog = dialog;           
        }

        private void btnSetAbandon_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.Abandon(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.Status = args.Result.BasicInfo.Status;

                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        private void btnSyncERP_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as RegisterVM;
            facade.SyncERP(vm.SysNo.Value, (obj, args) =>
            {
                vm.BasicInfo.ERPStatus = args.Result.BasicInfo.ERPStatus;
                SetButtonStatus();

                Window.Alert(ResRegisterMaintain.Info_OperateSuccessfully);
            });
        }

        
        #endregion
    }
}
