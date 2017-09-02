using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class AdvertisingQueryVM : ModelBase
    {
        public AdvertisingQueryVM()
        {
            this.AdvertisingTypeList = EnumConverter.GetKeyValuePairs<AdvertisingType>(EnumConverter.EnumAppendItemType.All);
        }


        public int? ProductLineCategorySysNo { get; set; }

        public int? ProductLineSysNo { get; set; }

        public int? ImageWidth { get; set; }

        public int? ImageHeight { get; set; }

        private AdvertisingType? _type;
        public AdvertisingType? Type
        {
            get { return _type; }
            set { base.SetValue("Type", ref _type, value); }
        }

        public DateTime? OperateDateFrom { get; set; }

        public DateTime? OperateDateTo { get; set; }

        private string _inUser;
        public string InUser
        {
            get { return _inUser; }
            set { SetValue("InUser", ref _inUser, value); }
        }

        public List<KeyValuePair<AdvertisingType?, string>> AdvertisingTypeList { get; set; }

        public string ImageSize
        {
            get
            {
                return this.ImageWidth + "*" + this.ImageHeight;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != "*")
                {
                    string[] dimission = value.Split('*');
                    this.ImageWidth = int.Parse(dimission[0]);
                    this.ImageHeight = int.Parse(dimission[1]);
                }
            }
        }

    }
}
