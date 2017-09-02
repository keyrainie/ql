using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using POASNMgmt.AutoCreateVendorSettle.Compoents;
using Newegg.Oversea.Framework.DataConfiguration;
using ECCentral.BizEntity.PO;

namespace POASNMgmt.AutoCreateGatherSettle.DataAccess
{
    public class SqlBulkCopyHelp
    {

        private static long SqlBulkCopyInsert(List<GatherSettlementItemInfo> list)
        {
            CommonDataInstance command = new CommonDataInstance();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            DataTable dataTable = GetTableSchema();
            for (int i = 0; i < list.Count; i++)
            {
                DataRow dataRow = GetDataRow(list[i], dataTable);
                dataTable.Rows.Add(dataRow);
            }
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy("connectionString");
            sqlBulkCopy.DestinationTableName = "Passport";
            sqlBulkCopy.BatchSize = dataTable.Rows.Count;
            SqlConnection sqlConnection = new SqlConnection("connectionString");
            sqlConnection.Open();
            if (dataTable != null && dataTable.Rows.Count != 0)
            {
                sqlBulkCopy.WriteToServer(dataTable);
            }
            sqlBulkCopy.Close();
            sqlConnection.Close();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private static DataRow GetDataRow(GatherSettlementItemInfo msg, DataTable dataTable)
        {
            DataRow dr = dataTable.NewRow();
            dr["ReferenceSysNo"] = msg.SONumber;
            dr["ReferenceType"] = msg.SettleType;
            dr["ProductSysNo"] = msg.ProductSysNo;
            dr["Qty"] = msg.Quantity;
            dr["SalesAmt"] = msg.OriginalPrice;
            dr["InUser"] = GlobalSettings.UserName;
            dr["InDate"] = DateTime.Now;
            dr["EditUser"] = GlobalSettings.UserName;
            dr["EditDate"] = DateTime.Now;
            dr["CurrencyCode"] = "CNY";
            dr["CompanyCode"] = GlobalSettings.CompanyCode;
            //dr["LanguageCode"] = 
            dr["StoreCompanyCode"] = GlobalSettings.StoreCompanyCode;
            dr["ItemType"] = msg.ProductSysNo.Value <= 0 ? "SHP" : "PRD";
            dr["SettlementSysNo"] = msg.SettleSysNo;
            return dr;
        }

        private static DataTable GetTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("SysNo"));
            //SysNo
            dt.Columns.Add(new DataColumn("ReferenceSysNo"));
            //ReferenceSysNo
            dt.Columns.Add(new DataColumn("ReferenceType"));
            //ReferenceType
            dt.Columns.Add(new DataColumn("ProductSysNo"));
            //ProductSysNo
            dt.Columns.Add(new DataColumn("Qty"));
            //Qty
            dt.Columns.Add(new DataColumn("SalesAmt"));
            //SalesAmt
            dt.Columns.Add(new DataColumn("InUser"));
            //InUser
            dt.Columns.Add(new DataColumn("InDate"));
            //InDate
            dt.Columns.Add(new DataColumn("EditUser"));
            //EditUser
            dt.Columns.Add(new DataColumn("EditDate"));
            //EditDate
            dt.Columns.Add(new DataColumn("CurrencyCode"));
            //CurrencyCode
            dt.Columns.Add(new DataColumn("CompanyCode"));
            //CompanyCode
            dt.Columns.Add(new DataColumn("LanguageCode"));
            //LanguageCode
            dt.Columns.Add(new DataColumn("StoreCompanyCode"));
            //StoreCompanyCode
            dt.Columns.Add(new DataColumn("ItemType"));
            //ItemType
            dt.Columns.Add(new DataColumn("SettlementSysNo"));
            //SettlementSysNo
            return dt;
        }


    }
}
