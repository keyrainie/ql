using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Nesoft.ECWeb.M
{
    public static class StringExtensions
    {

        /// <summary>
        /// 从inputString中抽取品牌系统编号
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static int ExtractBrandSysNo(this string inputString)
        {
            inputString = (inputString ?? "").Trim();
            var m = Regex.Match(inputString, @"http:\/\/www\.dchnu\.com\/BrandZone\/(\d+).*", RegexOptions.IgnoreCase);
            int groupBuySysNo;
            if (m.Success && int.TryParse(m.Groups[1].Value, out groupBuySysNo))
            {
                return groupBuySysNo;
            }

            return 0;
        }
    }
}