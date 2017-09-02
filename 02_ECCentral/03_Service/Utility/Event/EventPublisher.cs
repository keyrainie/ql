using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Nesoft.ServiceBus.Consumer;

namespace ECCentral.Service.Utility
{
    public static class EventPublisher
    {
        public static void Publish<T>(T eventMessage) where T : IEventMessage
        {
            var subscriptions = Subscription<T>.GetSubscribers();
            //if (subscriptions.Count <= 0)
            //{
            //    subscriptions.Add(new ESBConsumer<T>());
            //}
            subscriptions.ForEach(x => PublishToConsumer(x, eventMessage));
        }

        private static void PublishToConsumer<T>(IConsumer<T> x, T eventMessage) where T : IEventMessage
        {
            if (x is DummyConsumer<T>)
            {
                return;
            }
            if (x is ESBConsumer<T>)
            {
                EventMapping.AddEvent<T>(eventMessage.Subject);
            }
            if (x.ExecuteMode == ExecuteMode.Sync ||
                (x.ExecuteMode == ExecuteMode.AccordingToTransaction && Transaction.Current != null))
                // 同步执行
            {
                x.HandleEvent(eventMessage);
            }
            else // 异步执行
            {
                Action<T> act = new Action<T>(x.HandleEvent);
                act.BeginInvoke(eventMessage, ar =>
                {
                    Action<T> tmp = ar.AsyncState as Action<T>;
                    if (tmp != null)
                    {
                        tmp.EndInvoke(ar);
                    }
                }, act);
            }
        }
    }
}
