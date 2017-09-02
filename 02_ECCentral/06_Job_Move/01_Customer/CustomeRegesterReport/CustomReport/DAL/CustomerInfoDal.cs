using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomReport.Model;
using Newegg.Oversea.Framework.DataAccess;

namespace CustomReport.DAL
{
    public class CustomerInfoDal
    {
        public List<CustomerInfo> GetCustomerInfo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerInfo");
            command.SetParameterValue("@BeginRegisterTime",DateTime.Now.Date.AddDays(-7));
            command.SetParameterValue("@EndRegisterTime",DateTime.Now.Date);
            return command.ExecuteEntityList<CustomerInfo>();
        }
    }
}
