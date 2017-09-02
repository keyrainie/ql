using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.BizProcessor;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(RefundAdjustAppService))]
    public class RefundAdjustAppService
    {
        /// <summary>
        /// 创建补偿退款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void RefundAdjustCreate(RefundAdjustInfo entity)
        {
            ObjectFactory<RefundAdjustProcessor>.Instance.RefundAdjustCreate(entity);
        }

       

        /// <summary>
        /// 批量修改补偿退款单的状态
        /// </summary>
        /// <param name="infos"></param>
        public virtual void BatchUpdateRefundAdjustStatus(List<RefundAdjustInfo> infos)
        {
           
        }

        /// <summary>
        /// 根据订单号获取创建补偿退款单的相关信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundAdjustInfo GetInfoBySOSysNo(RefundAdjustInfo entity)
        {
            return ObjectFactory<RefundAdjustProcessor>.Instance.GetInfoBySOSysNo(entity);
        }

        /// <summary>
        /// 获取节能补贴详细信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<EnergySubsidyInfo> GetEnergySubsidyDetails(EnergySubsidyInfo entity)
        {
            return ObjectFactory<RefundAdjustProcessor>.Instance.GetEnergySubsidyDetails(entity);
        }
    }
}
