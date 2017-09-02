using System;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ICalcGrossMarginDA))]
    public class CalcGrossMarginDA : ICalcGrossMarginDA
    {
        /// <summary>
        /// 获取毛利率
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="unitcost"></param>
        /// <param name="point"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public decimal CalcGrossMarginRate(decimal currentPrice, decimal unitcost, decimal point, int productSysNo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取毛利率
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public decimal CalcGrossMarginRate(int productSysNo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取最低毛利率
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public decimal GetMinMarginByItemSysNo(int productSysNo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取最高毛利率以及最低毛利率
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="minMarin"></param>
        /// <param name="minMarinH"></param>
        public void GetMinMarginByProductSysNo(int productSysNo, ref decimal minMarin, ref decimal minMarinH)
        {
            throw new NotImplementedException();
        }
    }
}
