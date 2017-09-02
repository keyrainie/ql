using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Transactions;
using IPP.MktToolMgmt.GroupBuyingJob.BusinessEntities;
using IPP.MktToolMgmt.GroupBuyingJob.Dac;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Core.WebReference;
/**********************************************
1.1	当“当前时间”大于等于“团购开始时间”并且状态为就绪状态，
 *  则将“就绪”状态调整为“运行”状态，并将当前第一阶梯价格修改到Product_Price表中，   
 * 将积分与返现清0，如果每单限购数量>0则同时更新限购数量。
 * 同时镜像当前新蛋价格，积分，返现，限购数量。更新Product_Ex表中PromotionType为’GB’。
 * 调价LOG,邮件发送
1.2	当“当前时间”大于等于“团购结束时间”并且状态为运行状态，
 * 则将“运行”状态调整为“完成”状态，并将镜像的新蛋价格，积分，返现，限购数量更新到Product_Price表中。
 * 更新Product_Ex表中PromotionType为NULL。
 * 调价LOG,邮件发送
1.3	开始前一小时发现原价小于等于团购价，则发邮件提示用户团购信息异常，
1.4	如果为当前时间发现原价小于等于团购价，直接作废该团购信息，则发邮件提示用户团购信息作废。
 * 阶梯销售的商品，只要有1个阶梯价格高于了原价，就遵照该逻辑处理；
1.5	读取西安提供SP获取已支付并未作废状态属于该团购活动的订单中所有商品的售出数量，
 * 并将售出数量更新到团购表中的CurrentSellCount，根据CurrentSellCount读取阶梯价格。 * 
 * ********************************************/

namespace IPP.MktToolMgmt.GroupBuyingJob.Biz
{
    public class GroupBuyingBP
    {
        private static string BizLogFile;
        public static JobContext jobContext = null;

        #region Check团购信息

