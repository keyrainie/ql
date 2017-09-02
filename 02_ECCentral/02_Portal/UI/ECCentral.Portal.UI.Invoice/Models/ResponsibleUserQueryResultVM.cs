using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class ResponsibleUserQueryResultVM : ModelBase
    {
        private List<ResponsibleUserVM> m_ResultList;
        public List<ResponsibleUserVM> ResultList
        {
            get
            {
                return m_ResultList.DefaultIfNull();
            }
            set
            {
                base.SetValue("ResultList", ref m_ResultList, value);
            }
        }

        /// <summary>
        /// 查询结果总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }
    }

    public class ResponsibleUserVM : ModelBase
    {
        private bool m_IsChecked;
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref m_IsChecked, value);
            }
        }

        public int? SysNo
        {
            get;
            set;
        }

        public string PayTypeName
        {
            get;
            set;
        }

        public string ShipTypeName
        {
            get;
            set;
        }

        private ResponseUserOrderStyle? m_IncomeStyle;
        [Validate(ValidateType.Required)]
        public ResponseUserOrderStyle? IncomeStyle
        {
            get
            {
                return m_IncomeStyle;
            }
            set
            {
                base.SetValue("IncomeStyle", ref m_IncomeStyle, value);
            }
        }

        private int? m_PayTypeSysNo;
        public int? PayTypeSysNo
        {
            get
            {
                return m_PayTypeSysNo;
            }
            set
            {
                base.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value);
            }
        }

        private int? m_ShipTypeSysNo;
        public int? ShipTypeSysNo
        {
            get
            {
                return m_ShipTypeSysNo;
            }
            set
            {
                base.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value);
            }
        }

        private string m_CustomerSysNo;

        [Validate(ValidateType.Interger)]
        //[Validate(ValidateType.Regex, "^\\d{0,6}$")]
        public string CustomerSysNo
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

        private string m_ResponsibleUserName;

        [Validate(ValidateType.Required)]
        public string ResponsibleUserName
        {
            get
            {
                return m_ResponsibleUserName;
            }
            set
            {
                base.SetValue("ResponsibleUserName", ref m_ResponsibleUserName, value);
            }
        }

        private string m_EmailAddress;

        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Email)]
        public string EmailAddress
        {
            get
            {
                return m_EmailAddress;
            }
            set
            {
                base.SetValue("EmailAddress", ref m_EmailAddress, value);
            }
        }

        private ResponseUserStatus? m_Status;

        [Validate(ValidateType.Required)]
        public ResponseUserStatus? Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                base.SetValue("Status", ref m_Status, value);
            }
        }

        private ResponsibleSource? m_SourceType;
        /// <summary>
        /// 来源方式
        /// 针对“款到发货”，系统自动判断“款到发货”类型的订单，
        /// 如果收款来源为网关自动记录，责任人自动配置为“Bank”；
        /// 如果收款来源为“人工添加”，责任人列示为人工配置的责任人名称
        /// </summary>
        public ResponsibleSource? SourceType
        {
            get
            {
                return m_SourceType;
            }
            set
            {
                base.SetValue("SourceType", ref m_SourceType, value);
            }
        }

        public string CompanyCode
        {
            get;
            set;
        }

        #region 扩展属性

        public bool EditIsEnabled
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_ARWindowConfig_CreateConfig);
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