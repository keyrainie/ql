using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(ICustomerContactDA))]
    public class CustomerContactDA : ICustomerContactDA
    {
        public CustomerContactInfo Insert(CustomerContactInfo entity)
        {
            DataCommand insertCommand = DataCommandManager.GetDataCommand("InsertCustomerContact");
            insertCommand.SetParameterValue<CustomerContactInfo>(entity);           

            insertCommand.ExecuteNonQuery();
            entity.SysNo = (int)insertCommand.GetParameterValue("@SysNo");
            return entity;
        }

        public bool Update(CustomerContactInfo entity)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateCustomerContactInfo");

            updateCommand.SetParameterValue<CustomerContactInfo>(entity);   


            return updateCommand.ExecuteNonQuery() > 0;
        }

        public bool UpdateByRequestSysNo(CustomerContactInfo entity)
        {
            DataCommand updateCommand = DataCommandManager.GetDataCommand("UpdateCustomerContactInfoByRequestSysNo");

            updateCommand.SetParameterValue<CustomerContactInfo>(entity);   

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public CustomerContactInfo Load(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("GetCustomerContactBySysNo");
            selectCommand.SetParameterValue("@SysNo", sysNo);           

            return selectCommand.ExecuteEntity<CustomerContactInfo>();
        }

        public CustomerContactInfo LoadOrigin(int sysNo)
        {
            DataCommand selectCommand = DataCommandManager.GetDataCommand("GetOriginCustomerContactBySysNo");
            selectCommand.SetParameterValue("@SysNo", sysNo);            

            return selectCommand.ExecuteEntity<CustomerContactInfo>();
        }

        public CustomerContactInfo LoadByRegisterSysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerByRegisterSysNo");

            command.SetParameterValue("@RegisterSysNo", sysNo);

            return command.ExecuteEntity<CustomerContactInfo>();            
        }
    }
}
