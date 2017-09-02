using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Member.Models
{
    [Serializable]
    public class TLYHUserInfo
    {
        public string CustomerAlias { get; set; }
        public string CustomerId { get; set; }
        public string CustomerNameCN { get; set; }
        public string CustomerSex { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Birthday { get; set; }
        public string CertType { get; set; }
        public string CertNo { get; set; }
        public string CustomerMobile { get; set; }
    }
}
