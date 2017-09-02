using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.Service
{
    public class UnifiedHelper
    {
        /// <summary>
        /// 舍去金额的分,直接舍去,非四舍五入
        /// </summary>
        internal static decimal TruncMoney(decimal amount)
        {
            int tempAmt = (int)(amount * 10);
            return tempAmt / 10M;
        }
        /// <summary>
        /// 保留2位小数
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static decimal ToMoney(decimal amount)
        {
            return decimal.Round(amount, 2);
        }

        public static bool IsEmailAddress(string strEmailAddress)
        {
            Regex objNotEmailAddress = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return objNotEmailAddress.IsMatch(strEmailAddress);
        }
    }
}
