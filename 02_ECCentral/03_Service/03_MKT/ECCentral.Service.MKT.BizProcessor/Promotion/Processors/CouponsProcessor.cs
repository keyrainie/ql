using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity.MKT;
using System.ComponentModel.Composition;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.MKT.IDataAccess.Promotion;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using System.Transactions;
using ECCentral.BizEntity.SO;
using System.Data;
using ECCentral.BizEntity.MKT.Promotion.Calculate;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage.MKT;

namespace ECCentral.Service.MKT.BizProcessor
{
    /// <summary>
    /// 优惠券BizProcessor
    /// </summary>
    [Export(typeof(IPromotionCalculate))]
    [Export(typeof(IPromotionActivityJob))]
    [VersionExport(typeof(CouponsProcessor))]
    public class CouponsProcessor : CalculateBaseProcessor, IPromotionCalculate, IPromotionActivityJob
    {
        private ICouponsDA _da = ObjectFactory<ICouponsDA>.Instance;

        #region 计算类、应用行为
        /// <summary>
        /// 计算订单的促销优惠结果
        /// </summary>
        /// <param name="soInfo"></param>
        /// <returns></returns>
        public virtual List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, List<SOPromotionInfo> alreadyApplyPromoList)
        {
            return CalculateSOPromotion(soInfo, true);
        }

