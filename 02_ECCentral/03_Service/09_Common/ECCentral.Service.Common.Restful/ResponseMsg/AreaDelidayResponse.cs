using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.Restful.ResponseMsg
{
    public class AreaDelidayResponse
    {
        /// <summary>
        /// 仓库所在城市
        /// </summary>
        public int? WHArea { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
    }
}
