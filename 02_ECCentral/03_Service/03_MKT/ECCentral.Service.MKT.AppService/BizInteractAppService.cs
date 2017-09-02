using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.Service.MKT.AppService.Promotion;
using ECCentral.Service.MKT.BizProcessor.Promotion.Processors;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(IMKTBizInteract))]
    public class BizInteractAppService : IMKTBizInteract
    {
        /// <summary>
        /// 根据订单信息计算所有促销活动的优惠信息，目前包括：Combo，Coupons
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns>订单参与的促销记录</returns>
        public List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo)
        {
            return ObjectFactory<PromotionCalculateProcessor>.Instance.CalculateSOPromotion(soInfo);
        }

        /// <summary>
        /// 根据订单信息计算所有促销活动的优惠信息，目前包括：Combo，Coupons
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <param name="isModifyCoupons">是否修改蛋卷</param>
        /// <returns>订单参与的促销记录</returns>
        public List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, bool isModifyCoupons)
        {
            return ObjectFactory<PromotionCalculateProcessor>.Instance.CalculateSOPromotion(soInfo, isModifyCoupons);
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
            ObjectFactory<CouponsProcessor>.Instance.CouponCodeApply(couponSysNo, couponCode, customerSysNo,
             shoppingCartSysNo, soSysNo, redeemAmount);
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
            ObjectFactory<CouponsProcessor>.Instance.CouponCodeCancel(couponCode, soSysNo, shoppingCartSysNo);
        }


        /// <summary>
        /// 根据优惠券编号检查优惠券是否有效，不管优惠券是否有效都要返回优惠券代码
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        public bool CheckCouponIsValid(int couponSysNo, out string couponCode)
        {
            return ObjectFactory<CouponsProcessor>.Instance.CheckCouponIsValidAndGetCode(couponSysNo, out couponCode);
        }

        /// <summary>
        /// 根据组合系统编号列表获取详细信息列表
        /// </summary>
        /// <param name="comboSysNoList"></param>
        /// <returns></returns>
        public List<ComboInfo> GetComboList(List<int> comboSysNoList)
        {
            List<ComboInfo> list = new List<ComboInfo>();
            foreach (int sysno in comboSysNoList)
            {
                ComboInfo info = ObjectFactory<ComboAppService>.Instance.Load(sysno);
                list.Add(info);
            }
            return list;
        }



        /// <summary>
        /// 判断商品在赠品活动中是否作废赠品存在，但是要排除已“作废”和“完成”两种状态的记录的赠品活动
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public bool ProductIsGift(int productSysNo)
        {
            return ObjectFactory<SaleGiftProcessor>.Instance.ProductIsGift(productSysNo);
        }


        /// <summary>
        /// 提供一个接口供商品价格管理模块来调用，传入商品ID或者sysno
        /// 然后检查商品对应捆绑规则是否有低于成本价的情况(价格和+折扣 《 成本价格和)，有的就将其变为待审核(status=1)
        /// </summary>
        /// <param name="productSysNo"></param>
        public void CheckComboPriceAndSetStatus(int productSysNo)
        {
            ObjectFactory<ComboProcessor>.Instance.CheckComboPriceAndSetStatus(productSysNo);
        }

        /// <summary>
        /// 验证商品是否可以调价(例如是赠品或团购以及限时抢购不能调价,条件：赠品-本身作为赠品；生效状态-只有运行性中的)
        /// </summary>
        /// <param name="productSysNo"></param>
        public bool CheckMarketIsActivity(int productSysNo)
        {
            return ObjectFactory<SaleGiftProcessor>.Instance.CheckMarketIsActivity(productSysNo);
        }

        /// <summary>
        /// 获取订单产生的优惠卷信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public PromotionCode_Customer_Log GetPromotionCodeLog(int soSysNo)
        {
            return ObjectFactory<CouponsProcessor>.Instance.GetPromotionCodeLog(soSysNo);
        }


        /// <summary>
        /// 获取就绪/运行的限时促销计划列表
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns>就绪/运行的限时促销计划列表</returns>
        public List<CountdownInfo> GetReadyOrRunningCountDownByProductSysNo(int productSysNo)
        {
            return ObjectFactory<CountdownProcessor>.Instance.GetCountDownByProductSysNo(productSysNo).FindAll(x => x.Status == CountdownStatus.Ready || x.Status == CountdownStatus.Running);
        }

        public List<ProductPromotionDiscountInfo> GetProductPromotionDiscountInfoList(int productSysNo)
        {
            List<ProductPromotionDiscountInfo> result = new List<ProductPromotionDiscountInfo>();

            //随心配折扣列表
            result.AddRange(ObjectFactory<OptionalAccessoriesProcessor>.Instance.GetOptionalAccessoriesDiscountListByProductSysNo(productSysNo));

            //赠品折扣列表
            result.AddRange(ObjectFactory<SaleGiftProcessor>.Instance.GetGiftDiscountListByProductSysNo(productSysNo));

            //优惠券折扣列表
            result.AddRange(ObjectFactory<CouponsProcessor>.Instance.GetCouponDiscountListByProductSysNo(productSysNo));

            //团购折扣列表
            result.AddRange(ObjectFactory<GroupBuyingProcessor>.Instance.LoadGroupBuyingPriceByProductSysNo(productSysNo, GroupBuyingStatus.Active));

            //限时抢购折扣列表
            List<CountdownInfo> listTmp = ObjectFactory<CountdownProcessor>.Instance.GetCountDownByProductSysNo(productSysNo);
            listTmp.ForEach(cd =>
            {
                if (cd.Status == CountdownStatus.Running)
                {
                    result.Add(new ProductPromotionDiscountInfo()
                    {
                        Discount = cd.SnapShotCurrentPrice ?? 0m - cd.CountDownCurrentPrice ?? 0m,
                        PromotionType = PromotionType.Countdown,
                        ReferenceSysNo = cd.SysNo.Value
                    });
                }

            });

            return result;
        }

        #region Job 相关
        #region 计算优惠券code有效期
        /// <summary>
        /// 计算优惠券code有效期
        /// </summary>
        /// <param name="master"></param>
        /// <param name="codeEntity"></param>
        public static void GetCodeExpireDate(CouponsInfo master, CouponCodeSetting codeEntity)
        {
            #region beginDate,EndDate
            switch (master.ValidPeriod.Value)
            {
                case CouponsValidPeriodType.All:
                    codeEntity.StartTime = master.StartTime;
                    codeEntity.EndTime = master.EndTime;
                    break;
                case CouponsValidPeriodType.PublishDayToOneWeek:
                    codeEntity.StartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(7);
                    break;
                case CouponsValidPeriodType.PublishDayToOneMonth:
                    codeEntity.StartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(1);
                    break;
                case CouponsValidPeriodType.PublishDayToTwoMonths:
                    codeEntity.StartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(2);
                    break;
                case CouponsValidPeriodType.PublishDayToThreeMonths:
                    codeEntity.StartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(3);
                    break;
                case CouponsValidPeriodType.PublishDayToSixMonths:
                    codeEntity.StartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(6);
                    break;
                case CouponsValidPeriodType.CustomPeriod:
                    codeEntity.StartTime = master.CustomBindBeginDate;
                    codeEntity.EndTime = master.CustomBindEndDate;
                    break;
                default:
                    codeEntity.StartTime = master.StartTime;
                    codeEntity.EndTime = master.EndTime;
                    break;
            }
            #endregion
        }
        #endregion
        /// <summary>
        /// 检查订单是否满足赠送优惠券条件，如果满足则赠送优惠券给订单用户
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns></returns>
        public bool CheckAndGivingPromotionCodeForSO(SOInfo soInfo)
        {
            if (soInfo == null || soInfo.Merchant == null || !soInfo.Merchant.SysNo.HasValue
                || soInfo.BaseInfo == null || !soInfo.BaseInfo.CreateTime.HasValue)
            {
                throw new BizException("订单信息不完整，错误的操作");
            }
            //获取订单商家在订单时间时有效的购物赠送优惠券活动
            var coupons = ObjectFactory<CouponsProcessor>.Instance.QueryCoupons(soInfo.Merchant.SysNo.Value, soInfo.BaseInfo.CreateTime.Value);
            if (coupons == null || coupons.Count == 0)
            {
                return false;
            }
            foreach (CouponsInfo couponsInfo in coupons)
            {
                //（3）	活动类型为“购物赠送型”
                if (couponsInfo.BindCondition != CouponsBindConditionType.SO)
                {
                    continue;
                }
                //（4）	订单金额大于等于“订单门槛金额”
                if (couponsInfo.BindRule.AmountLimit > soInfo.BaseInfo.SOAmount)
                {
                    continue;
                }
                //（5）	商品范围为“所有商品”或者（商品范围为“限定商品”且（“指定商品”或者“排除商品”））
                if (couponsInfo.BindRule.ProductRangeType == ProductRangeType.Limit)
                {
                    if (couponsInfo.BindRule.RelProducts != null
                        && couponsInfo.BindRule.RelProducts.ProductList != null
                        && couponsInfo.BindRule.RelProducts.ProductList.Count > 0)
                    {
                        bool? ok = null;
                        foreach (var item in soInfo.Items)
                        {
                            //规则里面是否包含订单商品
                            bool contain = couponsInfo.BindRule.RelProducts.ProductList.Find(p => p.ProductSysNo.Value.Equals(item.ProductSysNo.Value)) != null;
                            if (couponsInfo.BindRule.RelProducts.IsIncludeRelation.Value && contain)
                            {
                                //包含商品
                                ok = true;
                                break;
                            }
                            if (!couponsInfo.BindRule.RelProducts.IsIncludeRelation.Value && contain)
                            {
                                //排除商品
                                ok = false;
                                break;
                            }
                        }
                        if (ok.HasValue && !ok.Value)
                        {
                            continue;
                        }
                    }
                }
                //（6）	顾客范围为“所有顾客”或者指定“顾客”
                if (couponsInfo.CustomerCondition != null
                    && couponsInfo.CustomerCondition.RelCustomers != null
                    && couponsInfo.CustomerCondition.RelCustomers.CustomerIDList != null
                    && couponsInfo.CustomerCondition.RelCustomers.CustomerIDList.Count > 0)
                {
                    bool ok = false;
                    foreach (var c in couponsInfo.CustomerCondition.RelCustomers.CustomerIDList)
                    {
                        if (c.CustomerSysNo.Value == soInfo.BaseInfo.CustomerSysNo.Value)
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (!ok)
                    {
                        continue;
                    }
                }
                //判断用户是否已经赠送过了
                if (ObjectFactory<CouponsProcessor>.Instance.CheckExistCouponCodeCustomerLog(couponsInfo.SysNo.Value, soInfo.BaseInfo.CustomerSysNo.Value, soInfo.SysNo.Value))
                {
                    continue;
                }
                //生成一张优惠券并发给用户
                CouponCodeSetting info = new CouponCodeSetting()
                {
                    CouponsSysNo = couponsInfo.SysNo,
                    CouponCodeType = CouponCodeType.ThrowIn,
                    InUser = "订单赠送优惠券活动",
                };
                GetCodeExpireDate(couponsInfo, info);
                ObjectFactory<CouponsProcessor>.Instance.CreateCouponCodeToCustomer(info, soInfo.BaseInfo.CustomerSysNo.Value, soInfo.SysNo.Value);
            }
            return true;
        }

        /// <summary>
        /// 取得团购编号取得团购信息
        /// </summary>
        /// <param name="sysNo"> 团购编号</param>
        /// <returns></returns>
        public GroupBuyingInfo GetGroupBuyInfoBySysNo(int sysNo)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.Load(sysNo);
        }

        /// <summary>
        /// 取得没有处理的团购信息
        /// </summary>
        /// <param name="companyCode">如果为null,表示取得所有没有处理的团购信息</param>
        /// <returns></returns>
        public List<GroupBuyingInfo> GetGroupBuyInfoForNeedProcess(string companyCode)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.GetGroupBuyInfoForNeedProcess(companyCode);
        }


        /// <summary>
        /// 修改团购处理状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="settlementStatus"></param>
        public void UpdateGroupBuySettlementStatus(int sysNo, GroupBuyingSettlementStatus settlementStatus)
        {
            ObjectFactory<GroupBuyingProcessor>.Instance.UpdateGroupBuySettlementStatus(sysNo, settlementStatus);
        }
        #endregion

        /// <summary>
        /// 根据商品编号获取正在参加团购的商品编号
        /// </summary>
        /// <param name="products">待验证的商品编号</param>
        /// <returns>正在参加团购的商品编号</returns>
        public List<int> GetProductsOnGroupBuying(IEnumerable<int> products)
        {
            return ObjectFactory<GroupBuyingProcessor>.Instance.GetProductsOnGroupBuying(products);
        }

        public void GetGiftSNAndCouponSNByProductSysNo(int productsysno,out int giftsysno,out int couponsysno) 
        {
            ObjectFactory<CountdownProcessor>.Instance.GetGiftSNAndCouponSNByProductSysNo(productsysno,out giftsysno,out couponsysno);
        }
    }
}
