using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Transactions;
using System.Linq;
using System.Text;
using ECCentral.Service.EventMessage.MKT;

//using ECCentral.Service.MKT.BizProcessor.PromotionEngine;
using ECCentral.BizEntity.Invoice;
using System.Data;

namespace ECCentral.Service.MKT.BizProcessor
{
    [Export(typeof(IPromotionActivityJob))]
    [VersionExport(typeof(GroupBuyingProcessor))]
    public class GroupBuyingProcessor : IPromotionActivityJob
    {
        private IGroupBuyingDA m_GroupBuyingDA = ObjectFactory<IGroupBuyingDA>.Instance;
        private IGroupBuyingQueryDA m_GroupBuyingQueryDA = ObjectFactory<IGroupBuyingQueryDA>.Instance;
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public GroupBuyingInfo Load(int? sysNo)
        {
            var result = m_GroupBuyingDA.Load(sysNo.Value);
            if (result == null)
            {
                //throw new BizException("团购信息不存在!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_NotExsitGroupBuyInfo"));
            }
            if (result.ProductSysNo > 0)
            {
                var product = ExternalDomainBroker.GetProductInfo(result.ProductSysNo.Value);
                result.BasicPrice = product.ProductPriceInfo.BasicPrice;
                List<object> oPriceList = m_GroupBuyingDA.GetProductOriginalPrice(product.SysNo, result.IsByGroup ?? false ? "Y" : "N", result.CompanyCode);
                //result.OriginalPrice = (oPriceList == null || oPriceList[0] == null
                //            ? 0m : decimal.Round(decimal.Parse(oPriceList[0].ToString()), 2));
            }
            if (result.CategoryType == GroupBuyingCategoryType.Virtual)
            {
                result.VendorStoreSysNoList = m_GroupBuyingDA.GetGroupBuyingVendorStores(result.SysNo.Value);
            }
            return result;
        }

        public List<ProductPromotionDiscountInfo> LoadGroupBuyingPriceByProductSysNo(int productSysNo, GroupBuyingStatus gbStatus)
        {
            return m_GroupBuyingDA.GetProductGroupBuyingPriceByProductSysNo(productSysNo, gbStatus);
        }

        public Dictionary<int, string> GetGroupBuyingTypes()
        {
            return ObjectFactory<IGroupBuyingDA>.Instance.GetGroupBuyingTypes();
        }

        public Dictionary<int, string> GetGroupBuyingAreas()
        {
            return ObjectFactory<IGroupBuyingDA>.Instance.GetGroupBuyingAreas();
        }

