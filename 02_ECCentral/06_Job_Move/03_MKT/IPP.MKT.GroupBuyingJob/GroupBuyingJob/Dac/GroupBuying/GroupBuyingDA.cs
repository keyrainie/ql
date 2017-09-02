using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.MktToolMgmt.GroupBuyingJob.BusinessEntities;
using System.Collections;
using System.Data.SqlClient;
using Newegg.Oversea.Framework.DataAccess;
using System.Configuration;

namespace IPP.MktToolMgmt.GroupBuyingJob.Dac
{
    public class GroupBuyingDA
    {
        public static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public static string LanguageCode = ConfigurationManager.AppSettings["LanguageCode"];
        public static string OrderCount = ConfigurationManager.AppSettings["OrderCount"];



        #region Query
        public static List<ProductGroupBuyingEntity> GetProductGroupBuyingList()
        {
            List<ProductGroupBuyingEntity> result;
            DataCommand command = DataCommandManager.GetDataCommand("GetProductGroupBuyingList");
            int GroupBuyingSysNo = 0;
            int.TryParse(ConfigurationManager.AppSettings["GroupBuyingSysNo"], out GroupBuyingSysNo);
            command.SetParameterValue("@GroupBuyingSysNo", GroupBuyingSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            result = command.ExecuteEntityList<ProductGroupBuyingEntity>();

            return result;
        }

        public static ProductGroupBuyingEntity GetGroupBuyingItemBySysno(int GroupBuyingSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductGroupBuyingList");
            command.SetParameterValue("@GroupBuyingSysNo", GroupBuyingSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            ProductGroupBuyingEntity entity = command.ExecuteEntity<ProductGroupBuyingEntity>();
            return entity;
        }

        public static List<ProductGroupBuying_PriceEntity> GetProductGroupBuying_PriceList(ProductGroupBuyingEntity entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupBuyingPriceList");
            cmd.SetParameterValue("@ProductGroupBuyingSysNo", entity.SysNo);
            return cmd.ExecuteEntityList<ProductGroupBuying_PriceEntity>();

        }

        public static List<ProductPriceInfoEntity> GetProductPriceInfoList(int productSysNo, string isByGroup)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductPriceInfoList");

            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@IsByGroup", isByGroup);
            cmd.SetParameterValue("@CompanyCode", CompanyCode);

            return cmd.ExecuteEntityList<ProductPriceInfoEntity>();

        }

        public static List<ProductGroupBuying_SnapShotPriceEntity> GetSnapShotPriceList(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductSnapShotPriceList");

            cmd.SetParameterValue("@ProductGroupBuyingSysNo", productSysNo);

            return cmd.ExecuteEntityList<ProductGroupBuying_SnapShotPriceEntity>();

        }

        public static ProductPriceInfoEntity LoadItemPrice(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetItemPriceInfo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            ProductPriceInfoEntity result = command.ExecuteEntity<ProductPriceInfoEntity>();
            return result;
        }


        #endregion

        #region  Action

        public static void CreateSnapShotPrice(ProductGroupBuying_SnapShotPriceEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateSnapShotPrice");
            command.SetParameterValue("@ProductGroupBuyingSysNo", entity.ProductGroupBuyingSysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@SnapShotCashRebate", entity.SnapShotCashRebate);
            command.SetParameterValue("@SnapShotCurrentPrice", entity.SnapShotCurrentPrice);
            command.SetParameterValue("@SnapShotMaxPerOrder", entity.SnapShotMaxPerOrder);
            command.SetParameterValue("@SnapShotPoint", entity.SnapShotPoint);
            command.SetParameterValue("@SnapshotBasicPrice", entity.SnapshotBasicPrice);
            command.ExecuteNonQuery();
        }

        #region 2011-12-1更新 Rik.K.Li

        /// <summary>
        /// 商品价格更新
        /// </summary>
        /// <param name="entity">价格信息</param>
        /// <param name="note">日志所需Note</param>
        /// <param name="priceLogType">日志所需PriceLogType</param>
        /// <returns></returns>
        public static int UpdateItemPrice(ProductPriceInfoEntity entity, ProductGroupBuyingEntity gBuyEntity,
                                                DateTime? createDate,
                                                String updateUser,
                                                DateTime? updateDate,
                                                String note,
                                                String fromSystem,
                                                String priceLogType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateItemPriceInfoBySP");

            cmd.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            cmd.SetParameterValue("@BasicPrice", entity.BasicPrice);
            cmd.SetParameterValue("@CurrentPrice", entity.CurrentPrice);
            cmd.SetParameterValue("@IsWholeSale", DBNull.Value);
            cmd.SetParameterValue("@Q1", DBNull.Value);
            cmd.SetParameterValue("@P1", DBNull.Value);
            cmd.SetParameterValue("@Q2", DBNull.Value);
            cmd.SetParameterValue("@P2", DBNull.Value);
            cmd.SetParameterValue("@Q3", DBNull.Value);
            cmd.SetParameterValue("@P3", DBNull.Value);
            cmd.SetParameterValue("@IsCheckedWholeSale1", "true");
            cmd.SetParameterValue("@IsCheckedWholeSale2", "true");
            cmd.SetParameterValue("@IsCheckedWholeSale3", "true");
            cmd.SetParameterValue("@UnitCost", entity.UnitCost);
            cmd.SetParameterValue("@CashRebate", entity.CashRebate);
            cmd.SetParameterValue("@Point", entity.Point);
            cmd.SetParameterValue("@PointType", DBNull.Value);
            cmd.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
            cmd.SetParameterValue("@ClearanceSale", DBNull.Value);
            cmd.SetParameterValue("@LastOnSaleTime", DBNull.Value);
            cmd.SetParameterValue("@CreateUser", gBuyEntity.InUser);
            cmd.SetParameterValue("@CreateDate", createDate);
            cmd.SetParameterValue("@UpdateUser", updateUser);
            cmd.SetParameterValue("@UpdateDate", updateDate);
            cmd.SetParameterValue("@CompanyCode", CompanyCode);
            cmd.SetParameterValue("@LanguageCode", LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", DBNull.Value);
            cmd.SetParameterValue("@Note", note);
            cmd.SetParameterValue("@OptIP", fromSystem);
            cmd.SetParameterValue("@PriceLogType", priceLogType);
            cmd.SetParameterValue("@PriceStatus", DBNull.Value);
            cmd.SetParameterValue("@AuditUserSysNo", DBNull.Value);
            cmd.SetParameterValue("@IsExistRankPrice", DBNull.Value);
            cmd.SetParameterValue("@Discount", DBNull.Value);

            return cmd.ExecuteNonQuery();
        }

        //public static ItemEntity UpdateItemPrice(ProductPriceInfoEntity entity)
        //{
        //    DataCommand command = DataCommandManager.GetDataCommand("UpdateItemPriceInfo");
        //    command.SetParameterValue("@CurrentPrice", entity.CurrentPrice);
        //    command.SetParameterValue("@CashRebate", entity.CashRebate);
        //    command.SetParameterValue("@Point", entity.Point);
        //    command.SetParameterValue("@MaxPerOrder", entity.MaxPerOrder);
        //    command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
        //    command.SetParameterValue("@CompanyCode", CompanyCode);
        //    return command.ExecuteEntity<ItemEntity>();
        //}
        #endregion

        public static ProductGroupBuyingEntity UpdateGroupBuying(ProductGroupBuyingEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateGroupBuying");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            return command.ExecuteEntity<ProductGroupBuyingEntity>();
        }

        public static ProductGroupBuyingEntity UpdateProductGroupBuyingRun(ProductGroupBuyingEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductGroupBuyingRun");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@OriginalPrice", entity.OriginalPrice);
            command.SetParameterValue("@BasicPrice", entity.BasicPrice);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            return command.ExecuteEntity<ProductGroupBuyingEntity>();
        }

        public static ProductGroupBuyingEntity UpdateProductGroupBuyingFinish(ProductGroupBuyingEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductGroupBuyingFinish");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@DealPrice", entity.DealPrice);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            return command.ExecuteEntity<ProductGroupBuyingEntity>();
        }

        public static bool UpdateProductEx(int productSysNo, string promotionType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductEx");
            command.SetParameterValue("@PromotionType", promotionType);
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            return command.ExecuteNonQuery() > 0;
        }

        public static ProductGroupBuyingEntity UpdateGroupBuyingAbandon(ProductGroupBuyingEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateGroupBuyingAbandon");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            return command.ExecuteEntity<ProductGroupBuyingEntity>();
        }

        public static void UpdateCurrentSellCount(int sysNo, int currentSellCount, string isSettlement, decimal dealPrice)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCurrentSellCount");

            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@CurrentSellCount", currentSellCount);
            command.SetParameterValue("@IsSettlement", isSettlement);
            command.SetParameterValue("@DealPrice", dealPrice);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.ExecuteNonQuery();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="isSuccess">Y为成功，N为失败</param>
        /// <returns></returns>
        public static int SetGroupBuyingSuccesDate(int sysNo, string isSuccess)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SetGroupBuyingSuccesDate");

            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@IsSuccess", isSuccess);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 读取订单数
        /// </summary>
        /// <param name="productGroupBuyingSysNo"></param>
        /// <param name="isByGroup"></param>
        /// <returns></returns>
        public static int GetCurrentSellCount(int productGroupBuyingSysNo, string isByGroup)
        {
            int result = 0;
            DataCommand command = DataCommandManager.GetDataCommand("GetCurrentSellCount");
            command.SetParameterValue("@ProductGroupBuyingSysNo", productGroupBuyingSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            result = Convert.ToInt32(command.ExecuteScalar());
            if (result == 0) result = int.Parse(OrderCount);

            return result;
        }


        /// <summary>
        /// 读取参与抽奖人数
        /// </summary>
        /// <param name="productGroupBuyingSysNo"></param>
        /// <param name="isByGroup"></param>
        /// <returns></returns>
        public static int GetCurrentSellCountForLottery(int productGroupBuyingSysNo, string isByGroup)
        {
            int result = 0;
            DataCommand command = DataCommandManager.GetDataCommand("GetCurrentSellCountForLottery");
            command.SetParameterValue("@ProductGroupBuyingSysNo", productGroupBuyingSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            result = Convert.ToInt32(command.ExecuteScalar());
            if (result == 0) result = int.Parse(OrderCount);

            return result;
        }


        /// <summary>
        /// 读取商品原价
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="isByGroup">是否根据组读取</param>
        /// <returns></returns>
        public static decimal GetOriginalPrice(int productSysNo, string isByGroup)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetOriginalPrice");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@IsByGroup", isByGroup);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            return Convert.ToDecimal(command.ExecuteScalar());
        }


        #endregion

        public static void SendMailAbandonGroupBuyingInfo(string mailTo, string mailInfo, string mailSubject)
        {

            mailTo += ";" + Convert.ToString(ConfigurationManager.AppSettings["MailPMCC"]);
            string MailSubject = DateTime.Now + mailSubject;

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", mailTo);
            command.SetParameterValue("@CCMailAddress", "");
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", mailInfo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", LanguageCode);
            command.ExecuteNonQuery();
        }

