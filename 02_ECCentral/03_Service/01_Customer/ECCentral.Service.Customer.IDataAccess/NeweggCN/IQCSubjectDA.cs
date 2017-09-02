using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IQCSubjectDA
    {
        void Create(BizEntity.Customer.QCSubject entity);

        /// <summary>
        /// 获取当前节点的父节点的兄弟节点
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        List<BizEntity.Customer.QCSubject> GetParentsQCSubject(int sysno);

        List<BizEntity.Customer.QCSubject> GetChildrenQCSubject(int parentSysno);

        void Update(BizEntity.Customer.QCSubject entity);
    }
}
