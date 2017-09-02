using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.ETPCalculator
{
    public interface IETPCalculator
    {
        /// <summary>
        /// 获取ETP时间
        /// </summary>
        /// <returns>ETP计算结果</returns>
        DateTime? Calculate();
    }
}