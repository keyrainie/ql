using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.PO.Commission;
namespace ECCentral.Service.PO.IDataAccess
{
    public interface ICollectionPaymentDA
    {

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        CollectionPaymentInfo Load(int sysNo);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CollectionPaymentInfo Create(CollectionPaymentInfo entity);

        /// <summary>
        /// CreateSettleItem
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CollectionPaymentItem CreateSettleItem(CollectionPaymentItem entity);

        /// <summary>
        /// DeleteSettleItem
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool DeleteSettleItem(CollectionPaymentItem entity);

        /// <summary>
        /// GetVendorSettleBySysNo
        /// </summary>
        /// <param name="settleSysNo"></param>
        /// <returns></returns>
        List<ConsignSettlementRulesInfo> GetSettleRuleQuantityCount(int settleSysNo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CollectionPaymentInfo GetVendorSettleBySysNo(CollectionPaymentInfo entity);

        /// <summary>
        /// IsAbandonPayItem
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        bool IsAbandonPayItem(int sysNo);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CollectionPaymentInfo Update(CollectionPaymentInfo entity);

        /// <summary>
        /// UpdateVendorSettleStatus
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        CollectionPaymentInfo UpdateVendorSettleStatus(CollectionPaymentInfo entity);

        int UpdatePOInstockAmtAndStatus(int poSysNo, int poStatus);

        POEntity GetPOMaster(int poSysNo);


    }
}
