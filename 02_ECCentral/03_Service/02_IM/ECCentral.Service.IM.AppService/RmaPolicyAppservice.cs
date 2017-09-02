using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
   [VersionExport(typeof(RmaPolicyAppservice))]
   public class RmaPolicyAppservice
    {
        /// <summary>
        /// 创建退换货信息
        /// </summary>
        /// <param name="info"></param>
        public void CreateRmaPolicy(RmaPolicyInfo info)
        {
            ObjectFactory<RmaPolicyProcessor>.Instance.CreateRmaPolicy(info);
        }
        /// <summary>
        /// 更新退换货信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateRmaPolicy(RmaPolicyInfo info)
        {
            ObjectFactory<RmaPolicyProcessor>.Instance.UpdateRmaPolicy(info);
        }

        /// <summary>
        ///作废
        /// </summary>
        /// <param name="sysNo"></param>
        public void DeActiveRmaPolicy(List<RmaPolicyInfo> list)
        {
            ObjectFactory<RmaPolicyProcessor>.Instance.DeActiveRmaPolicy(list);
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="sysNo"></param>
        public void ActiveRmaPolicy(List<RmaPolicyInfo> list)
        {
             ObjectFactory<RmaPolicyProcessor>.Instance.ActiveRmaPolicy(list);
        }
    }
}