        public static void CheckGroupBuying(string bizLogFile)
        {
            BizLogFile = bizLogFile;
            WriteLog("\r\n------------------- Begin-------------------------");

            // 获取团购中秒杀商品的记录
            List<ProductGroupBuyingEntity> groupBuyingItemList = GroupBuyingDA.GetProductGroupBuyingList();
            string startmsg = "获取到团购的记录，共" + groupBuyingItemList.Count + "条.";
            WriteLog(startmsg);

            if (groupBuyingItemList.Count == 0)
            {
                return;
            }
            StringBuilder startNum = new StringBuilder();
            StringBuilder finishNum = new StringBuilder();
            StringBuilder abandonNum = new StringBuilder();

            foreach (ProductGroupBuyingEntity groupBuyingEntity in groupBuyingItemList)
            {
                string info = string.Format("团购SysNo|产品SysNo|状态-->{0}|{1}|{2}\r\n",
                    groupBuyingEntity.SysNo, groupBuyingEntity.ProductSysNo, groupBuyingEntity.Status);

                //如果团购状态为就绪状态且“团购开始时间”为“当前时间”前一小时
                if (groupBuyingEntity.Status == "P"
                    && (groupBuyingEntity.BeginDate > DateTime.Now && DateTime.Now.AddHours(1) >= groupBuyingEntity.BeginDate)
                    && groupBuyingEntity.ProductStatus == 1)
                {
                    try
                    {
                        CheckGroupReadToRun(groupBuyingEntity, false);
                        continue;
                    }
                    catch (Exception exp)
                    {
                        SendExceptionInfoEmail(exp.Message.ToString());
                        WriteLog(string.Format("{0}就绪->原价判断 出错了!异常:{1}", info, exp.Message.ToString()));
                        continue;
                    }

                }

                //如果团购状态为就绪状态且当“当前时间”大于等于“团购开始时间”并且“结束时间”小于“当前时间”
                if (groupBuyingEntity.Status == "P"
                    && (groupBuyingEntity.BeginDate <= DateTime.Now && groupBuyingEntity.EndDate > DateTime.Now)
                    && groupBuyingEntity.ProductStatus == 1)
                {

                    try
                    {
                        if (CheckGroupReadToRun(groupBuyingEntity, true))
                        {
                            SetRunning(groupBuyingEntity);
                            startNum.AppendLine(info);
                        }

                        continue;
                    }
                    catch (Exception exp)
                    {
                        SendExceptionInfoEmail(exp.Message.ToString());
                        WriteLog(string.Format("{0}就绪->运行 出错了!异常:{1}", info, exp.Message.ToString()));
                        continue;
                    }
                }

                //如果团购状态为就绪状态并且“结束时间”小于“当前时间”则作废
                if (groupBuyingEntity.Status == "P"
                    && groupBuyingEntity.EndDate < DateTime.Now)
                {
                    try
                    {
                        SetAbandon(groupBuyingEntity);
                        abandonNum.AppendLine(info);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        SendExceptionInfoEmail(ex.Message.ToString());
                        WriteLog("作废过期团购 出错了![" + info + "]" + "异常:" + ex.Message.ToString());
                        continue;
                    }
                }

                //如果团购状态为运行状态
                if (groupBuyingEntity.Status == "A")
                {
                    //当“当前时间”大于等于“团购结束时间”并且状态为运行状态
                    if (groupBuyingEntity.EndDate < DateTime.Now || groupBuyingEntity.ProductStatus!=1)
                    {
                        try
                        {
                            //将“运行”状态调整为“完成”状态
                            SetFinish(groupBuyingEntity);
                            finishNum.AppendLine(info);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            SendExceptionInfoEmail(ex.Message.ToString());
                            WriteLog("运行->结束 出错了![" + info + "]" + "异常:" + ex.Message.ToString());
                            continue;
                        }
                    }
                    else
                    {
                        try
                        {
                            SetStepPrice(groupBuyingEntity);
                            continue;

                        }
                        catch (Exception ex)
                        {
                            SendExceptionInfoEmail(ex.Message.ToString());
                            WriteLog("运行->调价 出错了![" + info + "]" + "异常:" + ex.Message.ToString());
                            continue;
                        }
                    }
                }


            }
            string endMsg = "本轮运行结果：\r\n就绪->运行:\r\n" + startNum.ToString() + "\r\n就绪->作废：\r\n"
                                  + abandonNum.ToString() + "\r\n运行->完成：\r\n" + finishNum.ToString();
            WriteLog(endMsg);
            WriteLog("------------------- End-----------------------------\r\n");
            //WriteConsoleInfo(endMsg);
        }

        #endregion

        #region 就绪-->运行Check

        private static bool CheckGroupReadToRun(ProductGroupBuyingEntity groupBuyingItem, bool isAbandon)
        {
            //如果团购类型为抽奖（6），则不需下列操作
            if (groupBuyingItem.GroupBuyingTypeSysNo == 6) { return true; }

            //读取原价
            decimal originalPrice = GroupBuyingDA.GetOriginalPrice(groupBuyingItem.ProductSysNo, groupBuyingItem.IsByGroup);
            decimal groupBuyingPrice = 0;
            List<ProductGroupBuying_PriceEntity> gbPriceList = GroupBuyingDA.GetProductGroupBuying_PriceList(groupBuyingItem);

            bool result = true;

            foreach (ProductGroupBuying_PriceEntity gbPrice in gbPriceList)
            {
                if (originalPrice <= gbPrice.GroupBuyingPrice)
                {
                    groupBuyingPrice = gbPrice.GroupBuyingPrice;
                    result = false;
                }
            }

            //Check不通过需要发送邮件
            if (!result)
            {
                string mailtype = "";

                if (isAbandon)
                {
                    mailtype = "已被系统自动作废";
                    SetAbandon(groupBuyingItem);
                }
                else
                {
                    mailtype = "一小时后将被系统自动作废，请修改团购信息";
                }

                string mailTo = GroupBuyingDA.GetUserEmailByUserName(groupBuyingItem.InUser);
                string mailSubject = string.Format("团购商品{0}Item# " + groupBuyingItem.ProductID, mailtype);


                StringBuilder mailInfo = new StringBuilder();

                mailInfo.AppendLine("Hi 创建人、PMCC：");
                mailInfo.AppendLine(string.Format("   商品" + groupBuyingItem.ProductID + "的团购价" + groupBuyingPrice.ToString("0.00") + "大于等于了原价" + originalPrice.ToString("0.00") + "，{0}。", mailtype));
                mailInfo.AppendLine("该邮件由系统自动发出，请勿回复！");
                mailInfo.AppendLine("其中" + groupBuyingPrice.ToString("0.00") + "为团购价；" + originalPrice.ToString("0.00") + "为原价；");


                GroupBuyingDA.SendMailAbandonGroupBuyingInfo(mailTo, mailInfo.ToString(), mailSubject);

            }

            return result;
        }

