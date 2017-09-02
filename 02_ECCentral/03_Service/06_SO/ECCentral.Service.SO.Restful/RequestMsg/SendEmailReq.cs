using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.Restful.RequestMsg
{
    public class SendEmailReq
    {
        public SOInfo soInfo { get; set; }
        public List<string> EmailList { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Language { get; set; }
    }
}
