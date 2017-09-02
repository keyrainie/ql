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
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.NeweggCN.Models
{
    public class SaleDiscountRuleVM : ModelBase
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

        private string m_VendorSysNo = "1";
        public string VendorSysNo
        {
            get
            {
                return m_VendorSysNo;
            }
            set
            {
                this.SetValue("VendorSysNo", ref m_VendorSysNo, value);
            }
        }
        private string m_VendorName = "泰隆优选";
        public string VendorName
        {
            get
            {

                return m_VendorName;
            }
            set
            {
                this.SetValue("VendorName", ref m_VendorName, value);
            }
        }

        private string _activityName;
        /// <summary>
        /// 活动名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ActivityName
        {
            get { return _activityName; }
            set
            {
                base.SetValue("ActivityName", ref _activityName, value);
            }
        }
        private DateTime? _beginDate;
        /// <summary>
        /// 开始时间
        /// </summary>
        [Validate(ValidateType.Required)]
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
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                base.SetValue("EndDate", ref _endDate, value);
            }
        }
        private int? _c3SysNo;
        /// <summary>
        /// 三级类别系统编号
        /// </summary>
        public int? C3SysNo
        {
            get { return _c3SysNo; }
            set
            {
                base.SetValue("C3SysNo", ref _c3SysNo, value);
            }
        }
        private int? _brandSysNo;
        /// <summary>
        /// 品牌系统编号
        /// </summary>
        public int? BrandSysNo
        {
            get { return _brandSysNo; }
            set
            {
                base.SetValue("BrandSysNo", ref _brandSysNo, value);
            }
        }
        private int? _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }
        private SaleDiscountRuleType _ruleType;
        /// <summary>
        /// 规则类型，0-限定金额，1-限定数量
        /// </summary>
        public SaleDiscountRuleType RuleType
        {
            get { return _ruleType; }
            set
            {
                base.SetValue("RuleType", ref _ruleType, value);
            }
        }
        private bool _isCycle;
        /// <summary>
        /// 循环标识，指是否可以成倍享受折扣，如果为false,则最多享受一次
        /// </summary>
        public bool IsCycle
        {
            get { return _isCycle; }
            set
            {
                base.SetValue("IsCycle", ref _isCycle, value);
            }
        }
        private bool _isSingle;
        /// <summary>
        /// 单品标记,表示符合条件的商品范围按单品来享受折扣
        /// </summary>
        public bool IsSingle
        {
            get { return _isSingle; }
            set
            {
                base.SetValue("IsSingle", ref _isSingle, value);
            }
        }
        private string _minAmt;
        /// <summary>
        /// 活动商品的金额下限
        /// </summary>
        [Validate(ValidateType.Required)]
        public string MinAmt
        {
            get { return _minAmt; }
            set
            {
                base.SetValue("MinAmt", ref _minAmt, value);
            }
        }
        private string _maxAmt;
        /// <summary>
        /// 活动商品的金额上限
        /// </summary>
        [Validate(ValidateType.Required)]
        public string MaxAmt
        {
            get { return _maxAmt; }
            set
            {
                base.SetValue("MaxAmt", ref _maxAmt, value);
            }
        }
        private string _minQty;
        /// <summary>
        /// 购买数量下限
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string MinQty
        {
            get { return _minQty; }
            set
            {
                base.SetValue("MinQty", ref _minQty, value);
            }
        }
        private string _maxQty;
        /// <summary>
        /// 购买数量上限
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string MaxQty
        {
            get { return _maxQty; }
            set
            {
                base.SetValue("MaxQty", ref _maxQty, value);
            }
        }
        private string _discountAmount;
        /// <summary>
        /// 销售折扣
        /// </summary>
        [Validate(ValidateType.Required)]
        public string DiscountAmount
        {
            get { return _discountAmount; }
            set
            {
                base.SetValue("DiscountAmount", ref _discountAmount, value);
            }
        }
        private SaleDiscountRuleStatus _status;
        /// <summary>
        /// 状态：0-无效，1-有效
        /// </summary>
        public SaleDiscountRuleStatus Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }

        #region UI Properties

        /// <summary>
        /// 商品ID(用于维护界面上显示)
        /// </summary>
        public string UIProductID { get; set; }

        /// <summary>
        /// 品牌名称(用于维护界面上显示)
        /// </summary>
        public string UIBrandName { get; set; }


        #endregion

        public bool IsC3SysNoValid()
        {
            return C3SysNo > 0;
        }

        public bool IsBrandSysNoValid()
        {
            return BrandSysNo > 0;
        }

        public bool IsProductSysNoValid()
        {
            return ProductSysNo > 0;
        }
    }
}
