using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.Basic.Utilities
{
    public static class RegexHelper
    {
        public const string Phone = @"^[0]?[1][3-8][0-9]{9}$|^(0\d{1,3}[-_]?)?\d{7,8}([-_]?\d{1,7})?$";

        public const string MatchEmail = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

        public const string ZIP = @"^\d{6}$";

        /// <summary>
        /// 提取匹配邮件集合
        /// </summary>
        /// <param name="emailText"></param>
        /// <returns></returns>
        public static List<string> GetRegexEmail(string emailText)
        {
            List<string> result = new List<string>();
            Regex rex = new Regex(MatchEmail, RegexOptions.IgnoreCase);
            var matchEmails = rex.Matches(emailText);

            foreach (Match item in matchEmails)
            {
                result.Add(item.Value);
            }

            return result;
        }

    }
}
