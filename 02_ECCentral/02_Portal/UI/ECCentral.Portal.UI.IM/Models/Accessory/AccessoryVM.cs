
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class AccessoryVM : ModelBase
    {
        /// <summary>
        /// 配件名称
        /// </summary>
        private string _accessoryID;
        public string AccessoryID
        {
            get { return _accessoryID; }
            set { base.SetValue("AccessoryID", ref _accessoryID, value); }
        }

        /// <summary>
        /// 配件名称
        /// </summary>
        public string AccessoryName { get; set; }

        private int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { base.SetValue("SysNo", ref _sysNo, value); }
        }

        public bool HasAccessoryMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Accessory_AccessoryMaintain); }
        }
    }
}
