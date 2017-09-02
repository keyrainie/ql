using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.BizProcessor
{
    public static class UtilityHelper
    {
        internal static string EmptyIfNull(this System.String str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str;
        }
    }
}