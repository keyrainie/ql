using System;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ECCentral.Service.SO.BizProcessor
{
    public static class UtilityHelper
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