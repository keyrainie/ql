using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using IPP.ECommerceMgmt.AutoInnerOnlineList.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ECommerceMgmt.AutoInnerOnlineList.DA
{
    public class OnlinelistDA
    {
        public static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public static string LanguageCode = ConfigurationManager.AppSettings["LanguageCode"];
        public static string GiftC3SysNo = ConfigurationManager.AppSettings["GiftC3SysNo"];

        public static List<OnlineList> CheckOnlinelistItemByPageType(int pageType)
        {
            List<OnlineList> onlineList = null;

            DataCommand command = DataCommandManager.GetDataCommand("CheckOnlinelistItemByPageType");
            command.SetParameterValue("@PageType", pageType);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }
        //for  home
        public static List<OnlineList> GetOnlinelistItemByPageType(int pageType)
        {
            List<OnlineList> onlineList = null;

            DataCommand command = DataCommandManager.GetDataCommand("GetOnlinelistItemByPageType");
            command.SetParameterValue("@PageType", pageType);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static int GetLocationSysno(int pagetype, int positionID, int pageID, string description)
        {
            int locationsysno = 0;
            DataCommand command = DataCommandManager.GetDataCommand("CheckOnlineListLocation");
            command.SetParameterValue("@PageType", pagetype);
            command.SetParameterValue("@PositionID", positionID);
            command.SetParameterValue("@PageID", pageID);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            try
            {
                locationsysno = command.ExecuteScalar<int>();
            }
            catch
            {
                locationsysno = 0;
            }
            if (locationsysno == 0)
            {
                DataCommand dc = DataCommandManager.GetDataCommand("InsertOnlineListlocation");
                DateTime ss = DateTime.Now;
                dc.SetParameterValue("@PageType", pagetype);
                dc.SetParameterValue("@PositionID", positionID);
                dc.SetParameterValue("@Priority", 0);
                dc.SetParameterValue("@PageID", pageID);
                dc.SetParameterValue("@Description", description);
                dc.SetParameterValue("@InDate", DateTime.Now);
                dc.SetParameterValue("@InUser", "System");
                dc.SetParameterValue("@EditDate", DateTime.Now);
                dc.SetParameterValue("@EditUser", "System");
                dc.SetParameterValue("@Status", "A");
                dc.SetParameterValue("@CompanyCode", CompanyCode);
                dc.SetParameterValue("@LanguageCode", LanguageCode);
                dc.SetParameterValue("@StoreCompanyCode", CompanyCode);

                dc.ExecuteNonQuery();
                locationsysno = (int)dc.GetParameterValue("@OnlineListlocationSysNo");
            }
            return locationsysno;
        }

        public static List<OnlineList> GetAllBrand()
        {
            List<OnlineList> result = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllBrand");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            result = command.ExecuteEntityList<OnlineList>();
            return result;
        }

        public static List<OnlineList> GetAllC1SysNolist()
        {
            List<OnlineList> onlineList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllC1SysNolist");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static List<OnlineList> GetC1BackSysNo(int C1SysNo)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetC1BackSysNo");
            command.SetParameterValue("@C1SysNo", C1SysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static List<OnlineList> GetC2BackSysNo(int C2SysNo)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetC2BackSysNo");
            command.SetParameterValue("@C2SysNo", C2SysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static List<OnlineList> GetAllC2SysNolist()
        {
            List<OnlineList> onlineList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllC2SysNolist");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static List<OnlineList> GetAllC3SysNolist()
        {
            List<OnlineList> onlineList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllC3SysNolist");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static List<OnlineList> GetSaleHightItem(int num, int categorySysNo
                                           , List<int> ItemSysnolist, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }
            if (num > 0)
            {
                if (ItemSysnolist != null && ItemSysnolist.Count > 0)
                {
                    if (groupSysNoList != null && groupSysNoList.Count > 0)
                    {
                        command = DataCommandManager.GetDataCommand("GetSaleHightItemByGroup");
                        command.SetParameterValue("@GroupSysNoList", grouplist);
                    }
                    else
                    {
                        command = DataCommandManager.GetDataCommand("GetSaleHightItem");
                    }
                    command.SetParameterValue("@ItemSysnolist", ItemSysnolist.ToListString());
                }
                else
                {
                    if (groupSysNoList != null && groupSysNoList.Count > 0)
                    {
                        command = DataCommandManager.GetDataCommand("GetSaleHightItemNoHistoryByGroup");
                        command.SetParameterValue("@GroupSysNoList", grouplist);
                    }
                    else
                    {
                        command = DataCommandManager.GetDataCommand("GetSaleHightItemNoHistory");
                    }
                }
                command.SetParameterValue("@Num", num);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@CategorySysNo", categorySysNo);
                command.SetParameterValue("@GiftC3SysNo", GiftC3SysNo);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        public static List<OnlineList> GetSaleHightItemForBrand(int num, List<int> HasItemSysnolist, int BrandSysNo, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }
            if (num > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    command = DataCommandManager.GetDataCommand("GetSaleHightItemForBrand");
                    command.SetParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                }
                else
                {
                    command = DataCommandManager.GetDataCommand("GetSaleHightItemForBrandNoHistory");
                }
                command.SetParameterValue("@Num", num);
                command.SetParameterValue("@BrandSysNo", BrandSysNo);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@GiftC3SysNo", GiftC3SysNo);
                command.SetParameterValue("@GroupSysNoList", grouplist);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        public static List<OnlineList> GetSaleHightItemForC1Page(int diffnum, List<int> HasItemSysnolist, int C1Sysno
            , int C2Sysno, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            if (diffnum > 0)
            {
                if (C2Sysno != -1)
                {
                    onlineList = GetSaleHightItem(diffnum, C2Sysno, HasItemSysnolist, groupSysNoList);
                }
                else
                {
                    onlineList = GetSaleHightItem(diffnum, C1Sysno, HasItemSysnolist, groupSysNoList);
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetSaleHightItemForHundredShop(int diffnum, List<int> HasItemSysnolist
            , int C1Sysno, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }
            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    command = DataCommandManager.GetDataCommand("GetSaleHightItemForHundredShop");
                    command.SetParameterValue("@ItemSysnolist", HasItemSysnolist.ToListString());
                }
                else
                {
                    command = DataCommandManager.GetDataCommand("GetSaleHightItemForHundredShopNoHistory");
                }
                command.SetParameterValue("@Num", diffnum);
                command.SetParameterValue("@CategorySysNo", C1Sysno);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@GiftC3SysNo", GiftC3SysNo);
                command.SetParameterValue("@GroupSysNoList", grouplist);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        public static List<OnlineList> GetNewItemForBrand(int diffnum, List<int> HasItemSysnolist, int BrandSysNo, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }
            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    command = DataCommandManager.GetDataCommand("GetNewItemForBrand");
                    command.SetParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());

                }
                if (onlineList == null || onlineList.Count == 0)
                {
                    command = DataCommandManager.GetDataCommand("GetNewItemForBrandNoHistory");
                }
                command.SetParameterValue("@Num", diffnum);
                command.SetParameterValue("@BrandSysNo", BrandSysNo);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@GiftC3SysNo", GiftC3SysNo);
                command.SetParameterValue("@GroupSysNoList", grouplist);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        public static List<OnlineList> GetNewItem(int diffnum, int categorySysNo, List<int> HasItemSysnolist, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    command = DataCommandManager.GetDataCommand("GetNewItem");
                    command.SetParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                }
                else
                {
                    command = DataCommandManager.GetDataCommand("GetNewItemNoHistory");
                }
                command.SetParameterValue("@Num", diffnum);
                command.SetParameterValue("@CategorySysNo", categorySysNo);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@GiftC3SysNo", GiftC3SysNo);
                command.SetParameterValue("@GroupSysNoList", grouplist);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        public static List<OnlineList> GetSpecialItem(int diffnum, int categorySysNo, List<int> HasItemSysnolist, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    command = DataCommandManager.GetDataCommand("GetSpecialItem");
                    command.SetParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                }
                else
                {
                    command = DataCommandManager.GetDataCommand("GetSpecialItemNoHistory");
                }
                command.SetParameterValue("@Num", diffnum);
                command.SetParameterValue("@CategorySysNo", categorySysNo);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@GiftC3SysNo", GiftC3SysNo);
                command.SetParameterValue("@GroupSysNoList", grouplist);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        public static List<OnlineList> GetPriceDownItem(int num, int categorySysNo, List<int> HasItemSysnolist, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            int loopNum = 0;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }

            if (num > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    command = DataCommandManager.GetDataCommand("GetPriceDownItem");
                    command.SetParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                }
                else
                {
                    command = DataCommandManager.GetDataCommand("GetPriceDownItemNoHistory");
                }                
                command.SetParameterValue("@Num", num);
                command.SetParameterValue("@DateTime", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss"));
                command.SetParameterValue("@CategorySysNo", categorySysNo);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                command.SetParameterValue("@GroupSysNoList", grouplist);
                onlineList = command.ExecuteEntityList<OnlineList>();
                DateTime day;
                while (onlineList != null && onlineList.Count < num && loopNum <= 6)//取一年之内的数据
                {
                    loopNum++;
                    if (loopNum == 1)
                    {
                        day = DateTime.Now.AddMonths(-loopNum);
                    }
                    else
                    {
                        day = DateTime.Now.AddMonths(-loopNum * 2 + 1);
                    }
                    command.SetParameterValue("@Num", num);
                    command.SetParameterValue("@DateTime", day.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.SetParameterValue("@CategorySysNo", categorySysNo);
                    command.SetParameterValue("@CompanyCode", CompanyCode);
                    command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                    command.SetParameterValue("@GroupSysNoList", grouplist);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetPriceDownItemForBrand(int diffnum, List<int> HasItemSysnolist, int BrandSysNo
            , List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }          

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    command = DataCommandManager.GetDataCommand("GetPriceDownItemForBrand");
                    command.SetParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                }
                if (onlineList == null || onlineList.Count == 0)
                {
                    command = DataCommandManager.GetDataCommand("GetPriceDownItemForBrandNoHistory");
                }
                command.SetParameterValue("@Num", diffnum);
                command.SetParameterValue("@BrandSysNo", BrandSysNo);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@DateTime", DateTime.Now.AddDays(-7));
                command.SetParameterValue("@GiftC3SysNo", GiftC3SysNo);
                command.SetParameterValue("@GroupSysNoList", grouplist);
                onlineList = command.ExecuteEntityList<OnlineList>();
                DateTime day;
                int loopNum = 0;
                while (onlineList != null && onlineList.Count < diffnum && loopNum <= 6)//取一年之内的数据
                {
                    loopNum++;
                    if (loopNum == 1)
                    {
                        day = DateTime.Now.AddMonths(-loopNum);
                    }
                    else
                    {
                        day = DateTime.Now.AddMonths(-loopNum * 2 + 1);
                    }
                    command.SetParameterValue("@Num", diffnum);
                    command.SetParameterValue("@DateTime", day);
                    command.SetParameterValue("@BrandSysNo", BrandSysNo);
                    command.SetParameterValue("@CompanyCode", CompanyCode);
                    command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                    command.SetParameterValue("@GroupSysNoList", grouplist);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static OnlineList Insert(OnlineList entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertOnlineList");

            command.SetParameterValue("@OnlineListlocationSysNo", entity.OnlineLocationSysNo);
            command.SetParameterValue("@ProductID", entity.ProductID);
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@BeginDate", DateTime.Now);
            command.SetParameterValue("@EndDate", DateTime.Now.AddDays(1));
            command.SetParameterValue("@InDate", DateTime.Now);
            command.SetParameterValue("@InUser", "System");
            command.SetParameterValue("@EditDate", DateTime.Now);
            command.SetParameterValue("@EditUser", "System");
            command.SetParameterValue("@Status", "A");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", LanguageCode);
            command.SetParameterValue("@StoreCompanyCode", CompanyCode);

            command.ExecuteNonQuery();
            entity.SysNo = (int)command.GetParameterValue("@SysNo");
            return entity;
        }

        public static void SendMailAboutExceptionInfo(string ErrorMsg)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["MailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = DateTime.Now + " IPP-ECommerceMgmt-AutoInnerOnlineListJOB 运行时异常";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", ErrorMsg);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", LanguageCode);

            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 删除作废已标记数据
        /// </summary>
        /// <param name="pagetype"></param>
        /// <param name="positionid"></param>
        /// <param name="pageID"></param>
        public static void DeleteOnlineListItem(int pagetype, int positionid, int pageID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteOnlineListSystem");
            command.ReplaceParameterValue("@PageType", pagetype.ToString());
            command.ReplaceParameterValue("@PageID", pageID.ToString());
            command.ReplaceParameterValue("@PositionID", positionid.ToString());
            command.ReplaceParameterValue("@CompanyCode", CompanyCode);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 将该位置系统之前添加的数据标记为待删除
        /// </summary>
        /// <param name="pagetype"></param>
        /// <param name="positionid"></param>
        /// <param name="pageID"></param>
        public static void UpdateInvalidData(int pagetype, int positionid, int pageID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInvalidData");
            command.ReplaceParameterValue("@PageType", pagetype.ToString());
            command.ReplaceParameterValue("@PageID", pageID.ToString());
            command.ReplaceParameterValue("@PositionID", positionid.ToString());
            command.ReplaceParameterValue("@CompanyCode", CompanyCode);
            command.ExecuteNonQuery();
        }


        public static int CheckBrandItemNum(int brandsysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckBrandItemNum");
            command.SetParameterValue("@BrandSysNo", brandsysno);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            return command.ExecuteScalar<int>();
        }

        public static List<OnlineList> CheckC2ItemNumOnC1(int C1sysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckC2ItemNumOnC1");
            command.ReplaceParameterValue("@Category1Sysno", C1sysno.ToString());
            command.ReplaceParameterValue("@CompanyCode", CompanyCode);
            return command.ExecuteEntityList<OnlineList>();

        }

        public static void ClearTableOnLinelist(string day)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ClearTableOnLinelist");
            command.ReplaceParameterValue("@CompanyCode", CompanyCode);
            command.ReplaceParameterValue("@Day", day);
            command.ExecuteNonQuery();
        }

        public static List<OnlineList> GetPriceDownItemForHundred(int diffnum, List<int> HasItemSysnolist
            , int C1SysNo, List<int> groupSysNoList)
        {
            DataCommand command = null;
            List<OnlineList> onlineList = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }

            if (diffnum > 0)
            {
                string hasSysNoList = string.Empty;
                int loopNum = 0;
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    #region HasItemSysnolist

                    hasSysNoList = HasItemSysnolist.ToListString();
                    command = DataCommandManager.GetDataCommand("GetPriceDownItemForHundred");
                    DateTime day = DateTime.Now;
                    command.SetParameterValue("@Num", diffnum);
                    command.SetParameterValue("@DateTime", day.AddDays(-7));
                    command.SetParameterValue("@HasItemSysnolist", hasSysNoList);
                    command.SetParameterValue("@CategorySysNo", C1SysNo);
                    command.SetParameterValue("@CompanyCode", CompanyCode);
                    command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));

                    command.SetParameterValue("@GroupSysNoList", grouplist);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                    while (onlineList != null && onlineList.Count < diffnum && loopNum <= 6)//取一年之内的数据
                    {
                        loopNum++;
                        if (loopNum == 1)
                        {
                            day = DateTime.Now.AddMonths(-loopNum);
                        }
                        else
                        {
                            day = DateTime.Now.AddMonths(-loopNum * 2 + 1);
                        }
                        command.SetParameterValue("@Num", diffnum);
                        command.SetParameterValue("@DateTime", day);
                        command.SetParameterValue("@HasItemSysnolist", hasSysNoList);
                        command.SetParameterValue("@CategorySysNo", C1SysNo.ToString());
                        command.SetParameterValue("@CompanyCode", CompanyCode);
                        command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                        command.SetParameterValue("@GroupSysNoList", grouplist);
                        onlineList = command.ExecuteEntityList<OnlineList>();
                    }
                    #endregion
                }
                else
                {
                    #region 无历史记录HasItemSysnolist

                    command = DataCommandManager.GetDataCommand("GetPriceDownItemForHundredNoHistory");
                    DateTime day = DateTime.Now;
                    command.SetParameterValue("@Num", diffnum);
                    command.SetParameterValue("@DateTime", day.AddDays(-7));
                    command.SetParameterValue("@CategorySysNo", C1SysNo);
                    command.SetParameterValue("@CompanyCode", CompanyCode);
                    command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                    command.SetParameterValue("@GroupSysNoList", grouplist);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                    while (onlineList != null && onlineList.Count < diffnum && loopNum <= 6)
                    {
                        loopNum++;
                        if (loopNum == 1)
                        {
                            day = DateTime.Now.AddMonths(-loopNum);
                        }
                        else
                        {
                            day = DateTime.Now.AddMonths(-loopNum * 2 + 1);
                        }
                        command.SetParameterValue("@Num", diffnum);
                        command.SetParameterValue("@DateTime", day);
                        command.SetParameterValue("@CategorySysNo", C1SysNo);
                        command.SetParameterValue("@CompanyCode", CompanyCode);
                        command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                        command.SetParameterValue("@GroupSysNoList", grouplist);
                        onlineList = command.ExecuteEntityList<OnlineList>();
                    }
                    #endregion
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetNewItemForHundred(int diffnum, List<int> HasItemSysnolist, int C1SysNo
            , decimal itemPrice, List<int> groupSysNoList)
        {
            List<OnlineList> onlineList = null;
            DataCommand command = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    command = DataCommandManager.GetDataCommand("GetNewItemForC1PageForHundred");
                    command.SetParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                }
                else
                {
                    command = DataCommandManager.GetDataCommand("GetNewItemForC1PageForHundredNoHistory");
                }
                command.SetParameterValue("@Num", diffnum);
                command.SetParameterValue("@ItemPrice", itemPrice);
                command.SetParameterValue("@CategorySysNo", C1SysNo);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                command.SetParameterValue("@GiftC3SysNo", GiftC3SysNo);
                command.SetParameterValue("@GroupSysNoList", grouplist);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        public static List<Domain> GetNewItemDomainList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetNewItemDomainList");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            return command.ExecuteEntityList<Domain>();
        }

        public static List<OnlineList> GetSaleHightItemForHomePage(int diffnum, Domain domainItem
                                               , List<int> itemSysNolist, List<int> groupSysNoList)
        {
            DataCommand command = null;
            List<OnlineList> onlineList = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }

            if (diffnum > 0)
            {
                if (itemSysNolist != null && itemSysNolist.Count > 0)
                {
                    if (groupSysNoList != null && groupSysNoList.Count > 0)
                    {
                        command = DataCommandManager.GetDataCommand("GetSaleHightItemForHomePageByGroup");
                        command.SetParameterValue("@GroupSysNoList", grouplist);
                    }
                    else
                    {
                        command = DataCommandManager.GetDataCommand("GetSaleHightItemForHomePage");
                    }
                    command.SetParameterValue("@ItemSysnolist", itemSysNolist.ToListString());
                }
                else
                {
                    if (groupSysNoList != null && groupSysNoList.Count > 0)
                    {
                        command = DataCommandManager.GetDataCommand("GetSaleHightItemForHomePageNoHistoryByGroup");
                        command.SetParameterValue("@GroupSysNoList", grouplist);
                    }
                    else
                    {
                        command = DataCommandManager.GetDataCommand("GetSaleHightItemForHomePageNoHistory");
                    }
                }
          //      DateTime day = DateTime.Now;
                command.SetParameterValue("@Num", diffnum);
                command.SetParameterValue("@C1List", domainItem.C1List);
                command.SetParameterValue("@ExceptC3List", domainItem.ExceptC3List);
                command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                command.SetParameterValue("@CompanyCode", CompanyCode);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        //为首页取商品 除去已存在商品所在的组
        public static List<OnlineList> GetPriceDownItemForHomePage(int diffnum, Domain domainItem
                                                    , List<int> itemSysNolist, List<int> groupSysNoList)
        {
            DataCommand command = null;
            List<OnlineList> onlineList = null;
            string grouplist = string.Empty;
            if (groupSysNoList != null)
            {
                grouplist = groupSysNoList.ToListString();
            }

            if (diffnum > 0)
            {
                string hasSysNoList = string.Empty;
                int loopNum = 0;
                if (itemSysNolist != null && itemSysNolist.Count > 0)
                {
                    if (groupSysNoList != null && groupSysNoList.Count > 0)
                    {
                        command = DataCommandManager.GetDataCommand("GetPriceDownItemForHomePageByGroup");
                        command.SetParameterValue("@GroupSysNoList", grouplist);
                    }
                    else
                    {
                        command = DataCommandManager.GetDataCommand("GetPriceDownItemForHomePage");
                    }
                    command.SetParameterValue("@HasItemSysnolist", hasSysNoList);
                }
                else
                {
                    if (groupSysNoList != null && groupSysNoList.Count > 0)
                    {
                        command = DataCommandManager.GetDataCommand("GetPriceDownItemForHomePageNoHistoryByGroup");
                        command.SetParameterValue("@GroupSysNoList", grouplist);
                    }
                    else
                    {
                        command = DataCommandManager.GetDataCommand("GetPriceDownItemForHomePageNoHistory");
                    }
                }
                DateTime day = DateTime.Now;             
                command.SetParameterValue("@Num", diffnum);
                command.SetParameterValue("@DateTime", DateTime.Now.AddDays(-7));
                command.SetParameterValue("@C1List", domainItem.C1List);
                command.SetParameterValue("@ExceptC3List", domainItem.ExceptC3List);
                command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                command.SetParameterValue("@CompanyCode", CompanyCode);
                onlineList = command.ExecuteEntityList<OnlineList>();
                while (onlineList != null && onlineList.Count < diffnum && loopNum <= 6)//取一年之内的数据
                {
                    loopNum++;
                    if (loopNum == 1)
                    {
                        day = DateTime.Now.AddMonths(-loopNum);
                    }
                    else
                    {
                        day = DateTime.Now.AddMonths(-loopNum * 2 + 1);
                    }
                    command.SetParameterValue("@Num", diffnum);
                    command.SetParameterValue("@DateTime", day);
                    command.SetParameterValue("@C1List", domainItem.C1List);
                    command.SetParameterValue("@ExceptC3List", domainItem.ExceptC3List);
                    command.SetParameterValue("@CompanyCode", CompanyCode);
                    command.SetParameterValue("@GiftC3SysNo", Convert.ToInt32(GiftC3SysNo));
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }
    }
}
