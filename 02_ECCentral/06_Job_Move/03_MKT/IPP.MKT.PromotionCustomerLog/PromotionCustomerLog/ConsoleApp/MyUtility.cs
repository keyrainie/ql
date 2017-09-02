using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.Oversea.CN.ServiceCommon.Library;
using MktToolMgmt.PromotionCustomerLogApp.Entities;

namespace MktToolMgmt.PromotionCustomerLogApp
{
    public static class MyUtility
    {
        public static void WriteLog<T>(T t, string message)
            where T : DetailBaseEntity
        {
            Console.WriteLine("{0}: {1}"
                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")
                , message);
            LogHelper.WriteOverseaLog<T>(t,
               string.Empty, string.Empty, ConstValues.LogCategory,
               message, t.SysNo.ToString());
        }

        public static string Concate(this IEnumerable<string> list, string spliter)
        {
            if (list == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            foreach (string item in list)
            {
                builder.Append(item);
                builder.Append(spliter);
            }
            if (builder.Length > spliter.Length)
            {
                builder.Remove(builder.Length - spliter.Length, spliter.Length);
            }
            return builder.ToString();
        }

        public static DateTime GetEndDate(this DateTime endDate)
        {
            if (endDate.TimeOfDay.Ticks == 0)
            {
                return endDate.AddDays(1);
            }
            else
            {
                return endDate;
            }
        }

    }
}
