using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService.Promotion
{
    [VersionExport(typeof(PromotionCalculateAppService))]
    public  class PromotionCalculateAppService
    {
        /// <summary>
        /// 为SO提供计算当前订单能能享受的所有促销活动结果
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns></returns>
        public List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo)
        {
            return ObjectFactory<PromotionCalculateProcessor>.Instance.CalculateSOPromotion(soInfo);
        }

        /// <summary>
        /// 为RMA退货时，需要退款的促销引擎计算服务
        /// </summary>
        /// <param name="originSOSysNo"></param>
        /// <param name="rmaInfo"></param>
        /// <returns></returns>
        public RMAPromotionResult CalculateRMAPromotion(int originSOSysNo, RMARegisterInfo rmaInfo)
        {
            return ObjectFactory<PromotionCalculateProcessor>.Instance.CalculateRMAPromotion(originSOSysNo, rmaInfo);
        }
    }
}
