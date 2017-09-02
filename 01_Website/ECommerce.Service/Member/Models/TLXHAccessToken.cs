using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Member.Models
{
    [Serializable]
    public class TLYHAccessToken
    {
        //public string access_token { get; set; }
        //public string expires_in { get; set; }
        //public string remind_in { get; set; }
        //public string uid { get; set; }
        public string uniteLoginCode { get; set; }
        public string authorizedSerial { get; set; }
    }
}
