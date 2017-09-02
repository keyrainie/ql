using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity;
using System.Transactions;
namespace ECCentral.Service.ExternalSYS.BizProcessor
{
    [VersionExport(typeof(CpsUserProcessor))]
    public class CpsUserProcessor
    {
        private ICpsUserDA cpsUserDA = ObjectFactory<ICpsUserDA>.Instance;
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="info"></param>
        public void UpdateUserStatus(CpsUserInfo info)
        {

            cpsUserDA.UpdateUserStatus(info);

        }

        /// <summary>
        /// 更新Source
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateUserSource(CpsUserInfo info)
        {
            if (!cpsUserDA.IsExistsUserSource(info))
            {
                cpsUserDA.UpdateUserSource(info);
            }
            else
            {
                throw new BizException("已存在的Source,无法修改!");
            }
        }
        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateBasicUser(CpsUserInfo info)
        {
            cpsUserDA.UpdateBasicUser(info);

        }

        /// <summary>
        /// 更新收款账户信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCpsReceivablesAccount(CpsUserInfo info)
        {
            cpsUserDA.UpdateCpsReceivablesAccount(info);
        }
        /// <summary>
        /// 创建source
        /// </summary>
        /// <param name="info"></param>
        public void CreateUserSource(CpsUserInfo info)
        {
            if (!cpsUserDA.IsExistsUserSource(info))
            {
                cpsUserDA.CreateUserSource(info);
            }
            else
            {
                throw new BizException("已存在的Source,无法创建!");
            }

        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditUser(CpsUserInfo info)
        {
            if (info.Status == AuditStatus.AuditClearance) //审核通过
            {
                cpsUserDA.AuditUser(info);
            }
            if (info.Status == AuditStatus.AuditNoClearance) //审核拒绝
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    cpsUserDA.AuditUser(info);
                    cpsUserDA.InsertChangeLog(info);
                    scope.Complete();
                }
            }
        }



    }
}
