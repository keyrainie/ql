using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Models
{
    public class BalanceAccountQueryVM : ModelBase
    {
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

        private String m_CustomerID;
        public String CustomerID
        {
            get
            {
                return this.m_CustomerID;
            }
            set
            {
                this.SetValue("CustomerID", ref m_CustomerID, value);
            }
        }

        private DateTime? m_CreateTimeFrom;
        public DateTime? CreateTimeFrom
        {
            get
            {
                return this.m_CreateTimeFrom;
            }
            set
            {
                this.SetValue("CreateTimeFrom", ref m_CreateTimeFrom, value);
            }
        }

        private DateTime? m_CreateTimeTo;
        public DateTime? CreateTimeTo
        {
            get
            {
                return this.m_CreateTimeTo;
            }
            set
            {
                this.SetValue("CreateTimeTo", ref m_CreateTimeTo, value);
            }
        }

        private BalanceAccountDetailType? m_DetailType;
        public BalanceAccountDetailType? DetailType
        {
            get
            {
                return this.m_DetailType;
            }
            set
            {
                this.SetValue("DetailType", ref m_DetailType, value);
            }
        }

        private String m_WebChannelID;
        public String WebChannelID
        {
            get
            {
                return this.m_WebChannelID;
            }
            set
            {
                this.SetValue("WebChannelID", ref m_WebChannelID, value);
            }
        }

        #region 扩展属性

        public List<KeyValuePair<BalanceAccountDetailType?, string>> DetailTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<BalanceAccountDetailType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        public List<WebChannelVM> WebChannelList
        {
            get
            {
                var webchannleList = CPApplication.Current.CurrentWebChannelList.Convert<UIWebChannel, WebChannelVM>();
                webchannleList.Insert(0, new WebChannelVM()
                {
                    ChannelName = ResCommonEnum.Enum_All
                });
                return webchannleList;
            }
        }

        #endregion 扩展属性
    }
}