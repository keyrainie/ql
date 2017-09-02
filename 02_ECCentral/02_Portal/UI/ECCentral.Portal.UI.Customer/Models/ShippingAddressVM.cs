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
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Customer.Models
{
    /// <summary>
    /// 收货地址视图模型
    /// </summary>
    public class ShippingAddressVM : ModelBase
    {
        private int? m_SysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get
            {
                return m_SysNo;
            }
            set
            {
                base.SetValue("SysNo", ref m_SysNo, value);
            }
        }

        private string m_CustomerID;
        /// <summary>
        /// 收货人顾客ID
        /// </summary>
        public string CustomerID
        {
            get
            {
                return m_CustomerID;
            }
            set
            {
                base.SetValue("CustomerID", ref m_CustomerID, value);
            }
        }

        private int? m_CustomerSysNo;
        /// <summary>
        /// 收货人系统编号
        /// </summary>
        public int? CustomerSysNo
        {
            get
            {
                return m_CustomerSysNo;
            }
            set
            {
                base.SetValue("CustomerSysNo", ref m_CustomerSysNo, value);
            }
        }

        private string m_ReceiveName;
        /// <summary>
        /// 收货人名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ReceiveName
        {
            get
            {
                return m_ReceiveName;
            }
            set
            {
                base.SetValue("ReceiveName", ref m_ReceiveName, value);
            }
        }

        private string m_ReceiveContact;
        /// <summary>
        /// 联系人名称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ReceiveContact
        {
            get
            {
                return m_ReceiveContact;
            }
            set
            {
                base.SetValue("ReceiveContact", ref m_ReceiveContact, value);
            }
        }

        private string m_ReceivePhone;
        /// <summary>
        /// 收货人电话
        /// </summary>
        [Validate(ValidateType.MaxLength, 20)]
        public string ReceivePhone
        {
            get
            {
                return m_ReceivePhone;
            }
            set
            {
                base.SetValue("ReceivePhone", ref m_ReceivePhone, value);
            }
        }

        private string m_ReceiveCellPhone;
        /// <summary>
        /// 收货人手机
        /// </summary>
        [Validate(ValidateType.MaxLength, 20)]
        public string ReceiveCellPhone
        {
            get
            {
                return m_ReceiveCellPhone;
            }
            set
            {
                base.SetValue("ReceiveCellPhone", ref m_ReceiveCellPhone, value);
            }
        }

        private string m_ReceiveFax;
        /// <summary>
        /// 收货人传真
        /// </summary>
        [Validate(ValidateType.MaxLength, 20)]
        public string ReceiveFax
        {
            get
            {
                return m_ReceiveFax;
            }
            set
            {
                base.SetValue("ReceiveFax", ref m_ReceiveFax, value);
            }
        }

        private string m_ReceiveZip;
        /// <summary>
        /// 收货人邮编
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.ZIP)]
        public string ReceiveZip
        {
            get
            {
                return m_ReceiveZip;
            }
            set
            {
                base.SetValue("ReceiveZip", ref m_ReceiveZip, value);
            }
        }

        private string m_ReceiveAreaSysNo;
        /// <summary>
        /// 收货人地区
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ReceiveAreaSysNo
        {
            get
            {
                return m_ReceiveAreaSysNo;
            }
            set
            {
                base.SetValue("ReceiveAreaSysNo", ref m_ReceiveAreaSysNo, value);
            }
        }

        private string m_ReceiveAddress;
        /// <summary>
        /// 收货人地址
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ReceiveAddress
        {
            get
            {
                return m_ReceiveAddress;
            }
            set
            {
                base.SetValue("ReceiveAddress", ref m_ReceiveAddress, value);
            }
        }

        private string m_AddressTitle;
        /// <summary>
        /// 收货地址简称
        /// </summary>
        [Validate(ValidateType.Required)]
        public string AddressTitle
        {
            get
            {
                return m_AddressTitle;
            }
            set
            {
                base.SetValue("AddressTitle", ref m_AddressTitle, value);
            }
        }

        private bool? m_IsDefault;
        /// <summary>
        /// 是否是默认收货地址
        /// </summary>
        public bool? IsDefault
        {
            get
            {
                return m_IsDefault;
            }
            set
            {
                base.SetValue("IsDefault", ref m_IsDefault, value);
            }
        }

        private List<KeyValuePair<YNStatus?, string>> m_YNList;
        public List<KeyValuePair<YNStatus?, string>> YNList
        {
            get
            {
                if (m_YNList == null)
                {
                    m_YNList = EnumConverter.GetKeyValuePairs<YNStatus>();
                }
                return m_YNList;
            }
        }
    }
}