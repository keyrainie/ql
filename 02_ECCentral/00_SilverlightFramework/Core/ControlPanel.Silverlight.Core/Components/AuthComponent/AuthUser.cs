using System;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public class AuthUser
    {
        public string ID { get; set; }
        public string LoginName { get; set; }
        public string Domain { get; set; }
        public string DisplayName { get; set; }
        public string UserEmailAddress { get; set; }
        public string BusinessLanguageCode
        {
            get
            {
                return "zh-CH";
            }
        }

        public string DepartmentNumber { get; set; }
        public string DepartmentName { get; set; }
        public int? userSysNo = -1;
        public int? UserSysNo
        {
            get
            {
                return userSysNo;
            }
            set
            {
                userSysNo = value;
            }
        }
    }
}

