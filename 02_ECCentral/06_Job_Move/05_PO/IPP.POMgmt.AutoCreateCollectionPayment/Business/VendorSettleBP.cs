using System;
using System.Collections.Generic;
using System.Linq;
using POASNMgmt.AutoCreateCollectionPayment.DataAccess;
using POASNMgmt.AutoCreateCollectionPayment.Compoents;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using POASNMgmt.AutoCreateCollectionPayment.Entities;
using System.Configuration;
using POASNMgmt.AutoCreateCollectionPayment.Compoents.Configuration;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.Job.Utility;
using ECCentral.BizEntity.PO;

namespace POASNMgmt.AutoCreateCollectionPayment.Business
{
    internal sealed class VendorSettleBP
    {
        public VendorSettleBP(JobContext context)
        {
            RuleDic = new Dictionary<int, SettleRulesEntity>();
            this.CurrentContext = context;
        }

        #region 实例字段
        private JobContext CurrentContext;

        private static VendorSettleDAL dal = new VendorSettleDAL();

        public Action<string> DisplayMessage;

        private static string NODATA_MESSAGE_TEMPLATE = "{0:yyyy-MM-dd}没有供应商需要生成代收代付结算单";

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
            bool hasMaxEndData = false;
            DateTime maxOrderEndData = DateTime.Now;
            DateTime now = DateTime.Now;
            int specVendorSysno = 0;

            #region 修复数据Code
            if (CurrentContext != null)
            {
                //从上下文中获取当前日期（用于修复数据使用，平时都为空）
                if (CurrentContext.Properties.ContainsKey("CurrentDay"))
                {
                    now = Convert.ToDateTime(CurrentContext.Properties["CurrentDay"]);
                }

                //从上下文中获取最大截至日期（用于修复数据使用，平时都为空）
                if (CurrentContext.Properties.ContainsKey("MaxOrderEndData"))
                {
                    maxOrderEndData = Convert.ToDateTime(CurrentContext.Properties["MaxOrderEndData"]);
                    hasMaxEndData = true;
                }

                //从上下文中获取供应商编号（用于修复数据使用，平时都为空）
                if (CurrentContext.Properties.ContainsKey("RunVendorSysno"))
                {
                    specVendorSysno = Convert.ToInt32(CurrentContext.Properties["RunVendorSysno"]);
                }
            }
            #endregion

            List<int> payPeriodTypes = GetConsignToAccTypes(now);

            if (payPeriodTypes == null || payPeriodTypes.Count == 0)
            {
                OnDisplayMessage(string.Format(NODATA_MESSAGE_TEMPLATE, now));
                return;
            }

            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("需要自动生成代收代付结算单的账期类型:" + String.Join(",", payPeriodTypes.Select(x => x.ToString()).ToArray()));
            OnDisplayMessage(messageBuilder.ToString());

            //var acclogList = hasMaxEndData ? dal.GetConsginToAccLogList(payPeriodTypes, maxOrderEndData, specVendorSysno) : dal.GetConsginToAccLogList(payPeriodTypes);
            var acclogList = dal.GetConsginToAccLogList(payPeriodTypes);

