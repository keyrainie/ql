using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.Utility
{
    public static class StringUtility
    {
        public static string TrimNull(this object input)
        {
            return (input != null ? input.ToString().Trim() : string.Empty);
        }

        public static string[] BubbleSort(this string[] r)
        {
            for (int i = 0; i < r.Length; i++)
            {
                bool flag = false;
                for (int j = r.Length - 2; j >= i; j--)
                {
                    if (string.CompareOrdinal(r[j + 1], r[j]) < 0)
                    {
                        string str = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = str;
                        flag = true;
                    }
                }
                if (!flag)
                {
                    return r;
                }
            }
            return r;
        }

        public static string[] ToStringArray<T>(this T[] sourceArray)
        {
            string[] destArray = new string[sourceArray.Length];
            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (sourceArray[i] == null)
                {
                    destArray[i] = null;
                }
                else
                {
                    destArray[i] = sourceArray[i].ToString();
                }
            }
            return destArray;
        }

        public static T[] ToArray<T>(this string[] sourceArray, Func<string, T> convertFunc)
        {
            T[] destArray = new T[sourceArray.Length];
            for (int i = 0; i < sourceArray.Length; i++)
            {
                destArray[i] = convertFunc(sourceArray[i]);
            }
            return destArray;
        }

        public static string GetRandomString(int stringLength)
        {
            string strArray = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random((int)DateTime.Now.Ticks);
            char[] array = new char[strArray.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = strArray[random.Next(strArray.Length)];
            }
            return new string(array);
        }

        public static string RemoveHtmlTag(this string str)
        {
            Regex regex = new Regex(@"<\/*[^<>]*>");
            return regex.Replace(str, string.Empty);
        }

        #region 验证字符串格式合法性

        private static bool RegexCheck(string input, string regex, RegexOptions regexOptions = RegexOptions.None)
        {
            if (input == null || regex == null)
            {
                return false;
            }
            return Regex.IsMatch(input, regex, regexOptions);
        }

        /// <summary>
        /// 验证字符串是否表示合法的手机号码
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsMobileNo(this string input)
        {
            return RegexCheck(input, "^(13|15|18)[0-9]{9}$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的座机号码
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsPhoneNo(this string input)
        {
            return RegexCheck(input, @"^[0-9\-]{0,20}$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的日期时间
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsDateTime(this string input)
        {
            DateTime time;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return DateTime.TryParse(input, out time);
        }

        /// <summary>
        /// 验证字符串是否表示合法的浮点数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsFloat(this string input)
        {
            return RegexCheck(input, @"^-?([1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0)$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的负整数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsNegativeInteger(this string input)
        {
            return RegexCheck(input, @"^-[1-9]\d*$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的非负整数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsNonNegativeInteger(this string input)
        {
            return RegexCheck(input, @"^[1-9]\d*|0$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的正整数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsPositiveInteger(this string input)
        {
            return RegexCheck(input, @"^[1-9]\d*$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的非正整数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsNonPositiveInteger(this string input)
        {
            return RegexCheck(input, @"^-[1-9]\d*|0$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的整数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsInteger(this string input)
        {
            return RegexCheck(input, @"^-?[1-9]\d*$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的中国邮政编码
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsZipcode(this string input)
        {
            return RegexCheck(input, @"^[1-9]\d{5}(?!\d)$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的电子邮件地址
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsEmail(this string input)
        {
            return RegexCheck(input, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的IPv4地址
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsIPv4(this string input)
        {
            return RegexCheck(input, @"^((?:2[0-5]{2}|1\d{2}|[1-9]\d|[1-9])\.(?:(?:2[0-5]{2}|1\d{2}|[1-9]\d|\d)\.){2}(?:2[0-5]{2}|1\d{2}|[1-9]\d|\d)):(\d|[1-9]\d|[1-9]\d{2,3}|[1-5]\d{4}|6[0-4]\d{3}|654\d{2}|655[0-2]\d|6553[0-5])$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的IPv6地址
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsIPv6(this string input)
        {
            return RegexCheck(input, "^([0-9A-Fa-f]{1,4}:){7}[0-9A-Fa-f]{1,4}$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的货币金额
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsMoney(this string input)
        {
            return RegexCheck(input, "^([0-9]+|[0-9]{1,3}(,[0-9]{3})*)(.[0-9]{1,2})?$");
        }

        /// <summary>
        /// 验证字符串是否表示合法的Url地址
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsUrl(this string input)
        {
            return RegexCheck(input, @"(mailto\:|(news|(ht|f)tp(s?))\://)(([^[:space:]]+)|([^[:space:]]+)( #([^#]+)#)?) ");
        }

        /// <summary>
        /// 验证字符串是否表示合法的身份证号
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsIDCardNo(this string input)
        {
            DateTime d;
            bool m;
            return IsIDCardNo(input, out d, out m);
        }

        /// <summary>
        /// 验证字符串是否表示合法的身份证号
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <param name="birthday">从合法的身份证号中获得的生日信息</param>
        /// <param name="isMale">从合法的身份证号中获得的性别信息，true为男性，false为女性</param>
        /// <returns></returns>
        public static bool IsIDCardNo(this string input, out DateTime birthday, out bool isMale)
        {
            birthday = default(DateTime);
            isMale = default(bool);
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            input = input.Trim().ToUpper();
            if (input.Length != 15 && input.Length != 18)
            {
                return false;
            }

            string yearStr;
            string monthStr;
            string dayStr;
            string sexStr;
            if (input.Length == 15) // 15位身份证
            {
                if (RegexCheck(input, @"^\d{15}$") == false)
                {
                    return false;
                }
                yearStr = "19" + input.Substring(6, 2);
                monthStr = input.Substring(8, 2);
                dayStr = input.Substring(10, 2);
                sexStr = input.Substring(14, 1);
            }
            else // 18位身份证
            {
                if (RegexCheck(input, @"^\d{17}[0123456789X]{1}$") == false)
                {
                    return false;
                }
                //加权因子常数 
                int[] iW = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
                int total = 0;
                for (int i = 0; i < 17; i++)
                {
                    total += Convert.ToInt32(input[i]) * iW[i];
                }
                int iY = total % 11;
                //校验码常数
                string lastChar = "10X98765432";
                if (lastChar[iY] != input[17])
                {
                    return false;
                }
                yearStr = input.Substring(6, 4);
                monthStr = input.Substring(10, 2);
                dayStr = input.Substring(12, 2);
                sexStr = input.Substring(16, 1);
            }
            int y, m, d;
            if (int.TryParse(yearStr, out y) == false || int.TryParse(monthStr, out m) == false || int.TryParse(dayStr, out d) == false)
            {
                return false;
            }
            if (DateTime.TryParse(yearStr + "-" + monthStr + "-" + dayStr, out birthday) == false)
            {
                return false;
            }
            if (birthday.Year != y || birthday.Month != m || birthday.Day != d)
            {
                return false;
            }
            int s;
            if (int.TryParse(sexStr, out s) == false)
            {
                return false;
            }
            isMale = (s % 2) != 0;
            return true;
        }

        /// <summary>
        /// 验证字符串是否含有Html或Xml标签
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsHtml(this string input)
        {
            Regex regex = new Regex("<([^<>]*?)>");
            MatchCollection mc = regex.Matches(input);
            if (mc == null || mc.Count <= 0)
            {
                return false;
            }
            foreach (Match m in mc)
            {
                if (m == null || m.Success == false || m.Groups.Count < 2 || string.IsNullOrWhiteSpace(m.Groups[1].Value))
                {
                    continue;
                }
                string tag = m.Groups[1].Value.Trim();
                if (RegexCheck(input, @"<\s*\/\s*" + tag + @"\s*>", RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Private Method

        private static readonly string[] ChineseNum = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };

        private static string GetSmallMoney(string moneyValue)
        {
            var intMoney = Convert.ToInt32(moneyValue);
            if (intMoney == 0)
            {
                return "";
            }
            var strMoney = intMoney.ToString();
            int temp;
            var strBuf = new StringBuilder(10);
            if (strMoney.Length == 4)
            {
                temp = Convert.ToInt32(strMoney.Substring(0, 1));
                strMoney = strMoney.Substring(1, strMoney.Length - 1);
                strBuf.Append(ChineseNum[temp]);
                if (temp != 0)
                    strBuf.Append("仟");
            }
            if (strMoney.Length == 3)
            {
                temp = Convert.ToInt32(strMoney.Substring(0, 1));
                strMoney = strMoney.Substring(1, strMoney.Length - 1);
                strBuf.Append(ChineseNum[temp]);
                if (temp != 0)
                    strBuf.Append("佰");
            }
            if (strMoney.Length == 2)
            {
                temp = Convert.ToInt32(strMoney.Substring(0, 1));
                strMoney = strMoney.Substring(1, strMoney.Length - 1);
                strBuf.Append(ChineseNum[temp]);
                if (temp != 0)
                    strBuf.Append("拾");
            }
            if (strMoney.Length == 1)
            {
                temp = Convert.ToInt32(strMoney);
                strBuf.Append(ChineseNum[temp]);
            }
            return strBuf.ToString();
        }

        #endregion

        public static string ToChineseMoney(this decimal moneyValue)
        {
            var result = "";
            if (moneyValue == 0)
                return "零";

            if (moneyValue < 0)
            {
                moneyValue *= -1;
                result = "负";
            }
            var intMoney = Convert.ToInt32(moneyValue * 100);
            var strMoney = intMoney.ToString();
            var moneyLength = strMoney.Length;
            var strBuf = new StringBuilder(100);
            if (moneyLength > 14)
            {
                throw new Exception("Money Value Is Too Large");
            }

            //处理亿部分
            if (moneyLength > 10 && moneyLength <= 14)
            {
                strBuf.Append(GetSmallMoney(strMoney.Substring(0, strMoney.Length - 10)));
                strMoney = strMoney.Substring(strMoney.Length - 10, 10);
                strBuf.Append("亿");
            }

            //处理万部分
            if (moneyLength > 6)
            {
                strBuf.Append(GetSmallMoney(strMoney.Substring(0, strMoney.Length - 6)));
                strMoney = strMoney.Substring(strMoney.Length - 6, 6);
                strBuf.Append("万");
            }

            //处理元部分
            if (moneyLength > 2)
            {
                strBuf.Append(GetSmallMoney(strMoney.Substring(0, strMoney.Length - 2)));
                strMoney = strMoney.Substring(strMoney.Length - 2, 2);
                strBuf.Append("元");
            }

            //处理角、分处理分
            if (Convert.ToInt32(strMoney) == 0)
            {
                strBuf.Append("整");
            }
            else
            {
                if (moneyLength > 1)
                {
                    var intJiao = Convert.ToInt32(strMoney.Substring(0, 1));
                    strBuf.Append(ChineseNum[intJiao]);
                    if (intJiao != 0)
                    {
                        strBuf.Append("角");
                    }
                    strMoney = strMoney.Substring(1, 1);
                }

                var intFen = Convert.ToInt32(strMoney.Substring(0, 1));
                if (intFen != 0)
                {
                    strBuf.Append(ChineseNum[intFen]);
                    strBuf.Append("分");
                }
            }
            var temp = strBuf.ToString();
            while (temp.IndexOf("零零") != -1)
            {
                strBuf.Replace("零零", "零");
                temp = strBuf.ToString();
            }

            strBuf.Replace("零亿", "亿");
            strBuf.Replace("零万", "万");
            strBuf.Replace("亿万", "亿");

            return result + strBuf;
        }

        public static string ToChineseMoney(this decimal? moneyValue)
        {
            if (moneyValue == null)
            {
                return string.Empty;
            }
            return moneyValue.Value.ToChineseMoney();
        }

        #region 格式化为不带人民币符号的货币
        /// <summary>
        /// 格式化为人民币，不带人民币符号，精度为小数点后两位。
        /// </summary>
        /// <param name="price">需要格式化的值</param>
        /// <returns>格式化过的人民币。</returns>
        public static string FormatRMB(this decimal price)
        {
            return FormatRMB(price, 2);
        }

        /// <summary>
        /// 格式化为人民币，不带人民币符号，指定小数点后位数。
        /// </summary>
        /// <param name="price">需要格式化的值</param>
        /// <param name="bitCount">小数点后位数</param>
        /// <returns>格式化过的人民币。</returns>
        public static string FormatRMB(this decimal price, int bitCount)
        {
            string priceFormat = bitCount >= 0 ?
                    string.Format("f{0}", bitCount) : "f0";

            if (price < decimal.Zero)
            {
                return string.Format(@"-{0}", Math.Abs(price).ToString(priceFormat));
            }
            return price.ToString(priceFormat);
        }
        #endregion

        #region 格式化为带人民币符号的货币
        /// <summary>
        /// 格式化为人民币，带人民币符号，精度为小数点后两位。
        /// </summary>
        /// <param name="price">需要格式化的值</param>
        /// <returns>格式化过的人民币。</returns>
        public static string FormatRMBWithSign(this decimal price)
        {
            return FormatRMBWithSign(price, 2);
        }

        /// <summary>
        /// 格式化为人民币，带人民币符号，指定小数点后位数。
        /// </summary>
        /// <param name="price">需要格式化的值</param>
        /// <param name="bitCount">小数点后位数</param>
        /// <returns>格式化过的人民币。</returns>
        public static string FormatRMBWithSign(this decimal price, int bitCount)
        {
            string priceFormat = bitCount >= 0 ?
                    string.Format("f{0}", bitCount) : "f0";

            if (price < decimal.Zero)
            {
                return string.Format(@"&yen;-{0}", Math.Abs(price).ToString(priceFormat));
            }
            return string.Format(@"&yen;{0}", price.ToString(priceFormat));
        }
        #endregion

        #region 格式化为带人民币符号的货币，并且自定义人民币符号和价格的样式
        /// <summary>
        /// 格式化为人民币，自定义人民币符号和价格样式，精度为小数点后两位。
        /// </summary>
        /// <param name="price">需要格式化的值</param>
        /// <param name="html">
        /// 人民币符号和价格样式
        /// 样式格式：<![CDATA[<s class="ico_y">&yen;</s><span class="digi">{0}</span>]]>
        /// 需要将价格处设置为：{0}
        /// </param>
        /// <returns>格式化过的人民币。</returns>
        public static string FormatRMBWithSign(this decimal price, string html)
        {
            return FormatRMBWithSign(price, 2, html);
        }

        /// <summary>
        /// 格式化为人民币，自定义人民币符号和价格样式，指定小数点后位数
        /// </summary>
        /// <param name="price">需要格式化的值</param>
        /// <param name="bitCount">小数点后位数</param>
        /// <param name="html">
        /// 人民币符号和价格样式
        /// 样式格式：<![CDATA[<s class="ico_y">&yen;</s><span class="digi">{0}</span>]]>
        /// 需要将价格处设置为：{0}
        /// </param>
        /// <returns>格式化过的人民币。</returns>
        public static string FormatRMBWithSign(this decimal price, int bitCount, string html)
        {
            string priceFormat = bitCount >= 0 ?
                    string.Format("f{0}", bitCount) : "f0";

            if (price < decimal.Zero)
            {
                string result = string.Format(@"-{0}", Math.Abs(price).ToString(priceFormat));
                return string.Format(html, result);
            }
            return string.Format(html, price.ToString(priceFormat));
        }
        #endregion

        /// <summary>
        /// 截断字符串
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="length">累计长度（中文，英文只占长度的1/2）</param>
        /// <param name="replaceChar">如果过长，替换字符</param>
        /// <returns></returns>
        public static string TruncateString(string input, int length, string replaceChar)
        {
            if (input.Length < length)
            {
                return input;
            }
            length = 2 * length;
            int strlen = System.Text.Encoding.Default.GetByteCount(input);
            int j = 0;//记录遍历的字节数
            int L = 0;//记录每次截取开始，遍历到开始的字节位，才开始记字节数
            int strW = 0;//字符宽度
            bool b = false;//当每次截取时，遍历到开始截取的位置才为true
            string restr = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                char C = input[i];
                if ((int)C >= 0x4E00 && (int)C <= 0x9FA5)
                {
                    strW = 2;
                }
                else
                {
                    strW = 1;
                }
                if ((L == length - 1) && (L + strW > length))
                {
                    b = false;
                    break;
                }
                if (j >= 0)
                {
                    restr += C;
                    b = true;
                }

                j += strW;

                if (b)
                {
                    L += strW;
                    if (((L + 1) > length))
                    {
                        b = false;
                        break;
                    }
                }

            }
            return restr + replaceChar;
        }

    }
}
