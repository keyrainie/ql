using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface ICustomerVisitDA
    {
        VisitLog InsertCustomerVisitLog(VisitLog log);
        VisitCustomer GetVisitCustomerByCustomerSysNo(int customerSysNo);
        VisitCustomer InsertVisitCustomer(VisitCustomer info);
        VisitCustomer UpdateVisitCustomer(VisitCustomer info);
        VisitLog InsertVisitBlackCustomer(VisitLog info);
        VisitCustomer GetVisitCustomerBySysNo(int visitSysNo);
        VisitLog InsertOrderVisitLog(VisitLog info);

        List<VisitLog> GetCustomerVisitLogsByCustomerSysNo(int customerSysNo);
        List<VisitLog> GetOrderVisitLogsByCustomerSysNo(int customerSysNo);
        List<VisitLog> GetCustomerVisitLogsByVisitSysNo(int visitSysNo);
        List<VisitLog> GetOrderVisitLogsByVisitSysNo(int visitSysNo);

        List<VisitOrder> GetVisitOrderByCustomerSysNo(int customerSysNo, string companyCode);
    }
}
