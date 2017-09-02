
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ManufacturerQueryVM : ModelBase
    {
        public ManufacturerQueryVM()
        {
            this.ManufacturerStatusList = EnumConverter.GetKeyValuePairs<ManufacturerStatus>(EnumConverter.EnumAppendItemType.All);
        }

        private string manufacturerNameLocal;
        public string ManufacturerNameLocal
        {
            get
            {
                return manufacturerNameLocal;
            }
            set
            {
                base.SetValue("ManufacturerNameLocal", ref manufacturerNameLocal, value);
            }
        }

        private string manufacturerNameGlobal;
        public string ManufacturerNameGlobal
        {
            get
            {
                return manufacturerNameGlobal;
            }
            set
            {
                base.SetValue("ManufacturerNameGlobal", ref manufacturerNameGlobal, value);
            }
        }

        private ManufacturerStatus? status;
        public ManufacturerStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        public List<KeyValuePair<ManufacturerStatus?, string>> ManufacturerStatusList { get; set; }

        public bool HasManufacturerRequestApplyPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Manufacturer_ManufacturerRequestApply); }
        }
    }
}
