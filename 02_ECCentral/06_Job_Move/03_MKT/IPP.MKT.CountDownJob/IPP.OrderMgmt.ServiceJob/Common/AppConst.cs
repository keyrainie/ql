using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.ServiceJob
{
    public class AppConst
    {
        public const string StringNull = "";
        public const int IntNull = -999999;
        public const decimal DecimalNull = -999999;
        public const float FloatNull = -999999;  //Add By Teracy
        public const float DoubleNull = -999999;  //Add By Teracy 
        public const string DateFormat = "yyyy-MM-dd";
        public static DateTime DateTimeNull = DateTime.Parse("1900-01-01");
        public const string DecimalFormat = "#########0.00";
        public const string DateFormatLong = "yyyy-MM-dd HH:mm:ss";

        public static int SystemUserSysNo
        {
            get
            {
                string dd = System.Configuration.ConfigurationManager.AppSettings["SystemUserSysNo"];
                if (dd == null)
                    return AppConst.IntNull;
                try
                {
                    int intSystemUserSysNo = int.Parse(dd);
                    return intSystemUserSysNo;
                }
                catch
                {
                    return AppConst.IntNull;
                }
            }
        }

        public static string SOAutoAudit_OrderCount
        {
            get
            {
                string dd = System.Configuration.ConfigurationManager.AppSettings["SOAutoAudit_OrderCount"];
                if (dd == null)
                    return "200";
                else
                    return dd;
            }
        }

        public static string NewSOMailTemplet
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["NewOrderMailTemplet"];
            }
        }

    }
}