        public Dictionary<int, string> GetGroupBuyingVendors()
        {
            return ObjectFactory<IGroupBuyingDA>.Instance.GetGroupBuyingVendors();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GroupBuyingInfo Create(GroupBuyingInfo info)
        {
            // 验证团购信息
            //CheckGroupBuyingBase(info);
            ValidateEntity(info);

            // 验证阶梯价格
            CheckPriceRank(info.PriceRankList, info.GroupBuyingTypeSysNo);

            TransactionScopeFactory.TransactionAction(() =>
            {
                if (info.CategoryType != GroupBuyingCategoryType.ZeroLottery)
                {
                    info.LotteryRule = string.Empty;
                }
                if (info.CategoryType != GroupBuyingCategoryType.Virtual)
                {
                    info.CouponValidDate = null;
                }
                // 创建团购信息
                info.SysNo = m_GroupBuyingDA.Create(info);

                if (info.VendorStoreSysNoList != null && info.CategoryType == GroupBuyingCategoryType.Virtual)
                {
                    info.VendorStoreSysNoList.ForEach(p =>
                    {
                        m_GroupBuyingDA.CreateGroupBuyingActivityRel(info.SysNo.Value, p);
                    });
                }

                // 循环创建阶梯价格
                foreach (var item in info.PriceRankList)
                {
                    if (item.MinQty == null || item.MinQty < 1) { continue; }
                    m_GroupBuyingDA.CreateProductGroupBuyingPrice(info.SysNo.Value, item.MinQty, item.DiscountValue, info.GroupBuyingPoint, info.CostAmt);
                }

                //将团购信息写入到促销引擎中，写入时促销活动是停止状态，审核通过后变成运行状态。
                //ObjectFactory<GroupBuyingPromotionEngine>.Instance.CreateNewGroupBuyingActivity(info);

                // 发送待办消息
                EventPublisher.Publish<GroupBuySaveMessage>(new GroupBuySaveMessage
                {
                    GroupBuySysNo = info.SysNo.Value,
                    GroupBuyName = info.GroupBuyingTitle.Content,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

            });
            ExternalDomainBroker.CreateOperationLog(BizLogType.GroupBuyingCreate.ToEnumDesc(), BizLogType.GroupBuyingCreate, info.SysNo.Value, info.CompanyCode);

            return info;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        public GroupBuyingInfo Update(GroupBuyingInfo info)
        {
            // 验证团购信息
            //CheckGroupBuyingBase(info);
            ValidateEntity(info);

            // 验证阶梯价格
            CheckPriceRank(info.PriceRankList, info.GroupBuyingTypeSysNo);

            TransactionScopeFactory.TransactionAction(() =>
            {
                if (info.CategoryType != GroupBuyingCategoryType.ZeroLottery)
                {
                    info.LotteryRule = string.Empty;
                }
                if (info.CategoryType != GroupBuyingCategoryType.Virtual)
                {
                    info.CouponValidDate = null;
                }
                // 更新团购信息
                m_GroupBuyingDA.Update(info);

                m_GroupBuyingDA.DeleteGroupBuyingActivityRel(info.SysNo.Value);

                if (info.VendorStoreSysNoList != null && info.CategoryType == GroupBuyingCategoryType.Virtual)
                {
                    info.VendorStoreSysNoList.ForEach(p =>
                    {
                        m_GroupBuyingDA.CreateGroupBuyingActivityRel(info.SysNo.Value, p);
                    });
                }

                //先删除该团购的阶梯价格
                m_GroupBuyingDA.DeleteProductGroupBuyingPrice(info.SysNo.Value);
                // 循环更新阶梯价格
                foreach (var price in info.PriceRankList)
                {
                    if (price.MinQty == null || price.MinQty < 1) { continue; }
                    price.ProductSysNo = m_GroupBuyingDA.CreateProductGroupBuyingPrice(info.SysNo.Value, price.MinQty, price.DiscountValue, info.GroupBuyingPoint, info.CostAmt);
                }

                //更新活动状态 
                //ObjectFactory<GroupBuyingPromotionEngine>.Instance.CreateNewGroupBuyingActivity(info);

                // 发送待办消息
                EventPublisher.Publish<GroupBuyUpdateMessage>(new GroupBuyUpdateMessage
                {
                    GroupBuySysNo = info.SysNo.Value,
                    GroupBuyName = info.GroupBuyingTitle.Content,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
            });

            ExternalDomainBroker.CreateOperationLog(BizLogType.GroupBuyingUpdate.ToEnumDesc(), BizLogType.GroupBuyingUpdate, info.SysNo.Value, info.CompanyCode);

            return info;
        }
        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Void(List<int> sysNoList)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);


            foreach (int sysNo in sysNoList)
            {
                GroupBuyingInfo infoTemp = Load(sysNo);
                if (infoTemp.Status.Value != GroupBuyingStatus.Pending)
                {
                    //throw new BizException(string.Format("团购[{0}]作废失败，只有就绪状态的团购才能作废！",sysNo));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_JustReadyCanDel"), sysNo));
                }

                TransactionScopeFactory.TransactionAction(() =>
                {
                    m_GroupBuyingDA.UpdataSatus(sysNo, "D", userfullname);
                    //同步Seller Portal团购状态
                    infoTemp.Status = GroupBuyingStatus.VerifyFaild;
                    m_GroupBuyingDA.SyncGroupBuyingStatus(infoTemp);
                    ExternalDomainBroker.CreateOperationLog(BizLogType.GroupBuyingVoid.ToEnumDesc(), BizLogType.GroupBuyingVoid, sysNo, infoTemp.CompanyCode);

                    //促销引擎-更新活动状态 -作废
                    //ObjectFactory<GroupBuyingPromotionEngine>.Instance.UpdateGroupBuyingActivityStatus(sysNo, GroupBuyingStatus.Deactive);

                    // 发送待办消息
                    EventPublisher.Publish<GroupBuyVoidMessage>(new GroupBuyVoidMessage
                    {
                        GroupBuySysNo = sysNo,
                        CurrentUserSysNo = ServiceContext.Current.UserSysNo
                    });
                });
            }
        }
        /// <summary>
        /// 中止
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Stop(List<int> sysNoList)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            foreach (int sysNo in sysNoList)
            {
                GroupBuyingInfo infoTemp = Load(sysNo);
                if (infoTemp.Status.Value != GroupBuyingStatus.Active)
                {
                    //throw new BizException(string.Format("团购[{0}]中止失败，只有运行状态的团购才能作废！", sysNo));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "roupBuying_JustRunStop"), sysNo));
                }
                TransactionScopeFactory.TransactionAction(() =>
                {
                    m_GroupBuyingDA.UpdateGroupBuyingEndDate(sysNo, userfullname);

                    //促销引擎-更新活动状态 -中止即为完成
                    //ObjectFactory<GroupBuyingPromotionEngine>.Instance.UpdateGroupBuyingActivityStatus(sysNo, GroupBuyingStatus.Finished);

                    ExternalDomainBroker.CreateOperationLog(BizLogType.GroupBuyingStop.ToEnumDesc(), BizLogType.GroupBuyingStop, sysNo, infoTemp.CompanyCode);
                });
            }
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void SubmitAudit(int sysNo)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            GroupBuyingInfo infoTemp = Load(sysNo);
            if (infoTemp.Status.Value != GroupBuyingStatus.Init && infoTemp.Status.Value != GroupBuyingStatus.VerifyFaild)
            {
                //throw new BizException(string.Format("团购[{0}]提交审核失败，只有初始状态的团购才能提交审核！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_JustInitCanSubmit"), sysNo));
            }
            if (infoTemp.Status.Value == GroupBuyingStatus.WaitingAudit)
            {
                //throw new BizException(string.Format("团购[{0}]信息状态已变更，请刷新后再试！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_TryAgainAfterF5"), sysNo));
            }

            TransactionScopeFactory.TransactionAction(() =>
            {
                m_GroupBuyingDA.UpdataSatus(sysNo, "W", userfullname);

                // 发送待办消息
                EventPublisher.Publish<GroupBuyAuditMessage>(new GroupBuyAuditMessage
                {
                    GroupBuySysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
            });
            ExternalDomainBroker.CreateOperationLog(BizLogType.GroupBuyingSubmitAudit.ToEnumDesc(), BizLogType.GroupBuyingSubmitAudit, sysNo, infoTemp.CompanyCode);
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void CancelAudit(int sysNo)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            GroupBuyingInfo infoTemp = Load(sysNo);
            if (infoTemp.Status.Value != GroupBuyingStatus.WaitingAudit)
            {
                //throw new BizException(string.Format("团购[{0}]撤销审核失败，只有待审核状态的团购才能撤销审核！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_JustFaileCanCancel"), sysNo));
            }
            if (infoTemp.Status.Value == GroupBuyingStatus.Init)
            {
                //throw new BizException(string.Format("团购[{0}]信息状态已变更，请刷新后再试！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_TryAgainAfterF5"), sysNo));
            }
            m_GroupBuyingDA.UpdataSatus(sysNo, "O", userfullname);
            ExternalDomainBroker.CreateOperationLog(BizLogType.GroupBuyingCancelAudit.ToEnumDesc(), BizLogType.GroupBuyingCancelAudit, sysNo, infoTemp.CompanyCode);
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void AuditApprove(int sysNo, string reasonStr)
        {
            GroupBuyingInfo infoTemp = Load(sysNo);
            if (infoTemp.Status == GroupBuyingStatus.WaitingAudit)
            {
                CheckGroupBuyAndCountDownConflict(infoTemp);
            }

            if (infoTemp.Status.Value != GroupBuyingStatus.WaitingAudit)
            {
                //throw new BizException(string.Format("团购[{0}]审核通过失败，只有待审核状态的团购才能审核通过！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_JustWaitCanAuditPass"), sysNo));
            }
            if (infoTemp.Status.Value == GroupBuyingStatus.Pending || infoTemp.Status.Value == GroupBuyingStatus.WaitHandling)
            {
                //throw new BizException(string.Format("团购[{0}]信息状态已变更，请刷新后再试！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_TryAgainAfterF5"), sysNo));
            }
            if (string.IsNullOrWhiteSpace(reasonStr))
            {
                //throw new BizException(string.Format("审核理由不能为空！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_AuditPassReasonIsNull"), sysNo));
            }
            //if (infoTemp.RequestSysNo == null || infoTemp.RequestSysNo.Value < 1)
            //{
            infoTemp.Status = GroupBuyingStatus.Pending;  //David:不管商家商品或是中商商品，审核通过全部更改为就绪
            //}
            //else
            //{
            //    //商家商品
            //    infoTemp.Status = GroupBuyingStatus.WaitHandling;
            //}
            infoTemp.AuditUser = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            //if (infoTemp.InUser == infoTemp.AuditUser)
            //{
            //    throw new BizException(string.Format("团购[{0}]审核操作时，审核人不能与创建人相同！", sysNo));
            //}
            infoTemp.Reasons = reasonStr;

            TransactionScopeFactory.TransactionAction(() =>
            {
                m_GroupBuyingDA.UpdateProductGroupBuyingStatus(infoTemp);

                //促销引擎-更新活动状态 -审核通过即为运行
                //ObjectFactory<GroupBuyingPromotionEngine>.Instance.UpdateGroupBuyingActivityStatus(sysNo, GroupBuyingStatus.Active);

                ExternalDomainBroker.CreateOperationLog(BizLogType.GroupBuyingAuditApprove.ToEnumDesc(), BizLogType.GroupBuyingAuditApprove, sysNo, infoTemp.CompanyCode);
            });
        }

        /// <summary>
        /// 审核拒绝
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void AuditRefuse(int sysNo, string reasonStr)
        {
            GroupBuyingInfo infoTemp = Load(sysNo);
            if (infoTemp.Status.Value != GroupBuyingStatus.WaitingAudit)
            {
                //throw new BizException(string.Format("团购[{0}]审核拒绝失败，只有待审核状态的团购才能审核拒绝！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_JustWaitCanRefuse"), sysNo));
            }
            if (infoTemp.Status.Value == GroupBuyingStatus.Deactive)
            {
                //throw new BizException(string.Format("团购[{0}]信息状态已变更，请刷新后再试！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_TryAgainAfterF5"), sysNo));
            }
            if (string.IsNullOrWhiteSpace(reasonStr))
            {
                //throw new BizException(string.Format("审核理由不能为空！", sysNo));
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_AuditPassReasonIsNull"));
            }
            if (infoTemp.RequestSysNo == null || infoTemp.RequestSysNo.Value < 1)
            {
                infoTemp.Status = GroupBuyingStatus.VerifyFaild;
            }
            else
            {
                //商家商品
                infoTemp.Status = GroupBuyingStatus.Deactive;
            }
            infoTemp.AuditUser = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            //if (infoTemp.InUser == infoTemp.AuditUser)
            //{
            //    throw new BizException(string.Format("团购[{0}]审核操作时，审核人不能与创建人相同！", sysNo));
            //}
            infoTemp.Reasons = reasonStr;

            TransactionScopeFactory.TransactionAction(() =>
            {
                m_GroupBuyingDA.UpdateProductGroupBuyingStatus(infoTemp);

                //促销引擎-更新活动状态 -审核拒绝即为作废
                //ObjectFactory<GroupBuyingPromotionEngine>.Instance.UpdateGroupBuyingActivityStatus(sysNo, GroupBuyingStatus.Deactive);

                //同步Seller Portal团购状态
                infoTemp.Status = GroupBuyingStatus.VerifyFaild;
                m_GroupBuyingDA.SyncGroupBuyingStatus(infoTemp);
                ExternalDomainBroker.CreateOperationLog(BizLogType.GroupBuyingAuditRefuse.ToEnumDesc(), BizLogType.GroupBuyingAuditRefuse, sysNo, infoTemp.CompanyCode);
            });
        }

        /// <summary>
        /// 根据商品编号获取正在参加团购的商品编号
        /// </summary>
        /// <param name="products">待验证的商品编号</param>
        /// <returns>正在参加团购的商品编号</returns>
        public virtual List<int> GetProductsOnGroupBuying(IEnumerable<int> products)
        {
            return ObjectFactory<IGroupBuyingDA>.Instance.GetProductsOnGroupBuying(products);
        }

        public virtual List<object> GetProductOriginalPriceList(int productSysNo, string isByGroup, string companyCode)
        {
            return m_GroupBuyingDA.GetProductOriginalPrice(productSysNo, isByGroup, companyCode);
        }

        #region 团购业务验证规则
        /// <summary>
        /// Check团购基本信息
        /// </summary>
        /// <param name="entity"></param>
        public static void CheckGroupBuyingBase(GroupBuyingInfo entity)
        {
            if (entity.GroupBuyingAreaSysNo < 1)
            {
                //throw new BizException("请先选择城市！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_CityIsNotNull"));
            }
            if (entity.GroupBuyingTypeSysNo < 0)
            {
                //throw new BizException("请先选择分类！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_CategoryIsNotNull"));
            }
        }

        /// <summary>
        /// 团购业务规则
        /// </summary>
        /// <param name="entity"></param>
        private void ValidateEntity(GroupBuyingInfo entity)
        {



            // TODO:验证商品是否存在

            // 验证团购时间
            if (entity.BeginDate > entity.EndDate)
            {
                // throw new BizException(string.Format("团购开始时间晚于结束时间！", entity.ProductSysNo.Value, entity.BeginDate.Value, entity.EndDate.Value));
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_StartLaterEndDate"));
            }
            if (entity.GroupBuyingTypeSysNo != 6)
            {
                // 验证商品是否存在相冲突时间段的团购中
                CheckGroupBuyConflict(entity.SysNo, entity);

                // 验证商品是否存在相冲突时间段的限时抢购中
                CheckGroupBuyAndCountDownConflict(entity);
            }
            else
            {
                entity.ProductID = "";
                entity.ProductSysNo = 0;
                entity.GroupBuyingVendorName = "";
                entity.GroupBuyingVendorSysNo = 0;
                entity.PriceRankList[0].DiscountValue = 0m;
                entity.PriceRankList = new List<PSPriceDiscountRule>() { entity.PriceRankList[0] };
            }
        }
        /// <summary>
        /// 阶梯价格业务规则
        /// </summary>
        /// <param name="PriceRankList"></param>
        private void CheckPriceRank(List<PSPriceDiscountRule> PriceRankList, int? gbTypeSysNo)
        {
            if (gbTypeSysNo != 6)
            {
                for (int i = 0; i < PriceRankList.Count; i++)
                {
                    if (PriceRankList[i].MinQty <= 0 || PriceRankList[i].DiscountValue <= 0)
                    {
                        //throw new BizException("阶梯数量与价格必需同时设定！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_LadderQuntityAndPriceSetSameTime"));
                    }

                    for (int j = i + 1; j < PriceRankList.Count; j++)
                    {
                        if (PriceRankList[j].MinQty <= PriceRankList[i].MinQty)
                        {
                            //throw new BizException("阶梯数量必需逐级递增加！");
                            throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_LadderQuntityGraduAdd"));
                        }

                        if (PriceRankList[j].DiscountValue >= PriceRankList[i].DiscountValue)
                        {
                            //throw new BizException("阶梯价格必需逐级递减！");
                            throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_LadderPriceGraduReduce"));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 验证商品是否存在相冲突时间段的团购中
        /// </summary>
        private void CheckGroupBuyConflict(int? GroupBuySysNo, GroupBuyingInfo groupBuy)
        {
            List<int> productSysNos = new List<int>();
            productSysNos.Add(groupBuy.ProductSysNo.Value);

            if (groupBuy.IsByGroup.Value)
            {
                List<ProductInfo> products = ExternalDomainBroker.GetProductsInSameGroupWithProductSysNo(groupBuy.ProductSysNo.Value);

                foreach (ProductInfo p in products)
                {
                    productSysNos.Add(p.SysNo);
                }
            }

            if (m_GroupBuyingDA.CheckConflict(GroupBuySysNo, productSysNos, groupBuy.BeginDate.Value, groupBuy.EndDate.Value))
            {
                //throw new BizException("该商品或者同组商品已经存在一个时间冲突的团购！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_ExsistTimeConflictGroupBuying"));
            }
        }

        /// <summary>
        /// 验证商品是否存在相冲突时间段的团购中
        /// </summary>
        /// <param name="productsysno"></param>
        /// <param name="begindate"></param>
        /// <param name="enddate"></param>
        /// <returns>存在：True；不存在：False</returns>
        public bool CheckGroupBuyConflict(int productsysno, DateTime begindate, DateTime enddate)
        {
            List<int> productSysNos = new List<int>();
            productSysNos.Add(productsysno);

            return m_GroupBuyingDA.CheckConflict(null, productSysNos, begindate, enddate);
        }

        /// <summary>
        /// 验证商品是否存在相冲突时间段的限时抢购中
        /// </summary>
        private void CheckGroupBuyAndCountDownConflict(GroupBuyingInfo groupBuy)
        {
            List<int> productSysNos = new List<int>();
            productSysNos.Add(groupBuy.ProductSysNo.Value);

            if (groupBuy.IsByGroup.Value)
            {
                List<ProductInfo> products = ExternalDomainBroker.GetProductsInSameGroupWithProductSysNo(groupBuy.ProductSysNo.Value);

                foreach (ProductInfo p in products)
                {
                    productSysNos.Add(p.SysNo);
                }
            }

            if (ObjectFactory<CountdownProcessor>.Instance.CheckGroupBuyAndCountDownConflict(productSysNos, groupBuy.BeginDate.Value, groupBuy.EndDate.Value))
            {
                // throw new BizException("该商品或者同组商品已经存在一个时间冲突的限时抢购！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_ExsistTimeConflictCountdown"));
            }
        }
        #endregion

        #region IPromotionActivityJob
        public void ActivityStatusProcess()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Job 相关

        /// <summary>
        /// 取得没有处理的团购信息
        /// </summary>
        /// <param name="companyCode">如果为null,表示取得所有没有处理的团购信息</param>
        /// <returns></returns>
        public List<GroupBuyingInfo> GetGroupBuyInfoForNeedProcess(string companyCode)
        {
            return m_GroupBuyingDA.GetGroupBuyInfoForNeedProcess(companyCode);
        }

        /// <summary>
        /// 修改团购处理状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="settlementStatus"></param>
        public void UpdateGroupBuySettlementStatus(int sysNo, ECCentral.BizEntity.MKT.GroupBuyingSettlementStatus settlementStatus)
        {
            m_GroupBuyingDA.UpdateGroupBuySettlementStatus(sysNo, settlementStatus);
        }

        public List<GroupBuyingInfo> GetGroupBuyingList(int groupBuyingSysNo, int companyCode)
        {
            return m_GroupBuyingDA.GetGroupBuyingList(groupBuyingSysNo, companyCode);
        }
        #endregion

        #region "check随心配在团购最低阶梯价的毛利率"

        /// <summary>
        /// 得到随心配的商品在团购里的毛利率
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetProductPromotionMarginByGroupBuying(GroupBuyingInfo info)
        {
            ProductInfo productInfo = ExternalDomainBroker.GetProductInfo(info.ProductSysNo.Value);
            ProductPriceRequestInfo priceMsg = new ProductPriceRequestInfo()
            {
                CurrentPrice = info.PriceRankList.OrderBy(s => s.DiscountValue).First().DiscountValue,//最低阶梯价格
                UnitCost = productInfo.ProductPriceInfo.UnitCost,
                Point = info.GroupBuyingPoint,
                Category = productInfo.ProductBasicInfo.ProductCategoryInfo

            };
            string returnMsgStr = string.Empty;
            StringBuilder checkMsg = new StringBuilder();
            List<ProductPromotionMarginInfo> marginList = ObjectFactory<IIMBizInteract>.Instance.GetProductPromotionMargin(
                                             priceMsg, info.ProductSysNo.Value, "", 0m, ref returnMsgStr);
            marginList = marginList.Where(ppm => ppm.PromotionType == PromotionType.OptionalAccessories).ToList();

            foreach (var mgInfo in marginList)
            {
                //checkMsg.Append(string.Format("此商品的最低阶梯价格在在随心配{0}中毛利率{1}%\r", mgInfo.ReferenceSysNo
                //    , (Decimal.Round(mgInfo.Margin, 4) * 100m).ToString("0.00")));
                checkMsg.Append(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_LowestGrossMagin"), mgInfo.ReferenceSysNo
                    , (Decimal.Round(mgInfo.Margin, 4) * 100m).ToString("0.00")));
            }
            return checkMsg.ToString();
        }
        #endregion

        public GroupBuyingCategoryInfo CreateGroupBuyingCategory(GroupBuyingCategoryInfo entity)
        {
            PreCheck(entity);

            return m_GroupBuyingDA.CreateGroupBuyingCategory(entity);
        }

        public GroupBuyingCategoryInfo UpdateGroupBuyingCategory(GroupBuyingCategoryInfo entity)
        {
            if ((entity.SysNo ?? 0) == 0)
            {
                throw new ArgumentException("entity.SysNo");
            }
            PreCheck(entity);

            return m_GroupBuyingDA.UpdateGroupBuyingCategory(entity);
        }

        public List<GroupBuyingCategoryInfo> GetAllGroupBuyingCategory()
        {
            return m_GroupBuyingDA.GetAllGroupBuyingCategory();
        }

        private void PreCheck(GroupBuyingCategoryInfo entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                //throw new BizException("类别名称不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_CategoryNameIsNotNull"));
            }
            if (m_GroupBuyingDA.CheckGroupBuyingCategoryNameExists(entity.Name, entity.SysNo ?? 0, entity.CategoryType.Value))
            {
                //throw new BizException(string.Format("类别[{0}]已经存在！", entity.Name));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_AlreadyExsitCategory"), entity.Name));
            }
        }

        public void ReadGroupBuyingFeedback(int sysNo)
        {
            m_GroupBuyingDA.ReadGroupbuyingFeedback(sysNo);
        }

        public void HandleGroupbuyingBusinessCooperation(int sysNo)
        {
            m_GroupBuyingDA.HandleGroupbuyingBusinessCooperation(sysNo);
        }

        public GroupBuyingSettlementInfo LoadGroupBuyingSettleBySysNo(int sysno)
        {
            return m_GroupBuyingDA.LoadGroupBuyingSettleBySysNo(sysno);
        }

        /// <summary>
        /// 审核团购结算单
        /// </summary>
        /// <param name="sysNo"></param>
        public void AuditPassGroupBuyingSettlement(int sysNo)
        {
            if (sysNo <= 0)
            {
                throw new ArgumentException("sysNo");
            }
            var origin = m_GroupBuyingDA.LoadGroupBuyingSettleBySysNo(sysNo);
            if (origin == null)
            {
                //throw new BizException(string.Format("团购结算单[{0}]不存在！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying","GroupBuying_NotExsitGroupBuyingBalanceSheet"), sysNo));
            }
            if (origin.Status == SettlementBillStatus.Settled)
            {
                //throw new BizException(string.Format("团购结算单已经审核，不能重复审核！", sysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_BalanceSheetAlreadyAudit"), sysNo));
            }
            using (TransactionScope scope = new TransactionScope())
            {
                m_GroupBuyingDA.UpdateGroupBuyingSettlementStatus(sysNo, SettlementBillStatus.Settled);
                //生成付款单
                ExternalDomainBroker.CreatePayItem(new PayItemInfo()
                {
                    OrderSysNo = origin.SysNo,
                    PayAmt = origin.SettleAmt,
                    OrderType = PayableOrderType.GroupSettle,
                    PayStyle = PayItemStyle.Normal,
                    CompanyCode = "8601"
                });
                scope.Complete();
            }
        }

        public DataTable LoadGroupBuyingSettlementItemBySettleSysNo(int sysno)
        {
            return ObjectFactory<IGroupBuyingQueryDA>.Instance.LoadGroupBuyingSettlementItemBySettleSysNo(sysno);
        }

        public DataTable LoadTicketByGroupBuyingSysNo(int groupBuyingSysNo)
        {
            return ObjectFactory<IGroupBuyingQueryDA>.Instance.LoadTicketByGroupBuyingSysNo(groupBuyingSysNo);
        }

        public void VoidGroupBuyingTicket(int sysNo)
        {
            if (sysNo <= 0)
            {
                throw new ArgumentException("sysNo");
            }
            var origin = m_GroupBuyingDA.LoadGroupBuyingTicketBySysNo(sysNo);
            if (origin == null)
            {
                //throw new BizException(string.Format("团购活动券[{0}]不存在！", origin.TicketID));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying","GroupBuying_NotExsistGroupBuyingTicket"), origin.TicketID));
            }
            if (origin.Status == GroupBuyingTicketStatus.Used || origin.Status == GroupBuyingTicketStatus.Abandon)
            {
                //throw new BizException(string.Format("团购活动券[{0}]已经使用或作废，不能作废！", origin.TicketID));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_AlreadyUseGroupBuyingTicket"), origin.TicketID));
            }
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            { 
                if (origin.Status == GroupBuyingTicketStatus.Created)
                {
                    m_GroupBuyingDA.UpdateGroupBuyingTicketStatus(sysNo, GroupBuyingTicketStatus.Abandon);
                }
                else
                {
                    var soIncomeInfo = new SOIncomeInfo()
                    {
                        OrderSysNo = origin.SysNo.Value,
                        OrderType = SOIncomeOrderType.GroupRefund,
                        OrderAmt = origin.TicketAmt.Value * -1,
                        IncomeAmt = origin.TicketAmt.Value * -1,
                        GiftCardPayAmt = 0,
                        PointPay = 0,
                        IncomeStyle = SOIncomeOrderStyle.Advanced,
                        //ReferenceID = origin.GroupBuyingSysNo.ToString(),
                        Status = SOIncomeStatus.Origin,
                        Note = "虚拟团购负收款单",
                        CompanyCode = origin.CompanyCode,
                        MasterSoSysNo = origin.OrderSysNo,
                    };
                    //生成付款单
                    ExternalDomainBroker.CreateSOIncome(soIncomeInfo);
                    var item = new GroupBuyingTicketInfo
                        {
                            SysNo = origin.SysNo,
                            RefundDate = DateTime.Now,
                            RefundStatus = RefundStatus.Origin,
                            RefundMemo = "退款且作废团购券",
                            Status = GroupBuyingTicketStatus.Abandon

                        };
                    m_GroupBuyingDA.UpdateGroupBuyingTicketRefundInfo(item);
                }
                scope.Complete();
            }

        }
    }
}
