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
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Components.Models;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.UI.MKT.Models
{
    /// <summary>
    /// 广告商
    /// </summary>
    public class AdvertisersVM : ModelBase
    {
        public AdvertisersVM()
        {
            this.ShowStatusList = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
        }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 显示与不显示
        /// </summary>
        public List<KeyValuePair<ADStatus?, string>> ShowStatusList { get; set; }

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
        /// 监测代码
        /// </summary>
        private string monitorCode;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^\w+(-_-)\w+$", ErrorMessage = "请至少输入两层代码(以-_-隔开)。")]
        public string MonitorCode
        {
            get { return monitorCode; }
            set
            {
                base.SetValue("MonitorCode", ref monitorCode, value); 
  
                Regex reg = new Regex(@"^\w+(-_-)\w+$", RegexOptions.IgnoreCase);
                Match match = reg.Match(value);
                if (match.Success)
                {
                    string[] str = Regex.Split(value, @"(?<!'(?:\d+-)*\d+)-_-(?!\d+(?:-\d+)*')");
                    AdvertiserUserName = AdvertiserUserName ?? str[1];
                    AdvertiserPassword = AdvertiserUserName ?? str[1];
                }             

            }
        }

        /// <summary>
        /// 广告效果查询账号
        /// </summary>
        private string advertiserUserName;
        [Validate(ValidateType.Required)]
        public string AdvertiserUserName
        {
            get { return advertiserUserName; }
            set { base.SetValue("AdvertiserUserName", ref advertiserUserName, value); }
        }

        /// <summary>
        /// 广告效果密码
        /// </summary>
        private string advertiserPassword;
        [Validate(ValidateType.Required)]
        public string AdvertiserPassword
        {
            get { return advertiserPassword; }
            set { base.SetValue("AdvertiserPassword", ref advertiserPassword, value); }
        }

        /// <summary>
        /// SysNo
        /// </summary>
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// Cookie有效期
        /// </summary>
        private string cookiePeriod;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,5}$", ErrorMessage = "请输入0至999999的整数！")]
        public string CookiePeriod
        {
            get { return cookiePeriod; }
            set { base.SetValue("CookiePeriod", ref cookiePeriod, value); }
        }

        /// <summary>
        /// 广告商
        /// </summary>
        private string advertiserName;
        [Validate(ValidateType.Required)]
        public string AdvertiserName
        {
            get { return advertiserName; }
            set { base.SetValue("AdvertiserName", ref advertiserName, value); }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        private string inUser;
        public string InUser
        {
            get { return inUser; }
            set { base.SetValue("InUser", ref inUser, value); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? inDate;
        public DateTime? InDate
        {
            get { return inDate; }
            set { base.SetValue("InDate", ref inDate, value); }
        }

        /// <summary>
        /// 更新人
        /// </summary>
        private string editUser;
        public string EditUser
        {
            get { return editUser; }
            set { base.SetValue("EditUser", ref editUser, value); }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        private DateTime? editDate;
        public DateTime? EditDate
        {
            get { return editDate; }
            set { base.SetValue("EditDate", ref editDate, value); }
        }

        /// <summary>
        /// 效果查询链接
        /// </summary>
        private string effectLink;
        public string EffectLink
        {
            get { return effectLink; }
            set { base.SetValue("EffectLink", ref effectLink, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
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
        #region UI扩展属性

        /// <summary>
        /// ”有效“字符串
        /// </summary>
        public string ActiveString
        {
            get{ return EnumConverter.GetDescription(ADStatus.Active);}
        }

        /// <summary>
        /// ”无效“字符串
        /// </summary>
        public string DeactiveString
        {
            get{return EnumConverter.GetDescription(ADStatus.Deactive);}
        }

        public bool IsActive
        {
            get
            {
                return Status == ADStatus.Active;
            }
            set
            {
                if (value)
                    Status = ADStatus.Active;
                else
                    Status = ADStatus.Deactive;
            }
        }
        #endregion

        public bool HasAdvertisersSavePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Advertisers_Save); }
        }
    }


    public class AdvertisersQueryVM : ModelBase
    {
        public AdvertisersQueryVM()
        {
            this.ShowStatusList = EnumConverter.GetKeyValuePairs<ADStatus>(EnumConverter.EnumAppendItemType.All);
        }

        /// <summary>
        /// 显示与不显示
        /// </summary>
        public List<KeyValuePair<ADStatus?, string>> ShowStatusList { get; set; }


        /// <summary>
        /// 监测代码
        /// </summary>
        private string monitorCode;
        public string MonitorCode
        {
            get { return monitorCode; }
            set { base.SetValue("MonitorCode", ref monitorCode, value); }
        }


        /// <summary>
        /// Cookie有效期
        /// </summary>         
        private string cookiePeriod;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,5}$", ErrorMessage = "请输入0至999999的整数！")]
        public string CookiePeriod
        {
            get { return cookiePeriod; }
            set { base.SetValue("CookiePeriod", ref cookiePeriod, value); }
        }

        /// <summary>
        /// 广告商
        /// </summary>
        private string advertiserName;
        public string AdvertiserName
        {
            get { return advertiserName; }
            set { base.SetValue("AdvertiserName", ref advertiserName, value); }
        }


        /// <summary>
        /// 状态
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
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

        public bool HasAdvertisersBatchApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Advertisers_BatchApprove); }
        }
    }
}

       