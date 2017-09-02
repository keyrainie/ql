using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.BizEntity.PO;
using System.Transactions;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 佣金 - BizProcessor
    /// </summary>
    [VersionExport(typeof(CommissionProcessor))]
    public class CommissionProcessor
    {

        #region Fields
        private ICommissionDA m_CommissionDA;

        public ICommissionDA CommissionDA
        {
            get
            {
                if (null == m_CommissionDA)
                {
                    m_CommissionDA = ObjectFactory<ICommissionDA>.Instance;
                }
                return m_CommissionDA;
            }
        }
        #endregion


        /// <summary>
        /// 创建佣金规则
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public virtual CommissionRule CreateCommissionRule(CommissionRule rule)
        {
            CommissionRule newRule = CommissionDA.CreateCommission(rule);
            return newRule;
        }

        /// <summary>
        /// 关闭佣金信息
        /// </summary>
        /// <param name="commissionMaster"></param>
        /// <returns></returns>
        public virtual CommissionMaster CloseCommission(CommissionMaster commissionMaster)
        {
            //预先检查状态是否满足关闭条件：
            string checkMsg = BatchCheckCommissionStatus(new List<BizEntity.PO.CommissionMaster>() { commissionMaster });
            if (!string.IsNullOrEmpty(checkMsg))
            {
                throw new BizException(checkMsg);
            }
            //关闭佣金操作:
            int result = CommissionDA.CloseCommission(commissionMaster);
            if (result != 0)
            {
                //记录关闭日志:
                string logMsg = string.Format(GetMessageString("Commission_CloseCommissionFormat"), string.Empty, DateTime.Now, commissionMaster.SysNo.Value);

                ExternalDomainBroker.CreateLog(logMsg
               , BizEntity.Common.BizLogType.Commission_CloseCommission
               , commissionMaster.SysNo.Value
               , commissionMaster.CompanyCode);
            }
            return commissionMaster;

        }

        /// <summary>
        /// 批量关闭佣金信息
        /// </summary>
        /// <param name="commissionMaster"></param>
        /// <returns></returns>
        public virtual int BatchCloseCommissions(List<CommissionMaster> commissionList)
        {
            int result = 0;

            //预先检查状态是否满足关闭条件:
            string ErrorMsg = BatchCheckCommissionStatus(commissionList);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                throw new BizException(ErrorMsg);
            }
            //进行批量关闭操作:
            foreach (var commission in commissionList)
            {
                string getSysNo = commission.SysNo.Value.ToString();
                if (!string.IsNullOrEmpty(getSysNo))
                {
                    int tempSysNo = int.Parse(getSysNo);
                    CommissionMaster entity = new CommissionMaster() { SysNo = tempSysNo };
                    if (null != CloseCommission(entity))
                    {
                        result++;
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// 检查状态是否满足关闭条件
        /// </summary>
        /// <param name="commissionSysNos"></param>
        /// <returns></returns>
        public virtual string BatchCheckCommissionStatus(List<CommissionMaster> commissionList)
        {
            string errorMsg = string.Empty;
            foreach (var commission in commissionList)
            {
                string commissionSysNo = commission.SysNo.Value.ToString();
                if (!string.IsNullOrEmpty(commissionSysNo))
                {
                    int tempSysNo = int.Parse(commissionSysNo);
                    CommissionMaster commissionMaster = CommissionDA.LoadCommissionMaster(tempSysNo);
                    if (!commissionMaster.SysNo.HasValue)
                    {
                        //佣金账扣单编号不能为空!
                        errorMsg += string.Format(GetMessageString("Commission_SysNoEmpty"), tempSysNo);
                    }
                    if (commissionMaster.Status != VendorCommissionMasterStatus.SET)
                    {
                        //编号为{0}的佣金账扣单不为'已出单'状态，不能关闭!
                        errorMsg += string.Format(GetMessageString("Commission_SetStatusInvalid"), tempSysNo);
                    }

                    if (commissionMaster.SettleStatus != VendorCommissionSettleStatus.Abandon)
                    {
                        //编号为{0}的佣金账扣单对应的财务单据不为'已作废'状态，请先作废相应的付款单!
                        errorMsg += string.Format(GetMessageString("Commission_AbandonStatusInvalid"), tempSysNo);
                    }
                }
            }
            return errorMsg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commissionSysNo"></param>
        /// <returns></returns>
        public virtual CommissionMaster LoadCommissionInfo(int commissionSysNo)
        {
            CommissionMaster returnEntity = new CommissionMaster();
            //1.加载佣金主信息:
            returnEntity = CommissionDA.LoadCommissionMaster(commissionSysNo);
            if (returnEntity == null)
            {
                throw new BizException(GetMessageString("Commission_Error_InvalidItem"));
            }
            //2.加载Item信息:
            returnEntity.ItemList = CommissionDA.LoadCommissionItems(commissionSysNo);
            //3.加载佣金详细信息:
            if (null != returnEntity.ItemList)
            {
                returnEntity.ItemList.ForEach(x =>
                {
                    switch (x.CommissionType)
                    {
                        case VendorCommissionItemType.DEF:
                            x.DetailDeliveryList = CommissionDA.LoadCommissionItemDetails(x.ItemSysNo.Value, x.CommissionType);
                            break;
                        case VendorCommissionItemType.SOC:
                            x.DetailOrderList = CommissionDA.LoadCommissionItemDetails(x.ItemSysNo.Value, x.CommissionType);
                            break;
                        case VendorCommissionItemType.SAC:
                            x.DetailList = CommissionDA.LoadCommissionItemDetails(x.ItemSysNo.Value, x.CommissionType);
                            break;
                    }
                    x.TotalQty = x.TotalQty ?? 0;
                });

            }
            return returnEntity;
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetMessageString(string key)
        {
            return ResouceManager.GetMessageString("PO.Commission", key);
        }

        public CommissionMaster CreateSettleCommission(CommissionMaster req)
        {
            req = GetManualCommissionMaster(req, true);
            if (req.ItemList.Count == 0)
            {
                throw new BizException(GetMessageString("Commission_Error_NoItemData"));
            }
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                req.Status = VendorCommissionMasterStatus.SET;
                CommissionDA.InsertCommissionMaster(req);

                CommissionDA.InsertCommissionItems(req);

                #region Insert Detail
                foreach (var item in req.ItemList)
                {
                    if (item.DetailList != null)
                    {
                        item.DetailList.ForEach(p =>
                        {
                            p.CommissionItemSysNo = item.ItemSysNo;
                            CommissionDA.InsertCommissionDetail(p, req.CompanyCode, VendorCommissionItemType.SAC);
                        });
                    }
                    if (item.DetailOrderList != null)
                    {
                        item.DetailOrderList.ForEach(p =>
                        {
                            p.CommissionItemSysNo = item.ItemSysNo;
                            p.SalePrice = item.OrderCommissionFee;
                            p.PromotionDiscount = 0;
                            if (p.ReferenceType == VendorCommissionReferenceType.RMA)
                            {
                                p.SalePrice = -p.SalePrice;
                            }
                            p.Quantity = null;
                            CommissionDA.InsertCommissionDetail(p, req.CompanyCode, VendorCommissionItemType.SOC);
                        });
                    }
                    if (item.DetailDeliveryList != null)
                    {
                        item.DetailDeliveryList.ForEach(p =>
                        {
                            p.CommissionItemSysNo = item.ItemSysNo;
                            p.SalePrice = item.DeliveryFee;
                            p.PromotionDiscount = 0;
                            p.Quantity = null;
                            CommissionDA.InsertCommissionDetail(p, req.CompanyCode, VendorCommissionItemType.DEF);
                        });
                    }
                }
                #endregion

                #region CreatePayItem
                ExternalDomainBroker.CreatePayItem(new PayItemInfo()
                {
                    OrderSysNo = req.SysNo.Value,
                    PayAmt = -Math.Round(req.TotalAmt ?? 0, 2),
                    OrderType = PayableOrderType.Commission,
                    PayStyle = PayItemStyle.Normal,
                    BatchNumber = 1,
                    Note = GetMessageString("Commission_ManualNote")
                });
                #endregion

                scope.Complete();
            }

            return req;
        }

        public CommissionMaster GetManualCommissionMaster(CommissionMaster master,bool isCreateCommission = false)
        {
            #region valid

            if (!master.MerchantSysNo.HasValue)
            {
                throw new BizException(GetMessageString("Commission_Error_VendorSelect"));
            }
            if (!master.BeginDate.HasValue || !master.EndDate.HasValue)
            {
                throw new BizException(GetMessageString("Commission_Error_DateSelect"));
            }
            if (master.BeginDate.Value > master.EndDate.Value)
            {
                throw new BizException(GetMessageString("Commission_Error_StartMoreThanEnd"));
            }
            DateTime now = DateTime.Now;
            if (master.EndDate.Value > now.Date.AddDays(1))
            {
                throw new BizException(GetMessageString("Commission_Error_EndMoreThanNow"));
            }
            if (master.EndDate.Value > now)
            {
                master.EndDate = now;
            }

            if (isCreateCommission)
            {
                //供应商是否可手工结算
                var vendorInfo = ObjectFactory<VendorProcessor>.Instance.LoadVendorFinanceInfo(master.MerchantSysNo.Value);
                if (vendorInfo == null)
                {
                    throw new BizException(GetMessageString("Commission_Error_InvalidPayPeriod"));
                }
                var manualPayPeriodTypes = AppSettingManager.GetSetting("PO", "ManualSettleCommissionPayType").Split(',');
                if (!manualPayPeriodTypes.ToList().Exists(p => p == (vendorInfo.PayPeriodType.PayTermsNo ?? 0).ToString()))
                {
                    throw new BizException(GetMessageString("Commission_Error_NotManualPayPeriod"));
                }
            }

            #endregion

            #region load data
            master.ItemList = new List<CommissionItem>();

            var details = CommissionDA.QueryCommissionItemDetails(master.MerchantSysNo.Value, master.BeginDate.Value, master.EndDate.Value, master.CompanyCode);
            if (details != null)
            {
                var groupDetails = details.Where(p => p.VendorManufacturerSysNo > 0).GroupBy(p => p.VendorManufacturerSysNo);
                var rules = CommissionDA.QueryCommissionRuleByMerchantSysNo(master.MerchantSysNo.Value);
                if (rules.Count > 0)
                {
                    foreach (var group in groupDetails)
                    {
                        var commissionItem = new CommissionItem();
                        commissionItem.VendorManufacturerSysNo = group.Key;
                        var rule = rules.FirstOrDefault(p => p.VendorManufacturerSysNo == group.Key);
                        if (rule == null || string.IsNullOrEmpty(rule.StagedSaleRuleItemsXml))
                        {
                            commissionItem.RuleSysNo = 0;
                        }
                        else
                        {
                            rule.SaleRuleEntity = SerializationUtility.XmlDeserialize<VendorStagedSaleRuleEntity>(rule.StagedSaleRuleItemsXml);

                            var agent = group.Where(p => !string.IsNullOrEmpty(p.ReferenceSysNo));
                            if (agent == null)
                            {
                                continue;
                            }
                            agent.ForEach(p => p.SalePrice = Math.Round((p.SalePrice ?? 0) - (p.Point ?? 0) / 10m - Math.Abs((p.DiscountAmout ?? 0) / (p.Quantity ?? 0)), 2));
                            commissionItem.TotalSaleAmt = agent.Sum(p => p.SalePrice * (p.Quantity ?? 0)  - Math.Abs(p.PromotionDiscount));
                            commissionItem.RentFee = rule.RentFee;
                            //销售提成不在比较保底
                            //commissionItem.SalesCommissionFee = Math.Max(GetSaleCommissionAmount(commissionItem.TotalSaleAmt, rule.RuleEntity), rule.RuleEntity.MinCommissionAmt);
                            commissionItem.SalesCommissionFee = GetSaleCommissionAmount(commissionItem.TotalSaleAmt ?? 0, rule.SaleRuleEntity);
                            commissionItem.DetailList = agent.ToList();
                            #region 初算提成和运费
                            #region 运费
                            commissionItem.DetailDeliveryList = new List<CommissionItemDetail>();
                            foreach (var item in agent)
                            {
                                if (item.ReferenceType == VendorCommissionReferenceType.SO && !commissionItem.DetailDeliveryList.Exists(p => p.ReferenceSysNo == item.ReferenceSysNo))
                                {
                                    commissionItem.DetailDeliveryList.Add(item);
                                }
                            }
                            commissionItem.DeliveryFee = rule.DeliveryFee;
                            #endregion

                            #region 订单提成
                            commissionItem.DetailOrderList = new List<CommissionItemDetail>();
                            foreach (var item in agent)
                            {
                                if (item.ReferenceType == VendorCommissionReferenceType.SO && !commissionItem.DetailOrderList.Exists(p => p.ReferenceSysNo == item.ReferenceSysNo))
                                {
                                    commissionItem.DetailOrderList.Add(item);
                                }
                            }

                            var autoRmaOrder = new List<CommissionItemDetail>();
                            foreach (var autoRma in agent.Where(p => p.ReferenceType == VendorCommissionReferenceType.RMA && p.HaveAutoRMA))
                            {
                                if (!autoRmaOrder.Exists(p => p.SOSysNo == autoRma.SOSysNo))
                                {
                                    autoRmaOrder.Add(autoRma);
                                }
                            }
                            commissionItem.OrderCommissionFee = rule.OrderCommissionAmt;
                            commissionItem.DetailOrderList.AddRange(autoRmaOrder);
                            #endregion
                            #endregion

                            commissionItem.RuleSysNo = rule.CommissionSysNo;
                            commissionItem.SaleRule = rule.SaleRuleEntity;

                            #region 更新基础信息

                            SetDetailDetailSysNo(commissionItem.DetailList);
                            SetDetailDetailSysNo(commissionItem.DetailOrderList);
                            SetDetailDetailSysNo(commissionItem.DetailDeliveryList);

                            var itemBaseInfo = CommissionDA.QueryVendorManufacturerBySysNo(group.Key);
                            if (itemBaseInfo != null)
                            {
                                commissionItem.ManufacturerName = itemBaseInfo.ManufacturerName;
                                commissionItem.BrandName = itemBaseInfo.BrandName;
                                commissionItem.C3Name = itemBaseInfo.C3Name;
                                commissionItem.C2Name = itemBaseInfo.C2Name;
                            }
                            #endregion
                        }
                        master.ItemList.Add(commissionItem);
                    }
                    #region 细算
                    //如果一个订单有两个或者多个供应商，需获取最大的进行匹配计算，此逻辑为订单提成和运费相关
                    var itemList = master.ItemList;
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].RuleSysNo == 0) continue;
                        #region 运费
                        if (itemList[i].DetailDeliveryList != null)
                        {
                            for (int k = 0; k < itemList[i].DetailDeliveryList.Count; )
                            {
                                var detail = itemList[i].DetailDeliveryList[k];
                                k++;
                                for (int j = i + 1; j < itemList.Count; j++)
                                {
                                    if (itemList[j].DetailDeliveryList != null && itemList[j].DetailDeliveryList.Exists(p => p.ReferenceSysNo == detail.ReferenceSysNo && itemList[i].DeliveryFee.Value <= itemList[j].DeliveryFee.Value))
                                    {
                                        itemList[i].DetailDeliveryList.Remove(detail);
                                        k--;
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 订单提成

                        if (itemList[i].DetailOrderList != null)
                        {
                            for (int k = 0; k < itemList[i].DetailOrderList.Count; )
                            {
                                var detail = itemList[i].DetailOrderList[k];
                                k++;
                                for (int j = i + 1; j < itemList.Count; j++)
                                {
                                    if (itemList[j].DetailOrderList != null && itemList[j].DetailOrderList.Exists(p => p.ReferenceSysNo == detail.ReferenceSysNo && itemList[i].OrderCommissionFee <= itemList[j].OrderCommissionFee))
                                    {
                                        //移除较低价的订单
                                        itemList[i].DetailOrderList.Remove(detail);
                                        k--;
                                        break;
                                    }
                                }
                            }
                        }

                        #endregion

                        itemList[i].DeliveryQty = itemList[i].DetailDeliveryList.Count;
                        itemList[i].TotalDeliveryFee = itemList[i].DeliveryQty * itemList[i].DeliveryFee;

                        itemList[i].OrderQty = itemList[i].DetailOrderList.Count;
                        itemList[i].TotalOrderCommissionFee = (itemList[i].DetailOrderList.Count(p => p.ReferenceType == VendorCommissionReferenceType.SO) - itemList[i].DetailOrderList.Count(p => p.ReferenceType == VendorCommissionReferenceType.RMA)) * itemList[i].OrderCommissionFee;
                    }
                    #endregion
                    //手工结算店租为0
                    //master.RentFee = rules.Sum(p => p.RentFee ?? 0);
                    master.RentFee = 0;
                    master.DeliveryFee = master.ItemList.Sum(p => p.TotalDeliveryFee ?? 0);
                    master.OrderCommissionFee = master.ItemList.Sum(p => p.TotalOrderCommissionFee ?? 0);
                    master.SalesCommissionFee = master.ItemList.Sum(p => p.SalesCommissionFee ?? 0);
                    master.TotalAmt = master.RentFee + master.DeliveryFee + master.OrderCommissionFee + master.SalesCommissionFee;
                }
            }
            #endregion

            return master;
        }

        private void SetDetailDetailSysNo(List<CommissionItemDetail> list)
        {
            if (list != null)
            {
                for (int i = 1; i <= list.Count; i++)
                {
                    list[i - 1].DetailSysNo = i;
                }
            }
        }

        /// <summary>
        /// 计算销售提成
        /// </summary>
        /// <param name="saleAmt"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static decimal GetSaleCommissionAmount(decimal saleAmt, VendorStagedSaleRuleEntity rule)
        {
            var orderRules = from item in rule.StagedSaleRuleItems
                             orderby item.Order descending
                             select item;

            rule.StagedSaleRuleItems.Sort(delegate(VendorStagedSaleRuleEntity.VendorStagedSaleRuleInfo x, VendorStagedSaleRuleEntity.VendorStagedSaleRuleInfo y)
            {
                return y.Order ?? 0 - x.Order ?? 0;
            });

            var commissionList = new decimal[rule.StagedSaleRuleItems.Count];
            for (int i = 0; i < rule.StagedSaleRuleItems.Count; i++)
            {
                var ruleItem = rule.StagedSaleRuleItems[i];

                if (saleAmt > ruleItem.StartAmt)
                {
                    decimal minAmt = ruleItem.EndAmt.HasValue && ruleItem.EndAmt.Value != 0 ? Math.Min(saleAmt, ruleItem.EndAmt.Value) : saleAmt;

                    commissionList[i] = (minAmt - ruleItem.StartAmt ?? 0) * ((Decimal)ruleItem.Percentage / 100m);
                }
            }

            return commissionList.Sum();
        }
    }
}
