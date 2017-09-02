using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.RMA.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;
using ECCentral.Service.EventMessage.Invoice;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice.EBank;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.BizEntity.Invoice.Income;
using ECCentral.BizEntity.SO;
//using ECCentral.Service.ThirdPart.Interface;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(SOIncomeProcessor))]
    public class SOIncomeProcessor
    {
        private ISOIncomeDA m_SOIncomeDA = ObjectFactory<ISOIncomeDA>.Instance;

        /// <summary>
        /// 创建销售收款单
        /// </summary>
        /// <param name="entity">待创建的销售收款单</param>
        /// <returns>创建后的销售收款单</returns>
        public virtual SOIncomeInfo Create(SOIncomeInfo entity)
        {
            PreCheckForCreate(entity);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                entity = m_SOIncomeDA.Create(entity);

                //发送创建收款单Message
                EventPublisher.Publish(new CreateSOIncomeMessage()
                {
                    SOIncomeSysNo = entity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                ts.Complete();
            }

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("SOIncome_Log_Create", ServiceContext.Current.UserSysNo, entity.SysNo)
                , BizEntity.Common.BizLogType.Finance_SOIncome_Add
                , entity.SysNo.Value
                , entity.CompanyCode);

            return entity;
        }

        /// <summary>
        /// 创建收款单前的检查
        /// 主要检查点:是否存在未作废的收款单，如果有则不能创建。
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForCreate(SOIncomeInfo entity)
        {
            List<SOIncomeInfo> result = GetListByCriteria(null, entity.OrderSysNo, entity.OrderType, null);
            if (result.Exists(s => s.Status != SOIncomeStatus.Abandon))
            {
                ThrowBizException("SOIncome_CannotInsert");
            }
        }

        /// <summary>
        /// 确认收款单
        /// </summary>
        public virtual void Confirm(SOIncomeInfo entity)
        {
            Confirm(entity, false);
        }

        /// <summary>
        /// 确认收款单
        /// </summary>
        public virtual void Confirm(SOIncomeInfo entity, bool isRejectAuto)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentException("entity.SysNo");
            }

            SOIncomeInfo soIncomeInfo = LoadBySysNo(entity.SysNo.Value);

            PreCheckForConfirm(soIncomeInfo, isRejectAuto);

            #region Action

            //更新收款单状态为已确认
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                if (soIncomeInfo.OrderType == SOIncomeOrderType.SO)
                {
                    SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(soIncomeInfo.OrderSysNo.Value);
                    if (soBaseInfo == null)
                    {
                        ThrowBizException("SOIncome_SONotExist", soIncomeInfo.OrderSysNo);
                    }
                    if (soBaseInfo.Status == ECCentral.BizEntity.SO.SOStatus.Origin)
                    {
                        ThrowBizException("SOIncome_SOOrigin", soIncomeInfo.OrderSysNo);
                    }
                    //判断是否是货到付款且取得的积分不等于0
                    if (soBaseInfo.PayWhenReceived.Value && soBaseInfo.GainPoint != 0)
                    {
                        //调用Customer接口调整用户积分
                        ExternalDomainBroker.AdjustPoint(new AdjustPointRequest()
                        {
                            CustomerSysNo = soIncomeInfo.CustomerSysNo,
                            SOSysNo = soIncomeInfo.OrderSysNo,
                            Point = soBaseInfo.GainPoint,
                            PointType = (int)AdjustPointType.SalesDiscountPoint, //销售折扣与折让
                            Memo = soIncomeInfo.OrderSysNo.ToString(),
                            Source = "Invoice Domain"
                        });
                    }

                    if (IsAccountPeriodPayType(soBaseInfo.PayTypeSysNo.Value) && soBaseInfo.Status >= 0 && soBaseInfo.ReceivableAmount != 0)
                    {
                        //调用Custormer服务更新客户可用账期额度
                        ExternalDomainBroker.UpdateCustomerCreditLimitByReceipt(soIncomeInfo.CustomerSysNo.Value, soBaseInfo.ReceivableAmount);
                    }

                    //SO单被确认，单据处理进度自动修改为“核销完毕”
                    //TrackingInfo trackingInfo = ObjectFactory<TrackingInfoProcessor>.Instance.GetTrackingInfoByOrderSysNo(soIncomeInfo.OrderSysNo.Value, SOIncomeOrderType.SO);
                    //if (trackingInfo != null)
                    //{
                    //    trackingInfo.Status = TrackingInfoStatus.Confirm; //核销完毕
                    //    ObjectFactory<TrackingInfoProcessor>.Instance.UpdateTrackingInfoStatus(trackingInfo);
                    //}

                    //处理柜台库存模式
                    ProcessERPInventroy(soIncomeInfo);

                }
                else if (soIncomeInfo.OrderType == SOIncomeOrderType.AO || soIncomeInfo.OrderType == SOIncomeOrderType.RO)
                {
                    //银联退款，如货到付款物流拒收则无作为
                    Refund(soIncomeInfo);
                }

                //更新客户累计购买金额
                if (soIncomeInfo.OrderAmt != 0)
                {
                    ExternalDomainBroker.UpdateCustomerTotalMoney(soIncomeInfo.CustomerSysNo.Value, soIncomeInfo.OrderAmt.Value);
                }

                //网关退款等待银行后台回调处理中，不能直接确认
                if (soIncomeInfo.Status != SOIncomeStatus.Processing)
                {
                    soIncomeInfo.Status = SOIncomeStatus.Confirmed;
                }

                //获取确认人用户编号
                soIncomeInfo.ConfirmUserSysNo = ServiceContext.Current.UserSysNo;
                m_SOIncomeDA.UpdateStatus(soIncomeInfo);

                EventPublisher.Publish(new SOIncomeConfirmedMessage()
                {
                    SOIncomeSysNo = entity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                ts.Complete();

            }

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("SOIncome_Log_Confirm", ServiceContext.Current.UserSysNo, soIncomeInfo.SysNo)
                , BizEntity.Common.BizLogType.Finance_SOIncome_Confirm
                , soIncomeInfo.SysNo.GetValueOrDefault()
                , soIncomeInfo.CompanyCode);

            #endregion Action
        }
        /// <summary>
        /// 银联退款
        /// </summary>
        /// <param name="entity"></param>
        private void Refund(SOIncomeInfo entity)
        {
            #region AO,RO

            //SOSysNo
            int sosysNo = entity.OrderSysNo.GetValueOrDefault();
            int refundPayType = -1;
            if (entity.OrderType == SOIncomeOrderType.RO)
            {
                sosysNo = m_SOIncomeDA.GetROSOSysNO(entity.OrderSysNo.GetValueOrDefault(), (int)entity.OrderType);
            }
            if (sosysNo == 0)
            {
                return;
            }

            SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(sosysNo);
            if (soBaseInfo == null)
            {
                //throw new BizException("未找到相关订单");
                ThrowBizException("SOIncome_RelativeOrderNotFound");
            }

            //货到付款则退出
            if (soBaseInfo.PayWhenReceived.Value)
            {
                return;
            }

            PayType payType = ExternalDomainBroker.GetPayTypeBySysNo(soBaseInfo.PayTypeSysNo.GetValueOrDefault());

            //退款方式
            if (entity.OrderType == SOIncomeOrderType.AO)
            {
                refundPayType = m_SOIncomeDA.GetSOIncomeBankInfoRefundPayType(sosysNo);
            }
            else if (entity.OrderType == SOIncomeOrderType.RO)
            {
                refundPayType = m_SOIncomeDA.GetSOIncomeBankInfoRefundPayType(sosysNo);

                int status = m_SOIncomeDA.GetSOIncomeBankInfoStatus(sosysNo);
                if (status != (int)RefundStatus.Audit)
                {
                    //throw new BizException("退款审核未审核通过，不能确认。");
                    ThrowBizException("SOIncome_RefoundNotAuditPassCanntConfirm");
                }
            }
            //泰隆优选支持部分退
            //decimal incomeAmt = m_SOIncomeDA.GetSOIncomeAmt(sosysNo);
            //处理单件退款，金额比订单金额小
            //if (incomeAmt != Math.Abs(entity.IncomeAmt.GetValueOrDefault()))
            //{
            //    if (refundPayType == (int)RefundPayType.NetWorkRefund)
            //    {
            //        throw new BizException("退款金额与订单金额不符时，退款方式不能为网关直退");
            //    }
            //}

            //必须是在线支付
            //if (payType.IsPayWhenRecv == true)
            //{
            //    throw new BizException("该订单为货到付款订单。需要人工线下退款后，再进行强制确认操作。");

            //}
            //是网关直退
            if (refundPayType == (int)RefundPayType.NetWorkRefund)
            {
                RefundResult result = RefundByPayTerm(entity, soBaseInfo);
                if (!result.Result)
                {
                    ThrowBizException("SOIncome_GatwayRefoundFailed", result.Message);
                }
                else
                {
                    entity.ExternalKey = result.ExternalKey;//退款流水号
                    entity.Status = SOIncomeStatus.Confirmed;//等待银行后台回调处理中
                }
            }
            #endregion

        }

        private RefundResult RefundByPayTerm(SOIncomeInfo entity, SOBaseInfo soBaseInfo)
        {
            var result = new RefundResult();
            List<CodeNamePair> chinaPayPayTypeList = CodeNamePairManager.GetList("Invoice", "ChinaPayPayTypeList");
            List<string> payTypeList = chinaPayPayTypeList.Select(p => p.Code).ToList();

            if (payTypeList.Contains(soBaseInfo.PayTypeSysNo.GetValueOrDefault().ToString()))
            {
                var refundEntity = new RefundEntity
                    {
                        SOSysNo = soBaseInfo.SysNo.GetValueOrDefault(),
                        RefundSysNo = entity.OrderSysNo.GetValueOrDefault(),
                        RefundAmt = Math.Abs(entity.IncomeAmt.GetValueOrDefault()),
                        CompanyCode = entity.CompanyCode,
                        OrderDate = soBaseInfo.CreateTime.GetValueOrDefault(),
                        SOAmt = soBaseInfo.SOTotalAmount,
                    };
                if (entity.OrderType.GetValueOrDefault() == SOIncomeOrderType.AO)
                {
                    refundEntity.ProductAmount = Math.Abs(entity.IncomeAmt.GetValueOrDefault())
                                        - soBaseInfo.ShipPrice.GetValueOrDefault()
                                        - soBaseInfo.TariffAmount.GetValueOrDefault();
                    refundEntity.TaxFeeAmount = soBaseInfo.TariffAmount.GetValueOrDefault();
                    refundEntity.ShippingFeeAmount = soBaseInfo.ShipPrice.GetValueOrDefault();
                }
                else
                {
                    refundEntity.ProductAmount = refundEntity.RefundAmt;
                    refundEntity.TaxFeeAmount = 0m;
                    refundEntity.ShippingFeeAmount = 0m;
                }
                //switch (soBaseInfo.PayTypeSysNo)
                //{
                //    case 111:
                //        result = (new EasiPayUtils()).Refund(refundEntity);
                //        break;
                //    case 114:
                //        result = (new TenPayUtils()).Refund(refundEntity);
                //        break;
                //    case 115://支付宝直退--ID待定
                //        result = (new AlipayUtils()).Refund(refundEntity);
                //        break;
                //    case 116://环迅支付直退--ID待定
                //        result = (new IPSPayUtils()).Refund(refundEntity);
                //        break;
                //    default:
                //        result = null;
                //        break;
                //}
                if (soBaseInfo.PayTypeSysNo >= 200 && soBaseInfo.PayTypeSysNo < 300)
                {
                    
                    result = (new IPSPayUtils()).Refund(refundEntity);
                }
                else if (soBaseInfo.PayTypeSysNo == 112)
                {
                    BizLogType RefoundLogType = BizLogType.RMA_Refund_Refund;
                    if (entity.OrderType.GetValueOrDefault() == SOIncomeOrderType.AO)
                    {
                        RefoundLogType = BizLogType.AO_Refund_Refund;
                    }
                    ExternalDomainBroker.CreateOperationLog("系统开始进行支付宝退款!", RefoundLogType, refundEntity.RefundSysNo, refundEntity.CompanyCode);
                    refundEntity.OrderType=entity.OrderType.GetValueOrDefault();
                    result = (new AlipayUtils()).Refund(refundEntity);
                }
                else
                {
                    throw new ApplicationException("未实现此支付方式");
                }
            }
            else
            {
                result.Result = false;
                result.Message = GetMessageString("SOIncome_PayTypeNotSupportGatewayRefound", soBaseInfo.PayTypeSysNo.GetValueOrDefault());
            }
            return result;
        }

        private static void ProcessERPInventroy(SOIncomeInfo soIncomeInfo)
        {
            int SOSysNo = 0;
            if (soIncomeInfo.OrderType == SOIncomeOrderType.SO)
            {
                SOSysNo = soIncomeInfo.OrderSysNo.Value;
            }
            #region 库存逻辑
            var SOItemInfo = ExternalDomainBroker.GetSOItemList(SOSysNo);
            //柜台门店减少网站销售数量
            var ShopInventory = SOItemInfo.Where(p => p.InventoryType == ProductInventoryType.GetShopInventory);
            if (ShopInventory.Count() > 0)
            {
                ERPInventoryAdjustInfo erpAdjustInfo = new ERPInventoryAdjustInfo
                {
                    OrderSysNo = soIncomeInfo.OrderSysNo.Value,
                    OrderType = "SO",
                    AdjustItemList = new List<ERPItemInventoryInfo>(),
                    Memo = "财务确认收款"
                };
                foreach (var item in ShopInventory)
                {
                    ERPItemInventoryInfo adjustItem = new ERPItemInventoryInfo
                    {
                        ProductSysNo = item.ProductSysNo,
                        B2CSalesQuantity = -item.Quantity.Value
                    };

                    erpAdjustInfo.AdjustItemList.Add(adjustItem);

                }
                if (erpAdjustInfo.AdjustItemList.Count > 0)
                {
                    string adjustXml = ECCentral.Service.Utility.SerializationUtility.ToXmlString(erpAdjustInfo);
                    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(string.Format("销售确认扣除ERP【柜台门店】库存：{0}", adjustXml)
                      , BizLogType.Finance_SOIncome_Confirm
                      , soIncomeInfo.OrderSysNo.Value
                      , soIncomeInfo.CompanyCode);
                    //ObjectFactory<IAdjustERPInventory>.Instance.AdjustERPInventory(erpAdjustInfo);

                }
            }
            //var Inventory = SOItemInfo.Where(p => p.InventoryType == ProductInventoryType.Company || p.InventoryType == ProductInventoryType.TwoDoor);
            //if (Inventory.Count() > 0)
            //{
            //    ERPInventoryAdjustInfo erpAdjustInfo = new ERPInventoryAdjustInfo
            //    {
            //        OrderSysNo = soIncomeInfo.OrderSysNo.Value,
            //        OrderType = "SO",
            //        AdjustItemList = new List<ERPItemInventoryInfo>(),
            //        Memo = "财务确认收款"
            //    };
            //    foreach (var item in Inventory)
            //    {
            //        ERPItemInventoryInfo adjustItem = new ERPItemInventoryInfo
            //        {
            //            ProductSysNo = item.ProductSysNo,
            //            HQQuantity = -item.Quantity.Value
            //        };

            //        erpAdjustInfo.AdjustItemList.Add(adjustItem);

            //    }
            //    if (erpAdjustInfo.AdjustItemList.Count > 0)
            //    {
            //        string adjustXml = ECCentral.Service.Utility.SerializationUtility.ToXmlString(erpAdjustInfo);
            //        ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(string.Format("销售确认扣除ERP【总部家电/双开门】库存：{0}", adjustXml)
            //          , BizLogType.Finance_SOIncome_Confirm
            //          , soIncomeInfo.OrderSysNo.Value
            //          , soIncomeInfo.CompanyCode);
            //        ObjectFactory<IAdjustERPInventory>.Instance.AdjustERPInventory(erpAdjustInfo);

            //    }
            //} 
            #endregion
        }

        /// <summary>
        /// 确认预检查
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForConfirm(SOIncomeInfo entity, bool isRejectAuto)
        {
            if (entity.Status != SOIncomeStatus.Origin)
            {
                ThrowBizException("SOIncome_NotOriginConfirm");
            }
            /*
            //货到付款订单确认时判断订单实收金额和支付金额是否一致，如果不一致则确认不通过，提示信息：订单11111实收金额（PayAmount），现金支付：120，银行卡支付：129，预付款：100
            if (entity.IncomeStyle == SOIncomeOrderStyle.Normal && !isRejectAuto)//货到付款
            {
                var DecimalIsNull = new Func<decimal?, decimal>(decimalObject => { return decimalObject == null ? 0 : decimalObject.Value; });
                PosInfo posInfo = m_SOIncomeDA.GetPosInfoByOrderSysNo(entity.OrderSysNo.Value);
                var amount = DecimalIsNull(posInfo.PosBankCard) + DecimalIsNull(posInfo.PosCash) + DecimalIsNull(posInfo.PosPrePay);
                if (entity.IncomeAmt != amount)
                {
                    //var msg = string.Format("订单{0}实收金额{1}和支付金额{5}(现金支付{2},银行支付{3},预付款{4})不一致,无法确认.",
                    //          entity.OrderSysNo.Value.ToString(),
                    //          entity.IncomeAmt.Value.ToString("#0.00"),
                    //          DecimalIsNull(posInfo.PosCash).ToString("#0.00"),
                    //          DecimalIsNull(posInfo.PosBankCard).ToString("#0.00"),
                    //          DecimalIsNull(posInfo.PosPrePay).ToString("#0.00"),
                    //          DecimalIsNull(amount).ToString("#0.00"));

                    ThrowBizException("SOIncome_PayAmountNotEqualPaidIn", entity.OrderSysNo.Value.ToString(),
                              entity.IncomeAmt.Value.ToString("#0.00"),
                              DecimalIsNull(posInfo.PosCash).ToString("#0.00"),
                              DecimalIsNull(posInfo.PosBankCard).ToString("#0.00"),
                              DecimalIsNull(posInfo.PosPrePay).ToString("#0.00"),
                              DecimalIsNull(amount).ToString("#0.00"));
                }
            }
            */
            if (entity.OrderType == SOIncomeOrderType.AO || entity.OrderType == SOIncomeOrderType.RO || entity.OrderType == SOIncomeOrderType.OverPayment)
            {
                List<SOIncomeRefundInfo> list = ObjectFactory<SOIncomeRefundProcessor>.Instance.GetListByCriteria(new SOIncomeRefundInfo()
                {
                    OrderSysNo = entity.OrderSysNo,
                    OrderType = (RefundOrderType)((int)entity.OrderType),
                    Status = RefundStatus.Audit
                });

                if (list == null || list.Count == 0)
                {
                    ThrowBizException("SOIncome_NotBackAudit");
                }
            }
        }


        public void ForcesConfirm(int sysNo)
        {
            var soIncomeInfo = new SOIncomeInfo()
                {
                    SysNo = sysNo,
                    Status = SOIncomeStatus.Confirmed,
                    ConfirmUserSysNo = ServiceContext.Current.UserSysNo
                };
            m_SOIncomeDA.UpdateStatus(soIncomeInfo);
        }


        /// <summary>
        /// 取消确认销售收款单
        /// </summary>
        /// <param name="entity">待取消确认的销售收款单</param>
        public virtual void CancelConfirm(int sysNo)
        {
            SOIncomeInfo soIncomeInfo = LoadBySysNo(sysNo);
            PreCheckForCancelConfirm(soIncomeInfo);

            #region Action

            if (soIncomeInfo.OrderType == SOIncomeOrderType.SO)
            {
                SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(soIncomeInfo.OrderSysNo.Value);
                if (soBaseInfo == null)
                {
                    ThrowBizException("SOIncome_SONotExist", soIncomeInfo.OrderSysNo);
                }
                if (soBaseInfo.PayWhenReceived.Value && soBaseInfo.GainPoint != 0)
                {
                    ExternalDomainBroker.AdjustPoint(new AdjustPointRequest
                    {
                        CustomerSysNo = soIncomeInfo.CustomerSysNo,
                        SOSysNo = soIncomeInfo.OrderSysNo,
                        Point = -soBaseInfo.GainPoint, //获得的积分,SO_Master.PointAmt
                        PointType = (int)AdjustPointType.SalesDiscountPoint, //销售折扣与折让
                        Memo = soIncomeInfo.OrderSysNo.ToString(),
                        Source = "Invoice Domain"
                    });
                }

                if (IsAccountPeriodPayType(soBaseInfo.PayTypeSysNo.Value) && soBaseInfo.Status >= 0 && soBaseInfo.ReceivableAmount != 0)
                {
                    //调用Custormer服务根据应收款更新客户可用账期额度
                    ExternalDomainBroker.UpdateCustomerCreditLimitByReceipt(soIncomeInfo.CustomerSysNo.Value, -soBaseInfo.ReceivableAmount);
                }
            }

            //更新客户累计购买金额
            if (soIncomeInfo.OrderAmt != 0)
            {
                ExternalDomainBroker.UpdateCustomerTotalMoney(soIncomeInfo.CustomerSysNo.Value, -soIncomeInfo.OrderAmt.Value);
            }

            //更新收款单状态为待确认
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                soIncomeInfo.Status = SOIncomeStatus.Origin;
                m_SOIncomeDA.UpdateStatus(soIncomeInfo);


                //发送收款单取消确认Message
                EventPublisher.Publish(new SOIncomeConfirmCanceledMessage()
                {
                    SOIncomeSysNo = soIncomeInfo.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                ts.Complete();
            }

            #endregion Action

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
             GetMessageString("SOIncome_Log_CancelConfirm", ServiceContext.Current.UserSysNo, sysNo)
            , BizEntity.Common.BizLogType.Finance_SOIncome_UnConfirm
            , soIncomeInfo.SysNo.Value
            , soIncomeInfo.CompanyCode);
        }

        /// <summary>
        /// 是否是账期支付方式
        /// </summary>
        /// <param name="payTypeSysNo">要检查的支付方式系统编号</param>
        /// <returns>是否是账期支付</returns>
        protected bool IsAccountPeriodPayType(int payTypeSysNo)
        {
            string cfg = AppSettingManager.GetSetting("Invoice", "AccountPeriodPayTypeSysNo");
            if (!string.IsNullOrEmpty(cfg))
            {
                return cfg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Contains(payTypeSysNo.ToString());
            }
            return false;
        }

        /// <summary>
        /// 取消确认收款单预检查
        /// </summary>
        /// <param name="entity">需要检查的收款单</param>
        protected virtual void PreCheckForCancelConfirm(SOIncomeInfo entity)
        {
            if (entity.Status != SOIncomeStatus.Confirmed)
            {
                ThrowBizException("SOIncome_CannotUncomfirm");
            }
            //TODO:重写此方法，检查第三方订单是否支持取消确认收款单
        }

        /// <summary>
        /// 作废销售收款单
        /// </summary>
        /// <param name="entity">待作废的销售收款单</param>
        public virtual void Abandon(int sysNo)
        {
            SOIncomeInfo entity = LoadBySysNo(sysNo);
            PreCheckForAbandon(entity);

            #region Action

            if (entity.OrderType == SOIncomeOrderType.SO)
            {
                ObjectFactory<NetPayProcessor>.Instance.AbandonBySOSysNo(entity.OrderSysNo.Value);
                ObjectFactory<PostPayProcessor>.Instance.AbandonBySOSysNo(entity.OrderSysNo.Value);
            }

            var postIncomeConfirm = ObjectFactory<PostIncomeProcessor>.Instance.GetConfirmedListBySOSysNo(entity.OrderSysNo.ToString());
            if (postIncomeConfirm.Count > 0)
            {
                foreach (var post in postIncomeConfirm)
                {
                    ObjectFactory<PostIncomeProcessor>.Instance.UpdatePostIncomeConfirmStatus(post.SysNo.Value, PostIncomeConfirmStatus.Related);
                    ObjectFactory<PostIncomeProcessor>.Instance.Handle(new PostIncomeInfo()
                    {
                        SysNo = post.PostIncomeSysNo,
                        HandleStatus = PostIncomeHandleStatus.WaitingHandle
                    }, "");
                }
            }

            //更新收款单状态为已作废
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                entity.Status = SOIncomeStatus.Abandon;
                m_SOIncomeDA.UpdateStatus(entity);

                //发送收款单作废Message
                EventPublisher.Publish(new SOIncomeAbandonedMessage()
                {
                    SOIncomeSysNo = entity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                ts.Complete();
            }

            #endregion Action

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
             GetMessageString("SOIncome_Log_Abandon", ServiceContext.Current.UserSysNo, sysNo)
             , BizEntity.Common.BizLogType.Finance_SOIncome_Abandon
             , entity.SysNo.Value
             , entity.CompanyCode);
        }

        /// <summary>
        /// 作废预检查
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForAbandon(SOIncomeInfo entity)
        {
            if (entity.Status != SOIncomeStatus.Origin)
            {
                ThrowBizException("SOIncome_NotOriginAbandon");
            }

            //if (entity.IncomeStyle != SOIncomeOrderStyle.Advanced)
            //{
            //    ThrowBizException("SOIncome_NotAdvanceAbandon");
            //}

            //只有退款审核拒绝后才可以进行Abandon操作
            if (entity.OrderType == SOIncomeOrderType.AO || entity.OrderType == SOIncomeOrderType.RO
                || entity.OrderType == SOIncomeOrderType.OverPayment)
            {
                List<SOIncomeRefundInfo> list = null;

                list = ObjectFactory<SOIncomeRefundProcessor>.Instance.GetListByCriteria(new SOIncomeRefundInfo()
                {
                    OrderSysNo = entity.OrderSysNo,
                    OrderType = (RefundOrderType)((int)entity.OrderType),
                    Status = RefundStatus.Abandon
                });

                if (list == null || list.Count == 0)
                {
                    ThrowBizException("SOIncome_BackInfoAbandon");
                }
            }
        }

        /// <summary>
        /// 根据系统编号获取销售收款单
        /// </summary>
        /// <param name="sysNo">销售收款单系统编号</param>
        /// <returns>销售收款单</returns>
        public virtual SOIncomeInfo LoadBySysNo(int sysNo)
        {
            var entity = m_SOIncomeDA.LoadBySysNo(sysNo);
            if (entity == null)
            {
                ThrowBizException("SOIncome_RecordNotExist", sysNo);
            }
            return entity;
        }

        /// <summary>
        /// 根据系统编号列表取得收款单列表
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public virtual List<SOIncomeInfo> GetListBySOSysNoList(List<int> soSysNoList)
        {
            return m_SOIncomeDA.GetListBySOSysNoList(soSysNoList);
        }

        /// <summary>
        /// 根据查询条件取得收款单列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual List<SOIncomeInfo> GetListByCriteria(int? sysNo, int? orderSysNo, SOIncomeOrderType? orderType, List<SOIncomeStatus> soIncomeStatus)
        {
            return m_SOIncomeDA.GetListByCriteria(sysNo, orderSysNo, orderType, soIncomeStatus);
        }

        /// <summary>
        /// 更新主单收款单金额
        /// </summary>
        /// <param name="soBaseInfo"></param>
        public virtual void UpdateMasterSOAmt(SOBaseInfo soBaseInfo)
        {
            m_SOIncomeDA.UpdateMasterSOAmt(soBaseInfo);
        }

        /// <summary>
        /// 创建负收款单
        /// </summary>
        /// <param name="refundInfo">退款信息</param>
        /// <param name="payAmt">实收金额，如果实收金额大于0，则优先退现金</param>
        /// <returns></returns>
        public virtual SOIncomeInfo CreateNegative(SOIncomeRefundInfo refundInfo)
        {
            SOIncomeInfo entity = new SOIncomeInfo();

            //优先退现金(礼品卡项目调整).
            if (refundInfo.PayAmount > 0)
            {
                entity.IncomeAmt = -refundInfo.RefundCashAmt;
                entity.PrepayAmt = 0;
            }
            else
            {
                entity.IncomeAmt = 0;
                entity.PrepayAmt = -refundInfo.RefundCashAmt;
            }
            entity.OrderAmt = -refundInfo.RefundCashAmt;
            entity.OrderSysNo = refundInfo.OrderSysNo;
            entity.IncomeStyle = SOIncomeOrderStyle.Advanced;
            entity.OrderType = SOIncomeOrderType.OverPayment;
            entity.Note = GetMessageString("SOIncome_NegativeSOIncomeNote"); //用户多付款，创建财务负收款单。

            entity.ReferenceID = String.Empty;
            entity.Status = SOIncomeStatus.Origin;
            entity.PointPay = 0;
            entity.GiftCardPayAmt = 0;
            entity.PayAmount = entity.IncomeAmt;
            entity.CompanyCode = refundInfo.CompanyCode;

            return m_SOIncomeDA.Create(entity);
        }

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns></returns>
        public virtual SOIncomeInfo GetValid(int orderSysNo, SOIncomeOrderType orderType)
        {
            return m_SOIncomeDA.GetValid(orderSysNo, orderType);
        }

        /// <summary>
        /// 根据单据类型和单据编号取得已经确认的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns></returns>
        public virtual SOIncomeInfo GetConfirmed(int orderSysNo, SOIncomeOrderType orderType)
        {
            return m_SOIncomeDA.GetConfirmed(orderSysNo, orderType);
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="sysNo">收款单系统编号</param>
        /// <param name="referenceID">凭证号</param>
        public virtual void SetReferenceID(int sysNo, string referenceID)
        {
            var entity = LoadBySysNo(sysNo);
            entity.ReferenceID = referenceID;

            m_SOIncomeDA.SetReferenceID(sysNo, referenceID);
        }

        /// <summary>
        /// 设置收款单实收金额
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="incomeAmt"></param>
        public virtual void SetIncomeAmount(int sysNo, decimal incomeAmt)
        {
            var entity = LoadBySysNo(sysNo);
            if (entity.Status != SOIncomeStatus.Origin)
            {
                ThrowBizException("SOIncome_SetIncomeAmount_InvalidStatus");
            }
            if (entity.IncomeStyle != SOIncomeOrderStyle.Normal)
            {
                ThrowBizException("SOIncome_SetIncomeAmount_InvalidOrderStyle");
            }

            m_SOIncomeDA.SetIncomeAmount(sysNo, incomeAmt);

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
             GetMessageString("SOIncome_Log_SetIncomeAmount", ServiceContext.Current.UserSysNo, sysNo)
             , BizEntity.Common.BizLogType.Finance_SOIncome_UpdateIncomeAmt
             , sysNo
             , entity.CompanyCode);
        }

        /// <summary>
        /// 更新收款单状态为已处理，用于收款单自动确认时更新母单的收款状态为已处理
        /// </summary>
        /// <param name="entityList">收款单列表</param>
        public virtual void UpdateToProcessedStatus(List<SOIncomeInfo> entityList)
        {
            m_SOIncomeDA.UpdateToProcessedStatus(entityList);
        }

        #region [For SO Domain]

        /// <summary>
        /// 为PendingList生成销售收款单时需要调用，用来更新收款单单据金额
        /// </summary>
        /// <param name="soIncomeSysNo">销售-收款单系统编号</param>
        /// <param name="orderAmt">单据金额</param>
        public virtual void UpdateOrderAmtForSO(int sysNo, decimal orderAmt)
        {
            m_SOIncomeDA.UpdateOrderAmtForSO(sysNo, orderAmt);
        }

        #region [Split For SO]

        /// <summary>
        /// 拆分销售收款单
        /// </summary>
        /// <param name="master">主订单</param>
        /// <param name="subList">子订单列表</param>
        public virtual void CreateSplitPayForSO(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            PreCheckForSplitOrAbandonSplit(master, subList);
            var so = ExternalDomainBroker.GetSOBaseInfo(master.SysNo.Value);

            //判断订单的支付方式是否是电汇-银行邮局汇款
            if (!ObjectFactory<PostPayProcessor>.Instance.IsBankOrPostPayType(so.PayTypeSysNo.Value))
            {
                SplitNetPay(master, subList);
            }
            else
            {
                SplitPostPay(master, subList);
            }
        }

        /// <summary>
        /// 拆分PostPay
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="subSOList"></param>
        protected virtual void SplitPostPay(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            var postpay = ObjectFactory<PostPayProcessor>.Instance.GetListBySOSysNoAndStatus(master.SysNo.Value, PostPayStatus.Yes);
            var soincome = GetListByCriteria(null, master.SysNo, SOIncomeOrderType.SO, new List<SOIncomeStatus> { SOIncomeStatus.Origin });

            if ((postpay == null || postpay.Count == 0) || (soincome == null || soincome.Count == 0))
            {
                ThrowBizException("SOIncome_NotExistOriginPostPay", master.SysNo);
            }

            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, option))
            {
                var masterPostpay = postpay[0];
                //更新Postpay母单为已拆分
                masterPostpay.Status = PostPayStatus.Splited;
                ObjectFactory<PostPayProcessor>.Instance.SplitForSO(masterPostpay);

                var ppd = ObjectFactory<IPostPayDA>.Instance;
                foreach (var item in subList)
                {
                    PostPayInfo ppe = DeepCopy<PostPayInfo>(masterPostpay);
                    ppe.MasterSoSysNo = master.SysNo;
                    ppe.OrderAmt = item.SOTotalAmount;
                    ppe.PrepayAmt = item.PrepayAmount;
                    ppe.PointPay = item.PointPay;
                    ppe.GiftCardPay = item.GiftCardPay;
                    ppe.PayAmount = item.OriginalReceivableAmount;
                    ppe.Status = PostPayStatus.Yes;
                    ppe.SOSysNo = item.SysNo;
                    //写postpay
                    ppd.Create(ppe);
                }

                var masterSoIncome = soincome[0];
                masterSoIncome.Status = SOIncomeStatus.Splited;
                UpdateForSplit(masterSoIncome);
                foreach (var item in subList)
                {
                    SOIncomeInfo se = DeepCopy<SOIncomeInfo>(masterSoIncome);
                    se.MasterSoSysNo = master.SysNo;
                    se.OrderAmt = item.SOTotalAmount;
                    se.PrepayAmt = item.PrepayAmount;
                    se.PointPay = item.PointPay;
                    se.GiftCardPayAmt = item.GiftCardPay;
                    se.PayAmount = item.OriginalReceivableAmount;
                    se.Status = SOIncomeStatus.Origin;
                    se.OrderSysNo = item.SysNo;
                    se.IncomeAmt = item.OriginalReceivableAmount;
                    //写soicome
                    m_SOIncomeDA.Create(se);
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// 拆分NetPay
        /// </summary>
        /// <param name="master"></param>
        /// <param name="subSOList"></param>
        protected virtual void SplitNetPay(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            var netpay = ObjectFactory<NetPayProcessor>.Instance.GetListByCriteria(new NetPayInfo
            {
                SOSysNo = master.SysNo,
                Status = NetPayStatus.Approved
            });
            var soincome = GetListByCriteria(null, master.SysNo, SOIncomeOrderType.SO
                                            , new List<SOIncomeStatus> { SOIncomeStatus.Origin, SOIncomeStatus.Confirmed });
            if ((netpay == null || netpay.Count == 0) || (soincome == null || soincome.Count == 0))
            {
                ThrowBizException("SOIncome_NotExistOriginNetPay", master.SysNo);
            }

            //处理第三方平台支付
            System.Collections.Hashtable hash = DealThirdPartForSplit(master, subList, netpay[0]);

            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, option))
            {
                var masterNetpay = netpay[0];
                masterNetpay.Status = NetPayStatus.Splited;
                //更新netpay母单为已拆分
                ObjectFactory<NetPayProcessor>.Instance.SplitForSO(masterNetpay);

                var nd = ObjectFactory<INetPayDA>.Instance;
                //写入子单到netpay
                foreach (var item in subList)
                {
                    NetPayInfo ne = masterNetpay;
                    ne.MasterSoSysNo = master.SysNo.Value;
                    ne.OrderAmt = item.SOTotalAmount;
                    ne.PrePayAmt = item.PrepayAmount;
                    ne.PointPay = item.PointPay;
                    ne.GiftCardPayAmt = item.GiftCardPay;
                    ne.PayAmount = item.OriginalReceivableAmount;
                    ne.Status = NetPayStatus.Approved;
                    ne.SOSysNo = item.SysNo;
                    var hashValue = hash == null ? null : hash[item.SysNo];
                    if (hashValue != null)
                    {
                        ne.ExternalKey = hash[item.SysNo].ToString();
                    }
                    //写netpay
                    nd.Create(ne);
                }

                //SOIncome拆分消息

                SOIncomeSplitedMessage msg = new SOIncomeSplitedMessage();

                var masterSoIncome = soincome[0];
                masterSoIncome.Status = SOIncomeStatus.Splited;
                UpdateForSplit(masterSoIncome);
                foreach (var item in subList)
                {
                    SOIncomeInfo se = DeepCopy<SOIncomeInfo>(masterSoIncome);
                    se.MasterSoSysNo = master.SysNo;
                    se.OrderAmt = item.SOTotalAmount;
                    se.PrepayAmt = item.PrepayAmount;
                    se.PointPay = item.PointPay;
                    se.GiftCardPayAmt = item.GiftCardPay;
                    se.PayAmount = item.OriginalReceivableAmount;
                    se.Status = SOIncomeStatus.Origin;
                    se.OrderSysNo = item.SysNo;
                    se.IncomeAmt = item.OriginalReceivableAmount;
                    //写soicome
                    m_SOIncomeDA.Create(se);

                    msg.SubSOIncomeSysNoList.Add(se.SysNo.Value);
                }
                //发送拆分收款单Message
                EventPublisher.Publish(msg);

                ts.Complete();

            }
        }

        /// <summary>
        /// 拆分订单时更新收款单信息
        /// </summary>
        /// <param name="masterSoIncome"></param>
        protected virtual void UpdateForSplit(SOIncomeInfo masterSoIncome)
        {
            m_SOIncomeDA.UpdateStatusSplitForSO(masterSoIncome);
        }

        /// <summary>
        /// 处理第三方平台支付，返回键为订单系统编号，值为一个字符串的Hashtable，键值对中的字符串将用来设置NetPay的ExternalKey
        /// 中蛋逻辑需要处理平安万里通积分支付（PayTypeSysNo=48）
        /// </summary>
        /// <param name="master"></param>
        /// <param name="subList"></param>
        /// <param name="netpayInfo"></param>
        /// <returns></returns>
        protected System.Collections.Hashtable DealThirdPartForSplit(SOBaseInfo master, List<SOBaseInfo> subList, NetPayInfo netpayInfo)
        {
            return new System.Collections.Hashtable(0);
        }

        #endregion [Split For SO]

        #region [Abandon Split For SO]

        public void AbandonSplitPayForSO(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            PreCheckForSplitOrAbandonSplit(master, subList);
            var so = ExternalDomainBroker.GetSOBaseInfo(master.SysNo.Value);

            //处理第三方平台支付
            var externalKey = DealThirdPartForAbandon(master, subList);

            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, option))
            {
                if (!ObjectFactory<PostPayProcessor>.Instance.IsBankOrPostPayType(so.PayTypeSysNo.Value))
                {
                    ObjectFactory<NetPayProcessor>.Instance.AbandonSplitForSO(master, subList, externalKey);
                }
                else
                {
                    ObjectFactory<PostPayProcessor>.Instance.AbandonSplitForSO(master, subList);
                    ObjectFactory<PostIncomeProcessor>.Instance.AbandonSplitForSO(master, subList);
                }
                m_SOIncomeDA.AbandonSplitForSO(master, subList);
                ts.Complete();
            }
        }

        /// <summary>
        /// 处理第三方平台支付，返回一个字符串，作为母单NetPay的ExternalKey
        /// 中蛋逻辑需要处理平安万里通积分支付（PayTypeSysNo=48）
        /// </summary>
        /// <param name="master"></param>
        /// <param name="subList"></param>
        /// <returns></returns>
        protected virtual string DealThirdPartForAbandon(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            return "";
        }

        #endregion [Abandon Split For SO]

        /// <summary>
        /// 对象深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        protected virtual T DeepCopy<T>(T t)
        {
            if (t == null)
            {
                return default(T);
            }
            return SerializationUtility.DeepClone<T>(t);
        }

        /// <summary>
        /// 拆分或取消拆分预检查
        /// </summary>
        /// <param name="master"></param>
        /// <param name="subList"></param>
        protected virtual void PreCheckForSplitOrAbandonSplit(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            if (master == null || subList == null)
            {
                ThrowBizException("SOIncome_Split_CheckSONull");
            }
            if (!master.Status.HasValue || master.Status != SOStatus.Split)
            {
                ThrowBizException("SOIncome_Split_CheckMasterSOStatus");
            }
            if (!master.SysNo.HasValue || master.SysNo == 0)
            {
                ThrowBizException("SOIncome_Split_CheckMasterSOSysNo");
            }
            subList.ForEach(s => s.Validate(p => p.SysNo.HasValue && p.SysNo.Value != 0,
                () =>
                {
                    ThrowBizException("SOIncome_Split_CheckSubSOSysNo");
                })
             );
        }

        #endregion [For SO Domain]

        #region Helper Methods

        internal void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        internal string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.SOIncome, msgKeyName), args);
        }

        #endregion Helper Methods

        /// <summary>
        /// 手动网关退款
        /// </summary>
        /// <param name="sysNo">单据编号</param>
        public void ManualBankRefund(string sysNo)
        {
            var entity = LoadBySysNo(Convert.ToInt32(sysNo));

            if (entity == null || (entity.OrderType != SOIncomeOrderType.AO && entity.OrderType != SOIncomeOrderType.RO))
            {
                ThrowBizException("SOIncome_NoAoROGatway");
            }

            int sosysNo = entity.OrderSysNo.GetValueOrDefault();
            int refundPayType = -1;

            if (entity.OrderType == SOIncomeOrderType.RO)
            {
                sosysNo = m_SOIncomeDA.GetROSOSysNO(entity.OrderSysNo.GetValueOrDefault(), (int)entity.OrderType);
            }

            if (sosysNo == 0)
            {
                return;
            }

            SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(sosysNo);
            if (soBaseInfo == null)
            {
                ThrowBizException("SOIncome_RelativeOrderNotFound");
            }

            PayType payType = ExternalDomainBroker.GetPayTypeBySysNo(soBaseInfo.PayTypeSysNo.GetValueOrDefault());

            //退款方式
            if (entity.OrderType == SOIncomeOrderType.AO)
            {
                refundPayType = m_SOIncomeDA.GetSOIncomeBankInfoRefundPayType(sosysNo);
            }
            else if (entity.OrderType == SOIncomeOrderType.RO)
            {
                refundPayType = m_SOIncomeDA.GetSOIncomeBankInfoRefundPayType(sosysNo);
            }

            if (refundPayType != (int)RefundPayType.NetWorkRefund)
            {
                ThrowBizException("SOIncome_GatwayProcessingFailedRefundTypeError");
            }

            if (entity.Status != SOIncomeStatus.ProcessingFailed)
            {
                ThrowBizException("SOIncome_GatwayProcessingFailed");
            }

            RefundResult result = RefundByPayTerm(entity, soBaseInfo);
            if (!result.Result)
            {
                ThrowBizException("SOIncome_GatwayRefoundFailed", result.Message);
            }
            else
            {

                entity.ExternalKey = result.ExternalKey;
                if (result.IsSync)
                {
                    entity.Status = SOIncomeStatus.Confirmed;//直退成功
                }
                else
                {
                    entity.Status = SOIncomeStatus.Processing;//等待银行后台回调处理中
                }

                m_SOIncomeDA.UpdateStatus(entity);
            }
        }

        /// <summary>
        /// 网关订单查询
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public TransactionQueryBill QueryBill(string soSysNo)
        {
            TransactionQueryBill result = new TransactionQueryBill();
            SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(int.Parse(soSysNo));
            switch (soBaseInfo.PayTypeSysNo)
            {
                case 111:
                    result = (new EasiPayUtils()).QueryBill(soSysNo);
                    break;
                case 114:
                    result = (new TenPayUtils()).QueryBill(soSysNo);
                    break;
                default:
                    result.Message = "支付方式错误。";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 批量确认运费收入
        /// </summary>
        /// <param name="info"></param>
        public void SOFreightConfirm(int sysNo)
        {
            SOFreightStatDetail detail = m_SOIncomeDA.LoadSOFreightConfirmBySysNo(sysNo);
            if (detail == null || detail.SOFreightConfirm != CheckStatus.Pending)
            {
                throw new BizException("状态不是待确认的单据不能审核。");
            }
            m_SOIncomeDA.SOFreightConfirm(detail);
        }

        /// <summary>
        /// 批量确认运费支出
        /// </summary>
        /// <param name="info"></param>
        public void RealFreightConfirm(int sysNo)
        {
            SOFreightStatDetail detail = m_SOIncomeDA.LoadSOFreightConfirmBySysNo(sysNo);
            if (detail == null || detail.RealFreightConfirm != CheckStatus.Pending)
            {
                throw new BizException("状态不是待支出的单据不能审核。");
            }
            m_SOIncomeDA.RealFreightConfirm(detail);
        }
        /// <summary>
        /// 获取所有需要对账的关务对接相关信息
        /// </summary>
        /// <returns></returns>
        public List<VendorCustomsInfo> QueryVendorCustomsInfo()
        {
            return m_SOIncomeDA.QueryVendorCustomsInfo();
        }

        public List<int> GetSysNoListByRefund()
        {
            return m_SOIncomeDA.GetSysNoListByRefund();
        }
    }
}