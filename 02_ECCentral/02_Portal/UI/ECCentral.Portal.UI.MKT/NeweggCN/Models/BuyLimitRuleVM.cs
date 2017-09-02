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
    public class BuyLimitRuleVM : ModelBase
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

        private LimitType _limitType;
        /// <summary>
        /// 限购类型,0-单品限购，1-套餐限购
        /// </summary>
        public LimitType LimitType
        {
            get { return _limitType; }
            set
            {
                base.SetValue("LimitType", ref _limitType, value);
            }
        }


        private string _productSysNo;
        /// <summary>
        /// 如果是单品限购就是商品系统编号，如果是套餐限购就是套餐活动系统编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }

        private string _comboSysNo;
        /// <summary>
        /// 套餐系统编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string ComboSysNo
        {
            get { return _comboSysNo; }
            set
            {
                base.SetValue("ComboSysNo", ref _comboSysNo, value);
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
        private string _memberRanks;
        /// <summary>
        /// 限购的会员等级(多个项目用逗号分隔)
        /// </summary>
        public string MemberRanks
        {
            get { return _memberRanks; }
            set
            {
                base.SetValue("MemberRanks", ref _memberRanks, value);
            }
        }
        private string _memberCardTypes;
        /// <summary>
        /// 限购的会员卡类型(多个项目用逗号分隔)
        /// </summary>
        public string MemberCardTypes
        {
            get { return _memberCardTypes; }
            set
            {
                base.SetValue("MemberCardTypes", ref _memberCardTypes, value);
            }
        }
        private string _minQty;
        /// <summary>
        /// 限购下限
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
        /// 限购上限
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
        private string _orderTimes;
        /// <summary>
        /// 每天最大下单次数
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string OrderTimes
        {
            get { return _orderTimes; }
            set
            {
                base.SetValue("OrderTimes", ref _orderTimes, value);
            }
        }
        private LimitStatus _status;
        /// <summary>
        /// 状态：0-无效，1-有效
        /// </summary>
        public LimitStatus Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }

        #region UI Properties

        private string _uiProductID;
        /// <summary>
        /// 商品ID(用于维护界面上显示)
        /// </summary>
        public string UIProductID
        {
            get { return _uiProductID; }
            set
            {
                base.SetValue("UIProductID", ref _uiProductID, value);
            }
        }

        #endregion
    }
}