        #endregion

        #region SetRunning
        /// <summary>
        /// 就绪-->运行
        /// </summary>
        /// <param name="groupBuyingItem"></param>
        /// <returns></returns>
        private static bool SetRunning(ProductGroupBuyingEntity groupBuyingItem)
        {


            if (groupBuyingItem.Status != "P")
            {
                throw new BusinessException("不是就绪状态");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                List<ProductPriceInfoEntity> priceInfoList = GroupBuyingDA.GetProductPriceInfoList(groupBuyingItem.ProductSysNo, groupBuyingItem.IsByGroup);

                groupBuyingItem.Status = "A";

                if (groupBuyingItem.GroupBuyingTypeSysNo != 6)
                {
                    #region 如果团购类型为抽奖（6），则不需下列操作
                    ProductGroupBuying_PriceEntity gbPrice = GroupBuyingDA.GetProductGroupBuying_PriceList(groupBuyingItem).OrderBy(e => e.SellCount).ToList()[0];

                    //设置原价
                    groupBuyingItem.OriginalPrice = GroupBuyingDA.GetOriginalPrice(groupBuyingItem.ProductSysNo, groupBuyingItem.IsByGroup);

                    //设置团购为原始价格并记录团购最低阶价格
                    foreach (ProductPriceInfoEntity priceinfo in priceInfoList)
                    {

                        ProductGroupBuying_SnapShotPriceEntity snapShotPrice = new ProductGroupBuying_SnapShotPriceEntity();
                        snapShotPrice.SnapShotCashRebate = priceinfo.CashRebate;
                        snapShotPrice.SnapShotCurrentPrice = priceinfo.CurrentPrice;
                        snapShotPrice.SnapShotMaxPerOrder = priceinfo.MaxPerOrder;
                        snapShotPrice.SnapShotPoint = priceinfo.Point;
                        snapShotPrice.ProductSysNo = priceinfo.ProductSysNo;
                        snapShotPrice.ProductGroupBuyingSysNo = groupBuyingItem.SysNo;
                        snapShotPrice.SnapshotBasicPrice = priceinfo.BasicPrice;

                        //插入快照价格
                        GroupBuyingDA.CreateSnapShotPrice(snapShotPrice);


                        ProductPriceInfoEntity itemPrice = new ProductPriceInfoEntity();
                        itemPrice = GroupBuyingDA.LoadItemPrice(priceinfo.ProductSysNo);
                        itemPrice.CashRebate = 0;
                        itemPrice.BasicPrice = groupBuyingItem.OriginalPrice;
                        itemPrice.CurrentPrice = gbPrice.GroupBuyingPrice;
                        itemPrice.Point = 0;
                        

                        groupBuyingItem.BasicPrice = itemPrice.BasicPrice;

                        //判断MaxPerOrder是否输入
                        if (groupBuyingItem.MaxPerOrder > 0)
                        {
                            itemPrice.MaxPerOrder = groupBuyingItem.MaxPerOrder;
                        }

                        //修改价格
                        //GroupBuyingDA.UpdateItemPrice(itemPrice);
                        GroupBuyingDA.UpdateItemPrice(itemPrice, groupBuyingItem, groupBuyingItem.InDate, "IPPSystemAdmin", DateTime.Now, "团购调价就绪-->运行", "JobConsole", "GroupBuying");

                        //修改商品信息促销类型为团购“GB”
                        GroupBuyingDA.UpdateProductEx(itemPrice.ProductSysNo, "GB");

                        //验证一个规则内差价是否小于0
                        if (groupBuyingItem.ProductSysNo != 0)
                        {
                            CheckSaleRule(groupBuyingItem.ProductSysNo);
                        }


                        //类型PriceLogType：记录为“限时促销调价”；
                        //申请时间CreateDate：dbo.ProductGroupBuying .InDate；
                        //生效时间UpdateDate：dbo.ProductGroupBuying .BeginDate；
                        //申请人CreateUser：InUser；
                        //审核人UpdateUser：dbo.ProductGroupBuying．Audituser；
                        //商品原价OldPrice：SnapShotCurrentPrice；
                        //调后价格NewPrice：GroupBuyingCurrentPrice；
                        //调整幅度offset：调后价格减去商品原价减去积分
                        //返现金额CashRebate：GroupBuyingCashRebate；
                        //积分Point： GroupBuyingPoint

                        //2011-12-1 删除-该功能移入SP Rik.K.Li
                        //decimal offset = 0;
                        //decimal point = (0 - snapShotPrice.SnapShotPoint) / 10m;
                        //offset = itemPrice.CurrentPrice - snapShotPrice.SnapShotCurrentPrice - point;

                        //string userName = groupBuyingItem.InUser;

                        //GroupBuyingDA.InsertGroupBuyingProductPricechangeLog(itemPrice.ProductSysNo.ToString(),
                        //                                                 gbPrice.GroupBuyingPrice.ToString(),
                        //                                                 snapShotPrice.SnapShotCurrentPrice.ToString(),
                        //                                                 offset.ToString(),
                        //                                                 itemPrice.UnitCost,
                        //                                                 0,
                        //                                                 0,
                        //                                                 groupBuyingItem.InUser,
                        //                                                 groupBuyingItem.InDate,
                        //                                                 groupBuyingItem.AuditUser,
                        //                                                 groupBuyingItem.BeginDate,
                        //                                                 "团购调价就绪-->运行",
                        //                                                 "JobConsole",
                        //                                                 "GroupBuying");


                    }
                    #endregion
                }


                GroupBuyingDA.UpdateProductGroupBuyingRun(groupBuyingItem);
                groupBuyingItem.Reasons = "";
                SyncGroupBuyingStatus(groupBuyingItem);

                ts.Complete();
            }
            return true;
        }

