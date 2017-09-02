using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.WebFramework
{
    public class HtmlUtility
    {
        /// <summary>
        /// 移除所有Html标签
        /// </summary>
        /// <param name="source">内容</param>
        /// <returns></returns>
        public static string RemoveHtml(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}
