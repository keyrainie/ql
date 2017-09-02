using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newegg.Oversea.Framework.Contract;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities
{
    public class UserInfo
    {
        public string UserID { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string Department { get; set; }
    }
}
