using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.ServiceJob.Dac.SecKill;
using IPP.OrderMgmt.ServiceJob.BusinessEntities.SecKill;
using System.Transactions;
using Newegg.Oversea.Framework.ExceptionBase;
using System.Collections;
using IPP.OrderMgmt.ServiceJob.Providers;
using System.Configuration;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.ServiceJob.Biz.SecKill
{
    public class SecKillBP
    {
        /// <summary>
        /// 业务日志文件
        /// </summary>
        private static string BizLogFile;

        public static JobContext context { get; set; }

        #region Entry Point
        /// <summary>
        ///检查是否有要运行的秒杀商品
        /// </summary>
        /// <param name="bizLogFile">业务日志文件全名</param>
        public static void CheckCountDownSecKill(string bizLogFile)
        {
            BizLogFile = bizLogFile;
            WriteLog("\r\n------------------- Begin-------------------------");
            WriteConsoleInfo("\r\n------------------- Begin-------------------------");

            WriteLog("\r\n设置没有库存的正在运行的售完即止活动结束时间为当前时间");
            WriteConsoleInfo("\r\n设置没有库存的正在运行的售完即止活动结束时间为当前时间");
            int intEndCount = SecKillDA.CountDownSetEndTimeForNoQty();

            WriteLog(string.Format("\r\n{0}", intEndCount));
            WriteConsoleInfo(string.Format("\r\n{0}", intEndCount));

            // 获取限时抢购中秒杀商品的记录
            List<SecKillEntity> CountDownSecKillList = SecKillDA.GetCountDownItem4SecKill();
            string startmsg = "获取到限时抢购中就绪和运行态的记录，共" + CountDownSecKillList.Count + "条.";
            WriteLog(startmsg);
            WriteConsoleInfo(startmsg);

            if (CountDownSecKillList.Count == 0)
            {
                return;
            }
            StringBuilder startNum = new StringBuilder();
            StringBuilder finishNum = new StringBuilder();
            StringBuilder abandonNum = new StringBuilder();

            foreach (SecKillEntity oCountdown in CountDownSecKillList)
            {
                string info = string.Format("限时抢购SysNo|产品SysNo|状态-->{0}|{1}|{2}\r\n",
                    oCountdown.SysNo, oCountdown.ProductSysNo, oCountdown.Status);
                if (oCountdown.Status == 0
                    && (oCountdown.StartTime <= DateTime.Now
                    && oCountdown.EndTime > DateTime.Now))
                {
                    try
                    {
                        if (!SetRunning(oCountdown.SysNo))
                        {
                            SetAbandon(oCountdown.SysNo);
                            abandonNum.AppendLine(info);
                        }
                        else
                        {
                            startNum.AppendLine(info);
                        }
                    }
                    catch (Exception exp)
                    {
                        SendExceptionInfoEmail(exp.Message.ToString());
                        WriteLog(string.Format("{0}就绪->运行 出错了!异常:{1}", info, exp.Message.ToString()));
                        continue;
                    }
                }
                if (oCountdown.Status == 1 && (oCountdown.EndTime < DateTime.Now))
                {
                    try
                    {
                        SetFinish(oCountdown.SysNo);
                        finishNum.AppendLine(info);
                    }
                    catch (Exception ex)
                    {
                        SendExceptionInfoEmail(ex.Message.ToString());
                        WriteLog("运行->结束 出错了![" + info + "]" + "异常:" + ex.ToString());
                        continue;
                    }
                }
            }

            string endMsg = "本轮运行结果：\r\n就绪->运行:\r\n" + startNum.ToString() + "\r\n就绪->作废：\r\n"
                                  + abandonNum.ToString() + "\r\n运行->完成：\r\n" + finishNum.ToString();
            WriteLog(endMsg);
            WriteLog("------------------- End-----------------------------\r\n");
            WriteConsoleInfo(endMsg);
        }

        private static bool SetRunning(int countdownSysNo)
        {

            //必须是Ready状态，切换价格和库存
            SecKillEntity countdownItem = SecKillDA.GetCountDownItemBySysno(countdownSysNo);
            if (countdownItem.Status != (int)CountdownStatus.Ready)
            {
                throw new BusinessException("不是就绪状态");
            }
            TransactionOptions transactionOptions=new TransactionOptions();
            transactionOptions.IsolationLevel= IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                //最前端修改，不会影响其他逻辑
                if (countdownItem.IsReservedQty == 1)
                {
                    //这里改,更新库存.
                    UpdateReservedQTY(countdownItem);
                }

                countdownItem.Status = (int)CountdownStatus.Running;
                #region 设置限时抢购为原始价格并记录抢购价格//获取商品价格
                ItemEntity itemPrice = SecKillDA.LoadItemPrice(countdownItem.ProductSysNo);

                countdownItem.SnapShotCurrentPrice = itemPrice.CurrentPrice;
                countdownItem.SnapShotCashRebate = itemPrice.CashRebate;
                countdownItem.SnapShotPoint = itemPrice.Point;

                itemPrice.CurrentPrice = countdownItem.CountDownCurrentPrice;
                itemPrice.CashRebate = countdownItem.CountDownCashRebate;
                itemPrice.Point = countdownItem.CountDownPoint;

                #endregion

                InventoryEntity inventory = SecKillDA.GetInventoryByProductSysNo(countdownItem.ProductSysNo);
                countdownItem.AffectedVirtualQty = inventory.VirtualQty;

                //获取当前的分仓的虚库数量
                List<InventoryStockEntity> Invstocklist = SecKillDA.GetInventoryStock(countdownItem.ProductSysNo);
                StringBuilder stockonlineqty = new StringBuilder();
                foreach (InventoryStockEntity itemStock in Invstocklist)
                {
                    stockonlineqty.Append(string.Format("{0}:{1};", itemStock.StockSysNo
                        , itemStock.AvailableQty + itemStock.ConsignQty + itemStock.VirtualQty));
                }

                //判断是否满足各分仓的OnlineQty>=各分仓的限时抢购数量
                //--逻辑已经修改，不满足分仓数量也可以限时抢购
                string curVirtualQty = string.Empty;
                IsCanRunningCountDown(Invstocklist, countdownItem, out curVirtualQty);
                countdownItem.SnapShotCurrentVirtualQty = curVirtualQty;

                #region   设置记录为不自动设置虚拟库存
                ProductNotAutoSetVirtualEntity oEntity = new ProductNotAutoSetVirtualEntity();
                oEntity.CountDownSysNo = countdownSysNo;
                oEntity.CreateTime = DateTime.Now;
                oEntity.CreateUserSysNo = 493;
                oEntity.NotAutoSetVirtualType = (int)NotAutoSetVirtualType.CountDown;
                oEntity.Note = "CountDown Run";
                oEntity.ProductSysNo = countdownItem.ProductSysNo;
                oEntity.Status = 0;
                ProductNotAutoSetVirtualInsert(oEntity);

                #endregion

                #region 不通过虚库控制库存 注释此间代码
                if (1 == 2)
                {
                    #region 获取当前商品总库信息
                    string curStockQty = string.Empty;
                    if (!BatchUpdateStockVirtual_Run(countdownItem, stockonlineqty.ToString()))
                    {
                        return false;
                    }

                    List<InventoryStockEntity> inventoryStocklist = SecKillDA.GetInventoryStock(countdownItem.ProductSysNo);
                    //处理库存差额， 库存差额是为计算可用库存做准备的。最小为0，为0代表实际库存就是可卖库存。
                    int AffectedVirtualQty = 0;
                    foreach (InventoryStockEntity stock in inventoryStocklist)
                    {
                        AffectedVirtualQty += stock.VirtualQty;
                        curStockQty += stock.StockSysNo + ":" + (stock.AvailableQty + stock.ConsignQty + stock.VirtualQty).ToString() + ";";
                    }

                    if (!(countdownItem.IsLimitedQty == 1 && countdownItem.IsReservedQty == 1))
                    {
                        countdownItem.AffectedStock = curStockQty;
                    }

                    #endregion

                    ////更改总仓虚库
                    //if (!SetVirtualQty(countdownItem.ProductSysNo, AffectedVirtualQty))
                    //{
                    //    return false;
                    //}
                    InventoryEntity inventory2 = SecKillDA.GetInventoryByProductSysNo(countdownItem.ProductSysNo);
                    if (countdownItem.IsLimitedQty == 0 && countdownItem.IsReservedQty == 0)
                    {
                        countdownItem.CountDownQty = inventory2.AvailableQty + inventory2.ConsignQty;// +inventory2.VirtualQty;
                    }
                    else if (countdownItem.IsLimitedQty == 0
                        && inventory2.AvailableQty + inventory2.ConsignQty + inventory2.VirtualQty != countdownItem.CountDownQty)
                    {
                        countdownItem.CountDownQty = inventory2.AvailableQty + inventory2.ConsignQty;// +inventory2.VirtualQty;
                    }
                }
                #endregion
                SecKillDA.CountDownStartWithholdQty(countdownItem.SysNo);
                SecKillDA.UpdateCountdown(countdownItem);
                SecKillDA.UpdateItemPrice(itemPrice);
                if (countdownItem.IsPromotionSchedule == 1)
                {
                    SecKillDA.UpdateMaxPerOrder(countdownItem.ProductSysNo, countdownItem.MaxPerOrder);
                }
                if (!string.IsNullOrEmpty(countdownItem.PromotionType) && countdownItem.PromotionType.ToUpper() == "DC")
                {
                    SecKillDA.UpdateProduct_Ex(countdownItem.ProductSysNo, "DC");
                }
                ts.Complete();
            }
            return true;
        }

        private static void UpdateReservedQTY(SecKillEntity countdownItem)
        {
            SecKillDA.UpdateInventoryAvailabeQty(countdownItem.ProductSysNo, countdownItem.CountDownQty);
            foreach (var item in countdownItem.WarehouseList)
            {
                SecKillDA.UpdateInventoryStockAvailabeQty(item);
            }
        } 

        private static void SetAbandon(int countdownSysNo)
        {
            //必须是Ready或者VerifyFaild状态         
            SecKillEntity entity = SecKillDA.GetCountDownItemBySysno(countdownSysNo);

            if (entity.Status != (int)CountdownStatus.Ready && entity.Status != (int)CountdownStatus.VerifyFaild)
            {
                throw new BusinessException("the current status not allow such opertion");
            }

            int SysNo = SecKillDA.GetProductNotAutoSetVirtualKey(entity.ProductSysNo
                , (int)NotAutoSetVirtualType.CountDown, entity.SysNo);

            entity.Status = (int)CountdownStatus.Abandon;
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                if (entity.IsReservedQty == 1)
                {
                    UpdateReservedQTY(entity);
                }
                SetVirtualQty_WhenCountDownAbandon(entity, SysNo);
                if (entity.IsPromotionSchedule == 1)
                {
                    SecKillDA.RollbackMaxPerOrder(entity.ProductSysNo);
                }
                SecKillDA.CountDownEndReleaseWithholdQty(entity.SysNo);
                SecKillDA.UpdateCountdownStatus(entity);
                SecKillDA.UpdateProduct_Ex(entity.ProductSysNo, string.Empty);
                ts.Complete();
            }
        }

        public static void SetFinish(int countdownSysNo)
        {
            //必须是Running           
            SecKillEntity oCountdown = SecKillDA.GetCountDownItemBySysno(countdownSysNo);

            if (oCountdown.Status != (int)CountdownStatus.Running)
            {
                throw new BusinessException("the current status not allow such opertion");
            }

            oCountdown.Status = (int)CountdownStatus.Finish;
            //edit by kathy 2009-12-10
            //先更改限时抢购的状态为完成，在调整相应的虚拟库存。
            //SetInventoryVirtualQty_whenCountdownItemIsRun 此job每五分钟会检测运行态的countdownitem：
            //inventory的virtualQty和inventory_stock的virtualQty之和是否相等，如果不等,把inventory的virtualQty更改成inventory_stock的virtualQty之和）
            

            ItemEntity itemPrice = SecKillDA.LoadItemPrice(oCountdown.ProductSysNo);
            itemPrice.CurrentPrice = oCountdown.SnapShotCurrentPrice;
            itemPrice.CashRebate = oCountdown.SnapShotCashRebate;
            itemPrice.Point = oCountdown.SnapShotPoint;
            //限时抢购完成更改虚库信息
            int SysNo = SecKillDA.GetProductNotAutoSetVirtualKey(oCountdown.ProductSysNo
                , (int)NotAutoSetVirtualType.CountDown, oCountdown.SysNo);
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                SecKillDA.UpdateCountdown(oCountdown);
                SetVirtualQty_CountDownFinish(oCountdown, SysNo);
            
                SecKillDA.CountDownEndReleaseWithholdQty(oCountdown.SysNo);
                SecKillDA.UpdateItemPrice(itemPrice);
                SecKillDA.UpdateProduct_Ex(oCountdown.ProductSysNo, string.Empty);
                if (oCountdown.IsPromotionSchedule == 1)
                {
                    SecKillDA.RollbackMaxPerOrder(oCountdown.ProductSysNo);
                }
                ts.Complete();
            }
        }

        private static bool BatchUpdateStockVirtual_Run(SecKillEntity countdownItem, string snapShotOnlineQty)
        {
            if (string.IsNullOrWhiteSpace(countdownItem.AffectedStock))
            {
                return true;
            }
            string Info = null;
            if (countdownItem.AffectedStock.Trim().Length > 0)
            {
                Info = countdownItem.AffectedStock.Trim();
            }
            string[] InfoArray = Info.Substring(0, Info.Length - 1).Split(';');
            Hashtable InfoHt = new Hashtable(10);
            for (int i = 0; i < InfoArray.Length; i++)
            {
                InfoHt.Add(InfoArray[i].Split(':')[0], InfoArray[i].Split(':')[1]);
            }
            foreach (string key in InfoHt.Keys)
            {
                object item = InfoHt[key];
                InventoryStockEntity instocklist = SecKillDA.GetInventoryStock(countdownItem.ProductSysNo, int.Parse(key));

                int AffectedVirtualQty;
                if (countdownItem.IsLimitedQty == 1)
                {
                    AffectedVirtualQty = instocklist.AvailableQty + instocklist.ConsignQty
                       + instocklist.VirtualQty - Util.TrimIntNull(item);
                    if (AffectedVirtualQty < 0)
                    {
                        AffectedVirtualQty = 0;
                    }
                }
                else
                {
                    AffectedVirtualQty = 0;
                }
                bool bSuccess = SetInventorStockVirtualQty(int.Parse(key), countdownItem.ProductSysNo
                    , (-1) * AffectedVirtualQty, snapShotOnlineQty, Util.TrimIntNull(item));
                if (!bSuccess)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 运行完毕，归还总仓和分仓的虚拟仓库为活动前的虚拟仓库数量
        /// </summary>
        /// <param name="countDown"></param>
        private static void BatchUpdateStockVirtual_AfterRun(SecKillEntity countDown)
        {
            string snapShotCurrentVirtualQty = null;
            if ((!string.IsNullOrWhiteSpace(countDown.SnapShotCurrentVirtualQty)))
            {
                if (countDown.SnapShotCurrentVirtualQty.Trim().Length > 0)
                {
                    snapShotCurrentVirtualQty = countDown.SnapShotCurrentVirtualQty.Trim();
                }
                string[] InfoArray = snapShotCurrentVirtualQty.Substring(0, snapShotCurrentVirtualQty.Length - 1).Split(';');

                Hashtable InfoHt = new Hashtable(10);
                for (int i = 0; i < InfoArray.Length; i++)
                {
                    InfoHt.Add(InfoArray[i].Split(':')[0], string.IsNullOrWhiteSpace(InfoArray[i].Split(':')[1]) ? "0" : InfoArray[i].Split(':')[1]);//注：InfoHt(stocksysno,virtualQty)
                }
                foreach (string key in InfoHt.Keys)
                {
                    object VirtualQty = InfoHt[key];
                    SecKillDA.UpdateInventory_Stock(Convert.ToInt32(VirtualQty), countDown.ProductSysNo, int.Parse(key));
                }
            }
            SecKillDA.UpdateInventory(countDown.AffectedVirtualQty, countDown.ProductSysNo);
        }

        /// <summary>
        /// 增加分仓支持
        /// </summary>
        /// <param name="StockSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <param name="affectedVirtualQty"></param>
        public static bool SetInventorStockVirtualQty(int StockSysNo, int productSysNo, int affectedVirtualQty
               , string snapShotOnlineQty, int stockqty)
        {
            InitInventory(productSysNo);
            bool updateMoreRow = InventoryStockUpdateVirtualQty(productSysNo, StockSysNo, affectedVirtualQty
                , snapShotOnlineQty, stockqty);
            return updateMoreRow;
        }

        private static bool InventoryStockUpdateVirtualQty(int productSysNo, int StockSysNo, int affectedVirtualQty
               , string snapShotOnlineQty, int stockqty)
        {         
            string[] stockOnlineQty = snapShotOnlineQty.Split(';');
            for (int i = 0; i < stockOnlineQty.Length; i++)
            {
                string _stock = "";
                int _Onlineqty = 0;
                if (stockOnlineQty[i] != "")
                {
                    _stock = stockOnlineQty[i].Split(':')[0];
                    int.TryParse(stockOnlineQty[i].Split(':')[1], out _Onlineqty);
                    if (_stock == StockSysNo.ToString())
                    {
                        if (_Onlineqty >= 0 && stockqty >= 0)
                        {
                            int n = SecKillDA.InventoryStockUpdateVirtualQty(productSysNo, StockSysNo, affectedVirtualQty);
                            return n > 0 ? true : false;
                        }
                        else
                        {
                            WriteLog("OnlineQty<0------------->productSysNo, StockSysNo, affectedVirtualQty：" + productSysNo + "-" + StockSysNo + "-" + affectedVirtualQty);
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 设置虚拟库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="qty"></param>
        public static bool SetVirtualQty(int productSysNo, int affectedVirtualQty)
        {
            //初始化商品的库存信息
            InitInventory(productSysNo);

            if (!InventoryUpdateVirtualQty(productSysNo, affectedVirtualQty))
            {
                return false;
            }
            return true;
        }

        private static bool InventoryUpdateVirtualQty(int productSysNo, int affectedVirtualQty)
        {
            int n = SecKillDA.InventoryUpdateVirtualQty(productSysNo, affectedVirtualQty);
            return n > 0 ? true : false;
        }

        /// <summary>
        /// 新建商品的时候需要初始化库存, 
        /// 但是有的商品是以前建立的，目前商品转换的时候计算成本也需要初始化库存。
        /// </summary>
        /// <param name="productSysNo"></param>
        private static void InitInventory(int productSysNo)
        {
            int n = SecKillDA.CountInventoryByProductSysNo(productSysNo);

            if (n <= 0)
            {
                SecKillDA.InitInventory(productSysNo);
            }

            List<InventoryEntity> inventorylist = new List<InventoryEntity>();
            inventorylist = SecKillDA.GetInventorySysNoList(productSysNo);

            if (inventorylist != null && inventorylist.Count > 0)
            {
                foreach (InventoryEntity inventory in inventorylist)
                {
                    SecKillDA.InitInventoryStock(productSysNo, inventory.InventorySysNo);
                }
            }
        }

        private static void ProductNotAutoSetVirtualInsert(ProductNotAutoSetVirtualEntity entity)
        {
            if (!IsExistsNotAutoSetVirtual(entity.ProductSysNo))
            {
                //更新总仓,分仓虚库数量
                //SecKillDA.UpdateInventoryVirtualQty(entity.ProductSysNo);
                //插入记录
                SecKillDA.InsertProduct_NotAutoSetVirtual(entity);
            }
            else
            {
                if (!IsExistsNotAutoSetVirtual(entity.ProductSysNo, entity.NotAutoSetVirtualType))
                {
                    //插入记录
                    SecKillDA.InsertProduct_NotAutoSetVirtual(entity);
                }
                else
                {
                    throw new BusinessException("已存在同类型的有效的记录!商品编号：" + entity.ProductSysNo);
                }
            }
        }

        private static void IsCanRunningCountDown(List<InventoryStockEntity> Invstocklist, SecKillEntity countdownItem, out string curVirtualQty)
        {
            if (string.IsNullOrWhiteSpace(countdownItem.AffectedStock))
            {
                curVirtualQty = null;
                return;
            }
            curVirtualQty = "";
            string[] stock = countdownItem.AffectedStock.Split(';');
            for (int i = 0; i < stock.Length; i++)
            {
                string _stock = "";
                int _qty = 0;
                if (stock[i] != "")
                {
                    _stock = stock[i].Split(':')[0];
                    int.TryParse(stock[i].Split(':')[1], out _qty);
                    foreach (InventoryStockEntity stockitem in Invstocklist)
                    {
                        if (stockitem.StockSysNo.ToString() == _stock)
                        {
                            curVirtualQty += _stock + ":" + stockitem.VirtualQty + ";";                          
                        }
                    }
                }
            }          
        }

        public static void SetVirtualQty_CountDownFinish(SecKillEntity countDown, int SysNo)
        {
            if (SysNo != -999 && SysNo > 0)
            {
                ProductNotAutoSetVirtualEntity entity = new ProductNotAutoSetVirtualEntity();

                entity.SysNo = SysNo;
                entity.ProductSysNo = countDown.ProductSysNo;
                entity.Note = "CountDown  Interupted/Finished";
                entity.Status = -1;
                entity.AbandonTime = DateTime.Now;
                entity.AbandonUserSysNo = 493;
                entity.CountDownSysNo = countDown.SysNo;
                SecKillDA.ProductNotAuto_SetVirtualUpdate(entity);
                //BatchUpdateStockVirtual_AfterRun(countDown);
                //ProductNotAutoSetVirtualUpdate(entity, countDown);
            }
        }

        public static void SetVirtualQty_WhenCountDownAbandon(SecKillEntity countDown, int SysNo)
        {
            if (SysNo != -999 && SysNo > 0)
            {
                ProductNotAutoSetVirtualEntity entity = new ProductNotAutoSetVirtualEntity();

                entity.SysNo = SysNo;
                entity.ProductSysNo = countDown.ProductSysNo;
                entity.Note = "CountDown  Abandon";
                entity.Status = -1;
                entity.AbandonTime = DateTime.Now;
                entity.AbandonUserSysNo = 493;
                entity.CountDownSysNo = countDown.SysNo;

                //作废禁设虚库记录
                SecKillDA.ProductNotAuto_SetVirtualUpdate(entity);
                //更新总仓,分仓虚库数量
                //SecKillDA.UpdateInventoryVirtualQty(countDown.ProductSysNo);
                //SecKillDA.AotuSetStockVirtualQty(countDown.ProductSysNo);
            }
        }

        private static void ProductNotAutoSetVirtualUpdate(ProductNotAutoSetVirtualEntity entity
            , SecKillEntity oCountdown)
        {
            int ProductSysNo = entity.ProductSysNo;
            //作废禁设虚库记录
            SecKillDA.ProductNotAuto_SetVirtualUpdate(entity);

            if (!IsExistsNotAutoSetVirtual(ProductSysNo))
            {
                //更新总仓,分仓虚库数量
                //自动设虚库 20091214和shadow确认：先进行自动设虚库，在更改inventory的虚拟库存
                //SecKillDA.AotuSetStockVirtualQty(ProductSysNo);

                //把51+配置里的分仓的虚拟库存之和更新到inventory表的虚拟库存上
                //SecKillDA.Update_Inventory_VirtualQty(ProductSysNo);
            }
        }

        private static bool IsExistsNotAutoSetVirtual(int ProductSysNo)
        {
            try
            {
                if (ProductSysNo > 0)
                {
                    int num = SecKillDA.CountProduct_NotAutoSetVirtual(ProductSysNo);
                    return num > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private static bool IsExistsNotAutoSetVirtual(int ProductSysNo, int NotAutoSetVirtualType)
        {
            try
            {
                if (ProductSysNo > 0)
                {
                    int num = SecKillDA.CountProduct_NotAutoSetVirtual2(ProductSysNo, NotAutoSetVirtualType);
                    return num > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public static void GetInventoryInfoToWrite(int ProductSysNo, string CurrentStatus)
        {
            int AffectedVirtualQty = 0;
            InventoryEntity inventory = SecKillDA.GetInventoryByProductSysNo(ProductSysNo);
            if (inventory != null)
            {
                AffectedVirtualQty = inventory.VirtualQty;
            }

            //获取当前的分仓的虚库数量
            List<InventoryStockEntity> Invstocklist = SecKillDA.GetInventoryStock(ProductSysNo);
            StringBuilder stockvirtualqty = new StringBuilder();
            foreach (InventoryStockEntity itemStock in Invstocklist)
            {
                stockvirtualqty.Append(string.Format("{0}:{1};", itemStock.StockSysNo, itemStock.VirtualQty));
            }

            string msg = DateTime.Now.ToString() + "  商品：" + ProductSysNo.ToString() + "   " + CurrentStatus + " \r\n\t Inventory--VirtualQty: " + AffectedVirtualQty + "\r\n InventoryStock--VirtualQty： " + stockvirtualqty;

            WriteLog(msg);

            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                SecKillDA.SendMailAboutInventoryInfo(msg, ProductSysNo, CurrentStatus);
            }
        }

        public static void SendExceptionInfoEmail(string ErrorMsg)
        {
            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                SecKillDA.SendMailAboutExceptionInfo(ErrorMsg);
            }
        }
        #endregion
        private static void WriteConsoleInfo(string content)
        {
            Console.WriteLine(content);
            if (context != null)
            {
                context.Message += content +"\r\n";
            }
        }

        private static void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
        }
    }
}

