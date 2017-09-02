using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Nesoft.Utility;

namespace Nesoft.Job.WMS.AsyncCallAPIService.Core
{
    public class CallServiceManager
    {
        public event CallServiceCompletedEventHandler CallServiceFinished;
        public ServicesConfigInfo ConfigInfo { get; set; }
        public CallLogInfo CallLog { get; set; }
        public CallServiceManager(CallLogInfo callLog, ServicesConfigInfo configInfo)
        {
            this.ConfigInfo = configInfo;
            this.CallLog = callLog;
        }

        public void Do()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            DateTime callTime = DateTime.Now;
            if (null != ConfigInfo && ConfigInfo.Processors.Count > 0)
            {
                try
                {
                    foreach (var processorConfig in ConfigInfo.Processors)
                    {
                        IAPIServiceProcessor processor = Invoker.CreateInstance(Type.GetType(processorConfig.ProcessorImplementType)) as IAPIServiceProcessor;
                        if (null != processor)
                        {
                            var result = processor.Process(CallLog.PostData);
                            CallServiceFinished(this, new CallServiceCompletedEventArgs(this.CallLog, true, result, watch.ElapsedMilliseconds, callTime));

                        }
                    }
                    watch.Stop();
                }
                catch (BusinessException bizEx)
                {
                    watch.Stop();
                    CallServiceFinished(this, new CallServiceCompletedEventArgs(this.CallLog, false, bizEx.Message, watch.ElapsedMilliseconds, callTime));
                }
                catch (Exception ex)
                {
                    watch.Stop();
                    CallServiceFinished(this, new CallServiceCompletedEventArgs(this.CallLog, false, ex.Message, watch.ElapsedMilliseconds, callTime));
                }
            }
        }
    }
}
