using System;
using System.Linq;
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
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class StopWordsQueryVM : ModelBase
    {
        public StopWordsQueryVM()
        {
            ShowStatusList = EnumConverter.GetKeyValuePairs<ADTStatus>();
        }

        /// <summary>
        /// 展示模式
        /// </summary>
        public List<KeyValuePair<ADTStatus?, string>> ShowStatusList { get; set; }
        
        /// <summary>
        /// SysNo
        /// </summary>
        private int sysNo;
        public int SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 关键字
        /// </summary>
        private string keywords;
        public string Keywords
        {
            get { return keywords; }
            set { base.SetValue("Keywords", ref keywords, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        private ADTStatus? status;
        public ADTStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
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
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

        public string CompanyCode { get; set; }

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

        #region 扩展属性

        /// <summary>
        /// ”有效“字符串
        /// </summary>
        public string ActiveString
        {
            get { return EnumConverter.GetDescription(ADTStatus.Active); }
        }

        /// <summary>
        /// ”无效“字符串
        /// </summary>
        public string DeactiveString
        {
            get { return EnumConverter.GetDescription(ADTStatus.Deactive); }
        }

        /// <summary>
        /// ”测试“字符串
        /// </summary>
        public string TestString
        {
            get { return EnumConverter.GetDescription(ADTStatus.Test); }
        }


        public bool IsActive
        {
            get
            {
                return Status == ADTStatus.Active;
            }
            set
            {
                if (value)
                    Status = ADTStatus.Active;
            }
        }
        public bool IsDeactive
        {
            get
            {
                return Status == ADTStatus.Deactive;
            }
            set
            {
                if (value)
                    Status = ADTStatus.Deactive;
            }
        }
        public bool IsTest
        {
            get
            {
                return Status == ADTStatus.Test;
            }
            set
            {
                if (value)
                    Status = ADTStatus.Test;
            }
        }
        #endregion
    }
}

