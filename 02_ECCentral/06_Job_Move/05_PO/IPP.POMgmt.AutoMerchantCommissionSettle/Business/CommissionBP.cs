using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using MerchantCommissionSettle.Components;
using MerchantCommissionSettle.Components.Configuration;
using MerchantCommissionSettle.DataAccess;
using MerchantCommissionSettle.Entities;

using SaleRuleItem = MerchantCommissionSettle.Entities.SalesRuleEntity.SaleRuleItem;
using System.Transactions;
//??using IPP.Oversea.CN.InvoiceMgmt.ServiceInterfaces.DataContracts;
//??using IPP.Oversea.CN.InvoiceMgmt.ServiceInterfaces.ServiceContracts;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Job.Utility;

namespace MerchantCommissionSettle.Business
{
    internal sealed class CommissionBP
    {
        #region 实例字段

        private static JobContext CurrentContext;
        private DateTime now = DateTime.Now;
        private DateTime maxOrderEndData = DateTime.Now;
        private int runVendorSysno = 0;

        public Action<string> DisplayMessage;

        private CommissionDA dal;

        private Dictionary<int, CommissionRule> rules;

        private List<Vendor> merchants;

        private ScheduleConfigurationSection config;

        Dictionary<int, string> payPeriods;

        #endregion

        #region 构造函数

        public CommissionBP(JobContext context)
        {
            CurrentContext = context;

            #region 修复数据Code
            //<-增加手动结算的修复处理 2012-4-24
            this.now = DateTime.Now;
            this.maxOrderEndData = DateTime.Now;
            this.runVendorSysno = 0;

            //从上下文中获取修复参数
            if (CurrentContext != null)
            {
                //当前日期（用于修复数据使用，平时都为空）
                if (CurrentContext.Properties.ContainsKey("CurrentDay"))
                {
                    now = Convert.ToDateTime(CurrentContext.Properties["CurrentDay"]);
                }

                //最大截至日期（用于修复数据使用，平时都为空）
                if (CurrentContext.Properties.ContainsKey("MaxOrderEndData"))
                {
                    maxOrderEndData = Convert.ToDateTime(CurrentContext.Properties["MaxOrderEndData"]);                    
                }

                //要修复供应商编号（用于修复数据使用，平时都为空）
                if (CurrentContext.Properties.ContainsKey("RunVendorSysno"))
                {
                    runVendorSysno = Convert.ToInt32(CurrentContext.Properties["RunVendorSysno"]);
                }
            }
            //->
            #endregion

            dal = new CommissionDA();

            rules = new Dictionary<int, CommissionRule>();

            config = (ScheduleConfigurationSection)ConfigurationManager.GetSection("schedules");

            payPeriods = new Dictionary<int, string>();

            foreach (ScheduleElement item in config.MonthlySchedule)
            {
                payPeriods.Add(item.ConsignToAccType, item.DayOfMonth);
            }

            var basicRules = dal.GetCommissionRules();
            
            foreach (var item in basicRules)
            {
                rules.Add(item.VendorManufacturerSysNo, item);
            }

            //获取当前日期的结算类型
            var types = GetConsignToAccTypes(now);

            if (types != null && types.Count > 0)
            {
                merchants = dal.GetVendorByPayPeriodType(types);
            }
            else
            {
                merchants = new List<Vendor>();
            }
        }

        #endregion

        #region 发送邮件

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        public void SendMail(string mailSubject, string mailBody)
        {
            dal.SendEmail(GlobalSettings.MailAddress, mailSubject, mailBody, 0);
        }

        #endregion

        #region 显示消息

        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message">消息字符串</param>
        private void OnDisplayMessage(string message)
        {
            if (this.DisplayMessage != null)
            {
                DisplayMessage(message);
            }
        }

        #endregion

        #region 获取当前日期需要处理的供应商账期类型

