using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.RMA;
using ECommerce.Entity.Common;
using ECommerce.Entity.Customer;
using ECommerce.Entity.Invoice;
using ECommerce.Entity.RMA;
using ECommerce.Enums;
using ECommerce.Service.Customer;
using ECommerce.Service.SO;
using ECommerce.Utility;
using ECommerce.DataAccess.Invoice;
using ECommerce.Entity.SO;
using ECommerce.Service.Invoice;

namespace ECommerce.Service.RMA
{
    public class RMARefundService
    {
        private static decimal pointExchangeRate = CustomerService.GetPointToMoneyRatio();

        public static QueryResult<RMARefundQueryResultInfo> RMARefundOrderQuery(RMARefundQueryFilter filter)
        {
            return RMARefundDA.QueryList(filter);
        }

        public static RMARefundInfo LoadWithRefundSysNo(int rmaRefundSysNo, int sellerSysNo)
        {
            if (rmaRefundSysNo > 0)
            {
                RMARefundInfo info = RMARefundDA.LoadWithRefundSysNo(rmaRefundSysNo, sellerSysNo);
                return info;
            }
            return null;
        }

        public static RMARefundInfo Create(RMARefundInfo refundInfo, LoginUser operateUser)
        {
            if (!refundInfo.SOSysNo.HasValue)
            {
                throw new BusinessException(L("订单号不能为空"));
            }

            if (refundInfo.RefundPayType == RefundPayType.BankRefund)
            {
                if (string.IsNullOrWhiteSpace(refundInfo.CardOwnerName))
                {
                    throw new BusinessException(L("收款人不能为空"));
                }
                if (string.IsNullOrWhiteSpace(refundInfo.BankName))
                {
                    throw new BusinessException(L("银行名称不能为空"));
                }
                if (string.IsNullOrWhiteSpace(refundInfo.CardNumber))
                {
                    throw new BusinessException(L("银行卡号不能为空"));
                }
                if (!refundInfo.CashAmt.HasValue || refundInfo.CashAmt.Value < 0m)
                {
                    throw new BusinessException(L("退款金额不能小于0"));
                }
                var maxRefundAmt = Decimal.Round(refundInfo.RefundItems.Sum(x => x.ProductValue.Value), 2, MidpointRounding.AwayFromZero);
                if (refundInfo.CashAmt > maxRefundAmt)
                {
                    throw new BusinessException(L("实际退款金额不能大于应退金额：{0}", maxRefundAmt));
                }
            }

            #region 检查订单最大可退金额
            //数据check不包含在事务中
            using (ITransaction trans = TransactionManager.SuppressTransaction())
            {
                var soItemList = SOService.GetSOItemInfoList(refundInfo.SOSysNo.Value);
                decimal maxRMARefundAmt = soItemList.Sum(soItem =>
                {
                    return (soItem.OriginalPrice - Math.Abs(soItem.PromotionDiscount)
                        - Math.Abs(soItem.DiscountAmt / soItem.Quantity)) * soItem.Quantity;
                });

                decimal thisRefundAmt = Math.Abs(refundInfo.CashAmt.GetValueOrDefault()) + Math.Abs(refundInfo.PointPay.GetValueOrDefault() * pointExchangeRate)
                    + Math.Abs(refundInfo.GiftCardAmt.GetValueOrDefault());
                decimal historyRefundAmt = 0m;

                var validRefundList = RMARefundDA.GetValidRefundListBySOSysNo(refundInfo.SOSysNo.Value);
                if (validRefundList != null && validRefundList.Count > 0)
                {
                    historyRefundAmt = validRefundList.Sum(info =>
                    {
                        return Math.Abs(info.CashAmt.GetValueOrDefault()) + Math.Abs(info.PointPay.GetValueOrDefault() * pointExchangeRate)
                            + Math.Abs(info.GiftCardAmt.GetValueOrDefault());
                    });
                }
                if (thisRefundAmt + historyRefundAmt > Decimal.Round(maxRMARefundAmt, 2))
                {
                    throw new BusinessException(L("超过原始购物订单#{0}的最大可退金额{1}，不能再退款", refundInfo.SOSysNo, Decimal.Round(maxRMARefundAmt, 2)));
                }
                trans.Complete();
            }
            #endregion

            refundInfo.Status = RMARefundStatus.WaitingAudit;
            refundInfo.SOIncomeStatus = SOIncomeStatus.Origin;
            refundInfo.InUserSysNo = operateUser.UserSysNo;
            refundInfo.InUserName = operateUser.UserDisplayName;
            using (ITransaction trans = TransactionManager.Create())
            {
                int newSysNo = RMARefundDA.CreateNewRefundSysNo();
                refundInfo.SysNo = newSysNo;
                refundInfo.RefundID = String.Format("R3{0:00000000}", newSysNo);
                //创建RMA Refund记录 
                RMARefundDA.Create(refundInfo);

                //创建退款银行信息 
                SOIncomeRefundInfo soIncomeRefundInfo = new SOIncomeRefundInfo()
                {
                    OrderType = refundInfo.OrderType,
                    OrderSysNo = refundInfo.SysNo,
                    SOSysNo = refundInfo.SOSysNo,
                    BankName = refundInfo.BankName,
                    CardNumber = refundInfo.CardNumber,
                    CardOwnerName = refundInfo.CardOwnerName,
                    RefundPayType = refundInfo.RefundPayType,
                    CreateUserSysNo = operateUser.UserSysNo,
                    CreateUserName = operateUser.UserDisplayName,
                    Status = RefundStatus.Origin,
                    HaveAutoRMA = false,
                    RefundCashAmt = refundInfo.CashAmt,
                    RefundPoint = refundInfo.PointPay,
                    RefundGiftCard = refundInfo.GiftCardAmt
                };
                RMARefundDA.CreateRefundBankInfo(soIncomeRefundInfo);

                if (refundInfo.RefundItems != null)
                {
                    foreach (var item in refundInfo.RefundItems)
                    {
                        item.RefundSysNo = refundInfo.SysNo;
                        RMARefundDA.CreateItem(item);
                    }
                }
                trans.Complete();
            }

            return refundInfo;
        }

