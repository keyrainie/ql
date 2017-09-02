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
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Models.Promotion.QueryFilter
{
    public class CountdownQueryFilterVM : ModelBase
    {
        #region FilterFields

        /// <summary>
        /// 是否促销计划(促销类型)
        /// </summary>
        private int? m_IsPromotionSchedule;
        public int? IsPromotionSchedule
        {
            get { return m_IsPromotionSchedule; }
            set { SetValue("IsPromotionSchedule", ref m_IsPromotionSchedule, value); }
        }

        /// <summary>
        /// 所属渠道
        /// </summary>
        private string m_ChannelID;
        public string ChannelID
        {
            get { return m_ChannelID; }
            set { SetValue("ChannelID", ref m_ChannelID, value); }
        }

        /// <summary>
        /// 促销计划描述
        /// </summary>
        public string m_promotionTitle;
        public string PromotionTitle 
        {
            get { return m_promotionTitle; }
            set { SetValue("PromotionTitle", ref m_promotionTitle, value); }
        }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        private int? m_MerchantSysNo;
        public int? MerchantSysNo
        {
            get { return m_MerchantSysNo; }
            set { SetValue("MerchantSysNo", ref m_MerchantSysNo, value); }
        }

        /// <summary>
        /// 创建时间区间从
        /// </summary>
        private DateTime? m_CreateFromTime;
        public DateTime? CreateFromTime
        {
            get { return m_CreateFromTime; }
            set { SetValue("CreateFromTime", ref m_CreateFromTime, value); }
        }
        /// <summary>
        /// 创建时间区间到
        /// </summary>
        private DateTime? m_CreateToTime;
        public DateTime? CreateToTime
        {
            get { return m_CreateToTime; }
            set { SetValue("CreateToTime", ref m_CreateToTime, value); }
        }

        /// <summary>
        /// 促销开始时间区间从
        /// </summary>
        private DateTime? m_CountdownFromStartTime;
        public DateTime? CountdownFromStartTime
        {
            get { return m_CountdownFromStartTime; }
            set { SetValue("CountdownFromStartTime", ref m_CountdownFromStartTime, value); }
        }
        /// <summary>
        /// 促销开始时间区间到
        /// </summary>
        private DateTime? m_CountdownToStartTime;
        public DateTime? CountdownToStartTime
        {
            get { return m_CountdownToStartTime; }
            set { SetValue("CountdownToStartTime", ref m_CountdownToStartTime, value); }
        }

        /// <summary>
        /// 促销结束时间区间从
        /// </summary>
        private DateTime? m_CountdownFromEndTime;
        public DateTime? CountdownFromEndTime
        {
            get { return m_CountdownFromEndTime; }
            set { SetValue("CountdownFromEndTime", ref m_CountdownFromEndTime, value); }
        }
        /// <summary>
        /// 促销结束时间区间到
        /// </summary>
        private DateTime? m_CountdownToEndTime;
        public DateTime? CountdownToEndTime
        {
            get { return m_CountdownToEndTime; }
            set { SetValue("CountdownToEndTime", ref m_CountdownToEndTime, value); }
        }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        private string m_ProductSysNo;
        public string ProductSysNo
        {
            get { return m_ProductSysNo; }
            set { SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        private string m_ProductID;
        public string ProductID
        {
            get { return m_ProductID; }
            set { SetValue("ProductID", ref m_ProductID, value); }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        private string m_CreateUserName;
        public string CreateUserName
        {
            get { return m_CreateUserName; }
            set { SetValue("CreateUserName", ref m_CreateUserName, value); }
        }

        /// <summary>
        /// 单据状态
        /// </summary>
        private CountdownStatus? m_Status;
        public CountdownStatus? Status
        {
            get { return m_Status; }
            set { SetValue("Status", ref m_Status, value); }
        }

        /// <summary>
        /// 是否组团
        /// </summary>
        private bool? m_IsGroupOn = null;
        public bool? IsGroupOn
        {
            get { return m_IsGroupOn; }
            set { SetValue("IsGroupOn", ref m_IsGroupOn, value); }
        }
        private bool? m_IsGroupOnAll = true;
        public bool? IsGroupOnAll
        {
            get { return m_IsGroupOnAll; }
            set
            {
                if (value.Value)
                {
                    m_IsGroupOn = null;
                }
            }
        }

        /// <summary>
        /// 限时抢购专区
        /// </summary>
        private bool? m_IsCountDownAreaShow = false;
        public bool? IsCountDownAreaShow
        {
            get { return m_IsCountDownAreaShow; }
            set
            {
                SetValue("IsCountDownAreaShow", ref m_IsCountDownAreaShow, value);
                IsCountDownAreaShowVal = value.Value ? 1 : 0;
            }
        }
        public int? IsCountDownAreaShowVal { get; set; }

        public int? IsSecondKill { get; set; }

        /// <summary>
        /// 首页限时抢购
        /// </summary>
        private bool? m_IsHomePageShow = false;
        public bool? IsHomePageShow
        {
            get { return m_IsHomePageShow; }
            set
            {
                SetValue("IsHomePageShow", ref m_IsHomePageShow, value);
                IsHomePageShowVal = value.Value ? 1 : 0;
            }
        }

        public int? IsHomePageShowVal { get; set; }

        /// <summary>
        /// 分类页面显示
        /// </summary>
        private bool? m_IsShowCategory = null;
        public bool? IsShowCategory
        {
            get { return m_IsShowCategory; }
            set { SetValue("IsShowCategory", ref m_IsShowCategory, value); }
        }

        private bool? m_IsShowCategoryAll = true;
        public bool? IsShowCategoryAll
        {
            get { return m_IsShowCategoryAll; }
            set { SetValue("IsShowCategoryAll", ref m_IsShowCategoryAll, value); }
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

        public string PMUserName { get; set; }

        #endregion

        private ObservableCollection<CountdownQueryResultVM> _CountdownList;

        public ObservableCollection<CountdownQueryResultVM> CountdownList
        {
            get { return _CountdownList; }
            set
            {
                SetValue("CountdownList", ref  _CountdownList, value);
            }
        }
    }
    public class CountdownQueryResultVM : ModelBase
    {
        public int SysNo { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public CountdownStatus Status { get; set; }
        public int IsSecondKill { get; set; }
        public int IsCountDownAreaShow { get; set; }
        public int? AreaShowPriority { get; set; }
        public int IsLimitedQty { get; set; }
        public int IsReservedQty { get; set; }
        public int IsHomePageShow { get; set; }
        public int? HomeShowPriority { get; set; }
        public string PromotionType
        {
            get
            {
                if (IsPromotionSchedule)
                    return ResCountdownQuery.Msg_PromotionPlan;
                else
                    return ResCountdownQuery.Msg_Countdown;
            }
        }
        public bool IsPromotionSchedule { get; set; }
        public string PromotionTitle { get; set; }
        public string Kind { get; set; }
        public int IsC1Show { get; set; }
        public int IsC2Show { get; set; }
        public string SalesVolume { get; set; }
        public decimal CountDownTotal { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CountDownCurrentPrice { get; set; }
        public string CountDownCashRebate { get; set; }
        public string CountDownPoint { get; set; }
        public string SnapShotCurrentPrice { get; set; }
        public string SnapShotCashRebate { get; set; }
        public string SnapShotPoint { get; set; }
        public decimal BasicPrice { get; set; }
        public string DisplayName { get; set; }
        public DateTime? CreateTime { get; set; }
        public string EditUser { get; set; }
        public DateTime? EditDate { get; set; }
        public string AuditUser { get; set; }
        public DateTime? AuditDate { get; set; }
        public string VendorName { get; set; }

        public bool hlbEditEnable
        {
            get
            {
                //return Status == CountdownStatus.Ready && !IsPromotionSchedule && HasCountdownUpdateAfterVerifyPermission;
                return true;
            }
        }
        public bool hlbMgtEnable
        {
            get
            {
                //return !(Status == CountdownStatus.Interupt || Status == CountdownStatus.Finish || Status == CountdownStatus.Abandon)
                   // && HasCountdownUpdatePermission;
                return true;
            }

        }
        public bool hlbOpMgtEnable
        {
            get
            {
                //return HasCountdownVerifyPermission || HasCountdownInterruptPermission;
                return true;
            }

        }

        #region 权限
        //审核未通过，产品促销初始
        public bool HasCountdownUpdatePermission
        { get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountDownUpdate_Check); } }
        //就绪
        public bool HasCountdownUpdateAfterVerifyPermission
        { get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountDownUpdateAfterVerify_Check); } }
        //待审核
        public bool HasCountdownVerifyPermission
        { get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountDownVerify_Check); } }
        //中止
        public bool HasCountdownInterruptPermission
        { get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Countdown_CountDownInterrupt_Check); } }

        #endregion

        public string CreateUserName { get; set; }
    }
}
