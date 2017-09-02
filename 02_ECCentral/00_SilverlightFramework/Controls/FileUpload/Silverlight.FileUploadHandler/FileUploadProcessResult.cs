using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newegg.Oversea.Silverlight.FileUploadHandler
{
    public class FileUploadProcessResult
    {
        public ProcessStatus Status { get;  set; }
        public string ResponseMsg { get;  set; }

        public FileUploadProcessResult()
        {
        }

        public FileUploadProcessResult(string responseMsg, ProcessStatus processStatus)
            : this()
        {
            this.ResponseMsg = responseMsg;
            this.Status = processStatus;
        }
    }

    public enum ProcessStatus
    {
        Success,
        Failed
    }
}
