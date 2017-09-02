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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SEOMetadataVM : ModelBase
    {

        public int? SysNo { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        private string pageTitle;
        [Validate(ValidateType.Required)]
        public string PageTitle
        {
            get { return pageTitle; }
            set { base.SetValue("PageTitle", ref pageTitle, value); }
        }

        /// <summary>
        /// 描述
        /// </summary>
        private string pageDescription;
        [Validate(ValidateType.Required)]
        public string PageDescription
        {
            get { return pageDescription; }
            set { base.SetValue("PageDescription", ref pageDescription, value); }
        }

        /// <summary>
        /// 页面附加代码（HTML 用于优化SEO的HTML代码
        /// </summary>
        private string pageAdditionContent;
        //[Validate(ValidateType.Required)]
        public string PageAdditionContent
        {
            get { return pageAdditionContent; }
            set { base.SetValue("PageAdditionContent", ref pageAdditionContent, value); }
        }

        /// <summary>
        /// 页面关键字
        /// </summary>
        private string pageKeywords;
        [Validate(ValidateType.Required)]
        public string PageKeywords
        {
            get { return pageKeywords; }
            set { base.SetValue("PageKeywords", ref pageKeywords, value); }
        }

        /// <summary>
        /// 扩展生效
        /// </summary>
        private bool? isExtendValid;
        public bool? IsExtendValid
        {
            get { return isExtendValid; }
            set { base.SetValue("IsExtendValid", ref isExtendValid, value); }
        }

        /// <summary>
        /// 页面ID
        /// </summary>
        private int? pageID;
        public int? PageID
        {
            get { return pageID; }
            set { base.SetValue("PageID", ref pageID, value); }
        }

        /// <summary>
        /// 页面类型
        /// </summary>
        private int? pageType;
        public int? PageType
        {
            get { return pageType; }
            set { base.SetValue("PageType", ref pageType, value); }
        }

        /// <summary>
        /// 页面类型名称
        /// </summary>
        private string pageTypeName;
        public string PageTypeName
        {
            get { return pageTypeName; }
            set { base.SetValue("PageTypeName", ref pageTypeName, value); }
        }

        /// <summary>
        /// 页面ID名称
        /// </summary>
        private string pageIDName;
        public string PageIDName
        {
            get { return pageIDName; }
            set { base.SetValue("PageIDName", ref pageIDName, value); }
        }

        /// <summary>
        /// 所选渠道
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
        /// 状态
        /// </summary>
        private ADStatus? status = ADStatus.Active;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
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

        public string CompanyCode { get; set; }

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
    }
   
}
