using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using ReportInterface;

namespace IPPOversea.Invoicemgmt.AutoSAP
{
    class CustomeReportFactory
    {
        public static List<ICustomeReport> GetHandlers()
        {
            List<ICustomeReport> handlers = new List<ICustomeReport>();
            Assembly ab = Assembly.LoadFrom(Path.Combine( AppDomain.CurrentDomain.BaseDirectory,"CustomReport.dll"));
            object obj = ab.CreateInstance("CustomReport.Biz.CustomReportBiz");
            ICustomeReport handler = obj as ICustomeReport;
            handlers.Add(handler);
            return handlers;
        }
    }
}