        #endregion

        #region SetAbandon
        /// <summary>
        /// 作废过期未运行的记录
        /// </summary>
        /// <param name="GroupBuyingSysNo"></param>
        private static void SetAbandon(ProductGroupBuyingEntity entity)
        {
            if (entity.Status != "P")
            {
                throw new BusinessException("the current status not allow such opertion");
            }

            entity.Status = "D";

            GroupBuyingDA.UpdateGroupBuyingAbandon(entity);
            entity.Reasons = "GroupBuyingJob自动作废";
            SyncGroupBuyingStatus(entity);
        }

        #endregion

        #region SetFinish
        /// <summary>
        /// 运行-->结束
        /// </summary>
        /// <param name="GroupBuyingSysNo"></param>
        public static void SetFinish(ProductGroupBuyingEntity groupBuyingItem)
        {

            //必须是Running           
            if (groupBuyingItem.Status != "A")
            {
                throw new BusinessException("the current status not allow such opertion");
            }
            else
            {
                //设置阶梯价格
                SetStepPrice(groupBuyingItem);

            }

            groupBuyingItem = GroupBuyingDA.GetGroupBuyingItemBySysno(groupBuyingItem.SysNo);

            groupBuyingItem.Status = "F";

            decimal gbCurentPrice = 0;

            List<ProductGroupBuying_SnapShotPriceEntity> snapShotList = GroupBuyingDA.GetSnapShotPriceList(groupBuyingItem.SysNo);

            using (TransactionScope ts = new TransactionScope())
            {
                foreach (ProductGroupBuying_SnapShotPriceEntity snapShot in snapShotList)
                {

                    ProductPriceInfoEntity itemPrice = GroupBuyingDA.LoadItemPrice(snapShot.ProductSysNo);

                    gbCurentPrice = itemPrice.CurrentPrice;

                    itemPrice.BasicPrice = snapShot.SnapshotBasicPrice;
                    itemPrice.CurrentPrice = snapShot.SnapShotCurrentPrice;
                    itemPrice.CashRebate = snapShot.SnapShotCashRebate;
                    itemPrice.Point = snapShot.SnapShotPoint;
                    itemPrice.MaxPerOrder = snapShot.SnapShotMaxPerOrder;


                    //GroupBuyingDA.UpdateItemPrice(itemPrice);
                    GroupBuyingDA.UpdateItemPrice(itemPrice, groupBuyingItem, groupBuyingItem.InDate, "IPPSystemAdmin", DateTime.Now, "团购调价就绪-->运行", "JobConsole", "GroupBuying");

                    GroupBuyingDA.UpdateProductEx(snapShot.ProductSysNo, string.Empty);


                    //验证一个规则内差价是否小于0
                    if (snapShot != null && snapShot.ProductSysNo != 0)
                    {
                        CheckSaleRule(snapShot.ProductSysNo);
                    }

                    //decimal offset = 0;
                    //decimal point = (itemPrice.Point - 0) / 10m;
                    //offset = itemPrice.CurrentPrice - gbCurentPrice - point;

                    //类型PriceLogType：记录为“限时促销调价”；
                    //申请时间CreateDate：dbo.ProductGroupBuying .CreateTime；
                    //生效时间UpdateDate: EndTime；
                    //申请人CreateUser：dbo.ProductGroupBuying．Createusersysno对应的username；
                    //审核人UpdateUser：dbo.ProductGroupBuying．Audituser；
                    //商品原价OldPrice：GroupBuyingCurrentPrice；
                    //调后价格NewPrice：SnapShotCurrentPrice；
                    //调整幅度offset：调后价格减去商品原价；
                    //返现金额CashRebate：SnapShotCashRebate；


                    //2011-12-1 删除-该功能移入SP Rik.K.Li
                    //GroupBuyingDA.InsertGroupBuyingProductPricechangeLog(itemPrice.ProductSysNo.ToString(),
                    //                                                 snapShot.SnapShotCurrentPrice.ToString(),
                    //                                                 gbCurentPrice.ToString(),
                    //                                                 offset.ToString(),
                    //                                                 itemPrice.UnitCost,
                    //                                                 snapShot.SnapShotCashRebate,
                    //                                                 snapShot.SnapShotPoint,
                    //                                                 groupBuyingItem.InUser,
                    //                                                 groupBuyingItem.InDate,
                    //                                                 groupBuyingItem.AuditUser,
                    //                                                 groupBuyingItem.EndDate,
                    //                                                 "团购调价运行-->结束",
                    //                                                 "JobConsole",
                    //                                                 "GroupBuying");

                }


                if (groupBuyingItem.SuccessDate != null)
                {
                    groupBuyingItem.DealPrice = gbCurentPrice;
                }
                else
                {
                    groupBuyingItem.DealPrice = 0;
                }

                GroupBuyingDA.UpdateProductGroupBuyingFinish(groupBuyingItem);
                SyncGroupBuyingStatus(groupBuyingItem);


                ts.Complete();
            }

            

        }


