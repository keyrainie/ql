using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.BizEntity.PO.Commission;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(CollectionPaymentAppService))]
    public class CollectionPaymentAppService
    {
        #region [Fields]
        private CollectionPaymentProcessor m_Processor;

        public CollectionPaymentProcessor Processor
        {
            get
            {
                if (null == m_Processor)
                {
                    m_Processor = ObjectFactory<CollectionPaymentProcessor>.Instance;
                }
                return m_Processor;
            }

        }
        #endregion

        public virtual CollectionPaymentInfo Load(int sysNo)
        {
            return Processor.Load(sysNo);
        }


        public virtual CollectionPaymentInfo Create(CollectionPaymentInfo info)
        {
            return Processor.Create(info);
        }

        public virtual CollectionPaymentInfo Audit(CollectionPaymentInfo consignInfo)
        {
            return Processor.Audit(consignInfo);
        }
        public virtual CollectionPaymentInfo CancelAudited(CollectionPaymentInfo consignInfo)
        {
            return Processor.CancelAudited(consignInfo);
        }

        public virtual CollectionPaymentInfo Update(CollectionPaymentInfo consignInfo)
        {
            return Processor.Update(consignInfo);
        }

        public virtual CollectionPaymentInfo Abandon(CollectionPaymentInfo consignInfo)
        {
            return Processor.Abandon(consignInfo);
        }
        public virtual CollectionPaymentInfo CancelAbandon(CollectionPaymentInfo consignInfo)
        {
            return Processor.CancelAbandon(consignInfo);
        }
        public virtual CollectionPaymentInfo Settle(CollectionPaymentInfo consignInfo)
        {
            return Processor.Settle(consignInfo);
        }
        public virtual CollectionPaymentInfo CancelSettled(CollectionPaymentInfo consignInfo)
        {
            return Processor.CancelSettled(consignInfo);
        }

        /// <summary>
        ///2012-9-14 Jack 根据不同权限获取PMList:
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="currentUserName"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual List<int> GetPMSysNoListByType(ConsignSettlementBizInfo info)
        {
            return ObjectFactory<ConsignSettlementProcessor>.Instance.GetPMSysNoListByType(info);
        }

        public virtual POBatchInfo POBatchInstock(POBatchInfo info)
        {
          return   Processor.POBatchInstock(info);
        }
    }
}
