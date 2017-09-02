using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT.Promotion;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Models.Promotion
{
    public class CountdownInfoVM : ModelBase
    {


        private string m_ProductSysNo;    // 商品系统编号
        private string m_ProductID;     // 商品ID
        private string m_ChannelID;     // 所属渠道ID
        private WebChannel m_Channel;   // 渠道
        private string m_CompanyCode;  // 公司系统编号
        private string m_PromotionTitle; // 活动描述
        private DateTime? m_StartTime;  // 开始日期
        private DateTime? m_EndTime;    // 结束日期
        private string m_MaxPerOrder; // 每单限购数量
        private string m_CountDownCurrentPrice;// 促销价格
        private string m_CountDownCashRebate;  // 促销活动赠送优惠券数量 
        private string m_CountDownPoint;  // 促销活动赠送积分数量 
        private string m_Reasons;  // 开展此次促销活动的理由 
        private string m_VerifyMemo;  // 审核批注
        private CountdownStatus? m_Status;   // 状态

        private bool? m_IsTodaySpecials;    // 是否在今日特价位置显示
        private bool? m_IsSecondKill;    // 是否秒杀商品
        private bool? m_IsCountDownAreaShow;// 是否在限时抢购专区显示
        private bool? m_IsHomePageShow;     // 是否在首页限时抢购位置显示
        private bool? m_IsC1Show;       // 是否在一级分类页面显示
        private bool? m_IsC2Show;       // 是否在二级分类页面显示
        private bool? m_Is24hNotice;    // 是否在24小时预告中显示
        private bool? m_IsShowPriceInNotice;    // 在24小时预告中显示时是否显示商品的促销价格
        private bool? m_IsLimitedQty;     // 是否限量发售
        private bool? m_IsEndIfNoQty;     // 限量发售的时候，如果限制的数量卖完了，是否需要结束这个促销
        private bool? m_IsReservedQty;    // 是否预留库存
        private bool m_IsEmailNotify;    // 如果库存不足是否邮件通知
        private bool? m_IsCommitAudit;    // 是否提交审核
        private bool? m_IsAuditPassed;    // 是否审核通过

        private string m_AreaShowPriority;    // 在限时抢购专区限时的优先级
        private string m_HomePagePriority;    // 在首页限时抢购位置显示的优先级
        private string m_BaseLine;            // 需要邮件通知的警戒库存量 
        private string m_CountdownCount;      // 促销数量
        private string m_VendorName;            //所属商家
        private int m_VendorSysNo;                //商家编号

        public CountdownInfoVM()
        {
            StatusVisibility = false;
            m_IsTodaySpecials = false;
            m_IsSecondKill = false;
            m_IsCountDownAreaShow = false;
            m_IsHomePageShow = false;
            m_IsC1Show = false;
            m_IsC2Show = false;
            m_Is24hNotice = false;
            m_IsShowPriceInNotice = false;
            m_IsLimitedQty = true;
            m_IsEndIfNoQty = false;
            m_IsReservedQty = false;
            m_IsEmailNotify = false;
            m_IsCommitAudit = false;
            m_IsAuditPassed = false;
            m_IsReservedQty = false;
            AffectedStockList = new ObservableCollection<StockQtySettingVM>();
            m_CountDownPoint = "0";
            m_Channel = new WebChannel();
        }
        public int SysNo { get; set; }


        /// <summary>
        /// 商品系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string ProductSysNo
        {
            get { return m_ProductSysNo; }
            set { SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID
        {
            get { return m_ProductID; }
            set { SetValue("ProductID", ref m_ProductID, value); }
        }
        /// <summary>
        /// 所属渠道ID
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get { return m_ChannelID; }
            set { SetValue("ChannelID", ref m_ChannelID, value); }
        }

        public WebChannel WebChannel
        {
            get { return m_Channel; }
            set { SetValue("WebChannel", ref m_Channel, value); }
        }
        /// <summary>
        /// 公司系统编号
        /// </summary>
        public string CompanyCode
        {
            get { return m_CompanyCode; }
            set { SetValue("CompanyCode", ref m_CompanyCode, value); }
        }
        /// <summary>
        /// 活动描述
        /// </summary>
        public string PromotionTitle
        {
            get { return m_PromotionTitle; }
            set { SetValue("PromotionTitle", ref m_PromotionTitle, value); }
        }
        /// <summary>
        /// 开始日期
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? StartTime
        {
            get { return m_StartTime; }
            set { SetValue("StartTime", ref m_StartTime, value); }
        }
        /// <summary>
        /// 结束日期
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? EndTime
        {
            get { return m_EndTime; }
            set { SetValue("EndTime", ref m_EndTime, value); }
        }
        /// <summary>
        /// 每单限购数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessage = "请输入大于等于0的整数！")]
        public string MaxPerOrder
        {
            get { return m_MaxPerOrder; }
            set { SetValue("MaxPerOrder", ref m_MaxPerOrder, value); }
        }
        /// <summary>
        /// 促销价格
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessage = "请输入0至99999999.99的小数！")]
        public string CountDownCurrentPrice
        {
            get { return m_CountDownCurrentPrice; }
            set { SetValue("CountDownCurrentPrice", ref m_CountDownCurrentPrice, value); }
        }
        /// <summary>
        /// 促销活动赠送优惠券数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessage = "请输入大于等于0的数字！")]
        public string CountDownCashRebate
        {
            get { return m_CountDownCashRebate; }
            set { SetValue("CountDownCashRebate", ref m_CountDownCashRebate, value); }
        }
        /// <summary>
        /// 促销活动赠送积分数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,7}$", ErrorMessage = "请输入大于等于0的整数！")]
        public string CountDownPoint
        {
            get { return m_CountDownPoint; }
            set { SetValue("CountDownPoint", ref m_CountDownPoint, value); }
        }
        /// <summary>
        /// 开展此次促销活动的理由
        /// </summary>
        public string Reasons
        {
            get { return m_Reasons; }
            set { SetValue("Reasons", ref m_Reasons, value); }
        }
        /// <summary>
        /// 审核批注
        /// </summary>
        public string VerifyMemo
        {
            get { return m_VerifyMemo; }
            set { SetValue("VerifyMemo", ref m_VerifyMemo, value); }
        }
        /// <summary>
        /// 是否在限时抢购专区显示
        /// </summary>
        public bool? IsCountDownAreaShow
        {
            get
            {
                if (IsPromotionSchedule)
                {
                    m_IsCountDownAreaShow = false;
                }
                return m_IsCountDownAreaShow;
            }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsCountDownAreaShow", ref m_IsCountDownAreaShow, value);
            }
        }
        /// <summary>
        /// 在限时抢购专区限时的优先级
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string AreaShowPriority
        {
            get { return m_AreaShowPriority; }
            set { SetValue("AreaShowPriority", ref m_AreaShowPriority, value); }
        }
        /// <summary>
        /// 是否在首页限时抢购位置显示
        /// </summary>
        public bool? IsHomePageShow
        {
            get { return m_IsHomePageShow; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsHomePageShow", ref m_IsHomePageShow, value);
            }
        }
        /// <summary>
        /// 在首页限时抢购位置显示的优先级
        /// </summary>
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Required)]
        public string HomePagePriority
        {
            get { return m_HomePagePriority; }
            set { SetValue("HomePagePriority", ref m_HomePagePriority, value); }
        }
        /// <summary>
        /// 是否在今日特价位置显示
        /// </summary>
        public bool? IsTodaySpecials
        {
            get { return m_IsTodaySpecials; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsTodaySpecials", ref m_IsTodaySpecials, value);
                if (value.Value)
                {
                    StartTime = DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString() + " 09:00:00");
                    EndTime = DateTime.Parse(DateTime.Now.AddDays(2).ToShortDateString() + " 09:00:00");
                }
            }
        }
        /// <summary>
        /// 是否秒杀商品
        /// </summary>
        public bool? IsSecondKill
        {
            get { return m_IsSecondKill; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsSecondKill", ref m_IsSecondKill, value);
            }
        }
        /// <summary>
        /// 是否在一级分类页面显示
        /// </summary>
        public bool? IsC1Show
        {
            get { return m_IsC1Show; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsC1Show", ref m_IsC1Show, value);
            }
        }
        /// <summary>
        /// 是否在二级分类页面显示 
        /// </summary>
        public bool? IsC2Show
        {
            get { return m_IsC2Show; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsC2Show", ref m_IsC2Show, value);
            }
        }
        /// <summary>
        /// 是否在24小时预告中显示
        /// </summary>
        public bool? Is24hNotice
        {
            get { return m_Is24hNotice; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("Is24hNotice", ref m_Is24hNotice, value);
            }
        }
        /// <summary>
        /// 在24小时预告中显示时是否显示商品的促销价格
        /// </summary>
        public bool? IsShowPriceInNotice
        {
            get { return m_IsShowPriceInNotice; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsShowPriceInNotice", ref m_IsShowPriceInNotice, value);
            }
        }
        /// <summary>
        /// 是否限量发售
        /// </summary>
        public bool? IsLimitedQty
        {
            get { return m_IsLimitedQty; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsLimitedQty", ref m_IsLimitedQty, value);


                if (!value.Value)
                {
                    IsEndIfNoQty = false;
                }



            }
        }
        /// <summary>
        /// 限量发售的时候，如果限制的数量卖完了，是否需要结束这个促销
        /// </summary>
        public bool? IsEndIfNoQty
        {
            get { return m_IsEndIfNoQty; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsEndIfNoQty", ref m_IsEndIfNoQty, value);
            }
        }
        /// <summary>
        /// 是否预留库存
        /// </summary>
        public bool? IsReservedQty
        {
            get { return m_IsReservedQty; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsReservedQty", ref m_IsReservedQty, value);
            }
        }
        /// <summary>
        /// 促销数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^([1-9]\d{0,7})$", ErrorMessage = "请输入大于0的整数！")]
        public string CountDownQty
        {
            get { return m_CountdownCount; }
            set { SetValue("CountDownQty", ref m_CountdownCount, value); }
        }
        /// <summary>
        /// 如果库存不足是否邮件通知
        /// </summary>
        public bool IsEmailNotify
        {
            get { return m_IsEmailNotify; }
            set
            {
                SetValue("IsEmailNotify", ref m_IsEmailNotify, value);
            }
        }
        /// <summary>
        /// 需要邮件通知的警戒库存量 
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,7}$", ErrorMessage = "请输入大于等于0的整数！")]
        public string BaseLine
        {
            get
            {
                if (IsEmailNotify)
                    return m_BaseLine;
                else
                    return "0";
            }
            set
            {
                SetValue("BaseLine", ref m_BaseLine, value);
                if (m_BaseLine != "-1")
                {
                    IsEmailNotify = true;
                }
                else
                    IsEmailNotify = false;
            }
        }
        /// <summary>
        /// 是否提交审核
        /// </summary>
        public bool? IsCommitAudit
        {
            get { return m_IsCommitAudit; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsCommitAudit", ref m_IsCommitAudit, value);
            }
        }
        /// <summary>
        /// 是否审核通过
        /// </summary>
        public bool? IsAuditPassed
        {
            get { return m_IsAuditPassed; }
            set
            {
                if (value == null)
                {
                    value = false;
                }
                SetValue("IsAuditPassed", ref m_IsAuditPassed, value);

                if (value == false)
                {
                    m_Status = CountdownStatus.VerifyFaild;
                }
            }
        }

        public bool IsSubmitAudit { get; set; }

        public CountdownStatus? Status
        {
            get { return m_Status; }
            set { SetValue("Status", ref m_Status, value); }
        }
        /// <summary>
        /// 类型 true:促销计划 false:限时抢购
        /// </summary>
        public bool IsPromotionSchedule { get; set; }

        public string IsPromotionScheduleStr
        {

            get
            {
                if (IsPromotionSchedule)
                    return "促销计划";
                else
                    return "限时抢购";
            }
        }

        private int _pmRole;

        /// <summary>
        /// PM权限
        /// </summary>
        public int PMRole
        {
            get
            {
                _pmRole = 0;
                if (AdvancedReadCountDown_Check())
                {
                    _pmRole = 2;
                }
                else if (PrimaryReadCountDown_Check())
                {
                    _pmRole = 1;
                }
                return _pmRole;
            }
            set { _pmRole = value; }
        }
        private bool PrimaryReadCountDown_Check()
        {
            return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_PrimaryReadCountDown_Check);
        }

        private bool AdvancedReadCountDown_Check()
        {
            return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_AdvancedReadCountDown_Check);
        }

        public int VendorSysNo
        {
            get { return m_VendorSysNo; }
            set { SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }
        public string VendorName
        {
            get { return m_VendorName; }
            set { SetValue("VendorName", ref m_VendorName, value); }
        }

        private ObservableCollection<StockQtySettingVM> _AffectedStockList;

        public ObservableCollection<StockQtySettingVM> AffectedStockList
        {
            get { return _AffectedStockList; }
            set { SetValue("AffectedStockList", ref _AffectedStockList, value); }
        }

        public Visibility ButtonNewVisibility
        {
            get
            {
                if (SysNo <= 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility ButtonSaveVisibility
        {
            get
            {
                if (SysNo <= 0 || (SysNo > 0 && Status == CountdownStatus.Ready && !IsPromotionSchedule) || Status == CountdownStatus.VerifyFaild || Status == CountdownStatus.WaitForVerify)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility ButtonSubmitAuditVisibility
        {
            get
            {
                if (SysNo > 0 && (Status == CountdownStatus.VerifyFaild || Status == CountdownStatus.Init ))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility ButtonPassAuditVisibility
        {
            get
            {
                if (SysNo > 0 && (Status == CountdownStatus.WaitForVerify || Status == CountdownStatus.WaitForPrimaryVerify))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility ButtonAbandonVisibility
        {
            get
            {
                if (SysNo > 0 && (Status == CountdownStatus.VerifyFaild || Status == CountdownStatus.Ready || Status == CountdownStatus.Init))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility ButtonInteruptVisibility
        {
            get
            {
                if (SysNo > 0 && Status == CountdownStatus.Running)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 设置状态是否显示
        /// </summary>
        private bool _statusVisibility = false;
        public bool StatusVisibility
        {
            get { return _statusVisibility; }
            set { _statusVisibility = value; }
        }

        #region 权限

        /// <summary>
        /// 判断是否有审核权限，控制UI审核按钮启动状态
        /// </summary>
        public bool ButtonPassAuditEnable
        {
            get
            {
                //判断是限时抢购还是促销计划
                if (IsPromotionSchedule)
                {
                    switch (Status)
                    {
                        case CountdownStatus.WaitForVerify:
                            return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_AdvancedAuditPromotionSchedule_Check);
                            break;
                        case CountdownStatus.WaitForPrimaryVerify:
                            return (AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_AdvancedAuditPromotionSchedule_Check) || AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_PrimaryAuditPromotionSchedule_Check));
                            break;
                    }
                }
                else
                {
                    switch (Status)
                    {
                        case CountdownStatus.WaitForVerify:
                            return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_AdvancedAuditCountDown_Check);
                            break;
                        case CountdownStatus.WaitForPrimaryVerify:
                            return (AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_AdvancedAuditCountDown_Check) || AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_PrimaryAuditCountDown_Check));
                            break;
                    }
                }
                return false;
            }
        }

        public bool HasCountdownUserRightOnAddOrUpdateMaintainPermission
        {
            //get 
            //{
            //    bool result = false;
            //    if(SysNo>0)
            //    {
            //        result = HasCountdownUserRightOnUpdateMaintainPermission;
            //    }
            //    else
            //    {
            //        result = HasCountdownUserRightOnAddMaintainPermission;
            //    }
            //    return result;
            //}
            get { return true; }
        }

        //新增
        public bool HasCountdownUserRightOnAddMaintainPermission
        {
            //get
            //{
            //    return IsPromotionSchedule ? AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_PromotionScheduleUserRightOnAdd_Check)
            //        : AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountdownUserRightOnAdd_Check);
            //}
            get { return true; }
        }
        //更新
        public bool HasCountdownUserRightOnUpdateMaintainPermission
        {
            //get
            //{
            //    return IsPromotionSchedule ? AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_PromotionScheduleUserRightOnUpdate_Check)
            //        : AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountdownUserRightOnUpdate_Check);
            //}
            get { return true; }
        }
        //审核
        public bool HasCountdownUserRightOnVerifyMaintainPermission
        {
            //get
            //{
            //    return IsPromotionSchedule ? AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_PromotionScheduleUserRightOnVerify_Check)
            //        : AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountdownUserRightOnVerify_Check);
            //}
            get { return true; }
        }
        //终止
        public bool HasCountdownUserRightOnInterruptMaintainPermission
        {
            //get
            //{
            //    return IsPromotionSchedule ? AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_PromotionScheduleUserRightOnInterrupt_Check)
            //        : AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountdownUserRightOnInterrupt_Check);
            //}
            get { return true; }
        }

        //提交审核
        public bool IsEnabledSubmitAudit
        {
            //get
            //{
            //    return IsPromotionSchedule ? AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_PromotionScheduleUserRightOnWaitForVerify_Check) : true;
            //}
            get { return true; }
        }


        public bool HasCountdownAreaShowMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountdownAreaShow_Check); }
        }

        public bool HasCountdownIsHomePageShowMaintainPermission
        { get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountdownIsHomePageShow_Check); } }

        public bool HasCountdownHasTodaySpecialMaintainPermission
        {
            get
            {
                return IsPromotionSchedule ? AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_PromotionSchedule_CountdownHasTodaySpecial_Check)
                    : AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountdownHasTodaySpecial_Check);
            }
        }

        //此功能已取消
        //public bool HasUserHasGroupOnRightMaintainPermission
        //{ get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_UserHasGroupOnRightCheck); } }

        #endregion
    }
    public class StockQtySettingVM : ModelBase
    {

        public string StockName { get; set; }
        public int? StockSysNo { get; set; }
        private int? availableQty;
        public int? AvailableQty
        {
            get { return availableQty + Convert.ToInt32(Qty); }
            set { availableQty = value; }
        }

        public int? ConsignQty { get; set; }
        public int? VirtualQty { get; set; }
        public int? TotalQty
        {
            get
            {
                if (AvailableQty.HasValue && ConsignQty.HasValue && VirtualQty.HasValue)
                    return AvailableQty.Value + ConsignQty.Value + VirtualQty.Value;
                else
                    return null;
            }
        }
        private string _Qty;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,7}$", ErrorMessage = "请输入大于等于0的整数！")]
        public string Qty
        {
            get { return _Qty; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = "0";
                }
                SetValue("Qty", ref _Qty, value);
            }
        }
    }

}
