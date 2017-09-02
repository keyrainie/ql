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
using ECCentral.BizEntity.Enum;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ComputerConfigItemVM : ModelBase
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
        private int _masterSysNo;
        /// <summary>
        /// 所属配置单系统编号
        /// </summary>
        public int MasterSysNo
        {
            get { return _masterSysNo; }
            set
            {
                base.SetValue("MasterSysNo", ref _masterSysNo, value);
            }
        }
        private int? _productSysNo;
        /// <summary>
        /// 产品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }
        private int _computerPartSysNo;
        /// <summary>
        /// 组件系统编号
        /// </summary>
        public int ComputerPartSysNo
        {
            get { return _computerPartSysNo; }
            set
            {
                base.SetValue("ComputerPartSysNo", ref _computerPartSysNo, value);
            }
        }
        private decimal? _discount;
        /// <summary>
        /// 产品折扣
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^((-\d+(\.\d+)?)|(0+(\.0+)?))$", ErrorMessage = "折扣必须是小于等于0的数字")]
        public decimal? Discount
        {
            get { return _discount; }
            set
            {
                base.SetValue("Discount", ref _discount, value);
            }
        }
        private int? _productQty;
        /// <summary>
        /// 产品数量
        /// </summary>
        [Validate(ValidateType.Interger)]
        public int? ProductQty
        {
            get { return _productQty; }
            set
            {
                base.SetValue("ProductQty", ref _productQty, value);
            }
        }
        private string _computerPartName;
        /// <summary>
        /// 组件名称，比如CPU,内存等
        /// </summary>
        public string ComputerPartName
        {
            get { return _computerPartName; }
            set
            {
                base.SetValue("ComputerPartName", ref _computerPartName, value);
            }
        }
        private YNStatus _isMust;
        /// <summary>
        /// 是否必选组件
        /// </summary>
        public YNStatus IsMust
        {
            get { return _isMust; }
            set
            {
                base.SetValue("IsMust", ref _isMust, value);
            }
        }
        private int _priority;
        /// <summary>
        /// 组件在列表中显示的优先级
        /// </summary>
        public int Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private string _note;
        /// <summary>
        /// 组件说明
        /// </summary>
        public string Note
        {
            get { return _note; }
            set
            {
                base.SetValue("Note", ref _note, value);
            }
        }
        private decimal _unitCost;
        /// <summary>
        /// 产品成本
        /// </summary>
        public decimal UnitCost
        {
            get { return decimal.Round(_unitCost, 2); }
            set { base.SetValue("UnitCost", ref _unitCost, value); }
        }
        private decimal? _currentPrice;
        /// <summary>
        /// 产品卖价
        /// </summary>
        public decimal? CurrentPrice
        {
            get { return decimal.Round(_currentPrice == null ? 0m : _currentPrice.Value, 2); }
            set
            {
                base.SetValue("CurrentPrice", ref _currentPrice, value);
            }
        }
        private int _onlineQty;
        /// <summary>
        /// 可卖库存
        /// </summary>
        public int OnlineQty
        {
            get { return _onlineQty; }
            set
            {
                base.SetValue("OnlineQty", ref _onlineQty, value);
            }
        }
        private string _productID;
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }
        private string _productName;
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            set
            {
                base.SetValue("ProductName", ref _productName, value);
            }
        }


        #region UI相关属性
        private bool _isControlReadOnly;
        public bool IsControlReadOnly
        {
            get
            {
                return _isControlReadOnly == null ? false : _isControlReadOnly;
            }
            set
            {
                base.SetValue("IsControlReadOnly", ref _isControlReadOnly, value);
            }
        }
        public bool IsControlEnabled
        {
            get
            {
                return !IsControlReadOnly;
            }
        }

        public Visibility IsMustVisibility
        {
            get
            {
                return IsMust == YNStatus.Yes ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 组件商品的可选分类
        /// </summary>
        public string ValidC3List { get; set; }
        #endregion
    }
}
