using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Common
{
    public class LoginCountRequest
    {
        public int Action { set; get; } // 0,insert; 1 read
        public string SystemNo { set; get; }
        public string InUser { set; get; }
    }
}
