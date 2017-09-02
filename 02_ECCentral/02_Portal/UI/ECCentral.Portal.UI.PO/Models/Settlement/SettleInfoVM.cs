using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class SettleInfoVM : ModelBase
    {
        private int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

        private int? _vendorSysNo;
        public int? VendorSysNo
        {
            get { return _vendorSysNo; }
            set
            {
                base.SetValue("VendorSysNo", ref _vendorSysNo, value);
            }
        }

        private int? _stockSysNo;
        public int? StockSysNo
        {
            get { return _stockSysNo; }
            set
            {
                base.SetValue("StockSysNo", ref _stockSysNo, value);
            }
        }

        private decimal? _totalAmt;
        public decimal? TotalAmt
        {
            get { return _totalAmt; }
            set
            {
                base.SetValue("TotalAmt", ref _totalAmt, value);
            }
        }

        private DateTime? _createTime;
        public DateTime? CreateTime
        {
            get { return _createTime; }
            set
            {
                base.SetValue("CreateTime", ref _createTime, value);
            }
        }

        private int? _createUserSysNo;
        public int? CreateUserSysNo
        {
            get { return _createUserSysNo; }
            set
            {
                base.SetValue("CreateUserSysNo", ref _createUserSysNo, value);
            }
        }

        private DateTime? _auditTime;
        public DateTime? AuditTime
        {
            get { return _auditTime; }
            set
            {
                base.SetValue("AuditTime", ref _auditTime, value);
            }
        }

        private int? _auditUserSysNo;
        public int? AuditUserSysNo
        {
            get { return _auditUserSysNo; }
            set
            {
                base.SetValue("AuditUserSysNo", ref _auditUserSysNo, value);
            }
        }

        private DateTime? _editTime;
        public DateTime? EditTime
        {
            get { return _editTime; }
            set
            {
                base.SetValue("EditTime", ref _editTime, value);
            }
        }

        private int? _editUserSysNo;
        public int? EditUserSysNo
        {
            get { return _editUserSysNo; }
            set
            {
                base.SetValue("EditUserSysNo", ref _editUserSysNo, value);
            }
        }

        private POSettleStatus? status;

        public POSettleStatus? Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        public string CompanyCode { get; set; }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                base.SetValue("IsChecked", ref _isChecked, value);
            }
        }

        private decimal _RateAmount;
        /// <summary>
        /// 应付税金
        /// </summary>
        public decimal RateAmount
        {
            get { return _RateAmount; }
            set
            {
                base.SetValue("RateAmount", ref _RateAmount, value);
            }
        }

        private decimal _rateTotal;
        /// <summary>
        /// 应付价税合计
        /// </summary>
        public decimal RateTotal
        {
            get { return _rateTotal; }
            set
            {
                base.SetValue("RateTotal", ref _rateTotal, value);
            }
        }

        private decimal _RateAmountOther;
        /// <summary>
        /// 税金(其他)
        /// </summary>
        public decimal RateAmountOther
        {
            get { return _RateAmountOther; }
            set
            {
                base.SetValue("RateAmountOther", ref _RateAmountOther, value);
            }
        }

        private decimal _RateCost13;
        /// <summary>
        /// 税金(13%)
        /// </summary>
        public decimal RateCost13
        {
            get { return _RateCost13; }
            set
            {
                base.SetValue("RateCost13", ref _RateCost13, value);
            }
        }

        private decimal _RateCost17;
        /// <summary>
        /// 税金(17%)
        /// </summary>
        public decimal RateCost17
        {
            get { return _RateCost17; }
            set
            {
                base.SetValue("RateCost17", ref _RateCost17, value);
            }
        }

        private decimal _Cost17;
        /// <summary>
        /// 价款(17%)
        /// </summary>
        public decimal Cost17
        {
            get { return _Cost17; }
            set
            {
                base.SetValue("Cost17", ref _Cost17, value);
            }
        }

        private decimal _Cost13;
        /// <summary>
        /// 价款(13%)
        /// </summary>
        public decimal Cost13
        {
            get { return _Cost13; }
            set
            {
                base.SetValue("Cost13", ref _Cost13, value);
            }
        }

        private decimal _CostOther;
        /// <summary>
        /// 价款(其它)
        /// </summary>
        public decimal CostOther
        {
            get { return _CostOther; }
            set
            {
                base.SetValue("CostOther", ref _CostOther, value);
            }
        }
    }

    public class SettleItemInfoVM : ModelBase
    {
        private int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

        private int? _settleSysNo;
        public int? SettleSysNo
        {
            get { return _settleSysNo; }
            set
            {
                base.SetValue("SettleSysNo", ref _settleSysNo, value);
            }
        }

        private decimal? _taxRate;
        public decimal? TaxRate
        {
            get { return _taxRate; }
            set
            {
                base.SetValue("TaxRate", ref _taxRate, value);
            }
        }

        private decimal? _payAmt;
        public decimal? PayAmt
        {
            get { return _payAmt; }
            set
            {
                base.SetValue("PayAmt", ref _payAmt, value);
            }
        }

        public string CompanyCode { get; set; }
    }
}
