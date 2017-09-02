using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace ECCentral.Service.Utility
{
    public class ExcelHelper
    {
        private static readonly string CONNECTION_STRING_2003 = "provider = microsoft.jet.oledb.4.0;data source={0};Extended Properties='Excel 8.0;HDR=No;IMEX=1'";
        private static readonly string CONNECTION_STRING_2007 = "provider = microsoft.ace.oledb.12.0;data source={0};Extended Properties='Excel 12.0;HDR=No;IMEX=1'";
        private static readonly string QUERY_STRING = "select * from [{0}]";
        /// <summary>
        /// 读取Excel文件中的所有数据
        /// </summary>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public static DataSet ReadAll(string excelPath)
        {
            if (string.IsNullOrWhiteSpace(excelPath))
            {
                throw new ArgumentNullException("excelPath");
            }
            DataSet result = new DataSet();
            using (OleDbConnection cn = CreateConnection(excelPath))
            {
                cn.Open();
                string[] sheets = GetAllSheetNames(cn);
                string sheetName = string.Empty;
                for (int i = 0; i < sheets.Length; i++)
                {
                    sheetName = sheets[i];
                    DataTable table = GetTable(cn, sheetName);
                    result.Tables.Add(table);
                }
                cn.Close();
            }
            return result;
        }

        /// <summary>
        /// 读取Excel文件中序号为<paramref name="sheetIndex"/>的Sheet页的数据
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="sheetIndex">取值从0开始</param>
        /// <returns></returns>
        public static DataTable Read(string excelPath, int sheetIndex)
        {
            if (string.IsNullOrWhiteSpace(excelPath))
            {
                throw new ArgumentNullException("excelPath");
            }
            if (sheetIndex < 0)
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }

            DataTable table = null;
            using (OleDbConnection cn = CreateConnection(excelPath))
            {
                cn.Open();
                string[] sheets = GetAllSheetNames(cn);
                string sheetName = string.Empty;
                sheetName = sheets[sheetIndex];
                table = GetTable(cn, sheetName);
                cn.Close();
            }

            return table;
        }

        /// <summary>
        /// 读取Excel文件中名称为<paramref name="sheetName"/>的sheet页的数据
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static DataTable Read(string excelPath, string sheetName)
        {
            if (string.IsNullOrWhiteSpace(excelPath))
            {
                throw new ArgumentNullException("excelPath");
            }
            if (string.IsNullOrWhiteSpace(sheetName))
            {
                throw new ArgumentNullException("sheetName");
            }
            if (!sheetName.EndsWith("$"))
            {
                sheetName += "$";
            }
            DataTable table = null;
            using (OleDbConnection cn = CreateConnection(excelPath))
            {
                cn.Open();
                //string[] sheets = GetAllSheetNames(cn);
                //for (int i = 0; i < sheets.Length; i++)
                //{
                //    if (sheetName.Equals(sheets[i], StringComparison.OrdinalIgnoreCase))
                //    {
                table = GetTable(cn, sheetName);
                //    }
                //}
                cn.Close();
            }

            return table;
        }

        /// <summary>
        /// 按条件查询Excel文件中的数据
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="sql">查询语句</param>
        /// <returns></returns>
        public static DataTable Query(string excelPath, string sql)
        {
            if (string.IsNullOrWhiteSpace(excelPath))
            {
                throw new ArgumentNullException("excelPath");
            }
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException("sql");
            }

            DataTable table = null;
            using (OleDbConnection cn = CreateConnection(excelPath))
            {
                cn.Open();
                table = QueryTable(cn, sql);
                cn.Close();
            }

            return table;
        }

        private static DataTable GetTable(OleDbConnection conn, string sheetName)
        {
            string sql = string.Format(QUERY_STRING, sheetName);
            DataTable table = QueryTable(conn, sql);
            table.TableName = Regex.Replace(sheetName, @"\$$", string.Empty);
            return table;
        }

        private static DataTable QueryTable(OleDbConnection conn, string sql)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        private static OleDbConnection CreateConnection(string excelPath)
        {
            string connString = string.Empty;
            if (excelPath.EndsWith(".xlsx"))
            {
                connString = string.Format(CONNECTION_STRING_2007, excelPath);
            }
            else
            {
                connString = string.Format(CONNECTION_STRING_2003, excelPath);
            }
            return new OleDbConnection(connString);
        }

        private static string[] GetAllSheetNames(OleDbConnection conn)
        {
            DataTable schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            string[] sheetNames = new string[schema.Rows.Count];
            for (int i = 0; i < sheetNames.Length; i++)
            {
                DataRow row = schema.Rows[i];
                sheetNames[i] = Regex.Replace(row["TABLE_NAME"].ToString(), @"^['`]|['`]$", string.Empty);
            }
            return sheetNames;
        }
    }
}
