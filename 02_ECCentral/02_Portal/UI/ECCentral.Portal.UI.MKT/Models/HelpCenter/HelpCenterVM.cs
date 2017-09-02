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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class HelpCenterVM : ModelBase
    {
        private int _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }


        private string _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]{0,4}$", ErrorMessage = "优先级为0-9999的整数")]
        public string Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private FeatureType? _type;
        /// <summary>
        /// 标识类型，比如New,Hot
        /// </summary>
        public FeatureType? Type
        {
            get { return _type; }
            set
            {
                base.SetValue("Type", ref _type, value);
            }
        }
        //默认为无效
        private ADStatus? _status = ADStatus.Deactive;
        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }
        private int? _categorySysNo;
        /// <summary>
        /// 所属类型系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public int? CategorySysNo
        {
            get { return _categorySysNo; }
            set
            {
                base.SetValue("CategorySysNo", ref _categorySysNo, value);
            }
        }
        private string _companyCode;
        /// <summary>
        /// 公司编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
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

        private string _title;
        /// <summary>
        /// 帮助主题名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string Title
        {
            get { return _title; }
            set
            {
                base.SetValue("Title", ref _title, value);
            }
        }
        private string _description;
        /// <summary>
        /// 帮助主题描述
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                base.SetValue("Description", ref _description, value);
            }
        }
        private string _content;
        /// <summary>
        /// 帮助内容(输入Html代码
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                base.SetValue("Content", ref _content, value);
            }
        }
        private string _keywords;
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords
        {
            get { return _keywords; }
            set
            {
                base.SetValue("Keywords", ref _keywords, value);
            }
        }
        private bool _showInCategory;
        /// <summary>
        /// 是否在分类页面显示
        /// </summary>
        public bool ShowInCategory
        {
            get { return _showInCategory; }
            set
            {
                base.SetValue("ShowInCategory", ref _showInCategory, value);
            }
        }
        private string _relatedSysNoList;
        /// <summary>
        /// 相关帮助内容系统编号
        /// </summary>
        public string RelatedSysNoList
        {
            get { return _relatedSysNoList; }
            set
            {
                base.SetValue("RelatedSysNoList", ref _relatedSysNoList, value);
            }
        }
        private string _link;
        /// <summary>
        /// 帮助内容链接
        /// </summary>
        public string Link
        {
            get { return _link; }
            set
            {
                base.SetValue("Link", ref _link, value);
            }
        }

        #region UI相关属性


        /// <summary>
        /// 状态是否为有效
        /// </summary>
        public bool? IsActive
        {
            get { return this.Status == ADStatus.Active; }
            set
            {
                if (value.HasValue && value == true) this._status = ADStatus.Active;
            }
        }

        /// <summary>
        /// 状态是否为无效
        /// </summary>
        public bool? IsDeactive
        {
            get { return this.Status == ADStatus.Deactive; }
            set
            {
                if (value.HasValue && value == true) this._status = ADStatus.Deactive;
            }
        }

        /// <summary>
        /// 标识类型是否为新
        /// </summary>
        public bool? IsNew
        {
            get { return this.Type == FeatureType.New; }
            set
            {
                if (value.HasValue && value == true) this._type = FeatureType.New;
            }
        }

        /// <summary>
        /// 标识类型是否为热
        /// </summary>
        public bool? IsHot
        {
            get { return this.Type == FeatureType.Hot; }
            set
            {
                if (value.HasValue && value == true) this._type = FeatureType.Hot;
            }
        }

        /// <summary>
        /// 标识类型是否为无
        /// </summary>
        public bool? IsTypeNone
        {
            get { return this.Type == null; }
            set
            {
                if (value.HasValue && value == true) this._type = null;
            }
        }

        #endregion
    }
}
