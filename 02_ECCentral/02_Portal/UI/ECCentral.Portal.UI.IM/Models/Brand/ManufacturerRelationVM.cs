
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ManufacturerRelationVM : ModelBase
    {
        private int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { base.SetValue("SysNo", ref _sysNo, value); }
        }

        private int? _localManufacturerSysNo;
        public int? LocalManufacturerSysNo
        {
            get { return _localManufacturerSysNo; }
            set { base.SetValue("LocalManufacturerSysNo", ref _localManufacturerSysNo, value); }
        }


        private string neweggManufacturer;
        [Validate(ValidateType.MaxLength, 100)]
        public string NeweggManufacturer
        {
            get { return neweggManufacturer; }
            set { base.SetValue("NeweggManufacturer", ref neweggManufacturer, value); }
        }

        private string amazonManufacturer;
        [Validate(ValidateType.MaxLength, 100)]
        public string AmazonManufacturer
        {
            get { return amazonManufacturer; }
            set { base.SetValue("AmazonManufacturer", ref amazonManufacturer, value); }
        }

        private string eBayManufacturer;
        [Validate(ValidateType.MaxLength, 100)]
        public string EBayManufacturer
        {
            get { return eBayManufacturer; }
            set { base.SetValue("EBayManufacturer", ref eBayManufacturer, value); }
        }


        private int? _otherManufacturerSysNo;
        [Validate(ValidateType.Interger)]
        public int? OtherManufacturerSysNo
        {
            get { return _otherManufacturerSysNo; }
            set { base.SetValue("OtherManufacturerSysNo", ref _otherManufacturerSysNo, value); }
        }
    }
}
