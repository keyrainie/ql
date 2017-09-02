using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Cart
{
    public class SltOrderGiftReqModel
    {
        /// <summary>
        /// 活动编号
        /// </summary>
        public int ActivityNo { get; set; }

        /// <summary>
        /// 赠品编号
        /// </summary>
        public List<int> GiftSysNos { get; set; }
    }
}