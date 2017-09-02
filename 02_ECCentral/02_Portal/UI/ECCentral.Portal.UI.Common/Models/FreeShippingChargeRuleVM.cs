using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.Models
{
    public class FreeShippingChargeRuleVM : ModelBase
    {
        public FreeShippingChargeRuleVM()
        {
            m_PayTypeSettingValue = new ObservableCollection<SimpleObject>();
            m_ShipAreaSettingValue = new ObservableCollection<SimpleObject>();
            m_ProductSettingValue = new ObservableCollection<SimpleObject>();
            m_AmountSettingTypeList = EnumConverter.GetKeyValuePairs<FreeShippingAmountSettingType>(EnumConverter.EnumAppendItemType.Select);
        }

        private int? m_SysNo;

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo
        {
            get { return m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private DateTime? m_StartDate;
        /// <summary>
        /// 开始日期
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? StartDate
        {
            get { return m_StartDate; }
            set { this.SetValue("StartDate", ref m_StartDate, value); }
        }

        private DateTime? m_EndDate;
        /// <summary>
        /// 截止日期
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get { return m_EndDate; }
            set { this.SetValue("EndDate", ref m_EndDate, value); }
        }

        private bool? m_IsGlobal;
        /// <summary>
        /// 是否是全网商品
        /// </summary>
        public bool? IsGlobal
        {
            get { return m_IsGlobal; }
            set { this.SetValue("IsGlobal", ref m_IsGlobal, value); }
        }

        private FreeShippingAmountSettingType? m_AmountSettingType;
        /// <summary>
        /// 免运费条件金额类型
        /// </summary>
        [Validate(ValidateType.Required)]
        public FreeShippingAmountSettingType? AmountSettingType
        {
            get { return m_AmountSettingType; }
            set { this.SetValue("AmountSettingType", ref m_AmountSettingType, value); }
        }

        private FreeShippingAmountSettingStatus? m_Status;
        /// <summary>
        /// 状态
        /// </summary>
        public FreeShippingAmountSettingStatus? Status
        {
            get { return m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private string m_AmountSettingValue;
        /// <summary>
        /// 免运费条件的门槛金额
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]+(\d{1,8})?(\.(\d){1,2})?$")]
        public string AmountSettingValue
        {
            get { return m_AmountSettingValue; }
            set { this.SetValue("AmountSettingValue", ref m_AmountSettingValue, value); }
        }

        private ObservableCollection<SimpleObject> m_PayTypeSettingValue;
        /// <summary>
        /// 支付类型
        /// </summary>
        public ObservableCollection<SimpleObject> PayTypeSettingValue
        {
            get
            {
                if (m_PayTypeSettingValue == null)
                {
                    m_PayTypeSettingValue = new ObservableCollection<SimpleObject>();
                }
                return m_PayTypeSettingValue;
            }
            set { this.SetValue("PayTypeSettingValue", ref m_PayTypeSettingValue, value); }
        }

        private ObservableCollection<SimpleObject> m_ShipAreaSettingValue;
        /// <summary>
        /// 配送区域
        /// </summary>
        public ObservableCollection<SimpleObject> ShipAreaSettingValue
        {
            get
            {
                if (m_ShipAreaSettingValue == null)
                {
                    m_ShipAreaSettingValue = new ObservableCollection<SimpleObject>();
                }
                return m_ShipAreaSettingValue;
            }
            set { this.SetValue("ShipAreaSettingValue", ref m_ShipAreaSettingValue, value); }
        }


        private ObservableCollection<SimpleObject> m_ProductSettingValue;
        /// <summary>
        ///  免运费商品
        /// </summary>
        public ObservableCollection<SimpleObject> ProductSettingValue
        {
            get {
                if (m_ProductSettingValue == null)
                {
                    m_ProductSettingValue = new ObservableCollection<SimpleObject>();
                }
                return m_ProductSettingValue;
            }
            set { this.SetValue("ProductSettingValue", ref m_ProductSettingValue, value); }
        }


        private string m_Description;
        /// <summary>
        /// 规则描述
        /// </summary>
        [Validate(ValidateType.MaxLength, 350)]
        public string Description
        {
            get { return m_Description; }
            set { this.SetValue("Description", ref m_Description, value); }
        }

        private List<KeyValuePair<FreeShippingAmountSettingType?, string>> m_AmountSettingTypeList;
        /// <summary>
        /// 金额类型
        /// </summary>
        public List<KeyValuePair<FreeShippingAmountSettingType?, string>> AmountSettingTypeList
        {
            get { return m_AmountSettingTypeList; }
        }
    }
}