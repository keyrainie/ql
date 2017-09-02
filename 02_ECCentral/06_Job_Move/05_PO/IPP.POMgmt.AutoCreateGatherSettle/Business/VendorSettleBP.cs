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
using System.Transactions;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.BizEntity.PO;

namespace POASNMgmt.AutoCreateVendorSettle.Business
{
    public class VendorSettleBP
    {
        #region 实例字段
        private static JobContext CurrentContext;

        private VendorSettleDAL dal = new VendorSettleDAL();

        public Action<string> DisplayMessage;

        private static string NODATA_MESSAGE_TEMPLATE = "{0:yyyy-MM-dd}没有供应商需要生成代收结算单";
        private static string NODATA_MESSAGE_TEMPLATE_NOPERIODTYPE = "{0:yyyy-MM-dd}没有账期类型";

        #endregion

        public VendorSettleBP(JobContext context)
        {
            CurrentContext = context;
        }

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

        #region 自动创建代收结算单

        /// <summary>
        /// 自动创建代收结算单
        /// </summary>
        public void CreateGatherSettle()
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
                OnDisplayMessage(string.Format(NODATA_MESSAGE_TEMPLATE_NOPERIODTYPE, now));
                return;
            }

            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("需要自动生成结算单的账期类型:" + String.Join(",", payPeriodTypes.Select(x => x.ToString()).ToArray()));
            OnDisplayMessage(messageBuilder.ToString());

            var acclogList = hasMaxEndData ? dal.GetConsginToAccLogList(payPeriodTypes, maxOrderEndData, specVendorSysno) : dal.GetConsginToAccLogList(payPeriodTypes);

            if (acclogList == null || acclogList.Count == 0)
            {
                OnDisplayMessage(string.Format(NODATA_MESSAGE_TEMPLATE, now));

                return;
            }

            var vendorSettleList = GetVendorSettleList(acclogList);

            CallSystemCreateService(vendorSettleList, payPeriodTypes);
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

        #region 根据AccLog构建代收结算单

        /// <summary>
        /// 根据AccLog构建代收结算单
        /// </summary>
        /// <param name="acclogList">AccLog</param>
        /// <returns>代收结算单列表</returns>
        private List<GatherSettlementInfo> GetVendorSettleList(List<GatherSettleInfo> acclogList)
        {
            
            //计算TotalAmt
            List<GatherSettlementInfo> settleGatherList = new List<GatherSettlementInfo>();
            var result = from item in acclogList
                         group item by item.VendorSysno;
            
            foreach (var data in result)
            {
               
                var stockGroup = data.GroupBy(ite => ite.WarehouseNumber);
                foreach (var re in stockGroup)
                {
                    GatherSettlementInfo msg = new GatherSettlementInfo();
                    msg.GatherSettlementItemInfoList = new List<GatherSettlementItemInfo>();
                    msg.VendorSysNo = data.Key;     //@@
                    msg.StockSysNo = re.Key;        //@@
                    foreach (var it in re)
                    {
                        GatherSettlementItemInfo itemsg = new GatherSettlementItemInfo();
                        itemsg.InvoiceNumber = it.InvoiceNumber;
                        itemsg.OrderDate = it.OrderDate;
                        itemsg.OriginalPrice = it.OriginalPrice;

                        //??itemsg.Point = it.Point;        
                        itemsg.Point = Convert.ToInt32(it.Point);

                        itemsg.ProductID = it.ProductID;
                        itemsg.ProductName = it.ProductName;
                        itemsg.ProductSysNo = it.ProductSysNo;
                        itemsg.Quantity = it.Quantity;
                        itemsg.SettleStatus = it.SettleStatus;      //@@


                        itemsg.SettleSysNo = it.SettleSysNo;
                        //??itemsg.SettleType = it.SettleType;              
                        itemsg.SettleType = it.EnumSettleType;
                        itemsg.SoItemSysno = it.SoItemSysno;        //@@
                        itemsg.SONumber = it.SONumber;
                        itemsg.StockName = it.StockName;
                        itemsg.SysNo = it.SysNo;                    //@@
                        itemsg.TransactionNumber = it.TransactionNumber;    //@@
                        itemsg.VendorSysno = it.VendorSysno;                //@@
                        itemsg.WarehouseNumber = it.WarehouseNumber;
                        itemsg.PromotionDiscount = it.PromotionDiscount;//优惠券折扣
                        msg.GatherSettlementItemInfoList.Add(itemsg);
                    }
                    if (msg.GatherSettlementItemInfoList.Count > 0)
                    {
                        msg.TotalAmt = msg.GatherSettlementItemInfoList.Sum(d => (d.OriginalPrice * d.Quantity) + (d.PromotionDiscount.HasValue ? d.PromotionDiscount.Value : 0)).Value;
                    }
                    settleGatherList.Add(msg);
                }
            }

            return settleGatherList;
        }
        private GatherSettlementInfo InternalCreate(GatherSettlementInfo entity, List<int> payPeriodTypes)
        {
            string OperationUserUniqueName = GlobalSettings.CompanyCode;
            entity.CreateDate = DateTime.Now;
            //entity.Status = "ORG";
            entity.Status = SettleStatus.WaitingAudit;
            
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                entity.TotalAmt = entity.GatherSettlementItemInfoList.Sum(item => (item.OriginalPrice.Value * item.Quantity.Value) + (item.PromotionDiscount.HasValue ? item.PromotionDiscount.Value : 0));
                var list = from itme in entity.GatherSettlementItemInfoList
                           select itme.SONumber.Value;
                List<int> soList = list.ToList().Distinct().ToList();
                List<List<int>> listso = new List<List<int>>();
                for (int i = 0; i < soList.Count; i++)
                {
                    if (i % 1000 == 0)
                    {
                        List<int> listint = new List<int>();
                        listso.Add(listint);
                    }
                    listso[i / 1000].Add(soList[i]);
                }
                foreach (List<int> listi in listso)
                {
                    dal.Create(entity,payPeriodTypes, listi);
                }
                scope.Complete();
            }
            return entity;
        }
       
      

        #endregion

        #region 计算结算金额

        #endregion

        #region 调用PO的系统创建代收结算单服务

        /// <summary>
        /// 调用PO的系统创建代收结算单服务
        /// </summary>
        private void CallSystemCreateService(IList<GatherSettlementInfo> vendorSettleList, List<int> payPeriodTypes)
        {
            foreach (var item in vendorSettleList)
            {
                InternalCreate(item, payPeriodTypes);
            }
        }

        #endregion
    }
}
