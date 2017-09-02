using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility
{
    internal class ESBConsumer<T> : IConsumer<T> where T : IEventMessage
    {
        public void HandleEvent(T eventMessage)
        {
            Nesoft.ServiceBus.Consumer.EventPublisher.Default.Publish<T>(eventMessage);
        }

        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }

    internal class DummyConsumer<T> : IConsumer<T> where T : IEventMessage
    {
        public void HandleEvent(T eventMessage)
        {
            // do nothing...
        }

        public ExecuteMode ExecuteMode
        {
            get { return ExecuteMode.Sync; }
        }
    }
}
