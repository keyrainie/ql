using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice.Refund;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(BalanceRefundAppService))]
    public class BalanceRefundAppService
    {
        /// <summary>
        /// 创建余额退款信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual BalanceRefundInfo Create(BalanceRefundInfo entity)
        {
            return ObjectFactory<BalanceRefundProcessor>.Instance.Create(entity);
        }

        /// <summary>
        /// 更新退款信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(BalanceRefundInfo entity)
        {
            if (entity.SysNo == null || entity.SysNo == 0)
            {
                throw new ArgumentException("entity.SysNo");
            }
            ObjectFactory<BalanceRefundProcessor>.Instance.Update(entity);
        }

        /// <summary>
        /// 批量CS审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchCSConfirm(List<int> sysNoList)
        {
            List<BatchActionItem<BalanceRefundAuditInfo>> items = sysNoList.Select(x => new BatchActionItem<BalanceRefundAuditInfo>()
            {
                ID = x.ToString(),
                Data = new BalanceRefundAuditInfo { AuditUserSysNo = ServiceContext.Current.UserSysNo, SysNo = x }
            }).ToList();

            var BL = ObjectFactory<BalanceRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, (info) =>
            {
                BL.CSConfirm(info);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量财务审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchFinConfirm(List<int> sysNoList)
        {
            List<BatchActionItem<BalanceRefundAuditInfo>> items = sysNoList.Select(x => new BatchActionItem<BalanceRefundAuditInfo>()
            {
                ID = x.ToString(),
                Data = new BalanceRefundAuditInfo { AuditUserSysNo = ServiceContext.Current.UserSysNo, SysNo = x }
            }).ToList();

            var BL = ObjectFactory<BalanceRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, (info) =>
            {
                BL.FinConfirm(info);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量取消审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchCancelConfirm(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var BL = ObjectFactory<BalanceRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, sysNo =>
            {
                BL.CancelConfirm(sysNo);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchAbandon(List<int> sysNoList)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var BL = ObjectFactory<BalanceRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, sysNo =>
            {
                BL.Abandon(sysNo);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchSetReferenceID(List<int> sysNoList, string referenceID)
        {
            List<BatchActionItem<int>> items = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var BL = ObjectFactory<BalanceRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(items, sysNo =>
            {
                BL.SetReferenceID(sysNo, referenceID);
            });
            return result.PromptMessage;
        }

        public virtual void UpdatePointExpiringDate(int obtainSysNo, DateTime expiredDate)
        {
            ObjectFactory<BalanceRefundProcessor>.Instance.UpdatePointExpiringDate(obtainSysNo, expiredDate);
        }
    }
}