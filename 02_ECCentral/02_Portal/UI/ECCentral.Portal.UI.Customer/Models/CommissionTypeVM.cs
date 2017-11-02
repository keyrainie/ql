using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CommissionTypeVM : ModelBase
    {
        public CommissionTypeVM()
        {
            this.ListCommissionStatus = EnumConverter.GetKeyValuePairs<SYNStatus>();
            //this.ListIsPayWhenRev = EnumConverter.GetKeyValuePairs<SYNStatus>();
            //this.ListIsNet = EnumConverter.GetKeyValuePairs<SYNStatus>();
            //this.ListNetPayType = EnumConverter.GetKeyValuePairs<NetPayType>();
           // this.ListCommissionStatus = EnumConverter.GetKeyValuePairs<HYNStatus>();
            this.CommissionStatus = true;
        }

        /// <summary>
        /// List区域
        /// </summary>
        public List<KeyValuePair<SYNStatus?, string>> ListCommissionStatus { get; set; }
        //public List<KeyValuePair<SYNStatus?, string>> ListIsPayWhenRev { get; set; }
        //public List<KeyValuePair<SYNStatus?, string>> ListIsNet { get; set; }
        //public List<KeyValuePair<NetPayType?, string>> ListNetPayType { get; set; }

        //public List<KeyValuePair<SYNStatus?, string>> ListCommissionStatus { get; set; }

        public int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }

        public string _commissionTypeID;
        /// <summary>
        /// 返佣方式编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CommissionTypeID
        {
            get { return _commissionTypeID; }
            set { SetValue("CommissionTypeID", ref _commissionTypeID, value); }
        }

        private string _commissionTypeName;
        /// <summary>
        /// 返佣方式名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CommissionTypeName
        {
            get { return _commissionTypeName; }
            set { SetValue("CommissionTypeName", ref _commissionTypeName, value); }
        }

        private string _commissionTypeDesc;
        /// <summary>
        /// 返佣方式描述
        /// </summary>
        public string CommissionTypeDesc
        {
            get { return _commissionTypeDesc; }
            set { SetValue("CommissionTypeDesc", ref _commissionTypeDesc, value); }
        }

        private string _commissionRate;
        /// <summary>
        /// 手续费率 [Validate(ValidateType.Regex, @"^(0|1|1\.[0]*|0?\.(?!0+$)[\d]+)$")]
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CommissionRate
        {
            get { return _commissionRate; }
            set { SetValue("CommissionRate", ref _commissionRate, value); }
        }

        private string _lowerLimit;
        /// <summary>
        /// 上限 [Validate(ValidateType.Regex, @"^(0|1|1\.[0]*|0?\.(?!0+$)[\d]+)$")]
        /// </summary>
        [Validate(ValidateType.Required)]
        public string LowerLimit
        {
            get { return _lowerLimit; }
            set { SetValue("LowerLimit", ref _lowerLimit, value); }
        }

        private string _upperLimit;
        /// <summary>
        /// 下限 [Validate(ValidateType.Regex, @"^(0|1|1\.[0]*|0?\.(?!0+$)[\d]+)$")]
        /// </summary>
        [Validate(ValidateType.Required)]
        public string UpperLimit
        {
            get { return _upperLimit; }
            set { SetValue("UpperLimit", ref _upperLimit, value); }
        }



        /// <summary>
        /// 状态
        /// </summary>
        public bool? CommissionStatus { get; set; }
        /// <summary>
        /// 状态 ： 正常，停用
        /// </summary>
        public SYNStatus? CommissionStatusNeum
        {
            get
            {
                return (CommissionStatus.HasValue && CommissionStatus.Value) ? SYNStatus.Yes : SYNStatus.No;
            }
            set
            {
                CommissionStatus = value.HasValue ? (value.Equals(SYNStatus.Yes) ? true : false) : (bool?)null;
            }
        }

        private string m_CommissionOrder;
        /// <summary>
        /// 显示优先级
        /// </summary>
        public string CommissionOrder
        {
            get { return this.m_CommissionOrder; }
            set { this.SetValue("CommissionOrder", ref m_CommissionOrder, value); }
        }

       

        #region 扩展属性

        private bool isEdit;
        /// <summary>
        /// 用于界面控制是否编辑状态
        /// </summary>
        public bool IsEdit
        {
            get { return isEdit; }
            set { SetValue("IsEdit", ref isEdit, value); }
        }

        private bool isNetPay;
        /// <summary>
        /// 用于控制支付类型的 显示|隐藏
        /// </summary>
        public bool IsNetPay
        {
            get { return isNetPay; }
            set { SetValue("IsNetPay", ref isNetPay, value); }
        }
        #endregion
    }
}