        public static RMARefundInfo Valid(int rmaRefundSysNo, LoginUser operateUser)
        {
            RMARefundInfo info = LoadWithRefundSysNo(rmaRefundSysNo, operateUser.SellerSysNo);
            if (info == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退款单", rmaRefundSysNo));
            }
            if (info.Status != RMARefundStatus.WaitingAudit)
            {
                throw new BusinessException(L("退款单不是“待审核”，不能审核通过"));
            }

            //info.Status = RMARefundStatus.WaitingRefund;
            info.Status = RMARefundStatus.Refunded;
            info.SOIncomeStatus = SOIncomeStatus.Origin;
            info.AuditUserSysNo = operateUser.UserSysNo;
            info.AuditUserName = operateUser.UserDisplayName;
            info.AuditDate = DateTime.Now;

            using (ITransaction ts = TransactionManager.Create())
            {
                RMARefundDA.Update(info);
                //更新BankInfoStatus的为审核通过
                RMARefundDA.AuditSOIncomeRefund(info.SysNo.Value, (int)info.OrderType, (int)RefundStatus.Refunded, operateUser.UserSysNo, operateUser.UserDisplayName);
                RMARefundDA.BatchUpdateRegisterRefundStatus(info.SysNo.Value, RMARefundStatus.WaitingRefund);
                //写入退款单
                SOIncomeInfo soIncomeInfo = new SOIncomeInfo()
                {
                    OrderType = SOIncomeOrderType.RO,
                    OrderSysNo = info.SysNo,
                    OrderAmt = -1 * info.CashAmt,
                    IncomeStyle = SOIncomeOrderStyle.RO,
                    IncomeAmt = -1 * info.CashAmt,
                    PayAmount = -1 * info.CashAmt,
                    InUserSysNo = operateUser.UserSysNo,
                    InUserName = operateUser.UserDisplayName,
                    Status = SOIncomeStatus.Origin,
                    PointPay = info.PointPay,
                    GiftCardPayAmt = info.GiftCardAmt,
                };
                RMARefundDA.CreateRefundSOIncome(soIncomeInfo);

                ts.Complete();
            }

            return info;
        }

