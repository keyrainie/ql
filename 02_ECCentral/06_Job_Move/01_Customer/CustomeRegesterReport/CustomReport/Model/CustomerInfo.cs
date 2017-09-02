using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace CustomReport.Model
{
    public class CustomerInfo
    {
        [DataMapping("CustomerID", DbType.String)]
        public string CustomerID{get;set;}
        [DataMapping("Status", DbType.Int32)]
        public int? Status{get;set;}
        [DataMapping("RegisterTime", DbType.DateTime)]
        public DateTime? RegisterTime{get;set;}
        [DataMapping("Email", DbType.String)]
        public string Email{get;set;}

        public string StausString
        {
            get
            {
                if (Status.HasValue && Status.Value == 0)
                {
                    return "有效";
                }
                else
                {
                    return "无效";
                }
            }
        }
    }
}
