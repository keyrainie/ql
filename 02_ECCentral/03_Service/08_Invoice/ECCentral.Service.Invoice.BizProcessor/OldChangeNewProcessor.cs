using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.BizEntity;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(OldChangeNewProcessor))]
    public class OldChangeNewProcessor
    {
        private IOldChangeNewDA m_OldChangeNewDA = ObjectFactory<IOldChangeNewDA>.Instance;

        /// <summary>
        /// 创建以旧换新信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo Create(OldChangeNewInfo info)
        {
            return m_OldChangeNewDA.Create(info);
        }

        /// <summary>
        /// 更新以旧换新折扣金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo UpdateOldChangeNewRebate(OldChangeNewInfo info)
        {
            if (info.SysNo < 1)
            {
                //throw new BizException("无效的SysNo！");
                throw new BizException(ResouceManager.GetMessageString("Invoice.SOIncome","SOIncome_DeActiveSysNo"));
            }
            return m_OldChangeNewDA.UpdateOldChangeNewRebate(info);
        }

        /// <summary>
        /// 更新以旧换新状态信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo UpdatOldChangeNewStatus(OldChangeNewInfo info)
        {
            if (info.SysNo < 1)
            {
                //throw new BizException("无效的SysNo！");
                 throw new BizException(ResouceManager.GetMessageString("Invoice.SOIncome","SOIncome_DeActiveSysNo"));
            }
            return m_OldChangeNewDA.UpdateOldChangeNewStatus(info);
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo MaintainReferenceID(OldChangeNewInfo info)
        {
            return m_OldChangeNewDA.MaintainReferenceID(info);
        }

        /// <summary>
        /// 添加财务备注
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo MaintainStatusWithNote(OldChangeNewInfo info)
        {
            return m_OldChangeNewDA.MaintainStatusWithNote(info);
        }
    }
}
