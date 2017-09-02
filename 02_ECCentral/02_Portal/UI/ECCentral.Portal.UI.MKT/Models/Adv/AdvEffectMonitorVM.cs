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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class AdvEffectMonitorVM : ModelBase
    {
        public AdvEffectMonitorVM()
        {
            //this.ShowStatusList = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
            this.SOStatusList = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.SO.SOStatus>(EnumConverter.EnumAppendItemType.All);
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.SO.SOStatus?, string>> SOStatusList { get; set; }

        /// <summary>
        /// 显示与不显示
        /// </summary>
        //public List<KeyValuePair<YNStatus?, string>> ShowStatusList { get; set; }

        /// <summary>
        /// 包含产生RO退款单
        /// </summary>
        public NYNStatus? IsRefund { get; set; }

        public bool IsRefundChecked
        {
            get
            {
                return IsRefund == NYNStatus.Yes;
            }
            set
            {
                IsRefund = value ? NYNStatus.Yes : NYNStatus.No;
            }
        }

        /// <summary>
        /// 关键字
        /// </summary>
        private string cmp;
        public string CMP
        {
            get { return cmp; }
            set { base.SetValue("CMP", ref cmp, value); }
        }

        /// <summary>
        /// 创建时间开始于
        /// </summary>
        private DateTime? inDateFrom;
        public DateTime? InDateFrom
        {
            get { return inDateFrom; }
            set { base.SetValue("InDateFrom", ref inDateFrom, value); }
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        private string customerID;
        public string CustomerID
        {
            get { return customerID; }
            set { base.SetValue("CustomerID", ref customerID, value); }
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        private ECCentral.BizEntity.Customer.CustomerRank rank;
        public ECCentral.BizEntity.Customer.CustomerRank Rank
        {
            get { return rank; }
            set { base.SetValue("Rank", ref rank, value); }
        }
        /// <summary>
        /// 会员等级
        /// </summary>
        //private string showRank;
        public string ShowRank
        {
            get { return customerID + "(" + rank.ToDescription() + ")"; }
            //set { base.SetValue("ShowRank", ref showRank, value); }
        }

        /// <summary>
        /// 订单最大金额
        /// </summary>
        public decimal? MinSOAmt { get; set; }

        /// <summary>
        /// 订单最小金额
        /// </summary>
        public decimal? MaxSOAmt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? createDate;
        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
        }

        /// <summary>
        /// 用户注册时间
        /// </summary>
        private DateTime? registerTime;
        public DateTime? RegisterTime
        {
            get { return registerTime; }
            set { base.SetValue("RegisterTime", ref registerTime, value); }
        }

        /// <summary>
        /// 创建时间结束于
        /// </summary>
        private DateTime? inDateTo;
        public DateTime? InDateTo
        {
            get { return inDateTo; }
            set { base.SetValue("InDateTo", ref inDateTo, value); }
        }

        /// <summary>
        /// 监视动作类型
        /// </summary>
        private string operationType;
        public string OperationType
        {
            get { return operationType; }
            set { base.SetValue("OperationType", ref operationType, value); }
        }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 显示的订单金额等级
        /// </summary>
        public string ShowSOAmtLevel{ get;set; }

        /// <summary>
        /// 订单金额等级
        /// </summary>
        public string SOAmtLevel
        {
            get {
                return null;
            }
            set
            {
                switch (value)
                {
                    case "Z":
                        MaxSOAmt = 0;
                        MinSOAmt = 0;
                        break;
                    case "A":
                        MaxSOAmt = 100;
                        MinSOAmt = 0;
                        break;
                    case "B":
                        MaxSOAmt = 300;
                        MinSOAmt = 100;
                        break;
                    case "C":
                        MaxSOAmt = 500;
                        MinSOAmt = 300;
                        break;
                    case "D":
                        MaxSOAmt = 800;
                        MinSOAmt = 500;
                        break;
                    case "E":
                        MaxSOAmt = 1000;
                        MinSOAmt = 800;
                        break;
                    case "F":
                        MaxSOAmt = 1500;
                        MinSOAmt = 1000;
                        break;
                    case "G":
                        MaxSOAmt = 2000;
                        MinSOAmt = 1500;
                        break;
                    case "H":
                        MaxSOAmt = 3000;
                        MinSOAmt = 2000;
                        break;
                    case "I":
                        MaxSOAmt = 5000;
                        MinSOAmt = 3000;
                        break;
                    case "J":
                        MaxSOAmt = 8000;
                        MinSOAmt = 5000;
                        break;
                    case "K":
                        MaxSOAmt = 10000;
                        MinSOAmt = 8000;
                        break;
                    case "L":
                        MaxSOAmt = 9999999999;
                        MinSOAmt = 10000;
                        break;
                    default:
                        MaxSOAmt = null;
                        MinSOAmt = null;
                        break;
                }
                
            }
            //get { return soAmtLevel; }
            //set { base.SetValue("SOAmtLevel", ref soAmtLevel, value); }
        }

        /// <summary>
        /// 是否有效订单
        /// </summary>
        private NYNStatus? isValidSO;
        public NYNStatus? IsValidSO
        {
            get { return isValidSO; }
            set { base.SetValue("IsValidSO", ref isValidSO, value); }
        }

        /// <summary>
        /// 订单编号
        /// </summary>
        private string soID;
        public string SOID
        {
            get { return soID; }
            set { base.SetValue("SOID", ref soID, value); }
        }

        /// <summary>
        /// 所有的广告效果所涉及的总价钱
        /// </summary>
        public decimal? TotalSOAmount { get; set; }


        /// <summary>
        /// 
        /// </summary>
        //public decimal? ToltalPrice { get; set; }

        /// <summary>
        /// 订单金额   广告效果所涉及的总价钱
        /// </summary>
        private decimal? soAmount;
        public decimal? SOAmount
        {
            get { return soAmount; }
            set { base.SetValue("SOAmount", ref soAmount, value); }
        }

        /// <summary>
        /// 退款金额
        /// </summary>
        private decimal? refundAmt;
        public decimal? RefundAmt
        {
            get { return refundAmt; }
            set { base.SetValue("RefundAmt", ref refundAmt, value); }
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        private ECCentral.BizEntity.SO.SOStatus? soStatus;
        public ECCentral.BizEntity.SO.SOStatus? SOStatus
        {
            get { return soStatus; }
            set { base.SetValue("SOStatus", ref soStatus, value); }
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string ReferenceSysNo { get; set; }
        /// <summary>
        /// 顾客编号
        /// </summary>
        public string CustomerSysNo { get; set; }
        /// <summary>
        /// 是否通过手机验证
        /// </summary>
        private NYNStatus? isPhone;
        public NYNStatus? IsPhone
        {
            get {
                return isPhone;
                }
            set { 
                //if (isPhone == NYNStatus.No)
                //    isPhone = NYNStatus.Yes;
                //else
                //    isPhone = NYNStatus.No;
               
                base.SetValue("IsPhone", ref isPhone, value); }
        }

        /// <summary>
        /// 是否通过邮箱验证
        /// </summary>
        private NYNStatus? isEmailConfirmed;
        public NYNStatus? IsEmailConfirmed
        {
            get
            {
                return isEmailConfirmed;
            }
            set {
                //if (isEmailConfirmed == NYNStatus.No)
                //    isEmailConfirmed = NYNStatus.Yes;
                //else
                //    isEmailConfirmed = NYNStatus.No;
               
                base.SetValue("IsEmailConfirmed", ref isEmailConfirmed, value); }
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
    }
}
