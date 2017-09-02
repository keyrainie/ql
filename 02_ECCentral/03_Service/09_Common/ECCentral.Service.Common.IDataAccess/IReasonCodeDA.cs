using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IReasonCodeDA
    {
        ReasonCodeEntity InsertReasonCode(ReasonCodeEntity entity);
        int UpdateReasonCode(ReasonCodeEntity query);
        ReasonCodeEntity GetReasonCodeBySysNo(int sysNo);
        List<ReasonCodeEntity> GetReasonCodeByNodeLevel(int nodeLevel, string companyCode);
        List<ReasonCodeEntity> GetChildrenReasonCode(int parentNodeSysNo);
        int UpdateReasonStatusList(string strWhere, int status, string editUser);
        List<ReasonCodeEntity> GetAllReasonCodeByCompany(string companyCode);
    }
}
