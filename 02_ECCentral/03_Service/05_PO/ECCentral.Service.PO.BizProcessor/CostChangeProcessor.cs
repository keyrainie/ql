using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.EventMessage.EIMS;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.Common;
using ECCentral.Service.EventMessage.PO;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 成本结算单 - BizProcessor
    /// </summary>
    [VersionExport(typeof(CostChangeProcessor))]
    public class CostChangeProcessor
    {

        #region [Fields]
        private ICostChangeDA m_CostChangeDA;
        private IInventoryBizInteract m_InventoryBizInteract;

        public IInventoryBizInteract InventoryBizInteract
        {
            get
            {
                if (m_InventoryBizInteract==null)
                {
                    m_InventoryBizInteract = ObjectFactory<IInventoryBizInteract>.Instance;
                }
                return m_InventoryBizInteract;
            }
        }

        public ICostChangeDA CostChangeDA
        {
            get
            {
                if (m_CostChangeDA == null)
                {
                    m_CostChangeDA = ObjectFactory<ICostChangeDA>.Instance;
                }
                return m_CostChangeDA;
            }
        }

        #endregion

        /// <summary>
        /// 加载成本变价单详细
        /// </summary>
        /// <param name="settlementSysNo"></param>
        /// <returns></returns>
        public virtual CostChangeInfo LoadCostChangeInfo(int ccSysNo)
        {
            CostChangeInfo returnEntity = new CostChangeInfo();
            CostChangeBasicInfo baseInfo = CostChangeDA.LoadCostChangeBasicInfo(ccSysNo);
            if (null != baseInfo && baseInfo.SysNo>0)
            {
                returnEntity.SysNo = ccSysNo;
                returnEntity.CostChangeBasicInfo = baseInfo;
                returnEntity.CostChangeItems = CostChangeDA.LoadCostChangeItemList(baseInfo.SysNo);
            }
            return returnEntity;
        }

        /// <summary>
        /// 更新成本变价单信息
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        public virtual CostChangeInfo UpdateCostChange(CostChangeInfo costChangeInfo)
        {
            #region [Check更新实体逻辑]

            //VerifyCreate(costChangeInfo);

            #endregion [Check更新实体逻辑]

            List<CostChangeItemsInfo> deleteList = new List<CostChangeItemsInfo>();
            decimal totalDiffAmt = 0;
            using (TransactionScope ts = new TransactionScope())
            {
                deleteList = costChangeInfo.CostChangeItems.Where(item => item.ItemActionStatus==ItemActionStatus.Delete).ToList();
                if (deleteList.Count>0)
                {
                    deleteList.ForEach(delegate(CostChangeItemsInfo deleteItemInfo)
                    {
                        // 删除Item:
                        CostChangeDA.DeleteCostChangeItems(deleteItemInfo);
                        //将删除的Item从List中去掉:
                        costChangeInfo.CostChangeItems.Remove(deleteItemInfo);
                    });
                }

                costChangeInfo.CostChangeItems.ForEach(delegate(CostChangeItemsInfo ItemInfo)
                {
                    totalDiffAmt += (ItemInfo.NewPrice - ItemInfo.OldPrice)*ItemInfo.ChangeCount;
                });

                costChangeInfo.CostChangeBasicInfo.EditUser = ServiceContext.Current.UserSysNo;
                costChangeInfo.CostChangeBasicInfo.TotalDiffAmt = totalDiffAmt;
                costChangeInfo = CostChangeDA.UpdateCostChange(costChangeInfo);
                //记录日志
                ExternalDomainBroker.CreateLog("Update CostChange"
                , BizEntity.Common.BizLogType.CostChange_Update
                , costChangeInfo.SysNo.Value, costChangeInfo.CompanyCode);
                ts.Complete();
            }

            return costChangeInfo;
        }

        /// <summary>
        /// 创建成本变价单
        /// </summary>
        /// <param name="costChangeInfo">成本变价单Entity</param>
        /// <returns></returns>
        public virtual CostChangeInfo CreateCostChange(CostChangeInfo costChangeInfo)
        {
            #region [Check 实体逻辑]
            //VerifyCreate(consignSettleInfo);
            //VerifySettleItems(consignSettleInfo, SettlementVerifyType.CREATE);
            #endregion

            decimal totalDiffAmt = 0;
            using (TransactionScope scope = new TransactionScope())
            {
                costChangeInfo.CostChangeItems.ForEach(delegate(CostChangeItemsInfo ItemInfo)
                {
                    totalDiffAmt += (ItemInfo.NewPrice - ItemInfo.OldPrice) * ItemInfo.ChangeCount;
                });

                costChangeInfo.CostChangeBasicInfo.InUser = ServiceContext.Current.UserSysNo;
                costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Created;
                costChangeInfo.CostChangeBasicInfo.TotalDiffAmt = totalDiffAmt;
                costChangeInfo = CostChangeDA.CreateCostChange(costChangeInfo);

                //写日志
                ExternalDomainBroker.CreateLog("Create CostChange"
                    , BizEntity.Common.BizLogType.CostChange_Create
                    , costChangeInfo.SysNo.Value, costChangeInfo.CompanyCode);

                scope.Complete();
            }

            return costChangeInfo;
        }

        /// <summary>
        /// 提交审核成本变价单信息
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        public virtual CostChangeInfo SubmitAuditCostChange(CostChangeInfo costChangeInfo)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.WaitingForAudited;
                costChangeInfo = CostChangeDA.UpdateCostChangeStatus(costChangeInfo);
                //记录日志
                ExternalDomainBroker.CreateLog("SubmitAudit CostChange"
                    , BizEntity.Common.BizLogType.CostChange_SubmitAudit
                    , costChangeInfo.SysNo.Value, costChangeInfo.CompanyCode);
                ts.Complete();
            }

            return costChangeInfo;
        }

        /// <summary>
        /// 撤销提交审核成本变价单信息
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        public virtual CostChangeInfo CancelSubmitAuditPOCostChange(CostChangeInfo costChangeInfo)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Created;
                costChangeInfo = CostChangeDA.UpdateCostChangeStatus(costChangeInfo);
                //记录日志
                ExternalDomainBroker.CreateLog("CancelSubmitAudit CostChange"
                    , BizEntity.Common.BizLogType.CostChange_CancelSubmitAudit
                    , costChangeInfo.SysNo.Value, costChangeInfo.CompanyCode);
                ts.Complete();
            }

            return costChangeInfo;
        }

        /// <summary>
        /// 作废成本变价单信息
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        public virtual CostChangeInfo AbandonCostChange(CostChangeInfo costChangeInfo)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Abandoned;
                costChangeInfo = CostChangeDA.UpdateCostChangeStatus(costChangeInfo);
                //记录日志
                ExternalDomainBroker.CreateLog("Abandon CostChange"
                    , BizEntity.Common.BizLogType.CostChange_Void
                    , costChangeInfo.SysNo.Value, costChangeInfo.CompanyCode);
                ts.Complete();
            }

            return costChangeInfo;
        }

        /// <summary>
        /// 审核拒绝并退回成本变价单信息
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        public virtual CostChangeInfo RefuseCostChange(CostChangeInfo costChangeInfo)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Created;
                costChangeInfo = CostChangeDA.UpdateCostChangeAuditStatus(costChangeInfo);
                //记录日志
                ExternalDomainBroker.CreateLog("Refuse CostChange"
                    , BizEntity.Common.BizLogType.CostChange_Refuse
                    , costChangeInfo.SysNo.Value, costChangeInfo.CompanyCode);
                ts.Complete();
            }

            return costChangeInfo;
        }

        /// <summary>
        /// 审核通过成本变价单信息
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        public virtual CostChangeInfo AuditCostChange(CostChangeInfo costChangeInfo)
        {
            TransactionOptions option = new TransactionOptions();
            //option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, option))
            {
                costChangeInfo.CostChangeBasicInfo.Status = CostChangeStatus.Audited;
                //修改单据状态
                costChangeInfo = CostChangeDA.UpdateCostChangeAuditStatus(costChangeInfo);
                //调用Inventory接口:更新库存成本
                ExternalDomainBroker.UpdateCostInWhenCostChange(costChangeInfo);
                //调用Invoice接口:生成结算单的付款单
                ExternalDomainBroker.CreatePayItem(new PayItemInfo()
                {
                    OrderSysNo = costChangeInfo.SysNo.Value,
                    PayAmt = costChangeInfo.CostChangeBasicInfo.TotalDiffAmt,
                    OrderType = PayableOrderType.CostChange,
                    PayStyle = PayItemStyle.Normal,
                    CompanyCode = costChangeInfo.CompanyCode
                });

                //记录日志
                ExternalDomainBroker.CreateLog("Audit CostChange"
                    , BizEntity.Common.BizLogType.CostChange_Audit
                    , costChangeInfo.SysNo.Value, costChangeInfo.CompanyCode);
                ts.Complete();
            }

            return costChangeInfo;
        }
    }
}
