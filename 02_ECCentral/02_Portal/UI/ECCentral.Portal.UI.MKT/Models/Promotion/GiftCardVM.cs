using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GiftCardVM: ModelBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        public int? BindingCustomerSysNo { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        private ECCentral.BizEntity.IM.GiftCardStatus? status;
        public ECCentral.BizEntity.IM.GiftCardStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private ECCentral.BizEntity.MKT.YNStatus? isHold;
        public ECCentral.BizEntity.MKT.YNStatus? IsHold
        {
            get { return isHold; }
            set { base.SetValue("IsHold", ref isHold, value); }
        }

        /// <summary>
        /// 礼品卡类型
        /// </summary>
        private ECCentral.BizEntity.IM.GiftCardType? cardType;
        public ECCentral.BizEntity.IM.GiftCardType? CardType
        {
            get { return cardType; }
            set { base.SetValue("CardType", ref cardType, value); }
        }

        /// <summary>
        /// 面值
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal? AvailAmount { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        ///到期时间开始于
        /// </summary>
        private DateTime? endDateFrom;
        public DateTime? EndDateFrom
        {
            get { return endDateFrom; }
            set { base.SetValue("EndDateFrom", ref endDateFrom, value); }
        }

        /// <summary>
        /// 到期时间结束于
        /// </summary>
        private DateTime? endDateTo;
        public DateTime? EndDateTo
        {
            get { return endDateTo; }
            set { base.SetValue("EndDateTo", ref endDateTo, value); }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime? beginDate;
        public DateTime? BeginDate
        {
            get { return beginDate; }
            set { base.SetValue("BeginDate", ref beginDate, value); }
        }

        /// <summary>
        /// 到期时间
        /// </summary>
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { base.SetValue("EndDate", ref endDate, value); }
        }

        private DateTime? preEndDate;
        public DateTime? PreEndDate
        {
            get { return preEndDate; }
            set { base.SetValue("PreEndDate", ref preEndDate, value); }
        }

        /// <summary>
        /// 渠道编号
        /// </summary>
        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set
            {
                base.SetValue("ChannelID", ref channelID, value);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                return CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            }
        }
        
        public int? Type { get; set; }

        /// <summary>
        /// 是否过期
        /// </summary>
        public int? PeriodAdjustCount { get; set; }
        
        /// <summary>
        /// 订单号
        /// </summary>
        private string soSysNo;
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get { return soSysNo; }
            set { base.SetValue("SOSysNo", ref soSysNo, value); }
        }

        /// <summary>
        /// 客户编号
        /// </summary>
        private string customerSysNo;
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set { base.SetValue("CustomerSysNo", ref customerSysNo, value); }
        }

        private string customerID;
        public string CustomerID
        {
            get { return customerID; }
            set { base.SetValue("CustomerID", ref customerID, value); }
        }

        /// <summary>
        /// 拼接用户名称头
        /// </summary>
        private string showCustomer;
        public string ShowCustomer
        {
            get { return CustomerID+"["+CustomerSysNo+"]"; }
            set { base.SetValue("ShowCustomer", ref showCustomer, value); }
        }

        /// <summary>
        /// 客户信息页面地址
        /// </summary>
        private string customerURL;
        public string CustomerURL
        {
            get { return string.Format(ConstValue.CustomerMaintainUrlFormat, CustomerSysNo); }
            set { base.SetValue("CustomerURL", ref customerURL, value); }
        }
    }
}
