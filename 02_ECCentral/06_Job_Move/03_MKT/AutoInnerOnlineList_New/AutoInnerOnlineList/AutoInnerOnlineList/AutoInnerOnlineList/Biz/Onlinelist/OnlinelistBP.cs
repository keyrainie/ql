using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.ECommerceMgmt.AutoInnerOnlineList.DA;
using IPP.ECommerceMgmt.AutoInnerOnlineList.BusinessEntities;
using Newegg.Oversea.Framework.JobConsole.Client;
/***************************************************************
 * Author:Kathy Gao
 * Edit Date:2011-1-25
 * Content:首页二期改版  需要补位的
页面类型	位置	商品数量	自动补位	补足数量
一级分类	明星产品	3个	     是	         6个
一级分类	新品上架	4个	     是	         8个
二级分类	新品上架	4个	     是          8个
二级分类	特别推荐	4个	     是	         8个
三级分类	今日特惠	2个	     是	         4个 
品牌专卖    新品上架             是          6  
品牌专卖    让利促销             是          6  
品牌专卖    当即热销             是          6  
百元品推荐  推荐专属             是  
百元店      最新降价             是      （所有商品金额（CurrentPrice）小于等于110，
百元店      百元新品1~4          是        日用百货大类补6个，其他大类每个大类各1个）
Domainlist  大                   是          12  选取降价的商品，
Domainlist  小                   是          10  选取销量最高的商品
  * 
 * ************************************************************/

namespace IPP.ECommerceMgmt.AutoInnerOnlineList.Biz
{
    public class OnlinelistBP
    {
        #region rules and const

        const int pagenumm = 4;
        const int SpecialItemMaxNumPerPage = 12;
        const int RecommendedItemMaxNumPerPage = 20;

        const int BrandItemMaxNum = 6;

        const int NewItemMaxNum = 10;
        const int HotItemMaxNum = 20;

        const int C1StarNum = 4;
        const int C1NewNum = 4;
        private const int C1SpecialNum = 4;

        const int C2RecommendNum = 8;
        const int C2NewNum = 8;

        const int C3ItemMaxNum = 5;

        const int HundredStoresNum = 6;

        const int DomainBigNum = 12;
        const int DomainSmallNum = 10;


        private static List<int> _itemSysnolist;
        private static List<int> _groupSysNo;
        private static List<int> _oldSysnolist;
        private static List<int> _oldgroupSysNo;

        //网站首页：范围：5大类的二级类商品:笔记本电脑，手机，数码相机/摄像机，显示器，平板电视 "536,549,550,535,711"; 
        static string homeC2sysnolist = ConfigurationManager.AppSettings["OnlnelistHomeC2Sysnolist"];
        static int Priority = Convert.ToInt32(ConfigurationManager.AppSettings["OnlnelistItemPriority"]);
        //清除历史数据离现在时间
        static string DeleteDataTime = ConfigurationManager.AppSettings["DeleteDataTime"];
        static string AppliancesStores = ConfigurationManager.AppSettings["AppliancesStores"];
        static decimal HundredShopPrice = Convert.ToDecimal(ConfigurationManager.AppSettings["HundredShopPrice"]);

        //测试专卖店
        static int TestBrandSysNo = Convert.ToInt32(ConfigurationManager.AppSettings["TestBrand"]);

        /// <summary>
        /// 业务日志文件
        /// </summary>
        public static string BizLogFile;
        #endregion

        #region Property
        protected static List<int> ItemSysnolist
        {
            get
            {
                if (_itemSysnolist == null)
                {
                    _itemSysnolist = new List<int>();
                }
                return _itemSysnolist;
            }
            set { _itemSysnolist = value; }
        }
        protected static List<int> GroupSysNo
        {
            get
            {
                if (_groupSysNo == null)
                {
                    _groupSysNo = new List<int>();
                }
                return _groupSysNo;
            }
            set { _groupSysNo = value; }
        }
        protected static List<int> nitemSysnolist { get; set; }


        List<int> groupSysNo = new List<int>();
        #endregion

        #region Business

        public static JobContext jobContext = null;

        public static void CheckOnlinelistItem()
        {

            string endMsg = string.Empty;
            WriteLog("******************** Begin ***********************");
            WriteLog("*******商品推荐管理自动补位job开始运行************");

            try
            {
                //添加清除一周以前无效的系统推荐数据
                OnlinelistDA.ClearTableOnLinelist(DeleteDataTime);

                ////domainlist
                //WriteLog("\r\n" + DateTime.Now + "正在进行首页商品补位......");
                //SetDomainItem();

                // 1级类别
                WriteLog("\r\n" + DateTime.Now + "正在进行一级类页面商品补位......");
                SetC1PageItem();

                //2级类别
                WriteLog("\r\n" + DateTime.Now + "正在进行二级类页面商品补位......");
                SetC2PageItem();

                //3级类别
                WriteLog("\r\n" + DateTime.Now + "正在进行三级类页面商品补位......");
                SetC3PageItem();

                ////专卖店
                //WriteLog("\r\n" + DateTime.Now + "正在进行专卖店商品补位......");
                //SetBrandItem();

                //百元店
                //WriteLog("\r\n" + DateTime.Now + "正在进行百元店推荐专属&百元店商品补位......");
                //SetHundredShopItem();

                endMsg = DateTime.Now + " 本次job成功结束!";
                WriteLog(endMsg);

            }
            catch (Exception er)
            {
                endMsg = DateTime.Now + " job运行异常，异常信息如下：\r\n " + er.Message;
                SendExceptionInfoEmail(endMsg);
                WriteLog(endMsg);
            }
            WriteLog("********************** End *************************");
        }

