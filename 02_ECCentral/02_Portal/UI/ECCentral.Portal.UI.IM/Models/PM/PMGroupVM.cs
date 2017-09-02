using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class PMGroupVM : ModelBase
    {

        public List<KeyValuePair<int, string>> PMGroupStatusList { get; set; }

        public PMGroupVM()
        {
            List<KeyValuePair<int, string>> statusList = new List<KeyValuePair<int, string>>();

            statusList.Add(new KeyValuePair<int, string>(0, ResCategoryKPIMaintain.SelectTextValid));
            statusList.Add(new KeyValuePair<int, string>(-1, ResCategoryKPIMaintain.SelectTextInvalid));

            this.PMGroupStatusList = statusList;
        }

        private int _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo
        {
            get
            {
                return _sysNo;
            }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

        public string PMUserName
        {
            get;
            set;
        }

        private string _status = "-1";

        public string Status
        {
            get { return _status; }
            set
            {
                bool result = Enum.IsDefined(typeof(PMGroupStatus), Convert.ToInt32(value));
                if (result)
                {
                    PMGroupStatus status;
                    Enum.TryParse(value, out status);
                    _status = EnumExtension.ToDescription(status);
                }
                else
                {
                    _status = "";
                }
            }
        }

        public string PMGroupName
        {
            get;
            set;
        }

        public string PMGroupManager
        {
            get;
            set;
        }

        public string Selected
        {
            get;
            set;
        }

        /// <summary>
        /// PM组组长
        /// </summary>
        private string pMUserSysNo;
        [Validate(ValidateType.Required)]
        public string PMUserSysNo
        {
            get { return pMUserSysNo; }
            set { SetValue("PMUserSysNo", ref pMUserSysNo, value); }
        }

        public List<ProductManagerInfo> PMList
        {
            get;
            set;
        }

        public bool HasPMGroupMaintainPermission
        {
            get {return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_PM_PMGroupMaintain);}
        }
    }
}
