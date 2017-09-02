using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using System.Data;
using ECCentral.BizEntity.PO.Settlement;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(ConsignSettlementAppService))]
    public class ConsignSettlementAppService
    {
        #region [Fields]
        private ConsignSettlementProcessor m_ConsignSettlementProcessor;

        public ConsignSettlementProcessor ConsignSettlementProcessor
        {
            get
            {
                if (null == m_ConsignSettlementProcessor)
                {
                    m_ConsignSettlementProcessor = ObjectFactory<ConsignSettlementProcessor>.Instance;
                }
                return m_ConsignSettlementProcessor;
            }

        }
        #endregion


        public virtual ConsignSettlementInfo CreateConsignSettlementInfo(ConsignSettlementInfo info)
        {
            return ConsignSettlementProcessor.CreateConsignSettlementInfo(info, false);
        }
        public virtual ConsignSettlementInfo CreateConsignSettlementInfoBySystem(ConsignSettlementInfo info)
        {
            return ConsignSettlementProcessor.CreateConsignSettlementInfo(info, true);
        }

        public virtual ConsignSettlementInfo SystemCreateConsignSettlementInfo(ConsignSettlementInfo info)
        {
            return ConsignSettlementProcessor.CreateConsignSettlementInfo(info, true);
        }
        public virtual ConsignSettlementInfo LoadConsignSettlementInfoBySysNo(int consignSettleSysNo)
        {
            return ConsignSettlementProcessor.LoadConsignSettlementInfo(consignSettleSysNo);
        }

        public virtual ConsignSettlementInfo UpdateConsignSettlementInfo(ConsignSettlementInfo consignInfo)
        {
            return ConsignSettlementProcessor.UpdateConsignSettlementInfo(consignInfo);
        }

        public virtual ConsignSettlementInfo AbandonConsignSettlementInfo(ConsignSettlementInfo consignInfo)
        {
            return ConsignSettlementProcessor.AbandonConsignSettlement(consignInfo);
        }

        public virtual ConsignSettlementInfo CancelAbandonSettlementInfo(ConsignSettlementInfo consignInfo)
        {
            return ConsignSettlementProcessor.CancelAbandonConsignSettlement(consignInfo);
        }

        public virtual ConsignSettlementInfo SettleConsignSettlementInfo(ConsignSettlementInfo consignInfo)
        {
            return ConsignSettlementProcessor.SettleConsignSettlement(consignInfo);
        }

        public virtual ConsignSettlementInfo CancelSettleConsignSettlement(ConsignSettlementInfo consignInfo)
        {
            return ConsignSettlementProcessor.CancelSettleConsignSettlement(consignInfo);
        }

        public virtual ConsignSettlementInfo AuditConsignSettlement(ConsignSettlementInfo consignInfo)
        {
            return ConsignSettlementProcessor.AuditConsignSettlement(consignInfo);
        }

        public virtual ConsignSettlementInfo CancelAuditConsignSettlement(ConsignSettlementInfo consignInfo)
        {
            return ConsignSettlementProcessor.CancalAuditConsignSettlement(consignInfo);
        }

        public virtual List<ConsignSettlementEIMSInfo> LoadConsignEIMSList(ConsignSettlementInfo info, int? pageIndex, int? pageSize, string sortBy, out int totalCount)
        {
            return ObjectFactory<ConsignSettlementProcessor>.Instance.LoadConsignEIMSList(info, pageIndex, pageSize, sortBy, out totalCount);
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

        public DataTable GetSettleList(DataTable dt)
        {
            return ObjectFactory<ConsignSettlementProcessor>.Instance.GetSettleList(dt);
        }

        #region 经销商品结算

        /// <summary>
        /// 创建经销商品结算单
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
        public SettleInfo CreateSettleAccount(SettleInfo SettleInfo)
        {
            return ObjectFactory<ConsignSettlementProcessor>.Instance.CreateSettleAccount(SettleInfo);
        }

        /// <summary>
        /// 查询经销商品详细信息(基本信息和个子项税率信息)
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
        public SettleInfo GetSettleAccount(SettleInfo SettleInfo)
        {
            return ObjectFactory<ConsignSettlementProcessor>.Instance.GetSettleAccount(SettleInfo);
        }

        /// <summary>
        ///  审核经销商品结算单
        /// </summary>
        /// <param name="SettleInfo"></param>
        public void AuditSettleAccount(SettleInfo SettleInfo)
        {
            ObjectFactory<ConsignSettlementProcessor>.Instance.AuditSettleAccount(SettleInfo);
        }

        #endregion
    }
}
