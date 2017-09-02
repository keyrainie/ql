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
    [View(IsSingleton = false)]
    public partial class PurchaseOrderNew : PageBase
    {
        public PurchaseOrderInfoVM newPOInfoVM;
        public PurchaseOrderFacade serviceFacade;
        public VendorFacade vendorFacade;
        List<WarehouseInfo> m_warehouseInfoList;
        List<CodeNamePair> m_purchaseOrderCompanyMappingDefaultStock;

        public PurchaseOrderInfoVM RequestPurchaseOrderInfo
        {
            get { 
                return this.Request.UserState as PurchaseOrderInfoVM;
            }
        }

        #region [自动发送Email相关]
        public string AutoMail_AutoMailAddress = string.Empty;
        public string AutoMail_VendorMailAddress = string.Empty;
        public string AutoMail_HaveSentMailAddress = string.Empty;
        #endregion

        public PurchaseOrderNew()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            serviceFacade = new PurchaseOrderFacade(this);
            vendorFacade = new VendorFacade(this);

            InitializeComboBoxData();

            SetAccessControl();
            
            //初始供应商选择控件动作
            ucVendor.InitVendorAction = InitVendor;

            newPOInfoVM = new PurchaseOrderInfoVM();
            if (RequestPurchaseOrderInfo != null && RequestPurchaseOrderInfo.VendorInfo.SysNo > 0)
            {
                //从采购蓝过来的数据
                newPOInfoVM = RequestPurchaseOrderInfo;
                ucVendor.LoadVendorInfo(RequestPurchaseOrderInfo.VendorInfo.SysNo.Value);
            }
        }
        private void SetAccessControl()
        {
            //创建PO单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_CreatePO))
            {
                this.btnCreatePO.IsEnabled = false;
            }
        }

        private void InitializeComboBoxData()
        {
            //直送仓库默认选择仓
            CodeNamePairHelper.GetList(ConstValue.DomainName_PO, ConstValue.PO_KeyPurchaseOrderCompanyMappingDefaultStock, (o, r) =>
            {
                if (r.FaultsHandle()) return;
                m_purchaseOrderCompanyMappingDefaultStock = r.Result;
            });
            //仓库:
            serviceFacade.GetPurchaseOrderWarehouseList((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                m_warehouseInfoList = args.Result;

                List<KeyValuePair<TransferType?, string>> its = EnumConverter.GetKeyValuePairs<TransferType>(EnumConverter.EnumAppendItemType.None);
                its.RemoveAt(1);

                this.cmbTransferType.ItemsSource = its;

                this.cmbTransfer.ItemsSource = EnumConverter.GetKeyValuePairs<PaySettleITCompany>(EnumConverter.EnumAppendItemType.None);
                this.cmbTransfer.SelectedIndex = 0;
            });
            //账期属性:
            this.cmbConsignFlag.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderConsignFlag>(EnumConverter.EnumAppendItemType.None);
            //采购单类型:
            this.cmbPOType.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderType>(EnumConverter.EnumAppendItemType.None).GetRange(0, 2);
            //增值税率:
            this.cmbTaxRate.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderTaxRate>(EnumConverter.EnumAppendItemType.None);
            //预计到货时间段:
            this.cmbETAHalfDay.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderETAHalfDayType>(EnumConverter.EnumAppendItemType.Select);
        }

        #region [Events]

        public void InitVendor()
        {
            InitPage();
        }

        private void SetConsignFlag()
        {
            if (newPOInfoVM.VendorInfo.VendorBasicInfo.ConsignFlag == VendorConsignFlag.Sell)
            {
                newPOInfoVM.PurchaseOrderBasicInfo.ConsignFlag = PurchaseOrderConsignFlag.UnConsign;
            }
            else if (newPOInfoVM.VendorInfo.VendorBasicInfo.ConsignFlag == VendorConsignFlag.GatherPay)
            {
                newPOInfoVM.PurchaseOrderBasicInfo.ConsignFlag = PurchaseOrderConsignFlag.GatherPay;
            }
            else
            {
                newPOInfoVM.PurchaseOrderBasicInfo.ConsignFlag = PurchaseOrderConsignFlag.Consign;
            }
        }

        private void InitPage()
        {
            SetTransferEnable(false);         
            hpkRMAOver15Days.Visibility = Visibility.Collapsed;
        }

        void SetTransferEnable(bool isEnabled)
        {
            if (!isEnabled)
            {
                this.cmbTransferType.SelectedIndex = this.cmbStock.SelectedIndex = -1;
                this.cmbTransfer.Visibility = System.Windows.Visibility.Collapsed;
            }
            this.cmbTransferType.IsEnabled = this.cmbStock.IsEnabled = isEnabled;
        }

        private void cmbTransferType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTransferType.SelectedValue != null)
            {
                var selectItem = (TransferType)cmbTransferType.SelectedValue;
                var defaultStock = m_purchaseOrderCompanyMappingDefaultStock.FirstOrDefault(p =>p.Code == (!newPOInfoVM.VendorInfo.VendorBasicInfo.PaySettleCompany.HasValue
                                                                                                ? "0"
                                                                                                : ((int)newPOInfoVM.VendorInfo.VendorBasicInfo.PaySettleCompany).ToString())
                                                                                           );
                switch (selectItem)
                {
                    case TransferType.Direct:
                        this.cmbTransfer.Visibility = Visibility.Collapsed;
                        this.cmbStock.ItemsSource = m_warehouseInfoList;
                        
                        if (defaultStock != null && RequestPurchaseOrderInfo == null)
                        {
                            newPOInfoVM.PurchaseOrderBasicInfo.StockSysNo = int.Parse(defaultStock.Name);
                        }
                        break;
                    case TransferType.Indirect:
                        this.cmbTransfer.Visibility = System.Windows.Visibility.Visible;
                        this.cmbTransfer.SelectedValue = Enum.Parse(typeof(PaySettleITCompany)
                            , (!newPOInfoVM.VendorInfo.VendorBasicInfo.PaySettleCompany.HasValue ? PaySettleCompany.SH.ToString() : newPOInfoVM.VendorInfo.VendorBasicInfo.PaySettleCompany.ToString())
                            , true);
                        //移除自身的默认城市选项
                        if (defaultStock != null)
                        {
                            this.cmbStock.ItemsSource = m_warehouseInfoList.Where(p => p.SysNo != int.Parse(defaultStock.Name));
                        }
                        else
                        {
                            this.cmbStock.ItemsSource = m_warehouseInfoList;
                        }
                        if (RequestPurchaseOrderInfo == null)
                        {
                            this.cmbStock.SelectedIndex = 0;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ucCurrency_CurrencySelectChanged(object sender, Basic.Components.UserControls.CurrencyTypePicker.CurrencySelectedEventArgs e)
        {
            if (newPOInfoVM.PurchaseOrderBasicInfo.TotalAmt.HasValue)
            {
                if (null != e.SelectedCurrencyInfo.SysNo)
                {
                    this.txtTotalAmt.Text = string.Format("{0}{1}", e.SelectedCurrencyInfo.CurrencySymbol, newPOInfoVM.PurchaseOrderBasicInfo.TotalAmt.Value.ToString("f2"));
                    this.newPOInfoVM.PurchaseOrderBasicInfo.CurrencySymbol = e.SelectedCurrencyInfo.CurrencySymbol;
                }
                else
                {
                    this.txtTotalAmt.Text = string.Format("{0}", newPOInfoVM.PurchaseOrderBasicInfo.TotalAmt.Value.ToString("f2"));
                    this.newPOInfoVM.PurchaseOrderBasicInfo.CurrencySymbol = null;
                }
            }

        }

        private void gridVendorAgentInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.gridVendorAgentInfo.ItemsSource = newPOInfoVM.VendorInfo.VendorAgentInfoList;
        }

        private void gridProductsListInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            this.gridProductsListInfo.ItemsSource = this.newPOInfoVM.POItems;
        }

        private void gridEIMSInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.gridEIMSInfo.ItemsSource = newPOInfoVM.EIMSInfo.EIMSInfoList;
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
                if (this.newPOInfoVM.VendorInfo.SysNo.HasValue)
                {
                    vendorFacade.GetVendorBySysNo(newPOInfoVM.VendorInfo.SysNo.Value.ToString(), (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        VendorInfo getVendorInfo = args.Result;
                        if (null != getVendorInfo)
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
                WarehouseInfo selectedItem = this.cmbStock.SelectedItem as WarehouseInfo;
                string getCurrenctSelectedStockName = selectedItem.WarehouseName.ToString();
                PurchaseOrderProductsMaintain editItemCtrl = new PurchaseOrderProductsMaintain(getCurrenctSelectedStockName, itemVM, newPOInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType.Value);
                editItemCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderNew.AlertMsg_ModifyProductTitle, editItemCtrl, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        PurchaseOrderItemInfoVM editVM = args.Data as PurchaseOrderItemInfoVM;
                        if (null != editVM)
                        {
                            itemVM.PurchaseQty = editVM.PurchaseQty;
                            itemVM.OrderPrice = editVM.OrderPrice;
                            this.gridProductsListInfo.Bind();
                            UpdatePOTotalAmt();
                        }
                    }
                });
            }
        }

        private void hpkEditReasonMemo_Click(object sender, RoutedEventArgs e)
        {
            //详细原因查看:
            PurchaseOrderCheckReasonDetail detailCtrl = new PurchaseOrderCheckReasonDetail(newPOInfoVM, false);
            detailCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderNew.AlertMsg_CheckPOTitle, detailCtrl, (obj, args) =>
            {

            }, new Size(400, 200));
        }

        private void hpkMaintainReceivePerson_Click(object sender, RoutedEventArgs e)
        {
            //维护收件人列表:
            if (!this.newPOInfoVM.VendorInfo.SysNo.HasValue)
            {
                Window.Alert(ResPurchaseOrderNew.AlertMsg_VendorEmpty);
                return;
            }
            PurchaseOrderAutoSendMailMaintain mailMaintain = new PurchaseOrderAutoSendMailMaintain(this.newPOInfoVM.VendorInfo.SysNo.Value, AutoMail_AutoMailAddress, AutoMail_VendorMailAddress, AutoMail_HaveSentMailAddress);
            mailMaintain.Dialog = Window.ShowDialog(ResPurchaseOrderNew.AlertMsg_MailMaintain, mailMaintain, (obj, args) =>
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

        private void hpkQueryInventory_Click(object sender, RoutedEventArgs e)
        {
            // 查询分仓库存链接:
            PurchaseOrderItemInfoVM getSelectedItem = this.gridProductsListInfo.SelectedItem as PurchaseOrderItemInfoVM;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.Inventory/InventoryQuery/{0}", getSelectedItem.ProductSysNo.Value), null, true);
            }
        }

        private void hpkContractDetailView_Click(object sender, RoutedEventArgs e)
        {
            //查看详情:
            if (!string.IsNullOrEmpty(this.txtContractNumber.Text))
            {
                PurchaseOrderEIMSRuleQuery eimsRuleInfoCtrl = new PurchaseOrderEIMSRuleQuery(this.txtContractNumber.Text);
                eimsRuleInfoCtrl.Dialog = Window.ShowDialog("EIMS合同信息查看", eimsRuleInfoCtrl, (obj, args) =>
                {

                }, new Size(900, 400));
            }
        }

        private void hpkEimsInvoice_Click(object sender, RoutedEventArgs e)
        {
            //可用返点提醒  ：
            if (newPOInfoVM.VendorInfo == null || !newPOInfoVM.VendorInfo.SysNo.HasValue)
            {
                Window.Alert(ResPurchaseOrderNew.AlertMsg_VendorEmpty);
                return;
            }
            //if (!string.IsNullOrEmpty(this.txtContractNumber.Text))
            //{
            PurchaseOrderEIMSInvoiceList eimsSearchCtrl = new PurchaseOrderEIMSInvoiceList(newPOInfoVM.VendorInfo.SysNo.Value);
            eimsSearchCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderNew.AlertMsg_LeftEIMS, eimsSearchCtrl, (obj, args) =>
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
                                newPOInfoVM.EIMSInfo.EIMSInfoList.Add(x);
                            }
                        });
                        gridEIMSInfo.Bind();
                        RefreshEIMSTotalText();
                    }
                }
            }, new Size(700, 400));
            //}
        }

        //判断当前返点列表中是否存在相同记录
        private bool IsExistForEIMSInfoVM(EIMSInfoVM eimsInfo)
        {
            foreach (EIMSInfoVM tobj in newPOInfoVM.EIMSInfo.EIMSInfoList)
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

        private void btnCreatePO_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (newPOInfoVM == null || newPOInfoVM.VendorInfo == null || newPOInfoVM.VendorInfo.SysNo == null)
            {
                Window.Alert("请选择供应商！");
                return;
            }
            //新建采购单:
            //1.检查收件人列表:
            BuildPOReceivedMailAddress();
            AddPurchaseOrderPrivilege();
            //检查返点信息:
            if (!CheckEIMSInfo())
            {
                return;
            }
            //保存前 计算返点信息
            CalcEIMSInfo();
            PurchaseOrderInfo info = BuildVMToEntity();
            //保存PM高级权限，用于业务端验证
            info.PurchaseOrderBasicInfo.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
            serviceFacade.CreatePurchaseOrder(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result.SysNo.HasValue)
                {
                    Window.Alert(ResPurchaseOrderNew.Msg_Title, ResPurchaseOrderNew.Msg_CreatePOSuc, MessageType.Information, (objj, argss) =>
                    {
                        if (argss.DialogResult == DialogResultType.Cancel)
                        {
                            Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/PurchaseOrderMaintain/{0}", args.Result.SysNo.Value), true);
                        }
                    });
                }
                else
                {
                    Window.Alert(ResPurchaseOrderNew.AlertMsg_CreatePOFailed);
                    return;
                }
            });
        }

        private bool CheckEIMSInfo()
        {
            bool result = true;
            decimal eimsTotalAmt = 0;
            if (null != this.newPOInfoVM.EIMSInfo.EIMSInfoList)
            {
                foreach (EIMSInfoVM item in this.newPOInfoVM.EIMSInfo.EIMSInfoList)
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
            foreach (PurchaseOrderItemInfoVM item in this.newPOInfoVM.POItems)
            {
                totalAmt += item.PurchaseQty.ToInteger() * item.OrderPrice.ToDecimal();
            }
            if (this.newPOInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Normal && totalAmt < eimsTotalAmt)
            {
                result = false;
                Window.Alert(ResPurchaseOrderMaintain.AlertMsg_ErrorTitle, string.Format(ResPurchaseOrderMaintain.AlertMsg_EIMSTotalAmtCheck, eimsTotalAmt.ToString("f2"), totalAmt.ToString("f2")), MessageType.Error);
            }
            return result;
        }

        private void CalcEIMSInfo()
        {
            if (null != this.newPOInfoVM.EIMSInfo.EIMSInfoList)
            {
                foreach (EIMSInfoVM item in this.newPOInfoVM.EIMSInfo.EIMSInfoList)
                {
                    item.AlreadyUseAmt = (item.EIMSTotalAmt ?? 0m) - (item.EIMSLeftAmt ?? 0m);
                    item.LeftAmt = Convert.ToDecimal(item.EIMSAmt ?? "0");
                }
            }
        }

        private void ucVendor_VendorSelected(object sender, Basic.Components.UserControls.VendorPicker.VendorSelectedEventArgs e)
        {
            //修改供应商，重新选择供应商:
            InitPage();
            var selectVendorArgs = e as ECCentral.Portal.Basic.Components.UserControls.VendorPicker.VendorSelectedEventArgs;
            if (selectVendorArgs.SelectedVendorInfo.SysNo.HasValue)
            {
                SetTransferEnable(true);
                var selectVendor = selectVendorArgs.SelectedVendorInfo;             

                newPOInfoVM.VendorInfo = EntityConverter<VendorInfo, VendorInfoVM>.Convert(selectVendor);
                for (int i = 0; i < selectVendor.VendorAgentInfoList.Count; i++)
                {
                    newPOInfoVM.VendorInfo.VendorAgentInfoList[i].BrandInfo.BrandNameDisplay = selectVendor.VendorAgentInfoList[i].BrandInfo.BrandNameLocal.Content;
                    newPOInfoVM.VendorInfo.VendorAgentInfoList[i].ManufacturerInfo.ManufacturerNameDisplay = selectVendor.VendorAgentInfoList[i].ManufacturerInfo.ManufacturerNameLocal.Content;
                }
                AutoMail_VendorMailAddress = string.IsNullOrEmpty(newPOInfoVM.VendorInfo.VendorBasicInfo.EmailAddress) ? string.Empty : newPOInfoVM.VendorInfo.VendorBasicInfo.EmailAddress.Trim();
                this.gridVendorAgentInfo.Bind();

                if (selectVendor.VendorBasicInfo.ConsignFlag == VendorConsignFlag.Sell)
                {
                    this.cmbConsignFlag.SelectedIndex = 0;
                }
                else if (selectVendor.VendorBasicInfo.ConsignFlag == VendorConsignFlag.GatherPay)
                {
                    this.cmbConsignFlag.SelectedIndex = 2;
                }
                else
                {
                    this.cmbConsignFlag.SelectedIndex = 1;
                }
                //送修超过15天催讨未果RMAList,请协助:
                //this.hpkRMAOver15Days.Visibility = Visibility.Visible;
                this.hpkRMAOver15Days.Content = ResPurchaseOrderNew.Label_RMARegisterText;

                if (RequestPurchaseOrderInfo != null)
                {
                    this.gridVendorAgentInfo.Bind();
                    SetConsignFlag();

                    //加载Item信息:
                    List<PurchaseOrderItemProductInfo> listNewProduct = new List<PurchaseOrderItemProductInfo>();
                    foreach (var item in newPOInfoVM.POItems)
                    {
                        PurchaseOrderItemProductInfo product = new PurchaseOrderItemProductInfo();
                        product.CurrencySysNo = Convert.ToInt32(newPOInfoVM.PurchaseOrderBasicInfo.CurrencyCode);
                        product.OrderPrice = !string.IsNullOrEmpty(item.OrderPrice) ? Convert.ToDecimal(item.OrderPrice) : 0m;
                        product.SysNo = item.ProductSysNo.Value;
                        product.TaxRate = newPOInfoVM.PurchaseOrderBasicInfo.TaxRate.Value;
                        product.PurchaseQty = item.Quantity.Value;
                        product.POItemSysNo = -item.ItemSysNo.Value;
                        product.ReadyQuantity = item.ReadyQuantity;
                        product.StockSysNo = newPOInfoVM.PurchaseOrderBasicInfo.StockSysNo.Value;
                        listNewProduct.Add(product);
                    }
                    serviceFacade.BatchAddNewPurchaseOrderItem(listNewProduct, (objItem, argsItem) =>
                    {
                        if (argsItem.FaultsHandle())
                        {
                            return;
                        }
                        newPOInfoVM.POItems = EntityConverter<List<PurchaseOrderItemInfo>, List<PurchaseOrderItemInfoVM>>.Convert(argsItem.Result);
                        //加载Item信息完毕:
                        this.DataContext = newPOInfoVM;
                        this.gridProductsListInfo.Bind();
                        //4.获取赠品信息，附件信息:
                        GetProductAppendedInfo(() =>
                        {
                            UpdatePOTotalAmt();
                        });
                    });

                    //采购仓库的显示部分
                    cmbTransferType.SelectedValue = newPOInfoVM.PurchaseOrderBasicInfo.ITStockSysNo.HasValue ? TransferType.Indirect : TransferType.Direct;
                    var transferType = (TransferType)cmbTransferType.SelectedValue;
                    int stockSysNo = newPOInfoVM.PurchaseOrderBasicInfo.StockSysNo ?? 0;
                    if (stockSysNo > 0)
                    {
                        if (transferType == TransferType.Indirect)
                        {
                            cmbTransfer.Visibility = System.Windows.Visibility.Visible;
                            cmbTransfer.SelectedValue = Enum.Parse(typeof(PaySettleITCompany), newPOInfoVM.VendorInfo.VendorBasicInfo.PaySettleCompany.ToString(), true);
                            newPOInfoVM.PurchaseOrderBasicInfo.StockSysNo = newPOInfoVM.PurchaseOrderBasicInfo.ITStockSysNo;
                        }
                    }
                }
                else
                {                  
                    newPOInfoVM.PurchaseOrderBasicInfo.CurrencyCode = "1";
                    newPOInfoVM.PurchaseOrderBasicInfo.CurrencySymbol = "￥";
                    //送货类型默认为“厂方直送”:
                    newPOInfoVM.PurchaseOrderBasicInfo.ShippingTypeSysNo = "12";
                    newPOInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType = PurchaseOrderType.Normal;
                    newPOInfoVM.PurchaseOrderBasicInfo.TaxRateType = PurchaseOrderTaxRate.Percent017;
                    newPOInfoVM.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Origin;
                    cmbTransferType.SelectedIndex = 0;
                    SetConsignFlag();
                    this.DataContext = newPOInfoVM;
                }
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            //添加采购商品:
            if (!newPOInfoVM.VendorInfo.SysNo.HasValue)
            {
                Window.Alert(ResPurchaseOrderNew.AlertMsg_VendorEmpty);
                VendorInfo.Focus();
                return;
            }
            if (!newPOInfoVM.PurchaseOrderBasicInfo.StockSysNo.HasValue)
            {
                Window.Alert(ResPurchaseOrderNew.AlertMsg_StockEmpty);
                VendorInfo.Focus();
                return;
            }

            //获取当前选中仓库名称:
            WarehouseInfo selectedItem = this.cmbStock.SelectedItem as WarehouseInfo;
            string getCurrenctSelectedStockName = EnumConverter.GetDescription((TransferType)cmbTransferType.SelectedValue);
            if (cmbTransfer.Visibility == System.Windows.Visibility.Visible)
            {
                //中转，字符串重新构造
                getCurrenctSelectedStockName = EnumConverter.GetDescription((PaySettleITCompany)cmbTransfer.SelectedValue);
            }
            getCurrenctSelectedStockName += selectedItem.WarehouseName;

            PurchaseOrderProductsNew newCtrl = new PurchaseOrderProductsNew(getCurrenctSelectedStockName, newPOInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType.Value);
            newCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderNew.Button_AddProduct, newCtrl, (obj, args) =>
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
                            //货币
                            CurrencySysNo = newPOInfoVM.PurchaseOrderBasicInfo.CurrencyCode.ToNullableToInteger()
                        };
                        //1.首先判断添加的商品是否已经存在:
                        if (newPOInfoVM.POItems.SingleOrDefault(i => i.ProductSysNo == productInfo.SysNo) != null)
                        {
                            Window.Alert(ResPurchaseOrderNew.AlertMsg_POItemExists);
                            return;
                        }

                        //产品线相关检测
                        //CheckProductLine(newPOInfoVM, getAddedVM, myobj =>
                        //{
                            //2.如果是新商品，则调用Service获取价格，库存等详细信息:
                            serviceFacade.AddNewPurchaseOrderItem(productInfo, (obj2, args2) =>
                            {
                                if (args2.FaultsHandle())
                                {
                                    return;
                                }

                                PurchaseOrderItemInfoVM getNewVM = EntityConverter<PurchaseOrderItemInfo, PurchaseOrderItemInfoVM>.Convert(args2.Result);
                                //3.添加到List中，并刷新Grid:
                                this.newPOInfoVM.POItems.Add(getNewVM);
                                this.gridProductsListInfo.Bind();
                                //计算总价格:

                               // 4.获取赠品信息，附件信息:
                                GetProductAppendedInfo(() =>
                                {
                                    UpdatePOTotalAmt();
                                });
                            });
                        //    return null;
                        //});
                    }
                }
            });
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
            if (newPOInfoVM.POItems.Count > 0)
            {
                serviceFacade.GetProductLineSysNoByProductList(newPOInfoVM.POItems.Select(x => x.ProductSysNo.Value).ToArray(), (obj1, args1) =>
                        {
                            if (args1.FaultsHandle())
                                return;
                            List<ProductPMLine> tList = args1.Result;
                            //如果当前ItemList中只存在一条产品线，则更新所属PM
                            if (tList != null && tList.Count != 0)
                                if (tList.Select(x => x.ProductLineSysNo.Value).Distinct().ToList().Count == tList.Count)
                                {
                                    newPOInfoVM.PurchaseOrderBasicInfo.PMSysNo = tList[0].PMSysNo.ToString();
                                }
                            callback(null);
                        });
            }
            else
            {
                //如果删完item，则清空所属PM
                newPOInfoVM.PurchaseOrderBasicInfo.PMSysNo = null;
                callback(null);
            }
        }

        #endregion

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
                giftString += ResPurchaseOrderNew.AlertMsg_GiftExists + Environment.NewLine;
                foreach (var giftItem in giftList)
                {
                    giftString += string.Format(ResPurchaseOrderMaintain.AlertMsg_GiftExistsFormatString, giftItem.Id, giftItem.GiftId, giftItem.ProductName, giftItem.OnlineQty) + Environment.NewLine;
                }
            }
            else
            {
                accessoryString += ResPurchaseOrderNew.AlertMsg_NoGift;
            }
            //附件提示:
            if (null != accessoryList && 0 < accessoryList.Count)
            {
                accessoryString += ResPurchaseOrderNew.AlertMsg_AccessoryInfo + Environment.NewLine;
                foreach (var accessoryItem in accessoryList)
                {
                    accessoryString += string.Format("【{0}】 {1}{2}", accessoryItem.Id, accessoryItem.Description, Environment.NewLine);
                }
            }
            else
            {
                accessoryString += ResPurchaseOrderNew.AlertMsg_NoAccessory + Environment.NewLine;

            }
            //商品价格异常:
            if (null != priceExceptionList && 0 < priceExceptionList.Count)
            {
                priceExceptionString += ResPurchaseOrderNew.AlertMsg_ProductPriceException + Environment.NewLine;
                foreach (var priceExceptionItem in priceExceptionList)
                {
                    priceExceptionItem.PriceChangePercent = GetPriceChangePercent(priceExceptionItem.OrderPrice.ToDecimal(), priceExceptionItem.LastOrderPrice.Value);
                    priceExceptionString += string.Format(ResPurchaseOrderNew.AlertMsg_ProductPriceExceptionFormatString, priceExceptionItem.ProductID, priceExceptionItem.LastOrderPrice.Value.ToString("f2"), Convert.ToDecimal(priceExceptionItem.OrderPrice).ToString("f2"), priceExceptionItem.PriceChangePercent.Value.ToString("f2")) + Environment.NewLine;
                }
            }

            this.lblCheckResult.Text = giftString + accessoryString + priceExceptionString;
        }

        private void GetProductAppendedInfo(Action action)
        {
            List<int> productSysNoList = new List<int>();

            foreach (var item in newPOInfoVM.POItems)
            {
                productSysNoList.Add(item.ProductSysNo.Value);
            }
            if (0 < productSysNoList.Count)
            {
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
                           List<PurchaseOrderItemInfoVM> priceExceptionList = newPOInfoVM.POItems.Where(a => a.ExecptStatus == -1 && a.LastOrderPrice.HasValue).ToList();
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

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            //删除采购商品:
            if (null == this.newPOInfoVM.POItems || 0 >= this.newPOInfoVM.POItems.Count)
            {
                Window.Alert(ResPurchaseOrderNew.AlertMsg_NoItemDelete);
                return;
            }
            if (this.newPOInfoVM.POItems.Count(i => i.IsCheckedItem == true) <= 0)
            {
                Window.Alert(ResPurchaseOrderNew.AlertMsg_SelectDeleteItem);
                return;
            }

            Window.Confirm(ResPurchaseOrderNew.AlertMsg_ConfrimItemDelete, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    //1.移除所选商品:
                    this.newPOInfoVM.POItems.RemoveAll(i => i.IsCheckedItem == true);
                    //验证并更新所属PM
                    WritePMSysNo(obj1 =>
                    {
                        this.gridProductsListInfo.Bind();
                        //2.刷新商品信息:
                        if (newPOInfoVM.POItems.Count > 0)
                        {
                            GetProductAppendedInfo(() =>
                            {
                                UpdatePOTotalAmt();
                            });
                        }
                        else
                        {
                            lblCheckResult.Text = string.Empty;
                            UpdatePOTotalAmt();
                        }
                        return null;
                    });
                }
            });
        }
        #endregion

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

        //检查并构建收件人列表:
        private void BuildPOReceivedMailAddress()
        {
            if (this.chkAutoEmail.IsChecked == true)
            {
                if (null != this.lbPOReceivePersonList.Items && 0 >= this.lbPOReceivePersonList.Items.Count)
                {
                    Window.Alert(ResPurchaseOrderNew.AlertMsg_MailAddressEmpty);
                    return;
                }
            }
            string getReceivedMailAddress = string.Empty;
            if (null != this.lbPOReceivePersonList.Items)
            {
                foreach (var item in this.lbPOReceivePersonList.Items)
                {
                    getReceivedMailAddress += string.Format("{0};", item.ToString());
                }
                getReceivedMailAddress = getReceivedMailAddress.TrimEnd(';');
            }
            this.newPOInfoVM.PurchaseOrderBasicInfo.AutoSendMailAddress = getReceivedMailAddress;
        }

        private void UpdatePOTotalAmt()
        {
            decimal updateTotalAmt = 0;
            this.newPOInfoVM.POItems.ForEach(x =>
            {
                updateTotalAmt += x.OrderPrice.ToDecimal() * x.PurchaseQty.ToInteger();
            });
            this.newPOInfoVM.PurchaseOrderBasicInfo.TotalAmt = updateTotalAmt;

            string getSelectCurrencySymbol =
                this.txtTotalAmt.Text = string.Format("{0}{1}", (string.IsNullOrEmpty(this.newPOInfoVM.PurchaseOrderBasicInfo.CurrencySymbol) ? "" : this.newPOInfoVM.PurchaseOrderBasicInfo.CurrencySymbol), updateTotalAmt.ToString("f2"));
        }

        private void cmbPOType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private decimal GetPriceChangePercent(decimal OrderPrice, decimal lastOrderPrice)
        {
            decimal diff = (OrderPrice - Convert.ToDecimal(lastOrderPrice));
            lastOrderPrice = lastOrderPrice == 0 ? 1m : lastOrderPrice;
            return (diff * 100 / lastOrderPrice);
        }

        private void RefreshEIMSTotalText()
        {
            decimal totalEIMS = 0m;
            decimal totalUsedEIMS = 0m;
            if (null != newPOInfoVM.EIMSInfo && null != newPOInfoVM.EIMSInfo.EIMSInfoList && 0 < newPOInfoVM.EIMSInfo.EIMSInfoList.Count)
            {
                newPOInfoVM.EIMSInfo.EIMSInfoList.ForEach(vm =>
                {
                    totalEIMS += (!string.IsNullOrEmpty(vm.EIMSAmt) ? Convert.ToDecimal(vm.EIMSAmt) : 0);
                    totalUsedEIMS += (vm.AlreadyUseAmt.HasValue ? vm.AlreadyUseAmt.Value : 0);
                });
            }
            newPOInfoVM.EIMSInfo.TotalEIMS = totalEIMS;
            newPOInfoVM.EIMSInfo.TotalUsedEIMS = totalUsedEIMS;
            this.lblEIMSTotal.Text = string.Format(ResPurchaseOrderNew.AlertMsg_EIMSTotalFormatString, newPOInfoVM.EIMSInfo.TotalEIMS.Value.ToString("f2"), newPOInfoVM.EIMSInfo.TotalUsedEIMS.Value.ToString("f2"));
        }

        private PurchaseOrderInfo BuildVMToEntity()
        {
            PurchaseOrderInfo info = EntityConverter<PurchaseOrderInfoVM, PurchaseOrderInfo>.Convert(newPOInfoVM, (s, t) =>
            {
                t.PurchaseOrderBasicInfo.StockInfo = new BizEntity.Inventory.StockInfo();
                t.PurchaseOrderBasicInfo.ITStockInfo = new BizEntity.Inventory.StockInfo();
                t.PurchaseOrderBasicInfo.ProductManager = new BizEntity.IM.ProductManagerInfo() { UserInfo = new BizEntity.Common.UserInfo() };
                t.PurchaseOrderBasicInfo.ShippingType = new BizEntity.Common.ShippingType();
                t.PurchaseOrderBasicInfo.PayType = new BizEntity.Common.PayType();

                t.VendorInfo.VendorBasicInfo.PaySettleCompany = s.VendorInfo.VendorBasicInfo.PaySettleCompany;
                t.PurchaseOrderBasicInfo.StockInfo.SysNo = s.PurchaseOrderBasicInfo.StockSysNo;
                t.PurchaseOrderBasicInfo.StockInfo.StockName = s.PurchaseOrderBasicInfo.StockName;
                t.PurchaseOrderBasicInfo.ITStockInfo.SysNo = s.PurchaseOrderBasicInfo.ITStockSysNo;
                t.PurchaseOrderBasicInfo.ITStockInfo.StockName = s.PurchaseOrderBasicInfo.ITStockName;
                t.PurchaseOrderBasicInfo.LogisticsNumber = s.PurchaseOrderBasicInfo.LogisticsNumber;
                t.PurchaseOrderBasicInfo.ExpressName = s.PurchaseOrderBasicInfo.ExpressName;
                if (cmbTransferType.SelectedValue != null && t.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue)
                {
                    if ((TransferType)cmbTransferType.SelectedValue == TransferType.Indirect)
                    {
                        t.PurchaseOrderBasicInfo.StockInfo.SysNo = int.Parse(string.Format("{0}{1}", 50, t.PurchaseOrderBasicInfo.StockInfo.SysNo));
                        t.PurchaseOrderBasicInfo.ITStockInfo.SysNo = s.PurchaseOrderBasicInfo.StockSysNo;
                        t.PurchaseOrderBasicInfo.ITStockInfo.StockName = s.PurchaseOrderBasicInfo.StockName;
                    }
                }
                t.PurchaseOrderBasicInfo.ProductManager.SysNo = s.PurchaseOrderBasicInfo.PMSysNo.ToNullableToInteger();
                t.PurchaseOrderBasicInfo.ProductManager.UserInfo.UserName = s.PurchaseOrderBasicInfo.PMName;
                t.PurchaseOrderBasicInfo.ShippingType.SysNo = s.PurchaseOrderBasicInfo.ShippingTypeSysNo.ToNullableToInteger();
                t.PurchaseOrderBasicInfo.ShippingType.ShippingTypeName = s.PurchaseOrderBasicInfo.ShippingTypeName;
                t.PurchaseOrderBasicInfo.PayType.SysNo = s.PurchaseOrderBasicInfo.PayTypeSysNo;
                t.PurchaseOrderBasicInfo.PayType.PayTypeName = s.PurchaseOrderBasicInfo.PayTypeName;
                t.PurchaseOrderBasicInfo.TaxRate = ((decimal)t.PurchaseOrderBasicInfo.TaxRateType) / 100;
                //if (0 < t.POItems.Count)
                //{
                //    t.POItems.ForEach(x => { x.ItemSysNo = null; });
                //}
            });
            return info;
        }

        private void hpkRMAOver15Days_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(string.Format("/ECCentral.Portal.UI.RMA/OutBoundNotReturnQuery/{0}|{1}|{2}", 15, 1, newPOInfoVM.VendorInfo.SysNo.Value), null, true);
        }

        private void hpkProductID_Click(object sender, RoutedEventArgs e)
        {
            var item = this.gridProductsListInfo.SelectedItem as PurchaseOrderItemInfoVM;
            if (null != item)
            {
                //Ocean.20130514, Move to ControlPanelConfiguration
                string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebsiteProductDetailBySysNoUrl);
                UtilityHelper.OpenWebPage(string.Format(urlFormat, item.ProductSysNo.Value));
            }
        }

        private void AddPurchaseOrderPrivilege()
        {
            if (null != this.newPOInfoVM.PurchaseOrderBasicInfo.Privilege && 0 < this.newPOInfoVM.PurchaseOrderBasicInfo.Privilege.Count)
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
            this.newPOInfoVM.PurchaseOrderBasicInfo.Privilege = privilege;
        }

        private void txtEIMSAmt_TextChanged(object sender, TextChangedEventArgs e)
        {
            EIMSInfoVM vm = this.gridEIMSInfo.SelectedItem as EIMSInfoVM;
            if (null != vm)
            {
                vm.EIMSAmt = ((TextBox)sender).Text.Trim();
            }
        }

        private void hpkEIMS_Delete_Click(object sender, RoutedEventArgs e)
        {
            //返点信息 - 删除操作 ：
            EIMSInfoVM vm = this.gridEIMSInfo.SelectedItem as EIMSInfoVM;
            if (null != vm)
            {
                this.newPOInfoVM.EIMSInfo.EIMSInfoList.Remove(vm);
                this.gridEIMSInfo.Bind();
                RefreshEIMSTotalText();
            }
        }

        private void txtEIMSAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshEIMSTotalText();
        }

        private void hpkSearchBatchInfoList_Click(object sender, RoutedEventArgs e)
        {
            if (newPOInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType != PurchaseOrderType.Negative)
            {
                Window.Alert(ResPurchaseOrderNew.AlertMsg_ErrorOrderType);
                return;
            }
            //退货批次(负采购单 ）：
            PurchaseOrderItemInfoVM getSelectedVM = this.gridProductsListInfo.SelectedItem as PurchaseOrderItemInfoVM;
            if (null != getSelectedVM)
            {

                PurchaseOrderBatchInfoList batchDetailCtrl = new PurchaseOrderBatchInfoList(getSelectedVM.ItemSysNo.Value, getSelectedVM.ProductSysNo.Value, newPOInfoVM.PurchaseOrderBasicInfo.StockSysNo.Value, getSelectedVM.BatchInfo);
                batchDetailCtrl.Dialog = Window.ShowDialog(ResPurchaseOrderMaintain.AlertMsg_BatchInfo, batchDetailCtrl, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        getSelectedVM.BatchInfo = batchDetailCtrl.batchInfo;
                    }
                }, new Size(700, 350));
            }
        }
    }
}
