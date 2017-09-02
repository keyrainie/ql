using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NeweggCN.NoBizQuery
{
    public interface IQCSubjectQueryDA
    {
          List<QCSubject> GetAllQCSubject(QCSubjectFilter filter);
    }
}
