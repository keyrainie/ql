using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.Contract;
using System.Configuration;

namespace IPP.OrderMgmt.CsReportsJob.Utilities
{
    public class Util
    {
        public static string TrimNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToString().Trim();
            }
        }


        public static string ToSqlString(string paramStr)
        {
            return "'" + SafeFormat(paramStr) + "'";
        }

        private static string SafeFormat(string strInput)
        {
            if (string.IsNullOrEmpty(strInput) == true)
                return string.Empty;
            return strInput.Trim().Replace("'", "''");
        }

        public static decimal TrimDecimalNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return AppConst.DecimalNull;
            }
            else
            {
                return decimal.Parse(obj.ToString());
            }
        }

        public static DateTime TrimDateNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return AppConst.DateTimeNull;
            }
            else
            {
                return DateTime.Parse(obj.ToString());
            }
        }

        /// <summary>
        /// 判断是否手机号码
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static bool IsCellNumber(string cell)
        {
            if (string.IsNullOrEmpty(cell) == true)
                return false;

            try
            {
                // 验证为数字，防止全角字符
                Convert.ToInt64(cell);

                return Regex.IsMatch(cell, @"^1\d{10}$");
            }
            catch
            {
                return false;
            }
        }



    }
}