        /// <summary>
        /// 获取当前日期需要处理的供应商账期类型
        /// </summary>
        /// <returns>供应商账期类型列表</returns>
        private List<int> GetConsignToAccTypes(DateTime now)
        {            
            var typeList = new List<int>();

            foreach (ScheduleElement item in config.MonthlySchedule)
            {
                if (string.IsNullOrEmpty(item.DayOfMonth))
                {
                    continue;
                }

                foreach (var originValue in item.DayOfMonth.Split(','))
                {
                    int day;

                    if (Int32.TryParse(originValue, out day))
                    {
                        if (now.Day == day)
                        {
                            typeList.Add(item.ConsignToAccType);
                        }
                    }
                }
            }

            return typeList;
        }

        #endregion

        #region 获取供应商佣金的开始与结束时间

        private void SetCommissionTimeInfo(CommissionMaster master,DateTime now)
        {
            Vendor vendor = dal.GetVendorBySysNo(master.MerchantSysNo);
            DateTime startTime, endTime;

            if (!payPeriods.ContainsKey(vendor.PayPeriodType))
            {
                throw new Exception(string.Format("供应商{0} 账期{1}  没有找到对应的账期配置信息", vendor.SysNo, vendor.PayPeriodType));
            }

            var rule = payPeriods[vendor.PayPeriodType];

            var daysOfMonth = rule.Split(',').Select(x => Convert.ToInt32(x)).ToList();

            if (daysOfMonth.Count > 2 || daysOfMonth.Count == 0)
            {
                throw new NotSupportedException("账期配置错误,当前版本仅支持半月结与整月结");
            }
            else if (daysOfMonth.Count == 1)
            {
                if (daysOfMonth[0] < 0 || daysOfMonth[0] > 31)
                {
                    throw new OverflowException(string.Format("{0}不是合法的日期,应该为1 ~ 30之间的整数", daysOfMonth[0]));
                }

                if (now.Day <= daysOfMonth[0])
                {
                    startTime = new DateTime(now.Year, now.Month, daysOfMonth[0]).AddMonths(-1);
                }
                else
                {
                    startTime = new DateTime(now.Year, now.Month, daysOfMonth[0]);
                }

                endTime = startTime.AddMonths(1);
                master.Percentage = 1m;
            }
            else
            {
                var min = daysOfMonth.Min();
                var max = daysOfMonth.Max();

                if (min < 0 || max > 30)
                {
                    throw new OverflowException(string.Format("Start:{0} End:{1}不是合法的日期,应该为1 ~ 30之间的整数", min, max));
                }

                if (now.Day <= min)
                {
                    startTime = new DateTime(now.Year, now.Month, max).AddMonths(-1);
                    endTime = new DateTime(now.Year, now.Month, min);
                }
                else if (now.Day <= max)
                {
                    startTime = new DateTime(now.Year, now.Month, min);
                    endTime = new DateTime(now.Year, now.Month, max);
                }
                else
                {
                    startTime = new DateTime(now.Year, now.Month, max);
                    endTime = new DateTime(now.Year, now.Month, min).AddMonths(1);
                }

                master.Percentage = 0.5m;
            }

            master.BeginDate = startTime;
            master.EndDate = endTime;
        }

        #endregion

        #region 为没有找到代理信息的数据发送邮件提醒

        public void SendMailForProductWithNoVendorManufacturer(IEnumerable<CommissionLog> items)
        {
            var vendors = from item in items
                          group item by item.MerchantSysNo into g
                          select g;

            foreach (var itemsForVendor in vendors)
            {
                if (merchants.Select(x => x.SysNo).Contains(itemsForVendor.Key))
                {
                    StringBuilder sbMessage = new StringBuilder();
                    CommissionMaster master = new CommissionMaster { MerchantSysNo = itemsForVendor.Key };
                    SetCommissionTimeInfo(master,DateTime.Now);

                    sbMessage.AppendLine(string.Format("供应商系统编号:{0} 账期{1:yyyy-MM-dd} --> {2:yyyy-MM-dd}", master.MerchantSysNo, master.BeginDate, master.EndDate));
                    foreach (var product in itemsForVendor)
                    {
                        sbMessage.AppendLine(string.Format("订单类型:{0}  订单编号:{1} 商品系统编号:{2}", product.ReferenceType, product.ReferenceSysNo, product.ProductSysNo));
                    }

                    dal.SendEmail(GlobalSettings.AlertMailAddress, GlobalSettings.AlertMailSubject, sbMessage.ToString(), 0);
                }
            }
        }

