using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using System.Linq;
using System.Collections.Generic;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CommissionTypeQueryVM : ModelBase
    {
        public List<KeyValuePair<HYNStatus?, string>> ListIsOnLineShow { get; set; }
        public CommissionTypeQueryVM()
        {
            this.ListIsOnLineShow = EnumConverter.GetKeyValuePairs<HYNStatus>(EnumConverter.EnumAppendItemType.All);
            this.PagingInfo = new PagingInfo();
        }

        public PagingInfo PagingInfo { get; set; }
        private string sysNo;
        [Validate(ValidateType.Interger)]
        public string SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }

        private string commissionTypeID;
        public string CommissionTypeID
        {
            get
            {
                return commissionTypeID;
            }
            set
            {
                SetValue("CommissionTypeID", ref commissionTypeID, value);
            }
        }

        private string commissionTypeName;
        public string CommissionTypeName
        {
            get
            {
                return commissionTypeName;
            }
            set
            {
                SetValue("CommissionTypeName", ref commissionTypeName, value);
            }
        }
        private HYNStatus? m_isOnlineShow;
        public HYNStatus? IsOnlineShow
        {
            get { return m_isOnlineShow; }
            set
            {
                SetValue("IsOnlineShow", ref m_isOnlineShow, value);
            }
        }
        #region 扩展属性
        private string organizationID;
        public string OrganizationID
        {
            get
            {
                return organizationID;
            }
            set
            {
                SetValue("OrganizationID", ref organizationID, value);
            }
        }
        private string organizationName;
        public string OrganizationName
        {
            get
            {
                return organizationName;
            }
            set
            {
                SetValue("OrganizationName", ref organizationName, value);
            }
        }
        #endregion
    }
}
