using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public enum FindPasswordType
    {
        None = 0,// 0：没有绑定手机或者邮箱，不能使用此功能
        OnlyCellphone = 1,//1：只能通过手机找回
        OnlyEmail = 2,//2：只能通过邮箱找回
        All = 3,//3：:手机和邮箱都能找回
    }

    public class FindPasswordTypeViewModel
    {
        public FindPasswordType Type { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
    }

    /// <summary>
    /// 直接使用短信验证码重置密码使用CustomerID和SMSCode
    /// 通过Token重置密码使用Token
    /// </summary>
    public class FindResetPasswordViewModel : ChangePasswordViewModel
    {
        public string CustomerID { get; set; }
        public string SMSCode { get; set; }

        public string Token { get; set; }
        public int CustomerSysNo { get; set; }
    }
}