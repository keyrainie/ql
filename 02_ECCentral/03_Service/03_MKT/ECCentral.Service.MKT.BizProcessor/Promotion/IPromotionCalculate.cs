using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.BizProcessor
{
    public interface IPromotionCalculate
    {
        /// <summary>
        /// 计算订单的促销折扣结果
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <param name="alreadyApplyPromoList">已应用促销信息</param>
        /// <returns></returns>
        List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, List<SOPromotionInfo> alreadyApplyPromoList);

        ///// <summary>
        ///// 暂时用RMARegisterInfo做输入，待确认
        ///// </summary>
        ///// <param name="rmaInfo"></param>
        ///// <returns></returns>
        //RMAPromotionResult CalculateRMAPromotion(int originSOSysNo, RMARegisterInfo rmaInfo);
    }
}