        #endregion

        #region SetStepPrice

        /// <summary>
        /// 运行-->根据阶梯调价
        /// </summary>
        /// <param name="groupBuyingEntity"></param>
        public static void SetStepPrice(ProductGroupBuyingEntity groupBuyingItem)
        {
            //必须是Running           
            if (groupBuyingItem.Status != "A")
            {
                throw new BusinessException("the current status not allow such opertion");
            }

            using (TransactionScope ts = new TransactionScope())
            {

                //读取已成功支付的正常状态的订单商品数量
                int orderNumber = 0;

                if (groupBuyingItem.GroupBuyingTypeSysNo == 6)
                {//如果类型为抽奖
                    orderNumber = GroupBuyingDA.GetCurrentSellCountForLottery(groupBuyingItem.SysNo, groupBuyingItem.IsByGroup);
                }
                else
                {
                    orderNumber = GroupBuyingDA.GetCurrentSellCount(groupBuyingItem.SysNo, groupBuyingItem.IsByGroup);
                    groupBuyingItem.CurrentSellCount = orderNumber;
                    SyncGroupBuyingSellCount(groupBuyingItem);
                }

                //根据商品数量读取价格
                List<ProductGroupBuying_PriceEntity> gbPriceList = GroupBuyingDA.GetProductGroupBuying_PriceList(groupBuyingItem).OrderBy(e => e.SellCount).ToList();

                decimal stepPrice = 0;
                string isSuccess = "N";

                int maxSellCount = gbPriceList.Max(e => e.SellCount);

                for (int i = 0; i < gbPriceList.Count; i++)
                {
                    ProductGroupBuying_PriceEntity gbPrice = gbPriceList[i];
                    if (i == 0) stepPrice = gbPrice.GroupBuyingPrice;

                    if (orderNumber >= gbPrice.SellCount)
                    {
                        isSuccess = "Y";
                        stepPrice = gbPrice.GroupBuyingPrice;
                    }
                }

                //如果团购人数大于最高成团人数，则更新结算状态为C
                string isSettlement = "N";
                decimal dealPrice = 0;

                if (orderNumber >= maxSellCount)
                {
                    isSettlement = "C";
                    dealPrice = gbPriceList.Min(e => e.GroupBuyingPrice);

                }

                GroupBuyingDA.UpdateCurrentSellCount(groupBuyingItem.SysNo, orderNumber, isSettlement, dealPrice);


                //只有在最小成团时间为空且团购已成功或者最小成团时间不为空且最小成团失败才更新
                if (!(groupBuyingItem.SuccessDate != null && isSuccess == "Y"))
                {
                    //设置最小成团时间
                    GroupBuyingDA.SetGroupBuyingSuccesDate(groupBuyingItem.SysNo, isSuccess);
                }

                //如果团购类型为抽奖（6），则不需下列操作
                if (groupBuyingItem.GroupBuyingTypeSysNo != 6)
                {
                    List<ProductPriceInfoEntity> itemPriceList = GroupBuyingDA.GetProductPriceInfoList(groupBuyingItem.ProductSysNo, groupBuyingItem.IsByGroup);

                    foreach (ProductPriceInfoEntity itemPrice in itemPriceList)
                    {
                        //更新团购成功时间
                        if (itemPrice.CurrentPrice != stepPrice)
                        {

                            decimal gbCurentPrice = itemPrice.CurrentPrice;
                            itemPrice.CurrentPrice = stepPrice;
                            //修改价格
                            //GroupBuyingDA.UpdateItemPrice(itemPrice);
                            GroupBuyingDA.UpdateItemPrice(itemPrice, groupBuyingItem, groupBuyingItem.InDate, "IPPSystemAdmin", DateTime.Now, "团购调价就绪-->运行", "JobConsole", "GroupBuying");

                            //验证一个规则内差价是否小于0
                            if (groupBuyingItem.ProductSysNo != 0)
                            {
                                CheckSaleRule(groupBuyingItem.ProductSysNo);
                            }

                            //类型PriceLogType：记录为“团购调价运行”；
                            //申请时间CreateDate：dbo.ProductGroupBuying .InDate；
                            //生效时间UpdateDate：Now；
                            //申请人CreateUser：InUser；
                            //审核人UpdateUser：dbo.ProductGroupBuying．Audituser；
                            //商品原价OldPrice：SnapShotCurrentPrice；
                            //调后价格NewPrice：GroupBuyingCurrentPrice；
                            //调整幅度offset：调后价格减去商品原价减去积分
                            //返现金额CashRebate：GroupBuyingCashRebate；
                            //积分Point： GroupBuyingPoint


                            //2011-12-1 删除-该功能移入SP Rik.K.Li
                            //decimal offset = 0;
                            //decimal point = 0;
                            //offset = itemPrice.CurrentPrice - gbCurentPrice - point;

                            //GroupBuyingDA.InsertGroupBuyingProductPricechangeLog(itemPrice.ProductSysNo.ToString(),
                            //                                                 itemPrice.CurrentPrice.ToString(),
                            //                                                 gbCurentPrice.ToString(),
                            //                                                 offset.ToString(),
                            //                                                 itemPrice.UnitCost,
                            //                                                 0,
                            //                                                 0,
                            //                                                 groupBuyingItem.InUser,
                            //                                                 groupBuyingItem.InDate,
                            //                                                 groupBuyingItem.AuditUser,
                            //                                                 DateTime.Now,
                            //                                                 "团购调价运行-->阶梯价格",
                            //                                                 "JobConsole",
                            //                                                 "GroupBuying");


                        }
                    }
                }
                ts.Complete();
            }
        }

