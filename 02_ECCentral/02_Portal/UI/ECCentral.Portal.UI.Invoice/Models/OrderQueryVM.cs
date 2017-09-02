using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class OrderQueryVM : ModelBase
    {
        private SOIncomeOrderType m_OrderType;
        /// <summary>
        /// 单据类型
        /// </summary>
        public SOIncomeOrderType OrderType
        {
            get
            {
                return m_OrderType;
            }
            set
            {
                base.SetValue("OrderType", ref m_OrderType, value);
            }
        }

        private string m_OrderSysNo;
        /// <summary>
        /// 单据编号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, "^((\\d*)?|((\\s)*\\d*(\\.+(\\s)*\\d+)*(\\s)*)?)+$")]
        public string OrderSysNo
        {
            get
            {
                return m_OrderSysNo;
            }
            set
            {
                base.SetValue("OrderSysNo", ref m_OrderSysNo, value);
            }
        }

        /// <summary>
        /// 单据类型列表
        /// </summary>
        public List<KeyValuePair<SOIncomeOrderType?, string>> OrderTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeOrderType>(EnumConverter.EnumAppendItemType.None);
            }
        }
    }
}