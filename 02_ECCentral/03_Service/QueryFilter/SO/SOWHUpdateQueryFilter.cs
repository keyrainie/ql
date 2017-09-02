using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.SO
{
    public class SOWHUpdateQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        //订单编号
        public string SOSysNo { get; set; }
        //商品ID
        public string ProductSysNo { get; set; }
        //公司代码
        public string CompanyCode { get; set; }
    }
}
