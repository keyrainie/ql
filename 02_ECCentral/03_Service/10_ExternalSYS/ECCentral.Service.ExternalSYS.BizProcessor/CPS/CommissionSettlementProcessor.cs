using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS.CPS;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.ExternalSYS.IDataAccess.CPS;
using ECCentral.Service.Utility;
using DeliveryType = ECCentral.BizEntity.Invoice.DeliveryType;

namespace ECCentral.Service.ExternalSYS.BizProcessor.CPS
{
    [VersionExport(typeof(CommissionSettlementProcessor))]
    public class CommissionSettlementProcessor
    {
        public const decimal DefaultPercentage = 0.01m;

        public List<int> GetSettledUserSysNoList()
        {
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var beginDate = endDate.AddMonths(-1);
            return ObjectFactory<ICommissionSettlementItemDA>.Instance.GetHavingOrderUserList(beginDate, endDate,
                                                                                              FinanceStatus.Unsettled);
        }

        public void ProcessUserSettledCommissionInfo(int userSysNo)
        {
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var beginDate = endDate.AddMonths(-1);
            var commissionSettlementItemInfoList =
                ObjectFactory<ICommissionSettlementItemDA>.Instance.GetCommissionSettlementItemInfoListByUserSysNo(
                    userSysNo, beginDate, endDate, FinanceStatus.Unsettled);
            var commissionSettlementInfo = new CommissionSettlementInfo
                                               {
                                                   UserSysNo = userSysNo,
                                                   Status = FinanceStatus.Settled,
                                                   SettledBeginDate = beginDate,
                                                   SettledEndDate = endDate,
                                                   OperateUser = "System Job"
                                               };
            ObjectFactory<ICommissionSettlementDA>.Instance.Insert(commissionSettlementInfo);
            if (commissionSettlementInfo.SysNo.HasValue)
            {
                commissionSettlementItemInfoList.ForEach(item =>
                {
                    item.CommissionSettlementItemSOItemCalculateInfoList =
                        item.Type == CPSOrderType.SO
                            ? GetSOCalculateInfo(item.OrderSysNo)
                            : GetRMACalculateInfo(item.OrderSysNo);
                    item.CommissionAmt =
                        item.CommissionSettlementItemSOItemCalculateInfoList.Sum(i => i.Price * i.Qty * i.Percentage);
                    item.Status = FinanceStatus.Settled;
                    item.CommissionSettlementSysNo = commissionSettlementInfo.SysNo;
                    item.OperateUser = "System Job";
                    ObjectFactory<ICommissionSettlementItemDA>.Instance.Update(item);
                });
                decimal totalAmt = 0;
                commissionSettlementItemInfoList.ForEach(item =>
                {
                    if (item.Type == CPSOrderType.SO)
                    {
                        totalAmt += item.CommissionAmt;
                    }
                    else
                    {
                        totalAmt -= item.CommissionAmt;
                    }
                });
                commissionSettlementInfo.CommissionAmt = totalAmt;
                ObjectFactory<ICommissionSettlementDA>.Instance.Update(commissionSettlementInfo);
                ObjectFactory<ICpsUserDA>.Instance.AddUserBalanceAmt(userSysNo, totalAmt);
            }
        }

        private List<CommissionSettlementItemSOItemCalculateInfo> GetSOCalculateInfo(int orderSysNo)
        {
            var commissionSettlementItemSOItemCalculateInfoList =
                new List<CommissionSettlementItemSOItemCalculateInfo>();
            var soInfo = ExternalDomainBroker.GetSOInfo(orderSysNo);
            foreach (var item in soInfo.Items)
            {
                var commissionSettlementItemSOItemCalculateInfo =
                    new CommissionSettlementItemSOItemCalculateInfo();
                var calculatePrice = item.Price + item.CouponAverageDiscount;
                commissionSettlementItemSOItemCalculateInfo.Price = calculatePrice.HasValue ? calculatePrice.Value : 0;
                commissionSettlementItemSOItemCalculateInfo.Qty = item.Quantity.HasValue ? item.Quantity.Value : 0;
                if (item.ItemExtList.Exists(ext => ext.Type == SOProductActivityType.GroupBuy))
                {
                    commissionSettlementItemSOItemCalculateInfo.Percentage = DefaultPercentage;
                }
                else
                {
                    if (soInfo.InvoiceInfo.InvoiceType == InvoiceType.MET
                        || soInfo.ShippingInfo.ShippingType == DeliveryType.MET
                        || soInfo.ShippingInfo.StockType == StockType.MET)
                    {
                        continue;
                    }

                    if (!item.C3SysNo.HasValue)
                    {
                        throw new BizException("SO Item C3 Has No Value");
                    }
                    var c1 = ExternalDomainBroker.GetCategory1ByCategory3(item.C3SysNo.Value);
                    if (!c1.SysNo.HasValue)
                    {
                        throw new BizException("C3 " + item.C3SysNo + " Has No C1");
                    }
                    var c1SysNo = c1.SysNo.Value;
                    List<CommissionPercentage> commissionPercentageList =
                        ObjectFactory<ICommissionPercentageDA>.Instance.GetByC1SysNo(c1SysNo);
                    if (commissionPercentageList.Count == 0)
                    {
                        commissionPercentageList = ObjectFactory<ICommissionPercentageDA>.Instance.GetByDefault();
                    }
                    commissionSettlementItemSOItemCalculateInfo.Percentage =
                        commissionPercentageList.First().Percentage;
                }
                commissionSettlementItemSOItemCalculateInfoList.Add(commissionSettlementItemSOItemCalculateInfo);
            }
            return commissionSettlementItemSOItemCalculateInfoList;
        }

