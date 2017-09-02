using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints.Utilities
{
    public static class ConvertHelper
    {
        public static int ToInt(string s)
        {
            return ToInt(s, 0);
        }

        public static int ToInt(string s, int defaultValue)
        {
            int result = defaultValue;
            int.TryParse(s, out result);
            return result;
        }
    }
}