        #endregion

        #region 计算佣金

        /// <summary>
        /// 计算佣金
        /// </summary>
        public void SettleCommission()
        { 
            //<-增加手动结算的修复处理 2012-4-24
            //var newItem = dal.GetCommissionLog(payPeriods.Keys.ToList());
            //var existingItem = dal.GetExistingCommissionLog();
            var newItem = dal.GetCommissionLog(payPeriods.Keys.ToList(), this.runVendorSysno, this.maxOrderEndData);
            var existingItem = dal.GetExistingCommissionLog(this.runVendorSysno);
            //->

            var items = newItem.Union(existingItem);

            var itemsWithoutVendorManufacturer = from item in items
                                                 where item.VendorManufacturerSysNo == 0
                                                 select item;

            //为已经到达结算账期的错误数据发送提醒邮件
            SendMailForProductWithNoVendorManufacturer(itemsWithoutVendorManufacturer);

            //筛选掉没有找到供代理信息的记录
            var filteredItems = items.Except(itemsWithoutVendorManufacturer);
            var vendors = from item in filteredItems
                          group item by item.MerchantSysNo into g
                          select g;

            foreach (var vendor in vendors)
            {
                 ComputeCommissionForVendor(vendor);
            }
        }

        private void ComputeCommissionForVendor(IGrouping<int, CommissionLog> vendor)
        {
            CommissionMaster master;

            try
            {
                if (vendor.Key == 0)
                {
                    throw new Exception(string.Format("错误数据,VendorSysNo为0,ProductSysNo:{0}", String.Join(",", vendor.Select(x => x.ProductSysNo.ToString()).ToArray())));
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    #region 计算供应商的佣金

                    master = dal.GetCommissionMasterByMerchantSysNo(vendor.Key);

                    bool isOnlyCalcRentFee = IsOnlyCalculateRentFee(master);

                    //计算店租佣金
                    //<-增加手动结算的修复处理 2012-4-24
                    //SetCommissionTimeInfo(master);
                    SetCommissionTimeInfo(master,this.now);
                    //->
                    SetTotalRent(master);

                    if (!isOnlyCalcRentFee)
                    {
                        var itemsForComputing = new List<CommissionItem>();
                        var logsForVendor = new List<CommissionLog>();

                        var agentGroup = from i in vendor
                                         group i by i.VendorManufacturerSysNo into g
                                         select g;

                        foreach (var agent in agentGroup)
                        {
                            var item = dal.GetCommissionItemByVMSysNo(agent.Key, master.SysNo);
                            itemsForComputing.Add(item);
                            var logsForItem = new List<CommissionLog>();
                            CommissionRule rule = null;

                            foreach (var itemWithSameAgent in agent)
                            {

                                #region 计算代理信息的佣金

                                if (rules.ContainsKey(itemWithSameAgent.VendorManufacturerSysNo))
                                {
                                    rule = rules[itemWithSameAgent.VendorManufacturerSysNo];

                                    if (itemWithSameAgent.ReferenceSysNo > 0)
                                    {
                                        if (itemWithSameAgent.SysNo.HasValue)
                                        {
                                            #region 更新log

                                            switch (itemWithSameAgent.Type)
                                            {
                                                case Constants.CommissionLogType.OrderCommission:
                                                    if (itemWithSameAgent.ReferenceType == Constants.OrderType.SO)
                                                    {
                                                        itemWithSameAgent.CommissionAmt = rule.OrderCommissionFee;
                                                        itemWithSameAgent.Price = rule.OrderCommissionFee;
                                                    }
                                                    else if (itemWithSameAgent.HaveAutoRMA == 1)
                                                    {
                                                        itemWithSameAgent.CommissionAmt = -rule.OrderCommissionFee;
                                                        itemWithSameAgent.Price = -rule.OrderCommissionFee;
                                                        //SetOrderCommissionForRMA(rule, itemWithSameAgent);
                                                    }
                                                    break;
                                                case Constants.CommissionLogType.SaleCommission:
                                                    if (itemWithSameAgent.ReferenceType == Constants.OrderType.SO)
                                                    {
                                                        var salesPrice = itemWithSameAgent.Price - itemWithSameAgent.Point / 10m - itemWithSameAgent.DiscountAmt / itemWithSameAgent.Qty;
                                                        itemWithSameAgent.Price = salesPrice;
                                                        decimal? promotiondiscount = itemWithSameAgent.PromotionDiscount.HasValue ? itemWithSameAgent.PromotionDiscount : 0;//优惠券折扣
                                                        itemWithSameAgent.CommissionAmt = Math.Round((itemWithSameAgent.Price * itemWithSameAgent.Qty) + promotiondiscount.Value, 2);
                                                    }
                                                    else
                                                    {
                                                        var salesPrice = itemWithSameAgent.Price - itemWithSameAgent.Point / 10m - itemWithSameAgent.DiscountAmt;
                                                        itemWithSameAgent.Price = salesPrice;
                                                        itemWithSameAgent.CommissionAmt = Math.Round(itemWithSameAgent.Price * itemWithSameAgent.Qty, 2);
                                                    }
                                                    break;
                                                case Constants.CommissionLogType.DeliveryFee:
                                                    itemWithSameAgent.CommissionAmt = rule.DeliveryFee;
                                                    itemWithSameAgent.Price = rule.DeliveryFee;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            dal.UpdateCommissionLog(itemWithSameAgent);
                                            logsForItem.Add(itemWithSameAgent);
                                            logsForVendor.Add(itemWithSameAgent);

                                            #endregion
                                        }
                                        else
                                        {
                                            #region 新增LOG

                                            if (itemWithSameAgent.ReferenceType == Constants.OrderType.SO)
                                            {
                                                #region SO

                                                //计算实际出售价格
                                                var salesPrice = itemWithSameAgent.Price - itemWithSameAgent.Point / 10m - itemWithSameAgent.DiscountAmt / itemWithSameAgent.Qty;
                                                itemWithSameAgent.Price = salesPrice;
                                                //优惠券折扣
                                                decimal? promotiondiscount = itemWithSameAgent.PromotionDiscount.HasValue ? itemWithSameAgent.PromotionDiscount : 0;


                                                //新建log
                                                CommissionLog orderCommission = new CommissionLog
                                                {
                                                    CommissionItemSysNo = item.SysNo,
                                                    ReferenceSysNo = itemWithSameAgent.ReferenceSysNo,
                                                    ReferenceType = itemWithSameAgent.ReferenceType,
                                                    ProductSysNo = itemWithSameAgent.ProductSysNo,
                                                    Price = rule.OrderCommissionFee,
                                                    InUser = GlobalSettings.UserName,
                                                    EditUser = GlobalSettings.UserName,
                                                    Type = Constants.CommissionLogType.OrderCommission,
                                                    CommissionAmt = rule.OrderCommissionFee
                                                };

                                                logsForItem.Add(orderCommission);
                                                logsForVendor.Add(orderCommission);

                                                //新建log
                                                CommissionLog deliveryFee = new CommissionLog
                                                {
                                                    CommissionItemSysNo = item.SysNo,
                                                    ReferenceSysNo = itemWithSameAgent.ReferenceSysNo,
                                                    ReferenceType = itemWithSameAgent.ReferenceType,
                                                    ProductSysNo = itemWithSameAgent.ProductSysNo,
                                                    Price = rule.DeliveryFee,
                                                    InUser = GlobalSettings.UserName,
                                                    EditUser = GlobalSettings.UserName,
                                                    Type = Constants.CommissionLogType.DeliveryFee,
                                                    CommissionAmt = rule.DeliveryFee
                                                };

                                                logsForItem.Add(deliveryFee);
                                                logsForVendor.Add(deliveryFee);

                                                //新建log
                                                CommissionLog salesCommission = new CommissionLog
                                                {
                                                    CommissionItemSysNo = item.SysNo,
                                                    ReferenceSysNo = itemWithSameAgent.ReferenceSysNo,
                                                    ReferenceType = itemWithSameAgent.ReferenceType,
                                                    ProductSysNo = itemWithSameAgent.ProductSysNo,
                                                    Price = itemWithSameAgent.Price,
                                                    Qty = itemWithSameAgent.Qty,
                                                    InUser = GlobalSettings.UserName,
                                                    EditUser = GlobalSettings.UserName,
                                                    Type = Constants.CommissionLogType.SaleCommission,
                                                    CommissionAmt = Math.Round((itemWithSameAgent.Price * itemWithSameAgent.Qty) + promotiondiscount.Value, 2)
                                                    ,
                                                    PromotionDiscount = promotiondiscount.Value
                                                };

                                                dal.CreateCommissionLog(salesCommission);
                                                logsForItem.Add(salesCommission);
                                                logsForVendor.Add(salesCommission);

                                                #endregion
                                            }
                                            else
                                            {
                                                #region RMA

                                                var salesPrice = itemWithSameAgent.Price - itemWithSameAgent.Point / 10m - itemWithSameAgent.DiscountAmt;
                                                itemWithSameAgent.Price = salesPrice;

                                                //物流拒收需要退回订单提成
                                                if (itemWithSameAgent.HaveAutoRMA == 1)
                                                {
                                                    //新建log
                                                    CommissionLog orderCommission = new CommissionLog
                                                    {
                                                        CommissionItemSysNo = item.SysNo,
                                                        ReferenceSysNo = itemWithSameAgent.ReferenceSysNo,
                                                        ReferenceType = itemWithSameAgent.ReferenceType,
                                                        ProductSysNo = itemWithSameAgent.ProductSysNo,
                                                        Price = -rule.OrderCommissionFee,
                                                        InUser = GlobalSettings.UserName,
                                                        EditUser = GlobalSettings.UserName,
                                                        Type = Constants.CommissionLogType.OrderCommission,
                                                        CommissionAmt = -rule.OrderCommissionFee
                                                    };

                                                    logsForItem.Add(orderCommission);
                                                    logsForVendor.Add(orderCommission);
                                                }

                                                //新建log
                                                CommissionLog salesCommission = new CommissionLog
                                                {
                                                    CommissionItemSysNo = item.SysNo,
                                                    ReferenceSysNo = itemWithSameAgent.ReferenceSysNo,
                                                    ReferenceType = itemWithSameAgent.ReferenceType,
                                                    ProductSysNo = itemWithSameAgent.ProductSysNo,
                                                    Price = itemWithSameAgent.Price,
                                                    Qty = itemWithSameAgent.Qty,
                                                    InUser = GlobalSettings.UserName,
                                                    EditUser = GlobalSettings.UserName,
                                                    Type = Constants.CommissionLogType.SaleCommission,
                                                    CommissionAmt = Math.Round(itemWithSameAgent.Price * itemWithSameAgent.Qty, 2)
                                                };

                                                dal.CreateCommissionLog(salesCommission);
                                                logsForItem.Add(salesCommission);
                                                logsForVendor.Add(salesCommission);

                                                #endregion
                                            }

                                            #endregion
                                        }
                                    }

                                    //记录使用的佣金规则编号
                                    item.RuleSysNo = rule.SysNo;
                                }

                                #endregion
                            }

                            if (rule != null)
                            {
                                item.TotalSaleAmt = Math.Round(logsForItem
                                    .Where(x =>
                                    {
                                        return x.Type == Constants.CommissionLogType.SaleCommission;
                                    })
                                    .Sum(x => x.CommissionAmt), 2);

                                if (rule.SalesRule == null) {
                                    break;
                                }
                                var salesRule = rule.SalesRule.ToObject<SalesRuleEntity>();
                                item.Rent = rule.RentFee * master.Percentage;
                                if (salesRule != null)
                                {
                                    item.SalesCommissionFee = Math.Max(GetSaleCommissionAmount(item.TotalSaleAmt, salesRule), salesRule.MinCommissionAmt);
                                }
                                item.EditUser = GlobalSettings.UserName;
                            }
                        }

                        #region 计算订单提成

                        var queryOrderCommission = from i in logsForVendor
                                                   where i.Type == Constants.CommissionLogType.OrderCommission
                                                   group i by new { i.ReferenceSysNo, i.ReferenceType } into g
                                                   select g;

                        List<CommissionLog> orderFee = new List<CommissionLog>();
                        foreach (var orderCommission in queryOrderCommission)
                        {
                            if (orderCommission.Key.ReferenceType == Constants.OrderType.SO)
                            {
                                var soList = from i in orderCommission
                                             where i.ReferenceType == Constants.OrderType.SO
                                             orderby i.CommissionAmt descending
                                             select i;

                                orderFee.Add(soList.First());
                            }
                        }

                        foreach (var orderCommission in queryOrderCommission)
                        {
                            if (orderCommission.Key.ReferenceType == Constants.OrderType.RMA)
                            {
                                var rmaList = from i in orderCommission
                                              where i.ReferenceType == Constants.OrderType.RMA
                                              orderby i.CommissionAmt
                                              select i;
                                var rma = rmaList.First();
                                var so1 = dal.GetOrderCommissionLog(rma.SoSysNo);
                                var so2 = orderFee.Where(so => so.Type == Constants.OrderType.SO && so.ReferenceSysNo == rma.SoSysNo).OrderByDescending(so => so.Price).FirstOrDefault();

                                if (so2 != null)
                                {
                                    rma.CommissionItemSysNo = so2.CommissionItemSysNo;
                                    rma.Price = -so2.Price;
                                }
                                else if (so1 != null)
                                {
                                    rma.CommissionItemSysNo = so1.CommissionItemSysNo;
                                    rma.Price = -so1.Price;
                                }

                                orderFee.Add(rma);
                            }
                        }

                        #endregion

                        #region 计算配送费

                        var queryDeliveryFee = from i in logsForVendor
                                               where i.Type == Constants.CommissionLogType.DeliveryFee
                                               group i by new { i.ReferenceSysNo, i.ReferenceType } into g
                                               select g;

                        List<CommissionLog> delivery = new List<CommissionLog>();

                        foreach (var deliveryCommission in queryDeliveryFee)
                        {
                            var soList = from i in deliveryCommission
                                         orderby i.CommissionAmt descending
                                         select i;

                            var so = soList.FirstOrDefault();

                            if (so != null)
                            {
                                delivery.Add(so);
                            }
                        }

                        #endregion

                        #region 更新SOC DEF LOG

                        foreach (var item in itemsForComputing)
                        {
                            var ordersBelongToItem = from log in orderFee
                                                     where log.CommissionItemSysNo == item.SysNo
                                                     select log;

                            item.OrderCommissionFee = ordersBelongToItem.Sum(x => x.CommissionAmt);

                            dal.DeleteCommissionLog(item.SysNo, Constants.CommissionLogType.OrderCommission);

                            foreach (var log in ordersBelongToItem)
                            {
                                dal.CreateCommissionLog(log);
                            }

                            var deliverysBelongToItem = from log in delivery
                                                        where log.CommissionItemSysNo == item.SysNo
                                                        select log;

                            dal.DeleteCommissionLog(item.SysNo, Constants.CommissionLogType.DeliveryFee);

                            foreach (var log in deliverysBelongToItem)
                            {
                                dal.CreateCommissionLog(log);
                            }

                            item.DeliveryFee = deliverysBelongToItem.Sum(x => x.CommissionAmt);

                            dal.UpdateCommissionItem(item);
                        }

                        #endregion

                        master.OrderCommissionFee = itemsForComputing.Sum(x => x.OrderCommissionFee);
                        master.DeliveryFee = itemsForComputing.Sum(x => x.DeliveryFee);
                        master.SalesCommissionFee = itemsForComputing.Sum(x => x.SalesCommissionFee);
                    }
                    master.TotalAmt = master.DeliveryFee + master.OrderCommissionFee + master.RentFee + master.SalesCommissionFee;

                    dal.UpdateCommissionMaster(master);

                    scope.Complete();

                    #endregion
                }

                if (merchants.Select(x => x.SysNo).Contains(master.MerchantSysNo))
                {
                    SettleCommission(master);
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message + Environment.NewLine + ex.StackTrace;
                OnDisplayMessage(message);
                SendMail(GlobalSettings.MailSubject, message);
            }
        }

        private void SetOrderCommissionForRMA(CommissionRule rule, CommissionLog item)
        {
            var orderCommission = dal.GetOrderCommissionLog(item.SoSysNo);

            if (orderCommission == null)
            {
                item.CommissionAmt = -rule.OrderCommissionFee;
            }
            else
            {
                item.CommissionItemSysNo = orderCommission.CommissionItemSysNo;
                item.CommissionAmt = -orderCommission.Price;
            }

            item.Price = item.CommissionAmt;
        }

        bool IsOnlyCalculateRentFee(CommissionMaster master)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(GlobalSettings.OnlyCalcRentFeeAccType))
            { 
                Vendor vendor = dal.GetVendorBySysNo(master.MerchantSysNo);
                result = GlobalSettings.OnlyCalcRentFeeAccType.Split(',').Select(p => int.Parse(p)).Contains(vendor.PayPeriodType);
            }

            return result;
        }

