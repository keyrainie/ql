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
using ECCentral.BizEntity.PO.Settlement;
using System.Data;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 代销结算单 - BizProcessor
    /// </summary>
    [VersionExport(typeof(ConsignSettlementProcessor))]
    public class ConsignSettlementProcessor
    {

        #region [Fields]
        private IConsignSettlementDA m_ConsignSettlementDA;
        private IVendorDA m_VendorDA;
        private IInvoiceBizInteract m_InvoiceBizInteract;

        public IInvoiceBizInteract InvoiceBizInteract
        {
            get
            {
                if (null == m_InvoiceBizInteract)
                {
                    m_InvoiceBizInteract = ObjectFactory<IInvoiceBizInteract>.Instance;
                }
                return m_InvoiceBizInteract;
            }
        }

        public IConsignSettlementDA ConsignSettlementDA
        {
            get
            {
                if (null == m_ConsignSettlementDA)
                {
                    m_ConsignSettlementDA = ObjectFactory<IConsignSettlementDA>.Instance;
                }
                return m_ConsignSettlementDA;
            }
        }

        public IVendorDA VendorDA
        {
            get
            {
                if (null == m_VendorDA)
                {
                    m_VendorDA = ObjectFactory<IVendorDA>.Instance;
                }
                return m_VendorDA;
            }
        }
        #endregion

        /// <summary>
        /// 加载代销结算单详细
        /// </summary>
        /// <param name="settlementSysNo"></param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo LoadConsignSettlementInfo(int settlementSysNo)
        {
            //加载代销结算单主信息:
            ConsignSettlementInfo returnEntity = new ConsignSettlementInfo();
            returnEntity = ConsignSettlementDA.LoadConsignSettlementInfo(settlementSysNo);
            if (null != returnEntity && returnEntity.SysNo.HasValue && 0 < returnEntity.SysNo.Value)
            {
                //if (returnEntity.PM_ReturnPointSysNo.HasValue)
                //{
                //    //如果存在返点信息，则加载
                //    ConsignSettlementEIMSInfo eims = GetConsignEIMSInfoBySysNo(returnEntity.PM_ReturnPointSysNo.Value);
                //    if (eims != null && returnEntity.EIMSInfo != null)
                //    {
                //        returnEntity.EIMSInfo.ReturnPointName = eims.ReturnPointName;
                //        returnEntity.EIMSInfo.RemnantReturnPoint = eims.RemnantReturnPoint;
                //    }
                //}
                //加载代销结算单商品信息:
                returnEntity.ConsignSettlementItemInfoList = ConsignSettlementDA.LoadConsignSettlementItemList(returnEntity.SysNo.Value);
                //********
            }
            return returnEntity;
        }

        /// <summary>
        /// 更新代销结算单信息
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo UpdateConsignSettlementInfo(ConsignSettlementInfo consignSettleInfo)
        {
            CheckPMSysNo(consignSettleInfo);

            #region [Check 返点信息]

            CheckReturnPointInfo(consignSettleInfo);

            #endregion [Check 返点信息]

            #region [Check更新实体逻辑 - 主信息]

            VerifyCreate(consignSettleInfo);

            #endregion [Check更新实体逻辑 - 主信息]

            #region [Check更新实体逻辑 - 结算商品]

            VerifySettleItems(consignSettleInfo, SettlementVerifyType.UPDATE);
            //如果产品线验证通过，则更新单据所属产品线
            int tProductLineSysNo = 0;
            VerifyProductPMSysNo(consignSettleInfo, out tProductLineSysNo);
            if (tProductLineSysNo != 0)
                consignSettleInfo.ProductLineSysNo = tProductLineSysNo;

            #endregion [Check更新实体逻辑 - 结算商品]

            List<ConsignSettlementItemInfo> deleteList = new List<ConsignSettlementItemInfo>();
            using (TransactionScope ts = new TransactionScope())
            {
                //STEP1:删除Item;
                deleteList = consignSettleInfo.ConsignSettlementItemInfoList.Where(item => item.SettleSysNo.HasValue && item.SettleSysNo == -1).ToList();
                if (0 < deleteList.Count)
                {
                    deleteList.ForEach(delegate(ConsignSettlementItemInfo deleteItemInfo)
                    {
                        //将需要删除的Item对应的Acclog状态改为[初始状态:0]
                        ConsignSettlementDA.UpdateConsignToAccountLogStatus(deleteItemInfo.ConsignToAccLogInfo.LogSysNo.Value, ConsignToAccountLogStatus.Origin);
                        // 删除Item:
                        ConsignSettlementDA.DeleteConsignSettlementItemInfo(deleteItemInfo);
                        //将删除的Item从List中去掉:
                        consignSettleInfo.ConsignSettlementItemInfoList.Remove(deleteItemInfo);
                    });
                }
                consignSettleInfo.ConsignSettlementItemInfoList.ForEach(delegate(ConsignSettlementItemInfo itemInfo)
                {
                    //如果item的sysno没有值,则表示是新增加的item,需要更新对应的ConsignToAcclog
                    if (!itemInfo.ItemSysNo.HasValue)
                    {
                        itemInfo.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.ManualCreated;
                        ConsignSettlementDA.UpdateConsignToAccountLogStatus(itemInfo.ConsignToAccLogInfo.LogSysNo.Value, ConsignToAccountLogStatus.ManualCreated);
                    }
                });

                //STEP2:更新结算单:
                consignSettleInfo = ConsignSettlementDA.UpdateConsignSettlementInfo(consignSettleInfo);
                ts.Complete();
            }
            //3 记录删除的items
            foreach (ConsignSettlementItemInfo item in deleteList)
            {
                //如果ItemSysNo为空，则表示记录为新增并删除的，无需记录日志！
                if (item.ItemSysNo != null)
                    ExternalDomainBroker.CreateLog(" Deleted SettleItem "
                 , BizEntity.Common.BizLogType.POC_VendorSettle_Item_Delete
                 , item.ItemSysNo.Value, consignSettleInfo.CompanyCode);
            }

            //4 记录修改的items
            foreach (ConsignSettlementItemInfo item in consignSettleInfo.ConsignSettlementItemInfoList)
            {
                //写LOG： CommonService.WriteLog<SettleItem>(item, " Updated SettleItem ", item.SysNo.Value.ToString(), (int)LogType.VendorSettle_Item_Update);
                if (item.ItemSysNo.HasValue)
                {
                    ExternalDomainBroker.CreateLog(" Updated SettleItem "
      , BizEntity.Common.BizLogType.POC_VendorSettle_Item_Update
      , item.ItemSysNo.Value, consignSettleInfo.CompanyCode);
                }
            }

            return consignSettleInfo;
        }

        /// <summary>
        /// 创建代销结算单
        /// </summary>
        /// <param name="consignSettleInfo">代销结算单Entity</param>
        /// <param name="isSystemCreate">是否是系统创建</param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo CreateConsignSettlementInfo(ConsignSettlementInfo consignSettleInfo, bool isSystemCreate)
        {
            //去除已经删除的Item（SysNo=-1） 次方法已经在Portal加过了，此处只为以防万一了
            consignSettleInfo.ConsignSettlementItemInfoList = (from tItem in consignSettleInfo.ConsignSettlementItemInfoList
                                                               where tItem.SettleSysNo != -1
                                                               select tItem).ToList();
            if (!isSystemCreate)
            {
                CheckReturnPointInfo(consignSettleInfo);
                CheckPMSysNo(consignSettleInfo);
            }
            consignSettleInfo.Status = SettleStatus.WaitingAudit;


            #region [Check 实体逻辑]

            VerifyCreate(consignSettleInfo);
            VerifySettleItems(consignSettleInfo, SettlementVerifyType.CREATE);
            //如果产品线验证通过，则更新单据所属产品线
            int tProductLineSysNo = 0;
            VerifyProductPMSysNo(consignSettleInfo, out tProductLineSysNo);
            if (tProductLineSysNo != 0)
                consignSettleInfo.ProductLineSysNo = tProductLineSysNo;

            #endregion [Check 实体逻辑]

            //修改代销转财务记录的状态
            foreach (var item in consignSettleInfo.ConsignSettlementItemInfoList)
            {
                item.ConsignToAccLogInfo.ConsignToAccStatus = (isSystemCreate ? ConsignToAccountLogStatus.SystemCreated : ConsignToAccountLogStatus.ManualCreated);
            }

            //用于收集操作过的结算单
            List<ConsignSettlementInfo> settleEntityList = new List<ConsignSettlementInfo>();
            //计算返点,返点总额将会平均分配到以下结算单，当返点总额大于单个结算单的返点额，那单个结算单返点将和计算单总额相等
            //剩余返点额将在下个结算单中使用

            //decimal totalUsingReturnPoint = 0;//外包项目无EIMS系统
            decimal totalUsingReturnPoint = consignSettleInfo.EIMSInfo.UsingReturnPoint ?? 0;


            using (TransactionScope scope = new TransactionScope())
            {
                //泰隆优选不需要分仓
                //按照仓库的不同自动分结算单
                //var stockSysNoList = consignSettleInfo.ConsignSettlementItemInfoList.Select(p => p.StockSysNo).Distinct();

                //foreach (int? stockSysNo in stockSysNoList)
                //{
                //    var newEntity = SerializationUtility.DeepClone<ConsignSettlementInfo>(consignSettleInfo);

                //    newEntity.SourceStockInfo.SysNo = stockSysNo;

                //    var tempSettleList = new List<ConsignSettlementItemInfo>();
                //    tempSettleList.AddRange(newEntity.ConsignSettlementItemInfoList.Where(p => p.StockSysNo == stockSysNo));
                //    //计算总结算数
                //    newEntity.TotalAmt = tempSettleList.Sum(p => p.Cost * p.ConsignToAccLogInfo.ProductQuantity);                    
                //    //计算返点
                //    if (totalUsingReturnPoint > 0 && newEntity.TotalAmt > 0)
                //    {
                //        if (newEntity.TotalAmt >= totalUsingReturnPoint)
                //        {
                //            newEntity.EIMSInfo.UsingReturnPoint = totalUsingReturnPoint;
                //            totalUsingReturnPoint = 0;
                //        }
                //        else
                //        {
                //            newEntity.EIMSInfo.UsingReturnPoint = newEntity.TotalAmt;
                //            totalUsingReturnPoint -= newEntity.TotalAmt.Value;
                //        }
                //    }
                //    else
                //    {
                //        newEntity.EIMSInfo.UsingReturnPoint = null;
                //    }                   
                //    newEntity.ConsignSettlementItemInfoList = tempSettleList;

                //    //创建代销结算单 ：
                //    newEntity = ConsignSettlementDA.CreateConsignSettlementInfo(newEntity);
                //    //将操作完成的收集在临时列表中
                //    settleEntityList.Add(newEntity);

                //    consignSettleInfo.SysNo = newEntity.SysNo;
                //}
                settleEntityList.Add(consignSettleInfo);
                consignSettleInfo = ConsignSettlementDA.CreateConsignSettlementInfo(consignSettleInfo);

                #region [Check相关的代销转财务记录,更新代销转财务记录状态]
                //验证
                settleEntityList.ForEach(master =>
                {
                    master.ConsignSettlementItemInfoList.ForEach(item =>
                    {
                        if (item.ConsignToAccLogInfo == null || !item.ConsignToAccLogInfo.LogSysNo.HasValue || !item.ConsignToAccLogInfo.ConsignToAccStatus.HasValue)
                        {
                            //没有相关的代销转财务记录， 或者代销转财务记录状态不能为空
                            throw new BizException(GetMessageString("Consign_AcctLogIsEmpty"));
                        }
                        //回写状态
                        ConsignSettlementDA.UpdateConsignToAccountLogStatus(item.ConsignToAccLogInfo.LogSysNo.Value, item.ConsignToAccLogInfo.ConsignToAccStatus.Value);
                    });
                });
                #endregion [Check相关的代销转财务记录,更新代销转财务记录状态]

                //发送ESB消息
                EventPublisher.Publish<ConsignSettlementCreateMessage>(new ConsignSettlementCreateMessage()
                {
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = consignSettleInfo.SysNo.Value
                });

                //写LOG： CommonService.WriteLog<VendorSettleEntity>(entity, " Create VendorSettle ", entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_Create);
                foreach (var newEntity in settleEntityList)
                {
                    //写日志
                    ExternalDomainBroker.CreateLog(" Create VendorSettle "
         , BizEntity.Common.BizLogType.POC_VendorSettle_Create
         , newEntity.SysNo.Value, newEntity.CompanyCode);
                }

                scope.Complete();
            }

            foreach (var newEntity in settleEntityList)
            {
                //根据Vendor的IsAutoAudit，决定是否进行代销结算单自动审核:
                VendorInfo vendor = VendorDA.LoadVendorInfo(newEntity.VendorInfo.SysNo.Value);
                if (vendor.VendorFinanceInfo.IsAutoAudit != null && vendor.VendorFinanceInfo.IsAutoAudit == true)
                {
                    AuditConsignSettlement(newEntity);
                    //自动结算
                    consignSettleInfo = SettleConsignSettlement(newEntity);
                }

            }

            return consignSettleInfo;
        }

        /// <summary>
        /// 创建代销转财务记录
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public virtual ConsignToAcctLogInfo CreateConsignToAcccountLog(ConsignToAcctLogInfo logInfo)
        {
            //1.校验

            #region [校验逻辑]

            //1 商品编号是否为空
            if (!logInfo.ProductSysNo.HasValue || logInfo.ProductSysNo == 0)
            {
                throw new BizException(GetMessageString("Consign_ProductSysNoIsEmpty"));
            }
            //2 供应商是否为空
            if (!logInfo.VendorSysNo.HasValue || logInfo.VendorSysNo == 0)
            {
                //供应商不能为空
                throw new BizException(GetMessageString("Consign_VendorIsEmpty"));
            }
            //3 仓库编号是否为空
            if (!logInfo.StockSysNo.HasValue || logInfo.StockSysNo == 0)
            {
                //仓库编号不能为空
                throw new BizException(GetMessageString("Consign_StockSysNoIsEmpty"));
            }
            //Quantity,
            if (!logInfo.ProductQuantity.HasValue)
            {
                //商品不能为空
                throw new BizException(GetMessageString("Consign_ProductIsEmpty"));
            }
            //CreateCost,
            if (!logInfo.CreateCost.HasValue)
            {
                //商品创建成本不能为空
                throw new BizException(GetMessageString("Consign_ProductCreateCostIsEmpty"));
            }
            //RetailPrice,
            if (!logInfo.SalePrice.HasValue)
            {
                //商品销售价格不能为空
                throw new BizException(GetMessageString("Consign_ProductSalePriceIsEmpty"));
            }
            //Point,
            if (!logInfo.Point.HasValue)
            {
                //商品积分不能为空
                throw new BizException(GetMessageString("Consign_ProductPointIsEmpty"));
            }
            //OrderSysNo,
            if (!logInfo.OrderSysNo.HasValue || logInfo.OrderSysNo == 0)
            {
                //订单编号不能为空
                throw new BizException(GetMessageString("Consign_OrderSysNoIsEmpty"));
            }
            //ConsignToAccType
            if (!logInfo.ConsignToAccType.HasValue || logInfo.ConsignToAccType == null)
            {
                //代销转财务类型不能为空
                throw new BizException(GetMessageString("Consign_ConsignToAccLogTypeIsEmpty"));
            }

            #endregion [校验逻辑]

            //2.获取SettleType，SettlePercentage
            VendorAgentInfo vendorAgentEntity = VendorDA.LoadVendorAgentInfoByVendorAndProductID(logInfo.VendorSysNo.Value, logInfo.ProductSysNo.Value);
            if (vendorAgentEntity == null)
            {
                //供应商代理信息不能为空
                throw new BizException(GetMessageString("Consign_VendorAgentInfoIsEmpty"));
            }
            logInfo.SettleType = vendorAgentEntity.SettleType;
            logInfo.SettlePercentage = vendorAgentEntity.SettlePercentage;

            //Status默认值
            if (!logInfo.ConsignToAccStatus.HasValue || logInfo.ConsignToAccStatus == null)
            {
                logInfo.ConsignToAccStatus = ConsignToAccountLogStatus.Origin;
            }

            //3.Create
            logInfo = ConsignSettlementDA.CreateConsignToAccountLog(logInfo);

            return logInfo;
        }

        /// <summary>
        /// 创建代销转财务记录(Inventory)
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public virtual ConsignToAcctLogInfo CreatePOConsignToAccLogForInventory(ConsignToAcctLogInfo logInfo)
        {
            return ConsignSettlementDA.CreatePOConsignToAccLogForInventory(logInfo);
        }


        /// <summary>
        /// 审核代销结算单
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo AuditConsignSettlement(ConsignSettlementInfo consignSettleInfo)
        {
            //加载当前要审核的代销结算单信息:
            consignSettleInfo = LoadConsignSettlementInfo(consignSettleInfo.SysNo.Value);

            CheckPMSysNo(consignSettleInfo);

            #region [Check实体逻辑]

            //1 检查当前结算单状态:
            if (consignSettleInfo.Status != SettleStatus.WaitingAudit)
            {
                //当前结算单的状态不为待审核状态
                throw new BizException(GetMessageString("Consign_WaitingAudit_StatusInvalid"));
            }
            //对系统账户的过滤
            //if (!IsSystemAccount(consignSettleInfo.CreateUserSysNo.Value))
            //    if (consignSettleInfo.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //    {
            //        //创建人和审核人不能相同
            //        throw new BizException(GetMessageString("Consign_WaitingAudit_CreateUser"));
            //    }
            //2.检查PM:
            //如果产品线验证通过，则更新单据所属产品线
            //int tProductLineSysNo = 0;
            //VerifyProductPMSysNo(consignSettleInfo, out tProductLineSysNo);
            //if (tProductLineSysNo != 0)
            //    consignSettleInfo.ProductLineSysNo = tProductLineSysNo;

            #endregion [Check实体逻辑]

            consignSettleInfo.Status = SettleStatus.AuditPassed;
            consignSettleInfo.AuditUser = new BizEntity.Common.UserInfo() { UserID = ServiceContext.Current.UserSysNo.ToString() };
            consignSettleInfo.AuditDate = DateTime.Now;

            //Use EIMS To Minus ReturnPoint
            if (consignSettleInfo.EIMSInfo != null && consignSettleInfo.EIMSInfo.UsingReturnPoint.HasValue && consignSettleInfo.PM_ReturnPointSysNo.HasValue)
            {
                //TODO：调用EIMS接口，扣减返点信息：　
                MinusReturnPointUseEIMS(consignSettleInfo.VendorInfo.SysNo, consignSettleInfo.PM_ReturnPointSysNo, consignSettleInfo.EIMSInfo.UsingReturnPoint, consignSettleInfo.AuditUser.UserID.ToInteger(), consignSettleInfo.AuditUser.UserName ?? string.Empty, consignSettleInfo.SysNo.ToString());
                //写LOG： CommonService.WriteLog<VendorSettleEntity>(entity, " Audit VendorSettle  " + entity.PM_ReturnPointSysNo.ToString() + "扣除返点" + entity.UsingReturnPoint.ToString(), entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_Audit);
                string logMsg = " Audit VendorSettle  " + consignSettleInfo.PM_ReturnPointSysNo.ToString() + GetMessageString("Consign_SubstractEIMS") + consignSettleInfo.EIMSInfo.UsingReturnPoint.ToString();

                ExternalDomainBroker.CreateLog(logMsg
             , BizEntity.Common.BizLogType.POC_VendorSettle_Audit
             , consignSettleInfo.SysNo.Value
             , consignSettleInfo.CompanyCode);
            }

            consignSettleInfo = ConsignSettlementDA.UpdateAuditStatus(consignSettleInfo);

            //发送ESB消息
            EventPublisher.Publish<ConsignSettlementAuditMessage>(new ConsignSettlementAuditMessage()
            {
                CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                SysNo = consignSettleInfo.SysNo.Value
            });

            //写LOG： CommonService.WriteLog<VendorSettleEntity>(entity, " Audit VendorSettle ", entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_Audit);

            ExternalDomainBroker.CreateLog(" Audit VendorSettle "
        , BizEntity.Common.BizLogType.POC_VendorSettle_Audit
        , consignSettleInfo.SysNo.Value
        , consignSettleInfo.CompanyCode);
            return consignSettleInfo;
        }

        //对系统账户的过滤
        private bool IsSystemAccount(int userSysNo)
        {
            string SystemAdminKey = "SystemAdmin";
            string tmpStr = ExternalDomainBroker.GetSystemConfiguration(SystemAdminKey, "8601");
            if (string.IsNullOrEmpty(tmpStr))
                return false;
            string pattern = "(?<SysNo>\\d+),";
            if (!Regex.IsMatch(tmpStr, pattern, RegexOptions.IgnoreCase))
            {
                return false;
            }
            MatchCollection matchCollection = Regex.Matches(tmpStr, pattern, RegexOptions.IgnoreCase);
            if (matchCollection == null
                || matchCollection.Count == 0)
            {
                return false;
            }
            int[] result = new int[matchCollection.Count];
            for (int i = 0; i < matchCollection.Count; i++)
            {
                Match match = matchCollection[i];
                result[i] = int.Parse(match.Groups["SysNo"].Value);
            }
            return result.Contains(userSysNo);
        }

        /// <summary>
        /// 取消审核代销结算单
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo CancalAuditConsignSettlement(ConsignSettlementInfo consignSettleInfo)
        {
            #region [Check实体逻辑]

            //1 检查当前结算单状态
            if (consignSettleInfo.Status != SettleStatus.AuditPassed)
            {
                //当前结算单的状态不为已审核状态
                throw new BizException(GetMessageString("Consign_AuditPassed_StatusInvalid"));
            }

            #endregion [Check实体逻辑]

            consignSettleInfo.Status = SettleStatus.WaitingAudit;
            consignSettleInfo.AuditUser = new BizEntity.Common.UserInfo() { UserID = ServiceContext.Current.UserSysNo.ToString() };
            consignSettleInfo.AuditDate = DateTime.Now;
            //当进行取消审核操作时，收回返点信息:When CancelAudited ,comeback returnPoint
            if (consignSettleInfo.EIMSInfo.UsingReturnPoint.HasValue && consignSettleInfo.PM_ReturnPointSysNo.HasValue)
            {
                //TODO:调用EIMS接口: //
                ComeBackReturnPoint(consignSettleInfo.VendorInfo.SysNo, consignSettleInfo.PM_ReturnPointSysNo, consignSettleInfo.EIMSInfo.UsingReturnPoint, consignSettleInfo.AuditUser.UserID.ToInteger(), consignSettleInfo.AuditUser.UserName ?? string.Empty, consignSettleInfo.SysNo.ToString());
                //写LOG ： CommonService.WriteLog<VendorSettleEntity>(entity, " CancelAudit VendorSettle  " + entity.PM_ReturnPointSysNo.ToString() + "返回返点" + entity.UsingReturnPoint.ToString(), entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_Audit);
                string logMsg = " CancelAudit VendorSettle  " + consignSettleInfo.PM_ReturnPointSysNo.ToString() + GetMessageString("Consign_RecycleEIMS") + consignSettleInfo.EIMSInfo.UsingReturnPoint.ToString();

                ExternalDomainBroker.CreateLog(logMsg
             , BizEntity.Common.BizLogType.POC_VendorSettle_Audit
             , consignSettleInfo.SysNo.Value
             , consignSettleInfo.CompanyCode);
            }
            consignSettleInfo = ConsignSettlementDA.UpdateAuditStatus(consignSettleInfo);

            //发送ESB消息
            EventPublisher.Publish<ConsignSettlementCancelMessage>(new ConsignSettlementCancelMessage()
            {
                CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                SysNo = consignSettleInfo.SysNo.Value
            });

            return consignSettleInfo;
        }

        /// <summary>
        /// 作废代销结算单
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo AbandonConsignSettlement(ConsignSettlementInfo consignSettleInfo)
        {
            #region [Check 实体逻辑]

            //1 检查当前结算单状态
            if (consignSettleInfo.Status != SettleStatus.WaitingAudit)
            {
                //当前结算单的状态不为待审核状态
                throw new BizException(GetMessageString("Consign_WaitingAudit_StatusInvalid"));
            }

            #endregion [Check 实体逻辑]

            consignSettleInfo.Status = SettleStatus.Abandon;

            //将代销转财务记录的状态修改为初始状态:
            foreach (var item in consignSettleInfo.ConsignSettlementItemInfoList)
            {
                item.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.Origin;
            }
            //当前结算单已被{0}在{1}作废
            consignSettleInfo.Note = string.Format(GetMessageString("Consign_Abandon_StatusInvalid"), "SysAdmin", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

            using (TransactionScope scope = new TransactionScope())
            {
                consignSettleInfo = ConsignSettlementDA.UpdateSettleStatus(consignSettleInfo);

                #region [Check相关的代销转财务记录,更新代销转财务记录状态]

                //:
                foreach (var item in consignSettleInfo.ConsignSettlementItemInfoList)
                {
                    if (item.ConsignToAccLogInfo == null || !item.ConsignToAccLogInfo.LogSysNo.HasValue || !item.ConsignToAccLogInfo.ConsignToAccStatus.HasValue)
                    {
                        //没有相关的代销转财务记录， 或者代销转财务记录状态不能为空
                        throw new BizException(GetMessageString("Consign_AcctLogIsEmpty"));
                    }
                }
                foreach (var item in consignSettleInfo.ConsignSettlementItemInfoList)
                {
                    ConsignSettlementDA.UpdateConsignToAccountLogStatus(item.ConsignToAccLogInfo.LogSysNo.Value, item.ConsignToAccLogInfo.ConsignToAccStatus.Value);
                }

                #endregion [Check相关的代销转财务记录,更新代销转财务记录状态]

                //写LOG：
                //CommonService.WriteLog<VendorSettleEntity>(entity, " Abandon VendorSettle ", entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_Abandon);

                ExternalDomainBroker.CreateLog(" Abandon VendorSettle "
                                                                             , BizEntity.Common.BizLogType.POC_VendorSettle_Abandon
                                                                             , consignSettleInfo.SysNo.Value
                                                                             , consignSettleInfo.CompanyCode);
                scope.Complete();
            }

            return consignSettleInfo;
        }

        /// <summary>
        /// 取消作废代销结算单
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo CancelAbandonConsignSettlement(ConsignSettlementInfo consignSettleInfo)
        {
            #region [Check实体逻辑]

            //1 检查当前结算单状态
            if (consignSettleInfo.Status != SettleStatus.Abandon)
            {
                //当前结算单的状态不为已作废状态
                throw new BizException(GetMessageString("Consign_Abandoned_StatusInvalid"));
            }
            //2 检查当前作废的item是否已存在于其他结算单据中

            foreach (ConsignSettlementItemInfo item in consignSettleInfo.ConsignSettlementItemInfoList)
            {
                bool tmpIsExist = ConsignSettlementDA.IsAccountLogExistOtherVendorSettle(item.ConsignToAccLogInfo.LogSysNo.Value);
                if (tmpIsExist)
                {
                    //当前商品(财务记录编号:{0})已存在于其他供应商结算单中！
                    throw new BizException(string.Format(GetMessageString("Consign_ProductExist"), (item.ConsignToAccLogInfo.LogSysNo.HasValue ? item.ConsignToAccLogInfo.LogSysNo.Value.ToString() : "")));
                }
            }

            #endregion [Check实体逻辑]

            consignSettleInfo.Status = SettleStatus.WaitingAudit;

            //3.将代销转财务记录的状态修改为人工已建:
            foreach (var item in consignSettleInfo.ConsignSettlementItemInfoList)
            {
                item.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.ManualCreated;
            }

            using (TransactionScope scope = new TransactionScope())
            {
                //更新状态:
                consignSettleInfo = ConsignSettlementDA.UpdateSettleStatus(consignSettleInfo);

                #region [Check相关的代销转财务记录,更新代销转财务记录状态]

                foreach (var item in consignSettleInfo.ConsignSettlementItemInfoList)
                {
                    if (item.ConsignToAccLogInfo == null || !item.ConsignToAccLogInfo.LogSysNo.HasValue || !item.ConsignToAccLogInfo.ConsignToAccStatus.HasValue)
                    {
                        //没有相关的代销转财务记录， 或者代销转财务记录状态不能为空!
                        throw new BizException(GetMessageString("Consign_AcctLogIsEmpty"));
                    }
                }
                foreach (var item in consignSettleInfo.ConsignSettlementItemInfoList)
                {
                    ConsignSettlementDA.UpdateConsignToAccountLogStatus(item.ConsignToAccLogInfo.LogSysNo.Value, item.ConsignToAccLogInfo.ConsignToAccStatus.Value);
                }

                #endregion [Check相关的代销转财务记录,更新代销转财务记录状态]

                //发送ESB消息
                EventPublisher.Publish<ConsignSettlementAbandonMessage>(new ConsignSettlementAbandonMessage()
                {
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = consignSettleInfo.SysNo.Value
                });

                //写LOG：
                //CommonService.WriteLog<VendorSettleEntity>(entity, " CancelAbandon VendorSettle ", entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_CancelAbandon);
                ExternalDomainBroker.CreateLog(" CancelAbandon VendorSettle "
                                                                            , BizEntity.Common.BizLogType.POC_VendorSettle_CancelAbandon
                                                                            , consignSettleInfo.SysNo.Value
                                                                            , consignSettleInfo.CompanyCode);

                scope.Complete();
            }

            return consignSettleInfo;
        }

        /// <summary>
        /// 结算代销结算单
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo SettleConsignSettlement(ConsignSettlementInfo consignSettleInfo)
        {
            #region [Check代销结算单已经结算商品逻辑]

            //加载当前要审核的代销结算单信息:
            consignSettleInfo = LoadConsignSettlementInfo(consignSettleInfo.SysNo.Value);

            //1 检查当前结算单状态
            if (consignSettleInfo.Status != SettleStatus.AuditPassed)
            {
                //当前结算单的状态不为已审核状态
                throw new BizException(GetMessageString("Consign_AuditPassed_StatusInvalid"));
            }
            VerifySettleItems(consignSettleInfo, SettlementVerifyType.SETTLE);

            #endregion [Check代销结算单已经结算商品逻辑]

            consignSettleInfo.SettleUser.SysNo = ServiceContext.Current.UserSysNo;

            List<ConsignSettlementRulesInfo> quantityCountList = ObjectFactory<IConsignSettlementDA>.Instance.GetSettleRuleQuantityCount(consignSettleInfo.SysNo.Value);
            foreach (var quantityCount in quantityCountList)
            {
                //规则状态到结算这一步，一定会是已生效或者是未生效，其他的一律视为异常
                if (quantityCount.Status != ConsignSettleRuleStatus.Available && quantityCount.Status != ConsignSettleRuleStatus.Enable)
                {
                    //规则已经是无效状态，抛出异常
                    throw new BizException(string.Format(GetMessageString("Consign_Check_InvalidConsignSettleRule"), quantityCount.SettleRulesName));
                }
                //超过结算数量
                bool result = false;
                if (quantityCount.SettleRulesQuantity.HasValue)
                {
                    result = quantityCount.SettleRulesQuantity.Value - (quantityCount.SettledQuantity ?? 0) - (quantityCount.SubmitSettleQuantity ?? 0) < 0;
                }
                if (result)
                {
                    //超过结算数量，抛出异常
                    throw new BizException(string.Format(GetMessageString("Consign_Check_MoreThanSettleRuleQuantity"), quantityCount.SettleRulesName));
                }
            }


            using (TransactionScope scope = new TransactionScope())
            {
                #region 结算规则操作
                foreach (var quantityCount in quantityCountList)
                {
                    quantityCount.SettledQuantity = quantityCount.SettledQuantity.Value + quantityCount.SubmitSettleQuantity;
                    //修改规则状态
                    if (quantityCount.Status == ConsignSettleRuleStatus.Available)
                        quantityCount.Status = ConsignSettleRuleStatus.Enable;

                    //如果数量为0了,将修改状态为已过期
                    if (quantityCount.SettleRulesQuantity.HasValue && quantityCount.SettleRulesQuantity.Value == (quantityCount.SettledQuantity ?? 0))
                    {
                        quantityCount.Status = ConsignSettleRuleStatus.Disable;
                        quantityCount.SettledQuantity = quantityCount.SettleRulesQuantity;
                    }
                    //修改结算规则状态为已生效
                    ObjectFactory<IConsignSettlementDA>.Instance.UpdateConsignSettleRuleStatusAndQuantity(quantityCount.RuleSysNo.Value, quantityCount.Status.Value, quantityCount.SettledQuantity.Value);
                }
                #endregion

                //1 更改每个item的财务记录为最终结算
                foreach (ConsignSettlementItemInfo item in consignSettleInfo.ConsignSettlementItemInfoList)
                {
                    if (ConsignToAccountLogStatus.Settled == item.ConsignToAccLogInfo.ConsignToAccStatus.Value)
                    {
                        //当前商品的代销转财务记录(记录号:{0})的状态已经为“已结算”状态！
                        throw new BizException(string.Format(GetMessageString("Consign_AccStatus_Settled_Check"), item.ConsignToAccLogInfo.ConsignToAccStatus.Value));
                    }

                    if (item.Cost < 0)
                    {
                        //当前商品(商品系统编号:{0})的结算金额小于0！
                        throw new BizException(string.Format(GetMessageString("Consign_Products_SettleAmt_Check"), item.ProductSysNo.Value));
                    }
                    decimal foldCost = item.ConsignToAccLogInfo.CreateCost.Value - item.Cost.Value;
                    ConsignSettlementDA.SettleConsignToAccountLog(item.ConsignToAccLogInfo.LogSysNo.Value, item.Cost, foldCost);
                }

                //2 修改结算单的状态
                consignSettleInfo.SettleDate = DateTime.Now;
                consignSettleInfo.Status = SettleStatus.SettlePassed;
                consignSettleInfo = ConsignSettlementDA.UpdateSettleStatus(consignSettleInfo);

                scope.Complete();
            }

            //结算商品期间过期状态判断
            foreach (var quantityCount in quantityCountList)
            {
                if (quantityCount.Status == ConsignSettleRuleStatus.Enable)
                {
                    //如果这期间的结算商品已经全部结算完毕，那么这个结算规则也无效,对于是否还有剩余将无效
                    ObjectFactory<IConsignSettlementDA>.Instance.UpdateExistsConsignSettleRuleItemStatus(quantityCount.RuleSysNo.Value);
                }
            }

            decimal userReturnPoint = 0;
            if (consignSettleInfo.EIMSInfo.UsingReturnPoint.HasValue)
            {
                userReturnPoint = consignSettleInfo.EIMSInfo.UsingReturnPoint.Value;
            }
            //3 生成结算单的付款单
            //调用Invoice接口:生成结算单的付款单

            ExternalDomainBroker.CreatePayItem(new PayItemInfo()
            {
                OrderSysNo = consignSettleInfo.SysNo.Value,
                PayAmt = consignSettleInfo.TotalAmt - userReturnPoint,
                OrderType = consignSettleInfo.LeaseType == VendorIsToLease.UnLease ? PayableOrderType.VendorSettleOrder : PayableOrderType.LeaseSettle,
                PayStyle = PayItemStyle.Normal
            });

            //TODO:调用EIMS接口 扣减返点
            //use EIME To Minus ReturnPoint
            //if (entity.UsingReturnPoint.HasValue)
            //{
            //    MinusReturnPointUseEIMS(entity.SysNo, entity.PM_ReturnPointSysNo, entity.UsingReturnPoint, entity.AuditUserSysNo, entity.AuditUser, entity.SettleID);
            //}

            //发送ESB消息
            EventPublisher.Publish<ConsignSettlementSettlementMessage>(new ConsignSettlementSettlementMessage()
            {
                CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                SysNo = consignSettleInfo.SysNo.Value
            });

            //写LOG：
            //CommonService.WriteLog<VendorSettleEntity>(entity, " Settled VendorSettle ", entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_Settle);

            ExternalDomainBroker.CreateLog(" Settled VendorSettle "
                                                                        , BizEntity.Common.BizLogType.POC_VendorSettle_Settle
                                                                        , consignSettleInfo.SysNo.Value
                                                                        , consignSettleInfo.CompanyCode);
            //记录规则更新日志
            foreach (var qLog in quantityCountList)
            {
                ExternalDomainBroker.CreateLog("Settled VendorSettle Update SettleRule"
                                                                          , BizEntity.Common.BizLogType.ConsignSettleRule_Update
                                                                          , qLog.RuleSysNo.Value
                                                                          , qLog.CompanyCode);
            }

            return consignSettleInfo;
        }

        /// <summary>
        /// 取消结算代销结算单
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        public virtual ConsignSettlementInfo CancelSettleConsignSettlement(ConsignSettlementInfo consignSettleInfo)
        {
            #region [Check实体逻辑]

            //1 检查当前结算单状态
            if (consignSettleInfo.Status != SettleStatus.SettlePassed)
            {
                //当前结算单的状态不为已结算状态
                throw new BizException(GetMessageString("Consign_SettlePassed_StatusInvalid"));
            }
            //2 调用Invoice接口,检查付款单是否作废,如果付款单未作废抛出异常
            if (!InvoiceBizInteract.IsAbandonPayItem(consignSettleInfo.SysNo.Value))
            {
                throw new BizException(GetMessageString("Consign_ExistPayItemNotAbandoned"));
            }

            #endregion [Check实体逻辑]

            consignSettleInfo.SettleUser.SysNo = ServiceContext.Current.UserSysNo;

            List<ConsignSettlementRulesInfo> quantityCountList = ObjectFactory<IConsignSettlementDA>.Instance.GetSettleRuleQuantityCount(consignSettleInfo.SysNo.Value);
            foreach (var quantityCount in quantityCountList)
            {
                //规则状态到结算这一步，一定会是已生效或者是已过期，还包括已终止，其他的一律视为异常
                if (quantityCount.Status != ConsignSettleRuleStatus.Disable
                    && quantityCount.Status != ConsignSettleRuleStatus.Enable
                    && quantityCount.Status != ConsignSettleRuleStatus.Stop)
                {
                    throw new BizException(string.Format(GetMessageString("Consign_Check_InvalidConsignSettleRule"), quantityCount.SettleRulesName));
                }
            }

            using (TransactionScope scope = new TransactionScope())
            {
                #region 结算规则操作
                foreach (var quantityCount in quantityCountList)
                {
                    //计算结算数量
                    quantityCount.SettledQuantity -= quantityCount.SubmitSettleQuantity;
                    //修改规则状态
                    switch (quantityCount.Status)
                    {
                        case ConsignSettleRuleStatus.Enable:
                            quantityCount.IsNeedUpdateStatus = true;
                            break;
                        case ConsignSettleRuleStatus.Disable:
                            quantityCount.IsNeedUpdateStatus = true;
                            quantityCount.Status = ConsignSettleRuleStatus.Enable;
                            break;
                        case ConsignSettleRuleStatus.Stop:
                            //已终止状态将不在处理
                            break;
                        default:
                            break;
                    }
                    if (quantityCount.SettledQuantity.Value == 0)
                    {
                        //刚好使用到的规则的商品数量为0，证明只有该数据使用规则,将其变成以未生效状态
                        quantityCount.Status = ConsignSettleRuleStatus.Available;
                    }
                    if (quantityCount.IsNeedUpdateStatus)
                    {
                        //修改结算规则状态为已生效
                        ObjectFactory<IConsignSettlementDA>.Instance.UpdateConsignSettleRuleStatusAndQuantity(quantityCount.RuleSysNo.Value, quantityCount.Status.Value, quantityCount.SettledQuantity.Value);
                    }
                }
                #endregion

                //1 将所有以结算的财务单设置为ManualCreated状态
                foreach (ConsignSettlementItemInfo item in consignSettleInfo.ConsignSettlementItemInfoList)
                {
                    item.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.ManualCreated;
                    item.ConsignToAccLogInfo.Cost = 0;
                    item.ConsignToAccLogInfo.FoldCost = null;
                    ConsignSettlementDA.CancelSettleConsignToAccountLog(item.ConsignToAccLogInfo.LogSysNo.Value);
                }

                //2 修改结算单的状态
                consignSettleInfo.SettleDate = DateTime.Now;
                consignSettleInfo.SettleUser.SysNo = ServiceContext.Current.UserSysNo;
                consignSettleInfo.Status = SettleStatus.AuditPassed;

                consignSettleInfo = ConsignSettlementDA.UpdateSettleStatus(consignSettleInfo);

                //发送ESB消息
                EventPublisher.Publish<ConsignSettlementCancelSettlementMessage>(new ConsignSettlementCancelSettlementMessage()
                {
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = consignSettleInfo.SysNo.Value
                });

                //写LOG；CommonService.WriteLog<VendorSettleEntity>(entity, " CancelSettled VendorSettle ", entity.SysNo.Value.ToString(), (int)LogType.VendorSettle_CancelSettle);

                ExternalDomainBroker.CreateLog(" CancelSettled VendorSettle "
                                                                          , BizEntity.Common.BizLogType.POC_VendorSettle_CancelSettle
                                                                          , consignSettleInfo.SysNo.Value
                                                                          , consignSettleInfo.CompanyCode);
                //记录规则更新日志
                foreach (var qLog in quantityCountList.Where(p => p.IsNeedUpdateStatus))
                {
                    ExternalDomainBroker.CreateLog("CancelSettled VendorSettle Update SettleRule"
                                                                         , BizEntity.Common.BizLogType.ConsignSettleRule_Update
                                                                         , qLog.RuleSysNo.Value
                                                                         , qLog.CompanyCode);
                }
                scope.Complete();
            }
            return consignSettleInfo;
        }

        /// <summary>
        /// 批量删除代销结算单商品
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        /// <returns></returns>
        public virtual ConsignSettlementInfo BatchDeleteConsignSettleItems(ConsignSettlementInfo consignSettleInfo)
        {
            #region [Check 实体逻辑]

            if (consignSettleInfo.Status != SettleStatus.WaitingAudit)
            {
                //当前结算单的状态不为待审核状态
                throw new BizException(GetMessageString("Consign_WaitingAudit_StatusInvalid"));
            }

            #endregion [Check 实体逻辑]

            //删除代销结算单商品操作 ：
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (ConsignSettlementItemInfo item in consignSettleInfo.ConsignSettlementItemInfoList)
                {
                    ConsignSettlementDA.DeleteConsignSettlementItemInfo(item);
                }
                scope.Complete();
            }

            return consignSettleInfo;
        }

        /// <summary>
        /// 新建/更新结算单时对结算商品的Check逻辑
        /// </summary>
        /// <param name="info"></param>
        /// <param name="verifyType"></param>
        public virtual void VerifySettleItems(ConsignSettlementInfo info, SettlementVerifyType verifyType)
        {
            decimal totalCount = 0;

            foreach (ConsignSettlementItemInfo item in info.ConsignSettlementItemInfoList)
            {
                //检查当前item是否是要删除的项目
                if (item.SettleSysNo.HasValue && item.SettleSysNo.Value == -1)
                {
                    continue;
                }
                ConsignToAcctLogInfo getConsignToAccLog = ConsignSettlementDA.LoadConsignToAccountLogInfo(item.ConsignToAccLogInfo.LogSysNo);
                //1 检查当前item是否有对应的account记录
                if (getConsignToAccLog == null)
                {
                    //未找到当前商品(商品系统编号:{0})对应的代销转财务记录！
                    throw new BizException(string.Format(GetMessageString("Consign_ProductsAccLog_NotFound"), item.ProductSysNo));
                }

                switch (verifyType)
                {
                    case SettlementVerifyType.CREATE:
                        //1 检查当前财务记录的状态
                        if (getConsignToAccLog.ConsignToAccStatus != ConsignToAccountLogStatus.Origin)
                        {
                            //当前商品(商品系统编号:{0})对应的到财务记录不为待结算状态！
                            throw new BizException(string.Format(GetMessageString("Consign_ProductsAccLog_WaitingSettle"), item.ProductSysNo));
                        }
                        break;
                    case SettlementVerifyType.SETTLE:
                        //2 检查当前财务记录的状态
                        if (getConsignToAccLog.ConsignToAccStatus != ConsignToAccountLogStatus.SystemCreated && getConsignToAccLog.ConsignToAccStatus != ConsignToAccountLogStatus.ManualCreated)
                        {
                            //当前商品(商品系统编号:{0})对应的到财务记录不为待结算状态！
                            throw new BizException(string.Format(GetMessageString("Consign_ProductsAccLog_WaitingSettle"), item.ProductSysNo));
                        }
                        break;
                    case SettlementVerifyType.UPDATE:
                        //3 检查当前财务记录的状态
                        if (getConsignToAccLog.ConsignToAccStatus == ConsignToAccountLogStatus.Finance || getConsignToAccLog.ConsignToAccStatus == ConsignToAccountLogStatus.Settled)
                        {
                            //当前商品(商品系统编号:{0})对应的到财务记录不为待结算状态！
                            throw new BizException(string.Format(GetMessageString("Consign_ProductsAccLog_WaitingSettle"), item.ProductSysNo));
                        }
                        break;
                    default:
                        //verifyType没有传入有效的值
                        throw new BizException(GetMessageString("Consign_VerifytType_Invalid"));
                }

                //3  检查当前记录的结算价格
                if (item.Cost < 0)
                {
                    //当前商品(商品系统编号:{0})的结算价格不能小于零！
                    throw new BizException(string.Format(GetMessageString("Consign_Products_SettleAmt_Check2"), item.ProductSysNo));
                }

                //4 检查当前记录是否是同一个vendor
                if (getConsignToAccLog.VendorSysNo.Value != info.VendorInfo.SysNo)
                {
                    //当前商品(商品系统编号:{0})的供应商与结算单的供应商不一致！
                    throw new BizException(string.Format(GetMessageString("Consign_VendorNotTheSame"), item.ProductSysNo));
                }

                //5 检查当前记录是否是同一个stock
                if (getConsignToAccLog.StockSysNo != info.SourceStockInfo.SysNo)
                {
                    //因为存在按仓库拆分逻辑，创建时不判断仓库
                    if (verifyType != SettlementVerifyType.CREATE)
                        //当前商品(商品系统编号:{0})的仓库与结算单的仓库不一致！
                        throw new BizException(string.Format(GetMessageString("Consign_StockNotTheSame"), item.ProductSysNo));
                }

                //6 计算总价
                totalCount += item.Cost.Value * getConsignToAccLog.ProductQuantity.Value;
            }
            totalCount = totalCount - info.DeductAmt;
            //7 检查计算的总价
            if (info.TotalAmt != totalCount)
            {
                //结算单的总金额与当前实际结算金额不一致
                throw new BizException(GetMessageString("Consign_SettleAmtTheSame"));
            }
        }

        /// <summary>
        /// 创建时检查实体逻辑(更新时也会调用此方法.)
        /// </summary>
        /// <param name="consignSettleInfo"></param>
        public virtual void VerifyCreate(ConsignSettlementInfo consignSettleInfo)
        {
            //1 检查当前结算单状态
            if (consignSettleInfo.Status != SettleStatus.WaitingAudit)
            {
                //当前结算单的状态不为待审核状态
                throw new BizException(GetMessageString("Consign_WaitingAudit_StatusInvalid"));
            }

            //2 检查settleItems是否为0
            if (consignSettleInfo.ConsignSettlementItemInfoList.Count == 0)
            {
                //结算商品的数量不能为0
                throw new BizException(GetMessageString("Consign_ProductZero"));
            }

            //3 供应商是否为空
            if (consignSettleInfo.VendorInfo == null || !consignSettleInfo.VendorInfo.SysNo.HasValue)
            {
                //供应商不能为空
                throw new BizException(GetMessageString("Consign_VendorEmpty"));
            }

            //4 税率是否为空
            if (!consignSettleInfo.TaxRateData.HasValue)
            {
                //税率不能为空
                throw new BizException(GetMessageString("Consign_TaxEmpty"));
            }

            //5 仓库编号是否为空
            if (consignSettleInfo.SourceStockInfo == null || !consignSettleInfo.SourceStockInfo.SysNo.HasValue)
            {
                    //仓库编号不能为空
                    throw new BizException(GetMessageString("Consign_StockSysNoIsEmpty"));
            }

            //6 货币是否为空
            if (!consignSettleInfo.CurrencyCode.HasValue)
            {
                //货币不能为空
                throw new BizException(GetMessageString("Consign_CurrencyEmpty"));
            }
        }

        /// <summary>
        /// 检查代销结算单的归属PM不能为空,进行产品线相关验证
        /// </summary>
        public virtual void VerifyProductPMSysNo(ConsignSettlementInfo entity, out int productLineSysNo)
        {
            productLineSysNo = 0;
            //if (entity.PMInfo == null || !entity.PMInfo.SysNo.HasValue)
            //{
            //    //代销结算单的归属PM不能为空
            //    throw new BizException("代销结算单的归属PM不能为空");
            //}

            //#region Jack.W.Wang 本代码已停用，更换为产品线验证
            //////获取PM Backup List:
            ////List<int> pms = ConsignSettlementDA.GetBackUpPMList(entity.PMInfo.SysNo.Value, entity.CompanyCode);

            ////pms.Add(entity.PMInfo.SysNo.Value);

            //////代销单归属PM（或归属PM的备份PM）与商品PM不一致 :
            ////bool exists = ConsignSettlementDA.ExistsDifferentPMSysNo(pms, entity.ConsignSettlementItemInfoList.Select(x => x.ProductSysNo.Value).ToList(), entity.CompanyCode);

            ////if (exists)
            ////{
            ////    //代销单归属PM（或归属PM的备份PM）与商品PM不一致
            ////    throw new BizException(GetMessageString("Consign_PMNotTheSame"));
            ////}
            //#endregion

            //#region 产品线验证 CRL21776  2012-11-6  by Jack
            ///*CRL21776  2012-11-6  by Jack
            // * 验证业务说明:
            //    *1.判断商品是否都有产品线
            //    *2.判断登陆PM是否拥有所有商品的产品线权限
            //    *3.判断一张订单上是否只有一条产品线
            //    *4.判断单据的产品线的OwnerPM是否为单据上的所属PM
            // */

            ////验证当前登陆PM是否有对item的操作权限
            //List<ProductPMLine> tPMLineList = ExternalDomainBroker.GetProductLineInfoByPM(ServiceContext.Current.UserSysNo);
            //bool tIsManager = entity.IsManagerPM ?? false;
            //if (tPMLineList.Count > 0 || tIsManager)
            //{
            //    List<int> tProList = (from item in entity.ConsignSettlementItemInfoList
            //                          where item.SettleSysNo != -1
            //                          select item.ProductSysNo.Value).ToList();
            //    List<ProductPMLine> tList = ExternalDomainBroker.GetProductLineSysNoByProductList(tProList.ToArray());
            //    string tErrorMsg = string.Empty;
            //    //检测没有产品线的商品
            //    tList.ForEach(x =>
            //    {
            //        if (x.ProductLineSysNo == null)
            //            tErrorMsg += x.ProductID + Environment.NewLine;
            //    });
            //    if (!tErrorMsg.Equals(string.Empty))
            //    {
            //        throw new BizException(GetMessageString("Consign_CheckMsg_NoProductLine") + Environment.NewLine + tErrorMsg);
            //    }
            //    //检测当前登陆PM对ItemList中商品是否有权限
            //    if (!tIsManager)
            //        tList.ForEach(x =>
            //        {
            //            if (tPMLineList.SingleOrDefault(item => item.ProductLineSysNo == x.ProductLineSysNo) == null)
            //                tErrorMsg += x.ProductID + Environment.NewLine;
            //        });
            //    if (!tErrorMsg.Equals(string.Empty))
            //    {
            //        throw new BizException(GetMessageString("Consign_CheckMsg_NoProductAccess") + Environment.NewLine + tErrorMsg);
            //    }
            //    //验证ItemList中产品线是否唯一
            //    if (tList.Select(x => x.ProductLineSysNo.Value).Distinct().ToList().Count != 1)
            //    {
            //         throw new BizException(GetMessageString("Consign_CheckMsg_NotSamePL"));
            //    }
            //    if ((entity.PMInfo.SysNo == null) || (entity.PMInfo.SysNo.Value != tList[0].PMSysNo.Value))
            //    {
            //        //注释原因：因为结算单存在返点PM验证，所以不能自动更新PM
            //        //需要根据商品的产品线加载PO单的所属PM
            //        //entity.PMInfo.SysNo = tList[0].PMSysNo;
            //        throw new BizException(GetMessageString("Consign_CheckMsg_NotOwnerPM"));
            //    }
            //    else
            //    {
            //        productLineSysNo = tList[0].ProductLineSysNo.Value;
            //    }
            //}
            //else
            //{
            //    throw new BizException(GetMessageString("Consign_CheckMsg_NotOperationPL"));
            //}
            //#endregion
        }

        /// <summary>
        /// EIMS扣减返点
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="pM_ReturnPointSysNo"></param>
        /// <param name="usingReturnPoint"></param>
        /// <param name="AuditUserSysNo"></param>
        /// <param name="AuditUser"></param>
        /// <param name="SettleID"></param>
        public virtual void MinusReturnPointUseEIMS(int? sysNo, int? pM_ReturnPointSysNo, decimal? usingReturnPoint, int? AuditUserSysNo, string AuditUser, string SettleID)
        {
            //TODO:调用EIMS接口,扣减返点
            EIMSResumeReturnPointMessage msg = new EIMSResumeReturnPointMessage()
            {
                IsComeBackPoint = false

                ,
                AuditUserSysNo = AuditUserSysNo.Value
                ,
                AuditUser = AuditUser
                ,
                PM_ReturnPointSysNo = pM_ReturnPointSysNo.Value
                ,
                UsingReturnPoint = usingReturnPoint.Value
                ,
                SettleID = SettleID
                ,
                VendorSettleSysNo = sysNo.Value

                ,
                IsSucceed = false
            };
            EventPublisher.Publish<EIMSResumeReturnPointMessage>(msg);

        }

        /// <summary>
        /// when cancle settle ,comeback returnpoint
        /// </summary>
        /// <param name="nullable"></param>
        /// <param name="nullable_2"></param>
        /// <param name="p"></param>
        private void ComeBackReturnPoint(int? sysNo, int? pM_ReturnPointSysNo, decimal? usingReturnPoint, int? AuditUserSysNo, string AuditUser, string SettleID)
        {
            ////TODO:调用EIMS接口,收回返点
            EIMSResumeReturnPointMessage msg = new EIMSResumeReturnPointMessage()
            {
                IsComeBackPoint = true

                ,
                AuditUserSysNo = AuditUserSysNo.Value
                ,
                AuditUser = AuditUser
                ,
                PM_ReturnPointSysNo = pM_ReturnPointSysNo.Value
                ,
                UsingReturnPoint = usingReturnPoint.Value
                ,
                SettleID = SettleID
                ,
                VendorSettleSysNo = sysNo.Value

                ,
                IsSucceed = false
            };
            EventPublisher.Publish<EIMSResumeReturnPointMessage>(msg);
        }

        public List<ConsignSettlementEIMSInfo> LoadConsignEIMSList(ConsignSettlementInfo info, int? pageIndex, int? pageSize, string sortBy, out int totalCount)
        {
            EIMSInvoiceInfoMessage msg = new EIMSInvoiceInfoMessage()
            {
                IsPage = true,
                PMSysNo = info.PMInfo.SysNo.Value,
                VendorSysNo = info.VendorInfo.SysNo.Value,
                CompanyCode = "8601",
                ReceiveType = "3",
                PageIndex = pageIndex.Value,
                PageSize = pageSize.Value,
                SortField = sortBy
            };
            EventPublisher.Publish<EIMSInvoiceInfoMessage>(msg);
            totalCount = msg.TotalCount;
            List<ConsignSettlementEIMSInfo> list = new List<ConsignSettlementEIMSInfo>();
            msg.ResultList.ForEach(item =>
            {
                list.Add(new ConsignSettlementEIMSInfo()
                {
                    PMSysNo = msg.PMSysNo,
                    VendorSysNo = msg.VendorSysNo,
                    ReturnPointSysNo = item.SysNo,
                    RemnantReturnPoint = item.RemnantReturnPoint,
                    ReturnPointName = item.ReturnPointName,
                    ReturnPoint = item.ReturnPoint
                });
            });
            return list;
        }

        public ConsignSettlementEIMSInfo GetConsignEIMSInfoBySysNo(int ReturnPointSysNo)
        {
            EIMSInvoiceInfoMessage msg = new EIMSInvoiceInfoMessage()
            {
                IsPage = false,
                CompanyCode = "8601",
                InvoiceNumber = ReturnPointSysNo
            };
            EventPublisher.Publish<EIMSInvoiceInfoMessage>(msg);
            ConsignSettlementEIMSInfo result = new ConsignSettlementEIMSInfo()
            {
                ReturnPointSysNo = msg.Result.SysNo,
                ReturnPointName = msg.Result.ReturnPointName,
                ReturnPoint = msg.Result.ReturnPoint,
                RemnantReturnPoint = msg.Result.RemnantReturnPoint
            };
            return result;
        }

        private void CheckReturnPointInfo(ConsignSettlementInfo consignSettleInfo)
        {
            if (consignSettleInfo.PMInfo == null || !consignSettleInfo.PMInfo.SysNo.HasValue)
            {
                //返点所属PM不能为空
                throw new BizException(GetMessageString("Consign_ReturnPointPMEmpty"));
            }
            if (consignSettleInfo.PM_ReturnPointSysNo.HasValue)
            {
                if (!consignSettleInfo.EIMSInfo.UsingReturnPoint.HasValue)
                {
                    consignSettleInfo.EIMSInfo.UsingReturnPoint = 0;
                }
                //TODO:调用EIMS接口，根据编号查询返点信息:
                EIMSInvoiceInfoForConsignMessage msg = new EIMSInvoiceInfoForConsignMessage()
                {
                    InvoiceNumber = consignSettleInfo.PM_ReturnPointSysNo.Value,
                    CompanyCode = "8601",
                    ReceiveType = 3
                };
                EventPublisher.Publish<EIMSInvoiceInfoForConsignMessage>(msg);
                if (!msg.IsError)
                {
                    if (consignSettleInfo.EIMSInfo.UsingReturnPoint.Value >= msg.RemnantReturnPoint)
                    {
                        consignSettleInfo.EIMSInfo.UsingReturnPoint = msg.RemnantReturnPoint;
                    }
                    if (consignSettleInfo.EIMSInfo.UsingReturnPoint.Value <= 0)
                    {
                        consignSettleInfo.EIMSInfo.UsingReturnPoint = 0;
                    }
                    if (consignSettleInfo.EIMSInfo.UsingReturnPoint.Value >= consignSettleInfo.TotalAmt)
                    {
                        consignSettleInfo.EIMSInfo.UsingReturnPoint = consignSettleInfo.TotalAmt;
                    }
                    if (consignSettleInfo.TotalAmt <= 0)
                    {
                        consignSettleInfo.EIMSInfo.UsingReturnPoint = 0;
                    }
                }
                if (msg.PM != consignSettleInfo.PMInfo.SysNo.Value.ToString())
                {
                    //返点所属PM和当前PM不一致
                    throw new BizException(GetMessageString("Consign_ReturnPointPMNotTheSame"));
                }
                if (msg.VendorSysNo != consignSettleInfo.VendorInfo.SysNo.Value)
                {
                    //返点所属供应商和当前供应商不一致
                    throw new BizException(GetMessageString("Consign_ReturnPointVendorNotTheSame"));
                }
                if (msg.RemnantReturnPoint < consignSettleInfo.EIMSInfo.UsingReturnPoint.Value)
                {
                    //当前使用返点金额超过可用金额
                    throw new BizException(GetMessageString("Consign_ReturnPointAmtLarge"));
                }
            }
            else
            {
                consignSettleInfo.EIMSInfo.UsingReturnPoint = 0;
            }
        }

        private void CheckPMSysNo(ConsignSettlementInfo info)
        {
            if (!info.PMInfo.SysNo.HasValue)
            {
                //所属PM不能为空
                throw new BizException(GetMessageString("Consign_PMEmpty"));
            }
            if (info.PM_ReturnPointSysNo.HasValue)
            {
                if (!info.EIMSInfo.UsingReturnPoint.HasValue)
                {
                    //选择“返点”后，则“使用返点金额”必须填写
                    throw new BizException(GetMessageString("Consign_UsingReturnPointEmpty"));
                }
            }
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetMessageString(string key)
        {
            return ResouceManager.GetMessageString("PO.ConsignSettlement", key);
        }

        public virtual int? GetConsignSettlementReturnPointSysNo(int consignSettleSysNo)
        {
            return ConsignSettlementDA.GetConsignSettlementReturnPointSysNo(consignSettleSysNo);
        }

        /// <summary>
        /// 根据供应商系统编号取得代销结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public virtual List<int> GetVendorSettleSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            return ConsignSettlementDA.GetVendorSettleSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);
        }

        /// <summary>
        /// 2012-9-14 Jack 根据不同权限获取PMList:
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual List<int> GetPMSysNoListByType(ConsignSettlementBizInfo info)
        {
            List<BizEntity.IM.ProductManagerInfo> tmpPMList = ObjectFactory<ECCentral.Service.IBizInteract.IIMBizInteract>.Instance.GetPMListByType(info.QueryType, info.CurrentUserName, info.CompanyCode);
            List<int> result = (from item in tmpPMList
                                select item.SysNo.Value).ToList<int>();
            return result;
        }

        public DataTable GetSettleList(DataTable dt)
        {
            if (dt == null && dt.Rows.Count == 0)
                return dt;

            foreach (DataRow row in dt.Rows)
            {
                int settleSysNo = Convert.ToInt32(row["SysNo"]);
                decimal TotalAmt = Convert.ToDecimal(row["TotalAmt"]);

                //应付价款
                decimal RateAmount = 0;

                //应付税金
                decimal RateTotal = 0;

                // 税金(其他)
                decimal RateAmountOther = 0;

                //税金(13%)
                decimal RateCost13 = 0;

                //税金(17%)
                decimal RateCost17 = 0;

                //价款(17%)
                decimal Cost17 = 0;

                // 价款(13%)
                decimal Cost13 = 0;

                //价款(其它)
                decimal CostOther = 0;

                List<SettleItemInfo> itemList = ObjectFactory<IGatherSettlementDA>.Instance.GetSettleItemList(settleSysNo);
                if (itemList != null)
                {
                    itemList.ForEach(x =>
                    {
                        if (x.TaxRate.HasValue)
                        {
                            if (x.TaxRate.Value != (decimal)0.17 && x.TaxRate.Value != (decimal)0.13)
                            {
                                RateAmountOther += PointTwo(x.PayAmt.Value * x.TaxRate.Value / (decimal)(1 + x.TaxRate.Value));
                                CostOther += PointTwo(x.PayAmt.Value / (decimal)(1 + x.TaxRate.Value));
                            }
                            else if (x.TaxRate.Value == (decimal)0.17)
                            {
                                RateCost17 += PointTwo(x.PayAmt.Value * x.TaxRate.Value / (decimal)(1 + 0.17));

                                Cost17 += PointTwo(x.PayAmt.Value / (decimal)(1 + 0.17));
                            }
                            else if (x.TaxRate.Value == (decimal)0.13)
                            {
                                RateCost13 += PointTwo(x.PayAmt.Value * x.TaxRate.Value / (decimal)(1 + 0.13));

                                Cost13 += PointTwo(x.PayAmt.Value / (decimal)(1 + 0.13));
                            }
                        }
                    });
                }

                RateAmount = RateCost13 + RateCost17 + RateAmountOther;
                RateTotal = Cost13 + Cost17 + CostOther;

                row["RateAmount"] = SSWR(RateAmount);
                row["RateTotal"] = SSWR(RateTotal);
                row["RateAmountOther"] = SSWR(RateAmountOther);
                row["RateCost13"] = SSWR(RateCost13);
                row["RateCost17"] = SSWR(RateCost17);
                row["Cost17"] = SSWR(Cost17);
                row["Cost13"] = SSWR(Cost13);
                row["CostOther"] = SSWR(CostOther);

                row["TotalAmt"] = SSWR(TotalAmt);
            }

            return dt;
        }

        private string SSWR(decimal item)
        {
            return Math.Round(item, 2, MidpointRounding.AwayFromZero).ToString();
        }

        private decimal PointTwo(decimal item)
        {
            return Math.Round(item, 2, MidpointRounding.AwayFromZero);
        }

        #region 经销商品结算

        /// <summary>
        /// 创建经销商品结算单
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
        public SettleInfo CreateSettleAccount(SettleInfo settleInfo)
        {
            settleInfo.CreateTime = DateTime.Now;
            settleInfo.Status = POSettleStatus.Created;
            settleInfo.StockSysNo = 51;//默认
            /*
             * 1.插入主表
             * 2.插入子表
             */
            using (TransactionScope ts = new TransactionScope())
            {
                 //1.插入主表
                settleInfo = ConsignSettlementDA.SettleInfoAdd(settleInfo);
                foreach (var sub in settleInfo.SettleItemInfos)
                {
                    //2.插入子表
                    sub.SettleSysNo = settleInfo.SysNo;
                    sub.CompanyCode = settleInfo.CompanyCode;

                   var result = ConsignSettlementDA.SettleItemInfoAdd(sub);
                   sub.SysNo = result.SysNo;
                    //修改订单支付状态为已支付
                   ConsignSettlementDA.ChangeFinancePayStatus(PayableStatus.FullPay, sub.FinancePaySysNo.Value);
                   ConsignSettlementDA.FinancePayItemPaid( sub.FinancePaySysNo.Value);
                }
                ts.Complete();
            }
            return settleInfo;
        }

        /// <summary>
        /// 查询经销商品详细信息(基本信息和个子项税率信息)
        /// </summary>
        /// <param name="SettleInfo"></param>
        /// <returns></returns>
        public SettleInfo GetSettleAccount(SettleInfo settleInfo)
        {
           settleInfo =  ConsignSettlementDA.GetSettleInfo(settleInfo);
           if (settleInfo == null)
           {
               throw new BizException("经销商品结算单信息不存在");
           }
           settleInfo.SettleItemInfos = ConsignSettlementDA.GetSettleItemInfoWithTaxAndCost(settleInfo);
           return settleInfo;
        }

        /// <summary>
        /// 审核经销商品结算单
        /// </summary>
        /// <param name="SettleInfo"></param>
        public void AuditSettleAccount(SettleInfo settleInfo)
        {
            settleInfo.AuditTime = DateTime.Now;
            ConsignSettlementDA.AuditSettleAccount(settleInfo);
            using (TransactionScope ts = new TransactionScope())
            {
                if (settleInfo.Status == POSettleStatus.AuditPassed)//审核后，支付状态为已支付2
                {
                }
                else if (settleInfo.Status == POSettleStatus.Abandon) //作废后，支付状态还原未支付0
                {
                    var settleInfoList = ConsignSettlementDA.GetSettleItemInfo(settleInfo);
                    foreach (var sub in settleInfoList)
                    {
                        ConsignSettlementDA.ChangeFinancePayStatus(PayableStatus.UnPay, sub.FinancePaySysNo.Value);
                        ConsignSettlementDA.FinancePayItemOrigin(sub.FinancePaySysNo.Value);
                    }
                }

                ts.Complete();
            }
        }

        #endregion

    }
}
