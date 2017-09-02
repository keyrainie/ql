using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
{
    public class MailEntity
    {
        public string BCC { get; set; }

        public string Body { get; set; }

        public string CC { get; set; }

        public string From { get; set; }

        public bool IsBodyHtml { get; set; }

        public string Subject { get; set; }

        public string To { get; set; }
    }
}
