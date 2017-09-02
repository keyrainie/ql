using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using System.ComponentModel.Composition;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess.Promotion;
using ECCentral.Service.IBizInteract;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.EventMessage.MKT;
//using ECCentral.Service.MKT.BizProcessor.PromotionEngine;

namespace ECCentral.Service.MKT.BizProcessor
{
    [Export(typeof(IPromotionCalculate))]
    [Export(typeof(IPromotionActivityJob))]
    [VersionExport(typeof(SaleGiftProcessor))]
    public class SaleGiftProcessor : CalculateBaseProcessor, IPromotionCalculate, IPromotionActivityJob
    {
        private ISaleGiftDA _da = ObjectFactory<ISaleGiftDA>.Instance;


        #region 计算类行为
        public List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, List<SOPromotionInfo> alreadyApplyPromoList)
        {
            List<SOPromotionInfo> promotionInfoList = new List<SOPromotionInfo>();
            return promotionInfoList;
            //目前后台SO不支持对赠品活动的处理，因此直接返回空的列表
            //如果以后客户对后台有这方面的需要，可以在下面2处按照客户的业务要求进行处理
            if (soInfo.Items == null || soInfo.Items.Count == 0)
            {
                return promotionInfoList;
            }
            //1.获取当前运行状态的所有赠品活动List，只取计算属性（降低内存消耗）
            List<SaleGiftInfo> curSaleGiftList = _da.LoadAllRunSaleGiftList();
            if (curSaleGiftList.Count == 0)
            {
                return promotionInfoList;
            }
            //2.遍历每个活动
            foreach (SaleGiftInfo salegift in curSaleGiftList)
            {
                //如果是买满即送，则需要判断SO的条件；
                //如果是同时购买，需要判断SOItem
                //几种赠品的优先级，需要根据客户需求来定
            }
            return promotionInfoList;
        }
        #endregion

