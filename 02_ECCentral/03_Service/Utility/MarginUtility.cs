using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Utility
{
    public static class MarginUtility
    {
        public static decimal PointDivisor = 10m;
        /// <summary>
        /// 获取当前毛利率
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="point"></param>
        /// <param name="unitCost"></param>
        /// <returns></returns>
        public static decimal GetOldMargin(decimal currentPrice, int point, decimal unitCost)
        {
            if ((currentPrice - point / PointDivisor) <= 0)
            {
                return 0;
            }
            return (currentPrice - (decimal)point / PointDivisor - unitCost) / (currentPrice - point / PointDivisor);
        }

        /// <summary>
        /// 获取当前毛利率百分数描述
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="point"></param>
        /// <param name="unitCost"></param>
        /// <returns></returns>
        public static string GetOldMarginDesc(decimal currentPrice, int point, decimal unitCost)
        {
            if ((currentPrice - point / PointDivisor) <= 0)
            {
                return 0.ToString("P");
            }
            var currentMargin = (currentPrice - (decimal)point / PointDivisor - unitCost) / (currentPrice - point / PointDivisor);
            return currentMargin.ToString("P");
        }
    }
}
