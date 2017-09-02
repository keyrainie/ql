using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.BizProcessor;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.AppService
{
    [VersionExport(typeof(CpsUserAppService))]
    public class CpsUserAppService
    {
        private CpsUserProcessor processor = ObjectFactory<CpsUserProcessor>.Instance;
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="info"></param>
        public void UpdateUserStatus(CpsUserInfo info)
        {

            processor.UpdateUserStatus(info);

        }

        /// <summary>
        /// 更新Source
        /// </summary>
        /// <param name="Info"></param>
        public void UpdateUserSource(CpsUserInfo info)
        {
            processor.UpdateUserSource(info);


        }
        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateBasicUser(CpsUserInfo info)
        {
            processor.UpdateBasicUser(info);

        }

        /// <summary>
        /// 更新收款账户信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCpsReceivablesAccount(CpsUserInfo info)
        {
            processor.UpdateCpsReceivablesAccount(info);
        }
        /// <summary>
        /// 创建source
        /// </summary>
        /// <param name="info"></param>
        public void CreateUserSource(CpsUserInfo info)
        {
            processor.CreateUserSource(info);

        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditUser(CpsUserInfo info)
        {
            processor.AuditUser(info);
        }
    }
}