        #region Domain
        private static void SetDomainItem()
        {
            //页面类型增加：
            //DomainList[i+1].Name，value=10(DomainList[i+1].Sysno)
            //这些页面类型下的位置都是“大”（value=100）和“小”（value=101） 
            List<OnlineList> domainItemlist = new List<OnlineList>();
            List<int> productSysNolist = new List<int>();
            List<int> HasAddProductSysNolist = new List<int>();

            List<int> hasGroupSysNo = new List<int>();
            List<int> groupSysNo = new List<int>();

            List<int> SysNolist = new List<int>();//原来记录的sysno
            List<int> HasAddSysNolist = new List<int>(); //原来记录和新添记录的sysno
            List<Domain> domainlist = OnlinelistDA.GetNewItemDomainList();
            if (domainlist != null && domainlist.Count > 0)
            {
                foreach (Domain domainItem in domainlist)
                {
                    //为待删除数据打标记                  
                    OnlinelistDA.UpdateInvalidData(domainItem.DomianValue, (int)DomainList.Big, 0);
                    OnlinelistDA.UpdateInvalidData(domainItem.DomianValue, (int)DomainList.Small, 0);

                    domainItemlist = OnlinelistDA.GetOnlinelistItemByPageType(domainItem.DomianValue);
                    if (domainItemlist != null && domainItemlist.Count > 0)
                    {
                        #region 有手工记录
                        var hasDomainlist = (from item in domainItemlist
                                             where (item.locationInfo.PositionID == (int)DomainList.Big
                                             || item.locationInfo.PositionID == (int)DomainList.Small)
                                             select item).Distinct().ToList();

                        if (hasDomainlist != null && hasDomainlist.Count >= 0)
                        {
                            #region  domain big & small

                            int bigCount = 0;
                            int SmallCount = 0;
                            foreach (OnlineList online in hasDomainlist)
                            {
                                productSysNolist.Add(online.ProductSysNo); //该位置已经存在的商品记录 保证添加的商品不重复
                                SysNolist.Add(online.SysNo);

                                groupSysNo.Add(online.ProductGroupSysNo);

                                if (online.locationInfo.PositionID == (int)DomainList.Big)
                                {
                                    bigCount++;
                                }
                                else if (online.locationInfo.PositionID == (int)DomainList.Small)
                                {
                                    SmallCount++;
                                }
                            }

                            DomainListItemProcess(DomainBigNum - bigCount, domainItem, (int)DomainList.Big
                                , productSysNolist, out HasAddProductSysNolist, SysNolist, out HasAddSysNolist, groupSysNo, out hasGroupSysNo);
                            productSysNolist = HasAddProductSysNolist;
                            SysNolist = HasAddSysNolist;
                            groupSysNo = hasGroupSysNo;
                            DeleteOnlineListItem(domainItem.DomianValue, (int)DomainList.Big, 0);

                            DomainListItemProcess(DomainSmallNum - SmallCount, domainItem, (int)DomainList.Small
                            , productSysNolist, out HasAddProductSysNolist, SysNolist, out HasAddSysNolist, groupSysNo, out hasGroupSysNo);
                            productSysNolist = HasAddProductSysNolist;
                            SysNolist = HasAddSysNolist;
                            groupSysNo = hasGroupSysNo;
                            DeleteOnlineListItem(domainItem.DomianValue, (int)DomainList.Small, 0);

                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region 该页面类型没有记录
                        WriteLog("首页没有有效记录，需全部新添加。");

                        DomainListItemProcess(DomainBigNum, domainItem, (int)DomainList.Big
                            , productSysNolist, out HasAddProductSysNolist, SysNolist, out  HasAddSysNolist, groupSysNo, out hasGroupSysNo);
                        productSysNolist = HasAddProductSysNolist;
                        SysNolist = HasAddSysNolist;
                        groupSysNo = hasGroupSysNo;

                        DeleteOnlineListItem(domainItem.DomianValue, (int)DomainList.Big, 0);

                        DomainListItemProcess(DomainSmallNum, domainItem, (int)DomainList.Small
                            , productSysNolist, out HasAddProductSysNolist, SysNolist, out  HasAddSysNolist, groupSysNo, out hasGroupSysNo);
                        productSysNolist = HasAddProductSysNolist;
                        SysNolist = HasAddSysNolist;
                        groupSysNo = hasGroupSysNo;
                        DeleteOnlineListItem(domainItem.DomianValue, (int)DomainList.Small, 0);
                        #endregion
                    }
                }
            }
        }
        /// <summary>       
        ///1）补位所选择的商品必须从此位置对应的大类中取，并排除需排除的分类（Domain表）
        ///2）位置“大”：12个；选取1周内降价的商品，1周内的商品数量不足的，
        ///依次取1月、3月、6月、1年内的降价商品。同一时间段内的降价商品，
        /// 幅度越大的越优先，降价幅度指当前价格与时间段内的最高价格的差额
        ///3）位置“小”：10个；选取1周销量最高的商品；和位置“大”排重
        /// </summary>
        /// <param name="diffnum"></param>
        /// <param name="domainItem"></param>
        /// <param name="positionID"></param>
        /// <param name="itemSysNolist"></param>
        /// <param name="hasAddItemSysNolist"></param>
        /// <param name="sysNolist"></param>
        /// <param name="hasAddSysNolist"></param>
        private static void DomainListItemProcess(int diffnum, Domain domainItem, int positionID, List<int> itemSysNolist
            , out List<int> hasAddItemSysNolist, List<int> sysNolist, out List<int> hasAddSysNolist
            , List<int> groupSysNo, out List<int> hasGroupSysNo)
        {

            hasAddItemSysNolist = new List<int>();
            hasAddSysNolist = new List<int>();
            hasGroupSysNo = new List<int>();
            int loopNum = 0;
            int addnum = 0;
            List<OnlineList> findDomainItemList = new List<OnlineList>();
            List<OnlineList> SecfindDomainItemList = new List<OnlineList>();
            List<OnlineList> resultList = new List<OnlineList>();

            int onlinelocationsysno = OnlinelistDA.GetLocationSysno(domainItem.DomianValue, positionID, 0, domainItem.DomainName);
            if (positionID == (int)DomainList.Big)
            {
                findDomainItemList = OnlinelistDA.GetPriceDownItemForHomePage(diffnum, domainItem, itemSysNolist, groupSysNo);
                findDomainItemList = FilterItem(findDomainItemList, groupSysNo, out hasGroupSysNo, resultList);
                groupSysNo = hasGroupSysNo;
                while (diffnum > findDomainItemList.Count && loopNum <= 2)
                {
                    loopNum++;
                    SecfindDomainItemList = OnlinelistDA.GetPriceDownItemForHomePage(diffnum - findDomainItemList.Count
                        , domainItem, itemSysNolist, groupSysNo);
                    findDomainItemList = FilterItem(SecfindDomainItemList, groupSysNo, out hasGroupSysNo, findDomainItemList);
                    groupSysNo = hasGroupSysNo;
                }
            }
            else if (positionID == (int)DomainList.Small)
            {
                findDomainItemList = OnlinelistDA.GetSaleHightItemForHomePage(diffnum, domainItem, itemSysNolist, groupSysNo);
                findDomainItemList = FilterItem(findDomainItemList, groupSysNo, out hasGroupSysNo, resultList);
                groupSysNo = hasGroupSysNo;
                while (diffnum > findDomainItemList.Count && loopNum <= 2)
                {
                    loopNum++;
                    SecfindDomainItemList = OnlinelistDA.GetSaleHightItemForHomePage(diffnum - findDomainItemList.Count
                            , domainItem, itemSysNolist, groupSysNo);
                    findDomainItemList = FilterItem(SecfindDomainItemList, groupSysNo, out hasGroupSysNo, findDomainItemList);
                    groupSysNo = hasGroupSysNo;
                }
            }
            if (findDomainItemList != null && findDomainItemList.Count > 0)
            {
                foreach (OnlineList linelist in findDomainItemList)
                {
                    linelist.Priority = Priority;
                    if (onlinelocationsysno > 0)
                    {
                        linelist.OnlineLocationSysNo = onlinelocationsysno;
                    }
                    OnlinelistDA.Insert(linelist);
                    sysNolist.Add(linelist.SysNo);
                    itemSysNolist.Add(linelist.ProductSysNo);
                    groupSysNo.Add(linelist.ProductGroupSysNo);
                    addnum++;
                }
            }
            if (addnum > 0)
            {
                string msg = "首页:{0} {1}图标商品 成功添加{2}";
                WriteLog(string.Format(msg, domainItem.DomainName, positionID == (int)DomainList.Big ? "大" : "小", addnum));
            }

            hasAddItemSysNolist = itemSysNolist;
            hasAddSysNolist = sysNolist;
            hasGroupSysNo = groupSysNo;
        }

        private static List<OnlineList> FilterItem(List<OnlineList> secfinfDomainItemList, List<int> groupSysNo, out List<int> hasGroupSysNo
            , List<OnlineList> hasFindList)
        {
            hasGroupSysNo = new List<int>();

            if (secfinfDomainItemList != null && secfinfDomainItemList.Count > 0)
            {
                foreach (OnlineList item in secfinfDomainItemList)
                {
                    if (CheckItemGroupUnique(item, groupSysNo))
                    {
                        groupSysNo.Add(item.ProductGroupSysNo);
                        hasFindList.Add(item);
                    }
                }
            }
            hasGroupSysNo = groupSysNo;
            return hasFindList;
        }

        private static bool CheckItemGroupUnique(OnlineList item, List<int> groupSysNo)
        {
            if (groupSysNo != null)
            {
                foreach (int gsysno in groupSysNo)
                {
                    if (item.ProductGroupSysNo == gsysno)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region C1
        public static void SetC1PageItem()
        {
            bool newItemFlag = true;
            bool starFlag = true;
            bool specialFlag = true;

            //所有C1下的所有商品
            List<OnlineList> C1Pagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.C1page);

            //所有C1目录列表
            List<OnlineList> C1SysNolist = OnlinelistDA.GetAllC1SysNolist();

            if (C1Pagelist != null && C1Pagelist.Count > 0)
            {
                if (C1SysNolist != null)
                {

                    var NewItemlist = (from item in C1Pagelist
                                       where item.locationInfo.PositionID == (int)Category1.New
                                       select item).ToList();

                    var starItemlist = (from item in C1Pagelist
                                        where item.locationInfo.PositionID == (int)Category1.StarItem
                                        select item).ToList();
                    //特价商品
                    var specialItemList = (from item in C1Pagelist
                                           where item.locationInfo.PositionID == (int)Category1.Pricedown
                                           select item).ToList();

                    foreach (OnlineList oll in C1SysNolist)
                    {
                        if (NewItemlist != null && NewItemlist.Count >= 0)
                        {
                            #region  新品上架
                            if (newItemFlag)
                            {
                                WriteLog("正在进行 一级类页面--新品上架，现在共有记录" + NewItemlist.Count);
                                newItemFlag = false;
                            }
                            foreach (OnlineList linelist in NewItemlist)
                            {
                                ItemSysnolist.Add(linelist.ProductSysNo);
                                GroupSysNo.Add(linelist.ProductGroupSysNo);
                            }
                            var c1newlist = (from item in NewItemlist
                                             where item.C1SysNo == oll.C1SysNo
                                             select item).ToList();

                            int diffnum = C1NewNum - c1newlist.Count;

                            if (diffnum > 0)
                            {
                                C1pageItemProcess(diffnum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.New);
                            }
                            DeleteOnlineListItem((int)PageType.C1page, (int)Category1.New, oll.C1SysNo);
                            #endregion
                        }
                        if (starItemlist != null && starItemlist.Count >= 0)
                        {
                            #region 一级类页面 明星商品
                            if (starItemlist != null && starItemlist.Count >= 0)
                            {
                                if (starFlag)
                                {
                                    WriteLog("正在进行 一级类页面--今日推荐，现在共有记录" + starItemlist.Count);
                                    starFlag = false;
                                }
                                foreach (OnlineList linelist in starItemlist)
                                {
                                    ItemSysnolist.Add(linelist.ProductSysNo);
                                    GroupSysNo.Add(linelist.ProductGroupSysNo);
                                }
                                var c1hotlist = (from im in starItemlist
                                                 where im.C1SysNo == oll.C1SysNo
                                                 select im).ToList();
                                int diffnum = C1StarNum - c1hotlist.Count;
                                if (diffnum > 0)
                                {
                                    C1pageItemProcess(diffnum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.StarItem);
                                }
                                DeleteOnlineListItem((int)PageType.C1page, (int)Category1.StarItem, oll.C1SysNo);

                            }
                            #endregion
                        }
                        if (specialItemList.Count > 0)
                        {
                            #region 一级类页面 特价商品
                            if (specialFlag)
                            {
                                WriteLog("正在进行 一级类页面--特价商品，现在共有记录" + specialItemList.Count);
                                specialFlag = false;
                            }
                            foreach (OnlineList linelist in specialItemList)
                            {
                                ItemSysnolist.Add(linelist.ProductSysNo);
                                GroupSysNo.Add(linelist.ProductGroupSysNo);
                            }
                            var c1hotlist = (from im in starItemlist
                                             where im.C1SysNo == oll.C1SysNo
                                             select im).ToList();
                            int diffnum = C1SpecialNum - c1hotlist.Count;
                            if (diffnum > 0)
                            {
                                C1pageItemProcess(diffnum, oll.C1SysNo, (int)PageType.C1page, (int)Category1.Pricedown);
                            }
                            DeleteOnlineListItem((int)PageType.C1page, (int)Category1.Pricedown, oll.C1SysNo);
                            #endregion
                        }
                        disposeProperty();
                    }
                    newItemFlag = true;
                    starFlag = true;
                    specialFlag = true;
                }
            }
            else
            {
                if (C1SysNolist != null)
                {
                    WriteLog("一级类页面没有有效记录，需全部重新添加.");
                    foreach (OnlineList c1 in C1SysNolist)
                    {
                        #region 一级类页面 新品上架
                        if (newItemFlag) { WriteLog("正在进行 一级类页面--新品上架"); newItemFlag = false; }
                        C1pageItemProcess(C1NewNum, c1.C1SysNo, (int)PageType.C1page, (int)Category1.New);
                        //修改状态为 删除
                        DeleteOnlineListItem((int)PageType.C1page, (int)Category1.New, c1.C1SysNo);
                        #endregion

                        #region 一级类页面 今日推荐
                        if (starFlag) { WriteLog("正在进行 一级类页面--明星商品"); starFlag = false; }
                        C1pageItemProcess(C1StarNum, c1.C1SysNo, (int)PageType.C1page, (int)Category1.StarItem);
                        //修改状态为 删除
                        DeleteOnlineListItem((int)PageType.C1page, (int)Category1.StarItem, c1.C1SysNo);
                        #endregion

                        #region 一级类页面 特价商品
                        if (specialFlag) { WriteLog("正在进行 一级类页面--特价商品"); specialFlag = false; }
                        C1pageItemProcess(C1SpecialNum, c1.C1SysNo, (int)PageType.C1page, (int)Category1.Pricedown);
                        //修改状态为 删除
                        DeleteOnlineListItem((int)PageType.C1page, (int)Category1.Pricedown, c1.C1SysNo);
                        #endregion

                        disposeProperty();
                    }


                }
            }


        }
        public static void C1pageItemProcess(int diffnum, int C1SysNo, int pagetype, int positionid)
        {
            //为待删除数据打标记 pagetype = C1 positionid = new C1SysNo = 1级目录编号
            OnlinelistDA.UpdateInvalidData(pagetype, positionid, C1SysNo);

            //新品上架和热销商品的商品信息不可重复
            int addnum = 0;
            int loopNum = 0;
            List<int> hasGroupSysNo = new List<int>();
            List<OnlineList> C1StarItemList = new List<OnlineList>();
            List<OnlineList> secC1StarItemList = new List<OnlineList>();
            //Category1.StarItem 热销
            if (positionid == (int)Category1.StarItem)
            {
                #region  StarItem
                //取销售数量最多为              
                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, C1SysNo, "今日推荐");
                C1StarItemList = OnlinelistDA.GetSaleHightItem(diffnum, C1SysNo, ItemSysnolist, GroupSysNo);

                if (C1StarItemList != null && C1StarItemList.Count > 0)
                {
                    C1StarItemList = FilterItem(C1StarItemList, GroupSysNo, out hasGroupSysNo, secC1StarItemList);
                    GroupSysNo = hasGroupSysNo;
                    while (diffnum > C1StarItemList.Count && loopNum <= 2)
                    {
                        loopNum++;
                        foreach (int item in C1StarItemList.Select(p => p.ProductSysNo).Distinct().ToList())
                        {
                            ItemSysnolist.Add(item);
                        }
                        secC1StarItemList = OnlinelistDA.GetSaleHightItem(diffnum - C1StarItemList.Count, C1SysNo
                                , ItemSysnolist, GroupSysNo);
                        C1StarItemList = FilterItem(secC1StarItemList, GroupSysNo, out hasGroupSysNo, C1StarItemList);
                        GroupSysNo = hasGroupSysNo;

                    }
                    WriteLog("一级类:" + C1SysNo + " 找出销售较多的商品" + C1StarItemList.Count() + "， 要补位" + diffnum);
                    foreach (OnlineList linelist in C1StarItemList)
                    {
                        linelist.Priority = Priority;
                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        addnum++;
                    }
                }
                #endregion
                if (addnum > 0)
                {
                    WriteLog("一级类:" + C1SysNo + "今日推荐 成功添加" + addnum);
                }
            }
            //新蛋上架
            else if (positionid == (int)Category1.New)
            {
                addnum = 0;
                #region newc1
                //取最新上架的产品     
                List<OnlineList> secnewC1List = new List<OnlineList>();
                List<OnlineList> newC1List = OnlinelistDA.GetNewItem(diffnum, C1SysNo, ItemSysnolist, GroupSysNo);
                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, C1SysNo, "新品上架");

                if (newC1List != null && newC1List.Count > 0)
                {
                    newC1List = FilterItem(newC1List, GroupSysNo, out hasGroupSysNo, secnewC1List);
                    GroupSysNo = hasGroupSysNo;
                    while (diffnum > newC1List.Count && loopNum <= 2)
                    {
                        loopNum++;
                        ItemSysnolist = newC1List.Count > 0
                          ? newC1List.Select(p => p.ProductSysNo).Distinct().ToList() : new List<Int32>();
                        secnewC1List = OnlinelistDA.GetNewItem(diffnum - newC1List.Count, C1SysNo
                            , ItemSysnolist, GroupSysNo);
                        newC1List = FilterItem(secnewC1List, GroupSysNo, out hasGroupSysNo, newC1List);
                        GroupSysNo = hasGroupSysNo;
                    }

                    WriteLog("一级类" + C1SysNo + "找出新上架可用商品" + newC1List.Count() + "， 要补位" + diffnum);

                    foreach (OnlineList linelist in newC1List)
                    {
                        linelist.Priority = Priority;
                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        addnum++;
                    }
                }
                #endregion
                if (addnum > 0)
                {
                    WriteLog("一级类" + C1SysNo + "新品上架 成功添加" + addnum);
                }
            }
            else if (positionid == (int)Category1.Pricedown)
            {
                addnum = 0;
                #region 特价商品
                //取最新上架的产品     
                List<OnlineList> secnewC1List = new List<OnlineList>();
                List<OnlineList> newC1List = OnlinelistDA.GetSpecialItem(diffnum, C1SysNo, ItemSysnolist, GroupSysNo);
                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, C1SysNo, "特价商品");

                if (newC1List != null && newC1List.Count > 0)
                {
                    newC1List = FilterItem(newC1List, GroupSysNo, out hasGroupSysNo, secnewC1List);
                    GroupSysNo = hasGroupSysNo;
                    while (diffnum > newC1List.Count && loopNum <= 2)
                    {
                        loopNum++;
                        ItemSysnolist = newC1List.Count > 0
                          ? newC1List.Select(p => p.ProductSysNo).Distinct().ToList() : new List<Int32>();
                        secnewC1List = OnlinelistDA.GetSpecialItem(diffnum - newC1List.Count, C1SysNo
                            , ItemSysnolist, GroupSysNo);
                        newC1List = FilterItem(secnewC1List, GroupSysNo, out hasGroupSysNo, newC1List);
                        GroupSysNo = hasGroupSysNo;
                    }

                    WriteLog("一级类" + C1SysNo + "找出特价商品可用商品" + newC1List.Count() + "， 要补位" + diffnum);

                    foreach (OnlineList linelist in newC1List)
                    {
                        linelist.Priority = Priority;
                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        addnum++;
                    }
                }
                #endregion
                if (addnum > 0)
                {
                    WriteLog("一级类" + C1SysNo + "特价商品 成功添加" + addnum);
                }
            }
        }
        #endregion

