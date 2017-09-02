using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.Restful.ResponseMsg
{
    /// <summary>
    /// 批量处理结果
    /// </summary>
    public class BatchResultRsp
    { 
        public List<string> SuccessRecords { get; set; }
        public List<string> FailureRecords { get; set; }
    }
}