        /// <summary>
        /// 计算订单的促销优惠结果
        /// </summary>
        /// <param name="soInfo"></param>
        /// <returns></returns>
        public virtual List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, bool isModifyCoupons)
        {
            List<SOPromotionInfo> promotionInfoList = new List<SOPromotionInfo>();

            if (soInfo.Items == null || soInfo.Items.Count == 0)
            {
                return promotionInfoList;
            }
            //目前只处理单张优惠券，如果以后有多张，soInfo.CouponCode里用逗号分隔，则只需要Split后循环即可
            if (string.IsNullOrEmpty(soInfo.CouponCode) || soInfo.CouponCode.Trim() == "")
            {
                return promotionInfoList;
            }

            if (!soInfo.BaseInfo.CustomerSysNo.HasValue)
            {
                //throw new BizException("订单信息异常：请在订单信息中传入客户信息！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_MissingCustomerInfo"));
            }
            CouponCodeSetting codeInfo;
            var modifySo = false;
            if (soInfo.SysNo == null || soInfo.SysNo <= 0 || isModifyCoupons)
            {
                //1.根据优惠券号码获取有效的优惠活动的号码信息
                codeInfo = _da.GetActivedCouponCodeInfoByCode(soInfo.CouponCode);
            }
            else
            {
                codeInfo = _da.GetCouponCodeInfoByCode(soInfo.CouponCode);
                modifySo = true;
            }

            if (codeInfo == null)
            {
                //throw new BizException("优惠券应用失败：该优惠券号码无效，或者该优惠券所属活动还未开始或者已经全部结束！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_InvalidNum"));
            }
            //2.根据优惠活动SysNo再Load整个优惠券活动的所有信息
            CouponsInfo couponsInfo = Load(codeInfo.CouponsSysNo);

            //3.判断本优惠券活动本身是否有效以及当前Order是否满足优惠券活动条件   
            #region Check Condition
            //(1)时间条件
            if ((couponsInfo.StartTime > DateTime.Now || couponsInfo.EndTime < DateTime.Now) && !modifySo)
            {
                //throw new BizException("优惠券应用失败：优惠券不可用，优惠券不在有效期内！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OutInvalidDate"));
            }
            //(2)订单条件
            List<string> errListOrderCondition = base.CheckOrderCodition(soInfo, couponsInfo.OrderCondition);
            if (errListOrderCondition.Count > 0)
            {
                // throw new BizException("优惠券应用失败："+ errListOrderCondition[0]);
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ApplyFaild") + errListOrderCondition[0]);
            }
            //(3)客户条件
            List<string> errListCustomerCondition = base.CheckCustomerCondition(soInfo, couponsInfo.CustomerCondition);
            if (errListCustomerCondition.Count > 0)
            {
                //throw new BizException("优惠券应用失败：" + errListCustomerCondition[0]);
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ApplyFaild") + errListCustomerCondition[0]);
            }

            //(4)使用次数条件
            if (couponsInfo.UsingFrequencyCondition != null && !modifySo)
            {
                DataRow rowUsedCount = _da.GetCouponsUsedCount(soInfo.BaseInfo.CustomerSysNo, couponsInfo.SysNo, soInfo.CouponCode);
                couponsInfo.UsingFrequencyCondition.UsedFrequency = Convert.ToInt32(rowUsedCount["TotalCount"]);
                couponsInfo.UsingFrequencyCondition.CustomerUsedFrequency = Convert.ToInt32(rowUsedCount["CustomerTotalCount"]);
                //活动参与次数的检查
                List<string> errListFrequencyCondition = base.CheckActivityFrequencyCondition(couponsInfo.UsingFrequencyCondition);
                if (errListFrequencyCondition.Count > 0)
                {
                    //throw new BizException("优惠券应用失败：" + errListFrequencyCondition[0]);
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ApplyFaild") + errListFrequencyCondition[0]);
                }
                //本优惠券使用次数的检查
                int CCTotalCount = Convert.ToInt32(rowUsedCount["CCTotalCount"]);
                int CCCustomerTotalCount = Convert.ToInt32(rowUsedCount["CCCustomerTotalCount"]);
                if (codeInfo.CCCustomerMaxFrequency.HasValue && CCCustomerTotalCount >= codeInfo.CCCustomerMaxFrequency.Value)
                {
                    //throw new BizException("优惠券应用失败：本券的使用次数已达到单客户使用次数的上限！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OverCostomerTopLimit"));
                }
                if (codeInfo.CCMaxFrequency.HasValue && CCTotalCount >= codeInfo.CCMaxFrequency.Value && codeInfo.CCMaxFrequency.Value > 0)
                {
                    //throw new BizException("优惠券应用失败：本券的使用次数已达到全网使用次数的上限！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OVerTotalTopLimit"));
                }
            }
            //(5)Check触发方式为：注册，生日，支付宝金账户，Check是否存在发放记录
            if (couponsInfo.BindCondition.Value != CouponsBindConditionType.None)
            {
                if (!_da.CheckExistCustomerIssueLog(soInfo.BaseInfo.CustomerSysNo, soInfo.CouponCode))
                {
                    // throw new BizException(string.Format("优惠券应用失败：本券的发放触发方式是{0}，但是没有找到该客户的发放记录！", couponsInfo.BindCondition.Value.ToEnumDesc()));
                    throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_NoIssueRecord"), couponsInfo.BindCondition.Value.ToEnumDesc()));
                }
            }
            //(6)对促销规则进行检查（
            if ((couponsInfo.PriceDiscountRule == null || couponsInfo.PriceDiscountRule.Count == 0)
                && (couponsInfo.OrderAmountDiscountRule == null || couponsInfo.OrderAmountDiscountRule.OrderAmountDiscountRank == null || couponsInfo.OrderAmountDiscountRule.OrderAmountDiscountRank.Count == 0))
            {
                // throw new BizException("优惠券应用失败：本券的活动没有设置折扣规则！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_NoDiscountRule"));
            }

            //(7)最后进行商品条件检查并得到可参与活动的商品列表。如果商品范围是所有商品，那么订单中所有商品都是可以参与活动
            List<SOItemInfo> canPromotionSOItemList = new List<SOItemInfo>();
            if (couponsInfo.ProductRangeType.Value != CouponsProductRangeType.AllProducts)
            {
                CondProduct condProduct = new CondProduct();
                condProduct.CondVendor = couponsInfo.ProductCondition.ListRelVendor;

                if (couponsInfo.ProductCondition.RelBrands != null && couponsInfo.ProductCondition.RelBrands.BrandList != null)
                {
                    condProduct.CondBrand = new ConditionBase<List<int>>();
                    if (couponsInfo.ProductCondition.RelBrands.IsIncludeRelation.HasValue
                        && couponsInfo.ProductCondition.RelBrands.IsIncludeRelation.Value)
                    {
                        condProduct.CondBrand.AndOrTypeRelation = AndOrType.Or;
                    }
                    else
                    {
                        condProduct.CondBrand.AndOrTypeRelation = AndOrType.Not;
                    }
                    condProduct.CondBrand.Data = new List<int>();
                    couponsInfo.ProductCondition.RelBrands.BrandList.ForEach(f => condProduct.CondBrand.Data.Add(f.SysNo.Value));
                }

                if (couponsInfo.ProductCondition.RelCategories != null && couponsInfo.ProductCondition.RelCategories.CategoryList != null)
                {
                    condProduct.CondC3 = new ConditionBase<List<int>>();
                    if (couponsInfo.ProductCondition.RelCategories.IsIncludeRelation.HasValue
                        && couponsInfo.ProductCondition.RelCategories.IsIncludeRelation.Value)
                    {
                        condProduct.CondC3.AndOrTypeRelation = AndOrType.Or;
                    }
                    else
                    {
                        condProduct.CondC3.AndOrTypeRelation = AndOrType.Not;
                    }
                    condProduct.CondC3.Data = new List<int>();
                    couponsInfo.ProductCondition.RelCategories.CategoryList.ForEach(f => condProduct.CondC3.Data.Add(f.SysNo.Value));
                }

                if (couponsInfo.ProductCondition.RelProducts != null && couponsInfo.ProductCondition.RelProducts.ProductList != null)
                {
                    condProduct.CondItem = new ConditionBase<List<int>>();
                    if (couponsInfo.ProductCondition.RelProducts.IsIncludeRelation.HasValue
                        && couponsInfo.ProductCondition.RelProducts.IsIncludeRelation.Value)
                    {
                        condProduct.CondItem.AndOrTypeRelation = AndOrType.Or;
                    }
                    else
                    {
                        condProduct.CondItem.AndOrTypeRelation = AndOrType.Not;
                    }
                    condProduct.CondItem.Data = new List<int>();
                    couponsInfo.ProductCondition.RelProducts.ProductList.ForEach(f => condProduct.CondItem.Data.Add(f.ProductSysNo.Value));
                }

                canPromotionSOItemList = base.CheckProductConditionAndGetActivityItemList(soInfo, condProduct);
            }
            else
            {
                canPromotionSOItemList = soInfo.Items;
                //排除非主商品的商品，如赠品，附件等等
                base.RemoveNotSaleMasterProduct(canPromotionSOItemList);
            }
            #endregion

            //4. 添加每个商品的活动优惠结果，填充SOPromotionInfo，SOPromotionDetailInfo
            #region Get Rule
            //根据返回的符合条件的订单商品列表来判断，如果返回列表数量为空，说明没有符合条件的商品
            if (canPromotionSOItemList.Count == 0)
            {
                //throw new BizException("优惠券应用失败：没有符合活动条件的商品！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_NoMatchCondGoods"));
            }
            SOPromotionInfo promotionInfo = new SOPromotionInfo();
            promotionInfo.PromotionType = SOPromotionType.Coupon;
            promotionInfo.PromotionSysNo = codeInfo.CodeSysNo;
            InitPromotionProductInfo(promotionInfo, soInfo, canPromotionSOItemList);

            //计算优惠券最终促销结果
            base.CalcAmountRule(promotionInfo, soInfo, canPromotionSOItemList, couponsInfo);
            if (promotionInfo.DiscountAmount == 0)
            {
                // throw new BizException("优惠券应用失败：不符合折扣规则，最终折扣为0！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_NoMatchDiscountRule"));
            }

            #endregion

            promotionInfoList.Add(promotionInfo);
            return promotionInfoList;
        }


        protected virtual void InitPromotionProductInfo(SOPromotionInfo promotionInfo, SOInfo soInfo,
            List<SOItemInfo> canPromotionSOItemList)
        {
            foreach (SOItemInfo item in canPromotionSOItemList)
            {
                SOItemInfo soItem = soInfo.Items.Find(f => f.ProductSysNo == item.ProductSysNo);
                SOPromotionDetailInfo detail = new SOPromotionDetailInfo();
                detail.MasterProductSysNo = soItem.ProductSysNo;
                //detail.MasterProductType = soItem.ProductType;
                detail.MasterProductQuantity = soItem.Quantity;
                promotionInfo.SOPromotionDetails.Add(detail);
            }
        }



        /// <summary>
        /// 优惠券应用
        /// </summary>
        /// <param name="couponSysNo">优惠券活动系统编号</param>
        /// <param name="couponCode">优惠券代码</param>
        /// <param name="customerSysNo">客户系统编号</param>
        /// <param name="shoppingCartSysNo">购物车系统编号</param>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="redeemAmount">折扣总金额</param>
        public void CouponCodeApply(int couponSysNo, string couponCode, int customerSysNo,
            int shoppingCartSysNo, int soSysNo, decimal redeemAmount)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            _da.CouponCodeApply(couponSysNo, couponCode, customerSysNo,
              shoppingCartSysNo, soSysNo, redeemAmount, userfullname);

        }

        /// <summary>
        /// 已应用的优惠券被Cancel
        /// </summary>
        /// <param name="couponSysNo">优惠券活动系统编号</param>
        /// <param name="couponCode">优惠券代码</param>
        /// <param name="shoppingCartSysNo">购物车系统编号</param>
        /// <param name="soSysNo">订单系统编号</param>
        public void CouponCodeCancel(string couponCode, int soSysNo, int shoppingCartSysNo)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            _da.CouponCodeCancel(couponCode, soSysNo, shoppingCartSysNo, userfullname);
        }
        #endregion

        #region 维护类行为

        #region  全局行为

        /// <summary>
        /// 获取优惠券本身的所有信息，但是不Load优惠券代码信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CouponsInfo Load(int? sysNo)
        {
            CouponsInfo info= _da.Load(sysNo);
            if (info.ProductCondition == null)
                info.ProductCondition = new PSProductCondition();
                info.ProductCondition.ListRelVendor = _da.GetVendorSaleRulesByCouponSysNo((int)sysNo);
            return info;
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
                CouponsInfo info = Load(sysNo);
                if (info == null)
                {
                    //failureRecords.Add(string.Format("活动[{0}]信息加载失败!", sysNo));
                    failureRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ActivityInfoLoadFailed"), sysNo));
                    continue;
                }
                CouponsStatus targetStatus = info.Status.Value;
                string errorDescription = null;
                if (!CheckAndOperateStatus(PSOperationType.SubmitAudit, info.SysNo, info.Status.Value, out targetStatus, out errorDescription))
                {
                    failureRecords.Add(errorDescription);
                    continue;
                }

                #region 如果触发条件是“不限”时需要判断 是否有代码,用户和用户组
                if (info.BindCondition == null)
                {
                    //errorDescription = string.Format("活动[{0}]提交审核失败：没有优惠券绑定规则,不能提交审核！", sysNo);
                    errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_NeedCouponsRule"), sysNo);

                    failureRecords.Add(errorDescription);
                    continue;
                }
                if (info.BindCondition == CouponsBindConditionType.None)
                {
                    //判断是否有优惠券代码
                    if (!_da.CheckCouponCodeIsHave(sysNo))
                    {
                        // errorDescription = string.Format("活动[{0}]提交审核失败：没有优惠券代码,不能提交审核！", sysNo);
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_MissCounponsSys"), sysNo);
                        failureRecords.Add(errorDescription);
                        continue;
                    }
                    //判断是否有用户和用户组
                    if (info.CustomerCondition == null)
                    {
                        //errorDescription = string.Format("活动[{0}]提交审核失败：没有用户或用户组,不能提交审核！", sysNo);
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_MissUsers"), sysNo);
                        failureRecords.Add(errorDescription);
                        continue;
                    }
                }
                #endregion

                if (info.OrderAmountDiscountRule == null)
                {
                    //errorDescription = string.Format("活动[{0}]提交审核失败：没有折扣记录,不能提交审核！", sysNo);
                    errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_NoDiscounRecord"), sysNo);
                    failureRecords.Add(errorDescription);
                    continue;
                }

                if (info.ProductRangeType == CouponsProductRangeType.LimitCategoryBrand)
                {
                    if (info.ProductCondition == null || (info.ProductCondition.RelBrands == null && info.ProductCondition.RelCategories == null&&
                        info.ProductCondition.ListRelVendor==null))
                    {
                        //errorDescription = string.Format("活动[{0}]提交审核失败：没有品牌和分类记录,不能提交审核！", sysNo);
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_NoBrandRecord"), sysNo);
                        failureRecords.Add(errorDescription);
                        continue;
                    }
                }

                if (info.ProductRangeType == CouponsProductRangeType.LimitProduct)
                {
                    if (info.ProductCondition == null || info.ProductCondition.RelProducts == null)
                    {
                        //errorDescription = string.Format("活动[{0}]提交审核失败：没有商品记录,不能提交审核！", sysNo);
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_NoGoodsRecord"), sysNo);
                        failureRecords.Add(errorDescription);
                        continue;
                    }
                }

                TransactionScopeFactory.TransactionAction(() =>
                {
                    _da.UpdateStatus(sysNo, targetStatus, userfullname);

                    // 发送待办消息
                    EventPublisher.Publish<CouponSubmitMessage>(new CouponSubmitMessage
                    {
                        CouponSysNo = sysNo,
                        CouponName = info.Title.Content,
                        CurrentUserSysNo = ServiceContext.Current.UserSysNo
                    });
                });

                successRecords.Add(sysNo.ToString());
           

                ExternalDomainBroker.CreateOperationLog(BizLogType.PromotionSubmit.ToEnumDesc(), BizLogType.PromotionSubmit, sysNo, info.CompanyCode);
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
            //OperateActivity(operation, sysNoList, out successRecords, out failureRecords);       
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);

            foreach (int sysNo in sysNoList)
            {
                CouponsInfo info = LoadMaster(sysNo);
                string errorDescription = null;
                //var result = CheckOperate(info.InUser, out errorDescription);
                //if (!result)
                //{
                //    var message = "优惠券编号:" + Convert.ToString(sysNo) +","+ errorDescription;
                //    failureRecords.Add(message);
                //    continue;
                //}
                if (info == null)
                {
                    //failureRecords.Add(string.Format("活动[{0}]信息加载失败!", sysNo));
                    failureRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ActivityInfoLoadFailed"), sysNo));
                    continue;
                }

                CouponsStatus targeStatus = info.Status.Value;
                
                if (!CheckAndOperateStatus(operation, info.SysNo, info.Status.Value, out targeStatus, out errorDescription))
                {
                    failureRecords.Add(errorDescription);
                    continue;
                }

                using (TransactionScope transcope = TransactionScopeFactory.CreateTransactionScope())
                {
                    _da.Audit(sysNo, targeStatus, userfullname);
                    //successRecords.Add(string.Format("优惠券活动[{0}]处理成功！", sysNo.ToString()));
                    successRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_HandleAceccess"), sysNo.ToString()));

                    // 发送待办消息
                    switch (targeStatus)
                    {
                        // 审核通过
                        case CouponsStatus.Ready:
                            EventPublisher.Publish<CouponAuditMessage>(new CouponAuditMessage
                            {
                                CouponSysNo = info.SysNo.Value,
                                CouponName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                        // 审核拒绝
                        case CouponsStatus.Init:
                            EventPublisher.Publish<CouponAuditRejectMessage>(new CouponAuditRejectMessage
                            {
                                CouponSysNo = info.SysNo.Value,
                                CouponName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                    }
                    transcope.Complete();
                }

                BizLogType logtype = auditType == PromotionAuditType.Approve ? BizLogType.PromotionApprove : BizLogType.PromotionReject;
                ExternalDomainBroker.CreateOperationLog(logtype.ToEnumDesc(),
                    logtype, sysNo, info.CompanyCode);

            }

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
        /// 操作优惠券活动
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
                CouponsInfo info = LoadMaster(sysNo);
                if (info == null)
                {
                    // failureRecords.Add(string.Format("活动[{0}]信息加载失败!", sysNo));
                    failureRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ActivityInfoLoadFailed"), sysNo));
                    continue;
                }

                CouponsStatus targetStatus = info.Status.Value;
                string errorDescription = null;
                if (!CheckAndOperateStatus(operation, info.SysNo, info.Status.Value, out targetStatus, out errorDescription))
                {
                    failureRecords.Add(errorDescription);
                    continue;
                }

                TransactionScopeFactory.TransactionAction(() =>
                {
                    _da.UpdateStatus(sysNo, targetStatus, userfullname);

                    // 发送待办消息
                    switch (operation)
                    {
                        // 审核
                        case PSOperationType.AuditApprove:
                            EventPublisher.Publish<CouponAuditMessage>(new CouponAuditMessage
                            {
                                CouponSysNo = sysNo,
                                CouponName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                        // 取消审核
                        case PSOperationType.CancelAudit:
                            EventPublisher.Publish<CouponAuditCancelMessage>(new CouponAuditCancelMessage
                            {
                                CouponSysNo = sysNo,
                                CouponName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                        case PSOperationType.AuditRefuse:
                            EventPublisher.Publish<CouponAuditRejectMessage>(new CouponAuditRejectMessage
                            {
                                CouponSysNo = sysNo,
                                CouponName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                        case PSOperationType.Void:
                            EventPublisher.Publish<CouponVoidMessage>(new CouponVoidMessage
                            {
                                CouponSysNo = sysNo,
                                CouponName = info.Title.Content,
                                CurrentUserSysNo = ServiceContext.Current.UserSysNo
                            });
                            break;
                    }
                });
                

                //successRecords.Add(string.Format("优惠券活动[{0}]处理成功！", sysNo.ToString()));
                successRecords.Add(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_HandleAceccess"), sysNo.ToString()));


                BizLogType logType = BizLogType.PromotionAdd;
                if (operation == PSOperationType.CancelAudit)
                {
                    logType = BizLogType.PromotionCancel;
                }
                else if (operation == PSOperationType.Stop)
                {
                    logType = BizLogType.PromotionStop;
                }
                else if (operation == PSOperationType.Void)
                {
                    logType = BizLogType.PromotionAbandon;
                }
                if (logType != BizLogType.PromotionAdd)
                {
                    // ExternalDomainBroker.CreateOperationLog("优惠券处理-" + operation.ToString(), logType, sysNo, info.CompanyCode);
                    ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_CouponsHandle") + operation.ToString(), logType, sysNo, info.CompanyCode);
                }
            }
        }



        #endregion

        #region 局部对象处理
        /// <summary>
        /// Load 活动基本信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual CouponsInfo LoadMaster(int? sysNo)
        {
            return _da.LoadMaster(sysNo);
        }

        /// <summary>
        /// 根据优惠券SysNo 获取对应优惠券的折扣活动信息
        /// </summary>
        /// <param name="sysNo">优惠券系统SysNo</param>
        /// <returns></returns>
        public virtual CouponsInfo GetCouponsInfoByCouponCodeSysNo(int couponCodeSysNo)
        {
            return _da.GetCouponsInfoByCouponCodeSysNo(couponCodeSysNo);
        }

        /// <summary>
        /// 创建优惠券主信息，因为优惠券是分步进行创建的
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int? CreateMaster(CouponsInfo info)
        {
            ValidateCoupons(info);
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.InUser = userfullname;

            int? sysNo = _da.CreateMaster(info);

            ExternalDomainBroker.CreateOperationLog(BizLogType.PromotionAdd.ToEnumDesc(), BizLogType.PromotionAdd, sysNo.Value, info.CompanyCode);

            return sysNo;
        }
        /// <summary>
        /// 更新主信息
        /// </summary>
        /// <param name="info"></param>
        public virtual void UpdateMaster(CouponsInfo info)
        {
            ValidateCoupons(info);

            //判断处理期间，活动状态是否已经发生了变化，如果发生了变化，则操作失败，需要用户重新刷新处理
            CouponsInfo tempEntity = LoadMaster(info.SysNo.Value);
            if (info.Status != tempEntity.Status)
            {
                //throw new BizException(string.Format("活动[{0}]编辑失败：编辑期间，状态已经发生了变化，请重新刷新处理！", info.SysNo));
                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_EditDateStatusChanged"), info.SysNo));
            }

            CouponsStatus targetStatus = info.Status.Value;
            string errorDescription = null;
            if (!CheckAndOperateStatus(PSOperationType.Edit, info.SysNo, info.Status.Value, out targetStatus, out errorDescription))
            {
                throw new BizException(errorDescription);
            }
            info.Status = targetStatus;
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.EditUser = userfullname;
            _da.UpdateMaster(info);

            ExternalDomainBroker.CreateOperationLog(BizLogType.PromotionEdit.ToEnumDesc(), BizLogType.PromotionEdit, info.SysNo.Value, info.CompanyCode);
        }

        /// <summary>
        /// 创建、更新检查
        /// </summary>
        /// <param name="info"></param>
        public virtual void ValidateCoupons(CouponsInfo info)
        {
            if (string.IsNullOrEmpty(info.Title.Content))
            {
                //throw new BizException("优惠券名称不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_CouponsNameRequired"));
            }

            if (info.CouponChannelType == CouponsMKTType.MKTPM)
            {
                if (info.EIMSSysNo.HasValue)
                {
                    //TODO: 中蛋特有，产品不做这个处理
                    if (!_da.IsExistEMIS(info.EIMSSysNo.Value))
                    {
                        //throw new BizException("EIMS编号不存在！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_NotExsistSysNo"));
                    }
                }
            }
            if (!info.EndTime.HasValue || !info.StartTime.HasValue)
            {
                //throw new BizException("必须设置开始和结束日期！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_StatAndEndDateRequired"));
            }

            if (info.EndTime <= DateTime.Now)
            {
                //throw new BizException("失效日期必须大于当前日期！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_NotEffectDateGreaterThanCurrent"));

            }
            if (info.StartTime > info.EndTime)
            {
                //throw new BizException("失效日期必须大于等于生效日期！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_EqualOrGreaterThanTakeEffectDate"));
            }
        }

        /// <summary>
        /// 检查当前状态的下当前操作是否符合业务逻辑。同时也通过out参数返回操作后的状态
        /// </summary>
        /// <param name="operation">操作类型</param>
        /// <param name="curStatus">当前状态</param>
        /// <param name="targetStatus">操作后的状态</param>
        /// <returns>当前状态下本操作是否正确</returns>
        public virtual bool CheckAndOperateStatus(PSOperationType operation, int? sysNo, CouponsStatus? curStatus,
            out CouponsStatus targetStatus, out string errorDescription)
        {
            targetStatus = curStatus.HasValue ? curStatus.Value : CouponsStatus.Init;
            errorDescription = null;
            bool checkPassResult = true;

            switch (operation)
            {
                case PSOperationType.Edit:
                    if (curStatus != CouponsStatus.Init && curStatus != CouponsStatus.Ready)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]编辑失败:只有初始状态和就绪状态可以进行编辑！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_OnlyInitOrReadyCanEdit"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    if (curStatus != CouponsStatus.Init)
                    {
                        targetStatus = CouponsStatus.Init;
                    }

                    break;
                case PSOperationType.SubmitAudit:
                    if (curStatus != CouponsStatus.Init)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]提交审核失败:只有初始状态可以提交审核！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_OnlyInitToExamine"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    targetStatus = CouponsStatus.WaitingAudit;
                    break;
                case PSOperationType.CancelAudit:
                    if (curStatus != CouponsStatus.WaitingAudit)
                    {
                        checkPassResult = false;
                        // errorDescription = string.Format("活动[{0}]取消审核失败:只有待审核状态可以取消审核！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_OnlyCheckPendingCancel"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    targetStatus = CouponsStatus.Init;
                    break;
                case PSOperationType.AuditApprove:
                    if (curStatus != CouponsStatus.WaitingAudit)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]审核通过失败:只有待审核状态可以审核通过！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_OnlyCheckPendingPass"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    targetStatus = CouponsStatus.Ready;
                    break;
                case PSOperationType.AuditRefuse:
                    if (curStatus != CouponsStatus.WaitingAudit)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]审核拒绝失败:只有待审核状态可以进行审核拒绝！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_OnlyCheckPendingRefuse"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    targetStatus = CouponsStatus.Init;
                    break;
                case PSOperationType.Stop:
                    if (curStatus != CouponsStatus.Run)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]提前中止失败:只有运行状态可以进行提前中止！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_OnlyRunStopBefore"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    targetStatus = CouponsStatus.Finish;
                    break;
                case PSOperationType.Void:
                    if (curStatus != CouponsStatus.Init && curStatus != CouponsStatus.WaitingAudit && curStatus != CouponsStatus.Ready)
                    {
                        checkPassResult = false;
                        //errorDescription = string.Format("活动[{0}]作废失败:只有初始、待审核、就绪状态可以进行作废！本活动当前状态:{1}！", sysNo, curStatus.Value.ToDisplayText());
                        errorDescription = string.Format(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_Check_OnlyInitCheckPendRunCanDel"), sysNo, curStatus.Value.ToDisplayText());
                        break;
                    }
                    targetStatus = CouponsStatus.Void;
                    break;
            }

            return checkPassResult;
        }

       public virtual bool CheckOperate(string inUser, out string errorDescription)
        {
            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            errorDescription = "";
            bool result = true;
            if(userfullname==inUser)
            {
                errorDescription = ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_InUserName");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 设置商品范围
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetProductCondition(CouponsInfo info)
        {
            if (info.ProductRangeType != CouponsProductRangeType.AllProducts)
            {
                if (info.ProductCondition == null)
                {
                    //throw new BizException("必须至少设置一项商品范围条件！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_MustOneMoreGoodsRangeCond"));
                }

                if (info.ProductRangeType == CouponsProductRangeType.LimitProduct)
                {
                    if (info.ProductCondition.RelProducts == null
                        || info.ProductCondition.RelProducts.ProductList == null
                        || info.ProductCondition.RelProducts.ProductList.Count == 0)
                    {
                        //throw new BizException("最少要设置一个商品！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_MustOneMoreGoods"));
                    }
                    else
                    {
                        if (info.PriceDiscountRule != null && info.PriceDiscountRule.Count > 0)
                        {
                            if (info.ProductCondition.RelProducts.ProductList.Count != 1)
                            {
                                // throw new BizException("折扣类型为直减或者最终售价时，只能设置1个商品！");
                                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OnlyNeedOneGoods"));
                            }
                        }

                        if (info.ProductCondition.RelProducts.ProductList.Count > 200)
                        {
                            // throw new BizException("最多可以设置200个商品！");
                            throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_MostGoodsQuntity"));
                        }
                        //把商品有效性放到客户端添加时判断
                        //StringBuilder sb = new StringBuilder();
                        //IIMBizInteract imInstance = ObjectFactory<IIMBizInteract>.Instance;                        
                        //foreach (RelProductAndQty prod in info.ProductCondition.RelProducts.ProductList)
                        //{
                        //SampleObject objProduct = imInstance.GetProductBasicInfo(prod.Product.SysNo.Value);
                        //if(objProduct==null)
                        //{
                        //    sb.AppendLine(string.Format("商品{0}不存在！", objProduct.ID));
                        //    continue;
                        //}
                        //if (!imInstance.IsOnSalesProduct(prod.Product.SysNo.Value))
                        //{
                        //    sb.AppendLine(string.Format("商品{0}状态必须为上架！", prod.Product.ID));
                        //    continue;
                        //}
                        //}
                    }
                }

                if (info.ProductRangeType == CouponsProductRangeType.LimitCategoryBrand)
                {
                    if ((info.ProductCondition.RelBrands == null || info.ProductCondition.RelBrands.BrandList == null || info.ProductCondition.RelBrands.BrandList.Count == 0)
                        && (info.ProductCondition.RelCategories == null || info.ProductCondition.RelCategories.CategoryList == null || info.ProductCondition.RelCategories.CategoryList.Count == 0)
                        &&(info.ProductCondition.ListRelVendor==null||info.ProductCondition.ListRelVendor.Count==0))
                    {
                        // throw new BizException("最少要设置一个商品分类条件或者品牌条件或者商家条件！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_MustOneMoreGoodsCategryCondOrBrandCond"));
                    }
                }
            }

            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.InUser = userfullname;
            _da.SetProductCondition(info);
            //添加商家限定
            if (info.ProductCondition.ListRelVendor != null && info.ProductCondition.ListRelVendor.Count > 0)
            {
                foreach (var item in info.ProductCondition.ListRelVendor)
                {
                    _da.CreateVendorSaleRules(info, item.VendorSysNo);   
                }
            }
            ExternalDomainBroker.CreateOperationLog(BizLogType.PromotionProductAdd.ToEnumDesc(), BizLogType.PromotionProductAdd, info.SysNo.Value, info.CompanyCode);
        }

        /// <summary>
        /// 设置客户范围
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetCustomerCondition(CouponsInfo info)
        {
            //当选择为购物赠送型优惠券活动时要求指定用户组或者用户,由于Job不支持用户组现在只支持指定用户
            if(info.BindCondition==CouponsBindConditionType.SO)
            {
                bool NoCustomer = false;
                bool NoCustomerRank = false;
                //当指定的用户列表为空时或者用户列表无成员时
                if (info.CustomerCondition.RelCustomers.CustomerIDList == null || info.CustomerCondition.RelCustomers.CustomerIDList.Count<=0)
                {
                    NoCustomer = true;
                }
                ////当指定的用户组列表为空时
                //if (info.CustomerCondition.RelCustomerRanks.CustomerRankList == null)
                //{
                //    NoCustomerRank = true;
                //}
                ////当用户组列表只有不限用户组时
                //if (info.CustomerCondition.RelCustomerRanks.CustomerRankList != null&&
                //    info.CustomerCondition.RelCustomerRanks.CustomerRankList.Count==1 && info.CustomerCondition.RelCustomerRanks.CustomerRankList[0].SysNo == -1)
                //{
                //    NoCustomerRank = true;
                //}
                //if (NoCustomer && NoCustomerRank)
                //{
                //    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_SoNeedCustomers"));
                //}

                if (NoCustomer)
                {
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_SoNeedCustomers"));
                }
            }
            if (info.CustomerCondition == null ||
                (
                    (info.CustomerCondition.RelAreas == null || info.CustomerCondition.RelAreas.AreaList == null || info.CustomerCondition.RelAreas.AreaList.Count == 0)
                    && (info.CustomerCondition.RelCustomerRanks == null || info.CustomerCondition.RelCustomerRanks.CustomerRankList == null || info.CustomerCondition.RelCustomerRanks.CustomerRankList.Count == 0)
                    && (info.CustomerCondition.RelCustomers == null || info.CustomerCondition.RelCustomers.CustomerIDList == null || info.CustomerCondition.RelCustomers.CustomerIDList.Count == 0)
                    )
                )
            {
                // throw new BizException("必须至少设置一项客户范围条件！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_MustOneMoreCustomerRangeCond"));
            }
            if (info.CustomerCondition.RelCustomers != null && info.CustomerCondition.RelCustomers.CustomerIDList != null && info.CustomerCondition.RelCustomers.CustomerIDList.Count > 0)
            {
                if (info.CustomerCondition.RelCustomers.CustomerIDList.Count > 200)
                {
                    //throw new BizException("最少要设置一个客户，最多可以设置200个客户！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_LeatAndMostCustomer"));
                }
            }

            if (info.CustomerCondition.RelCustomerRanks != null
                && info.CustomerCondition.RelCustomerRanks.CustomerRankList != null
                && info.CustomerCondition.RelCustomerRanks.CustomerRankList.Count > 0)
            {
                //如果存在等级不限
                if (info.CustomerCondition.RelCustomerRanks.CustomerRankList.Find(f => f.SysNo == -1) != null)
                {
                    if (info.CustomerCondition.RelCustomerRanks.CustomerRankList.Count > 1)
                    {
                        //throw new BizException("已存在\"不限用户等级\"，不能选择其它用户等级！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ExsistedUnlimitUserLevel"));
                    }
                }

            }

            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.InUser = userfullname;
            _da.SetCustomerCondition(info);
            //ExternalDomainBroker.CreateOperationLog("设置优惠券客户范围", BizLogType.PromotionLimitEdit , info.SysNo.Value, info.CompanyCode);
            ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_SetCustomerRange"), BizLogType.PromotionLimitEdit, info.SysNo.Value, info.CompanyCode);
        }
        /// <summary>
        /// 设置优惠券活动规则，包含：订单条件，使用频率条件和每单折扣上限；设置客户发放规则
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetSaleRuleEx(CouponsInfo info)
        {
            if (!info.OrderCondition.OrderMinAmount.HasValue)
            {
                //throw new BizException("必须设置订单金额下限！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_NeedSetLowerLimitAmount"));
            }

            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.InUser = userfullname;
            info.EditUser = userfullname;
            _da.SetSaleRuleEx(info);

            _da.SetCustomerNotifyRule(info);
            //ExternalDomainBroker.CreateOperationLog("设置优惠券活动规则,包含：订单条件，使用频率条件和每单折扣上限；设置客户发放规则", BizLogType.PromotionLimitEdit , info.SysNo.Value, info.CompanyCode);
            ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_SetCouponsActivityRule"), BizLogType.PromotionLimitEdit, info.SysNo.Value, info.CompanyCode);

        }
        /// <summary>
        /// 设置折扣规则
        /// </summary>
        /// <param name="info"></param>
        public virtual void SetDiscountRule(CouponsInfo info)
        {
            //金额类型折扣与单个商品折扣不可并存
            if ((info.OrderAmountDiscountRule != null
                && info.OrderAmountDiscountRule.OrderAmountDiscountRank != null
                && info.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
                &&
                (info.PriceDiscountRule != null && info.PriceDiscountRule.Count > 0))
            {
                //throw new BizException("已存在不同折扣类型的数据，请先删除其它折扣类型数据！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ExsistedDifDicountType"));
            }

            //至少需要设置一条折扣规则
            if ((info.OrderAmountDiscountRule == null
                || info.OrderAmountDiscountRule.OrderAmountDiscountRank == null
                || info.OrderAmountDiscountRule.OrderAmountDiscountRank.Count == 0)
                &&
                 (info.PriceDiscountRule == null || info.PriceDiscountRule.Count == 0))
            {
                //throw new BizException("至少需要设置一条折扣规则！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_MustOneMoreDiscountRule"));
            }

            //单个商品直减和最终售价
            if (info.PriceDiscountRule != null)
            {
                if (info.PriceDiscountRule.Count > 0)
                {
                    if (info.PriceDiscountRule.Count != 1)
                    {
                        //throw new BizException("直减和最终售价只能设置1条规则！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OnlyOneRule"));
                    }

                    if (info.ProductCondition != null && info.ProductCondition.RelProducts != null && info.ProductCondition.RelProducts.ProductList != null)
                    {
                        if (info.ProductCondition.RelProducts.ProductList.Count != 1)
                        {
                            // throw new BizException("直减和最终售价在商品范围中只能设置1个指定商品！");
                            throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OnlyOneGoods"));
                        }
                    }
                    else
                    {
                        // throw new BizException("直减和最终售价在商品范围中只能设置1个指定商品！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OnlyOneGoods"));
                    }
                }
            }
            //金额折扣
            if (info.OrderAmountDiscountRule != null
                && info.OrderAmountDiscountRule.OrderAmountDiscountRank != null
                && info.OrderAmountDiscountRule.OrderAmountDiscountRank.Count > 0)
            {
                bool existDouble = false;
                for (int i = 0; i < info.OrderAmountDiscountRule.OrderAmountDiscountRank.Count; i++)
                {
                    if (existDouble)
                    {
                        break;
                    }
                    for (int j = i + 1; j < info.OrderAmountDiscountRule.OrderAmountDiscountRank.Count; j++)
                    {
                        if (info.OrderAmountDiscountRule.OrderAmountDiscountRank[i].OrderMinAmount == info.OrderAmountDiscountRule.OrderAmountDiscountRank[j].OrderMinAmount)
                        {
                            existDouble = true;
                            break;
                        }
                    }
                }
                if (existDouble)
                {
                    // throw new BizException("相同的限定金额只能设置1条！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OnlyOneLimitAmount"));
                }

            }


            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.InUser = userfullname;
            _da.SetAmountDiscountRule(info);

            // ExternalDomainBroker.CreateOperationLog("设置优惠券折扣规则", BizLogType.PromotionLimitEdit, info.SysNo.Value, info.CompanyCode);
            ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_SetDiscountRule"), BizLogType.PromotionLimitEdit, info.SysNo.Value, info.CompanyCode);
        }

        #region 优惠券代码业务处理
        /// <summary>
        /// 新增优惠券
        /// </summary>
        /// <param name="info"></param>
        public virtual void CreateCouponCode(CouponCodeSetting info)
        {
            CouponsInfo couponsInfo = Load(info.CouponsSysNo);

            string userfullname = ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(ServiceContext.Current.UserSysNo.ToString(), true);
            info.InUser = userfullname;
            if (info.CouponCodeType == CouponCodeType.Common)
            {
                //通用型创建
                CouponCodeCheck(info, couponsInfo);

                _da.CreateCouponCode(info);
            }
            else
            {
                //投放型批量创建
                CouponCodeCheck(info, couponsInfo);

                ThrowInCodeCheck(info, couponsInfo);
                BatchCreateThrowInCode(info, couponsInfo);
            }

            ExternalDomainBroker.CreateOperationLog(BizLogType.PromotionCodeAdd.ToEnumDesc(), BizLogType.PromotionCodeAdd, couponsInfo.SysNo.Value, couponsInfo.CompanyCode);

        }



        protected virtual void CouponCodeCheck(CouponCodeSetting info, CouponsInfo couponsInfo)
        {
            if (CheckExistThisTypeCouponCode(info.CouponsSysNo, CouponCodeType.Common)
                || CheckExistThisTypeCouponCode(info.CouponsSysNo, CouponCodeType.ThrowIn))
            {
                //throw new BizException("已生成过优惠券代码，请删除原来的优惠券代码后，再重新生成！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_AlreadyEggCode"));
            }
            if (couponsInfo.BindCondition != CouponsBindConditionType.None)
            {
                //触发条件是注册、生日和支付宝金账户，不可以创建优惠券代码
                //throw new BizException("触发条件必须是\"不限\"，才可以创建优惠券代码！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_OnlyUnlimitedCreateCouponsCode"));
            }
            if (!string.IsNullOrEmpty(info.CouponCode))
            {
                if (_da.CheckExistCode(info.CouponCode))
                {
                    //throw new BizException("已存在有效的优惠券代码，请重新输入！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ExsistValidCouponsCode"));
                }
            }
        }

        protected virtual void ThrowInCodeCheck(CouponCodeSetting info, CouponsInfo couponsInfo)
        {
            //指定用户组必须为不限，才可以建立投放型优惠券
            if (!(couponsInfo.CustomerCondition.RelCustomerRanks != null
                && couponsInfo.CustomerCondition.RelCustomerRanks.CustomerRankList != null
                && couponsInfo.CustomerCondition.RelCustomerRanks.CustomerRankList.Count == 1
                && couponsInfo.CustomerCondition.RelCustomerRanks.CustomerRankList[0].SysNo == -1))
            {
                //throw new BizException("指定用户组必须为不限，才能可以建立投放型优惠券！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_UnlimitedUserCreateThrowInEgg"));
            }

        }

        protected virtual void BatchCreateThrowInCode(CouponCodeSetting info, CouponsInfo couponsInfo)
        {
            const string template = "234679ACDEFGHJKLMNPQRTUVWXYZ";
            int success = 0;
            Random random = new Random();
            random.Next();

            using (TransactionScope scope = new TransactionScope())
            {
                for (int i = 0; i < info.ThrowInCodeCount.Value; ++i)
                {
                    if (success >= info.ThrowInCodeCount.Value)
                    {
                        break;
                    }
                    info.CouponCode = GenerateRandomCode(template, random, 10);

                    if (!_da.CheckExistCode(info.CouponCode))
                    {
                        _da.CreateCouponCode(info);
                        ++success;
                    }
                }
                scope.Complete();
            }
        }


        private static string GenerateRandomCode(string template, Random random, int length)
        {
            StringBuilder builder = new StringBuilder();
            int maxRandom = template.Length - 1;
            for (int i = 0; i < length; i++)
            {
                builder.Append(template[random.Next(maxRandom)]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        /// <param name="couponCodeSysNoList"></param>
        public virtual void DelCouponCode(List<int?> couponCodeSysNoList)
        {
            _da.DelCouponCode(couponCodeSysNoList);
            //ExternalDomainBroker.CreateOperationLog("批量删除优惠券:" + couponCodeSysNoList.Join(","), BizLogType.PromotionCodeDelete, 0, "");
            ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_BatchDelCoupons") + couponCodeSysNoList.Join(","), BizLogType.PromotionCodeDelete, 0, "");
        }
        /// <summary>
        /// 删除全部优惠券
        /// </summary>
        /// <param name="couponsSysNo"></param>
        public virtual void DelAllCouponCode(int? couponsSysNo)
        {
            _da.DelAllCouponCode(couponsSysNo);
            ExternalDomainBroker.CreateOperationLog(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_DelAllCoupons"), BizLogType.PromotionCodeDelete, couponsSysNo.Value, "");

        }

        /// <summary>
        /// 是否已经存在投放型的Code
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public bool CheckExistThisTypeCouponCode(int? couponSysNo, CouponCodeType codeType)
        {
            return _da.CheckExistThisTypeCouponCode(couponSysNo, codeType);
        }
        //触发条件和指定用户组为不限，才可以建立投放型优惠券
        #endregion

        #endregion
        #endregion


        /// <summary>
        /// 根据优惠券编号检查优惠券是否有效，不管优惠券是否有效都要返回优惠券代码
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public virtual bool CheckCouponIsValidAndGetCode(int couponSysNo, out string couponCode)
        {
            return _da.CheckCouponIsValidAndGetCode(couponSysNo, out couponCode);
        }


        public virtual void ActivityStatusProcess()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取订单产生的优惠卷信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public PromotionCode_Customer_Log GetPromotionCodeLog(int soSysNo)
        {
            return _da.GetPromotionCodeLog(soSysNo);
        }

        
        #region 外部Service将访问
        /// <summary>
        /// 获取商品所在优惠券中的折扣
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<ProductPromotionDiscountInfo> GetCouponDiscountListByProductSysNo(int productSysNo)
        {
            List<ProductPromotionDiscountInfo> couponDiscountList = _da.GetCouponAmount(productSysNo);
            couponDiscountList.ForEach(c => c.PromotionType = PromotionType.Coupons);
            return couponDiscountList;
        }
        #endregion

        #region 赠送优惠券相关
        /// <summary>
        /// 查询下单时有效或者已完成的商家优惠券活动
        /// </summary>
        /// <param name="merchantSysNo"></param>
        /// <param name="soDateTime"></param>
        /// <returns></returns>
        public List<CouponsInfo> QueryCoupons(int merchantSysNo, DateTime soDateTime)
        {
            return _da.QueryCoupons(merchantSysNo, soDateTime);
        }
        public void CreateCouponCodeToCustomer(CouponCodeSetting info, int customerSysNo, int soSysNo)
        {
            const string template = "234679ACDEFGHJKLMNPQRTUVWXYZ";
            int success = 0;
            Random random = new Random();
            random.Next();
            //生成一张优惠券
            while (true)
            {
                info.CouponCode = GenerateRandomCode(template, random, 10);
                if (!_da.CheckExistCode(info.CouponCode))
                {
                    _da.CreateCouponCodeForSend(info);
                    break;
                }
            }
            //将优惠券发给用户
            _da.CreateCouponCodeCustomerLog(new CouponCodeCustomerLog()
            {
                CouponCode = info.CouponCode,
                CouponSysNo = info.CouponsSysNo.Value,
                SOSysNo = soSysNo,
                CustomerSysNo = customerSysNo,
                InUser = info.InUser,
            });
        }
        /// <summary>
        /// 检查用户这个订单是否已经赠送过优惠券
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="customerSysNo"></param>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public bool CheckExistCouponCodeCustomerLog(int couponSysNo, int customerSysNo, int soSysNo)
        {
            return _da.CheckExistCouponCodeCustomerLog(couponSysNo, customerSysNo, soSysNo);
        }
        #endregion
    }
}
