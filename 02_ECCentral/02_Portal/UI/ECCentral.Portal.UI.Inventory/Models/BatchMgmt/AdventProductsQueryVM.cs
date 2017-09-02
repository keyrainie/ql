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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class AdventProductsInfoVM : ModelBase
    {
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }

        private string brandSysNo;
        public string BrandSysNo
        {
            get { return brandSysNo; }
            set { SetValue("BrandSysNo", ref brandSysNo, value); }
        }

        private string brandName;
        public string BrandName
        {
            get { return brandName; }
            set { SetValue("BrandName", ref brandName, value); }
        }

        private string c3SysNo;
        public string C3SysNo
        {
            get { return c3SysNo; }
            set { SetValue("BrandSysNo", ref c3SysNo, value); }
        }

        private string ringDay;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[1-9]\d{0,4}$", ErrorMessage = @"必须为小于5位的正整数")]
        public string RingDay
        {
            get { return ringDay; }
            set { SetValue("RingDay", ref ringDay, value); }
        }

        private string email;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Email)]
        public string Email
        {
            get { return email; }
            set { SetValue("Email", ref email, value); }
        }


        private int? inUser;

        public int? InUser
        {
            get { return inUser; }
            set { SetValue("InUser", ref inUser, value); }
        }
        private int? editUser;

        public int? EditUser
        {
            get { return editUser; }
            set { SetValue("EditUser", ref editUser, value); }
        }

        private DateTime? inDate;

        public DateTime? InDate
        {
            get { return inDate; }
            set { SetValue("InDate", ref inDate, value); }
        }
        private DateTime? editDate;

        public DateTime? EditDate
        {
            get { return editDate; }
            set { SetValue("EditDate", ref editDate, value); }
        }
    }
}
