using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Components.UserControls.CustomerPicker;
using ECCentral.Portal.Basic.Components.UserControls.PayTypePicker;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.UI.SO.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Converters;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class SOMaintain : PageBase
    {
        #region 页面加载事件

        #region 属性定义

        private SOFacade soFacade;
        private SOQueryFacade queryFacade;
        private StockQueryFacade stockFacade;
        public SOVM soViewModel
        {
            get
            {
                var sovm = this.gridSOMaintain.DataContext as SOVM;
                if (sovm != null)
                {
                    HoldStatusInfoVisibility(sovm);
                }
                return sovm;
            }
            set
            {
                SOSysNo = value.SysNo ?? 0;
                if (SOSysNo > 0)
                {
                    HoldStatusInfoVisibility(value);
                }
                this.gridSOMaintain.DataContext = value;
            }        
        }

        private void HoldStatusInfoVisibility(SOVM soVM)
        {
            if (soVM.BaseInfoVM.HoldStatus == SOHoldStatus.BackHold
                || soVM.BaseInfoVM.HoldStatus == SOHoldStatus.WebHold)
            {
                stkHoldStatus.Visibility = Visibility.Visible;
                tbHoldStatus.Text = ResSOMaintain.Lable_Holded;
            }
            else
            {
                tbHoldStatus.Text = ResSOMaintain.Lable_UnHolded;
            }
        }

        private CommonDataFacade CommonDataFacade;

        private int ElectronicCard_ProductSysNo;
        private string ElectronicCard_ProductID = string.Empty;
        private string ElectronicCard_ProductName = string.Empty;
        private string PhysicalCard_ProductID_Prefix = string.Empty;
        private string InstalmentPayTypeSysNos = string.Empty;
        private string OnlineInstalmentPayTypeSysNos = string.Empty;

        private bool IsVATSO = false;
        private bool IsNewCreateSO = true;
        private int _soSysNo;
        private int SOSysNo
        {
            get
            {
                if (_soSysNo == 0)
                {
                    if (null != this.Request.QueryString && this.Request.QueryString.ContainsKey("SOSysNo"))
                    {
                        _soSysNo = int.TryParse(this.Request.QueryString["SOSysNo"], out _soSysNo) ? _soSysNo : 0;
                    }
                }
                return _soSysNo;
            }
            set
            {
                _soSysNo = value;
            }
        }
        private int _customerSysNo;
        private int CustomerSysNo
        {
            get
            {
                if (_customerSysNo == 0)
                {
                    if (null != this.Request.QueryString && this.Request.QueryString.ContainsKey("CustomerSysNo"))
                    {
                        _customerSysNo = int.TryParse(this.Request.QueryString["CustomerSysNo"], out _customerSysNo) ? _customerSysNo : 0;
                    }
                }
                return _customerSysNo;
            }
            set
            {
                _customerSysNo = value;
            }
        }

        private string OtherInfo
        {
            get
            {
                if (null != this.Request.QueryString && this.Request.QueryString.ContainsKey("OtherInfo"))
                {
                    return this.Request.QueryString["OtherInfo"];
                }
                return string.Empty;
            }
        }

        private List<StockVM> StockList { get; set; }

        public SOMaintain()
        {
            InitializeComponent();
        }

        #endregion

        ECCentral.BizEntity.Invoice.SOIncomeInfo CurrentSOIncomeInfo;
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            int loadCompletedCount = 0;
            int wellLoadedCount = 4;

            ECCentral.Portal.Basic.Utilities.AppSettingHelper.GetSetting(ConstValue.DomainName_SO, ConstValue.SOElectronicCard_ProductSysNo, (obj, args) =>
            {
                int tempProductSysNo = 0;
                if (int.TryParse(args.Result, out tempProductSysNo))
                {
                    ElectronicCard_ProductSysNo = tempProductSysNo;
                }

                Interlocked.Increment(ref loadCompletedCount);
                if (loadCompletedCount == wellLoadedCount)
                {
                    LoadData();
                }
            });
            ECCentral.Portal.Basic.Utilities.AppSettingHelper.GetSetting(ConstValue.DomainName_SO, ConstValue.SOElectronicCard_ProductID, (obj, args) =>
            {
                if (!string.IsNullOrEmpty(args.Result))
                {
                    ElectronicCard_ProductID = args.Result;
                }

                Interlocked.Increment(ref loadCompletedCount);
                if (loadCompletedCount == wellLoadedCount)
                {
                    LoadData();
                }
            });
            ECCentral.Portal.Basic.Utilities.AppSettingHelper.GetSetting(ConstValue.DomainName_SO, ConstValue.SOElectronicCard_ProductName, (obj, args) =>
            {
                if (!string.IsNullOrEmpty(args.Result))
                {
                    ElectronicCard_ProductName = args.Result;
                }

                Interlocked.Increment(ref loadCompletedCount);
                if (loadCompletedCount == wellLoadedCount)
                {
                    LoadData();
                }
            });
            ECCentral.Portal.Basic.Utilities.AppSettingHelper.GetSetting(ConstValue.DomainName_SO, ConstValue.SOPhysicalCard_ProductID_Prefix, (obj, args) =>
            {
                if (!string.IsNullOrEmpty(args.Result))
                {
                    PhysicalCard_ProductID_Prefix = args.Result;
                }

                Interlocked.Increment(ref loadCompletedCount);
                if (loadCompletedCount == wellLoadedCount)
                {
                    LoadData();
                }
            });

            soFacade = new SOFacade(this);
            queryFacade = new SOQueryFacade(this);
            stockFacade = new StockQueryFacade(this);
            CommonDataFacade = new CommonDataFacade(this);
            IniPageData();
        }

        private void LoadData()
        {
            if (SOSysNo > 0)
            {
                LoadSOInfo();
                SetControlReadOnly();
            }
            else if (CustomerSysNo != 0)
            {
                soViewModel = new SOVM();
                ucCustomer.CustomerSysNo = CustomerSysNo;
                ReloadCustomer();
                LoadAvailableButton();
            }
            else
            {
                soViewModel = new SOVM();
                //btnNew.Visibility = Visibility.Visible;
                LoadAvailableButton();

                switch (OtherInfo)
                {
                    case ConstValue.Key_SOIsExpiateOrder:
                        soViewModel.BaseInfoVM.IsExpiateOrder = true;
                        break;
                    case ConstValue.Key_SOIsPhoneOrder:
                        soViewModel.BaseInfoVM.IsPhoneOrder = true;
                        break;
                    case ConstValue.Key_NeedInvoice:
                        soViewModel.BaseInfoVM.NeedInvoice = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetControlReadOnly()
        {
            gridCustomerInfo.ReadOnlyControl(gridCustomerInfo.Children.Count, true);
            gridSOReceiveInfo.ReadOnlyControl(gridSOReceiveInfo.Children.Count, true);
            
            gridShippingInfo.ReadOnlyControl(gridShippingInfo.Children.Count, true);
            gridPayInfo.ReadOnlyControl(gridPayInfo.Children.Count, true);
            gridCarInfo.ReadOnlyControl(gridCarInfo.Children.Count, true);            
            hlkb_AddProductInShoppingCar.Visibility = Visibility.Collapsed;
            hlkb_btnCalculateInShoppingCar.Visibility = Visibility.Collapsed;
            hlbtnSelectAddress.Visibility = Visibility.Collapsed;

            hlkb_btnTrackingNumber.IsEnabled = true;
            ucReceiveArea.ReadOnlyControl(1,false);
            txtReceiveAddress.IsReadOnly=false;
        }

        private void LoadSOInfo()
        {
            queryFacade.QuerySOInfo(SOSysNo, vm =>
            {
                if (vm == null)
                {
                    Window.Alert(ResSO.Info_SOIsNotExist, ResSO.Info_SOIsNotExist, MessageType.Warning, (obj, args) =>
                    {
                        Window.Close();
                    });
                    return;
                }

                IsNewCreateSO = false;

                // 读取枚举 初始化订单类型
                cmbSOType.ItemsSource = EnumConverter.GetKeyValuePairs<SOType>(EnumConverter.EnumAppendItemType.None);
                cmbSOType.IsEnabled = false;

                txtSOSysNo.IsReadOnly = true;

                this.dataGridItemInfo.ItemsSource = vm.ItemsVM;
                soViewModel = vm;
                IsVATSO = false;
                if (soViewModel.InvoiceInfoVM.IsVAT == true)
                {
                    IsVATSO = true;
                }
                if (soViewModel.Merchant.MerchantID.HasValue)
                {
                    tbVendorName.Text = string.Format(ResSOMaintain.Lable_VendorName, soViewModel.Merchant.MerchantName);
                }

                ReloadSOMaintainData();

                if (soViewModel.BaseInfoVM.SOType == SOType.ElectronicCard || soViewModel.BaseInfoVM.SOType == SOType.PhysicalCard)
                {
                    ReloadGiftCardInShoppingCar();
                }
                else
                {
                    ReloadShoppingCar(false);
                }

                if (soViewModel.BaseInfoVM.IsBackOrder == true)
                {
                    txtInventoryNotEnough.Visibility = Visibility.Visible;
                    hlkb_ShowBackOrder.Visibility = Visibility.Visible;
                }

                LoadAvailableButton();

                if (soViewModel.BaseInfoVM.Status == SOStatus.OutStock)
                {
                    if ((soViewModel.InvoiceInfoVM == null) || soViewModel.InvoiceInfoVM.IsVAT != true)
                    {
                        //hlkb_SetOInvoiceTOVATInvoice.Visibility = Visibility.Visible;
                        //hlkb_SetOInvoiceTOVATInvoice.IsEnabled = true;
                    }
                    if (soViewModel.InvoiceInfoVM != null && soViewModel.InvoiceInfoVM.IsVAT == true)
                    {
                        //hlkb_SetVATInvoice.Visibility = Visibility.Visible;
                        //hlkb_SetVATInvoice.IsEnabled = true;
                        //hlkb_SaveOInvoiceTOVATInvoice.Visibility = Visibility.Visible;
                        //hlkb_SaveOInvoiceTOVATInvoice.IsEnabled = true;
                    }
                }

                //加载优惠券
                var couponPromotion = soViewModel.PromotionsVM.FirstOrDefault(p => p.PromotionType == SOPromotionType.Coupon);
                if (couponPromotion != null)
                {
                    queryFacade.QueryMKTCouponsInfoByCouponCodeSysNo(couponPromotion.PromotionSysNo.Value, (objMKT, argsMKT) =>
                    {
                        if (!argsMKT.FaultsHandle())
                        {
                            if (argsMKT.Result != null)
                            {
                                soViewModel.CouponCode = argsMKT.Result.CouponCodeSetting.CouponCode;
                            }
                        }
                    });
                }
                //配送信息
                if (soViewModel.ShippingInfoVM.FreightUserSysNo.HasValue)
                {
                    var deliveryQuery = new SODeliveryAssignTaskSearchVM();
                    deliveryQuery.PageInfo = new PagingInfo
                    {
                        PageIndex = 0,
                        PageSize = 1,
                    };
                    deliveryQuery.OrderType = ECCentral.BizEntity.SO.DeliveryType.SO;
                    deliveryQuery.OrderSysNo = soViewModel.SysNo;
                    deliveryQuery.DeliveryTime = null;
                    queryFacade.SODeliveryAssignTaskQuery(deliveryQuery, (objDelivery, argsDelivery) =>
                    {
                        if (!argsDelivery.FaultsHandle())
                        {
                            if (argsDelivery.Result.TotalCount == 1)
                            {
                                string freightUserName = (string)argsDelivery.Result.Rows[0]["Username"];
                                string deliveryUserPhone = (string)argsDelivery.Result.Rows[0]["DeliveryUserPhone"];
                                if (!string.IsNullOrEmpty(deliveryUserPhone))
                                {
                                    freightUserName = string.Format("{0}[{1}]", freightUserName, deliveryUserPhone);
                                }

                                DateTime deliveryDate = (DateTime)argsDelivery.Result.Rows[0]["DeliveryDate"];
                                string freightDeliveryDate = deliveryDate.ToString("yyyy-MM-dd");
                                freightDeliveryDate += string.Format("[{0}]", (int)argsDelivery.Result.Rows[0]["DeliveryTimeRange"]);

                                svDeliveryMsg.Visibility = System.Windows.Visibility.Visible;
                                tbDeliveryMsg.Text = string.Format(ResSOMaintain.Info_DeliveryMsg, freightUserName, freightDeliveryDate);
                            }
                        }
                    });
                }
                if (soViewModel != null && soViewModel.BaseInfoVM != null && soViewModel.BaseInfoVM.OrderTime.HasValue)
                {
                    tbOrderTime.Text = soViewModel.BaseInfoVM.OrderTime.Value.ToString(ResConverter.DateTime_LongFormat);
                }

                //虚拟团购订单隐藏所有按钮
                if (vm.BaseInfoVM.SOType == SOType.VirualGroupBuy)
                {
                    gridFunctionButton.Visibility = System.Windows.Visibility.Collapsed;
                }

                SetReportedButtonIsEnabled();
            });
        }

        private void OperateDisableCustomerSelect(bool isEnabled)
        {
            ucCustomer.IsEnabled = isEnabled;
            hlkb_RefreshCustomer.Visibility = isEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void IniPageData()
        {
            // 获取订单渠道信息 并初始化订单渠道列表
            CommonDataFacade.GetWebChannelList(false, (sender, e) =>
            {
                if (e.Result != null)
                {
                    e.Result.Insert(0, new WebChannelVM() { ChannelName = ResSO.SelectItem_DefaultChannel });
                    cbmSOWebChannel.ItemsSource = e.Result;
                }
            });

            //泰隆优选定制化 不存在多公司 所以注销了以下信息
            //// 获取订单所属公司信息 并初始化公司列表
            //CommonDataFacade.GetCompanyList(false, (sender, e) =>
            //{
            //    if (e.Result != null)
            //    {
            //        cmbCompany.ItemsSource = e.Result;
            //        if (cmbCompany.SelectedValue == null)
            //        {
            //            cmbCompany.SelectedIndex = 0;
            //        }
            //    }
            //});

            // 读取配置 初始化配送 时间范围,日期范围
            CodeNamePairHelper.GetList(ConstValue.DomainName_Common, new string[] { ConstValue.Key_TimeRange, ConstValue.Key_DeliveryDayRangeType }, CodeNamePairAppendItemType.None, (sender, e) =>
            {
                if (e.Result != null)
                {
                    cmbDeliveryRange.ItemsSource = e.Result[ConstValue.Key_TimeRange];
                    cmbDeliveryDayRange.ItemsSource = e.Result[ConstValue.Key_DeliveryDayRangeType];
                }
            });

            //读取配置 初始化客户类型
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_KFCType, CodeNamePairAppendItemType.None, (sender, e) =>
            {
                if (e.Result != null)
                {
                    cbmCustomerType.ItemsSource = e.Result;
                    if (cbmCustomerType.SelectedValue == null)
                    {
                        cbmCustomerType.SelectedIndex = 0;
                    }
                }
            });

            //获取所有仓库
            stockFacade.QueryStockAll((sender, e) =>
            {
                if (e.FaultsHandle())
                {
                    return;
                }
                List<StockVM> list = EntityConverter<StockInfo, StockVM>.Convert(e.Result, (source, target) =>
                {
                    target.CountryCode = source.WarehouseInfo.CountryCode;
                });
                this.StockList = list;
                list.Insert(0, new StockVM() { StockName = ResSOMaintain.Enum_Select });
                cmbStock.ItemsSource = list;
                cmbStock.SelectedIndex = 0;
            });

            // 读取枚举 初始化订单类型
            cmbSOType.ItemsSource = EnumConverter.GetKeyValuePairs<SOType>(EnumConverter.EnumAppendItemType.None)
                                    .Where(p => p.Key == SOType.General);
            cmbSOType.IsEnabled = true;

            ClearItem();
        }

        /// <summary>
        /// 加载页面 Grid数据 
        /// </summary>
        private void ReloadSOMaintainData()
        {
            if (IsNewCreateSO && !soViewModel.IsManualChangePrice)
            {
                stkManualChangePrice.Visibility = Visibility.Collapsed;
            }
            else
            {
                stkManualChangePrice.Visibility = Visibility.Visible;
            }

            //if (!string.IsNullOrEmpty(soViewModel.BaseInfoVM.Memo))
            //{
            //    spCustomerMemo.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    spCustomerMemo.Visibility = Visibility.Collapsed;
            //}

            OperateDisableCustomerSelect(IsNewCreateSO);

            if (string.IsNullOrEmpty(ucCustomer.CustomerID))
            {
                ucCustomer.CustomerID = soViewModel.BaseInfoVM.CustomerID;
            }
            //泰隆优选定制化 不存在多公司 所以注销了以下信息
            //if (string.IsNullOrEmpty(soViewModel.BaseInfoVM.CompanyCode))
            //{
            //    cmbCompany.SelectedIndex = 0;
            //    soViewModel.BaseInfoVM.CompanyCode = cmbCompany.SelectedValue.ToString();
            //}
            if (!soViewModel.BaseInfoVM.KFCStatus.HasValue)
            {
                cbmCustomerType.SelectedIndex = 0;
                soViewModel.BaseInfoVM.KFCStatus = Convert.ToInt32(cbmCustomerType.SelectedValue);
            }
            if (soViewModel.InvoiceInfoVM.IsVAT.HasValue && soViewModel.InvoiceInfoVM.IsVAT == true)
            {
              //  hlkb_SetVATInvoice.Visibility = Visibility.Visible;
            }
            else
            {
               // hlkb_SetVATInvoice.Visibility = Visibility.Collapsed;
            }
            //if (soViewModel.BaseInfoVM.IsUseGiftCard.HasValue && soViewModel.BaseInfoVM.IsUseGiftCard == true)
            //{
            //    chkGiftCardPay.IsChecked = true;
            //    hlkb_Car_T_GiftCardPay.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    chkGiftCardPay.IsChecked = false;
            //    hlkb_Car_T_GiftCardPay.Visibility = Visibility.Collapsed;
            //}
            //if (soViewModel.BaseInfoVM.IsVIP.HasValue && soViewModel.BaseInfoVM.IsVIP == true)
            //{
            //    VIPSOInfo.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    VIPSOInfo.Visibility = Visibility.Collapsed;
            //}
            
            if (!IsNewCreateSO)
            {
                string soSplitStatusDescript = string.Empty;
                switch (soViewModel.BaseInfoVM.SplitType)
                {
                    case SOSplitType.Customer:
                        soSplitStatusDescript = ResSOMaintain.Lable_CustomSplitSO;
                        break;
                    case SOSplitType.Force:
                        soSplitStatusDescript = ResSOMaintain.Lable_ForceSplitSO;
                        break;
                    case SOSplitType.SubSO:
                        soSplitStatusDescript = string.Format(ResSOMaintain.Lable_SplitSOSysNo, soViewModel.BaseInfoVM.SOSplitMaster);
                        OperationControlStatusHelper.SetControlsStatus(gridCarEventInfo, true);
                        OperationControlStatusHelper.SetControlsStatus(spGridCarOperate, true);
                        OperationControlStatusHelper.SetControlsStatus(gridCustomerInfo, true);
                        OperationControlStatusHelper.SetControlsStatus(tlkReceiveInfo, true);
                        OperationControlStatusHelper.SetControlsStatus(grdPayInfoAndShippingInfo, true);
                        OperationControlStatusHelper.SetControlsStatus(gridNote, true);
                        break;
                    default:
                        break;
                }
                if (soSplitStatusDescript.Length > 0)
                {
                    txtSOSplitStatus.Text = soSplitStatusDescript;
                    txtSOSplitStatus.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        #endregion

        #region 页面内部事件

        /// <summary>
        /// 用户选择控件 输入用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucCustomer_CustomerSelected(object sender, CustomerSelectedEventArgs e)
        {
            ReloadCustomer();
        }

        /// <summary>
        /// 刷新客户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_RefreshCustomer_Click(object sender, RoutedEventArgs e)
        {
            ReloadCustomer();
        }

        /// <summary>
        /// 跳转到配送公司网站
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_btnTrackingNumber_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(soViewModel.ShippingInfoVM.OfficialWebsite))
            {
                UtilityHelper.OpenWebPage(soViewModel.ShippingInfoVM.OfficialWebsite);
            }
        }

        /// <summary>
        /// 加载客户以及收货地址列表
        /// </summary>
        private void ReloadCustomer()
        {
            #region 加载客户相关信息
            if (ucCustomer.CustomerSysNo.HasValue)
            {
                queryFacade.QuerySOCustomerInfo(ucCustomer.CustomerSysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CustomerInfo customerInfo = args.Result as CustomerInfo;
                    if (customerInfo != null)
                    {
                        ucCustomer.CustomerID = customerInfo.BasicInfo.CustomerID;
                        soViewModel.BaseInfoVM.LanguageCode = customerInfo.BasicInfo.FavoriteLanguageCode;
                        soViewModel.BaseInfoVM.CustomerPoint = customerInfo.ValidScore;
                        soViewModel.BaseInfoVM.CustomerSysNo = customerInfo.BasicInfo.CustomerSysNo;
                        soViewModel.BaseInfoVM.CustomerName = customerInfo.BasicInfo.CustomerName;
                        soViewModel.BaseInfoVM.CustomerID = customerInfo.BasicInfo.CustomerID;
                        soViewModel.BaseInfoVM.CustomerChannel.ChannelID = customerInfo.WebChannel == null ? "0" : customerInfo.WebChannel.ChannelID;
                        soViewModel.InvoiceInfoVM.Header = customerInfo.BasicInfo.CustomerName;
                        soViewModel.ReceiverInfoVM.CustomerSysNo = customerInfo.BasicInfo.CustomerSysNo;
                        queryFacade.QueryKnownFraudCustomerInfo(ucCustomer.CustomerSysNo.Value, (kfcObj, kfcArgs) =>
                        {
                            if (!kfcArgs.FaultsHandle())
                            {
                                soViewModel.BaseInfoVM.KFCStatus = kfcArgs.Result.KFCType == null ? 0 : (int)kfcArgs.Result.KFCType;
                            }
                        });
                        List<SOReceiverInfoVM> SOReceiverInfoVMList = customerInfo.ShippingAddressList.Convert<ShippingAddressInfo, SOReceiverInfoVM>();
                        bool hasDefaultAddress = false;
                        if (SOReceiverInfoVMList != null && SOReceiverInfoVMList.Count > 0)
                        {
                            foreach (var item in SOReceiverInfoVMList)
                            {
                                if (item.IsDefault.HasValue && item.IsDefault.Value)
                                {
                                    hasDefaultAddress = true;
                                    soViewModel.ReceiverInfoVM = item;
                                    if (string.IsNullOrEmpty(soViewModel.InvoiceInfoVM.Header))
                                    {
                                        soViewModel.InvoiceInfoVM.Header = item.ReceiveName;
                                    }
                                }
                            }
                            if (!hasDefaultAddress)
                            {
                                soViewModel.ReceiverInfoVM = SOReceiverInfoVMList[0];
                            }
                        }
                        else
                        {
                            soViewModel.ReceiverInfoVM = new SOReceiverInfoVM();
                            hlbtnSelectAddress.Visibility = System.Windows.Visibility.Collapsed;
                        }

                        cmbSOType.IsEnabled = true;
                        IsNewCreateSO = true;
                        LoadAvailableButton();
                        ReloadSOMaintainData();
                        ucReceiveArea.DataContext = null; //地区选择控件需要优化：目前只能人为出发他的DataContext事件
                        ucReceiveArea.DataContext = soViewModel;
                       // cbVATInvoice.IsChecked = false;
                        chkIsManualChangePrice.IsChecked = false;
                        //chkGiftCardPay.IsChecked = false;
                        hlkb_Car_T_PriceCompensation.Visibility = Visibility.Collapsed;
                        //hlkb_Car_T_GiftCardPay.Visibility = Visibility.Collapsed;
                        //hlkb_SetVATInvoice.Visibility = Visibility.Collapsed;
                        dataGridItemInfo.ItemsSource = null;
                        dataGridTotalInfo.ItemsSource = null;
                        dataGridTotalInfo.Visibility = Visibility.Collapsed;
                        gridCarEventInfo.Visibility = Visibility.Collapsed;
                        //VIPSOInfo.Visibility = Visibility.Collapsed;
                        ClearItem();
                    }
                });
            }
            #endregion
        }

        /// <summary>
        /// 根据订单编号查询订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_Order_Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSOSysNo.Text))
            {
                try
                {
                    SOSysNo = Convert.ToInt32(txtSOSysNo.Text);
                }
                catch (Exception)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOSysNoInput_Error, MessageType.Error);
                    return;
                }
                LoadSOInfo();
            }
        }

        /// <summary>
        /// 订单类型 选择礼品卡订单 （电子卡 和实物卡）时，清空购物车，显示添加礼品卡HyperLinkButton 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSOType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsNewCreateSO)
            {
                if (UCShipType.SelectedShipType.HasValue
                    && UCShipType.SelectedShipType != -1
                    && UCShipType.SelectedShipType != 0)
                {
                    UCShipType.SelectedShipType = 0;
                    UCShipType.IsEnabled = true;
                }
            }
            if (((Combox)sender).SelectedValue != null
                && (((Combox)sender).SelectedValue.ToString() == SOType.ElectronicCard.ToString()
                     || ((Combox)sender).SelectedValue.ToString() == SOType.PhysicalCard.ToString())
                   )
            {
                if (((Combox)sender).SelectedValue.ToString() == SOType.ElectronicCard.ToString())
                {
                    UCShipType.SelectedShipType = 336;//电子礼品卡的默认配送方式 为 “电子邮件配送”
                    UCShipType.IsEnabled = false;
                    if (soViewModel.ItemsVM.Count > 0)
                    {
                        bool isElectronicCardSO = false;
                        foreach (var item in soViewModel.ItemsVM)
                        {
                            if (item.ProductSysNo == ElectronicCard_ProductSysNo || (item.ProductID.ToUpper() == ElectronicCard_ProductID))//电子卡:GC-001-001
                            {
                                isElectronicCardSO = true;
                            }
                        }
                        if (!isElectronicCardSO)
                        {
                            soViewModel.ItemsVM = new List<SOItemInfoVM>();
                            ReloadShoppingCar(false);
                        }
                    }
                }
                else if (((Combox)sender).SelectedValue.ToString() == SOType.PhysicalCard.ToString())
                {
                    //if (soViewModel.ItemsVM.Count > 0)
                    //{
                    //    bool isPhysicalCardSO = false;
                    //    foreach (var item in soViewModel.ItemsVM)
                    //    {
                    //        if (item.ProductID.ToUpper().Contains(PhysicalCard_ProductID_Prefix))//实物卡
                    //        {
                    //            isPhysicalCardSO = true;
                    //            break;
                    //        }
                    //    }
                    //    if (!isPhysicalCardSO)
                    //    {
                    //        soViewModel.ItemsVM = new List<SOItemInfoVM>();
                    //        ReloadShoppingCar(false);
                    //    }
                    //}
                }
               // hlkb_AddGiftCardInShoppingCar.Visibility = Visibility.Visible;
                hlkb_AddProductInShoppingCar.Visibility = Visibility.Collapsed;
                if (soViewModel.InvoiceInfoVM != null && soViewModel.InvoiceInfoVM.VATInvoiceInfoVM != null)
                {
                    soViewModel.InvoiceInfoVM.VATInvoiceInfoVM = new SOVATInvoiceInfoVM();
                }
               // cbVATInvoice.IsChecked = false;
               // cbVATInvoice.IsEnabled = false;
               // hlkb_SetVATInvoice.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (soViewModel.ItemsVM.Count > 0)
                {
                    bool isPhysicalCardORElectronicCardSO = false;
                    foreach (var item in soViewModel.ItemsVM)
                    {
                        if ((item.ProductType != SOProductType.Coupon)
                            && (item.ProductID.ToUpper() == ElectronicCard_ProductID || item.ProductID.ToUpper().Contains(PhysicalCard_ProductID_Prefix)))
                        {
                            isPhysicalCardORElectronicCardSO = true;
                            break;
                        }
                    }
                    if (isPhysicalCardORElectronicCardSO == true)
                    {
                        soViewModel.ItemsVM = new List<SOItemInfoVM>();
                        ReloadShoppingCar(false);
                    }
                }
                //hlkb_AddGiftCardInShoppingCar.Visibility = Visibility.Collapsed;
                //hlkb_AddProductInShoppingCar.Visibility = Visibility.Visible;
                //hlkb_AddProductInShoppingCar.IsEnabled = true;

                //cbVATInvoice.IsEnabled = true;
                //cbVATInvoice.Visibility = Visibility.Visible;

                //if (cbVATInvoice.IsChecked == true)
                //{
                //  //  hlkb_SetVATInvoice.Visibility = Visibility.Visible;
                //}
            }
        }

        /// <summary>
        /// 设置增值税发票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_SetVATInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (soViewModel.BaseInfoVM.CustomerSysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SetVATInvoiceError);
            }
            else
            {
                soViewModel.InvoiceInfoVM.VATInvoiceInfoVM.CustomerSysNo = soViewModel.BaseInfoVM.CustomerSysNo.Value;
                VATInvoice vat = new VATInvoice(soViewModel.InvoiceInfoVM.VATInvoiceInfoVM);
                var window = CPApplication.Current.CurrentPage.Context.Window;
                IDialog dialog = window.ShowDialog(ResSOMaintain.Title_ValueAddedTaxInfo, vat, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        soViewModel.InvoiceInfoVM.VATInvoiceInfoVM = args.Data as SOVATInvoiceInfoVM;
                    }
                    else
                    {
                        if (!IsVATSO && string.IsNullOrEmpty(soViewModel.InvoiceInfoVM.VATInvoiceInfoVM.CompanyName))//原订单 非增值税发票  有没有选择  则增值税发票选择框
                        {
                            soViewModel.InvoiceInfoVM.IsVAT = false;
                           // hlkb_SetVATInvoice.Visibility = Visibility.Collapsed;
                            soViewModel.InvoiceInfoVM.VATInvoiceInfoVM = new SOVATInvoiceInfoVM();
                            soViewModel.InvoiceInfoVM.VATInvoiceInfoVM.CustomerSysNo = soViewModel.BaseInfoVM.CustomerSysNo.Value;
                        }
                    }
                    ReloadSOMaintainData();
                });
                vat.Dialog = dialog;
            }
        }

        /// <summary>
        /// 开启/关闭 设置增值税发票功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbVATInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked.Value)// 设置增值税发票功能
            {
                if (soViewModel.BaseInfoVM.CustomerSysNo == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SetVATInvoiceError);
                    ((CheckBox)sender).IsChecked = false;
                }
                else
                {
                   // hlkb_SetVATInvoice.Visibility = Visibility.Visible;
                }
            }
            else
            {
               // hlkb_SetVATInvoice.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 设置普票改增票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_SetOInvoiceTOVATInvoice_Click(object sender, RoutedEventArgs e)
        {
            ((HyperlinkButton)sender).Visibility = Visibility.Collapsed;
            ((HyperlinkButton)sender).IsEnabled = false;
           // cbVATInvoice.IsChecked = true;
            //hlkb_SetVATInvoice.Visibility = Visibility.Visible;
            //hlkb_SaveOInvoiceTOVATInvoice.Visibility = Visibility.Visible;
            //hlkb_SetVATInvoice.IsEnabled = true;
            //hlkb_SaveOInvoiceTOVATInvoice.IsEnabled = true;
        }

        /// <summary>
        /// 保存 普票改增票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_SaveOInvoiceTOVATInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (soViewModel.InvoiceInfoVM != null
                && soViewModel.InvoiceInfoVM.VATInvoiceInfoVM != null
                && !string.IsNullOrEmpty(soViewModel.InvoiceInfoVM.VATInvoiceInfoVM.TaxNumber))
            {
                soViewModel.InvoiceInfoVM.VATInvoiceInfoVM.SOSysNo = soViewModel.BaseInfoVM.SysNo;
                soFacade.SetSOVATInvoiveWhenSOOutStock(soViewModel, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SetSOVATInvoiveWhenSOOutStock_Sucess, MessageType.Information);
                        //hlkb_SaveOInvoiceTOVATInvoice.Visibility = Visibility.Collapsed;
                        //hlkb_SaveOInvoiceTOVATInvoice.IsEnabled = false;
                        return;
                    }
                });
            }
        }

        /// <summary>
        /// 选择收货地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtnSelectAddress_Click(object sender, RoutedEventArgs e)
        {
            if (soViewModel.BaseInfoVM.CustomerSysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SetVATInvoiceError, MessageType.Error);
            }
            else
            {
                SOReceiveInfoDetail receiveIfno = new SOReceiveInfoDetail(soViewModel.ReceiverInfoVM);
                var window = CPApplication.Current.CurrentPage.Context.Window;
                IDialog dialog = window.ShowDialog(ResSOMaintain.Title_SOReceiveInfoDetail, receiveIfno, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        SOReceiverInfoVM receiverInfoVM = args.Data as SOReceiverInfoVM;
                        soViewModel.ReceiverInfoVM = receiverInfoVM;
                        ReloadSOMaintainData();
                        ucReceiveArea.DataContext = null;//地区选择控件需要优化：目前只能人为出发他的DataContext事件
                        ucReceiveArea.DataContext = soViewModel;
                    }
                });
                receiveIfno.Dialog = dialog;
            }
        }

        /// <summary>
        /// 选择支付方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (soViewModel != null)
            {
                if (((UCPayTypePicker)sender).SelectedPayType.HasValue)
                {
                    queryFacade.IsPayWhenReceived(((UCPayTypePicker)sender).SelectedPayType.Value, string.IsNullOrEmpty(soViewModel.BaseInfoVM.CompanyCode) ? "companyCode" : soViewModel.BaseInfoVM.CompanyCode, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        soViewModel.BaseInfoVM.PayWhenReceived = (Boolean)args.Result;
                    });
                    
                    if (cmbSOType.SelectedValue != null
             && (cmbSOType.SelectedValue.ToString() == SOType.ElectronicCard.ToString()
                  || cmbSOType.SelectedValue.ToString() == SOType.PhysicalCard.ToString())
                )
                    {
                        //cbVATInvoice.IsChecked = false;
                        //cbVATInvoice.IsEnabled = false;
                        //hlkb_SetVATInvoice.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        //cbVATInvoice.IsEnabled = true;
                        //cbVATInvoice.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        //泰隆优选定制化 不存在多公司 所以注销以下代码
        ///// <summary>
        ///// 选择公司
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void cmbCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (soViewModel != null)
        //    {
        //        if (soViewModel.BaseInfoVM.PayTypeSysNo.HasValue)
        //        {
        //            queryFacade.IsPayWhenReceived(soViewModel.BaseInfoVM.PayTypeSysNo.Value, ((CompanyVM)((ComboBox)sender).SelectedValue).CompanyCode, (obj, args) =>
        //            {
        //                if (args.FaultsHandle())
        //                {
        //                    return;
        //                }
        //                soViewModel.BaseInfoVM.PayWhenReceived = (Boolean)args.Result;
        //            });
        //        }
        //    }
        //}

        /// <summary>
        /// 显示BackOrder订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_ShowBackOrder_Click(object sender, RoutedEventArgs e)
        {
            SOBackOrder ctrl = new SOBackOrder(soViewModel.SysNo.Value);
            ctrl.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(
                        ResSOMaintain.UC_Title_BackOrder
                        , ctrl
                        , null
                );
        }

        /// <summary>
        /// 配送方式选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCShipType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //实际触发的为一个下拉框控件
            ComboBox comboBoxSender = sender as ComboBox;

            //初始化选择控件
            dateDelivery.IsEnabled = false;
            dateDelivery.Visibility = System.Windows.Visibility.Visible;
            cmbDeliveryRange.IsEnabled = false;
            cmbDeliveryRange.Visibility = System.Windows.Visibility.Visible;
            cmbDeliveryDayRange.Visibility = Visibility.Collapsed;
            //dateDelivery.SelectedDate = DateTime.Now;
            //cmbDeliveryRange.SelectedIndex = 0;
            //cmbDeliveryDayRange.SelectedIndex = 0;

            if (comboBoxSender != null)
            {
                var data = comboBoxSender.SelectedItem as ShippingType;
                if (data != null)
                {
                    if (data.SysNo.HasValue && data.DeliveryType.HasValue)
                    {
                        //判断送货时间区域的控件选择
                        switch (data.DeliveryType.Value)
                        {
                            //可修改日期，不可修改时间段
                            case ShipDeliveryType.OneDayOnce:
                                dateDelivery.IsEnabled = true;
                                cmbDeliveryRange.Visibility = System.Windows.Visibility.Collapsed;
                                break;
                            //可修改日期和时间段
                            case ShipDeliveryType.OneDayTwice:
                                dateDelivery.IsEnabled = true;
                                cmbDeliveryRange.IsEnabled = true;
                                break;
                            //送货时间控件须切换成为下拉选择控件共有三个选项：工作日配送、双休日配送、工作日和双休日均可配送，可在三个选项间切换，不可修改时间段
                            case ShipDeliveryType.EveryDay:
                                dateDelivery.Visibility = Visibility.Collapsed;
                                cmbDeliveryRange.Visibility = System.Windows.Visibility.Collapsed;
                                cmbDeliveryDayRange.Visibility = Visibility.Visible;
                                break;
                            //case ShipDeliveryType.OneDaySix:
                            //    isDisableDataControl = false;
                            //    break;
                            default:
                                break;
                        }
                    }
                    cmbStock.SelectedValue = data.OnlyForStockSysNo;
                }
            }
        }

        private void chkIsManualChangePrice_Checked(object sender, RoutedEventArgs e)
        {
            hlkb_Car_T_PriceCompensation.Visibility = Visibility.Visible;
        }

        private void chkIsManualChangePrice_Unchecked(object sender, RoutedEventArgs e)
        {
            hlkb_Car_T_PriceCompensation.Visibility = Visibility.Collapsed;
            soViewModel.ItemsVM.ForEach(p =>
            {
                p.OriginalPrice = p.Price = (p.OriginalPrice ?? 0) + (p.AdjustPrice ?? 0);
                p.AdjustPrice = 0;
                p.AdjustPriceReason = null;
            });
            ReloadShoppingCar(true);
        }

        #region VIP订单 选择后相关事件

        //private void cbIsVIP_Click(object sender, RoutedEventArgs e)
        //{
        //    if (((CheckBox)sender).IsChecked.HasValue && ((CheckBox)sender).IsChecked == true)
        //    {
        //        VIPSOInfo.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        VIPSOInfo.Visibility = Visibility.Collapsed;
        //    }
        //}

        private void Rdo_VIP_Enterprise_SO_Checked(object sender, RoutedEventArgs e)
        {
            soViewModel.BaseInfoVM.Is_Enterprise_SO = true;
            soViewModel.BaseInfoVM.VIPSOType = "E";
            soViewModel.BaseInfoVM.Is_Channel_SO = false;
            soViewModel.BaseInfoVM.Is_Person_SO = false;
            ((RadioButton)sender).IsChecked = true;
        }

        private void Rdo_VIP_Person_SO_Checked(object sender, RoutedEventArgs e)
        {
            soViewModel.BaseInfoVM.Is_Person_SO = true;
            soViewModel.BaseInfoVM.VIPSOType = "P";
            soViewModel.BaseInfoVM.Is_Enterprise_SO = false;
            soViewModel.BaseInfoVM.Is_Channel_SO = false;
            ((RadioButton)sender).IsChecked = true;
        }

        private void Rdo_VIP_Channel_SO_Checked(object sender, RoutedEventArgs e)
        {
            soViewModel.BaseInfoVM.Is_Channel_SO = true;
            soViewModel.BaseInfoVM.VIPSOType = "C";
            soViewModel.BaseInfoVM.Is_Enterprise_SO = false;
            soViewModel.BaseInfoVM.Is_Person_SO = false;
            ((RadioButton)sender).IsChecked = true;
        }

        private void Rdo_VIP_New_Customer_Checked(object sender, RoutedEventArgs e)
        {
            soViewModel.BaseInfoVM.Is_New_Customer = true;
            soViewModel.BaseInfoVM.VIPUserType = "N";
            soViewModel.BaseInfoVM.Is_Old_Customer = false;
            ((RadioButton)sender).IsChecked = true;
        }

        private void Rdo_VIP_Old_Customer_Checked(object sender, RoutedEventArgs e)
        {
            soViewModel.BaseInfoVM.Is_Old_Customer = true;
            soViewModel.BaseInfoVM.VIPUserType = "O";
            soViewModel.BaseInfoVM.Is_New_Customer = false;
            ((RadioButton)sender).IsChecked = true;
        }

        #endregion

        #endregion

        #region 购物车 相关逻辑

        /// <summary>
        /// 添加礼品卡到购物车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_AddGiftCardInShoppingCar_Click(object sender, RoutedEventArgs e)
        {
            if (soViewModel.BaseInfoVM.CustomerSysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SetVATInvoiceError, MessageType.Error);
            }
            else
            {
                // 添加实物卡
                if (cmbSOType.SelectedValue.ToString() == SOType.PhysicalCard.ToString())
                {
                    SOPhysicalCard physicalCard = new SOPhysicalCard(this, soViewModel.BaseInfoVM.CompanyCode);
                    var window = CPApplication.Current.CurrentPage.Context.Window;
                    IDialog dialog = window.ShowDialog(ResSOMaintain.Title_AddSOPhysicalCard, physicalCard, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            List<SOItemInfoVM> itemList = args.Data as List<SOItemInfoVM>;
                            if (itemList != null && itemList.Count > 0)
                            {
                                foreach (var item in itemList)
                                {
                                    var itemTemp = soViewModel.ItemsVM.FirstOrDefault(x => { return x.ProductID == item.ProductID; });
                                    if (itemTemp != null)
                                    {
                                        itemTemp = item;
                                    }
                                    else
                                    {
                                        soViewModel.ItemsVM.Add(item);
                                    }
                                }
                                ReloadGiftCardInShoppingCar();
                            }
                        }
                    });
                    physicalCard.Dialog = dialog;
                }//添加电子卡
                else if (cmbSOType.SelectedValue.ToString() == SOType.ElectronicCard.ToString())
                {
                    SOElectronicCard electronicCard = new SOElectronicCard();
                    var window = CPApplication.Current.CurrentPage.Context.Window;
                    IDialog dialog = window.ShowDialog(ResSOMaintain.Title_AddSOElectronicCard, electronicCard, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            List<SOItemInfoVM> itemList = args.Data as List<SOItemInfoVM>;
                            if (itemList != null && itemList.Count > 0)
                            {
                                foreach (var item in itemList)
                                {
                                    List<SOItemInfoVM> itemListTemp = soViewModel.ItemsVM.Where(x => { return x.ProductID == item.ProductID; }).ToList();
                                    if (itemListTemp != null && itemListTemp.Count > 0)
                                    {
                                        itemListTemp[0].Quantity += item.Quantity.Value;
                                    }
                                    else
                                    {
                                        soViewModel.ItemsVM.Add(item);
                                    }
                                }
                                ReloadGiftCardInShoppingCar();
                            }
                        }
                    });
                    electronicCard.Dialog = dialog;
                }
            }
        }

        /// <summary>
        /// 添加礼品卡后 设置相应控件为只读或者可用
        /// </summary>
        private void ReloadGiftCardInShoppingCar()
        {
            ReloadShoppingCar(false);
            UtilityHelper.ReadOnlyControl(gridCarInfo, gridCarInfo.Children.Count, true);
           // hlkb_AddGiftCardInShoppingCar.IsEnabled = true;
            hlkb_btnCalculateInShoppingCar.IsEnabled = true;
            chk_Premium.IsEnabled = true;
            chkAccountPrepayAmount.IsEnabled = true;
            ((System.Windows.Controls.DataGrid)dataGridItemInfo).IsEnabled = true;
        }

        /// <summary>
        /// 添加商品到购物车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_AddProductInShoppingCar_Click(object sender, RoutedEventArgs e)
        {
            if (soViewModel.BaseInfoVM.CustomerSysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SetVATInvoiceError, MessageType.Error);
            }
            else
            {
                #region 商品选择控件

                UCProductSearchForSO ucProductSearch = new UCProductSearchForSO();
                var window = CPApplication.Current.CurrentPage.Context.Window;
                IDialog dialog = window.ShowDialog(ResSOMaintain.Title_SOShoppingCarSelectItem, ucProductSearch, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        List<ProductVM> IMProductVM = new List<ProductVM>();
                        IMProductVM = args.Data as List<ProductVM>;
                        if (IMProductVM.Count > 0)
                        {
                            //判断当前添加的商品中是否存在 及是主商品又是赠品的 商品  （此类商品不允许添加）
                            var resultInfo = from a in IMProductVM
                                             join b in IMProductVM
                                             on a.ProductID equals b.ProductID
                                             where a.SOProductType != b.SOProductType
                                             select a.ProductID;
                            if (resultInfo.Count() > 0)
                            {
                                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOMaintain_AddProductError_DoubleProduct, MessageType.Error);
                                return;
                            }

                            //判断当前添加的商品是否已经存在于购物车中且和购物车中的商品类型不一致（此类商品不允许添加）
                            foreach (ProductVM item in IMProductVM)
                            {
                                List<SOItemInfoVM> itemListTemp = soViewModel.ItemsVM.Where(x => { return x.ProductID == item.ProductID && x.ProductType != item.SOProductType; }).ToList();
                                if (itemListTemp != null && itemListTemp.Count > 0)
                                {
                                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOMaintain_AddProductError_DoubleProduct, MessageType.Error);
                                    return;
                                }
                            }
                            foreach (ProductVM item in IMProductVM)
                            {
                                List<SOItemInfoVM> itemListTemp = soViewModel.ItemsVM.Where(x => { return x.ProductID == item.ProductID; }).ToList();
                                if (itemListTemp != null && itemListTemp.Count > 0)
                                {
                                    itemListTemp[0].Quantity += item.Quantity.Value;
                                }
                                else
                                {
                                    SOItemInfoVM itemInfo = new SOItemInfoVM();
                                    itemInfo.CostPrice = item.AvgCost;
                                    itemInfo.PromotionAmount = item.DiscountAmount;//商品折扣
                                    itemInfo.GainAveragePoint = item.Point;//商品赠送的积分
                                    itemInfo.Price = item.CurrentPrice;
                                    itemInfo.ProductName = item.ProductName;
                                    itemInfo.ProductSysNo = item.SysNo;
                                    itemInfo.MasterProductSysNo = (item.MasterSysNos != null && item.MasterSysNos.Count > 0) ? (item.MasterSysNos.Join(",") == null ? "" : item.MasterSysNos.Join(",")) : "";
                                    itemInfo.RuleQty = item.RuleQty;//主商品 对应的 赠送赠品基数 （如 基数为2  则表示一个主商品 赠送2个赠品 一次类推）
                                    itemInfo.ProductID = item.ProductID;
                                    itemInfo.ProductType = item.SOProductType;
                                    if (item.SOProductType == SOProductType.ExtendWarranty)
                                    {
                                        if (string.IsNullOrEmpty(itemInfo.MasterProductSysNo))
                                        {
                                            itemInfo.MasterProductSysNo = item.MasterSysNo.ToString();
                                        }
                                    }
                                    itemInfo.Quantity = item.Quantity;
                                    itemInfo.OnlineQty = item.OnlineQty;
                                    itemInfo.Weight = item.Weight;//重量
                                    itemInfo.GrossProfit = item.SalesMargin;
                                    itemInfo.GrossProfitRate = item.SalesMarginRate;
                                    itemInfo.Warranty = string.IsNullOrEmpty(item.Warranty) ? ResSOMaintain.Info_SOMaintain_Warranty_HaveNot : item.Warranty;
                                    itemInfo.InventoryType = item.InventoryType;
                                    //--------------------------------------------------------------------
                                    //最优分仓编号,名称, 取Ipp3.dbo.Stock WarehouseRate字段 降序
                                    if (!string.IsNullOrWhiteSpace(item.OptimalizingStock))
                                    {
                                        StockVM stock = JsonHelper.JsonDeserialize<StockVM>(item.OptimalizingStock);
                                        itemInfo.StockName = stock.StockName;
                                        itemInfo.StockSysNo = stock.SysNo;
                                        itemInfo.SysNo = stock.SysNo;
                                    }
                                    itemInfo.StoreType = item.StoreType;
                                    itemInfo.TariffRate = item.TariffRate;
                                    soViewModel.ItemsVM.Add(itemInfo);
                                }

                                #region 组织Vendor赠品Promotion信息，补偿方案，此信息最好由MKT给出

                                //找赠品
                                if (item.SOProductType == SOProductType.Gift)
                                {
                                    //原来存在就不加
                                    if (soViewModel.PromotionsVM == null
                                        || soViewModel.PromotionsVM.FirstOrDefault(x => x.PromotionSysNo == item.PromotionSysNo) == null)
                                    {
                                        //找主商品
                                        var master = IMProductVM.FirstOrDefault(x => x.SysNo == item.MasterSysNos[0]);
                                        if (master != null)
                                        {
                                            //组织Promotion
                                            SOPromotionInfoVM promotion = new SOPromotionInfoVM();

                                            promotion.PromotionType = SOPromotionType.VendorGift;

                                            promotion.PromotionSysNo = item.PromotionSysNo;

                                            promotion.GiftListVM.Add(new GiftInfoVM { SysNo = item.SysNo.Value, QtyPreTime = item.RuleQty.Value });

                                            promotion.MasterListVM.Add(new MasterInfoVM { ProductSysNo = master.SysNo.Value, QtyPreTime = 1 });

                                            soViewModel.PromotionsVM.Add(promotion);
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        CalcAddProductItemAndLoadShipCarItems();
                    }
                });
                ucProductSearch.DialogHandler = dialog;

                #endregion
            }
        }

        /// <summary>
        /// 计算购物车中商品价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_CalculateInShoppingCar_Click(object sender, RoutedEventArgs e)
        {
            if (soViewModel.ItemsVM.Count == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_PleaseSelectedItemFirst, MessageType.Error);
            }
            else
            {
                soFacade.CalculateSO(soViewModel, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    soViewModel = SOFacade.ConvertTOSOVMFromSOInfo(args.Result);
                    ReloadShoppingCar(false);
                });
            }
        }

        /// <summary>
        /// 产品信息中的产品名称Link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_Car_ProductName_Click(object sender, RoutedEventArgs e)
        {
            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
            string url = string.Format(urlFormat, ((HyperlinkButton)sender).Tag.ToString());
            Window.Navigate(url);
        }

        /// <summary>
        /// 购物车中修改商品数量  数量增加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_Car_OrdeQuantityAddsubtract_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string ProductID = ((Image)sender).Tag.ToString();
            foreach (var item in soViewModel.ItemsVM)
            {
                if (item.ProductID == ProductID)
                {
                    item.Quantity += 1;
                    break;
                }
            }
            ProductQtyChange();
        }

        /// <summary>
        /// 购物车中修改商品数量  数量减少
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_Car_OrdeQuantitySubtract_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string ProductID = ((Image)sender).Tag.ToString();
            foreach (var item in soViewModel.ItemsVM)
            {
                if (item.ProductID == ProductID)
                {
                    if (item.Quantity == 1)
                    {
                        return;
                    }
                    item.Quantity -= 1;
                    break;
                }
            }
            ProductQtyChange();
        }

        private void txtInputOrderQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            int qty = int.TryParse(((TextBox)sender).Text, out qty) ? qty : 1;
            string ProductID = ((TextBox)sender).Tag.ToString();
            foreach (var item in soViewModel.ItemsVM)
            {
                if (item.ProductID == ProductID)
                {
                    if (qty < 1)
                    {
                        qty = 1;
                    }
                    item.Quantity = qty;
                    break;
                }
            }
            ProductQtyChange();
        }

        /// <summary>
        /// 购物车中的删除商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_Car_DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm(ResSOMaintain.Info_Confirm_DeleteItem, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    string ProductID = ((HyperlinkButton)sender).CommandParameter.ToString();
                    List<SOItemInfoVM> soItemInfoVMList = soViewModel.ItemsVM;
                    int? deletedProductSysNo = null;

                    for (int i = soItemInfoVMList.Count - 1; i >= 0; i--)
                    {
                        if (soItemInfoVMList[i].ProductID == ProductID)
                        {
                            soViewModel.BaseInfoVM.IsPremium = null;//取消保价
                            soViewModel.BaseInfoVM.PremiumAmount = null;//清空保价费
                            soViewModel.BaseInfoVM.ShipPrice = null;//清空之前计算出来的运费
                            soViewModel.BaseInfoVM.ManualShipPrice = null;//清空之前手动设置的运费
                            if (soViewModel.BaseInfoVM.PromotionAmount.HasValue)
                            {
                                soViewModel.BaseInfoVM.PromotionAmount -= soItemInfoVMList[i].PromotionAmount;
                            }
                            deletedProductSysNo = soItemInfoVMList[i].ProductSysNo;

                            soItemInfoVMList.RemoveAt(i);
                        }
                    }

                    if (deletedProductSysNo.HasValue)
                    {
                        for (int i = soItemInfoVMList.Count - 1; i >= 0; i--)
                        {
                            if (!string.IsNullOrEmpty(soItemInfoVMList[i].MasterProductSysNo)
                                && soItemInfoVMList[i].MasterProductSysNo.Contains(deletedProductSysNo.ToString())
                                && soItemInfoVMList[i].ProductType != SOProductType.Gift
                                )
                            {
                                soItemInfoVMList.RemoveAt(i);
                            }
                        }
                    }

                    //如果没有主商品，删除全部
                    if (soItemInfoVMList.Count(p => p.ProductType == SOProductType.Product) == 0)
                    {
                        ClearItem();
                    }

                    ReloadShoppingCar(true);
                }
            });
        }

        private void ClearItem()
        {
            if (soViewModel != null)
            {
                soViewModel.ItemsVM.Clear();
                soViewModel.PromotionsVM.Clear();
                soViewModel.CouponCode = "";
            }
        }

        /// <summary>
        /// 重新加载购物车信息
        /// </summary>
        private void ProductQtyChange()
        {
                string tempCouponCode = soViewModel.CouponCode;
                //调用计算服务 重新加载购物车相关金额 并加载购物车信息
                soFacade.ProductQtyChange(soViewModel, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    soViewModel = SOFacade.ConvertTOSOVMFromSOInfo(args.Result);
                    if (string.IsNullOrEmpty(soViewModel.CouponCode) && !string.IsNullOrEmpty(tempCouponCode))
                    {
                        soViewModel.CouponCode = tempCouponCode;
                    }
                    ReloadShoppingCarDeep();
                });
        }

        /// <summary>
        /// 重新加载购物车信息
        /// </summary>
        private void ReloadShoppingCar(bool IsCalculateSO)
        {
            if (IsCalculateSO)
            {
                string tempCouponCode = soViewModel.CouponCode;
                //调用计算服务 重新加载购物车相关金额 并加载购物车信息
                soFacade.CalculateSO(soViewModel, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    soViewModel = SOFacade.ConvertTOSOVMFromSOInfo(args.Result);
                    if (string.IsNullOrEmpty(soViewModel.CouponCode) && !string.IsNullOrEmpty(tempCouponCode))
                    {
                        soViewModel.CouponCode = tempCouponCode;
                    }
                    ReloadShoppingCarDeep();
                });
            }
            else
            {
                //加载购物车信息
                ReloadShoppingCarDeep();
            }
        }

        /// <summary>
        /// 加载购物车信息
        /// </summary>
        private void ReloadShoppingCarDeep()
        {
            if (soViewModel.ItemsVM.Count == 0)
            {
                soViewModel.BaseInfoVM.PointPay = null;//清空应用的积分
                soViewModel.BaseInfoVM.GainPoint = 0; //清空商品赠送的积分
                soViewModel.BaseInfoVM.PayPrice = null;//清空手续费
                soViewModel.BaseInfoVM.CouponAmount = null;
                soViewModel.BaseInfoVM.IsUsePrePay = null;//取消账户余额支付
                soViewModel.BaseInfoVM.PrepayAmount = null;//请空客户账户余额支付
                soViewModel.BaseInfoVM.PointPayAmount = null; //清空应用积分对应 金额
                soViewModel.BaseInfoVM.IsUseGiftCard = null;//取消使用礼品卡                
                soViewModel.GiftCardListVM = new List<GiftCardInfoVM>();//清空礼品卡信息               
                soViewModel.BaseInfoVM.GiftCardPay = null;//清空使用礼品卡的金额
                soViewModel.BaseInfoVM.PromotionAmount = null;//清空订单折扣    
                soViewModel.BaseInfoVM.TariffAmount = null;//清空关税
                dataGridTotalInfo.Visibility = Visibility.Collapsed;
                gridCarEventInfo.Visibility = Visibility.Collapsed;
                //hlkb_Car_T_GiftCardPay.Visibility = Visibility.Collapsed;
            }
            else
            {
                dataGridTotalInfo.Visibility = Visibility.Visible;
                gridCarEventInfo.Visibility = Visibility.Visible;
                //if (soViewModel.BaseInfoVM.IsUseGiftCard.HasValue && soViewModel.BaseInfoVM.IsUseGiftCard == true)
                //{
                //    hlkb_Car_T_GiftCardPay.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    hlkb_Car_T_GiftCardPay.Visibility = Visibility.Collapsed;
                //}
            }

            //LoadControl
            dataGridItemInfo.ItemsSource = soViewModel.ItemsVM;     //加载商品信息
            dataGridTotalInfo.ItemsSource = new List<SOBaseInfoVM>() { soViewModel.BaseInfoVM }; //加载总计信息        
            gridCarEventInfo.DataContext = soViewModel;

            ShowComboPromotions();
        }

        private void ShowComboPromotions()
        {
            if (soViewModel.ComboPromotionsVM.Count > 0)
            {
                Expander_SOSaleRule.Visibility = Visibility.Visible;
                DataGridComboPromotions.ItemsSource = soViewModel.ComboPromotionsVM;
            }
            else
            {
                Expander_SOSaleRule.Visibility = Visibility.Collapsed;
                DataGridComboPromotions.ItemsSource = null;
            }
        }

        /// <summary>
        /// 价格补偿
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_Car_T_PriceCompensation_Click(object sender, RoutedEventArgs e)
        {
            SOPriceCompensation priceCompensation = new SOPriceCompensation(soViewModel.ItemsVM);
            var window = CPApplication.Current.CurrentPage.Context.Window;
            IDialog dialog = window.ShowDialog(ResSOMaintain.Title_PriceCompensation, priceCompensation, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    soViewModel.ItemsVM = args.Data as List<SOItemInfoVM>;
                    ReloadShoppingCar(true);
                }
                if (args.DialogResult == DialogResultType.Cancel)
                {
                    if (soViewModel.ItemsVM != null && soViewModel.ItemsVM.Count >= 0)
                    {
                        bool viewLink = false;
                        foreach (var item in soViewModel.ItemsVM)
                        {
                            if (item.AdjustPrice.HasValue && item.AdjustPrice != 0)
                            {
                                viewLink = true;
                            }
                        }
                        if (!viewLink)
                        {
                            chkIsManualChangePrice.IsChecked = false;
                            hlkb_Car_T_PriceCompensation.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            });
            priceCompensation.Dialog = dialog;
        }

        /// <summary>
        /// 应用积分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_Car_T_Apply_PointPay_Click(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(txtPointPayInput.Text))
            //{
            //    int PointPayInput = int.TryParse(txtPointPayInput.Text, out PointPayInput) ? PointPayInput : 0;
            //    if (PointPayInput < 0)
            //    {
            //        txtPointPayInput.Text = string.Empty;
            //        CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOPointPayInput_Error_Must_Than_Zero, MessageType.Error);
            //        return;
            //    }
            //    //else if (PointPayInput > soViewModel.BaseInfoVM.CustomerPoint)
            //    //{
            //    //    txtPointPayInput.Text = string.Empty;
            //    //    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOPointPayInput_Error_No_Than_CustomerPoint, MessageType.Error);
            //    //    return;
            //    //}
            //    soViewModel.BaseInfoVM.PointPay = (int)PointPayInput;
            //    soViewModel.BaseInfoVM.PointPayAmount = PointPayInput / 10.00M;
            //    ReloadShoppingCar(true);
            //}
            //else
            //{
            //    soViewModel.BaseInfoVM.PointPay = 0;
            //    soViewModel.BaseInfoVM.PointPayAmount = 0.00M;
            //    ReloadShoppingCar(true);
            //}
        }

        /// <summary>
        /// 应用优惠券
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_Car_T_Apply_CouponAmount_Click(object sender, RoutedEventArgs e)
        {
            soViewModel.CouponCode = txtPromotionDiscount.Text;
            if (string.IsNullOrEmpty(soViewModel.CouponCode))
            {
                soViewModel.BaseInfoVM.CouponAmount = 0.00M;
                int OldCout = soViewModel.ItemsVM.Count;
                for (int i = OldCout - 1; i >= 0; i--)
                {
                    if (soViewModel.ItemsVM[i].ProductType == SOProductType.Coupon)
                    {
                        soViewModel.ItemsVM.RemoveAt(i);
                    }
                }
                if (OldCout != soViewModel.ItemsVM.Count)
                {
                    foreach (var item in soViewModel.ItemsVM)
                    {
                        item.CouponAverageDiscount = 0.00M;
                    }
                    ReloadShoppingCar(true);
                }
            }
            else
            {
                for (int i = soViewModel.ItemsVM.Count - 1; i >= 0; i--)
                {
                    if (soViewModel.ItemsVM[i].ProductType == SOProductType.Coupon)
                    {
                        soViewModel.ItemsVM.RemoveAt(i);
                    }
                }
                #region 应用优惠券 （调用计算服务 重新加载购物车相关金额 并加载购物车信息 ）
                soFacade.CalculateSO(soViewModel, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    soViewModel = SOFacade.ConvertTOSOVMFromSOInfo(args.Result);
                    if (soViewModel.BaseInfoVM.CouponAmount.HasValue && soViewModel.BaseInfoVM.CouponAmount > 0)
                    {
                        List<SOPromotionInfoVM> couponInfoList = soViewModel.PromotionsVM.Where(item => item.PromotionType == SOPromotionType.Coupon).ToList();
                        if (couponInfoList != null && couponInfoList.Count > 0)
                        {
                            SOPromotionInfoVM item = couponInfoList[0];   //现在只支持单张订单使用单张优惠券                
                            if (item != null && item.PromotionSysNo > 0)
                            {
                                queryFacade.QueryMKTCouponsInfoByCouponCodeSysNo(item.PromotionSysNo.Value, (objMKT, argsMKT) =>
                                {
                                    if (argsMKT.FaultsHandle())
                                    {
                                        return;
                                    }
                                    CouponsInfo couponInfo = argsMKT.Result;
                                    if (couponInfo != null)
                                    {
                                        SOItemInfoVM itemInfo = new SOItemInfoVM();
                                        itemInfo.ProductID = item.PromotionSysNo.ToString();
                                        itemInfo.ProductSysNo = item.PromotionSysNo;
                                        itemInfo.OriginalPrice = -item.DiscountAmount;
                                        itemInfo.PromotionAmount = 0.00M;//商品折扣
                                        itemInfo.Price = -item.DiscountAmount;
                                        itemInfo.ProductName = couponInfo.Title.Content + "(" + couponInfo.CouponCodeSetting.CouponCode + ")";
                                        itemInfo.ProductType = SOProductType.Coupon;
                                        itemInfo.Quantity = 1;
                                        itemInfo.Warranty = ResSOMaintain.Info_SOMaintain_Warranty_HaveNot;
                                        soViewModel.ItemsVM.Add(itemInfo);
                                        soViewModel.CouponCode = couponInfo.CouponCodeSetting.CouponCode;
                                        ReloadShoppingCarDeep();
                                    }
                                });
                            }
                        }
                    }
                });
                #endregion
            }
        }

        /// <summary>
        /// 手动设置 运费
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkManualInputShipPrice_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked.Value == true)//运费
            {
                txtManualShipPrice.IsReadOnly = false;
                txtManualShipPrice.Visibility = Visibility.Visible;
                hlkb_Car_T_Apply_ManualShipPrice.Visibility = Visibility.Visible;
            }
            else
            {
                soViewModel.BaseInfoVM.ManualShipPrice = null;
                ReloadShoppingCar(true);
                txtManualShipPrice.IsReadOnly = true;
                txtManualShipPrice.Visibility = Visibility.Collapsed;
                hlkb_Car_T_Apply_ManualShipPrice.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 应用 手动设置的运费
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_Car_T_Apply_ManualShipPrice_Click(object sender, RoutedEventArgs e)
        {
            ReloadShoppingCar(true);
        }

        /// <summary>
        /// 保价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chk_Premium_Click(object sender, RoutedEventArgs e)
        {
            soViewModel.BaseInfoVM.IsPremium = ((CheckBox)sender).IsChecked.Value;
            ReloadShoppingCar(true);
            if (((CheckBox)sender).IsChecked.Value == false)//保价费
            {
                soViewModel.BaseInfoVM.PremiumAmount = null;
            }
        }

        /// <summary>
        /// 账户余额支付
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAccountPrepayAmount_Click(object sender, RoutedEventArgs e)
        {
            soViewModel.BaseInfoVM.IsUsePrePay = ((CheckBox)sender).IsChecked.Value;
            ReloadShoppingCar(true);
            if (soViewModel.BaseInfoVM.PrepayAmount == null || soViewModel.BaseInfoVM.PrepayAmount == 0)
            {
                soViewModel.BaseInfoVM.IsPremium = false;
                ((CheckBox)sender).IsChecked = false;
            }
            if (((CheckBox)sender).IsChecked.Value == false)// 余额支付 
            {
                soViewModel.BaseInfoVM.PrepayAmount = null;
                soViewModel.BaseInfoVM.IsPremium = false;
            }
        }

        /// <summary>
        /// 显示  礼品卡支付 LinkButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkGiftCardPay_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked.Value == true)// 礼品卡支付
            {
                //hlkb_Car_T_GiftCardPay.Visibility = Visibility.Visible;
            }
            else
            {
                soViewModel.GiftCardListVM = new List<GiftCardInfoVM>();
                soViewModel.BaseInfoVM.GiftCardPay = null;
                soViewModel.BaseInfoVM.IsUseGiftCard = null;
                //hlkb_Car_T_GiftCardPay.Visibility = Visibility.Collapsed;
                ReloadShoppingCar(true);
            }
        }

        /// <summary>
        /// 礼品卡支付
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_Car_T_GiftCardPay_Click(object sender, RoutedEventArgs e)
        {
            //SOApplyGiftCardPay giftCardPay = new SOApplyGiftCardPay(soViewModel.GiftCardListVM, soViewModel.BaseInfoVM.CustomerSysNo.Value);
            //var window = CPApplication.Current.CurrentPage.Context.Window;
            //IDialog dialog = window.ShowDialog(ResSOMaintain.Title_ApplyGiftCardPay, giftCardPay, (obj, args) =>
            //{
            //    if (args.DialogResult == DialogResultType.OK)
            //    {
            //        List<GiftCardInfoVM> selectedList = args.Data as List<GiftCardInfoVM>;
            //        if (selectedList != null && selectedList.Count > 0)
            //        {
            //            soViewModel.GiftCardListVM = selectedList;
            //            soViewModel.BaseInfoVM.IsUseGiftCard = true;
            //            //chkGiftCardPay.IsChecked = true;
            //            //hlkb_Car_T_GiftCardPay.Visibility = Visibility.Visible;
            //        }
            //        else if (soViewModel.GiftCardListVM == null && soViewModel.GiftCardListVM.Count == 0)
            //        {
            //            soViewModel.BaseInfoVM.GiftCardPay = null;
            //            soViewModel.BaseInfoVM.IsUseGiftCard = false;
            //            //chkGiftCardPay.IsChecked = false;
            //            //hlkb_Car_T_GiftCardPay.Visibility = Visibility.Collapsed;
            //        }
            //        ReloadShoppingCar(true);
            //    }
            //    if (args.DialogResult == DialogResultType.Cancel)
            //    {
            //        if (soViewModel.GiftCardListVM == null || soViewModel.GiftCardListVM.Count == 0)
            //        {
            //            soViewModel.BaseInfoVM.IsUseGiftCard = false;
            //            //chkGiftCardPay.IsChecked = false;
            //            //hlkb_Car_T_GiftCardPay.Visibility = Visibility.Collapsed;
            //        }
            //    }
            //});
            //giftCardPay.Dialog = dialog;
        }

        /// <summary>
        /// 待采购
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlkb_NeedPO_Click(object sender, RoutedEventArgs e)
        {
            SOItemInfoVM info = this.dataGridItemInfo.SelectedItem as SOItemInfoVM;
            if (info != null)
            {
                Window.Navigate(string.Format(ConstValue.PO_VirtualStockPurchaseOrderNew, info.SOSysNo, info.ProductSysNo), null, true);
            }
        }

        private void hlkb_BatchUploadItem_Click(object sender, RoutedEventArgs e)
        {
            if (soViewModel.BaseInfoVM.CustomerSysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SetVATInvoiceError, MessageType.Error);
            }
            else
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Excel (*.xls)|*.xls";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        var file = dlg.File;
                        Stream stream = file.OpenRead();
                        byte[] fileBytes = new byte[stream.Length];
                        stream.Read(fileBytes, 0, fileBytes.Length);
                        stream.Close();
                        List<SOItemInfo> items = null;

                        soFacade.BatchDealItemInFile(fileBytes, (o, arg) =>
                        {
                            if (arg.FaultsHandle())
                            {
                                return;
                            }
                            items = arg.Result;
                            #region 加载购物车
                            ProductQueryFilter query = new ProductQueryFilter();
                            //不读取作废商品
                            query.IsNotAbandon = true;
                            //不加载商家商品
                            query.AZCustomer = 0;
                            query.MerchantSysNo = 1;
                            query.ProductIds = items.Select(p => p.ProductID).ToList();
                            query.PagingInfo = new PagingInfo()
                            {
                                PageSize = int.MaxValue,
                                PageIndex = 0
                            };

                            new OtherDomainQueryFacade(this).QueryProductRequest(query, (queryObj, queryPorductArgs) =>
                            {
                                if (queryPorductArgs.FaultsHandle())
                                {
                                    return;
                                }
                                var list = queryPorductArgs.Result.Rows;

                                //无效商品的集合
                                var invalidItems = new List<string>();
                                //有存在不是有效地商品
                                bool isExists = false;
                                foreach (var item in items)
                                {
                                    isExists = false;
                                    foreach (var productRow in queryPorductArgs.Result.Rows)
                                    {
                                        if ((string)productRow["ProductID"] == item.ProductID)
                                        {
                                            //验证仓库
                                            if (item.Quantity > (int)productRow["OnlineQty"])
                                            {
                                                Window.Alert(string.Format(ResSOMaintain.Info_Error_MoreOnlineQTY, item.ProductID));
                                                return;
                                            }
                                            if (item.Quantity > (int)productRow["MaxQuantity"])
                                            {
                                                Window.Alert(string.Format(ResSOMaintain.Info_Error_MoreMaxQty, item.ProductID, (int)productRow["MaxQuantity"]));
                                                return;
                                            }

                                            item.ProductSysNo = (int)productRow["SysNo"];
                                            item.ProductType = SOProductType.Product;
                                            item.Weight = (int)productRow["Weight"];
                                            item.OnlineQty = (int)productRow["OnlineQty"];
                                            isExists = true;
                                            break;
                                        }
                                    }
                                    if (!isExists)
                                    {
                                        invalidItems.Add(item.ProductID);
                                    }
                                }
                                if (invalidItems.Count > 0)
                                {
                                    Window.Alert(string.Format(ResSOMaintain.Info_Error_InvalidProducts, string.Join(",", invalidItems.ToArray())));
                                    return;
                                }
                                #region 加载显示
                                //按钮操作
                                //hlkb_BatchUploadItem.Visibility = hlkb_DownloadModel.Visibility
                                //    = hlkb_AddGiftCardInShoppingCar.Visibility = 
                                 hlkb_AddProductInShoppingCar.Visibility = System.Windows.Visibility.Collapsed;
                                chkIsManualChangePrice.Visibility = System.Windows.Visibility.Visible;
                                chkIsManualChangePrice.IsChecked = true;

                                soViewModel.ItemsVM = items.Convert<SOItemInfo, SOItemInfoVM>();

                                CalcAddProductItemAndLoadShipCarItems();

                                #endregion
                                
                            });

                            #endregion
                        });
                    }
                    catch (Exception ex)
                    {
                        Window.Alert(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 计算添加商品项，并且加载到购物车
        /// </summary>
        private void CalcAddProductItemAndLoadShipCarItems()
        {
            if (soViewModel.ItemsVM != null && soViewModel.ItemsVM.Count > 0)
            {
                ProductQtyChange();
            }
            UtilityHelper.ReadOnlyControl(gridCarInfo, gridCarInfo.Children.Count, false);
            dataGridTotalInfo.Visibility = Visibility.Visible;
            gridCarEventInfo.Visibility = Visibility.Visible;
            List<SOBaseInfoVM> baseInfoList = new List<SOBaseInfoVM>();
            baseInfoList.Add(soViewModel.BaseInfoVM);
            dataGridTotalInfo.ItemsSource = baseInfoList;
            ReloadSOMaintainData();
        }

        #endregion

        #region 页面底部 Button 事件(订单操作相关)

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //只更新备注
            SOMaintainUpdateNote();
            return;

            if (soViewModel.ItemsVM == null || soViewModel.ItemsVM.Count == 0)
            {
                Window.Alert(ResSOMaintain.Info_SaveSOMaintain_Error, MessageType.Error);
                return;
            }

            #region 新增的时候，地区选择能验证通过，并且还不能赋值给ReceiverInfoVM的地区。这里先手动赋值，以后有时间需要改地区选择控件
            if (string.IsNullOrEmpty(ucReceiveArea.SelectedAreaSysNo))
            {
                //请选择收货人地区
                Window.Alert(ResSOMaintain.Info_ReceiveArea_Error, MessageType.Error);
                return;
            }
            soViewModel.ReceiverInfoVM.ReceiveAreaSysNo = int.Parse(ucReceiveArea.SelectedCitySysNo);
            #endregion

            ValidationManager.Validate(this.gridSOMaintain);
            if (!soViewModel.HasValidationErrors
                && soViewModel.BaseInfoVM.ValidationErrors.Count == 0
                && soViewModel.ReceiverInfoVM.ValidationErrors.Count == 0
                && soViewModel.ShippingInfoVM.ValidationErrors.Count == 0
                && soViewModel.InvoiceInfoVM.ValidationErrors.Count == 0
                )
            {
                if (string.IsNullOrEmpty(soViewModel.ReceiverInfoVM.ReceivePhone)
                    && string.IsNullOrEmpty(soViewModel.ReceiverInfoVM.ReceiveCellPhone))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_ReceivePhoneOrCallPhoneError, MessageType.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(soViewModel.ReceiverInfoVM.ReceiveCellPhone)
                     && soViewModel.ReceiverInfoVM.ReceiveCellPhone.Length != 11)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_ReceiveCallPhoneInput_Error, MessageType.Error);
                    return;
                }

                #region 检查单个订单不拆单条件
                //服务端去检查
                //var productItems = soViewModel.ItemsVM.Where(item => item.ProductType == SOProductType.Accessory
                //                                     || item.ProductType == SOProductType.Award
                //                                     || item.ProductType == SOProductType.Gift
                //                                     || item.ProductType == SOProductType.Product
                //                                     || item.ProductType == SOProductType.SelfGift);

                //if (productItems.Any(x => x.StockSysNo != soViewModel.ShippingInfoVM.LocalWHSysNo))
                //{
                //    //保存失败： 商品的所在仓库需要与配送方式对应的发货仓库一致。
                //    CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_Warehouse_Error, MessageType.Error);
                //    return;
                //}

                //if (productItems.Sum(x=>x.Quantity.Value) > 1)
                //{
                //    //检查存储运输方式
                //    var storeType = productItems.First().StoreType;
                //    if (productItems.Any(x => x.StoreType != storeType))
                //    {
                //        //保存失败： 商品的存储运输方式必须一致。
                //        CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_StoreType_Error, MessageType.Error);
                //        return;
                //    }

                //    //检查金额
                //    var productItemsPriceSum = productItems.Sum(item => item.ProductPriceSum);
                //    var selectedStock = this.StockList.FirstOrDefault(s => s.SysNo == soViewModel.ShippingInfoVM.LocalWHSysNo);
                //    if (selectedStock.CountryCode.ToUpper() == "HK")
                //    {
                //        if (productItemsPriceSum > 800m)
                //        {
                //            //保存失败： 香港仓发货的订单中商品总金额不得大于800元，单个商品不受此限制。
                //            CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOAmt_800_Error, MessageType.Error);
                //            return;
                //        }
                //    }
                //    else if (selectedStock.CountryCode.ToUpper() == "JP")
                //    {
                //        if (productItemsPriceSum > 1000m)
                //        {
                //            //保存失败： 日本仓发货的订单中商品总金额不得大于1000元，单个商品不受此限制。
                //            CPApplication.Current.CurrentPage.Context.Window.Alert(ResSOMaintain.Info_SOAmt_1000_Error, MessageType.Error);
                //            return;
                //        }
                //    }
                //}
                #endregion

                if (IsNewCreateSO)//新建订单
                {
                    if (soViewModel.BaseInfoVM.SOType == SOType.ElectronicCard && !(AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_CreateGiftCardOrder)))
                    {
                        Window.Alert(ResSO.Msg_Error_RightGiftCardOrderCreate, MessageType.Error);
                        btnSave.IsEnabled = true;
                        return;
                    }

                    soViewModel.SysNo = null;
                    soViewModel.BaseInfoVM.SysNo = null;
                    string tempCouponCode = soViewModel.CouponCode;
                    soFacade.CreateSO(soViewModel, (obj, args) =>
                    {
                        if (!args.FaultsHandle())
                        {
                            soViewModel = SOFacade.ConvertTOSOVMFromSOInfo(args.Result);
                            if (soViewModel.BaseInfoVM.IsBackOrder == true)
                            {
                                txtInventoryNotEnough.Visibility = Visibility.Visible;
                                hlkb_ShowBackOrder.Visibility = Visibility.Visible;
                            }

                            if (string.IsNullOrEmpty(soViewModel.CouponCode) && !string.IsNullOrEmpty(tempCouponCode))
                            {
                                soViewModel.CouponCode = tempCouponCode;
                            }

                            txtSOSysNo.IsReadOnly = true;
                            IsNewCreateSO = false;
                            ReloadSOMaintainData();
                            ReloadShoppingCar(false);
                            cmbSOType.IsEnabled = false;
                            LoadAvailableButton();

                            CheckIsSplit();

                            if (soViewModel != null && soViewModel.BaseInfoVM != null && soViewModel.BaseInfoVM.OrderTime.HasValue)
                            {
                                tbOrderTime.Text = soViewModel.BaseInfoVM.OrderTime.Value.ToString(ResConverter.DateTime_LongFormat);
                            }
                            this.SetControlReadOnly();
                            //tbVaildCustomerPoint.Text = soViewModel.BaseInfoVM.CustomerPoint.ToString();
                        }
                    });
                }
                else//编辑订单
                {
                    if (soViewModel.BaseInfoVM.SOType == SOType.ElectronicCard && !(AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_CreateGiftCardOrder)))
                    {
                        Window.Alert(ResSO.Msg_Error_RightGiftCardOrderCreate, MessageType.Error);
                        btnSave.IsEnabled = true;
                        return;
                    }
                    soFacade.ConfirmOperationSubSO(soViewModel, ConfirmAbandonAllSubSO, UpdateSO);
                }
            }
        }

        private void UpdateSO()
        {
            string tempCouponCode = soViewModel.CouponCode;
            soFacade.UpdateSO(soViewModel, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    soViewModel = SOFacade.ConvertTOSOVMFromSOInfo(args.Result);

                    #region 特殊不能复制的情况

                    //tbVaildCustomerPoint.Text = soViewModel.BaseInfoVM.CustomerPoint.ToString();
                    if (string.IsNullOrEmpty(soViewModel.CouponCode) && !string.IsNullOrEmpty(tempCouponCode))
                    {
                        soViewModel.CouponCode = tempCouponCode;
                    }

                    #endregion

                    LoadAvailableButton();

                    CheckIsSplit();
                }
            });
        }

        /// <summary>
        /// 提示是否需要提示自动拆单
        /// </summary>
        private void CheckIsSplit()
        {
            if (soViewModel.BaseInfoVM.SplitType == SOSplitType.Force)
            {
                Window.Alert(ResSOMaintain.Info_ForceSplitOrder);
            }
            else if (soViewModel.BaseInfoVM.SplitType == SOSplitType.Customer)
            {
                //IPP的做法没有任何意义，这里只是一个空的提示
                Window.Confirm(ResSOMaintain.Info_CustomerSplitOrder, (sendor, args) =>
                {
                    btnAudit.IsEnabled = args.DialogResult == DialogResultType.Cancel;
                    btnSplit.IsEnabled = args.DialogResult == DialogResultType.OK;
                });
            }
            else
            {
                //normal
                Window.Alert(ResSOMaintain.Info_SaveSuccessfully);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResSOMaintain.Info_ConfirmNewSO, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    int queryIndex = this.Request.URL.IndexOf('?');
                    string url = queryIndex > -1 ? this.Request.URL.Substring(0, queryIndex) : this.Request.URL;
                    url += "?" + DateTime.Now.Ticks.ToString();
                    Window.Navigate(url, null, false);
                }
            });
        }

        #region 订单拆分/取消拆分
        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            soFacade.SplitSO(SOSysNo, subSOSysList =>
            {
                if (subSOSysList != null && subSOSysList.Count > 0)
                {
                    Window.Alert(String.Format(ResSO.Info_SO_Processer_SO_Split_SubSO, String.Join(",", (from so in subSOSysList select so.SysNo))));
                    img_Order_Search_MouseLeftButtonDown(null, null);
                }
                soViewModel.BaseInfoVM.Status = SOStatus.Split;
            });
        }

        #endregion

        #region 订单审核
        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            if ((soViewModel.BaseInfoVM.SOType == SOType.ElectronicCard || soViewModel.BaseInfoVM.SOType == SOType.PhysicalCard)
                && !AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_AuditGiftCardOrder))
            {
                Window.Alert(ResSO.Msg_Error_RightGiftCardOrderAudit, MessageType.Error);
                return;
            }
            soFacade.AuditSO(SOSysNo, false, (vm) =>
            {
                soViewModel.BaseInfoVM.Status = vm.BaseInfoVM.Status;
                ReLoadPage(soViewModel.BaseInfoVM.SysNo.Value);
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            soFacade.ConfirmOperationSubSO(soViewModel, ConfirmAbandonAllSubSO, CancelAudit);
        }

        private void CancelAudit()
        {
            soFacade.CancelAuditSO(SOSysNo, (vm) =>
            {
                soViewModel.BaseInfoVM.Status = vm.BaseInfoVM.Status;
                ReLoadPage(soViewModel.BaseInfoVM.SysNo.Value);
                Window.Alert(ResSO.Info_SO_Processer_SO_CancelAudit_Pass, MessageType.Information);
            });
        }

        private void btnForceAudit_Click(object sender, RoutedEventArgs e)
        {
            if ((soViewModel.BaseInfoVM.SOType == SOType.ElectronicCard || soViewModel.BaseInfoVM.SOType == SOType.PhysicalCard)
                && !AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_AuditGiftCardOrder))
            {
                Window.Alert(ResSO.Msg_Error_RightGiftCardOrderAudit, MessageType.Error);
                return;
            }
            soFacade.AuditSO(SOSysNo, true, (vm) =>
            {
                soViewModel.BaseInfoVM.Status = vm.BaseInfoVM.Status;
                ReLoadPage(soViewModel.BaseInfoVM.SysNo.Value);
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }

        private void btnManagerAudit_Click(object sender, RoutedEventArgs e)
        {
            soFacade.ManagerAuditSO(SOSysNo, false, (vm) =>
            {
                soViewModel.BaseInfoVM.Status = vm.BaseInfoVM.Status;
                ReLoadPage(soViewModel.BaseInfoVM.SysNo.Value);
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }

        private void btnForceManagerAudit_Click(object sender, RoutedEventArgs e)
        {
            soFacade.ManagerAuditSO(SOSysNo, true, (vm) =>
            {
                soViewModel.BaseInfoVM.Status = vm.BaseInfoVM.Status;
                ReLoadPage(soViewModel.BaseInfoVM.SysNo.Value);
                Window.Alert(ResSO.Info_SO_Processer_SO_Audit_Pass, MessageType.Information);
            });
        }
        #endregion

        #region 订单作废
        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            Abandon(false);
        }

        private void btnAbandonAndReturnInventory_Click(object sender, RoutedEventArgs e)
        {
            Abandon(true);
        }

        private void btnBatchAbandonKFCSO_Click(object sender, RoutedEventArgs e)
        {
            SOKFCBatchAbandon content = new SOKFCBatchAbandon(this);
            content.CustomerSysNo = soViewModel.BaseInfoVM.CustomerSysNo.Value;
            content.Width = 820D;
            content.Height = 600D;
            IDialog dialog = this.Window.ShowDialog(ResSO.UC_Title_SOKFCProcess, content, (obj, args) =>
            {

            });
            content.Dialog = dialog;
        }

        private void Abandon(bool immediatelyReturnInventory)
        {
            UCReasonCodePicker content = new UCReasonCodePicker();
            content.ReasonCodeType = ReasonCodeType.Order;
            content.Dialog = Window.ShowDialog(ResSOMaintain.Info_SOMaintain_AbandSO, content, (obj, args) =>
            {
                if (args.Data != null)
                {
                    KeyValuePair<string, string> data = (KeyValuePair<string, string>)args.Data;

                    if (soViewModel.BaseInfoVM.SplitType == SOSplitType.SubSO)
                    {
                        soFacade.GetIsAllSubSONotOutStock(SOSysNo, (subSOSender, subSOargs) =>
                        {
                            if (!subSOargs.FaultsHandle())
                            {
                                if (subSOargs.Result)
                                {
                                    ConfirmAbandonAllSubSO();
                                }
                                else
                                {
                                    AbandonSO(immediatelyReturnInventory, data);
                                }
                            }
                        });
                    }
                    else
                    {
                        AbandonSO(immediatelyReturnInventory, data);
                    }
                }
            });
        }

        private void AbandonSO(bool immediatelyReturnInventory, KeyValuePair<string, string> data)
        {
            soFacade.AbandonSO(SOSysNo, immediatelyReturnInventory, (vm) =>
            {
                if (vm != null)
                {
                    new SOInternalMemoFacade().Create(new SOInternalMemoInfo
                    {
                        SOSysNo = SOSysNo,
                        Content = data.Value,
                        LogTime = DateTime.Now,
                        ReasonCodeSysNo = int.Parse(data.Key),
                        CompanyCode = CPApplication.Current.CompanyCode,
                        Status = SOInternalMemoStatus.FollowUp
                    }, null);
                    Window.Alert(ResSOMaintain.Info_SOMaintain_SO_Abanded, MessageType.Information);
                    img_Order_Search_MouseLeftButtonDown(null, null);
                }
            });
        }

        private void btnCreateAOAndAbandon_Click(object sender, RoutedEventArgs e)
        {
            soFacade.ConfirmOperationSubSO(soViewModel, ConfirmAbandonAllSubSO, AbandonAndCreateAO);
        }

        private void ConfirmAbandonAllSubSO()
        {
            Window.Confirm(ResSOMaintain.Info_AbandonAllSubSO, (cancelSplitSender, cancelSplitArgs) =>
            {
                if (cancelSplitArgs.DialogResult == DialogResultType.OK)
                {
                    soFacade.CancelSplitSO(soViewModel.BaseInfoVM.SOSplitMaster.Value, (vm) =>
                    {
                        ReLoadPage(soViewModel.BaseInfoVM.SOSplitMaster.Value);
                    });
                }
            });
        }

        private void AbandonAndCreateAO()
        {
            ECCentral.Portal.UI.SO.UserControls.CreateAOAndAbandonSO createAOControl = new CreateAOAndAbandonSO
            {
                Page = this,
                SOSysNo = SOSysNo,
                CurrentSOIncomeInfo = CurrentSOIncomeInfo,
                PayTypeSysNo = soViewModel.BaseInfoVM.PayTypeSysNo
            };
            createAOControl.Saved += (vm) =>
            {
                if (vm != null)
                {
                    soViewModel = vm;
                    LoadData();
                    if (soViewModel.BaseInfoVM == null ||  soViewModel.BaseInfoVM.Status != SOStatus.OutStock)
                    {
                        btnReportedFaulure.IsEnabled = false;
                    }

                    if (soViewModel.BaseInfoVM.Status == SOStatus.Abandon || soViewModel.BaseInfoVM.Status == SOStatus.SystemCancel || soViewModel.BaseInfoVM.Status == SOStatus.Reject)
                    {
                        btnSOInternalMemo.IsEnabled = btnCloneSO.IsEnabled = true;
                        btnHold.IsEnabled = false;
                        btnCreateAOAndAbandon.IsEnabled = false;
                        btnAbandonAndReturnInventory.IsEnabled = false;
                        btnAbandon.IsEnabled = false;
                        btnCancelAudit.IsEnabled = false;
                        btnSave.IsEnabled = false;
                    }
                }
            };
            createAOControl.Dialog = Window.ShowDialog(String.Format(ResSO.UC_Title_CreateAO, SOSysNo), createAOControl, (obj, args) =>
            {

            });
        }
        #endregion

        /// <summary>
        /// HoldSO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHold_Click(object sender, RoutedEventArgs e)
        {
            HoldSO content = new HoldSO();
            content.Page = this;
            content.CurrentSOVM = soViewModel;
            content.Dialog = Window.ShowDialog(ResSO.UC_Title_SOHold, content, (obj, args) =>
            {
                soViewModel = content.CurrentSOVM;
            });
        }

        /// <summary>
        /// 拆分发票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSplitInvoice_Click(object sender, RoutedEventArgs e)
        {
            SplitInvoice invoiceDialog = new SplitInvoice(this, soViewModel);
            invoiceDialog.Width = 650;
            invoiceDialog.Height = 420;
            invoiceDialog.Dialog = Window.ShowDialog(ResSOMaintain.Info_SOMaintain_SplitSO, invoiceDialog);
        }

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrintSO_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dPrint = new Dictionary<string, string>();
            dPrint.Add("SOSysNoList", soViewModel.BaseInfoVM.SysNo.ToString());
            HtmlViewHelper.WebPrintPreview("SO", "SOInfo", dPrint);
        }

        /// <summary>
        /// 设置恶意用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetMaliceCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (!soViewModel.BaseInfoVM.SysNo.HasValue && soViewModel.BaseInfoVM.SysNo == 0)
            {
                Window.Alert(ResSOMaintain.Info_SetVATInvoiceError, MessageType.Error);
                return;
            }
            soFacade.SetMaliceCustomer(soViewModel.BaseInfoVM.CustomerSysNo.Value, string.Format(ResSOMaintain.Info_SetMaliceCustomer_Memo, soViewModel.BaseInfoVM.SysNo.Value, soViewModel.BaseInfoVM.CustomerSysNo.Value), soViewModel.BaseInfoVM.SysNo.Value, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Window.Alert(ResSOMaintain.Info_SetMaliceCustomerSuccessfully);
                }
            });
        }

        /// <summary>
        /// 订单拦截
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInterceptSO_Click(object sender, RoutedEventArgs e)
        {
            SendSOInterceptEmail soInterceptEmail = new SendSOInterceptEmail(soViewModel);
            var window = CPApplication.Current.CurrentPage.Context.Window;
            IDialog dialog = window.ShowDialog(ResSOMaintain.Title_SendSOInterceptEmail, soInterceptEmail, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {

                }
            });
            soInterceptEmail.Dialog = dialog;
        }

        /// <summary>
        /// Button 控制
        /// </summary>
        private void LoadAvailableButton()
        {
            #region By Status

            OperationControlStatusHelper.SetAllButtonNotEnabled(gridFunctionButton);

            btnPrintSO.IsEnabled = !IsNewCreateSO;

            if (soViewModel.BaseInfoVM.Status == null)
            {
                btnSave.IsEnabled = btnNew.IsEnabled = true;
               // hlkb_BatchUploadItem.Visibility = hlkb_DownloadModel.Visibility = System.Windows.Visibility.Visible;
            }
            else if (soViewModel.BaseInfoVM.Status == SOStatus.Abandon || soViewModel.BaseInfoVM.Status == SOStatus.SystemCancel || soViewModel.BaseInfoVM.Status == SOStatus.Reject)
            {
                btnSOInternalMemo.IsEnabled = btnCloneSO.IsEnabled = true;
            }
            else if (soViewModel.BaseInfoVM.Status == SOStatus.Origin)
            {
                btnSave.IsEnabled = btnNew.IsEnabled =
                btnAudit.IsEnabled = btnForceAudit.IsEnabled =
                AbandonButtonSet = btnHold.IsEnabled = btnSplitInvoice.IsEnabled =
                btnSetMaliceCustomer.IsEnabled = btnSOInternalMemo.IsEnabled = true;

                if (soViewModel.BaseInfoVM.SplitType == SOSplitType.Customer
                    || soViewModel.BaseInfoVM.SplitType == SOSplitType.Force)
                {
                    btnSplit.IsEnabled = true;
                    btnAudit.IsEnabled = btnForceAudit.IsEnabled = false;
                }
            }
            else if (soViewModel.BaseInfoVM.Status == SOStatus.WaitingOutStock)
            {
                btnCancelAudit.IsEnabled = AbandonButtonSet = btnSplitInvoice.IsEnabled = btnHold.IsEnabled = btnSOInternalMemo.IsEnabled = true; //btnInterceptSO.IsEnabled
            }
            else if (soViewModel.BaseInfoVM.Status == SOStatus.WaitingManagerAudit)
            {
                btnCancelAudit.IsEnabled = AbandonButtonSet = btnManagerAudit.IsEnabled
                    = btnForceManagerAudit.IsEnabled = btnSplitInvoice.IsEnabled = btnSOInternalMemo.IsEnabled = true;
            }
            else if (soViewModel.BaseInfoVM.Status == SOStatus.OutStock)
            {
                btnSplitInvoice.IsEnabled = btnHold.IsEnabled = btnSOInternalMemo.IsEnabled = true;   //= btnInterceptSO.IsEnabled
            }
            if (IsVATSO)
            {
                btnHold.IsEnabled = true;
            }
            if (soViewModel.BaseInfoVM.Status != SOStatus.Origin && soViewModel.BaseInfoVM.SOType == SOType.ElectronicCard)
            {
                AbandonButtonSet = false;
            }
            if (soViewModel.ShippingInfoVM.StockType == StockType.MET)
            {
                btnSave.IsEnabled = btnSplit.IsEnabled = btnCloneSO.IsEnabled = btnCancelAudit.IsEnabled = false;
                //是团购订单、还要是团购失败
                if (soViewModel.BaseInfoVM.SOType == SOType.GroupBuy
                    && soViewModel.BaseInfoVM.SettlementStatus == SettlementStatus.PlanFail)
                {
                    btnSave.IsEnabled = true;
                }
                switch (soViewModel.BaseInfoVM.Status)
                {
                    case SOStatus.WaitingOutStock:
                        btnSplitInvoice.IsEnabled = AbandonButtonSet = false;
                        break;
                    case SOStatus.OutStock:
                        btnSplitInvoice.IsEnabled = false;
                        break;
                    default:
                        break;
                }
            }
            if (soViewModel != null && soViewModel.BaseInfoVM != null && soViewModel.BaseInfoVM.Status == SOStatus.OutStock)
            {
                new OtherDomainQueryFacade(this).GetSOIncomeBySOSysNo(SOSysNo, (soIncomeInfo) =>
                {
                    CurrentSOIncomeInfo = soIncomeInfo;
                    btnReportedFaulure.IsEnabled = true;
                });
            }
            else
            {
                btnReportedFaulure.IsEnabled = false;
            }
            if (btnAbandon.IsEnabled)
            {
                new OtherDomainQueryFacade(this).GetSOIncomeBySOSysNo(SOSysNo, (soIncomeInfo) =>
                {
                    CurrentSOIncomeInfo = soIncomeInfo;
                    soFacade.ConfirmOperationSubSO(soViewModel, SetAbandon, SetAOAbondon);
                });
            }

            #endregion

            #region 总是可使用的按钮

            hlkb_ShowBackOrder.IsEnabled = true;
            //btnNew.Visibility = System.Windows.Visibility.Visible;
            btnNew.IsEnabled = true;

            #endregion

            #region Right Controls

            //账户余额支付
            chkAccountPrepayAmount.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_UsePrepay);

            //手动改价
            if (!IsNewCreateSO
                && (AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_IsWholeSale) || AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_ViewMunalPriceInfo))
                )
            {
                chkIsManualChangePrice.Visibility = Visibility.Visible;
            }

            //有效积分显示
            //tbVaildCustomerPoint.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_ShowPoint);

            //积分支付应用
            //hlkb_Car_T_Apply_PointPay.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_ApplyPoint);

            //基本客户信息
            tlkReceiveInfo.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_ViewCustomerInfo);

            //修改财务备注
            txtFinanceNote.IsReadOnly = AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_CanUpdateSOFinanceNote);

            ////修改是否VIP订单
            //cbIsVIP.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_CanUpdateIsVIP);

            ////是否VIP订单选中状态
            //cbIsVIP.IsChecked = !IsNewCreateSO && AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_IsVIPRole);

            //平均成本那列
            //var colCostPrice = dataGridItemInfo.Columns.First(p => p.Header.ToString() == ResSOMaintain.Grid_Car_ProductUnitCost);
            //colCostPrice.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_ViewGrossProfit);

            //取消审核
            btnCancelAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOCancelAudit);

            //审核
            btnAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAudit);
            //强制审核
            btnForceAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAudit);

            //创建
            if (IsNewCreateSO)
            {
                //btnSave.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOCreate);
                //btnNew.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOCreate);
                btnSave.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                //更新
                btnSave.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOUpdate);
            }

            //拆分
            btnSplit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SplitSO);
            btnCloneSO.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SplitSO);

            //主管审核
            btnManagerAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOManagerAudit);
            btnForceManagerAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOManagerAudit);

            //作废
            btnAbandon.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAbandon);
            btnBatchAbandonKFCSO.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAbandon);
            btnAbandonAndReturnInventory.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOEmployeeAbandon);
            btnCreateAOAndAbandon.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAbandon, AuthKeyConst.SO_SOMaintain_CreateNegativeFinanceRecord);

            //锁定
            btnHold.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_HoldSO);

            //设置恶意用户
            btnSetMaliceCustomer.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.Customer_MaintainMaliceUser);

            //是否有权限编辑客户收货地址信息
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_EditSOCustomerInfo))
            {
                OperationControlStatusHelper.SetControlsStatus(gridSOReceiveInfo, true);
            }

            //hlkb_SetOInvoiceTOVATInvoice.CollapsedWhenNotRight(AuthKeyConst.SO_SOMaintain_InvoiceToVatOpt);

            //hlkb_SaveOInvoiceTOVATInvoice.CollapsedWhenNotRight(AuthKeyConst.SO_SOMaintain_InvoiceToVatOpt);

            //批量上传订单
            //hlkb_BatchUploadItem.CollapsedWhenNotRight(AuthKeyConst.SO_SOMaintain_HasBatchUploadItem);
            //hlkb_DownloadModel.CollapsedWhenNotRight(AuthKeyConst.SO_SOMaintain_HasBatchUploadItem);

            #endregion
            btnSplit.Visibility = System.Windows.Visibility.Collapsed;
            btnSplitInvoice.Visibility = System.Windows.Visibility.Collapsed;
            btnCloneSO.Visibility = System.Windows.Visibility.Collapsed;
        }

        void SetAbandon()
        {
            btnCreateAOAndAbandon.IsEnabled = false;
            btnAbandonAndReturnInventory.IsEnabled = btnAbandon.IsEnabled = !btnCreateAOAndAbandon.IsEnabled;
        }

        void SetAOAbondon()
        {
            btnCreateAOAndAbandon.IsEnabled = CurrentSOIncomeInfo != null;
            btnAbandonAndReturnInventory.IsEnabled = btnAbandon.IsEnabled = !btnCreateAOAndAbandon.IsEnabled;
        }

        bool AbandonButtonSet
        {
            set
            {
                btnAbandon.IsEnabled = btnAbandonAndReturnInventory.IsEnabled
                = btnCreateAOAndAbandon.IsEnabled = btnBatchAbandonKFCSO.IsEnabled = value;
            }
        }

        /// <summary>
        /// 重新加载订单
        /// </summary>
        /// <param name="SOSysNo"></param> 
        private void ReLoadPage(int soSysNo)
        {
            this.SOSysNo = soSysNo;
            LoadSOInfo();
        }
        #endregion

        #region 拆分生成新订单
        private void btnCloneSO_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.SO_SOMaintain_SplitNewSO))
            {
                Window.Alert(ResSOMaintain.Msg_HasNoRight);
                return;
            }
            var list = this.dataGridItemInfo.ItemsSource as List<SOItemInfoVM>;
            if (list != null
                && list.Count != 0)
            {
                var selectList = list.Where(item => item.IsChecked).ToList();
                if (selectList != null
                    && selectList.Count != 0)
                {
                    string prodctName = string.Empty;
                    for (int i = 0; i < selectList.Count; i++)
                    {
                        prodctName += "[" + selectList[i].ProductName + "]";
                    }
                    Window.Confirm(string.Format(ResSOMaintain.Msg_ConfirmSplitProduct, prodctName), (o, s) =>
                        {
                            if (s.DialogResult == DialogResultType.OK)
                            {
                                this.soViewModel.ItemsVM = selectList;

                                soFacade.CloneSO(this.soViewModel, (obj, args) =>
                                {
                                    if (args.FaultsHandle()) return;
                                    if (args.Result != null)
                                    {
                                        SOVM newSOVM = new SOVM();
                                        newSOVM = SOFacade.ConvertTOSOVMFromSOInfo(args.Result);
                                        if (newSOVM.BaseInfoVM.IsBackOrder == true)
                                        {
                                            txtInventoryNotEnough.Visibility = Visibility.Visible;
                                            hlkb_ShowBackOrder.Visibility = Visibility.Visible;
                                        }
                                        string url = String.Format(ConstValue.SOMaintainUrlFormat, newSOVM.BaseInfoVM.SOID);
                                        this.Window.Navigate(url, null, true);
                                    }
                                });
                            }
                        });
                }
                else
                {
                    Window.Alert(ResSOMaintain.Msg_SelectSplitProduct);
                    return;
                }
            }
        }
        #endregion

        private void btnSOInternalMemo_Click(object sender, RoutedEventArgs e)
        {

            HyperlinkButton btn = sender as HyperlinkButton;
            publicProcess content = new publicProcess(this, this.SOSysNo);
            content.Width = 800D;
            content.Height = 500D;
            IDialog dialog = this.Window.ShowDialog(String.Format("{0}{1}", ResSO.UC_Title_SOMemoProcessor, this.SOSysNo), content, (obj, args) =>
            {

            });
            content.Dialog = dialog;
        }


        private void btnReportedFaulure_Click(object sender, RoutedEventArgs e)
        {
            //申报失败 作废订单 走AO单流程
            AbandonAndCreateAO();
        }

        void SetReportedButtonIsEnabled()
        {
            btnReported.IsEnabled = false;
            btnReject.IsEnabled = false;
            btnCustomsPass.IsEnabled = false;
            btnCustomsReject.IsEnabled = false;
            if (soViewModel.BaseInfoVM.Status == SOStatus.OutStock)
            {
                btnReported.IsEnabled = true;
                btnReject.IsEnabled = true;
            }
            if (soViewModel.BaseInfoVM.Status == SOStatus.Reported)
            {
                btnCustomsPass.IsEnabled = true;
                btnCustomsReject.IsEnabled = true;
            }
        }

        private void btnReported_Click(object sender, RoutedEventArgs e)
        {
            //申报成功;
            if (soViewModel == null || (!soViewModel.SysNo.HasValue))
            {
                return;
            }
            soFacade.UpdateSOStatusToReported(soViewModel.SysNo.Value, (result) =>
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    soViewModel.BaseInfoVM.Status =  SOStatus.Reported;
                    ReLoadPage(soViewModel.SysNo.Value);
                    Window.Alert("操作成功", MessageType.Information);
                }
                else
                {
                     Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning);
                }
            });
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            //申报失败;
            if (soViewModel == null || (!soViewModel.SysNo.HasValue))
            {
                return;
            }
            soFacade.UpdateSOStatusToReject(soViewModel.SysNo.Value, (result) =>
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    soViewModel.BaseInfoVM.Status = SOStatus.Reported;
                    ReLoadPage(soViewModel.SysNo.Value);
                    Window.Alert("操作成功", MessageType.Information);
                }
                else
                {
                    Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning);
                }
            });
        }

        private void btnCustomsPass_Click(object sender, RoutedEventArgs e)
        {
            //通关成功;
            if (soViewModel == null || (!soViewModel.SysNo.HasValue))
            {
                return;
            }
            soFacade.UpdateSOStatusToCustomsPass(soViewModel.SysNo.Value, (result) =>
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    soViewModel.BaseInfoVM.Status = SOStatus.Reported;
                    ReLoadPage(soViewModel.SysNo.Value);
                    Window.Alert("操作成功", MessageType.Information);
                }
                else
                {
                    Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning);
                }
            });
        }

        private void btnCustomsReject_Click(object sender, RoutedEventArgs e)
        {
            //通关失败;
            if (soViewModel == null || (!soViewModel.SysNo.HasValue))
            {
                return;
            }
            soFacade.UpdateSOStatusToCustomsReject(soViewModel.SysNo.Value, (result) =>
            {
                if (string.IsNullOrWhiteSpace(result))
                {
                    soViewModel.BaseInfoVM.Status = SOStatus.Reported;
                    ReLoadPage(soViewModel.SysNo.Value);
                    Window.Alert("操作成功", MessageType.Information);
                }
                else
                {
                    Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning);
                }
            });
        }



        private void SOMaintainUpdateNote()
        {
            //当保存按钮处于可保存状态切商品处于待审核或者待出库状态可修改配送区域--2015.10.20 by John.E.Kang
            if ((btnSave.IsEnabled == true && btnSave.Visibility == Visibility.Visible) && (soViewModel.BaseInfoVM.Status == SOStatus.Origin || soViewModel.BaseInfoVM.Status == SOStatus.WaitingOutStock))
            {
                soViewModel.ReceiverInfoVM.ReceiveAreaSysNo = this.ucReceiveArea.QueryAreaSysNo;
                soViewModel.ReceiverInfoVM.ReceiveAddress = this.txtReceiveAddress.Text;
            }
            //更新订单备注
            soFacade.SOMaintainUpdateNote(soViewModel, (obj, args) =>
            {
                if (!args.FaultsHandle())
                {
                    Window.Alert("保存成功！", MessageType.Information);
                }
            });
        }
    }
}