        #endregion

        #region 计算店租佣金

        /// <summary>
        /// 计算总店租佣金
        /// </summary>
        /// <param name="master"></param>
        private void SetTotalRent(CommissionMaster master)
        {
            var rulesForVendor = dal.GetCommissionRulesByMerchantSysNo(master.MerchantSysNo);
            master.RentFee = rulesForVendor.Sum(x => x.RentFee) * master.Percentage;

            foreach (var rule in rulesForVendor)
            {
                var item = dal.GetCommissionItemByVMSysNo(rule.VendorManufacturerSysNo, master.SysNo);
                item.RuleSysNo = rule.SysNo;
                item.Rent = rule.RentFee * master.Percentage;

                dal.UpdateCommissionItem(item);
            }
        }

        #endregion

        #region 结算已经到账期的佣金单据

        /// <summary>
        /// 结算已经到账期的佣金单据
        /// </summary>
        /// <param name="master"></param>
        private void SettleCommission(CommissionMaster master)
        {
            DisplayMessage(string.Format("佣金结算单{0}调用财务接口",master.SysNo));

            master.Status = Constants.CommissionMasterStatus.Settled;
            master.EndDate = DateTime.Now;
            dal.UpdateCommissionMaster(master);
            CreatePayItem(master);
            dal.SettleCommission(master);
        }

