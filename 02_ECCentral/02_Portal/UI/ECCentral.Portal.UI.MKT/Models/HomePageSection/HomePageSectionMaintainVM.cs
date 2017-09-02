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
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class HomePageSectionMaintainVM:ModelBase
    {
        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }
        private string _domainName;
        /// <summary>
        /// 商城首页区域名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string DomainName
        {
            get { return _domainName; }
            set
            {
                base.SetValue("DomainName", ref _domainName, value);
            }
        }
        private string _c1List;
        /// <summary>
        /// 商城首页区域推荐商品不足时，从C1List中的指定一级分类自动补充商品到推荐位
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^((\d+[,，]){0,10000})\d+$", ErrorMessage = "输入数字以,隔开")]
        public string C1List
        {
            get { return _c1List; }
            set
            {
                base.SetValue("C1List", ref _c1List, value);
            }
        }
        private String _exceptC3List;
        /// <summary>
        /// 商城首页区域推荐商品不足时，从C1List中的一级分类自动补充商品到推荐位时，排除指定三级分类下的商品
        /// </summary>
        public String ExceptC3List
        {
            get { return _exceptC3List; }
            set
            {
                base.SetValue("ExceptC3List", ref _exceptC3List, value);
            }
        }
        private int? _priority;
        /// <summary>
        /// 展示优先级
        /// </summary>
        [Validate(ValidateType.Required)]
        public int? Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private ADStatus _status;
        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }

        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

        #region UI扩展属性

        /// <summary>
        /// ”有效“字符串
        /// </summary>
        public string ActiveString
        {
            get
            {
                return EnumConverter.GetDescription(ADStatus.Active);
            }
        }

        /// <summary>
        /// ”无效“字符串
        /// </summary>
        public string DeactiveString
        {
            get
            {
                return EnumConverter.GetDescription(ADStatus.Deactive);
            }
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
    }
}