        internal static void SendMailAboutExceptionInfo(string ErrorMsg)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["MailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = DateTime.Now + "团购job在运行时异常";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", ErrorMsg);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", LanguageCode);
            command.ExecuteNonQuery();
        }

        public static string GetUserEmailByUserName(string userName)
        {
            try
            {
                DataCommand command = DataCommandManager.GetDataCommand("GetUserEmailByUserName");
                command.SetParameterValue("@UserName", userName);
                command.SetParameterValue("@CompanyCode", CompanyCode);
                return command.ExecuteScalar().ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 记录商品价格日志
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="currentPrice"></param>
        /// <param name="oldPrice"></param>
        /// <param name="offset"></param>
        /// <param name="UnitCost"></param>
        /// <param name="CashRebate"></param>
        /// <param name="Point"></param>
        /// <param name="createUser"></param>
        /// <param name="createDate"></param>
        /// <param name="updateUser"></param>
        /// <param name="updateDate"></param>
        /// <param name="Note"></param>
        /// <param name="FromSystem"></param>        
        /// <param name="priceLogType">PMAdjust,GroupBuying,ComparePrice,Seller</param>
        /// <returns></returns>
        public static int InsertGroupBuyingProductPricechangeLog(string productSysNo,
                                                               string currentPrice,
                                                               string oldPrice,
                                                               string offset,
                                                               decimal UnitCost,
                                                               decimal CashRebate,
                                                               int Point,
                                                               string createUser,
                                                               DateTime? createDate,
                                                               string updateUser,
                                                               DateTime? updateDate,
                                                               string Note,
                                                               string FromSystem,
                                                               string priceLogType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertGroupBuyingProductPricechangeLog");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@NewPrice", currentPrice);
            cmd.SetParameterValue("@OldPrice", oldPrice);
            cmd.SetParameterValue("@AdjustmentPrice", offset);
            cmd.SetParameterValue("@UnitCost", UnitCost);
            cmd.SetParameterValue("@CashRebate", CashRebate);
            cmd.SetParameterValue("@Point", Point);
            cmd.SetParameterValue("@CreateUser", createUser);
            cmd.SetParameterValue("@CreateDate", createDate);
            cmd.SetParameterValue("@UpdateUser", updateUser);
            cmd.SetParameterValue("@UpdateDate", updateDate.HasValue ? updateDate : null);
            cmd.SetParameterValue("@Note", Note);
            cmd.SetParameterValue("@FromSystem", FromSystem);
            cmd.SetParameterValue("@CompanyCode", CompanyCode);
            cmd.SetParameterValue("@PriceLogType", priceLogType);
            return cmd.ExecuteNonQuery();
        }

        public static void SyncGroupBuyingStatus(ProductGroupBuyingEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SyncGroupBuyingStatus");

            command.SetParameterValue("@RequestSysNo", entity.RequestSysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@Reasons", entity.Reasons);
            command.SetParameterValue("@ReplyType", "ChangeGroupBuyingStatus");

            command.ExecuteNonQuery();
        }

        public static void SyncGroupBuyingSellCount(int requestSysNo,int sellcount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SyncGroupBuyingSellCount");

            command.SetParameterValue("@RequestSysNo", requestSysNo);
            command.SetParameterValue("@SellCount", sellcount);
            command.SetParameterValue("@ReplyType", "UpdateSaleCount");

            command.ExecuteNonQuery();
        }
    }
}
