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
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SEOMetadataQueryVM : ModelBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        private string pageTitle;
        public string PageTitle
        {
            get { return pageTitle; }
            set { base.SetValue("PageTitle", ref pageTitle, value); }
        }

        /// <summary>
        /// 描述
        /// </summary>
        private string pageDescription;
        public string PageDescription
        {
            get { return pageDescription; }
            set { base.SetValue("PageDescription", ref pageDescription, value); }
        }

        /// <summary>
        /// 页面关键字
        /// </summary>
        private string pageKeywords;
        public string PageKeywords
        {
            get { return pageKeywords; }
            set { base.SetValue("PageKeywords", ref pageKeywords, value); }
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
        private ADStatus? status;
        public ADStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        public string CompanyCode { get; set; }

        public bool HasSEOEditPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_SEO_SEOMetadataEdit); }
        }
    }
}
