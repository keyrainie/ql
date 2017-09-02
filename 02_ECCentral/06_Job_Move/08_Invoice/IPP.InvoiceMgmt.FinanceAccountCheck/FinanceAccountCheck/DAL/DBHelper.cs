using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
namespace IPPOversea.Invoicemgmt.DAL
{
    class DBHelper
    {       
        public static bool ExecuteSql(SqlCommand cmd)
        {
            try
            {
                cmd.Connection.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public static Int32 ExecuteNoQuery(SqlCommand cmd)
        {
            try
            {
                cmd.Connection.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public static DataTable GetData(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter dap = new SqlDataAdapter();
            dap.SelectCommand = cmd;
            dap.Fill(dt);
            return dt;
        }

        public static object GetObject(SqlCommand cmd)
        {
            try
            {
                cmd.Connection.Open();
                return cmd.ExecuteScalar();
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
    }
}
