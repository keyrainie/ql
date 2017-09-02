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
using ECCentral.BizEntity.PO.Commission;
using ECCentral.Service.EventMessage.PO;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 代收代付结算单 - BizProcessor
    /// </summary>
    [VersionExport(typeof(CollectionPaymentProcessor))]
    public class CollectionPaymentProcessor
    {

        #region [Fields]
        private ICollectionPaymentDA m_CollectionPaymentDA;
        private IConsignSettlementDA m_ConsignSettlementDA;
        private IVendorDA m_VendorDA;
        private IInvoiceBizInteract m_InvoiceBizInteract;
        private IConsignSettlementRulesDA m_ConsignSettlementRulesDA;

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

        public ICollectionPaymentDA CollectionPaymentDA
        {
            get
            {
                if (null == m_CollectionPaymentDA)
                {
                    m_CollectionPaymentDA = ObjectFactory<ICollectionPaymentDA>.Instance;
                }
                return m_CollectionPaymentDA;
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

        public IConsignSettlementRulesDA ConsignSettlementRulesDA
        {
            get
            {
                if (null == m_ConsignSettlementRulesDA)
                {
                    m_ConsignSettlementRulesDA = ObjectFactory<IConsignSettlementRulesDA>.Instance;
                }
                return m_ConsignSettlementRulesDA;
            }
        }


        #endregion

        /// <summary>
        /// 加载代销结算单详细
        /// </summary>
        /// <param name="settlementSysNo"></param>
        /// <returns></returns>
        public virtual CollectionPaymentInfo Load(int sysNo)
        {
            //加载代销结算单主信息:
            CollectionPaymentInfo returnEntity = new CollectionPaymentInfo();
            
            returnEntity = CollectionPaymentDA.Load(sysNo);
           
           
            return returnEntity;
        }

        /// <summary>
        /// 加载代销结算单详细
        /// </summary>
        /// <param name="settlementSysNo"></param>
        /// <returns></returns>
        public virtual CollectionPaymentInfo Create(CollectionPaymentInfo entity)
        {
            //去除已经删除的Item（SysNo=-1） 次方法已经在Portal加过了，此处只为以防万一了
            entity.SettleItems = (from tItem in entity.SettleItems
                                                               where tItem.SettleSysNo != -1
                                                               select tItem).ToList();

            string OperationIP = string.Empty;
            string OperationUserUniqueName = string.Empty;

            VerifyCreate(entity);
            VerifySettleItems(entity, SettlementVerifyType.CREATE);
            VerifyProductPMSysNo(entity);
            //VerityOverConsignRuleQuantity(entity);

            //entity.CreateUserSysNo = SystemUserHelper.GetUserSystemNumber(BusinessContext.Current.OperationUserFullName,
            //        BusinessContext.Current.OperationUserSourceDirectoryKey, BusinessContext.Current.OperationUserLoginName,
            //        BusinessContext.Current.CompanyCode);
            entity.CreateUserSysNo =  entity.CurrentUserSysNo; ;
            entity.CreateTime = DateTime.Now;
            entity.Status = POCollectionPaymentSettleStatus.Origin;

            //修改代销转财务记录的状态
            foreach (var item in entity.SettleItems)
            {
                item.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.ManualCreated;
                //item.
            }
            //用于收集操作过的结算单
            List<CollectionPaymentInfo> settleEntityList = new List<CollectionPaymentInfo>();

            //计算返点,返点总额将会平均分配到以下结算单，当返点总额大于单个结算单的返点额，那单个结算单返点将和计算单总额相等
            //剩余返点额将在下个结算单中使用
            decimal totalUsingReturnPoint = entity.UsingReturnPoint ?? 0;

            using (TransactionScope scope = new TransactionScope())
            {
                //按照仓库的不同自动分结算单
                var stockSysNoList = entity.SettleItems.Select(p => p.ConsignToAccLogInfo.StockSysNo).Distinct();

                foreach (int? stockSysNo in stockSysNoList)
                {
                    //需要深度赋值新的类
                    var newEntity = SerializationUtility.DeepClone<CollectionPaymentInfo>(entity);

                    newEntity.SourceStockInfo.SysNo = stockSysNo;
                    var tempSettleList = new List<CollectionPaymentItem>();
                    tempSettleList.AddRange(newEntity.SettleItems.Where(p => p.ConsignToAccLogInfo.StockSysNo == stockSysNo));
                    //计算总结算数
                    newEntity.TotalAmt = tempSettleList.Sum(p => p.Cost * p.ConsignToAccLogInfo.ProductQuantity.Value);

                
                    newEntity.SettleItems = tempSettleList;

                    //判断规则数量是否还有剩余


                    newEntity = CollectionPaymentDA.Create(newEntity);
                    entity.SysNo = newEntity.SysNo;
                    //将操作完成的收集在临时列表中
                    settleEntityList.Add(newEntity);
                }
                //更新代销转财务记录状态
                settleEntityList.ForEach(x =>
                {
                    UpdateConsignToAccLogStatus(x);
                });

                //发送ESB消息
                EventPublisher.Publish<CollectionPaymentCreateMessage>(new CollectionPaymentCreateMessage()
                {
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = entity.SysNo.Value
                });

                scope.Complete();
            }

            foreach (var newEntity in settleEntityList)
            {
                //执行其它操作
                newEntity.OperationIP = OperationIP;
                newEntity.OperationUserSysNumber = entity.CreateUserSysNo;
                newEntity.OperationUserUniqueName = OperationUserUniqueName;
                //CommonService.WriteLog<CollectionPaymentEntity>(newEntity, " Create VendorSettle ", newEntity.SysNo.Value.ToString(), (int)LogType.CollectionPayment_Create);
                VendorInfo vendor = VendorDA.LoadVendorInfo(newEntity.VendorInfo.SysNo.Value);
                if (vendor.VendorFinanceInfo.IsAutoAudit != null && vendor.VendorFinanceInfo.IsAutoAudit==true )
                {
                    try
                    {
                        Audit(newEntity);
                    }
                    catch (Exception  ex)
                    {
                       //MailService.SendMail(ConfigurationManager.AppSettings["VendorSettleFrom"], ConfigurationManager.AppSettings["VendorSettleTo"], "代销结算单自动审核", newEntity.SysNo.ToString() + ":" + ex.ErrorDescription);
                    }

                    //自动结算
                    entity = Settle(newEntity);
                }
            }
            return entity;
        }

        public virtual void VerifyProductPMSysNo(CollectionPaymentInfo entity)
        {
            if (entity.PMInfo == null || !entity.PMInfo.SysNo.HasValue)
            {
                //代销结算单的归属PM不能为空
                throw new BizException("代销结算单的归属PM不能为空");
            }

            #region Jack.W.Wang 本代码已停用，更换为产品线验证
            //获取PM Backup List:
            List<int> pms = ConsignSettlementDA.GetBackUpPMList(entity.PMInfo.SysNo.Value, entity.CompanyCode);

            pms.Add(entity.PMInfo.SysNo.Value);

            //代销单归属PM（或归属PM的备份PM）与商品PM不一致 :
            bool exists = ConsignSettlementDA.ExistsDifferentPMSysNo(pms, entity.SettleItems.Select(x => x.ProductSysNo.Value).ToList(), entity.CompanyCode);

            if (exists)
            {
                //代销单归属PM（或归属PM的备份PM）与商品PM不一致
                throw new BizException("归属PM（或归属PM的备份PM）与商品PM不一致");
            }
            #endregion
        }

        private void VerifyCancelAudited(CollectionPaymentInfo entity)
        {
            Verfy(entity);

            //1 检查当前结算单状态
            if (entity.Status.Value != POCollectionPaymentSettleStatus.Audited)
            {
                throw new BizException("不是审核状态不能作废");
            }
        }

        private void Verfy(CollectionPaymentInfo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
        }


        private void VerifyUpdate(CollectionPaymentInfo entity)
        {
            VerifyCreate(entity);

            if (!entity.SysNo.HasValue)
            {
                throw new BizException("当前结算单的状态不为已审核状态！");
            }
        }

        private void VerifyCreate(CollectionPaymentInfo entity)
        {
            Verfy(entity);

            //1 检查当前结算单状态
            if (entity.Status.HasValue)
            {
                if (entity.Status != POCollectionPaymentSettleStatus.Origin)
                {
                    throw new BizException("当前结算单的状态不为待审核状态！");
                }
            }

            //3 检查settleItems是否为0
            if (entity.SettleItems.Count == 0)
            {
                throw new BizException("结算单条目不能为空！");
            }

            //4 供应商是否为空
            if (!entity.VendorInfo.SysNo.HasValue)
            {
                throw new BizException("供应商不能为空！");
            }

            //5 税率是否为空
            if (!entity.TaxRateData.HasValue)
            {
                throw new BizException("税率不能为空！");
            }

            //6 仓库编号是否为空
            if (entity.SourceStockInfo == null || !entity.SourceStockInfo.SysNo.HasValue)
            {
                //新增存在多个仓库，不会对此进行判断
                if (entity.SysNo.HasValue)
                    //仓库编号不能为空
                    throw new BizException(GetMessageString("Consign_StockSysNoIsEmpty"));
            }

            //7 货币是否为空
            if (!entity.CurrencyCode.HasValue)
            {
                throw new BizException("货币类型不能为空！");
            }
        }

        private void VerifyAbandon(CollectionPaymentInfo entity)
        {
            Verfy(entity);

            //1 检查当前结算单状态
            if (entity.Status.Value != POCollectionPaymentSettleStatus.Origin)
            {
                throw new BizException("当前结算单的状态不为待审核状态!");
            }
        }

        /// <summary>
        /// 验证用到的规则结算数量是否超过设定
        /// </summary>
        /// <param name="entity">请求的结算单</param>
        void VerityOverConsignRuleQuantity(CollectionPaymentInfo entity)
        {
            
            var overRule = GetOverConsignRule(entity);
            if (overRule != null)
            {
                throw new BizException("规则结算数量超过设定");
            }
            
        }

        
        /// <summary>
        /// 获取已经超过结算数量的规则
        /// </summary>
        /// <param name="entity">请求的结算单</param>
        /// <returns>超过结算数量的规则</returns>
       public ConsignSettlementRulesInfo GetOverConsignRule(CollectionPaymentInfo entity)
        {
            ConsignSettlementRulesInfo result = null;

            //提取结算单中包括的规则的项，并统计
            StringBuilder ruleIds = new StringBuilder();
            ruleIds.Append("-9999");
            var ruleSysNoList = entity.SettleItems.Select(p => p.SettleRuleSysNo.ToString()).Distinct();
            foreach (var item in ruleSysNoList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    ruleIds.Append("," + item + "");
                }
            }

            //提出需要判断的规则相关参数
            var ruleList = ConsignSettlementRulesDA.GetSettleRuleListBySysNos(ruleIds.ToString());

            if (ruleList.Count > 0)
            {
                foreach (var rule in ruleList)
                {
                    //提交规则对应的数量hack
                    rule.SubmitSettleQuantity = (int)entity.SettleItems.Where(p => p.SettleRuleSysNo == rule.RuleSysNo).Sum(p => p.ConsignQty);

                    //判断数量是否超过结算数量
                    if (rule.IsOverQuantity)
                    {
                        result = rule;
                        break;
                    }
                }
            }

            return result;
        }
    
        
        /// <summary>
        /// 新建/更新结算单时对结算商品的Check逻辑
        /// </summary>
        /// <param name="info"></param>
        /// <param name="verifyType"></param>
        public virtual void VerifySettleItems(CollectionPaymentInfo info, SettlementVerifyType verifyType)
        {
            decimal totalCount = 0;

            foreach (CollectionPaymentItem item in info.SettleItems)
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
                    throw new BizException(string.Format(GetMessageString("Consign_ProductsAccLog_NotFound"), item.ConsignToAccLogInfo.ProductSysNo));
                }

                switch (verifyType)
                {
                    case SettlementVerifyType.CREATE:
                        //1 检查当前财务记录的状态
                        if (getConsignToAccLog.ConsignToAccStatus != ConsignToAccountLogStatus.Origin)
                        {
                            //当前商品(商品系统编号:{0})对应的到财务记录不为待结算状态！
                            throw new BizException(string.Format(GetMessageString("Consign_ProductsAccLog_WaitingSettle"), item.ConsignToAccLogInfo.ProductSysNo));
                        }
                        break;
                    case SettlementVerifyType.SETTLE:
                        //2 检查当前财务记录的状态
                        if (getConsignToAccLog.ConsignToAccStatus != ConsignToAccountLogStatus.SystemCreated && getConsignToAccLog.ConsignToAccStatus != ConsignToAccountLogStatus.ManualCreated)
                        {
                            //当前商品(商品系统编号:{0})对应的到财务记录不为待结算状态！
                            throw new BizException(string.Format(GetMessageString("Consign_ProductsAccLog_WaitingSettle"), item.ConsignToAccLogInfo.ProductSysNo));
                        }
                        break;
                    case SettlementVerifyType.UPDATE:
                        //3 检查当前财务记录的状态
                        if (getConsignToAccLog.ConsignToAccStatus == ConsignToAccountLogStatus.Finance || getConsignToAccLog.ConsignToAccStatus == ConsignToAccountLogStatus.Settled)
                        {
                            //当前商品(商品系统编号:{0})对应的到财务记录不为待结算状态！
                            throw new BizException(string.Format(GetMessageString("Consign_ProductsAccLog_WaitingSettle"), item.ConsignToAccLogInfo.ProductSysNo));
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
                    throw new BizException(string.Format(GetMessageString("Consign_Products_SettleAmt_Check2"), item.ConsignToAccLogInfo.ProductSysNo));
                }

                //4 检查当前记录是否是同一个vendor
                if (getConsignToAccLog.VendorSysNo.Value != info.VendorInfo.SysNo)
                {
                    //当前商品(商品系统编号:{0})的供应商与结算单的供应商不一致！
                    throw new BizException(string.Format(GetMessageString("Consign_VendorNotTheSame"), item.ConsignToAccLogInfo.ProductSysNo));
                }

                //5 检查当前记录是否是同一个stock
                if (getConsignToAccLog.StockSysNo != info.SourceStockInfo.SysNo)
                {
                    //因为存在按仓库拆分逻辑，创建时不判断仓库
                    if (verifyType != SettlementVerifyType.CREATE)
                        //当前商品(商品系统编号:{0})的仓库与结算单的仓库不一致！
                        throw new BizException(string.Format(GetMessageString("Consign_StockNotTheSame"), item.ConsignToAccLogInfo.ProductSysNo));
                }

                //6 计算总价
                totalCount += item.Cost * getConsignToAccLog.ProductQuantity.Value;
            }         
            //7 检查计算的总价
            if (info.TotalAmt != totalCount)
            {
                //结算单的总金额与当前实际结算金额不一致
                throw new BizException(GetMessageString("Consign_SettleAmtTheSame"));
            }
        }


         private void VerifySettle(CollectionPaymentInfo entity)
        {
            Verfy(entity);

            //1 检查当前结算单状态
            if (entity.Status.Value != POCollectionPaymentSettleStatus.Audited)
            {
                throw new BizException("当前结算单的状态不为已审核状态！");
            }
        }

         private void VerifyCancelSettled(CollectionPaymentInfo entity)
         {
             Verfy(entity);

             //1 检查当前结算单状态
             if (entity.Status.Value != POCollectionPaymentSettleStatus.Settled)
             {
                 throw new BizException("当前结算单的状态不为已结算状态！");
             }

             //2 检查付款单是否作废,如果付款单未作废抛出异常
             //dal = collVendorSettleDAL;

             if (!this.CollectionPaymentDA.IsAbandonPayItem(entity.SysNo.Value))
             {
                 throw new BizException("当前结算单对应的应付款记录应全部为“已作废”状态！");
             }
         }

         private void VerifyCancelAbandon(CollectionPaymentInfo entity)
         {
             //VendorSettleDAL dal;

             Verfy(entity);

             //1 检查当前结算单状态
             if (entity.Status.Value != POCollectionPaymentSettleStatus.Abandon)
             {
                 throw new BizException("当前结算单的状态不为已作废状态!");
             }
             //2 检查当前作废的item是否已存在于其他结算单据中
             //dal = new VendorSettleDAL();

             foreach (CollectionPaymentItem item in entity.SettleItems)
             {
                 bool tmpIsExist = this.ConsignSettlementDA.IsAccountLogExistOtherVendorSettle(item.ConsignToAccLogInfo.LogSysNo.Value);
                 if (tmpIsExist)
                 {
                     throw new BizException(
                         string.Format("当前商品(财务记录编号:{0})已存在于其他供应商结算单中！", (item.ConsignToAccLogInfo.LogSysNo.HasValue ? item.POConsignToAccLogSysNo.Value.ToString() : ""))
                         );
                 }
             }
         }

         private void VerifyAudite(CollectionPaymentInfo entity)
         {
             Verfy(entity);
             //<!--Add By Kilin 2012-08-24
             //代销结算单，创建人和审核人不能相同
             CollectionPaymentInfo old = this.CollectionPaymentDA .Load(entity.SysNo.Value);
             if (old == null)
             {
                 throw new BizException("单据不存在!");
             }
             //1 检查当前结算单状态
             if (old.Status.Value !=POCollectionPaymentSettleStatus.Origin)
             {
                 throw new BizException("不是待审核状态！");
             }
           //if (old.CreateUserSysNo == entity.AuditUserSysNo)
           //  {
           //      throw new BizException("创建人，审核人不能相同");
           //  }
             
         }

        public CollectionPaymentInfo Update(CollectionPaymentInfo entity)
        {
            string OperationIP = entity.OperationIP;
            int? OperationUserSysNumber = entity.OperationUserSysNumber;
            string OperationUserUniqueName = entity.OperationUserUniqueName;

             VerifyUpdate(entity);
             VerifySettleItems(entity, SettlementVerifyType.UPDATE);
            VerifyProductPMSysNo(entity);
            VerityOverConsignRuleQuantity(entity);

           
            List<CollectionPaymentItem> list = new List<CollectionPaymentItem>();

            using (TransactionScope scope = new TransactionScope())
            {
                //1 删除需要delete的item
                foreach (CollectionPaymentItem item in entity.SettleItems)
                {
                    if (item.SettleSysNo.HasValue && item.SettleSysNo == -1)
                    {
                        list.Add(item);
                    }
                }

                if (list.Count > 0)
                {
                    foreach (CollectionPaymentItem item in list)
                    {
                        //将需要删除的Item对应的Acclog状态改为[初始状态:0]
                        //VendorDA.UpdateConsignToAccountLogStatus(item.ConsignToAccLogInfo.LogSysNo.Value, ConsignToAccountLogStatus.Origin);
                        this.ConsignSettlementDA.UpdateConsignToAccountLogStatus(
                            item.ConsignToAccLogInfo.LogSysNo.Value, ConsignToAccountLogStatus.Origin);
                        CollectionPaymentDA.DeleteSettleItem(item);

                        entity.SettleItems.Remove(item);
                    }
                }

                //将所有选中的SettleItem状态改为[人工已建:4]
                var validItems = entity.SettleItems.Except(list);

                foreach (var item in validItems)
                {
                    //如果item的sysno没有值,则表示是新增加的item,需要更新对应的ConfignToAcclog
                    if (!item.ItemSysNo.HasValue)
                    {
                        item.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.ManualCreated;
                        this.ConsignSettlementDA.UpdateConsignToAccountLogStatus(item.ConsignToAccLogInfo.LogSysNo.Value, ConsignToAccountLogStatus.ManualCreated);
                    }
                }

                //2 更新结算单
                CollectionPaymentDA.Update(entity);

                scope.Complete();
            }

            //3 记录删除的items
            foreach (CollectionPaymentItem item in list)
            {
                if (item.ItemSysNo.HasValue)
                {
                    item.OperationIP = OperationIP;
                    item.OperationUserSysNumber = OperationUserSysNumber;
                    item.OperationUserUniqueName = OperationUserUniqueName;
                    //CommonService.WriteLog<CollectionPaymentItem>(item, " Deleted CollectionPaymentItem ", item.SysNo.Value.ToString(), (int)LogType.CollectionPayment_Item_Delete);
                    ExternalDomainBroker.CreateLog(" Deleted CollectionPaymentItem "
           , BizEntity.Common.BizLogType.POC_VendorSettle_Item_Delete
           , item.ItemSysNo.Value
           , entity.CompanyCode);
                }
            }

            //4 记录修改的items
            foreach (CollectionPaymentItem item in entity.SettleItems)
            {
                if (item.ItemSysNo.HasValue)
                {
                    item.OperationIP = OperationIP;
                    item.OperationUserSysNumber = OperationUserSysNumber;
                    item.OperationUserUniqueName = OperationUserUniqueName;
                    //CommonService.WriteLog<CollectionPaymentItem>(item, " Updated CollectionPaymentSettleItem ", item.SysNo.Value.ToString(), (int)LogType.CollectionPayment_Item_Update);
                    ExternalDomainBroker.CreateLog(" Updated CollectionPaymentSettleItem "
             , BizEntity.Common.BizLogType.POC_VendorSettle_Item_Update
             , item.ItemSysNo.Value
             , entity.CompanyCode);
                }
            }

            return entity;
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CollectionPaymentInfo Audit(CollectionPaymentInfo entity)
        {
            string OperationIP = entity.OperationIP;
            int? OperationUserSysNumber = entity.OperationUserSysNumber;
            string OperationUserUniqueName = entity.OperationUserUniqueName;
            entity.AuditUserSysNo = entity.CurrentUserSysNo;
            //entity.AuditUserSysNo = SystemUserHelper.GetUserSystemNumber(BusinessContext.Current.OperationUserFullName,
            //    BusinessContext.Current.OperationUserSourceDirectoryKey, BusinessContext.Current.OperationUserLoginName,
            //    BusinessContext.Current.CompanyCode);
            //entity.AuditUserSysNo = 3256;
            VerifyAudite(entity);
            VerifyProductPMSysNo(entity);


            entity.AuditTime = DateTime.Now;
            entity.Status = POCollectionPaymentSettleStatus.Audited;

            entity = CollectionPaymentDA.UpdateVendorSettleStatus(entity);

            //发送ESB消息
            EventPublisher.Publish<CollectionPaymentAuditMessage>(new CollectionPaymentAuditMessage()
            {
                CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                SysNo = entity.SysNo.Value
            });

            entity.OperationIP = OperationIP;
            entity.OperationUserSysNumber = OperationUserSysNumber;
            entity.OperationUserUniqueName = OperationUserUniqueName;

            //CommonService.WriteLog<CollectionPaymentEntity>(entity, " Audit CollectionPayment ", entity.SysNo.Value.ToString(), (int)LogType.CollectionPayment_Audit);
            ExternalDomainBroker.CreateLog(" Audit CollectionPayment "
       , BizEntity.Common.BizLogType.POC_VendorSettle_Audit
       , entity.SysNo.Value
       , entity.CompanyCode);
            return entity;
        }
        public CollectionPaymentInfo CancelAudited(CollectionPaymentInfo entity)
        {
            VerifyCancelAudited(entity);
            entity.AuditUserSysNo = entity.CurrentUserSysNo;
            //entity.AuditUserSysNo = SystemUserHelper.GetUserSystemNumber(BusinessContext.Current.OperationUserFullName,
            //    BusinessContext.Current.OperationUserSourceDirectoryKey, BusinessContext.Current.OperationUserLoginName,
            //    BusinessContext.Current.CompanyCode);
            entity.CreateTime = DateTime.Now;
            entity.Status = POCollectionPaymentSettleStatus.Origin;
            
            entity = CollectionPaymentDA.UpdateVendorSettleStatus(entity);

            //发送ESB消息
            EventPublisher.Publish<CollectionPaymentCancelMessage>(new CollectionPaymentCancelMessage()
            {
                CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                SysNo = entity.SysNo.Value
            });

            return entity;
        }

        public CollectionPaymentInfo Settle(CollectionPaymentInfo entity)
        {
            
            string OperationIP = entity.OperationIP;
            int? OperationUserSysNumber = entity.OperationUserSysNumber;
            string OperationUserUniqueName = entity.OperationUserUniqueName;
            entity.SettleUserSysNo = entity.CurrentUserSysNo;
            VerifySettle(entity);
           
            this.VerifySettleItems(entity,SettlementVerifyType.SETTLE);
            //entity.SettleUserSysNo = SystemUserHelper.GetUserSystemNumber(BusinessContext.Current.OperationUserFullName,
            //    BusinessContext.Current.OperationUserSourceDirectoryKey, BusinessContext.Current.OperationUserLoginName,
            //    BusinessContext.Current.CompanyCode);
            //CollVendorSettleDAL dal = collVendorSettleDAL;
            //entity.CreateUserSysNo = 3256;
           

            //统计规则
            string text=string.Empty;
            List<ConsignSettlementRulesInfo> ruleList = CollectionPaymentDA .GetSettleRuleQuantityCount(entity.SysNo.Value);
            foreach (var currentRule in ruleList)
            {
                //规则状态到结算这一步，一定会是已生效或者是未生效，其他的一律视为异常
                if (currentRule.Status != ConsignSettleRuleStatus.Available && currentRule.Status != ConsignSettleRuleStatus.Enable)
                {
                    //规则已经是无效状态，抛出异常
                    text=string.Format("无效的规则:{0}",currentRule.SettleRulesName);
                    throw new BizException(text);
                }
              
                 //超过结算数量
                bool result = false;
                if (currentRule.SettleRulesQuantity.HasValue)
                {
                    result = currentRule.SettleRulesQuantity.Value - (currentRule.SettledQuantity ?? 0) - (currentRule.SubmitSettleQuantity ?? 0) < 0;
                }
                if (result)
                {
                    //超过结算数量，抛出异常
                    throw new BizException(string.Format(GetMessageString("Consign_Check_MoreThanSettleRuleQuantity"), currentRule.SettleRulesName));
                }
            }
            
            using (TransactionScope scope = new TransactionScope())
            {

                #region 结算规则操作
                foreach (var quantityCount in ruleList)
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

                //更改每个item的财务记录为最终结算
                foreach (CollectionPaymentItem item in entity.SettleItems)
                {
                     if (ConsignToAccountLogStatus.Settled == item.ConsignToAccLogInfo.ConsignToAccStatus.Value)
                    {
                        //当前商品的代销转财务记录(记录号:{0})的状态已经为“已结算”状态！
                        throw new BizException(string.Format(GetMessageString("Consign_AccStatus_Settled_Check"), item.ConsignToAccLogInfo.ConsignToAccStatus.Value));
                    }

                    if (item.Cost < 0)
                    {
                        //当前商品(商品系统编号:{0})的结算金额小于0！
                        throw new BizException(string.Format(GetMessageString("Consign_Products_SettleAmt_Check"), item.ConsignToAccLogInfo.ProductSysNo.Value));
                    }
                    decimal foldCost = item.ConsignToAccLogInfo.CreateCost.Value - item.Cost;
                    ConsignSettlementDA.SettleConsignToAccountLog(item.ConsignToAccLogInfo.LogSysNo.Value, item.Cost, foldCost);
                
                }

                //2 修改结算单的状态
                entity.SettleTime = DateTime.Now;
                entity.Status = POCollectionPaymentSettleStatus.Settled;
                entity = this.CollectionPaymentDA .UpdateVendorSettleStatus(entity);

                //发送ESB消息
                EventPublisher.Publish<CollectionPaymentSettlementMessage>(new CollectionPaymentSettlementMessage()
                {
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = entity.SysNo.Value
                });

                scope.Complete();
            }

            
            //结算商品期间过期状态判断
            foreach (var quantityCount in ruleList)
            {
                if (quantityCount.Status == ConsignSettleRuleStatus.Enable)
                {
                    //如果这期间的结算商品已经全部结算完毕，那么这个结算规则也无效,对于是否还有剩余将无效
                    ObjectFactory<IConsignSettlementDA>.Instance.UpdateExistsConsignSettleRuleItemStatus(quantityCount.RuleSysNo.Value);
                }
            }
             //3 生成结算单的付款单
            //调用Invoice接口:生成结算单的付款单

            ExternalDomainBroker.CreatePayItem(new PayItemInfo()
            {
                OrderSysNo = entity.SysNo.Value,
                PayAmt = entity.TotalAmt ,
                OrderType = PayableOrderType.CollectionPayment,
                PayStyle = PayItemStyle.Normal
            });

            ExternalDomainBroker.CreateLog(" Settled VendorSettle "
                                                                                   , BizEntity.Common.BizLogType.POC_VendorSettle_Settle
                                                                                   , entity.SysNo.Value
                                                                                   , entity.CompanyCode);
            //记录规则更新日志
            foreach (var qLog in ruleList)
            {
                ExternalDomainBroker.CreateLog("Settled VendorSettle Update SettleRule"
                                                                          , BizEntity.Common.BizLogType.ConsignSettleRule_Update
                                                                          , qLog.RuleSysNo.Value
                                                                          , qLog.CompanyCode);
            }

            return entity;
        }

        public CollectionPaymentInfo CancelSettled(CollectionPaymentInfo entity)
        {
            
            string OperationIP = entity.OperationIP;
            int? OperationUserSysNumber = entity.OperationUserSysNumber;
            string OperationUserUniqueName = entity.OperationUserUniqueName;
            entity.SettleUserSysNo = entity.CurrentUserSysNo;
            VerifyCancelSettled(entity);


            //entity.SettleUserSysNo = SystemUserHelper.GetUserSystemNumber(BusinessContext.Current.OperationUserFullName,
            //BusinessContext.Current.OperationUserSourceDirectoryKey, BusinessContext.Current.OperationUserLoginName,
            //BusinessContext.Current.CompanyCode);

            //1 将所有以结算的财务单设置为ManualCreated状态
            //VendorSettleDAL dal = new VendorSettleDAL();

            //统计规则
            List<ConsignSettlementRulesInfo> quantityCountList = CollectionPaymentDA.GetSettleRuleQuantityCount(entity.SysNo.Value);
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

                foreach (CollectionPaymentItem item in entity.SettleItems)
                {

                    item.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.ManualCreated;
                    item.ConsignToAccLogInfo.Cost = 0;
                    item.ConsignToAccLogInfo.FoldCost = null;
                    ConsignSettlementDA.CancelSettleConsignToAccountLog(item.ConsignToAccLogInfo.LogSysNo.Value);
                }

                //2 修改结算单的状态
                entity.SettleTime = DateTime.Now;
                entity.Status = POCollectionPaymentSettleStatus.Audited;
                entity.AuditUserSysNo = entity.CurrentUserSysNo;
                entity = this.CollectionPaymentDA.UpdateVendorSettleStatus(entity);

                //发送ESB消息
                EventPublisher.Publish<CollectionPaymentCancelSettlementMessage>(new CollectionPaymentCancelSettlementMessage()
                {
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = entity.SysNo.Value
                });

                scope.Complete();
            }

            ExternalDomainBroker.CreateLog(" CancelSettled VendorSettle "
                                                                          , BizEntity.Common.BizLogType.POC_VendorSettle_CancelSettle
                                                                          , entity.SysNo.Value
                                                                          , entity.CompanyCode);
            //记录规则更新日志
            foreach (var qLog in quantityCountList.Where(p => p.IsNeedUpdateStatus))
            {
                ExternalDomainBroker.CreateLog("CancelSettled VendorSettle Update SettleRule"
                                                                     , BizEntity.Common.BizLogType.ConsignSettleRule_Update
                                                                     , qLog.RuleSysNo.Value
                                                                     , qLog.CompanyCode);
            }
            return entity;
           
        }

        public CollectionPaymentInfo Abandon(CollectionPaymentInfo entity)
        {
            
            string OperationIP = entity.OperationIP;
            int? OperationUserSysNumber = entity.OperationUserSysNumber;
            string OperationUserUniqueName = entity.OperationUserUniqueName;
            entity.CreateUserSysNo = entity.CurrentUserSysNo;
            VerifyAbandon(entity);

            entity.Status = POCollectionPaymentSettleStatus.Abandon;

            //将代销转财务记录的状态修改为初始状态
            foreach (var item in entity.SettleItems)
            {
                item.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.Origin;
            }

            entity.Note = string.Format("当前结算单已被{0}在{1}作废", "SysAdmin", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

            using (TransactionScope scope = new TransactionScope())
            {
                entity =this.CollectionPaymentDA .UpdateVendorSettleStatus(entity);
                //更新代销转财务记录状态
                UpdateConsignToAccLogStatus(entity);

                //发送ESB消息
                EventPublisher.Publish<CollectionPaymentAbandonMessage>(new CollectionPaymentAbandonMessage()
                {
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = entity.SysNo.Value
                });

                scope.Complete();
            }

            entity.OperationIP = OperationIP;
            entity.OperationUserSysNumber = OperationUserSysNumber;
            entity.OperationUserUniqueName = OperationUserUniqueName;

            //CommonService.WriteLog<CollectionPaymentEntity>(entity, " Abandon CollectionPayment ", entity.SysNo.Value.ToString(), (int)LogType.CollectionPayment_Abandon);

            ExternalDomainBroker.CreateLog(" Abandon CollectionPayment "
                                                                         , BizEntity.Common.BizLogType.POC_VendorSettle_Abandon
                                                                         , entity.SysNo.Value
                                                                         , entity.CompanyCode);
            return entity;
        }

        public CollectionPaymentInfo CancelAbandon(CollectionPaymentInfo entity)
        {
            
            string OperationIP = entity.OperationIP;
            int? OperationUserSysNumber = entity.OperationUserSysNumber;
            string OperationUserUniqueName = entity.OperationUserUniqueName;
            entity.CreateUserSysNo = entity.CreateUserSysNo;
            VerifyCancelAbandon(entity);

            entity.Status = POCollectionPaymentSettleStatus.Origin;

            //将代收代付转财务记录的状态修改为人工已建
            foreach (var item in entity.SettleItems)
            {
                item.ConsignToAccLogInfo.ConsignToAccStatus = ConsignToAccountLogStatus.ManualCreated;
            }

            using (TransactionScope scope = new TransactionScope())
            {
              

                entity = CollectionPaymentDA.UpdateVendorSettleStatus(entity);
                //更新代销转财务记录状态
                UpdateConsignToAccLogStatus(entity);

                scope.Complete();
            }

            entity.OperationIP = OperationIP;
            entity.OperationUserSysNumber = OperationUserSysNumber;
            entity.OperationUserUniqueName = OperationUserUniqueName;

            ExternalDomainBroker.CreateLog(" Abandon Cancel CollectionPayment "
                                                                         , BizEntity.Common.BizLogType.POC_VendorSettle_Abandon
                                                                         , entity.SysNo.Value
                                                                         , entity.CompanyCode);
            
            return entity;
        }

        private void UpdateConsignToAccLogStatus(CollectionPaymentInfo entity)
        {
            foreach (var item in entity.SettleItems)
            {

                if (!item.ConsignToAccLogInfo.LogSysNo.HasValue || !item.ConsignToAccLogInfo.ConsignToAccStatus.HasValue)
                {
                    throw new InvalidOperationException("POConsignToAccLogSysNo 或者 ConsignToAccStatus不能为空");
                }
                
            }

            foreach (var item in entity.SettleItems)
            {
                this.ConsignSettlementDA.UpdateConsignToAccountLogStatus(
                    item.POConsignToAccLogSysNo.Value, 
                    item.ConsignToAccLogInfo.ConsignToAccStatus.Value);
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

        public POBatchInfo POBatchInstock(POBatchInfo info)
        {
            POBiz pobiz = new POBiz();
            try
            {
                pobiz.WriteLogPOBatchInstock(info);
                pobiz.UpdatePOInstockAmtAndStatus(info.POSysNo.Value, info.POStatus);
                pobiz.UpdateInvoiceInfo(info.POSysNo.Value);
                return info;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(info.POSysNo.Value+ex.Message,"PO");
                throw;
            }
        }
    }
}
