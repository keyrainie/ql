using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.JobConsole.Client;
using GroupSettle.Model;
using GroupSettle.DAL;
using System.Xml;

using System.Transactions;


namespace GroupSettle.Biz
{
    public class BP
    {
        #region Field
        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        //private static ILog log = LogerManger.GetLoger();
        private static JobContext CurrentContext;

        //public  DateTime BeginDate { get; set; }

        //public  DateTime EndDate { get; set; }
        #endregion
        public static void DoWork(JobContext currentContext)
        {
            CurrentContext = currentContext;
            //if (DateTime.Today.Day.ToString() != Settings.RunDay)
            //{
            //    OnShowInfo("当前日不为结算日");
            //}
            //else
            //{
            DateTime beginDate = DateTime.Now;
            DateTime endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            var settleList = DA.GetSettleGroup();

            //OnShowInfo(string.Format("{0}至{1}:获取到{2}条团购记录需要结算", BeginDate.ToDisplay(), EndDate.ToDisplay(), settleList.Count));
            OnShowInfo(string.Format("获取到{0}条团购记录需要结算", settleList.Count));
            var masters = settleList.GroupBy(p => p.VendorSysNo);
            OnShowInfo(string.Format("需要生成{0}张结算单", masters.Count()));
            //结算单集合
            List<GroupBuyingSettleMaster> SettleList = new List<GroupBuyingSettleMaster>();

            OnShowInfo("开始计算结算单......");


            #region Old
            //foreach (var master in masters)
            //{


            //    GroupBuyingSettleMaster entity = new GroupBuyingSettleMaster
            //                               {
            //                                   //BeginDate = BeginDate,
            //                                   //EndDate = EndDate,
            //                                   VendorSysNo = master.Key,
            //                                   SettleAmt = master.Sum(p => p.CostAmt)
            //                               };
            //    List<GroupBuyingSettleItem> itemList = new List<GroupBuyingSettleItem>();
            //    var items = master.GroupBy(p => p.GroupBuyingSysNo);
            //    foreach (var item in items)
            //    {
            //        GroupBuyingSettleItem settleItem = new GroupBuyingSettleItem
            //        {
            //            SettleAmt = item.Sum(p => p.CostAmt),
            //            GroupBuyingSysNo = item.Key,
            //            SettlementSysNo = entity.SysNo,
            //            AccList = item.ToList()
            //        };
            //        itemList.Add(settleItem);
            //    }
            //    entity.ItemList = itemList;
            //    SettleList.Add(entity);
            //}

            #endregion

            foreach (var master in masters)
            {
                GroupBuyingTicketToAcc GroupBuyingTicketToAcc = master.First();
                if (GroupBuyingTicketToAcc.PayPeriodType != Settings.MonthsPaytermsNo && GroupBuyingTicketToAcc.PayPeriodType != Settings.WeeksPaytermsNo)
                {

                    continue;
                }
                else
                {
                    #region     月结的处理

                    if (GroupBuyingTicketToAcc.PayPeriodType == Settings.MonthsPaytermsNo)
                    {
                        OnShowInfo("结算方式为月结......");
                        if (DateTime.Now.Day != Settings.MonthsDay_One && DateTime.Now.Day != Settings.MonthsDay_Two)
                        {
                            OnShowInfo("当前日不为结算日!");
                            continue;
                        }
                        else
                        {
                            if (DateTime.Now.Day == Settings.MonthsDay_One)
                            {
                                beginDate = Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01")).AddDays(Settings.MonthsDay_Two);
                            }
                            else if (DateTime.Now.Day == Settings.MonthsDay_Two)
                            {
                                beginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-02"));
                            }
                            GroupBuyingSettleMaster entity = new GroupBuyingSettleMaster
                            {
                                BeginDate = beginDate,
                                EndDate = endDate,
                                VendorSysNo = master.Key,
                                SettleAmt = master.Sum(p => p.CostAmt)
                            };
                            List<GroupBuyingSettleItem> itemList = new List<GroupBuyingSettleItem>();
                            var items = master.GroupBy(p => p.GroupBuyingSysNo);
                            foreach (var item in items)
                            {
                                GroupBuyingSettleItem settleItem = new GroupBuyingSettleItem
                                {
                                    SettleAmt = item.Sum(p => p.CostAmt),
                                    GroupBuyingSysNo = item.Key,
                                    SettlementSysNo = entity.SysNo,
                                    AccList = item.ToList()
                                };
                                itemList.Add(settleItem);
                            }
                            entity.ItemList = itemList;
                            SettleList.Add(entity);
                        }
                    }

                    #endregion


                    #region  周结的处理
                    if (GroupBuyingTicketToAcc.PayPeriodType == Settings.WeeksPaytermsNo)
                    {
                        OnShowInfo("结算方式为周结......");
                        if (DateTime.Now.Day != Settings.WeeksDay_One && DateTime.Now.Day != Settings.WeeksDay_Two &&
                            DateTime.Now.Day != Settings.WeeksDay_Three && DateTime.Now.Day != LastDayMonth())
                        {
                            OnShowInfo("当前日不为结算日!");
                            continue;
                        }
                        else
                        {
                            if (DateTime.Now.Day == Settings.WeeksDay_One)
                            {
                                beginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01"));
                            }
                            else if (DateTime.Now.Day == Settings.WeeksDay_Two)
                            {
                                beginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01")).AddDays(Settings.WeeksDay_One);
                            }
                            else if (DateTime.Now.Day == Settings.WeeksDay_Three)
                            {
                                beginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01")).AddDays(Settings.WeeksDay_Two);
                            }
                            else if (DateTime.Now.Day == LastDayMonth())
                            {
                                beginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01")).AddDays(Settings.WeeksDay_Three);
                            }
                            GroupBuyingSettleMaster entity = new GroupBuyingSettleMaster
                            {
                                BeginDate = beginDate,
                                EndDate = endDate,
                                VendorSysNo = master.Key,
                                SettleAmt = master.Sum(p => p.CostAmt)
                            };
                            List<GroupBuyingSettleItem> itemList = new List<GroupBuyingSettleItem>();
                            var items = master.GroupBy(p => p.GroupBuyingSysNo);
                            foreach (var item in items)
                            {
                                GroupBuyingSettleItem settleItem = new GroupBuyingSettleItem
                                {
                                    SettleAmt = item.Sum(p => p.CostAmt),
                                    GroupBuyingSysNo = item.Key,
                                    SettlementSysNo = entity.SysNo,
                                    AccList = item.ToList()
                                };
                                itemList.Add(settleItem);
                            }
                            entity.ItemList = itemList;
                            SettleList.Add(entity);
                        }
                    }

                    #endregion
                }
            }
            OnShowInfo("开始持久化到数据库......");
            SaveToDB(SettleList);
            OnShowInfo("团购结算单结算完成。");
        }

        private static void SaveToDB(List<GroupBuyingSettleMaster> List)
        {
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            foreach (var master in List)
            {
                //每单主子表及回写为一个事务
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    var entity = DA.CreateMaster(master);
                    master.SysNo = entity.SysNo;
                    master.ItemList.ForEach(p => p.SettlementSysNo = master.SysNo);
                    foreach (var item in master.ItemList)
                    {
                        var itementity = DA.CreateItem(item);
                        item.AccList.ForEach(p => p.SettlementItemSysNo = itementity.SysNo);
                        foreach (var acc in item.AccList)
                        {
                            DA.UpdateSettleAcc(acc);
                        }
                    }
                    scope.Complete();
                }
            }
        }

        private static void OnShowInfo(string info)
        {
            ILog log = LogerManger.GetLoger();
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }
            Console.WriteLine(info);
            log.WriteLog(info);
            if (CurrentContext != null)
            {
                CurrentContext.Message += info + Environment.NewLine;
            }
        }

        public static int LastDayMonth()
        {
            DateTime lastDay = Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01")).AddDays(-1);
            return lastDay.Day;
        }

    }
}
