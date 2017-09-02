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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using System.Linq;
namespace ECCentral.Portal.Basic.Components.UserControls.TariffPicker
{
    public class TariffInfoVM : ModelBase
    {


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
        private string _Tcode;
        [Validate(ValidateType.Required)]
        public string Tcode
        {
            get { return _Tcode; }
            set
            {
                base.SetValue("Tcode", ref _Tcode, value);
            }
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

        public string ShowName
        {
            get
            {
                var r = string.Format("[{0}]{1}", TariffCode, ItemCategoryName);
                if (string.IsNullOrWhiteSpace(Tcode))
                {
                    r = ItemCategoryName;
                }
                return r;
            }
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

        private bool? _IsChecked;
        public bool? IsChecked
        {
            get { return _IsChecked; }
            set { base.SetValue("IsChecked", ref _IsChecked, value); }
        }

    }

    public class TariffPickerVM : ModelBase
    {
        private List<TariffInfoVM> _level1;
        public List<TariffInfoVM> Level1
        {
            get { return _level1; }
            set { SetValue<List<TariffInfoVM>>("Level1", ref _level1, value); }
        }
        private List<TariffInfoVM> _level2;
        public List<TariffInfoVM> Level2
        {
            get { return _level2; }
            set { SetValue<List<TariffInfoVM>>("Level2", ref _level2, value); }
        }
        private List<TariffInfoVM> _level3;
        public List<TariffInfoVM> Level3
        {
            get { return _level3; }
            set { SetValue<List<TariffInfoVM>>("Level3", ref _level3, value); }
        }

        public TariffInfoVM SelectedLevel1
        {
            get { return Level1 == null ? null : (from t in Level1 where t.Tcode == SelectedLevelCode1 select t).FirstOrDefault(); }
        }
        public TariffInfoVM SelectedLevel2
        {
            get { return Level1 == null ? null : (from t in Level2 where t.Tcode == SelectedLevelCode2 select t).FirstOrDefault(); }
        }
        public TariffInfoVM SelectedLevel3
        {
            get { return Level1 == null ? null : (from t in Level3 where t.Tcode == SelectedLevelCode3 select t).FirstOrDefault(); }
        }

        private string _selectedLevelCode1;
        public string SelectedLevelCode1
        {
            get { return _selectedLevelCode1; }
            set { SetValue<string>("SelectedLevelCode1", ref _selectedLevelCode1, value); }
        }
        private string _selectedLevelCode2;
        public string SelectedLevelCode2
        {
            get { return _selectedLevelCode2; }
            set { SetValue<string>("SelectedLevelCode2", ref _selectedLevelCode2, value); }
        }
        private string _selectedLevelCode3;
        public string SelectedLevelCode3
        {
            get { return _selectedLevelCode3; }
            set { SetValue<string>("SelectedLevelCode3", ref _selectedLevelCode3, value); }
        }

    }
}
