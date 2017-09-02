using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess.NeweggCN
{
    [VersionExport(typeof(IQCSubjectDA))]
    public class QCSubjectDA : IQCSubjectDA
    {
        #region IQCSubjectDA Members

        public void Create(BizEntity.Customer.QCSubject entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateQCSubject");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }

        public List<BizEntity.Customer.QCSubject> GetParentsQCSubject(int sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetParentsQCSubject");
            cmd.SetParameterValue("@SysNo", sysno);
            return cmd.ExecuteEntityList<QCSubject>();
        }

        public List<BizEntity.Customer.QCSubject> GetChildrenQCSubject(int parentSysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetChildrenQCSubject");
            cmd.SetParameterValue("@SysNo", parentSysno);
            return cmd.ExecuteEntityList<QCSubject>();
        }

        public void Update(BizEntity.Customer.QCSubject entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateQCSubject");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