        #region C2
        public static void SetC2PageItem()
        {

            bool newItemFlag = true;
            bool reccommendFlag = true;

            List<OnlineList> C2Pagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.C2page);

            List<OnlineList> C2SysNolist = OnlinelistDA.GetAllC2SysNolist();

            if (C2Pagelist != null && C2Pagelist.Count > 0)
            {
                // 二级分类 新品上架
                //var C2NewItemlist = (from item in C2Pagelist
                //                     where item.locationInfo.PositionID == (int)Category2.New
                //                     select item).ToList();

                // 二级分类 特别推荐
                var C2ReccItemlist = (from item in C2Pagelist
                                      where item.locationInfo.PositionID == (int)Category2.Reccommend
                                      select item).ToList();

                if (C2SysNolist != null && C2SysNolist.Count > 0)
                {
                    foreach (OnlineList oll in C2SysNolist)
                    {

                        //if (C2NewItemlist != null && C2NewItemlist.Count >= 0)
                        //{
                        //    #region New
                        //    if (newItemFlag)
                        //    {
                        //        WriteLog("正在进行 二级分类--新品上架，已有记录" + C2NewItemlist.Count);
                        //        newItemFlag = false;
                        //    }
                        //    foreach (OnlineList linelist in C2NewItemlist)
                        //    {
                        //        ItemSysnolist.Add(linelist.ProductSysNo);
                        //        GroupSysNo.Add(linelist.ProductGroupSysNo);
                        //    }
                        //    var haslist = (from im in C2NewItemlist
                        //                   where im.C2SysNo == oll.C2SysNo
                        //                   select im).ToList();
                        //    int diffnum = C2NewNum - haslist.Count;
                        //    if (diffnum > 0)
                        //    {
                        //        C2pageItemProcess(diffnum, oll.C2SysNo, (int)PageType.C2page, (int)Category2.New);
                        //    }
                        //    DeleteOnlineListItem((int)PageType.C2page, (int)Category2.New, oll.C2SysNo);
                        //    #endregion
                        //}
                        if (C2ReccItemlist != null && C2ReccItemlist.Count >= 0)
                        {
                            #region Reccommend
                            if (reccommendFlag)
                            {
                                WriteLog("正在进行 二级分类-- 特别推荐，已有记录" + C2ReccItemlist.Count);
                                reccommendFlag = false;
                            }
                            foreach (OnlineList linelist in C2ReccItemlist)
                            {
                                ItemSysnolist.Add(linelist.ProductSysNo);
                                GroupSysNo.Add(linelist.ProductGroupSysNo);
                            }
                            var haslist = (from im in C2ReccItemlist
                                           where im.C2SysNo == oll.C2SysNo
                                           select im).ToList();

                            int diffnum = C2RecommendNum - haslist.Count;
                            if (diffnum > 0)
                            {
                                C2pageItemProcess(diffnum, oll.C2SysNo, (int)PageType.C2page, (int)Category2.Reccommend);
                            }
                            DeleteOnlineListItem((int)PageType.C2page, (int)Category2.Reccommend, oll.C2SysNo);
                            #endregion
                        }
                        disposeProperty();
                    }
                    newItemFlag = true;
                    reccommendFlag = true;
                }
            }
            else
            {
                WriteLog("二级类页面 没有有效记录，需全部新添加商品。");
                if (C2SysNolist != null && C2SysNolist.Count > 0)
                {
                    foreach (OnlineList oll in C2SysNolist)
                    {
                        #region New
                        //if (newItemFlag) { WriteLog("正在进行 二级类页面--新品上架"); newItemFlag = false; }
                        //C2pageItemProcess(C2NewNum, oll.C2SysNo, (int)PageType.C2page, (int)Category2.New);
                        //DeleteOnlineListItem((int)PageType.C2page, (int)Category2.Reccommend, oll.C2SysNo);
                        #endregion
                        #region Reccommend
                        if (reccommendFlag) { WriteLog("正在进行 二级类页面--特别推荐"); reccommendFlag = false; }
                        C2pageItemProcess(C2RecommendNum, oll.C2SysNo, (int)PageType.C2page, (int)Category2.Reccommend);
                        DeleteOnlineListItem((int)PageType.C2page, (int)Category2.Reccommend, oll.C2SysNo);
                        #endregion
                        disposeProperty();
                    }
                }
            }
        }
        public static void C2pageItemProcess(int diffnum, int C2SysNo, int pagetype, int positionid)
        {
            //为待删除数据打标记
            OnlinelistDA.UpdateInvalidData(pagetype, positionid, C2SysNo);
            List<int> hasGroupSysNo = new List<int>();

            int loopNum = 0;
            int addnum = 0;
            List<OnlineList> reccommendC2List = new List<OnlineList>();
            List<OnlineList> secreccommendC2List = new List<OnlineList>();
            if (positionid == (int)Category2.Reccommend)
            {
                #region  recc
                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, C2SysNo, "特别推荐");
                reccommendC2List = OnlinelistDA.GetPriceDownItem(diffnum, C2SysNo, ItemSysnolist, GroupSysNo);
                if (reccommendC2List != null && reccommendC2List.Count > 0)
                {
                    reccommendC2List = FilterItem(reccommendC2List, GroupSysNo, out hasGroupSysNo, secreccommendC2List);
                    GroupSysNo = hasGroupSysNo;
                    while (diffnum > reccommendC2List.Count && loopNum <= 2)
                    {
                        loopNum++;
                        foreach (int item in reccommendC2List.Select(p => p.ProductSysNo).Distinct().ToList())
                        {
                            ItemSysnolist.Add(item);
                        }
                        secreccommendC2List = OnlinelistDA.GetPriceDownItem(diffnum - reccommendC2List.Count, C2SysNo
                                , ItemSysnolist, GroupSysNo);
                        reccommendC2List = FilterItem(secreccommendC2List, GroupSysNo, out hasGroupSysNo, reccommendC2List);
                        GroupSysNo = hasGroupSysNo;
                    }

                    foreach (OnlineList linelist in reccommendC2List)
                    {
                        linelist.Priority = Priority;
                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        addnum++;
                    }
                }
                #endregion
                if (addnum > 0)
                {
                    WriteLog("二级类" + C2SysNo + "特别推荐 成功添加" + addnum);
                }
            }
            //else if (positionid == (int)Category2.New)
            //{
            //    addnum = 0;
            //    #region newc1
            //    //取最新上架的产品    
            //    List<OnlineList> secnewC1List = new List<OnlineList>();
            //    List<OnlineList> newC1List = OnlinelistDA.GetNewItem(diffnum, C2SysNo, ItemSysnolist, GroupSysNo);
            //    int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, C2SysNo, "新品上架");

