using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(NeweggAmbassadorAppService))]
    public class NeweggAmbassadorAppService
    {
        private NeweggAmbassadorProcessor _neweggAmbassadorProcessor = ObjectFactory<NeweggAmbassadorProcessor>.Instance;

        /// <summary>
        /// 更新泰隆优选大使的状态信息，即激活或取消泰隆优选大使。
        /// </summary>
        /// <param name="batchInfo"></param>
        public void MaintainNeweggAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {
            _neweggAmbassadorProcessor.MaintainNeweggAmbassadorStatus(batchInfo);
        }

        /// <summary>
        /// 尝试更新泰隆优选大使的状态，返回需要确认的泰隆优选大使。
        /// </summary>
        /// <param name="batchInfo"></param>
        /// <returns></returns>
        public NeweggAmbassadorBatchInfo TryUpdateAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {
            return _neweggAmbassadorProcessor.TryUpdateAmbassadorStatus(batchInfo);
        }

        /// <summary>
        /// 取消申请。
        /// </summary>
        /// <param name="batchInfo"></param>
        public void CancelRequestNeweggAmbassadorStatus(NeweggAmbassadorBatchInfo batchInfo)
        {
            _neweggAmbassadorProcessor.CancelRequestNeweggAmbassadorStatus(batchInfo);
        }

    }
}
