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

namespace ECCentral.Portal.UI.MKT.NeweggCN.Models
{
    public class SaleDiscountRuleQueryVM : ModelBase
    {
        private string _activityName;
        /// <summary>
        /// 活动名称
        /// </summary>
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

        private int? _c1SysNo;
        /// <summary>
        /// 一级类别系统编号
        /// </summary>
        public int? C1SysNo
        {
            get { return _c1SysNo; }
            set
            {
                base.SetValue("C1SysNo", ref _c1SysNo, value);
            }
        }

        private int? _c2SysNo;
        /// <summary>
        /// 二级类别系统编号
        /// </summary>
        public int? C2SysNo
        {
            get { return _c2SysNo; }
            set
            {
                base.SetValue("C2SysNo", ref _c2SysNo, value);
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

        private string _productID;
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
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
        private SaleDiscountRuleType? _ruleType;
        /// <summary>
        /// 规则类型，0-限定金额，1-限定数量
        /// </summary>
        public SaleDiscountRuleType? RuleType
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
        private SaleDiscountRuleStatus? _status;
        /// <summary>
        /// 状态：0-无效，1-有效
        /// </summary>
        public SaleDiscountRuleStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }

        private int? _vendorSysNo;
        /// <summary>
        /// 所属商家
        /// </summary>
        public int? VendorSysNo
        {
            get { return _vendorSysNo; }
            set
            {
                base.SetValue("VendorSysNo", ref _vendorSysNo, value);
            }
        }
    }
}
