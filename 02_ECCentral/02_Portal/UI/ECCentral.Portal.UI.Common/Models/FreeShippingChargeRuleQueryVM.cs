using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.Models
{
    public class FreeShippingChargeRuleQueryVM : ModelBase
    {
        private DateTime? m_StartDate;
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate
        {
            get { return m_StartDate; }
            set { this.SetValue("StartDate", ref m_StartDate, value); }
        }

        private DateTime? m_EndDate;
        /// <summary>
        /// 截至日期
        /// </summary>
        public DateTime? EndDate
        {
            get { return m_EndDate; }
            set { this.SetValue("EndDate", ref m_EndDate, value); }
        }

        private FreeShippingAmountSettingType? m_AmountSettingType;
        /// <summary>
        /// 免运费条件金额类型
        /// </summary>
        public FreeShippingAmountSettingType? AmountSettingType
        {
            get { return m_AmountSettingType; }
            set { this.SetValue("AmountSettingType", ref m_AmountSettingType, value); }
        }

        private FreeShippingAmountSettingStatus? m_Status;
        /// <summary>
        /// 免运费规则状态
        /// </summary>
        public FreeShippingAmountSettingStatus? Status
        {
            get { return m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private string m_AmtFrom;
        /// <summary>
        /// 门槛金额起始值
        /// </summary>
        [Validate(ValidateType.Regex, @"^[0-9]+(\d{1,8})?(\.(\d){1,2})?$")]
        public string AmtFrom
        {
            get { return m_AmtFrom; }
            set { this.SetValue("AmtFrom", ref m_AmtFrom, value); }
        }

        private string m_AmtTo;
        /// <summary>
        /// 门槛金额终止值
        /// </summary>
        [Validate(ValidateType.Regex, @"^[0-9]+(\d{1,8})?(\.(\d){1,2})?$")]
        public string AmtTo
        {
            get { return m_AmtTo; }
            set { this.SetValue("AmtTo", ref m_AmtTo, value); }
        }

        private int? m_PayTypeSysNo;
        /// <summary>
        /// 支付方式编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get { return m_PayTypeSysNo; }
            set { this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value); }
        }

        private int? m_ShipAreaID;
        /// <summary>
        /// 配送区域编号
        /// </summary>
        public int? ShipAreaID
        {
            get { return m_ShipAreaID; }
            set { this.SetValue("ShipAreaID", ref m_ShipAreaID, value); }
        }

        /// <summary>
        /// 金额类型
        /// </summary>
        public List<KeyValuePair<FreeShippingAmountSettingType?, string>> AmountSettingTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<FreeShippingAmountSettingType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public List<KeyValuePair<FreeShippingAmountSettingStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<FreeShippingAmountSettingStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }
    }
}