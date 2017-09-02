using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.RMA.Restful.ResponseMsg
{
    public class RequestDetailInfoRsp
    {
        public RMARequestInfo RequestInfo { get; set; }

        public string CustomerID { get; set; }

        public string CustomerName { get; set; }

        public string SOID { get; set; }

        public DeliveryStatus? DeliveryStatus { get; set; }

        public string DeliveryUserName { get; set; }

        /// <summary>
        /// 商家信息
        /// </summary>
        public string BusinessModel { get; set; }
    }
}
