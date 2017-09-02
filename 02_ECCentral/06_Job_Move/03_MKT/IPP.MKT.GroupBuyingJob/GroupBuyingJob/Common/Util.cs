using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace IPP.MktToolMgmt.GroupBuyingJob
{
    public class Util
    {
        private Util()
        {
        }
        public static string TrimNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return AppConst.StringNull;
            }
            else
            {
                return obj.ToString().Trim();
            }
        }

        public static int TrimIntNull(Object obj)
        {
            if (obj is System.DBNull)
            {
                return AppConst.IntNull;
            }
            else
            {
                return Int32.Parse(obj.ToString());
            }
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

        public static string SafeFormat(string strInput)
        {
            if (string.IsNullOrEmpty(strInput) == true)
                return string.Empty;

            return strInput.Trim().Replace("'", "''");
        }

        public static string ToSqlString(string paramStr)
        {
            return "'" + SafeFormat(paramStr) + "'";
        }

        public static bool HasMoreRow(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool HasMoreRow(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return false;
            else
                return true;
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
