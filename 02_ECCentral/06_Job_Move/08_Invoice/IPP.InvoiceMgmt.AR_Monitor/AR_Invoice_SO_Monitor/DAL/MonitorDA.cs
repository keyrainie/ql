using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.Model;
using IPPOversea.Invoicemgmt.DAL;
using System.Data;

namespace IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.DAL
{
    class MonitorDA
    {      
        public static List<SOEntity> GetSO(DateTime beginDate,string companyCode)
        {
            SqlCommand cmd = CommandManager.GetCommand("GetSO");
            cmd.CommandTimeout = 30;
            cmd.Parameters.Add(new SqlParameter("@OrderDate", beginDate));
            cmd.Parameters.Add(new SqlParameter{ParameterName="@Companycode",
                Value= companyCode,
                DbType= DbType.AnsiStringFixedLength,
                Size=50         
            });
            return DBHelper.GetData(cmd).ToList<SOEntity>();
        }
        public static decimal GetAR(int SOSysNo)
        {
           SqlCommand cmd = CommandManager.GetCommand("GetAR");
           cmd.Parameters.Add(new SqlParameter("@OrderSysNo", SOSysNo));
           return Convert.ToDecimal(DBHelper.GetObject(cmd));
        }
        public static decimal GetInvoice(int SOSysNo)
        {
            SqlCommand cmd = CommandManager.GetCommand("GetInvoice");
            cmd.Parameters.Add(new SqlParameter("@SONumber", SOSysNo));
            return Convert.ToDecimal(DBHelper.GetObject(cmd));
        }

        public static decimal CreateMonitorLog(int SOSysNo,decimal SO,decimal AR,decimal Invoice,string Status)
        {
            SqlCommand cmd = CommandManager.GetCommand("CreateMonitorLog");
            cmd.Parameters.Add(new SqlParameter("@SOSysNo", SOSysNo));
            cmd.Parameters.Add(new SqlParameter("@SO", SO));
            cmd.Parameters.Add(new SqlParameter("@AR", AR));
            cmd.Parameters.Add(new SqlParameter("@Invoice", Invoice));
            cmd.Parameters.Add(new SqlParameter("@Status", Status));
            return Convert.ToDecimal(DBHelper.GetObject(cmd));
        }

        public static string GetSIMCard()
        {
            SqlCommand cmd = CommandManager.GetCommand("GetSIMCard");
            DataTable dt = DBHelper.GetData(cmd);
            StringBuilder stb = new StringBuilder("-99999");
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    stb.Append("," + dt.Rows[i][0].ToString());
                }
                return stb.ToString();
            }
            else
            {
                return stb.ToString();
            }
        }
        public static decimal GetSIMCardPrice(int SOSysNo, string SIMCard)
        {
            SqlCommand cmd = CommandManager.GetCommand("GetSIMCardPrice");
            cmd.Parameters.Add(new SqlParameter("@SOSysNo", SOSysNo));
            cmd.CommandText = cmd.CommandText.Replace("@ProductSysNo",SIMCard);
            object obj = DBHelper.GetObject(cmd);
            if (obj == null)
            {
                return 0.0m;
            }
            else
            {
                return Convert.ToDecimal(obj);
            }
        }
    }
}