        #region 维护类行为
        #region 全局行为
        /// <summary>
        /// 获取赠品所有信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual SaleGiftInfo Load(int? sysNo)
        {
            return _da.Load(sysNo);
        }


        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void SubmitAudit(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            successRecords = new List<string>();
            failureRecords = new List<string>();
            foreach (int sysNo in sysNoList)
            {
                SaleGiftInfo info = Load(sysNo);

                if (CheckGiftCompleted(info))
                {
                    SaleGiftStatus resultStatus = info.Status.Value;
                    string errorDescription = null;
                    string successDescription = string.Empty;
                    if (!CheckAndOperateStatus(PSOperationType.SubmitAudit, info.SysNo, info.Status.Value, out resultStatus, out errorDescription))
                    {
                        failureRecords.Add(errorDescription);
                        continue;
                    }


                    #region Check 厂商赠品不能存在于其他赠品
                    List<string> errorMsg = this.CheckGiftRules(info);
                    if (errorMsg.Count > 0)
                    {
                        failureRecords.AddRange(errorMsg);
                        continue;
                    }
                    #endregion

                    //提交审核有自动审核的流程，如果满足下面的条件，将是系统自动审核，否则需要人工审核
                    string checkFailMsg = string.Empty;
                    //switch (info.DisCountType.Value)
                    //{
                    //    case SaleGiftDiscountBelongType.BelongGiftItem:
                    //        if (CheckPMisPass(info))
                    //        {
                    //            resultStatus = SaleGiftStatus.Ready;
                    //        }
                    //        break;
                    //    case SaleGiftDiscountBelongType.BelongMasterItem:
                    //        if (CheckMarginIsPass(info, out checkFailMsg))
                    //        {
                    //            resultStatus = SaleGiftStatus.Ready;

                    //            successRecords.Add("");
                    //        }

                    //        break;
                    //}

                    TransactionScopeFactory.TransactionAction(() =>
                    {
                        _da.UpdateStatus(sysNo, resultStatus, userfullname);
                        //更新活动状态
                        //ObjectFactory<SaleGiftPromotionEngine>.Instance.UpdateSaleGiftActivityStatus(sysNo, resultStatus);

                        // 发送待办消息
                        EventPublisher.Publish<SaleGiftSubmitMessage>(new SaleGiftSubmitMessage
                        {
                            SaleGiftSysNo = sysNo,
                            SaleGiftName = info.Title.Content,
                            CurrentUserSysNo = ServiceContext.Current.UserSysNo
                        });
                    });

                    //successRecords.Add(string.Format("赠品[{0}]提交审核处理成功！",sysNo.ToString()));
                    successRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_SubExamSucced"), sysNo.ToString()));
                    if (resultStatus == SaleGiftStatus.Ready)
                    {
                        //successRecords.Add(string.Format("赠品[{0}]符合自动审核条件，已自动转为就绪状态！", sysNo.ToString()));
                        successRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AlreayReady"), sysNo.ToString()));
                    }
                    ExternalDomainBroker.CreateOperationLog(BizLogType.SaleGiftSubmitAudit.ToEnumDesc(), BizLogType.SaleGiftSubmitAudit, sysNo, info.CompanyCode);

                }
                else
                {
                    //string errorMsg = string.Format("赠品活动[{0}]信息不完整,无法提交审核!\r\n", sysNo);
                    string errorMsg = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_SubExamFailed"), sysNo);
                    failureRecords.Add(errorMsg);
                    continue;
                }

            }


        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void CancelAudit(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            successRecords = new List<string>();
            failureRecords = new List<string>();
            OperateActivity(PSOperationType.CancelAudit, sysNoList, out successRecords, out failureRecords);
        }

        /// <summary>
        /// 审核，包含审核通过和审核拒绝
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="auditType"></param>
        public virtual void Audit(List<int?> sysNoList, PromotionAuditType auditType, out List<string> successRecords, out List<string> failureRecords)
        {
            PSOperationType operation = auditType == PromotionAuditType.Approve ? PSOperationType.AuditApprove : PSOperationType.AuditRefuse;
            successRecords = new List<string>();
            failureRecords = new List<string>();
            OperateActivity(operation, sysNoList, out successRecords, out failureRecords);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Void(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            successRecords = new List<string>();
            failureRecords = new List<string>();
            OperateActivity(PSOperationType.Void, sysNoList, out successRecords, out failureRecords);
        }
        /// <summary>
        /// 手动提前中止
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void ManualStop(List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            successRecords = new List<string>();
            failureRecords = new List<string>();
            OperateActivity(PSOperationType.Stop, sysNoList, out successRecords, out failureRecords);
        }


        /// <summary>
        /// 操作赠品活动
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="sysNoList"></param>
        /// <param name="successRecords"></param>
        /// <param name="failureRecords"></param>
        protected void OperateActivity(PSOperationType operation, List<int?> sysNoList, out List<string> successRecords, out List<string> failureRecords)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            successRecords = new List<string>();
            failureRecords = new List<string>();
            foreach (int sysNo in sysNoList)
            {
                SaleGiftInfo info = Load(sysNo);
                if (info == null)
                {
                    //failureRecords.Add(string.Format("活动[{0}]信息加载失败!", sysNo));
                    failureRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ActivityInfoLoadFailed"), sysNo));
                    continue;
                }

                SaleGiftStatus resultStatus = info.Status.Value;
                string errorDescription = null;
                if (!CheckAndOperateStatus(operation, info.SysNo, info.Status.Value, out resultStatus, out errorDescription))
                {
                    failureRecords.Add(errorDescription);
                    continue;
                }

                TransactionScopeFactory.TransactionAction(() =>
                {
                    _da.UpdateStatus(sysNo, resultStatus, userfullname);
                    //更新活动状态
                    //ObjectFactory<SaleGiftPromotionEngine>.Instance.UpdateSaleGiftActivityStatus(sysNo, resultStatus);

                    // 发送待办消息
                    switch (operation)
                    {
                        // 审核
                        case PSOperationType.AuditApprove:
                            EventPublisher.Publish<SaleGiftAuditMessage>(new SaleGiftAuditMessage
                            {
                                SaleGiftSysNo = sysNo,
                                SaleGiftName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                        // 取消审核
                        case PSOperationType.CancelAudit:
                            EventPublisher.Publish<SaleGiftAuditCancelMessage>(new SaleGiftAuditCancelMessage
                            {
                                SaleGiftSysNo = sysNo,
                                SaleGiftName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                        case PSOperationType.AuditRefuse:
                            EventPublisher.Publish<SaleGiftAuditRejectMessage>(new SaleGiftAuditRejectMessage
                            {
                                SaleGiftSysNo = sysNo,
                                SaleGiftName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                        case PSOperationType.Void:
                            EventPublisher.Publish<SaleGiftVoidMessage>(new SaleGiftVoidMessage
                            {
                                SaleGiftSysNo = sysNo,
                                SaleGiftName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                    }
                });

                SyncGiftStatus(info.RequestSysNo, resultStatus);
                //successRecords.Add(string.Format("赠品活动[{0}]处理成功！", sysNo.ToString()));
                successRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_GiftActivityHandleSucced"), sysNo.ToString()));
                BizLogType logtype = BizLogType.SaleGiftCreateMaster;
                switch (operation)
                {
                    case PSOperationType.Void:
                        logtype = BizLogType.SaleGiftVoid;
                        break;
                    case PSOperationType.AuditApprove:
                        logtype = BizLogType.SaleGiftAuditApprove;
                        break;
                    case PSOperationType.AuditRefuse:
                        logtype = BizLogType.SaleGiftAuditRefuse;
                        break;
                    case PSOperationType.CancelAudit:
                        logtype = BizLogType.SaleGiftCancelAudit;
                        break;
                    case PSOperationType.Stop:
                        logtype = BizLogType.SaleGiftStop;
                        break;
                    default:
                        break;
                }

                if (logtype != BizLogType.SaleGiftCreateMaster)
                {
                    ExternalDomainBroker.CreateOperationLog(logtype.ToEnumDesc(), logtype, sysNo, info.CompanyCode);
                }
            }
        }

        #endregion

        #region 局部对象处理
        /// <summary>
        /// 创建赠主信息 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int? CreateMaster(SaleGiftInfo info)
        {
            ValidateCoupons(info);
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.InUser = userfullname ?? ServiceContext.Current.UserDisplayName;

            if (info.Type == SaleGiftType.Full)
            {
                info.IsGlobalProduct = true;
            }
            //if (info.Type == SaleGiftType.Additional)
            //{
            //    info.GiftComboType = SaleGiftGiftItemType.GiftPool;
            //    info.ItemGiftCount = 1;
            //}
            TransactionScopeFactory.TransactionAction(() =>
            {
                info.SysNo = _da.CreateMaster(info);
                //保存活动信息
                //ObjectFactory<SaleGiftPromotionEngine>.Instance.SaveSaleGiftActivity(info);
            });

            ExternalDomainBroker.CreateOperationLog(BizLogType.SaleGiftCreateMaster.ToEnumDesc(), BizLogType.SaleGiftCreateMaster, info.SysNo.Value, info.CompanyCode);
            return info.SysNo;

        }

        /// <summary>
        /// 复制新建
        /// </summary>
        /// <param name="oldSysNo"></param>
        /// <returns></returns>
        public virtual int? CopyCreateNew(int? oldSysNo)
        {
            SaleGiftInfo info = Load(oldSysNo);

            if (info.Status != SaleGiftStatus.Finish && info.Status != SaleGiftStatus.Stoped && info.Status != SaleGiftStatus.Void)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_TheStatusCanNotCopy"), info.SysNo));
            }

            info.SysNo = null;
            info.Status = SaleGiftStatus.Init;
            if (info.EndDate < DateTime.Now)
            {
                info.BeginDate = DateTime.Now.AddHours(1);
                info.EndDate = DateTime.Now.AddDays(1).AddHours(1);
            }

            //厂商赠品的赠品不能作为其它赠品活动的赠品
            List<string> errorMsg = this.CheckGiftRules(info);
            if (errorMsg.Count > 0)
            {
                throw new BizException(errorMsg.Join(Environment.NewLine));
            }

            int sysno = CreateMaster(info).Value;
            info.SysNo = sysno;
            SetSaleRules(info);
            SetGiftItemRules(info);
            return sysno;
        }

        /// <summary>
        /// 更新主信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateMaster(SaleGiftInfo info)
        {
            ValidateCoupons(info);
            //判断处理期间，活动状态是否已经发生了变化，如果发生了变化，则操作失败，需要用户重新刷新处理
            SaleGiftInfo tempEntity = Load(info.SysNo.Value);
            if (info.Status != tempEntity.Status)
            {
                // throw new BizException(string.Format("活动[{0}]编辑失败：编辑期间，状态已经发生了变化，请重新刷新处理！", info.SysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ReLoad"), info.SysNo));
            }
            SaleGiftStatus resultStatus = info.Status.Value;
            string errorDescription = null;
            if (!CheckAndOperateStatus(PSOperationType.Edit, info.SysNo, info.Status.Value, out resultStatus, out errorDescription))
            {
                throw new BizException(errorDescription);
            }
            info.Status = resultStatus;

            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.EditUser = userfullname;

            TransactionScopeFactory.TransactionAction(() =>
            {
                _da.UpdateMaster(info);
                //保存活动信息
                //ObjectFactory<SaleGiftPromotionEngine>.Instance.SaveSaleGiftActivity(info);
            });

            ExternalDomainBroker.CreateOperationLog(BizLogType.SaleGiftUpdateMaster.ToEnumDesc(), BizLogType.SaleGiftUpdateMaster, info.SysNo.Value, info.CompanyCode);
        }

        /// <summary>
        /// 促销活动规则设置
        /// </summary>
        /// <param name="info"></param>
        public void SetSaleRules(SaleGiftInfo info)
        {
            if (info.IsGlobalProduct.Value == false)
            {
                if (info.ProductCondition == null || info.ProductCondition.Count == 0)
                {
                    //throw new BizException("请至少设置一项活动规则！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_MustOneMoreActivityRule"));
                }
            }


            SaleGiftInfo tempEntity = _da.Load(info.SysNo.Value);
            int vendorsysno = tempEntity.VendorSysNo.Value;
            if (tempEntity.VendorType == 0)
            {
                vendorsysno = 1;
            }


            if (info.Type == SaleGiftType.Single)
            {
                if (!(info.ProductCondition.Count == 1 && info.ProductCondition[0].RelProduct != null))
                {
                    //throw new BizException("单品买赠只能设置1个主商品！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_JustOneMainGoods"));
                }
            }

            if (info.Type == SaleGiftType.Vendor)
            {
                if (!(info.ProductCondition.Count == 1 && info.ProductCondition[0].RelProduct != null))
                {
                    //throw new BizException("厂商赠品只能设置1个主商品！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_JustOneMainGoodsForGift"));
                }

                List<SaleGiftInfo> giftInfoList = _da.GetGiftInfoListByProductSysNo(info.ProductCondition[0].RelProduct.ProductSysNo.Value, SaleGiftStatus.Run);

                if (giftInfoList.Where(p => p.SysNo != info.SysNo && p.Type == SaleGiftType.Vendor).ToList().Count > 0)
                {
                    //throw new BizException(string.Format("厂商主商品 ({0}) 存在有效的重复的记录！", info.ProductCondition[0].RelProduct.ProductID));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_PrimaryProductExsisit2"), info.ProductCondition[0].RelProduct.ProductID));
                }

            }

            if (info.Type == SaleGiftType.Multiple)
            {
                int productCount = 0;

                info.ProductCondition.ForEach(p => productCount = productCount + (p.RelProduct.MinQty.HasValue ? p.RelProduct.MinQty.Value : 0));

                if (productCount < 2)
                {
                    //throw new BizException("同时购买必须至少设置2个商品！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_MustTwoMoreGoods"));
                }

                if (info.ProductCondition.Where(p => p.RelProduct != null && p.RelProduct.ProductSysNo.HasValue).ToList().Count > 12)
                {
                    //throw new BizException("当类型为同时购买时，商品最多设置12个。");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_Most12Goods"));
                }
            }

            if (info.Type == SaleGiftType.Full)
            //info.Type == SaleGiftType.FirstOrder || info.Type == SaleGiftType.Additional)
            {
                if (info.ProductCondition != null && info.ProductCondition.Where(p => (p.RelC3 != null && p.RelC3.SysNo.HasValue) || (p.RelBrand != null && p.RelBrand.SysNo.HasValue)).ToList().Count > 20)
                {
                    //“{0}”主商品规则控制最多为20条
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_RuleLessThanTwenty"), info.Type.ToEnumDesc()));
                }
            }

            foreach (var item in info.ProductCondition)
            {
                if (item.RelProduct.ProductSysNo.HasValue)
                {
                    int productvendorsysno = _da.GetVendorSysNoByProductSysNo(item.RelProduct.ProductSysNo.Value);
                    if (productvendorsysno != vendorsysno)
                    {
                        //throw new BizException(string.Format("{0}不能添加其他商家的商品【{1}】", tempEntity.VendorName, item.RelProduct.ProductID));
                        throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_CanntAddOtherMerchantProduct2"), tempEntity.VendorName, item.RelProduct.ProductID));
                    }
                }
            }

            if (info.Status != tempEntity.Status)
            {
                //throw new BizException(string.Format("活动[{0}]编辑失败：编辑期间，状态已经发生了变化，请重新刷新处理！", info.SysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ReLoad"), info.SysNo));
            }

            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            CheckStatusWhenUpdate(info, userfullname);

            TransactionScopeFactory.TransactionAction(() =>
            {
                int promotionSysNo = info.SysNo.Value;

                _da.DeleteSaleRules(promotionSysNo);
                if (info.ProductCondition != null && info.ProductCondition.Count > 0)
                {
                    foreach (SaleGift_RuleSetting setting in info.ProductCondition)
                    {
                        _da.CreateSaleRules(promotionSysNo, setting);
                    }
                }
                //else
                //{
                //    _da.CreateGloableSaleRules(promotionSysNo, info);
                //}

                _da.UpdateGiftIsGlobal(promotionSysNo, info.IsGlobalProduct.Value, userfullname);
                //保存活动
                //ObjectFactory<SaleGiftPromotionEngine>.Instance.SaveSaleGiftActivity(info);
            });

            ExternalDomainBroker.CreateOperationLog(BizLogType.SaleGiftSetSaleRules.ToEnumDesc(), BizLogType.SaleGiftSetSaleRules, info.SysNo.Value, info.CompanyCode);
        }



        /// <summary>
        /// 赠品设置
        /// </summary>
        /// <param name="info"></param>
        public void SetGiftItemRules(SaleGiftInfo info)
        {

            CheckGiftStockNumber(info);

            CheckGiftRulesIsPass(info);

            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            SaleGiftInfo tempEntity = _da.Load(info.SysNo.Value);
            int vendorsysno = tempEntity.VendorSysNo.Value;
            if (tempEntity.VendorType == 0)
            {
                vendorsysno = 1;
            }

            foreach (var item in info.GiftItemList)
            {
                if (item.ProductSysNo.HasValue)
                {
                    item.VendorSysNo = _da.GetVendorSysNoByProductSysNo(item.ProductSysNo.Value);
                    if (item.VendorSysNo.Value != vendorsysno)
                    {
                        // throw new BizException(string.Format("{0}不能添加其他商家的商品【{1}】", tempEntity.VendorName, item.ProductID));
                        throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_CanntAddOtherMerchantProduct2"), tempEntity.VendorName, item.ProductID));
                    }
                }
            }

            CheckStatusWhenUpdate(info, userfullname);

            TransactionScopeFactory.TransactionAction(() =>
            {
                int promotionSysNo = info.SysNo.Value;

                _da.DeleteGiftItemRules(promotionSysNo);
                if (info.GiftItemList != null && info.GiftItemList.Count > 0)
                {
                    foreach (RelProductAndQty setting in info.GiftItemList)
                    {
                        _da.CreateGiftItemRules(promotionSysNo, setting);
                    }
                }
                _da.UpdateGiftItemCount(promotionSysNo, info.GiftComboType.Value, info.ItemGiftCount, userfullname);
                //保存活动
                //ObjectFactory<SaleGiftPromotionEngine>.Instance.SaveSaleGiftActivity(info);
            });
            ExternalDomainBroker.CreateOperationLog(BizLogType.SaleGiftSetGiftItemRules.ToEnumDesc(), BizLogType.SaleGiftSetGiftItemRules, info.SysNo.Value, info.CompanyCode);
        }

        /// <summary>
        /// 检测赠品和主商品在同一个仓库是否都有货 
        /// </summary>
        /// <param name="info"></param>
        private void CheckGiftStockNumber(SaleGiftInfo info)
        {
            if (!info.IsAccess)//没权限
            {
                string result = CheckGiftStockResult(info);
                if (result.Length > 0)
                {
                    throw new BizException(result);
                }
            }
        }

        /// <summary>
        /// Check 厂商赠品的赠品不能作为其它赠品活动的赠品
        /// Added by Poseidon.y.tong 加购赠品不能作为其他赠品活动的赠品
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private List<String> CheckGiftRules(SaleGiftInfo info)
        {
            List<SaleGiftInfo> giftInfoListTmp = new List<SaleGiftInfo>();
            List<String> msgTmp = new List<String>();
            foreach (var item in info.GiftItemList)
            {
                giftInfoListTmp.AddRange(_da.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.Run));
                giftInfoListTmp.AddRange(_da.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.Ready));
                giftInfoListTmp.AddRange(_da.GetGiftItemListByProductSysNo(item.ProductSysNo.Value, SaleGiftStatus.WaitingAudit));

                if (info.Type == SaleGiftType.Vendor)
                //|| info.Type == SaleGiftType.Additional)
                {
                    if (giftInfoListTmp != null && giftInfoListTmp.Where(p => p.SysNo != info.SysNo && p.Type != SaleGiftType.Vendor).ToList().Count > 0)
                    {
                        //msgTmp.Add(string.Format(@"""厂商赠品""和""满额加购赠品"" ({0}) 在其他赠品活动中存在有效的重复的记录！", ExternalDomainBroker.GetSimpleProductInfo(item.ProductSysNo.Value).ProductID));
                        msgTmp.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ExsisitActivityInOther2"), ExternalDomainBroker.GetSimpleProductInfo(item.ProductSysNo.Value).ProductID));
                        continue;
                    }
                }
                else
                {
                    if (giftInfoListTmp != null && giftInfoListTmp.Where(p => p.SysNo != info.SysNo && (p.Type == SaleGiftType.Vendor)).ToList().Count > 0)
                    //|| p.Type == SaleGiftType.Additional)).ToList().Count > 0)
                    {
                        //msgTmp.Add(string.Format(@"赠品 ({0}) 在""厂商赠品""或""满额加购赠品""活动中存在有效的重复的记录！", ExternalDomainBroker.GetSimpleProductInfo(item.ProductSysNo.Value).ProductID));
                        msgTmp.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ExsisitActivityInMerchant2"), ExternalDomainBroker.GetSimpleProductInfo(item.ProductSysNo.Value).ProductID));
                        continue;
                    }
                }
            }
            return msgTmp;
        }

        /// <summary>
        ///检查主商品和赠品库存后返回结果 赠品类型为 单品买赠或厂商赠送时 需检查
        ///该方法在也会在Service中公开给Portal端供高级权限check时调用
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string CheckGiftStockResult(SaleGiftInfo info)
        {
            string result = string.Empty;
            if (info.ProductCondition.Count() <= 0) { return result; }
            if (info.Type == SaleGiftType.Single || info.Type == SaleGiftType.Vendor)
            {
                //得到主商品的所有库存
                List<ProductInventoryInfo> list = GetAllProductInventoryByProductSysNo(info.ProductCondition.FirstOrDefault().RelProduct.ProductSysNo);
                if (list.Count == 0)
                {
                    //result = "主商品" + info.ProductCondition.FirstOrDefault().RelProduct.ProductName + "在所有仓库中无货，无法创建赠品规则";
                    result = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_MasterProductStockNull"), info.ProductCondition.FirstOrDefault().RelProduct.ProductName);
                    return result;
                }
                //获取赠品的库存信息和主商品库存信息对比
                foreach (var item in info.GiftItemList)
                {
                    List<ProductInventoryInfo> tempList = GetAllProductInventoryByProductSysNo(item.ProductSysNo);
                    if (tempList.Count == 0)
                    {
                        var giftProduct = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
                        result = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_OneGiftProductStockNull"), giftProduct.ProductID);
                        return result;
                    }
                    /*
                    if (tempList.Count == 0 || tempList.Count > list.Count)
                    {
                        int productSysNo = list.FirstOrDefault().ProductSysNo.Value;
                        var product = ExternalDomainBroker.GetProductInfo(productSysNo);
                        var giftProduct = ExternalDomainBroker.GetProductInfo(item.ProductSysNo.Value);
                        int stockSysNo = 0;

                        foreach (var p in tempList)
                        {
                            var i = list.FirstOrDefault(k => k.ProductSysNo == p.ProductSysNo && k.StockSysNo == p.StockSysNo);
                            if (i == null)
                            {
                                stockSysNo = p.StockSysNo.Value;
                                break;
                            }
                        };
                        
                        if (stockSysNo < 1)
                        {
                            //result = string.Format("主商品{0}和赠品{1}在{2}仓未同时有货,无法创建赠品规则", product.ProductID, giftProduct.ProductID, stockSysNo);
                            result = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_MasterOrGiftProductStockNull"), product.ProductID, giftProduct.ProductID);
                            return result;
                        }
                    }*/
                    //List<ProductInventoryInfo> data;
                    //if (list.Count > tempList.Count)
                    //{
                    //    data = (from p in list from s in tempList where p.StockSysNo != s.StockSysNo select p).ToList();
                    //}
                    //else
                    //{
                    //    data = (from p in list from s in tempList where p.StockSysNo != s.StockSysNo select s).ToList();
                    //}
                    //if (data.Count > 0)
                    //{
                    //    result = string.Format("主商品{0}和赠品{1}在{2}未同时有货,无法创建赠品规则", data.FirstOrDefault().ProductID, item.ProductID, data.FirstOrDefault().StockInfo.StockName);
                    //    return result;
                    //}
                }
            }
            return result;

        }

        /// <summary>
        /// 根据商品编号获取有货的仓库信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        private List<ProductInventoryInfo> GetAllProductInventoryByProductSysNo(int? productSysNo)
        {
            return ExternalDomainBroker.GetProductInventoryInfo((int)productSysNo).Where(s => s.OnlineQty > 0).ToList();
        }

        protected virtual void CheckStatusWhenUpdate(SaleGiftInfo info, string userfullname)
        {
            SaleGiftStatus resultStatus = info.Status.Value;
            string errorDescription = null;
            if (!CheckAndOperateStatus(PSOperationType.Edit, info.SysNo, info.Status.Value, out resultStatus, out errorDescription))
            {
                throw new BizException(errorDescription);
            }
            //如果当前状态是就绪状态，那么Check后应该是Init状态，所以需要更改为Init状态：就绪状态下一旦Upadate了，就要更新状态为Init

            if (resultStatus != info.Status)
            {
                _da.UpdateStatus(info.SysNo.Value, resultStatus, userfullname);
            }
        }
        #endregion

        #region 验证Check
        /// <summary>
        /// 主信息保存验证
        /// </summary>
        /// <param name="info"></param>
        public virtual void ValidateCoupons(SaleGiftInfo info)
        {
            //if (string.IsNullOrEmpty(info.Title.Content))
            //{
            //    //throw new BizException("规则名称不能为空！");
            //    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_RuleNameRequired"));
            //}

            //if (string.IsNullOrEmpty(info.Description.Content))
            //{
            //    //throw new BizException("规则描述不能为空！");
            //    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_RuleDecrRequired"));
            //}


            if (!info.EndDate.HasValue || !info.BeginDate.HasValue)
            {
                //throw new BizException("必须设置开始和结束日期！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_StartAndEndDateRequired"));
            }

            if (info.EndDate <= DateTime.Now)
            {
                //throw new BizException("失效日期必须大于当前日期！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_LoseEffectDateGreaterThanCurrent"));
            }
            if (info.BeginDate > info.EndDate)
            {
                // throw new BizException("失效日期必须大于等于生效日期！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_EqualOrGreaterThanTakeEffectDate"));
            }
        }

        /// <summary>
        /// 检查当前状态的下当前操作是否符合业务逻辑。同时也通过out参数返回操作后的状态
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="curStatus">当前状态</param>
        /// <param name="resultStatus">操作后的状态</param>
        /// <returns>当前状态下本操作是否正确</returns>
        public virtual bool CheckAndOperateStatus(PSOperationType operation, int? sysNo, SaleGiftStatus? curStatus,
            out SaleGiftStatus resultStatus, out string errorDescription)
        {
            resultStatus = curStatus.HasValue ? curStatus.Value : SaleGiftStatus.Init;
            errorDescription = null;
            bool checkPassResult = true;

            switch (operation)
            {
                case PSOperationType.Edit:
                    if (curStatus != SaleGiftStatus.Init)
                    {
                        checkPassResult = false;
                        // errorDescription = string.Format("活动[{0}]编辑失败:只有初始状态可以进行编辑！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_OnlyInitCanEdit"), sysNo, curStatus.Value.ToDisplayText());
                    }
                    break;
                case PSOperationType.SubmitAudit:
                    if (curStatus != SaleGiftStatus.Init)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]提交审核失败:只有初始状态可以提交审核！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_JustInitCanSubmitAudit"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    resultStatus = SaleGiftStatus.WaitingAudit;
                    break;
                case PSOperationType.CancelAudit:
                    if (curStatus != SaleGiftStatus.WaitingAudit && curStatus != SaleGiftStatus.Ready)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]撤回失败:只有待审核状态和就绪状态可以取消审核！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_OnlyCheckPendingOrReadyCancel"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    resultStatus = SaleGiftStatus.Init;
                    break;
                case PSOperationType.AuditApprove:
                    if (curStatus != SaleGiftStatus.WaitingAudit)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]审核通过失败:只有待审核状态可以审核通过！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_OnlyCheckPendingPass"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    resultStatus = SaleGiftStatus.Ready;
                    break;
                case PSOperationType.AuditRefuse:
                    if (curStatus != SaleGiftStatus.WaitingAudit)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]审核拒绝失败:只有待审核状态可以进行审核拒绝！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_OnlyCheckPendingRefuse"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    resultStatus = SaleGiftStatus.Void;
                    break;
                case PSOperationType.Stop:
                    if (curStatus != SaleGiftStatus.Run)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]提前中止失败:只有运行状态可以进行提前中止！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_OnlyRunStop"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    resultStatus = SaleGiftStatus.Stoped;
                    break;
                case PSOperationType.Void:
                    //if (curStatus != SaleGiftStatus.Init && curStatus != SaleGiftStatus.WaitingAudit && curStatus != SaleGiftStatus.Ready)
                    //{
                    if (curStatus != SaleGiftStatus.Ready)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]作废失败:只有初始、待审核、就绪状态可以进行作废！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_OnlyReadyCanDeny"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    resultStatus = SaleGiftStatus.Void;
                    break;
            }

            return checkPassResult;
        }


