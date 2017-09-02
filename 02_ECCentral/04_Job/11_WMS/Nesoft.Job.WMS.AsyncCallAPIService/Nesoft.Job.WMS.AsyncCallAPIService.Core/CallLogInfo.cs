using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.AsyncCallAPIService.Core
{
    public class CallLogInfo
    {
        public int SysNo { get; set; }
        public string Sign { get; set; }
        public string MethodName { get; set; }
        public string PostData { get; set; }
        public CallLogStatus Status { get; set; }
        public int? RetryCount { get; set; }
        public bool? LastCallIsSuccess { get; set; }
        public string LastCallResponseData { get; set; }
        public DateTime? LastCallTime { get; set; }
        public int? LastCallCostMillionSeconds { get; set; }
        public DateTime InDate { get; set; }
    }
}
