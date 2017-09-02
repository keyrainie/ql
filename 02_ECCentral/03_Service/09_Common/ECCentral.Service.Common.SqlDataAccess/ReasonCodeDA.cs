using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IReasonCodeDA))]
    public class ReasonCodeDA : IReasonCodeDA
    {
        #region IReasonCodeDA Members

        //[FlushCache(typeof(ReasonCodeDA), "GetAllReasonCode")]
        public virtual ReasonCodeEntity InsertReasonCode(BizEntity.Common.ReasonCodeEntity entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertReasonCode");
            cmd.SetParameterValue<ReasonCodeEntity>(entity);
            cmd.ExecuteNonQuery();
            object result = cmd.GetParameterValue("@SysNo");
            entity.SysNo = Convert.ToInt32(result);
            return entity;
        }

        //[FlushCache(typeof(ReasonCodeDA), "GetAllReasonCode")]
        public virtual int UpdateReasonCode(BizEntity.Common.ReasonCodeEntity query)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateReasonCode");
            cmd.SetParameterValue<ReasonCodeEntity>(query);
            return cmd.ExecuteNonQuery();
        }

        //[FlushCache(typeof(ReasonCodeDA), "GetAllReasonCode")]
        public virtual int UpdateReasonStatusList(string strWhere, int status, string editUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateReasonStatusList");

            cmd.ReplaceParameterValue("#StrWhere#", strWhere);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@EditUser", editUser);
            return cmd.ExecuteNonQuery();
        }

        //[Caching(ExpiryType = ExpirationType.Never)]
        public virtual List<ReasonCodeEntity> GetAllReasonCode()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllReasonCode");
            return cmd.ExecuteEntityList<ReasonCodeEntity>();
        }
        public virtual List<ReasonCodeEntity> GetAllReasonCodeByCompany(string companyCode)
        {
            return GetAllReasonCode().Where(p => p.CompanyCode.Trim() == companyCode).ToList();
        }
        public virtual ReasonCodeEntity GetReasonCodeBySysNo(int sysNo)
        {
            return GetAllReasonCode().Where(p => p.SysNo == sysNo).FirstOrDefault();
        }

        public virtual List<BizEntity.Common.ReasonCodeEntity> GetReasonCodeByNodeLevel(int nodeLevel, string companyCode)
        {
            return GetAllReasonCodeByCompany(companyCode).Where(p => p.NodeLevel <= nodeLevel).ToList();
        }

        public virtual List<BizEntity.Common.ReasonCodeEntity> GetChildrenReasonCode(int parentNodeSysNo)
        {
            return GetAllReasonCode().Where(p => p.ParentNodeSysNo == parentNodeSysNo).ToList();
        }

        #endregion
    }
}
