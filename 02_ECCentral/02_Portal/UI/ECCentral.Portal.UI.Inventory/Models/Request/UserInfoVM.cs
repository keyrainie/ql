using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class UserInfoVM : ModelBase
    {
        private int? sysNo;
        public int? SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                base.SetValue("SysNo", ref sysNo, value);
            }
        }

        private string userID;
        public string UserID
        {
            get
            {
                return userID;
            }
            set
            {
                base.SetValue("UserID", ref userID, value);
            }
        }

        private string userName;
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                base.SetValue("UserName", ref userName, value);
            }
        }

        private string userDisplayName;
        public string UserDisplayName
        {
            get
            {
                return userDisplayName;
            }
            set
            {
                base.SetValue("UserDisplayName", ref userDisplayName, value);
            }
        }
    }
}
