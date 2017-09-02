using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IOldChangeNewDA
    {
        /// <summary>
        /// 创建以旧换新信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        OldChangeNewInfo Create(OldChangeNewInfo entity);

        /// <summary>
        /// 更新以旧换新折扣金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        OldChangeNewInfo UpdateOldChangeNewRebate(OldChangeNewInfo info);

        /// <summary>
        /// 更新以旧换新状态信息
        /// </summary>
        /// <param name="into"></param>
        /// <returns></returns>
        OldChangeNewInfo UpdateOldChangeNewStatus(OldChangeNewInfo into);

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        OldChangeNewInfo MaintainReferenceID(OldChangeNewInfo info);

        /// <summary>
        /// 添加财务备注
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        OldChangeNewInfo MaintainStatusWithNote(OldChangeNewInfo info);
    }
}