            //    if (newC1List != null && newC1List.Count > 0)
            //    {
            //        newC1List = FilterItem(newC1List, GroupSysNo, out hasGroupSysNo, secnewC1List);
            //        GroupSysNo = hasGroupSysNo;
            //        while (diffnum > newC1List.Count && loopNum <= 2)
            //        {
            //            loopNum++;
            //            ItemSysnolist = newC1List.Count > 0
            //            ? newC1List.Select(p => p.ProductSysNo).Distinct().ToList() : new List<Int32>();
            //            secnewC1List = OnlinelistDA.GetNewItem(diffnum - newC1List.Count, C2SysNo
            //                , ItemSysnolist, GroupSysNo);
            //            newC1List = FilterItem(secnewC1List, GroupSysNo, out hasGroupSysNo, newC1List);
            //            GroupSysNo = hasGroupSysNo;
            //        }
            //        WriteLog("二级类" + C2SysNo + "找出新上架可用商品" + newC1List.Count() + "， 要补位" + diffnum);

            //        foreach (OnlineList linelist in newC1List)
            //        {
            //            linelist.Priority = Priority;
            //            if (onlinelocationsysno > 0)
            //            {
            //                linelist.OnlineLocationSysNo = onlinelocationsysno;
            //            }
            //            OnlinelistDA.Insert(linelist);
            //            addnum++;
            //        }
            //    }
            //    #endregion
            //    if (addnum > 0)
            //    {
            //        WriteLog("二级类" + C2SysNo + "新品上架 成功添加" + addnum);
            //    }
            //}
        }
        #endregion

        #region C3
        public static void SetC3PageItem()
        {
            List<OnlineList> C3Pagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.C3page);
            List<OnlineList> C3SysNolist = OnlinelistDA.GetAllC3SysNolist();

            if (C3Pagelist != null && C3Pagelist.Count > 0)
            {
                var C3Itemlist = (from item in C3Pagelist
                                  where item.locationInfo.PositionID == (int)Category3.SmallSpecialOfferToday
                                  select item).ToList();

                if (C3Itemlist != null && C3Itemlist.Count >= 0)
                {
                    WriteLog("正在进行 三级分类--今日特惠，已有记录" + C3Itemlist.Count);

                    //取降价最大的商品     
                    if (C3SysNolist != null && C3SysNolist.Count > 0)
                    {
                        foreach (OnlineList oll in C3SysNolist)
                        {
                            foreach (OnlineList linelist in C3Itemlist)
                            {
                                ItemSysnolist.Add(linelist.ProductSysNo);
                                GroupSysNo.Add(linelist.ProductGroupSysNo);
                            }
                            var c3list = (from im in C3Itemlist
                                          where im.C3SysNo == oll.C3SysNo
                                          select im).ToList();

                            int diffnum = C3ItemMaxNum - c3list.Count;

                            if (diffnum > 0)
                            {
                                C3pageItemProcess(diffnum, oll.C3SysNo, (int)PageType.C3page, (int)Category3.SmallSpecialOfferToday);
                            }
                            DeleteOnlineListItem((int)PageType.C3page, (int)Category3.SmallSpecialOfferToday, oll.C3SysNo);
                            disposeProperty();
                        }
                    }
                }
            }
            else
            {
                WriteLog("三级类页面 没有有效记录，需全部新添加商品。");

                if (C3SysNolist != null && C3SysNolist.Count > 0)
                {
                    foreach (OnlineList oll in C3SysNolist)
                    {
                        C3pageItemProcess(C3ItemMaxNum, oll.C3SysNo, (int)PageType.C3page, (int)Category3.SmallSpecialOfferToday);
                        DeleteOnlineListItem((int)PageType.C3page, (int)Category3.SmallSpecialOfferToday, oll.C3SysNo);
                        disposeProperty();
                    }
                }
            }
        }
        public static void C3pageItemProcess(int diffnum, int C3SysNo, int pagetype, int positionid)
        {
            //为待删除数据打标记
            OnlinelistDA.UpdateInvalidData(pagetype, positionid, C3SysNo);
            List<int> groupSysNo = new List<int>();
            //5.三级分类：固定显示数量为2个
            //今日特惠最新数量为4个（所有商品不可重复）
            //三级分类  7天之内降价最大的商品，降价金额=7天之内最高金额-当前金额
            List<int> hasGroupSysNo = new List<int>();
            int loopNum = 0;
            int addnum = 0;
            #region promotion
            //（取降价最大的商品
            List<OnlineList> secpromotionC3List = new List<OnlineList>();
            List<OnlineList> promotionC3List = OnlinelistDA.GetPriceDownItem(diffnum, C3SysNo, ItemSysnolist, groupSysNo);
            int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, C3SysNo, "今日特惠");

            if (promotionC3List != null && promotionC3List.Count > 0)
            {
                promotionC3List = FilterItem(promotionC3List, groupSysNo, out hasGroupSysNo, secpromotionC3List);
                groupSysNo = hasGroupSysNo;
                while (diffnum > promotionC3List.Count && loopNum <= 2)
                {
                    loopNum++;
                    foreach (int item in promotionC3List.Select(p => p.ProductSysNo).Distinct().ToList())
                    {
                        ItemSysnolist.Add(item);
                    }
                    secpromotionC3List = OnlinelistDA.GetPriceDownItem(diffnum - promotionC3List.Count, C3SysNo
                            , ItemSysnolist, groupSysNo);
                    promotionC3List = FilterItem(secpromotionC3List, groupSysNo, out hasGroupSysNo, promotionC3List);
                    groupSysNo = hasGroupSysNo;
                }

                WriteLog("三级类" + C3SysNo + "找出可用商品" + promotionC3List.Count() + "， 要补位" + diffnum);
                foreach (OnlineList linelist in promotionC3List)
                {
                    linelist.Priority = Priority;
                    if (onlinelocationsysno > 0)
                    {
                        linelist.OnlineLocationSysNo = onlinelocationsysno;
                    }
                    OnlinelistDA.Insert(linelist);
                    addnum++;
                }
            }
            #endregion
            if (addnum > 0)
            {
                WriteLog("三级类" + C3SysNo + "今日特惠 成功添加" + addnum);
            }
        }
        #endregion

        #region Brand
        public static void SetBrandItem()
        {
            List<OnlineList> BrandPagelist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.Brand);

            List<int> Itemlist = new List<int>(); //已有专卖店商品
            List<int> HasAddItemlist = new List<int>(); //已有和新加的专卖店商品         

            List<int> SysNolist = new List<int>();
            List<int> HasSysNolist = new List<int>();

            List<int> groupSysNo = new List<int>();
            List<int> hasGroupSysNo = new List<int>();

            List<OnlineList> brandsysnolist = OnlinelistDA.GetAllBrand();

            if (BrandPagelist != null && BrandPagelist.Count > 0)
            {
                if (brandsysnolist != null && brandsysnolist.Count > 0)
                {
                    List<int> allBrandItem = (from item in BrandPagelist
                                              select item.ProductSysNo).Distinct().ToList();

                    #region 品牌专卖 新品上架

                    var NewItemlist = (from item in BrandPagelist
                                       where item.locationInfo.PositionID == (int)Brand.New
                                       select item).ToList();

                    if (NewItemlist != null && NewItemlist.Count >= 0)
                    {
                        WriteLog("正在进行 品牌专卖--新品上架，已有记录" + NewItemlist.Count);
                        foreach (OnlineList linelist in NewItemlist)
                        {
                            SysNolist.Add(linelist.SysNo);
                            Itemlist.Add(linelist.ProductSysNo);
                            groupSysNo.Add(linelist.ProductGroupSysNo);
                        }

                        foreach (OnlineList brand in brandsysnolist)
                        {
                            if (TestBrandSysNo != 0 && brand.BrandSysNo != TestBrandSysNo)
                            {
                                continue;
                            }
                            var itemlist = (from item in NewItemlist
                                            where item.BrandSysNo == brand.BrandSysNo
                                            select item).ToList();
                            int diffnum = BrandItemMaxNum - itemlist.Count;
                            WriteLog("专卖店" + brand.BrandSysNo + "已有" + itemlist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                BrandpageItemProcess(diffnum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.New
                                    , Itemlist, out HasAddItemlist, SysNolist, out HasSysNolist, groupSysNo, out hasGroupSysNo);
                                Itemlist = HasAddItemlist;
                                SysNolist = HasSysNolist;
                                groupSysNo = hasGroupSysNo;

                            }
                            DeleteOnlineListItem((int)PageType.Brand, (int)Brand.New, brand.BrandSysNo);
                        }
                    }
                    #endregion

                    #region 品牌专卖  让利促销

                    var PromotionItemlist = (from item in BrandPagelist
                                             where item.locationInfo.PositionID == (int)Brand.Promotion
                                             select item).ToList();

                    if (PromotionItemlist != null && PromotionItemlist.Count >= 0)
                    {
                        WriteLog("正在进行 品牌专卖--让利促销，已有记录" + PromotionItemlist.Count);
                        foreach (OnlineList linelist in PromotionItemlist)
                        {
                            SysNolist.Add(linelist.SysNo);
                            Itemlist.Add(linelist.ProductSysNo);
                            groupSysNo.Add(linelist.ProductGroupSysNo);
                        }
                        foreach (OnlineList brand in brandsysnolist)
                        {
                            if (TestBrandSysNo != 0 && brand.BrandSysNo != TestBrandSysNo)
                            {
                                continue;
                            }
                            var itemlist = (from item in PromotionItemlist
                                            where item.BrandSysNo == brand.BrandSysNo
                                            select item).ToList();
                            int diffnum = BrandItemMaxNum - itemlist.Count;
                            WriteLog("专卖店" + brand.BrandSysNo + "已有" + itemlist.Count + "个商品，需新添加" + diffnum + "个商品。");
                            if (diffnum > 0)
                            {

                                BrandpageItemProcess(diffnum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Promotion
                                    , Itemlist, out HasAddItemlist, SysNolist, out HasSysNolist, groupSysNo, out hasGroupSysNo);

                                Itemlist = HasAddItemlist;
                                SysNolist = HasSysNolist;
                                groupSysNo = hasGroupSysNo;
                            }
                            DeleteOnlineListItem((int)PageType.Brand, (int)Brand.Promotion, brand.BrandSysNo);
                        }

                    }
                    #endregion

                    #region 品牌专卖 Hot
                    var HotBrandItemlist = (from item in BrandPagelist
                                            where item.locationInfo.PositionID == (int)Brand.Hot
                                            select item).ToList();
                    if (HotBrandItemlist != null && HotBrandItemlist.Count >= 0)
                    {
                        WriteLog("正在进行 品牌专卖--当季热销，已有记录" + HotBrandItemlist.Count);
                        foreach (OnlineList linelist in HotBrandItemlist)
                        {
                            SysNolist.Add(linelist.SysNo);
                            Itemlist.Add(linelist.ProductSysNo);
                            groupSysNo.Add(linelist.ProductGroupSysNo);
                        }
                        SysNolist.Distinct();
                        foreach (OnlineList brand in brandsysnolist)
                        {
                            if (TestBrandSysNo != 0 && brand.BrandSysNo != TestBrandSysNo)
                            {
                                continue;
                            }

                            var itemlist = (from item in HotBrandItemlist
                                            where item.BrandSysNo == brand.BrandSysNo
                                            select item).ToList();
                            int diffnum = BrandItemMaxNum - itemlist.Count;

                            WriteLog("专卖店" + brand.BrandSysNo + "已有" + itemlist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                BrandpageItemProcess(diffnum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Hot
                                               , Itemlist, out HasAddItemlist, SysNolist, out HasSysNolist, groupSysNo, out hasGroupSysNo);
                                Itemlist = HasAddItemlist;
                                SysNolist = HasSysNolist;
                                groupSysNo = hasGroupSysNo;
                            }
                            DeleteOnlineListItem((int)PageType.Brand, (int)Brand.Hot, brand.BrandSysNo);
                        }
                    }
                    #endregion
                }
            }
            else
            {
                WriteLog("专卖店没有有效记录，每个专卖店的每个位置需新加" + BrandItemMaxNum + " 条记录。");
                if (brandsysnolist != null && brandsysnolist.Count > 0)
                {
                    WriteLog("正在进行 品牌专卖--新品上架");
                    foreach (OnlineList brand in brandsysnolist)
                    {
                        if (TestBrandSysNo != 0 && brand.BrandSysNo != TestBrandSysNo)
                        {
                            continue;
                        }

                        BrandpageItemProcess(BrandItemMaxNum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.New
                            , Itemlist, out HasAddItemlist, SysNolist, out HasSysNolist, groupSysNo, out hasGroupSysNo);
                        Itemlist = HasAddItemlist;
                        SysNolist = HasSysNolist;
                        groupSysNo = hasGroupSysNo;
                        DeleteOnlineListItem((int)PageType.Brand, (int)Brand.New, brand.BrandSysNo);
                    }

                    WriteLog("正在进行 品牌专卖--让利促销");
                    foreach (OnlineList brand in brandsysnolist)
                    {
                        if (TestBrandSysNo != 0 && brand.BrandSysNo != TestBrandSysNo)
                        {
                            continue;
                        }

                        BrandpageItemProcess(BrandItemMaxNum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Promotion
                            , Itemlist, out HasAddItemlist, SysNolist, out HasSysNolist, groupSysNo, out hasGroupSysNo);
                        Itemlist = HasAddItemlist;
                        SysNolist = HasSysNolist;
                        groupSysNo = hasGroupSysNo;
                        DeleteOnlineListItem((int)PageType.Brand, (int)Brand.Promotion, brand.BrandSysNo);
                    }

                    WriteLog("正在进行 品牌专卖--当季热销");
                    foreach (OnlineList brand in brandsysnolist)
                    {
                        if (TestBrandSysNo != 0 && brand.BrandSysNo != TestBrandSysNo)
                        {
                            continue;
                        }

                        BrandpageItemProcess(BrandItemMaxNum, brand.BrandSysNo, (int)PageType.Brand, (int)Brand.Hot
                            , Itemlist, out HasAddItemlist, SysNolist, out HasSysNolist, groupSysNo, out hasGroupSysNo);
                        Itemlist = HasAddItemlist;
                        SysNolist = HasSysNolist;
                        groupSysNo = hasGroupSysNo;
                        DeleteOnlineListItem((int)PageType.Brand, (int)Brand.Hot, brand.BrandSysNo);
                    }
                }
            }
        }

        public static void BrandpageItemProcess(int diffnum, int BrandSysNo, int pagetype, int positionid
         , List<int> ItemSysnolist, out List<int> HasAddItemSysnolist
         , List<int> Sysnolist, out List<int> HasSysnolist
         , List<int> groupSysNo, out List<int> hasGroupSysNo)
        {
            //为待删除数据打标记
            OnlinelistDA.UpdateInvalidData(pagetype, positionid, BrandSysNo);

            //每一个专卖店都要添加,3个位置的商品可以重复
            List<int> itmlist = new List<int>();
            itmlist = ItemSysnolist;
            HasAddItemSysnolist = new List<int>();
            HasSysnolist = new List<int>();
            hasGroupSysNo = new List<int>();
            int loopNum = 0;
            int addnum = 0;
            if (positionid == (int)Brand.Hot)
            {
                #region hotbrand
                //品牌旗舰店/品牌专卖店中当季热销 如果少于6条记录则抽取销售数量最多的商品（取销售数量最多为
                List<OnlineList> hotBrandList = OnlinelistDA.GetSaleHightItemForBrand(diffnum, ItemSysnolist, BrandSysNo, groupSysNo);
                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, BrandSysNo, "当季热销");
                List<OnlineList> sechotBrandList = new List<OnlineList>();
                if (hotBrandList != null && hotBrandList.Count > 0)
                {
                    hotBrandList = FilterItem(hotBrandList, groupSysNo, out hasGroupSysNo, sechotBrandList);
                    groupSysNo = hasGroupSysNo;
                    while (diffnum > groupSysNo.Count && loopNum <= 2)
                    {
                        loopNum++;
                        sechotBrandList = OnlinelistDA.GetSaleHightItemForBrand(diffnum - hotBrandList.Count, ItemSysnolist
                                , BrandSysNo, groupSysNo);
                        hotBrandList = FilterItem(sechotBrandList, groupSysNo, out hasGroupSysNo, hotBrandList);
                        groupSysNo = hasGroupSysNo;
                    }

                    WriteLog("专卖店" + BrandSysNo + "  找出销售数量较多的商品" + hotBrandList.Count() + "， 要补位" + diffnum);

                    foreach (OnlineList linelist in hotBrandList)
                    {
                        linelist.Priority = Priority;
                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        ItemSysnolist.Add(linelist.ProductSysNo);
                        Sysnolist.Add(linelist.SysNo);
                        itmlist.Add(linelist.ProductSysNo);
                        groupSysNo.Add(linelist.ProductGroupSysNo);
                        addnum++;
                    }
                }
                if (addnum < diffnum)
                {
                    List<OnlineList> hotBrandList11 = OnlinelistDA.GetSaleHightItemForBrand(diffnum - addnum, itmlist, BrandSysNo, groupSysNo);
                    List<OnlineList> sechotBrandList11 = new List<OnlineList>();
                    if (hotBrandList11 != null && hotBrandList11.Count > 0)
                    {
                        hotBrandList11 = FilterItem(hotBrandList11, groupSysNo, out hasGroupSysNo, sechotBrandList11);
                        groupSysNo = hasGroupSysNo;
                        while (diffnum > hotBrandList11.Count && loopNum <= 2)
                        {
                            loopNum++;
                            sechotBrandList11 = OnlinelistDA.GetSaleHightItemForBrand(diffnum - hotBrandList11.Count, ItemSysnolist
                                , BrandSysNo, groupSysNo);
                            hotBrandList11 = FilterItem(sechotBrandList11, groupSysNo, out hasGroupSysNo, hotBrandList11);
                            groupSysNo = hasGroupSysNo;
                        }
                        foreach (OnlineList linelist in hotBrandList11)
                        {
                            linelist.Priority = Priority + 1;
                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            ItemSysnolist.Add(linelist.ProductSysNo);
                            Sysnolist.Add(linelist.SysNo);
                            groupSysNo.Add(linelist.ProductGroupSysNo);
                            addnum++;
                        }
                    }
                }
                if (addnum > 0)
                {
                    WriteLog("专卖店" + BrandSysNo + "当季热销 成功添加" + addnum);
                }
                #endregion
            }
            else if (positionid == (int)Brand.New)
            {
                addnum = 0;
                #region newbrand
                int n = OnlinelistDA.CheckBrandItemNum(BrandSysNo);
                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, BrandSysNo, "新品上架");

                if (n > 3)
                {
                    List<OnlineList> secnewBrandList = new List<OnlineList>();
                    List<OnlineList> newBrandList = OnlinelistDA.GetNewItemForBrand(diffnum, ItemSysnolist, BrandSysNo, groupSysNo);
                    if (newBrandList != null && newBrandList.Count > 0)
                    {
                        newBrandList = FilterItem(newBrandList, groupSysNo, out hasGroupSysNo, secnewBrandList);
                        groupSysNo = hasGroupSysNo;
                        while (diffnum > newBrandList.Count && loopNum <= 2)
                        {
                            loopNum++;
                            secnewBrandList = OnlinelistDA.GetNewItemForBrand(diffnum - newBrandList.Count, ItemSysnolist
                                    , BrandSysNo, groupSysNo);

                            newBrandList = FilterItem(secnewBrandList, groupSysNo, out hasGroupSysNo, newBrandList);
                            groupSysNo = hasGroupSysNo;
                        }
                        WriteLog("专卖店" + BrandSysNo + "  找出新上架的商品" + newBrandList.Count() + "， 要补位" + diffnum);

                        foreach (OnlineList linelist in newBrandList)
                        {
                            linelist.Priority = Priority;
                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            Sysnolist.Add(linelist.SysNo);
                            ItemSysnolist.Add(linelist.ProductSysNo);
                            itmlist.Add(linelist.ProductSysNo);
                            groupSysNo.Add(linelist.ProductGroupSysNo);
                            addnum++;
                        }
                    }
                    if (addnum < diffnum)
                    {
                        List<OnlineList> newBrandList11 = OnlinelistDA.GetNewItemForBrand(diffnum - addnum, itmlist, BrandSysNo, groupSysNo);
                        List<OnlineList> secnewBrandList11 = new List<OnlineList>();
                        if (newBrandList11 != null && newBrandList11.Count > 0)
                        {
                            newBrandList11 = FilterItem(newBrandList11, groupSysNo, out hasGroupSysNo, secnewBrandList11);
                            groupSysNo = hasGroupSysNo;
                            while (diffnum > newBrandList11.Count && loopNum <= 2)
                            {
                                loopNum++;
                                secnewBrandList11 = OnlinelistDA.GetNewItemForBrand(diffnum - newBrandList11.Count, ItemSysnolist
                                    , BrandSysNo, groupSysNo);
                                newBrandList11 = FilterItem(secnewBrandList11, groupSysNo, out hasGroupSysNo, newBrandList11);
                                groupSysNo = hasGroupSysNo;
                            }
                            foreach (OnlineList linelist in newBrandList11)
                            {
                                linelist.Priority = Priority + 1;
                                if (onlinelocationsysno > 0)
                                {
                                    linelist.OnlineLocationSysNo = onlinelocationsysno;
                                }
                                OnlinelistDA.Insert(linelist);
                                ItemSysnolist.Add(linelist.ProductSysNo);
                                Sysnolist.Add(linelist.SysNo);
                                groupSysNo.Add(linelist.ProductGroupSysNo);
                                addnum++;
                            }
                        }
                    }
                    if (addnum > 0)
                    {
                        WriteLog("专卖店" + BrandSysNo + "新品上架 成功添加" + addnum);
                    }
                }
                #endregion
            }
            else
            {
                addnum = 0;
                #region promotionBrand
                //品牌旗舰店/品牌专卖店中让利促销 如果少于6条记录则抽取降价最大的商品
                int n = OnlinelistDA.CheckBrandItemNum(BrandSysNo);
                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pagetype, positionid, BrandSysNo, "让利促销");
                if (n > 6)
                {
                    List<OnlineList> promotionBrandList = OnlinelistDA.GetPriceDownItemForBrand(diffnum, ItemSysnolist, BrandSysNo, groupSysNo);
                    List<OnlineList> secpromotionBrandList = new List<OnlineList>();
                    if (promotionBrandList != null && promotionBrandList.Count > 0)
                    {
                        promotionBrandList = FilterItem(promotionBrandList, groupSysNo, out hasGroupSysNo, secpromotionBrandList);
                        groupSysNo = hasGroupSysNo;
                        while (diffnum > promotionBrandList.Count && loopNum <= 2)
                        {
                            loopNum++;
                            secpromotionBrandList = OnlinelistDA.GetPriceDownItemForBrand(diffnum - promotionBrandList.Count, ItemSysnolist
                                    , BrandSysNo, groupSysNo);
                            promotionBrandList = FilterItem(secpromotionBrandList, groupSysNo, out hasGroupSysNo, promotionBrandList);
                            groupSysNo = hasGroupSysNo;
                        }

                        WriteLog("专卖店" + BrandSysNo + "  找出可用商品" + promotionBrandList.Count() + "， 要补位" + diffnum);

                        foreach (OnlineList linelist in promotionBrandList)
                        {
                            linelist.Priority = Priority;
                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            Sysnolist.Add(linelist.SysNo);
                            ItemSysnolist.Add(linelist.ProductSysNo);
                            itmlist.Add(linelist.ProductSysNo);
                            groupSysNo.Add(linelist.ProductGroupSysNo);
                            addnum++;
                        }
                    }
                    if (addnum < diffnum)
                    {
                        List<OnlineList> promotionBrandList11 = OnlinelistDA.GetPriceDownItemForBrand(diffnum - addnum, itmlist
                            , BrandSysNo, groupSysNo);
                        List<OnlineList> secpromotionBrandList11 = new List<OnlineList>();
                        if (promotionBrandList11 != null && promotionBrandList11.Count > 0)
                        {
                            promotionBrandList11 = FilterItem(promotionBrandList11, groupSysNo, out hasGroupSysNo, secpromotionBrandList11);
                            groupSysNo = hasGroupSysNo;
                            while (diffnum > promotionBrandList11.Count && loopNum <= 2)
                            {
                                loopNum++;
                                secpromotionBrandList11 = OnlinelistDA.GetPriceDownItemForBrand(diffnum - promotionBrandList11.Count
                                    , ItemSysnolist, BrandSysNo, groupSysNo);
                                promotionBrandList11 = FilterItem(secpromotionBrandList11, groupSysNo, out hasGroupSysNo, promotionBrandList11);
                                groupSysNo = hasGroupSysNo;
                            }
                            foreach (OnlineList linelist in promotionBrandList11)
                            {
                                linelist.Priority = Priority + 1;
                                if (onlinelocationsysno > 0)
                                {
                                    linelist.OnlineLocationSysNo = onlinelocationsysno;
                                }
                                OnlinelistDA.Insert(linelist);
                                ItemSysnolist.Add(linelist.ProductSysNo);
                                Sysnolist.Add(linelist.SysNo);
                                groupSysNo.Add(linelist.ProductGroupSysNo);
                                addnum++;
                            }
                        }
                    }
                    if (addnum > 0)
                    {
                        WriteLog("专卖店" + BrandSysNo + "让利促销 成功添加" + addnum);
                    }
                }
                #endregion
            }
            HasAddItemSysnolist = ItemSysnolist;
            HasSysnolist = Sysnolist;
            hasGroupSysNo = groupSysNo;
        }
        #endregion

        #region HundredShop
        /// <summary>
        /// //增加“百元品推荐”（PageType=8，Position=31）、-----推荐专属
        ///百元店“最新降价”（PageType=9，Position=46）和
        ///百元店“百元新品1-4”（PageType=9，Position=47/48/49/50）的自动补位
        ///1）	所有商品金额（CurrentPrice）小于等于110
        ///2）	日用百货大类补6个，其他大类每个大类各1个
        /// </summary>
        private static void SetHundredShopItem()
        {
            List<int> itemList = new List<int>();  //已有productsysno
            List<int> hasAddItemlist = new List<int>();   //已有+新productsysno

            List<int> sysNolist = new List<int>(); //已有记录的sysno
            List<int> hasAddSysNolist = new List<int>();//已有+新sysno

            List<int> groupSysNo = new List<int>();
            List<int> hasGroupSysNo = new List<int>();

            List<OnlineList> C1SysNolist = OnlinelistDA.GetAllC1SysNolist();

            //为待删除数据打标记           
            OnlinelistDA.UpdateInvalidData((int)PageType.HundredShopRecommend, (int)RecommendedHundredExclusiveShop.RecommendedExclusive, 0);
            OnlinelistDA.UpdateInvalidData((int)PageType.HundredShop, (int)HundredShop.LatestPrice, 0);
            OnlinelistDA.UpdateInvalidData((int)PageType.HundredShop, (int)HundredShop.HundredNew1, 0);
            OnlinelistDA.UpdateInvalidData((int)PageType.HundredShop, (int)HundredShop.HundredNew2, 0);
            OnlinelistDA.UpdateInvalidData((int)PageType.HundredShop, (int)HundredShop.HundredNew3, 0);
            OnlinelistDA.UpdateInvalidData((int)PageType.HundredShop, (int)HundredShop.HundredNew4, 0);

            //百元品推荐 推荐专属
            List<OnlineList> HundredShopRecommendlist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.HundredShopRecommend);
            List<OnlineList> HundredShoplist = OnlinelistDA.CheckOnlinelistItemByPageType((int)PageType.HundredShop);
            foreach (OnlineList list in HundredShopRecommendlist)
            {
                HundredShoplist.Add(list);
                itemList.Add(list.ProductSysNo);
                groupSysNo.Add(list.ProductGroupSysNo);
            }
            foreach (OnlineList linelist in HundredShoplist)
            {
                sysNolist.Add(linelist.SysNo);
                itemList.Add(linelist.ProductSysNo);
                groupSysNo.Add(linelist.ProductGroupSysNo);
            }

            #region 百元品推荐 推荐专属
            var RecommItemlist = (from item in HundredShoplist
                                  where item.locationInfo.PositionID == (int)RecommendedHundredExclusiveShop.RecommendedExclusive
                                  select item).ToList();

            if (RecommItemlist != null && RecommItemlist.Count >= 0)
            {
                #region 百元品推荐--推荐专属
                WriteLog("\r\n正在进行 百元品推荐--推荐专属，现在共有记录" + RecommItemlist.Count);
                foreach (OnlineList oll in C1SysNolist)
                {
                    var itemlist = (from item in RecommItemlist
                                    where item.C1SysNo == oll.C1SysNo
                                    select item).ToList();
                    int diffnum = 0;
                    if (oll.C1Name == AppliancesStores)
                    {
                        diffnum = HundredStoresNum - itemlist.Count;
                    }
                    else
                    {
                        diffnum = 1 - itemlist.Count;
                    }
                    //  WriteLog("大类" + oll.C1Name + " 已有" + itemlist.Count + "个商品，需新添加" + diffnum + "个商品。");
                    if (diffnum > 0)
                    {
                        HundredShopItemProcess(diffnum, oll.C1SysNo, (int)PageType.HundredShopRecommend
                            , (int)RecommendedHundredExclusiveShop.RecommendedExclusive
                            , itemList, out hasAddItemlist, sysNolist, out hasAddSysNolist, groupSysNo, out hasGroupSysNo);
                        itemList = hasAddItemlist;
                        sysNolist = hasAddSysNolist;
                        groupSysNo = hasGroupSysNo;
                    }
                }
                DeleteOnlineListItem((int)PageType.HundredShopRecommend
                            , (int)RecommendedHundredExclusiveShop.RecommendedExclusive, 0);
                #endregion
            }
            #endregion

            //百元店  
            if (HundredShoplist != null && HundredShoplist.Count >= 0)
            {
                if (C1SysNolist != null)
                {
                    #region 百元店页面 最新降价
                    var LatestPriceItemlist = (from item in HundredShoplist
                                               where item.locationInfo.PositionID == (int)HundredShop.LatestPrice
                                               select item).ToList();

                    if (LatestPriceItemlist != null && LatestPriceItemlist.Count >= 0)
                    {
                        WriteLog("\r\n正在进行 百元店页面--最新降价，现在共有记录" + LatestPriceItemlist.Count);
                        foreach (OnlineList oll in C1SysNolist)
                        {
                            var itemlist = (from item in LatestPriceItemlist
                                            where item.C1SysNo == oll.C1SysNo
                                            select item).ToList();
                            int diffnum = 0;
                            if (oll.C1Name == AppliancesStores)
                            {
                                diffnum = HundredStoresNum - itemlist.Count;
                            }
                            else
                            {
                                diffnum = 1 - itemlist.Count;
                            }
                            // WriteLog("大类" + oll.C1Name + " 已有" + itemlist.Count + "个商品，需新添加" + diffnum + "个商品。");

                            if (diffnum > 0)
                            {
                                HundredShopItemProcess(diffnum, oll.C1SysNo, (int)PageType.HundredShop
                                    , (int)HundredShop.LatestPrice
                                    , itemList, out hasAddItemlist, sysNolist, out hasAddSysNolist, groupSysNo, out hasGroupSysNo);
                                itemList = hasAddItemlist;
                                sysNolist = hasAddSysNolist;
                                groupSysNo = hasGroupSysNo;
                            }
                        }
                        DeleteOnlineListItem((int)PageType.HundredShop, (int)HundredShop.LatestPrice, 0);
                    }
                    #endregion

                    #region 百元店页面 百元新品1
                    var new1Itemlist = (from item in HundredShoplist
                                        where item.locationInfo.PositionID == (int)HundredShop.HundredNew1
                                        select item).ToList();

                    if (new1Itemlist != null && new1Itemlist.Count >= 0)
                    {
                        WriteLog("\r\n正在进行 百元店页面--百元新品1，现在共有记录" + new1Itemlist.Count);
                        foreach (OnlineList oll in C1SysNolist)
                        {
                            var Item1list = (from im in new1Itemlist
                                             where im.C1SysNo == oll.C1SysNo
                                             select im).ToList();
                            int diffnum = 0;
                            if (oll.C1Name == AppliancesStores)
                            {
                                diffnum = HundredStoresNum - Item1list.Count;
                            }
                            else
                            {
                                diffnum = 1 - Item1list.Count;
                            }
                            //   WriteLog("大类" + oll.C1Name + "已有" + Item1list.Count + "个商品，需添加" + diffnum + "个商品。");
                            if (diffnum > 0)
                            {
                                HundredShopItemProcess(diffnum, oll.C1SysNo, (int)PageType.HundredShop
                                    , (int)HundredShop.HundredNew1
                                    , itemList, out hasAddItemlist, sysNolist, out hasAddSysNolist, groupSysNo, out hasGroupSysNo);
                                itemList = hasAddItemlist;
                                sysNolist = hasAddSysNolist;
                                groupSysNo = hasGroupSysNo;
                            }
                        }
                        DeleteOnlineListItem((int)PageType.HundredShop, (int)HundredShop.HundredNew1, 0);
                    }
                    #endregion

                    #region 百元店页面 百元新品2
                    var new2Itemlist = (from item in HundredShoplist
                                        where item.locationInfo.PositionID == (int)HundredShop.HundredNew2
                                        select item).ToList();

                    if (new2Itemlist != null && new2Itemlist.Count >= 0)
                    {
                        WriteLog("\r\n正在进行 百元店页面--百元新品2，现在共有记录" + new2Itemlist.Count);

                        foreach (OnlineList oll in C1SysNolist)
                        {
                            var Item2list = (from im in new2Itemlist
                                             where im.C1SysNo == oll.C1SysNo
                                             select im).ToList();
                            int diffnum = 0;
                            if (oll.C1Name == AppliancesStores)
                            {
                                diffnum = HundredStoresNum - Item2list.Count;
                            }
                            else
                            {
                                diffnum = 1 - Item2list.Count;
                            }
                            //  WriteLog("大类" + oll.C1Name + " 已有" + Item2list.Count + "个商品，需添加" + diffnum + "个商品。");
                            if (diffnum > 0)
                            {
                                HundredShopItemProcess(diffnum, oll.C1SysNo, (int)PageType.HundredShop
                                    , (int)HundredShop.HundredNew2
                                    , itemList, out hasAddItemlist, sysNolist, out hasAddSysNolist, groupSysNo, out  hasGroupSysNo);
                                itemList = hasAddItemlist;
                                sysNolist = hasAddSysNolist;
                                groupSysNo = hasGroupSysNo;
                            }
                        }
                        DeleteOnlineListItem((int)PageType.HundredShop, (int)HundredShop.HundredNew2, 0);

                    }
                    #endregion

                    #region 百元店页面 百元新品3
                    var new3Itemlist = (from item in HundredShoplist
                                        where item.locationInfo.PositionID == (int)HundredShop.HundredNew3
                                        select item).ToList();

                    if (new3Itemlist != null && new3Itemlist.Count >= 0)
                    {
                        WriteLog("\r\n正在进行 百元店页面--百元新品3，现在共有记录" + new3Itemlist.Count);

                        foreach (OnlineList oll in C1SysNolist)
                        {
                            var Item3list = (from im in new3Itemlist
                                             where im.C1SysNo == oll.C1SysNo
                                             select im).ToList();

                            int diffnum = 0;
                            if (oll.C1Name == AppliancesStores)
                            {
                                diffnum = HundredStoresNum - Item3list.Count;
                            }
                            else
                            {
                                diffnum = 1 - Item3list.Count;
                            }

                            //  WriteLog("大类" + oll.C1Name + " 已有" + Item3list.Count + "个商品，需添加" + diffnum + "个商品。");
                            if (diffnum > 0)
                            {
                                HundredShopItemProcess(diffnum, oll.C1SysNo, (int)PageType.HundredShop
                                    , (int)HundredShop.HundredNew3
                                    , itemList, out hasAddItemlist, sysNolist, out hasAddSysNolist, groupSysNo, out  hasGroupSysNo);
                                itemList = hasAddItemlist;
                                sysNolist = hasAddSysNolist;
                                groupSysNo = hasGroupSysNo;
                            }
                        }
                        DeleteOnlineListItem((int)PageType.HundredShop, (int)HundredShop.HundredNew3, 0);
                    }
                    #endregion

                    #region 百元店页面 百元新品4
                    var new4Itemlist = (from item in HundredShoplist
                                        where item.locationInfo.PositionID == (int)HundredShop.HundredNew4
                                          && (item.OnlineQty > 0 || item.ShiftQty > 0)
                                        select item).ToList();

                    if (new4Itemlist != null && new4Itemlist.Count >= 0)
                    {
                        WriteLog("\r\n正在进行 百元店页面--百元新品4，现在共有记录" + new4Itemlist.Count);

                        foreach (OnlineList oll in C1SysNolist)
                        {
                            var Item4list = (from im in new4Itemlist
                                             where im.C1SysNo == oll.C1SysNo
                                             select im).ToList();

                            int diffnum = 0;
                            if (oll.C1Name == AppliancesStores)
                            {
                                diffnum = HundredStoresNum - Item4list.Count;
                            }
                            else
                            {
                                diffnum = 1 - Item4list.Count;
                            }
                            //  WriteLog("大类" + oll.C1Name + " 已有" + Item4list.Count + "个商品，需添加" + diffnum + "个商品。");
                            if (diffnum > 0)
                            {
                                HundredShopItemProcess(diffnum, oll.C1SysNo, (int)PageType.HundredShop
                                    , (int)HundredShop.HundredNew4
                                    , itemList, out hasAddItemlist, sysNolist, out hasAddSysNolist, groupSysNo, out  hasGroupSysNo);
                                itemList = hasAddItemlist;
                                sysNolist = hasAddSysNolist;
                                groupSysNo = hasGroupSysNo;
                            }
                        }
                        DeleteOnlineListItem((int)PageType.HundredShop, (int)HundredShop.HundredNew4, 0);
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diffnum">添加商品个数</param>
        /// <param name="C1SysNo">大类sysno</param>
        /// <param name="pageType">页面类型</param>
        /// <param name="positionID">位置</param>
        /// <param name="HasItemlist">该位置已有的商品（ProductSysNo）</param>
        /// <param name="hasAddItemlist">该位置已有和新添加的商品（ProductSysNo）</param>
        /// <param name="HasSysNolist">该位置已有记录的编号（SysNo）</param>
        /// <param name="hasAddSysNolist">该位置已有和新添记录的编号（SysNo）</param>
        private static void HundredShopItemProcess(int diffnum, int C1SysNo, int pageType, int positionID
            , List<int> itemlist, out List<int> hasAddItemlist
            , List<int> sysNolist, out List<int> hasAddSysNolist
            , List<int> groupSysNo, out List<int> hasGroupSysNo)
        {

            //新品上架和热销商品的商品信息不可重复
            hasAddItemlist = new List<int>();
            hasAddSysNolist = new List<int>();
            hasGroupSysNo = new List<int>();
            int addnum = 0;
            int loopNum = 0;

            if (pageType == (int)PageType.HundredShopRecommend
                && positionID == (int)RecommendedHundredExclusiveShop.RecommendedExclusive)
            {
                #region 百元品推荐 推荐专属
                List<OnlineList> RecommendItemList = OnlinelistDA.GetSaleHightItemForHundredShop(diffnum, itemlist, C1SysNo, groupSysNo);
                List<OnlineList> secRecommendItemList = new List<OnlineList>();
                int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pageType, positionID, 0, "推荐专属");

                if (RecommendItemList != null && RecommendItemList.Count > 0)
                {
                    RecommendItemList = FilterItem(RecommendItemList, groupSysNo, out hasGroupSysNo, secRecommendItemList);
                    groupSysNo = hasGroupSysNo;
                    while (diffnum > RecommendItemList.Count && loopNum <= 2)
                    {
                        loopNum++;
                        secRecommendItemList = OnlinelistDA.GetSaleHightItemForHundredShop(diffnum - RecommendItemList.Count
                            , itemlist, C1SysNo, groupSysNo);
                        RecommendItemList = FilterItem(secRecommendItemList, groupSysNo, out hasGroupSysNo, RecommendItemList);
                        groupSysNo = hasGroupSysNo;
                    }
                    foreach (OnlineList linelist in RecommendItemList)
                    {
                        linelist.Priority = Priority;
                        if (onlinelocationsysno > 0)
                        {
                            linelist.OnlineLocationSysNo = onlinelocationsysno;
                        }
                        OnlinelistDA.Insert(linelist);
                        sysNolist.Add(linelist.SysNo);
                        itemlist.Add(linelist.ProductSysNo);
                        groupSysNo.Add(linelist.ProductGroupSysNo);
                        addnum++;
                    }
                }
                if (addnum > 0)
                {
                    WriteLog("一级类" + C1SysNo + "成功添加" + addnum + "个商品");
                }
                #endregion
            }
            else if (pageType == (int)PageType.HundredShop)
            {
                #region 百元店
                if (positionID == (int)HundredShop.LatestPrice)
                {
                    #region 最新降价
                    List<OnlineList> PriceDownItemList = OnlinelistDA.GetPriceDownItemForHundred(diffnum, itemlist, C1SysNo, groupSysNo);
                    int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pageType, positionID, 0, "最新降价");
                    List<OnlineList> secPriceDownItemList = new List<OnlineList>();

                    if (PriceDownItemList != null && PriceDownItemList.Count > 0)
                    {
                        PriceDownItemList = FilterItem(PriceDownItemList, groupSysNo, out hasGroupSysNo, secPriceDownItemList);
                        groupSysNo = hasGroupSysNo;
                        while (diffnum > PriceDownItemList.Count && loopNum <= 2)
                        {
                            loopNum++;
                            secPriceDownItemList = OnlinelistDA.GetPriceDownItemForHundred(diffnum - PriceDownItemList.Count
                                , itemlist, C1SysNo, groupSysNo);
                            PriceDownItemList = FilterItem(secPriceDownItemList, groupSysNo, out hasGroupSysNo, PriceDownItemList);
                            groupSysNo = hasGroupSysNo;
                        }

                        foreach (OnlineList linelist in PriceDownItemList)
                        {
                            linelist.Priority = Priority;
                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            sysNolist.Add(linelist.SysNo);
                            itemlist.Add(linelist.ProductSysNo);
                            groupSysNo.Add(linelist.ProductGroupSysNo);
                            addnum++;
                        }
                    }
                    if (addnum > 0)
                    {
                        WriteLog("一级类" + C1SysNo + "成功添加" + addnum + "个商品");
                    }
                    #endregion
                }
                else if (pageType == (int)PageType.HundredShop)
                {
                    #region 百元店 新品

                    List<OnlineList> OnlineItemList = new List<OnlineList>();
                    List<OnlineList> secOnlineItemList = new List<OnlineList>();

                    int onlinelocationsysno = OnlinelistDA.GetLocationSysno(pageType, positionID, 0, "百元新品");
                    addnum = 0;

                    OnlineItemList = OnlinelistDA.GetNewItemForHundred(diffnum, itemlist, C1SysNo, HundredShopPrice, groupSysNo);
                    if (OnlineItemList != null && OnlineItemList.Count > 0)
                    {
                        OnlineItemList = FilterItem(OnlineItemList, groupSysNo, out hasGroupSysNo, secOnlineItemList);
                        groupSysNo = hasGroupSysNo;
                        while (diffnum > OnlineItemList.Count && loopNum <= 2)
                        {
                            loopNum++;
                            secOnlineItemList = OnlinelistDA.GetNewItemForHundred(diffnum - OnlineItemList.Count
                                , itemlist, C1SysNo, HundredShopPrice, groupSysNo);
                            OnlineItemList = FilterItem(secOnlineItemList, groupSysNo, out hasGroupSysNo, OnlineItemList);
                            groupSysNo = hasGroupSysNo;
                        }

                        foreach (OnlineList linelist in OnlineItemList)
                        {
                            linelist.Priority = Priority;
                            if (onlinelocationsysno > 0)
                            {
                                linelist.OnlineLocationSysNo = onlinelocationsysno;
                            }
                            OnlinelistDA.Insert(linelist);
                            sysNolist.Add(linelist.SysNo);
                            itemlist.Add(linelist.ProductSysNo);
                            groupSysNo.Add(linelist.ProductGroupSysNo);
                            addnum++;
                        }
                    }
                    if (addnum > 0)
                    {
                        WriteLog("一级类" + C1SysNo + "成功添加" + addnum + "个商品");
                    }
                    #endregion
                }
                #endregion
            }
            hasAddItemlist = itemlist;
            hasAddSysNolist = sysNolist;
            hasGroupSysNo = groupSysNo;
        }
        #endregion

        public static void DeleteOnlineListItem(int pagetype, int positionid, int pageID)
        {
            OnlinelistDA.DeleteOnlineListItem(pagetype, positionid, pageID);
        }

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

        public static void disposeProperty()
        {
            ItemSysnolist = null;
            GroupSysNo = null;
        }
        #endregion
    }
}
