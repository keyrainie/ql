using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.QueryFilter.Invoice
{
    public class ResponsibleUserQueryVM : ModelBase
    {
        public ResponsibleUserQueryVM()
        {
            IsSpecialMode = false;
        }

        private ResponseUserOrderStyle? m_IncomeStyle;
        public ResponseUserOrderStyle? IncomeStyle
        {
            get
            {
                return this.m_IncomeStyle;
            }
            set
            {
                this.SetValue("IncomeStyle", ref m_IncomeStyle, value);
            }
        }

        private Int32? m_PayTypeSysNo;
        public Int32? PayTypeSysNo
        {
            get
            {
                return this.m_PayTypeSysNo;
            }
            set
            {
                this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value);
            }
        }

        private String m_PayTypeName;
        public String PayTypeName
        {
            get
            {
                return this.m_PayTypeName;
            }
            set
            {
                this.SetValue("PayTypeName", ref m_PayTypeName, value);
            }
        }

        private Int32? m_ShipTypeSysNo;
        public Int32? ShipTypeSysNo
        {
            get
            {
                return this.m_ShipTypeSysNo;
            }
            set
            {
                this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value);
            }
        }

        private String m_ShipTypeName;
        public String ShipTypeName
        {
            get
            {
                return this.m_ShipTypeName;
            }
            set
            {
                this.SetValue("ShipTypeName", ref m_ShipTypeName, value);
            }
        }

        private Int32? m_CustomerSysNo;
        public Int32? CustomerSysNo
        {
            get
            {
                return this.m_CustomerSysNo;
            }
            set
            {
                this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value);
            }
        }

        private String m_ResponsibleUserName;
        public String ResponsibleUserName
        {
            get
            {
                return this.m_ResponsibleUserName;
            }
            set
            {
                this.SetValue("ResponsibleUserName", ref m_ResponsibleUserName, value);
            }
        }

        private String m_EmailAddress;
        public String EmailAddress
        {
            get
            {
                return this.m_EmailAddress;
            }
            set
            {
                this.SetValue("EmailAddress", ref m_EmailAddress, value);
            }
        }

        private ResponseUserStatus? m_Status;
        public ResponseUserStatus? Status
        {
            get
            {
                return this.m_Status;
            }
            set
            {
                this.SetValue("Status", ref m_Status, value);
            }
        }

        private Boolean? m_IsSpecialMode;
        public Boolean? IsSpecialMode
        {
            get
            {
                return this.m_IsSpecialMode;
            }
            set
            {
                this.SetValue("IsSpecialMode", ref m_IsSpecialMode, value);
            }
        }

        private ResponsibleSource? m_SourceType;
        public ResponsibleSource? SourceType
        {
            get
            {
                return this.m_SourceType;
            }
            set
            {
                this.SetValue("SourceType", ref m_SourceType, value);
            }
        }

        #region 扩展属性

        public string CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
            }
        }

        /// <summary>
        /// 收款单类型
        /// </summary>
        public List<KeyValuePair<ResponseUserOrderStyle?, string>> IncomeStyleList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ResponseUserOrderStyle>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public List<KeyValuePair<ResponseUserStatus?, string>> StatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ResponseUserStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 来源方式
        /// </summary>
        public List<KeyValuePair<ResponsibleSource?, string>> SourceTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ResponsibleSource>(EnumConverter.EnumAppendItemType.All);
            }
        }

        #endregion 扩展属性
    }
}