using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(SOIncomeRefundAppService))]
    public class SOIncomeRefundAppService
    {
        /// <summary>
        /// 更新销售退款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(SOIncomeRefundInfo entity)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.Update(entity);
        }

        /// <summary>
        /// CS审核退款单
        /// </summary>
        /// <param name="sysNo">财务-销售退款单系统编号</param>
        public virtual void CSAudit(SOIncomeRefundInfo entity)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.CSAudit(entity);
        }

        /// <summary>
        /// CS审核拒绝退款单
        /// </summary>
        /// <param name="sysNo">财务-销售退款单系统编号</param>
        public virtual void CSReject(int sysNo)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.CSReject(sysNo);
        }

        /// <summary>
        /// 财务审核退款单
        /// </summary>
        /// <param name="sysNo">财务-销售退款单系统编号</param>
        public virtual void FinAudit(SOIncomeRefundInfo entity)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.FinAudit(entity);
        }

        /// <summary>
        /// CS审核拒绝退款单
        /// </summary>
        /// <param name="sysNo">财务-销售退款单系统编号</param>
        /// <param name="appendFinNote">财务附加备注</param>
        public virtual void FinReject(int sysNo, string appendFinNote)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.FinReject(sysNo, appendFinNote);
        }

        /// <summary>
        /// 取消审核退款单
        /// </summary>
        /// <param name="sysNo">财务-销售退款单系统编号</param>
        public virtual void CancelAudit(int sysNo)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.CancelAudit(sysNo);
        }

        /// <summary>
        /// 审核RMA物流拒收
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void AuditAutoRMA(int sysNo)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.AuditAutoRMA(sysNo);
        }

        #region 批量操作

        /// <summary>
        /// 批量CS审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchCSAudit(List<SOIncomeRefundInfo> entitys)
        {
            var request = entitys.Select(x => new BatchActionItem<SOIncomeRefundInfo>()
                {
                    ID = x.SysNo.ToString(),
                    Data = x
                }).ToList();

            var refundBL = ObjectFactory<SOIncomeRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request,entity=>
            {
                refundBL.CSAudit(entity);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量CS审核拒绝
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchCSReject(List<int> sysNoList)
        {
            var request = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var refundBL = ObjectFactory<SOIncomeRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, sysNo =>
            {
                refundBL.CSReject(sysNo);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量财务审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchFinAudit(List<SOIncomeRefundInfo> entitys)
        {
            var request = entitys.Select(x => new BatchActionItem<SOIncomeRefundInfo>()
            {
                ID = x.SysNo.ToString(),
                Data = x
            }).ToList();

            var refundBL = ObjectFactory<SOIncomeRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, entity =>
            {
                refundBL.FinAudit(entity);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量财务审核拒绝
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual string BatchFinReject(List<int> sysNoList, string finAppendNote)
        {
            var request = sysNoList.Select(x => new BatchActionItem<int>()
                {
                    ID = x.ToString(),
                    Data = x
                }).ToList();

            var refundBL = ObjectFactory<SOIncomeRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, sysNo =>
            {
                refundBL.FinReject(sysNo, finAppendNote);
            });
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量取消审核
        /// </summary>
        /// <returns></returns>
        public virtual string BatchCancelAudit(List<int> sysNoList)
        {
            var request = sysNoList.Select(x => new BatchActionItem<int>()
            {
                ID = x.ToString(),
                Data = x
            }).ToList();

            var refundBL = ObjectFactory<SOIncomeRefundProcessor>.Instance;
            var result = BatchActionManager.DoBatchAction(request, sysNo =>
            {
                refundBL.CancelAudit(sysNo);
            });
            return result.PromptMessage;
        }

        #endregion 批量操作
    }
}