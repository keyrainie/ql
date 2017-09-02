using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(PostPayProcessor))]
    public class PostPayProcessor
    {
        private IPostPayDA m_PostPayDA = ObjectFactory<IPostPayDA>.Instance;

        /// <summary>
        /// 创建电汇邮局付款记录信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual PostPayInfo Create(PostPayInfo postpayEntity, SOIncomeRefundInfo refundEntity, bool isForceCheck)
        {
            var soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(postpayEntity.SOSysNo.Value);

            PreCheckForCreate(postpayEntity, refundEntity, soBaseInfo, isForceCheck);

            SOIncomeInfo soIncomeInfo;

            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadCommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                //Step1:添加收款数据
                soIncomeInfo = CreateSOIncomeInfo(postpayEntity, soBaseInfo);

                bool isOverpay = postpayEntity.PayAmount > soBaseInfo.ReceivableAmount;
                if (isOverpay && isForceCheck)
                {
                    //Step2:创建退款数据
                    refundEntity.CompanyCode = soBaseInfo.CompanyCode;
                    refundEntity.SOIncomeSysNo = soIncomeInfo.SysNo;
                    CreateRefundInfoForForceCheck(postpayEntity, refundEntity);
                }

                //Step3:添加PostPay
                postpayEntity.Status = PostPayStatus.Yes;
                postpayEntity.Note = "Insert--Record--Valid";
                postpayEntity.CompanyCode = soBaseInfo.CompanyCode;
                postpayEntity = m_PostPayDA.Create(postpayEntity);

                var postIncomeConfirm = ObjectFactory<PostIncomeProcessor>.Instance.GetConfirmedListBySOSysNo(postpayEntity.SOSysNo.ToString());
                if (postIncomeConfirm != null && postIncomeConfirm.Count > 0)
                {
                    ObjectFactory<PostIncomeProcessor>.Instance.UpdatePostIncomeConfirmStatus(postIncomeConfirm[0].SysNo.Value, PostIncomeConfirmStatus.Audit);
                }
                scope.Complete();
            }
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("PostPay_Log_Create", ServiceContext.Current.UserSysNo, postpayEntity.SysNo)
                , BizLogType.Finance_AdvancePay_Check
                , postpayEntity.SysNo.Value
                , postpayEntity.CompanyCode);

            return postpayEntity;
        }

        /// <summary>
        /// 为强制核收创建退款信息
        /// </summary>
        /// <param name="postpayEntity"></param>
        /// <param name="refundEntity"></param>
        /// <param name="soIncomeSysNo"></param>
        private void CreateRefundInfoForForceCheck(PostPayInfo postpayEntity, SOIncomeRefundInfo refundEntity)
        {
            refundEntity.OrderSysNo = postpayEntity.SOSysNo.Value;
            refundEntity.OrderType = RefundOrderType.OverPayment;
            refundEntity.SOSysNo = postpayEntity.SOSysNo.Value;

            //如果是现金退款，则直接审核通过
            if (refundEntity.RefundPayType.Value == RefundPayType.CashRefund)
            {
                refundEntity.Status = RefundStatus.Audit;
            }
            else
            {
                refundEntity.Status = RefundStatus.Origin;
            }

            if (refundEntity.RefundPayType == RefundPayType.TransferPointRefund)
            {
                refundEntity.RefundPoint = int.Parse(refundEntity.RefundCashAmt.Value.ToString());
                refundEntity.RefundCashAmt = 0;
            }
            else
            {
                refundEntity.RefundCashAmt = refundEntity.RefundCashAmt ?? 0;
                refundEntity.RefundPoint = 0;
            }

            //客户多付款
            refundEntity.RefundReason = 5;
            //非物流拒收
            refundEntity.HaveAutoRMA = false;

            var list = ObjectFactory<SOIncomeRefundProcessor>.Instance.GetListByCriteria(new SOIncomeRefundInfo
            {
                OrderSysNo = postpayEntity.SOSysNo,
                OrderType = RefundOrderType.OverPayment
            });
            if (list != null && list.Count > 0)
            {
                refundEntity.SysNo = list[0].SysNo;
                if (refundEntity.OrderType == RefundOrderType.OverPayment)
                {
                    //创建财务负收款单
                    refundEntity.PayAmount = postpayEntity.PayAmount;
                    ObjectFactory<SOIncomeProcessor>.Instance.CreateNegative(refundEntity);
                }
                ObjectFactory<SOIncomeRefundProcessor>.Instance.Update(refundEntity);
            }
            else
            {
                refundEntity.PayAmount = postpayEntity.PayAmount;
                ObjectFactory<SOIncomeRefundProcessor>.Instance.Create(refundEntity);
            }
        }

        /// <summary>
        /// 创建前预检查
        /// TODO：重写该方法添加额外的检查。中蛋实施时需要检查[货到付款(OZZO奥硕物流)和现金支付才可以选择现金退款]
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="refundInfo"></param>
        /// <param name="isForceCheck"></param>
        protected virtual void PreCheckForCreate(PostPayInfo entity, SOIncomeRefundInfo refundInfo, SOBaseInfo soBaseInfo, bool isForceCheck)
        {
            if (soBaseInfo.PayTypeSysNo != entity.PayTypeSysNo.Value)
            {
                ThrowBizException("PostPay_PayTypeNotMachSO");
            }

            decimal soEndMoney = soBaseInfo.ReceivableAmount;
            var postIncomeConfirmList = ObjectFactory<PostIncomeProcessor>.Instance.GetConfirmedListBySOSysNo(entity.SOSysNo.ToString());

            if (postIncomeConfirmList.Count == 0)
            {
                ThrowBizException("PostPay_NotExistSOValidPostIncome", entity.SOSysNo.ToString());
            }

            var postIncome = ObjectFactory<PostIncomeProcessor>.Instance.LoadBySysNo(postIncomeConfirmList[0].PostIncomeSysNo.Value);
            if (postIncome == null || postIncome.HandleStatus != PostIncomeHandleStatus.Handled)
            {
                ThrowBizException("PostPay_StatusNotMatchHandled");
            }

            postIncomeConfirmList = postIncome.ConfirmInfoList;
            var relatedConfirmList = postIncomeConfirmList.Where(w => w.Status == PostIncomeConfirmStatus.Related).Select(s => s);

            if (isForceCheck && relatedConfirmList.Count() > 1)
            {
                ThrowBizException("PostPay_HasAnotherUnConfirmedSO");
            }
            //强制核收时，实收金额必须与剩余金额一致
            if (isForceCheck && relatedConfirmList.Count() == 1)
            {
                if (entity.RemainAmt != entity.PayAmount)
                {
                    ThrowBizException("PostPay_ForcePayNotMatchAmt");
                }
            }
            //CS核收订单的退款金额,负值
            var confirmOrderList = postIncomeConfirmList.Where(w => w.Status == PostIncomeConfirmStatus.Audit).ToList();
            //取得已确认的多付金额
            decimal refundamt = GetRefundAmtByConfirmedSOSysNoList(confirmOrderList.Select(s => s.ConfirmedSoSysNo.Value).ToList());

            if (soEndMoney > entity.PayAmount)
            {
                ThrowBizException("PostPay_IncomeAmtLessThanSOAmt");
            }
            if (soEndMoney == entity.PayAmount && isForceCheck)
            {
                ThrowBizException("PostPay_CannotForcePay");
            }
            if (soEndMoney < entity.PayAmount && !isForceCheck)
            {
                ThrowBizException("PostPay_IncomeAmtMoreThanSOAmt");
            }
            if (entity.RemainAmt < entity.PayAmount)
            {
                ThrowBizException("PostPay_IncomeAmtMoreThanRemainAmt");
            }
            if (isForceCheck && refundInfo.RefundCashAmt <= 0)
            {
                ThrowBizException("PostPay_RefundCashAmtRequired");
            }
            if (isForceCheck && Math.Abs(refundInfo.ToleranceAmt ?? 0) > 0.1M)
            {
                ThrowBizException("PostPay_ToleranceAmtNotCorrect");
            }

            //减去核收订单的退款金额
            decimal incomeAmt = postIncome.IncomeAmt.Value + refundamt;
            decimal totalAmt = soEndMoney;

            if (soEndMoney > incomeAmt || incomeAmt < 0)
            {
                ThrowBizException("PostPay_IncomeAmtMoreThanPostIncomeAmt");
            }
            foreach (var order in confirmOrderList)
            {
                totalAmt += GetTotalAmt(order.ConfirmedSoSysNo.Value);
            }

            if (totalAmt > incomeAmt)
            {
                ThrowBizException("PostPay_TotalAmtMoreThanPostIncomeAmt");
            }

            if (isForceCheck)
            {
                var list = ObjectFactory<SOIncomeProcessor>.Instance.GetListByCriteria(entity.SOSysNo,null,SOIncomeOrderType.OverPayment,null);
                if (list != null)
                {
                    var result = list.FindAll(p => p.Status != SOIncomeStatus.Abandon);
                    if (result != null && result.Count > 0)
                    {
                        ThrowBizException("PostPay_MOExists");
                    }
                }
            }
        }

        /// <summary>
        /// 取得已确认的多付金额
        /// </summary>
        /// <param name="soSysNoList">订单系统编号列表</param>
        /// <returns></returns>
        public virtual decimal GetRefundAmtByConfirmedSOSysNoList(List<int> soSysNoList)
        {
            if (soSysNoList == null || soSysNoList.Count == 0)
            {
                return 0;
            }
            var soSysNoStr = string.Join(",", soSysNoList.Distinct());
            return m_PostPayDA.GetRefundAmtByConfirmedSOSysNoList(soSysNoStr);
        }

        /// <summary>
        /// 创建财务收款信息
        /// </summary>
        /// <param name="postPayInfo"></param>
        /// <param name="soBaseInfo"></param>
        /// <returns></returns>
        private SOIncomeInfo CreateSOIncomeInfo(PostPayInfo postPayInfo, SOBaseInfo soBaseInfo)
        {
            var soIncomeInfo = new SOIncomeInfo()
            {
                OrderType = SOIncomeOrderType.SO,
                OrderSysNo = postPayInfo.SOSysNo.Value,
                OrderAmt = soBaseInfo.SOTotalAmount,
                IncomeStyle = SOIncomeOrderStyle.Advanced,
                IncomeAmt = postPayInfo.PayAmount,
                PrepayAmt = soBaseInfo.PrepayAmount,
                Status = SOIncomeStatus.Origin,
                PayAmount = postPayInfo.PayAmount,
                PointPay = soBaseInfo.PointPay,
                GiftCardPayAmt = soBaseInfo.GiftCardPay,
                CompanyCode = soBaseInfo.CompanyCode
            };
            return ObjectFactory<SOIncomeProcessor>.Instance.Create(soIncomeInfo);
        }

        /// <summary>
        /// 获取已确认订单总的应收金额
        /// </summary>
        /// <param name="confirmedSOSysNo"></param>
        /// <returns></returns>
        private decimal GetTotalAmt(int confirmedSOSysNo)
        {
            decimal totalAmt = 0M;
            var list = m_PostPayDA.GetListByConfirmedSOSysNo(confirmedSOSysNo);
            if (list != null && list.Count > 0)
            {
                list.ForEach(p =>
                {
                    var so = ExternalDomainBroker.GetSOBaseInfo(p.SOSysNo.Value);
                    if (so != null)
                    {
                        totalAmt += so.ReceivableAmount;
                    }
                });
            }
            return totalAmt;
        }

        /// <summary>
        /// 根据订单系统编号作废PostPay
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        public virtual void AbandonBySOSysNo(int soSysNo)
        {
            m_PostPayDA.AbandonBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 检查支付方式是否是银行、电汇支付
        /// </summary>
        public virtual bool IsBankOrPostPayType(int payTypeSysNO)
        {
            return ObjectFactory<PostPayProcessor>.Instance.GetBankOrPostPayTypeList()
                .Exists(m => m.SysNo == payTypeSysNO);
        }

        /// <summary>
        /// 取得银行电汇-邮局付款支付方式列表
        /// </summary>
        /// <returns></returns>
        public virtual List<PayType> GetBankOrPostPayTypeList()
        {
            var paytypeList = ExternalDomainBroker.GetPayTypeList();
            if (paytypeList != null)
            {
                string cfg = AppSettingManager.GetSetting("Invoice", "BankAndPostPayTypeSysNo");
                if (!string.IsNullOrEmpty(cfg))
                {
                    var bankOrPostPayTypeSysNo = cfg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var result = paytypeList.Where(w => bankOrPostPayTypeSysNo.Contains(w.SysNo.ToString()))
                        .Select(s => s)
                        .ToList();
                    return result;
                }
            }
            return new List<PayType>();
        }

        /// <summary>
        /// 根据订单系统编号和PostPay状态取得PostPay列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="status">PostPay状态，可以包含多种状态</param>
        /// <returns></returns>
        public virtual List<PostPayInfo> GetListBySOSysNoAndStatus(int soSysNo, params PostPayStatus[] status)
        {
            return m_PostPayDA.GetListBySOSysNoAndStatus(soSysNo, status);
        }

        /// <summary>
        /// 根据订单编号取得订单有效的postpay
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public virtual PostPayInfo GetValidPostPayBySOSysNo(int soSysNo)
        {
            return m_PostPayDA.GetValidPostPayBySOSysNo(soSysNo);
        }

        #region [For SO Domain]

        /// <summary>
        /// 拆分PostPay
        /// </summary>
        /// <param name="entity"></param>
        public virtual void SplitForSO(PostPayInfo entity)
        {
            m_PostPayDA.UpdateStatusSplitForSO(entity);
        }

        /// <summary>
        /// 作废拆分PostPay
        /// </summary>
        /// <param name="master"></param>
        /// <param name="subList"></param>
        public virtual void AbandonSplitForSO(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            m_PostPayDA.AbandonSplitForSO(master, subList);
        }

        #endregion [For SO Domain]

        #region Helper Methods

        protected void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PostPay, msgKeyName), args);
        }

        #endregion Helper Methods
    }
}