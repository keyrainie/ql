using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
//using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(ExperienceHallAppService))]
    public class ExperienceHallAppService
    {
        private ExperienceHallProcessor ExperienceProcessor = ObjectFactory<ExperienceHallProcessor>.Instance;

        #region 调拨单维护

        /// <summary>
        /// 根据调拨单的SysNo获取调拨单的全部信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual ExperienceInfo GetExperienceInfoBySysNo(int requestSysNo)
        {
            ExperienceInfo requestInfo = ExperienceProcessor.GetExperienceInfoBySysNo(requestSysNo);
            return requestInfo;
        }

        /// <summary>
        /// 创建调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ExperienceInfo CreateExperience(ExperienceInfo entityToCreate)
        {
            ExperienceInfo resultRequest = ExperienceProcessor.CreateRequest(entityToCreate);

            return resultRequest;
        }

        /// <summary>
        /// 更新调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ExperienceInfo UpdateRequest(ExperienceInfo entityToUpdate)
        {
            ExperienceInfo resultRequest = ExperienceProcessor.UpdateRequest(entityToUpdate);
            return resultRequest;
        }

        /// <summary>
        /// 审核调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void AuditExperience(ExperienceInfo entityToUpdate)
        {
            ExperienceProcessor.AuditExperience(entityToUpdate);
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sysno"></param>
        public virtual void CancelAuditExperience(ExperienceInfo entityToUpdate)
        {
            ExperienceProcessor.CancelAuditExperience(entityToUpdate);
        }

        public virtual void ExperienceInOrOut(ExperienceInfo entityToUpdate)
        {
            ExperienceProcessor.ExperienceInOrOut(entityToUpdate);
        }

        /// <summary>
        /// 作废调拨单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void AbandonExperience(ExperienceInfo entityToUpdate)
        {
            ExperienceProcessor.AbandonExperience(entityToUpdate);
        }

        #endregion
    }
}
