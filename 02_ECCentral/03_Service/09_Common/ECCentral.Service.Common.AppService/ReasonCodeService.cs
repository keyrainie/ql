using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(ReasonCodeService))]
    public class ReasonCodeService
    {
        public virtual ReasonCodeEntity Create(ReasonCodeEntity entity)
        {
            return ObjectFactory<ReasonCodeProcessor>.Instance.Create(entity);
        }

        public virtual void Update(ReasonCodeEntity entity)
        {
            ObjectFactory<ReasonCodeProcessor>.Instance.Update(entity);
        }

        public virtual void UpdateStatusList(List<ReasonCodeEntity> list)
        {
            ObjectFactory<ReasonCodeProcessor>.Instance.UpdateReasonStatusList(list);
        }

        public virtual ReasonCodeEntity GetReasonCodeBySysNo(int SysNo)
        {
            return ObjectFactory<ReasonCodeProcessor>.Instance.GetReasonCodeBySysNo(SysNo);
        }

        public virtual List<ReasonCodeEntity> GetChildrenReasonCode(int parentSysNo)
        {
            return ObjectFactory<ReasonCodeProcessor>.Instance.GetChildrenReasonCode(parentSysNo);
        }

        public virtual List<ReasonCodeEntity> GetReasonCodeByNodeLevel(int level, string companyCode)
        {
            return ObjectFactory<ReasonCodeProcessor>.Instance.GetReasonCodeByNodeLevel(level, companyCode);
        }
        public virtual string GetReasonCodePath(int sysNo, string companyCode)
        {
            return ObjectFactory<ReasonCodeProcessor>.Instance.GetReasonCodePath(sysNo, companyCode);
        }
        /// <summary>
        /// 根据codeSysNo查询ReasonCode路径
        /// </summary>
        /// <param name="list">ReasonCodeID 和 companyCode必须指定 </param>
        /// <returns></returns>
        public virtual List<ReasonCodeEntity> GetReasonCodePathList(List<ReasonCodeEntity> list)
        {
            foreach (var item in list)
            {
                if (item.SysNo.HasValue)
                {
                    item.Path = ObjectFactory<ReasonCodeService>.Instance.GetReasonCodePath(item.SysNo.Value, item.CompanyCode);
                }
            }
            return list;
        }
    }
}
