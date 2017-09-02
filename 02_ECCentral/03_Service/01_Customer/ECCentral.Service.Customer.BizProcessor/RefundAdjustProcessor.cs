using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(RefundAdjustProcessor))]
    public class RefundAdjustProcessor
    {
        private IRefundAdjustDA da = ObjectFactory<IRefundAdjustDA>.Instance;

        /// <summary>
        /// 创建补偿退款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void RefundAdjustCreate(RefundAdjustInfo entity)
        {
            PreCheck(entity);
            RefundAdjustInfo info = da.GetRefundDetailBySoSysNo(entity);
            if (entity.AdjustOrderType == RefundAdjustType.EnergySubsidy)
            {
                info = da.GetEnergySubsidyBySOSysNo(entity);
            }
            if (info != null)
            {
                //补偿类型为【运费补偿】时，必须判断对应的RMA申请单的处理状态是否【处理完毕】
                if (entity.AdjustOrderType == RefundAdjustType.ShippingAdjust)
                {
                    if (string.IsNullOrEmpty(info.RequestID))
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.RefundAdjust", "CreateRefundAdjust_RefundStatusError"));
                    }
                }
                entity.RequestSysNo = info.RequestSysNo;
                entity.CustomerSysNo = info.CustomerSysNo;
            }
            da.RefundAdjustCreate(entity);
        }

        private void PreCheck(RefundAdjustInfo entity)
        {
            if (!entity.SOSysNo.HasValue)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.RefundAdjust", "CreateRefundAdjust_SoSysNoHasNoValue"));
            }
            if (entity.AdjustOrderType == RefundAdjustType.ShippingAdjust && string.IsNullOrEmpty(entity.RequestID))
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.RefundAdjust", "CreateRefundAdjust_RequestIDIsNull"));
            }
            if (entity.CashAmt == null || entity.CashAmt == 0m)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.RefundAdjust", "CreateRefundAdjust_CashAmtError"));
            }
            if (string.IsNullOrEmpty(entity.Note))
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.RefundAdjust", "CreateRefundAdjust_AdjustNoteIsNUll"));
            }
            if (entity.AdjustOrderType == RefundAdjustType.EnergySubsidy)
            {
                bool isEffective = da.GetEffectiveEnergySubsidySO(entity.SOSysNo.Value);
                bool isShippingOut = da.IsShippingOut(entity.SOSysNo.Value);
                bool isHaveAutoRMA = da.IsHaveAutoRMA(entity.SOSysNo.Value);
                if (!isEffective)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.RefundAdjust", "CreateRefundAdjust_EnergySubsidyEffectiveError"));
                }
                else if (!isShippingOut)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.RefundAdjust", "CreateRefundAdjust_OrderShippingError"));
                }
                else if (!isHaveAutoRMA)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.RefundAdjust", "CreateRefundAdjust_HaveAutoRMAError"));
                }
            }
        }

      

       

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public virtual RefundAdjustStatus? GetRefundAdjustStatus(int SysNo)
        {
            int? status = null;
            status = da.GetRefundAdjustStatus(SysNo);
            return (RefundAdjustStatus)status;
        }

        /// <summary>
        /// 根据订单号获取创建补偿退款单的相关信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundAdjustInfo GetInfoBySOSysNo(RefundAdjustInfo entity)
        {
            if (entity.AdjustOrderType == RefundAdjustType.ShippingAdjust)
            {
                //补偿类型为【运费补偿时】从RMA_Request表中查询数据
                return da.GetRefundDetailBySoSysNo(entity);
            }
            else if (entity.AdjustOrderType == RefundAdjustType.Other)
            {
                //补偿类型为【其他】时从OverseaOrderManagement.dbo.V_OM_SO_Master查询数据
                return da.GetCustomerIDBySOSysNo(entity);
            }
            else if (entity.AdjustOrderType == RefundAdjustType.EnergySubsidy)
            {
                //补偿类型为【节能补贴】时，需要计算补贴金额
                //优先在[OverseaOrderManagement].[dbo].[SO_EnergySubsidy]查询
                //如果在节能惠民表中查找不到数据，则再到ProductEnergySubsidy表中查
                RefundAdjustInfo info = null;
                info = da.GetEnergySubsidyBySOSysNo(entity);
                if (info == null)
                    info = da.GetInProductEnergySubsidyBySOSysNo(entity);
                return info;
            }
            return null;
        }

        /// <summary>
        /// 获取节能补贴详细信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual List<EnergySubsidyInfo> GetEnergySubsidyDetails(EnergySubsidyInfo entity)
        {
            List<EnergySubsidyInfo> results = null;
            results = da.GetEnergySubsidyDetialsBySOSysNo(entity);
            if (results == null || results.Count == 0)
                results = da.GetInProductEnergySubsidyDetials(entity);
            return results;
        }
    }
}
