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
    public class ProductRecommendVM : ModelBase
    {
        public ProductRecommendVM()
        {
            LocationVM = new ProductRecommendLocationVM();
        }

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
        private string _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }
        private string _productID;
        /// <summary>
        /// 商品编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }
        private string _priority;
        /// <summary>
        /// 显示优先级
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private string _beginDate=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// 生效开始时间
        /// </summary>
         [Validate(ValidateType.Required)]
        public string BeginDate
        {
            get { return _beginDate; }
            set
            {
                base.SetValue("BeginDate", ref _beginDate, value);
            }
        }
         private string _endDate=DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// 失效结束日期
        /// </summary>
         [Validate(ValidateType.Required)]
         public string EndDate
        {
            get { return _endDate; }
            set
            {
                base.SetValue("EndDate", ref _endDate, value);
            }
        }
         //默认为无效
         private ADStatus _status = ADStatus.Deactive;
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
        private string _icon;
        /// <summary>
        /// 推荐商品上要显示的小图标，比如"新品，"惊爆价"等小图标
        /// </summary>
        public string Icon
        {
            get { return _icon; }
            set
            {
                base.SetValue("Icon", ref _icon, value);
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

        private ProductRecommendLocationVM _locationVM;
        /// <summary>
        /// 推荐商品所在位置信息
        /// </summary>
        public ProductRecommendLocationVM LocationVM
        {
            get { return _locationVM; }
            set
            {
                base.SetValue("LocationVM", ref _locationVM, value);
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
