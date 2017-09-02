using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(OldChangeNewAppService))]
    public class OldChangeNewAppService
    {
        private OldChangeNewProcessor processor = ObjectFactory<OldChangeNewProcessor>.Instance;

        /// <summary>
        /// 创建以旧换新信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo Create(OldChangeNewInfo info)
        {
            return processor.Create(info);
        }

        /// <summary>
        /// 更新以旧换新折扣金额
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo UpdateOldChangeNewRebate(OldChangeNewInfo info)
        {
            return processor.UpdateOldChangeNewRebate(info);
        }

        /// <summary>
        /// 更新以旧换新状态信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo UpdateOldChangeNewStatus(OldChangeNewInfo info)
        {
            return processor.UpdatOldChangeNewStatus(info);
        }

        /// <summary>
        /// 批量更新以旧换新状态信息
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public virtual string BatchUpdateOldChangeNewStatus(List<OldChangeNewInfo> infos)
        {
            var request = infos.Select(s => new BatchActionItem<OldChangeNewInfo>()
            {
                ID = s.SysNo.ToString(),
                Data = s
            }).ToList();

            var result = BatchActionManager.DoBatchAction(request, info => this.UpdateOldChangeNewStatus(info));
            return result.PromptMessage;
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo MaintainReferenceID(OldChangeNewInfo info)
        {
            return processor.MaintainReferenceID(info);
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public virtual string BatchMaintainReferenceID(List<OldChangeNewInfo> infos)
        {
            var request = infos.Select(s => new BatchActionItem<OldChangeNewInfo>()
                {
                    ID = s.SysNo.ToString(),
                    Data = s
                }).ToList();

            var result = BatchActionManager.DoBatchAction(request, info => this.MaintainReferenceID(info));
            return result.PromptMessage;
        }

        /// <summary>
        /// 添加财务备注
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual OldChangeNewInfo MaintainStatusWithNote(OldChangeNewInfo info)
        {
            return processor.MaintainStatusWithNote(info);
        }
    }
}
