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
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.UI.MKT.Models.Promotion;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Service.MKT.Restful.ResponseMsg;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic.Utilities;
using System.Text;
using ECCentral.Portal.Basic.Components.UserControls.DatetimePicker;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CountdownMaintain : PageBase
    {
        private string _OP;
        private int _SysNo;
        private string _Status;
        private CountdownInfoVM _ViewModel;
        private CountdownFacade _Facade;

        public CountdownMaintain()
        {
            InitializeComponent();
           
        }

        void ucItemMaster_ProductSelected(object sender, ProductSelectedEventArgs e)
        {
            new ECCentral.Portal.UI.MKT.Facades.ExternalServiceFacade(this).GetProductInfo(e.SelectedProduct.SysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                _ViewModel.VendorSysNo = args.Result.Merchant.SysNo.Value;
                _ViewModel.VendorName = args.Result.Merchant.MerchantName;
                _Facade.GetPMByProductSysNo(e.SelectedProduct.SysNo.Value, (obj2, args2) =>
                                                                               {
                                                                                   PM.Text = args2.Result;
                                                                               });
            });
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                _OP = this.Request.Param;
            }
            this.ucItemMaster.ProductSelected += new EventHandler<ProductSelectedEventArgs>(ucItemMaster_ProductSelected);
            _Facade = new CountdownFacade(this);

            LoadChannel();
            if (int.TryParse(_OP, out _SysNo))
            {
                _Facade.Load(_SysNo, (o, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        _ViewModel = args.Result;
                        _ViewModel.StatusVisibility = true;
                        this.DataContext = _ViewModel;
                        ucItemMaster.SetProductSysNo(int.Parse(_ViewModel.ProductSysNo));
                        GetProductDetail();
                        SetStockQty();
                        _ViewModel.ChannelID = "1";
                        if (_ViewModel.Status == CountdownStatus.Interupt || _ViewModel.Status == CountdownStatus.Finish || _ViewModel.Status == CountdownStatus.Abandon || _ViewModel.Status == CountdownStatus.WaitForVerify || _ViewModel.Status == CountdownStatus.WaitForPrimaryVerify)
                            SetAllReadOnlyOrEnable();
                        if (_ViewModel.Status == CountdownStatus.WaitForVerify || _ViewModel.Status == CountdownStatus.WaitForPrimaryVerify)
                        {
                            txtVerifyMemo.IsEnabled = true;
                            txtVerifyMemo.IsReadOnly = false;
                        }
                    }
                    
                    this.Title = _ViewModel.IsPromotionScheduleStr + ResCountdownMaintain.Msg_Maintain;
                    #region 促销计划
                    if (_ViewModel.IsPromotionSchedule)
                    {
                        _ViewModel.IsHomePageShow = false;
                        txtQuickTimes.Visibility = Visibility.Collapsed;
                        lstQuickTimes.Visibility = Visibility.Collapsed;
                    }
                    #endregion
                });
            }
            else
            {
                _ViewModel = InitParam(_OP.StartsWith("sd"));
                #region 促销计划
                if (_ViewModel.IsPromotionSchedule)
                {
                    rbIsNeedVerify.IsChecked = true;
                    txtIsNeedSubmit.Visibility = Visibility.Collapsed;
                    ButtonFormClear.Visibility = Visibility.Visible;
                    txtQuickTimes.Visibility = Visibility.Collapsed;
                    lstQuickTimes.Visibility = Visibility.Collapsed;
                }
                #endregion
                this.DataContext = _ViewModel;
                this.Title = _ViewModel.IsPromotionScheduleStr + ResCountdownMaintain.Msg_Maintain;
            }
        

        }
       
        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel = InitParam(_OP.StartsWith("sd"));
            this.DataContext = _ViewModel;
            cbReserveInventory_Checked(null, null);
            //ButtonSave.IsEnabled = true;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ModifyMaintain();
        }
        private void ButtonSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInputIsPass())
                return;
            _ViewModel.IsSubmitAudit = true;
            _ViewModel.Status = CountdownStatus.WaitForPrimaryVerify;
            _Facade.Update(_ViewModel, (o, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    Window.Alert(ResCountdownMaintain.Msg_AlreadySubAudit);
                    Window.Refresh();
                }
            });
        }
        private void ModifyMaintain()
        {
           
            if (!CheckInputIsPass())
                return;
            
            if (_ViewModel.SysNo <= 0)
            {
                if (rbIsNeedVerify.IsChecked.Value)//如果勾选提交审核，那么需要更新状态
                    _ViewModel.Status = CountdownStatus.WaitForPrimaryVerify;

                //提示是否确认不提交审核
                if (_ViewModel.Status == CountdownStatus.Ready)
                {

                    _Facade.GetGrossMargin(_ViewModel, (objM, argM) =>
                    {
                        if (argM.FaultsHandle())
                        {
                            return;
                        }
                        GrossMarginMsg msg = argM.Result;

                        _Facade.CheckOptionalAccessoriesInfoMsg(_ViewModel, (objC, argC) =>
                        {
                            if (!argC.FaultsHandle())
                            {
                                //if (Number(smargin) <= Number(margin) || Number(smarginRate) <= Number(marginRate) || _hdCheckOptionalAccessoriesInfo) {
                                if (msg.GrossMargin <= msg.CountDownMargin|| msg.GrossMarginRate <= msg.CountDownMarginRate
                                    || !string.IsNullOrEmpty(argC.Result))
                                {
                                    StringBuilder confirmStr = new StringBuilder();
                                    confirmStr.Append(string.Format(ResCountdownMaintain.Info_GrossMarginError,
                                        msg.GrossMarginRate, msg.GrossMargin));

                                    confirmStr.Append("\r\n" + argC.Result + "\r\n");

                                    confirmStr.Append(ResCountdownMaintain.Info_SureNotSubmitAudit);
                                    this.Window.Confirm(confirmStr.ToString(), (obj, args) =>
                                    {
                                        if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                                        {
                                            _Facade.Create(_ViewModel, (o, arg) =>
                                            {
                                                if (!arg.FaultsHandle())
                                                {
                                                    //Window.Alert("创建成功");
                                                    Window.Alert(ResCountdownMaintain.Msg_CreateSuccess);
                                                    Window.Close();
                                                }
                                            });

                                        }
                                    });
                                }
                                else
                                {
                                    _Facade.Create(_ViewModel, (o, arg) =>
                                    {
                                        if (!arg.FaultsHandle())
                                        {
                                            //Window.Alert("创建成功");
                                            Window.Alert(ResCountdownMaintain.Msg_CreateSuccess);
                                            Window.Close();
                                        }
                                    });

                                }
                            }
                        });
                    });
                }
                else
                {
                    _Facade.Create(_ViewModel, (o, arg) =>
                    {
                        if (!arg.FaultsHandle())
                        {
                            //Window.Alert("创建成功");
                            Window.Alert(ResCountdownMaintain.Msg_CreateSuccess);
                            Window.Close();
                        }
                    });
                }

            }
            else
            {
                _Facade.Update(_ViewModel, (o, arg) =>
                {
                    if (!arg.FaultsHandle())
                    {
                        // Window.Alert("更新成功");
                        Window.Alert(ResCountdownMaintain.Msg_UpdateSuccess);
                        Window.Refresh();
                    }
                });
            }
        }

        private bool CheckInputIsPass()
        {
            _ViewModel.ValidationErrors.Clear();
            if (!ValidationManager.Validate(CheckBoxContainer))
            {
                return false;
            }
            if (!ValidationManager.Validate(headerContainer))
            {
                return false;
            }
            if (!ValidationManager.Validate(dgdSetCount))
            {
                return false;
            }
            
            int validationErrorCount = 0;
            if (_ViewModel.HasValidationErrors)
            {
                foreach (var item in _ViewModel.ValidationErrors)
                {
                    foreach (var member in item.MemberNames)
                    {
                        if (member.Equals("MaxPerOrder") && !_ViewModel.IsPromotionSchedule)
                        {
                            validationErrorCount++;
                        }

                        if (member.Equals("BaseLine") && !_ViewModel.IsEmailNotify)
                        {
                            validationErrorCount++;
                        }
                        if (member.Equals("CountdownCount"))
                        {
                            validationErrorCount++;
                        }
                        if (member.Equals("AreaShowPriority") && !_ViewModel.IsCountDownAreaShow.Value)
                        {
                            validationErrorCount++;
                        }
                        if (member.Equals("HomePagePriority") && !_ViewModel.IsHomePageShow.Value)
                        {
                            validationErrorCount++;
                        }
                    }
                }
            }
            if (_ViewModel.ValidationErrors.Count > 0 && _ViewModel.ValidationErrors.Count - validationErrorCount > 0)
            {
                return false;
            }
            return true;
        }

        private void ShowProductDetail_Click(object sender, RoutedEventArgs e)
        {
            GetProductDetail();
        }

        private void GetProductDetail()
        {
            _ViewModel.ValidationErrors.Clear();
            ValidationManager.Validate(ucItemMaster);
            if (_ViewModel.HasValidationErrors)
                return;
            _Facade.LoadProductInfoDetail(_ViewModel.ProductSysNo, (o, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    if (_ViewModel.IsReservedQty.HasValue && _ViewModel.IsReservedQty.Value)
                        arg.Result.CurrentReservedQty = int.Parse(_ViewModel.CountDownQty ?? "0");
                    arg.Result.LastPurchaseDate = arg.Result.LastPurchaseDate == null ? DateTime.Parse("0001/1/1 0:00:00") : arg.Result.LastPurchaseDate;
                    dgdProductDetail.ItemsSource = new List<MKTProductDetailMsg>() { arg.Result };
                }
            });
        }

      
    

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            // 跳转至产品的编辑页面
            this.Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, _ViewModel.ProductSysNo), null, true);
        }

        private void lbtnSetCount_Click(object sender, RoutedEventArgs e)
        {
            SetStockQty();
        }

        private void SetStockQty()
        {
            _Facade.LoadStockQtyList(_ViewModel.ProductSysNo, (o, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    var list = new System.Collections.ObjectModel.ObservableCollection<StockQtySettingVM>();
                    arg.Result.ForEach(item =>
                    {
                        if (item.StockInfo != null && item.StockInfo.StockStatus == ECCentral.BizEntity.Inventory.ValidStatus.Valid)
                        {
                            var vm = new StockQtySettingVM()
                            {
                                StockName = item.StockInfo.StockName,
                                StockSysNo = item.StockInfo.SysNo,
                                AvailableQty = item.AvailableQty,
                                ConsignQty = item.ConsignQty,
                                VirtualQty = item.VirtualQty
                            };
                            if (_ViewModel.AffectedStockList != null && _ViewModel.AffectedStockList.Count > 0)
                            {
                                var orgain = _ViewModel.AffectedStockList.Where(p => p.StockSysNo == item.StockInfo.SysNo);
                                if (orgain != null && orgain.Count() > 0)
                                    vm.Qty = orgain.FirstOrDefault().Qty;
                                else
                                    vm.Qty = "0";
                            }
                            list.Add(vm);
                        }
                    });
                    _ViewModel.AffectedStockList = list;
                }
            });
        }
        private void cbReserveInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (cbLimitSalesCount.IsChecked.Value && !_ViewModel.IsPromotionSchedule)
            {
                cbStop.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                cbStop.Visibility = System.Windows.Visibility.Collapsed;
            }

            //if (cbReserveInventory.IsChecked.Value || cbLimitSalesCount.IsChecked.Value)
            //{
            //    //dgdSetCount.Visibility = System.Windows.Visibility.Visible;
            //    //lbtnSetCount.Visibility = System.Windows.Visibility.Visible;
            //    txbCountdownCount.Visibility = System.Windows.Visibility.Visible;
            //    txtCountdownCount.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    //dgdSetCount.Visibility = System.Windows.Visibility.Collapsed;
            //    //lbtnSetCount.Visibility = System.Windows.Visibility.Collapsed;
            //    txbCountdownCount.Visibility = System.Windows.Visibility.Collapsed;
            //    txtCountdownCount.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }



        private void SetAllReadOnlyOrEnable()
        {
            foreach (var item in CheckBoxContainer.Children)
            {
                if (item is CheckBox)
                {
                    (item as CheckBox).IsEnabled = false;
                }
                if (item is TextBox)
                {
                    (item as TextBox).IsReadOnly = true;
                }
                if (item is DatePicker)
                {
                    (item as DatePicker).IsEnabled = false;
                }
            }

            foreach (var item in headerContainer.Children)
            {
                if (item is ComboBox)
                {
                    (item as ComboBox).IsEnabled = false;
                }
                if (item is UCProductPicker)
                {
                    (item as UCProductPicker).IsEnabled = false;
                }
            }
         
        }

        private void LoadChannel()
        {
            List<WebChannelVM> webChennelList = new List<WebChannelVM>();
            foreach (UIWebChannel uiChannel in CPApplication.Current.CurrentWebChannelList)
            {
                webChennelList.Add(new WebChannelVM() { ChannelID = uiChannel.ChannelID, ChannelName = uiChannel.ChannelName });
            }
            webChennelList.Insert(0, new WebChannelVM() { ChannelName = ResCommonEnum.Enum_Select });
            lstChannel.ItemsSource = webChennelList;

            //绑定快速选择时间
            //List<CodeNamePair> quickTimes=
            _Facade.GetQuickTimes((o, args) =>
            {
                if (!args.FaultsHandle())
                {
                    lstQuickTimes.ItemsSource = args.Result;
                }
            });
        }

        private CountdownInfoVM InitParam(bool _isPromotionSchedule)
        {
            CountdownInfoVM vm = new CountdownInfoVM();
            vm.IsPromotionSchedule = _isPromotionSchedule;

            // 以下为初始化时的默认值
            vm.StartTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00");

            if (vm.IsPromotionSchedule)
            {
                vm.Status = CountdownStatus.Init;//促销计划初始状态为  初始
            }
            else
            {
                vm.Status = CountdownStatus.Ready;//限时抢购初始状态为 就绪
                vm.EndTime = DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString() + " 23:59:59");
            }
            vm.BaseLine = "5";
            vm.CompanyCode = CPApplication.Current.CompanyCode;

            //vm.IsReservedQty = true;
            vm.MaxPerOrder = "5";
            vm.ChannelID = "1";
            return vm;
        }

        private void ButtonPassAudit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_ViewModel.VerifyMemo))
            {
                //this.txtVerifyMemo.Validation("请输入审核理由");
                this.txtVerifyMemo.Validation(ResCountdownMaintain.Msg_AuditReason);
                return;
            }

            _ViewModel.Status = CountdownStatus.Ready;
            if (_ViewModel.IsPromotionSchedule)
            {
                if (!AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_AdvancedAuditPromotionSchedule_Check))
                {
                    _ViewModel.Status = CountdownStatus.WaitForVerify;
                }
            }
            else
            {
                if (!AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_AdvancedAuditCountDown_Check))
                {
                    _ViewModel.Status = CountdownStatus.WaitForVerify;
                }
            }
            
            _Facade.Verify(_ViewModel, (o, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    // Window.Alert("操作成功");
                    Window.Alert(ResCountdownMaintain.Msg_OperationSuccess);
                    Window.Refresh();
                }
            });


        }

        private void ButtonAuditFailure_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_ViewModel.VerifyMemo))
            {
                // this.txtVerifyMemo.Validation("请输入审核理由");
                this.txtVerifyMemo.Validation(ResCountdownMaintain.Msg_AuditReason);
                return;
            }
            _ViewModel.Status = CountdownStatus.VerifyFaild;
            _Facade.Verify(_ViewModel, (o, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    //Window.Alert("操作成功");
                    Window.Alert(ResCountdownMaintain.Msg_OperationSuccess);
                    Window.Refresh();
                }
            });

        }

        private void ButtonAbandon_Click(object sender, RoutedEventArgs e)
        {
            _Facade.Void(_ViewModel, (o, arg) =>
                {
                    if (!arg.FaultsHandle())
                    {
                        //Window.Alert("操作成功");
                        Window.Alert(ResCountdownMaintain.Msg_OperationSuccess);
                        Window.Refresh();
                    }
                });
        }

        private void ButtonInterupt_Click(object sender, RoutedEventArgs e)
        {
            _Facade.Stop(_ViewModel, (o, arg) =>
            {
                if (!arg.FaultsHandle())
                {
                    // Window.Alert("操作成功");
                    Window.Alert(ResCountdownMaintain.Msg_OperationSuccess);
                    Window.Refresh();
                }
            });
        }

        private void hlbCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInputIsPass())
                return;
            _Facade.GetGrossMargin(_ViewModel, (o, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                GrossMarginMsg msg = arg.Result;
                // tblGrossMargin.Text = string.Format("限时抢购毛利(含赠品、优惠券):{0} 限时抢购毛利(不含赠品、优惠券):{1}\r\n限时抢购毛利率(含赠品、优惠券): {2}% 限时抢购毛利率(不含赠品、优惠券): {3}%", 
                //msg.GrossMargin, msg.GrossMarginWithOutPointAndGift, msg.GrossMarginRate, msg.GrossMarginRateWithOutPointAndGift);
                spCheckResult.Visibility = Visibility.Visible;
                txtGrossMargin.Text = msg.GrossMargin.ToString();
                txtGrossMarginWithOutPointAndGift.Text = msg.GrossMarginWithOutPointAndGift.ToString();
                txtGrossMarginRate.Text = msg.GrossMarginRate.ToString();
                txtGrossMarginRateWithOutPointAndGift.Text = msg.GrossMarginRateWithOutPointAndGift.ToString();
                txtGiftSysNo.Text = msg.GiftSysNo.HasValue ? msg.GiftSysNo.Value.ToString() : "无";
                txtCouponSysNo.Text = msg.CouponSysNo.HasValue ? msg.CouponSysNo.Value.ToString() : "无";
                // tblGrossMargin.Text = string.Format(ResCountdownMaintain.Msg_CountdownGrossProfit, msg.GrossMargin, msg.GrossMarginWithOutPointAndGift, msg.GrossMarginRate, msg.GrossMarginRateWithOutPointAndGift, "\r\n");

            });
        }

        //private void cb24H_Click(object sender, RoutedEventArgs e)
        //{
        //    this.cbIsShowPriceInNotice.IsChecked = false;
        //}

        private void lstQuickTimes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).ItemsSource != null)
            {

                string valDateStr = string.Empty;
                string valStartTimeStr = string.Empty;
                string valEndTimeStr = string.Empty;
                string valStartDateStr = string.Empty;
                string valEndDateStr = string.Empty;

                valDateStr = ((sender as ComboBox).SelectedItem as CodeNamePair).Name;
                if (!string.IsNullOrEmpty(valDateStr) && valDateStr.Split('-').Count() > 1)
                {
                    valStartTimeStr = valDateStr.Split('-')[0];
                    valEndTimeStr = valDateStr.Split('-')[1];
                    valStartDateStr = (dtBeginDateTime.SelectedDateTime == null ? DateTime.Now : dtBeginDateTime.SelectedDateTime.Value).Date.ToString("yyyy-MM-dd");
                    valEndDateStr = (dtEndDateTime.SelectedDateTime == null ? DateTime.Now : dtEndDateTime.SelectedDateTime.Value).Date.ToString("yyyy-MM-dd");

                    dtBeginDateTime.SelectedDateTime = DateTime.Parse(valStartDateStr + " " + valStartTimeStr);
                    dtEndDateTime.SelectedDateTime = DateTime.Parse(valEndDateStr + " " + valEndTimeStr);
                }
            }
        }

        private void ButtonFormClear_Click(object sender, RoutedEventArgs e)
        {
            var dataTmp = this.lstQuickTimes.ItemsSource;
            this.DataContext = this.InitParam(true);
            this.dgdProductDetail.ItemsSource = null;
            this.lstQuickTimes.ItemsSource = null;
            this.lstQuickTimes.ItemsSource = dataTmp;
        }
        /// <summary>
        /// check Stock
        /// </summary>
        /// <returns></returns>
        private bool CheckStock()
        {
            var data = (from p in _ViewModel.AffectedStockList where Convert.ToInt32(p.Qty) > p.TotalQty select p).ToList();
            if (data.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in data)
                {
                    //sb.AppendFormat("{0}预留仓库不能大于总库存!\r", item.StockName);
                    sb.AppendFormat(ResCountdownMaintain.Info_CanntMoreThanStock, item.StockName);
                }
               
                Window.MessageBox.Show(sb.ToString(),Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Error);
                return false;
            }
            return true;
        }

        private void LatestPurchaseDate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_ViewModel.ProductSysNo) && !string.IsNullOrEmpty(_ViewModel.ProductID))
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/PurchaseOrderItemHistory/{0}|{1}", _ViewModel.ProductSysNo,_ViewModel.ProductID), null, true);
            }
        }

    }
    
}