        private List<CommissionSettlementItemSOItemCalculateInfo> GetRMACalculateInfo(int orderSysNo)
        {
            var commissionSettlementItemSOItemCalculateInfoList =
                new List<CommissionSettlementItemSOItemCalculateInfo>();
            var refundInfo = ExternalDomainBroker.GetRefund(orderSysNo);
            if (!refundInfo.SysNo.HasValue || !refundInfo.SOSysNo.HasValue)
            {
                throw new BizException("RefundInfo Error Or Has No SOSysNo");
            }
            var soInfo = ExternalDomainBroker.GetSOInfo(refundInfo.SOSysNo.Value);
            foreach (var item in refundInfo.RefundItems)
            {
                var commissionSettlementItemSOItemCalculateInfo =
                    new CommissionSettlementItemSOItemCalculateInfo();
                var calculatePrice = item.RefundCash + item.RefundPoint / 10 + item.OrgGiftCardAmt;
                commissionSettlementItemSOItemCalculateInfo.Price = calculatePrice.HasValue ? calculatePrice.Value : 0;
                commissionSettlementItemSOItemCalculateInfo.Qty = 1;

                var soItem = soInfo.Items.Find(i => i.SysNo == item.SysNo);

                if (soItem.ItemExtList.Exists(ext => ext.Type == SOProductActivityType.GroupBuy))
                {
                    commissionSettlementItemSOItemCalculateInfo.Percentage = DefaultPercentage;
                }
                else
                {
                    if (soInfo.InvoiceInfo.InvoiceType == InvoiceType.MET
                        || soInfo.ShippingInfo.ShippingType == DeliveryType.MET
                        || soInfo.ShippingInfo.StockType == StockType.MET)
                    {
                        continue;
                    }

                    if (!soItem.C3SysNo.HasValue)
                    {
                        throw new BizException("SO Item C3 Has No Value");
                    }
                    var c1 = ExternalDomainBroker.GetCategory1ByCategory3(soItem.C3SysNo.Value);
                    if (!c1.SysNo.HasValue)
                    {
                        throw new BizException("C3 " + soItem.C3SysNo + " Has No C1");
                    }
                    var c1SysNo = c1.SysNo.Value;
                    List<CommissionPercentage> commissionPercentageList =
                        ObjectFactory<ICommissionPercentageDA>.Instance.GetByC1SysNo(c1SysNo);
                    if (commissionPercentageList.Count == 0)
                    {
                        commissionPercentageList = ObjectFactory<ICommissionPercentageDA>.Instance.GetByDefault();
                    }
                    commissionSettlementItemSOItemCalculateInfo.Percentage =
                        commissionPercentageList.First().Percentage;
                }
                commissionSettlementItemSOItemCalculateInfoList.Add(commissionSettlementItemSOItemCalculateInfo);
            }
            return commissionSettlementItemSOItemCalculateInfoList;
        }

        public List<int> GetPendingCommissionSettlement()
        {
            return ObjectFactory<ICommissionSettlementDA>.Instance.GetPendingCommissionSettlementUserList();
        }

