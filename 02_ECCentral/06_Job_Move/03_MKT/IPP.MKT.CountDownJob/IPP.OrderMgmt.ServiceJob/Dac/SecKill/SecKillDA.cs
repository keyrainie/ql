using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.OrderMgmt.ServiceJob.BusinessEntities.SecKill;
using System.Collections;
using System.Data.SqlClient;
using Newegg.Oversea.Framework.DataAccess;
using System.Configuration;

namespace IPP.OrderMgmt.ServiceJob.Dac.SecKill
{
    public class SecKillDA
    {
        #region Query
        public static List<SecKillEntity> GetCountDownItem4SecKill()
        {
            List<SecKillEntity> result;
            DataCommand command = DataCommandManager.GetDataCommand("GetCountDownItem4SecKill");

            result = command.ExecuteEntityList<SecKillEntity>();

            return result;
        }

        public static SecKillEntity GetCountDownItemBySysno(int countdownSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCountDownItem4SecKillbySysNo");
            command.SetParameterValue("@SysNo", countdownSysNo);

            SecKillEntity entity = command.ExecuteEntity<SecKillEntity>();
            return entity;
        }

        public static ItemEntity LoadItemPrice(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetItemPriceInfo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            ItemEntity result = command.ExecuteEntity<ItemEntity>();
            return result;
        }

        public static InventoryEntity GetInventoryByProductSysNo(int ProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetInventoryByProductSysNo");
            command.SetParameterValue("@ProductSysNo", ProductSysNo);

            InventoryEntity result = command.ExecuteEntity<InventoryEntity>();
            return result;
        }

        public static List<InventoryStockEntity> GetInventoryStock(int ProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetInventoryStockByProductSysNo");

            command.SetParameterValue("@ProductSysNo", ProductSysNo);

            List<InventoryStockEntity> result = command.ExecuteEntityList<InventoryStockEntity>();
            return result;
        }

        public static int GetProductNotAutoSetVirtualKey(int ProductSysNo, int notAutoSetVirtualType, int CountDownSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductNotAutoSetVirtualKey");

            command.SetParameterValue("@ProductSysNo", ProductSysNo);
            command.SetParameterValue("@NotAutoSetVirtualType", notAutoSetVirtualType);
            command.SetParameterValue("@CountDownSysNo", CountDownSysNo);

            object sysno = command.ExecuteScalar();
            return sysno != null ? (int)sysno : -999;
        }

        public static List<InventoryEntity> GetInventorySysNoList(int productSysNo)
        {
            List<InventoryEntity> resultlist = new List<InventoryEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetInventorySysNoList");
            command.SetParameterValue("@ProductSysNo", productSysNo);

            try
            {
                resultlist = command.ExecuteEntityList<InventoryEntity>();

            }
            catch
            {
                return resultlist;
            }
            return resultlist;
        }

        public static int CountProduct_NotAutoSetVirtual(int ProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CountProduct_NotAutoSetVirtual");

            command.SetParameterValue("@ProductSysNo", ProductSysNo);

            return command.ExecuteScalar<int>();
        }

        public static int CountProduct_NotAutoSetVirtual2(int ProductSysNo, int NotAutoSetVirtualType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CountProduct_NotAutoSetVirtual2");

            command.SetParameterValue("@ProductSysNo", ProductSysNo);
            command.SetParameterValue("@NotAutoSetVirtualType", NotAutoSetVirtualType);

            return command.ExecuteScalar<int>();
        }
        #endregion

        #region  Action
        public static void UpdateInventory(int AffectedVirtualQty, int ProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInventory");

            command.SetParameterValue("@ProductSysNo", ProductSysNo);
            command.SetParameterValue("@VirtualQty", AffectedVirtualQty);

            command.ExecuteNonQuery();
        }

        public static void UpdateInventory_Stock(int VirtualQty, int ProductSysNo, int StockSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInventory_Stock");

            command.SetParameterValue("@ProductSysNo", ProductSysNo);
            command.SetParameterValue("@VirtualQty", VirtualQty);
            command.SetParameterValue("@StockSysNo", StockSysNo);

            command.ExecuteNonQuery();
        }

        public static void UpdateInventoryAvailabeQty(int ProductSysNo,int CountDownQty)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInventoryAvailabeQty");

