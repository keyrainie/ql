using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.FPCheck
{
    public class JobFPCheckReq
    {
        public string Interorder { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 提前获取断货支持仓库
        /// </summary>
        public List<int> OutStockList { get; set; }

        /// <summary>
        /// 忽略验证的对象
        /// </summary>
        public List<string> IgnoreCustomIDList { get; set; }
    }
}
