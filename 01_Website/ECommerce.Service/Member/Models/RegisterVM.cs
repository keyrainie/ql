using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Member.Models
{
    public class RegisterVM
    {
        public string CustomerID { get; set; }

        public string Password { get; set; }

        public string RePassword { get; set; }

        public string ValidatedCode { get; set; }

        public string FromLinkSource { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
        /// <summary>
        /// 注册手机信息ID
        /// </summary>
        public string  CellPhoneCode { get; set; }
        /// <summary>
        /// 注册手机号码
        /// </summary>
        public string CellPhone { get; set; }

    }
}
