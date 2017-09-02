using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.Restful.RequestMsg
{
    /// <summary>
    /// 串货订单
    /// </summary>
    public class CHSetReq
    {
        public string ChannelID { get; set; }
        public FPCheckItemStatus? Status { get; set; }
        public int? CategorySysNo { get; set; }
        public string ProductID { get; set; }
    }
    /// <summary>
    /// 炒货订单
    /// </summary>
    public class CCSetReq
    {
        public int SysNo { get; set; }
        public string Params { get; set; }
        public bool? Status { get; set; }
    }
}
