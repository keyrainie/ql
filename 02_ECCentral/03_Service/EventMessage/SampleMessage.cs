using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage
{
    public class SampleMessage_1 : IEventMessage
    {
        public string Subject
        {
            get { return "SampleMessage_1"; }
        }
    }

    public class SampleMessage_2 : IEventMessage
    {
        public string Subject
        {
            get { return "SampleMessage_2"; }
        }
    }
}
