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
        #region definition

        //--新品上架：定义：第一次上架日期离当前系统日期最近的商品
        //--让利促销：定义：7天之内降价最大的商品，降价金额=7天之内最高金额-当前金额 
        //--当季热销(m3)：定义：7天之内销售数量最高的商品

        //--新品上架：定义：第一次上架日期离当前系统日期最近的商品
        //--热销商品：定义：依次抓取该一级分类下每个二级分类中销售数量最高的商品（总数量在14-20个）?????
        //--             例如：一级分类下有10个二级分类，则每个二级分类中抓取销量最高的2个商品
        //--                   一级分类下有6个二级分类，则每个二级分类中抓取销量最高的3个商品
        //--4.二级分类：范围：该分类下的所有商品（系统改进前修改为销量排行）
        //--            定义：7天之内降价最大的商品，降价金额=7天之内最高金额-当前金额
        //--                  7天之内降价最大的商品，降价百分比=（7天之内最高金额-当前金额）/7天之内最高金额*100%
        //--5.三级分类：范围：该分类下的所有商品
        //--            定义：7天之内降价最大的商品，降价金额=7天之内最高金额-当前金额

        #endregion

        public static List<OnlineList> CheckOnlinelistItemByPageType(int pageType)
        {
            List<OnlineList> onlineList = null;

            DataCommand command = DataCommandManager.GetDataCommand("CheckOnlinelistItemByPageType");
            command.SetParameterValue("@PageType", pageType);
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static int GetLocationSysno(int pagetype, int positionID, int pageID)
        {
            int locationsysno = 0;
            DataCommand command = DataCommandManager.GetDataCommand("CheckOnlineListLocation");
            command.SetParameterValue("@PageType", pagetype);
            command.SetParameterValue("@PositionID", positionID);
            command.SetParameterValue("@PageID", pageID);
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
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

                dc.SetParameterValue("@PageType", pagetype);
                dc.SetParameterValue("@PositionID", positionID);
                dc.SetParameterValue("@Priority", 0);
                dc.SetParameterValue("@PageID", pageID);
                dc.SetParameterValue("@Description", "系统添加位置");
                dc.SetParameterValue("@InDate", DateTime.Now);
                dc.SetParameterValue("@InUser", "System");
                dc.SetParameterValue("@EditDate", DateTime.Now);
                dc.SetParameterValue("@EditUser", "System");
                dc.SetParameterValue("@Status", "A");
                dc.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                dc.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);
                dc.SetParameterValue("@StoreCompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);

                dc.ExecuteNonQuery();
                locationsysno = (int)dc.GetParameterValue("@OnlineListlocationSysNo");
            }
            return locationsysno;
        }

        public static List<OnlineList> GetAllBrand()
        {
            List<OnlineList> result = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllBrand");
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            result = command.ExecuteEntityList<OnlineList>();
            return result;
        }

        public static List<OnlineList> GetAllC1SysNolist()
        {
            List<OnlineList> onlineList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllC1SysNolist");
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static List<OnlineList> GetAllC2SysNolist()
        {
            List<OnlineList> onlineList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllC2SysNolist");
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }

        public static List<OnlineList> GetAllC3SysNolist()
        {
            List<OnlineList> onlineList = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetAllC3SysNolist");
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            onlineList = command.ExecuteEntityList<OnlineList>();
            return onlineList;
        }


        public static List<OnlineList> GetSaleHightItem(int num, int C2Sysno, List<int> ItemSysnolist)
        {
            List<OnlineList> onlineList = null;

            if (num > 0)
            {
                if (ItemSysnolist != null && ItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleHightItemForHomePage");
                    command.ReplaceParameterValue("@num", num.ToString());
                    command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                    command.ReplaceParameterValue("@ItemSysnolist", ItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSaleHightItemForHomePageSingle");
                    command.ReplaceParameterValue("@num", num.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetSaleHightItemForBrand(int num, List<int> HasItemSysnolist, int BrandSysNo)
        {
            List<OnlineList> onlineList = null;

            if (num > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetSaleHightItemForBrand");
                    command.ReplaceParameterValue("@num", num.ToString());
                    command.ReplaceParameterValue("@BrandSysNo", BrandSysNo.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetSaleHightItemForBrandSingle");
                    command.ReplaceParameterValue("@num", num.ToString());
                    command.ReplaceParameterValue("@BrandSysNo", BrandSysNo.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetSaleHightItemForC1Page(int diffnum, List<int> HasItemSysnolist, int C1Sysno, int C2Sysno)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    if (C2Sysno != -1)
                    {
                        DataCommand command = DataCommandManager.GetDataCommand("GetSaleHightItemForC1PagebyC2sysno");
                        command.ReplaceParameterValue("@num", diffnum.ToString());
                        command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                        command.ReplaceParameterValue("@ItemSysnolist", HasItemSysnolist.ToListString());
                        command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                        onlineList = command.ExecuteEntityList<OnlineList>();
                    }
                    else
                    {
                        DataCommand command = DataCommandManager.GetDataCommand("GetSaleHightItemForC1Page");
                        command.ReplaceParameterValue("@num", diffnum.ToString());
                        command.ReplaceParameterValue("@C1SysNo", C1Sysno.ToString());
                        command.ReplaceParameterValue("@ItemSysnolist", HasItemSysnolist.ToListString());
                        command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                        onlineList = command.ExecuteEntityList<OnlineList>();
                    }
                }
                else
                {
                    if (C2Sysno != -1)
                    {
                        DataCommand command = DataCommandManager.GetDataCommand("GetSaleHightItemForC1PageByC2sysnoSingle");
                        command.ReplaceParameterValue("@num", diffnum.ToString());
                        command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                        command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                        onlineList = command.ExecuteEntityList<OnlineList>();
                    }
                    else
                    {
                        DataCommand command = DataCommandManager.GetDataCommand("GetSaleHightItemForC1PageSingle");
                        command.ReplaceParameterValue("@num", diffnum.ToString());
                        command.ReplaceParameterValue("@C1SysNo", C1Sysno.ToString());
                        command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                        onlineList = command.ExecuteEntityList<OnlineList>();
                    }
                }
            }
            return onlineList;
        }


        public static List<OnlineList> GetNewItem(int num, int C2Sysno)
        {
            List<OnlineList> onlineList = null;

            if (num > 0)
            {
                DataCommand command = DataCommandManager.GetDataCommand("GetSaleHightItem");
                command.ReplaceParameterValue("@num", num.ToString());
                command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                onlineList = command.ExecuteEntityList<OnlineList>();
            }
            return onlineList;
        }

        public static List<OnlineList> GetNewItemForBrand(int diffnum, List<int> HasItemSysnolist, int BrandSysNo)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetNewItemForBrand");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@BrandSysNo", BrandSysNo.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                if (onlineList == null || onlineList.Count == 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetNewItemForBrandSingle");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@BrandSysNo", BrandSysNo.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetNewItemForC1Page(int diffnum, List<int> HasItemSysnolist, int C1Sysno)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetNewItemForC1Page");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C1SysNo", C1Sysno.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetNewItemForC1PageSingle");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C1SysNo", C1Sysno.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetSpecialItemForC1Page(int diffnum, List<int> HasItemSysnolist, int C1Sysno)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetSpecialItemForC1Page");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C1SysNo", C1Sysno.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetSpecialItemForC1PageSingle");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C1SysNo", C1Sysno.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }


        public static List<OnlineList> GetPriceDownItem(int num, int C2Sysno, List<int> HasItemSysnolist)
        {
            List<OnlineList> onlineList = null;
            if (num > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForHome");
                    command.ReplaceParameterValue("@num", num.ToString());
                    command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                    command.ReplaceParameterValue("@ItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItem");
                    command.ReplaceParameterValue("@num", num.ToString());
                    command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetPriceDownItemForC1Page(int diffnum, List<int> HasItemSysnolist, int C1Sysno)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForC1Page");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C1SysNo", C1Sysno.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForC1PageSingle");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C1SysNo", C1Sysno.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetPriceDownItemForC2(int diffnum, List<int> HasItemSysnolist, int C2Sysno)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForC2Page");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForC2PageSingle");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetPriceDownItemForC3(int diffnum, List<int> HasItemSysnolist, int C3Sysno)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForC3Page");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C3SysNo", C3Sysno.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForC3PageSingle");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C3SysNo", C3Sysno.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetPriceDownItemForBrand(int diffnum, List<int> HasItemSysnolist, int BrandSysNo)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForBrand");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@BrandSysNo", BrandSysNo.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                if (onlineList == null || onlineList.Count == 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownItemForBrandSingle");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@BrandSysNo", BrandSysNo.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
            }
            return onlineList;
        }

        public static List<OnlineList> GetPriceDownPercentItemForC2(int diffnum, List<int> HasItemSysnolist, int C2Sysno)
        {
            List<OnlineList> onlineList = null;

            if (diffnum > 0)
            {
                if (HasItemSysnolist != null && HasItemSysnolist.Count > 0)
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownPercentItemForC2");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                    command.ReplaceParameterValue("@HasItemSysnolist", HasItemSysnolist.ToListString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                    onlineList = command.ExecuteEntityList<OnlineList>();
                }
                else
                {
                    DataCommand command = DataCommandManager.GetDataCommand("GetPriceDownPercentItemForC2Single");
                    command.ReplaceParameterValue("@num", diffnum.ToString());
                    command.ReplaceParameterValue("@C2SysNo", C2Sysno.ToString());
                    command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
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
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);
            command.SetParameterValue("@StoreCompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);

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
            command.SetParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);

            command.ExecuteNonQuery();
        }

        public static void DeleteOnlineListItem(int pagetype, int positionid, List<int> sysnolist, DateTime beginTime)
        {
            if (sysnolist != null && sysnolist.Count > 0)
            {
                DataCommand command = DataCommandManager.GetDataCommand("DeleteOnlineList");
                command.ReplaceParameterValue("@PageType", pagetype.ToString());
                command.ReplaceParameterValue("@PositionID", positionid.ToString());
                command.ReplaceParameterValue("@BeginTime", beginTime.ToString("yyyy-MM-dd HH:mm:ss"));
                command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                command.ExecuteNonQuery();
            }
            else
            {
                DataCommand command = DataCommandManager.GetDataCommand("DeleteOnlineListSingle");
                command.ReplaceParameterValue("@PageType", pagetype.ToString());
                command.ReplaceParameterValue("@PositionID", positionid.ToString());
                command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
                command.ExecuteNonQuery();
            }
        }

        internal static int CheckBrandItemNum(int brandsysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckBrandItemNum");
            command.ReplaceParameterValue("@BrandSysNo", brandsysno.ToString());
            command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            try
            {
                return command.ExecuteScalar<int>();
            }
            catch
            {

                return 0;
            }
        }

        internal static List<OnlineList> CheckC2ItemNumOnC1(int C1sysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckC2ItemNumOnC1");
            command.ReplaceParameterValue("@Category1Sysno", C1sysno.ToString());
            command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            return command.ExecuteEntityList<OnlineList>();

        }

        internal static void ClearTableOnLinelist(string day)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ClearTableOnLinelist");
            command.ReplaceParameterValue("@CompanyCode", ConfigurationManager.AppSettings["CompanyCode"]);
            command.ReplaceParameterValue("@Day", day);
            command.ExecuteNonQuery();
        }
    }
}
