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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class HotSaleCategoryVM:ModelBase
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
        private string _companyCode;
        /// <summary>
        /// 所属公司
        /// </summary>
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
        /// 所属渠道
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
        private string _groupName;
        /// <summary>
        /// 组名
        /// </summary>
        [Validate(ValidateType.Required)]
        public string GroupName
        {
            get { return _groupName; }
            set
            {
                base.SetValue("GroupName", ref _groupName, value);
            }
        }
        private string _itemName;
        /// <summary>
        /// 分类别名
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                base.SetValue("ItemName", ref _itemName, value);
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
        private int? _c3SysNo;
        /// <summary>
        /// 三级分类编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public int? C3SysNo
        {
            get { return _c3SysNo; }
            set
            {
                base.SetValue("C3SysNo", ref _c3SysNo, value);
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
        private int? _position;
        /// <summary>
        /// 位置
        /// </summary>
        [Validate(ValidateType.Required)]
        public int? Position
        {
            get { return _position; }
            set
            {
                base.SetValue("Position", ref _position, value);
            }
        }
        private int? _pageType;
        /// <summary>
        /// 页面类型
        /// </summary>
        [Validate(ValidateType.Required)]
        public int? PageType
        {
            get { return _pageType; }
            set
            {
                base.SetValue("PageType", ref _pageType, value);
            }
        }
        private int _pageId;
        /// <summary>
        /// 页面编号
        /// </summary>
        public int PageId
        {
            get { return _pageId; }
            set
            {
                base.SetValue("PageId", ref _pageId, value);
            }
        }

        #region UI扩展属性

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
        #endregion
    }
}
