using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.EventMessage.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 代收结算单 -Processor
    /// </summary>
    [VersionExport(typeof(GatherSettlementProcessor))]
    public class GatherSettlementProcessor
    {

        #region [Fields]
        private IGatherSettlementDA m_GatherSettlementDA;

        public IGatherSettlementDA GatherSettlementDA
        {
            get
            {
                if (null == m_GatherSettlementDA)
                {
                    m_GatherSettlementDA = ObjectFactory<IGatherSettlementDA>.Instance;
                }
                return m_GatherSettlementDA;
            }
        }
        #endregion
        /// <summary>
        /// 加载供应商代收结算信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo LoadGetherSettlement(GatherSettlementInfo info)
        {
            GatherSettlementInfo settleInfo = new GatherSettlementInfo();
            //1.加载代收结算单主信息:
            settleInfo = GatherSettlementDA.LoadGatherSettlement(info);
            //2.加载代收结算单，结算商品:
            settleInfo.GatherSettlementItemInfoList = new List<GatherSettlementItemInfo>();
            settleInfo.GatherSettlementItemInfoList = GatherSettlementDA.GetSettleGatherItemsInfoPage(settleInfo);
            if (0 < settleInfo.GatherSettlementItemInfoList.Count)
            {
                //计算总金额:
                settleInfo.GatherSettlementItemInfoList.ForEach(delegate(GatherSettlementItemInfo item)
                {
                    if (item.ItemType == "SHP")
                    {
                        item.ProductName = "运费";
                    }
                    item.TotalAmt = item.SalePrice * item.ProductQuantity;
                });
            }
            return settleInfo;
        }

        /// <summary>
        /// 创建供应商代收结算信息(人工创建)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo CreateGatherSettlement(GatherSettlementInfo info)
        {
            info.GatherSettlementItemInfoList = GetAllSettleGatherItems(info);
            //验证仓库号 ：
            if (null != info.GatherSettlementItemInfoList && 0 < info.GatherSettlementItemInfoList.Count)
            {
                foreach (var item in info.GatherSettlementItemInfoList)
                {
                    //if (item.SettleType != GatherSettleType.RO_Adjust
                    //&& item.StockSysNo != info.SourceStockInfo.SysNo.Value)
                    if (item.StockSysNo != info.SourceStockInfo.SysNo.Value)
                    {
                        //单据号为{0}的{1}单，仓库号和代收单仓库不一致！
                        throw new BizException(string.Format(GetMessageString("Gather_StockNotTheSame"), item.OrderSysNo, item.SettleType.ToString()));
                    }
                }
            }

            ///创建操作:
            info.SettleStatus = GatherSettleStatus.ORG;
            using (TransactionScope ts = new TransactionScope())
            {
                info.TotalAmt = info.GatherSettlementItemInfoList.Sum(i => (i.SalePrice * i.ProductQuantity)+(i.PromotionDiscount.HasValue?i.PromotionDiscount.Value:0));
                GatherSettlementDA.CreateGatherSettlement(info);

                //发送创建Message
                EventPublisher.Publish(new CreateGatherSettlementMessage()
                {
                    SettlementSysNo = info.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                //写日志:   CommonService.WriteLog<VendorSettleGatherEntity>(entity, " Create Gather ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

                ExternalDomainBroker.CreateLog(" Create Gather "
              , BizEntity.Common.BizLogType.Purchase_Verify_InStock
              , info.SysNo.Value
              , info.CompanyCode);
                ts.Complete();

            }
            return info;
        }

        /// <summary>
        /// 创建供应商代收结算信息(系统创建)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo SystemCreateGatherSettlement(GatherSettlementInfo info)
        {
            return CreateGatherSettlement(info);
        }

        /// <summary>
        /// 修改供应商代收结算信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo UpdateGatherSettlement(GatherSettlementInfo info)
        {
            List<GatherSettlementItemInfo> deletetlist = GatherSettlementDA.GetSettleGatherItems(info);

            //获取已经存在的代收结算单基本信息:
            GatherSettlementInfo oldSettle = GatherSettlementDA.GetVendorSettleGatherInfo(info);

            if (oldSettle.SettleStatus != GatherSettleStatus.ORG)
            {
                //该代收结算单不是待审核状态，不能进行删除操作！
                throw new BizException(GetMessageString("Gather_Orgin_Invalid"));
            }

            //获取已经存在的代收结算单Item信息:
            List<GatherSettlementItemInfo> oldlist = GatherSettlementDA.GetSettleGatherItems(info.SysNo.Value);

            if (deletetlist.Count >= oldlist.Count)
            {
                //不能全部删除代收结算单Item
                throw new BizException(GetMessageString("Gather_DeleteAllItems"));
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //删除Item操作:
                foreach (GatherSettlementItemInfo item in deletetlist)
                {
                    GatherSettlementDA.DeleteSettleItem(item, info.SysNo.Value);
                }
                //修改主表信息:
                info = GatherSettlementDA.UpdateGatherSettlement(info);

                //写LOG：
                //CommonService.WriteLog<VendorSettleGatherEntity>(entity, " Update Gather ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

                ExternalDomainBroker.CreateLog(" Update Gather "
                  , BizEntity.Common.BizLogType.Purchase_Verify_InStock
                  , info.SysNo.Value
                  , info.CompanyCode);

                scope.Complete();
                return info;
            }
        }

        /// <summary>
        /// 审核供应商代收结算信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo AuditGatherSettlement(GatherSettlementInfo info)
        {
            GatherSettlementInfo oldSettle = GatherSettlementDA.GetVendorSettleGatherInfo(info);
            if (oldSettle.SettleStatus != GatherSettleStatus.ORG)
            {
                //该结算单不是待审核状态，不能进行审核操作
                throw new BizException(GetMessageString("Gather_Orgin_Invalid_Audit"));
            }
            //if (oldSettle.InUser == info.AuditUser.UserDisplayName)
            //{
            //    //创建人和审核人不能相同
            //    throw new BizException(GetMessageString("Gather_WaitingAudit_CreateUser"));
            //}
            //更新代收结算单状态:
            info.SettleStatus = GatherSettleStatus.AUD;
            GatherSettlementDA.UpdateGatherSettlementStatus(info, true);

            //发送审核Message
            EventPublisher.Publish(new GatherSettlementAuditedMessage()
            {
                SettlementSysNo = info.SysNo.Value,
                CurrentUserSysNo = ServiceContext.Current.UserSysNo
            });

            //写Log:
            //CommonService.WriteLog<VendorSettleGatherEntity>(entity, " Audit Gather ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

            ExternalDomainBroker.CreateLog(" Audit Gather "
             , BizEntity.Common.BizLogType.Purchase_Verify_InStock
             , info.SysNo.Value
             , info.CompanyCode);
            return info;
        }

        /// <summary>
        /// 代收结算单 - 取消审核
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo CancelAuditGatherSettlement(GatherSettlementInfo info)
        {
            GatherSettlementInfo oldSettle = GatherSettlementDA.GetVendorSettleGatherInfo(info);
            if (oldSettle.SettleStatus != GatherSettleStatus.AUD)
            {
                //该结算单不是已审核状态，不能进行取消审核操作！
                throw new BizException(GetMessageString("Gather_Audited_Invalid_CancelAudit"));
            }
            info.SettleStatus = GatherSettleStatus.ORG;
            GatherSettlementDA.UpdateGatherSettlementStatus(info, false);

            //发送取消审核Message
            EventPublisher.Publish(new GatherSettlementAuditCanceledMessage()
            {
                SettlementSysNo = info.SysNo.Value,
                CurrentUserSysNo = ServiceContext.Current.UserSysNo
            });

            //写LOG:
            // CommonService.WriteLog<VendorSettleGatherEntity>(entity, " CancelAudited Gather ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

            ExternalDomainBroker.CreateLog(" CancelAudited Gather "
             , BizEntity.Common.BizLogType.Purchase_Verify_InStock
             , info.SysNo.Value
             , info.CompanyCode);
            return info;
        }

        /// <summary>
        /// 代收结算单 - 结算
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo SettleGatherSettlement(GatherSettlementInfo info)
        {
            info.SettleStatus = GatherSettleStatus.SET;
            GatherSettlementInfo oldSettle = GatherSettlementDA.GetVendorSettleGatherInfo(info);
            if (oldSettle.SettleStatus != GatherSettleStatus.AUD)
            {
                //该结算单不是已审核状态，不能进行结算操作！
                throw new BizException(GetMessageString("Gather_Audited_Invalid_Settle"));
            }
            GatherSettlementDA.UpdateGatherSettlementSettleStatus(info);
            //调用Invoice接口，创建PayItem:

            ExternalDomainBroker.CreatePayItem(
                new PayItemInfo()
                {
                    OrderSysNo = info.SysNo.Value,
                    PayAmt = info.TotalAmt,
                    OrderType = PayableOrderType.CollectionSettlement,
                    PayStyle = PayItemStyle.Normal
                });

            //发送结算Message
            EventPublisher.Publish(new GatherSettlementSettledMessage()
            {
                SettlementSysNo = info.SysNo.Value,
                CurrentUserSysNo = ServiceContext.Current.UserSysNo
            });

            //写Log:
            //CommonService.WriteLog<VendorSettleGatherEntity>(entity, " Settle Gather ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

            ExternalDomainBroker.CreateLog(" Settle Gather "
          , BizEntity.Common.BizLogType.Purchase_Verify_InStock
          , info.SysNo.Value
          , info.CompanyCode);
            return info;
        }

        /// <summary>
        /// 代收结算单 - 取消结算
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo CancelSettleGatherSettlement(GatherSettlementInfo info)
        {
            //1 检查当前结算单状态
            if (info.SettleStatus != GatherSettleStatus.SET)
            {
                //当前结算单的状态不为已结算状态!
                throw new BizException(GetMessageString("Gather_Settle_Invalid"));
            }
            ////2 检查付款单是否作废,如果付款单未作废抛出异常(调用Invoice接口)
            if (!ExternalDomainBroker.IsAbandonGatherPayItem(info.SysNo.Value))
            {
                throw new BizException(GetMessageString("Gather_Settle_Abandon_CancelSettle"));
            }

            GatherSettlementInfo oldSettle = GatherSettlementDA.GetVendorSettleGatherInfo(info);
            if (info.SettleStatus != GatherSettleStatus.SET)
            {
                //该结算单不是已结算状态，不能进行取消结算操作！
                throw new BizException(GetMessageString("Gather_Settle_Invalid_CancelSettle"));
            }

            info.SettleStatus = GatherSettleStatus.AUD;
            GatherSettlementDA.UpdateGatherSettlementStatus(info, false);

            //发送取消结算Message
            EventPublisher.Publish(new GatherSettlementSettleCanceledMessage()
            {
                SettlementSysNo = info.SysNo.Value,
                CurrentUserSysNo = ServiceContext.Current.UserSysNo
            });

            //写LOG：CommonService.WriteLog<VendorSettleGatherEntity>(entity, " CancelSettled Gather ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

            ExternalDomainBroker.CreateLog(" CancelSettled Gather "
                        , BizEntity.Common.BizLogType.Purchase_Verify_InStock
                        , info.SysNo.Value
                        , info.CompanyCode);

            return info;
        }

        /// <summary>
        /// 代收结算单 - 作废
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo AbandonGatherSettlement(GatherSettlementInfo info)
        {
            info.SettleStatus = GatherSettleStatus.ABD;
            GatherSettlementDA.UpdateGatherSettlementStatus(info, false);

            //发送作废Message
            EventPublisher.Publish(new GatherSettlementAbandonedMessage()
            {
                SettlementSysNo = info.SysNo.Value,
                CurrentUserSysNo = ServiceContext.Current.UserSysNo
            });

            //写LOG;CommonService.WriteLog<VendorSettleGatherEntity>(entity, " Abandon Gather ", entity.SysNo.Value.ToString(), (int)LogType.Purchase_Verify_InStock);

            ExternalDomainBroker.CreateLog(" Abandon Gather "
              , BizEntity.Common.BizLogType.Purchase_Verify_InStock
              , info.SysNo.Value
              , info.CompanyCode);
            return info;
        }

        /// <summary>
        ///  代收结算单 - 取消作废
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GatherSettlementInfo CancelAbandonGatherSettlement(GatherSettlementInfo info)
        {
            //写LOG：CommonService.WriteLog<VendorSettleGatherEntity>(entity, " CancelAbandon VendorSettle ", entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_CancelAbandon);

            ExternalDomainBroker.CreateLog(" CancelAbandon Gather "
             , BizEntity.Common.BizLogType.Purchase_Verify_InStock
             , info.SysNo.Value
             , info.CompanyCode);
            return info;
        }

        /// <summary>
        /// 获取所有供应商代收结算单Items
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual List<GatherSettlementItemInfo> GetAllSettleGatherItems(GatherSettlementInfo info)
        {
            List<GatherSettlementItemInfo> shippingChargeList = GatherSettlementDA.LoadConsignSettlementAllShippingCharge(info);
            List<GatherSettlementItemInfo> productList = GatherSettlementDA.QueryConsignSettlementProductList(info);
            List<GatherSettlementItemInfo> ro_adjustItems = GatherSettlementDA.QueryConsignSettleGatherROAdjust(info);
            return shippingChargeList.Union(productList).Union(ro_adjustItems).ToList();
        }

        /// <summary>
        /// 根据供应商系统编号取得代收结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public virtual List<int> GetGatherSettlementSysNoListByVendorSysNo(int vendorSysNo)
        {
            return GatherSettlementDA.GetGatherSettlementSysNoListByVendorSysNo(vendorSysNo);
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetMessageString(string key)
        {
            return ResouceManager.GetMessageString("PO.GatherSettlement", key);
        }
    }
}
