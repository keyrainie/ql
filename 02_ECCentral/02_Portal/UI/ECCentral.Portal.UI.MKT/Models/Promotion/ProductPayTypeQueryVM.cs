using System;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.MKT.Models.Promotion
{
    public class ProductPayTypeQueryVM : ModelBase
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        private bool _isEnable = true;
        public bool IsEnable
        {
            get
            {
                if (_status == PayTypeStatus.D)
                {
                    _isEnable = false;
                }
                return _isEnable;
            }
            set { SetValue("IsEnable", ref _isEnable, value); }
        }

        /// <summary>
        /// 自动编号
        /// </summary>
        public int? _sysNo;

        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? _productSysNo;

        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set { SetValue("ProductSysNo", ref _productSysNo, value); }
        }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string _productTitle;

        public string ProductTitle
        {
            get { return _productTitle; }
            set { SetValue("ProductTitle", ref _productTitle, value); }
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string _productID;

        public string ProductID
        {
            get { return _productID; }
            set { SetValue("ProductID", ref _productID, value); }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public int? _payTypeSysNo;

        public int? PayTypeSysNo
        {
            get { return _payTypeSysNo; }
            set { SetValue("PayTypeSysNo", ref _payTypeSysNo, value); }
        }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        public string _payTypeName;

        public string PayTypeName
        {
            get { return _payTypeName; }
            set { SetValue("PayTypeName", ref _payTypeName, value); }
        }

        /// <summary>
        /// 开始生效时间
        /// </summary>
        public DateTime? _beginDate;

        public DateTime? BeginDate
        {
            get { return _beginDate; }
            set { SetValue("BeginDate", ref _beginDate, value); }
        }

        /// <summary>
        /// 开始时间从
        /// </summary>
        public DateTime? BeginDateFrom { get; set; }

        /// <summary>
        /// 开始时间到
        /// </summary>
        public DateTime? BeginDateTo { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? _endDate;

        public DateTime? EndDate
        {
            get { return _endDate; }
            set { SetValue("EndDate", ref _endDate, value); }
        }

        /// <summary>
        /// 结束时间从
        /// </summary>
        public DateTime? EndDateFrom { get; set; }

        /// <summary>
        /// 结束时间到
        /// </summary>
        public DateTime? EndDateTo { get; set; }

        /// <summary>
        /// 最后编辑人
        /// </summary>
        public string _editUser;

        public string EditUser
        {
            get { return _editUser; }
            set { SetValue("EditUser", ref _editUser, value); }
        }

        /// <summary>
        /// 最后编辑时间
        /// </summary>
        public DateTime _editDate;

        public DateTime EditDate
        {
            get { return _editDate; }
            set { SetValue("EditDate", ref _editDate, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public PayTypeStatus? _status;

        public PayTypeStatus? Status
        {
            get { return _status; }
            set { SetValue("Status", ref _status, value); }
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
    }
}
