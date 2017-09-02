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
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Linq;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class BannerLocationVM : ModelBase
    {
        public BannerLocationVM()
        {
            this.Infos = new BannerInfoVM();
            BannerDimension = new BannerDimensionVM();
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
        private bool _isExtendValid;
        /// <summary>
        /// 是否扩展生效
        /// </summary>
        public bool IsExtendValid
        {
            get { return _isExtendValid; }
            set
            {
                base.SetValue("IsExtendValid", ref _isExtendValid, value);
            }
        }
        private int? _pageID;
        /// <summary>
        /// 广告显示页面的编号
        /// </summary>
        public int? PageID
        {
            get { return _pageID; }
            set
            {
                base.SetValue("PageID", ref _pageID, value);

            }
        }
        private int _bannerDimensionSysNo;
        /// <summary>
        /// 广告的尺寸系统编号
        /// </summary>
        public int BannerDimensionSysNo
        {
            get { return _bannerDimensionSysNo; }
            set
            {
                base.SetValue("BannerDimensionSysNo", ref _bannerDimensionSysNo, value);  
            }
        }
        /// <summary>
        /// 广告的尺寸
        /// </summary>
        public BannerDimensionVM BannerDimension { get; set; }

        private string _ratio;
        /// <summary>
        /// 广告比率（%）
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,3}$|^-[1-9]\d{0,3}$|^10000$|-10000$", ErrorMessage = "请输入-10000至10000的整数。")]
        public string Ratio
        {
            get { return _ratio; }
            set
            {
                base.SetValue("Ratio", ref _ratio, value);
            }
        }

        private DateTime? _beginDate;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate
        {
            get { return _beginDate; }
            set
            {
                base.SetValue("BeginDate", ref _beginDate, value);
            }
        }
        private DateTime? _endDate;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                base.SetValue("EndDate", ref _endDate, value);
            }
        }
        private string _relativeTags;
        /// <summary>
        /// 相关Tags
        /// </summary>
        public string RelativeTags
        {
            get { return _relativeTags; }
            set
            {
                base.SetValue("RelativeTags", ref _relativeTags, value);
            }
        }
        private string _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,3}$", ErrorMessage = "请输入0至9999的整数。")]
        public string Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private string _exceptPageID;
        /// <summary>
        /// 排除PageID
        /// </summary>
        [Validate(ValidateType.MaxLength, 500)]
        public string ExceptPageID
        {
            get { return _exceptPageID; }
            set
            {
                base.SetValue("ExceptPageID", ref _exceptPageID, value);
            }
        }
        /// <summary>
        /// 主要投放区域
        /// </summary>
        public List<int> AreaShow
        {
            get
            {
                var list = new List<int>();
                if (!string.IsNullOrEmpty(SelectedArea))
                {
                    SelectedArea.Split(',').ForEach(item =>
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            list.Add(int.Parse(item));
                        }
                    });
                }
                return list;
            }
            set
            {
                SelectedArea = value.Join(",");
            }
        }
        //默认为无效
        private ADStatus? _status = ADStatus.Active;
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
        private BannerInfoVM _infos;
        /// <summary>
        /// 广告信息
        /// </summary>
        public BannerInfoVM Infos
        {
            get { return _infos; }
            set
            {
                base.SetValue("Infos", ref _infos, value);
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
                if (value.HasValue && value.Value) this._status = ADStatus.Active;
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
                if (value.HasValue && value.Value) this._status = ADStatus.Deactive;
            }
        }

        private string _SelectedArea;
        /// <summary>
        /// 以逗号分隔的投放区域编号
        /// </summary>
        public string SelectedArea
        {
            get { return _SelectedArea; }
            set { base.SetValue("SelectedArea", ref _SelectedArea, value); }
        }


        #endregion
    }
}
