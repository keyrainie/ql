using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.AppService
{
    [VersionExport(typeof(SOLogisticsAppService))]
    public class SOLogisticsAppService
    {
        SOLogisticsProcessor processor;

        public SOLogisticsAppService()
        {
            processor = ObjectFactory<SOLogisticsProcessor>.Instance;
 
        }

        /// <summary>
        /// 标记异常单据
        /// </summary>
        /// <param name="info"></param>
        public void MarkDeliveryExp(DeliveryExpMarkEntity info)
        {
            processor.MarkDeliveryExp(info);
        }






    }
}
