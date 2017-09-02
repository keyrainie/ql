using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Inventory;
using System.Transactions;
using ECCentral.Service.MKT.BizProcessor.Promotion.Processors;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.Resources;
using System.IO;
using System.Data;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
//using ECCentral.Service.MKT.BizProcessor.PromotionEngine;
namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(CountdownProcessor))]
    public class CountdownProcessor : IPromotionActivityJob
    {
        //积分与钱的兑换比例
        private decimal pointExchangeMoneyRate = ExternalDomainBroker.GetPointExChangeRate();
        //private CountdownPromotionEngine _countdownPromotionEngine = ObjectFactory<CountdownPromotionEngine>.Instance;

        /// <summary>
        /// 创建限时抢购
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CountdownInfo CreateCountdown(CountdownInfo entity)
        {
            string errorMsg = string.Empty;
            if (entity == null)
                return null;
            if (entity.PMRole == 1)
            {
                //查询该用户是否有该商品的产品线权限
                var isRight = ObjectFactory<ICountdownDA>.Instance.CheckCreatePermissions(entity.ProductSysNo.Value, ServiceContext.Current.UserSysNo);
                if (!isRight)
                {
                    //throw new BizException(string.Format("{0} 对不起，您没有该商品的产品线权限。", entity.ProductID));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Countdown","Countdown_SorryYouNotHaveAuthorityFormat"), entity.ProductID));
                }
            }
            if (entity.IsPromotionSchedule.HasValue && entity.IsPromotionSchedule.Value)//创建促销计划
            {
                CreatePromotionSchedule(entity);
            }
            else//创建限时抢购
            {
                //1.验证该限时抢购的有效性
                RequiredCountdown4CreateValidate(entity);
                CheckProductPrice(entity);

                TransactionScopeFactory.TransactionAction(() =>
                {
                    //2.如果选择了预留库存的话，那应该通知仓库预留库存
                    if (entity.IsReservedQty.HasValue && entity.IsReservedQty.Value)   //1=预留；0不预留
                    {
                        SetInventoryReservedQty(entity);
                    }
                    //3.设置各分仓对应的库存数据
                    SetWareHouseList(entity);
                    //4.记录活动创建时的快照，这个步骤为了和ipp数据一致，其实是没有意义的
                    ProductInfo product = ExternalDomainBroker.GetProductInfo(entity.ProductSysNo.Value);
                    entity.SnapShotCashRebate = product.ProductPriceInfo.CashRebate;
                    entity.SnapShotCurrentPrice = product.ProductPriceInfo.CurrentPrice;
                    entity.SnapShotPoint = product.ProductPriceInfo.Point;

                    //5.让该限时抢购持久化
                    ObjectFactory<ICountdownDA>.Instance.CreateCountdown(entity);

                    //6.将Countdown活动保存到PromotionEngine配置库中
                    //_countdownPromotionEngine.SaveActivity(entity);


                });
            }
            //记录日志
            // string msg = "创建{0}: ProductSysNo--Status--IsPromotionSchedule--IsLimitedQty--IsReservedQty--CountDownCurrentPrice--CountDownQty：{0}--{1}--{2}--{3}--{4}--{5}--{6}";
            // ExternalDomainBroker.CreateOperationLog(string.Format(msg, entity.ProductSysNo, entity.Status, (entity.IsPromotionSchedule.Value ? "产品促销计划" : "限时抢购"), entity.IsLimitedQty, entity.IsReservedQty, entity.CountDownCurrentPrice, entity.CountDownQty), BizEntity.Common.BizLogType.Countdown_Insert, entity.SysNo.Value, entity.CompanyCode);

            string msg = ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_Create") + ":ProductSysNo--Status--IsPromotionSchedule--IsLimitedQty--IsReservedQty--CountDownCurrentPrice--CountDownQty：{0}--{1}--{2}--{3}--{4}--{5}--{6}";
            ExternalDomainBroker.CreateOperationLog(string.Format(msg, entity.ProductSysNo, entity.Status, (entity.IsPromotionSchedule.Value ? ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_GoodsPromoPlan") : ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_Countdown")), entity.IsLimitedQty, entity.IsReservedQty, entity.CountDownCurrentPrice, entity.CountDownQty), BizEntity.Common.BizLogType.Countdown_Insert, entity.SysNo.Value, entity.CompanyCode);
            return entity;
        }
        /// <summary>
        /// 检查限时抢购价格不能低于捆绑销售折扣价格
        /// </summary>
        /// <param name="entity"></param>
        private void CheckProductPrice(CountdownInfo entity)
        {
            if (ObjectFactory<ICountdownDA>.Instance.CheckSaleRuleDiscount(entity))
            {
                //throw new BizException("限时抢购价格不能低于捆绑销售折扣价格，请先处理对应Valid状态的捆绑销售折扣价格");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownNeedMoreThanDiscount"));
            }
        }


        #region 2012-10-22  修改UpdateCountdown该方法
        /*
         *原因:解决TFS上BUG 96041(限时抢购就绪状态做修改，提交审核后，数据未保存数据库，数据未生效)
         * 修改内容：去掉了判断是提交审核还是保存逻辑
         * 目的:使得提交审核同样走更新逻辑,这样才会将客户端更改数据后点击提交审核将新数据保存在数据库中
         */
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public CountdownInfo UpdateCountdown(CountdownInfo entity)
        {
            if (entity == null)
            {
                return null;
            }
            CheckProductPrice(entity);
            string msg = "{0}SystemNumber--ProductSysNo--Status--IsPromotionSchedule--IsLimitedQty--IsReservedQty--CountDownCurrentPrice--CountDownQty：{1}--{2}--{3}--{4}--{5}--{6}--{7}--{8}";
            string newmsg = string.Empty;

            CountdownInfo oldEntity = Load(entity.SysNo);
            if (oldEntity != null)
            {
                // if (entity.IsSubmitAudit.HasValue && !entity.IsSubmitAudit.Value) 
                //{
                RequiredCountdownUpdateValidate(entity, oldEntity);
                //  }
                msg = string.Format(msg, ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_OlderRecord"), oldEntity.SysNo, oldEntity.ProductSysNo, oldEntity.Status, oldEntity.IsPromotionSchedule, oldEntity.IsPromotionSchedule, oldEntity.IsLimitedQty, oldEntity.IsReservedQty, oldEntity.CountDownCurrentPrice, oldEntity.CountDownQty);

                if (oldEntity.Status != CountdownStatus.Init
                    && oldEntity.Status != CountdownStatus.VerifyFaild
                    && oldEntity.Status != CountdownStatus.Ready)
                {
                    // throw new BizException("当前记录状态已经发生变化，请关闭页面重新打开！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ReOpenPage"));
                }

                if (oldEntity.Status == CountdownStatus.Ready)
                {
                    bool result = ObjectFactory<GroupBuyingProcessor>.Instance.CheckGroupBuyConflict(entity.ProductSysNo.Value, entity.StartTime.Value, entity.EndTime.Value);
                    if (result)
                    {
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "GroupBuying_Duplicate"));
                    }
                }
            }


            TransactionScopeFactory.TransactionAction(() =>
            {
                if (oldEntity != null)
                {
                    //if (entity.IsSubmitAudit.HasValue && !entity.IsSubmitAudit.Value)
                    //{
                    if (!CheckRunningLimitedItem(entity))
                    {
                        oldEntity.CompanyCode = entity.CompanyCode;
                        if (entity.IsReservedQty.HasValue && entity.IsReservedQty.Value && oldEntity.IsReservedQty.HasValue && oldEntity.IsReservedQty.Value)  // 前一次预留和更改后的预留的数量不一致
                        {
                            ReturnInventoryReservedQty2AvailableQty(oldEntity);
                            SetInventoryReservedQty(entity);
                        }
                        else if (entity.IsReservedQty.HasValue && entity.IsReservedQty.Value && oldEntity.IsReservedQty.HasValue && !oldEntity.IsReservedQty.Value)   //1=预留；0不预留
                        {
                            SetInventoryReservedQty(entity);
                        }
                        else
                        {
                            if (oldEntity.IsReservedQty.HasValue && oldEntity.IsReservedQty.Value && entity.IsReservedQty.HasValue && !entity.IsReservedQty.Value)
                            {
                                ReturnInventoryReservedQty2AvailableQty(entity);
                            }
                        }
                    }
                    //  }
                }
                //if (entity.IsSubmitAudit.HasValue && !entity.IsSubmitAudit.Value)
                // {
                SetWareHouseList(entity);
                if (entity.IsPromotionSchedule.HasValue && entity.IsPromotionSchedule.Value)
                {
                    ObjectFactory<ICountdownDA>.Instance.MaintainPromotionSchedule(entity);
                }
                else
                {
                    ObjectFactory<ICountdownDA>.Instance.MaintainCountdown(entity);
                    if (oldEntity != null)
                    {
                        if (oldEntity.RequestSysNo > 0 && oldEntity.Status != entity.Status)
                        {
                            SyncCountdownStatus(oldEntity.RequestSysNo, (int)entity.Status, entity.VerifyMemo);
                        }
                    }
                }
                //将数据保存到PromotionEngine配置库中
                //_countdownPromotionEngine.SaveActivity(entity);
                //newmsg = string.Format(msg, "新记录", entity.SysNo, entity.ProductSysNo, entity.Status, entity.IsPromotionSchedule, entity.IsPromotionSchedule, entity.IsLimitedQty, entity.IsReservedQty, entity.CountDownCurrentPrice, entity.CountDownQty);
                newmsg = string.Format(msg, ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NewRecord"), entity.SysNo, entity.ProductSysNo, entity.Status, entity.IsPromotionSchedule, entity.IsPromotionSchedule, entity.IsLimitedQty, entity.IsReservedQty, entity.CountDownCurrentPrice, entity.CountDownQty);
                ExternalDomainBroker.CreateOperationLog(msg + "\r\n" + newmsg, BizEntity.Common.BizLogType.Countdown_Update, entity.SysNo.Value, entity.CompanyCode);
                //  }
                entity.IsSubmitAudit = entity.IsSubmitAudit ?? false;
                if (entity.IsSubmitAudit.Value)
                {
                    ObjectFactory<ICountdownDA>.Instance.MaintainCountdownStatus(entity);
                    //ExternalDomainBroker.CreateOperationLog("促销计划" + entity.SysNo + "提交审核.", BizEntity.Common.BizLogType.Countdown_Update, entity.SysNo.Value, entity.CompanyCode);
                    ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_PromoPlan") + entity.SysNo + ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_SubExam"), BizEntity.Common.BizLogType.Countdown_Update, entity.SysNo.Value, entity.CompanyCode);
                }
            });
            return entity;
        }

        public CountdownInfo AbandonCountdown(CountdownInfo entity)
        {
            if (entity == null) { return null; }

            CountdownInfo oldEntity = Load(entity.SysNo);

            if (oldEntity != null)
            {
                if (oldEntity.Status != CountdownStatus.Init
                    && oldEntity.Status != CountdownStatus.Ready
                    && oldEntity.Status != CountdownStatus.VerifyFaild)
                {
                    // throw new BizException("当前记录状态已经发生变化，请关闭页面重新打开！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ReOpenPage"));
                }

                TransactionScopeFactory.TransactionAction(() =>
                {
                    #region rule
                    //如果该商品有正在限量运行记录，则作废不返还预留库存。
                    //运行结束时，预留库存=所有状态为初始状态，就绪状态，待审核状态的预留库存之和
                    //可用库存+=原预留库存-现在的预留库存(所有状态为初始状态，就绪状态，待审核状态的预留库存之和)
                    //如果不符合以上条件，则直接作废并返还预留库存给可用库存 
                    #endregion
                    if (oldEntity.IsReservedQty.HasValue && oldEntity.IsReservedQty.Value)
                    {
                        if (!CheckRunningLimitedItem(oldEntity))
                        {
                            ReturnInventoryReservedQty2AvailableQty(oldEntity);
                        }
                    }
                    oldEntity.Status = CountdownStatus.Abandon;
                    ObjectFactory<ICountdownDA>.Instance.MaintainCountdownStatus(oldEntity);

                    SyncCountdownStatus(oldEntity.RequestSysNo, (int)oldEntity.Status, "");


                    //更新PromotionEngine配置中活动的状态
                    //_countdownPromotionEngine.UpdateActivityStatus(entity.SysNo.Value, CountdownStatus.Abandon);

                    // ExternalDomainBroker.CreateOperationLog("作废记录" + entity.SysNo, BizEntity.Common.BizLogType.Countdown_Abandon, entity.SysNo.Value, entity.CompanyCode);
                    ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_DeledRecord") + entity.SysNo, BizEntity.Common.BizLogType.Countdown_Abandon, entity.SysNo.Value, entity.CompanyCode);

                });
            }
            return entity;
        }

        public CountdownInfo InterruptCountdown(CountdownInfo entity)
        {
            if (entity == null)
            {
                return null;
            }
            RequiredInterruptCountdownValidate(entity);
            CountdownInfo oldEntity = Load(entity.SysNo);
            //必须是运行状态才能够被中止
            if (oldEntity.Status != CountdownStatus.Running)
            {
                //throw new BizException("当前记录状态已经发生变化，请关闭页面重新打开！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ReOpenPage"));
            }
            else
            {
                oldEntity.EndTime = DateTime.Now;
                TransactionScopeFactory.TransactionAction(() =>
                {
                    MainTainCountdownEndTime(oldEntity);

                    //更新PromotionEngine配置中活动的状态
                    //_countdownPromotionEngine.UpdateActivityStatus(entity.SysNo.Value, CountdownStatus.Interupt);

                    #region 取消中止逻辑
                    //using (TransactionScope ts = new TransactionScope())
                    //{
                    //    if (oldEntity.IsSecondKill.HasValue && oldEntity.IsSecondKill.Value)//如果是秒杀的话
                    //    {
                    //        ExternalDomainBroker.UpdateProductPromotionType(oldEntity.ProductSysNo.Value, string.Empty);
                    //    }
                    //    ////作废禁设虚库记录
                    //    //ExternalDomainBroker.UpdateProductNotAutoSetVirtual(oldEntity.ProductSysNo.Value, -1, "Countdown interrupt");
                    //    ////回滚价格
                    //    ExternalDomainBroker.RollBackPriceWhenCountdownInterrupted(oldEntity);
                    //    //CountdownDA.UpdateItemPrice(oldEntity, oldEntity.CreateDate, oldEntity.AuditUser, DateTime.Now, "限时促销调价运行-->终止", "CountDown");

                    //    //回滚库存
                    //    GetWareHouseList(oldEntity);//计算完成后分仓预留库存和总仓预留库存都是除本商品外，其它商品的总和了
                    //    oldEntity.AffectedStockList.ForEach(item =>
                    //    {
                    //        ProductInventoryInfo inventory = ExternalDomainBroker.GetProductInventoryInfoByStock(oldEntity.ProductSysNo.Value, item.StockSysNo.Value);
                    //        //扣除当前商品的预留库存 ： 当前产品的预留库存=当前产品在仓库里的预留库存- 当前产品其它活动的预留库存
                    //        ExternalDomainBroker.SetProductReservedQty(oldEntity.ProductSysNo.Value, item.StockSysNo.Value, -(inventory.ReservedQty - item.Qty.Value));
                    //    });

                    //    oldEntity.Status = CountdownStatus.Interupt;
                    //    ObjectFactory<ICountdownDA>.Instance.MaintainCountdownStatus(oldEntity);

                    //    ts.Complete();
                    //}
                    #endregion
                    //ExternalDomainBroker.CreateOperationLog("中止记录" + entity.SysNo, BizEntity.Common.BizLogType.Countdown_Interupt, entity.SysNo.Value, entity.CompanyCode);
                    ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_StopedRecord") + entity.SysNo, BizEntity.Common.BizLogType.Countdown_Interupt, entity.SysNo.Value, entity.CompanyCode);
                });
                return entity;
            }
        }

        public void VerifyCountdown(CountdownInfo entity)
        {

            if (entity == null)
            {
                return;
            }
            //if (ObjectFactory<ICountdownDA>.Instance.CheckUser((int)entity.SysNo, ServiceContext.Current.UserSysNo))
            //{
            //    throw new BizException("审核人和创建人不能相同!");
            //}
            RequiredVerifyCountdownValidate(entity);
            //并发操作的控制
            string msg = string.Empty;
            CountdownInfo oldEntity = Load(entity.SysNo.Value);


            TransactionScopeFactory.TransactionAction(() =>
            {
                if (oldEntity != null)
                {
                    oldEntity.CompanyCode = entity.CompanyCode;
                    entity.CreateUserSysNo = oldEntity.CreateUserSysNo;
                    if (oldEntity.Status != CountdownStatus.WaitForVerify && oldEntity.Status != CountdownStatus.WaitForPrimaryVerify)
                    {
                        //throw new BizException("当前记录状态已经发生变化，请关闭页面重新打开！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ReOpenPage"));
                    }
                    else
                    {
                        if (entity.Status == CountdownStatus.WaitForVerify || entity.Status == CountdownStatus.WaitForPrimaryVerify)
                        {
                            bool result = ObjectFactory<GroupBuyingProcessor>.Instance.CheckGroupBuyConflict(entity.ProductSysNo.Value, entity.StartTime.Value, entity.EndTime.Value);
                            if (result)
                            {
                                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "GroupBuying_Duplicate"));
                            }
                        }

                        CountdownStatus syncstatus = entity.Status.Value;

                        if (oldEntity.RequestSysNo > 0 && entity.Status.Value == CountdownStatus.VerifyFaild)
                        {
                            entity.Status = CountdownStatus.Abandon;
                        }
                        // msg = "原纪录状态:" + oldEntity.Status.ToString() + " 新记录状态：" + entity.Status;
                        msg = ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_OldRecordStatus") + oldEntity.Status.ToString() + ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NewRecordStatus") + entity.Status;
                        if (entity.IsPromotionSchedule.HasValue && !entity.IsPromotionSchedule.Value)
                        {
                            //如果是现时抢购，则只改状态
                            ObjectFactory<ICountdownDA>.Instance.VerifyCountdown(entity);
                        }
                        else
                        {
                            //如果是产品促销，则审核会更改其他更改信息
                            oldEntity.Status = entity.Status;
                            oldEntity.VerifyMemo = entity.VerifyMemo;
                            ObjectFactory<ICountdownDA>.Instance.VerifyPromotionSchedule(oldEntity);
                            entity = oldEntity;
                        }
                        SyncCountdownStatus(oldEntity.RequestSysNo, (int)syncstatus, entity.VerifyMemo);
                        entity.ProductID = oldEntity.ProductID;
                        entity.Status = syncstatus;
                    }
                }

                //更新PromotionEngine配置中活动的状态
                //_countdownPromotionEngine.UpdateActivityStatus(entity.SysNo.Value, entity.Status.Value);

                //SendMail(entity);

                //ExternalDomainBroker.CreateOperationLog("审核操作" + msg, BizEntity.Common.BizLogType.Countdown_Verify, entity.SysNo.Value, entity.CompanyCode);
                ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ExamOperation") + msg, BizEntity.Common.BizLogType.Countdown_Verify, entity.SysNo.Value, entity.CompanyCode);
               
            });


        }
        /// <summary>
        /// 创建促销计划
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private CountdownInfo CreatePromotionSchedule(CountdownInfo entity)
        {
            entity.Status = CountdownStatus.Init;
            RequiredCountdown4CreateValidate(entity);
            ProductInfo product = ExternalDomainBroker.GetProductInfo(entity.ProductSysNo.Value);
            if (entity.Status != CountdownStatus.WaitForPrimaryVerify
                && entity.CountDownCurrentPrice > product.ProductPriceInfo.UnitCost)
            {
                //如果是产品促销，则未提交之前的状态为初始状态
                entity.Status = CountdownStatus.Init;
            }
            TransactionScopeFactory.TransactionAction(() =>
            {
                if (entity.IsReservedQty.HasValue && entity.IsReservedQty.Value)   //1=预留；0不预留&& !CheckRunningLimitedItem(entity)
                {
                    SetInventoryReservedQty(entity);
                }
                SetWareHouseList(entity);

                ObjectFactory<ICountdownDA>.Instance.CreatePromotionSchedule(entity);

                //更新PromotionEngine配置中活动的状态
                //_countdownPromotionEngine.UpdateActivityStatus(entity.SysNo.Value, entity.Status.Value);

            });
            return entity;
        }

        /// <summary>
        /// 添加前的验证
        /// </summary>
        /// <param name="entity"></param>
        private void RequiredCountdown4CreateValidate(CountdownInfo entity)
        {
            // Check if there are some missed required fields when create TopicReply item.
            StringBuilder builder = new StringBuilder();
            if (entity == null)
            {
                //throw new BizException("待保存的记录为空");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NullRecord"));
            }
            else
            {

                if (entity.Status == CountdownStatus.Ready)
                {
                    bool result = ObjectFactory<GroupBuyingProcessor>.Instance.CheckGroupBuyConflict(entity.ProductSysNo.Value, entity.StartTime.Value, entity.EndTime.Value);
                    if (result)
                    {
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "GroupBuying_Duplicate"));
                    }
                }

                //新需求预留数量引起的问题，改动如下：
                //如果Item已经在限时抢购表中，并且status in （0，1,-5）不能再添加该商品的限时抢购记录
                List<CountdownInfo> resultlist = GetCountDownByProductSysNo(entity.ProductSysNo.Value);
                if (resultlist != null)
                {    //时间段不能重复
                    var dateRepeatCount = (from old in resultlist
                                           where ((entity.StartTime >= old.StartTime && entity.StartTime <= old.EndTime)
                                               || (entity.EndTime <= old.EndTime && entity.EndTime >= old.StartTime)
                                               || (entity.StartTime <= old.StartTime && entity.EndTime >= old.EndTime))
                                               && (old.Status == CountdownStatus.Ready
                                                       || old.Status == CountdownStatus.WaitForPrimaryVerify
                                                       || old.Status == CountdownStatus.Running
                                                       || old.Status == CountdownStatus.Init)
                                           select old).Count();
                    if (dateRepeatCount > 0)
                    {
                        //throw new BizException("在限时抢购记录中，该商品已在同一时间段有记录（待审核，就绪，运行），不能重复添加");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownExsistSameProduct"));
                    }
                    //如果该商品有正在运行的记录, 只能添加不预留的记录, 不管运行的记录时限量还是不限量
                    int RepeatItem = (from item in resultlist
                                      where (item.Status == CountdownStatus.Running && entity.IsReservedQty.HasValue && entity.IsReservedQty.Value)
                                      select item).Count();

                    if (RepeatItem > 0)
                    {
                        // throw new BizException("该商品已经在“限时抢购”/“产品促销计划”中设置了相冲突时间段的价格规则，无法重复添加");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_PriceRuleConflict"));
                    }
                }

                //Check该商品不能为团购记录
                List<int> productSysNos = new List<int>();
                productSysNos.Add(entity.ProductSysNo.Value);
                if (ObjectFactory<IGroupBuyingDA>.Instance.CheckConflict(null, productSysNos, entity.StartTime.Value, entity.EndTime.Value))
                {
                    //修改提示  该商品已经在“团购”中设置了相冲突时间段的团购信息，无法重复添加
                    //throw new BizException("该商品已经在“团购”中设置了相冲突时间段的团购信息，无法重复添加");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_GroupBuyConflict"));

                }

                if (entity.StartTime >= entity.EndTime)
                {
                    //throw new BizException("限时抢购的开始时间不可等于结束时间");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownStartTimeCanEqualEndTime"));
                }
                //在限时抢购专区显示,开始时间~结束时间必须小于等于72小时
                else if (entity.EndTime.Value.AddSeconds(-3 * 24 * 3600).CompareTo(entity.StartTime) >= 0) //精确到秒
                {
                    //throw new BizException("在限时抢购专区显示,开始时间~结束时间必须小于等于72小时,且不可相同");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownTimeLessThanThreeDays"));
                }
                CheckEndTime(entity);
                //检查和产品有关的规则
                CheckProduct(entity);

                if (string.IsNullOrEmpty(entity.Reasons))
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownReasonsIsEmpty"));
                }

                //商品的毛利率与库龄阶段的高级毛利率比较，小于库龄高级毛利率则需要提交审批
                CheckMarginRate(entity);
                if ((entity.IsLimitedQty.HasValue && entity.IsLimitedQty.Value) || (entity.IsReservedQty.HasValue && entity.IsReservedQty.Value))
                {
                    CheckWareHouseInfo(entity, null);
                }
            }
        }
        private void RequiredCountdownUpdateValidate(CountdownInfo entity, CountdownInfo oldEntity)
        {
            // Check if there are some missed required fields when create TopicReply item.
            StringBuilder builder = new StringBuilder();
            if (entity == null)
            {
                //throw new BizException("待保存的记录为空!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NullRecord"));
            }
            else
            {
                if (entity.SysNo <= 0)
                {
                    // throw new BizException("请提供必要的内容:系统编号!");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ApplySysNo"));
                }

                if (entity.StartTime >= entity.EndTime)
                {
                    //throw new BizException("限时抢购的开始时间不可等于结束时间");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownStartTimeCanEqualEndTime"));
                }
                //在限时抢购专区显示,开始时间~结束时间必须小于等于72小时
                if ((entity.IsPromotionSchedule.HasValue && !entity.IsPromotionSchedule.Value) && (entity.IsCountDownAreaShow.HasValue && entity.IsCountDownAreaShow.Value)
                    && entity.EndTime.Value.AddSeconds(-3 * 24 * 3600).CompareTo(entity.StartTime) >= 0) //精确到秒
                {
                    //throw new BizException("在限时抢购专区显示,开始时间~结束时间必须小于等于72小时,且不可相同");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownTimeLessThanThreeDays"));

                }

                List<CountdownInfo> resultlist = GetCountDownByProductSysNo(entity.ProductSysNo.Value);
                if (resultlist != null)
                {    //时间段不能重复
                    var dateRepeatCount = (from old in resultlist
                                           where ((entity.StartTime >= old.StartTime && entity.StartTime <= old.EndTime)
                                                   || (entity.EndTime <= old.EndTime && entity.EndTime >= old.StartTime)
                                                   || (entity.StartTime <= old.StartTime && entity.EndTime >= old.EndTime)
                                                 )
                                                && (old.Status == CountdownStatus.Ready
                                                       || old.Status == CountdownStatus.WaitForVerify
                                                       || old.Status == CountdownStatus.WaitForPrimaryVerify
                                                       || old.Status == CountdownStatus.Running
                                                       || old.Status == CountdownStatus.Init)
                                                 && (old.SysNo != entity.SysNo)
                                           select old).Count();
                    if (dateRepeatCount > 0)
                    {
                        //throw new BizException("在限时抢购记录中，该商品已在同一时间段有记录（待审核，就绪，运行），不能重复添加");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownExsistSameProduct"));

                    }
                }
                CheckEndTime(entity);
                //检查和产品有关的规则
                CheckProduct(entity);
                //商品的毛利率与库龄阶段的高级毛利率比较，小于库龄高级毛利率则需要提交审批
                CheckMarginRate(entity);

                if ((entity.IsLimitedQty.HasValue && entity.IsLimitedQty.Value) || (entity.IsReservedQty.HasValue && entity.IsReservedQty.Value))
                {
                    CheckWareHouseInfo(entity, null);
                }
            }
        }

        private void RequiredInterruptCountdownValidate(CountdownInfo entity)
        {
            // Check if there are some missed required fields when create TopicReply item.
            StringBuilder builder = new StringBuilder();
            if (entity == null)
            {
                //throw new BizException("待保存的记录为空!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NullRecord"));
            }
            else
            {
                if (entity.SysNo <= 0)
                {
                    // throw new BizException("请提供下列必要信息的内容：记录编号!");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ApplySysNo"));
                }
                if (entity.Status != CountdownStatus.Running)
                {
                    //throw new BizException("当前状态不支持此操作。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NotSupportThisOperation"));
                }
            }
        }

        private void RequiredVerifyCountdownValidate(CountdownInfo entity)
        {
            // Check if there are some missed required fields when create TopicReply item.
            StringBuilder builder = new StringBuilder();
            if (entity == null)
            {
                //throw new BizException("待保存的记录为空");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NullRecord"));
            }
            else
            {
                //entity.Status == CountdownStatus.Ready || entity.Status == CountdownStatus.VerifyFaild)
                if (entity.SysNo == 0)
                {
                    //throw new BizException("当前状态不支持此操作");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NotSupportThisOperation"));
                }
                if (entity.Status == 0)
                {
                    CheckEndTime(entity);
                }
            }
        }

        private void CheckEndTime(CountdownInfo entity)
        {
            if (entity.EndTime <= DateTime.Now)
            {
                // throw new BizException("在限时抢购专区显示,当前时间必须小于抢购结束时间");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CurrentTimeLessThanEndTime"));
            }
        }

        /// <summary>
        /// 检查分仓库存信息
        /// </summary>
        /// <param name="entity"></param>
        private void CheckInvenory(CountdownInfo entity)
        {
            foreach (var item in entity.AffectedStockList)
            {
                int productSysNo = (int)entity.ProductSysNo;
                int stockSysNo = (int)item.StockSysNo;
                ProductInventoryInfo inventory = ExternalDomainBroker.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
                if (item.Qty > (inventory.AvailableQty + inventory.ConsignQty + inventory.VirtualQty + inventory.ReservedQty))
                {
                    //throw new BizException(string.Format("{0}预留库存大于分仓总库存（可用库存+代销库存+虚拟库存）", inventory.StockInfo.StockName));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ObligateMoreThanAll"), inventory.StockInfo.StockName));
                }
            }

        }
        private void CheckProduct(CountdownInfo entity)
        {
            decimal saleGiftAmount = 0;
            decimal couponAmount = 0;
            if (!entity.ProductSysNo.HasValue)
            {
                //throw new BizException("请选择一个商品");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_SelectProduct"));
            }
            if (entity.ProductSysNo != null)
            {
                saleGiftAmount = ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGiftCurrentAmountForVendor(entity.ProductSysNo.Value);
                couponAmount = ObjectFactory<GrossMarginProcessor>.Instance.GetCouponCurrentAmountForPM(entity.ProductSysNo.Value);
            }
            ProductInfo product = ExternalDomainBroker.GetProductInfo(entity.ProductSysNo.Value);
            ProductInventoryInfo inventory = ExternalDomainBroker.GetProductTotalInventoryInfo(entity.ProductSysNo.Value);
            /*限时抢购时商品状态必须有效：Status=1 */
            if (product == null || inventory == null)
            {
                // throw new BizException("请选择一个商品");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_SelectProduct"));
            }
            else
            {
                if (product.ProductStatus != ProductStatus.Active)
                {
                    //throw new BizException("产品状态无效。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_InvalidStatus"));
                }
                //检查库存
                if (((entity.IsPromotionSchedule.HasValue && !entity.IsPromotionSchedule.Value)
                    || ((entity.IsPromotionSchedule.HasValue && entity.IsPromotionSchedule.Value) && ((entity.IsLimitedQty.HasValue && entity.IsLimitedQty.Value) || entity.IsReservedQty.HasValue && entity.IsReservedQty.Value))
                    )
                    && entity.CountDownQty > (inventory.AvailableQty + inventory.ConsignQty + inventory.VirtualQty + inventory.ReservedQty))
                {
                    //throw new BizException("抢购数量不得大于剩余库存数量(可用库存+代销库存+虚拟库存)");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownQuntityLessStock"));
                }

            }
            #region "check 勾选预留库存 分仓限时抢购数量不能大于剩余库存数量(可用库存+代销库存+虚拟库存)"

            if (entity.IsReservedQty.HasValue && entity.IsReservedQty.Value)
            {
                CheckInvenory(entity);
            }
            #endregion

            if (entity.IsPromotionSchedule.HasValue && !entity.IsPromotionSchedule.Value)//限时抢购的check，促销计划不用这块
            {
                // CountdownCurrentPrice-cashrebate-point/10<UnitCost            
                //低于产品成本！毛利小于0（毛利太小）
                if (entity.Status != CountdownStatus.WaitForPrimaryVerify
                      && (entity.CountDownCurrentPrice < 0
                      || (entity.CountDownCurrentPrice - entity.CountDownPoint / pointExchangeMoneyRate - saleGiftAmount - couponAmount < product.ProductPriceInfo.UnitCost
                          && entity.CountDownCurrentPrice >= entity.CountDownPoint
                      )))
                {
                    //throw new BizException("限时抢购价低于成本，需要PMD审核！请设置为提交审核后再次添加！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_PriceLessCost"));
                }

                //售价-成本价>成本 需提交审核 （毛利太大的情况）
                if (entity.Status != CountdownStatus.WaitForPrimaryVerify
                    && (entity.CountDownCurrentPrice.HasValue
                    && (entity.CountDownCurrentPrice - entity.CountDownPoint / pointExchangeMoneyRate - product.ProductPriceInfo.UnitCost - saleGiftAmount - couponAmount) > product.ProductPriceInfo.UnitCost))
                {
                    //throw new BizException("毛利大于该商品的成本价！请提交审核！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_GrossProfitThanCost"));
                }

                // point/10 > Countdownprice
                //item的送积分已经超出该产品的积分上限！
                if (entity.Status != CountdownStatus.WaitForPrimaryVerify
                    && (entity.CountDownPoint.HasValue && entity.CountDownPoint / pointExchangeMoneyRate > product.ProductPriceInfo.UnitCost
                        && entity.CountDownCurrentPrice < entity.CountDownPoint))
                {
                    //throw new BizException("item的送积分已经超出产品的积分上限！请设置为提交审核后再次添加！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_OverMaxScore"));
                }

                //限时抢购价格低于产品成本！item的送积分已经超出该产品的积分上限！请设置为提交审核后再次添加！
                #region 2012-11-09 update
                //修改原因:解决bug92022 将检查积分逻辑改为 当前限时抢购价格>当前限时抢购积分/10
                //修改类容:entity.CountDownCurrentPrice - entity.CountDownPoint / pointExchangeMoneyRate < product.ProductPriceInfo.UnitCost && entity.CountDownCurrentPrice < entity.CountDownPoint
                //修改后:entity.CountDownCurrentPrice<entity.CountDownPoint/10
                #endregion
                if (entity.Status != CountdownStatus.WaitForPrimaryVerify
                && (entity.CountDownCurrentPrice < 0
                    || (entity.CountDownCurrentPrice < entity.CountDownPoint / 10)))
                {
                    //throw new BizException("限时抢购价格低于产品成本!item的送积分已经超出该产品的积分上限！请设置为提交审核后再次添加！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_PriceLessCostAndOverMaxScore"));
                }
            }
            if (ObjectFactory<ICountdownDA>.Instance.HasDuplicateProduct(entity))
            {
                // throw new BizException("不能在同一时间段为同一产品添加限时抢购活动。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CanntJoinCountdown"));
            }
        }

        private void CheckMarginRate(CountdownInfo entity)
        {
            if (!entity.ProductSysNo.HasValue)
            {
                // throw new BizException("请选择一个商品");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_SelectProduct"));
            }

            //计算毛利率
            decimal marginRate = 0;
            decimal unitcost = ExternalDomainBroker.GetProductInfo(entity.ProductSysNo.Value).ProductPriceInfo.UnitCost;
            //分母不能小于等于0
            if (entity.CountDownCurrentPrice.Value - entity.CountDownPoint.Value * 0.1m <= 0)
            {
                //throw new BizException("积分金额不能大于等于促销价格!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_PiontMoreThanPromotion"));
            }
            if (entity.ProductSysNo != null && entity.CountDownCurrentPrice != null && entity.CountDownPoint != null)
                marginRate = Math.Round(
                    ObjectFactory<GrossMarginProcessor>.Instance.GetCurrentGrossMarginForCountdown(entity.CountDownCurrentPrice.Value, unitcost, entity.CountDownPoint.Value, entity.ProductSysNo.Value)
                                        / (entity.CountDownCurrentPrice.Value - entity.CountDownPoint.Value * (1 / pointExchangeMoneyRate))
                                        * 100 - 0.0005m, 2);
            decimal marginRatePoint = marginRate / 100.0m;
            //如果不提交审核，
            if (entity.Status == 0)
            {
                //取库龄的毛利率值与毛利率比较
                if (marginRatePoint <= ObjectFactory<GrossMarginProcessor>.Instance.GetStockSeniorGrossMarginRate(entity.ProductSysNo.Value))
                {
                    //throw new BizException("商品毛利率小于库龄对应的毛利率下限需要提交审核！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_RateOfMarginLess"));
                }

            }
            if (ObjectFactory<ICountdownDA>.Instance.HasDuplicateProduct(entity))
            {
                //throw new BizException("不能在同一时间段为同一产品添加限时抢购活动。");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CanntJoinCountdown"));
            }
        }

        private void CheckWareHouseInfo(CountdownInfo entity, CountdownInfo oldEntity)
        {
            //泰隆优选 不设置分仓 不验证库存
            return;
            if (entity.AffectedStockList == null || (entity.AffectedStockList != null && entity.AffectedStockList.Count == 0) && entity.IsReservedQty == true)
            {
                //throw new BizException("请设置各分仓限时抢购数量，分仓的现实抢购和为0!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_SetQuntity"));
            }
            ProductInventoryInfo inventory = ExternalDomainBroker.GetProductTotalInventoryInfo(entity.ProductSysNo.Value);// CountdownDA.GetWareHouseReservedQty(entity);
            if (inventory != null)
            {
                int totalCount = 0;
                entity.AffectedStockList.ForEach(x => { totalCount += x.Qty.Value; });
                if ((totalCount == 0 || totalCount != entity.CountDownQty) && entity.IsReservedQty == true)
                {
                    //throw new BizException("各分仓限时抢购数量总和不等于限时抢购数量!");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_Inequatity"));
                }
                if (oldEntity != null)
                {
                    if (entity.CountDownQty > 0
                        && ((oldEntity.IsReservedQty.HasValue && oldEntity.IsReservedQty.Value) && entity.CountDownQty > inventory.AvailableQty + inventory.ConsignQty + inventory.VirtualQty + oldEntity.CountDownQty)
                        || ((oldEntity.IsLimitedQty.HasValue && oldEntity.IsLimitedQty.Value) && entity.CountDownQty > inventory.AvailableQty + inventory.ConsignQty + inventory.VirtualQty))
                    {
                        //throw new BizException("限时抢购数量应大于等于0且小于等于剩余数量");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownQuntityMoreThanZero"));
                    }
                }
                else
                {
                    if (entity.CountDownQty > 0
                        && entity.CountDownQty > inventory.AvailableQty + inventory.ConsignQty + inventory.VirtualQty)
                    {
                        //throw new BizException("限时抢购数量应大于等于0且小于等于剩余数量");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownQuntityMoreThanZero"));
                    }
                }
            }
        }

        private bool CheckRunningLimitedItem(CountdownInfo entity)
        {
            //flag=true 表示存在正在运行的限量的记录
            bool flag = ObjectFactory<ICountdownDA>.Instance.CheckRunningLimitedItem(entity);
            return flag;
        }


        /// <summary>
        /// Check随心配毛利率,返回Check信息
        /// </summary>
        /// <param name="itemPriceEntity"></param>
        /// <returns></returns>
        public string CheckOptionalAccessoriesInfoMsg(CountdownInfo entity)
        {
            string returnMsgStr = string.Empty;
            StringBuilder checkMsg = new StringBuilder();
            ProductInfo productInfo = ExternalDomainBroker.GetProductInfo(entity.ProductSysNo.Value);

            ProductPriceRequestInfo priceMsg = new ProductPriceRequestInfo()
            {
                CurrentPrice = entity.CountDownCurrentPrice.Value,
                UnitCost = productInfo.ProductPriceInfo.UnitCost,
                Point = entity.CountDownPoint.Value,
                Category = productInfo.ProductBasicInfo.ProductCategoryInfo

            };
            List<ProductPromotionMarginInfo> marginList = ObjectFactory<IIMBizInteract>.Instance.GetProductPromotionMargin(
                                                priceMsg, entity.ProductSysNo.Value, "", 0m, ref returnMsgStr);
            marginList = marginList.Where(ppm => ppm.PromotionType == PromotionType.OptionalAccessories).ToList();

            foreach (var mgInfo in marginList)
            {
                //checkMsg.Append(string.Format("在随心配{0}中毛利率{1}%\r", mgInfo.ReferenceSysNo
                //    , (Decimal.Round(mgInfo.Margin, 4) * 100m).ToString("0.00")));
                checkMsg.Append(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Countdown","Countdown_GrossMarginInOptionalAccessories"), mgInfo.ReferenceSysNo
                    , (Decimal.Round(mgInfo.Margin, 4) * 100m).ToString("0.00")));
            }

            return checkMsg.ToString();
        }

        /// <summary>
        /// 获取快速选择时间列表
        /// </summary>
        /// <returns></returns>
        public List<CodeNamePair> GetQuickTimeList()
        {
            return CodeNamePairManager.GetList("MKT", "QuickTimeList");
        }

        private void GetWareHouseList(CountdownInfo entity)
        {
            //SnapShotCurrentVirtualQty 更改意义为 预留库存快照
            int totalCountdownQty = 0;
            List<CountdownInfo> entitylist = ObjectFactory<ICountdownDA>.Instance.CountItemHasReserveQtyNotRunning(entity);
            foreach (var ware in entity.AffectedStockList)
            {
                ware.Qty = 0;
            }
            if (entitylist != null && entitylist.Count > 0)
            {
                entity.AffectedStockList = new List<StockQtySetting>();
                foreach (CountdownInfo item in entitylist)
                {
                    if (item.CountDownQty.HasValue)
                    {
                        totalCountdownQty += item.CountDownQty.Value;
                    }
                    if (!string.IsNullOrEmpty(item.AffectedStock))
                    {

                        string[] strWarehouse = item.AffectedStock.Split(";".ToCharArray());
                        string[] strItem;
                        int warehouseSysNumber = 0;
                        int warehouseCountdownQty = 0;
                        for (int i = 0; i < strWarehouse.Length - 1; i++)
                        {
                            warehouseSysNumber = 0;
                            warehouseCountdownQty = 0;
                            strItem = strWarehouse[i].Split(":".ToCharArray());
                            if (entity.AffectedStockList != null && entity.AffectedStockList.Count >= strWarehouse.Length - 1)
                            {
                                foreach (var ware in entity.AffectedStockList)
                                {
                                    if (ware.StockSysNo == Convert.ToInt32(strItem[0]) && Convert.ToInt32(strItem[1]) > 0)
                                    {
                                        ware.Qty += Convert.ToInt32(strItem[1]);
                                    }
                                }
                            }
                            else
                            {
                                if (strItem.Length == 1 && int.TryParse(strItem[0], out warehouseSysNumber))
                                {
                                    entity.AffectedStockList.Add(new StockQtySetting() { StockSysNo = warehouseSysNumber, Qty = 0 });
                                }
                                else if (strItem.Length >= 2 && int.TryParse(strItem[0], out warehouseSysNumber) && int.TryParse(strItem[1], out warehouseCountdownQty))
                                {
                                    entity.AffectedStockList.Add(new StockQtySetting() { StockSysNo = warehouseSysNumber, Qty = warehouseCountdownQty });
                                }
                            }
                        }
                    }
                }
            }
            entity.CountDownQty = totalCountdownQty;
        }
        /// <summary>
        /// 为各个仓库添加预留库存
        /// </summary>
        /// <param name="entity"></param>
        private void SetInventoryReservedQty(CountdownInfo entity)
        {
            foreach (var item in entity.AffectedStockList)
            {
                if (item.Qty > 0)
                {
                    ExternalDomainBroker.SetProductReservedQty(entity.ProductSysNo.Value, item.StockSysNo.Value, item.Qty.Value);
                }
            }
        }
        /// <summary>
        /// 将各个仓库原来的预留库存还回去
        /// </summary>
        /// <param name="entity"></param>
        private void ReturnInventoryReservedQty2AvailableQty(CountdownInfo entity)
        {
            foreach (var item in entity.AffectedStockList)
            {
                if (item.Qty > 0)
                {
                    ExternalDomainBroker.SetProductReservedQty(entity.ProductSysNo.Value, item.StockSysNo.Value, -item.Qty.Value);
                }
            }
        }

        private void SetWareHouseList(CountdownInfo entity)
        {
            entity.AffectedStock = null;
            return;
            #region 泰隆优选不设置分仓信息
            StringBuilder affectedStock = new StringBuilder();
            if ((entity.IsReservedQty.HasValue && entity.IsReservedQty.Value) || (entity.IsLimitedQty.HasValue && entity.IsLimitedQty.Value))
            {
                if (entity.AffectedStockList != null
                    && entity.AffectedStockList.Count > 0)
                {
                    foreach (var item in entity.AffectedStockList)
                    {

                        affectedStock.Append(string.Format("{0}:{1};", item.StockSysNo.Value, item.Qty.Value));

                    }
                }
            }
            else
            {

                List<ProductInventoryInfo> inventoryList = ExternalDomainBroker.GetProductInventoryInfo(entity.ProductSysNo.Value);
                if (inventoryList != null)
                {
                    foreach (var item in inventoryList)
                    {
                        if (item.StockInfo != null)
                        {
                            affectedStock.Append(string.Format("{0}:{1};", item.StockInfo.SysNo, 0));
                        }
                    }
                }
                entity.CountDownQty = 0;
            }
            entity.AffectedStock = affectedStock.ToString();
            #endregion
        }

        public List<CountdownInfo> GetCountDownByProductSysNo(int productSysNo)
        {
            return ObjectFactory<ICountdownDA>.Instance.GetCountDownByProductSysNo(productSysNo);
        }

        private void SendMail(CountdownInfo entity)
        {
            string emailAddress = "";
            string mailSubject = "";

            UserInfo userInfo = ExternalDomainBroker.GetUserInfoBySysNo(entity.CreateUserSysNo.Value);

            if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.EmailAddress))
            {
                return;
            }

            if (userInfo != null)
            {
                emailAddress = userInfo.EmailAddress;
            }

            if (entity.Status == CountdownStatus.Ready)
            {
                // mailSubject = "限时抢购商品价格审核通过"; // "限时抢购商品价格审核通过";
                mailSubject = ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownProductPassExamine");

            }
            else if (entity.Status == CountdownStatus.WaitForVerify)
            {
                mailSubject = "限时抢购商品价格初级审核通过";
            }
            else if (entity.Status == CountdownStatus.VerifyFaild)
            {
                //mailSubject = "限时抢购商品价格审核未通过";// "限时抢购商品价格审核未通过";
                mailSubject = ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_CountdownProductNotPassExamine");
            }
            ProductInfo product = ExternalDomainBroker.GetProductInfo(entity.ProductSysNo.Value);

            if ((entity.CountDownCurrentPrice - entity.CountDownPoint / pointExchangeMoneyRate) < product.ProductPriceInfo.UnitCost)//"(限时抢购价格-送积分金额)<成本")
            {
                emailAddress += ";" + AppSettingManager.GetSetting("MKT", "CountDownMailGroup");
            }
            else
            {
                if (product.ProductBasicInfo != null && product.ProductBasicInfo.ProductManager != null && product.ProductBasicInfo.ProductManager.UserInfo != null)
                    emailAddress += ";" + product.ProductBasicInfo.ProductManager.UserInfo.EmailAddress;
            }

            KeyValueVariables param = new KeyValueVariables();
            decimal minmargin = ObjectFactory<GrossMarginProcessor>.Instance.GetCategoryMinGrossMarginRate(entity.ProductSysNo.Value);
            decimal margin = entity.CountDownCurrentPrice.Value - (entity.CountDownPoint.Value / pointExchangeMoneyRate) - product.ProductPriceInfo.UnitCost;
            decimal MarginCurrent = decimal.Round(margin, 2);

            param.Add("Subject", mailSubject);
            param.Add("MinMarginCurrent", MarginCurrent.ToString());
            param.Add("ProductSysno", entity.ProductSysNo.ToString());
            param.Add("MinMargin", minmargin);
            param.Add("ProductID", product.ProductID);
            param.Add("ProductName", product.ProductName);
            param.Add("CountDownPoint", entity.CountDownPoint.ToString());
            param.Add("CountDownCurrentPrice", (decimal.Round(entity.CountDownCurrentPrice.Value, 2)).ToString());
            param.Add("UnitCost", (decimal.Round(product.ProductPriceInfo.UnitCost, 2)).ToString());
            param.Add("Appaly", userInfo.UserDisplayName);
            param.Add("Year", DateTime.Now.Year);
            ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(emailAddress, "MKT_Countdown_Verify", param, true);
        }
        public void GetGrossMargin(CountdownInfo entity, out decimal GrossMargin, out decimal GrossMarginWithOutPointAndGift, out decimal GrossMarginRate, out decimal GrossMarginRateWithOutPointAndGift)
        {
            decimal unitcost = ExternalDomainBroker.GetProductInfo(entity.ProductSysNo.Value).ProductPriceInfo.UnitCost;
            //毛利
            GrossMargin = Math.Round(ObjectFactory<GrossMarginProcessor>.Instance.GetCurrentGrossMarginForCountdown(
                            entity.CountDownCurrentPrice.Value, unitcost, entity.CountDownPoint.Value, entity.ProductSysNo.Value), 2);
            //毛利率
            GrossMarginRate = Math.Round(ObjectFactory<GrossMarginProcessor>.Instance.GetCurrentGrossMarginRateForCountdown(
                            entity.CountDownCurrentPrice.Value, unitcost, entity.CountDownPoint.Value, entity.ProductSysNo.Value)
                            * 100 - 0.0005m, 2);
            //活动未设置赠积分和礼券时的毛利
            GrossMarginWithOutPointAndGift = Math.Round(ObjectFactory<IIMBizInteract>.Instance.GetProductMarginAmount(
                            entity.CountDownCurrentPrice.Value, entity.CountDownPoint.Value, unitcost), 2);
            //活动未设置赠积分和礼券时的毛利率
            GrossMarginRateWithOutPointAndGift = Math.Round(ObjectFactory<IIMBizInteract>.Instance.GetProductMargin(
                            entity.CountDownCurrentPrice.Value, entity.CountDownPoint.Value, unitcost, 0m)
                            * 100 - 0.0005m, 2);

            //Check设置各分仓限时抢购数量
            CheckWareHouseInfo(entity, null);
        }
        #region 维护类行为  全局行为

        public CountdownInfo Load(int? sysNo)
        {
            return ObjectFactory<ICountdownDA>.Instance.Load(sysNo);
        }
        public bool IsProductInSCByDateTime(int excludeSysNo, List<int> productSysNos, DateTime beginDate, DateTime endDate)
        {
            return ObjectFactory<ICountdownDA>.Instance.CheckConflict(excludeSysNo, productSysNos, beginDate, endDate);
        }

        public bool CheckGroupBuyAndCountDownConflict(List<int> productSysNos, DateTime beginDate, DateTime endDate)
        {
            return ObjectFactory<ICountdownDA>.Instance.CheckGroupBuyConflict(productSysNos, beginDate, endDate);
        }

        #endregion



        #region Job行为
        public void ActivityStatusProcess()
        {
            throw new NotImplementedException();
        }
        #endregion

        public List<UserInfo> GetAllCountdownCreateUser(string channleID)
        {
            return ObjectFactory<ICountdownDA>.Instance.GetAllCountdownCreateUser(channleID);
        }

        public void SyncCountdownStatus(int requestSysNo, int status, string reason)
        {
            if (requestSysNo > 0)
            {
                ObjectFactory<ICountdownDA>.Instance.SyncCountdownStatus(requestSysNo, status, reason);
            }
        }

        public List<ECCentral.BizEntity.PO.VendorInfo> GetCountdownVendorList()
        {
            return ObjectFactory<ICountdownDA>.Instance.GetCountdownVendorList();
        }

        private void MainTainCountdownEndTime(CountdownInfo entity)
        {
            ObjectFactory<ICountdownDA>.Instance.MainTainCountdownEndTime(entity);
        }
        #region "导入操作 Jack.G.Tang 2012-11-26"

        /// <summary>
        ///导入促销活动
        /// </summary>
        /// <param name="fileName">Execl 路径</param>
        /// <param name="IsPromotionSchedule">true:促销计划 false:限时抢购</param>
        public void ImportCountInfo(string fileName, int pmRole, bool IsPromotionSchedule)
        {
            //检查是否有创建权限
            if (pmRole == 0)
            {
                //throw new BizException("对不起，您没有产品线权限。\r\n");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_SorryYouNotHaveAuthority"));
                return;
            }
            List<string> errorList = new List<string>();
            if (FileUploadManager.FileExists(fileName))
            {

                string configPath = AppSettingManager.GetSetting("MKT", "PostCountDownFilePath");
                if (!Path.IsPathRooted(configPath))
                {
                    configPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, configPath);
                }
                string ExtensionName = FileUploadManager.GetFilePhysicalFullPath(fileName);
                string destinationPath = Path.Combine(configPath, ExtensionName);
                string folder = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                FileUploadManager.MoveFile(fileName, destinationPath);

                DataTable table = ObjectFactory<ICountdownDA>.Instance.ReadExcelFileToDataTable(destinationPath);

                ImportCountDownCheck(table, IsPromotionSchedule);
                //模板列和实体属性名称Mapping
                Dictionary<string, string> dic;
                if (IsPromotionSchedule)
                {
                    dic = new Dictionary<string, string>()
                    {
                        {"Item#","ProductID"},
                        {"促销活动描述","PromotionTitle"},
                        {"是否秒杀","IsSecondKill"},
                        {"今日炸蛋","IsTodaySpecials"},
                        {"警戒库存","BaseLine"},
                        {"每单限购数量","MaxPerOrder"},
                        {"开始时间","StartTime"},
                        {"结束时间","EndTime"},
                        {"价格","CountDownCurrentPrice"},
                        {"礼券","CouponRebate"},
                        {"积分","CountDownPoint"},
                        {"理由","Reasons"},
                        {"专区优先级","AreaShowPriority"},
                        {"首页优先级","HomePagePriority"},
                    };
                }
                else
                {
                    dic = new Dictionary<string, string>()
                    {
                        {"Item#","ProductID"},
                        {"限时抢购专区显示","IsCountDownAreaShow"},
                        {"首页限时抢购","IsHomePageShow"},
                        {"今日炸蛋","IsTodaySpecials"},
                        {"一级分类页面显示","IsC1Show"},
                        {"二级分类页面显示","IsC2Show"},
                        {"警戒库存","BaseLine"},
                        {"开始时间","StartTime"},
                        {"结束时间","EndTime"},
                        {"价格","CountDownCurrentPrice"},
                        {"礼券","CouponRebate"},
                        {"积分","CountDownPoint"},
                        {"理由","Reasons"},
                        {"专区优先级","AreaShowPriority"},
                        {"首页优先级","HomePagePriority"},
                  };

                }
                List<CountdownInfo> list = GetListCountDownInfoByTable(table, IsPromotionSchedule, dic, ref errorList);
                foreach (var item in list)
                {
                    try
                    {
                        item.PMRole = pmRole;
                        item.Status = CountdownStatus.WaitForPrimaryVerify;
                        CreateCountdown(item);
                    }
                    catch (Exception e) //Create方法的异常也和导入时的异常一起存起来 最后统一抛出
                    {
                        errorList.Add("'" + item.ProductID + "' " + e.Message);
                    }
                }
                if (errorList.Count > 0)
                {
                    throw new BizException(errorList.Join("\n"));
                }
               // throw new BizException("全部导入成功!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ImportSuccess"));
            }

        }

        /// <summary>
        /// check Execl的数据有效性
        /// </summary>
        /// <param name="IsPromotionSchedule">true:促销计划</param>
        private void ImportCountDownCheck(DataTable table, bool IsPromotionSchedule)
        {

            if (table == null || table.Rows.Count == 0)
            {
                //throw new BizException("Execl中没有数据,或者工作簿名称不是Sheet1或者格式不正确！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ExcelError"));
            }
            List<string> columns;
            int columnsCount = 0;
            if (IsPromotionSchedule) //促销计划
            {
                columnsCount = 14;
                columns = "Item#|促销活动描述|是否秒杀|今日炸蛋|警戒库存|每单限购数量|开始时间|结束时间|价格|礼券|积分|理由|专区优先级|首页优先级".Split('|').ToList();
            }
            else
            {
                columnsCount = 15;
                columns = "Item#|限时抢购专区显示|首页限时抢购|今日炸蛋|一级分类页面显示|二级分类页面显示|警戒库存|开始时间|结束时间|价格|礼券|积分|理由|专区优先级|首页优先级".Split('|').ToList();
            }
            //check列数
            if (table.Columns.Count != columnsCount)
            {
                //throw new BizException("Execl中的列和模板不同!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ExcelColumnError"));
            }
            //check列名是否和模板相同
            foreach (DataColumn column in table.Columns)
            {
                if (!columns.Contains(column.ColumnName))
                {
                    //throw new BizException("Execl中的列名和模板中不同!");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ExcelColumnError"));
                }
            }
        }
        /// <summary>
        /// 根据Datatable得到List<CountdownInfo>
        /// </summary>
        /// <param name="table">DataTable数据源</param>
        /// <param name="IsPromotionSchedule">是促销计划</param>
        /// <param name="dic">模板和实体属性的Mapping</param>
        /// <param name="errorList">错误List</param>
        /// <returns></returns>
        private List<CountdownInfo> GetListCountDownInfoByTable(DataTable table, bool IsPromotionSchedule, Dictionary<string, string> dic, ref List<string> errorList)
        {
            List<CountdownInfo> list = new List<CountdownInfo>();
            bool flag = true;
            int index = 1;
            foreach (DataRow row in table.Select("[Item#]<>''")) //去掉空行
            {
                CountdownInfo info = new CountdownInfo();
                flag = true;
                foreach (DataColumn column in table.Columns)
                {
                    try
                    {
                        System.Reflection.PropertyInfo property = typeof(CountdownInfo).GetProperty(dic[column.ColumnName]);
                        if (!property.CanWrite)
                        {
                            //throw new Exception(property.Name + "属性无法写入!");
                            throw new BizException(property.Name + ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_PropertyError"));
                        }
                        if (property.PropertyType == typeof(int?) || property.PropertyType == typeof(int))
                        {
                            property.SetValue(info, row[column].ToInteger(), null); //ToInteger 如果发生异常说明Execl中该列的值有错误
                        }
                        if (property.PropertyType == typeof(string))
                        {

                            property.SetValue(info, row[column].ToString(), null);
                        }
                        if (property.PropertyType == typeof(bool?) || property.PropertyType == typeof(bool))
                        {
                            var tempValue = true;
                            if (row[column].ToString() == "是")
                            {
                                tempValue = true;
                            }
                            else if (row[column].ToString() == "否")
                            {
                                tempValue = false;
                            }
                            else
                            {
                                throw new Exception();
                            }
                            property.SetValue(info, tempValue, null);
                        }
                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        {
                            DateTime tempValue;
                            if (!DateTime.TryParse(row[column].ToString(), out tempValue))
                            {
                                throw new Exception();
                            }
                            property.SetValue(info, tempValue, null);
                        }
                        if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                        {
                            decimal tempValue;
                            if (!decimal.TryParse(row[column].ToString(), out tempValue))
                            {
                                throw new Exception();
                            }
                            property.SetValue(info, tempValue, null);
                        }

                    }
                    catch (Exception)
                    {
                        //errorList.Add(string.Format("第{0}行{1}该列值存在异常，系统无法识别!", index, column.ColumnName)); //发生错误放到ErrorList中最后做统计
                        errorList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_ExcelCellError"), index, column.ColumnName)); //发生错误放到ErrorList中最后做统计
                        flag = false; //只要某一个属性发生错误，则该行记录就是无效
                        continue;
                    }


                }
                index = index + 1;
                if (flag)
                {
                    ProductInfo productInfo = ExternalDomainBroker.GetProductInfo(info.ProductID);
                    if (productInfo == null)
                    {
                       // errorList.Add(string.Format("{0}商品不存在!", info.ProductID));
                        errorList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Countdown", "Countdown_NotExistProduct"), info.ProductID));
                    }
                    else
                    {
                        info.ProductSysNo = productInfo.SysNo;
                        info.IsPromotionSchedule = IsPromotionSchedule;
                        list.Add(info);
                    }

                }
            }
            return list;
        }
        #endregion

        public void GetGiftSNAndCouponSNByProductSysNo(int productsysno, out int giftsysno, out int couponsysno)
        {
            ObjectFactory<ICountdownQueryDA>.Instance.GetGiftAndCouponSysNo(productsysno, out giftsysno, out couponsysno);
        }
    }
}