        public static RMARefundInfo Reject(int rmaRefundSysNo, LoginUser operateUser)
        {
            RMARefundInfo info = LoadWithRefundSysNo(rmaRefundSysNo, operateUser.SellerSysNo);
            if (info == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退款单", rmaRefundSysNo));
            }
            if (info.Status != RMARefundStatus.WaitingAudit)
            {
                throw new BusinessException(L("退款单不是“待审核”，不能审核拒绝"));
            }

            info.Status = RMARefundStatus.Abandon;
            info.SOIncomeStatus = SOIncomeStatus.Abandon;
            using (ITransaction ts = TransactionManager.Create())
            {
                RMARefundDA.Update(info);
                RMARefundDA.BatchUpdateRegisterRefundStatusAndStatus(info.SysNo.Value, RMARefundStatus.Abandon, RMARequestStatus.Complete);

                var rmaRequstSysNoList = RMARefundDA.QueryRMARequsetSysNoByRefundSysNo(info.SysNo.Value);
                if (rmaRequstSysNoList != null && rmaRequstSysNoList.Count > 0)
                {
                    foreach (var rmaRequestSysNo in rmaRequstSysNoList)
                    {
                        RMARequestService.RefreshRequestStatus(rmaRequestSysNo, operateUser.SellerSysNo);
                    }
                }

                ts.Complete();
            }

            return info;
        }

        public static RMARefundInfo Confirm(int rmaRefundSysNo, LoginUser operateUser)
        {
            RMARefundInfo info = LoadWithRefundSysNo(rmaRefundSysNo, operateUser.SellerSysNo);
            if (info == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退款单", rmaRefundSysNo));
            }
            if (info.Status != RMARefundStatus.WaitingRefund)
            {
                throw new BusinessException(L("退款单不是“待退款”，不能确认退款"));
            }
            SOInfo soInfo = SOService.GetSOInfo(info.SOSysNo.Value);
            if (soInfo == null)
            {
                throw new BusinessException(L("订单不存在"));
            }
            info.Status = RMARefundStatus.Refunded;
            info.SOIncomeStatus = SOIncomeStatus.Confirmed;
            info.RefundUserSysNo = operateUser.UserSysNo;
            info.RefundUserName = operateUser.UserDisplayName;
            info.RefundDate = DateTime.Now;

            //using (ITransaction ts = TransactionManager.Create(
            //     System.Transactions.TransactionScopeOption.Required, System.Transactions.IsolationLevel.ReadUncommitted))
            using (ITransaction ts = TransactionManager.Create())
            {
                //积分撤销
                ReturnProductPoint(info, operateUser.UserSysNo);
                //退入余额帐户
                RefundPrepay(info);

                //更新客户累计购买金额
                if (info.CashAmt != 0)
                {
                    CustomerService.UpdateCustomerOrderedAmt(info.CustomerSysNo.Value, -1 * info.CashAmt.Value);
                }
                RMARefundDA.Update(info);
                RMARefundDA.BatchUpdateRegisterRefundStatusAndStatus(info.SysNo.Value, RMARefundStatus.Refunded, RMARequestStatus.Complete);
                //RMARefundDA.ConfirmRefundSOIncome(info);
                SOIncomeInfo soIncomeInfo = new SOIncomeInfo()
                {
                    OrderType = SOIncomeOrderType.RO,
                    OrderSysNo = info.SysNo,
                    OrderAmt = -1 * info.CashAmt,
                    IncomeStyle = SOIncomeOrderStyle.RO,
                    IncomeAmt = -1 * info.CashAmt,
                    PayAmount = -1 * info.CashAmt,
                    InUserSysNo = operateUser.UserSysNo,
                    InUserName = operateUser.UserDisplayName,
                    Status = SOIncomeStatus.Origin,
                    PointPay = info.PointPay,
                    GiftCardPayAmt = info.GiftCardAmt,
                };
                RMARefundDA.CreateRefundSOIncome(soIncomeInfo);
                

                var rmaRequstSysNoList = RMARefundDA.QueryRMARequsetSysNoByRefundSysNo(info.SysNo.Value);
                if (rmaRequstSysNoList != null && rmaRequstSysNoList.Count > 0)
                {
                    foreach (var rmaRequestSysNo in rmaRequstSysNoList)
                    {
                        RMARequestService.RefreshRequestStatus(rmaRequestSysNo, operateUser.SellerSysNo);
                    }
                }

                SOIncomeInfo rmaIncomeInfo = SOIncomeDA.GetValidSOIncomeInfo(info.SysNo.Value, SOIncomeOrderType.RO);
                //ECC确认退款开始
                if (info.RefundPayType == RefundPayType.NetWorkRefund)
                {
                    //发起银行网关退款
                    RefundResult result = ProcessNetWorkRefund(rmaIncomeInfo, soInfo);
                    if (!result.Result)
                    {
                        throw new BusinessException(result.Message);
                    }
                    else
                    {
                        rmaIncomeInfo.ExternalKey = result.ExternalKey;//退款流水号
                        rmaIncomeInfo.Status = SOIncomeStatus.Confirmed;//等待银行后台回调处理中
                        RMARefundDA.ConfirmRefundSOIncomeNet(info, soIncomeInfo);
                    }
                }
                else if (info.RefundPayType == RefundPayType.BankRefund)
                {
                    RMARefundDA.ConfirmRefundSOIncome(info);
                }
                

                ts.Complete();
            }
            return info;
        }