        /// <summary>
        /// 检查赠品信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual void CheckGiftRulesIsPass(SaleGiftInfo info)
        {
            //销售价格为0或999999的商品不能作为赠品
            if (info == null || info.GiftItemList == null || info.GiftItemList.Count == 0) return;

            if (info.GiftItemList.Count > 8 || info.GiftItemList.Count < 1)
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_GiftCountLessThanEight"));
            }

            //赠品必须是非展示状态，这个在Portal端已经验证
            //判断是否为附件
            List<string> errList = new List<string>();
            foreach (RelProductAndQty relProd in info.GiftItemList)
            {
                if (ExternalDomainBroker.CheckIsAttachment(relProd.ProductSysNo.Value))
                {
                    //errList.Add(string.Format("商品({0})是附件，附件不能作为赠品！", relProd.ProductSysNo));
                    errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AttachNotAsGift"), relProd.ProductID));
                }
                ProductInfo product = ExternalDomainBroker.GetProductInfo(relProd.ProductSysNo.Value);
                //if (product.ProductPriceInfo.CurrentPrice == 0 || product.ProductPriceInfo.CurrentPrice == 999999)
                //{
                //    //errList.Add(string.Format("商品({0})销售价格为0或999999，不能作为赠品!", relProd.ProductSysNo));
                //    errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_GoodsNotAsGift"), relProd.ProductID));
                //}
                //满额加购赠品的加购价格不能大于赠品的售价
                //if (info.Type == SaleGiftType.Additional && product.ProductPriceInfo.CurrentPrice < relProd.PlusPrice)
                //{
                //    //商品({0})的加购价格超过商品售价，不能作为加购赠品！
                //    errList.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AdditionGiftValidPrice"), relProd.ProductID));
                //}
            }
            if (errList.Count > 0)
            {
                string errmsg = errList.Join(Environment.NewLine);
                throw new BizException(errmsg);
            }

        }

        /// <summary>
        /// 检查PM
        /// </summary>
        /// <param name="info"></param>
        public virtual bool CheckPMisPass(SaleGiftInfo info)
        {
            if (info == null) return true;
            var isSubmit = true;
            if ((info.Type.HasValue && info.Type.Value == SaleGiftType.Full)
                || (info.GiftComboType.HasValue && info.GiftComboType.Value == SaleGiftGiftItemType.GiftPool))
            {
                return false;
            }
            List<RelProductAndQty> giftList = info.GiftItemList;

            List<SaleGift_RuleSetting> saleRuleList = info.ProductCondition.FindAll(f => f.Type.Value == SaleGiftSaleRuleType.Item && f.ComboType.Value != AndOrType.Not);
            if (saleRuleList == null || saleRuleList.Count == 0)
            {
                return false;
            }

            List<RelProductAndQty> saleList = new List<RelProductAndQty>();
            saleRuleList.ForEach(f => saleList.Add(f.RelProduct));

            List<int> giftProdudctSysNoList = new List<int>();
            giftList.ForEach(f => giftProdudctSysNoList.Add(f.ProductSysNo.Value));
            List<int> saleProductSysNoList = new List<int>();
            saleList.ForEach(f => saleProductSysNoList.Add(f.ProductSysNo.Value));

            List<ProductInfo> giftProductList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(giftProdudctSysNoList);
            List<ProductInfo> saleProductList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(saleProductSysNoList);

            List<int> giftPMList = new List<int>();
            foreach (ProductInfo product in giftProductList)
            {
                if (!giftPMList.Contains(product.ProductBasicInfo.ProductManager.SysNo.Value))
                {
                    giftPMList.Add(product.ProductBasicInfo.ProductManager.SysNo.Value);
                }
            }
            List<int> salePMList = new List<int>();
            foreach (ProductInfo product in saleProductList)
            {
                if (!salePMList.Contains(product.ProductBasicInfo.ProductManager.SysNo.Value))
                {
                    salePMList.Add(product.ProductBasicInfo.ProductManager.SysNo.Value);
                }
            }

            if (giftPMList.Count != salePMList.Count)
            {
                return false;
            }

            var value = giftPMList.Except(salePMList);
            if (value.Count() > 0)
            {
                return false;
            }

            return isSubmit;
        }

        /// <summary>
        /// 检查毛利率
        /// </summary>
        /// <param name="info"></param>
        /// <param name="checkFailMsg"></param>
        public virtual bool CheckMarginIsPass(SaleGiftInfo info, out string checkFailMsg)
        {
            checkFailMsg = string.Empty;
            if (info == null) return true;
            var isSubmit = true;
            if (info.Type == SaleGiftType.Full || info.GiftComboType == SaleGiftGiftItemType.GiftPool)
            {
                //当前赠品活动状态需要提交审核
                return false;
            }

            if (info.DisCountType.Value == SaleGiftDiscountBelongType.BelongGiftItem)
            {
                //Note: 实际上成本计入赠品时，不会Check毛利率，所以下面这段代码永远不会被调用
                var value = new List<int>();
                foreach (RelProductAndQty relProd in info.GiftItemList)
                {
                    ProductInfo giftProduct = ExternalDomainBroker.GetProductInfo(relProd.ProductSysNo.Value);

                    relProd.GrossMarginRate = ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGift_GiftItemGrossMarginRate(giftProduct, info.DisCountType.Value);
                    decimal minMargin = ObjectFactory<GrossMarginProcessor>.Instance.GetStockPrimaryGrossMarginRate(relProd.ProductSysNo.Value);
                    if (relProd.GrossMarginRate < minMargin)
                    {
                        value.Add(relProd.ProductSysNo.Value);
                    }
                    if (value.Count > 0)
                    {
                        isSubmit = false;
                        //checkFailMsg = "赠品(商品编号:" + value.Join(",") + ")毛利率低于初级毛利率,自动提交为待审核状态。";
                        checkFailMsg = ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_GiftGoodsSysNo") + value.Join(",") + ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AutoToCheckPending");
                    }
                }
            }
            else
            {
                var value = new List<int>();
                List<SaleGift_RuleSetting> saleRuleList = info.ProductCondition.FindAll(f => f.Type.Value == SaleGiftSaleRuleType.Item);
                if (saleRuleList != null && saleRuleList.Count > 0)
                {
                    foreach (SaleGift_RuleSetting rule in saleRuleList)
                    {
                        if (rule.RelProduct == null || !rule.RelProduct.ProductSysNo.HasValue)
                        {
                            continue;
                        }
                        ProductInfo product = ExternalDomainBroker.GetProductInfo(rule.RelProduct.ProductSysNo.Value);
                        rule.RelProduct.GrossMarginRate = ObjectFactory<GrossMarginProcessor>.Instance.GetSaleGift_SaleItemGrossMarginRate(product, rule.RelProduct.MinQty.Value, info.SysNo.Value, info);

                        decimal minMargin = ObjectFactory<GrossMarginProcessor>.Instance.GetStockPrimaryGrossMarginRate(rule.RelProduct.ProductSysNo.Value);
                        //返回的毛利没有处以100
                        if (rule.RelProduct.GrossMarginRate < minMargin / 100)
                        {
                            value.Add(rule.RelProduct.ProductSysNo.Value);
                        }
                        if (value.Count > 0)
                        {
                            isSubmit = false;
                            //checkFailMsg = "赠品(商品编号:" + value.Join(",") + ")毛利率低于初级毛利率,自动提交为待审核状态。"; 
                            checkFailMsg = ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_GiftGoodsSysNo") + value.Join(",") + ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_AutoToCheckPending");
                        }
                    }
                }
            }

            return isSubmit;
        }

        /// <summary>
        /// 判断提交审核
        /// </summary>
        /// <param name="promotionSysNo"></param>
        /// <param name="companyCode"></param>
        private bool CheckGiftCompleted(SaleGiftInfo info)
        {
            int saleRulesCount = 0;
            if ((info.Type == SaleGiftType.Full)
                //|| info.Type == SaleGiftType.FirstOrder || info.Type == SaleGiftType.Additional)
                && info.IsGlobalProduct.HasValue && info.IsGlobalProduct.Value)
            {
                saleRulesCount = 1;
            }
            else
            {
                saleRulesCount = info.ProductCondition != null ? info.ProductCondition.Count : 0;
            }

            int giftRulesCount = info.GiftItemList != null ? info.GiftItemList.Count : 0;

            bool result = saleRulesCount != 0 && giftRulesCount != 0;
            return result;
        }

        #endregion

        /// <summary>
        /// 判断商品在赠品活动中是否作废赠品存在，但是要排除已“作废”“完成”“中止”状态的记录的赠品活动
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public bool ProductIsGift(int productSysNo)
        {
            return _da.ProductIsGift(productSysNo);
        }

        #endregion

        #region Job行为
        public void ActivityStatusProcess()
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// 验证商品是否可以调价(例如是赠品或团购以及限时抢购不能调价,条件：赠品-本身作为赠品；生效状态-只有运行性中的)
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public bool CheckMarketIsActivity(int productSysNo)
        {
            return _da.CheckMarketIsActivity(productSysNo);
        }

        public void SyncGiftStatus(int? requestSysNo, SaleGiftStatus status)
        {
            if (requestSysNo > 0)
            {
                _da.SyncGiftStatus(requestSysNo.Value, status);
            }
        }

        public List<RelVendor> GetGiftVendorList()
        {
            return _da.GetGiftVendorList();
        }


        #region 外部Service将访问
        /// <summary>
        /// 获取商品所在赠品活动中的折扣
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<ProductPromotionDiscountInfo> GetGiftDiscountListByProductSysNo(int productSysNo)
        {
            List<ProductPromotionDiscountInfo> giftDiscountList = _da.GetGiftAmount(productSysNo);
            giftDiscountList.ForEach(g => g.PromotionType = PromotionType.SaleGift);
            SetCouponDiscountByProductSysNo(giftDiscountList);
            return giftDiscountList;
        }

        /// <summary>
        /// 根据赠品活动情况设置折扣
        /// </summary>
        /// <param name="couponDiscountList"></param>
        private void SetCouponDiscountByProductSysNo(List<ProductPromotionDiscountInfo> couponDiscountList)
        {
            if (couponDiscountList == null || !couponDiscountList.Any()) return;
            var discountList = couponDiscountList.Where(p => p.PromotionType == PromotionType.SaleGift).ToList();
            if (discountList.Count == 0) return;
            discountList.ForEach(v =>
            {
                var sysNo = v.ReferenceSysNo;
                if (sysNo > 0)
                {
                    var gift = Load(sysNo);
                    if (gift != null)
                    {
                        if (gift.DisCountType == SaleGiftDiscountBelongType.BelongGiftItem)
                        {
                            v.Discount = 0m;
                        }
                    }
                }


            });
        }
        #endregion
    }
}
