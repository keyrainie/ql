using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.AsyncCallAPIService.Core
{
    public delegate void CallServiceCompletedEventHandler(object sender, CallServiceCompletedEventArgs e);

    public class CallServiceCompletedEventArgs : EventArgs
    {
        public CallLogInfo LogInfo { get; set; }
        public bool IsSuccess { get; set; }
        public string ResponseMessage { get; set; }
        public long CostMillionSeconds { get; set; }
        public DateTime CallTime { get; set; }

        public CallServiceCompletedEventArgs(CallLogInfo logInfo, bool isSuccess, string responseMessage, long costMillionSeconds, DateTime callTime)
        {
            this.LogInfo = logInfo;
            this.IsSuccess = isSuccess;
            this.ResponseMessage = responseMessage;
            this.CostMillionSeconds = costMillionSeconds;
            this.CallTime = callTime;
        }
    }
}
