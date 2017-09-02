using ECommerce.DataAccess.Invoice;
using ECommerce.Entity.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Service.Invoice
{
    public class FinancialReportService
    {
        public static SalesStatisticsReport SalesStatisticsReportQuery(SalesStatisticsReportQueryFilter filter)
        {
            
            return FinancialReportDA.SalesStatisticsReportQuery(filter);
        }
    }
}
