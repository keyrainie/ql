using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.BizProcessor;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(QCSubjectAppService))]
    public class QCSubjectAppService
    {

        public void Create(BizEntity.Customer.QCSubject entity)
        {
            ObjectFactory<QCSubjectProcessor>.Instance.Create(entity);
        }

        public void Update(BizEntity.Customer.QCSubject entity)
        {
            ObjectFactory<QCSubjectProcessor>.Instance.Update(entity);
        }

        public List<BizEntity.Customer.QCSubject> GetParentsQCSubject(QCSubject entity)
        {
            return ObjectFactory<QCSubjectProcessor>.Instance.GetParentsQCSubject(entity);
        }
    }
}
