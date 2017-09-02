using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.RMA
{
    public class RMARegisterDetailModel
    {
        /// <summary>
        /// 售后基本信息
        /// </summary>
        public RMARequestInfoModel RMARequestInfo { get; set; }

        /// <summary>
        /// 售后明细
        /// </summary>
        public RMARegisterInfoModel RMARegisterInfo { get; set; }
    }
}