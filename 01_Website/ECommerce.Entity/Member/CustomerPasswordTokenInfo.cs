using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    /// <summary>
    /// 【OverseaCustomerManagement.[dbo].Customer_PasswordToken】
    /// </summary>
    public class CustomerPasswordTokenInfo:EntityBase
    {
        public int SysNo { get; set; }

        public int CustomerSysNo { get; set; }

		public string Token{get;set;}

		public DateTime CreateTime{get;set;}

        /// <summary>
        /// A-D
        /// </summary>
        public ADStatus Status { get; set; }

        /// <summary>
        /// E-P
        /// </summary>
        public TokenType TokenType { get; set; }
    }
}
