using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity;
using ECCentral.Service.PO.BizProcessor;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(CommissionAppService))]
    public class CommissionAppService
    {
        #region [Fields]
        private CommissionProcessor m_CommissionProcessor;

        public CommissionProcessor CommissionProcessor
        {
            get
            {
                if (null == m_CommissionProcessor)
                {
                    m_CommissionProcessor = ObjectFactory<CommissionProcessor>.Instance;
                }
                return m_CommissionProcessor;
            }
        }
        #endregion


        /// <summary>
        /// 关闭佣金
        /// </summary>
        /// <param name="commissionMaster"></param>
        /// <returns></returns>
        public CommissionMaster CloseCommission(CommissionMaster commissionMaster)
        {
            return CommissionProcessor.CloseCommission(commissionMaster);
        }

        /// <summary>
        /// 批量关闭佣金
        /// </summary>
        /// <param name="commissionSysNos"></param>
        /// <returns></returns>
        public int BatchCloseCommission(string commissionSysNos)
        {
            if (string.IsNullOrEmpty(commissionSysNos))
            {
                throw new BizException("请先选择一个要关闭的佣金帐扣单!");
            }
            string[] strs = commissionSysNos.Split(';');
            List<CommissionMaster> commissionList = new List<CommissionMaster>();
            foreach (var item in strs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    commissionList.Add(new CommissionMaster() { SysNo = int.Parse(item) });
                }
            }
            return CommissionProcessor.BatchCloseCommissions(commissionList);

        }

        /// <summary>
        /// 创建新的佣金规则
        /// </summary>
        /// <param name="newCommissionRule"></param>
        /// <returns></returns>
        public CommissionRule CreateCommission(CommissionRule newCommissionRule)
        {
            return newCommissionRule;
        }

        public CommissionMaster LoadCommissionMaseterInfo(int commissionSysNo)
        {
            if (commissionSysNo == 0)
                return null;
            return CommissionProcessor.LoadCommissionInfo(commissionSysNo);
        }

        public CommissionMaster CreateSettleCommission(CommissionMaster req)
        {
            return CommissionProcessor.CreateSettleCommission(req);
        }

        public CommissionMaster GetManualCommissionMaster(CommissionMaster req)
        {
            return CommissionProcessor.GetManualCommissionMaster(req);
        }
    }
}
