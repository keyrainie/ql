using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.AuthCenter
{
    public class KeystoneUserQueryMessage:IEventMessage
    {
        public string LoginName { get; set; }
        public dynamic Result { get; set; }

        public string Subject
        {
            get { return "KeystoneUserQueryMessage"; }
        }
    }
}
