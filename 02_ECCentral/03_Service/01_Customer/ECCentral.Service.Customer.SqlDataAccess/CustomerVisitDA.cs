using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data.Common;
using System.Data.SqlClient;
using ECCentral.Service.Customer.IDataAccess;

namespace ECCentral.Service.Customer.SqlDataAccess
{

    [VersionExport(typeof(ICustomerVisitDA))]
    public class CustomerVisitDA : ICustomerVisitDA
    {
        public virtual VisitCustomer InsertVisitCustomer(VisitCustomer info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Insert_VisitCustomer");
            cmd.SetParameterValue<VisitCustomer>(info);
            cmd.ExecuteNonQuery();
            info.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return info;

        }

        public virtual VisitCustomer UpdateVisitCustomer(VisitCustomer info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Update_VisitCustomer");
            cmd.SetParameterValue<VisitCustomer>(info);
            cmd.ExecuteNonQuery();
            return info;
        }

        public virtual VisitLog InsertCustomerVisitLog(VisitLog info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Insert_VisitLog");
            cmd.SetParameterValue<VisitLog>(info);
            cmd.ExecuteNonQuery();
            return info;
        }

        public virtual VisitLog InsertOrderVisitLog(VisitLog info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Insert_OrderVisitLog");
            cmd.SetParameterValue<VisitLog>(info);
            cmd.ExecuteNonQuery();
            return info;
        }

        public virtual VisitLog InsertVisitBlackCustomer(VisitLog info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Insert_VisitBlackList");
            cmd.SetParameterValue<VisitLog>(info);
            cmd.ExecuteNonQuery();
            return info;


        }

        #region 加载方法

        public virtual VisitCustomer GetVisitCustomerByCustomerSysNo(int customerSysNo)
        {
            VisitCustomer info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Get_VisitCustomerByCustomerSysNo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                info = DataMapper.TryGetEntity<VisitCustomer>(reader, out info) ? info : null;
            }
            return info;
        }

        public virtual VisitCustomer GetVisitCustomerBySysNo(int visitSysNo)
        {
            VisitCustomer info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Get_VisitCustomerBySysNo");
            cmd.SetParameterValue("@SysNo", visitSysNo);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                info = DataMapper.TryGetEntity<VisitCustomer>(reader, out info) ? info : null;
            }
            return info;
        }

        public virtual List<VisitOrder> GetVisitOrderByCustomerSysNo(int customerSysNo, string companyCode)
        {
            List<VisitOrder> visitOrder = null;
            DataCommand dc = DataCommandManager.GetDataCommand("Customer_Get_VisitOrderByCustomerSysNo");
            dc.SetParameterValue("@CustomerSysNo", customerSysNo);
            dc.SetParameterValue("@CompanyCode", companyCode);
            visitOrder = dc.ExecuteEntityList<VisitOrder>();
            return visitOrder;
        }

        public virtual List<VisitLog> GetCustomerVisitLogsByCustomerSysNo(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Get_CustomerVisitLogsByCustomerSysNo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            List<VisitLog> logs = new List<VisitLog>();
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                logs = DataMapper.GetEntityList<VisitLog, List<VisitLog>>(reader);
            }
            return logs;
        }

        public virtual List<VisitLog> GetOrderVisitLogsByCustomerSysNo(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Get_OrderVisitLogsByCustomerSysNo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            List<VisitLog> logs = new List<VisitLog>();
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                logs = DataMapper.GetEntityList<VisitLog, List<VisitLog>>(reader);
            }
            return logs;
        }

        public virtual List<VisitLog> GetCustomerVisitLogsByVisitSysNo(int visitSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Get_CustomerVisitLogsByVisitSysNo");
            cmd.SetParameterValue("@VisitSysNo", visitSysNo);
            List<VisitLog> logs = new List<VisitLog>();
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                logs = DataMapper.GetEntityList<VisitLog, List<VisitLog>>(reader);
            }
            return logs;
        }

        public virtual List<VisitLog> GetOrderVisitLogsByVisitSysNo(int visitSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_Visit_Get_OrderVisitLogsByVisitSysNo");
            cmd.SetParameterValue("@VisitSysNo", visitSysNo);
            List<VisitLog> logs = new List<VisitLog>();
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                logs = DataMapper.GetEntityList<VisitLog, List<VisitLog>>(reader);
            }
            return logs;
        }

        #endregion

    }
}
