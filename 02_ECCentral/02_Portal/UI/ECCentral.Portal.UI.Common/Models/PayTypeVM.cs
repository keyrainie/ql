using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.Common.Models
{
    public class PayTypeVM : ModelBase
    {
        public PayTypeVM()
        {
            this.ListIsOnLineShow = EnumConverter.GetKeyValuePairs<HYNStatus>();
            this.ListIsPayWhenRev = EnumConverter.GetKeyValuePairs<SYNStatus>();
            this.ListIsNet = EnumConverter.GetKeyValuePairs<SYNStatus>();
            this.ListNetPayType = EnumConverter.GetKeyValuePairs<NetPayType>();
            this.PayRate = "0";
        }

        /// <summary>
        /// List区域
        /// </summary>
        public List<KeyValuePair<HYNStatus?, string>> ListIsOnLineShow { get; set; }
        public List<KeyValuePair<SYNStatus?, string>> ListIsPayWhenRev { get; set; }
        public List<KeyValuePair<SYNStatus?, string>> ListIsNet { get; set; }
        public List<KeyValuePair<NetPayType?, string>> ListNetPayType { get; set; }

        public int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }

        public string _payTypeID;
        /// <summary>
        /// 支付方式编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string PayTypeID
        {
            get { return _payTypeID; }
            set { SetValue("PayTypeID", ref _payTypeID, value); }
        }

        private string _payTypeName;
        /// <summary>
        /// 支付方式名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string PayTypeName
        {
            get { return _payTypeName; }
            set { SetValue("PayTypeName", ref _payTypeName, value); }
        }

        private string _payTypeDesc;
        /// <summary>
        /// 支付方式描述
        /// </summary>
        public string PayTypeDesc
        {
            get { return _payTypeDesc; }
            set { SetValue("PayTypeDesc", ref _payTypeDesc, value); }
        }

        private string _payRate;
        /// <summary>
        /// 手续费率 [Validate(ValidateType.Regex, @"^(0|1|1\.[0]*|0?\.(?!0+$)[\d]+)$")]
        /// </summary>
        [Validate(ValidateType.Required)]
        public string PayRate
        {
            get { return _payRate; }
            set { SetValue("PayRate", ref _payRate, value); }
        }

        private string m_Period;
        /// <summary>
        /// 到账周期
        /// </summary>
        public string Period
        {
            get { return this.m_Period; }
            set { this.SetValue("Period", ref m_Period, value); }
        }

        private string m_PaymentPage;
        /// <summary>
        /// 支付页面
        /// </summary>
        public string PaymentPage
        {
            get { return this.m_PaymentPage; }
            set { this.SetValue("PaymentPage", ref m_PaymentPage, value); }
        }

        private SYNStatus? m_IsPayWhenRecvEnum;
        /// <summary>
        /// 是否货到付款
        /// </summary>
        public SYNStatus? IsPayWhenRecvEnum
        {
            get { return this.m_IsPayWhenRecvEnum; }
            set { this.SetValue("IsPayWhenRecvEnum", ref m_IsPayWhenRecvEnum, value); }
        }

        private SYNStatus? m_IsNetEnum;
        /// <summary>
        /// 是否在线支付
        /// </summary>
        public SYNStatus? IsNetEnum
        {
            get { return this.m_IsNetEnum; }
            set { this.SetValue("IsNetEnum", ref m_IsNetEnum, value); }
        }

        private HYNStatus? m_IsOnlineShow;
        /// <summary>
        /// 是否前台显示
        /// </summary>
        public HYNStatus? IsOnlineShow
        {
            get { return this.m_IsOnlineShow; }
            set { this.SetValue("IsOnlineShow", ref m_IsOnlineShow, value); }
        }

        private string m_OrderNumber;
        /// <summary>
        /// 显示优先级
        /// </summary>
        public string OrderNumber
        {
            get { return this.m_OrderNumber; }
            set { this.SetValue("OrderNumber", ref m_OrderNumber, value); }
        }

        private NetPayType? m_NetPayType;
        /// <summary>
        /// 网上支付类型
        /// </summary>
        public NetPayType? NetPayType
        {
            get { return m_NetPayType; }
            set { this.SetValue("NetPayType", ref m_NetPayType, value); }
        }

        #region 扩展属性

        private bool isEdit;
        /// <summary>
        /// 用于界面控制是否编辑状态
        /// </summary>
        public bool IsEdit
        {
            get { return isEdit; }
            set { SetValue("IsEdit", ref isEdit, value); }
        }

        private bool isNetPay;
        /// <summary>
        /// 用于控制支付类型的 显示|隐藏
        /// </summary>
        public bool IsNetPay
        {
            get { return isNetPay; }
            set { SetValue("IsNetPay", ref isNetPay, value); }
        }
        #endregion

    }
}