        #endregion

        #region 团购开始，结束 后都需要验证销售规则内差价是否小于0
        /// <summary>
        /// 团购开始，结束 后都需要验证销售规则内差价是否小于0
        /// </summary>
        /// <param name="item"></param>
        private static void CheckSaleRule(int productSysNo)
        {
            //IMaintainSaleRuleV31 service = ServiceBroker.FindService<IMaintainSaleRuleV31>();
            try
            {
                //SaleRuleV31 saleRule = new SaleRuleV31();
                //saleRule.Body = new SaleRuleMsg();
                //saleRule.Body.ProductSysNo = productSysNo;
                //saleRule.Header = new Newegg.Oversea.Framework.Contract.MessageHeader();
                //saleRule.Header.CompanyCode = AppConfig.CompanyCode;
                //saleRule.Header.OperationUser = AppConfig.GetOperationUser();
                //service.CheckSaleRulePrice(saleRule);


                string baseUrl = System.Configuration.ConfigurationManager.AppSettings["MKTRestFulBaseUrl"];
                string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
                string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
                ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
                ECCentral.Job.Utility.RestServiceError error;
                var ar = client.Create("/Combo/CheckComboPriceAndSetStatus", productSysNo, out error);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
            finally
            {
               //ServiceBroker.DisposeService(service);
            }
        }
        #endregion

        public static void SendExceptionInfoEmail(string ErrorMsg)
        {
            bool sendmailflag = Convert.ToBoolean(ConfigurationManager.AppSettings["SendMailFlag"]);
            if (sendmailflag == true)
            {
                GroupBuyingDA.SendMailAboutExceptionInfo(ErrorMsg);
            }
        }

        private static void WriteConsoleInfo(string content)
        {
            Console.WriteLine(content);
        }

        private static void WriteLog(string content)
        {
            // Log.WriteLog(content, BizLogFile);

            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
            if (jobContext != null)
            {
                jobContext.Message += content + "\r\n";
            }
        }

        private static void SyncGroupBuyingStatus(ProductGroupBuyingEntity entity)
        {
            if (entity.GroupBuyingTypeSysNo != 6&&entity.RequestSysNo>0)
            {
                GroupBuyingDA.SyncGroupBuyingStatus(entity);
            }
        }

        private static void SyncGroupBuyingSellCount(ProductGroupBuyingEntity entity)
        {
            if (entity.GroupBuyingTypeSysNo != 6 && entity.RequestSysNo > 0)
            {
                GroupBuyingDA.SyncGroupBuyingSellCount(entity.RequestSysNo,entity.CurrentSellCount);
            }
        }
    }
}

