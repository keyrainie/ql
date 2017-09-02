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
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ReviewScoreItemQueryVM : ModelBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        private string sysNo;
        [Validate(ValidateType.Regex, @"^[0-9]\d*$", ErrorMessage = "编号必须是整数，且大于等于0")]
        public string SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 一级类别编号
        /// </summary>
        private int? c1SysNo;
        public int? C1SysNo
        {
            get { return c1SysNo; }
            set { base.SetValue("C1SysNo", ref c1SysNo, value); }
        }

        /// <summary>
        /// 二级类别编号
        /// </summary>
        private int? c2SysNo;
        public int? C2SysNo
        {
            get { return c2SysNo; }
            set { base.SetValue("C2SysNo", ref c2SysNo, value); }
        }

        /// <summary>
        /// 三级类别编号
        /// </summary>
        private int? c3SysNo;
        public int? C3SysNo
        {
            get { return c3SysNo; }
            set { base.SetValue("C3SysNo", ref c3SysNo, value); }
        }

        /// <summary>
        /// 渠道ID
        /// </summary>
        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set { base.SetValue("ChannelID", ref channelID, value); }
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

        /// <summary>
        /// 名称
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
            set { base.SetValue("Name", ref name, value); }
        }

        /// <summary>
        /// 状态    A：有效 D:无效
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        public bool HasReviewScoreAddPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ReviewScore_Add); }
        }
    }
    public class ReviewScoreItemVM : ModelBase
    {
        public string CompanyCode { get; set; }

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
        /// 编号
        /// </summary>
        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 一级类别编号
        /// </summary>
        private int? c1SysNo;
        public int? C1SysNo
        {
            get { return c1SysNo; }
            set { base.SetValue("C1SysNo", ref c1SysNo, value); }
        }

        /// <summary>
        /// 二级类别编号
        /// </summary>
        private int? c2SysNo;
        public int? C2SysNo
        {
            get { return c2SysNo; }
            set { base.SetValue("C2SysNo", ref c2SysNo, value); }
        }

        /// <summary>
        /// 三级类别编号
        /// </summary>
        private int? c3SysNo;
        [Validate(ValidateType.Required)]
        public int? C3SysNo
        {
            get { return c3SysNo; }
            set { base.SetValue("C3SysNo", ref c3SysNo, value); }
        }

        /// <summary>
        /// 三级类别名称
        /// </summary>
        private string category3Name;
        public string Category3Name
        {
            get { return category3Name; }
            set { base.SetValue("Category3Name", ref category3Name, value); }
        }

        /// <summary>
        /// 渠道ID
        /// </summary>
        private string channelID;
        public string ChannelID
        {
            get { return channelID; }
            set { base.SetValue("ChannelID", ref channelID, value); }
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

        /// <summary>
        /// 名称
        /// </summary>
        private string name;
        [Validate(ValidateType.Required)]
        public string Name
        {
            get { return name; }
            set { base.SetValue("Name", ref name, value); }
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
        /// 状态    A：有效 D:无效
        /// </summary>
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        /// <summary>
        /// ”有效“字符串
        /// </summary>
        public string ActiveString
        {
            get { return EnumConverter.GetDescription(ADStatus.Active); }
        }

        /// <summary>
        /// ”无效“字符串
        /// </summary>
        public string DeactiveString
        {
            get { return EnumConverter.GetDescription(ADStatus.Deactive); }
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

    }
}
