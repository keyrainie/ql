using ECommerce.DataAccess.Promotion;
using ECommerce.Entity.Common;
using ECommerce.Entity.Promotion;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.WebFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ECommerce.Service.Promotion
{
    public class CouponService
    {
        public static QueryResult QueryCouponList(CouponQueryFilter queryFilter)
        {
            return CouponDA.QueryCouponList(queryFilter);
        }

        public static Coupon Load(int? sysNo)
        {
            return CouponDA.Load(sysNo);
        }

        public static Coupon Create(Coupon info)
        {
            if (info != null)
            {
                SetCoupon(info);
                CheckCoupon(info);
                using (ECommerce.Utility.ITransaction tran = ECommerce.Utility.TransactionManager.Create())
                {
                    //活动基本信息
                    info.SysNo = CouponDA.CreateMaster(info);

                    //折扣方式
                    CouponDA.AddDiscountRule(info);

                    //使用范围
                    CouponDA.SetCouponSaleRule(info);
                    CouponDA.AddSeleRulesProductCondition(info);
                    CouponDA.AddCustomerCondition(info);
                    //获取方式
                    CouponDA.SetCouponBindRules(info);
                    CouponDA.AddBindRulesProductCondition(info);

                    //优惠券
                    if (info.SysNo.HasValue && info.BindRule != null && info.BindRule.BindCondition == CouponsBindConditionType.None)
                    {
                        if (info.GeneralCode != null && (!string.IsNullOrWhiteSpace(info.GeneralCode.Code)))
                        {
                            if (CouponDA.CheckExistCode(info.GeneralCode.Code, info.SysNo))
                            {
                                throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("优惠券代码已存在"));
                            }
                            info.GeneralCode.CouponSysNo = info.SysNo.Value;
                            CouponDA.CreateCouponCode(info.GeneralCode, info.InUser);
                        }
                        else if (!string.IsNullOrWhiteSpace(info.ThrowInCodes))
                        {
                            string couponCodeXml = GetCouponCodeXml(info.ThrowInCodes);
                            if (couponCodeXml != null)
                            {
                                info.GeneralCode.CouponSysNo = info.SysNo.Value;
                                CouponDA.BatchCreateCouponCode(couponCodeXml, info, info.InUser);
                            }
                        }

                    }
                    tran.Complete();
                }
                if (info.SysNo.HasValue)
                {
                    info = CouponDA.Load(info.SysNo);
                }
            }
            return info;
        }

        private static string GetCouponCodeXml(string ThrowInCodes)
        {
            if (string.IsNullOrWhiteSpace(ThrowInCodes))
            {
                return null;
            }
            List<string> codes = new List<string>();
            //if(!string.IsNullOrWhiteSpace(GeneralCode.Code))
            //{
            //    codes.Add(GeneralCode.Code);
            //}
            codes = ThrowInCodes.Trim('\n').Split('\n').Distinct().ToList();
            StringBuilder codeXml = new StringBuilder();
            codeXml.Append("<CouponCodes>");
            foreach (var c in codes)
            {
                codeXml.Append(string.Format("<CouponCode>{0}</CouponCode>", c));
            }
            codeXml.Append("</CouponCodes>");
            return codeXml.ToString();
        }

        public static Coupon Update(Coupon info)
        {
            if (info != null && info.SysNo.HasValue)
            {
                CouponStatus status = CouponDA.GetCouponStatus(info.SysNo.Value);
                if (status != CouponStatus.Init)
                {
                    throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("只有初始态的活动才能保存或发布"));
                }
                SetCoupon(info);
                CheckCoupon(info);
                using (ECommerce.Utility.ITransaction tran = ECommerce.Utility.TransactionManager.Create())
                {
                    //活动基本信息
                    CouponDA.UpdateMaster(info);

                    //折扣方式
                    CouponDA.DeleteDiscountRule(info.SysNo);
                    CouponDA.AddDiscountRule(info);

                    //使用范围
                    CouponDA.SetCouponSaleRule(info);
                    CouponDA.DeleteSeleRulesProductCondition(info.SysNo);
                    CouponDA.AddSeleRulesProductCondition(info);
                    CouponDA.DeleteSeleRulesCustomerCondition(info.SysNo);
                    CouponDA.AddCustomerCondition(info);
                    //获取方式
                    CouponDA.SetCouponBindRules(info);
                    CouponDA.DeleteBindRulesProductCondition(info.SysNo);
                    CouponDA.AddBindRulesProductCondition(info);

                    //优惠券
                    CouponDA.DelAllCouponCode(info.SysNo);
                    //优惠券
                    if (info.SysNo.HasValue && info.BindRule != null && info.BindRule.BindCondition == CouponsBindConditionType.None)
                    {
                        if (info.GeneralCode != null && (!string.IsNullOrWhiteSpace(info.GeneralCode.Code)))
                        {
                            if (CouponDA.CheckExistCode(info.GeneralCode.Code, info.SysNo))
                            {
                                throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("优惠券代码已存在"));
                            }
                            info.GeneralCode.CouponSysNo = info.SysNo.Value;
                            CouponDA.CreateCouponCode(info.GeneralCode, info.EditUser);
                        }
                        else if (!string.IsNullOrWhiteSpace(info.ThrowInCodes))
                        {
                            string couponCodeXml = GetCouponCodeXml(info.ThrowInCodes);
                            if (couponCodeXml != null)
                            {
                                CouponDA.BatchCreateCouponCode(couponCodeXml, info, info.InUser);
                            }
                        }

                    }

                    //if (info.CouponCodes != null && info.CouponCodes.Count > 0)
                    //{
                    //    foreach (CouponCode code in info.CouponCodes)
                    //    {
                    //        if (CouponDA.CheckExistCode(code.Code))
                    //        {
                    //            throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("优惠券代码已存在"));
                    //        }
                    //        CouponDA.CreateCouponCode(code, info.InUser);
                    //    }
                    //}
                    tran.Complete();
                }
            }
            return info;
        }

        public static void StopCoupon(int couponSysNo, int merchantSysNo, string userName)
        {
            if (couponSysNo > 0 && merchantSysNo > 0)
            {
                if (merchantSysNo != CouponDA.GetCouponMerchantSysNo(couponSysNo))
                {
                    throw new BusinessException(LanguageHelper.GetText("您没有权限操作该数据"));
                }
            }

            if (CouponDA.GetCouponStatus(couponSysNo) == CouponStatus.Run)
            {
                CouponDA.UpdateStatus(couponSysNo, CouponStatus.Stoped, userName);
            }
            else
            {
                throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("只有运行状态的活动才能终止！"));
            }
        }

        public static void VoidCoupon(int couponSysNo, int merchantSysNo, string userName)
        {
            if (couponSysNo > 0 && merchantSysNo > 0)
            {
                if (merchantSysNo != CouponDA.GetCouponMerchantSysNo(couponSysNo))
                {
                    throw new BusinessException(LanguageHelper.GetText("您没有权限操作该数据"));
                }
            }

            CouponStatus status = CouponDA.GetCouponStatus(couponSysNo);
            if (status == CouponStatus.Init
                || status == CouponStatus.Ready
                || status == CouponStatus.WaitingAudit)
            {
                CouponDA.UpdateStatus(couponSysNo, CouponStatus.Void, userName);
            }
            else
            {
                throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("只有初始态、待审核、就绪状态的活动才能作废！"));
            }
        }
        public static void UpdateStatus(int couponSysNo, CouponStatus status, string userName)
        {
            CouponDA.UpdateStatus(couponSysNo, status, userName);
        }

        public static QueryResult<CouponCode> QueryCouponCodeList(CouponCodeQueryFilter queryFilter)
        {
            return CouponDA.QueryCouponCodeList(queryFilter);
        }

        public static void CheckCoupon(Coupon info)
        {
            if (info != null)
            {
                if (info.SysNo.HasValue && info.MerchantSysNo > 0)
                {
                    if (info.MerchantSysNo != CouponDA.GetCouponMerchantSysNo(info.SysNo.Value))
                    {
                        throw new BusinessException(LanguageHelper.GetText("您没有权限操作该数据"));
                    }
                }
                if (!info.BeginDate.HasValue)
                {
                    throw new BusinessException(LanguageHelper.GetText("开始时间不能为空"));
                }

                if (!info.EndDate.HasValue)
                {
                    throw new BusinessException(LanguageHelper.GetText("结束时间不能为空"));
                }

                if (info.EndDate < DateTime.Now.Date)
                {
                    throw new BusinessException(LanguageHelper.GetText("结束时间必须大于当前时间"));
                }

                if (info.BeginDate >= info.EndDate)
                {
                    throw new BusinessException(LanguageHelper.GetText("开始时间必须小于结束时间"));
                }

                if (info.DiscountRules == null
                    || info.DiscountRules.Count < 1)
                {
                    throw new BusinessException(LanguageHelper.GetText("折扣方式不能为空"));
                }

                if (info.BindRule != null)
                {
                    if (info.BindRule != null && info.BindRule.BindCondition == CouponsBindConditionType.None)
                    {
                        if ((info.GeneralCode == null || string.IsNullOrEmpty(info.GeneralCode.Code))&&string.IsNullOrWhiteSpace(info.ThrowInCodes))
                        {
                            throw new BusinessException(LanguageHelper.GetText("触发条件不限的优惠券活动必须生成优惠券代码"));
                        }
                        if (!string.IsNullOrWhiteSpace(info.GeneralCode.Code)&&CouponDA.CheckExistCode(info.GeneralCode.Code, info.SysNo))
                        {
                            throw new ECommerce.Utility.BusinessException(LanguageHelper.GetText("优惠券代码已存在"));
                        }
                    }

                    //泰隆暂时不提供此活动
                    if (info.BindRule.BindCondition == CouponsBindConditionType.SO
                      && info.BindRule.ProductRange.ProductRangeType == ProductRangeType.Limit)
                    {
                        if (info.BindRule.ProductRange.Products == null
                             || info.BindRule.ProductRange.Products.Count < 1)
                        {
                            throw new BusinessException(LanguageHelper.GetText("优惠券获取方式限制商品列表不能为空"));
                        }
                    }
                    if (info.BindRule.BindCondition == CouponsBindConditionType.SO)
                    {
                        if (info.SaleRule.CustomerRange.Customers == null
                                 || info.SaleRule.CustomerRange.Customers.Count < 1)
                        {
                            throw new BusinessException(LanguageHelper.GetText("优惠券使用规则顾客范围列表不能为空"));
                        }
                    }

                    if (info.BindRule.ValidPeriod == CouponValidPeriodType.CustomPeriod)
                    {
                        if ((!info.BindRule.BindBeginDate.HasValue)
                            || (!info.BindRule.BindEndDate.HasValue))
                        {
                            throw new BusinessException(LanguageHelper.GetText("自定义优惠券有效期起止时间不能为空"));
                        }
                    }
                }

                if (info.SaleRule != null
                    && info.SaleRule.ProductRange.ProductRangeType == ProductRangeType.Limit)
                {
                    if (info.SaleRule.ProductRange.Products == null
                         || info.SaleRule.ProductRange.Products.Count < 1)
                    {
                        throw new BusinessException(LanguageHelper.GetText("优惠券使用范围限制商品列表不能为空"));
                    }
                }

                if (info.SaleRule != null
                    && info.SaleRule.CustomerRange.CustomerRangeType == CouponCustomerRangeType.Limit)
                {
                    if (info.SaleRule.CustomerRange.Customers == null
                         || info.SaleRule.CustomerRange.Customers.Count < 1)
                    {
                        throw new BusinessException(LanguageHelper.GetText("优惠券使用范围限制顾客列表不能为空"));
                    }
                }
            }
            else
            {
                throw new BusinessException(LanguageHelper.GetText("优惠券活动信息不能为空"));
            }
        }

        public static void SetCoupon(Coupon info)
        {
            if (info != null)
            {
                if (info.BeginDate.HasValue)
                {
                    info.BeginDate = info.BeginDate.Value.Date;
                }

                if (info.EndDate.HasValue)
                {
                    info.EndDate = info.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                }

                if (info.BindRule != null
                && info.BindRule.ProductRange != null
                && info.BindRule.ProductRange.ProductRangeType == ProductRangeType.All)
                {
                    info.BindRule.ProductRange.Products = null;
                }

                if (info.SaleRule != null)
                {
                    info.SaleRule.OrderAmountLimit = info.SaleRule.OrderAmountLimit.HasValue ? info.SaleRule.OrderAmountLimit : 0m;
                    if (info.SaleRule.ProductRange == null)
                    {
                        info.SaleRule.ProductRange = new ProductRelation();
                        info.SaleRule.ProductRange.ProductRangeType = ProductRangeType.All;
                    }

                    if (info.SaleRule.ProductRange.ProductRangeType == ProductRangeType.All)
                    {
                        info.SaleRule.ProductRange.Products = null;
                    }

                    if (info.SaleRule.CustomerRange == null)
                    {
                        info.SaleRule.CustomerRange = new CustomerRelation();
                        info.SaleRule.CustomerRange.CustomerRangeType = CouponCustomerRangeType.All;
                    }
                    if (info.SaleRule.CustomerRange.CustomerRangeType == CouponCustomerRangeType.All)
                    {
                        info.SaleRule.CustomerRange.Customers = null;
                    }
                }
                if (info.BindRule != null && info.BindRule.BindCondition == CouponsBindConditionType.SO && !info.BindRule.AmountLimit.HasValue)
                {
                    info.BindRule.AmountLimit = 0;
                }


                if (info.BindRule != null
                    && info.SaleRule != null
                    && info.BindRule.BindCondition == CouponsBindConditionType.None
                    && info.GeneralCode != null)
                {
                    info.GeneralCode.TotalCount = info.SaleRule.MaxFrequency;
                    info.GeneralCode.CustomerMaxFrequency = info.SaleRule.CustomerMaxFrequency;
                }
            }
        }

        public static string GenerateRandomCode(int length)
        {
            Thread.Sleep(10);
            const string template = "234679ACDEFGHJKLMNPQRTUVWXYZ";
            int success = 0;
            Random random = new Random();
            random.Next();
            string CouponCode = string.Empty;
            while (success == 0)
            {
                StringBuilder builder = new StringBuilder();
                int maxRandom = template.Length - 1;
                for (int i = 0; i < length; i++)
                {
                    builder.Append(template[random.Next(maxRandom)]);
                }
                CouponCode = builder.ToString();
                if (!CouponDA.CheckExistCode(CouponCode, null))
                {
                    success = 1;
                }
            }
            return CouponCode;
        }

        public static string GenerateRandomCodeByTime()
        {
            var indexs = DateTime.Now.ToString("yMMddHHmmssfff").Select(c=>(int)c).ToList();
            const string template = "ACD46EQLMN79RTUVFGHJ3KPWX2YZ";
            int success = 0;
            Random random = new Random();
            string CouponCode = string.Empty;
            while (success == 0)
            {
                StringBuilder builder = new StringBuilder();
                int maxRandom = template.Length - 1;
                foreach (var i in indexs)
                {
                    builder.Append(template[i]);
                }
                builder.Append(template[random.Next(2)]);
                CouponCode = builder.ToString();
                //if (!CouponDA.CheckExistCode(CouponCode, null))
                //{
                success = 1;
                //}
            }
            return CouponCode;
        }

        public static QueryResult QueryCouponCodeRedeemLog(CouponCodeRedeemLogFilter filter)
        {
            return CouponDA.QueryCouponCodeRedeemLog(filter);
        }

        public static QueryResult QueryCouponCodeCustomerLog(CouponCodeCustomerLogFilter filter)
        {
            return CouponDA.QueryCouponCodeCustomerLog(filter);
        }

    }
}
