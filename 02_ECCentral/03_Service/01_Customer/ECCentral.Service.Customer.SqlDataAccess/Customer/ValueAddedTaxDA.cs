using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IValueAddedTaxDA))]
    public class ValueAddedTaxDA : IValueAddedTaxDA
    {
        #region IValueAddedTaxDA Members

        public virtual BizEntity.Customer.ValueAddedTaxInfo CreateValueAddedTaxInfo(ValueAddedTaxInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertCustomerVATInfo");
            cmd.SetParameterValue<ValueAddedTaxInfo>(entity);
            cmd.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity;
        }

        public virtual void UpdateValueAddedTaxInfo(ValueAddedTaxInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerVATInfo");
            cmd.SetParameterValue<ValueAddedTaxInfo>(entity);
            cmd.ExecuteNonQuery();
        }

        public virtual List<ValueAddedTaxInfo> QueryValueAddedTaxInfo(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerVATInfoByCustomerSysNo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteEntityList<ValueAddedTaxInfo>();
        }
        #endregion IValueAddedTaxDA Members
    }
}