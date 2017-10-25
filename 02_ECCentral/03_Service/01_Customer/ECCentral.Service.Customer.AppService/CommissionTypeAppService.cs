using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Customer.Society;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CommissionTypeAppService))]
    public class CommissionTypeAppService
    {
        public virtual CommissionType Create(CommissionType entity)
        {
            return ObjectFactory<CommissionTypeProcessor>.Instance.Create(entity);
        }

        public virtual CommissionType Update(CommissionType entity)
        {
            return ObjectFactory<CommissionTypeProcessor>.Instance.Update(entity);
        }

        public CommissionType QueryCommissionType(int sysNo)
        {
            return ObjectFactory<CommissionTypeProcessor>.Instance.QueryCommissionType(sysNo);
        }

        public ECCentral.Service.Utility.WCF.QueryResult QueryCommissionType(CommissionTypeQueryFilter request, out int totalCount)
        {
            totalCount = 0;
            return ObjectFactory<CommissionTypeProcessor>.Instance.QueryCommissionType(request, out totalCount);
        }
        #region 扩展
        public ECCentral.Service.Utility.WCF.QueryResult SocietyCommissionQuery(CommissionTypeQueryFilter request, out int totalCount)
        {
            totalCount = 0;
            return ObjectFactory<CommissionTypeProcessor>.Instance.SocietyCommissionQuery(request, out totalCount);
        }
        #endregion
    }
}
