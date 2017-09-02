using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.RMA;
using ECommerce.Entity.Common;
using ECommerce.Entity.Customer;
using ECommerce.Entity.RMA;
using ECommerce.Entity.SO;
using ECommerce.Enums;
using ECommerce.Service.Common;
using ECommerce.Service.Customer;
using ECommerce.Service.SO;
using ECommerce.Utility;

namespace ECommerce.Service.RMA
{
    public class RMARequestService
    {
        public static QueryResult<RMARequestQueryResultInfo> RMARequestOrderQuery(RMARequestQueryFilter filter)
        {
            return RMARequestDA.QueryList(filter);
        }

        public static RMARequestInfo LoadWithRequestSysNo(int rmaRequestSysNo, int sellerSysNo)
        {
            if (rmaRequestSysNo > 0 && sellerSysNo > 0)
            {
                RMARequestInfo info = RMARequestDA.LoadWithRequestSysNo(rmaRequestSysNo, sellerSysNo);
                return info;
            }
            return null;
        }

        /// <summary>
        /// 审核通过退换货申请单
        /// </summary>
        /// <param name="rmaRequestSysNo">退换货申请单编号</param>
        /// <param name="userSysNo">操作人用户</param>
        /// <returns></returns>
        public static RMARequestInfo Valid(int rmaRequestSysNo, LoginUser operateUser)
        {
            string serviceCode = RMARequestDA.CreateServiceCode();
            if (string.IsNullOrEmpty(serviceCode) || serviceCode.Length != 6)
            {
                throw new ArgumentNullException("生成服务编号时发生异常");
            }
            RMARequestInfo request = LoadWithRequestSysNo(rmaRequestSysNo, operateUser.SellerSysNo);
            if (request == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退换货申请单", rmaRequestSysNo));
            }
            if (request.Status != RMARequestStatus.WaitingAudit)
            {
                throw new BusinessException(L("申请单不是“待审核”，不能审核通过"));
            }

            request.ServiceCode = serviceCode;
            //审核通过后状态变成“待处理”，这个时候可以进行收货操作
            request.Status = RMARequestStatus.Origin;
            request.AuditTime = DateTime.Now;
            request.AuditUserSysNo = operateUser.UserSysNo;
            request.AuditUserName = operateUser.UserDisplayName;

            using (ITransaction trans = TransactionManager.Create())
            {
                UpdateWithRegisters(request);
                trans.Complete();
            }

            if (request.IsReceiveMsg == true && StringUtility.IsPhoneNo(request.Phone))
            {
                string message = L("您好！您的售后单号{0}已审核通过，服务编号：{1}，请在快递面单上备注清楚您的售后单号及编号，谢谢您的配合！", request.RequestID, request.ServiceCode);
                SMSService.SendSMS(request.Phone, message);
            }
            return request;
        }

        /// <summary>
        /// 审核拒绝退换货申请单
        /// </summary>
        /// <param name="rmaRequestSysNo">退换货申请单编号</param>
        /// <param name="userSysNo">操作人用户</param>
        /// <returns></returns>
        public static RMARequestInfo Reject(int rmaRequestSysNo, LoginUser operateUser)
        {
            RMARequestInfo request = LoadWithRequestSysNo(rmaRequestSysNo, operateUser.SellerSysNo);
            if (request == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退换货申请单", rmaRequestSysNo));
            }
            if (request.Status != RMARequestStatus.WaitingAudit)
            {
                throw new BusinessException(L("申请单不是“待审核”，不能审核拒绝"));
            }

            request.Status = RMARequestStatus.AuditRefuesed;
            using (ITransaction trans = TransactionManager.Create())
            {
                UpdateWithRegisters(request);
                trans.Complete();
            }

            return request;
        }

