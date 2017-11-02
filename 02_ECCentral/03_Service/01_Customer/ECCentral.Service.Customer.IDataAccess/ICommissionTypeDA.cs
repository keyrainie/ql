using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Customer.Society;
using ECCentral.QueryFilter.Customer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface ICommissionTypeDA
    {
        CommissionType Create(CommissionType entity);

        CommissionType Update(CommissionType entity);

        CommissionType QueryCommissionType(int sysNo);

    

        bool IsExistCommissionTypeID(string commissionTypeID);

        bool IsExistCommissionTypeName(string commissionTypeName);

        List<CommissionType> GetCommissionTypeListBySysNo(string sysNo);
        #region 扩展属性
        CommissionType QueryCommissionType(string societyID);
        #endregion
    }
}
