using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.ECommerceMgmt.AutoInnerOnlineList.DA;
using IPP.ECommerceMgmt.AutoInnerOnlineList.BusinessEntities;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.ECommerceMgmt.AutoInnerOnlineList.Biz
{
    public class OnlinelistBP
    {
        #region rules and const

        //1.首页：特价快报每分页的最高数量为10个（所有分页内容不可重复）
        // 首页推荐每分页的最高数量为20个（所有分页内容不可重复）
        //特价快报和首页推荐的商品信息不可重复

        //2.品牌旗舰店/品牌专卖店：固定显示数量（旗舰店为3个3行，专卖店为4个3行）
        //新品上架最高数量为6个（所有商品不可重复）
        //让利促销最高数量为6个（所有商品不可重复）
        //当季热销最高数量为6个（所有商品不可重复）

        //3.一级分类：固定显示数量（新品上架为6个，热销商品为12个）
        //新品上架最高数量为10个（所有商品不可重复）
        //热销商品最高数量为20个（所有商品不可重复）
        //新品上架和热销商品的商品信息不可重复

        //4.二级分类：固定显示数量为4个
        //今日特惠最新数量为6个，其中降价金额和降价百分比各3个（所有商品不可重复）

        //5.三级分类：固定显示数量为2个
        //今日特惠最新数量为4个（所有商品不可重复）

        const int pagenumm = 4;
        const int SpecialItemMaxNumPerPage = 10;
        const int RecommendedItemMaxNumPerPage = 20;

        const int BrandItemMaxNum = 6;//+4
        const int NewItemMaxNum = 10;
        const int HotItemMaxNum = 20;
        const int SpecialItemMaxNum = 4;

        const int Category2ItemMaxNum = 6;//+4
        const int Category3ItemMaxNum = 4;//+4
        //网站首页：范围：5大类的二级类商品:笔记本电脑，手机，数码相机/摄像机，显示器，平板电视 "536,549,550,535,711"; 
        static string homeC2sysnolist = ConfigurationManager.AppSettings["OnlnelistHomeC2Sysnolist"];

        static int Priority = Convert.ToInt32(ConfigurationManager.AppSettings["OnlnelistItemPriority"]);
        //清除历史数据离现在时间
        static string DeleteDataTime = ConfigurationManager.AppSettings["DeleteDataTime"];

        public static DateTime BeginTime;

        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;
        #endregion

        #region Business

        public static JobContext jobContext = null;

        public static void CheckOnlinelistItem()
        {
            BeginTime = DateTime.Now;
            string endMsg = string.Empty;
            WriteLog("\r\n" + DateTime.Now + " ------------------- Begin-------------------------");
            WriteLog(DateTime.Now + " 商品推荐管理自动补位job开始运行......");

            try
            {
                //添加清除一天以前无效的系统推荐数据，每次job只删除5000条
                WriteLog(DateTime.Now + " 正在清除一天前历史数据......");
                OnlinelistDA.ClearTableOnLinelist(DeleteDataTime);

                //首页
                //WriteLog(DateTime.Now + " 正在进行首页商品设置......");
                //SetHomePageItem();

                //1级类别
                WriteLog(DateTime.Now + " 正在进行一级类页面商品设置......");
                SetC1PageItem();

                //2级类别
                WriteLog(DateTime.Now + " 正在进行二级类页面商品设置......");
                SetC2PageItem();

                //3级类别
                WriteLog(DateTime.Now + " 正在进行三级类页面商品设置......");
                SetC3PageItem();

                //专卖店
                //WriteLog(DateTime.Now + " 正在进行专卖店商品设置......");
                //SetBrandItem();

                endMsg = DateTime.Now + " 本次job成功结束!";
                WriteLog(endMsg);
            }
            catch (Exception er)
            {
                endMsg = DateTime.Now + " job运行异常，异常信息如下：\r\n " + er.Message;
                SendExceptionInfoEmail(endMsg);
                WriteLog(endMsg);
            }
            WriteLog(DateTime.Now + " ------------------- End-----------------------------\r\n");
        }

        public static void SetHomePageItem()
        {
            //check 首页的item是否满足条件（30没手动改过，则系统设置优先；如果30天内有手动记录，则手动记录优先级高）
            List<OnlineList> homePagelist = new List<OnlineList>();
            List<int> HasItemsysnolist = new List<int>();
            List<int> HasAddItemsysnolist = new List<int>();

            List<int> sysnolist = new List<int>();//原来记录的sysno
            List<int> hassysnolist = new List<int>(); //原来记录和新添记录的sysno

            homePagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.Homepage);
            if (homePagelist != null && homePagelist.Count > 0)
            {
                #region  特价快报
                var SpecialResultlist = (from item in homePagelist
                                         where (item.locationInfo.PositionID == (int)HomePage.Special1
                                         || item.locationInfo.PositionID == (int)HomePage.Special2
                                         || item.locationInfo.PositionID == (int)HomePage.Special3
                                         || item.locationInfo.PositionID == (int)HomePage.Special4)
                                           && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                         select item).Distinct().ToList();

                if (SpecialResultlist != null && SpecialResultlist.Count >= 0)
                {
                    int diffnum = SpecialItemMaxNumPerPage * pagenumm - SpecialResultlist.Count;
                    if (diffnum > 0)
                    {
                        #region 如果需要补位
                        //每一页
                        int Special1Count = 0;
                        int Special2Count = 0;
                        int Special3Count = 0;
                        int Special4Count = 0;
                        foreach (OnlineList online in SpecialResultlist)
                        {
                            HasItemsysnolist.Add(online.ProductSysNo); //该位置已经存在的商品记录 保证添加的商品不重复
                            sysnolist.Add(online.SysNo);
                            if (online.locationInfo.PositionID == (int)HomePage.Special1)
                            {
                                Special1Count++;
                            }
                            else if (online.locationInfo.PositionID == (int)HomePage.Special2)
                            {
                                Special2Count++;
                            }
                            else if (online.locationInfo.PositionID == (int)HomePage.Special3)
                            {
                                Special3Count++;
                            }
                            else if (online.locationInfo.PositionID == (int)HomePage.Special4)
                            {
                                Special4Count++;
                            }
                        }

                        HomePageSpecialItemProcess(SpecialItemMaxNumPerPage - Special1Count, (int)PageType.Homepage
                            , (int)HomePage.Special1, HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out hassysnolist);
                        HasItemsysnolist = HasAddItemsysnolist;
                        sysnolist = hassysnolist;
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special1, sysnolist);

                        HomePageSpecialItemProcess(SpecialItemMaxNumPerPage - Special2Count, (int)PageType.Homepage
                            , (int)HomePage.Special2, HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out hassysnolist);
                        HasItemsysnolist = HasAddItemsysnolist;
                        sysnolist = hassysnolist;
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special2, sysnolist);

                        HomePageSpecialItemProcess(SpecialItemMaxNumPerPage - Special3Count, (int)PageType.Homepage
                            , (int)HomePage.Special3, HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out hassysnolist);
                        HasItemsysnolist = HasAddItemsysnolist;
                        sysnolist = hassysnolist;
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special3, sysnolist);

                        HomePageSpecialItemProcess(SpecialItemMaxNumPerPage - Special4Count, (int)PageType.Homepage
                            , (int)HomePage.Special4, HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out hassysnolist);
                        HasItemsysnolist = HasAddItemsysnolist;
                        sysnolist = hassysnolist;
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special4, sysnolist);

                        #endregion
                    }
                    else
                    {
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special1, sysnolist);
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special2, sysnolist);
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special3, sysnolist);
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special4, sysnolist);
                    }
                }
                #endregion

                #region  首页推荐
                var RecommendedResultlist = (from item in homePagelist
                                             where (item.locationInfo.PositionID == (int)HomePage.HomeRecommended1
                                             || item.locationInfo.PositionID == (int)HomePage.HomeRecommended2
                                             || item.locationInfo.PositionID == (int)HomePage.HomeRecommended3
                                             || item.locationInfo.PositionID == (int)HomePage.HomeRecommended4)
                                               && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                             select item).Distinct().ToList();
                if (RecommendedResultlist != null && RecommendedResultlist.Count >= 0)
                {
                    int diffnum = RecommendedItemMaxNumPerPage * pagenumm - RecommendedResultlist.Count;
                    if (diffnum > 0)
                    {
                        #region 如果需要补位
                        //哪一页
                        int Recommend1Count = 0;
                        int Recommend2Count = 0;
                        int Recommend3Count = 0;
                        int Recommend4Count = 0;
                        foreach (OnlineList online in RecommendedResultlist)
                        {
                            HasItemsysnolist.Add(online.ProductSysNo);//该位置已经存在的商品记录 保证添加的商品不重复
                            sysnolist.Add(online.SysNo);
                            if (online.locationInfo.PositionID == (int)HomePage.HomeRecommended1)
                            {
                                Recommend1Count++;
                            }
                            else if (online.locationInfo.PositionID == (int)HomePage.HomeRecommended2)
                            {
                                Recommend2Count++;
                            }
                            else if (online.locationInfo.PositionID == (int)HomePage.HomeRecommended3)
                            {
                                Recommend3Count++;
                            }
                            else if (online.locationInfo.PositionID == (int)HomePage.HomeRecommended4)
                            {
                                Recommend4Count++;
                            }
                        }
                        HomePageItemProcess(RecommendedItemMaxNumPerPage - Recommend1Count, (int)PageType.Homepage
                            , (int)HomePage.HomeRecommended1, HasItemsysnolist, out HasAddItemsysnolist
                            , sysnolist, out hassysnolist);
                        HasItemsysnolist = HasAddItemsysnolist;
                        sysnolist = hassysnolist;

                        HasItemsysnolist.Distinct();
                        sysnolist.Distinct();

                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended1, sysnolist);

                        HomePageItemProcess(RecommendedItemMaxNumPerPage - Recommend2Count, (int)PageType.Homepage
                            , (int)HomePage.HomeRecommended2, HasItemsysnolist, out HasAddItemsysnolist
                             , sysnolist, out hassysnolist);
                        HasItemsysnolist = HasAddItemsysnolist;
                        sysnolist = hassysnolist;

                        HasItemsysnolist.Distinct();
                        sysnolist.Distinct();

                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended2, sysnolist);

                        HomePageItemProcess(RecommendedItemMaxNumPerPage - Recommend3Count, (int)PageType.Homepage
                            , (int)HomePage.HomeRecommended3, HasItemsysnolist, out HasAddItemsysnolist
                            , sysnolist, out hassysnolist);
                        HasItemsysnolist = HasAddItemsysnolist;
                        sysnolist = hassysnolist;

                        HasItemsysnolist.Distinct();
                        sysnolist.Distinct();

                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended3, sysnolist);

                        HomePageItemProcess(RecommendedItemMaxNumPerPage - Recommend4Count, (int)PageType.Homepage
                            , (int)HomePage.HomeRecommended4, HasItemsysnolist, out HasAddItemsysnolist
                            , sysnolist, out hassysnolist);
                        HasItemsysnolist = HasAddItemsysnolist;
                        sysnolist = hassysnolist;

                        HasItemsysnolist.Distinct();
                        sysnolist.Distinct();

                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended4, sysnolist);
                        #endregion
                    }
                    else
                    {
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended1, sysnolist);
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended2, sysnolist);
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended3, sysnolist);
                        DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended4, sysnolist);
                    }
                }
                #endregion
            }
            else
            {
                //更换该位置的所有商品，并删除以前的记录(如果是手动添加记录，只更改优先级为100000)
                WriteLog("\r\n 首页没有有效记录，需全部新添加。\r\n 正在进行 首页--特价快报");

                HomePageSpecialItemProcess(SpecialItemMaxNumPerPage, (int)PageType.Homepage, (int)HomePage.Special1
                    , HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out  hassysnolist);
                HasItemsysnolist = HasAddItemsysnolist;
                sysnolist = hassysnolist;

                sysnolist.Distinct();
                HasItemsysnolist.Distinct();

                DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special1, sysnolist);

                HomePageSpecialItemProcess(SpecialItemMaxNumPerPage, (int)PageType.Homepage, (int)HomePage.Special2
                    , HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out  hassysnolist);
                HasItemsysnolist = HasAddItemsysnolist;
                sysnolist = hassysnolist;

                sysnolist.Distinct();
                HasItemsysnolist.Distinct();

                DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special2, sysnolist);

                HomePageSpecialItemProcess(SpecialItemMaxNumPerPage, (int)PageType.Homepage, (int)HomePage.Special3
                    , HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out  hassysnolist);
                HasItemsysnolist = HasAddItemsysnolist;
                sysnolist = hassysnolist;

                sysnolist.Distinct();
                HasItemsysnolist.Distinct();

                DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special3, sysnolist);

                HomePageSpecialItemProcess(SpecialItemMaxNumPerPage, (int)PageType.Homepage, (int)HomePage.Special4
                    , HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out  hassysnolist);
                HasItemsysnolist = HasAddItemsysnolist;
                sysnolist = hassysnolist;

                sysnolist.Distinct();
                HasItemsysnolist.Distinct();

                DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.Special4, sysnolist);

                WriteLog("\r\n 正在进行 首页--首页推荐");

                HomePageItemProcess(RecommendedItemMaxNumPerPage, (int)PageType.Homepage, (int)HomePage.HomeRecommended1
                   , HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out  hassysnolist);
                HasItemsysnolist = HasAddItemsysnolist;
                sysnolist = hassysnolist;

                sysnolist.Distinct();
                HasItemsysnolist.Distinct();

                DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended1, sysnolist);

                HomePageItemProcess(RecommendedItemMaxNumPerPage, (int)PageType.Homepage, (int)HomePage.HomeRecommended2
                   , HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out  hassysnolist);
                HasItemsysnolist = HasAddItemsysnolist;
                sysnolist = hassysnolist;

                sysnolist.Distinct();
                HasItemsysnolist.Distinct();

                DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended2, sysnolist);

                HomePageItemProcess(RecommendedItemMaxNumPerPage, (int)PageType.Homepage, (int)HomePage.HomeRecommended3
                  , HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out  hassysnolist);
                HasItemsysnolist = HasAddItemsysnolist;
                sysnolist = hassysnolist;

                sysnolist.Distinct();
                HasItemsysnolist.Distinct();

                DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended3, sysnolist);

                HomePageItemProcess(RecommendedItemMaxNumPerPage, (int)PageType.Homepage, (int)HomePage.HomeRecommended4
                   , HasItemsysnolist, out HasAddItemsysnolist, sysnolist, out  hassysnolist);
                HasItemsysnolist = HasAddItemsysnolist;
                sysnolist = hassysnolist;

                sysnolist.Distinct();
                HasItemsysnolist.Distinct();

                DeleteOnlineListItem((int)PageType.Homepage, (int)HomePage.HomeRecommended4, sysnolist);
            }
        }

        public static void SetC1PageItem()
        {
            List<int> HasItemlist = new List<int>();  //已有
            List<int> HasAddItemlist = new List<int>();   //已有+新

            List<int> sysnolist = new List<int>();
            List<int> hassysnolist = new List<int>();

            List<OnlineList> C1Pagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.C1page);
            List<OnlineList> C1SysNolist = OnlinelistDA.GetAllC1SysNolist();

            if (C1Pagelist != null && C1Pagelist.Count > 0)
            {
                if (C1SysNolist != null)
                {
                    #region 一级类页面 新品上架
                    var NewItemlist = (from item in C1Pagelist
                                       where item.locationInfo.PositionID == (int)Category1.New
                                         && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                       select item).ToList();

                    if (NewItemlist != null && NewItemlist.Count >= 0)
                    {
                        foreach (OnlineList linelist in NewItemlist)
                        {
                            sysnolist.Add(linelist.SysNo);
                            HasItemlist.Add(linelist.ProductSysNo);
                        }
                        sysnolist.Distinct();
                        HasItemlist.Distinct();
                        WriteLog("\r\n 正在进行 一级类页面--新品上架，现在共有记录" + NewItemlist.Count);
                        foreach (OnlineList oll in C1SysNolist)
                        {
                            var c1newlist = (from item in NewItemlist
                                             where item.C1SysNo == oll.C1SysNo
                                             select item).ToList();

                            int diffnum = NewItemMaxNum - c1newlist.Count;
                            WriteLog("一级类" + oll.C1SysNo + " 已有" + c1newlist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                C1pageItemProcess(diffnum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.New, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                                HasItemlist = HasAddItemlist;
                                sysnolist = hassysnolist;
                                sysnolist.Distinct();
                                HasItemlist.Distinct();
                            }
                        }
                        DeleteOnlineListItem((int)PageType.C1page, (int)Category1.New, sysnolist);
                    }
                    #endregion

                    #region 一级类页面 当季热销/今日推荐

                    var HotItemlist = (from item in C1Pagelist
                                       where item.locationInfo.PositionID == (int)Category1.Hot
                                         && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                       select item).ToList();

                    if (HotItemlist != null && HotItemlist.Count >= 0)
                    {
                        foreach (OnlineList linelist in HotItemlist)
                        {
                            sysnolist.Add(linelist.SysNo);
                            HasItemlist.Add(linelist.ProductSysNo);
                        }
                        WriteLog("\r\n 正在进行 一级类页面--当季热销，现在共有记录" + HotItemlist.Count);

                        foreach (OnlineList oll in C1SysNolist)
                        {
                            var c1hotlist = (from im in HotItemlist
                                             where im.C1SysNo == oll.C1SysNo
                                             select im).ToList();

                            int diffnum = HotItemMaxNum - c1hotlist.Count;
                            WriteLog("一级类" + oll.C1SysNo + " 已有" + c1hotlist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                C1pageItemProcess(diffnum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.Hot, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                                HasItemlist = HasAddItemlist;
                                sysnolist = hassysnolist;
                            }
                        }
                        DeleteOnlineListItem((int)PageType.C1page, (int)Category1.Hot, sysnolist);
                    }

                    #endregion

                    #region 特价商品,取折扣最大的前4个商品,当前售价/原价市场价格=折扣

                    var specialItem = (from a in C1Pagelist
                                       where a.locationInfo.PositionID == (int) Category1.SpecialItem
                                             && (a.OnlineQty > 0 || a.ShiftQty > 0)
                                       select a).ToList();

                    if(specialItem.Count>=0)
                    {
                        foreach (OnlineList linelist in specialItem)
                        {
                            sysnolist.Add(linelist.SysNo);
                            HasItemlist.Add(linelist.ProductSysNo);
                        }
                        WriteLog("\r\n 正在进行 一级类页面--特价商品，现在共有记录" + specialItem.Count);
                        foreach (OnlineList oll in C1SysNolist)
                        {
                            var c1hotlist = (from im in specialItem
                                             where im.C1SysNo == oll.C1SysNo
                                             select im).ToList();

                            int diffnum = SpecialItemMaxNum - c1hotlist.Count;
                            WriteLog("一级类" + oll.C1SysNo + " 已有" + c1hotlist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                C1pageItemProcess(diffnum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.SpecialItem, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                                HasItemlist = HasAddItemlist;
                                sysnolist = hassysnolist;
                            }
                        }
                        DeleteOnlineListItem((int)PageType.C1page, (int)Category1.SpecialItem, sysnolist);
                    }
                    #endregion
                }
            }
            else
            {
                if (C1SysNolist != null)
                {
                    WriteLog("\r\n  一级类页面没有有效记录，需全部重新添加.");

                    WriteLog("\r\n 正在进行 一级类页面--新品上架");
                    foreach (OnlineList oll in C1SysNolist)
                    {
                        C1pageItemProcess(NewItemMaxNum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.New, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                        HasItemlist = HasAddItemlist;
                        sysnolist = hassysnolist;
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    DeleteOnlineListItem((int)PageType.C1page, (int)Category1.New, sysnolist);
                    /* 今日推荐*/
                    WriteLog("\r\n 正在进行 一级类页面--当季热销");
                    foreach (OnlineList oll in C1SysNolist)
                    {
                        C1pageItemProcess(HotItemMaxNum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.Hot, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                        HasItemlist = HasAddItemlist;
                        sysnolist = hassysnolist;
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    DeleteOnlineListItem((int)PageType.C1page, (int)Category1.Hot, sysnolist);
                    /* 特价商品*/
                    WriteLog("\r\n 正在进行 一级类页面--特价商品");
                    foreach (OnlineList oll in C1SysNolist)
                    {
                        C1pageItemProcess(SpecialItemMaxNum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.SpecialItem, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                        HasItemlist = HasAddItemlist;
                        sysnolist = hassysnolist;
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    DeleteOnlineListItem((int)PageType.C1page, (int)Category1.SpecialItem, sysnolist);
                }
            }
        }

        public static void SetC2PageItem()
        {
            List<OnlineList> C2Pagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.C2page);
            List<int> HasItemlist = new List<int>(); //已有
            List<int> HasAddItemlist = new List<int>(); //已有和新加的

            List<int> sysnolist = new List<int>();
            List<int> hassysnolist = new List<int>();
            List<OnlineList> C2SysNolist = OnlinelistDA.GetAllC2SysNolist();

            if (C2Pagelist != null && C2Pagelist.Count > 0)
            {
                // 二级分类 特别推荐最新数量为6个，其中降价金额和降价百分比各3个（所有商品不可重复）
                var C2Itemlist = (from item in C2Pagelist
                                  where item.locationInfo.PositionID == (int)Category2.SpecialOfferToday
                                  && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                  select item).ToList();

                if (C2Itemlist != null && C2Itemlist.Count >= 0)
                {
                    foreach (OnlineList linelist in C2Itemlist)
                    {
                        sysnolist.Add(linelist.SysNo);
                        HasItemlist.Add(linelist.ProductSysNo);
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    WriteLog("正在进行 二级分类--特别推荐，已有记录" + C2Itemlist.Count);

                    if (C2SysNolist != null && C2SysNolist.Count > 0)
                    {
                        foreach (OnlineList oll in C2SysNolist)
                        {
                            var haslist = (from im in C2Itemlist
                                           where im.C2SysNo == oll.C2SysNo
                                           select im).ToList();

                            int diffnum = Category2ItemMaxNum - haslist.Count;
                            WriteLog("二级类" + oll.C2SysNo + " 已有" + haslist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                C2pageItemProcess(diffnum, oll.C2SysNo, (int)PageType.C2page, (int)Category2.SpecialOfferToday, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                                HasItemlist = HasAddItemlist;
                                sysnolist = hassysnolist;
                                sysnolist.Distinct();
                                HasItemlist.Distinct();
                            }
                        }
                        DeleteOnlineListItem((int)PageType.C2page, (int)Category2.SpecialOfferToday, sysnolist);

                    }
                }
            }
            else
            {
                WriteLog("\r\n 二级类页面 没有有效记录，需全部新添加商品。");
                if (C2SysNolist != null && C2SysNolist.Count > 0)
                {
                    foreach (OnlineList oll in C2SysNolist)
                    {
                        C2pageItemProcess(Category2ItemMaxNum, oll.C2SysNo, (int)PageType.C2page, (int)Category2.SpecialOfferToday, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                        HasItemlist = HasAddItemlist;
                        sysnolist = hassysnolist;
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    DeleteOnlineListItem((int)PageType.C2page, (int)Category2.SpecialOfferToday, sysnolist);
                }
            }
        }

        public static void SetC3PageItem()
        {
            List<OnlineList> C3Pagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.C3page);
            List<int> HasItemlist = new List<int>(); //已有
            List<int> HasAddItemlist = new List<int>(); //已有和新加的

            List<int> sysnolist = new List<int>();
            List<int> hassysnolist = new List<int>();
            List<OnlineList> C3SysNolist = OnlinelistDA.GetAllC3SysNolist();

            if (C3Pagelist != null && C3Pagelist.Count > 0)
            {
                #region 三级分类  7天之内降价最大的商品，降价金额=7天之内最高金额-当前金额
                
                var C3Itemlist = (from item in C3Pagelist
                                  where item.locationInfo.PositionID == (int)Category3.SmallSpecialOfferToday
                                  && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                  select item).ToList();

                if (C3Itemlist != null && C3Itemlist.Count >= 0)
                {
                    WriteLog("\r\n 正在进行 三级分类--热销推荐，已有记录" + C3Itemlist.Count);
                    foreach (OnlineList linelist in C3Itemlist)
                    {
                        sysnolist.Add(linelist.SysNo);
                        HasItemlist.Add(linelist.ProductSysNo);
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    //取降价最大的商品     
                    if (C3SysNolist != null && C3SysNolist.Count > 0)
                    {
                        foreach (OnlineList oll in C3SysNolist)
                        {
                            var c3list = (from im in C3Itemlist
                                          where im.C3SysNo == oll.C3SysNo
                                          select im).ToList();

                            int diffnum = Category3ItemMaxNum - c3list.Count;
                            WriteLog("三级类" + oll.C3SysNo + " 已有" + c3list.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                C3pageItemProcess(diffnum, oll.C3SysNo, (int)PageType.C3page, (int)Category3.SmallSpecialOfferToday, HasItemlist
                                    , out HasAddItemlist, sysnolist, out hassysnolist);
                                HasItemlist = HasAddItemlist;
                                sysnolist = hassysnolist;
                                sysnolist.Distinct();
                                HasItemlist.Distinct();
                            }
                        }
                        DeleteOnlineListItem((int)PageType.C3page, (int)Category3.SmallSpecialOfferToday, sysnolist);
                    }
                }
                #endregion

                
            }
            else
            {
                

                #region 三级类页面 没有有效记录，需全部新添加商品
                
                WriteLog("\r\n三级类页面 没有有效记录，需全部新添加商品。");

                if (C3SysNolist != null && C3SysNolist.Count > 0)
                {
                    foreach (OnlineList oll in C3SysNolist)
                    {
                        C3pageItemProcess(Category3ItemMaxNum, oll.C3SysNo, (int)PageType.C3page, (int)Category3.SmallSpecialOfferToday, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                        HasItemlist = HasAddItemlist;
                        sysnolist = hassysnolist;
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    DeleteOnlineListItem((int)PageType.C3page, (int)Category3.SmallSpecialOfferToday, sysnolist);

                }
                 
                #endregion
            }
        }

        public static void SetBrandItem()
        {
            List<OnlineList> BrandPagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.Brand);

            List<int> HasItemlist = new List<int>(); //已有专卖店商品
            List<int> HasAddItemlist = new List<int>(); //已有和新加的专卖店商品         

            List<int> sysnolist = new List<int>();
            List<int> hassysnolist = new List<int>();

            List<OnlineList> brandsysnolist = OnlinelistDA.GetAllBrand();

            if (BrandPagelist != null && BrandPagelist.Count > 0)
            {
                if (brandsysnolist != null && brandsysnolist.Count > 0)
                {
                    List<int> allBrandItem = (from item in BrandPagelist
                                              where (item.OnlineQty > 0 || item.ShiftQty > 0)
                                              select item.ProductSysNo).Distinct().ToList();

                    #region 品牌专卖 新品上架

                    var NewItemlist = (from item in BrandPagelist
                                       where item.locationInfo.PositionID == (int)Brand.New
                                         && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                       select item).ToList();

                    if (NewItemlist != null && NewItemlist.Count >= 0)
                    {
                        WriteLog("\r\n 正在进行 品牌专卖--新品上架，已有记录" + NewItemlist.Count);
                        foreach (OnlineList linelist in NewItemlist)
                        {
                            sysnolist.Add(linelist.SysNo);
                        }
                        sysnolist.Distinct();
                        foreach (OnlineList brand in brandsysnolist)
                        {
                            var itemlist = (from item in NewItemlist
                                            where item.BrandSysNo == brand.BrandSysNo
                                            select item).ToList();
                            int diffnum = BrandItemMaxNum - itemlist.Count;
                            WriteLog("专卖店" + brand.BrandSysNo + "已有" + itemlist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                BrandpageItemProcess(diffnum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.New, allBrandItem
                                    , out HasAddItemlist, sysnolist, out hassysnolist);
                                HasItemlist = HasAddItemlist;
                                sysnolist = hassysnolist;
                                sysnolist.Distinct();
                                HasItemlist.Distinct();
                            }
                        }
                        DeleteOnlineListItem((int)PageType.Brand, (int)Brand.New, sysnolist);
                    }
                    #endregion

                    #region 品牌专卖  让利促销

                    var PromotionItemlist = (from item in BrandPagelist
                                             where item.locationInfo.PositionID == (int)Brand.Promotion
                                               && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                             select item).ToList();

                    if (PromotionItemlist != null && PromotionItemlist.Count >= 0)
                    {
                        WriteLog("\r\n 正在进行 品牌专卖--让利促销，已有记录" + PromotionItemlist.Count);
                        foreach (OnlineList linelist in PromotionItemlist)
                        {
                            sysnolist.Add(linelist.SysNo);
                        }
                        sysnolist.Distinct();
                        foreach (OnlineList brand in brandsysnolist)
                        {
                            var itemlist = (from item in PromotionItemlist
                                            where item.BrandSysNo == brand.BrandSysNo
                                            select item).ToList();
                            int diffnum = BrandItemMaxNum - itemlist.Count;
                            WriteLog("专卖店" + brand.BrandSysNo + "已有" + itemlist.Count + "个商品，需新添加" + diffnum + "个商品。");
                            if (diffnum > 0)
                            {
                                if (HasItemlist.Count > 0)
                                {
                                    BrandpageItemProcess(diffnum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Promotion, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                                }
                                else
                                {
                                    BrandpageItemProcess(diffnum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Promotion, allBrandItem, out HasAddItemlist, sysnolist, out hassysnolist);
                                }
                                HasItemlist = HasAddItemlist;
                                sysnolist = hassysnolist;
                                sysnolist.Distinct();
                                HasItemlist.Distinct();
                            }
                        }
                        DeleteOnlineListItem((int)PageType.Brand, (int)Brand.Promotion, sysnolist);
                    }
                    #endregion

                    #region 品牌专卖 Hot
                    var HotBrandItemlist = (from item in BrandPagelist
                                            where item.locationInfo.PositionID == (int)Brand.Hot
                                            && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                            select item).ToList();
                    if (HotBrandItemlist != null && HotBrandItemlist.Count >= 0)
                    {
                        WriteLog("\r\n 正在进行 品牌专卖--当季热销，已有记录" + HotBrandItemlist.Count);
                        foreach (OnlineList linelist in HotBrandItemlist)
                        {
                            sysnolist.Add(linelist.SysNo);
                        }
                        sysnolist.Distinct();
                        foreach (OnlineList brand in brandsysnolist)
                        {
                            var itemlist = (from item in HotBrandItemlist
                                            where item.BrandSysNo == brand.BrandSysNo
                                            select item).ToList();
                            int diffnum = BrandItemMaxNum - itemlist.Count;

                            WriteLog("专卖店" + brand.BrandSysNo + "已有" + itemlist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                if (HasItemlist.Count > 0)
                                {
                                    BrandpageItemProcess(diffnum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Hot, HasItemlist
                                        , out HasAddItemlist, sysnolist, out hassysnolist);
                                }
                                else
                                {
                                    BrandpageItemProcess(diffnum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Hot, allBrandItem
                                   , out HasAddItemlist, sysnolist, out hassysnolist);
                                }
                                HasItemlist = HasAddItemlist;
                                sysnolist = hassysnolist;
                                sysnolist.Distinct();
                                HasItemlist.Distinct();
                            }
                        }
                        DeleteOnlineListItem((int)PageType.Brand, (int)Brand.Hot, sysnolist);
                    }
                    #endregion
                }
            }
            else
            {
                WriteLog("\r\n 专卖店没有有效记录，每个专卖店的每个位置需新加" + BrandItemMaxNum + " 条记录。");
                if (brandsysnolist != null && brandsysnolist.Count > 0)
                {
                    WriteLog("\r\n 正在进行 品牌专卖--新品上架");
                    foreach (OnlineList brand in brandsysnolist)
                    {
                        BrandpageItemProcess(BrandItemMaxNum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.New, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                        HasItemlist = HasAddItemlist;
                        sysnolist = hassysnolist;
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    DeleteOnlineListItem((int)PageType.Brand, (int)Brand.New, sysnolist);

                    WriteLog("\r\n 正在进行 品牌专卖--让利促销");
                    foreach (OnlineList brand in brandsysnolist)
                    {
                        BrandpageItemProcess(BrandItemMaxNum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Promotion, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                        HasItemlist = HasAddItemlist;
                        sysnolist = hassysnolist;
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    DeleteOnlineListItem((int)PageType.Brand, (int)Brand.Promotion, sysnolist);

                    WriteLog("\r\n 正在进行 品牌专卖--当季热销");
                    foreach (OnlineList brand in brandsysnolist)
                    {
                        BrandpageItemProcess(BrandItemMaxNum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Hot, HasItemlist, out HasAddItemlist, sysnolist, out hassysnolist);
                        HasItemlist = HasAddItemlist;
                        sysnolist = hassysnolist;
                    }
                    sysnolist.Distinct();
                    HasItemlist.Distinct();
                    DeleteOnlineListItem((int)PageType.Brand, (int)Brand.Hot, sysnolist);

                }
            }
        }

        public static void HomePageItemProcess(int diffnum, int pagetype, int positionid, List<int> HasItemSysnolist
            , out List<int> HasAddItemSysnolist, List<int> Sysnolist, out List<int> HasSysnolist)
        {
            string[] c2sysnoArr = homeC2sysnolist.Split(',');
            HasAddItemSysnolist = new List<int>();
            HasSysnolist = new List<int>();
            int addnum = 0;
            int quotient = diffnum / c2sysnoArr.Length;//商数
            int remainder = diffnum % c2sysnoArr.Length;//余数

            if (quotient > 0)
            {
                for (int j = 0; j < c2sysnoArr.Length; j++)
                {
                    //搜出销售最多的，除去已有的商品推荐记录
                    List<OnlineList> newItemlist = OnlinelistDA.GetSaleHightItem(quotient, Convert.ToInt32(c2sysnoArr[j]), HasItemSysnolist);
                    if (newItemlist != null)
                    {
                        foreach (OnlineList linelist in newItemlist)
                        {
                            linelist.Priority = Priority;
                            linelist.locationInfo = new OnlineListLocation();
                            linelist.locationInfo.PageID = 0;
                            linelist.locationInfo.PositionID = positionid;
                            int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            Sysnolist.Add(linelist.SysNo);
                            HasItemSysnolist.Add(linelist.ProductSysNo);
                            addnum++;
                        }
                    }
                }
            }
            for (int i = 0; i < remainder; i++)
            {
                for (int j = 0; j < c2sysnoArr.Length; j++)
                {
                    List<OnlineList> newItemlist = OnlinelistDA.GetSaleHightItem(1, Convert.ToInt32(c2sysnoArr[j]), HasItemSysnolist);

                    if (newItemlist != null)
                    {
                        foreach (OnlineList linelist in newItemlist)
                        {
                            linelist.Priority = Priority;
                            linelist.locationInfo = new OnlineListLocation();
                            linelist.locationInfo.PageID = 0;
                            linelist.locationInfo.PositionID = positionid;
                            int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            Sysnolist.Add(linelist.SysNo);
                            HasItemSysnolist.Add(linelist.ProductSysNo);
                            addnum++;
                        }
                    }
                }
            }

            WriteLog("首页--首页推荐" + positionid + " 成功添加" + addnum);
            HasAddItemSysnolist = HasItemSysnolist;
            HasSysnolist = Sysnolist;
        }

        public static void HomePageSpecialItemProcess(int diffnum, int pagetype, int positionid, List<int> HasItemSysnolist
            , out List<int> HasAddItemSysnolist, List<int> Sysnolist, out List<int> HasSysnolist)
        {
            HasAddItemSysnolist = new List<int>();
            HasSysnolist = new List<int>();
            string[] c2sysnoArr = homeC2sysnolist.Split(',');
            int addnum = 0;
            #region   降价最大的商品
            int quotient = diffnum / c2sysnoArr.Length;//商数
            int remainder = diffnum % c2sysnoArr.Length;//余数
            if (quotient > 0)
            {
                for (int j = 0; j < c2sysnoArr.Length; j++)
                {
                    List<OnlineList> newItemlist = OnlinelistDA.GetPriceDownItem(quotient, Convert.ToInt32(c2sysnoArr[j]), HasItemSysnolist);

                    if (newItemlist != null && newItemlist.Count > 0)
                    {
                        foreach (OnlineList linelist in newItemlist)
                        {
                            linelist.Priority = Priority;
                            linelist.locationInfo = new OnlineListLocation();
                            linelist.locationInfo.PageID = 0;
                            linelist.locationInfo.PositionID = positionid;
                            int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            Sysnolist.Add(linelist.SysNo);
                            HasItemSysnolist.Add(linelist.ProductSysNo);
                            addnum++;
                        }
                    }
                }
            }
            for (int i = 0; i < remainder; i++)
            {
                List<OnlineList> newItemlist = OnlinelistDA.GetPriceDownItem(1, Convert.ToInt32(c2sysnoArr[i]), HasItemSysnolist);

                if (newItemlist != null)
                {
                    foreach (OnlineList linelist in newItemlist)
                    {
                        linelist.Priority = Priority;
                        linelist.locationInfo = new OnlineListLocation();
                        linelist.locationInfo.PageID = 0;
                        linelist.locationInfo.PositionID = positionid;
                        int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        Sysnolist.Add(linelist.SysNo);
                        HasItemSysnolist.Add(linelist.ProductSysNo);
                        addnum++;
                    }
                }
            }
            #endregion

            WriteLog("首页--特价快报" + positionid + " 成功添加" + addnum);
            HasAddItemSysnolist = HasItemSysnolist;
            HasSysnolist = Sysnolist;
        }

        public static void C1pageItemProcess(int diffnum, int C1SysNo, int pagetype, int positionid, List<int> HasItemSysnolist
            , out List<int> HasAddItemSysnolist, List<int> Sysnolist, out List<int> hasSysnolist)
        {
            //新品上架和热销商品的商品信息不可重复
            HasAddItemSysnolist = new List<int>();
            hasSysnolist = new List<int>();
            int addnum = 0;
            int num;
            List<OnlineList> hotC1List = new List<OnlineList>();
            if (positionid == (int)Category1.Hot)
            {
                #region hotc1
                //取销售数量最多为
                List<OnlineList> c2numlist = OnlinelistDA.CheckC2ItemNumOnC1(C1SysNo);//取该一级类下的有效二级类
                if (c2numlist != null && c2numlist.Count > 0)
                {
                    if (c2numlist.Count >= diffnum)//要添加的个数  小于该一级类下所有二级类的个数
                    {
                        hotC1List = OnlinelistDA.GetSaleHightItemForC1Page(diffnum, HasItemSysnolist, C1SysNo, -1);
                        if (hotC1List != null && hotC1List.Count > 0)
                        {
                            var distincthotC1List = (from item in hotC1List
                                                     select item).Distinct();
                            WriteLog("一级类:" + C1SysNo + " 找出销售较多的商品" + distincthotC1List.Count() + "， 要补位" + diffnum);

                            foreach (OnlineList linelist in distincthotC1List)
                            {
                                linelist.Priority = Priority;
                                linelist.locationInfo = new OnlineListLocation();
                                linelist.locationInfo.PageID = C1SysNo;
                                linelist.locationInfo.PositionID = positionid;
                                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                                if (onlinelocationsysno > 0)
                                {
                                    linelist.OnlineLocationSysNo = onlinelocationsysno;
                                }
                                OnlinelistDA.Insert(linelist);
                                Sysnolist.Add(linelist.SysNo);
                                HasItemSysnolist.Add(linelist.ProductSysNo);
                                addnum++;
                            }
                        }
                    }
                    else
                    {
                        num = diffnum / c2numlist.Count;
                        foreach (OnlineList oo in c2numlist)
                        {
                            hotC1List = OnlinelistDA.GetSaleHightItemForC1Page(num, HasItemSysnolist, C1SysNo, oo.C2SysNo);
                            if (hotC1List != null && hotC1List.Count > 0)
                            {
                                var distincthotC1List = (from item in hotC1List
                                                         select item).Distinct();

                                WriteLog("一级类:" + C1SysNo + ", 二级类:" + oo.C2SysNo + " 找出销售较多的商品" + distincthotC1List.Count() + "， 要补位" + num);
                                foreach (OnlineList linelist in distincthotC1List)
                                {
                                    linelist.Priority = Priority;
                                    linelist.locationInfo = new OnlineListLocation();
                                    linelist.locationInfo.PageID = C1SysNo;
                                    linelist.locationInfo.PositionID = positionid;
                                    int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                                    if (onlinelocationsysno > 0)
                                    {
                                        linelist.OnlineLocationSysNo = onlinelocationsysno;
                                    }
                                    OnlinelistDA.Insert(linelist);
                                    Sysnolist.Add(linelist.SysNo);
                                    HasItemSysnolist.Add(linelist.ProductSysNo);
                                    addnum++;
                                }
                            }
                        }
                    }
                }
                #endregion
                WriteLog("一级类" + C1SysNo + "--热销商品 成功添加" + addnum);
            }
            else if (positionid == (int)Category1.New)
            {
                addnum = 0;
                #region newc1
                //取最新上架的产品           
                List<OnlineList> newC1List = OnlinelistDA.GetNewItemForC1Page(diffnum, HasItemSysnolist, C1SysNo);

                if (newC1List != null && newC1List.Count > 0)
                {
                    var distinctnewC1List = (from item in newC1List
                                             select item).Distinct();
                    WriteLog("一级类" + C1SysNo + " 找出新上架可用商品" + distinctnewC1List.Count() + "， 要补位" + diffnum);

                    foreach (OnlineList linelist in distinctnewC1List)
                    {
                        linelist.Priority = Priority;
                        linelist.locationInfo = new OnlineListLocation();
                        linelist.locationInfo.PageID = C1SysNo;
                        linelist.locationInfo.PositionID = positionid;
                        int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        Sysnolist.Add(linelist.SysNo);
                        HasItemSysnolist.Add(linelist.ProductSysNo);
                        addnum++;
                    }
                }
                #endregion
                WriteLog("一级类" + C1SysNo + "--新品上架 成功添加" + addnum);
            }
            #region 一级分类的特价商品,当前价格/市场价格,最小的前四个
            else if(positionid==(int)Category1.SpecialItem)
            {
                addnum = 0;
                List<OnlineList> specialC1List = OnlinelistDA.GetSpecialItemForC1Page(diffnum, HasItemSysnolist, C1SysNo);

                if (specialC1List != null && specialC1List.Count > 0)
                {
                    var distinctnewC1List = (from item in specialC1List
                                             select item).Distinct();
                    WriteLog("一级类" + C1SysNo + " 找出特价商品可用商品" + distinctnewC1List.Count() + "， 要补位" + diffnum);

                    foreach (OnlineList linelist in distinctnewC1List)
                    {
                        linelist.Priority = Priority;
                        linelist.locationInfo = new OnlineListLocation();
                        linelist.locationInfo.PageID = C1SysNo;
                        linelist.locationInfo.PositionID = positionid;
                        int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        Sysnolist.Add(linelist.SysNo);
                        HasItemSysnolist.Add(linelist.ProductSysNo);
                        addnum++;
                    }
                }
                WriteLog("一级类" + C1SysNo + "--特价商品 成功添加" + addnum);
            }
            #endregion
            else
            {
                addnum = 0;
                #region promotion
                //取降价最大的商品               
                List<OnlineList> promotionC1List = OnlinelistDA.GetPriceDownItemForC1Page(diffnum, HasItemSysnolist, C1SysNo);

                if (promotionC1List != null && promotionC1List.Count > 0)
                {
                    var distinctpromotionC1List = (from item in promotionC1List
                                                   select item).Distinct();
                    WriteLog("一级类" + C1SysNo + " 找出降价最大的可用商品" + distinctpromotionC1List.Count() + "， 要补位" + diffnum);

                    foreach (OnlineList linelist in distinctpromotionC1List)
                    {
                        linelist.Priority = Priority;
                        linelist.locationInfo = new OnlineListLocation();
                        linelist.locationInfo.PageID = C1SysNo;
                        linelist.locationInfo.PositionID = positionid;
                        int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        Sysnolist.Add(linelist.SysNo);
                        HasItemSysnolist.Add(linelist.ProductSysNo);
                        addnum++;
                    }
                }
                #endregion
                WriteLog("一级类" + C1SysNo + "--清库产品 成功添加" + addnum);
            }
            HasAddItemSysnolist = HasItemSysnolist;
            hasSysnolist = Sysnolist;
        }

        public static void C2pageItemProcess(int diffnum, int C2SysNo, int pagetype, int positionid, List<int> HasItemSysnolist
            , out List<int> HasAddItemSysnolist, List<int> Sysnolist, out List<int> HasSysnolist)
        {
            //4.二级分类：固定显示数量为4个
            //今日特惠最新数量为6个，其中降价金额和降价百分比各3个 所有商品不可重复  
            //降价金额=7天之内最高金额-当前金额 7天之内降价最大的商品，
            //降价百分比=（7天之内最高金额-当前金额）/7天之内最高金额*100% 

            HasAddItemSysnolist = new List<int>();
            HasSysnolist = new List<int>();
            int addnum = 0;
            #region

            List<OnlineList> promotionC2List = OnlinelistDA.GetPriceDownItemForC2(diffnum / 2, HasItemSysnolist, C2SysNo);

            if (promotionC2List != null && promotionC2List.Count > 0)
            {
                var distinctpromotionC2List = (from item in promotionC2List
                                               select item).Distinct();
                WriteLog(C2SysNo + " 二级类 找出降价金额较大的商品" + distinctpromotionC2List.Count() + "， 要补位" + diffnum / 2);

                foreach (OnlineList linelist in distinctpromotionC2List)
                {
                    linelist.Priority = Priority;
                    linelist.locationInfo = new OnlineListLocation();
                    linelist.locationInfo.PageID = C2SysNo;
                    linelist.locationInfo.PositionID = positionid;
                    int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                    if (onlinelocationsysno > 0)
                    {
                        linelist.OnlineLocationSysNo = onlinelocationsysno;
                    }
                    OnlinelistDA.Insert(linelist);
                    Sysnolist.Add(linelist.SysNo);
                    HasItemSysnolist.Add(linelist.ProductSysNo);
                    addnum++;
                }
            }
            #endregion

            #region 降价百分比
            //降价百分比=（7天之内最高金额-当前金额）/7天之内最高金额*100% 
            List<OnlineList> percentC2List = OnlinelistDA.GetPriceDownPercentItemForC2(diffnum - diffnum / 2, HasItemSysnolist, C2SysNo);
            if (percentC2List != null && percentC2List.Count > 0)
            {
                var distinctpercentC2List = (from item in percentC2List
                                             select item).Distinct();

                WriteLog(C2SysNo + " 二级类 找出降价百分比较大的商品" + distinctpercentC2List.Count() + "， 要补位" + (diffnum - diffnum / 2).ToString());

                foreach (OnlineList linelist in distinctpercentC2List)
                {
                    linelist.Priority = Priority;
                    linelist.locationInfo = new OnlineListLocation();
                    linelist.locationInfo.PageID = C2SysNo;
                    linelist.locationInfo.PositionID = positionid;
                    int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                    if (onlinelocationsysno > 0)
                    {
                        linelist.OnlineLocationSysNo = onlinelocationsysno;
                    }
                    OnlinelistDA.Insert(linelist);
                    Sysnolist.Add(linelist.SysNo);
                    HasItemSysnolist.Add(linelist.ProductSysNo);
                    addnum++;
                }
            }
            #endregion
            WriteLog("二级类" + C2SysNo + "--今日特惠 成功添加" + addnum);
            HasAddItemSysnolist = HasItemSysnolist;
            HasSysnolist = Sysnolist;
        }

        public static void C3pageItemProcess(int diffnum, int C3SysNo, int pagetype, int positionid, List<int> HasItemSysnolist
            , out List<int> HasAddItemSysnolist, List<int> Sysnolist, out List<int> HasSysnolist)
        {
            //5.三级分类：固定显示数量为2个
            //今日特惠最新数量为4个（所有商品不可重复）
            //三级分类  7天之内降价最大的商品，降价金额=7天之内最高金额-当前金额
            HasAddItemSysnolist = new List<int>();
            HasSysnolist = new List<int>();
            int addnum = 0;
            #region promotion
            //（取降价最大的商品
            List<OnlineList> promotionC3List = OnlinelistDA.GetPriceDownItemForC3(diffnum, HasItemSysnolist, C3SysNo);
            if (promotionC3List != null && promotionC3List.Count > 0)
            {
                var distinctlist = (from item in promotionC3List
                                    select item).Distinct();

                WriteLog(C3SysNo + " 三级类 找出降价最大的商品" + distinctlist.Count() + "， 要补位" + diffnum);

                foreach (OnlineList linelist in distinctlist)
                {
                    linelist.Priority = Priority;
                    linelist.locationInfo = new OnlineListLocation();
                    linelist.locationInfo.PageID = C3SysNo;
                    linelist.locationInfo.PositionID = positionid;
                    int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                    if (onlinelocationsysno > 0)
                    {
                        linelist.OnlineLocationSysNo = onlinelocationsysno;
                    }
                    OnlinelistDA.Insert(linelist);
                    Sysnolist.Add(linelist.SysNo);
                    HasItemSysnolist.Add(linelist.ProductSysNo);
                    addnum++;
                }
            }
            #endregion
            WriteLog("三级类" + C3SysNo + "--今日特惠 成功添加" + addnum);
            HasAddItemSysnolist = HasItemSysnolist;
            HasSysnolist = Sysnolist;
        }

        public static void BrandpageItemProcess(int diffnum, int BrandSysNo, int pagetype, int positionid, List<int> HasItemSysnolist
            , out List<int> HasAddItemSysnolist, List<int> Sysnolist, out List<int> HasSysnolist)
        {
            //每一个专卖店都要添加,3个位置的商品可以重复

            List<int> itmlist = new List<int>();
            itmlist = HasItemSysnolist;
            HasAddItemSysnolist = new List<int>();
            HasSysnolist = new List<int>();
            int addnum = 0;
            if (positionid == (int)Brand.Hot)
            {
                #region hotbrand
                //品牌旗舰店/品牌专卖店中当季热销 如果少于6条记录则抽取销售数量最多的商品（取销售数量最多为
                List<OnlineList> hotBrandList = OnlinelistDA.GetSaleHightItemForBrand(diffnum, HasItemSysnolist, BrandSysNo);

                if (hotBrandList != null && hotBrandList.Count > 0)
                {
                    var distincthotBrandList = (from item in hotBrandList
                                                select item).Distinct();

                    // WriteLog("专卖店" + BrandSysNo + "  找出销售数量较多的商品" + distincthotBrandList.Count() + "， 要补位" + diffnum);

                    foreach (OnlineList linelist in distincthotBrandList)
                    {
                        linelist.Priority = Priority;
                        linelist.locationInfo = new OnlineListLocation();
                        linelist.locationInfo.PageID = BrandSysNo;
                        linelist.locationInfo.PositionID = positionid;
                        int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        HasItemSysnolist.Add(linelist.ProductSysNo);
                        Sysnolist.Add(linelist.SysNo);
                        itmlist.Add(linelist.ProductSysNo);
                        addnum++;
                    }
                }
                if (addnum < diffnum)
                {
                    List<OnlineList> hotBrandList11 = OnlinelistDA.GetSaleHightItemForBrand(diffnum - addnum, itmlist, BrandSysNo);

                    if (hotBrandList11 != null && hotBrandList11.Count > 0)
                    {
                        var distincthotBrandList = (from item in hotBrandList11
                                                    select item).Distinct();

                        foreach (OnlineList linelist in distincthotBrandList)
                        {
                            linelist.Priority = Priority + 1;
                            linelist.locationInfo = new OnlineListLocation();
                            linelist.locationInfo.PageID = BrandSysNo;
                            linelist.locationInfo.PositionID = positionid;
                            int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            HasItemSysnolist.Add(linelist.ProductSysNo);
                            Sysnolist.Add(linelist.SysNo);
                            addnum++;
                        }
                    }
                }
                WriteLog("专卖店" + BrandSysNo + "--当季热销 成功添加" + addnum);
                #endregion
            }
            else if (positionid == (int)Brand.New)
            {
                addnum = 0;
                #region newbrand
                //品牌旗舰店/品牌专卖店中新品上架 如果少于6条记录则抽取最新上架的产品 （取最新上架的产品               
                //该品牌商品总数量少于3个时则只显示当季热销，
                //该品牌商品总数量少于6个时则显示让利促销和当季热销
                int n = OnlinelistDA.CheckBrandItemNum(BrandSysNo);
                if (n > 3)
                {
                    List<OnlineList> newBrandList = OnlinelistDA.GetNewItemForBrand(diffnum, HasItemSysnolist, BrandSysNo);

                    if (newBrandList != null && newBrandList.Count > 0)
                    {
                        var distinctnewBrandList = (from item in newBrandList
                                                    select item).Distinct();
                        //  WriteLog("专卖店" + BrandSysNo + "  找出新上架的商品" + distinctnewBrandList.Count() + "， 要补位" + diffnum);

                        foreach (OnlineList linelist in distinctnewBrandList)
                        {
                            linelist.Priority = Priority;
                            linelist.locationInfo = new OnlineListLocation();
                            linelist.locationInfo.PageID = BrandSysNo;
                            linelist.locationInfo.PositionID = positionid;
                            int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            Sysnolist.Add(linelist.SysNo);
                            HasItemSysnolist.Add(linelist.ProductSysNo);
                            itmlist.Add(linelist.ProductSysNo);
                            addnum++;
                        }

                    }
                    if (addnum < diffnum)
                    {
                        List<OnlineList> newBrandList11 = OnlinelistDA.GetSaleHightItemForBrand(diffnum - addnum, itmlist, BrandSysNo);

                        if (newBrandList11 != null && newBrandList11.Count > 0)
                        {
                            var distinctList = (from item in newBrandList11
                                                select item).Distinct();

                            foreach (OnlineList linelist in distinctList)
                            {
                                linelist.Priority = Priority + 1;
                                linelist.locationInfo = new OnlineListLocation();
                                linelist.locationInfo.PageID = BrandSysNo;
                                linelist.locationInfo.PositionID = positionid;
                                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                                if (onlinelocationsysno > 0)
                                {
                                    linelist.OnlineLocationSysNo = onlinelocationsysno;
                                }
                                OnlinelistDA.Insert(linelist);
                                HasItemSysnolist.Add(linelist.ProductSysNo);
                                Sysnolist.Add(linelist.SysNo);
                                addnum++;
                            }
                        }
                    }
                    WriteLog("专卖店" + BrandSysNo + "--新品上架 成功添加" + addnum);
                }
                #endregion
            }
            else
            {
                addnum = 0;
                #region promotionBrand
                //品牌旗舰店/品牌专卖店中让利促销 如果少于6条记录则抽取降价最大的商品（取降价最大的商品
                int n = OnlinelistDA.CheckBrandItemNum(BrandSysNo);
                if (n > 6)
                {
                    List<OnlineList> promotionBrandList = OnlinelistDA.GetPriceDownItemForBrand(diffnum, HasItemSysnolist, BrandSysNo);

                    if (promotionBrandList != null && promotionBrandList.Count > 0)
                    {
                        var distinctpromotionBrandList = (from item in promotionBrandList
                                                          select item).Distinct();

                        //WriteLog("专卖店" + BrandSysNo + "  找出降价比较大的商品" + distinctpromotionBrandList.Count() + "， 要补位" + diffnum);

                        foreach (OnlineList linelist in distinctpromotionBrandList)
                        {
                            linelist.Priority = Priority;
                            linelist.locationInfo = new OnlineListLocation();
                            linelist.locationInfo.PageID = BrandSysNo;
                            linelist.locationInfo.PositionID = positionid;
                            int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            Sysnolist.Add(linelist.SysNo);
                            HasItemSysnolist.Add(linelist.ProductSysNo);
                            itmlist.Add(linelist.ProductSysNo);
                            addnum++;
                        }
                    }
                    if (addnum < diffnum)
                    {
                        List<OnlineList> promotionBrandList11 = OnlinelistDA.GetSaleHightItemForBrand(diffnum - addnum, itmlist, BrandSysNo);

                        if (promotionBrandList11 != null && promotionBrandList11.Count > 0)
                        {
                            var distinctList = (from item in promotionBrandList11
                                                select item).Distinct();

                            foreach (OnlineList linelist in distinctList)
                            {
                                linelist.Priority = Priority + 1;
                                linelist.locationInfo = new OnlineListLocation();
                                linelist.locationInfo.PageID = BrandSysNo;
                                linelist.locationInfo.PositionID = positionid;
                                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, linelist.locationInfo.PageID);

                                if (onlinelocationsysno > 0)
                                {
                                    linelist.OnlineLocationSysNo = onlinelocationsysno;
                                }
                                OnlinelistDA.Insert(linelist);
                                HasItemSysnolist.Add(linelist.ProductSysNo);
                                Sysnolist.Add(linelist.SysNo);
                                addnum++;
                            }
                        }
                    }
                    WriteLog("专卖店" + BrandSysNo + "--让利促销 成功添加" + addnum);
                }
                #endregion
            }
            HasAddItemSysnolist = HasItemSysnolist;
            HasSysnolist = Sysnolist;
        }


        public static void DeleteOnlineListItem(int pagetype, int positionid, List<int> sysnolist)
        {
            OnlinelistDA.DeleteOnlineListItem(pagetype, positionid, sysnolist, BeginTime);
        }

        #endregion

        public static void SendExceptionInfoEmail(string ErrorMsg)
        {
            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                OnlinelistDA.SendMailAboutExceptionInfo(ErrorMsg);
            }
        }

        public static void WriteConsoleInfo(string content)
        {
            Console.WriteLine(content);
        }

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
            if (jobContext != null)
            {
                jobContext.Message += content + "\r\n";
            }
        }
    }
}