        private static void ReturnProductPoint(RMARefundInfo refundInfo, int userSysNo)
        {
            int affectedPoint1 = -1 * (refundInfo.DeductPointFromAccount ?? 0) + (refundInfo.PointPay ?? 0);

            if (affectedPoint1 != 0)
            {
                AdjustPointRequest itemPointInfo = new AdjustPointRequest()
                {
                    Source = "RMA",
                    CustomerSysNo = refundInfo.CustomerSysNo.Value,
                    Point = affectedPoint1,
                    PointType = (int)AdjustPointType.ReturnProductPoint,
                    Memo = refundInfo.SysNo.ToString(),
                    OperationType = AdjustPointOperationType.Abandon,
                    SOSysNo = refundInfo.SOSysNo
                };

                CustomerService.AdjustPoint(itemPointInfo, userSysNo);//item积分撤消
            }
        }

        private static void RefundPrepay(RMARefundInfo refundInfo)
        {
            if (refundInfo.RefundPayType == RefundPayType.PrepayRefund)
            {
                CustomerPrepayLog prepayLogInfo = new CustomerPrepayLog()
                {
                    SOSysNo = refundInfo.SOSysNo.Value,
                    CustomerSysNo = refundInfo.CustomerSysNo.Value,
                    AdjustAmount = refundInfo.CashAmt ?? 0M,
                    PrepayType = PrepayType.ROReturn,
                    Note = "RMA退款单退入余额账户"
                };
                CustomerService.AdjustPrePay(prepayLogInfo);
            }
        }

        private static string L(string key, params object[] args)
        {
            string multiLangText = ECommerce.WebFramework.LanguageHelper.GetText(key);
            return string.Format(multiLangText, args);
        }
        private static RefundResult ProcessNetWorkRefund(SOIncomeInfo entity, SOInfo soInfo)
        {
            var result = new RefundResult();
            List<string> payTypeList = CodeNamePairManager.GetList("Inventory", "ChinaPayPayTypeList").Select(p => p.Code).ToList();
            if (payTypeList.Contains(soInfo.Payment.PayTypeID.ToString()))
            {
                var biz = new IPSPayUtils();
                var refundEntity = new RefundEntity
                {
                    SOSysNo = soInfo.SOSysNo,
                    RefundSysNo = entity.OrderSysNo.Value,
                    RefundAmt = Math.Abs(entity.IncomeAmt.Value),
                    CompanyCode = entity.CompanyCode,
                    OrderDate = soInfo.OrderDate.Value
                };
                refundEntity.ProductAmount = Math.Abs(entity.IncomeAmt.Value)
                                               - soInfo.Amount.ShipPrice
                                               - soInfo.TariffAmt;
                refundEntity.TaxFeeAmount = soInfo.TariffAmt;
                refundEntity.ShippingFeeAmount = soInfo.Amount.ShipPrice;


                if (soInfo.Payment.PayTypeID >= 200 && soInfo.Payment.PayTypeID < 300)
                {
                    result = biz.Refund(refundEntity);
                }
                else
                {
                    throw new BusinessException("未实现此支付方式");
                }
            }
            else
            {
                result.Result = false;

                result.Message = ("支付方式" + soInfo.Payment.PayTypeName + "不支持网关直退。");
            }
            return result;
        }

    }
}
