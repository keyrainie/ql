using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购蓝批量创建采购单 返回对象 
    /// </summary>
    public class BatchCreateBasketResultInfo
    {
        public List<int> SucessPOSysNos { get; set; }
        public string ErrorMsg { get; set; }
    }
}
