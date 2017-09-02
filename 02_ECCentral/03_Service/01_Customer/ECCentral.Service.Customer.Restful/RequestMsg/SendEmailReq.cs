using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.Restful.RequestMsg
{
    public class SendEmailReq
    {
        public List<string> EmailList { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CompanyCode { get; set; }
    }
}