        public void ProcessUserAutoSettledCommissionInfo(int userSysNo)
        {
            var commissionSettlementList =
                ObjectFactory<ICommissionSettlementDA>.Instance.GetPendingCommissionSettlementByUserSysNo(userSysNo);
            decimal? total = commissionSettlementList.Sum(x => x.CommissionAmt);
            if (total.Value >= 100)
            {
                var userInfo = ObjectFactory<ICpsUserDA>.Instance.GetCpsUser(userSysNo);
                if (userInfo != null)
                {
                    var commissionToCashRecord = BuildCommissionToCashRecord(userInfo, total.Value);
                    if (commissionToCashRecord.CanProvideInvoice == CanProvideInvoice.No)
                    {
                        commissionToCashRecord.AfterTaxAmt =
                            CalculationPersonalIncomeTax(commissionToCashRecord.ToCashAmt);
                    }
                    else
                    {
                        commissionToCashRecord.AfterTaxAmt = commissionToCashRecord.ToCashAmt;
                    }
                    ObjectFactory<ICommissionToCashDA>.Instance.Insert(commissionToCashRecord);
                    ObjectFactory<ICpsUserDA>.Instance.AddUserBalanceAmt(userSysNo, -commissionToCashRecord.ToCashAmt);
                    commissionSettlementList.ForEach(i =>
                        {
                            if (i.SysNo.HasValue)
                                ObjectFactory<ICommissionSettlementDA>.Instance.UpdateCashRecord(i.SysNo.Value,
                                                                                                 commissionToCashRecord
                                                                                                     .SysNo);
                        });
                }
            }
        }

        private CommissionToCashInfo BuildCommissionToCashRecord(CpsUserInfo cpsUserInfo, decimal requestAmount)
        {
            if (String.IsNullOrEmpty(cpsUserInfo.ReceivablesAccount.BrankCode)
                || String.IsNullOrEmpty(cpsUserInfo.ReceivablesAccount.BranchBank)
                || String.IsNullOrEmpty(cpsUserInfo.ReceivablesAccount.BrankCardNumber)
                || String.IsNullOrEmpty(cpsUserInfo.ReceivablesAccount.ReceiveablesName))
            {
                throw new BizException(string.Format("用户{0}的收款账户信息不完整，请完善收款信息后再提交兑现申请！", cpsUserInfo.UserBasicInfo.CPSCustomerID));
            }

            if (cpsUserInfo.BalanceAmt < 0)
            {
                throw new BizException(string.Format("用户{0}申请兑现时，账户余额不能少于0元！", cpsUserInfo.UserBasicInfo.CPSCustomerID));
            }

            var commissionSettlementList =
                ObjectFactory<ICommissionSettlementDA>.Instance.GetUnRequestCommissionSettlementList(cpsUserInfo.SysNo);
            var unRequestAmt = commissionSettlementList.Sum(i => i.CommissionAmt);

            if (cpsUserInfo.BalanceAmt + requestAmount + unRequestAmt < 0)
            {
                throw new BizException(string.Format("用户{3}申请兑现后的余额账户不能少于0元！账户余额{0}元,本次申请金额{1}元,未提交申请{2}元", cpsUserInfo.BalanceAmt, requestAmount, unRequestAmt, cpsUserInfo.UserBasicInfo.CPSCustomerID));
            }

            return new CommissionToCashInfo
                {
                    CPSUserInfo = cpsUserInfo,
                    Status = ToCashStatus.Requested,
                    ToCashAmt = requestAmount,
                    BankCode = cpsUserInfo.ReceivablesAccount.BrankCode,
                    BankName = cpsUserInfo.ReceivablesAccount.BrankName,
                    BranchBank = cpsUserInfo.ReceivablesAccount.BranchBank,
                    BankCardNumber = cpsUserInfo.ReceivablesAccount.BrankCardNumber,
                    ReceivableName = cpsUserInfo.ReceivablesAccount.ReceiveablesName,
                    CanProvideInvoice = cpsUserInfo.CanProvideInvoice,
                    User = new UserInfo { SysNo = cpsUserInfo.SysNo }
                };
        }

        private decimal CalculationPersonalIncomeTax(decimal originalAmt)
        {
            decimal taxAmt = 0.00m;         //税费
            decimal taxableIncome = 0.00m;  //应纳税所得额


            //计算应纳税所得额
            if (originalAmt <= 800)
            {
                taxableIncome = 0;
            }
            else if (originalAmt > 800 && originalAmt <= 4000)
            {
                taxableIncome = originalAmt - 800;
            }
            else if (originalAmt > 4000)
            {
                taxableIncome = originalAmt * 0.8m;
            }

            //计算个税
            if (taxableIncome <= 20000)
            {
                taxAmt = taxableIncome * 0.2m;
            }
            else if (taxableIncome > 20000 && taxableIncome <= 50000)
            {
                taxAmt = taxableIncome * 0.3m - 2000m;
            }
            else if (taxableIncome > 50000)
            {
                taxAmt = taxableIncome * 0.4m - 7000m;
            }

            decimal afterTaxAmt = originalAmt - taxAmt;

            return afterTaxAmt;
        }
    }
}
