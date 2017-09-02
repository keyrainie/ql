using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class VendorPortalLog 
    {
        public string ReferenceKey { get; set; }

        public string Content { get; set; }

        public string RegionName { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public string ServerIP { get; set; }

        public string ServerName { get; set; }

        public string LogUserName { get; set; }

        public string ExtendedProperties { get; set; }

        public string LogType { get; set; }
    }
}