        /// <summary>
        /// 作废退换货申请单
        /// </summary>
        /// <param name="rmaRequestSysNo">退换货申请单编号</param>
        /// <param name="userSysNo">操作人用户</param>
        /// <returns></returns>
        public static RMARequestInfo Abandon(int rmaRequestSysNo, LoginUser operateUser)
        {
            RMARequestInfo request = LoadWithRequestSysNo(rmaRequestSysNo, operateUser.SellerSysNo);
            if (request == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退换货申请单", rmaRequestSysNo));
            }
            if (request.Status != RMARequestStatus.WaitingAudit && request.Status != RMARequestStatus.Origin)
            {
                throw new BusinessException(L("申请单不是“待审核”或“待处理”，不能作废"));
            }

            request.Status = RMARequestStatus.Abandon;
            using (ITransaction trans = TransactionManager.Create())
            {
                UpdateWithRegisters(request);
                trans.Complete();
            }
            return request;
        }

        /// <summary>
        /// 接收退货
        /// </summary>
        /// <param name="rmaRequestSysNo">退换货申请单编号</param>
        /// <param name="userSysNo">操作人用户</param>
        /// <returns></returns>
        public static RMARequestInfo Receive(int rmaRequestSysNo, LoginUser operateUser)
        {
            RMARequestInfo request = LoadWithRequestSysNo(rmaRequestSysNo, operateUser.SellerSysNo);
            if (request == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退换货申请单", rmaRequestSysNo));
            }
            if (request.Status != RMARequestStatus.Origin)
            {
                throw new BusinessException(L("不能接收非“待处理”状态的申请单"));
            }
            if (request.Registers == null || request.Registers.Count <= 0)
            {
                throw new BusinessException(L("没有需要退换货的商品"));
            }
            //TODO: 虚拟仓库编码
            request.ReceiveWarehouse = "90";
            request.RecvTime = DateTime.Now;
            request.RecvUserSysNo = operateUser.UserSysNo;
            request.RecvUserName = operateUser.UserDisplayName;
            //收货后状态变成“处理中”，这个时候可以进行退货或退款操作
            request.Status = RMARequestStatus.Handling;

            bool isWithin7Days = false;
            if (request.CustomerSendTime.HasValue)
            {
                isWithin7Days = request.CustomerSendTime.Value.AddDays(-7) < DateTime.Now;
            }
            List<SOItemInfo> soItems = SOService.GetSOItemInfoList(request.SOSysNo.Value);
            SOItemInfo soItem = null;
            foreach (var reg in request.Registers)
            {
                reg.OwnBy = RMAOwnBy.Customer;
                reg.Location = RMALocation.Self;
                reg.LocationWarehouse = request.ReceiveWarehouse;
                reg.IsWithin7Days = isWithin7Days;
                reg.Status = request.Status;

                soItem = soItems.Find(x => x.ProductSysNo == reg.ProductSysNo.Value
                    && x.ProductType == reg.SoItemType.Value);
                if (soItem == null)
                {
                    throw new BusinessException(L("订单中不存在申请退换货的商品：{0}【1】", reg.ProductName, reg.ProductID));
                }
                reg.Cost = soItem.Cost;
            };

            UpdateWithRegisters(request);
            return request;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="rmaRequestSysNo">退换货申请单编号</param>
        /// <param name="refundInfo">退款信息</param>
        /// <param name="userSysNo">操作人用户</param>
        /// <returns></returns>
        public static RMARequestInfo Refund(int rmaRequestSysNo, RMARefundInfo refundInfo, LoginUser operateUser)
        {
            RMARequestInfo requestInfo = LoadWithRequestSysNo(rmaRequestSysNo, operateUser.SellerSysNo);
            if (requestInfo == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退换货申请单", rmaRequestSysNo));
            }
            if (requestInfo.Status != RMARequestStatus.Handling)
            {
                throw new BusinessException(L("不是“处理中”的申请单不能退款"));
            }
            if (refundInfo == null || !refundInfo.RefundPayType.HasValue)
            {
                throw new ArgumentException(L("退款信息为空"));
            }

            requestInfo.Registers = requestInfo.Registers.Where(g => g.RequestType == RMARequestType.Return
                                                                    && g.Status == RMARequestStatus.Handling).ToList();
            if (requestInfo.Registers.Count <= 0)
            {
                throw new ArgumentException(L("没有“待处理”的退货商品"));
            }

            refundInfo.SOSysNo = requestInfo.SOSysNo;
            refundInfo.CustomerSysNo = requestInfo.CustomerSysNo;
            //计算退款金额
            CalcRefundAmount(refundInfo, requestInfo);

            using (ITransaction trans = TransactionManager.Create())
            {
                //创建退款单
                RMARefundService.Create(refundInfo, operateUser);
                //设置单件的退款状态为退款中
                foreach (var registerInfo in requestInfo.Registers)
                {
                    registerInfo.RefundStatus = RMARefundStatus.WaitingAudit;
                    RMARequestDA.UpdateRegisterStatus(registerInfo);
                }

                trans.Complete();
            }

            return requestInfo;
        }

        public static void RefreshRequestStatus(int rmaRequestSysNo, int sellerSysNo)
        {
            RMARequestInfo requestInfo = LoadWithRequestSysNo(rmaRequestSysNo, sellerSysNo);
            if (requestInfo != null)
            {
                if (requestInfo.Status == RMARequestStatus.Handling &&
                    requestInfo.Registers.Count(x => x.Status == RMARequestStatus.Handling) <= 0)
                {
                    requestInfo.Status = RMARequestStatus.Complete;
                    RMARequestDA.Update(requestInfo);
                }
            }
        }

        private static void CalcRefundAmount(RMARefundInfo refundInfo, RMARequestInfo request)
        {
            refundInfo.RefundItems = new List<RMARefundItemInfo>();
            SOInfo soInfo = SOService.GetSOInfo(request.SOSysNo.Value);

            //计算积分支付比例
            decimal originalSOCashPointRate = soInfo.PointAmt / (
                       soInfo.Amount.SOAmt
                     + soInfo.Amount.ShipPrice
                     - Math.Abs(soInfo.PromotionAmt)
                     - Math.Abs(soInfo.Amount.DiscountAmt));
            refundInfo.SOCashPointRate = Decimal.Round(originalSOCashPointRate, 4);

            List<SOItemInfo> soItems = SOService.GetSOItemInfoList(request.SOSysNo.Value);
            //计算总计应退金额，包括应退现金+积分+余额
            decimal totalRefundProductValue = Decimal.Round(request.Registers.Sum(registerInfo =>
            {
                var soItem = soItems.Find(x => x.ProductSysNo == registerInfo.ProductSysNo.Value
                    && x.ProductType == registerInfo.SoItemType.Value);
                return soItem.OriginalPrice - Math.Abs(soItem.PromotionDiscount) - Math.Abs(soItem.DiscountAmt / soItem.Quantity);
            }), 2);

            decimal assignedRefundCashAmt = 0m;
            refundInfo.OrgCashAmt = 0m; refundInfo.OrgPointAmt = 0; refundInfo.PointPay = 0;

            for (var index = 0; index < request.Registers.Count; index++)
            {
                RMARegisterInfo registerInfo = request.Registers[index];
                RMARefundItemInfo refundItem = new RMARefundItemInfo();
                var soItem = soItems.Find(x => x.ProductSysNo == registerInfo.ProductSysNo.Value
                     && x.ProductType == registerInfo.SoItemType.Value);

                refundItem.OrgPrice = soItem.OriginalPrice;
                refundItem.OrgPoint = soItem.Point;
                refundItem.PointType = soItem.PointType;
                refundItem.UnitDiscount = soItem.DiscountAmt / soItem.Quantity;
                refundItem.ProductValue = (soItem.OriginalPrice - Math.Abs(soItem.PromotionDiscount)) - Math.Abs(refundItem.UnitDiscount.Value);
                refundItem.RefundCost = soItem.Cost;
                refundItem.RefundCostWithoutTax = soItem.UnitCostWithoutTax;
                refundItem.RefundPoint = soItem.Point;
                refundItem.RegisterSysNo = registerInfo.SysNo;

                if (totalRefundProductValue <= 0m)
                {
                    refundItem.RefundCash = 0m;
                }
                else
                {
                    //按商品价值比例计算单个商品退款金额
                    if (index < request.Registers.Count - 1)
                    {
                        refundItem.RefundCash =
                            ((refundItem.ProductValue / totalRefundProductValue) * refundInfo.CashAmt * (1 - originalSOCashPointRate)).Value;
                    }
                    else
                    {
                        refundItem.RefundCash = refundInfo.CashAmt.Value - assignedRefundCashAmt;
                    }
                }
                refundItem.RefundPrice = refundItem.RefundCash;
                refundItem.RefundPoint = Convert.ToInt32(Decimal.Round((refundItem.RefundCash * originalSOCashPointRate).Value, 0));
                refundItem.RefundPriceType = RefundPriceType.OriginPrice;

                refundInfo.OrgCashAmt += refundItem.RefundCash.Value;
                refundInfo.OrgPointAmt += (-1) * refundItem.RefundPoint.Value;
                refundInfo.PointPay += refundInfo.OrgPointAmt;

                assignedRefundCashAmt += refundItem.RefundCash.Value;
                refundInfo.RefundItems.Add(refundItem);
            }

            #region 计算顾客积分归还积分折合现金

            refundInfo.DeductPointFromAccount = 0;
            refundInfo.DeductPointFromCurrentCash = 0m;
            if (refundInfo.OrgPointAmt < 0)
            {
                CustomerBasicInfo customer = CustomerService.GetCustomerInfo(refundInfo.CustomerSysNo.Value);
                if (refundInfo.OrgPointAmt * -1 < customer.ValidScore)
                {
                    refundInfo.DeductPointFromAccount = refundInfo.OrgPointAmt * -1;
                }
                else
                {
                    refundInfo.DeductPointFromAccount = customer.ValidScore;
                    refundInfo.DeductPointFromCurrentCash =
                        Decimal.Round(((refundInfo.OrgPointAmt ?? 0) * -1
                        - (customer.ValidScore ?? 0)) / CustomerService.GetPointToMoneyRatio(), 2);
                }
            }
            #endregion
        }

        /// <summary>
        /// 换货
        /// </summary>
        /// <param name="rmaRequestSysNo"></param>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public static RMARequestInfo Revert(int rmaRequestSysNo, LoginUser operateUser)
        {
            RMARequestInfo request = LoadWithRequestSysNo(rmaRequestSysNo, operateUser.SellerSysNo);
            if (request == null)
            {
                throw new BusinessException(L("未找到编号为【{0}】的退换货申请单", rmaRequestSysNo));
            }
            if (request.Status != RMARequestStatus.Handling)
            {
                throw new BusinessException(L("不是“处理中”的申请单不能退货"));
            }

            var rmaRevertRegister = request.Registers.Where(g => g.RequestType == RMARequestType.Exchange
                                                                    && g.Status == RMARequestStatus.Handling);
            if (rmaRevertRegister.Count() <= 0)
            {
                throw new BusinessException(L("没有“待处理”的换货商品"));
            }

            using (ITransaction trans = TransactionManager.Create())
            {
                //如果所有的单件都处理完毕，自动关闭退款申请
                if (request.Registers.Count(g => g.RequestType == RMARequestType.Exchange)
                    == request.Registers.Count(g => g.Status == RMARequestStatus.Handling))
                {
                    request.Status = RMARequestStatus.Complete;
                    RMARequestDA.Update(request);
                }
                //关闭单件， 更新单件发还状态为已发还
                foreach (var registerInfo in rmaRevertRegister)
                {
                    registerInfo.RevertStatus = RMARevertStatus.Reverted;
                    registerInfo.Status = RMARequestStatus.Complete;
                    RMARequestDA.UpdateRegisterStatus(registerInfo);
                }

                trans.Complete();
            }

            return request;
        }

        private static void UpdateWithRegisters(RMARequestInfo request)
        {
            RMARequestDA.Update(request);
            if (request.Registers != null)
            {
                if (request.Status == RMARequestStatus.AuditRefuesed)
                {
                    request.Registers.ForEach(
                                           reg =>
                                           {
                                               reg.Status = RMARequestStatus.Abandon;
                                               RMARequestDA.UpdateRegisterStatus(reg);
                                           }
                                       );
                }
                else
                {
                    request.Registers.ForEach(
                        reg =>
                        {
                            reg.Status = request.Status;
                            RMARequestDA.UpdateRegisterStatus(reg);
                        }
                    );
                }
            }
        }

        private static string L(string key, params object[] args)
        {
            string multiLangText = ECommerce.WebFramework.LanguageHelper.GetText(key);
            return string.Format(multiLangText, args);
        }

    }
}
