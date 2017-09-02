using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.Restful.RequestMsg
{
    public class SendSMSReq
    {
        public List<string> Numbers { get; set; }
        public string Message { get; set; }
    }
}
