using System;
using System.Collections.Generic;
using System.Linq;
using POASNMgmt.AutoCreateVendorSettle.DataAccess;
using POASNMgmt.AutoCreateVendorSettle.Compoents;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using POASNMgmt.AutoCreateVendorSettle.Entities;
using System.Configuration;
using POASNMgmt.AutoCreateVendorSettle.Compoents.Configuration;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.Job.Utility;

namespace POASNMgmt.AutoCreateVendorSettle.Business
{
    internal sealed class VendorSettleBP
    {
        public VendorSettleBP()
        {
            RuleDic = new Dictionary<int, SettleRulesEntity>();
        }

        #region 实例字段

        private static VendorSettleDAL dal = new VendorSettleDAL();

        public Action<string> DisplayMessage;

        private static string NODATA_MESSAGE_TEMPLATE = "{0:yyyy-MM-dd}没有供应商需要生成代销结算单";

        #endregion

        #region 发送邮件

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        public void SendMail(string mailSubject, string mailBody)
        {
            dal.SendEmail(GlobalSettings.MailAddress, mailSubject, mailBody, 0, GlobalSettings.CompanyCode);
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

        #region 自动创建代销结算单

        /// <summary>
        /// 自动创建代销结算单
        /// </summary>
        public void CreateVendorSettle()
        {
            List<int> payPeriodTypes = GetConsignToAccTypes();

            if (payPeriodTypes == null || payPeriodTypes.Count == 0)
            {
                OnDisplayMessage(string.Format(NODATA_MESSAGE_TEMPLATE, DateTime.Now));

                return;
            }

            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("需要自动生成代销结算单的账期类型:" + String.Join(",", payPeriodTypes.Select(x => x.ToString()).ToArray()));
            OnDisplayMessage(messageBuilder.ToString());

            var acclogList = dal.GetConsginToAccLogList(payPeriodTypes);

            if (acclogList == null || acclogList.Count == 0)
            {
                OnDisplayMessage(string.Format(NODATA_MESSAGE_TEMPLATE, DateTime.Now));

                return;
            }

            var vendorSettleList = GetVendorSettleList(acclogList);

            CallSystemCreateService(vendorSettleList);
        }

        #endregion

        #region 获取当前日期需要处理的供应商账期类型

        /// <summary>
        /// 获取当前日期需要处理的供应商账期类型
        /// </summary>
        /// <returns>供应商账期类型列表</returns>
        private static List<int> GetConsignToAccTypes()
        {
            DateTime now = DateTime.Now;
            var config = (ScheduleConfigurationSection)ConfigurationManager.GetSection("schedules");
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

        #region 根据AccLog构建代销结算单

        /// <summary>
        /// 根据AccLog构建代销结算单
        /// </summary>
        /// <param name="acclogList">AccLog</param>
        /// <returns>代销结算单列表</returns>
        private List<ConsignSettlementInfo> GetVendorSettleList(List<ConsginToAccLogEntity> acclogList)
        {
            var consignSettlementInfoList = new List<ConsignSettlementInfo>();

            //检查SettleType,去除非法数据
            var invalidDataQuery = from item in acclogList
                                   where (item.SettleType.ToUpper() != "O" && item.SettleType.ToUpper() != "P" ||
                                          item.SettleType.ToUpper() == "P" && !item.SettlePercentage.HasValue)
                                   select item;

            var invalidList = invalidDataQuery.ToList();

            //如果存在错误数据,则发邮件提醒
            SendMailForInvalidData(invalidList);

            //排除错误数据
            var validDataQuery = acclogList.Except(invalidDataQuery);
            //Ray  新增 通过 产品线分组
            var groups = from item in validDataQuery
                         group item by new { item.VendorSysNo, item.VendorName, item.StockSysNo, item.ProductLineSysNo, item.CurrencySysNo, item.TaxRate, item.PMUserSysNo, item.PayPeriodType,item.IsToLease }
                             into team
                             select team;
            
            foreach (var group in groups)
            {
                ConsignSettlementInfo settle = null;

                var i = 0;
                foreach (var item in group)
                {
                    //每个代销结算单最多只能有MaxItemCount个Item
                    if (i % GlobalSettings.MaxItemCount == 0)
                    {
                        settle = new ConsignSettlementInfo();              

                        settle.CreateUserName = GlobalSettings.UserName;
                        settle.CreateUserSysNo = GlobalSettings.UserSysNo;
                        settle.CreateDate = DateTime.Now;
                        settle.CurrencyCode = group.Key.CurrencySysNo;
                        //settle.PayPeriodType = group.Key.PayPeriodType;

                        //settle.PM_ReturnPointSysNo = group.Key.PMUserSysNo;
                        settle.Status = SettleStatus.WaitingAudit;
                        //settle.StockName = group.Key.TaxRate.ToString();
                        settle.SourceStockInfo = new ECCentral.BizEntity.Inventory.StockInfo
                        {
                            SysNo = group.Key.StockSysNo
                        };

                        //settle.TaxRateData = group.Key.TaxRate;
                        settle.TaxRateData = PurchaseOrderTaxRate.Percent017;
                        //settle.VendorName = group.Key.VendorName;
                        //settle.VendorSysNo = group.Key.VendorSysNo;
                        settle.VendorInfo = new VendorInfo
                        {
                            SysNo = group.Key.VendorSysNo
                        };
                        settle.Note = "System Create";
                        settle.ConsignSettlementItemInfoList = new List<ConsignSettlementItemInfo>();
                        settle.PMInfo = new ECCentral.BizEntity.IM.ProductManagerInfo { SysNo = group.Key.PMUserSysNo };
                        settle.EIMSInfo = new ConsignSettlementEIMSInfo { ReturnPoint = 0 };
                        settle.CompanyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
                        settle.LeaseType =item.IsToLease == 0 ? VendorIsToLease.UnLease : VendorIsToLease.Lease;
                       
                        //生成新单据
                        consignSettlementInfoList.Add(settle);
                    }

                    ConsignSettlementItemInfo settleItem = new ConsignSettlementItemInfo();

                    //为SettleItem的属性赋值
                    SetItemValue(item, settleItem);
                    settle.ConsignSettlementItemInfoList.Add(settleItem);                   
                    i++;
                }
                //扣款项逻辑
                ProcessDeduct(settle);
            }

            //计算TotalAmt
            consignSettlementInfoList.ForEach(x =>
                x.TotalAmt = x.ConsignSettlementItemInfoList.Sum(y => y.Cost * y.Quantity)-x.DeductAmt
            );

            return consignSettlementInfoList;
        }
        private void ProcessDeduct(ConsignSettlementInfo settle)
        {
            DateTime begin = DateTime.Today.AddMonths(-1);
            settle.ConsignRange = begin.ToString("yyyy-MM");    
            VendorDeductEntity deduct = dal.GetVendorDeductInfo(settle.VendorInfo.SysNo.Value);
            if (deduct!=null)
            {
                switch (deduct.CalcType)
                {
                    case VendorCalcType.Fix:
                        settle.DeductAmt = deduct.FixAmt;
                        break;
                    case VendorCalcType.Cost:
                        settle.DeductAmt = settle.ConsignSettlementItemInfoList.Sum(p=>p.CreateCost.Value*p.Quantity.Value*deduct.DeductPercent);
                        break;
                    case VendorCalcType.Sale:
                        settle.DeductAmt = settle.ConsignSettlementItemInfoList.Sum(p=>p.RetailPrice*p.Quantity.Value*deduct.DeductPercent);;
                        break;
                    default:
                        settle.DeductAmt = 0;
                        break;
                }
                if (deduct.MaxAmt>0)
                {
                    settle.DeductAmt = settle.DeductAmt > deduct.MaxAmt ? deduct.MaxAmt : settle.DeductAmt;
                }                
                settle.DeductAccountType = deduct.AccountType;
                settle.DeductMethod = deduct.DeductMethod;
            }
        }
        private void SendMailForInvalidData(List<ConsginToAccLogEntity> invalidList)
        {
            if (invalidList != null && invalidList.Count > 0)
            {
                StringBuilder mailMessage = new StringBuilder();
                mailMessage.AppendLine("非法的代销转财务数据:");

                invalidList.ForEach(x =>
                {
                    mailMessage.AppendLine(string.Format("系统编号:{0},供应商编号:{1},商品编号:{2},结算类型:{3},佣金百分比:{4}", x.SysNo, x.VendorSysNo, x.ProductSysNo, x.SettleType, x.SettlePercentage));
                });

                SendMail(GlobalSettings.MailSubject, mailMessage.ToString());
            }
        }

        #region 为SettleItem的属性赋值

        /// <summary>
        /// 为SettleItem的属性赋值
        /// </summary>
        /// <param name="currentItem">Acclog</param>
        /// <param name="settleItem">SettleItem</param>
        //private static void SetItemValue(ConsginToAccLogEntity currentItem, SettleItemEntity settleItem)
        private static void SetItemValue(ConsginToAccLogEntity currentItem, ConsignSettlementItemInfo settleItem)
        {
            settleItem.Quantity = currentItem.Quantity;
            settleItem.ConsignToAccStatus = currentItem.Status;
            settleItem.CreateCost = currentItem.CreateCost;
            settleItem.CreateTime = DateTime.Now;
            settleItem.FoldCost = currentItem.FoldCost;
            settleItem.MinCommission = currentItem.MinCommission;
            //settleItem.OnLineQty 
            settleItem.POConsignToAccLogSysNo = currentItem.SysNo;
            settleItem.Point = currentItem.Point;
            settleItem.ProductID = currentItem.ProductID;
            settleItem.ProductName = currentItem.ProductName;
            settleItem.ProductSysNo = currentItem.ProductSysNo;
            settleItem.Quantity = currentItem.Quantity;
            settleItem.RetailPrice = currentItem.RetailPrice;
            settleItem.SettlePercentage = currentItem.SettlePercentage;
            //settleItem.SettleSysNo 
            settleItem.SettleType = currentItem.SettleType == "O" ? SettleType.O : SettleType.P;
            settleItem.StockName = currentItem.StockName;
            settleItem.StockSysNo = currentItem.StockSysNo;
            settleItem.VendorName = currentItem.VendorName;
            settleItem.VendorSysNo = currentItem.VendorSysNo;
            //计算结算金额
            settleItem.Cost = CalculateCost(currentItem, settleItem);

            settleItem.ConsignToAccLogInfo = new ConsignToAcctLogInfo { LogSysNo = currentItem.SysNo, ProductQuantity = currentItem.Quantity };
            #region 之前的版本
            //settleItem.ConsignQty = currentItem.Quantity;
            //settleItem.ConsignToAccStatus = currentItem.Status;
            //settleItem.CreateCost = currentItem.CreateCost;
            //settleItem.CreateTime = DateTime.Now;
            //settleItem.FoldCost = currentItem.FoldCost;
            //settleItem.MinCommission = currentItem.MinCommission;
            ////settleItem.OnLineQty 
            //settleItem.POConsignToAccLogSysNo = currentItem.SysNo;
            //settleItem.Point = currentItem.Point;
            //settleItem.ProductID = currentItem.ProductID;
            //settleItem.ProductName = currentItem.ProductName;
            //settleItem.ProductSysNo = currentItem.ProductSysNo;
            //settleItem.Quantity = currentItem.Quantity;
            //settleItem.RetailPrice = currentItem.RetailPrice;
            //settleItem.SettlePercentage = currentItem.SettlePercentage;
            ////settleItem.SettleSysNo 
            //settleItem.SettleType = currentItem.SettleType;
            //settleItem.StockName = currentItem.StockName;
            //settleItem.StockSysNo = currentItem.StockSysNo;
            //settleItem.VendorName = currentItem.VendorName;
            //settleItem.VendorSysNo = currentItem.VendorSysNo;
            ////计算结算金额
            //settleItem.Cost = CalculateCost(currentItem, settleItem);
            #endregion
        }

        #endregion

        #endregion

        #region 计算结算金额

        /// <summary>
        /// 计算结算金额
        /// </summary>
        /// <param name="currentItem">Acclog</param>
        /// <param name="settleItem">代销结算Item</param>
        /// <returns>结算金额</returns>
        private static decimal CalculateCost(ConsginToAccLogEntity currentItem, ConsignSettlementItemInfo settleItem)
        {
            Decimal cost;

            //计算结算金额
            if (settleItem.SettleType == SettleType.O)
            {                
                cost = currentItem.CreateCost;//- currentItem.Point / 10m;
                //-->
            }
            else if (settleItem.SettleType == SettleType.P)
            {
                if (!currentItem.SettlePercentage.HasValue)
                {
                    throw new InvalidOperationException("结算类型为P的AccLog必须有SettlePercentage");
                }

                Decimal profit = currentItem.RetailPrice * currentItem.SettlePercentage.Value / 100m;

                if (profit >= currentItem.MinCommission)
                {
                    //<!--CRL21118 Modify By Kilin 去除积分扣除
                    cost = currentItem.RetailPrice * (1 - currentItem.SettlePercentage.Value / 100m);
                                   // -currentItem.Point / 10m;
                    //-->
                }
                else
                {
                    cost = currentItem.RetailPrice - currentItem.MinCommission;
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("非法的SettleType:{0}", settleItem.SettleType));
            }
      
            return Math.Round(cost, 2, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region CRL20438 By Kilin 传统代销引入结算规则
        private static Dictionary<int, SettleRulesEntity> RuleDic = null;

        //根据代销转财务记录SysNo查询满足的代销规则
        private static SettleRulesEntity GetRuleByLogSysNo(int sysNo)
        {
            SettleRulesEntity entity = dal.GetRuleByConsginToAccLogSysNo(sysNo);
            if (entity != null)
            {
                if (RuleDic.ContainsKey(entity.SysNo))
                {
                    return RuleDic[entity.SysNo];
                }
                else
                {
                    RuleDic.Add(entity.SysNo, entity);
                    return entity;
                }
            }
            return null;
        }

        private static decimal? CalculateRuleCost(ConsignSettlementItemInfo msg)
        {
            int sysNo = msg.POConsignToAccLogSysNo.Value;
            SettleRulesEntity entity = GetRuleByLogSysNo(sysNo);
            if (entity == null) //如果未能匹配到结算规则，则返回原价
            {
                return null;
            }

            //如果匹配到的结算规则的结算数量未设置，则默认为无穷多
            //如果设置了结算数量，则按照结算数量进行匹配
            if (entity.SettleRuleQuantity.HasValue)
            {
                //计算剩余的结算数量
                int resultQuantity = entity.SettleRuleQuantity.Value - (entity.SettleedQuantity ?? 0);

                int quantity = msg.Quantity ?? 0;
                if (resultQuantity < Math.Abs(quantity))
                {
                    return null;
                }
            }

            //修改规则中的已结算数量
            entity.SettleedQuantity = (entity.SettleedQuantity ?? 0) + Math.Abs(msg.Quantity ?? 0);
            //记录规则编号
            //??msg.ConsignSettleRuleSysNO = entity.SysNo;

            return entity.NewSettlePrice;
        }
        #endregion

        #region 调用PO的系统创建代销结算单服务

        
        /// <summary>
        /// 调用PO的系统创建代销结算单服务
        /// </summary>
        private void CallSystemCreateService(IList<ConsignSettlementInfo> vendorSettleList)
        {
            if (vendorSettleList == null || vendorSettleList.Count == 0)
            {
                return;
            }

            //获取配置参数
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["PORestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            int? UserSysNo = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UserSysNo"]);

            foreach (var item in vendorSettleList)
            {
                ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
                ECCentral.Job.Utility.RestServiceError error;               
                var ar = client.Create("/ConsignSettlement/CreateConsignSettlementBySystem", item, out error);
                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                    string errorMsg = string.Empty;
                    foreach (var errorItem in error.Faults)
                    {
                        errorMsg += errorItem.ErrorDescription;
                    }
                    OnDisplayMessage(errorMsg);
                }
            }

        }
   
       
        #endregion
    }
}