            if (acclogList == null || acclogList.Count == 0)
            {
                OnDisplayMessage(string.Format(NODATA_MESSAGE_TEMPLATE, now));

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
        private static List<int> GetConsignToAccTypes(DateTime now)
        {
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

        #region 根据AccLog构建代收代付结算单

        /// <summary>
        /// 根据AccLog构建代收代付结算单
        /// </summary>
        /// <param name="acclogList">AccLog</param>
        /// <returns>代收代付结算单列表</returns>
        private List<CollectionPaymentInfo> GetVendorSettleList(List<ConsginToAccLogEntity> acclogList)
        {
            var vendorSettleList = new List<CollectionPaymentInfo>();

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

            var groups = from item in validDataQuery
                         group item by new { item.VendorSysNo, item.VendorName, item.StockSysNo, item.CurrencySysNo, item.TaxRate, item.PMUserSysNo, item.PayPeriodType }
                             into team
                             select team;

            foreach (var group in groups)
            {
                CollectionPaymentInfo settle = null;

                var i = 0;
                foreach (var item in group)
                {
                    //每个代销结算单最多只能有MaxItemCount个Item
                    if (i % GlobalSettings.MaxItemCount == 0)
                    {
                        settle = new CollectionPaymentInfo();
                        vendorSettleList.Add(settle);

                        settle.CreateUser = GlobalSettings.UserName;
                        settle.CreateUserSysNo = GlobalSettings.UserSysNo;
                        settle.CurrentUserSysNo = GlobalSettings.UserSysNo; //服务端会把CurrentUserSysNo 赋值给CreateUserSysNo （TMD太贱了）

                        settle.CreateTime = DateTime.Now;
                        settle.CurrencySysNo = group.Key.CurrencySysNo;         //@@
                        settle.CurrencyCode = group.Key.CurrencySysNo;         //@@
                        settle.PayPeriodType = group.Key.PayPeriodType;
                        settle.ReturnPointPM = group.Key.PMUserSysNo;
                        settle.Status = POCollectionPaymentSettleStatus.Origin; //@@
                        //??settle.StockName = group.Key.TaxRate.ToString();        
                        settle.StockSysNo = group.Key.StockSysNo;               //@@
                        settle.TaxRate = group.Key.TaxRate;                     //@@
                        settle.VendorName = group.Key.VendorName;               //@@
                        settle.VendorSysNo = group.Key.VendorSysNo;             //@@
                        settle.VendorInfo = new VendorInfo { SysNo = group.Key.VendorSysNo };
                        settle.Note = "System Create";

                        settle.SettleItems = new List<CollectionPaymentItem>();

                        settle.PMInfo = new ECCentral.BizEntity.IM.ProductManagerInfo { SysNo = group.Key.PMUserSysNo };
                    }

                    CollectionPaymentItem settleItem = new CollectionPaymentItem();

                    //为SettleItem的属性赋值
                    SetItemValue(item, settleItem);
                    settle.SettleItems.Add(settleItem);

                    i++;
                }
            }

            //计算TotalAmt
            //vendorSettleList.ForEach(x => x.TotalAmt = x.SettleItems.Sum(y => y.Cost * y.Quantity));
            vendorSettleList.ForEach(x => x.TotalAmt = x.SettleItems.Sum(y => y.Cost * y.ConsignQty));

            return vendorSettleList;
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
        //private static void SetItemValue(ConsginToAccLogEntity currentItem, CollectionPaymentItemMsg settleItem)
        private static void SetItemValue(ConsginToAccLogEntity currentItem, CollectionPaymentItem settleItem)
            
        {
            settleItem.ConsignQty = currentItem.Quantity;
            settleItem.ConsignToAccStatus = currentItem.Status;     //@@
            settleItem.CreateCost = currentItem.CreateCost;         //@@
            settleItem.CreateTime = DateTime.Now;
            settleItem.ConsignToAccLogInfo = new ConsignToAcctLogInfo
            {
                LogSysNo = currentItem.SysNo,
                CreateCost = currentItem.CreateCost,
                StockSysNo = currentItem.StockSysNo,
                ProductQuantity = currentItem.Quantity
            };

            settleItem.FoldCost = currentItem.FoldCost;
            settleItem.MinCommission = currentItem.MinCommission;   //@@
            //settleItem.OnLineQty 
            settleItem.POConsignToAccLogSysNo = currentItem.SysNo;
            settleItem.Point = currentItem.Point;
            settleItem.ProductID = currentItem.ProductID;
            settleItem.ProductName = currentItem.ProductName;
            settleItem.ProductSysNo = currentItem.ProductSysNo;
            settleItem.Quantity = currentItem.Quantity;             //@@
            settleItem.RetailPrice = currentItem.RetailPrice;       //@@
            settleItem.SettlePercentage = currentItem.SettlePercentage;
            //settleItem.SettleSysNo 
            settleItem.SettleType = currentItem.SettleType;
            settleItem.StockName = currentItem.StockName;
            settleItem.StockSysNo = currentItem.StockSysNo;         //@@
            settleItem.VendorName = currentItem.VendorName;         //@@
            settleItem.VendorSysNo = currentItem.VendorSysNo;       //@@
            //计算结算金额
            settleItem.Cost = CalculateCost(currentItem, settleItem);
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
        private static decimal CalculateCost(ConsginToAccLogEntity currentItem, CollectionPaymentItem settleItem)
        {
            Decimal cost;

            //计算结算金额
            if (settleItem.SettleType.ToUpper() == "O")
            {
                //CRL20438 By Kilin
                //对于负数的单据不启用规则
                //if (settleItem.Quantity > 0)
                if (settleItem.ConsignQty > 0)
                {
                    //通过规则进行价格计算
                    decimal? ruleCost = CalculateRuleCost(settleItem);

                    //规则应用成功
                    if (ruleCost.HasValue)
                    {
                        cost = ruleCost.Value;
                        goto Lable_Result;
                    }
                }

                //传统模式结算价统一用正常采购价格
                //<!--CRL21118 Modify By Kilin 去除积分扣除
                cost = currentItem.SettleCost;//- currentItem.Point / 10m;
                //-->
            }
            else if (settleItem.SettleType.ToUpper() == "P")
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
        Lable_Result:
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

        private static decimal? CalculateRuleCost(CollectionPaymentItem msg)
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

                //if (resultQuantity < Math.Abs(msg.Quantity))
                if (resultQuantity < Math.Abs(msg.ConsignQty))
                {
                    return null;
                }
            }

            //修改规则中的已结算数量
            entity.SettleedQuantity = (entity.SettleedQuantity ?? 0) + Math.Abs(msg.Quantity);  //@@
            //记录规则编号
            msg.ConsignSettleRuleSysNO = entity.SysNo;          //@@

            return entity.NewSettlePrice;
        }
        #endregion

        #region 调用PO的系统创建代销结算单服务

        /// <summary>
        /// 调用PO的系统创建代销结算单服务
        /// </summary>
        private void CallSystemCreateService(IList<CollectionPaymentInfo> vendorSettleList)
        {
            if (vendorSettleList == null || vendorSettleList.Count == 0)
            {
                return;
            }

            //获取配置参数
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["PORestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];

            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;

            foreach (var item in vendorSettleList)
            {
                var ar = client.Create("/CollectionPayment/Create", item, out error);
                if (error != null && error.Faults != null && error.Faults.Count > 0)
                {
                    string errorMsg = string.Empty;
                    foreach (var errorItem in error.Faults)
                    {
                        errorMsg += errorItem.ErrorDescription;
                    }
                    SendMailForWCFFailure(errorMsg);
                    Logger.WriteLog(errorMsg, "JobConsole");
                }
            }
            #region 服务调用 //?
            //IMaintainCollectionPaymentV31 service = null;

            //try
            //{
            //    service = ServiceBroker.FindService<IMaintainCollectionPaymentV31>();

            //    if (service == null)
            //    {
            //        throw new InvalidOperationException("未找到IMaintainCollectionPaymentV31服务");
            //    }

            //    foreach (var item in vendorSettleList)
            //    {
            //        CollectionPaymentV31 settle = new CollectionPaymentV31();
            //        settle.Header = new Newegg.Oversea.Framework.Contract.MessageHeader
            //        {
            //            CompanyCode = GlobalSettings.CompanyCode,
            //            OperationUser = new Newegg.Oversea.Framework.Contract.OperationUser
            //            {
            //                CompanyCode = GlobalSettings.CompanyCode,
            //                FullName = GlobalSettings.UserName,
            //                LogUserName = GlobalSettings.UserName,
            //                SourceDirectoryKey = GlobalSettings.SourceDirectoryKey,
            //                SourceUserName = GlobalSettings.UserName,
            //                UniqueUserName = GlobalSettings.UserName
            //            },
            //            StoreCompanyCode = GlobalSettings.StoreCompanyCode,
            //            FromSystem = GlobalSettings.FromIP
            //        };

            //        settle.Body = item;

            //        var result = service.SystemCreate(settle);

            //        if (result.Faults != null && result.Faults.Count > 0)
            //        {
            //            SendMailForWCFFailure(result);
            //        }
            //    }
            //}
            //finally
            //{
            //    ServiceBroker.DisposeService<IMaintainCollectionPaymentV31>(service);
            //}
            #endregion
        }

        private void SendMailForWCFFailure(string errorMsg)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("调用/CollectionPayment/SystemCreate服务出错:");
            messageBuilder.AppendLine(errorMsg);
            SendMail(GlobalSettings.MailSubject, messageBuilder.ToString());
        }

        #endregion
    }
}