        #endregion

        #region 计算销售提成

        /// <summary>
        /// 计算销售提成
        /// </summary>
        /// <param name="saleAmt"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static decimal GetSaleCommissionAmount(decimal saleAmt, SalesRuleEntity rule)
        {
            var orderRules = from item in rule.Rules
                             orderby item.Order descending
                             select item;

            rule.Rules.Sort(delegate(SaleRuleItem x, SaleRuleItem y)
                            {
                                return y.Order - x.Order;
                            });

            var commissionList = new decimal[rule.Rules.Count];
            for (int i = 0; i < rule.Rules.Count; i++)
            {
                var ruleItem = rule.Rules[i];

                if (saleAmt > ruleItem.StartAmt)
                {
                    decimal minAmt = ruleItem.EndAmt.HasValue && ruleItem.EndAmt.Value != 0 ? Math.Min(saleAmt, ruleItem.EndAmt.Value) : saleAmt;

                    commissionList[i] = (minAmt - ruleItem.StartAmt) * ((Decimal)ruleItem.Percentage / 100m);
                }
            }

            return commissionList.Sum();
        }

        #endregion

        #region 调用财务接口

        public void CreatePayItem(CommissionMaster master)
        {

            //获取配置参数
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["InvoiceRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            int? UserSysNo = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UserSysNo"]);


