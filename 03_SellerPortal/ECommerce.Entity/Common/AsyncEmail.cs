using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class AsyncEmail
    {
        public int? SysNo { get; set; }

        public string MailAddress { get; set; }

        public string MailSubject { get; set; }

        public string MailBody { get; set; }

        public int? Status { get; set; }

        public string CCMailAddress { get; set; }

        public string BCMailAddress { get; set; }

        public string MailFrom { get; set; }

        public string MailSenderName { get; set; }

        public int? Priority { get; set; }
    }
}
