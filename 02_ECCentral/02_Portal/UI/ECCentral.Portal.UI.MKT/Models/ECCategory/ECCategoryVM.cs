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
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ECCategoryVM : ModelBase
    {
        public ECCategoryVM()
        {
            Parents = new ObservableCollection<ECCategoryVM>();
            Children = new ObservableCollection<ECCategoryVM>();
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

        /// <summary>
        /// 层级关系系统编号
        /// </summary>
        public int RSysNo { get; set; }

        private int? _rParentSysNo;
        /// <summary>
        /// 父级系统编号
        /// </summary>
        public int? RParentSysNo
        {
            get { return _rParentSysNo; }
            set
            {
                base.SetValue("RParentSysNo", ref _rParentSysNo, value);
            }
        }

        public int? SysNo { get; set; }

        private int? _parentSysNo;
        /// <summary>
        /// 父级系统编号
        /// </summary>
        public int? ParentSysNo
        {
            get { return _parentSysNo; }
            set
            {
                base.SetValue("ParentSysNo", ref _parentSysNo, value);
            }
        }


        private string _name;
        /// <summary>
        /// 分类名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string Name
        {
            get { return _name; }
            set
            {
                base.SetValue("Name", ref _name, value);
            }
        }


        private int? _C3SysNo;
        /// <summary>
        /// 前台三级分类对应的后台三级分类编号，只有前台三级分类有此属性
        /// </summary>
        public int? C3SysNo
        {
            get { return _C3SysNo; }
            set
            {
                base.SetValue("C3SysNo", ref _C3SysNo, value);
            }
        }

        private string _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,7}$", ErrorMessage = "请输入0至99999999的整数！")]
        public string Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }

        private YNStatus _isParentCategoryShow;
        /// <summary>
        /// 是否在前台父级分类页面是否显示
        /// </summary>
        public YNStatus IsParentCategoryShow
        {
            get { return _isParentCategoryShow; }
            set
            {
                base.SetValue("IsParentCategoryShow", ref _isParentCategoryShow, value);
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
        private FeatureType? _promotionStatus;
        /// <summary>
        /// 促销状态，比如New,Hot
        /// </summary>
        public FeatureType? PromotionStatus
        {
            get { return _promotionStatus; }
            set
            {
                base.SetValue("PromotionStatus", ref _promotionStatus, value);
            }
        }
        private ECCategoryLevel _level;
        /// <summary>
        /// 级别(类别类型),1级，2级，3级
        /// </summary>
        public ECCategoryLevel Level
        {
            get { return _level; }
            set
            {
                base.SetValue("Level", ref _level, value);
                this.IsRealC3Visibility = Level == ECCategoryLevel.Category3 ? Visibility.Visible : Visibility.Collapsed;
                this.IsParentCategoryShowVisibility = Level != ECCategoryLevel.Category1 ? Visibility.Visible : Visibility.Collapsed;
                this.IsViewChildrenEnabled = Level != ECCategoryLevel.Category3;
                this.IsMaintainParentEnabled = Level != ECCategoryLevel.Category1;
            }
        }

        public ObservableCollection<ECCategoryVM> Parents { get; set; }

        public ObservableCollection<ECCategoryVM> Children { get; set; }

        #region UI扩展

        public string DisplayName
        {
            get
            {
                if (SysNo.HasValue)
                {
                    return string.Format("[{0}]{1}", SysNo.ToString(), Name);
                }

                return Name;
            }
        }

        /// <summary>
        /// 是否在父类页面显示
        /// </summary>
        public bool? ShowOnParentPage
        {
            get
            {
                return this.IsParentCategoryShow == YNStatus.Yes;
            }
            set
            {
                if (value.HasValue && value == true)
                {
                    this.IsParentCategoryShow = YNStatus.Yes;
                }
                else
                {
                    this.IsParentCategoryShow = YNStatus.No;
                }
            }
        }

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
        /// 促销类型是否为新
        /// </summary>
        public bool? IsNew
        {
            get { return this.PromotionStatus == FeatureType.New; }
            set
            {
                if (value.HasValue && value == true) this._promotionStatus = FeatureType.New;
            }
        }

        /// <summary>
        /// 促销类型是否为热
        /// </summary>
        public bool? IsHot
        {
            get { return this.PromotionStatus == FeatureType.Hot; }
            set
            {
                if (value.HasValue && value == true) this._promotionStatus = FeatureType.Hot;
            }
        }

        /// <summary>
        /// 促销类型是否为无
        /// </summary>
        public bool? IsNone
        {
            get { return this.PromotionStatus == null; }
            set
            {
                if (value.HasValue && value == true) this._promotionStatus = null;
            }
        }

        private Visibility _isParentCategoryShowVisibility = Visibility.Collapsed;
        /// <summary>
        /// 是否显示CheckBox“父类页面显示”
        /// </summary>
        public Visibility IsParentCategoryShowVisibility
        {
            get { return _isParentCategoryShowVisibility; }
            set
            {
                base.SetValue("IsParentCategoryShowVisibility", ref _isParentCategoryShowVisibility, value);
            }
        }
        private bool _isMaintainParentEnabled = false;
        /// <summary>
        /// 是否可维护父类数量
        /// </summary>
        public bool IsMaintainParentEnabled
        {
            get { return _isMaintainParentEnabled; }
            set
            {
                base.SetValue("IsMaintainParentEnabled", ref _isMaintainParentEnabled, value);
            }
        }
        private bool _isViewChildrenEnabled = true;
        /// <summary>
        /// 是否可查看子类数量
        /// </summary>
        public bool IsViewChildrenEnabled
        {
            get { return _isViewChildrenEnabled; }
            set
            {
                base.SetValue("IsViewChildrenEnabled", ref _isViewChildrenEnabled, value);
            }
        }
        private Visibility _isRealC3Visibility = Visibility.Collapsed;
        /// <summary>
        /// 后台实际三级类别是否显示
        /// </summary>
        public Visibility IsRealC3Visibility
        {
            get { return _isRealC3Visibility; }
            set
            {
                base.SetValue("IsRealC3Visibility", ref _isRealC3Visibility, value);
            }
        }

        private int _parentCount;
        /// <summary>
        /// 父类数量
        /// </summary>
        public int ParentCount
        {
            get { return _parentCount; }
            set
            {
                base.SetValue("ParentCount", ref _parentCount, value);
            }
        }
        private int _childrenCount;
        /// <summary>
        /// 子类数量
        /// </summary>
        public int ChildrenCount
        {
            get { return _childrenCount; }
            set
            {
                base.SetValue("ChildrenCount", ref _childrenCount, value);
            }
        }

        private bool _isChecked;
        /// <summary>
        /// 在列表中是否选中
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                base.SetValue("IsChecked", ref _isChecked, value);
            }
        }


        #endregion

        public bool HasMaintainPermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ECCategory_Btn_Aud_Exec) &&
                       AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ECCategory_CategoryCustomized);
            }
        }
    }
}