            #region 转化数据
            PayItemInfo payItem = new PayItemInfo();

            payItem = new PayItemInfo()
            {
                OrderSysNo = master.SysNo,
                Note = "自动结算佣金",
                PayAmt = -Math.Round(master.TotalAmt, 2),
                OrderType = PayableOrderType.Commission,
                EditUserSysNo = UserSysNo,
                PayStyle = PayItemStyle.Normal,
                BatchNumber = 1,
                CompanyCode = GlobalSettings.CompanyCode
            };
            #endregion

            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Create("/PayItem/Create", payItem, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = string.Empty;
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");
            }

            #region FINISH 服务调用 //??
            //ICreatePayItemV31 service = null;

            //try
            //{
            //    service = ServiceBroker.FindService<ICreatePayItemV31>();

            //    if (service == null)
            //    {
            //        throw new InvalidOperationException("未找到ICreatePayItemV31服务");
            //    }

            //    PayItemV31 payItem = new PayItemV31();

            //    payItem.Header = new Newegg.Oversea.Framework.Contract.MessageHeader
            //    {
            //        CompanyCode = GlobalSettings.CompanyCode,
            //        OperationUser = new Newegg.Oversea.Framework.Contract.OperationUser
            //        {
            //            CompanyCode = GlobalSettings.CompanyCode,
            //            FullName = GlobalSettings.UserName,
            //            LogUserName = GlobalSettings.UserName,
            //            SourceDirectoryKey = GlobalSettings.SourceDirectoryKey,
            //            SourceUserName = GlobalSettings.UserName,
            //            UniqueUserName = GlobalSettings.UserName
            //        },
            //        StoreCompanyCode = GlobalSettings.StoreCompanyCode,
            //        FromSystem = GlobalSettings.FromIP
            //    };
                