            command.SetParameterValue("@ProductSysNo", ProductSysNo);
            command.SetParameterValue("@CountDownQty", CountDownQty);
            command.ExecuteNonQuery();
        }
        public static void UpdateInventoryStockAvailabeQty(InventoryStockEntity item)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInventoryStockAvailabeQty");

            command.SetParameterValue("@ProductSysNo",item.ProductSysNo);
            command.SetParameterValue("@StockSysNo", item.StockSysNo);
            command.SetParameterValue("@SubCountdownQty", item.SubCountdownQty);
            command.ExecuteNonQuery();
        }
        public static void RollbackMaxPerOrder(int ProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("RollbackMaxPerOrder");

            command.SetParameterValue("@ProductSysNo",ProductSysNo);
            command.ExecuteNonQuery();
        }
         public static void UpdateMaxPerOrder(int ProductSysNo,int MaxPerOrder)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateMaxPerOrder");

            command.SetParameterValue("@ProductSysNo",ProductSysNo);
            command.SetParameterValue("@MaxPerOrder", MaxPerOrder);
            command.ExecuteNonQuery();
        }
        public static void UpdateInventoryVirtualQty(int ProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInventoryTotalVirtualQty");

            command.SetParameterValue("@ProductSysNo", ProductSysNo);

            command.ExecuteNonQuery();
        }

        public static void AotuSetStockVirtualQty(int ProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AotuSetStockVirtualQty");
            command.SetParameterValue("@ProductSysNo", ProductSysNo);
            int n = command.ExecuteNonQuery();
        }

        public static ProductNotAutoSetVirtualEntity InsertProduct_NotAutoSetVirtual(ProductNotAutoSetVirtualEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertProduct_NotAutoSetVirtual");

            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@NotAutoSetVirtualType", entity.NotAutoSetVirtualType);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUserSysNo);
            command.SetParameterValue("@CreateTime", entity.CreateTime);
            command.SetParameterValue("@CountDownSysNo", entity.CountDownSysNo);
            command.SetParameterValue("@AbandonUserSysNo", entity.AbandonUserSysNo);
            command.SetParameterValue("@AbandonTime", entity.AbandonTime);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@Status", entity.Status);

            command.ExecuteEntity<ProductNotAutoSetVirtualEntity>();

            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));

            return entity;
        }

        public static ItemEntity UpdateItemPrice(ItemEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateItemPriceInfo");
            command.SetParameterValue("@CurrentPrice", entity.CurrentPrice);
            command.SetParameterValue("@CashRebate", entity.CashRebate);
            command.SetParameterValue("@Point", entity.Point);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            return command.ExecuteEntity<ItemEntity>();
        }

        public static SecKillEntity UpdateCountdown(SecKillEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCountdown");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@StartTime", entity.StartTime);
            command.SetParameterValue("@EndTime", entity.EndTime);
            command.SetParameterValue("@CountDownCurrentPrice", entity.CountDownCurrentPrice);
            command.SetParameterValue("@CountDownCashRebate", entity.CountDownCashRebate);
            command.SetParameterValue("@CountDownPoint", entity.CountDownPoint);
            command.SetParameterValue("@CountDownQty", entity.CountDownQty);
            command.SetParameterValue("@SnapShotCurrentPrice", entity.SnapShotCurrentPrice);
            command.SetParameterValue("@SnapShotCashRebate", entity.SnapShotCashRebate);
            command.SetParameterValue("@SnapShotPoint", entity.SnapShotPoint);
            command.SetParameterValue("@AffectedVirtualQty", entity.AffectedVirtualQty);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@AffectedStock", entity.AffectedStock);
            command.SetParameterValue("@Reasons", entity.Reasons);
            command.SetParameterValue("@SnapShotCurrentVirtualQty", entity.SnapShotCurrentVirtualQty);
            command.SetParameterValue("@IsCountDownAreaShow", entity.IsCountDownAreaShow);
            command.SetParameterValue("@PromotionType", entity.PromotionType);
            //command.SetParameterValue("@CompanyCode", entity.CompanyCode);        

            return command.ExecuteEntity<SecKillEntity>();
        }

        public static bool UpdateProduct_Ex(int productSysNo, string promotionType)
        {
            if (string.IsNullOrEmpty(promotionType))
            {
                DataCommand command = DataCommandManager.GetDataCommand("UPDATEProduct_Ex_Null");
                command.SetParameterValue("@ProductSysNo", productSysNo);
                return command.ExecuteNonQuery() > 0;
            }
            else
            {
                DataCommand command = DataCommandManager.GetDataCommand("UpdateProduct_Ex");
                command.SetParameterValue("@PromotionType", promotionType);
                command.SetParameterValue("@ProductSysNo", productSysNo);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public static SecKillEntity UpdateCountdownStatus(SecKillEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCountDownStatus");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);

            return command.ExecuteEntity<SecKillEntity>();
        }

        public static int CountInventoryByProductSysNo(int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CountInventoryByItemSysNo");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            return command.ExecuteScalar<int>();
        }

        public static InventoryEntity InitInventory(int productSysNo)
        {
            InventoryEntity entity = new InventoryEntity();
            entity.ProductSysNo = productSysNo;
            DataCommand command = DataCommandManager.GetDataCommand("InsertInventory");
            command.SetParameterValue("@ProductSysNo", productSysNo);

            command.ExecuteEntity<InventoryEntity>();

            entity.InventorySysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));
            return entity;
        }

        public static InventoryStockEntity InitInventoryStock(int productSysNo, int stockSysNo)
        {
            InventoryStockEntity entity = new InventoryStockEntity();
            entity.ProductSysNo = productSysNo;
            entity.StockSysNo = stockSysNo;
            DataCommand command = DataCommandManager.GetDataCommand("InsertInventoryStock");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@StockSysNo", stockSysNo);

            command.ExecuteEntity<InventoryStockEntity>();

            entity.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));
            return entity;
        }

        public static InventoryStockEntity GetInventoryStock(int ProductSysNo, int StockSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetInventoryStockByItemSysNoStockSysNo");
            command.SetParameterValue("@ProductSysNo", ProductSysNo);
            command.SetParameterValue("@StockSysNo", StockSysNo);

            InventoryStockEntity entity = command.ExecuteEntity<InventoryStockEntity>();

            return entity;
        }

        public static int InventoryUpdateVirtualQty(int productSysNo, int affectedVirtualQty)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInventoryVirtualQty2");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@Qty", affectedVirtualQty);

            int num = command.ExecuteNonQuery();
            return num;
        }



        public static int InventoryStockUpdateVirtualQty(int productSysNo, int StockSysNo, int affectedVirtualQty)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateInventoryStockVirtualQty2");
            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@StockSysNo", StockSysNo);
            command.SetParameterValue("@Qty", affectedVirtualQty);
            return command.ExecuteNonQuery();
        }

        public static void ProductNotAuto_SetVirtualUpdate(ProductNotAutoSetVirtualEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateProductNotAuto_SetVirtualKey");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@AbandonTime", entity.AbandonTime);
            command.SetParameterValue("@AbandonUserSysNo", entity.AbandonUserSysNo);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@SysNo", entity.SysNo);

            command.ExecuteNonQuery();
        }

        public static void Update_Inventory_VirtualQty(int ProductSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("update_inventory_virtualqty");
            command.SetParameterValue("@ProductSysNo", ProductSysNo);

            command.ExecuteNonQuery();
        }
        #endregion

        //public static void VirtualStoryFinish(string sql)
        //{
        //    CustomDataCommand command = DataCommandManager.CreateCustomDataCommand("IPP3");
        //    command.CommandText = sql;
        //    command.ExecuteNonQuery();
        //}     

        internal static void SendMailAboutInventoryInfo(string mailbody, int productsysno, string currstatus)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["MailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = "(info)" + DateTime.Now + "商品 " + productsysno + "在" + currstatus + " inventory & inventory_stock的虚拟仓库数量";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", mailbody);

            command.ExecuteNonQuery();
        }

        internal static void SendMailAboutExceptionInfo(string ErrorMsg)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["MailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = DateTime.Now + "限时抢购job在运行时异常";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", ErrorMsg);

            command.ExecuteNonQuery();
        }

        public static void CountDownStartWithholdQty(int countDownSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CountDown_Start_WithholdQty");

            command.SetParameterValue("@CountDownSysNo", countDownSysNo);
            command.ExecuteNonQuery();
        }

        public static void CountDownEndReleaseWithholdQty(int countDownSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CountDown_End_ReleaseWithholdQty");

            command.SetParameterValue("@CountDownSysNo", countDownSysNo);
            command.ExecuteNonQuery();
        }

        public static int CountDownSetEndTimeForNoQty()
        {
            DataCommand command = DataCommandManager.GetDataCommand("CountDown_SetEndTimeForNoQty");
            return command.ExecuteNonQuery();
        }
    }
}
