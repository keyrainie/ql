using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Enum;

namespace ECCentral.BizEntity.MKT
{
    public class AmbassadorNewsBatchInfo
    {

        /// <summary>
        /// 需要更新状态的泰隆优选大使系统编号。
        /// </summary>
        public List<int> AmbassadorNewsSysNos
        {
            get;
            set;
        }

        public AmbassadorNewsStatus Status { get; set; }

        public string CompanyCode { get; set; }
    }
}
