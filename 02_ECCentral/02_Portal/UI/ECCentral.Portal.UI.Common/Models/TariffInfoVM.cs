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
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.Models
{
    public class TariffInfoVM : ModelBase
    {
        public TariffInfoVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<TariffStatus>();
        }

        private int? _SysNo;
        public int? SysNo
        {
            get { return _SysNo; }
            set { base.SetValue("SysNo", ref _SysNo, value); }
        }


        private int? _ParentSysNo;
        [Validate(ValidateType.Required)]
        public int? ParentSysNo
        {
            get { return _ParentSysNo; }
            set { base.SetValue("ParentSysNo", ref _ParentSysNo, value); }
        }


        private string _TariffCode;
        [Validate(ValidateType.Required)]
        public string TariffCode
        {
            get { return _TariffCode; }
            set { base.SetValue("TariffCode", ref _TariffCode, value); }
        }

        private string _ItemCategoryName;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 100)]
        public string ItemCategoryName
        {
            get { return _ItemCategoryName; }
            set { base.SetValue("ItemCategoryName", ref _ItemCategoryName, value); }
        }


        private TariffStatus? _Status;
        public TariffStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }


        private string _Unit;
        public string Unit
        {
            get { return _Unit; }
            set { base.SetValue("Unit", ref _Unit, value); }
        }

        private string _TariffPrice;
        public string TariffPrice
        {
            get { return _TariffPrice; }
            set { base.SetValue("TariffPrice", ref _TariffPrice, value); }
        }

        private string _TariffRate;
        public string TariffRate
        {
            get { return _TariffRate; }
            set { base.SetValue("TariffRate", ref _TariffRate, value); }
        }

        private int? _InUserSysNo;
        public int? InUserSysNo
        {
            get { return _InUserSysNo; }
            set { base.SetValue("InUserSysNo", ref _InUserSysNo, value); }
        }

        private string _InUserName;
        public string InUserName
        {
            get { return _InUserName; }
            set { base.SetValue("InUserName", ref _InUserName, value); }
        }

        private DateTime? _InDate;
        public DateTime? InDate
        {
            get { return _InDate; }
            set { base.SetValue("InDate", ref _InDate, value); }
        }



        private string _EditUserSysNo;
        public string EditUserSysNo
        {
            get { return _EditUserSysNo; }
            set { base.SetValue("EditUserSysNo", ref _EditUserSysNo, value); }
        }

        private string _EditUserName;
        public string EditUserName
        {
            get { return _EditUserName; }
            set { base.SetValue("EditUserName", ref _EditUserName, value); }
        }

        private DateTime? _EditDate;
        public DateTime? EditDate
        {
            get { return _EditDate; }
            set { base.SetValue("EditDate", ref _EditDate, value); }
        }

        private string _Tcode;
        public string Tcode
        {
            get { return _Tcode; }
            set { base.SetValue("Tcode", ref _Tcode, value); }
        }
      
        private string _tCode1;
        public string TCode1
        {
            get { return _tCode1; }
            set { base.SetValue("TCode1", ref _tCode1, value); }
        }

        private string _tCode2;
        public string TCode2
        {
            get { return _tCode2; }
            set { base.SetValue("TCode2", ref _tCode2, value); }
        }
        private string _tCode3;
        public string TCode3
        {
            get { return _tCode3; }
            set { base.SetValue("TCode3", ref _tCode3, value); }
        }
        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<TariffStatus?, string>> StatusList { get; set; }

    }
}
