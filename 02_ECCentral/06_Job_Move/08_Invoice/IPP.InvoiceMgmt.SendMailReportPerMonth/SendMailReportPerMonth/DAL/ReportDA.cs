using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using System.Data;
using IPPOversea.InvoiceMgmt.PerMonthReport.Compoents;

namespace IPPOversea.InvoiceMgmt.PerMonthReport.DAL
{
    public static class ReportDA
    {
        public static DataSet ARAPReport(string dateFrom
            , string dateTo
            , string beginMonth
            , string endLastMonth)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ARAPReport");
            command.CommandTimeout = 120;

            command.SetParameterValue("@DateFrom", dateFrom);
            command.SetParameterValue("@DateTo", dateTo);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            command.SetParameterValue("@BeginMonth", beginMonth);
            command.SetParameterValue("@EndLastMonth", endLastMonth);

            return command.ExecuteDataSet();
        }

        public static DataSet LGPoint(string dateFrom, string dateTo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LGPoint");
            command.CommandTimeout = 120;

            command.SetParameterValue("@DateFrom", dateFrom);
            command.SetParameterValue("@DateTo", dateTo);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            return command.ExecuteDataSet();
        }
    }
}
