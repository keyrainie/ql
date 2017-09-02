using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventConsumer
{
    public class SampleMessageConsumer_A : IConsumer<SampleMessage_1>
    {
        public void HandleEvent(SampleMessage_1 eventMessage)
        {
            //TODO:做本Consumer的业务处理

        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

    public class SampleMessageConsumer_B : IConsumer<SampleMessage_1>
    {
        public void HandleEvent(SampleMessage_1 eventMessage)
        {
            //TODO:做本Consumer的业务处理

        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

    public class SampleMessageConsumer_C : IConsumer<SampleMessage_2>
    {
        public void HandleEvent(SampleMessage_2 eventMessage)
        {
            //TODO:做本Consumer的业务处理

        }


        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
