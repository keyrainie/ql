using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Service.Utility;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CouponMaintain : PageBase
    {
        public CouponsInfoViewModel _couponsInfoVM = new CouponsInfoViewModel();
        public CouponsFacade _facade;
        private string _operationType = "";

        public CouponMaintain()
        {
            InitializeComponent();
          
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
         
            base.OnPageLoad(sender, e);

            _facade = new CouponsFacade(this, _couponsInfoVM);

            int tempsysNo = -1;
            int? couponsSysNo = null;

            if (this.Request.QueryString != null
                && this.Request.QueryString.ContainsKey("sysno")
                && int.TryParse(this.Request.QueryString["sysno"].ToString().Trim(), out tempsysNo))
            {
                couponsSysNo = tempsysNo;
                if (this.Request.QueryString.ContainsKey("operation")
                    && !string.IsNullOrEmpty(this.Request.QueryString["operation"].ToString().Trim()))
                {
                    _operationType = this.Request.QueryString["operation"].ToString().ToLower();
                }

            }
            if (_operationType == "edit")
            {

                //TODO：判断是否有编辑的权限，如果没有，则将自动变为view权限
            }
            else if (_operationType == "mgt")
            {
            }
            else
            {
            }

            _facade.Load(couponsSysNo, (obj, args) =>
            {
                _couponsInfoVM = args.Result;
                this.DataContext = _couponsInfoVM;
                _couponsInfoVM.ValidationErrors.Clear();
                InitControl();
            });

        }

        private void ucBasic_OnProductRangeTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDiscountTypeControl();
        }

        private void SetDiscountTypeControl()
        {
            ucDiscount.rdoDiscountFinal.IsEnabled = false;
            ucDiscount.rdoDiscountSubtract.IsEnabled = false;
            if (_couponsInfoVM.ProductRangeType == CouponsProductRangeType.LimitProduct)
            {
                if (_couponsInfoVM.ProductCondition.RelProducts != null
                    && _couponsInfoVM.ProductCondition.RelProducts.ProductList != null
                    && _couponsInfoVM.ProductCondition.RelProducts.ProductList.Count == 1)
                {
                    ucDiscount.rdoDiscountFinal.IsEnabled = true;
                    ucDiscount.rdoDiscountSubtract.IsEnabled = true;
                }
            }
        }

        private void InitControl()
        {
            SetControlByOperation();

            if (_couponsInfoVM.CouponChannelType.HasValue && _couponsInfoVM.CouponChannelType == CouponsMKTType.MKTPM)
            {
                this.ucBasic.tbEIMSSysno.IsReadOnly = false;
                this.ucBasic.tbEIMSSysno.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                this.ucBasic.tbEIMSSysno.IsReadOnly = true;
                this.ucBasic.tbEIMSSysno.Background = new SolidColorBrush(Colors.Transparent);
            }

            SetDiscountTypeControl();

            if (_couponsInfoVM.SysNo.HasValue)
            {
                if (_operationType == "edit")
                {
                    //this.Title = string.Format("优惠券[{0}]-编辑", _couponsInfoVM.SysNo);
                    this.Title = string.Format(ResCouponMaintain.Msg_CouponEdit, _couponsInfoVM.SysNo);
                }
                else if (_operationType == "mgt")
                {
                    //this.Title = string.Format("优惠券[{0}]-管理", _couponsInfoVM.SysNo);
                    this.Title = string.Format(ResCouponMaintain.Msg_CouponManage, _couponsInfoVM.SysNo);
                    if(_couponsInfoVM!=null&&_couponsInfoVM.ProductRangeType==CouponsProductRangeType.AllProducts)
                    {
                        tabItemProductCondition.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        tabItemProductCondition.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    //this.Title = string.Format("优惠券[{0}]-浏览", _couponsInfoVM.SysNo);
                    this.Title = string.Format(ResCouponMaintain.Msg_CouponView, _couponsInfoVM.SysNo);
                }
            }
            else
            {
                //this.Title = string.Format("优惠券-新增");
                this.Title = string.Format(ResCouponMaintain.Msg_CouponInsert);
            }
        }

        private void SetControlByOperation()
        {
            spFunction.Visibility = System.Windows.Visibility.Collapsed;
            Visibility buttonVisibility = System.Windows.Visibility.Visible;
            if (!_couponsInfoVM.SysNo.HasValue)
            { 
                SetTabItem(0);
                _couponsInfoVM.IsOnlyViewMode = false;
            }
            else
            {
                if (_operationType.ToLower() == "edit")
                {
                    SetTabItem(0);
                    _couponsInfoVM.IsOnlyViewMode = false;
                }
                else if (_operationType.ToLower() == "mgt")
                {
                    SetTabItem(99);
                    buttonVisibility = System.Windows.Visibility.Collapsed;
                    _couponsInfoVM.IsOnlyViewMode = true;
                    spFunction.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    //浏览模式
                    SetTabItem(99);
                    buttonVisibility = System.Windows.Visibility.Collapsed;
                    _couponsInfoVM.IsOnlyViewMode = true;
                }
            }

            List<Button> list = new List<Button>();          
            list.Add(btnPreStep);
            list.Add(btnPreStep2);
            list.Add(btnPreStep3);
            list.Add(btnPreStep4);
            list.Add(btnSaveActivityRule);
            list.Add(btnSaveBasic);
            list.Add(btnSaveCustomerCondition);
            list.Add(btnSaveDiscountRule);
            list.Add(btnSaveProductCondition);

            list.Add(ucCouponCode.btnPreStep);
            list.Add(ucCouponCode.btnAddNewCode);
            list.Add(ucCouponCode.btnDeleteAllCode);
            list.Add(ucCouponCode.btnDeleteBachCode);

            if (list.Count > 0)
            {
                foreach(Button btn in list)
                {
                    btn.Visibility = buttonVisibility;
                }
            }
             
            if (_couponsInfoVM.IsOnlyViewMode)
            {
                OperationControlStatusHelper.SetControlsStatus(ucBasic.expanderCategory, true);
            }

            if (_operationType.ToLower() == "mgt")
            {
                btnSubmitAudit.IsEnabled = false;
                btnCancelAudit.IsEnabled = false;
                btnAuditApprove.IsEnabled = false;
                btnAuditRefuse.IsEnabled = false;
                btnStop.IsEnabled = false;
                btnVoid.IsEnabled = false;
                switch (_couponsInfoVM.Status.Value)
                {
                    case CouponsStatus.Init:
                        btnSubmitAudit.IsEnabled = true;
                        btnVoid.IsEnabled = true;
                        break;
                    case CouponsStatus.WaitingAudit:
                        btnCancelAudit.IsEnabled = true;
                        if (_couponsInfoVM.HasCouponCodeApprovePermission)
                        {
                            btnAuditApprove.IsEnabled = true;
                            btnAuditRefuse.IsEnabled = true;
                        }
                        btnVoid.IsEnabled = true;
                        break;
                    case CouponsStatus.Ready:                        
                        btnVoid.IsEnabled = true;
                        break;
                    case CouponsStatus.Run:
                        if (_couponsInfoVM.HasCouponCodeStopApprovePermission)
                        {
                            btnStop.IsEnabled = true;
                        }
                        break;
                    default:
                        break;
                }
 
            }            
        }

        private void SetTabItem(int afterStepNo)
        {
            tabItemBasic.Visibility = Visibility.Collapsed;
            tabItemProductCondition.Visibility = Visibility.Collapsed;
            tabItemDiscountRule.Visibility = Visibility.Collapsed;
            tabItemActivityRule.Visibility = Visibility.Collapsed;
            tabItemCustomer.Visibility = Visibility.Collapsed;
            tabItemCouponCode.Visibility = Visibility.Collapsed;
            if (afterStepNo == 0)
            {
                tabItemBasic.Visibility = Visibility.Visible;
            }
            else if (afterStepNo == 1)
            {
                tabItemProductCondition.Visibility = Visibility.Visible;
            }
            else if (afterStepNo == 2)
            {
                tabItemDiscountRule.Visibility = Visibility.Visible;
            }
            else if (afterStepNo == 3)
            {
                tabItemActivityRule.Visibility = Visibility.Visible;
            }
            else if (afterStepNo == 4)
            {
                if (_couponsInfoVM.BindCondition == CouponsBindConditionType.None || _couponsInfoVM.BindCondition == CouponsBindConditionType.SO)
                {
                    ucCustomerRange.expUser.Visibility = Visibility.Visible;
                }
                else
                {
                    ucCustomerRange.expUser.Visibility = Visibility.Collapsed;
                }
                tabItemCustomer.Visibility = Visibility.Visible;
            }
            else if (afterStepNo == 5)
            {
                tabItemCouponCode.Visibility = Visibility.Visible;
            }
            else if (afterStepNo > 5)
            {
                tabItemBasic.Visibility = Visibility.Visible;
                tabItemProductCondition.Visibility = Visibility.Visible;
                tabItemDiscountRule.Visibility = Visibility.Visible;
                tabItemActivityRule.Visibility = Visibility.Visible;
                tabItemCustomer.Visibility = Visibility.Visible;
                tabItemCouponCode.Visibility = Visibility.Visible;
            }
        }        

        private void btnSaveBasic_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.ucBasic.gridUCBasicLayout))
            {
                return;
            }            

            _facade.SaveMaster((obj, args) =>
                {

                    _couponsInfoVM = args.Result;
                    this.DataContext = _couponsInfoVM;
                    _operationType = "edit";
                    InitControl();
                    
                    if (_couponsInfoVM.ProductRangeType == CouponsProductRangeType.AllProducts)
                    {
                        SetTabItem(2);
                        tabGroup.SelectedIndex = 2;
                    }
                    else
                    {
                        SetTabItem(1);
                        tabGroup.SelectedIndex = 1;
                        if (_couponsInfoVM.ProductRangeType == CouponsProductRangeType.LimitCategoryBrand)
                        {
                            ucProductRange.expandeVendor.Visibility = Visibility.Visible;
                            ucProductRange.expanderCategory.Visibility = System.Windows.Visibility.Visible;
                            ucProductRange.expanderBrand.Visibility = System.Windows.Visibility.Visible;
                            ucProductRange.expanderProduct.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            ucProductRange.expandeVendor.Visibility = Visibility.Collapsed;
                            ucProductRange.expanderCategory.Visibility = System.Windows.Visibility.Collapsed;
                            ucProductRange.expanderBrand.Visibility = System.Windows.Visibility.Collapsed;
                            ucProductRange.expanderProduct.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                                        
                });
        }

        private void btnPreStep_Click(object sender, RoutedEventArgs e)
        {
            if (tabGroup.SelectedIndex == 2)
            {
                if (_couponsInfoVM.ProductRangeType == CouponsProductRangeType.AllProducts)
                {
                    SetTabItem(0);
                    tabGroup.SelectedIndex = 0;
                }
                else
                {
                    SetTabItem(1);
                    tabGroup.SelectedIndex = 1;
                }
            }
            else
            {
                SetTabItem(tabGroup.SelectedIndex - 1);
                tabGroup.SelectedIndex = tabGroup.SelectedIndex - 1;
            }
        }
        
        private void btnSaveProductCondition_Click(object sender, RoutedEventArgs e)
        {

            SetSaveToNextStep();
        }

        private void btnSaveDiscountRule_Click(object sender, RoutedEventArgs e)
        {

            SetSaveToNextStep();
        }

        private void btnSaveActivityRule_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.ucRule.gridUCRuleLayout);
            if (_couponsInfoVM.HasValidationErrors)
            {
                return;
            }

            decimal minAmount=-1.00m;
            decimal.TryParse(this.ucRule.tbMinAmount.Text.Trim(), out minAmount);
            if (minAmount < 0)
            {
               // Window.Alert("必须设置订单金额下限！并且为非负数！");
                Window.Alert(ResCouponMaintain.Msg_SetAmountFloor);
                return;
            }
            if (_couponsInfoVM.IsAutoBinding.Value && !_couponsInfoVM.BindingDate.HasValue)
            {
               // Window.Alert("当选择定时发放时，必须设置发放日期！");
                Window.Alert(ResCouponMaintain.Msg_SetIssueDate);
                return;
            }

            if (_couponsInfoVM.ValidPeriod.Value == CouponsValidPeriodType.CustomPeriod)
            {
                if (!_couponsInfoVM.CustomBindBeginDate.HasValue || !_couponsInfoVM.CustomBindEndDate.HasValue)
                {
                   // Window.Alert("如果发放的优惠券有效期为自定义，必须设置有效期的起止时间！");
                    Window.Alert(ResCouponMaintain.Msg_SetValidStartEndTime);
                    return;
                }

                if (_couponsInfoVM.BindingDate.HasValue && _couponsInfoVM.CustomBindEndDate.HasValue)
                {
                    if (_couponsInfoVM.BindingDate.Value > _couponsInfoVM.CustomBindEndDate.Value)
                    {
                       // Window.Alert("发放优惠券的日期不能大于优惠券的有效结束日期！");
                        Window.Alert(ResCouponMaintain.Msg_IssueDateLessEndDate);
                        return;
                    }
                }
            }
            if (_couponsInfoVM.ValidPeriod != CouponsValidPeriodType.CustomPeriod)
            {
                _couponsInfoVM.CustomBindBeginDate = null;
                _couponsInfoVM.CustomBindEndDate = null;
            }


            SetSaveToNextStep();
        }

        private void btnSaveCustomerCondition_Click(object sender, RoutedEventArgs e)
        {
            
            SetSaveToNextStep();
        }

        private void SetSaveToNextStep()
        {
            switch (tabGroup.SelectedIndex)
            {
                case 1:
                    _facade.SetProductCondition((obj, args) =>
                        {
                            SetTabItem(tabGroup.SelectedIndex + 1);
                            tabGroup.SelectedIndex = tabGroup.SelectedIndex + 1;

                            SetDiscountTypeControl();
                        }
                    );
                    break;
                case 2:
                    _facade.SetDiscountRule((obj, args) =>
                        {

                            SetTabItem(tabGroup.SelectedIndex + 1);
                            tabGroup.SelectedIndex = tabGroup.SelectedIndex + 1;
                        }
                    );
                    break;
                case 3:
                    _facade.SetSaleRuleEx((obj, args) =>
                    {
                        SetTabItem(tabGroup.SelectedIndex + 1);
                        tabGroup.SelectedIndex = tabGroup.SelectedIndex + 1;
                    }
                   );
                    break;
                case 4:
                    _facade.SetCustomerCondition((obj, args) =>
                    {
                        SetTabItem(tabGroup.SelectedIndex + 1);
                        tabGroup.SelectedIndex = tabGroup.SelectedIndex + 1;
                    }
                  );
                    break;
                default:
                    break;
            }

        }

        private void RefreshDataGrid()
        {
            ucProductRange.dgVendor.ItemsSource = _couponsInfoVM.ProductCondition.ListRelVendorViewModel;
            ucProductRange.dgVendor.Bind();
            ucProductRange.dgBrand.ItemsSource = _couponsInfoVM.ProductCondition.RelBrands.BrandList;
            ucProductRange.dgBrand.Bind();

            ucProductRange.dgCategory.ItemsSource = _couponsInfoVM.ProductCondition.RelCategories.CategoryList;
            ucProductRange.dgCategory.Bind();

            ucProductRange.dgProduct.ItemsSource = _couponsInfoVM.ProductCondition.RelProducts.ProductList;
            ucProductRange.dgCategory.Bind();

            ucDiscount.dgDiscountAmount.ItemsSource = _couponsInfoVM.OrderAmountDiscountRule.OrderAmountDiscountRank;
            ucDiscount.dgDiscountProduct.ItemsSource = _couponsInfoVM.PriceDiscountRule;

            ucCustomerRange.dgArea.ItemsSource = _couponsInfoVM.CustomerCondition.RelAreas.AreaList;
            ucCustomerRange.dgCustomerID.ItemsSource = _couponsInfoVM.CustomerCondition.RelCustomers.CustomerIDList;
            ucCustomerRange.dgCustomerRank.ItemsSource = _couponsInfoVM.CustomerCondition.RelCustomerRanks.CustomerRankList;
        }

        private void btnSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
           // CouponsProcess(PSOperationType.SubmitAudit, "提交审核", CouponsStatus.WaitingAudit);
            CouponsProcess(PSOperationType.SubmitAudit, ResCouponMaintain.Msg_SubAudit, CouponsStatus.WaitingAudit);
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
           // CouponsProcess(PSOperationType.CancelAudit, "撤销审核", CouponsStatus.Init);
            CouponsProcess(PSOperationType.CancelAudit, ResCouponMaintain.Msg_CancelAudit, CouponsStatus.Init);
        }

        private void btnAuditApprove_Click(object sender, RoutedEventArgs e)
        {
           // CouponsProcess(PSOperationType.AuditApprove, "审核通过", CouponsStatus.Ready);
            CouponsProcess(PSOperationType.AuditApprove, ResCouponMaintain.Msg_ApproveAudit, CouponsStatus.Ready);
        }

        private void btnAuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            //CouponsProcess(PSOperationType.AuditRefuse, "审核拒绝", CouponsStatus.Init);
            CouponsProcess(PSOperationType.AuditRefuse, ResCouponMaintain.Msg_RefuseAudit, CouponsStatus.Init);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
           // CouponsProcess(PSOperationType.Stop, "中止", CouponsStatus.Stoped);
            CouponsProcess(PSOperationType.Stop, ResCouponMaintain.Msg_StopAudit, CouponsStatus.Stoped);
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            //CouponsProcess(PSOperationType.Void, "作废", CouponsStatus.Void);
            CouponsProcess(PSOperationType.Void, ResCouponMaintain.Msg_VoidAudit, CouponsStatus.Void);
        }

        private void CouponsProcess(PSOperationType opt, string optname, CouponsStatus targetStatus)
        {
            List<int?> sysNoList = new List<int?>();
            sysNoList.Add(_couponsInfoVM.SysNo);

            _facade.BatchProcessCoupons(sysNoList, opt, (obj, args) =>
            {
                if (args.Result.FailureRecords.Count == 0)
                {
                    _couponsInfoVM.Status = targetStatus;
                    SetControlByOperation();
                    Window.Alert(string.Format(ResCouponMaintain.Msg_DealSuccess, optname));
                }
                else
                {
                    string msg = args.Result.FailureRecords.Join("\r\n") + Environment.NewLine;                    
                    Window.Alert(msg);
                }
                
            });

        }
       
    }
}
