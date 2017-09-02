using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(QCSubjectProcessor))]
    public class QCSubjectProcessor
    {
        public void Create(BizEntity.Customer.QCSubject entity)
        {
            var siblings = GetChildrenQCSubject(entity.ParentSysNo.Value);
            if (siblings != null)
            {
                if (siblings.Where(p => p.Name == entity.Name).Count() > 0)
                {
                    throw new BizException("已经存在该名称的类目.");
                }
            }
            ObjectFactory<IQCSubjectDA>.Instance.Create(entity);
            ResetOrder(entity);
        }

        public void Update(BizEntity.Customer.QCSubject entity)
        {
            var siblings = GetChildrenQCSubject(entity.ParentSysNo.Value);
            if (siblings != null)
            {
                if (siblings.Where(p => p.Name == entity.Name && p.SysNo != entity.SysNo).Count() > 0)
                {
                    throw new BizException("已经存在该名称的类目.");
                }
            }
            ObjectFactory<IQCSubjectDA>.Instance.Update(entity);
            ResetOrder(entity);
        }

        private void ResetOrder(QCSubject current)
        {
            var siblings = GetChildrenQCSubject(current.ParentSysNo.Value);
            if (siblings != null)
            {
                for (int i = 0; i < siblings.Count(); i++)
                {
                    QCSubject oragin = siblings[i];
                    if (oragin.OrderNum != i + 1)
                    {
                        oragin.OrderNum = i + 1;
                        ObjectFactory<IQCSubjectDA>.Instance.Update(oragin);
                    }
                }
            }
        }

        public List<BizEntity.Customer.QCSubject> GetParentsQCSubject(QCSubject entity)
        {
            return ObjectFactory<IQCSubjectDA>.Instance.GetParentsQCSubject(entity.SysNo.Value);
        }

        private List<BizEntity.Customer.QCSubject> GetChildrenQCSubject(int parentSysno)
        {
            return ObjectFactory<IQCSubjectDA>.Instance.GetChildrenQCSubject(parentSysno);
        }
    }
}
