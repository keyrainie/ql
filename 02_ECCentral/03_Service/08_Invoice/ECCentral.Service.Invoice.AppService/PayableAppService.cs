using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;

namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// 应付款应用层服务
    /// </summary>
    [VersionExport(typeof(PayableAppService))]
    public class PayableAppService
    {
        /// <summary>
        /// 根据应付款编号取得应付款信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual PayableInfo LoadBySysNo(int sysNo)
        {
            return ObjectFactory<PayableProcessor>.Instance.LoadBySysNo(sysNo);
        }

        /// <summary>
        /// 更新应付款信息
        /// </summary>
        /// <param name="entity">待更新的应付款信息</param>
        public virtual void UpdateInvoice(PayableInfo entity)
        {
            if (!entity.SysNo.HasValue)
            {
                throw new ArgumentException("entity.SysNO");
            }
            ObjectFactory<PayableProcessor>.Instance.UpdateInvoice(entity);
        }

        /// <summary>
        /// 批量更新应付款发票
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual string BatchUpdateInvoice(List<PayableInfo> entities)
        {
            var request = entities.Select(s => new BatchActionItem<PayableInfo>()
            {
                ID = s.SysNo.ToString(),
                Data = s
            }).ToList();

            var result = BatchActionManager.DoBatchAction(request, entity => this.UpdateInvoice(entity));
            return result.PromptMessage;
        }

        /// <summary>
        /// 审核应付款信息
        /// </summary>
        /// <param name="entity">待审核的应付款信息</param>
        public virtual void Audit(PayableInfo entity)
        {
            if (!entity.SysNo.HasValue)
            {
                throw new ArgumentException("entity.SysNo");
            }
            if (!entity.AuditStatus.HasValue)
            {
                throw new ArgumentException("entity.AuditStatus");
            }
            if (!string.IsNullOrEmpty(entity.Tag))
            {
                throw new BizException(entity.Tag);
            }
            ObjectFactory<PayableProcessor>.Instance.Audit(entity.SysNo.Value, entity.AuditStatus.Value);
        }

        /// <summary>
        /// 批量审核应付款信息
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual string BatchAudit(List<PayableInfo> entities)
        {
            var request = entities.Select(s => new BatchActionItem<PayableInfo>()
                {
                    ID = s.SysNo.ToString(),
                    Data = s
                }).ToList();
            var result = BatchActionManager.DoBatchAction(request, entity => this.Audit(entity));
            return result.PromptMessage;
        }

        /// <summary>
        /// 拒绝审核应付款信息
        /// </summary>
        /// <param name="entity">待拒绝审核的应付款信息</param>
        public virtual void RefuseAudit(PayableInfo entity)
        {
            if (!entity.SysNo.HasValue)
            {
                throw new ArgumentException("entity.SysNo");
            }
            ObjectFactory<PayableProcessor>.Instance.RefuseAudit(entity.SysNo.Value, 255);
        }

        /// <summary>
        /// 批量拒绝审核应付款信息
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual string BatchRefuseAudit(List<PayableInfo> entities)
        {
            var request = entities.Select(s => new BatchActionItem<PayableInfo>()
            {
                ID = s.SysNo.ToString(),
                Data = s
            }).ToList();
            var result = BatchActionManager.DoBatchAction(request, entity => this.RefuseAudit(entity));
            return result.PromptMessage;
        }

        /// <summary>
        /// 判断指定SysNo供应商是否已锁定
        /// </summary>
        /// <param name="vendorSysNo">供应商SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public virtual bool IsHolderVendorByVendorSysNo(int vendorSysNo)
        {
            return ExternalDomainBroker.IsHolderVendorByVendorSysNo(vendorSysNo);
        }

        public virtual void CreateByPO(PayableInfo info)
        {
            ObjectFactory<PayableProcessor>.Instance.CreateByVendor(info);
        }

        /// <summary>
        /// 更新应付款状态和已付款金额
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateStatusAndAlreadyPayAmt(PayableInfo entity)
        {
            if (!entity.SysNo.HasValue)
            {
                throw new ArgumentException("entity.SysNo");
            }
            if (!entity.AlreadyPayAmt.HasValue)
            {
                throw new ArgumentException("entity.AlreadyPayAmt");
            }
            if (!entity.AuditStatus.HasValue)
            {
                throw new ArgumentException("entity.AuditStatus");
            }
            ObjectFactory<PayableProcessor>.Instance.PayByPayableSysNo(entity, ServiceContext.Current.UserSysNo);
        }

        /// <summary>
        /// 批量付款
        /// </summary>
        /// <param name="entities"></param>
        public virtual string BatchUpdateStatusAndAlreadyPayAmt(List<PayableInfo> entities)
        {
            var request = entities.Select(s => new BatchActionItem<PayableInfo>()
                {
                    ID = s.SysNo.ToString(),
                    Data = s
                }).ToList();
            var result = BatchActionManager.DoBatchAction(request, entity => this.UpdateStatusAndAlreadyPayAmt(entity));
            return result.PromptMessage;
        }
    }
}
