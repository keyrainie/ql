using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace ECCentral.Service.MKT.Restful.Job
{
    public partial class MKTService
    {
        /// <summary>
        /// 查询待审核的秒杀订单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/MktJob/GetSOStatus", Method = "POST")]
        public List<ECCentral.BizEntity.SO.SOInfo> GetSOStatus()
        {
            return ObjectFactory<MKTJOBAppService>.Instance.GetSOStatus();
        }

        /// <summary>
        /// 更新订单支付状态
        /// </summary>
        /// <param name="soSysNo"></param>
        public void MakeOpered(int soSysNo)
        {
            ObjectFactory<MKTJOBAppService>.Instance.MakeOpered(soSysNo);
        }
    }
}
