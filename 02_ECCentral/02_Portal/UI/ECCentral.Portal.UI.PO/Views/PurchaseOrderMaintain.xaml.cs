using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.Service.PO.Restful.RequestMsg;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class PurchaseOrderMaintain : PageBase
    {

        public string POSysNo;

        public PurchaseOrderInfoVM poInfoVM;
        public PurchaseOrderFacade serviceFacade;
        public VendorFacade vendorFacade;

        #region [自动发送Email相关]
        public string AutoMail_AutoMailAddress = string.Empty;
        public string AutoMail_VendorMailAddress = string.Empty;
        public string AutoMail_HaveSentMailAddress = string.Empty;
        #endregion

        public PurchaseOrderMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            poInfoVM = new PurchaseOrderInfoVM();
            serviceFacade = new PurchaseOrderFacade(this);
            vendorFacade = new VendorFacade(this);
            POSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(POSysNo))
            {
                LoadComboBoxData();
            }
        }

        private void LoadComboBoxData()
        {
            //增值税率:
            this.cmbTaxRate.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderTaxRate>(EnumConverter.EnumAppendItemType.None);
            //预计到货时间段:
            this.cmbETAHalfDay.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderETAHalfDayType>(EnumConverter.EnumAppendItemType.Select);
            //this.cmbLeaseFlag.SelectedIndex =1;
            //是否转租赁
            this.cmbLeaseFlag.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderLeaseFlag>(EnumConverter.EnumAppendItemType.None);
            //仓库:
            serviceFacade.GetPurchaseOrderWarehouseList((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.cmbStock.ItemsSource = args.Result;
                this.cmbTransferType.ItemsSource = EnumConverter.GetKeyValuePairs<TransferType>(EnumConverter.EnumAppendItemType.None);
                this.cmbTransfer.ItemsSource = EnumConverter.GetKeyValuePairs<PaySettleITCompany>(EnumConverter.EnumAppendItemType.None);
                this.cmbTransfer.SelectedIndex = 0;
                //加载采购单信息:
                LoadPOInfo();
            });
        }

        private void LoadPOInfo()
        {
            //获取PO单信息:
            serviceFacade.LoadPurchaseOrderInfo(POSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                //转换Entity to ViewModel:
                this.poInfoVM = EntityConverter<PurchaseOrderInfo, PurchaseOrderInfoVM>.Convert(args.Result, (s, t) =>
                {
                    t.VendorInfo.VendorBasicInfo.PaySettleCompany = s.VendorInfo.VendorBasicInfo.PaySettleCompany;
                    t.PurchaseOrderBasicInfo.StockSysNo = s.PurchaseOrderBasicInfo.StockInfo.SysNo;
                    t.PurchaseOrderBasicInfo.StockName = s.PurchaseOrderBasicInfo.StockInfo.StockName;
                    t.PurchaseOrderBasicInfo.ITStockSysNo = s.PurchaseOrderBasicInfo.ITStockInfo.SysNo;
                    t.PurchaseOrderBasicInfo.ITStockName = s.PurchaseOrderBasicInfo.ITStockInfo.StockName;
                    t.PurchaseOrderBasicInfo.PMSysNo = s.PurchaseOrderBasicInfo.ProductManager.SysNo.IntegerToString();
                    t.PurchaseOrderBasicInfo.PMName = s.PurchaseOrderBasicInfo.ProductManager.UserInfo.UserName;
                    t.PurchaseOrderBasicInfo.ShippingTypeSysNo = s.PurchaseOrderBasicInfo.ShippingType.SysNo.IntegerToString();
                    t.PurchaseOrderBasicInfo.ShippingTypeName = s.PurchaseOrderBasicInfo.ShippingType.ShippingTypeName;
                    t.PurchaseOrderBasicInfo.PayTypeSysNo = s.PurchaseOrderBasicInfo.PayType.SysNo;
                    t.PurchaseOrderBasicInfo.PayTypeName = s.PurchaseOrderBasicInfo.PayType.PayTypeName;
                    t.PurchaseOrderBasicInfo.LogisticsNumber = s.PurchaseOrderBasicInfo.LogisticsNumber;
                    t.PurchaseOrderBasicInfo.ExpressName = s.PurchaseOrderBasicInfo.ExpressName;
                    if (s.PurchaseOrderBasicInfo.ETATimeInfo.InDate.HasValue && !string.IsNullOrEmpty(s.PurchaseOrderBasicInfo.ETATimeInfo.Memo))
                    {
                        t.PurchaseOrderBasicInfo.ETATimeInfo.Memo = string.Format("[{0}]:{1}", s.PurchaseOrderBasicInfo.ETATimeInfo.InDate.Value.ToString("yyyy/MM/dd HH:mm:ss"), s.PurchaseOrderBasicInfo.ETATimeInfo.Memo);
                    }

                    for (int i = 0; i < s.VendorInfo.VendorAgentInfoList.Count; i++)
                    {
                        t.VendorInfo.VendorAgentInfoList[i].ManufacturerInfo.ManufacturerNameDisplay = s.VendorInfo.VendorAgentInfoList[i].ManufacturerInfo.ManufacturerNameLocal.Content;
                        t.VendorInfo.VendorAgentInfoList[i].BrandInfo.BrandNameDisplay = s.VendorInfo.VendorAgentInfoList[i].BrandInfo.BrandNameLocal.Content;
                    }                  
                    //采购仓库的显示部分
                    cmbTransferType.SelectedValue = t.PurchaseOrderBasicInfo.ITStockSysNo.HasValue ? TransferType.Indirect : TransferType.Direct;
                    var transferType = (TransferType)cmbTransferType.SelectedValue;
                    int stockSysNo = t.PurchaseOrderBasicInfo.StockSysNo ?? 0;
                    if (stockSysNo > 0)
                    {
                        if (transferType == TransferType.Indirect)
                        {
                            cmbTransfer.Visibility = System.Windows.Visibility.Visible;
                            cmbTransfer.SelectedValue = Enum.Parse(typeof(PaySettleITCompany), t.VendorInfo.VendorBasicInfo.PaySettleCompany.ToString(), true);
                            t.PurchaseOrderBasicInfo.StockSysNo = t.PurchaseOrderBasicInfo.ITStockSysNo;
                        }
                    }
                });

                this.DataContext = poInfoVM;
                this.gridVendorAgentInfo.Bind();
                this.gridEIMSInfo.Bind();
                //采购单位退回或者创建状态，显示返点信息的删除按钮 :
                if (null != poInfoVM.EIMSInfo.EIMSInfoList && 0 < poInfoVM.EIMSInfo.EIMSInfoList.Count && (poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned || poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created))
                {
                    this.gridEIMSInfo.Columns[4].Visibility = Visibility.Visible;
                }
                RefreshEIMSTotalText();
                //绑定商品信息:
                this.gridProductsListInfo.Bind();

                //绑定收货信息(如果存在相关数据):
                if (null != this.poInfoVM.ReceivedInfoList && 0 < this.poInfoVM.ReceivedInfoList.Count)
                {
                    this.ProductsReceiptInfo.Visibility = Visibility.Visible;
                    this.gridProductsReceiptInfo.Bind();
                }

                //根据单据状态，Enable按钮:
                ShowActionButtons();

                AutoMail_VendorMailAddress = poInfoVM.VendorInfo.VendorBasicInfo.EmailAddress;
                AutoMail_HaveSentMailAddress = poInfoVM.PurchaseOrderBasicInfo.MailAddress;

                //自动Email:
                if (!string.IsNullOrEmpty(poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress) && 0 < poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress.Length)
                {
                    this.chkAutoEmail.IsChecked = true;
                    BuildPOReceivedMailAddressToControl();
                    AutoMail_AutoMailAddress = poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress;
                }
                //送修超过15天催讨未果RMAList,请协助:
                this.hpkRMAOver15Days.Content = string.Format(ResPurchaseOrderMaintain.Label_RMARegisterText, poInfoVM.PurchaseOrderBasicInfo.ARMCount);
                
            });
        }

        private void ShowActionButtons()
        {
            //已退回，已创建，待PM审核，Enable 供应商选择:
            switch (this.poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderStatus.Value)
            {
                case PurchaseOrderStatus.Returned:
                    //已退回:
                    this.btnUpdate.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.btnReset.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.btnCheck.IsEnabled = true;
                    this.btnSubmitAudit.IsEnabled = true;
                    this.btnAbandon.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Abandon);
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;

                    this.cmbTaxRate.IsEnabled = true;
                    this.dpkETATime.IsEnabled = true;
                    this.cmbETAHalfDay.IsEnabled = true;
                    this.txtMemo.IsReadOnly = false;
                    this.txtNote.IsReadOnly = false;
                    // 显示编辑按钮
                    this.gridProductsListInfo.Columns[1].Visibility = Visibility.Visible;
                    //显示采购价:
                    this.gridProductsListInfo.Columns[9].Visibility = Visibility.Visible;
                    this.btnAddProduct.Visibility = Visibility.Visible;
                    this.btnDeleteProduct.Visibility = Visibility.Visible;
                    break;
                case PurchaseOrderStatus.AutoAbandoned:
                    //自动作废 :
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;
                    this.btnOperateReNewPO.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.ucVendor.IsAllowVendorSelect = false;
                    break;
                case PurchaseOrderStatus.Abandoned:
                    //已作废:
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;
                    this.btnOperateReNewPO.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.btnCancelAbandon.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Abandon);
                    this.ucVendor.IsAllowVendorSelect = false;
                    this.txtPMRequestMemo.IsEnabled = false;
                    break;
                case PurchaseOrderStatus.Created:
                    //已创建:
                    this.btnUpdate.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.btnReset.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.btnCheck.IsEnabled = true;
                    this.btnSubmitAudit.IsEnabled = true;
                    this.btnAbandon.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Abandon);
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;

                    this.cmbTaxRate.IsEnabled = true;
                    this.dpkETATime.IsEnabled = true;
                    this.cmbETAHalfDay.IsEnabled = true;
                    this.txtMemo.IsReadOnly = false;
                    this.txtNote.IsReadOnly = false;
                    this.hpkEimsInvoice.Visibility = Visibility.Visible;
                    // 显示编辑按钮:
                    this.gridProductsListInfo.Columns[1].Visibility = Visibility.Visible;
                    //显示采购价:
                    this.gridProductsListInfo.Columns[9].Visibility = Visibility.Visible;

                    this.btnAddProduct.Visibility = Visibility.Visible;
                    this.btnDeleteProduct.Visibility = Visibility.Visible;
                    break;
                //case PurchaseOrderStatus.WaitingApportion:
                //    //：待分摊
                //    this.txtNote.IsReadOnly = false;
                //    this.txtMemo.IsReadOnly = false;
                //    this.dpkETATime.IsEnabled = true;
                //    this.cmbETAHalfDay.IsEnabled = true;

                //    this.btnCheck.IsEnabled = true;
                //    break;
                case PurchaseOrderStatus.WaitingInStock:
                    //等待入库:
                    this.btnCheck.IsEnabled = true;
                    this.btnCancelConfirm.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_CancelVerify);
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;
                    this.btnOperateInStockMemoAndTotalAmt.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_UpdateInstockMemo);
                    this.ucVendor.IsAllowVendorSelect = false;

                    this.dpkETATime.IsEnabled = true;
                    this.cmbETAHalfDay.IsEnabled = true;
                    this.txtPMRequestMemo.IsReadOnly = false;
                    this.txtInStockMemo.IsReadOnly = false;
                    this.txtCarriageCost.IsReadOnly = false;

                    this.lblETAMemo.Visibility = Visibility.Visible;
                    this.txtETAMemo.Visibility = Visibility.Visible;
                    this.spETATimeActionButtons.Visibility = Visibility.Visible;

                    //有待审核的预计到货时间申请,则显示审核通过和取消审核按钮:
                    if (poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.Status == 1)
                    {
                        this.dpkETATime.IsEnabled = false;
                        this.cmbETAHalfDay.IsEnabled = false;
                        this.txtETAMemo.IsReadOnly = true;
                        this.btnPassAudit.IsEnabled = true;
                        this.btnCancelAudit.IsEnabled = true;
                    }
                    //没有审核的预计到货时间信息，则显示提交审核按钮:
                    else
                    {
                        this.btnRequestAudit.IsEnabled = true;
                    }
                    break;
                case PurchaseOrderStatus.InStocked:
                    //已入库，
                    this.btnCheck.IsEnabled = true;
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;
                    this.ucVendor.IsAllowVendorSelect = false;
                    break;
                case PurchaseOrderStatus.WaitingAudit:
                    //待审核:
                    this.btnCheck.IsEnabled = true;
                    this.btnConfirm.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Verify);
                    this.btnRefuseBack.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_SendBackVPPO);
                    this.ucVendor.IsAllowVendorSelect = false;
                    break;
                case PurchaseOrderStatus.PartlyInStocked:
                    //部分入库:
                    this.btnCheck.IsEnabled = true;
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;
                    this.btnOperatePartlyInStock.IsEnabled = true;
                    this.ucVendor.IsAllowVendorSelect = false;
                    break;
                case PurchaseOrderStatus.ManualClosed:
                    // 手动关闭:
                    this.btnCheck.IsEnabled = true;
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;
                    this.btnOperateReNewPO.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.ucVendor.IsAllowVendorSelect = false;
                    break;
                case PurchaseOrderStatus.SystemClosed:
                    //系统关闭:
                    this.btnCheck.IsEnabled = true;
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;
                    this.btnOperateReNewPO.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.ucVendor.IsAllowVendorSelect = false;
                    break;
                case PurchaseOrderStatus.VendorClosed:
                    //供应商关闭 ：
                    this.btnCheck.IsEnabled = true;
                    this.btnConfirmWithVendor.IsEnabled = true;
                    this.btnPrint.IsEnabled = true;
                    this.btnPrintAppend.IsEnabled = true;
                    this.btnOperateReNewPO.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_Update);
                    this.ucVendor.IsAllowVendorSelect = false;
                    break;
                case PurchaseOrderStatus.WaitingPMConfirm:
                    //待PM确认:
                    this.btnNewPO.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_ConfirmVPPO);
                    this.btnCancelPO.IsEnabled = true;

                    this.cmbTaxRate.IsEnabled = true;
                    this.dpkETATime.IsEnabled = true;
                    this.cmbETAHalfDay.IsEnabled = true;
                    this.txtMemo.IsReadOnly = false;
                    this.txtNote.IsReadOnly = false;
                    this.txtInStockMemo.IsReadOnly = false;
                    this.lblBackReasonText.Visibility = Visibility.Visible;
                    this.txtBackReason.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            //TODO;权限添加
            //  <% if (this.HasRight("CanViewPO") || currentPageIsNew)
            if (poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Normal || (poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created && poInfoVM.POItems.Count <= 0))
            {
                this.gridProductsListInfo.Columns[19].Visibility = Visibility.Visible;
                this.gridProductsListInfo.Columns[20].Visibility = Visibility.Visible;
                this.gridProductsListInfo.Columns[21].Visibility = Visibility.Visible;
                this.gridProductsListInfo.Columns[22].Visibility = Visibility.Visible;
            }

            //隐藏返点
            this.hpkEimsInvoice.Visibility = Visibility.Collapsed;

            //如果是负采购单，则显示批次信息:
            if (poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
            {
                //显示批次信息:
                this.gridProductsListInfo.Columns[2].Visibility = Visibility.Visible;               
            }
            else
            {
                //隐藏批次信息:
                this.gridProductsListInfo.Columns[2].Visibility = Visibility.Collapsed;

            }
        }

        #region [Events]

        private void ucVendor_VendorSelected(object sender, Basic.Components.UserControls.VendorPicker.VendorSelectedEventArgs e)
        {
            //修改供应商，重新选择供应商:
            if (null != e.SelectedVendorInfo)
            {
                poInfoVM.VendorInfo.VendorBasicInfo.VendorNameLocal = e.SelectedVendorInfo.VendorBasicInfo.VendorNameLocal;
                poInfoVM.VendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName = e.SelectedVendorInfo.VendorFinanceInfo.PayPeriodType.PayTermsName;
                poInfoVM.VendorInfo.VendorBasicInfo.VendorStatus = e.SelectedVendorInfo.VendorBasicInfo.VendorStatus;
                poInfoVM.VendorInfo.VendorAgentInfoList = EntityConverter<VendorAgentInfo, VendorAgentInfoVM>.Convert(e.SelectedVendorInfo.VendorAgentInfoList, (s, t) =>
                {
                    t.BrandInfo.BrandNameDisplay = s.BrandInfo.BrandNameLocal.Content;
                    t.ManufacturerInfo.ManufacturerNameDisplay = s.ManufacturerInfo.ManufacturerNameLocal.Content;
                });
                this.gridVendorAgentInfo.Bind();
            }
        }
        private void gridVendorAgentInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.gridVendorAgentInfo.ItemsSource = poInfoVM.VendorInfo.VendorAgentInfoList;
        }
        private void gridProductsReceiptInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.gridProductsReceiptInfo.ItemsSource = poInfoVM.ReceivedInfoList;
        }
        private void gridProductsListInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.gridProductsListInfo.ItemsSource = poInfoVM.POItems;

        }
        private void gridEIMSInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.gridEIMSInfo.ItemsSource = poInfoVM.EIMSInfo.EIMSInfoList;
        }
        private void chkAutoEmail_Click(object sender, RoutedEventArgs e)
        {
            if (this.chkAutoEmail.IsChecked == true)
            {
                //PO单:自动发送邮件地址:AutoMailAddress:
                string returnAutoSendMailResult = string.Empty;
                //PO单:已发送邮件MailAddress:
                string returnMailAddresResult = string.Empty;
                //供应商:Vendor MailAddress:
                string returnVendorMailResult = string.Empty;

                #region 获取、更新MailAddress:

                //自动Email:
                if (!string.IsNullOrEmpty(this.POSysNo))
                {
                    serviceFacade.LoadPurchaseOrderInfo(POSysNo, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        PurchaseOrderInfo getPOInfo = args.Result;
                        if (null != getPOInfo)
                        {
                            // 传入的Vendor是编辑页面的初始Vendor:
                            if (getPOInfo.VendorInfo.SysNo == this.poInfoVM.VendorInfo.SysNo)
                            {
                                if (!string.IsNullOrEmpty(getPOInfo.PurchaseOrderBasicInfo.AutoSendMailAddress))//AutoSendMail 不为空 只显示当前AutoSendMail 为空 显示 当前登录账户Email + Vendor 的Email
                                {
                                    returnAutoSendMailResult = getPOInfo.PurchaseOrderBasicInfo.AutoSendMailAddress;
                                }
                                else if (!string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && !string.IsNullOrEmpty(getPOInfo.VendorInfo.VendorBasicInfo.EmailAddress))
                                {
                                    returnAutoSendMailResult = CPApplication.Current.LoginUser.UserEmailAddress + ";" + getPOInfo.VendorInfo.VendorBasicInfo.EmailAddress;
                                }
                                else if (!string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && string.IsNullOrEmpty(getPOInfo.VendorInfo.VendorBasicInfo.EmailAddress))
                                {
                                    returnAutoSendMailResult = CPApplication.Current.LoginUser.UserEmailAddress;
                                }
                                else if (string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && !string.IsNullOrEmpty(getPOInfo.VendorInfo.VendorBasicInfo.EmailAddress))
                                {
                                    returnAutoSendMailResult = getPOInfo.VendorInfo.VendorBasicInfo.EmailAddress;
                                }
                                returnVendorMailResult = getPOInfo.VendorInfo.VendorBasicInfo.EmailAddress;
                            }
                            else// 传入的Vendor  不是   编辑页面的初始Vendor 是 一个新更换的 Vendor 此时 需要加载新Vendor的Email + 当前登陆账户的Email
                            {
                                vendorFacade.GetVendorBySysNo(poInfoVM.VendorInfo.SysNo.Value.ToString(), (obj3, args3) =>
                                {
                                    if (args.FaultsHandle())
                                    {
                                        return;
                                    }
                                    VendorInfo getVendorInfo = args3.Result;
                                    if (getVendorInfo != null)
                                    {
                                        //vendor 的Email和 当前登录账号的Email都不为空 显示    VendorEmail  + 当前登录账户Email
                                        if (!string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && !string.IsNullOrEmpty(getVendorInfo.VendorBasicInfo.EmailAddress))
                                        {
                                            returnAutoSendMailResult = CPApplication.Current.LoginUser.UserEmailAddress + ";" + getVendorInfo.VendorBasicInfo.EmailAddress;
                                        }
                                        else if (!string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && string.IsNullOrEmpty(getVendorInfo.VendorBasicInfo.EmailAddress))
                                        {
                                            returnAutoSendMailResult = CPApplication.Current.LoginUser.UserEmailAddress;
                                        }
                                        else if (string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && !string.IsNullOrEmpty(getVendorInfo.VendorBasicInfo.EmailAddress))
                                        {
                                            returnAutoSendMailResult = getVendorInfo.VendorBasicInfo.EmailAddress;
                                        }
                                        returnVendorMailResult = getVendorInfo.VendorBasicInfo.EmailAddress;
                                    }
                                    AutoMail_VendorMailAddress = returnVendorMailResult;
                                    AutoMail_AutoMailAddress = returnAutoSendMailResult;
                                    AutoMail_HaveSentMailAddress = returnMailAddresResult;
                                    UpdateAutoMailAddressEditRegion();
                                    return;
                                });
                            }
                            returnMailAddresResult = getPOInfo.PurchaseOrderBasicInfo.MailAddress;
                        }

                        AutoMail_VendorMailAddress = returnVendorMailResult;
                        AutoMail_AutoMailAddress = returnAutoSendMailResult;
                        AutoMail_HaveSentMailAddress = returnMailAddresResult;
                        UpdateAutoMailAddressEditRegion();
                    });
                }
                #endregion
            }
        }
        private void Hyperlink_EditPOItem_Click(object sender, RoutedEventArgs e)
        {
            //编辑采购单商品:
            PurchaseOrderItemInfoVM itemVM = this.gridProductsListInfo.SelectedItem as PurchaseOrderItemInfoVM;
            if (null != itemVM)
            {
                PurchaseOrderProductsMaintain editItemCtrl = new PurchaseOrderProductsMaintain(poInfoVM.PurchaseOrderBasicInfo.StockName, itemVM, poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType.Value);
                editItemCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_ModifyProductsTitle, editItemCtrl, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        PurchaseOrderItemInfoVM editVM = args.Data as PurchaseOrderItemInfoVM;
                        if (null != editVM)
                        {
                            AddPurchaseOrderPrivilege();
                            PurchaseOrderInfoVM editItemVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PurchaseOrderInfoVM>(poInfoVM);
                            editItemVM.POItems.Clear();
                            editItemVM.POItems.Add(editVM);
                            editItemVM.PurchaseOrderBasicInfo.MemoInfo.Note = "EditUpdateOnePOItem";
                            serviceFacade.UpdatePurchaseOrderInfo(BuildVMToEntity(editItemVM), (obj3, args3) =>
                            {
                                if (args3.FaultsHandle())
                                {
                                    return;
                                }
                                itemVM.PurchaseQty = editVM.PurchaseQty;
                                itemVM.OrderPrice = editVM.OrderPrice;
                                itemVM.ExecptStatus = null;
                                this.gridProductsListInfo.Bind();
                                GetProductAppendedInfo(() =>
                                {
                                    UpdatePOTotalAmt();
                                });
                            });
                        }
                    }
                });
            }
        }
        private void hpkQueryInventory_Click(object sender, RoutedEventArgs e)
        {
            // 查询分仓库存链接:
            PurchaseOrderItemInfoVM getSelectedItem = this.gridProductsListInfo.SelectedItem as PurchaseOrderItemInfoVM;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.Inventory/InventoryQuery/{0}", getSelectedItem.ProductSysNo.Value), null, true);
            }
        }
        private void hpkMaintainReceivePerson_Click(object sender, RoutedEventArgs e)
        {
            //维护收件人列表:
            PurchaseOrderAutoSendMailMaintain mailMaintain = new PurchaseOrderAutoSendMailMaintain(this.poInfoVM.VendorInfo.SysNo.Value, AutoMail_AutoMailAddress, AutoMail_VendorMailAddress, AutoMail_HaveSentMailAddress);
            mailMaintain.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_MailMaintain, mailMaintain, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    Dictionary<string, object> getReturnDict = args.Data as Dictionary<string, object>;
                    if (null != getReturnDict)
                    {
                        string getUpdatedVendorEmailAddress = getReturnDict["updatedVendorMailAddress"].ToString();
                        this.AutoMail_VendorMailAddress = getUpdatedVendorEmailAddress;

                        List<string> getSelectedReceivedMailAddress = getReturnDict["updatedReceiveMailAddress"] as List<string>;
                        if (null != getSelectedReceivedMailAddress)
                        {
                            AutoMail_AutoMailAddress = string.Join(";", getSelectedReceivedMailAddress);
                            UpdateAutoMailAddressEditRegion();
                        }
                    }

                }

            }, new Size(600, 300));
        }
        private void hpkContractDetailView_Click(object sender, RoutedEventArgs e)
        {
            //查询返点合同:
            if (!string.IsNullOrEmpty(txtContractNumber.Text.Trim()))
            {
                PurchaseOrderEIMSRuleQuery queryEIMSCtrl = new PurchaseOrderEIMSRuleQuery(txtContractNumber.Text.Trim());
                queryEIMSCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_EIMSContractView, queryEIMSCtrl, null, new Size(900, 400));
            }
        }
        private void hpkEditReasonMemo_Click(object sender, RoutedEventArgs e)
        {
            //详细原因查看:
            PurchaseOrderCheckReasonDetail detailCtrl = new PurchaseOrderCheckReasonDetail(poInfoVM, true);
            detailCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_POCheck, detailCtrl, (obj, args) =>
            {

            }, new Size(400, 200));
        }
        private void hpkSearchBatchInfoList_Click(object sender, RoutedEventArgs e)
        {
            //退货批次(负采购单 ）：
            PurchaseOrderItemInfoVM getSelectedVM = this.gridProductsListInfo.SelectedItem as PurchaseOrderItemInfoVM;
            if (null != getSelectedVM)
            {

                PurchaseOrderBatchInfoList batchDetailCtrl = new PurchaseOrderBatchInfoList(getSelectedVM.ItemSysNo.Value, getSelectedVM.ProductSysNo.Value, poInfoVM.PurchaseOrderBasicInfo.StockSysNo.Value, getSelectedVM.BatchInfo);
                batchDetailCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_BatchInfo, batchDetailCtrl, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        getSelectedVM.BatchInfo = batchDetailCtrl.batchInfo;
                    }
                }, new Size(700, 350));
            }

        }
        private void hpkEimsInvoice_Click(object sender, RoutedEventArgs e)
        {
            //可用返点提醒  ：
            if (poInfoVM.VendorInfo == null || !poInfoVM.VendorInfo.SysNo.HasValue)
            {
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_VendorEmpty);
                return;
            }
            PurchaseOrderEIMSInvoiceList eimsSearchCtrl = new PurchaseOrderEIMSInvoiceList(poInfoVM.VendorInfo.SysNo.Value);
            eimsSearchCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_LeftEIMSTitle, eimsSearchCtrl, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    List<EIMSInfoVM> getSelectedList = args.Data as List<EIMSInfoVM>;
                    if (null != getSelectedList && 0 < getSelectedList.Count)
                    {
                        getSelectedList.ForEach(x =>
                        {
                            if (!IsExistForEIMSInfoVM(x))
                            {
                                x.EIMSTotalAmt = Convert.ToDecimal(x.EIMSAmt);
                                x.EIMSAmt = x.EIMSLeftAmt == null ? null : x.EIMSLeftAmt.ToString();
                                poInfoVM.EIMSInfo.EIMSInfoList.Add(x);
                            }
                        });
                        gridEIMSInfo.Bind();
                        RefreshEIMSTotalText();
                    }
                }
            }, new Size(700, 400));
        }
        //判断当前返点列表中是否存在相同记录
        private bool IsExistForEIMSInfoVM(EIMSInfoVM eimsInfo)
        {
            foreach (EIMSInfoVM tobj in poInfoVM.EIMSInfo.EIMSInfoList)
                if (tobj.EIMSSysNo.Equals(eimsInfo.EIMSSysNo))
                    return true;
            return false;
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
                        if (item is PurchaseOrderItemInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((PurchaseOrderItemInfoVM)item).IsCheckedItem)
                                {
                                    ((PurchaseOrderItemInfoVM)item).IsCheckedItem = true;
                                }
                            }
                            else
                            {
                                if (((PurchaseOrderItemInfoVM)item).IsCheckedItem)
                                {
                                    ((PurchaseOrderItemInfoVM)item).IsCheckedItem = false;
                                }
                            }

                        }
                    }
                }
            }
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //更新操作:
            #region [Check操作:]
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            CheckProductLine(poInfoVM, null, parmagrs =>
            {
                BuildPOReceivedMailAddress();
                //检查返点信息:
                if (!CheckEIMSInfo())
                {
                    return null;
                }
                //计算返点信息
                CalcEIMSInfo();
                //检查ETA信息:
                if ((poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue && !poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay.HasValue) || (!poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue && poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue))
                {
                    Window.Alert(ResPurchaseOrderMaintain.AlertMsg_CheckETA);
                    return null;
                }
                Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmOperate, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        AddPurchaseOrderPrivilege();
                        PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
                        //保存PM高级权限，用于业务端验证
                        info.PurchaseOrderBasicInfo.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
                        serviceFacade.UpdatePurchaseOrderInfo(info, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                            {
                                if (args3.DialogResult == DialogResultType.Cancel)
                                {
                                    Window.Refresh();
                                }
                            });
                        });
                    }
                });
                return null;
            });
            #endregion
        }


        #region 产品线相关检测  CRL21776  2012-11-6  by Jack

        private void CheckProductLine(PurchaseOrderInfoVM poInfoVM, PurchaseOrderItemInfoVM addItem, Func<object, object> callback)
        {
            //1.检测当前登陆PM对选择商品是否有操作权限
            serviceFacade.GetProductLineInfoByPM(CPApplication.Current.LoginUser.UserSysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                List<ProductPMLine> tPMLineList = args.Result;
                //是否存在高级权限
                bool tIsManager = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
                if (tPMLineList.Count > 0 || tIsManager)
                {
                    //获取已选和新选择的商品SysNo集合
                    List<int> tProList = poInfoVM.POItems.Select(x => x.ProductSysNo.Value).ToList<int>();
                    if (addItem != null)
                        tProList.Add(addItem.ProductSysNo.Value);
                    //获取ItemList中的 产品线和主PM
                    serviceFacade.GetProductLineSysNoByProductList(tProList.ToArray(), (obj1, args1) =>
                    {
                        if (args1.FaultsHandle())
                            return;
                        List<ProductPMLine> tList = args1.Result;
                        string tErrorMsg = string.Empty;
                        //检测没有产品线的商品
                        tList.ForEach(x =>
                        {
                            if (x.ProductLineSysNo == null)
                                tErrorMsg += x.ProductID + Environment.NewLine;
                        });
                        //if (!tErrorMsg.Equals(string.Empty))
                        //{
                        //    Window.Alert(ResPurchaseOrderNew.AlertMsg_NotLine + Environment.NewLine + tErrorMsg);
                        //    return;
                        //}
                        //检测当前登陆PM对ItemList中商品是否有权限
                        if (!tIsManager)
                            tList.ForEach(x =>
                            {
                                if (tPMLineList.SingleOrDefault(item => item.ProductLineSysNo == x.ProductLineSysNo) == null)
                                    tErrorMsg += x.ProductID + Environment.NewLine;
                            });
                        if (!tErrorMsg.Equals(string.Empty))
                        {
                            Window.Alert(ResPurchaseOrderNew.AlertMsg_NotAccessLine + Environment.NewLine + tErrorMsg);
                            return;
                        }
                        //验证ItemList中产品线是否唯一
                        if (tList.Select(x => x.ProductLineSysNo.Value).Distinct().ToList().Count != 1)
                        {
                            Window.Alert(ResPurchaseOrderNew.AlertMsg_NotOnlyOneLine);
                            return;
                        }
                        if ((string.IsNullOrEmpty(poInfoVM.PurchaseOrderBasicInfo.PMSysNo)) || (int.Parse(poInfoVM.PurchaseOrderBasicInfo.PMSysNo) != tList[0].PMSysNo))
                        {
                            //需要根据商品的产品线加载PO单的所属PM
                            poInfoVM.PurchaseOrderBasicInfo.PMSysNo = tList[0].PMSysNo.ToString();
                        }
                        callback(null);
                    });
                }
                else
                {
                    Window.Alert(ResPurchaseOrderNew.AlertMsg_NotAccessLine);
                    return;
                }
            });
        }

        private void WritePMSysNo(Func<object, object> callback)
        {
            //if (poInfoVM.POItems.Count > 0)
            //{
            //    serviceFacade.GetProductLineSysNoByProductList(poInfoVM.POItems.Select(x => x.ProductSysNo.Value).ToArray(), (obj1, args1) =>
            //    {
            //        if (args1.FaultsHandle())
            //            return;
            //        List<ProductPMLine> tList = args1.Result;
            //        //如果当前ItemList中只存在一条产品线，则更新所属PM
            //        if (tList != null && tList.Count != 0)
            //            if (tList.Select(x => x.ProductLineSysNo.Value).Distinct().ToList().Count == tList.Count)
            //            {
            //                poInfoVM.PurchaseOrderBasicInfo.PMSysNo = tList[0].PMSysNo.ToString();
            //            }
            //        callback(null);
            //    });
            //}
            //else
            //{
            //    //如果删完item，则清空所属PM
            //    poInfoVM.PurchaseOrderBasicInfo.PMSysNo = null;
            callback(null);
            //}
        }

        #endregion

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //重置操作:
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmOperate, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.chkAutoEmail.IsChecked = false;
                    this.lbPOReceivePersonList.Items.Clear();
                    LoadPOInfo();
                }
            });
        }
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            //检查操作:
            AddPurchaseOrderPrivilege();
            PurchaseOrderInfo entity = BuildVMToEntity(poInfoVM);
            //参考IPP.Oversea.CN.POASNMgmt.ServiceImpl.POService.Check()增加
            if (!entity.PurchaseOrderBasicInfo.AuditUserSysNo.HasValue)
                entity.PurchaseOrderBasicInfo.AuditUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            serviceFacade.CheckPurchaseOrderInfo(entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                //显示检查结果:
                PurchaseOrderCheckResult showCheckResultCtrl = new PurchaseOrderCheckResult(args.Result.PurchaseOrderBasicInfo.CheckResult);
                showCheckResultCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_POCheckResult, showCheckResultCtrl, null);
            });
        }
        private void btnSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
            //提交审核操作:
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfrimAudit, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    AddPurchaseOrderPrivilege();
                    if (!CheckETATimeInfoExists())
                    {
                        return;
                    }

                    StringBuilder msgStr = new StringBuilder();
                    if (null != poInfoVM.POItems)
                    {
                        foreach (PurchaseOrderItemInfoVM item in poInfoVM.POItems)
                        {
                            //同步价格低于采购单中的PM确认价格(如果是英迈同步商品)
                            if (item.IsVFItem == YNStatus.Yes && item.PurchasePrice < item.OrderPrice.ToNullableDecimal())
                            {
                                //记录应当弹出消息的产品编号 [商品***-***-***的同步价格低于当前采购价格，是否继续提交审核？]
                                msgStr.Append(string.Format(ResPurchaseOrderMaintain.AlertMsg_AuditCheckResult, item.ProductID) + Environment.NewLine);
                            }
                        }
                    }
                    if (msgStr.ToString().Trim() == "")
                    {
                        //提交PO审核
                        SubmitAuditAction();
                    }
                    else
                    {
                        msgStr.Append(ResPurchaseOrderMaintain.AlertMsg_ConfrimAudit);
                        string warningMsg = msgStr.ToString().Trim();
                        Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, warningMsg, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.OK)
                            {
                                SubmitAuditAction();
                            }
                        });
                    }
                }
            });
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //确认(等待入库 ）操作:
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmOperate, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    AddPurchaseOrderPrivilege();
                    if (!CheckETATimeInfoExists())
                    {
                        return;
                    }
                    #region 应付款低于应收的控制,如果为负PO，则进行此校验
                    if (poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
                    {
                        vendorFacade.QueryVendorPayBalanceByVendorSysNo(poInfoVM.VendorInfo.SysNo.Value.ToString(), (objj, argss) =>
                        {
                            if (argss.FaultsHandle())
                            {
                                return;
                            }
                            if (0 < argss.Result.TotalCount)
                            {
                                var row = argss.Result.Data[0];
                                //单据金额
                                decimal totalPOAmt = poInfoVM.POItems.Sum(item => item.PurchaseQty.ToInteger() * item.OrderPrice.ToDecimal());

                                decimal totalPayBalance = row["TotalPayBalance"] == null ? 0 : Convert.ToDecimal(row["TotalPayBalance"].ToString());
                                //供应商未支付的应付款金额(小于零不校验) < 绝对值（负PO金额），验证不通过
                                if (totalPayBalance >= 0 && totalPayBalance < Math.Abs(totalPOAmt))
                                {
                                    string getVendorName = row["VendorName"].ToString();

                                    Window.Confirm(string.Format(ResPurchaseOrderMaintain.AlertMsg_ConfirmCheckResult, getVendorName), (objcc, argscc) =>
                                    {
                                        if (argscc.DialogResult == DialogResultType.OK)
                                        {
                                            ProcessConfirmAction();
                                        }
                                    });
                                }
                                else
                                {
                                    ProcessConfirmAction();
                                }
                            }
                            else
                            {
                                ProcessConfirmAction();
                            }

                        });
                    }
                    else
                    {
                        ProcessConfirmAction();
                    }
                    #endregion
                }
            });

        }
        private void btnCancelConfirm_Click(object sender, RoutedEventArgs e)
        {
            //取消确认操作:
            PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
            if (!CheckETATimeInfoExists() || !CheckEIMSInfo())
            {
                return;
            }
            serviceFacade.CancelVerifyPO(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                {
                    if (args3.DialogResult == DialogResultType.Cancel)
                    {
                        Window.Refresh();
                    }
                });
            });
        }
        private void btnRefuseBack_Click(object sender, RoutedEventArgs e)
        {
            //拒绝/退回操作:
            if (string.IsNullOrEmpty(poInfoVM.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo) || string.IsNullOrWhiteSpace(poInfoVM.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo))
            {
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ErrorTitle, ResPurchaseOrderMaintain.AlertMsg_RefuseBackResult, MessageType.Error);
                return;
            }
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_ConfirmRefuseBack, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
                    if (info.PurchaseOrderBasicInfo.Source == "VP" && info.PurchaseOrderBasicInfo.PurchaseOrderID != info.SysNo.ToString())
                    {
                        PurchaseOrderRetreatReq request = new PurchaseOrderRetreatReq()
                        {
                            poSysNo = poInfoVM.SysNo.Value,
                            retreatType = "AuditReturn"
                        };
                        serviceFacade.RetreatVendorPortalPurchaseOrder(request, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                            {
                                if (args3.DialogResult == DialogResultType.Cancel)
                                {
                                    Window.Close();
                                }
                            });
                        });
                    }
                    else
                    {
                        serviceFacade.RefusePO(info, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_RefuseBackSuc, MessageType.Information, (obj3, args3) =>
                            {
                                if (args3.DialogResult == DialogResultType.Cancel)
                                {
                                    Window.Refresh();
                                }
                            });
                            return;
                        });
                    }
                }
            });
        }
        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            //作废操作:
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_ConfirmAbandon, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
                    serviceFacade.AbandonPO(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_AbandonSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Refresh();
                            }
                        });
                    });
                }
            });
        }
        private void btnCancelAbandon_Click(object sender, RoutedEventArgs e)
        {
            //取消作废操作:
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_ConfirmCancelAbandon, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
                    serviceFacade.CancelAbandonPO(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_CancelAbandonSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Refresh();
                            }
                        });
                    });
                }
            });
        }
        private void btnConfirmWithVendor_Click(object sender, RoutedEventArgs e)
        {
            //PM与供应商确认:
            PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
            serviceFacade.PMConfirmWithVendorPO(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_ConfirmVendorSuc, MessageType.Information, (obj3, args3) =>
                {
                    if (args3.DialogResult == DialogResultType.Cancel)
                    {
                        Window.Refresh();
                    }
                });
                return;
            });
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //打印操作(不包括附件):
            HtmlViewHelper.WebPrintPreview("PO", "PurchaseOrderPrint", new Dictionary<string, string>() { { "POSysNo", poInfoVM.SysNo.Value.ToString() }, { "PrintAccessory", "0" }, { "PrintTitle", btnPrint.Content.ToString() } });
        }
        private void btnPrintAppend_Click(object sender, RoutedEventArgs e)
        {
            //打印操作(包括附件):
            HtmlViewHelper.WebPrintPreview("PO", "PurchaseOrderPrint", new Dictionary<string, string>() { { "POSysNo", poInfoVM.SysNo.Value.ToString() }, { "PrintAccessory", "1" }, { "PrintTitle", btnPrintAppend.Content.ToString() } });
        }
        private void btnOperateInStockMemoAndTotalAmt_Click(object sender, RoutedEventArgs e)
        {
            //更新入库备注和到付运费金额:
            PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
            serviceFacade.UpdateInStockMemoPO(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, string.Format(ResPurchaseOrderMaintain.AlertMsg_UpdateStockMemoSuc, info.PurchaseOrderBasicInfo.MemoInfo.InStockMemo == null ? 0 : info.PurchaseOrderBasicInfo.MemoInfo.InStockMemo.Length), MessageType.Information, (obj2, args2) =>
                {
                    if (args2.DialogResult == DialogResultType.Cancel)
                    {
                        Window.Refresh();
                    }
                });
                return;
            });
        }
        private void btnOperatePartlyInStock_Click(object sender, RoutedEventArgs e)
        {
            //中止入库:
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_ConfirmOperate, (obj, args) =>
             {
                 if (args.DialogResult == DialogResultType.OK)
                 {
                     PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
                     serviceFacade.StopInStockPO(info, (obj2, args2) =>
                     {
                         if (args2.FaultsHandle())
                         {
                             return;
                         }
                         Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                         {
                             if (args3.DialogResult == DialogResultType.Cancel)
                             {
                                 Window.Refresh();
                             }
                         });
                         return;
                     });
                 }
             });
        }
        private void btnOperateReNewPO_Click(object sender, RoutedEventArgs e)
        {
            //补充创建PO单:
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmOperate, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
                    //保存PM高级权限，用于业务端验证
                    info.PurchaseOrderBasicInfo.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
                    info.PurchaseOrderBasicInfo.IsAutoFillPM = true;
                    serviceFacade.RenewCreatePurchaseOrder(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/PurchaseOrderMaintain/{0}", args2.Result.SysNo.Value.ToString()), true);
                    });
                }
            });
        }
        private void btnNewPO_Click(object sender, RoutedEventArgs e)
        {
            //创建PO单(待PM审核状态):

            #region [Check逻辑]

            decimal eimsTotalAmt = 0;
            decimal totalAmt = 0;
            if (0 < poInfoVM.EIMSInfo.EIMSInfoList.Count)
            {
                foreach (var item in poInfoVM.EIMSInfo.EIMSInfoList)
                {
                    decimal getInputEIMSAmt = 0m;
                    decimal.TryParse(item.EIMSAmt, out getInputEIMSAmt);
                    if (string.IsNullOrEmpty(item.EIMSAmt) || getInputEIMSAmt <= 0)
                    {
                        Window.Alert("使用返点金额必须大于0.00!");
                        return;
                    }
                    if (getInputEIMSAmt > item.EIMSLeftAmt)
                    {
                        Window.Alert("使用返点金额必须小于或等于相应返点的剩余金额!");
                        return;
                    }
                    eimsTotalAmt += getInputEIMSAmt;
                }
            }
            if (null != poInfoVM.POItems && 0 < poInfoVM.POItems.Count)
            {
                foreach (var item in poInfoVM.POItems)
                {
                    int getInputPurchaseQty = 0;
                    decimal getInputOrderPrice = 0m;
                    int.TryParse(item.PurchaseQty, out getInputPurchaseQty);
                    decimal.TryParse(item.OrderPrice, out getInputOrderPrice);
                    totalAmt += getInputPurchaseQty * getInputOrderPrice;
                }
                if (poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Normal && totalAmt < eimsTotalAmt)
                {
                    Window.Alert(string.Format("采购单返点金额 {0} 不能大于采购总价 {1} !", eimsTotalAmt.ToString("f2"), totalAmt.ToString("f2")));
                    return;
                }
            }

            if ((poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue && !poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay.HasValue) || (!poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue && poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue))
            {
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_CheckETA);
                return;
            }

            #endregion
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmOperate, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    BuildPOReceivedMailAddress();
                    AddPurchaseOrderPrivilege();
                    PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);
                    serviceFacade.ConfirmVendorPortalPurchaseOrder(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_CreateSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/PurchaseOrderMaintain/{0}", args2.Result.SysNo.Value.ToString()), true);
                            }
                        });
                    });
                }
            });
        }
        private void btnCancelPO_Click(object sender, RoutedEventArgs e)
        {
            //退回(待PM审核状态):
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmCancelPO, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    PurchaseOrderRetreatReq request = new PurchaseOrderRetreatReq()
                    {
                        poSysNo = poInfoVM.SysNo.Value,
                        retreatType="ComfirmReturn"
                    };
                    serviceFacade.RetreatVendorPortalPurchaseOrder(request, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Close();
                            }
                        });
                    });
                }
            });
        }

        #region [预计到货时间 - Events]
        private void btnRequestAudit_Click(object sender, RoutedEventArgs e)
        {
            //预计到货时间 - 提交审核:
            if (!poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue || !poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay.HasValue || string.IsNullOrEmpty(poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.Memo))
            {
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ETAEmpty);
                return;
            }
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmETASubmit, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    PurchaseOrderETATimeInfo info = EntityConverter<PurchaseOrderETATimeInfoVM, PurchaseOrderETATimeInfo>.Convert(poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo);
                    info.Status = 1;
                    serviceFacade.SubmitETAInfo(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("提示信息", ResPurchaseOrderMaintain.AlertMsg_ConfirmETASuc, MessageType.Information, (f, ar) =>
                            {
                                Window.Refresh();
                            });
                        //this.btnPassAudit.IsEnabled = true;
                        //this.btnCancelAudit.IsEnabled = true;
                        //this.btnRequestAudit.IsEnabled = false;
                        //this.dpkETATime.IsEnabled = false;
                        //this.cmbETAHalfDay.IsEnabled = false;
                        //this.txtETAMemo.IsEnabled = false;
                        //this.txtETAMemo.Text = string.Format("[{0}]:{1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), this.txtETAMemo.Text);
                        
                    });
                }
            });
        }

        private void btnPassAudit_Click(object sender, RoutedEventArgs e)
        {
            //预计到货时间 - 审核通过 ：
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmETAPass, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    PurchaseOrderETATimeInfo info = EntityConverter<PurchaseOrderETATimeInfoVM, PurchaseOrderETATimeInfo>.Convert(poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo);
                    info.Status = 2;
                    serviceFacade.PassETAInfo(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("提示信息", ResPurchaseOrderMaintain.AlertMsg_ETAPassSuc, MessageType.Information, (f, ar) =>
                        {
                            Window.Refresh();
                        });
                        //Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ETAPassSuc);
                        //this.btnPassAudit.IsEnabled = false;
                        //this.btnCancelAudit.IsEnabled = false;
                        //this.btnRequestAudit.IsEnabled = true;
                        //this.dpkETATime.IsEnabled = true;
                        //this.cmbETAHalfDay.IsEnabled = true;
                        //this.txtETAMemo.IsEnabled = true;
                        //this.txtETAMemo.Text = string.Empty;
                    });
                }
            });
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            //预计到货时间 - 取消审核:
            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfirmETACancel, (obj, args) =>
          {
              if (args.DialogResult == DialogResultType.OK)
              {
                  PurchaseOrderETATimeInfo info = EntityConverter<PurchaseOrderETATimeInfoVM, PurchaseOrderETATimeInfo>.Convert(poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo);
                  info.Status = 0;
                  serviceFacade.CancelETAInfo(info, (obj2, args2) =>
                  {
                      if (args2.FaultsHandle())
                      {
                          return;
                      }
                      Window.Alert("提示信息", ResPurchaseOrderMaintain.AlertMsg_ETACancelSuc, MessageType.Information, (f, ar) =>
                      {
                          Window.Refresh();
                      });
                      //Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ETACancelSuc);
                      //this.btnPassAudit.IsEnabled = false;
                      //this.btnCancelAudit.IsEnabled = false;
                      //this.btnRequestAudit.IsEnabled = true;
                      //this.dpkETATime.IsEnabled = true;
                      //this.cmbETAHalfDay.IsEnabled = true;
                      //this.txtETAMemo.IsEnabled = true;
                      //this.txtETAMemo.Text = string.Empty;
                  });
              }
          });
        }
        #endregion

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            //添加采购商品:
            if (!this.poInfoVM.VendorInfo.SysNo.HasValue)
            {
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_VendorEmpty);
                VendorInfo.Focus();
                return;
            }

            PurchaseOrderProductsNew newCtrl = new PurchaseOrderProductsNew(poInfoVM.PurchaseOrderBasicInfo.StockName, poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType.Value);
            newCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_AddPOItemTitle, newCtrl, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    //调用接口，查询商品相关的信息（包价格，库存 等.)
                    PurchaseOrderItemInfoVM getAddedVM = args.Data as PurchaseOrderItemInfoVM;
                    if (null != getAddedVM)
                    {
                        PurchaseOrderItemProductInfo productInfo = new PurchaseOrderItemProductInfo()
                        {
                            SysNo = getAddedVM.ProductSysNo.Value,
                            ProductName = getAddedVM.ProductName,
                            PurchaseQty = getAddedVM.PurchaseQty.ToInteger(),
                            OrderPrice = getAddedVM.OrderPrice.ToDecimal(),
                            UnitCost = getAddedVM.CurrentUnitCost.Value,
                            //人民币:
                            CurrencySysNo = poInfoVM.PurchaseOrderBasicInfo.CurrencyCode.ToNullableToInteger(),
                        };
                        //1.首先判断添加的商品是否已经存在:
                        if (poInfoVM.POItems.SingleOrDefault(i => i.ProductSysNo == productInfo.SysNo) != null)
                        {
                            Window.Alert(ResPurchaseOrderMaintain.AlertMsg_POItemExists);
                            return;
                        }
                        //2.如果是新商品，则调用Service获取价格，库存等详细信息:
                        serviceFacade.AddNewPurchaseOrderItem(productInfo, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }

                            PurchaseOrderItemInfoVM getNewVM = EntityConverter<PurchaseOrderItemInfo, PurchaseOrderItemInfoVM>.Convert(args2.Result, (s, t) =>
                            {
                                t.ExecptStatus = null;
                            });
                            getNewVM.ItemSysNo = null;
                            AddPurchaseOrderPrivilege();
                            PurchaseOrderInfoVM updateVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PurchaseOrderInfoVM>(poInfoVM);
                            updateVM.POItems.Clear();
                            updateVM.POItems.Add(getNewVM);
                            updateVM.PurchaseOrderBasicInfo.MemoInfo.Note = "EditAddOnePOItem";

                            serviceFacade.UpdatePurchaseOrderInfo(BuildVMToEntity(updateVM), (obj3, args3) =>
                            {
                                if (args3.FaultsHandle())
                                {
                                    return;
                                }
                                //3.添加到List中，并刷新Grid:
                                //获取最近添加的ItemsSysNo
                                getNewVM = (args3.Result.Convert<PurchaseOrderInfo, PurchaseOrderInfoVM>()).POItems[0];
                                this.poInfoVM.POItems.Add(getNewVM);
                                this.gridProductsListInfo.Bind();
                                //Update至DB：

                                //计算总价格:
                                //4.获取赠品信息，附件信息:
                                GetProductAppendedInfo(() =>
                                {
                                    UpdatePOTotalAmt();
                                });
                            });
                        });
                    }
                }
            });
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            //删除采购商品:
            if (null == this.poInfoVM.POItems || 0 >= this.poInfoVM.POItems.Count)
            {
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_NoItemDelete);
                return;
            }
            if (this.poInfoVM.POItems.Count(i => i.IsCheckedItem == true) <= 0)
            {
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_SelectDeleteItem);
                return;
            }

            Window.Confirm(ResPurchaseOrderMaintain.AlertMsg_ConfrimItemDelete, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    AddPurchaseOrderPrivilege();
                    PurchaseOrderInfoVM deleteVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PurchaseOrderInfoVM>(poInfoVM);
                    deleteVM.POItems.Clear();
                    deleteVM.POItems.AddRange(this.poInfoVM.POItems.Where(i => i.IsCheckedItem == true).ToList());
                    deleteVM.PurchaseOrderBasicInfo.MemoInfo.Note = "EditDeletePOItem";
                    serviceFacade.UpdatePurchaseOrderInfo(BuildVMToEntity(deleteVM), (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        //1.移除所选商品:
                        this.poInfoVM.POItems.RemoveAll(i => i.IsCheckedItem == true);
                        //验证并更新所属PM
                        WritePMSysNo(obj1 =>
                        {
                            this.gridProductsListInfo.Bind();
                            //2.刷新商品信息:
                            if (poInfoVM.POItems.Count > 0)
                            {
                                GetProductAppendedInfo(() =>
                                {
                                    UpdatePOTotalAmt();
                                });
                            }
                            else
                            {
                                this.lblCheckResult.Text = string.Empty;
                                UpdatePOTotalAmt();
                            }
                            return null;
                        });
                    });
                }
            });
        }
        #endregion

        private void GetProductAppendedInfo(Action action)
        {
            List<int> productSysNoList = new List<int>();
            if (0 < this.poInfoVM.POItems.Count)
            {
                foreach (var item in poInfoVM.POItems)
                {
                    productSysNoList.Add(item.ProductSysNo.Value);
                }
                //赠品信息:
                serviceFacade.GetPurchaseOrderGiftInfo(productSysNoList, (obj3, args3) =>
                {
                    if (args3.FaultsHandle())
                    {
                        return;
                    }
                    List<PurchaseOrderItemProductInfo> giftList = args3.Result;
                    //附件信息:
                    serviceFacade.GetPurchaseOrderAccessoriesInfo(productSysNoList, (obj4, args4) =>
                    {
                        if (args4.FaultsHandle())
                        {
                            return;
                        }
                        List<PurchaseOrderItemProductInfo> accessoriesList = args4.Result;

                        //商品价格异常:
                        List<PurchaseOrderItemInfoVM> priceExceptionList = poInfoVM.POItems.Where(a => a.ExecptStatus == -1 && a.LastOrderPrice.HasValue).ToList();
                        //显示提示信息 :
                        ShowAppendedItemInfo(giftList, accessoriesList, priceExceptionList);
                        if (null != action)
                        {
                            action();
                        }
                    });
                });
            }
        }
        /// <summary>
        /// 添加新的Item后，显示相关的Check信息:
        /// </summary>
        /// <param name="giftList"></param>
        /// <param name="accessoryList"></param>
        /// <param name="priceExceptionList"></param>
        private void ShowAppendedItemInfo(List<PurchaseOrderItemProductInfo> giftList, List<PurchaseOrderItemProductInfo> accessoryList, List<PurchaseOrderItemInfoVM> priceExceptionList)
        {
            string giftString = string.Empty;
            string accessoryString = string.Empty;
            string priceExceptionString = string.Empty;

            //赠品提示:
            if (null != giftList && 0 < giftList.Count)
            {
                giftString += ResPurchaseOrderMaintain.AlertMsg_GiftExists + Environment.NewLine;
                foreach (var giftItem in giftList)
                {
                    giftString += string.Format(ResPurchaseOrderMaintain.AlertMsg_GiftExistsFormatString, giftItem.Id, giftItem.GiftId, giftItem.ProductName, giftItem.OnlineQty) + Environment.NewLine;

                }
            }
            else
            {
                accessoryString += ResPurchaseOrderMaintain.AlertMsg_NoGift + Environment.NewLine;
            }
            //附件提示:
            if (null != accessoryList && 0 < accessoryList.Count)
            {
                accessoryString += ResPurchaseOrderMaintain.AlertMsg_AccessoryInfo + Environment.NewLine;
                foreach (var accessoryItem in accessoryList)
                {
                    accessoryString += string.Format("【{0}】 {1}{2}", accessoryItem.Id, accessoryItem.Description, Environment.NewLine);
                }
            }
            else
            {
                accessoryString += ResPurchaseOrderMaintain.AlertMsg_NoAccessory + Environment.NewLine;

            }
            //商品价格异常:
            if (null != priceExceptionList && 0 < priceExceptionList.Count)
            {
                priceExceptionString += ResPurchaseOrderMaintain.AlertMsg_ProductPriceException + Environment.NewLine;
                foreach (var priceExceptionItem in priceExceptionList)
                {
                    priceExceptionItem.PriceChangePercent = GetPriceChangePercent(priceExceptionItem.OrderPrice.ToDecimal(), priceExceptionItem.LastOrderPrice.Value);
                    priceExceptionString += string.Format(ResPurchaseOrderMaintain.AlertMsg_ProductPriceExceptionFormatString, priceExceptionItem.ProductID, priceExceptionItem.LastOrderPrice.Value.ToString("f2"), Convert.ToDecimal(priceExceptionItem.OrderPrice).ToString("f2"), priceExceptionItem.PriceChangePercent.Value.ToString("f2")) + Environment.NewLine;
                }
            }

            this.lblCheckResult.Text = giftString + accessoryString + priceExceptionString;
        }

        private decimal GetPriceChangePercent(decimal OrderPrice, decimal lastOrderPrice)
        {
            decimal diff = (OrderPrice - Convert.ToDecimal(lastOrderPrice));
            lastOrderPrice = lastOrderPrice == 0 ? 1m : lastOrderPrice;
            return (diff * 100 / lastOrderPrice);
        }

        private void UpdateAutoMailAddressEditRegion()
        {
            List<string> mailItems = new List<string>();
            if (!string.IsNullOrEmpty(AutoMail_AutoMailAddress))
            {
                if (AutoMail_AutoMailAddress.IndexOf(';') != -1)
                {
                    mailItems = AutoMail_AutoMailAddress.Split(';').ToList();
                }
                else
                {
                    mailItems.Add(AutoMail_AutoMailAddress);
                }
            }
            this.lbPOReceivePersonList.Items.Clear();
            foreach (string mailItem in mailItems)
            {
                this.lbPOReceivePersonList.Items.Add(mailItem);
            }
        }
        private void SubmitAuditAction()
        {
            PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);

            serviceFacade.SubmitAuditPO(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (null != args.Result)
                {
                    // 提交审核成功后 如果状态变为 待入库 则发送邮件:
                    if (args.Result.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.WaitingInStock)
                    {
                        PurchaseOrderInfo sendMailInfo = new PurchaseOrderInfo()
                        {
                            PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo() { MailAddress = poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress },
                            SysNo = poInfoVM.SysNo
                        };
                        serviceFacade.UpdateMailAddressAndHasSentMail(sendMailInfo, (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            //打印操作:
                            //HtmlViewHelper.WebPrintPreview("PO", "PurchaseOrderPrint", new Dictionary<string, string>() { { "POSysNo", args.Result.SysNo.Value.ToString() }, { "PrintAccessory", "1" }, { "PrintTitle", "给供应商发采购邮件" } });
                            Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj4, args4) =>
                            {
                                if (args4.DialogResult == DialogResultType.Cancel)
                                {
                                    Window.Refresh();
                                }
                            });
                        });
                    }
                    else
                    {

                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Refresh();
                            }
                        });
                    }
                }
            });
        }
        private void BuildPOReceivedMailAddress()
        {
            string getReceivedMailAddress = string.Empty;

            if (this.chkAutoEmail.IsChecked == true)
            {
                if (null != this.lbPOReceivePersonList.Items && 0 >= this.lbPOReceivePersonList.Items.Count)
                {
                    Window.Alert(ResPurchaseOrderMaintain.AlertMsg_MailAddressEmpty);
                    return;
                }
                if (null != this.lbPOReceivePersonList.Items)
                {
                    foreach (var item in this.lbPOReceivePersonList.Items)
                    {
                        getReceivedMailAddress += string.Format("{0};", item.ToString());
                    }
                    getReceivedMailAddress = getReceivedMailAddress.TrimEnd(';');
                }
            }
            this.poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress = getReceivedMailAddress;
        }
        private void BuildPOReceivedMailAddressToControl()
        {
            if (!string.IsNullOrEmpty(poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress))
            {
                if (poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress.IndexOf(';') != -1)
                {
                    string[] mailList = poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress.Split(';');
                    foreach (var item in mailList)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            this.lbPOReceivePersonList.Items.Add(item);
                        }
                    }
                }
                else
                {
                    this.lbPOReceivePersonList.Items.Add(poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress);

                }
            }
        }
        private void UpdatePOTotalAmt()
        {
            decimal updateTotalAmt = 0;
            this.poInfoVM.POItems.ForEach(x =>
            {
                updateTotalAmt += x.OrderPrice.ToDecimal() * x.PurchaseQty.ToInteger();
            });
            this.poInfoVM.PurchaseOrderBasicInfo.TotalAmt = updateTotalAmt;

            string getSelectCurrencySymbol =
                this.txtTotalAmt.Text = string.Format("{0}{1}", (string.IsNullOrEmpty(this.poInfoVM.PurchaseOrderBasicInfo.CurrencySymbol) ? "" : this.poInfoVM.PurchaseOrderBasicInfo.CurrencySymbol), updateTotalAmt.ToString("f2"));
        }
        private void UpdateMailAddressAndHasSendMail(Action callBackAction)
        {
            BuildPOReceivedMailAddress();
            //审核通过后发邮件  标识采购单已经发过邮件（HasSendMail=1）。记录至MailAddress,
            if (!string.IsNullOrEmpty(this.poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress))
            {
                //打印操作:
                HtmlViewHelper.WebPrintPreview("PO", "PurchaseOrderPrint", new Dictionary<string, string>() { { "POSysNo", poInfoVM.SysNo.Value.ToString() }, { "PrintAccessory", "1" }, { "PrintTitle", "给供应商发采购邮件" } });

                string getAutoSendMailAddress = this.poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress;
                PurchaseOrderInfo sendMailInfo = new PurchaseOrderInfo()
                {
                    PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo() { MailAddress = poInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress },
                    SysNo = poInfoVM.SysNo
                };
                //更新发送邮件地址 ：
                serviceFacade.UpdateMailAddressAndHasSentMail(sendMailInfo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (null != callBackAction)
                    {
                        callBackAction();
                    }
                });
            }
            else
            {
                if (null != callBackAction)
                {
                    callBackAction();
                }
            }
        }
        private void ProcessConfirmAction()
        {
            #region [确认操作:]
            PurchaseOrderInfo info = BuildVMToEntity(poInfoVM);

            if (info.PurchaseOrderBasicInfo.Source == "VP" && info.PurchaseOrderBasicInfo.PurchaseOrderID != info.SysNo.Value.ToString())
            {
                //VP高级用户提交的建议采购单
                serviceFacade.AuditVendorPortalPurchaseOrder(info, (obj2, args2) =>
                {
                    if (args2.FaultsHandle())
                    {
                        return;
                    }
                    //审核通过后发邮件  标识采购单已经发过邮件（HasSendMail=1）。记录至MailAddress,

                    UpdateMailAddressAndHasSendMail(() =>
                    {
                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Refresh();
                            }
                        });
                    });
                });

            }
            else
            {   //正常采购单
                serviceFacade.VerifyPO(info, (obj2, args2) =>
                {
                    if (args2.FaultsHandle())
                    {
                        return;
                    }
                    //审核通过后发邮件  标识采购单已经发过邮件（HasSendMail=1）。记录至MailAddress,
                    UpdateMailAddressAndHasSendMail(() =>
                    {
                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_AlertTitle, ResPurchaseOrderMaintain.AlertMsg_OprSuc, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Refresh();
                            }
                        });
                    });

                });
            }
            #endregion
        }
        private void RefreshEIMSTotalText()
        {
            decimal totalEIMS = 0m;
            decimal totalUsedEIMS = 0m;

            if (null != poInfoVM.EIMSInfo && null != poInfoVM.EIMSInfo.EIMSInfoList && 0 < poInfoVM.EIMSInfo.EIMSInfoList.Count)
            {
                poInfoVM.EIMSInfo.EIMSInfoList.ForEach(vm =>
                {
                    decimal eimsAmt = 0m;
                    decimal.TryParse(vm.EIMSAmt, out eimsAmt);
                    totalEIMS += (!string.IsNullOrEmpty(vm.EIMSAmt) ? eimsAmt : 0);
                    totalUsedEIMS += (vm.AlreadyUseAmt.HasValue ? vm.AlreadyUseAmt.Value : 0);
                });
            }
            poInfoVM.EIMSInfo.TotalEIMS = totalEIMS;
            poInfoVM.EIMSInfo.TotalUsedEIMS = totalUsedEIMS;
            this.lblEIMSTotal.Text = string.Format(ResPurchaseOrderMaintain.AlertMsg_EIMSTotalFormatString, poInfoVM.EIMSInfo.TotalEIMS.Value.ToString("f2"), poInfoVM.UsedEIMSTotal.Value.ToString("f2"));
        }

        #region [Check Methods]

        /// <summary>
        /// 验证预计到货时间和到货时间段不能为空:
        /// </summary>
        /// <returns></returns>
        private bool CheckETATimeInfoExists()
        {
            if (!this.poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue || !this.poInfoVM.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay.HasValue)
            {
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ErrorTitle, ResPurchaseOrderMaintain.AlertMsg_ETACheck, MessageType.Error);
                return false;
            }
            return true;
        }

        private bool CheckEIMSInfo()
        {
            bool result = true;
            decimal eimsTotalAmt = 0;
            if (null != this.poInfoVM.EIMSInfo.EIMSInfoList)
            {
                foreach (EIMSInfoVM item in this.poInfoVM.EIMSInfo.EIMSInfoList)
                {
                    decimal eimsAmt = 0m;
                    item.EIMSAmt = string.IsNullOrEmpty(item.EIMSAmt) ? "0" : item.EIMSAmt;
                    if (!decimal.TryParse(item.EIMSAmt, out eimsAmt))
                    {
                        Window.Alert(string.Format("返点编号[{0}]的使用返点金额必须为小数!", item.EIMSSysNo));
                        return false;
                    }
                    decimal eimsLeftAmt = item.EIMSLeftAmt.Value;
                    if (eimsAmt <= 0)
                    {
                        result = false;
                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ErrorTitle, ResPurchaseOrderMaintain.AlertMsg_EIMSAmtCheck, MessageType.Error);
                        break;
                    }
                    if (eimsAmt > eimsLeftAmt)
                    {
                        result = false;
                        Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ErrorTitle, ResPurchaseOrderMaintain.AlertMsg_EIMSLeftAmtCheck, MessageType.Error);
                        break;
                    }
                    eimsTotalAmt += eimsAmt;
                }
            }
            decimal totalAmt = 0;
            foreach (PurchaseOrderItemInfoVM item in this.poInfoVM.POItems)
            {
                totalAmt += item.PurchaseQty.ToInteger() * item.OrderPrice.ToDecimal();
            }
            if (this.poInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Normal && totalAmt < eimsTotalAmt)
            {
                result = false;
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ErrorTitle, string.Format(ResPurchaseOrderMaintain.AlertMsg_EIMSTotalAmtCheck, eimsTotalAmt.ToString("f2"), totalAmt.ToString("f2")), MessageType.Error);
            }
            return result;
        }

        private void CalcEIMSInfo()
        {
            if (null != this.poInfoVM.EIMSInfo.EIMSInfoList)
            {
                foreach (EIMSInfoVM item in this.poInfoVM.EIMSInfo.EIMSInfoList)
                {
                    item.AlreadyUseAmt = (item.EIMSTotalAmt ?? 0m) - (item.EIMSLeftAmt ?? 0m);
                    item.LeftAmt = Convert.ToDecimal(item.EIMSAmt ?? "0");
                }
            }
        }

        /// <summary>
        /// 添加PO单 - 审核操作权限:
        /// </summary>
        private void AddPurchaseOrderPrivilege()
        {
            if (null != this.poInfoVM.PurchaseOrderBasicInfo.Privilege && 0 < this.poInfoVM.PurchaseOrderBasicInfo.Privilege.Count)
            {
                return;
            }
            List<PurchaseOrderPrivilege> privilege = new List<PurchaseOrderPrivilege>();
            // 审核采购单(最高权限)
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_AuditAll))
            {
                privilege.Add(PurchaseOrderPrivilege.CanAuditAll);
            }
            // 审核采购单(中级权限)
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_AuditGeneric))
            {
                privilege.Add(PurchaseOrderPrivilege.CanAuditGeneric);
            }
            //具有对采购单审核-发票超期权限
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_AuditInvoiceAbsent))
            {
                privilege.Add(PurchaseOrderPrivilege.CanAuditInvoiceAbsent);
            }
            //具有对滞收发票PM的权限审核
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_AuditLagInvoice))
            {
                privilege.Add(PurchaseOrderPrivilege.CanAuditLagInvoice);
            }
            //审核采购单(一般权限)
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_AuditLow))
            {
                privilege.Add(PurchaseOrderPrivilege.CanAuditLow);
            }
            //具有审核负采购单的权限
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_AuditNegativeStock))
            {
                privilege.Add(PurchaseOrderPrivilege.CanAuditNegativeStock);
            }
            //审核滞销收货PO权限
            if (AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_AuditLagGoods))
            {
                privilege.Add(PurchaseOrderPrivilege.CanAuditLagGoods);
            }
            this.poInfoVM.PurchaseOrderBasicInfo.Privilege = privilege;
        }

        #endregion

        private PurchaseOrderInfo BuildVMToEntity(PurchaseOrderInfoVM vm)
        {
            PurchaseOrderInfo info = EntityConverter<PurchaseOrderInfoVM, PurchaseOrderInfo>.Convert(vm, (s, t) =>
            {
                t.PurchaseOrderBasicInfo.StockInfo = new BizEntity.Inventory.StockInfo();
                t.PurchaseOrderBasicInfo.ITStockInfo = new BizEntity.Inventory.StockInfo();
                t.PurchaseOrderBasicInfo.ProductManager = new BizEntity.IM.ProductManagerInfo() { UserInfo = new BizEntity.Common.UserInfo() };
                t.PurchaseOrderBasicInfo.ShippingType = new BizEntity.Common.ShippingType();
                t.PurchaseOrderBasicInfo.PayType = new BizEntity.Common.PayType();
                t.PurchaseOrderBasicInfo.StockInfo.SysNo = s.PurchaseOrderBasicInfo.StockSysNo;
                t.PurchaseOrderBasicInfo.CurrencyCode = s.PurchaseOrderBasicInfo.CurrencyCode.ToInteger();
                t.PurchaseOrderBasicInfo.StockInfo.StockName = s.PurchaseOrderBasicInfo.StockName;
                t.PurchaseOrderBasicInfo.ITStockInfo.SysNo = s.PurchaseOrderBasicInfo.ITStockSysNo;
                t.PurchaseOrderBasicInfo.ITStockInfo.StockName = s.PurchaseOrderBasicInfo.ITStockName;
                t.PurchaseOrderBasicInfo.ProductManager.SysNo = s.PurchaseOrderBasicInfo.PMSysNo.ToNullableToInteger();
                t.PurchaseOrderBasicInfo.ProductManager.UserInfo.UserName = s.PurchaseOrderBasicInfo.PMName;
                t.PurchaseOrderBasicInfo.ShippingType.SysNo = s.PurchaseOrderBasicInfo.ShippingTypeSysNo.ToNullableToInteger();
                t.PurchaseOrderBasicInfo.ShippingType.ShippingTypeName = s.PurchaseOrderBasicInfo.ShippingTypeName;
                t.PurchaseOrderBasicInfo.PayType.SysNo = s.PurchaseOrderBasicInfo.PayTypeSysNo;
                t.PurchaseOrderBasicInfo.PayType.PayTypeName = s.PurchaseOrderBasicInfo.PayTypeName;
                t.PurchaseOrderBasicInfo.TaxRate = ((decimal)t.PurchaseOrderBasicInfo.TaxRateType) / 100;
                t.PurchaseOrderBasicInfo.CarriageCost = string.IsNullOrEmpty(this.txtCarriageCost.Text.Trim()) ? (decimal?)null : Convert.ToDecimal(this.txtCarriageCost.Text.Trim());
                t.PurchaseOrderBasicInfo.Privilege = s.PurchaseOrderBasicInfo.Privilege;
                t.PurchaseOrderBasicInfo.LogisticsNumber = s.PurchaseOrderBasicInfo.LogisticsNumber;
                t.PurchaseOrderBasicInfo.ExpressName = s.PurchaseOrderBasicInfo.ExpressName;
            });
            return info;
        }

        private void hpkRMAOver15Days_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(string.Format("/ECCentral.Portal.UI.RMA/OutBoundNotReturnQuery/{0}|{1}|{2}", 15, 1, poInfoVM.VendorInfo.SysNo.Value), null, true);
        }

        private void hpkEIMS_Delete_Click(object sender, RoutedEventArgs e)
        {
            //返点信息 - 删除操作 ：
            EIMSInfoVM vm = this.gridEIMSInfo.SelectedItem as EIMSInfoVM;
            if (null != vm)
            {
                this.poInfoVM.EIMSInfo.EIMSInfoList.Remove(vm);
                this.gridEIMSInfo.Bind();
                RefreshEIMSTotalText();
            }
        }

        private void txtEIMSAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshEIMSTotalText();
        }

        private void txtEIMSAmt_TextChanged(object sender, TextChangedEventArgs e)
        {
            EIMSInfoVM vm = this.gridEIMSInfo.SelectedItem as EIMSInfoVM;
            if (null != vm)
            {
                vm.EIMSAmt = ((TextBox)sender).Text.Trim();
            }
        }

        private void hpkShiftSysNo_Click(object sender, RoutedEventArgs e)
        {
            //移仓单编号:链接:
            if (!string.IsNullOrEmpty(this.hpkShiftSysNo.Content.ToString()))
            {
                Window.Navigate("//ECCentral.Portal.UI.Inventory/ShiftRequestMaintain/" + this.hpkShiftSysNo.Content.ToString(), null, true);
            }
        }

        private void hpkProductID_Click(object sender, RoutedEventArgs e)
        {
            var item = this.gridProductsListInfo.SelectedItem as PurchaseOrderItemInfoVM;
            if (null != item)
            {
                //Ocean.20130514, Move to ControlPanelConfiguration
                string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
                UtilityHelper.OpenWebPage(string.Format(urlFormat, item.ProductSysNo));                
            }
        }
    }
}
