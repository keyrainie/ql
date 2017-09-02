using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorStoreInfoVM : ModelBase
    {
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        public int? VendorSysNo { get; set; }

        private string name;
        [Validate(ValidateType.Required)]
        public string Name
        {
            get { return name; }
            set { base.SetValue("Name", ref name, value); }
        }

        private int? areaSysNo;
        [Validate(ValidateType.Required)]
        public int? AreaSysNo
        {
            get { return areaSysNo; }
            set { base.SetValue("AreaSysNo", ref areaSysNo, value); }
        }

        private string address;
        [Validate(ValidateType.Required)]
        public string Address
        {
            get { return address; }
            set { base.SetValue("Address", ref address, value); }
        }

        private string mapAddress;
        public string MapAddress
        {
            get { return mapAddress; }
            set { base.SetValue("MapAddress", ref mapAddress, value); }
        }

        private string telephone;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Phone)]
        public string Telephone
        {
            get { return telephone; }
            set { base.SetValue("Telephone", ref telephone, value); }
        }

        private string otherContact;
        public string OtherContact
        {
            get { return otherContact; }
            set { base.SetValue("OtherContact", ref otherContact, value); }
        }

        private DateTime? openingHoursFrom;
        [Validate(ValidateType.Required)]
        public DateTime? OpeningHoursFrom
        {
            get { return openingHoursFrom; }
            set { base.SetValue("OpeningHoursFrom", ref openingHoursFrom, value); }
        }

        private DateTime? openingHoursTo;
        [Validate(ValidateType.Required)]
        public DateTime? OpeningHoursTo
        {
            get { return openingHoursTo; }
            set { base.SetValue("OpeningHoursTo", ref openingHoursTo, value); }
        }

        private string cityBusLine;
        [Validate(ValidateType.Required)]
        public string CityBusLine
        {
            get { return cityBusLine; }
            set { base.SetValue("CityBusLine", ref cityBusLine, value); }
        }

        private string carPark;
        [Validate(ValidateType.Required)]
        public string CarPark
        {
            get { return carPark; }
            set { base.SetValue("CarPark", ref carPark, value); }
        }

        private string floorSetting;
        [Validate(ValidateType.Required)]
        public string FloorSetting
        {
            get { return floorSetting; }
            set { base.SetValue("FloorSetting", ref floorSetting, value); }
        }
    }
}