            //    payItem.Body = new PayItemMessage
            //    {
            //        OrderSysNo = master.SysNo,
            //        Note = "自动结算佣金",
            //        PayAmt = -Math.Round(master.TotalAmt,2),
            //        OrderType = PayableOrderType.Commission,
            //        OperUserName = GlobalSettings.UserName,
            //        PayStyle = PayItemStyle.Normal,
            //        BatchNumber = 1
            //    };

            //    var result = service.Create(payItem);

            //    if (result.Faults != null && result.Faults.Count > 0)
            //    {
            //        SendMailForWCFFailure(result);
            //    }
            //}
            //catch(Exception ex)
            //{}
            //finally
            //{
            //    ServiceBroker.DisposeService<ICreatePayItemV31>(service);
            //}
            #endregion
        }

        #endregion

        #region 发送错误邮件

        private void SendMailForWCFFailure(PayItemInfo result)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("调用ICreatePayItemV31服务出错:");

            //result.Faults.ForEach(x =>
            //{
            //    messageBuilder.AppendLine(string.Format("ErrorCode:{0},ErrorMessage:{1},ErrorDetail:{2}", x.ErrorCode, x.ErrorDescription, x.ErrorDetail));
            //});

            SendMail(GlobalSettings.MailSubject, messageBuilder.ToString());
        }

        #endregion
    }
}