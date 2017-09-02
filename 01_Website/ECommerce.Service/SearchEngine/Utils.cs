using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.SearchEngine
{
    internal static class Utils
    {
        #region 获取' '之后的字符串

        /// <summary>
        /// 获取' '之后的字符串
        /// </summary>
        /// <param name="formatString">需要处理的字符串</param>
        /// <returns>' '之后的字符串</returns>
        public static string GetSplitName(string formatString)
        {
            StringBuilder result = new StringBuilder();

            // EndecaConfig.PropertyNameSplitFlag, '_';
            char splitChar = ' ';

            string[] splitString = formatString.Split(splitChar);
            if (splitString.Length > 1)
            {
                return splitString[splitString.Length - 1];
            }
            else
            {
                return formatString;
            }
        }

        #endregion

        #region 获取'_'之后的字符串

        /// <summary>
        /// 获取'_'之后的字符串
        /// </summary>
        /// <param name="formatString">需要处理的字符串</param>
        /// <returns>'_'之后的字符串</returns>
        public static string GetEndSplitName(string formatString)
        {
            StringBuilder result = new StringBuilder();
            char splitChar = '_';

            string[] splitString = formatString.Split(splitChar);
            if (splitString.Length > 1)
            {
                return splitString[splitString.Length - 1];
            }
            else
            {
                return formatString;
            }
        }

        #endregion

        /// <summary>
        /// 判断str中是否包含targetStr
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="targetStr"></param>
        /// <returns></returns>
        public static bool isStrExist(string[] strs, string targetStr)
        {
            bool ret = false;
            foreach (string str in strs)
            {
                if (!ret)
                    ret = (str == targetStr);
                else
                    return ret;
            }
            return ret;
        }

        /// <summary>
        /// 判断str中是否包含targetStr
        /// </summary>
        /// <param name="str">以空格分割的字符串</param>
        /// <param name="targetStr"></param>
        /// <returns></returns>
        public static bool isStrExist(string str, string targetStr)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            str = string.Format(" {0} ", str);
            targetStr = string.Format(" {0} ", targetStr.Trim());

            return str.Contains(targetStr);
        }

        /// <summary>
        /// 移除str中的targetStr
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="targetStr"></param>
        /// <returns></returns>
        public static string removeStr(string[] strs, string targetStr)
        {
            string ret = String.Empty;
            foreach (string str in strs)
            {
                if (str != targetStr)
                {
                    ret = ret + str + " ";
                }
            }
            return ret.TrimEnd(' ');

        }

        /// <summary>
        /// 移除str中的targetStr
        /// </summary>
        /// <param name="str">以空格分割的字符串</param>
        /// <param name="targetStr"></param>
        /// <returns></returns>
        public static string removeStr(string str, string targetStr)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            str = string.Format(" {0} ", str);
            targetStr = string.Format(" {0} ", targetStr.Trim());

            return str.Replace(targetStr, " ").Trim();
        }

        /// <summary>
        /// 移除属性名中存在的前缀
        /// DV_存储卡
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string removePrefix(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                int index = str.IndexOf('_');
                if (index > 0)
                {
                    str = str.Remove(0, index + 1);
                }
            }
            return str;
        }

        /// <summary>
        /// 转换成枚举值
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TEnum GetEnumByValue<TEnum>(string value, TEnum defaultValue)
            where TEnum : struct
        {
            TEnum result;
            if (!Enum.TryParse<TEnum>(value, out result))
            {
                return defaultValue;
            }
            return result;
        }
    }
}
